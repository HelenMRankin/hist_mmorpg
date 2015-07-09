using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using RiakClient;
using Lidgren.Network;
using ProtoBuf;
using System.Threading;
namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
    class Server
    {
        private Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        static Game game;
        NetServer server;
        bool isListening = true;
        //TODO initialise server IP address, port etc
        void initialise()
        {
            Globals_Server.rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
            Globals_Server.rClient = (RiakClient.RiakClient)Globals_Server.rCluster.CreateClient();
            NetPeerConfiguration config = new NetPeerConfiguration("test");
            config.LocalAddress = NetUtility.Resolve("localhost");
            Console.WriteLine(config.MaximumConnections);
            config.MaximumConnections = 2000;
            config.Port = 8000;
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            server = new NetServer(config);
            server.Start();
            Console.WriteLine("Server has started.");
        }
        //TODO listen to incoming connections
        void listen()
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
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                            System.Diagnostics.Trace.WriteLine("recieved data message");
                            int numBytes = im.ReadInt32();
                            byte[] bytes = new byte[numBytes];
                            im.ReadBytes(bytes, 0, numBytes);
                            MemoryStream ms = new MemoryStream(bytes);
                            ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                            // Magically, a player character appears (login, to do)
                            if (clientConnections.ContainsKey(im.SenderConnection))
                            {
                                readReply(m,im.SenderConnection);
                            }
                            
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            //   string reason = im.ReadString();
                            //     Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(im.SenderConnection.RemoteHailMessage.ReadString());
                            }
                            break;
                        default: Console.WriteLine("not recognised"); break;
                    }
                    server.Recycle(im);
                }
                Thread.Sleep(1);
            }
        }

        public bool acceptConnection(NetIncomingMessage message)
        {
            // TODO authentication
            string senderID = message.ReadString();
            return true;
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
            if (reply == null)
            {
                //TEMP- refactor to ensure always reply
                return;
            }
            NetOutgoingMessage message = server.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, reply,PrefixStyle.Fixed32);
            message.Write(ms.GetBuffer().Length);
            message.Write(ms.GetBuffer());
            server.SendMessage(message,connection,NetDeliveryMethod.ReliableOrdered);
            server.FlushSendQueue();
        }

        public Server()
        {
            initialise();
        }

        //TODO write all client details to database, remove client from connected list and close connection
        void disconnect()
        {

        }


    }
}
