using UnityEngine;
using System.Collections;
using Lidgren;
using Lidgren.Network;
using System.Net;
using System.Threading;
using System;
using ProtoBuf;
using System.IO;
using hist_mmorpg;
public class NetworkScript : MonoBehaviour {
	public static NetworkScript networkScript;
	public static NetClient client = null;
	private NetConnection connection;
	private string user;
	private string pass;
	private IPAddress ip = NetUtility.Resolve("localhost");
	private int port = 8000;
	private GameStateManager gameState;
	/// <summary>
	/// Awake this instance. Ensure singleton
	/// </summary>
	void Awake() {
		// If not already created, initialize
		if(!networkScript) {
			networkScript=this;
			DontDestroyOnLoad(this.gameObject);
		}
		// If already exists, self desctruct
		else {
			Destroy (this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
		initializeClient ();
		gameState=GameStateManager.gameState;
	}

	void OnApplicationQuit() {
		client.Disconnect ("Application quits");
	}

	void initializeClient() {
		NetPeerConfiguration config = new NetPeerConfiguration("test");
		config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, true);
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
		// remember to encrypt the bloody thing in the final
		NetOutgoingMessage msg = client.CreateMessage(username);
		NetConnection c = client.Connect(host,port,msg);

		// Start listening for responses
		Thread t_reader = new Thread(new ThreadStart(this.read));
		t_reader.Start();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public static void Send(ProtoMessage message) {
		Debug.Log ("In send");
		NetOutgoingMessage msg = client.CreateMessage();
		MemoryStream ms = new MemoryStream();
		try {
			Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, message,ProtoBuf.PrefixStyle.Fixed32);
			Debug.Log ("Serialized");
			msg.Write(ms.GetBuffer());
			Debug.Log ("Write to buffer");
			client.SendMessage(msg,NetDeliveryMethod.ReliableOrdered);
			client.FlushSendQueue();
			Debug.Log("Sent message: "+message.ActionType.ToString ());
		}
		catch(Exception e) {
			Debug.LogError (e.Message);
		}

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
					try {
						ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms,PrefixStyle.Fixed32);
						if(m!=null) {
							GameStateManager.ExecutionQueue.Enqueue (()=>gameState.ActionController (m));
						}
					}
					catch(Exception e) {
						Debug.LogError ("Error when deserializing message: "+e.Message);
					}
					break;
				case NetIncomingMessageType.StatusChanged:
					NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
					if (status == NetConnectionStatus.Connected)
					{
						Debug.Log ("Connected.");
						ProtoMessage login = new ProtoMessage();
						Debug.Log("Created Log in message");
						login.ActionType=Actions.LogIn;
						login.Message=user+","+pass;
						Send (login);
						Debug.Log("Sent log in");

						break;
					}
					else if(status==NetConnectionStatus.Disconnected) {
						string reason = im.ReadString();
						if(string.IsNullOrEmpty (reason)) {
							GameStateManager.gameState.DisplayMessage(reason);
						}
					}
					//   string reason = im.ReadString();
					//     Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
					if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
					{

					}
					break;
				case NetIncomingMessageType.ConnectionLatencyUpdated:
					break;
				default: Debug.Log("not recognised"); break;
				}
				client.Recycle(im);
			}
			Thread.Sleep(1);
		}
	}

}
