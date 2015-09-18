using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class allowing storage of game events (past and future)
    /// </summary>
    public class Journal
    {
        /// <summary>
        /// Holds entries
        /// </summary>
        public SortedList<uint, JournalEntry> entries = new SortedList<uint, JournalEntry>();
        /// <summary>
        /// Indicates presence of new (unread) entries
        /// </summary>
        public bool areNewEntries = false;
        /// <summary>
        /// Priority level of new (unread) entries
        /// </summary>
        public byte priority = 0;

        /// <summary>
        /// Constructor for Journal
        /// </summary>
        /// <param name="entList">SortedList(uint, JournalEntry) holding entries</param>
        public Journal(SortedList<uint, JournalEntry> entList = null)
        {
            if (entList != null)
            {
                this.entries = entList;
            }
        }

        /// <summary>
        /// Checks to see if there are any unviewed entries in the journal
        /// </summary>
        /// <returns>bool indicating presence of unviewed entries</returns>
        public bool CheckForUnviewedEntries()
        {
            bool areUnviewed = false;

            foreach (KeyValuePair <uint, JournalEntry> thisEntry in this.entries)
            {
                if (!thisEntry.Value.viewed)
                {
                    areUnviewed = true;
                    break;
                }
            }

            return areUnviewed;
        }
        
        /// <summary>
        /// Returns any entries matching search criteria (year, season)
        /// </summary>
        /// <returns>SortedList of JournalEntrys</returns>
        /// <param name="yr">Year to search for</param>
        /// <param name="seas">Season to search for</param>
        public SortedList<uint, JournalEntry> GetEventsOnDate(uint yr = 9999, Byte seas = 99)
        {
            SortedList<uint, JournalEntry> matchingEntries = new SortedList<uint, JournalEntry>();

            // determine scope of search
            String scope = "";
            // if no year specified, return events from all years and seasons
            if (yr == 9999)
            {
                scope = "all";
            }
            // if a year is specified
            else
            {
                // if no season specified, return events from all seasons in the specified year
                if (seas == 99)
                {
                    scope = "yr";
                }
                // if a season is specified, return events from specified season in the specified year
                else
                {
                    scope = "seas";
                }
            }

            switch (scope)
            {
                case "seas":
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in this.entries)
                    {
                        // year and season must match
                        if (jEntry.Value.year == yr)
                        {
                            if (jEntry.Value.season == seas)
                            {
                                matchingEntries.Add(jEntry.Key, jEntry.Value);
                            }
                        }
                    }
                    break;
                case "yr":
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in this.entries)
                    {
                        // year must match
                        if (jEntry.Value.year == yr)
                        {
                            matchingEntries.Add(jEntry.Key, jEntry.Value);
                        }
                    }
                    break;
                default:
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in this.entries)
                    {
                        // get all events
                        matchingEntries.Add(jEntry.Key, jEntry.Value);
                    }
                    break;
            }

            return matchingEntries;
        }

        /// <summary>
        /// Retrieves all unviewed JournalEntrys
        /// </summary>
        /// <returns>SortedList(uint, JournalEntry) containing relevant entries</returns>
        public SortedList<uint, JournalEntry> GetUnviewedEntries()
        {
            SortedList<uint, JournalEntry> foundEntries = new SortedList<uint,JournalEntry>();

            foreach (KeyValuePair<uint, JournalEntry> jEntry in this.entries)
            {
                if (!jEntry.Value.viewed)
                {
                    foundEntries.Add(jEntry.Key, jEntry.Value);
                }
            }

            return foundEntries;
        }

        /// <summary>
        /// Retrieves JournalEntrys associated with the specified character, role, and JournalEntry type
        /// </summary>
        /// <returns>List<JournalEntry> containing relevant entries</returns>
        /// <param name="thisPerson">The ID of the person of interest</param>
        /// <param name="role">The person's role (in personae)</param>
        /// <param name="entryType">The JournalEntry type</param>
        public List<JournalEntry> GetSpecificEntries(string thisPersonID, string role, string entryType)
        {
            List<JournalEntry> foundEntries = new List<JournalEntry>();
            bool entryFound = false;

            foreach (KeyValuePair<uint, JournalEntry> jEntry in this.entries)
            {
                // get entries of specified type
                if (jEntry.Value.type.Equals(entryType))
                {
                    // iterate through personae
                    for (int i = 0; i < jEntry.Value.personae.Length; i++)
                    {
                        // get and split personae
                        string thisPersonae = jEntry.Value.personae[i];
                        string[] thisPersonaeSplit = thisPersonae.Split('|');

                        if (thisPersonaeSplit[0] != null)
                        {
                            // look for specified role
                            if (thisPersonaeSplit[1].Equals(role))
                            {
                                // look for matching charID
                                if (thisPersonaeSplit[0].Equals(thisPersonID))
                                {
                                    foundEntries.Add(jEntry.Value);
                                    entryFound = true;
                                    break;
                                }
                            }
                        }

                    }
                }

                if (entryFound)
                {
                    break;
                }
            }

            return foundEntries;
        }

        /// <summary>
        /// Adds a new JournalEntry to the Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public bool AddNewEntry(JournalEntry jEntry)
        {
            bool success = false;

            if (jEntry.jEntryID > 0)
            {
                try
                {
                    // add entry
                    this.entries.Add(jEntry.jEntryID, jEntry);

                    if (this.entries.ContainsKey(jEntry.jEntryID))
                    {
                        success = true;
                    }
                    else
                    {
                        //HACK
                        /*
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Error: JournalEntry not added.", "INSERTION ERROR");
                        }*/
                    }
                }
                catch (System.ArgumentException ae)
                {
                    //HACK
                    /*
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ae.Message + "\r\nPlease check for duplicate jEventID.", "INSERTION ERROR");
                    }*/
                }
            }

            return success;

        }

        /// <summary>
        /// Returns a JournalEntry set, based on criteria passed in
        /// </summary>
        /// <returns>SortedList containing JournalEntrys</returns>
        /// <param name="setScope">The type of JournalEvent set to fetch</param>
        public SortedList<uint, JournalEntry> getJournalEntrySet(string setScope, uint thisYear, byte thisSeason)
        {
            SortedList<uint, JournalEntry> jEntrySet = new SortedList<uint, JournalEntry>();

            // get appropriate jEntry set
            switch (setScope)
            {
                // get entries for current year
                case "year":
                    jEntrySet = this.GetEventsOnDate(yr: thisYear);
                    break;
                // get entries for current season
                case "season":
                    jEntrySet = this.GetEventsOnDate(yr: thisYear, seas: thisSeason);
                    break;
                // get unread entries
                case "unread":
                    jEntrySet = this.GetUnviewedEntries();
                    break;
                // get all entries
                default:
                    jEntrySet = this.GetEventsOnDate();
                    break;
            }

            return jEntrySet;

        }

    }
    // Journal entries could be much better represented, as there are a fixed number of types and so the description can be stored on the client side
    /// <summary>
    /// Class containing details of a Journal entry
    /// </summary>
    public class JournalEntry
    {
        /// <summary>
        /// Holds JournalEntry ID
        /// </summary>
        public uint jEntryID { get; set; }
        /// <summary>
        /// Holds event year
        /// </summary>
        public uint year { get; set; }
        /// <summary>
        /// Holds event season
        /// </summary>
        public byte season { get; set; }
        /// <summary>
        /// Holds main objects (IDs) associated with event and their role
        /// </summary>
        public String[] personae { get; set; }
        /// <summary>
        /// Holds type of event (e.g. battle, birth)
        /// </summary>
        public String type { get; set; }
        /// <summary>
        /// Holds location of event (fiefID)
        /// </summary>
        public String location { get; set; }
        /// <summary>
        /// Indicates whether entry has been viewed
        /// </summary>
        public bool viewed { get; set; }
        /// <summary>
        /// Indicates whether entry has been replied to (e.g. for Proposals)
        /// </summary>
        public bool replied { get; set; }
        /// <summary>
        /// Holds ProtoMessage containing details of event. More flexible than strings.
        /// </summary>
        public ProtoMessage entryDetails { get; set; }
        /// <summary>
        /// Constructor for JournalEntry
        /// </summary>
        /// <param name="id">uint holding JournalEntry ID</param>
        /// <param name="yr">uint holding event year</param>
        /// <param name="seas">byte holding event season</param>
        /// <param name="pers">String[] holding main objects (IDs) associated with event and thier role</param>
        /// <param name="typ">String holding type of event</param>
        /// <param name="loc">String holding location of event (fiefID)</param>
        /// <param name="messageIdentifier">Enum representing description of event</param>
        public JournalEntry(uint id, uint yr, byte seas, String[] pers, String typ, ProtoMessage details, String loc = null)
        {
            // VALIDATION

            // SEAS
            // check between 0-3
            if (!Utility_Methods.ValidateSeason(seas))
            {
                throw new InvalidDataException("JournalEntry season must be a byte between 0-3");
            }
            this.entryDetails = details;
            // PERS
            if (pers.Length > 0)
            {
                for (int i = 0; i < pers.Length; i++)
                {
                    // split using'|'
                    string[] persSplit = pers[i].Split('|');
                    if (persSplit.Length > 1)
                    {
                        // character ID: trim and ensure 1st is uppercase
                        if (!persSplit[0].Contains("all"))
                        {
                            persSplit[0] = Utility_Methods.FirstCharToUpper(persSplit[0].Trim());
                        }
                        // trim role
                        persSplit[1] = persSplit[1].Trim();
                        // put back together
                        pers[i] = persSplit[0] + "|" + persSplit[1];
                    }

                    if (!Utility_Methods.ValidateJentryPersonae(pers[i]))
                    {
                        throw new InvalidDataException("Each JournalEntry personae must consist of 2 sections of letters, divided by '|', the 1st of which must be a valid character ID");
                    }
                }
            }

            // TYPE
            if (String.IsNullOrWhiteSpace(typ))
            {
                throw new InvalidDataException("JournalEntry type must be a string > 0 characters in length");
            }

            // LOC
            if (!String.IsNullOrWhiteSpace(loc))
            {
                // trim and ensure is uppercase
                loc = loc.Trim().ToUpper();

                if (!Utility_Methods.ValidatePlaceID(loc))
                {
                    throw new InvalidDataException("JournalEntry location id must be 5 characters long, start with a letter, and end in at least 2 numbers");
                }
            }

            this.jEntryID = id;
            this.year = yr;
            this.season = seas;
            this.personae = pers;
            this.type = typ;
            if (!String.IsNullOrWhiteSpace(loc))
            {
                this.location = loc;
            }

            this.viewed = false;
        }

        /// <summary>
        /// Create a new JournalEntry- used for more complex messages that would be more appropriate to be reconstructed on the client side
        /// </summary>
        /// <param name="m"></param>
        /// <param name="id"></param>
        /// <param name="yr"></param>
        /// <param name="seas"></param>
        /// <param name="pers"></param>
        /// <param name="typ"></param>
        /// <param name="loc"></param>
        /// <param name="desc"></param>
        public JournalEntry(ProtoMessage m,uint id, uint yr, byte seas, String[] pers, String typ,  String loc = null, string desc = null )
        {

            this.entryDetails = m;
            // VALIDATION

            // SEAS
            // check between 0-3
            if (!Utility_Methods.ValidateSeason(seas))
            {
                throw new InvalidDataException("JournalEntry season must be a byte between 0-3");
            }

            // PERS
            if (pers.Length > 0)
            {
                for (int i = 0; i < pers.Length; i++)
                {
                    // split using'|'
                    string[] persSplit = pers[i].Split('|');
                    if (persSplit.Length > 1)
                    {
                        // character ID: trim and ensure 1st is uppercase
                        if (!persSplit[0].Contains("all"))
                        {
                            persSplit[0] = Utility_Methods.FirstCharToUpper(persSplit[0].Trim());
                        }
                        // trim role
                        persSplit[1] = persSplit[1].Trim();
                        // put back together
                        pers[i] = persSplit[0] + "|" + persSplit[1];
                    }

                    if (!Utility_Methods.ValidateJentryPersonae(pers[i]))
                    {
                        throw new InvalidDataException("Each JournalEntry personae must consist of 2 sections of letters, divided by '|', the 1st of which must be a valid character ID");
                    }
                }
            }

            // TYPE
            if (String.IsNullOrWhiteSpace(typ))
            {
                throw new InvalidDataException("JournalEntry type must be a string > 0 characters in length");
            }

            // LOC
            if (!String.IsNullOrWhiteSpace(loc))
            {
                // trim and ensure is uppercase
                loc = loc.Trim().ToUpper();

                if (!Utility_Methods.ValidatePlaceID(loc))
                {
                    throw new InvalidDataException("JournalEntry location id must be 5 characters long, start with a letter, and end in at least 2 numbers");
                }
            }

            this.jEntryID = id;
            this.year = yr;
            this.season = seas;
            this.personae = pers;
            this.type = typ;
            if (!String.IsNullOrWhiteSpace(loc))
            {
                this.location = loc;
            }
            this.viewed = false;

        }
        /// <summary>
        /// Returns a string containing the details of a JournalEntry
        /// </summary>
        /// <returns>JournalEntry details</returns>
        public string GetJournalEntryDetails()
        {
            string entryText = "";

            // ID
            entryText += "ID: " + this.jEntryID + "\r\n\r\n";

            // year and season
            entryText += "Date: " + Globals_Game.clock.seasons[this.season] + ", " + this.year + "\r\n\r\n";

            // type
            entryText += "Type: " + this.type + "\r\n\r\n";

            // personae
            entryText += "Personae:\r\n";
            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');
                Character thisCharacter = null;

                // get character
                if (thisPersonaeSplit[0] != null)
                {
                    // filter out any "all|all" entries
                    if (!thisPersonaeSplit[0].Equals("all"))
                    {
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            thisCharacter = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            thisCharacter = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                    }
                }

                if (thisCharacter != null)
                {
                    entryText += thisCharacter.firstName + " " + thisCharacter.familyName
                        + " (" + thisPersonaeSplit[1] + ")\r\n";
                }
            }
            entryText += "\r\n";

            // location
            if (!String.IsNullOrWhiteSpace(this.location))
            {
                Place thisPlace = null;
                if (Globals_Game.fiefMasterList.ContainsKey(this.location))
                {
                    thisPlace = Globals_Game.fiefMasterList[this.location];
                }
                else if (Globals_Game.provinceMasterList.ContainsKey(this.location))
                {
                    thisPlace = Globals_Game.provinceMasterList[this.location];
                }
                else if (Globals_Game.kingdomMasterList.ContainsKey(this.location))
                {
                    thisPlace = Globals_Game.kingdomMasterList[this.location];
                }
                entryText += "Location: " + thisPlace.name + " (" + this.location + ")\r\n\r\n";
            }

            return entryText;
        }

        /// <summary>
        /// Check the level of priority for the JournalEntry
        /// </summary>
        /// <returns>byte indicating the level of priority</returns>
        /// <param name="jEntry">The JournalEntry</param>
        public byte CheckEventForPriority(PlayerCharacter playerChar)
        {
            byte priority = 0;

            // get player's role
            string thisRole = "";
            for (int i = 0; i < this.personae.Length; i++)
            {
                string[] personaeSplit = this.personae[i].Split('|');
                if (personaeSplit[0].Equals("all"))
                {
                    thisRole = personaeSplit[1];
                }
                else if (personaeSplit[0].Equals(playerChar.charID))
                {
                    thisRole = personaeSplit[1];
                    break;
                }
            }

            // get priority
            foreach (KeyValuePair <string[], byte> priorityEntry in Globals_Game.jEntryPriorities)
            {
                if (priorityEntry.Key[0] == this.type)
                {
                    if (thisRole.Equals(priorityEntry.Key[1]))
                    {
                        priority = priorityEntry.Value;
                        break;
                    }
                }
            }

            return priority;
        }
        /// <summary>
        /// Returms an array of all PlayerCharacters who may be interested in event
        /// (Refactored for efficiency)
        /// </summary>
        /// <returns></returns>
        public PlayerCharacter[] CheckEventForInterest()
        {
            List<PlayerCharacter> interestedParties = new List<PlayerCharacter>();
            for (int i = 0; i < this.personae.Length; i++)
            {
                // get personae ID
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');
                // Detect if personae is a player character
                if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                {
                    // Do not add same person twice
                    if (!interestedParties.Contains(Globals_Game.pcMasterList[thisPersonaeSplit[0]]))
                    {
                        interestedParties.Add(Globals_Game.pcMasterList[thisPersonaeSplit[0]]);
                    }
                }
                // Return all player characters if event effects all
                else if (thisPersonaeSplit[0].Equals("all"))
                {
                    interestedParties = Globals_Game.pcMasterList.Values.ToList();
                    return Globals_Game.pcMasterList.Values.ToArray();
                }
            }
            return interestedParties.ToArray();
        }

        /// <summary>
        /// Check to see if the JournalEntry is a valid proposal
        /// </summary>
        /// <returns>bool indicating whether the controls be enabled</returns>
        public bool CheckForProposal(PlayerCharacter playerChar)
        {
            bool isValidProposal = false;

            // check if is a marriage proposal
            if (this.type.Equals("proposalMade"))
            {
                // check if have already replied
                if (!this.replied)
                {
                    // check if player made or received proposal
                    for (int i = 0; i < this.personae.Length; i++)
                    {
                        string thisPersonae = this.personae[i];
                        string[] thisPersonaeSplit = thisPersonae.Split('|');
                        if (thisPersonaeSplit[0].Equals(playerChar.charID))
                        {
                            if (thisPersonaeSplit[1].Equals("headOfFamilyBride"))
                            {
                                isValidProposal = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isValidProposal;
        }

        // TODO I suspect there may be issues with this if any of the characters die. Test.
        /// <summary>
        /// Allows a character to reply to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether reply was processed successfully</returns>
        /// <param name="proposalAccepted">bool indicating whether proposal accepted</param>
        public bool ReplyToProposal(bool proposalAccepted)
        {
            bool success = true;
            string[] replyFields = new string[4];
            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint replyID = Globals_Game.GetNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(headOfFamilyBride.charID + "|headOfFamilyBride");
            tempPersonae.Add(headOfFamilyGroom.charID + "|headOfFamilyGroom");
            tempPersonae.Add(bride.charID + "|bride");
            tempPersonae.Add(groom.charID + "|groom");
            if (proposalAccepted)
            {
                tempPersonae.Add("all|all");
            }
            string[] myReplyPersonae = tempPersonae.ToArray();

            // type
            string type = "";
            if (proposalAccepted)
            {
                type = "proposalAccepted";
            }
            else
            {
                type = "proposalRejected";
            }

            // description
            replyFields[0] = groom.firstName + " " + groom.familyName;
            replyFields[1] = bride.firstName + " " + bride.familyName;

            if (proposalAccepted)
            {
                replyFields[2] = "ACCEPTED";
            }
            else
            {
                replyFields[2] = "REJECTED";
            }
            replyFields[3] = headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName;

            ProtoMessage proposalReply = new ProtoMessage();
            proposalReply.MessageFields = replyFields;
            proposalReply.ResponseType = DisplayMessages.JournalProposalReply;
            // create and send a proposal reply (journal entry)
            JournalEntry myProposalReply = new JournalEntry(replyID, year, season, myReplyPersonae, type, proposalReply, null);
            success = Globals_Game.AddPastEvent(myProposalReply);

            if (success)
            {
                string[] newFields = new string[this.entryDetails.MessageFields.Length + 2];
                Array.Copy(this.entryDetails.MessageFields, newFields, this.entryDetails.MessageFields.Length);
                newFields[newFields.Length - 1] = Globals_Game.clock.seasons[season] + ", " + year;
                this.entryDetails.MessageFields = newFields;
                this.replied = true;
                // mark proposal as replied
                if (proposalAccepted)
                {
                    this.entryDetails.MessageFields[this.entryDetails.MessageFields.Length - 2] = "ACCEPTED";
                }
                else
                {
                    this.entryDetails.MessageFields[this.entryDetails.MessageFields.Length - 2] = "REJECTED";
                }

                // if accepted, process engagement
                if (proposalAccepted)
                {
                    myProposalReply.ProcessEngagement();
                }
            }

            return success;
        }

        /// <summary>
        /// Processes the actions involved with an engagement
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        public bool ProcessEngagement()
        {
            bool success = false;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint replyID = Globals_Game.GetNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;
            if (season == 3)
            {
                season = 0;
                year++;
            }
            else
            {
                season++;
            }

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry };

            // type
            string type = "marriage";

            // create and add a marriage entry to the scheduledEvents journal
            JournalEntry marriageEntry = new JournalEntry(replyID, year, season, marriagePersonae, type,null);
            success = Globals_Game.AddScheduledEvent(marriageEntry);

            // show bride and groom as engaged
            if (success)
            {
                bride.fiancee = groom.charID;
                groom.fiancee = bride.charID;
            }

            return success;
        }

        /// <summary>
        /// Processes the actions involved with a marriage
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        public bool ProcessMarriage()
        {
            bool success = false;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint marriageID = Globals_Game.GetNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string allEntry = "all|all";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry, allEntry };

            // type
            string type = "marriage";

            string[] fields = new string[3];
            fields[0] = groom.firstName + " " + groom.familyName;
            fields[1] = bride.firstName + " " + groom.familyName;
            fields[2] = bride.familyName;
            // description

            ProtoMessage marriage = new ProtoMessage();
            marriage.MessageFields=fields;
            marriage.ResponseType=DisplayMessages.JournalMarriage;
            // create and add a marriage entry to the pastEvents journal
            JournalEntry marriageEntry = new JournalEntry(marriageID, year, season, marriagePersonae, type,marriage,null);
            success = Globals_Game.AddPastEvent(marriageEntry);

            if (success)
            {
                // remove fiancees
                bride.fiancee = null;
                groom.fiancee = null;

                // add spouses
                bride.spouse = groom.charID;
                groom.spouse = bride.charID;

                // change wife's family
                bride.familyID = groom.familyID;
                bride.familyName = groom.familyName;

                // switch myNPCs
                headOfFamilyBride.myNPCs.Remove(bride as NonPlayerCharacter);
                headOfFamilyGroom.myNPCs.Add(bride as NonPlayerCharacter);

                // move wife to groom's location
                bride.location = groom.location;

                // check to see if headOfFamilyBride should receive increase in stature
                // get highest rank for headOfFamilyBride and headOfFamilyGroom
                Rank brideHighestRank = headOfFamilyBride.GetHighestRank();
                Rank groomHighestRank = headOfFamilyGroom.GetHighestRank();

                // compare ranks
                if ((brideHighestRank != null) && (groomHighestRank != null))
                {
                    if (groomHighestRank.id < brideHighestRank.id)
                    {
                        headOfFamilyBride.AdjustStatureModifier((brideHighestRank.id - groomHighestRank.id) * 0.4);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Respond to ransom demands
        /// </summary>
        /// <param name="paid">Whether or not ransom is to be paid</param>
        /// <returns>Bool indicating success</returns>
        public bool RansomResponse(bool paid, out ProtoMessage error)
        {
            error = null;
            // Check if type is ransom
            if (this.type.Equals("ransom"))
            {
                // Check if already replied
                if (replied)
                {
                    // Already replied
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.RansomRepliedAlready;
                    return false;
                }
                Character captive=null;
                PlayerCharacter captor;
                PlayerCharacter captiveHeadOfHousehold;
                // Confirm captive is still alive and being held
                foreach(string persona in personae) {
                    string[] split = persona.Split(new char[]{'|'});
                    if (split[1].Equals("Captive"))
                    {
                        captive = Globals_Game.getCharFromID(split[0]);
                    }
                }
                if (captive == null)
                {
                    // Captive does not exist- error
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                    Globals_Server.logError("Captive unidentified in JEntry: " + this.jEntryID);
                    return false;
                }
                else if (!captive.isAlive)
                {
                    // Captive is dead
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.RansomCaptiveDead;
                    return false;
                }
                captor = Globals_Game.getCharFromID(captive.captorID) as PlayerCharacter;
                if (captor == null)
                {
                    // Captive does not have a captor
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.NotCaptive;
                    return false;
                }
                captiveHeadOfHousehold = captive.GetPlayerCharacter();
                if (captiveHeadOfHousehold == null)
                {
                    // Captive is not an employee, family member or player character
                    Globals_Server.logError("Captive has no PlayerCharacter: " + captive.charID);
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                    return false;
                }
                if(paid) {
                    // Get ransom amount
                    uint ransom = 0;
                    if (!UInt32.TryParse(entryDetails.MessageFields[1], out ransom))
                    {
                        // Error parsing to int
                        Globals_Server.logError("Could not parse ransom to uint in JEntry: " + jEntryID);
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                        return false;
                    }
                    else
                    {
                        // Check captive's head of household has the funds to release
                        if (captiveHeadOfHousehold.GetHomeFief().GetAvailableTreasury(false) >= ransom)
                        {
                            if (!captiveHeadOfHousehold.GetHomeFief().TreasuryTransfer(captor.GetHomeFief(), (Int32)ransom, out error))
                            {
                                return false;
                            }
                            else
                            {
                                // Release captive
                                captor.ReleaseCaptive(captive);
                                replied = true;
                                Globals_Game.UpdatePlayer(captor.playerID, DisplayMessages.RansomPaid, new string[] { captive.firstName + " " + captive.familyName });
                                return true;
                            }
                        }
                        else
                        {
                            // Insufficient funds
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericInsufficientFunds;
                            return false;
                        }
                    }
                }
                // If not paying ransom, inform captor
                else
                {
                    // Create journal entry and update captor
                    string[] newPersonae = new string[]{captive.charID+"|Captive",captor.charID+"|Captor",captiveHeadOfHousehold.charID+"|HeadOfCaptiveFamily"};
                    ProtoMessage deniedMessage = new ProtoMessage();
                    deniedMessage.ResponseType = DisplayMessages.RansonDenied;
                    deniedMessage.MessageFields=new string[]{captive.firstName+ " "+captive.familyName,captor.firstName+" " +captor.familyName, captiveHeadOfHousehold.firstName+ " " +captiveHeadOfHousehold.familyName};
                    Globals_Game.UpdatePlayer(captor.playerID, deniedMessage);
                    JournalEntry ransomDenied= new JournalEntry(Globals_Game.GetNextJournalEntryID(),Globals_Game.clock.currentYear,Globals_Game.clock.currentSeason,newPersonae,"ransomDenied",deniedMessage);
                    Globals_Game.AddPastEvent(ransomDenied);
                    replied = true;
                    return true;
                }
            }
            else
            {
                // Not a ransom
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.EntryNotRansom;
                return false;
            }
        }

    }
}
