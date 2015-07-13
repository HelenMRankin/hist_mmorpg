using UnityEngine;
using System.Collections;
using Lidgren;
using Lidgren.Network;
using System.Net;
using System.Threading;
using ProtoBuf;
using System.IO;
using hist_mmorpg;
public class NetworkScript : MonoBehaviour {
	public static NetClient client = null;
	private NetConnection connection;
	private string user;
	private string pass;
	private IPAddress ip = NetUtility.Resolve("localhost");
	private int port = 8000;
	private static Thread mainThread;
	private string level = "default";
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
		initializeClient ();
		mainThread = Thread.CurrentThread;
	}

	void initializeClient() {
		NetPeerConfiguration config = new NetPeerConfiguration("test");
		config.ConnectionTimeout = 3000f;
		client = new NetClient(config);
	}

	public void Connect(string username, string pass)
	{
		user=username;
		this.pass=pass;
		Debug.Log("connecting...");
		client.Start();
		string host = ip.ToString ();
		// remember to encrypt the bloody thing.
		NetOutgoingMessage msg = client.CreateMessage(username+","+pass);
		NetConnection c = client.Connect(host,port,msg);

		// Start listening for responses
		Debug.Log ("Starting listener");
		Thread t_reader = new Thread(new ThreadStart(this.read));
		t_reader.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(level.Equals ("Travel")) {
			Application.LoadLevel ("Travel");
		}
	}

	public static void Send(ProtoMessage message) {
		Debug.Log("In send");
		NetOutgoingMessage msg = client.CreateMessage();
		Debug.Log ("Created outgoing message");
		MemoryStream ms = new MemoryStream();
		Debug.Log ("Created memory stream");
		Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, message,ProtoBuf.PrefixStyle.Fixed32);
		Debug.Log ("serialized");
		msg.Write(ms.GetBuffer());
		client.SendMessage(msg,NetDeliveryMethod.ReliableOrdered);
		client.FlushSendQueue();
		Debug.Log("Sent message");
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
					Debug.Log ("Got data");
					MemoryStream ms = new MemoryStream(im.Data);
					ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms,PrefixStyle.Fixed32);
					ActionController (m);
					break;
				case NetIncomingMessageType.StatusChanged:
					NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
					if (status == NetConnectionStatus.Connected)
					{
						Debug.Log ("Connected.");
						ProtoMessage login = new ProtoMessage();
						Debug.Log ("Made ProtoMessage");
						login.ActionType=Actions.LogIn;
						login.Message=user+","+pass;
						Send (login);

						break;
					}
					//   string reason = im.ReadString();
					//     Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
					if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
					{

					}
					break;
				default: Debug.Log("not recognised"); break;
				}
				client.Recycle(im);
			}
			Thread.Sleep(1);
		}
	}

	private void ActionController(ProtoMessage m) {
		switch(m.ActionType) {
		case Actions.Update:
			// Perform update
			break;
		case Actions.LogIn:
			Debug.Log ("GotLogIn");
			if(m.ResponseType==DisplayMessages.LogInSuccess) {
				// Go to travel screen
				level="Travel";
			}
			else {
				// Show Error Message
			}
			break;
		default: break;
		}
	}
}
