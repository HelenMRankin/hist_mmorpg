
using System;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using ProtoBuf;
using System.Threading;
namespace hist_mmorpg
{

    /// <summary>
    /// Server for the JominiEngine- receives connections, deserialises incoming messages, serialises outgoing messages
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Dictionary mapping connections to the Client who owns the connection
        /// </summary>
        private static Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        /// <summary>
        /// The underlying NetServer that handles connections
        /// </summary>
        static NetServer server;

        /******Server Settings  ******/
        private readonly int port = 8000;
        private readonly string host_name = "localhost";
        private readonly int max_connections = 2000;
        // Used in the NetPeerConfiguration to identify application
        private readonly string app_identifier = "test";
        /******End of Settings *******/

        /// <summary>
        /// Consistency control for sever connection lists
        /// </summary>
        private static object ConnectionLock { get; set; }

        /// <summary>
        /// Server cancellation token. All connected clients can view this token and cancel if the server shuts down
        /// </summary>
        public static CancellationTokenSource ctSource { get; set; }

        /// <summary>
        /// Check if client connections contains a connection- used in testing
        /// </summary>
        /// <param name="user">Username of client to check for</param>
        /// <returns></returns>
        public static bool ContainsConnection(string user)
        {
            lock(ConnectionLock)
            {
                Client c=Utility_Methods.GetClient(user);
                if (c == null) return false;
                return clientConnections.ContainsValue(c);
            }

        }

        /// <summary>
        /// Initialise the server (including with some test clients
        /// </summary>
        public void Initialise()
        {
            ctSource=new CancellationTokenSource();
            ConnectionLock=new object();
            LogInManager.StoreNewUser("helen", "potato");
            LogInManager.StoreNewUser("test", "tomato");
            NetPeerConfiguration config = new NetPeerConfiguration(app_identifier);

            config.LocalAddress = NetUtility.Resolve(host_name);
            config.MaximumConnections = max_connections;
            config.Port = port;
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, true);
            config.PingInterval = 10f;
            config.ConnectionTimeout = 100f;
            server = new NetServer(config);
            server.Start();
            Globals_Server.server = server;
            Globals_Server.logEvent("Server started- host: " + host_name + ", port: " + port + ", appID: " + app_identifier + ", max connections: " + max_connections);

