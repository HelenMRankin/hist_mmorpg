using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ProtoBuf;
using Lidgren.Network;

namespace TestLidgren
{
    class Server
    {
        NetServer server;
        bool listen = true;
        public Server()
        {
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
        public void read()
        {
            while (listen)
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
                            Console.WriteLine("recieved data message");
                            int numBytes = im.ReadInt32();
                            byte[] bytes = new byte[numBytes];
                            im.ReadBytes(bytes, 0, numBytes);
                            MemoryStream ms = new MemoryStream(bytes);
                            ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms,PrefixStyle.Fixed32);
                            Console.WriteLine("Name: "+m.getName()+", age: "+m.getAge());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte()==NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(im.SenderConnection.RemoteHailMessage.ReadString());
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            im.SenderConnection.Deny();
                            Console.WriteLine("Disapproved connection");
                            break;
                        default: Console.WriteLine("not recognised"); break;
                    }
                    server.Recycle(im);
                }
                Thread.Sleep(1);
            }
        }

        public void toggleListen()
        {
            if (listen)
            {
                listen = false;
            }
            else
            {
                listen = true;
            }
        }

    }
}
