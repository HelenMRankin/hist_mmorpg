using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
namespace hist_mmorpg
{
    /// <summary>
    /// Represents a connected client
    /// </summary>
    public class Client : IEquatable<Client>
    {
        public NetConnection conn { get; set; }
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
        public Journal myPastEvents = new Journal();
        /// <summary>
        /// Holds current set of events being displayed in UI
        /// </summary>
        public SortedList<uint, JournalEntry> eventSetToView = new SortedList<uint, JournalEntry>();
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
        public Client(String user, String pcID)
        {
            // set username associated with client
            this.username = user;

            // get playercharacter from master list of player characers
            myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            myPlayerCharacter.playerID = user;
            // set inital fief to display
            fiefToView = myPlayerCharacter.location;

            // set player's character to display
            activeChar = myPlayerCharacter;

            Globals_Game.ownedPlayerCharacters.Add(user,myPlayerCharacter);
        }
        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="info"></param>
        public void Update(DisplayMessages type, string[] fields = null)
        {

            ProtoMessage m = new ProtoMessage();
            m.ActionType = Actions.Update;
            m.ResponseType = type;
            m.MessageFields = fields;
            if (conn != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + type.ToString());
                Console.WriteLine("Sending update " + type.ToString() + " to " + this.username);
                Server.SendViaProto(m, conn,alg);
            }
        }

        public void Update(ProtoMessage message)
        {
            message.ActionType = Actions.Update;
            if (conn != null)
            {
                Globals_Server.logEvent("Update " + this.username + ": " + message.ResponseType.ToString());
                Console.WriteLine("Sending update " + message.ResponseType.ToString() + " to " + this.username);
                Server.SendViaProto(message, conn, alg);
            }
        }

        public bool Equals(Client other)
        {
            return this.username.Equals(other.username);
        }
    }

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
