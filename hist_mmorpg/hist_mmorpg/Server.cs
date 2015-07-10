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
using System.Diagnostics;
namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
    public class Server
    {
        private Dictionary<NetConnection, Client> clientConnections = new Dictionary<NetConnection, Client>();
        NetServer server;
        bool isListening = true;
        //TODO initialise server IP address, port etc
        void initialise()
        {
         //   Globals_Server.rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
          //  Globals_Server.rClient = (RiakClient.RiakClient)Globals_Server.rCluster.CreateClient();
            Trace.WriteLine("Initialising server");
            NetPeerConfiguration config = new NetPeerConfiguration("test");
            config.LocalAddress = NetUtility.Resolve("localhost");
            Console.WriteLine(config.MaximumConnections);
            config.MaximumConnections = 2000;
            config.Port = 8000;
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            server = new NetServer(config);
            server.Start();
            Trace.WriteLine("Server has started.");
            Client client = new Client("helen", "Char_158");
            Globals_Server.clients.Add("helen", client);
        }
        //TODO listen to incoming connections
        public void listen()
        {
            Trace.WriteLine("Listening");
            while (isListening) 
            {
                NetIncomingMessage im;
                while ((im = server.ReadMessage()) != null)
                {
                    Trace.WriteLine("Got Message");
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                            isListening = false;
                            Trace.WriteLine("recieved data message");
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
                             NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                             string reason = im.ReadString();
                             Trace.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
                            {
                                Trace.WriteLine(im.SenderConnection.RemoteHailMessage.ReadString());
                                
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            acceptDenyConnection(im);
                            isListening = false;
                            return;
                        default: Trace.WriteLine("not recognised"); break;
                    }
                    server.Recycle(im);
                }
            }
        }

        public void acceptDenyConnection(NetIncomingMessage message)
        {
            // TODO authentication
            string senderID = message.ReadString();
            Client client;
            Globals_Server.clients.TryGetValue(senderID, out client);
            if (client != null)
            {
                clientConnections.Add(message.SenderConnection, client);
                message.SenderConnection.Approve();
                Trace.WriteLine("accepted");
            }
            else
            {
                message.SenderConnection.Deny("unrecognised");
                Trace.Write("denied");
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
