using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Lidgren.Network;
namespace hist_mmorpg
{
    /// <summary>
    /// Represents a client, their details and the information about their objects
    /// </summary>
#if V_CLIENT
    [ContractVerification(true)]
#endif
    public class Client : IEquatable<Client>
    {
        /// <summary>
        /// Holds the client's connection
        /// </summary>
        public NetConnection conn { get; set; }
        /// <summary>
        /// The client's username, aka playerID
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// Holds PlayerCharacter associated with the player using this client
        /// </summary>
        public PlayerCharacter myPlayerCharacter { get; set; }
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        public Character activeChar { get; set; }
        /// <summary>
        /// Holds Fief to view in UI
        /// </summary>
        public Fief fiefToView { get; set; }
        /// <summary>
        /// Holds Province to view in UI
        /// </summary>
        public Province provinceToView { get; set; }
        /// <summary>
        /// Holds Army to view in UI
        /// </summary>
        public Army armyToView { get; set; }
        /// <summary>
        /// Holds Siege to view in UI
        /// </summary>
        public Siege siegeToView { get; set; }
        /// <summary>
        /// Holds past events
        /// </summary>
        public Journal myPastEvents;
        /// <summary>
        /// Holds current set of events being displayed in UI
        /// </summary>
        public SortedList<uint, JournalEntry> eventSetToView;
        /// <summary>
        /// Holds index position of currently displayed entry in eventSetToView
        /// </summary>
        public int jEntryToView { get; set; }
        /// <summary>
        /// Holds highest index position in eventSetToView
        /// </summary>
        public int jEntryMax { get; set; }
        /// <summary>
        /// Holds bool indicating whether or not to display popup messages
        /// </summary>
        public bool showMessages = true;
        /// <summary>
        /// Holds bool indicating whether or not to display popup debug messages
        /// </summary>
        public bool showDebugMessages = false;
        /// <summary>
        /// Holds the algorithm to be used during encryption and decryption. Alg is generated using the peer and a key obtained from the client 
        /// </summary>
        public NetAESEncryption alg = null;

        /// <summary>
        /// Create a new client object
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="pcID">PlayerCharacterID</param>
        public Client(String user, String pcID)
        {
            Contract.Requires(!string.IsNullOrEmpty(user));
            Contract.Requires(!string.IsNullOrEmpty(pcID));
            Contract.Requires(Contract.Exists(Globals_Game.pcMasterList,(i=>i.Key.Equals(pcID))));
            // set username associated with client
            this.username = user;

            // get playercharacter from master list of player characers
            myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            myPlayerCharacter.playerID = user;
            // set inital fief to display
            fiefToView = myPlayerCharacter.location;

            // set player's character to display
            activeChar = myPlayerCharacter;

            // Set up journal
            myPastEvents = new Journal();
            // Set up journal events
            eventSetToView = new SortedList<uint, JournalEntry>();

            Globals_Game.ownedPlayerCharacters.Add(user,myPlayerCharacter);
        }

        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="message">The message code to send</param>
        /// <param name="fields">Additional information to add to the message</param>
        public void Update(DisplayMessages message, string[] fields = null)
        {
            ProtoMessage m = new ProtoMessage();
            m.ActionType = Actions.Update;
            m.ResponseType = message;
            m.MessageFields = fields;
            if (conn != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + message.ToString());
                Console.WriteLine("Sending update " + message.ToString() + " to " + this.username);
                Server.SendViaProto(m, conn,alg);
            }
        }

        /// <summary>
        /// Send an update to the client- used when the message to be sent requires additional information other than just a response code and some strings
        /// </summary>
        /// <param name="message">Message to be sent- can contain any number of details</param>
        public void Update(ProtoMessage message)
        {
            Contract.Requires(message != null);
            message.ActionType = Actions.Update;
            if (conn != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + message.ResponseType.ToString());
                Console.WriteLine("Sending update " + message.ResponseType.ToString() + " to " + this.username);
                Server.SendViaProto(message, conn, alg);
            }
        }

        /// <summary>
        /// Two clients are equal if their usernames are the same
        /// </summary>
        /// <param name="other">The client to be compared</param>
        /// <returns>True if the usernames match, false if otherwise</returns>
        public bool Equals(Client other)
        {
            return this.username.Equals(other.username);
        }
    }

    /// <summary>
    /// Serialised version of Client for used in reading/writing to database
    /// </summary>
    public class Client_Serialized
    {
        public string user { get; set; }
        public string pcID { get; set; }
        public string activeChar { get; set; }
        public Journal myPastEvents { get; set; }

        public Client_Serialized(Client c)
        {
            this.user = c.username;
            this.pcID = c.myPlayerCharacter.charID;
            this.myPastEvents = c.myPastEvents;
            this.activeChar = c.activeChar.charID;
        }

        public Client deserialise()
        {
            Client c = new Client(user, pcID);
            c.myPastEvents = this.myPastEvents;
            c.myPlayerCharacter = Globals_Game.getCharFromID(pcID) as PlayerCharacter;
            c.activeChar = Globals_Game.getCharFromID(activeChar);
            return c;
        }
    }
}
