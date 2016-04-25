
using System;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using ProtoBuf;
using System.Threading;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace hist_mmorpg
{
    /// <summary>
    /// The Server- accepts connections, keeps track of connected clients, deserialises incoming messages and sends message to clients
    /// </summary>
#if V_SERVER
    [ContractVerification(true)]
#endif
    public class Server
    {
        /// <summary>
        /// Contains the connection and Client object of all connected, but not necessarily logged in, clients
        /// </summary>
        private static Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();

        private static NetServer server;
        /******Server Settings  ******/
        private readonly int port = 8000;
        private readonly string host_name = "localhost";
        private readonly int max_connections = 2000;
        // Used in the NetPeerConfiguration to identify application
        private readonly string app_identifier = "test";
        /******End Settings  ******/

        /// <summary>
        /// Cancellation token- used to abort listening thread
        /// </summary>
        private CancellationTokenSource ctSource;

        /// <summary>
        /// Lock used to ensure list of connected clients is consistent
        /// </summary>
        private readonly object ServerLock = new object();

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(server!=null);
            Contract.Invariant(ctSource!=null);
            Contract.Invariant(ServerLock!=null);
        }

        /// <summary>
        /// Check if client connections contains a username- used in testing
        /// </summary>
        /// <param name="user">username of client</param>
        /// <returns>True if there is a connection, false if otherwise</returns>
        public static bool ContainsConnection(string user)
        {
            Contract.Requires(user != null);
            Client c;
            Globals_Server.Clients.TryGetValue(user, out c);
            if (c == null) return false;
            return clientConnections.ContainsValue(c);
        }

        /// <summary>
        /// Initialise the server, and store some test users and clients.
        /// </summary>
        private void initialise()
        {
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
            ctSource = new CancellationTokenSource();
            server.Start();
            Globals_Server.server = server;
            Globals_Server.logEvent("Server started- host: " + host_name + ", port: " + port + ", appID: " +
                                    app_identifier + ", max connections: " + max_connections);
            Client client = new Client("helen", "Char_158");
            Globals_Server.Clients.Add("helen", client);
            Client client2 = new Client("test", "Char_126");
            Globals_Server.Clients.Add("test", client2);
            String dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path = Path.Combine(dir, "RepairHist_mmo", "Certificates");
            LogInManager.InitialiseCertificateAndRSA(path);
        }

        /// <summary>
        /// Server listening thread- accepts connections, receives messages, deserializes them and hands them to ProcessMessage
        /// </summary>
        [ContractVerification(true)]
        public void Listen()
        {
            
            while (server.Status == NetPeerStatus.Running && !ctSource.Token.IsCancellationRequested)
            {
                NetIncomingMessage im;
                WaitHandle.WaitAny(new WaitHandle[] {server.MessageReceivedEvent, ctSource.Token.WaitHandle});
                while ((im = server.ReadMessage()) != null && !ctSource.Token.IsCancellationRequested)
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
                        {
#if DEBUG
                            Console.WriteLine("SERVER: recieved data message");
#endif
                            if (!clientConnections.ContainsKey(im.SenderConnection))
                            {
                                //error
                                im.SenderConnection.Disconnect("Not recognised");
                                return;
                            }
                            Client c = clientConnections[im.SenderConnection];
                            if (c.alg != null)
                            {
                                im.Decrypt(c.alg);
                            }
                            ProtoMessage m = null;
                            using (MemoryStream ms = new MemoryStream(im.Data))
                            {
                                try
                                {
                                    m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                                }
                                catch (Exception e)
                                {
                                    NetOutgoingMessage errorMessage = server.CreateMessage(
                                        "Failed to deserialise message. The message may be incorrect, or the decryption may have failed.");
                                    if (c.alg != null)
                                    {
                                        errorMessage.Encrypt(c.alg);
                                    }
                                    server.SendMessage(errorMessage, im.SenderConnection,
                                        NetDeliveryMethod.ReliableOrdered);
                                    Globals_Server.logError("Failed to deserialize message for client: " + c.username);
                                }
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

                            if (m.ActionType == Actions.LogIn)
                            {
                                ProtoLogIn login = m as ProtoLogIn;
                                if (login == null)
                                {
                                    im.SenderConnection.Disconnect("Not login");
                                    return;
                                }
                                lock (ServerLock)
                                {
                                    if (LogInManager.VerifyUser(c.username, login.userSalt))
                                    {
                                        if (LogInManager.ProcessLogIn(login, c))
                                        {
                                            string log = c.username + " logs in from " + im.SenderEndPoint.ToString();
                                            Globals_Server.logEvent(log);
                                        }
                                    }
                                    else
                                    {
                                        ProtoMessage reply = new ProtoMessage
                                        {
                                            ActionType = Actions.LogIn,
                                            ResponseType = DisplayMessages.LogInFail
                                        };
                                        im.SenderConnection.Disconnect("Authentication Fail");
                                    }
                                }
                            }
                            // temp for testing, should validate connection first
                            else if (clientConnections.ContainsKey(im.SenderConnection))
                            {
                                if (Globals_Game.IsObserver(c))
                                {
                                    ProcessMessage(m, im.SenderConnection);
                                    ProtoClient clientDetails = new ProtoClient(c);
                                    clientDetails.ActionType = Actions.Update;
                                    SendViaProto(clientDetails, im.SenderConnection, c.alg);
                                }
                                else
                                {
                                    im.SenderConnection.Disconnect("Not logged in- Disconnecting");
                                }
                            }
                        }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            byte stat = im.ReadByte();
                            NetConnectionStatus status = NetConnectionStatus.None;
                            if (Enum.IsDefined(typeof (NetConnectionStatus), Convert.ToInt32(stat)))
                            {
                                
                                status = (NetConnectionStatus) stat;
                            }
                            else
                            {
                                Globals_Server.logError("Failure to parse byte "+stat+" to NetConnectionStatus for endpoint "+im.ReadIPEndPoint());
                            }
                            if (status == NetConnectionStatus.Disconnected)
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
                            string text = im.ReadString();
                            Client client;
                            Globals_Server.Clients.TryGetValue(senderID, out client);
                            if (client != null)
                            {
                                ProtoLogIn logIn;
                                if (!LogInManager.AcceptConnection(client, text, out logIn))
                                {
                                    im.SenderConnection.Deny();
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
                                    server.FlushSendQueue();
                                    Globals_Server.logEvent("Accepted connection from "+client.username);
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
                            Globals_Server.logError("Received unrecognised incoming message type: " + im.MessageType);
                            break;
                    }
                    server.Recycle(im);
                }
            }
#if DEBUG
            Globals_Server.logEvent("Server listening thread exits");
#endif
        }

        /// <summary>
        /// Sends a message by serializing with ProtoBufs
        /// </summary>
        /// <param name="m">Message to be sent</param>
        /// <param name="conn">Connection to send across</param>
        /// <param name="alg">Optional encryption algorithm</param>
        public static void SendViaProto(ProtoMessage m, NetConnection conn, NetEncryption alg = null)
        {
            Contract.Requires(m != null&&conn!=null);
            NetOutgoingMessage msg = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m, PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            if (alg != null)
            {
                msg.Encrypt(alg);
            }
            server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            server.FlushSendQueue();
        }

        /// <summary>
        /// Read a message, get the relevant reply and send to client
        /// </summary>
        /// <param name="m">Deserialised message from client</param>
        /// <param name="connection">Client's connecton</param>
        public void ProcessMessage(ProtoMessage m, NetConnection connection)
        {
            Contract.Requires(connection != null&&m!=null);
            Client client;
            clientConnections.TryGetValue(connection, out client);
            if (client == null)
            {
                NetOutgoingMessage errorMessage =
                    server.CreateMessage("There was a problem with the connection. Please try re-connecting");
                server.SendMessage(errorMessage, connection, NetDeliveryMethod.ReliableOrdered);
                string log = "Connection from peer " + connection.Peer.UniqueIdentifier +
                             " not found in client connections. Timestamp: " +
                             DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo);
                Globals_Server.logError(log);
                return;
            }
            var pc = client.myPlayerCharacter;
            if (pc == null || !pc.isAlive)
            {
                NetOutgoingMessage msg = server.CreateMessage("You have no valid PlayerCharacter!");
                server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();
            }
            else
            {
                ProtoMessage reply = Game.ActionController(m, client);
                // Set action type to ensure client knows which action invoked response
                if (reply == null)
                {
                    ProtoMessage invalid = new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    invalid.ActionType = Actions.Update;
                    reply = invalid;
                }
                else
                {
                    reply.ActionType = m.ActionType;
                }
                SendViaProto(reply, connection, client.alg);
                Globals_Server.logEvent("From " + clientConnections[connection] + ": request = " +
                                        m.ActionType.ToString() + ", reply = " + reply.ResponseType.ToString());
            }
        }


        /// <summary>
        /// Initialise and start the server
        /// </summary>
        public Server()
        {
            initialise();
            Thread listenThread = new Thread(new ThreadStart(this.Listen));
            listenThread.Start();
        }


        /// <summary>
        /// Processes a client disconnecting from the server- removes the client as an observer, removes their connection and deletes their CryptoServiceProvider
        /// </summary>
        /// <param name="conn">Connection of the client who disconnected</param>
        private void Disconnect(NetConnection conn)
        {Contract.Requires(conn!=null);
            lock (ServerLock)
            {
                if (clientConnections.ContainsKey(conn))
                {
                    Client client = clientConnections[conn];
                    Globals_Server.logEvent("Client " + client.username + " disconnects");
                    Globals_Game.RemoveObserver(client);
                    client.conn = null;
                    clientConnections.Remove(conn);

                    client.alg = null;
                    conn.Disconnect("Disconnect");
                }
            }
        }

        /// <summary>
        /// Shut down the server and cancels the server's token, which should stop all client tasks
        /// </summary>
        public void Shutdown()
        {
            ctSource.Cancel();
            server.Shutdown("Server Shutdown");
        }
    }
}
