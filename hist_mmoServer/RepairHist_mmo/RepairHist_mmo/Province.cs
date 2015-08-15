using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on province
    /// </summary>
    [Serializable()]
    public class Province : Place, ISerializable
    {
        /// <summary>
        /// Holds province tax rate
        /// </summary>
        public Double taxRate { get; set; }
        /// <summary>
        /// Holds province kingdom object
        /// </summary>
        public Kingdom kingdom { get; set; }

        /// <summary>
        /// Constructor for Province
        /// </summary>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">Province's Kingdom object</param>
        public Province(String id, String nam, Double otax, String tiHo = null, PlayerCharacter own = null, Kingdom king = null, Rank r = null)
            : base(id, nam, tiHo, own, r)
        {
            // VALIDATION

            // OTAX
            if (!Utility_Methods.ValidatePercentage(otax))
            {
                throw new InvalidDataException("Province taxrate must be a double between 0 and 100");
            }

            this.taxRate = otax;
            this.kingdom = king;
        }

        /// <summary>
        /// Constructor for Province taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Province()
		{
		}

		/// <summary>
        /// Constructor for Province using Province_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ps">Province_Serialised object to use as source</param>
		public Province(Province_Serialised ps)
			: base(ps: ps)
		{
			this.taxRate = ps.taxRate;
            // kingdom to be inserted later
            this.kingdom = null;
        }

        /// <summary>
        /// Processes functions involved in lodging a new ownership challenge
        /// </summary>
        /// <param name="challenger">PlayerCharacter challenging for ownership</param>
        public ProtoMessage LodgeOwnershipChallenge(PlayerCharacter challenger)
        {
            ProtoMessage result = new ProtoMessage();
            bool proceed = true;

            // ensure aren't current owner
            if (challenger == this.owner)
            {
                proceed = false;
                result.ResponseType = DisplayMessages.ProvinceAlreadyOwn;
            }

            else
            {
                // create and send new OwnershipChallenge
                OwnershipChallenge newChallenge = new OwnershipChallenge(Globals_Game.GetNextOwnChallengeID(), challenger.charID, "province", this.id);
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
                string currentOwnerEntry = currentOwner.charID + "|owner";
                string challengerEntry = challenger.charID + "|challenger";
                string[] entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                // entry type
                string entryType = "ownershipChallenge_new";

                // journal entry description
                string[] fields = new string[4];
                fields[0] = this.name;
                fields[1] = this.id;
                fields[2] = challenger.firstName + " " + challenger.familyName;
                fields[3] = currentOwner.firstName + " " + currentOwner.familyName;

                ProtoMessage ownershipChallenge = new ProtoMessage();
                ownershipChallenge.MessageFields = fields;
                ownershipChallenge.ResponseType = DisplayMessages.ProvinceOwnershipChallenge;
                // create and send a proposal (journal entry)
                JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType,ownershipChallenge, loc: entryLoc);
                Globals_Game.AddPastEvent(myEntry);
            }
            return result;
        }

        /// <summary>
        /// Adjusts province tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void AdjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
            }
            else if (tx < 0)
            {
                tx = 0;
            }

            this.taxRate = tx;
        }

        /// <summary>
        /// Gets the province's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (this.kingdom != null)
            {
                thisKingdom = this.kingdom;
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the province's current kingdom (i.e. the kingdom of the current owner)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetCurrentKingdom()
        {
            Kingdom thisKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.owner.nationality)
                {
                    thisKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return thisKingdom;
        }

        /// <summary>
        /// Transfers ownership of the province to the specified PlayerCharacter
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

            // update province titleHolder property
            this.titleHolder = newOwner.charID;

            // remove from current owner's ownedProvinces
            this.owner.ownedProvinces.Remove(this);

            // add to newOwner's ownedProvinces
            newOwner.ownedProvinces.Add(this);

            // update province owner property
            this.owner = newOwner;
        }

        //temp for serializing to Client side Fief object
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("king", this.kingdom.id, typeof(string));
        }

        public Province(SerializationInfo info, StreamingContext context): base(info,context)
        {
            var tmpKing = info.GetString("king");
            this.kingdom = Globals_Game.kingdomMasterList[tmpKing];
        }

    }

	/// <summary>
	/// Class converting province data into serialised format suitable (JSON)
	/// </summary>
	public class Province_Serialised : Place_Serialised
	{
        /// <summary>
		/// Holds province tax rate
		/// </summary>
		public Double taxRate { get; set; }
        /// <summary>
        /// Holds province kingdom (ID)
        /// </summary>
        public String kingdom { get; set; }

		/// <summary>
        /// Constructor for Province_Serialised.
        /// For use when serialising.
        /// </summary>
		/// <param name="prov">Province object to be used as source</param>
		public Province_Serialised(Province prov)
            : base(p: prov)
		{
            this.taxRate = prov.taxRate;
            this.kingdom = prov.kingdom.id;
		}

        /// <summary>
        /// Constructor for Province_Serialised taking seperate values.
        /// For creating Province_Serialised from CSV file.
        /// </summary>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">string holding Province's Kingdom (id)</param>
        public Province_Serialised(String id, String nam, byte r, Double otax, String tiHo = null, string own = null, string king = null)
            : base(id, nam, r, own: own, tiHo: tiHo)
        {
            // VALIDATION

            // OTAX
            if (!Utility_Methods.ValidatePercentage(otax))
            {
                throw new InvalidDataException("Province_Serialised taxrate must be a double between 0 and 100");
            }

            // KING
            // trim and ensure is uppercase
            king = king.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(king))
            {
                throw new InvalidDataException("Province_Serialised kingdom ID must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            this.taxRate = otax;
            this.kingdom = king;
        }

        /// <summary>
        /// Constructor for Province_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Province_Serialised()
		{
		}
	}
}
