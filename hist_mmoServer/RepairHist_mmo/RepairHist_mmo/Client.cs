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
    public class Client: IEquatable<Client>
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

        public CancellationTokenSource cts { get; set; }
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
            cts = new CancellationTokenSource();
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
                Console.WriteLine("Sending update " + type.ToString() + " to " + this.username);
                Server.SendViaProto(m, connection,alg);
            }
        }

        public void Update(ProtoMessage message)
        {
            message.ActionType = Actions.Update;
            if (connection != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + message.ResponseType.ToString());
                Console.WriteLine("Sending update " + message.ResponseType.ToString() + " to " + this.username);
                Server.SendViaProto(message, connection, alg);
            }
        }

        /// <summary>
        /// Gets the next message from the server by repeatedly polling the message queue
        /// </summary>
        /// <returns>Message from server</returns>
        private ProtoMessage CheckForMessage()
        {
            ProtoMessage m = null;
            while (!protobufMessageQueue.TryDequeue(out m))
            {
                protobufMessageQueue.eventWaiter.WaitOne();
            }
            return m;
        }

        /// <summary>
        /// Gets the next message recieved from the server
        /// </summary>
        /// <returns>Task containing the reply as a result</returns>
        public async Task<ProtoMessage> GetMessage()
        {
            Console.WriteLine("SERVER: Getting next message...");
            CancellationToken ct = cts.Token;
            ProtoMessage reply = await (Task.Run(() => CheckForMessage(),ct));


            return reply;
        }


        public void ActionControllerAsync()
        {
            Task<ProtoMessage> GetMessageTask = GetMessage();
            if (!GetMessageTask.Wait(3000))
            {
                // Taken too long after accepting connection request to receive login
                Server.Disconnect(connection, "Failed to login due to timeout");
            }
            ProtoLogIn LogIn = GetMessageTask.Result as ProtoLogIn;
            if (LogIn == null||LogIn.ActionType!=Actions.LogIn)
            {
                Console.WriteLine("SERVER: was expecting log in, disconnecting");
                // Error- expecting LogIn. Disconnect and send message to client
                Server.Disconnect(connection, "Invalid message sequence-expecting login");
                return;
            }
            else
            {
                Console.WriteLine("SERVER: Processing log in");
                // Process LogIn
                if (!LogInManager.ProcessLogIn(LogIn, this))
                {
                    Console.WriteLine("SERVER: Log in fails");
                    // Error
                    Server.Disconnect(connection, "Log in failed");
                    return;
                }
            }
            // While client is connected
            while (!cts.IsCancellationRequested&&connection.Status == Lidgren.Network.NetConnectionStatus.Connected)
            {
                if (myPlayerCharacter == null || !myPlayerCharacter.isAlive)
                {
                    Console.WriteLine("SERVER: client has no valid playercharacter");
                    ProtoMessage error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.Error;
                    Server.SendViaProto(error, connection, alg);
                    Server.Disconnect(connection,"You have no head of family to play as");
                    return;
                }
                GetMessageTask = GetMessage();
                // TimeOut after 15 mins of inactivity
                if (!GetMessageTask.Wait(15 * 60 * 1000) || GetMessageTask.IsCanceled|| GetMessageTask.Result==null)
                {
                    Console.WriteLine("SERVER: client times out");
                    // Session TimeOut or Cancellation
                    // Disconnect
                    Server.Disconnect(connection, "You have timed out due to inactivity");
                    return;
                }
                else
                {
                    // Need to change
                    ProtoMessage clientRequest = GetMessageTask.Result;
                    Console.WriteLine("SERVER: Processing client request for action: " + clientRequest.ActionType);
                    ProtoMessage reply = Game.ActionController(clientRequest, this);
                    if(!cts.IsCancellationRequested)
                    {
                        if (reply == null)
                        {
                            Console.WriteLine("SERVER: Invalid message sequence");
                            ProtoMessage invalid = new ProtoMessage();
                            invalid.ActionType = clientRequest.ActionType;
                            invalid.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            Server.SendViaProto(invalid, connection, alg);
                        }
                        else
                        {
                            reply.ActionType = clientRequest.ActionType;
                            Server.SendViaProto(reply, connection, alg);
                        }
                    }
                }
            }
        }

        public bool Equals(Client other)
        {
            return this.username.Equals(other.username);
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
