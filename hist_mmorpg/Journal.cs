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
        /// Holds PlayerCharacter who owns entry
        /// </summary>
        public PlayerCharacter playerChar { get; set; }
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
        /// Holds description of event
        /// </summary>
        public String description { get; set; }
        /// <summary>
        /// Indicates whether entry has been viewed
        /// </summary>
        public bool viewed { get; set; }

        /// <summary>
        /// Constructor for JournalEntry
        /// </summary>
        /// <param name="id">uint holding JournalEntry ID</param>
        /// <param name="yr">uint holding event year</param>
        /// <param name="seas">byte holding event season</param>
        /// <param name="pers">String[] holding main objects (IDs) associated with event and thier role</param>
        /// <param name="typ">String holding type of event</param>
        /// <param name="loc">String holding location of event (fiefID)</param>
        /// <param name="descr">String holding description of event</param>
        public JournalEntry(uint id, uint yr, byte seas, String[] pers, String typ, String loc = null, String descr = null)
        {
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
            if (!String.IsNullOrWhiteSpace(descr))
            {
                this.description = descr;
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

            // description
            if (!String.IsNullOrWhiteSpace(this.description))
            {
                entryText += "Description:\r\n" + this.description + "\r\n\r\n";
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
        /// Check to see if the JournalEntry is of interest to the player
        /// </summary>
        /// <returns>bool indicating whether the JournalEntry is of interest</returns>
        public bool CheckEventForInterest(PlayerCharacter playerChar)
        {
            bool isOfInterest = false;

            for (int i = 0; i < this.personae.Length; i++)
            {
                // get personae ID
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                if (thisPersonaeSplit[0].Equals(playerChar.charID)
                    || (thisPersonaeSplit[0].Equals("all")))
                {
                    isOfInterest = true;
                    break;
                }
            }

            return isOfInterest;
        }

        /// <summary>
        /// Check to see if the JournalEntry requires that the proposal reply controls be enabled
        /// </summary>
        /// <returns>bool indicating whether the controls be enabled</returns>
        public bool CheckForProposalControlsEnabled(PlayerCharacter playerChar)
        {
            bool controlsEnabled = false;

            // check if is a marriage proposal
            if (this.type.Equals("proposalMade"))
            {
                // check if have already replied
                if (!this.description.Contains("**"))
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
                                controlsEnabled = true;
                                break;
                            }
                        }
                    }
                }
            }

            return controlsEnabled;
        }

        /// <summary>
        /// Allows a character to reply to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether reply was processed successfully</returns>
        /// <param name="proposalAccepted">bool indicating whether proposal accepted</param>
        public bool ReplyToProposal(bool proposalAccepted)
        {
            bool success = true;

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
            string description = "On this day of Our Lord the proposed marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + bride.familyName + " has been ";
            if (proposalAccepted)
            {
                description += "ACCEPTED";
            }
            else
            {
                description += "REJECTED";
            }
            description += " by " + headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName + ".";
            if (proposalAccepted)
            {
                description += " Let the bells ring out in celebration!";
            }

            // create and send a proposal reply (journal entry)
            JournalEntry myProposalReply = new JournalEntry(replyID, year, season, myReplyPersonae, type, descr: description);
            success = Globals_Game.AddPastEvent(myProposalReply);

            if (success)
            {
                // mark proposal as replied
                this.description += "\r\n\r\n** You ";
                if (proposalAccepted)
                {
                    this.description += "ACCEPTED ";
                }
                else
                {
                    this.description += "REJECTED ";
                }
                this.description += "this proposal in " + Globals_Game.clock.seasons[season] + ", " + year;

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
            JournalEntry marriageEntry = new JournalEntry(replyID, year, season, marriagePersonae, type);
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

            // description
            string description = "On this day of Our Lord there took place a marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + groom.familyName + " (nee " + bride.familyName + ").";
            description += " Let the bells ring out in celebration!";

            // create and add a marriage entry to the pastEvents journal
            JournalEntry marriageEntry = new JournalEntry(marriageID, year, season, marriagePersonae, type, descr: description);
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

    }
}
