
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
    /// Server for the JominiEngine- receives connections, deserialises incoming messages, serialises outgoing messages
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Dictionary mapping connections to the Client who owns the connection
        /// </summary>
        private static Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        static NetServer server;
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
            Globals_Server.Clients.TryGetValue(user, out c);
            if (c == null) return false;
            return clientConnections.ContainsValue(c);
        }
        /*******End of settings************/
        public void Initialise()
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
            server.Start();
            Globals_Server.server = server;
            Globals_Server.logEvent("Server started- host: " + host_name + ", port: " + port + ", appID: " + app_identifier + ", max connections: " + max_connections);

            String dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path = Path.Combine(dir, "RepairHist_mmo", "Certificates");
            LogInManager.InitialiseCertificateAndRSA(path);
        }

        public void AddTestUsers()
        {
            Client client = new Client("helen", "Char_158");
            Client client2 = new Client("test", "Char_126");
        }

        public void Listen()
        {
            server.MessageReceivedEvent.WaitOne();
            while (server.Status == NetPeerStatus.Running) 
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
                                    Disconnect(im.SenderConnection, "Not recognised");
                                    continue;
                                }
                                Client c = clientConnections[im.SenderConnection];
                                // Decrypt message if appropriate
                                if(c.alg!= null)
                                {
                                    try
                                    {
                                        im.Decrypt(c.alg);
                                    }
                                    catch(Exception e)
                                    {
                                        Console.WriteLine("SERVER: Bad decrypt");
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
                                    Globals_Server.logError("SERVER: Failure to deserialise message from " + c.username+": "+e.Message);
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
                                    Disconnect(im.SenderConnection,reason);
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
                                        client.connection = im.SenderConnection;
                                        im.SenderConnection.Approve(msg);
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

        /// <summary>
        /// Sets up the server and commences listening
        /// </summary>
        public Server()
        {
            Initialise();
            Thread listenThread = new Thread(new ThreadStart(this.Listen));
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
                    Globals_Server.logEvent("Client " + client.username + "disconnects");
                    // Cancel awaiting tasks
                    client.cts.Cancel();
                    Globals_Game.RemoveObserver(client);
                    client.connection = null;
                    clientConnections.Remove(conn);
                    conn.Disconnect(disconnectMsg);
                    Console.WriteLine("SERVER: Disconnecting client for reason: "+disconnectMsg);
                }
            }
        }

        public void Shutdown()
        {
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