            String dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path = Path.Combine(dir, "RepairHist_mmo", "Certificates");
            LogInManager.InitialiseCertificateAndRSA(path);
        }

        /// <summary>
        /// Add some test users. "helen" controls "Char_158"; "test" controls "Char_126"
        /// </summary>
        public void AddTestUsers()
        {
            Client client = new Client("helen", "Char_158");
            Client client2 = new Client("test", "Char_126");
        }

        /// <summary>
        /// Server listening thread- repeatedly receive and process messages until cancellation or shut down
        /// </summary>
        public void Listen()
        {

            while (server.Status == NetPeerStatus.Running && !ctSource.Token.IsCancellationRequested)
            {
                NetIncomingMessage im;
                while ((im = server.ReadMessage()) != null && !ctSource.Token.IsCancellationRequested)
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
                                    Disconnect(im.SenderConnection, "Not recognised");
                                    continue;
                                }
                                Client c;
                                lock (ConnectionLock)
                                {
                                     c = clientConnections[im.SenderConnection];
                                }
                                // Decrypt message if appropriate
                                if (c.alg != null)
                                {
                                    try
                                    {
                                        im.Decrypt(c.alg);
                                    }
                                    catch (Exception e)
                                    {;
                                        Disconnect(im.SenderConnection, e.Message);
                                        continue;
                                    }

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
                                    Globals_Server.logError("SERVER: Failure to deserialise message from " + c.username + ": " + e.Message);
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
#if DEBUG
                                Globals_Server.logEvent("Server receives from "+c.username+" request "+m.ActionType);
#endif
                                c.protobufMessageQueue.Enqueue(m);
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged: byte stat = im.ReadByte();
                            NetConnectionStatus status = NetConnectionStatus.None;
                            if (Enum.IsDefined(typeof(NetConnectionStatus), Convert.ToInt32(stat)))
                            {

                                status = (NetConnectionStatus)stat;
                            }
                            else
                            {
                                Globals_Server.logError("Failure to parse byte " + stat + " to NetConnectionStatus for endpoint " + im.ReadIPEndPoint());
                            }
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                lock (ConnectionLock)
                                {
                                    if (clientConnections.ContainsKey(im.SenderConnection))
                                    {
                                        Disconnect(im.SenderConnection);
                                    }
                                }
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                        {
                            string senderID = im.ReadString();
                            Client client = Utility_Methods.GetClient(senderID);
#if DEBUG
                            Console.WriteLine("SERVER: received request from " + senderID + " for approval");
#endif
                            if (client != null)
                            {
                                ProtoLogIn logIn;
                                if (!LogInManager.AcceptConnection(client, out logIn))
                                {
#if DEBUG
                                    Globals_Server.logEvent("SERVER: Denied connection approval for "+senderID);
#endif
                                    im.SenderConnection.Deny(
                                        "Access denied- you may already be logged in on another machine, or have entered the wrong credentials");
                                }
                                else
                                {
                                    NetOutgoingMessage msg = server.CreateMessage();
                                    MemoryStream ms = new MemoryStream();
                                    // Include X509 certificate as bytes for client to validate

                                    Serializer.SerializeWithLengthPrefix<ProtoLogIn>(ms, logIn, PrefixStyle.Fixed32);
                                    msg.Write(ms.GetBuffer());
                                    lock (ConnectionLock)
                                    {
                                        clientConnections.Add(im.SenderConnection, client);
                                        client.connection = im.SenderConnection;
                                        client.ProcessConnect();
                                        
                                    }
                                    im.SenderConnection.Approve(msg);
                                    server.FlushSendQueue();
                                    Thread clientThread = new Thread(new ThreadStart(client.ActionControllerAsync));
                                    clientThread.Start();
                                    Globals_Server.logEvent("Client "+client.username+" logs in from "+im.SenderEndPoint);
                                }
                            }
                            else
                            {
                                im.SenderConnection.Deny("unrecognised");
                            }
                        }

                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            break;
                        default:
                            Globals_Server.logError("Unrecognised message type: "+im.MessageType);
                            break;
                    }
                    server.Recycle(im);
                }
                // Wait for the next message if empty (or cancellation)
                WaitHandle.WaitAny(new WaitHandle[] {server.MessageReceivedEvent, ctSource.Token.WaitHandle});
            }
            Globals_Server.logEvent("Server listening thread ends");
        }

        /// <summary>
        /// Sends a message by serializing with ProtoBufs
        /// </summary>
        /// <param name="m">Message to be sent</param>
        /// <param name="conn">Connection to send across</param>
        /// <param name="alg">Optional encryption algorithm</param>
        public static void SendViaProto(ProtoMessage m, NetConnection conn, NetEncryption alg = null)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m, PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            if (alg != null)
            {
                msg.Encrypt(alg);
            }

            server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
#if DEBUG
            Globals_Server.logEvent("Server sends message of type " + m.GetType() + " with Action: " + m.ActionType + " and response: " + m.ResponseType);
#endif
            server.FlushSendQueue();
        }


        /// <summary>
        /// Sets up the server and commences listening
        /// </summary>
        public Server()
        {
            Initialise();
            Thread listenThread = new Thread(new ThreadStart(this.Listen));
            listenThread.Start();
        }

        /// <summary>
        /// Handles a client disconnecting
        /// </summary>
        /// <param name="conn">connection of the client</param>
        /// <param name="disconnectMsg">Reason for disconnection</param>
        public static void Disconnect(NetConnection conn, string disconnectMsg = "Disconnect")
        {
            lock(ConnectionLock)
            {
                if (conn != null && clientConnections.ContainsKey(conn))
                {

                    Client client = clientConnections[conn];
                    Globals_Server.logEvent("Client " + client.username + "disconnects");
                    client.ctSource.Cancel();
                    // Cancel awaiting tasks
                    Globals_Game.RemoveObserver(client);
                    client.connection = null;
                    client.alg = null;
                    clientConnections.Remove(conn);
                    conn.Disconnect(disconnectMsg);

                }
            }
        }

        /// <summary>
        /// Shut down the server and cancel the server's token (which should cancel all client tasks)
        /// </summary>
        public void Shutdown()
        {
            ctSource.Cancel();
            server.Shutdown("Server Shutdown");
        }


    }
}
