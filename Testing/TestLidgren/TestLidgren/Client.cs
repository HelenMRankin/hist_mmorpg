using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using ProtoBuf;
using System.IO;
namespace TestLidgren
{
    class Client
    {
        private NetClient client;
        private String name;
		private NetConnection connect;
        public Client(String name)
        {
            this.name = name;
            NetPeerConfiguration config = new NetPeerConfiguration("test");
			config.ConnectionTimeout = 3000f;
		//	config.PingInterval = 100f;
		//	config.ConnectionTimeout = 500f;
            client = new NetClient(config);
        }
        public void Connect(String host, int port)
        {
            client.Start();
            NetOutgoingMessage msg = client.CreateMessage(name+" hails");
            NetConnection c = client.Connect(host,port,msg);
			connect = c;
            read();
        }
        public void Send(string text)
        {
            NetOutgoingMessage om = client.CreateMessage(name+": " + text);
            client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
            client.FlushSendQueue();
        }
        public void SendViaProto()
        {
            ProtoMessage m = new ProtoMessage("this is a test",21);
            NetOutgoingMessage msg = client.CreateMessage();
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, m,PrefixStyle.Fixed32);
            msg.Write(ms.GetBuffer());
            client.SendMessage(msg,NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
            Console.WriteLine("Sent message");
        }
        public void read()
        {
            bool running = true;
            while (running)
            {
                NetIncomingMessage im;
                while ((im = client.ReadMessage()) != null)
                {

                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                            string text = im.ReadString();
                            Console.WriteLine(text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(name + " knows server accepted connection");
                                running = false;
                                break;
                            }
                            //   string reason = im.ReadString();
                            //     Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(im.SenderConnection.RemoteHailMessage.ReadString());
                            }
                            break;
                        default: Console.WriteLine("not recognised"); break;
                    }
                    client.Recycle(im);
                }
                Thread.Sleep(1);
            }
        }

    }
}
