using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using QuickGraph;
using RiakClient;
using Lidgren.Network;
using ProtoBuf;
using System.Threading;
using System.Diagnostics;
namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
    public class Server
    {
        private Dictionary<string, byte[]> sessionSalts = new Dictionary<string, byte[]>();
        private static Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        static NetServer server;
        public bool isListening = true;
        public LogInManager logInManager = new LogInManager();
        /******Server Settings (can move to config file) ******/
        private readonly int port=8000;
        private readonly string host_name="localhost";
        private readonly int max_connections=2000;
        // Used in the NetPeerConfiguration to identify application
        private readonly string app_identifier = "test";
        /*******End of settings************/

        void initialise()
        {
            logInManager.StoreNewUser("helen", "potato");
            logInManager.StoreNewUser("test", "tomato");
            NetPeerConfiguration config = new NetPeerConfiguration(app_identifier);
            config.LocalAddress = NetUtility.Resolve(host_name);
            config.MaximumConnections = max_connections;
            config.Port = port;
            Console.WriteLine("Default ping interval: " + config.PingInterval);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, true);
            config.PingInterval = 10f;
            config.ConnectionTimeout = 100f;
            server = new NetServer(config);
            server.Start();
            Globals_Server.server = server;
            Globals_Server.logEvent("Server started- host: " + host_name + ", port: " + port + ", appID: " + app_identifier + ", max connections: " + max_connections);
            Client client = new Client("helen", "Char_158");
            Globals_Server.clients.Add("helen", client);
            Client client2 = new Client("test", "Char_126");
            Globals_Server.clients.Add("test", client2);
            Globals_Server.logEvent("Total approximate memory: " + GC.GetTotalMemory(true));
            byte[] test = new byte[] { 1, 2, 3, 4 };
            byte[] result = logInManager.ComputeHash(test, test);
            Console.WriteLine("Testing hash");
            string hashstring = "";
            foreach (byte b in result)
            {
                hashstring += b.ToString();
            }
            Console.WriteLine(hashstring);
        }


        public void listen()
        {
            while (isListening) 
            {
                NetIncomingMessage im;
                while ((im = server.ReadMessage()) != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                            Globals_Server.logError("Recieved warning message: " + im.ReadString());
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                            Console.WriteLine("recieved data message");

                            MemoryStream ms = new MemoryStream(im.Data);
                            ProtoMessage m = null;
                            try
                            {
                                m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            if (m == null)
                            {
                                string error = "Recieved null message from " + im.SenderEndPoint.ToString();
                                if (clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    error += ", recognised client " + clientConnections[im.SenderConnection];
                                }
                                else
                                {
                                    error +=", unrecognised client (possible ping)";
                                }
                                error += ". Data: " + im.ReadString();
                                Globals_Server.logError(error);
                                break;
                            }
                            
                            if (m.ActionType == Actions.LogIn)
                            {
                                Console.WriteLine("Got log in");
                                ProtoLogIn login = m as ProtoLogIn;
                                if(login==null) {
                                    // error
                                    im.SenderConnection.Disconnect("Not login");
                                    return;
                                }
                                byte[] sessionSalt;
                                if (!clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    //error
                                    im.SenderConnection.Disconnect("Not recognised");
                                    return;
                                }
                                Client c = clientConnections[im.SenderConnection];
                                if (!sessionSalts.TryGetValue(c.user, out sessionSalt))
                                {
                                    //error;
                                    im.SenderConnection.Disconnect("No salt");
                                    return;
                                }
                                if (logInManager.VerifyUser(c.user, login.userSalt, sessionSalt))
                                {
                                    Globals_Server.logEvent(c.user + " logs in from " + im.SenderEndPoint.ToString());
                                    ProtoClient clientDetails = new ProtoClient(c);
                                    clientDetails.ActionType = Actions.LogIn;
                                    clientDetails.ResponseType = DisplayMessages.LogInSuccess;
                                    SendViaProto(clientDetails, im.SenderConnection);
                                    Globals_Game.RegisterObserver(c);
                                }
                                else
                                {
                                    ProtoMessage reply = new ProtoMessage();
                                    reply.ActionType = Actions.LogIn;
                                    reply.ResponseType = DisplayMessages.LogInFail;
                                    im.SenderConnection.Disconnect("Authentication Fail");
                                    sessionSalts.Remove(c.user);
                                }

                                
                               // Test();
                            }
                            // temp for testing, should validate connection first
                            else if (clientConnections.ContainsKey(im.SenderConnection))
                            {
                                readReply(m, im.SenderConnection);
                                Client c = clientConnections[im.SenderConnection];
                                ProtoClient clientDetails = new ProtoClient(c);
                                clientDetails.ActionType = Actions.Update;
                                SendViaProto(clientDetails, im.SenderConnection);
                            }
                            
                            break;
                        case NetIncomingMessageType.StatusChanged:
                             NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                             string reason = im.ReadString();
                             Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null &&status == NetConnectionStatus.Connected)
                            {
                                string username = (im.SenderConnection.RemoteHailMessage.ReadString());
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                if (clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    // TODO process log out
                                    Client client = clientConnections[im.SenderConnection];
                                    Globals_Server.logEvent("Client " + client.user + "disconencts: "+reason);
                                    Globals_Game.RemoveObserver(client);
                                    client.conn = null;
                                    clientConnections.Remove(im.SenderConnection);
                                }
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                acceptDenyConnection(im);
                            }

                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            Console.WriteLine("Latency: " + im.ReadFloat());
                            break;
                        default: Console.WriteLine("not recognised"); break;
                    }
                    server.Recycle(im);
                }
            }
        }
        /// <summary>
        /// Sends a ProtoMessage
        /// </summary>
        public static void SendViaProto(ProtoMessage m,NetConnection conn)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m, PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            server.FlushSendQueue();
        }

        public void acceptDenyConnection(NetIncomingMessage message)
        {
            Console.WriteLine("In accept-deny");
            string senderID = message.ReadString();
            Client client;
            Globals_Server.clients.TryGetValue(senderID, out client);
            if (client != null)
            {
                byte[] sessionSalt = logInManager.GetRandomSalt(32);
                byte[] passSalt = logInManager.GetUserSalt(senderID);
                if (passSalt == null)
                {
                    Console.WriteLine("No salt");
                    message.SenderConnection.Deny(); 
                    return;
                } 
                ProtoLogIn login = new ProtoLogIn();
                Console.WriteLine("Session salt: ");
                foreach (byte b in sessionSalt)
                {
                    Console.Write(b.ToString());
                }
                login.sessionSalt = sessionSalt;
                if (!sessionSalts.ContainsKey(client.user))
                {
                    sessionSalts.Add(senderID, sessionSalt);
                }
                else
                {
                    sessionSalts[senderID] = sessionSalt;
                }
                login.userSalt = passSalt;
                Console.WriteLine("\nPass salt: ");
                foreach (byte b in passSalt)
                {
                    Console.Write(b.ToString());
                }
                login.ActionType = Actions.LogIn;
                NetOutgoingMessage msg = server.CreateMessage();
                MemoryStream ms = new MemoryStream();
                Serializer.SerializeWithLengthPrefix<ProtoLogIn>(ms, login, PrefixStyle.Fixed32);
                msg.Write(ms.GetBuffer());
                clientConnections.Add(message.SenderConnection, client);
                client.conn = message.SenderConnection;
                message.SenderConnection.Approve(msg);
                server.FlushSendQueue();
                //SendViaProto(login, message.SenderConnection);
                Console.WriteLine("accepted");
            }
            else
            {
                message.SenderConnection.Deny("unrecognised");
                Console.Write("denied");
            }
        }

        public void readReply(ProtoMessage m, NetConnection connection)
        {
            Client client;
            PlayerCharacter pc;
            clientConnections.TryGetValue(connection, out client);
            if (client == null)
            {
                //TODO error
            }
            pc = client.myPlayerCharacter;
            if (pc == null)
            {
                //TODO error
            }
            ProtoMessage reply = Game.ActionController(m, pc);
            // Set action type to ensure client knows which action invoked response
            if (reply == null)
            {
                ProtoMessage invalid = new ProtoMessage();
                invalid.ActionType = Actions.Update;
                invalid.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                reply = invalid;
            }
            else
            {
                reply.ActionType = m.ActionType;
            }
            SendViaProto(reply, connection);
            Globals_Server.logEvent("From " + clientConnections[connection] + ": request = " + m.ActionType.ToString() + ", reply = " + reply.ResponseType.ToString());
        }

        public Server()
        {
            initialise();
            Thread listenThread = new Thread(new ThreadStart(this.listen));
            listenThread.Start();
        }

        //TODO write all client details to database, remove client from connected list and close connection
        void disconnect()
        {

        }
        public void Test()
        {
            NonPlayerCharacter marry = Globals_Game.npcMasterList["Char_626"];
            NonPlayerCharacter hubby = Globals_Game.npcMasterList["Char_390"];

            hubby.ProposeMarriage(marry);

        }

    }
}
