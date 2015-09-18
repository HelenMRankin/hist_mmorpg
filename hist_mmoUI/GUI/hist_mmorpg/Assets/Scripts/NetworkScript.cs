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
using System.Security.Cryptography;
using System.Text;
public class NetworkScript : MonoBehaviour {
	public static NetworkScript networkScript;
	public static NetClient client = null;
	private NetConnection connection;
	private string user;
	private string pass;
	private IPAddress ip = NetUtility.Resolve("localhost");
	private int port = 8000;
	private GameStateManager gameState;

	RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
	HashAlgorithm hash = new SHA256Managed();
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
		if(client.ConnectionStatus==NetConnectionStatus.Connected) {
			client.Disconnect ("Application quits");
		}
		client.Shutdown("Exit");
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

	/// <summary>
	/// Computes the hash of a salt appended to source byte array
	/// </summary>
	/// <param name="toHash">bytes to be hashed</param>
	/// <param name="salt">salt</param>
	/// <returns>computed hash</returns>
	public byte[] ComputeHash(byte[] toHash,byte[]salt)
	{
		byte[] fullHash = new byte[toHash.Length+salt.Length];
		toHash.CopyTo(fullHash,0);
		salt.CopyTo(fullHash,toHash.Length);
		byte[] hashcode = hash.ComputeHash(fullHash);
		return hashcode;
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

	public void ComputeAndSendHash(ProtoLogIn salts) {

		string hashstring="";
		foreach (byte b in salts.userSalt)
		{
			hashstring+=b.ToString ();
		}
		Debug.Log ("usersalt "+hashstring);
		string sessSalt="";
		foreach (byte b in salts.sessionSalt)
		{
			sessSalt+=b.ToString ();
		}
		Debug.Log ("session salt: "+sessSalt.ToString ());
		Debug.Log ("Password: "+pass);
		byte[] passBytes = Encoding.UTF8.GetBytes(pass);
		byte[] hashPassword = ComputeHash(passBytes, salts.userSalt);
		string passHash="";
		foreach (byte b in hashPassword)
		{
			passHash+=b.ToString ();
		}
		Debug.Log ("Password hash: "+passHash);
		byte[] hashFull = ComputeHash (hashPassword,salts.sessionSalt);
		string fullHash ="";
		foreach (byte b in hashFull)
		{
			fullHash+=b.ToString ();
		}
		Debug.Log ("Full computed hash: "+fullHash);
		ProtoLogIn response = new ProtoLogIn();
		response.userSalt=hashFull;
		response.ActionType=Actions.LogIn;
		NetworkScript.Send (response);
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
							if(m.ActionType==Actions.LogIn&&m.ResponseType==DisplayMessages.None) {
								ComputeAndSendHash (m as ProtoLogIn);
								Debug.Log ("Sent hash");
							}
							else {
								GameStateManager.ExecutionQueue.Enqueue (()=>gameState.ActionController (m));
							}

						}
					}
					catch(Exception e) {
						Debug.LogError ("Error when deserializing message: "+e.Message);
					}
					break;
				case NetIncomingMessageType.StatusChanged:

					NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
					Debug.Log ("Status change: "+status.ToString ());
					//MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
					if (status == NetConnectionStatus.Connected)
					{
						Debug.Log ("Connected.");
						if(im.SenderConnection.RemoteHailMessage!=null) {
						try {
							MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
							ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms2,PrefixStyle.Fixed32);
							if(m!=null) {
								if(m.ActionType==Actions.LogIn&&m.ResponseType==DisplayMessages.None) {
									ComputeAndSendHash (m as ProtoLogIn);
									Debug.Log ("Sent hash2");
								}
								
							}
						}
						catch(Exception e) {
							Debug.LogError ("Error when deserializing message: "+e.Message);
						}
						}
						break;
					}
					else if(status==NetConnectionStatus.Disconnected) {
						Debug.Log ("Disconnected.");
						string reason = im.ReadString();
						if(!string.IsNullOrEmpty (reason)) {
							GameStateManager.ExecutionQueue.Enqueue (()=>GameStateManager.gameState.DisplayMessage(reason))	;
						}
					}
					//   string reason = im.ReadString();
					//     Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
					if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
					{
						Debug.Log ("Got hail");
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
