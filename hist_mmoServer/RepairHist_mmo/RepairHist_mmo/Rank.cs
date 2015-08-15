using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on rank and title
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Holds rank ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }

        /// <summary>
        /// Constructor for Rank
        /// </summary>
        /// <param name="id">byte holding rank ID</param>
        /// <param name="ti">TitleName[] holding title name in various languages</param>
        /// <param name="stat">byte holding base stature for rank</param>
        public Rank(byte id, TitleName[] ti, byte stat)
        {
            // VALIDATION

            // STATURE
            if (stat < 1)
            {
                stat = 1;
            }
            else if (stat > 6)
            {
                stat = 6;
            }

            this.id = id;
            this.title = ti;
            this.stature = stat;

        }

        /// <summary>
        /// Constructor for Rank using Position_Serialised object
        /// For use when de-serialising
        /// </summary>
        /// <param name="ps">Position_Serialised object to use as source</param>
        public Rank(Position_Serialised ps)
        {
            this.id = ps.id;
            this.title = ps.title;
            this.stature = ps.stature;
        }

        /// <summary>
        /// Constructor for Rank taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Rank()
        {
        }

        /// <summary>
        /// Gets the correct name for the rank depending on the specified Language
        /// </summary>
        /// <returns>string containing the name</returns>
        /// <param name="l">The Language to be used</param>
        public string GetName(Language l)
        {
            string rankName = null;
            bool nameFound = false;

            // iterate through TitleNames and get correct name
            foreach (TitleName titleName in this.title)
            {
                if (titleName.langID == l.id)
                {
                    rankName = titleName.name;
                    nameFound = true;
                    break;
                }
            }

            // if no name found for specified language
            if (!nameFound)
            {
                // iterate through TitleNames and get generic name
                foreach (TitleName titleName in this.title)
                {
                    if ((titleName.langID.Equals("generic")) || (titleName.langID.Contains("lang_E")))
                    {
                        rankName = titleName.name;
                        nameFound = true;
                        break;
                    }
                }
            }

            // if still no name found
            if (!nameFound)
            {
                // get first name
                rankName = this.title[0].name;
            }

            return rankName;
        }
			

    }

    /// <summary>
    /// Class storing data on positions of power
    /// </summary>
    public class Position : Rank
    {
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds nationality associated with the position
        /// </summary>
        public Nationality nationality { get; set; }

        /// <summary>
        /// Constructor for Position
        /// </summary>
        /// <param name="holder">string holding ID of the office holder</param>
        /// <param name="nat">Nationality associated with the position</param>
        public Position(byte id, TitleName[] ti, byte stat, string holder, Nationality nat)
            : base(id, ti, stat)
        {
            // VALIDATION

            // HOLDER
            if (!String.IsNullOrWhiteSpace(holder))
            {
                // trim and ensure 1st is uppercase
                holder = Utility_Methods.FirstCharToUpper(holder.Trim());

                if (!Utility_Methods.ValidateCharacterID(holder))
                {
                    throw new InvalidDataException("Position officeHolder id must have the format 'Char_' followed by some numbers");
                }
            }

            this.officeHolder = holder;
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Position using Position_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ps">Position_Serialised object to use as source</param>
        public Position(Position_Serialised ps)
            : base(ps: ps)
        {
            this.officeHolder = ps.officeHolder;
            // nationality to be inserted later
            this.nationality = null;
        }

        /// <summary>
        /// Constructor for Position taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// Inserts the supplied PlayerCharacter's ID into the Position's officeHolder variable 
        /// </summary>
        /// <param name="newPositionHolder">PlayerCharacter being assigned to the Position</param>
        public void BestowPosition(PlayerCharacter newPositionHolder)
        {
            PlayerCharacter oldPositionHolder = null;

            // remove existing holder if necessary
            if (!String.IsNullOrWhiteSpace(this.officeHolder))
            {
                // get current holder
                if (Globals_Game.pcMasterList.ContainsKey(this.officeHolder))
                {
                    oldPositionHolder = Globals_Game.pcMasterList[this.officeHolder];
                }

                // remove from position
                this.RemoveFromOffice(oldPositionHolder);
            }

            // assign position
            this.officeHolder = newPositionHolder.charID;

            // update stature
            newPositionHolder.AdjustStatureModifier(this.stature);

            // CREATE JOURNAL ENTRY
            // get interested parties
            bool success = true;
            PlayerCharacter king = this.GetKingdom().owner;

            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(king.charID + "|king");
            tempPersonae.Add(newPositionHolder.charID + "|newPositionHolder");
            if (oldPositionHolder != null)
            {
                tempPersonae.Add(oldPositionHolder.charID + "|oldPositionHolder");
            }
            string[] thisPersonae = tempPersonae.ToArray();

            // type
            string type = "grantPosition";

            // description
            
            String[] fields = new string[] { this.title[0].name, king.firstName + " " + king.familyName, newPositionHolder.firstName + " " + newPositionHolder.familyName, "" };
            if (oldPositionHolder != null)
            {
                fields[3] = "; This has necessitated the removal of " + oldPositionHolder.firstName + " " + oldPositionHolder.familyName + " from the position";
            }

            ProtoMessage bestowPosition = new ProtoMessage();
            bestowPosition.MessageFields = fields;
            bestowPosition.ResponseType = DisplayMessages.RankTitleTransfer;
            // create and add a journal entry to the pastEvents journal
            JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type,bestowPosition);
            success = Globals_Game.AddPastEvent(thisEntry);
        }

        /// <summary>
        /// Removes the supplied PlayerCharacter's ID from the Position's officeHolder variable 
        /// </summary>
        /// <param name="pc">PlayerCharacter being removed from the Position</param>
        public void RemoveFromOffice(PlayerCharacter pc)
        {
            // remove from position
            this.officeHolder = null;

            // update stature
            pc.AdjustStatureModifier(this.stature * -1);
        }

        /// <summary>
        /// Gets the Kingdom associated with the position 
        /// </summary>
        /// <returns>The Kingdom</returns>
        public Kingdom GetKingdom()
        {
            Kingdom thisKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.nationality)
                {
                    thisKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the position's current office holder
        /// </summary>
        /// <returns>The office holder (PlayerCharacter)</returns>
        public PlayerCharacter GetOfficeHolder()
        {
            PlayerCharacter holder = null;

            if (Globals_Game.pcMasterList.ContainsKey(this.officeHolder))
            {
                holder = Globals_Game.pcMasterList[this.officeHolder];
            }

            return holder;
        }
			
    }

    /// <summary>
    /// Class used to convert Position to/from serialised format (JSON)
    /// </summary>
    public class Position_Serialised
    {
        /// <summary>
        /// Holds ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds ID of Nationality associated with the position
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Constructor for Position_Serialised
        /// </summary>
        /// <param name="pos">Position object to use as source</param>
        public Position_Serialised(Position pos)
        {
            this.id = pos.id;
            this.title = pos.title;
            this.stature = pos.stature;
            this.officeHolder = pos.officeHolder;
            this.nationality = pos.nationality.natID;
        }

        /// <summary>
        /// Constructor for Position_Serialised taking seperate values.
        /// For creating Position_Serialised from CSV file.
        /// </summary>
        /// <param name="id">byte holding Position ID</param>
        /// <param name="ti">title name in various languages</param>
        /// <param name="stat">byte holding stature for this position</param>
        /// <param name="holder">string ID of the office holder</param>
        /// <param name="nat">string holding ID of Nationality associated with the position</param>
        public Position_Serialised(byte id, TitleName[] ti, byte stat, string holder, string nat)
        {
            // VALIDATION

            // STAT
            if (stat < 1)
            {
                stat = 1;
            }
            else if (stat > 3)
            {
                stat = 3;
            }

            // HOLDER
            if (!String.IsNullOrWhiteSpace(holder))
            {
                // trim and ensure 1st is uppercase
                holder = Utility_Methods.FirstCharToUpper(holder.Trim());

                if (!Utility_Methods.ValidateCharacterID(holder))
                {
                    throw new InvalidDataException("Position_Serialised officeHolder id must have the format 'Char_' followed by some numbers");
                }
            }

            // NAT
            // trim and ensure 1st is uppercase
            nat = Utility_Methods.FirstCharToUpper(nat.Trim());

            if (!Utility_Methods.ValidateNationalityID(nat))
            {
                throw new InvalidDataException("Position_Serialised nationality ID must be 1-3 characters long, and consist entirely of letters");
            }

            this.id = id;
            this.title = ti;
            this.stature = stat;
            this.officeHolder = holder;
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Position_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Position_Serialised()
        {
        }
    }

    /// <summary>
    /// Struct storing data on title name
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public struct TitleName
    {
        /// <summary>
        /// Holds Language ID or "generic"
        /// </summary>
        public string langID;
        /// <summary>
        /// Holds title name associated with specific language
        /// </summary>
        public string name;

        /// <summary>
        /// Constructor for TitleName
        /// </summary>
        /// <param name="lang">string holding Language ID</param>
        /// <param name="nam">string holding title name associated with specific language</param>
        public TitleName(string lang, string nam)
        {
            // VALIDATION

            // LANG
            // trim
            lang = lang.Trim();

            if ((!Utility_Methods.ValidateLanguageID(lang)) && (!lang.Equals("generic")))
            {
                throw new InvalidDataException("TitleName langID must either be 'generic' or have the format 'lang_' followed by 1-2 letters, ending in 1-2 numbers");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("TitleName name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            this.langID = lang;
            this.name = nam;
        }
			
    }
}
