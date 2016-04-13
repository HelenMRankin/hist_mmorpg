﻿
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
using System.Diagnostics.Contracts;
using System.Windows.Forms;

namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
#if V_SERVER
    [ContractVerification(true)]
#endif
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
        /// <returns>True if there is a connection, false if otherwise</returns>
        public static bool ContainsConnection(string user)
        {
            Contract.Requires(user!=null);
            Client c;
            Globals_Server.Clients.TryGetValue(user, out c);
            if (c == null) return false;
            return clientConnections.ContainsValue(c);
        }
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
            Globals_Server.Clients.Add("helen", client);
            Client client2 = new Client("test", "Char_126");
            Globals_Server.Clients.Add("test", client2);
            String dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path = Path.Combine(dir, "RepairHist_mmo", "Certificates");
            LogInManager.InitialiseCertificateAndRSA(path);
            // TEST
            
            Globals_Server.logEvent("Total approximate memory: " + GC.GetTotalMemory(true));
            
        }

        [ContractVerification(true)]
        public void listen()
        {
            while (server.Status==NetPeerStatus.Running) 
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
                                if(c.alg!= null)
                                {
                                    Console.WriteLine("SERVER: decrypting client message");
                                    im.Decrypt(c.alg);
                                }
                                
                            MemoryStream ms = new MemoryStream(im.Data);
                            ProtoMessage m = null;
                            try
                            {
                                m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception at Server: " + e.ToString());
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
                                if (LogInManager.VerifyUser(c.username, login.userSalt))
                                {
                                    if (LogInManager.ProcessLogIn(login, c))
                                    {
                                        Globals_Server.logEvent(c.username + " logs in from " + im.SenderEndPoint.ToString());
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
                                        Console.WriteLine("___TEST: Is logged in");
                                        readReply(m, im.SenderConnection);
                                    ProtoClient clientDetails = new ProtoClient(c);
                                    clientDetails.ActionType = Actions.Update;
                                    SendViaProto(clientDetails, im.SenderConnection,c.alg);
                                }
                                else
                                {
                                    Console.WriteLine("___TEST: Not logged in");
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
                             string reason = im.ReadString();
                             Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null &&status == NetConnectionStatus.Connected)
                            {
                                string username = (im.SenderConnection.RemoteHailMessage.ReadString());
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine("__SERVER: Disconnection");
                                if (clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    Disconnect(im.SenderConnection);
                                }
                                else
                                {
                                    Console.WriteLine("___TEST: Key unrecognised!");
                                }
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                string senderID = im.ReadString();
                                Client client;
                                Globals_Server.Clients.TryGetValue(senderID, out client);
                                if (client != null)
                                {
                                    ProtoLogIn logIn;
                                    if (!LogInManager.AcceptConnection(client, out logIn))
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
        /// <param name="alg">Optional encryption algorithm</param>
        public static void SendViaProto(ProtoMessage m,NetConnection conn, NetEncryption alg = null)
        {
            Contract.Requires(m!=null);
            NetOutgoingMessage msg = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m, PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            string s = "unencrypted";
            if(alg!=null)
            {
                s = "encrypted";
                msg.Encrypt(alg);
            }
            var result = server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
#if DEBUG
            Console.WriteLine("Server sends " + s + " message of type: " + m.GetType() + " with action: " + m.ActionType + " and response: " + m.ResponseType+", result of send: "+result.ToString());
#endif
            server.FlushSendQueue();
        }

        /// <summary>
        /// Read a message, get the relevant reply and send to client
        /// </summary>
        /// <param name="m"></param>
        /// <param name="connection"></param>
        public void readReply(ProtoMessage m, NetConnection connection)
        {
            Contract.Requires(connection!=null);
            Client client;
            clientConnections.TryGetValue(connection, out client);
            if (client == null)
            {
                //TODO error
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

        public Server()
        {
            initialise();
            Thread listenThread = new Thread(new ThreadStart(this.listen));
            listenThread.Start();
        }

        //TODO write all client details to database, remove client from connected list and close connection
        void Disconnect(NetConnection conn)
        {
            Console.WriteLine("___TEST: in disconnect");
            if (clientConnections.ContainsKey(conn))
            {
                Console.WriteLine("___TEST: Log out process initialised!");
                // TODO process log out
                Client client = clientConnections[conn];
                Globals_Server.logEvent("Client " + client.username + "disconnects");
                Globals_Game.RemoveObserver(client);
                client.conn = null;
                clientConnections.Remove(conn);
                client.alg = null;
                conn.Disconnect("Disconnect");
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
