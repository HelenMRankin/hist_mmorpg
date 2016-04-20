using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using ProtoBuf;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

namespace hist_mmorpg
{
    /// <summary>
    /// Extends the ConcurrentQueue to fire an event whenever a new item is enqueued
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentQueueWithEvent<T> : ConcurrentQueue<T>
    {
        public EventWaitHandle eventWaiter { get; set; }

        public ConcurrentQueueWithEvent() : base()
        {
            eventWaiter = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        /// <summary>
        /// Add a new item to the queue and set the EventWaitHandle
        /// </summary>
        /// <param name="t">Item to be enqueued</param>
        public new void Enqueue(T t)
        {
            base.Enqueue(t);
            eventWaiter.Set();
        }


    }
    public partial class TestClient
    {
        public ConcurrentQueueWithEvent<ProtoMessage> protobufMessageQueue;
        public ConcurrentQueueWithEvent<string> stringMessageQueue;
        public Network net;
        public string playerID;


        public TestClient()
        {
            protobufMessageQueue = new ConcurrentQueueWithEvent<ProtoMessage>();
            stringMessageQueue = new ConcurrentQueueWithEvent<string>();
        }
        /*************************************
         * General Commands ***
         * **********************************/
        /// <summary>
        /// Log in to the server
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public void LogInAndConnect(string user, string pass, byte[] key = null)
        {
            
            net = new Network(this, key);
            net.Connect(user, pass);
            this.playerID = user;
        }

        public void ConnectNoLogin(string user, string pass, byte[] key = null)
        {
            this.playerID = user;
            net = new Network(this, key);
            net.autoLogIn = false;
            net.Connect(user, pass);
        }

        public void SendDummyLogIn(string user, string pass, byte[] key = null)
        {
            ProtoLogIn logIn = new ProtoLogIn();
            logIn.ActionType = Actions.LogIn;
            logIn.Key = key;
            net.Send(logIn);
        }

        public bool IsConnectedAndLoggedIn()
        {
            if (net.GetConnectionStatusString().Equals("Connected") && net.loggedIn)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clear both message queues- used in tests where we don't care about the login messages
        /// </summary>
        public void ClearMessageQueues()
        {
            // Note- With ConcurrentQueues, it is preferrable to discard the previous queue than to attempt to Dequeue all elements in case the queue is currently being written to
            protobufMessageQueue = new ConcurrentQueueWithEvent<ProtoMessage>();

            stringMessageQueue = new ConcurrentQueueWithEvent<string>();


        }
        public void LogOut()
        {
            net.Disconnect();
        }

        public void SendMessage(Actions a, object o)
        {
            ProtoMessage msg = new ProtoMessage();
            msg.ActionType = a;
            msg.Message = o.ToString();
            net.Send(msg, true);
        }
        /// <summary>
        /// Switch to commanding a different character
        /// </summary>
        /// <param name="charID">ID of character to control. Must own the character, and character must not be captured/dead</param>
        public void SwitchCharacter(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.UseChar;
            message.Message = charID;
            net.Send(message);
        }

        /// <summary>
        /// Gets the next message from the server by repeatedly polling the message queue. 
        /// </summary>
        /// <returns>Message from server</returns>
        /// <throws>TaskCanceledException if task is cancelled</throws>
        private ProtoMessage CheckForProtobufMessage()
        {
            ProtoMessage m = null;
            var waitHandles = new WaitHandle[] { protobufMessageQueue.eventWaiter, net.ctSource.Token.WaitHandle };
            while (!protobufMessageQueue.TryDequeue(out m))
            {
                EventWaitHandle.WaitAny(waitHandles);
                net.ctSource.Token.ThrowIfCancellationRequested();
            }
            return m;
        }

        /// <summary>
        /// Gets the next message from the server by repeatedly polling the message queue
        /// </summary>
        /// <returns>Message from server</returns>
        /// <throws>TaskCanceledException if task is cancelled</throws>
        private string CheckForStringMessage()
        {
            string s = null;
            var waitHandles = new WaitHandle[] {stringMessageQueue.eventWaiter, net.ctSource.Token.WaitHandle};
            while (!stringMessageQueue.TryDequeue(out s))
            {
                EventWaitHandle.WaitAny(waitHandles);
                net.ctSource.Token.ThrowIfCancellationRequested();
            }
            return s;
        }

        /// <summary>
        /// Gets the next message recieved from the server
        /// </summary>
        /// <returns>Task containing the reply as a result</returns>
        public async Task<ProtoMessage> GetReply(string id = null)
        {

            ProtoMessage reply = await (Task.Run(() => CheckForProtobufMessage()));
#if DEBUG
            if (reply == null)
            {
                Console.WriteLine("CLIENT: " + id + "got a null reply");
            }
            else
            {
                Console.WriteLine("CLIENT: " + id + " got reply with response type :" + reply.ResponseType);
            }
#endif
            return reply;
        }

        /// <summary>
        /// Gets the next message recieved from the server
        /// </summary>
        /// <returns>Task containing the reply as a result</returns>
        public async Task<string> GetServerMessage()
        {
            Console.WriteLine("CLIENT: Awaiting reply");

            string reply = await (Task.Run(() => CheckForStringMessage()));

            return reply;
        }

        /// <summary>
        /// Request that a season update be performed. Note that this is an admin command
        /// </summary>
        public void SeasonUpdate()
        {
            ProtoMessage updateRequest = new ProtoMessage();
            updateRequest.ActionType = Actions.SeasonUpdate;
            net.Send(updateRequest);
        }

        /// <summary>
        /// View a character
        /// </summary>
        /// <param name="charID">ID of character to view</param>
        public void ViewCharacter(string charID)
        {
            ProtoMessage viewChar = new ProtoMessage();
            viewChar.ActionType = Actions.ViewChar;
            viewChar.Message = charID;
            net.Send(viewChar);
        }

        protected virtual void Dispose(bool dispose)
        {
            net.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /*************************************
         * Travel-related Commands ***
    * **********************************/


    public partial class TestClient
    {
        /// <summary>
        /// Move a character to a chosen location
        /// </summary>
        /// <param name="character">Character ID</param>
        /// <param name="location">Location ID</param>
        public void Move(string character, string location, string[] travelInstructions = null)
        {
            ProtoTravelTo message = new ProtoTravelTo();
            message.characterID = character;
            message.travelTo = location;
            message.ActionType = Actions.TravelTo;
            message.travelVia = travelInstructions;
            net.Send(message);
        }

        /// <summary>
        /// Camp for a number of days at the character's location
        /// </summary>
        /// <param name="days">Number of days to camp for</param>
        /// <param name="charID">Character to camp. If character is leading an army, army will camp too</param>
        public void Camp(int days, string charID = null)
        {
            ProtoMessage message = new ProtoMessage();
            message.MessageFields = new string[] { days.ToString() };
            // Check which character is performing action
            message.Message = charID;
            message.ActionType = Actions.Camp;
            net.Send(message);
        }

        /// <summary>
        /// Get the potential candidates for a selected type and role
        /// </summary>
        /// <param name="type">One of "Grant"</param>
        /// <param name="role">One of "leader", "bailiff"</param>
        /// <param name="itemID">ID of item to which role will be granted (e.g. armyID)</param>
        public void GetNPCList(string type, string role = null, string itemID = null)
        {
            ProtoMessage getLeaders = new ProtoMessage();
            getLeaders.ActionType = Actions.GetNPCList;
            getLeaders.Message = type;
            getLeaders.MessageFields = new string[] { role, itemID };
            net.Send(getLeaders);
        }


        /// <summary>
        /// Enter the keep if not already inside; exit the keep if already inside
        /// </summary>
        /// <param name="charID">ID of character to enter/exit keep</param>
        public void EnterExitKeep(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.Message = charID;
            message.ActionType = Actions.EnterExitKeep;
            net.Send(message);
        }

        /// <summary>
        /// List all the characters in a place based on a character's location
        /// </summary>
        /// <param name="charID">charID of character whose location to use</param>
        /// <param name="place">One of "court", "tavern", or "outside"</param>
        public void ListCharactersInPlace(string charID, string place)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.ListCharsInMeetingPlace;
            message.MessageFields = new string[] { charID };
            message.Message = place;
            net.Send(message);
        }
    }




    /*****************
    **Subterfuge actions***
    ***************/
    public partial class TestClient
    {
        /// <summary>
        /// View all captives you own
        /// </summary>
        public void ViewCaptives()
        {
            ProtoMessage getCaptives = new ProtoMessage();
            getCaptives.ActionType = Actions.ViewCaptives;
            getCaptives.Message = "all";
            net.Send(getCaptives);
        }

        /// <summary>
        /// View captives kept in fief's gaol
        /// </summary>
        /// <param name="fiefID">ID of fief to view captives in</param>
        public void ViewCaptives(string fiefID)
        {
            ProtoMessage listCaptives = new ProtoMessage();
            listCaptives.ActionType = Actions.ViewCaptives;
            listCaptives.Message = fiefID;
            net.Send(listCaptives);
        }

        /// <summary>
        /// View details on a single captive
        /// </summary>
        /// <param name="captiveID">ID of captive to view</param>
        public void GetCaptive(string captiveID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.ViewCaptive;
            message.Message = captiveID;
            net.Send(message);
        }

        /// <summary>
        /// Release a captive 
        /// </summary>
        /// <param name="charID">ID of captive to release</param>
        public void ReleaseCaptive(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.ReleaseCaptive;
            message.Message = charID;
            net.Send(message);
        }

        /// <summary>
        /// Execute a captive
        /// </summary>
        /// <param name="charID">ID of character to execute</param>
        public void ExecuteCaptive(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.ExecuteCaptive;
            message.Message = charID;
            net.Send(message);
        }
        /// <summary>
        /// Ranson a captive (amount to ransom for is calculated based on character value on server side)
        /// </summary>
        /// <param name="charID">ID of character to ransom</param>
        public void RansomCaptive(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.RansomCaptive;
            message.Message = charID;
            net.Send(message);
        }

        /// <summary>
        /// Spy on a fief
        /// </summary>
        /// <param name="charID">ID of character to be spying</param>
        /// <param name="fiefID">ID of target fief to spy on</param>
        public void SpyOnFief(string charID, string fiefID)
        {
            ProtoMessage spy = new ProtoMessage();
            spy.ActionType = Actions.SpyFief;
            spy.Message = fiefID;
            spy.MessageFields = new string[] { charID };
            net.Send(spy);
        }

        public void SpyOnCharacter(string charID, string targetID)
        {
            ProtoMessage spy = new ProtoMessage();
            spy.ActionType = Actions.SpyCharacter;
            spy.Message = charID;
            spy.MessageFields = new string[] { targetID };
            net.Send(spy);
        }

        public void SpyOnArmy(string charID, string targetID)
        {
            ProtoMessage spy = new ProtoMessage();
            spy.ActionType = Actions.SpyArmy;
            spy.Message = charID;
            spy.MessageFields = new string[] { targetID };
            net.Send(spy);
        }
    }

    /*****************************
    ****Family-management commands
    **************************/
    public partial class TestClient
    {
        /// <summary>
        /// Name a new heir
        /// </summary>
        /// <param name="charID">ID of character to be heir. Must be a male family member of correct age</param>
        public void NameHeir(string charID)
        {
            ProtoMessage message = new ProtoMessage();
            message.ActionType = Actions.AppointHeir;
            message.Message = charID;
            net.Send(message);
        }

        /// <summary>
        /// Propose marriage between two characters
        /// </summary>
        /// <param name="groomID">character ID of the groom</param>
        /// <param name="brideID">character ID of the bride</param>
        public void Marry(string groomID, string brideID)
        {
            ProtoMessage marry = new ProtoMessage();
            marry.ActionType = Actions.ProposeMarriage;
            marry.Message = groomID;
            marry.MessageFields = new string[] { brideID };
            net.Send(marry);
        }

        /// <summary>
        /// Attempt to produce an offspring
        /// </summary>
        /// <param name="charID">ID of character who will be trying for a child. Must be a male family member with a spouse</param>
        public void TryForChild(string charID)
        {
            ProtoMessage tryForChild = new ProtoMessage();
            tryForChild.ActionType = Actions.TryForChild;
            tryForChild.Message = charID;
            net.Send(tryForChild);
        }
    }



    /**********************************
    *********Fief-control commands ***
    ***********************************/
    public partial class TestClient
    {
        /// <summary>
        /// Get a list of all owned fiefs
        /// </summary>
        public void GetFiefList()
        {
            ProtoMessage getFiefs = new ProtoMessage();
            getFiefs.ActionType = Actions.ViewMyFiefs;
            net.Send(getFiefs);
        }
        /// <summary>
        /// View in-depth information about a fief. Amount of information depends on whether player owns fief or not
        /// </summary>
        /// <param name="fiefID">ID of fief to view</param>
        public void ViewFief(string fiefID)
        {
            ProtoMessage requestFief = new ProtoMessage();
            requestFief.ActionType = Actions.ViewFief;
            requestFief.Message = fiefID;
            net.Send(requestFief);
        }

        /// <summary>
        /// Appoint a new bailiff for a fief
        /// </summary>
        /// <param name="charID">ID of character to become bailiff</param>
        /// <param name="fiefID">Fief ID</param>
        public void AppointBailiff(string charID, string fiefID)
        {
            ProtoMessage appoint = new ProtoMessage();
            appoint.ActionType = Actions.AppointBailiff;
            appoint.Message = fiefID;
            appoint.MessageFields = new string[] { charID };
            net.Send(appoint);
        }

        /// <summary>
        /// Remove the bailiff of a fief from his appointed position
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        public void RemoveBailiff(string fiefID)
        {
            ProtoMessage appoint = new ProtoMessage();
            appoint.ActionType = Actions.RemoveBailiff;
            appoint.Message = fiefID;
            net.Send(appoint);
        }

        /// <summary>
        /// Transfers funds to or from the home fief
        /// </summary>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="fiefID">ID of fief to transfer to/from</param>
        /// <param name="toHome">Indicates whether funds will be transferred to home fief or from home fief to fief indicated in fiefID</param>
        public void TransferFunds(int amount, string fiefID, bool toHome)
        {
            ProtoTransfer transfer = new ProtoTransfer();
            transfer.amount = amount;
            if (toHome)
            {
                transfer.fiefFrom = fiefID;
            }
            else
            {
                transfer.fiefTo = fiefID;
            }
            transfer.ActionType = Actions.TransferFunds;
            net.Send(transfer);
        }

        /// <summary>
        /// Get a list of other players (useful for transferring funds)
        /// </summary>
        public void GetPlayerList()
        {
            ProtoMessage requestPlayers = new ProtoMessage();
            requestPlayers.ActionType = Actions.GetPlayers;
            net.Send(requestPlayers);
        }

        /// <summary>
        /// Transfer funds to another player (home fief treasury to home fief treasury)
        /// </summary>
        /// <param name="amount">Amount to send</param>
        /// <param name="playerID">ID/Username of player to transfer to</param>
        public void TransferFundsToPlayer(int amount, string playerID)
        {
            ProtoTransferPlayer transfer = new ProtoTransferPlayer();
            transfer.ActionType = Actions.TransferFundsToPlayer;
            transfer.amount = amount;
            transfer.playerTo = playerID;
            net.Send(transfer);
        }

        /// <summary>
        /// Adjust this fief's expenditure
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        /// <param name="newTax">new Tax value</param>
        /// <param name="newOff">new Official spend value</param>
        /// <param name="newGarr">new Garrison spend value</param>
        /// <param name="newKeep">new Keep spend value</param>
        /// <param name="newInfra">ew Infrastructure spend value</param>
        public void AdjustExpenditure(string fiefID, double newTax, double newOff, double newGarr, double newKeep, double newInfra)
        {

            ProtoGenericArray<double> newExpenses = new ProtoGenericArray<double>();
            newExpenses.Message = fiefID;
            newExpenses.fields = new double[] { newTax, newOff, newGarr, newInfra, newKeep };
            newExpenses.ActionType = Actions.AdjustExpenditure;
            net.Send(newExpenses);
            Console.WriteLine("CLIENT: sent adjust expenditure message");
        }

        /// <summary>
        /// Auto-adjusts the overspend for a fief
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        public void AutoAdjustExpenditure(string fiefID)
        {
            ProtoGenericArray<double> newExpenses = new ProtoGenericArray<double>();
            newExpenses.Message = fiefID;
            newExpenses.ActionType = Actions.AdjustExpenditure;
            net.Send(newExpenses);
        }

        /// <summary>
        /// Bar a character from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        /// <param name="charID">ID of character to be barred</param>
        public void BarCharacter(string fiefID, string charID)
        {
            ProtoMessage bar = new ProtoMessage();
            bar.Message = fiefID;
            bar.MessageFields = new string[] { charID };
            bar.ActionType = Actions.BarCharacters;
            net.Send(bar);
        }

        /// <summary>
        /// Unbar a character from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        /// <param name="charID">ID of character</param>
        public void UnbarCharacter(string fiefID, string charID)
        {
            ProtoMessage unbarMessage = new ProtoMessage();
            unbarMessage.ActionType = Actions.UnbarCharacters;
            unbarMessage.Message = fiefID;
            unbarMessage.MessageFields = new string[] { charID };
            net.Send(unbarMessage);
        }

        public void BarNationality(string natID, string fiefID)
        {
            ProtoMessage barMessage = new ProtoMessage();
            barMessage.Message = fiefID;
            barMessage.MessageFields = new string[] { natID };
            barMessage.ActionType = Actions.BarNationalities;
            net.Send(barMessage);
        }

        public void UnBarNationality(string natID, string fiefID)
        {
            ProtoMessage barMessage = new ProtoMessage();
            barMessage.Message = fiefID;
            barMessage.MessageFields = new string[] { natID };
            barMessage.ActionType = Actions.UnbarNationalities;
            net.Send(barMessage);
        }

    }



    public partial class TestClient
    {

        public class Network : IDisposable
        {
            private TestClient tClient;
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            HashAlgorithm hash = new SHA256Managed();
            public NetClient client = null;
            private NetConnection connection;
            private string user;
            private string pass;
            private IPAddress ip = NetUtility.Resolve("localhost");
            private int port = 8000;
            private NetEncryption alg = null;
            /// <summary>
            /// Optional- set encryption key manually for use in testing
            /// </summary>
            private byte[] key;
            public bool autoLogIn { get; set; }
            public bool loggedIn { get; set; }
            public CancellationTokenSource ctSource;

            public Network(TestClient tc, byte[] key = null)
            {
                this.tClient = tc;
                autoLogIn = true;
                this.key = key;
                ctSource = new CancellationTokenSource();
                InitializeClient();
            }

            public NetConnection GetConnection()
            {
                return connection;
            }

            public NetConnection GetServerConnection()
            {
                return client.ServerConnection;
            }

            public string GetConnectionStatusString()
            {
                return client.ConnectionStatus.ToString();
            }

            public NetConnectionStatus GetConnectionStatus()
            {
                return client.ConnectionStatus;
            }

            void InitializeClient()
            {
                NetPeerConfiguration config = new NetPeerConfiguration("test");
                config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, true);
                config.ConnectionTimeout = 3000f;
                client = new NetClient(config);

            }
            public void Connect(string username, string pass)
            {
                user = username;
                this.pass = pass;
                client.Start();
                string host = ip.ToString();
                // remember to encrypt the bloody thing in the final
                if (username != null)
                {
                    NetOutgoingMessage msg = client.CreateMessage(username);
                    msg.Write("TestString");
                    NetConnection c = client.Connect(host, port, msg);
                }
                else
                {
                    connection = client.Connect(host, port);
                }
                // Start listening for responses
                Thread t_reader = new Thread(new ThreadStart(this.read));
                t_reader.Start();

            }

            public void Disconnect()
            {
                ctSource.Cancel();
                if (!(client.ConnectionStatus == NetConnectionStatus.None || client.ConnectionStatus == NetConnectionStatus.Disconnected))
                {
                    client.Disconnect("Log out");
                }
                Dispose();
                client.Shutdown("Exit");
            }

            /// <summary>
            /// Computes the hash of a salt appended to source byte array
            /// </summary>
            /// <param name="toHash">bytes to be hashed</param>
            /// <param name="salt">salt</param>
            /// <returns>computed hash</returns>
            public byte[] ComputeHash(byte[] toHash, byte[] salt)
            {
                byte[] fullHash = new byte[toHash.Length + salt.Length];
                toHash.CopyTo(fullHash, 0);
                salt.CopyTo(fullHash, toHash.Length);
                byte[] hashcode = hash.ComputeHash(fullHash);
                return hashcode;
            }

            public void Send(ProtoMessage message, bool encrypt = true)
            {
                NetOutgoingMessage msg = client.CreateMessage();
                MemoryStream ms = new MemoryStream();
                try
                {
                    Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, message, ProtoBuf.PrefixStyle.Fixed32);
                    Console.Write("Client: Sending");
                    msg.Write(ms.GetBuffer());
                    if (alg != null && encrypt)
                    {
                        Console.Write(" encrypted");
                        msg.Encrypt(alg);
                    }
                    var result = client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                    Console.WriteLine(" message of type " + message.GetType() + " with Action: " + message.ActionType + " with result: " + result.ToString());
                    client.FlushSendQueue();

                }
                catch (Exception e)
                {
                    Console.WriteLine("CLIENT: Failed to serialise message!");
                }

            }

            public void ComputeAndSendHashAndKey(ProtoLogIn salts, byte[] key)
            {

                string hashstring = "";
                foreach (byte b in salts.userSalt)
                {
                    hashstring += b.ToString();
                }
                string sessSalt = "";
                foreach (byte b in salts.sessionSalt)
                {
                    sessSalt += b.ToString();
                }
                byte[] passBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hashPassword = ComputeHash(passBytes, salts.userSalt);
                string passHash = "";
                foreach (byte b in hashPassword)
                {
                    passHash += b.ToString();
                }
                byte[] hashFull = ComputeHash(hashPassword, salts.sessionSalt);
                string fullHash = "";
                foreach (byte b in hashFull)
                {
                    fullHash += b.ToString();
                }
                ProtoLogIn response = new ProtoLogIn();
                response.userSalt = hashFull;
                response.ActionType = Actions.LogIn;
                response.Key = key;
                Send(response, false);
            }

            /// <summary>
            /// Validates the certificate supplied by the server, and also creates a symmetric encryption key
            /// </summary>
            /// <returns><c>true</c>, if certificate was validated, <c>false</c> otherwise.</returns>
            /// <param name="login">ProtoLogin containing certificate</param>
            public bool ValidateCertificateAndCreateKey(ProtoLogIn login, out byte[] key)
            {
                if (login == null || login.certificate == null)
                {
                    Console.WriteLine("CLIENT: No certificate");
                    key = null;
                    return false;
                }
                else
                {
                    try
                    {
                        // Get certificate
                        X509Certificate2 cert = new X509Certificate2(login.certificate);
                        RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
#if DEBUG
                        if (this.key != null)
                        {
                            if (this.key.Length == 0)
                            {
                                alg = new NetAESEncryption(client);
                            }
                            else
                            {
                                alg = new NetAESEncryption(client,
                                this.key, 0, this.key.Length);
                            }
                            key = rsa.Encrypt(this.key, false);
                        }
                        else
                        {
                            // If no key, do not use an encryption algorithm
                            alg = null;
                            key = null;
                        }
#else 
                        // Create a new symmetric key
                        TripleDES des = TripleDESCryptoServiceProvider.Create();
                        des.GenerateKey();
                        // Encrypt key with server's public key
                        this.key = des.Key;
                        key = rsa.Encrypt(des.Key, false);
                        // Initialise the algoitm
                        alg = new NetAESEncryption(client, des.Key, 0, des.Key.Length);
                        Console.WriteLine("CLIENT: my unencrypted key:");
                        foreach (var bite in des.Key)
                        {
                            Console.Write(bite.ToString());
                        }
#endif
                        // Validate certificate
                        if (!cert.Verify())
                        {
                            X509Chain CertificateChain = new X509Chain();
                            //If you do not provide revokation information, use the following line.
                            CertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                            bool IsCertificateChainValid = CertificateChain.Build(cert);
                            if (!IsCertificateChainValid)
                            {
                                for (int i = 0; i < CertificateChain.ChainStatus.Length; i++)
                                {
                                    Console.WriteLine(i + ": " + CertificateChain.ChainStatus[i].Status.ToString() + "; " + CertificateChain.ChainStatus[i].StatusInformation);
                                }
                                // TODO change to false after testing
                                return true;
                            }

                        }
                        // temporary certificate validation fix
                        return true;
                        //return cert.Verify();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("A problem occurred when parsing certificate from bytes: \n" + "type: " + e.GetType().FullName + "\n " + ", source: " + e.Source + "\n message: " + e.Message);
                        key = null;
                        return false;
                    }
                }
            }

            public void read()
            {
                while (client.Status == NetPeerStatus.Running && !ctSource.Token.IsCancellationRequested)
                {

                    WaitHandle.WaitAny(new WaitHandle[] { client.MessageReceivedEvent, ctSource.Token.WaitHandle });
                    NetIncomingMessage im;
                    while ((im = client.ReadMessage()) != null&&!ctSource.IsCancellationRequested)
                    {

                        switch (im.MessageType)
                        {
                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.ErrorMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.VerboseDebugMessage:
                            case NetIncomingMessageType.Data:
                                try
                                {
                                    if (alg != null)
                                    {
                                        im.Decrypt(alg);
                                    }
                                    MemoryStream ms = new MemoryStream(im.Data);
                                    ProtoMessage m = null;
                                    try
                                    {
                                        m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);

                                    }
                                    catch (Exception e)
                                    {
                                        // Attempt to read string and add to message queue
                                        string s = im.ReadString();
                                        if (!string.IsNullOrWhiteSpace(s))
                                        {
                                            Console.WriteLine("CLIENT: Got message: " + s);

                                            tClient.stringMessageQueue.Enqueue(s);
                                        }
                                    }
                                    if (m != null)
                                    {
                                        Console.WriteLine("CLIENT: Got ProtoMessage with ActionType: " + m.ActionType + " and response type: " + m.ResponseType);
                                        if (m.ResponseType == DisplayMessages.LogInSuccess)
                                        {
                                            loggedIn = true;
                                            tClient.protobufMessageQueue.Enqueue(m);
                                        }
                                        else
                                        {
                                            if (m.ActionType == Actions.Update)
                                            {
                                                // Don't do anything at the moment for updates
                                            }
                                            else
                                            {
                                                tClient.protobufMessageQueue.Enqueue(m);
                                                if (m.ActionType == Actions.LogIn && m.ResponseType == DisplayMessages.None)
                                                {
                                                    byte[] key = null;
                                                    if (ValidateCertificateAndCreateKey(m as ProtoLogIn, out key))
                                                    {
                                                        ComputeAndSendHashAndKey(m as ProtoLogIn, key);
                                                    }
                                                }
                                                else
                                                {
                                                    // Attempt to read string and add to message queue
                                                    string s = im.ReadString();
                                                    if (!string.IsNullOrWhiteSpace(s))
                                                    {

                                                        tClient.stringMessageQueue.Enqueue(s);

                                                    }
                                                }
                                            }

                                        }


                                    }
                                }
                                catch (Exception e)
                                {
                                    Globals_Server.logError("Error in reading data: " + e.GetType() + " :" + e.Message + "; Stack Trace: " + e.StackTrace);
                                }
                                break;
                            case NetIncomingMessageType.StatusChanged:

                                NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                                Console.WriteLine("CLIENT: Status changed to " + status.ToString());
                                //MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
                                if (status == NetConnectionStatus.Connected)
                                {

                                    if (im.SenderConnection.RemoteHailMessage != null)
                                    {
                                        try
                                        {
                                            MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
                                            ProtoMessage m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms2, PrefixStyle.Fixed32);
                                            if (m != null)
                                            {
                                                tClient.protobufMessageQueue.Enqueue(m);
                                                if (m.ActionType == Actions.LogIn && m.ResponseType == DisplayMessages.None)
                                                {
                                                    byte[] key = null;
                                                    if (ValidateCertificateAndCreateKey(m as ProtoLogIn, out key))
                                                    {
                                                        if (autoLogIn)
                                                        {
                                                            ComputeAndSendHashAndKey(m as ProtoLogIn, key);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Certificate validation failed: Server may be untrusted");
                                                        client.Disconnect("Invalid Certificate");
                                                    }

                                                }

                                            }
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                    break;
                                }
                                else if (status == NetConnectionStatus.Disconnected)
                                {
                                    string reason = im.ReadString();
                                    if (!string.IsNullOrEmpty(reason))
                                    {

                                        tClient.stringMessageQueue.Enqueue(reason);



                                    }
                                }
                                if (im.SenderConnection.RemoteHailMessage != null && (NetConnectionStatus)im.ReadByte() == NetConnectionStatus.Connected)
                                {

                                }
                                break;
                            case NetIncomingMessageType.ConnectionLatencyUpdated:
                                break;
                            default:
                                break;
                        }
                        client.Recycle(im);
                    }
                }
#if DEBUG
                Globals_Server.logEvent("Client listening thread ends");
#endif
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);

            }

            protected virtual void Dispose(bool dispose)
            {
                crypto.Dispose();
                hash.Dispose();
            }
        }
    }
}
