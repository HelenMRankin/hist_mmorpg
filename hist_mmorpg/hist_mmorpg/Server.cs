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
namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
    class Server
    {
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
                            PlayerCharacter pc;
                            readReply(m, pc,im.SenderConnection);
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
        public void readReply(ProtoMessage m, PlayerCharacter pc,NetConnection connection)
        {
            ProtoMessage reply = Game.ActionController(m, pc);
            //TODO send
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
