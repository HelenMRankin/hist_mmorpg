using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace hist_mmorpg
{
    /// <summary>
    /// Represents a connected client
    /// </summary>
    public class Client: IEquatable<Client>, IEquatable<string>
    {
        public NetConnection connection { get; set; }
        public string username { get; set; }
        /// <summary>
        /// Holds PlayerCharacter associated with the player using this client
        /// </summary>
        public PlayerCharacter myPlayerCharacter { get; set; }
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        public Character activeChar { get; set; }
        /// <summary>
        /// Holds Fief to view in UI
        /// </summary>
        public Fief fiefToView { get; set; }
        /// <summary>
        /// Holds Province to view in UI
        /// </summary>
        public Province provinceToView { get; set; }
        /// <summary>
        /// Holds Army to view in UI
        /// </summary>
        public Army armyToView { get; set; }
        /// <summary>
        /// Holds Siege to view in UI
        /// </summary>
        public Siege siegeToView { get; set; }
        /// <summary>
        /// Holds past events
        /// </summary>
        public Journal myPastEvents = new Journal();
        /// <summary>
        /// Holds current set of events being displayed in UI
        /// </summary>
        public SortedList<uint, JournalEntry> eventSetToView = new SortedList<uint, JournalEntry>();
        /// <summary>
        /// Holds index position of currently displayed entry in eventSetToView
        /// </summary>
        public int jEntryToView { get; set; }
        /// <summary>
        /// Holds highest index position in eventSetToView
        /// </summary>
        public int jEntryMax { get; set; }
        /// <summary>
        /// Holds bool indicating whether or not to display popup messages
        /// </summary>
        public bool showMessages = true;
        /// <summary>
        /// Holds bool indicating whether or not to display popup debug messages
        /// </summary>
        public bool showDebugMessages = false;
        /// <summary>
        /// Holds the algorithm to be used during encryption and decryption. Alg is generated using the peer and a key obtained from the client 
        /// </summary>
        public NetAESEncryption alg = null;

        /// <summary>
        /// ConcurrentQueue for messages received from client with added EventWaitHandle
        /// </summary>
        public ConcurrentQueueWithEvent<ProtoMessage> protobufMessageQueue { get; set; }
        /// <summary>
        /// Client's personal CTS- cancelled on disconnect
        /// </summary>
        public CancellationTokenSource ctSource { get; set; }
        /// <summary>
        /// Linked cancellation token source- checks whether client or server CTS has been cancelled (e.g. due to shut down)
        /// </summary>
        private CancellationTokenSource _linkedTokenSource;
        public Client(String user, String pcID)
        {
            // set username associated with client
            this.username = user;


            // get playercharacter from master list of player characers
            myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            myPlayerCharacter.playerID = user;
            // set inital fief to display
            fiefToView = myPlayerCharacter.location;

            // set player's character to display
            activeChar = myPlayerCharacter;
            protobufMessageQueue=new ConcurrentQueueWithEvent<ProtoMessage>();
            ctSource = new CancellationTokenSource();
            _linkedTokenSource=CancellationTokenSource.CreateLinkedTokenSource(ctSource.Token,Server.ctSource.Token);
            Globals_Game.ownedPlayerCharacters.Add(user,myPlayerCharacter);
            Globals_Server.Clients.Add(user, this);
        }

        
        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="info"></param>
        public void Update(DisplayMessages type, string[] fields = null)
        {

            ProtoMessage m = new ProtoMessage();
            m.ActionType = Actions.Update;
            m.ResponseType = type;
            m.MessageFields = fields;
            if (connection != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + type.ToString());
                Server.SendViaProto(m, connection,alg);
            }
        }

        public void Update(ProtoMessage message)
        {
            message.ActionType = Actions.Update;
            if (connection != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + message.ResponseType.ToString());
                Server.SendViaProto(message, connection, alg);
            }
        }

        /// <summary>
        /// Gets the next message from the server by repeatedly polling the message queue
        /// </summary>
        /// <returns>Message from server</returns>
        /// <exception cref="System.OperationCanceledException">Thrown if task canceled</exception>
        private ProtoMessage CheckForMessage()
        {
            ProtoMessage m = null;
            while (!protobufMessageQueue.TryDequeue(out m)&&!_linkedTokenSource.IsCancellationRequested)
            {
                WaitHandle.WaitAny(new WaitHandle[] { protobufMessageQueue.eventWaiter,_linkedTokenSource.Token.WaitHandle});
                if (_linkedTokenSource.IsCancellationRequested)
                {
                    return null;
                }
            }
            return m;
        }

        /// <summary>
        /// Gets the next message recieved from the server
        /// </summary>
        /// <exception cref="System.OperationCanceledException">Thrown if task canceled</exception>
        /// <returns>Task containing the reply as a result</returns>
        public async Task<ProtoMessage> GetMessage()
        {
            ProtoMessage reply = null;
            reply = await (Task.Run(() => CheckForMessage(), _linkedTokenSource.Token));
            _linkedTokenSource.Token.ThrowIfCancellationRequested();
            
            return reply;
        }

        public void ProcessLogOut()
        {
            ctSource.Cancel();
            connection = null;
            alg = null;
        }

        public void ProcessConnect()
        {
            ctSource=new CancellationTokenSource();
            _linkedTokenSource=CancellationTokenSource.CreateLinkedTokenSource(ctSource.Token,Server.ctSource.Token);
            protobufMessageQueue = new ConcurrentQueueWithEvent<ProtoMessage>();
        }

        public void ActionControllerAsync()
        {
            
            try
            {
                Task<ProtoMessage> GetMessageTask = GetMessage();
                if (!GetMessageTask.Wait(3000, _linkedTokenSource.Token))
                {
                    // Taken too long after accepting connection request to receive login
                    Server.Disconnect(connection, "Failed to login due to timeout");
                    return;
                }

                ProtoLogIn LogIn = GetMessageTask.Result as ProtoLogIn;
                if (LogIn == null || LogIn.ActionType != Actions.LogIn)
                {
                    // Error- expecting LogIn. Disconnect and send message to client
                    ctSource.Cancel();
                    Server.Disconnect(connection, "Invalid message sequence-expecting login");
                    return;
                }
                else
                {
                    // Process LogIn
                    if (!LogInManager.ProcessLogIn(LogIn, this, _linkedTokenSource.Token))
                    {
                        // Error
                        Server.Disconnect(connection, "Log in failed");
                        return;
                    }
                }
                // While client is connected
                while (!_linkedTokenSource.IsCancellationRequested && connection.Status == Lidgren.Network.NetConnectionStatus.Connected)
                {
                    if (myPlayerCharacter == null || !myPlayerCharacter.isAlive)
                    {
                        ProtoMessage error = new ProtoMessage(DisplayMessages.Error);
                        Server.SendViaProto(error, connection, alg);
                        Server.Disconnect(connection, "You have no head of family to play as");
                        return;
                    }
                    GetMessageTask = GetMessage();
                    // TimeOut after 15 mins of inactivity
                    if (!GetMessageTask.Wait(15 * 60 * 1000,_linkedTokenSource.Token) || GetMessageTask.IsCanceled || GetMessageTask.Result == null)
                    {
                        // Session TimeOut or Cancellation
                        // Disconnect
                        Server.Disconnect(connection, "You have timed out");
                        return;
                    }
                    else
                    {
                        // Need to change
                        ProtoMessage clientRequest = GetMessageTask.Result;
                        _linkedTokenSource.Token.ThrowIfCancellationRequested();

                        ProtoMessage reply = Game.ActionController(clientRequest, this, _linkedTokenSource);
                        if (!_linkedTokenSource.IsCancellationRequested)
                        {
                            if (reply == null)
                            {
                                ProtoMessage invalid = new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                                invalid.ActionType = clientRequest.ActionType;
                                Server.SendViaProto(invalid, connection, alg);
                            }
                            else
                            {
                                reply.ActionType = clientRequest.ActionType;
                                Server.SendViaProto(reply, connection, alg);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Globals_Server.logEvent("Client "+username+"'s current action was cancelled");
            }
            Globals_Server.logEvent("Server's client listener for "+username+" has ended.");
            
        }

        public bool Equals(Client other)
        {
            return this.username.Equals(other.username);
        }

        public bool Equals(string uname)
        {
            return this.username.Equals(uname);
        }
    }

    public class Client_Serialized
    {
        public string user { get; set; }
        public string pcID { get; set; }
        public string activeChar { get; set; }
        public Journal myPastEvents { get; set; }

        public Client_Serialized(Client c)
        {
            this.user = c.username;
            this.pcID = c.myPlayerCharacter.charID;
            this.myPastEvents = c.myPastEvents;
            this.activeChar = c.activeChar.charID;
        }

        public Client deserialise()
        {
            Client c = new Client(user, pcID);
            c.myPastEvents = this.myPastEvents;
            DisplayMessages charErr;
            c.myPlayerCharacter = Utility_Methods.GetCharacter(pcID,out charErr) as PlayerCharacter;
            c.activeChar = Utility_Methods.GetCharacter(activeChar,out charErr);
            return c;
        }
    }
}
