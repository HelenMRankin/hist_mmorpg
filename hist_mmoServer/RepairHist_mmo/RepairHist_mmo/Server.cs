
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
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace hist_mmorpg
{

    /// <summary>
    /// Initialises all server details
    /// </summary>
    public class Server
    {
        private static Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        static NetServer server;
        public bool isListening = true;
        /******Server Settings (can move to config file) ******/
        private readonly int port=8000;
        private readonly string host_name="localhost";
        private readonly int max_connections=2000;
        // Used in the NetPeerConfiguration to identify application
        private readonly string app_identifier = "test";

        /// <summary>
        /// Check if client connections contains a connection- used in testing
        /// </summary>
        /// <param name="conn">Connection of client</param>
        /// <returns></returns>
        public static bool ContainsConnection(string user)
        {
            Client c;
            Globals_Server.clients.TryGetValue(user, out c);
            if (c == null) return false;
            return clientConnections.ContainsValue(c);
        }
        /*******End of settings************/
        void initialise()
        {
            LogInManager.StoreNewUser("helen", "potato");
            LogInManager.StoreNewUser("test", "tomato");
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
            String dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path = Path.Combine(dir, "RepairHist_mmo", "Certificates");
            LogInManager.InitialiseCertificateAndRSA(path);
            // TEST

            Globals_Server.logEvent("Total approximate memory: " + GC.GetTotalMemory(true));
            byte[] test = new byte[] { 1, 2, 3, 4 };
            byte[] result = LogInManager.ComputeHash(test, test);
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
                            Globals_Server.logError("SERVER: Recieved warning message: " + im.ReadString());
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                        {
                                // Retrieve client data
                                if (!clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    //error
                                    im.SenderConnection.Disconnect("Not recognised");
                                    return;
                                }
                                Client c = clientConnections[im.SenderConnection];
                                // Decrypt message if appropriate
                                if(c.alg!= null)
                                {
                                    im.Decrypt(c.alg);
                                }
                            // Attempt to deserialise message    
                            MemoryStream ms = new MemoryStream(im.Data);
                            ProtoMessage m = null;
                            try
                            {
                                m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                            }
                            catch (Exception e)
                            {
                                    Globals_Server.logError("SERVER: Failure to deserialise message from " + c.user+": "+e.Message);
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
                                    error += ", unrecognised client (possible ping)";
                                }
                                error += ". Data: " + im.ReadString();
                                Globals_Server.logError(error);
                                break;
                            }
                            Console.WriteLine("SERVER: Received message with action: "+m.ActionType);
                            // Add message to client's message queue and set event waiter to wake up thread
                                c.MessageQueue.Add(m);
                                c.eventWaiter.Set();
                                /*
                            if (m.ActionType == Actions.LogIn)
                            {
#if TESTSUITE
                                Stopwatch timer = new Stopwatch();
                                timer.Start();
#endif         
                                Console.WriteLine("Got log in");
                                ProtoLogIn login = m as ProtoLogIn;
                                if (login == null)
                                {
                                    // error
                                    im.SenderConnection.Disconnect("Not login");
                                    return;
                                }
                                if (logInManager.VerifyUser(c.user, login.userSalt))
                                {

                                    Globals_Server.logEvent(c.user + " logs in from " + im.SenderEndPoint.ToString());
                                        // Decrypt key


                                    try
                                        {
                                            Console.WriteLine("SERVER: decrypting client key");
                                            byte[] key = rsa.Decrypt(login.Key, false);
                                            Console.WriteLine("SERVER: Key for client1: ");
                                            foreach (var bite in key)
                                            {
                                                Console.Write(bite.ToString());
                                            }
                                            Console.WriteLine("\n");
                                            c.alg = new NetAESEncryption(server, key, 0, key.Length);
                                            ProtoClient clientDetails = new ProtoClient(c);
                                            clientDetails.ActionType = Actions.LogIn;
                                            clientDetails.ResponseType = DisplayMessages.LogInSuccess;
                                            SendViaProto(clientDetails, im.SenderConnection, c.alg);
                                            Globals_Game.RegisterObserver(c);
                                        }
                                        catch(Exception e)
                                        {
                                            Console.WriteLine("Failure during decryption: " + e.GetType() + " " + e.Message + ";" + e.StackTrace);
                                            Application.Exit();
                                        }
                                    
                                }
                                else
                                {
                                    Console.WriteLine("SERVER: Verification Failure");
                                    ProtoMessage reply = new ProtoMessage();
                                    reply.ActionType = Actions.LogIn;
                                    reply.ResponseType = DisplayMessages.LogInFail;
                                    im.SenderConnection.Disconnect("Authentication Fail");
                                }

#if TESTSUITE
                                timer.Stop();
                                long timeLogin = timer.ElapsedMilliseconds;
                                TestSuite.LogData("LogIn time",timeLogin.ToString());
#endif
                            }
                            // temp for testing, should validate connection first
                            else if (clientConnections.ContainsKey(im.SenderConnection))
                            {
                                if (Globals_Game.IsObserver(c))
                                {
                                    Console.WriteLine("SERVER: TEST reached");
                                    readReply(m, im.SenderConnection);
                                    ProtoClient clientDetails = new ProtoClient(c);
                                    clientDetails.ActionType = Actions.Update;
                                    SendViaProto(clientDetails, im.SenderConnection,c.alg);
                                }
                                else
                                {
                                    im.SenderConnection.Disconnect("Not logged in- Disconnecting");
                                }
                            }
                            */
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
                                    Disconnect(im.SenderConnection);
                                }
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                string senderID = im.ReadString();
                                Client client;
                                Globals_Server.clients.TryGetValue(senderID, out client);
                                if (client != null)
                                {
                                    ProtoLogIn logIn;
                                    if (!LogInManager.AcceptConnection(client, out logIn))
                                    {
                                        im.SenderConnection.Deny("Access denied- you may already be logged in on another machine, or have entered the wrong credentials");
                                    }
                                    else
                                    {
                                        
                                        NetOutgoingMessage msg = server.CreateMessage();
                                        MemoryStream ms = new MemoryStream();
                                        // Include X509 certificate as bytes for client to validate
                                        
                                        Serializer.SerializeWithLengthPrefix<ProtoLogIn>(ms, logIn, PrefixStyle.Fixed32);
                                        msg.Write(ms.GetBuffer());
                                        clientConnections.Add(im.SenderConnection, client);
                                        client.conn = im.SenderConnection;
                                        im.SenderConnection.Approve(msg);
                                        Console.WriteLine("SERVER: approved connection");
                                        server.FlushSendQueue();
                                        Task.Run(() => client.ActionControllerAsync());
                                    }

                                }
                                else
                                {
                                    im.SenderConnection.Deny("unrecognised");
                                }
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
        /// Sends a message by serializing with ProtoBufs
        /// </summary>
        /// <param name="m">Message to be sent</param>
        /// <param name="conn">Connection to send across</param>
        /// <param name="key">Optional encryption key</param>
        public static void SendViaProto(ProtoMessage m,NetConnection conn, NetEncryption alg = null)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m, PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            if(alg!=null)
            {
                msg.Encrypt(alg);
            }

            server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);

            Console.WriteLine("Server sends message of type " + m.GetType() + " with Action: " + m.ActionType + " and response: " + m.ResponseType);
            server.FlushSendQueue();
        }

        /*
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
            if (pc == null || !pc.isAlive)
            {
                NetOutgoingMessage msg = server.CreateMessage("You have no valid PlayerCharacter!");
                server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
            else
            {
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
                SendViaProto(reply, connection, client.alg);
                Globals_Server.logEvent("From " + clientConnections[connection] + ": request = " + m.ActionType.ToString() + ", reply = " + reply.ResponseType.ToString());
            }
            
        }
        */
        public Server()
        {
            initialise();
            Thread listenThread = new Thread(new ThreadStart(this.listen));
            listenThread.Start();
        }

        //TODO write all client details to database, remove client from connected list and close connection
        public static void Disconnect(NetConnection conn, string disconnectMsg= "Disconnect")
        {
            if(conn!= null)
            {
                if (clientConnections.ContainsKey(conn))
                {
                    // TODO process log out
                    Client client = clientConnections[conn];
                    Globals_Server.logEvent("Client " + client.user + "disconnects");
                    // Cancel awaiting tasks
                    client.cts.Cancel();
                    Globals_Game.RemoveObserver(client);
                    client.conn = null;
                    clientConnections.Remove(conn);
                    conn.Disconnect(disconnectMsg);
                }
            }
        }

        public void Shutdown()
        {
            isListening = false;
            server.Shutdown("Server Shutdown");
        }

        public void Test()
        {
            NonPlayerCharacter marry = Globals_Game.npcMasterList["Char_626"];
            NonPlayerCharacter hubby = Globals_Game.npcMasterList["Char_390"];

            hubby.ProposeMarriage(marry);

        }

    }
}
