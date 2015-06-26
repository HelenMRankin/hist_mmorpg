using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Represents a connected client
    /// </summary>
    public class Client
    {
        public string user { get; set; }
        /// <summary>
        /// Holds PlayerCharacter associated with the player using this client
        /// </summary>
        public PlayerCharacter myPlayerCharacter { get; set; }
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        public Character charToView { get; set; }
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

        public Client(String user, String pcID)
        {
            // set username associated with client
            this.user = user;

            // register client as observer
            Globals_Game.RegisterObserver(this);

            // get playercharacter from master list of player characers
            myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            // set inital fief to display
            fiefToView = myPlayerCharacter.location;

            // set player's character to display
            charToView = myPlayerCharacter;
        }
        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="info"></param>
        public void Update(String info)
        {
            // get update info
            string[] infoSplit = info.Split('|');
            switch (infoSplit[0])
            {
                case "newEvent":
                    // get jEntry ID and retrieve from Globals_Game
                    if (!String.IsNullOrWhiteSpace(infoSplit[1]))
                    {
                        uint newJentryID = Convert.ToUInt32(infoSplit[1]);
                        JournalEntry newJentry = Globals_Game.pastEvents.entries[newJentryID];

                        // check to see if is of interest to player
                        if (newJentry.CheckEventForInterest(myPlayerCharacter))
                        {
                            myPastEvents.AddNewEntry(newJentry);
                        }
                    }
                    break;
                case "error":
                    //TODO implement error messages
                    break;
                default:
                    break;
            }
        }

    }

    public class Client_Serialized
    {
        public string user { get; set; }
        public string pcID { get; set; }
        public Journal myPastEvents { get; set; }

        public Client_Serialized(Client c)
        {
            this.user = c.user;
            this.pcID = c.myPlayerCharacter.charID;
            this.myPastEvents = c.myPastEvents;
        }

        public Client deserialise()
        {
            Client c = new Client(user, pcID);
            c.myPastEvents = this.myPastEvents;
            return c;
        }
    }
}
