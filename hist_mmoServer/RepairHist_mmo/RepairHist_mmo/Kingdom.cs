using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on kingdom
    /// </summary>
    public class Kingdom : Place
    {
        /// <summary>
        /// Holds Kingdom nationality
        /// </summary>
        public Nationality nationality { get; set; }

        /// <summary>
        /// Constructor for Kingdom
        /// </summary>
        /// <param name="nat">Kingdom's Nationality object</param>
        public Kingdom(String id, String nam, Nationality nat, String tiHo = null, PlayerCharacter own = null, Rank r = null)
        : base(id, nam, tiHo, own, r)
        {
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Kingdom taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Kingdom()
		{
		}

		/// <summary>
        /// Constructor for Kingdom using Kingdom_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ks">Kingdom_Serialised object to use as source</param>
        public Kingdom(Kingdom_Serialised ks)
			: base(ks: ks)
		{
            // nationality to be inserted later
            this.nationality = null;
        }

        /// <summary>
        /// Processes functions involved in lodging a new ownership (and kingship) challenge
        /// Returns ProtoMessage in case of error
        /// </summary>
        public ProtoMessage LodgeOwnershipChallenge(PlayerCharacter challenger)
        {
            bool proceed = true;
            ProtoMessage result = null;
            // ensure aren't current owner
            if (challenger == this.owner)
            {
                result = new ProtoMessage();
                result.ResponseType = DisplayMessages.KingdomAlreadyKing;
            }

            else
            {
                // create and send new OwnershipChallenge
                OwnershipChallenge newChallenge = new OwnershipChallenge(Globals_Game.GetNextOwnChallengeID(), challenger.charID, "kingdom", this.id);
                proceed = Globals_Game.AddOwnershipChallenge(newChallenge,out result);
            }

            if (proceed)
            {
                // create and send journal entry
                // get interested parties
                PlayerCharacter currentOwner = this.owner;

                // ID
                uint entryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // location
                string entryLoc = this.id;

                // journal entry personae
                string allEntry = "all|all";
                string currentOwnerEntry = currentOwner.charID + "|king";
                string challengerEntry = challenger.charID + "|pretender";
                string[] entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                // entry type
                string entryType = "depose_new";

                // journal entry description
                string[] fields = new string[4];
                fields[0] = this.name;
                fields[1] = this.id;
                fields[2] = challenger.firstName + " " + challenger.familyName;
                fields[3] = currentOwner.firstName + " " + currentOwner.familyName;

                ProtoMessage ownershipChallenge = new ProtoMessage();
                ownershipChallenge.MessageFields = fields;
                ownershipChallenge.ResponseType = DisplayMessages.KingdomOwnershipChallenge;
                // create and send a proposal (journal entry)
                JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType,ownershipChallenge, loc: entryLoc);
                Globals_Game.AddPastEvent(myEntry);
            }
            return result;
        }

        /// <summary>
        /// Transfers ownership of the kingdom (and the kingship) to the specified PlayerCharacter
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        public void TransferOwnership(PlayerCharacter newOwner)
        {
            // get current title holder
            Character titleHolder = this.GetTitleHolder();

            // remove from current title holder's titles
            titleHolder.myTitles.Remove(this.id);

            // add to newOwner's titles
            newOwner.myTitles.Add(this.id);

            // update kingdom titleHolder property
            this.titleHolder = newOwner.charID;

            // update Globals_Game king variable
            if (Globals_Game.kingOne == this.owner)
            {
                Globals_Game.kingOne = newOwner;
            }
            else if (Globals_Game.kingTwo == this.owner)
            {
                Globals_Game.kingTwo = newOwner;
            }

            // update kingdom owner property
            this.owner = newOwner;
        }
    }

    /// <summary>
    /// Class converting kingdom data into serialised format (JSON)
    /// </summary>
    public class Kingdom_Serialised : Place_Serialised
    {
        /// <summary>
        /// Holds nationality (ID)
        /// </summary>
        public String nationality { get; set; }

		/// <summary>
        /// Constructor for Kingdom_Serialised.
        /// For use when serialising.
        /// </summary>
        /// <param name="king">Kingdom object to be used as source</param>
        public Kingdom_Serialised(Kingdom king)
            : base(k: king)
		{
            this.nationality = king.nationality.natID;
		}

        /// <summary>
        /// Constructor for Kingdom_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Kingdom_Serialised()
		{
		}

        /// <summary>
        /// Constructor for Kingdom_Serialised taking seperate values.
        /// For creating Kingdom_Serialised from CSV file.
        /// </summary>
        /// <param name="nat">Kingdom's Nationality object</param>
        public Kingdom_Serialised(String id, String nam, byte r, string nat, String tiHo = null, string own = null)
            : base(id, nam, r, own: own, tiHo: tiHo)
        {
            // VALIDATION

            // NAT
            // trim and ensure 1st is uppercase
            nat = Utility_Methods.FirstCharToUpper(nat.Trim());

            if (!Utility_Methods.ValidateNationalityID(nat))
            {
                throw new InvalidDataException("Kingdom_Serialised nationality ID must be 1-3 characters long, and consist entirely of letters");
            }

            this.nationality = nat;
        }
    }

}
