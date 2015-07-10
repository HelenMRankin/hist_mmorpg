using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
    public abstract class Place
    {
        /// <summary>
        /// Holds place ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds place name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds place owner (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public String titleHolder { get; set; }
        /// <summary>
        /// Holds place rank (Rank object)
        /// </summary>
        public Rank rank { get; set; }

        /// <summary>
        /// Constructor for Place
        /// </summary>
        /// <param name="id">String holding place ID</param>
        /// <param name="nam">String holding place name</param>
        /// <param name="tiHo">String holding place title holder (charID)</param>
        /// <param name="own">Place owner (PlayerCharacter)</param>
        /// <param name="rnk">Place rank (Rank object)</param>
        public Place(String id, String nam, String tiHo, PlayerCharacter own, Rank r)
        {
            // VALIDATION

            // ID
            // trim and ensure is uppercase
            id = id.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(id))
            {
                throw new InvalidDataException("Place id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("Place name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // TIHO
            if (!String.IsNullOrWhiteSpace(tiHo))
            {
                // trim and ensure 1st is uppercase
                tiHo = Utility_Methods.FirstCharToUpper(tiHo.Trim());

                if (!Utility_Methods.ValidateCharacterID(tiHo))
                {
                    throw new InvalidDataException("Place titleHolder must have the format 'Char_' followed by some numbers");
                }
            }

            this.id = id;
            this.name = nam;
            this.owner = own;
            this.titleHolder = tiHo;
            this.rank = r;

        }

		/// <summary>
        /// Constructor for Place using Fief_Serialised, Province_Serialised or Kingdom_Serialised object.
		/// For use when de-serialising.
		/// </summary>
        /// <param name="fs">Fief_Serialised object to use as source</param>
        /// <param name="ps">Province_Serialised object to use as source</param>
        /// <param name="ks">Kingdom_Serialised object to use as source</param>
		public Place(Fief_Serialised fs = null, Province_Serialised ps = null, Kingdom_Serialised ks = null)
		{
			Place_Serialised placeToUse = null;
			if (fs != null)
			{
				placeToUse = fs;
			}
			else if (ps != null)
			{
				placeToUse = ps;
			}
			else if (ks != null)
			{
				placeToUse = ks;
			}

			if (placeToUse != null)
			{
				this.id = placeToUse.id;
				this.name = placeToUse.name;
				// owner to be inserted later
				this.owner = null;
				this.titleHolder = placeToUse.titleHolder;
				// rank to be inserted later
				this.rank = null;
			}

		}

        /// <summary>
        /// Constructor for Place taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Place()
        {
        }

        /// <summary>
        /// Gets the place's title holder
        /// </summary>
        /// <returns>The title holder</returns>
        public Character GetTitleHolder()
        {
            Character myTitleHolder = null;

            if (!String.IsNullOrWhiteSpace(this.titleHolder))
            {
                // get title holder from appropriate master list
                if (Globals_Game.npcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = Globals_Game.npcMasterList[this.titleHolder];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = Globals_Game.pcMasterList[this.titleHolder];
                }
            }

            return myTitleHolder;
        }

        // Serialise Place for client
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            // Use the AddValue method to specify serialized values.
            info.AddValue("id", this.id, typeof(string));
            info.AddValue("nam", this.name, typeof(string));
            info.AddValue("rank", this.rank.id, typeof(byte));
        }

        public Place(SerializationInfo info, StreamingContext context)
        {
            this.id = info.GetString("id");
            this.name = info.GetString("nam");
            var tmpRank = info.GetByte("rank");
            this.rank = Globals_Game.rankMasterList[tmpRank];
        }

    }

    /// <summary>
    /// Class converting Place data into serialised format (JSON)
    /// </summary>
    public abstract class Place_Serialised
    {
        /// <summary>
        /// Holds place ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Holds place name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Holds place owner (id)
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// Holds place title holder (id)
        /// </summary>
        public string titleHolder { get; set; }
        /// <summary>
        /// Holds place rank (id)
        /// </summary>
        public byte rank { get; set; }

        /// <summary>
        /// Constructor for Place_Serialised.
        /// For use when serialising.
        /// </summary>
        /// <param name="k">Kingdom object to be used as source</param>
        public Place_Serialised(Kingdom k = null, Province p = null, Fief f = null)
        {
            Place placeToUse = null;

            if (k != null)
            {
                placeToUse = k;
            }
            else if (p != null)
            {
                placeToUse = p;
            }
            else if (f != null)
            {
                placeToUse = f;
            }

            if (placeToUse != null)
            {
                this.id = placeToUse.id;
                this.name = placeToUse.name;
                this.owner = placeToUse.owner.charID;
                this.titleHolder = placeToUse.titleHolder;
                this.rank = placeToUse.rank.id;
            }
        }

        /// <summary>
        /// Constructor for Place_Serialised taking seperate values.
        /// For creating Place_Serialised from CSV file.
        /// </summary>
        /// <param name="id">String holding place ID</param>
        /// <param name="nam">String holding place name</param>
        /// <param name="own">String holding Place owner (ID)</param>
        /// <param name="tiHo">String holding place title holder (charID)</param>
        /// <param name="rnk">String holding Place rank (ID)</param>
        public Place_Serialised(String id, String nam, byte r, String tiHo = null, string own = null)
        {
            // VALIDATION

            // ID
            // trim and ensure is uppercase
            id = id.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(id))
            {
                throw new InvalidDataException("Place_Serialised id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("Place_Serialised name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // TIHO
            if (!String.IsNullOrWhiteSpace(tiHo))
            {
                // trim and ensure 1st is uppercase
                tiHo = Utility_Methods.FirstCharToUpper(tiHo.Trim());

                if (!Utility_Methods.ValidateCharacterID(tiHo))
                {
                    throw new InvalidDataException("Place_Serialised titleHolder must have the format 'Char_' followed by some numbers");
                }
            }

            // OWNER
            if (!String.IsNullOrWhiteSpace(owner))
            {
                // trim and ensure 1st is uppercase
                owner = Utility_Methods.FirstCharToUpper(owner.Trim());

                if (!Utility_Methods.ValidateCharacterID(owner))
                {
                    throw new InvalidDataException("Place_Serialised owner must have the format 'Char_' followed by some numbers");
                }
            }

            this.id = id;
            this.name = nam;
            this.owner = own;
            this.titleHolder = tiHo;
            this.rank = r;

        }
    }

}
