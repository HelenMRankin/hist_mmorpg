using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on character (PC and NPC)
    /// </summary>
    public abstract class Character : IEquatable<Character>
    {

        /// <summary>
        /// Holds character ID
        /// </summary>
        public string charID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
		public String firstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public String familyName { get; set; }
        /// <summary>
        /// Tuple holding character's year and season of birth
        /// </summary>
        public Tuple<uint, byte> birthDate { get; set; }
        /// <summary>
        /// Holds if character male
        /// </summary>
        public bool isMale { get; set; }
        /// <summary>
        /// Holds character nationality
        /// </summary>
        public Nationality nationality { get; set; }
        /// <summary>
        /// bool indicating whether character is alive
        /// </summary>
        public bool isAlive { get; set; }
        /// <summary>
        /// Holds character maximum health
        /// </summary>
        public Double maxHealth { get; set; }
        /// <summary>
        /// Holds character virility
        /// </summary>
        public Double virility { get; set; }
        /// <summary>
        /// Queue of Fiefs to auto-travel to
        /// </summary>
		public Queue<Fief> goTo = new Queue<Fief> ();
        /// <summary>
        /// Holds character's language and dialect
        /// </summary>
        public Language language { get; set; }
        /// <summary>
        /// Holds character's remaining days in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds modifier to character's base stature
        /// </summary>
        public Double statureModifier { get; set; }
        /// <summary>
        /// Holds character's management rating
        /// </summary>
        public Double management { get; set; }
        /// <summary>
        /// Holds character's combat rating
        /// </summary>
        public Double combat { get; set; }
        /// <summary>
        /// Array holding character's traits
        /// </summary>
        public Tuple<Trait, int>[] traits { get; set; }
        /// <summary>
        /// bool indicating if character is in the keep
        /// </summary>
        public bool inKeep { get; set; }
        /// <summary>
        /// Holds character pregnancy status
        /// </summary>
        public bool isPregnant { get; set; }
        /// <summary>
        /// Holds charID of head of family with which character associated
        /// </summary>
        public String familyID { get; set; }
        /// <summary>
        /// Holds spouse (charID)
        /// </summary>
        public String spouse { get; set; }
        /// <summary>
        /// Holds father (CharID)
        /// </summary>
        public String father { get; set; }
        /// <summary>
        /// Holds mother (CharID)
        /// </summary>
        public String mother { get; set; }
        /// <summary>
        /// Hold fiancee (charID)
        /// </summary>
        public string fiancee { get; set; }
        /// <summary>
        /// Holds current location (Fief object)
        /// </summary>
        public Fief location { get; set; }
        /// <summary>
        /// Holds character's titles (IDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }
        /// <summary>
        /// Holds ailments effecting character's health
        /// </summary>
        public Dictionary<string, Ailment> ailments = new Dictionary<string, Ailment>();
        /// <summary>
        /// Holds the characterID of captor, if being held captive
        /// </summary>
        public string captorID { get; set; }
        /// <summary>
        /// Holds the journal entry id of any ransom sent
        /// </summary>
        public string ransomDemand { get; set; }

#if DEBUG
        /// <summary>
        /// Fix the success chance- use -1 to calculate success based on traits
        /// </summary>
        public double fixedSuccessChance { get;set; }
#endif
        /**************LOCKS**************/
        protected object entourageLock = new Object();
        /// <summary>
        /// Constructor for Character
        /// </summary>
        /// <param name="id">string holding character ID</param>
        /// <param name="firstNam">String holding character's first name</param>
        /// <param name="famNam">String holding character's family name</param>
        /// <param name="dob">Tuple<uint, byte> holding character's year and season of birth</param>
        /// <param name="isM">bool holding if character male</param>
        /// <param name="nat">Character's Nationality object</param>
        /// <param name="alive">bool indicating whether character is alive</param>
        /// <param name="mxHea">Double holding character maximum health</param>
        /// <param name="vir">Double holding character virility rating</param>
        /// <param name="go">Queue<Fief> of Fiefs to auto-travel to</param>
        /// <param name="lang">Language object holding character's language</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">Double holding character stature rating</param>
        /// <param name="mngmnt">Double holding character management rating</param>
        /// <param name="cbt">Double holding character combat rating</param>
        /// <param name="trt">Array containing character's traits</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famID">String holding charID of head of family with which character associated</param>
        /// <param name="sp">String holding spouse (charID)</param>
        /// <param name="fath">String holding father (charID)</param>
        /// <param name="moth">String holding mother (charID)</param>
        /// <param name="fia">Holds fiancee (charID)</param>
        /// <param name="loc">Fief holding current location</param>
        /// <param name="myTi">List holding character's titles (fiefIDs)</param>
        /// <param name="aID">String holding armyID of army character is leading</param>
        /// <param name="ails">Dictionary<string, Ailment> holding ailments effecting character's health</param>
        public Character(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, Nationality nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Language lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Trait, int>[] trt, bool inK, bool preg,
            String famID, String sp, String fath, String moth, List<String> myTi, string fia, Dictionary<string, Ailment> ails = null, Fief loc = null, String aID = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.FirstCharToUpper(id.Trim());

            if (!Utility_Methods.ValidateCharacterID(id))
            {
                throw new InvalidDataException("Character id must have the format 'Char_' followed by some numbers");
            }

            // FIRSTNAM
            // trim and ensure 1st is uppercase
            firstNam = Utility_Methods.FirstCharToUpper(firstNam.Trim());

            if (!Utility_Methods.ValidateName(firstNam))
            {
				throw new InvalidDataException("Character firstname must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // FAMNAM
            // trim
            famNam = famNam.Trim();

            if (!Utility_Methods.ValidateName(famNam))
            {
                throw new InvalidDataException("Character family name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // DOB
            if (!Utility_Methods.ValidateSeason(dob.Item2))
            {
                throw new InvalidDataException("Character date-of-birth season must be a byte between 0-3");
            }

            // MXHEA
            if (!Utility_Methods.ValidateCharacterStat(mxHea))
            {
                throw new InvalidDataException("Character maxHealth must be a double between 1-9");
            }

            // VIR
            if (!Utility_Methods.ValidateCharacterStat(vir))
            {
                throw new InvalidDataException("Character virility must be a double between 1-9");
            }

            // DAYS
            if (!Utility_Methods.ValidateDays(day))
            {
                throw new InvalidDataException("Character days must be a double between 0-109");
            }

            // STAT
            if (!Utility_Methods.ValidateCharacterStat(stat, 0))
            {
                throw new InvalidDataException("Character stature must be a double between 0-9");
            }

            // MNGMNT
            if (!Utility_Methods.ValidateCharacterStat(mngmnt))
            {
                throw new InvalidDataException("Character management must be a double between 1-9");
            }

            // CBT
            if (!Utility_Methods.ValidateCharacterStat(cbt))
            {
                throw new InvalidDataException("Character combat must be a double between 1-9");
            }

            // SKL
            for (int i = 0; i < trt.Length; i++)
            {
                if (!Utility_Methods.ValidateCharacterStat(Convert.ToDouble(trt[i].Item2)))
                {
                    throw new InvalidDataException("Character trait level must be an integer between 1-9");
                }
            }

            // PREG
            if (preg)
            {
                if (isM)
                {
                    throw new InvalidDataException("Character cannot be pregnant if is male");
                }
            }

            // FAMID
            if (!String.IsNullOrWhiteSpace(famID))
            {
                // trim and ensure 1st is uppercase
                famID = Utility_Methods.FirstCharToUpper(famID.Trim());

                if (!Utility_Methods.ValidateCharacterID(famID))
                {
                    throw new InvalidDataException("Character family id must have the format 'Char_' followed by some numbers");
                }
            }

            // SP
            if (!String.IsNullOrWhiteSpace(sp))
            {
                // trim and ensure 1st is uppercase
                sp = Utility_Methods.FirstCharToUpper(sp.Trim());

                if (!Utility_Methods.ValidateCharacterID(sp))
                {
                    throw new InvalidDataException("Character spouse id must have the format 'Char_' followed by some numbers");
                }
            }

            // FATH
            if (!String.IsNullOrWhiteSpace(fath))
            {
                // trim and ensure 1st is uppercase
                fath = Utility_Methods.FirstCharToUpper(fath.Trim());

                if (!Utility_Methods.ValidateCharacterID(fath))
                {
                    throw new InvalidDataException("Character father id must have the format 'Char_' followed by some numbers");
                }
            }

            // MOTH
            if (!String.IsNullOrWhiteSpace(moth))
            {
                // trim and ensure 1st is uppercase
                moth = Utility_Methods.FirstCharToUpper(moth.Trim());

                if (!Utility_Methods.ValidateCharacterID(moth))
                {
                    throw new InvalidDataException("Character mother id must have the format 'Char_' followed by some numbers");
                }
            }

            // MYTI
            for (int i = 0; i < myTi.Count; i++ )
            {
                // trim and ensure is uppercase
                myTi[i] = myTi[i].Trim().ToUpper();

                if (!Utility_Methods.ValidatePlaceID(myTi[i]))
                {
                    throw new InvalidDataException("All Character title IDs must be 5 characters long, start with a letter, and end in at least 2 numbers");
                }
            }

            // FIA
            if (!String.IsNullOrWhiteSpace(fia))
            {
                // trim and ensure 1st is uppercase
                fia = Utility_Methods.FirstCharToUpper(fia.Trim());

                if (!Utility_Methods.ValidateCharacterID(fia))
                {
                    throw new InvalidDataException("Character fiancee id must have the format 'Char_' followed by some numbers");
                }
            }

            // AILS
            if (ails != null)
            {
                if (ails.Count > 0)
                {
                    string[] myAils = new string[ails.Count];
                    ails.Keys.CopyTo(myAils, 0);
                    for (int i = 0; i < myAils.Length; i++)
                    {
                        // trim and ensure 1st is uppercase
                        myAils[i] = Utility_Methods.FirstCharToUpper(myAils[i].Trim());

                        if (!Utility_Methods.ValidateAilmentID(myAils[i]))
                        {
                            throw new InvalidDataException("All IDs in Character ailments must have the format 'Ail_' followed by some numbers");
                        }
                    }
                }
            }

            // AID
            if (!String.IsNullOrWhiteSpace(aID))
            {
                // trim and ensure 1st is uppercase
                aID = Utility_Methods.FirstCharToUpper(aID.Trim());

                if (!Utility_Methods.ValidateArmyID(aID))
                {
                    throw new InvalidDataException("Character army id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            this.charID = id;
            this.firstName = firstNam;
            this.familyName = famNam;
            this.birthDate = dob;
            this.isMale = isM;
            this.nationality = nat;
            this.isAlive = alive;
            this.maxHealth = mxHea;
            this.virility = vir;
            this.goTo = go;
            this.language = lang;
            this.days = day;
            this.statureModifier = stat;
            this.management = mngmnt;
            this.combat = cbt;
            this.traits = trt;
            this.inKeep = inK;
            this.isPregnant = preg;
			this.location = loc;
            if (loc != null)
            {
                loc.charactersInFief.Add(this);
            }
            this.spouse = sp;
            this.father = fath;
            this.mother = moth;
            this.familyID = famID;
            this.myTitles = myTi;
            this.armyID = aID;
            if (ails != null)
            {
                this.ailments = ails;
            }
            this.fiancee = fia;
#if DEBUG
            // Default = trait-influenced success chance
            fixedSuccessChance = -1;
#endif
        }

		/// <summary>
        /// Constructor for Character using PlayerCharacter_Serialised or NonPlayerCharacter_Serialised object.
        /// For use when de-serialising.
		/// </summary>
        /// <param name="pcs">PlayerCharacter_Serialised object to use as source</param>
        /// <param name="npcs">NonPlayerCharacter_Serialised object to use as source</param>
		public Character(PlayerCharacter_Serialised pcs = null, NonPlayerCharacter_Serialised npcs = null)
		{
			Character_Serialised charToUse = null;

			if (pcs != null)
			{
				charToUse = pcs;
			}
			else if (npcs != null)
			{
				charToUse = npcs;
			}

			if (charToUse != null)
			{
				this.charID = charToUse.charID;
				this.firstName = charToUse.firstName;
                this.familyName = charToUse.familyName;
                this.birthDate = charToUse.birthDate;
				this.isMale = charToUse.isMale;
				this.nationality = null;
                this.isAlive = charToUse.isAlive;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
                // create empty Queue, to be populated later
                this.goTo = new Queue<Fief>();
				this.language = null;
				this.days = charToUse.days;
				this.statureModifier = charToUse.statureModifier;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
                // create empty array, to be populated later
                this.traits = new Tuple<Trait, int>[charToUse.traits.Length];
				this.inKeep = charToUse.inKeep;
				this.isPregnant = charToUse.isPregnant;
                this.spouse = charToUse.spouse;
                this.father = charToUse.father;
                this.mother = charToUse.mother;
                this.familyID = charToUse.familyID;
                this.location = null;
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
                this.ailments = charToUse.ailments;
                this.fiancee = charToUse.fiancee;
                this.captorID = charToUse.captorID;
                this.ransomDemand = charToUse.ransom;
			}
		}

        /// <summary>
        /// Constructor for Character using NonPlayerCharacter object,
        /// for use when respawning deceased NPCs or promoting NPC to PC (after PC death)
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        /// <param name="circumstance">The circumstance - respawn or promotion</param>
        public Character(NonPlayerCharacter npc, string circumstance, List<string> pcTitles = null)
        {
            switch (circumstance)
            {
                case "respawn":
                    this.charID = Globals_Game.GetNextCharID();
                    this.birthDate = new Tuple<uint, byte>(Globals_Game.clock.currentYear - 20, Globals_Game.clock.currentSeason);
                    this.maxHealth = Globals_Game.myRand.Next(4, 10);
                    // vary main stats slightly (virility, management, combat)
                    this.virility = npc.virility + Globals_Game.myRand.Next(-1, 2);
                    if (this.virility < 1)
                    {
                        this.virility = 1;
                    }
                    if (this.virility > 9)
                    {
                        this.virility = 9;
                    }
                    this.management = npc.management + Globals_Game.myRand.Next(-1, 2);
                    if (this.management < 1)
                    {
                        this.management = 1;
                    }
                    if (this.management > 9)
                    {
                        this.management = 9;
                    }
                    this.combat = npc.combat + Globals_Game.myRand.Next(-1, 2);
                    if (this.combat < 1)
                    {
                        this.combat = 1;
                    }
                    if (this.combat > 9)
                    {
                        this.combat = 9;
                    }
                    this.goTo = new Queue<Fief>();
                    this.days = 90;
                    this.statureModifier = 0;
                    this.inKeep = false;
                    this.isPregnant = false;
                    this.spouse = null;
                    this.father = null;
                    this.mother = null;
                    this.familyID = null;
                    this.myTitles = new List<string>();
                    this.armyID = null;
                    this.ailments = new Dictionary<string, Ailment>();
                    this.fiancee = null;
                    this.location = npc.location;
                    break;
                case "promote":
                    this.charID = npc.charID;
                    this.birthDate = npc.birthDate;
                    this.maxHealth = npc.maxHealth;
                    this.virility = npc.virility;
                    this.management = npc.management;
                    this.combat = npc.combat;
                    this.goTo = npc.goTo;
                    this.days = npc.days;
                    this.statureModifier = npc.statureModifier;
                    this.inKeep = npc.inKeep;
                    this.isPregnant = npc.isPregnant;
                    this.spouse = npc.spouse;
                    this.father = npc.father;
                    this.mother = npc.mother;
                    this.familyID = npc.charID;
                    this.myTitles = npc.myTitles;
                    if (pcTitles != null)
                    {
                        foreach (string thisTitle in pcTitles)
                        {
							// add to myTitles
							this.myTitles.Add(thisTitle);

							// change titleHolder in Place
							Place thisPlace = null;
							if (Globals_Game.fiefMasterList.ContainsKey(thisTitle))
							{
								thisPlace = Globals_Game.fiefMasterList[thisTitle];
							}
							else if (Globals_Game.provinceMasterList.ContainsKey(thisTitle))
							{
								thisPlace = Globals_Game.provinceMasterList[thisTitle];
							}
							else if (Globals_Game.kingdomMasterList.ContainsKey(thisTitle))
							{
								thisPlace = Globals_Game.kingdomMasterList[thisTitle];
							}

							if (thisPlace != null)
							{
								thisPlace.titleHolder = this.charID;
							}
                        }
                    }
                    this.armyID = npc.armyID;
                    this.ailments = npc.ailments;
                    this.fiancee = npc.fiancee;
                    this.location = npc.location;
                    if (this.location != null)
                    {
                        this.location.charactersInFief.Remove(npc);
                        this.location.charactersInFief.Add(this);
                    }
                    break;
                default:
                    break;
            }

            this.firstName = npc.firstName;
            this.familyName = npc.familyName;
            this.isMale = npc.isMale;
            this.nationality = npc.nationality;
            this.isAlive = true;
            this.language = npc.language;
            this.traits = new Tuple<Trait, int>[npc.traits.Length];
            for (int i = 0; i < npc.traits.Length; i++)
            {
                this.traits[i] = npc.traits[i];
            }
        }

        /// <summary>
        /// Calculates character's age
        /// </summary>
        /// <returns>int containing character's age</returns>
        public int CalcAge()
        {
            int myAge = 0;

            // subtract year of birth from current year
            myAge = Convert.ToByte(Globals_Game.clock.currentYear - this.birthDate.Item1);

            // if current season < season of birth, subtract 1 from age (not reached birthday yet)
            if (Globals_Game.clock.currentSeason < this.birthDate.Item2)
            {
                myAge--;
            }

            return myAge;
        }

        /// <summary>
        /// Retrieves character's highest rank
        /// </summary>
        /// <returns>The highest rank</returns>
        public Rank GetHighestRank()
        {
            Rank highestRank = null;
            byte rankValue = 255;

            foreach (String placeID in this.myTitles)
            {
                // get place
                Place thisPlace = null;

                if (Globals_Game.fiefMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.fiefMasterList[placeID];
                }
                else if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.provinceMasterList[placeID];
                }
                else if (Globals_Game.kingdomMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.kingdomMasterList[placeID];
                }

                if (thisPlace != null)
                {
                    if (thisPlace.rank.id < rankValue)
                    {
                        // update highest rank value
                        rankValue = thisPlace.rank.id;

                        // update highest rank
                        highestRank = thisPlace.rank;
                    }
                }
            }

            return highestRank;
        }

        /// <summary>
        /// Retrieves character's highest ranking places
        /// </summary>
        /// <returns>List containing character's highest ranking places</returns>
        public List<Place> GetHighestRankPlace()
        {
            List<Place> highestPlaces = new List<Place>();

            byte highRankStature = 0;

            foreach (String placeID in this.myTitles)
            {
                // get place
                Place thisPlace = null;

                if (Globals_Game.fiefMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.fiefMasterList[placeID];
                }
                else if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.provinceMasterList[placeID];
                }
                else if (Globals_Game.kingdomMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.kingdomMasterList[placeID];
                }

                if (thisPlace != null)
                {
                    if (thisPlace.rank.stature > highRankStature)
                    {
                        // clear existing places
                        if (highestPlaces.Count > 0)
                        {
                            highestPlaces.Clear();
                        }

                        // update highest rank
                        highRankStature = thisPlace.rank.stature;

                        // add new place to list
                        highestPlaces.Add(thisPlace);
                    }
                }
            }

            return highestPlaces;
        }

       
        /// <summary>
        /// Calculates character's base or current stature
        /// </summary>
        /// <returns>Double containing character's base stature</returns>
        /// <param name="type">bool indicating whether to return current stature (or just base)</param>
        public Double CalculateStature(bool currentStature = true)
        {
            Double stature = 0;

            // get stature for character's highest rank
            List<Place> myHighestPlaces = this.GetHighestRankPlace();
            if (myHighestPlaces.Count > 0)
            {
                stature += myHighestPlaces[0].rank.stature;
            }

            // factor in age
            int age = this.CalcAge();
            if (age <= 10)
            {
                stature += 0;
            }
            else if ((age > 10) && (age < 21))
            {
                stature += 0.5;
            }
            else if (age < 31)
            {
                stature += 1;
            }
            else if (age < 41)
            {
                stature += 2;
            }
            else if (age < 51)
            {
                stature += 3;
            }
            else if (age < 61)
            {
                stature += 4;
            }
            else
            {
                stature += 5;
            }

            // factor in sex (it's a man's world)
            if (!this.isMale)
            {
                stature -= 6;
            }

            // factor in character's current statureModifier if required
            if (currentStature)
            {
                stature += this.statureModifier;
            }

            // ensure returned stature lies between 1-9
            if (stature > 9)
            {
                stature = 9;
            }
            else if (stature < 1)
            {
                stature = 1;
            }

            return stature;
        }

        /// <summary>
        /// Adjusts the character's stature modifier
        /// </summary>
        /// <param name="amountToAdd">The amount of stature to add (can be negative)</param>
        public void AdjustStatureModifier(double amountToAdd)
        {
            // check if statureModifier cap is in force
            if (Globals_Game.statureCapInForce)
            {
                // adjust amountToAdd if required
                if (this.CalculateStature() + amountToAdd > 9)
                {
                    amountToAdd = 9 - this.CalculateStature();
                }
                else if (this.CalculateStature() + amountToAdd < 1)
                {
                    amountToAdd = (this.CalculateStature() - 1) * -1;
                }
            }

            this.statureModifier += amountToAdd;
        }

        /// <summary>
        /// Calculates character's base or current health
        /// </summary>
        /// <returns>Double containing character's health</returns>
        /// <param name="currentHealth">bool indicating whether to return current health (rather than base health)</param>
        public double CalculateHealth(bool currentHealth = true)
        {

            double charHealth = 0;
            double ageModifier = 0;
            int age = this.CalcAge();
            // calculate health age modifier, based on age
            if (age < 1)
            {
                ageModifier = 0.25;
            }
            else if (age < 5)
            {
                ageModifier = 0.5;
            }
            else if (age < 10)
            {
                ageModifier = 0.8;
            }
            else if (age < 20)
            {
                ageModifier = 0.9;
            }
            else if (age < 35)
            {
                ageModifier = 1;
            }
            else if (age < 40)
            {
                ageModifier = 0.95;
            }
            else if (age < 45)
            {
                ageModifier = 0.9;
            }
            else if (age < 50)
            {
                ageModifier = 0.85;
            }
            else if (age < 55)
            {
                ageModifier = 0.75;
            }
            else if (age < 60)
            {
                ageModifier = 0.65;
            }
            else if (age < 70)
            {
                ageModifier = 0.55;
            }
            else
            {
                ageModifier = 0.35;
            }

            // calculate health based on maxHealth and health age modifier
            charHealth = (this.maxHealth * ageModifier);

            // factor in current health modifers if appropriate
            if (currentHealth)
            {
                foreach (KeyValuePair<string, Ailment> ailment in this.ailments)
                {
                    charHealth -= ailment.Value.effect;
                }
            }

            // ensure health between 0 and maxHealth
            if (charHealth < 0)
            {
                charHealth = 0;
            }
            else if (charHealth > maxHealth)
            {
                charHealth = maxHealth;
            }

            return charHealth;
        }

        /// <summary>
        /// Checks for character death
        /// </summary>
        /// <returns>Boolean indicating whether character dead</returns>
        /// <param name="isBirth">bool indicating whether check is due to birth</param>
        /// <param name="isMother">bool indicating whether (if check is due to birth) character is mother</param>
        /// <param name="isStillborn">bool indicating whether (if check is due to birth) baby was stillborn</param>
        public Boolean CheckForDeath(bool isBirth = false, bool isMother = false, bool isStillborn = false)
        {
            // Check if chance of death effected by character traits
            double deathTraitsModifier = this.CalcTraitEffect(Globals_Game.Stats.DEATH);

            // calculate base chance of death
            // chance = 2.8% (2.5% for women) per health level below 10
            Double deathChanceIncrement = 0;
            if (this.isMale)
            {
                deathChanceIncrement = 2.8;
            }
            else
            {
                deathChanceIncrement = 2.5;
            }

            Double deathChance = (10 - this.CalculateHealth()) * deathChanceIncrement;

            // apply traits modifier (if exists)
            if (deathTraitsModifier != 0)
            {
                deathChance = deathChance + (deathChance * deathTraitsModifier);
            }

            // factor in birth event if appropriate
            if (isBirth)
            {
                // if check is on mother and baby was stillborn
                // (indicates unspecified complications with pregnancy)
                if ((isMother) && (isStillborn))
                {
                    deathChance = deathChance * 2;
                }
                // if is baby, or mother of healthy baby
                else
                {
                    deathChance = deathChance * 1.5;
                }
            }

            // generate a rndom double between 0-100 and compare to deathChance
            if ((Utility_Methods.GetRandomDouble(100)) <= deathChance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Performs necessary actions for aborting a pregnancy involving the character
        /// </summary>
        public void AbortPregnancy()
        {
            List<JournalEntry> births = new List<JournalEntry>();

            // get birth entry in Globals_Game.scheduledEvents
            births = Globals_Game.scheduledEvents.GetSpecificEntries(this.charID, "mother", "birth");

            // remove birth events from Globals_Game.scheduledEvents
            foreach (JournalEntry jEntry in births)
            {
                Globals_Game.scheduledEvents.entries.Remove(jEntry.jEntryID);
            }

            this.isPregnant = false;

            // clear births
            births.Clear();
        }

        /// <summary>
        /// Performs necessary actions for cancelling a marriage involving the character
        /// </summary>
        /// <param name="role">The role of the Character in the marriage</param>
        public void CancelMarriage(string role)
        {
            List<JournalEntry> marriages = Globals_Game.scheduledEvents.GetSpecificEntries(this.charID, role, "marriage");

            foreach (JournalEntry jEntry in marriages)
            {
                // generate marriageCancelled entry
                bool success = false;

                // get interested parties
                PlayerCharacter headOfFamilyBride = null;
                PlayerCharacter headOfFamilyGroom = null;
                Character bride = null;
                Character groom = null;

                for (int i = 0; i < jEntry.personae.Length; i++)
                {
                    string thisPersonae = jEntry.personae[i];
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
                uint newEntryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
                string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
                string thisBrideEntry = bride.charID + "|bride";
                string thisGroomEntry = groom.charID + "|groom";
                string allEntry = "all|all";
                string[] newEntryPersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry, allEntry };

                // type
                string type = "marriageCancelled";

                // description
                string[] fields = new string[] { groom.firstName + " " + groom.familyName, bride.firstName + " " + bride.familyName, this.firstName + " " + this.familyName };
                // create and add a marriageCancelled entry to Globals_Game.pastEvents
                ProtoMessage cancelMarriage = new ProtoMessage();
                cancelMarriage.ResponseType = DisplayMessages.CharacterMarriageDeath;
                cancelMarriage.MessageFields = fields;
                JournalEntry newEntry = new JournalEntry(newEntryID, year, season, newEntryPersonae, type, cancelMarriage);
                success = Globals_Game.AddPastEvent(newEntry);

                // delete marriage entry in Globals_Game.scheduledEvents
                Globals_Game.scheduledEvents.entries.Remove(jEntry.jEntryID);

                // remove fiancee entries
                if (bride != null)
                {
                    bride.fiancee = null;
                }
                if (groom != null)
                {
                    groom.fiancee = null;
                }
            }

            // clear marriages
            marriages.Clear();
        }

        /// <summary>
        /// Transfers all of a character's titles back to the owner
        /// </summary>
        public void AllMyTitlesToOwner()
        {
            Place thisPlace = null;

            foreach (string placeID in this.myTitles)
            {
                // get place
                if (Globals_Game.fiefMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.fiefMasterList[placeID];
                }

                else if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisPlace = Globals_Game.provinceMasterList[placeID];
                }

                // re-assign title to owner
                if (thisPlace != null)
                {
                    thisPlace.owner.myTitles.Add(placeID);
                    thisPlace.titleHolder = thisPlace.owner.charID;
                }
            }

             // remove from this character
            this.myTitles.Clear();
        }

        public PlayerCharacter GetPlayerCharacter()
        {
            if (this is PlayerCharacter)
            {
                return this as PlayerCharacter;
            }
            else if (this.GetHeadOfFamily() != null)
            {
                return this.GetHeadOfFamily();
            }
            else if (!string.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
            {
                return (this as NonPlayerCharacter).GetEmployer();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Performs necessary actions upon the death of a character
        /// </summary>
        /// <param name="circumstance">string containing the circumstance of the death</param>
        public void ProcessDeath(string circumstance = "natural")
        {
            Character mySpouse = null;
            NonPlayerCharacter thisHeir = null;

            // get role of character
            string role = "";
            // PCs
            if (this is PlayerCharacter)
            {
                if (!String.IsNullOrWhiteSpace((this as PlayerCharacter).playerID))
                {
                    role = "player";
                }
                else
                {
                    role = "PC";
                }
            }

            // NPCs
            else
            {
                if (!String.IsNullOrWhiteSpace((this as NonPlayerCharacter).familyID))
                {
                    if ((this as NonPlayerCharacter).isHeir)
                    {
                        role = "familyHeir";
                    }
                    else
                    {
                        role = "family";
                    }
                }
                else if (!String.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
                {
                    role = "employee";
                }
                else
                {
                    role = "NPC";
                }
            }

            // ============== 1. set isAlive = false and if was a captive, release
            this.isAlive = false;
            if (this.captorID != null)
            {
                this.location.gaol.Remove(this);
                PlayerCharacter captor = Globals_Game.getCharFromID(this.captorID) as PlayerCharacter;
                if (captor != null)
                {
                    captor.myCaptives.Remove(this);
                }
                this.captorID = null;
            }
            // ============== 2. remove from FIEF
            this.location.charactersInFief.Remove(this);

            // ============== 3. remove from ARMY LEADERSHIP
            if (!String.IsNullOrWhiteSpace(this.armyID))
            {
                // get army
                Army thisArmy = null;
                if (Globals_Game.armyMasterList.ContainsKey(this.armyID))
                {
                    thisArmy = Globals_Game.armyMasterList[this.armyID];

                    // set army leader to null
                    if (thisArmy != null)
                    {
                        thisArmy.leader = null;

                        // set default aggression and combatOdds levels
                        thisArmy.aggression = 1;
                        thisArmy.combatOdds = 4;
                    }
                }

            }

            // ============== 4. if married, remove from SPOUSE
            if (!String.IsNullOrWhiteSpace(this.spouse))
            {
                mySpouse = this.GetSpouse();

                if (mySpouse != null)
                {
                    mySpouse.spouse = null;
                }
                
            }

            // ============== 5. if engaged, remove from FIANCEE and CANCEL MARRIAGE
            if (!String.IsNullOrWhiteSpace(this.fiancee))
            {
                string marriageRole = "";
                Character myFiancee = this.GetFiancee();

                if (myFiancee != null)
                {
                    // get marriage entry in Globals_Game.scheduledEvents
                    // get role
                    if (this.isMale)
                    {
                        marriageRole = "groom";
                    }
                    else
                    {
                        marriageRole = "bride";
                    }

                    // cancel marriage
                    this.CancelMarriage(marriageRole);

                }

            }

            // ============== 6. check for PREGNANCY events (self or spouse)
            Character toAbort = null;

            if (this.isPregnant)
            {
                toAbort = this;
            }
            else if ((mySpouse != null) && (mySpouse.isPregnant))
            {
                toAbort = mySpouse;
            }

            if (toAbort != null)
            {
                // abort pregnancy
                toAbort.AbortPregnancy();
            }

            // ============== 7. check and remove from BAILIFF positions
            PlayerCharacter employer = null;
            if (this is PlayerCharacter)
            {
                employer = (this as PlayerCharacter);
            }
            else
            {
                // if is an employee
                if (!String.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
                {
                    // get boss
                    employer = (this as NonPlayerCharacter).GetEmployer();
                }

            }

            // check to see if is a bailiff.  If so, remove
            if (employer != null)
            {
                foreach (Fief thisFief in employer.ownedFiefs)
                {
                    if (thisFief.bailiff == this)
                    {
                        thisFief.bailiff = null;
                    }
                }
            }

            // ============== 8. (PC) check and remove any Positions
            if (this is PlayerCharacter)
            {
                // iterate through positions
                foreach (KeyValuePair<byte, Position> posEntry in Globals_Game.positionMasterList)
                {
                    // if deceased character is office holder, remove from office
                    if (posEntry.Value.GetOfficeHolder() == this)
                    {
                        posEntry.Value.RemoveFromOffice(this as PlayerCharacter);
                    }
                }
            }

            // ============== 9. (NPC) check and remove from PC MYNPCS list
            PlayerCharacter headOfFamily = null;
            if (this is NonPlayerCharacter)
            {
                // 8.1 employees
                if (role.Equals("employee"))
                {
                    // remove from boss's myNPCs
                    employer.myNPCs.Remove((this as NonPlayerCharacter));
                }

                // 8.2 family members
                else if (role.Contains("family"))
                {
                    // get head of family
                    headOfFamily = this.GetHeadOfFamily();

                    if (headOfFamily != null)
                    {
                        // remove from head of family's myNPCs
                        headOfFamily.myNPCs.Remove((this as NonPlayerCharacter));
                    }
                }

            }

            // ============== 10. (NPC) re-assign TITLES to fief owner
            if (this is NonPlayerCharacter)
            {
                this.AllMyTitlesToOwner();
            }


            // ============== 11. RESPAWN dead non-family NPCs
            if ((role.Equals("employee")) || (role.Equals("NPC")))
            {
                // respawn
                bool respawned = this.RespawnNPC(this as NonPlayerCharacter);
            }

            // ============== 12. (Player or PC) GET HEIR and PROCESS INHERITANCE
            else if ((role.Equals("player")) || (role.Equals("PC")))
            {
                // get heir
                thisHeir = (this as PlayerCharacter).GetHeir();

                if (thisHeir != null)
                {
                    this.ProcessInheritance((this as PlayerCharacter), inheritor: thisHeir);
                    Globals_Game.UpdatePlayer((this as PlayerCharacter).playerID,DisplayMessages.YouDied,new string[] {thisHeir.firstName + " " +thisHeir.familyName});

                }

                // if no heir, king inherits
                else
                {
                    // process inheritance
                    this.TransferPropertyToKing((this as PlayerCharacter), (this as PlayerCharacter).GetKing());
                    // Release captives
                    for (int i = (this as PlayerCharacter).myCaptives.Count - 1; i >= 0; i--)
                    {
                        Character captive = (this as PlayerCharacter).myCaptives.ElementAt(i);
                        (this as PlayerCharacter).ReleaseCaptive(captive);
                    }
                    Globals_Game.UpdatePlayer((this as PlayerCharacter).playerID, DisplayMessages.YouDiedNoHeir);
                }
            }

            // ============== 13. create and send DEATH JOURNAL ENTRY (unless is a nobody)
            if (!role.Equals("NPC"))
            {
                bool success = false;

                // ID
                uint deathEntryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae, type, description
                List<string> tempPersonae = new List<string>();
                string allEntry = "";
                string interestedPlayerEntry = "";
                string deceasedCharacterEntry = "";
                string type = "";
                string[] fields = new string[4];
                fields[0]= this.firstName + " " + this.familyName;

                // family member/heir
                if (role.Contains("family"))
                {
                    // personae
                    interestedPlayerEntry = headOfFamily.charID + "|headOfFamily";

                    if (role.Equals("heir"))
                    {
                        deceasedCharacterEntry += this.charID + "|deceasedHeir";

                        // type
                        type = "deathOfHeir";
                    }
                    else
                    {
                        deceasedCharacterEntry += this.charID + "|deceasedFamilyMember";

                        // type
                        type = "deathOfFamilyMember";
                    }

                    // description
                    fields[1] = (this as NonPlayerCharacter).GetFunction(headOfFamily) + " of "
                    + headOfFamily.firstName + " " + headOfFamily.familyName;
                }

                // employee
                else if (role.Equals("employee"))
                {
                    // personae
                    interestedPlayerEntry = employer.charID + "|employer";
                    deceasedCharacterEntry += this.charID + "|deceasedEmployee";

                    // type
                    type = "deathOfEmployee";

                    // description
                    fields[1] =  " employee of " + employer.firstName + " " + employer.familyName;
                }

                // player or non-played PC
                else if ((role.Equals("player")) || (role.Equals("PC")))
                {
                    // personae
                    allEntry = "all|all";
                    if (thisHeir != null)
                    {
                        interestedPlayerEntry = thisHeir.charID + "|newHeadOfFamily";
                    }
                    deceasedCharacterEntry += this.charID + "|deceasedHeadOfFamily";

                    // type
                    if (role.Equals("player"))
                    {
                        type = "deathOfPlayer";
                    }
                    else
                    {
                        type = "deathOfNoble";
                    }

                    // description
                    fields[1] =" head of the " + this.familyName + " family";
                }

                // personae
                if (!String.IsNullOrWhiteSpace(interestedPlayerEntry))
                {
                    tempPersonae.Add(interestedPlayerEntry);
                }
                tempPersonae.Add(deceasedCharacterEntry);
                if (!String.IsNullOrWhiteSpace(allEntry))
                {
                    tempPersonae.Add(allEntry);
                }
                string[] deathPersonae = tempPersonae.ToArray();

                // description
                switch (circumstance)
                {
                    case "injury":
                        fields[2] = "injuries sustained on the field of battle";
                        break;
                    case "childbirth":
                        fields[2] = "complications arising from childbirth";
                        break;
                    case "spy":
                        fields[2] = "injuries sustained while obtaining information";
                        break;
                    case "execute":
                        fields[2] = "execution at the hands of ";
                        if (isMale)
                        {
                            fields[2] += "his ";
                        }
                        else
                        {
                            fields[2] += "her ";
                        }
                        fields[2] += "captors";
                        break;
                    case "kidnap":
                        fields[2] = "a botched kidnap attempt";
                        break;
                    default:
                        fields[2]= "natural causes";
                        break;
                }

                // player death additional description
                if ((role.Equals("player")) || (role.Equals("PC")))
                {
                    // have an heir
                    if (thisHeir != null)
                    {
                        fields[3] = thisHeir.GetFunction(this as PlayerCharacter)+ ", " + thisHeir.firstName + " " + thisHeir.familyName;
                    }
                    
                }


                // create and add a death entry to Globals_Game.pastEvents
                ProtoMessage death = new ProtoMessage();
                death.MessageFields = fields;
                if (thisHeir == null&& this is PlayerCharacter)
                {
                    death.ResponseType = DisplayMessages.CharacterDeathNoHeir;
                }
                else
                {
                    death.ResponseType = DisplayMessages.CharacterDeath;
                }
                
                JournalEntry deathEntry = new JournalEntry(deathEntryID, year, season, deathPersonae, type, death);
                success = Globals_Game.AddPastEvent(deathEntry);

                // If currently controlled character dies, switch to player character
                if (this.GetPlayerCharacter() != null)
                {

                    if (!String.IsNullOrWhiteSpace(this.GetPlayerCharacter().playerID))
                    {
                        Client c;
                        Globals_Server.Clients.TryGetValue(this.GetPlayerCharacter().playerID, out c);
                        if (c != null)
                        {
                            if (c.activeChar == this)
                            {
                                c.activeChar = this.GetPlayerCharacter();
                            }
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Checks if the character is a province overlord
        /// </summary>
        /// <returns>bool indicating if the character is an overlord</returns>
        public bool CheckIfOverlord()
        {
            bool isOverlord = false;

            if (this is PlayerCharacter)
            {
                foreach (string placeID in this.myTitles)
                {
                    if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                    {
                        isOverlord = true;
                        break;
                    }
                }
            }

            return isOverlord;
        }

        /// <summary>
        /// Transfers property to the appropriate king upon the death of a PlayerCharacter with no heir
        /// </summary>
        /// <param name="deceased">Deceased PlayerCharacter</param>
        /// <param name="king">The king</param>
        public void TransferPropertyToKing(PlayerCharacter deceased, PlayerCharacter king)
        {
            // END SIEGES
            // copy siege IDs into temp list
            List<string> siegesToEnd = new List<string>();
            foreach (string siege in deceased.mySieges)
            {
                siegesToEnd.Add(siege);
            }

            if (siegesToEnd.Count > 0)
            {
                foreach (string siege in siegesToEnd)
                {
                    // get siege object
                    Siege thisSiege = null;
                    if (Globals_Game.siegeMasterList.ContainsKey(siege))
                    {
                        thisSiege = Globals_Game.siegeMasterList[siege];
                    }

                    // end siege
                    if (thisSiege != null)
                    {
                        thisSiege.SiegeEnd(false);
                    }
                }
                siegesToEnd.Clear();
            }

            // DISBAND ARMIES
            List<Army> tempArmyList = new List<Army>(); 
            for (int i = 0; i < deceased.myArmies.Count; i++ )
                {
                    tempArmyList.Add(deceased.myArmies[i]);
                }

            for (int i = 0; i < tempArmyList.Count; i++ )
            {
                tempArmyList[i].DisbandArmy();
                tempArmyList[i] = null;
            }
            tempArmyList.Clear();

            // EMPLOYEES/FAMILY
            for (int i = 0; i < deceased.myNPCs.Count; i++)
            {
                // get NPC
                NonPlayerCharacter npc = deceased.myNPCs[i];

                // remove from entourage
                deceased.RemoveFromEntourage(npc);

                // clear goTo queue
                npc.goTo.Clear();

                // employees are taken on by king
                if (!String.IsNullOrWhiteSpace(npc.employer))
                {
                    if (npc.employer.Equals(deceased.charID))
                    {
                        npc.employer = king.charID;
                        king.myNPCs.Add(npc);
                    }
                }

                // family members are cast into the cruel world
                else if (!String.IsNullOrWhiteSpace(npc.familyID))
                {
                    // familyID
                    npc.familyID = null;

                    // wage
                    npc.salary = 0;

                    // inKeep
                    npc.inKeep = false;

                    // titles
                    npc.AllMyTitlesToOwner();

                    // employment as bailiff
                    foreach (Fief fief in deceased.ownedFiefs)
                    {
                        if (fief.bailiff == npc)
                        {
                            fief.bailiff = null;
                        }
                    }

                    // pregnancy
                    Character npcSpouse = npc.GetSpouse();
                    Character toAbort = null;

                    if (npc.isPregnant)
                    {
                        toAbort = npc;
                    }
                    else if ((npcSpouse != null) && (npcSpouse.isPregnant))
                    {
                        toAbort = npcSpouse;
                    }

                    if (toAbort != null)
                    {
                        // abort pregnancy
                        toAbort.AbortPregnancy();
                    }

                    // forthcoming marriage
                    if (!String.IsNullOrWhiteSpace(npc.fiancee))
                    {
                        Character npcFiancee = npc.GetFiancee();

                        if (npcFiancee != null)
                        {
                            // get marriage entry in Globals_Game.scheduledEvents
                            // get role
                            string role = "";
                            if (npc.isMale)
                            {
                                role = "groom";
                            }
                            else
                            {
                                role = "bride";
                            }

                            // cancel marriage
                            npc.CancelMarriage(role);

                        }
                    }
                }
            }

            // TITLES
            foreach (string title in deceased.myTitles)
            {
                // get place
                Place thisPlace = null;
                if (Globals_Game.fiefMasterList.ContainsKey(title))
                {
                    thisPlace = Globals_Game.fiefMasterList[title];
                }
                else if (Globals_Game.provinceMasterList.ContainsKey(title))
                {
                    thisPlace = Globals_Game.provinceMasterList[title];
                }

                // transfer title
                if (thisPlace != null)
                {
                    if (thisPlace.owner == deceased)
                    {
                        thisPlace.titleHolder = king.charID;
                        king.myTitles.Add(title);
                    }

                    else
                    {
                        thisPlace.titleHolder = thisPlace.owner.charID;
                    }
                }
            }

            deceased.myTitles.Clear();

            // PLACES

            // fiefs
            foreach (Fief fief in deceased.ownedFiefs)
            {
                // ownership
                fief.owner = king;
                king.ownedFiefs.Add(fief);

                // ancestral ownership
                if (fief.ancestralOwner == deceased)
                {
                    fief.ancestralOwner = king;
                }
            }

            // provinces
            foreach (Province prov in deceased.ownedProvinces)
            {
                prov.owner = king;
            }

			// OWNERSHIPCHALLENGES
			List<OwnershipChallenge> toRemove = new List<OwnershipChallenge> ();
			foreach (KeyValuePair<string, OwnershipChallenge> challengeEntry in Globals_Game.ownershipChallenges)
			{
				if (challengeEntry.Value.GetChallenger() == deceased)
				{
					toRemove.Add(challengeEntry.Value);
				}
			}

			// process toRemove
			if (toRemove.Count > 0)
			{
				foreach (OwnershipChallenge thisChallenge in toRemove)
				{
					Globals_Game.ownershipChallenges.Remove (thisChallenge.id);
				}

				toRemove.Clear ();
			}

            // UPDATE GLOBALS_GAME.VICTORYDATA
            if (!String.IsNullOrWhiteSpace(deceased.playerID))
            {
                if (Globals_Game.victoryData.ContainsKey(deceased.playerID))
                {
                    Globals_Game.victoryData.Remove(deceased.playerID);
                }
            }

        }

        /// <summary>
        /// Performs the functions associated with the inheritance of property upon the death of a PlayerCharacter
        /// </summary>
        /// <param name="inheritor">Inheriting Character</param>
        /// <param name="deceased">Deceased PlayerCharacter</param>
        public void ProcessInheritance(PlayerCharacter deceased, NonPlayerCharacter inheritor = null)
        {
            if (inheritor != null)
            {
                Globals_Server.logEvent(deceased.charID + " dies; " + inheritor.charID + " inherits");
            }
            // ============== 1. CREATE NEW PC from NPC (inheritor)
			// remove inheritor from deceased's myNPCs
			if (deceased.myNPCs.Contains(inheritor))
			{
				deceased.myNPCs.Remove(inheritor);
			}

			// promote inheritor
            PlayerCharacter promotedNPC = new PlayerCharacter(inheritor, deceased);

			// remove from npcMasterList and mark for addition to pcMasterList
            Globals_Game.npcMasterList.Remove(inheritor.charID);
            Globals_Game.pcMasterList.Add(promotedNPC.charID,promotedNPC);
            // TODO ask about whether NPC should be promoted next season
            Globals_Game.promotedNPCs.Add(promotedNPC);

            // ============== 2. change all FAMILYID & EMPLOYER of MYNPCS to promotedNPC's
            for (int i = 0; i < promotedNPC.myNPCs.Count; i++ )
            {
                if (!String.IsNullOrWhiteSpace(promotedNPC.myNPCs[i].familyID))
                {
                    if (promotedNPC.myNPCs[i].familyID.Equals(deceased.charID))
                    {
                        promotedNPC.myNPCs[i].familyID = promotedNPC.charID;
                    }
                }

                else if (!String.IsNullOrWhiteSpace(promotedNPC.myNPCs[i].employer))
                {
                    if (promotedNPC.myNPCs[i].employer.Equals(deceased.charID))
                    {
                        promotedNPC.myNPCs[i].employer = promotedNPC.charID;
                    }
                }
            }

            // ============== 3. change OWNER & ANCESTRALOWNER for FIEFS and set inheritor and promotedNPC LOCATION
            List<string> ancestOwnerChanges = new List<string>();

            // NOTE: need to iterate through fiefMasterList 'cos might not own a fief where you are ancestralOwner
            foreach (KeyValuePair<string, Fief> thisFiefEntry in Globals_Game.fiefMasterList)
            {
                // get fiefs requiring change to ancestralOwner
                if (thisFiefEntry.Value.ancestralOwner == deceased)
                {
                    ancestOwnerChanges.Add(thisFiefEntry.Key);
                }
            }

            // make necessary changes
            if (ancestOwnerChanges.Count > 0)
            {
                foreach (string thisFiefID in ancestOwnerChanges)
                {
                    Globals_Game.fiefMasterList[thisFiefID].ancestralOwner = promotedNPC;
                }
            }

			// ============== 4. change OWNERSHIPCHALLENGES
            List<OwnershipChallenge> challsToChange = new List<OwnershipChallenge>();
			foreach (KeyValuePair<string, OwnershipChallenge> challengeEntry in Globals_Game.ownershipChallenges)
			{
				if (challengeEntry.Value.GetChallenger() == deceased)
				{
                    challsToChange.Add(challengeEntry.Value);
				}
			}

            // make necessary changes
            if (challsToChange.Count > 0)
            {
                foreach (OwnershipChallenge thisChallenge in challsToChange)
                {
                    Globals_Game.ownershipChallenges[thisChallenge.id].challengerID = promotedNPC.charID;
                }
            }

			// ============== 5. change OWNER for ARMIES
            for (int i = 0; i < promotedNPC.myArmies.Count; i++ )
            {
                promotedNPC.myArmies[i].owner = promotedNPC.charID;
            }

			// ============== 6. change BESIEGINGPLAYER for SIEGES
            for (int i = 0; i < promotedNPC.mySieges.Count; i++)
            {
                // get siege
                Siege thisSiege = null;
                thisSiege = Globals_Game.siegeMasterList[promotedNPC.mySieges[i]];

                // change besiegingPlayer
                if (thisSiege != null)
                {
                    thisSiege.besiegingPlayer = promotedNPC.charID;
                }
            }

            // ============== 7. update GLOBALS_GAME.VICTORYDATA
            if (!String.IsNullOrWhiteSpace(promotedNPC.playerID))
            {
                if (Globals_Game.victoryData.ContainsKey(promotedNPC.playerID))
                {
                    Globals_Game.victoryData[promotedNPC.playerID].playerCharacterID = promotedNPC.charID;
                }
            }

			// ============== 8. change references to player's PlayerCharacter
            string user = deceased.playerID;
            
            if (user != null)
            {
                if (Globals_Game.ownedPlayerCharacters.ContainsKey(user))
                {
                    Globals_Game.ownedPlayerCharacters[deceased.playerID] = promotedNPC;
                    promotedNPC.playerID = user;
                    Globals_Server.Clients[user].myPlayerCharacter = promotedNPC;
                    //TODO notify user if logged in and write to database
                    Globals_Server.logEvent("Debug: role is  : "+inheritor.GetFunction(deceased));
                    Client player;
                    Globals_Server.Clients.TryGetValue(user, out player);
                    if (player != null)
                    {
                        player.myPlayerCharacter = promotedNPC;
                        if (player.activeChar == deceased)
                        {
                            player.activeChar = promotedNPC;
                        }
                    }
                }
                else
                {
                    Globals_Server.logError(user + " not contained in list of registered users");
                }
            }

            // ======== 9. Transfer Captives
            List<Character> toRemove = new List<Character>();
            foreach (Character captive in deceased.myCaptives)
            {
                toRemove.Add(captive);
                promotedNPC.myCaptives.Add(captive);
                captive.captorID = promotedNPC.charID;
            }
            foreach (Character captive in toRemove)
            {
                deceased.myCaptives.Remove(captive);
            }
        }

        /// <summary>
        /// Creates new NonPlayerCharacter, based on supplied NonPlayerCharacter
        /// </summary>
        /// <param name="oldNPC">Old NonPlayerCharacter</param>
        public bool RespawnNPC(NonPlayerCharacter oldNPC)
        {
            bool success = false;

            // LOCATION
            List<string> fiefIDs = new List<string>();

            // get all fief where language same as newNPC's
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                if (fiefEntry.Value.language == oldNPC.language)
                {
                    fiefIDs.Add(fiefEntry.Key);
                }
            }

            // choose new location (by generating random int)
            string newLocationID = "";

            // choose from fiefs with same language
            if (fiefIDs.Count > 0)
            {
                newLocationID = fiefIDs[Globals_Game.myRand.Next(0, fiefIDs.Count)];
            }

            // if no fiefs with same language, choose random fief
            else
            {
                newLocationID = Globals_Game.fiefMasterList.Keys.ElementAt(Globals_Game.myRand.Next(0, fiefIDs.Count));
            }

            // create new NPC and assign location
            if (!String.IsNullOrWhiteSpace(newLocationID))
            {
                // create basic NPC
                NonPlayerCharacter newNPC = null;
                newNPC = new NonPlayerCharacter(oldNPC);
                if (newNPC != null)
                {
                    success = true;

                    // set location
                    newNPC.location = Globals_Game.fiefMasterList[newLocationID];
                    newNPC.location.charactersInFief.Add(newNPC);
                }
            }

            // TODO: FIRSTNAME

            if (!success)
            {
                //TODO error logging
                string errorLog = "NPC CREATION ERROR: " + "Error: NPC " + oldNPC.charID
                        + " (" + oldNPC.firstName + " " + oldNPC.familyName + ") could not be respawned";
            }

            return success;
        }
        
        //TODO determine what other players may be using this
        /// <summary>
        /// Enables character to enter keep (if not barred)
        /// </summary>
        /// <returns>bool indicating success</returns>
        public virtual bool EnterKeep(out ProtoMessage error)
        {
            error = null;
            bool proceed = true;
            Army thisArmy = null;
            PlayerCharacter player = null;
            if (this is PlayerCharacter) player = (PlayerCharacter)this;
            // check if character leading an army
            if (!String.IsNullOrWhiteSpace(this.armyID))
            {
                // get army
                thisArmy = this.GetArmy();

                if (thisArmy != null)
                {
                    // armies not owned by fief owner not allowed in keep
                    if (thisArmy.GetOwner() != location.owner)
                    {
                        proceed = false;
                        if (player!=null)
                        {
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterEnterArmy;}
                    }

                    // only one friendly field army in keep at a time
                    else if (this.location.CheckFieldArmyInKeep())
                    {
                        proceed = false;
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterAlreadyArmy;
                    }
                }
            }

            if (proceed)
            {
                // if character is of a barred nationality, don't allow entry
                if (location.barredNationalities.Contains(this.nationality.natID))
                {
                    proceed = false;
                    if (player!=null)
                    {
                        string title = "My Lord";
                        if (this.nationality.natID.Equals("Sco"))
                        {
                            title = "Laddie";
                        }
                        else if (this.nationality.natID.Equals("fr"))
                        {
                            title = "Mon Seigneur";
                        }
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterNationalityBarred;
                        error.MessageFields =  new string[]{this.nationality.name, title};
                    }
                }

                if (proceed)
                {
                    // if this character is specifically barred, don't allow entry
                    if (location.barredCharacters.Contains(this.charID))
                    {
                        proceed = false;
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterBarred;
                    }
                }
            }

            this.inKeep = proceed;

            // if have entered keep with an army, ensure aggression level set to at least 1 (to allow siege defence)
            if (proceed)
            {
                if (thisArmy != null)
                {
                    if (thisArmy.aggression == 0)
                    {
                        thisArmy.aggression = 1;
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Enables character to exit keep
        /// </summary>
        /// <returns>bool indicating hire-able status</returns>
        public virtual bool ExitKeep()
        {
            // exit keep
            this.inKeep = false;

            return !(this.inKeep);
        }

        /// <summary>
        /// Checks to see if the Character can be hired by the specified PlayerCharacter
        /// </summary>
        /// <returns>bool indicating hire-able status</returns>
        /// <param name="hiringPC">The potential employer (PlayerCharacter)</param>
        public bool CheckCanHire(PlayerCharacter hiringPC)
        {
            bool canHire = true;

            // must be an NPC
            if (this is PlayerCharacter)
            {
                canHire = false;
            }

                // cannot be current employee
            else
            {
                if (hiringPC.myNPCs.Contains(this as NonPlayerCharacter))
                {
                    canHire = false;
                }
            }

            // cannot be member of any family
            if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                canHire = false;
            }

            // must be over 13 years of age
            if (this.CalcAge() < 14)
            {
                canHire = false;
            }

            // must be male
            if (!this.isMale)
            {
                canHire = false;
            }

            return canHire;
        }

        /// <summary>
        /// Calculates effect of character's management rating on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcFiefIncMod()
        {
            double incomeModif = 0;

            // 2.5% increase in income per management level above 1
            incomeModif = (this.management - 1) * 2.5;

            incomeModif = incomeModif / 100;

            return incomeModif;
        }

        // TODO change existing trait names to enum
        /// <summary>
        /// Calculates effect of a particular trait effect
        /// </summary>
        /// <returns>double containing trait effect modifier</returns>
        /// <param name="effect">string specifying which trait effect to calculate</param>
        public double CalcTraitEffect(Globals_Game.Stats effect)
        {
            Globals_Game.Stats stat = effect;
           // Enum.TryParse<Globals_Game.Stats>(effect, true, out stat);
            double traitEffectModifier = 0;

            // iterate through traits
            for (int i = 0; i < this.traits.Length; i++)
            {
                // iterate through trait effects, looking for effect
                foreach (KeyValuePair<Globals_Game.Stats, double> entry in this.traits[i].Item1.effects)
                {
                    // if present, update total modifier
                    if (entry.Key.Equals(stat))
                    {
                        // get this particular modifer (based on character's trait level)
                        // and round up if necessary (i.e. to get the full effect)
                        double thisModifier = (this.traits[i].Item2 * 0.111);
                        if (this.traits[i].Item2 == 9)
                        {
                            thisModifier = 1;
                        }
                        // add to exisiting total modifier
                        traitEffectModifier += (entry.Value * thisModifier);
                    }
                }
            }

            return traitEffectModifier;
        }

        /// <summary>
        /// Gets the army being led by the character
        /// </summary>
        /// <returns>The army</returns>
        public Army GetArmy()
        {
            Army thisArmy = null;

            if (!String.IsNullOrWhiteSpace(this.armyID))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.armyID))
                {
                    thisArmy = Globals_Game.armyMasterList[this.armyID];
                }
                else
                {
                    Globals_Server.logError("Character " + this.charID + " leading army " + this.armyID + ", but army not found in army master list");
                }
            }

            return thisArmy;
        }

        /// <summary>
        /// Gets character's father
        /// </summary>
        /// <returns>The father</returns>
        public Character GetFather()
        {
            Character father = null;

            if (!String.IsNullOrWhiteSpace(this.father))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.father))
                {
                    father = Globals_Game.pcMasterList[this.father];
                }
                else if (Globals_Game.npcMasterList.ContainsKey(this.father))
                {
                    father = Globals_Game.npcMasterList[this.father];
                }
            }

            return father;
        }

        /// <summary>
        /// Gets character's mother
        /// </summary>
        /// <returns>The mother</returns>
        public Character GetMother()
        {
            Character mother = null;

            if (!String.IsNullOrWhiteSpace(this.mother))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.mother))
                {
                    mother = Globals_Game.pcMasterList[this.mother];
                }
                else if (Globals_Game.npcMasterList.ContainsKey(this.mother))
                {
                    mother = Globals_Game.npcMasterList[this.mother];
                }
            }

            return mother;
        }

        /// <summary>
        /// Gets character's head of family
        /// </summary>
        /// <returns>The head of the family</returns>
        public PlayerCharacter GetHeadOfFamily()
        {
            PlayerCharacter headFamily = null;

            if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.familyID))
                {
                    headFamily = Globals_Game.pcMasterList[this.familyID];
                }
            }

            return headFamily;
        }

        //TODO check if there is a way to do this without passing in client
        //ASK is user player? so in PlayerCharacter playerID = the player's id/username
        /// <summary>
        /// Performs conditional checks before granting a gift or postiton of responsibility
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="type">string identify type of grant</param>
        /// <param name="priorToList">bool indicating if check is prior to listing possible candidates</param>
        /// <param name="armyID">string containing the army ID (if choosing a leader)</param>
        public bool ChecksBeforeGranting(PlayerCharacter granter, string type, bool priorToList, out ProtoMessage error, string armyID = null)
        {
            error = null;
            bool proceed = true;
            // get army if appropriate
            Army armyToLead = null;
            if (!String.IsNullOrWhiteSpace(armyID))
            {
                if (Globals_Game.armyMasterList.ContainsKey(armyID))
                {
                    armyToLead = Globals_Game.armyMasterList[armyID];
                }
            }

            // royal gifts
            if (type.Contains("royal"))
            {
                // 1. check is PC
                if (!(this is PlayerCharacter))
                {
                    proceed = false;
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.CharacterRoyalGiftPlayer;
                }

                else
                {
                    // 2. check is an active player
                    if (String.IsNullOrWhiteSpace((this as PlayerCharacter).playerID))
                    {
                        proceed = false;
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterRoyalGiftPlayer;
                    }

                    else
                    {
                        // 3. check is not self
                        if ((this as PlayerCharacter) == granter)
                        {
                            proceed = false;
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterRoyalGiftSelf;
                        }
                    }
                }
            }

            // army leaders, bailiffs, fief titles
            else
            {
                // checks for all
                // 1. check is male
                if (!this.isMale)
                {
                    Console.WriteLine("Not male");
                    proceed = false;
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.CharacterNotMale;
                }

                else
                {
                    // 2. check is of age
                    if (this.CalcAge() < 14)
                    {
                        Console.Write("too young");
                        proceed = false;
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterNotOfAge;
                    }

                    // army leaders
                    else
                    {

                        // 3. army leaders must be in same hex as army
                        if (type.Equals("leader"))
                        {
                            // Army must be defined
                            if (armyToLead == null)
                            {
                                Console.WriteLine("No Army");
                                ProtoMessage noArmy = new ProtoMessage();
                                noArmy.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                                error = noArmy;
                                return false;
                            }
                            if ((!(this.location.id.Equals(armyToLead.location))))
                            {
                                Console.WriteLine("Not same location");
                                proceed = false;
                                error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.CharacterLeaderLocation;
                            }
                            else
                            {
                                // 4. check if army leader is already leader of this army
                                if ((!String.IsNullOrWhiteSpace(this.armyID)) && (!String.IsNullOrWhiteSpace(armyID)))
                                {
                                    if (this.armyID.Equals(armyID))
                                    {
                                        Console.WriteLine("Already leader");
                                        proceed = false;
                                        error = new ProtoMessage();
                                        error.ResponseType = DisplayMessages.CharacterLeadingArmy;
                                    }
                                }
                            }   
                        }
                    }
                }
            }
            //TODO confirm on client side: if character is leading army or is bailiff confirm becore appointing leader or bailiff
            // checks to be carried out at the time of selection (i.e. not prior to listing candidates)
            if (proceed)
            {
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.Success;
            }
       
            return proceed;
        }

        /// <summary>
        /// Allows the character to enter or exit the keep
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool ExitEnterKeep(out ProtoMessage result)
        {
            bool success = false;
            result = null;
            // if in keep
            if (this.inKeep)
            {
                // exit keep
                success = this.ExitKeep();
            }

            // if not in keep
            else
            {
                // attempt to enter keep
                success = this.EnterKeep(out result);
            }

            return success;
        }

        //TODO rewrite to get confirmation from client
        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public bool ChecksBeforeMove(Fief target, double cost, out ProtoMessage error, bool siegeCheck = true)
        {
            error = null;
            bool proceedWithMove = true;
            PlayerCharacter player = null;
            if (this is PlayerCharacter)
            {
                if (!string.IsNullOrWhiteSpace((this as PlayerCharacter).playerID))
                {
                    player = this as PlayerCharacter;
                }
            }
            else
            {
                if (this.familyID != null)
                {
                    player = this.GetHeadOfFamily();
                }
                if (!string.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
                {
                    player = (this as NonPlayerCharacter).GetEmployer();
                }
            }
            // check to see if character is leading a besieging army
            if (siegeCheck)
            {
                Army myArmy = this.GetArmy();
                if (myArmy != null)
                {
                    string thisSiegeID = myArmy.CheckIfBesieger();
                    if (!String.IsNullOrWhiteSpace(thisSiegeID))
                    {
                        // end the siege
                            Siege thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
                            if (player != null)
                            {
                                string[] fields = new string[3];
                                // construct event description to be passed into siegeEnd
                                fields[0]= thisSiege.GetBesiegingPlayer().firstName + " " + thisSiege.GetBesiegingPlayer().familyName;
                                fields[1]= thisSiege.GetFief().name;
                                fields[2] = thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;

                                // end siege and set to null
                                thisSiege.SiegeEnd(false, DisplayMessages.SiegeEndDefault,fields);
                                thisSiege = null;
                            }
                    }
                }

                if (proceedWithMove)
                {
                    // check for sufficient days
                    if (this.days < cost)
                    {
                        // if target fief not in character's goTo queue, add it
                        if (this.goTo.Count == 0)
                        {
                            this.goTo.Enqueue(target);
                        }

                        if (player != null)
                        {
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterDaysJourney;
                        }

                        proceedWithMove = false;
                    }
                }
            }

            return proceedWithMove;
        }

        //TODO determine how to implement confirmation from user
        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public virtual bool MoveCharacter(Fief target, double cost, out ProtoMessage error, bool siegeCheck = true)
        {
            bool success = this.ChecksBeforeMove(target, cost, out error, siegeCheck);
            if (!success) return false;
            //Holds the playercharacter moving or who initiated NPC move
            PlayerCharacter player = null;
            if (this is PlayerCharacter)
            {
                player = (PlayerCharacter)this;
            }
            else
            {
                if (this.GetHeadOfFamily() != null)
                {
                    player = this.GetHeadOfFamily();
                }
                if ((this as NonPlayerCharacter).GetEmployer() != null)
                {
                    player = (this as NonPlayerCharacter).GetEmployer();
                }
            }
            //TODO client side if goTo list not empty and target not first in goTo list confirm is ok to clear destination list
            if (this.goTo.Count!=0 && target != this.goTo.Peek())
            {
                this.goTo.Clear();
            }
            else {
                // remove character from current fief's character list
                this.location.RemoveCharacter(this);

                // set location to target fief
                this.location = target;

                // add character to target fief's character list
                this.location.AddCharacter(this);

                // arrives outside keep
                this.inKeep = false;

                // deduct move cost from days left
                if (this is PlayerCharacter)
                {
                    (this as PlayerCharacter).AdjustDays(cost);
                }
                else
                {
                    this.AdjustDays(cost);
                }

                // check if has accompanying army, if so move it
                if (!String.IsNullOrWhiteSpace(this.armyID))
                {
                    this.GetArmy().MoveArmy();
                }
            }

            return success;
        }

        /// <summary>
        /// Gets the character's full days allowance, including adjustment for traits
        /// </summary>
        /// <returns>Full days allowance</returns>
        public double GetDaysAllowance()
        {
            // base allowance
            double myDays = 90;

            // check for time efficiency in traits
            double timeTraitsMOd = this.CalcTraitEffect(Globals_Game.Stats.TIME);
            if (timeTraitsMOd != 0)
            {
                // apply trait effects
                myDays = myDays + (myDays * timeTraitsMOd);
            }

            return myDays;
        }

        /// <summary>
        /// Adjusts the character's remaining days by subtracting the specified number of days
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public virtual void AdjustDays(Double daysToSubtract)
        {
            // adjust character's days
            this.days -= daysToSubtract;

            // ensure days not < 0
            if (this.days < 0)
            {
                this.days = 0;
            }

            // if army leader, synchronise army days
            if (!String.IsNullOrWhiteSpace(this.armyID))
            {
                // get army
                Army thisArmy = this.GetArmy();

                if (thisArmy != null)
                {
                    thisArmy.days = this.days;
                }
            }
        }

        /// <summary>
        /// Uses up the character's remaining days, which will be added to bailiffDaysInFief if appropriate
        /// </summary>
        public void UseUpDays()
        {
            Double remainingDays = this.days;

            // if character is bailiff of this fief, increment bailiffDaysInFief
            if (this.location.bailiff == this)
            {
                this.location.bailiffDaysInFief += remainingDays;
                this.AdjustDays(remainingDays);
            }
        }

        /// <summary>
        /// Calculates whether character manages to get spouse pregnant
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="wife">Character's spouse</param>
        public bool GetSpousePregnant(Character wife,out ProtoMessage birthMessage)
        {
           
            birthMessage = null;
            bool isPlayer = this is PlayerCharacter;
            PlayerCharacter player = null;
            if (isPlayer)
            {
                player = (PlayerCharacter)this;
            }
                //TODO who is relevant playercharacter
            else
            {
                player = this.GetHeadOfFamily();
            }
            bool success = false;

            // generate random (0 - 100) to see if pregnancy successful
            double randPercentage = Utility_Methods.GetRandomDouble(100);

            // holds chance of pregnancy based on age and virility
            int chanceOfPregnancy = 0;

            // holds pregnancy modifier based on virility
            double pregModifier = 0;

            // spouse's age
            int spouseAge = wife.CalcAge();

            // calculate base chance of pregnancy, based on age of spouse
            if ((!(spouseAge < 14)) && (!(spouseAge > 55)))
            {
                if (spouseAge < 18)
                {
                    chanceOfPregnancy = 8;
                }
                else if (spouseAge < 25)
                {
                    chanceOfPregnancy = 10;
                }
                else if (spouseAge < 30)
                {
                    chanceOfPregnancy = 8;
                }
                else if (spouseAge < 35)
                {
                    chanceOfPregnancy = 6;
                }
                else if (spouseAge < 40)
                {
                    chanceOfPregnancy = 5;
                }
                else if (spouseAge < 45)
                {
                    chanceOfPregnancy = 4;
                }
                else if (spouseAge < 50)
                {
                    chanceOfPregnancy = 2;
                }
                else if (spouseAge < 55)
                {
                    chanceOfPregnancy = 1;
                }
            }

            // factor in effect of virility
            // but only if within child-bearing age bracket (14 - 55)
            if ((!(spouseAge < 14)) && (!(spouseAge > 55)))
            {
                // modifier will be in range 0.4 - -0.4 depending on parent's virility
                // 1. get average parent virility
                pregModifier = (this.virility + wife.virility) / 2;
                // 2. subtract 5 and divide by 10 to give final modifier
                pregModifier = (pregModifier - 5) / 10;

                // apply modifier to chanceOfPregnancy
                chanceOfPregnancy = chanceOfPregnancy + Convert.ToInt32(chanceOfPregnancy * pregModifier);
                if (chanceOfPregnancy < 0)
                {
                    chanceOfPregnancy = 0;
                }
            }

            // compare chanceOfPregnancy with randPercentage to see if pregnancy successful
            if (chanceOfPregnancy > 0)
            {
                // if attempt successful
                if (randPercentage <= chanceOfPregnancy)
                {
                    // set spouse as pregnant
                    wife.isPregnant = true;

                    // schedule birth in clock sheduledEvents
                    uint birthYear = Globals_Game.clock.currentYear;
                    byte birthSeason = Globals_Game.clock.currentSeason;
                    if (Globals_Game.clock.currentSeason == 0)
                    {
                        birthSeason = (byte)(birthSeason + 3);
                    }
                    else
                    {
                        birthSeason = (byte)(birthSeason - 1);
                        birthYear = birthYear + 1;
                    }
                    string[] birthPersonae = new string[] { wife.familyID + "|headOfFamily", wife.charID + "|mother", wife.spouse + "|father" };

                    // create entry
                    JournalEntry birth = new JournalEntry(Globals_Game.GetNextJournalEntryID(), birthYear, birthSeason, birthPersonae, "birth",null);
                    // add entry
                    Globals_Game.scheduledEvents.entries.Add(birth.jEntryID, birth);

                    // check has been added
                    if (Globals_Game.scheduledEvents.entries.ContainsKey(birth.jEntryID))
                    {
                        success = true;

                        // display message of celebration
                        if (player!=null)
                        {
                            birthMessage = new ProtoMessage();
                            birthMessage.ResponseType = DisplayMessages.CharacterSpousePregnant;
                            birthMessage.MessageFields = new string[] { wife.firstName + " " + wife.familyName };
                        }
                    }

                    // if not added
                    else
                    {
                        //TODO error logging
                    }
                }

                // if attempt not successful
                else
                {
                    // display encouraging message
                    if (player!=null)
                    {
                        birthMessage = new ProtoMessage();
                        birthMessage.ResponseType = DisplayMessages.CharacterSpouseNotPregnant;
                        birthMessage.MessageFields = new string[] { wife.firstName + " " + wife.familyName };
                    }
                }

                // succeed or fail, deduct a day
                this.AdjustDays(1);
                wife.AdjustDays(1);

            }
            // if pregnancy impossible
            else
            {
                // give the player the bad news
                if (player!=null)
                {
                    birthMessage = new ProtoMessage();
                    birthMessage.ResponseType = DisplayMessages.CharacterSpouseNeverPregnant;
                    birthMessage.MessageFields = new string[] { wife.firstName + " " + wife.familyName };
                }
            }

            return success;
        }

        /// <summary>
        /// Performs childbirth procedure
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        /// <param name="daddy">The new NPC's father</param>
        public void GiveBirth(Character daddy)
        {
            string description = "";

            // get head of family
            PlayerCharacter thisHeadOfFamily = daddy.GetHeadOfFamily();

            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = Birth.GenerateNewNPC(this, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.CheckForDeath(true, false, false);

            if (!isStillborn)
            {
                // add baby to npcMasterList
                Globals_Game.npcMasterList.Add(weeBairn.charID, weeBairn);

                // set baby's location
                weeBairn.location = this.location;
                weeBairn.location.charactersInFief.Add(weeBairn);

                // add baby to family
                thisHeadOfFamily.myNPCs.Add(weeBairn);
            }
            else
            {
                weeBairn.isAlive = false;
            }

            // check for mother dying during childbirth
            bool mummyDied = this.CheckForDeath(true, true, isStillborn);

            // construct and send JOURNAL ENTRY

            // personae
            string[] childbirthPersonae = new string[] { thisHeadOfFamily.charID + "|headOfFamily", this.charID + "|mother", daddy.charID + "|father", weeBairn.charID + "|child" };

            // description
            DisplayMessages ResponseType = DisplayMessages.None;
            string[] fields = new string[5];
            fields[0] = this.firstName + " " + this.familyName;
            fields[1] = daddy.firstName + " " + daddy.familyName;
            if (weeBairn.isMale)
            {
                fields[2] = "son";
            }
            else
            {
                fields[2] = "daughter";
            }
            fields[3] = thisHeadOfFamily.firstName + " " + thisHeadOfFamily.familyName;
            // mother and baby alive
            if ((!isStillborn) && (!mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthOK;
            }

            // baby OK, mother dead
            if ((!isStillborn) && (mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthMumDead;
            }

            // mother OK, baby dead
            if ((isStillborn) && (!mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthChildDead;
            }

            // both mother and baby died
            if ((isStillborn) && (mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthAllDead;
            }

            // put together new journal entry
            ProtoMessage birth = new ProtoMessage();
            birth.ResponseType=ResponseType;
            birth.MessageFields = null;
            JournalEntry childbirth = new JournalEntry(Globals_Game.GetNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, childbirthPersonae, "birth", birth);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(childbirth);

            // if appropriate, process mother's death
            if (mummyDied)
            {
                this.ProcessDeath("childbirth");
            }


            // display message
            if (!string.IsNullOrEmpty(thisHeadOfFamily.playerID))
            {
                //TODO message handling
                Globals_Game.UpdatePlayer(thisHeadOfFamily.playerID, ResponseType,fields);
            }
        }

        /// <summary>
        /// Calculates the character's leadership value (for army leaders)
        /// </summary>
        /// <returns>double containg leadership value</returns>
        /// <param name="isSiegeStorm">bool indicating if the circumstance is a siege storm</param>
        public double GetLeadershipValue(bool isSiegeStorm = false)
        {
            double lv = 0;

            // get base LV
            lv = (this.combat + this.management + this.CalculateStature()) / 3;

            // factor in traits effect
            double combatTraitsMod = 0;

            // if is siege, use 'siege' trait
            if (isSiegeStorm)
            {
                combatTraitsMod = this.CalcTraitEffect(Globals_Game.Stats.SIEGE);
            }
            // else use 'battle' trait
            else
            {
                combatTraitsMod = this.CalcTraitEffect(Globals_Game.Stats.BATTLE);
            }

            if (combatTraitsMod != 0)
            {
                lv = lv + (lv * combatTraitsMod);
            }

            return lv;
        }

        /// <summary>
        /// Calculates the character's combat value for a combat engagement
        /// </summary>
        /// <returns>double containg combat value</returns>
        public double GetCombatValue()
        {
            double cv = 0;

            // get base CV
            cv += (this.combat + this.CalculateHealth()) / 2;

            // factor in armour
            cv += 5;

            // factor in nationality
            if (this.nationality.natID.Equals("Eng"))
            {
                cv += 5;
            }

            return cv;
        }

        /// <summary>
        /// Calculates the character's estimate variance when estimating the size of an enemy army
        /// </summary>
        /// <returns>double containg estimate variance</returns>
        public double GetEstimateVariance()
        {
            // base estimate variance
            double ev = 0.05;

            // apply effects of leadership value (includes 'battle' trait)
            ev = ev + ((10 - this.GetLeadershipValue()) * 0.05);

            return ev;
        }

        /// <summary>
        /// gets the character's spouse
        /// </summary>
        /// <returns>The spouse or null</returns>
        public Character GetSpouse()
        {
            Character mySpouse = null;

            if (!String.IsNullOrWhiteSpace(this.spouse))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.spouse))
                {
                    mySpouse = Globals_Game.pcMasterList[this.spouse];
                }
                else if (Globals_Game.npcMasterList.ContainsKey(this.spouse))
                {
                    mySpouse = Globals_Game.npcMasterList[this.spouse];
                }
            }

            return mySpouse;
        }

        /// <summary>
        /// gets the character's fiancee
        /// </summary>
        /// <returns>The spouse or null</returns>
        public Character GetFiancee()
        {
            Character myFiancee = null;

            if (!String.IsNullOrWhiteSpace(this.fiancee))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.fiancee))
                {
                    myFiancee = Globals_Game.pcMasterList[this.fiancee];
                }
                else if (Globals_Game.npcMasterList.ContainsKey(this.fiancee))
                {
                    myFiancee = Globals_Game.npcMasterList[this.fiancee];
                }
            }

            return myFiancee;
        }

        /// <summary>
        /// Updates character data at the end/beginning of the season
        /// </summary>
        public void UpdateCharacter()
        {
            // check for character DEATH
            bool characterDead = this.CheckForDeath();

            // if character dead, process death
            if (characterDead)
            {
                this.ProcessDeath();
            }

            else
            {
                // update AILMENTS (decrement effects, remove)
                // keep track of any ailments that have healed
                List<Ailment> healedAilments = new List<Ailment>();
                bool isHealed = false;

                // iterate through ailments
                foreach (KeyValuePair<string, Ailment> ailmentEntry in this.ailments)
                {
                    isHealed = ailmentEntry.Value.UpdateAilment();

                    // add to healedAilments if appropriate
                    if (isHealed)
                    {
                        healedAilments.Add(ailmentEntry.Value);
                    }
                }

                // remove any healed ailments
                if (healedAilments.Count > 0)
                {
                    for (int i = 0; i < healedAilments.Count; i++)
                    {
                        // remove ailment
                        this.ailments.Remove(healedAilments[i].ailmentID);
                    }

                    // clear healedAilments
                    healedAilments.Clear();
                }

                // automatic BABY NAMING
                if (this is NonPlayerCharacter)
                {
                    // check for naming requirement and, if so, assign regent's first name
                    (this as NonPlayerCharacter).CheckNeedsNaming();
                }

                // reset DAYS
                this.days = this.GetDaysAllowance();

                // check for army (don't reset its days yet)
                double armyDays = 0;
                Army myArmy = this.GetArmy();

                // get army days
                if (myArmy != null)
                {
                    armyDays = myArmy.days;
                }

                // reset character days
                this.AdjustDays(0);

                // reset army days if necessary (to enable attrition checks later)
                if (myArmy != null)
                {
                    myArmy.days = armyDays;
                }

            }

        }

        /// <summary>
        /// Calculates the character's fief management rating (i.e. how good they are at managing a fief)
        /// </summary>
        /// <returns>double containing fief management rating</returns>
        public double CalcFiefManagementRating()
        {
            // baseline rating
            double fiefMgtRating = (this.management + this.CalculateStature()) / 2;

            // check for traits effecting fief loyalty
            double fiefLoyTrait = this.CalcTraitEffect(Globals_Game.Stats.FIEFLOY);

            // check for traits effecting fief expenses
            double fiefExpTrait = this.CalcTraitEffect(Globals_Game.Stats.FIEFEXPENSE);

            // combine traits into single modifier. Note: fiefExpTrait is * by -1 because 
            // a negative effect on expenses is good, so needs to be normalised
            double mgtTraits = (fiefLoyTrait + (-1 * fiefExpTrait));

            // calculate final fief management rating
            fiefMgtRating += (fiefMgtRating * mgtTraits);

            return fiefMgtRating;
        }

        /// <summary>
        /// Calculates the character's army leadership rating (i.e. how good they are at leading an army)
        /// </summary>
        /// <returns>double containing army leadership rating</returns>
        public double CalcArmyLeadershipRating()
        {
            // baseline rating
            double armyLeaderRating = (this.management + this.CalculateStature() + this.combat) / 3;

            // check for traits effecting battle
            double battleTraits = this.CalcTraitEffect(Globals_Game.Stats.BATTLE);

            // check for traits effecting siege
            double siegeTraits = this.CalcTraitEffect(Globals_Game.Stats.SIEGE);

            // combine traits into single modifier 
            double combatTraits = battleTraits + siegeTraits;

            // calculate final combat rating
            armyLeaderRating += (armyLeaderRating * combatTraits);

            return armyLeaderRating;
        }

        /// <summary>
        /// Calculates chance and effect of character injuries resulting from a battle
        /// </summary>
        /// <returns>bool indicating whether character has died of injuries</returns>
        /// <param name="armyCasualtyLevel">double indicating friendly army casualty level</param>
        public bool CalculateCombatInjury(double armyCasualtyLevel)
        {
            bool isDead = false;
            uint healthLoss = 0;

            // calculate base chance of injury (based on armyCasualtyLevel)
            double injuryPercentChance = (armyCasualtyLevel * 100);

            // factor in combat trait of character
            injuryPercentChance += 5 - this.combat;

            // ensure chance of injury between 1%-%80
            if (injuryPercentChance < 1)
            {
                injuryPercentChance = 1;
            }
            else if (injuryPercentChance > 80)
            {
                injuryPercentChance = 80;
            }

            // generate random percentage
            int randomPercent = Globals_Game.myRand.Next(101);

            // compare randomPercent with injuryChance to see if injury occurred
            if (randomPercent <= injuryPercentChance)
            {
                // generate random int 1-5 specifying health loss
                healthLoss = Convert.ToUInt32(Globals_Game.myRand.Next(1, 6));
            }

            // check if should create and add an ailment
            if (healthLoss > 0)
            {
                uint minEffect = 0;

                // check if character has died of injuries
                if (this.CalculateHealth() < healthLoss)
                {
                    isDead = true;
                }

                // if not dead, create ailment
                else
                {
                    // check if results in permanent damage
                    if (healthLoss > 4)
                    {
                        minEffect = 1;
                    }

                    // create ailment
                    Ailment myAilment = new Ailment(Globals_Game.GetNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, healthLoss, minEffect);

                    // add to character
                    this.ailments.Add(myAilment.ailmentID, myAilment);
                }

            }

            // =================== if is injured but not dead, create and send JOURNAL ENTRY
            if ((!isDead) && (healthLoss > 0))
            {
                // ID
                uint entryID = Globals_Game.GetNextJournalEntryID();

                // personae
                PlayerCharacter concernedPlayer = null;
                List<string> tempPersonae = new List<string>();

                // add injured character
                tempPersonae.Add(this.charID + "|injuredCharacter");
                if (this is NonPlayerCharacter)
                {
                    if (!String.IsNullOrWhiteSpace(this.familyID))
                    {
                        concernedPlayer = (this as NonPlayerCharacter).GetHeadOfFamily();
                        if (concernedPlayer != null)
                        {
                            tempPersonae.Add(concernedPlayer.charID + "|headOfFamily");
                        }
                    }

                    else if (!String.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
                    {
                        concernedPlayer = (this as NonPlayerCharacter).GetEmployer();
                        if (concernedPlayer != null)
                        {
                            tempPersonae.Add(concernedPlayer.charID + "|employer");
                        }
                    }
                }
                string[] injuryPersonae = tempPersonae.ToArray();

                // location
                string injuryLocation = this.location.id;

                // description
                string[] fields = new string[4];
                fields[0] = this.firstName + " " + this.familyName;
                fields[1] = "";
                if (concernedPlayer != null)
                {
                    fields[1] = ", your " + (this as NonPlayerCharacter).GetFunction(concernedPlayer) + ", ";
                }
                if (healthLoss > 4)
                {
                   fields[2]=  "severe ";
                }
                else if (healthLoss < 2)
                {
                    fields[2]= "light ";
                }
                else
                {
                    fields[2] = "moderate ";
                }
                fields[3] =  this.location.name;

                // create and send JOURNAL ENTRY
                ProtoMessage injury = new ProtoMessage();
                injury.ResponseType = DisplayMessages.CharacterCombatInjury;
                injury.MessageFields = fields;
                JournalEntry injuryEntry = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, injuryPersonae, "injury", injury, loc: injuryLocation);

                // add new journal entry to pastEvents
                Globals_Game.AddPastEvent(injuryEntry);
            }

            return isDead;
        }

        /// <summary>
        /// Gets the fiefs in which the character is the bailiff
        /// </summary>
        /// <returns>List containing the fiefs</returns>
        public List<Fief> GetFiefsBailiff()
        {
            List<Fief> myFiefs = new List<Fief>();

            // get employer
            PlayerCharacter employer = null;
            if (this is PlayerCharacter)
            {
                employer = (this as PlayerCharacter);
            }
            else if (!String.IsNullOrWhiteSpace((this as NonPlayerCharacter).employer))
            {
                employer = (this as NonPlayerCharacter).GetEmployer();
            }
            else if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                employer = this.GetHeadOfFamily();
            }

            if (employer != null)
            {
                // iterate through fiefs, searching for character as bailiff
                foreach (Fief thisFief in employer.ownedFiefs)
                {
                    if (thisFief.bailiff == this)
                    {
                        myFiefs.Add(thisFief);
                    }
                }
            }

            return myFiefs;
        }

        /// <summary>
        /// Gets the armies of which the character is the leader
        /// </summary>
        /// <returns>List<Army> containing the armies</returns>
        public List<Army> GetArmiesLeader()
        {
            List<Army> myArmies = new List<Army>();

            // get employer
            PlayerCharacter employer = null;
            if (this is PlayerCharacter)
            {
                employer = (this as PlayerCharacter);
            }
            else
            {
                employer = (this as NonPlayerCharacter).GetEmployer();
            }

            if (employer != null)
            {
                // iterate through armies, searching for character as leader
                foreach (Army thisArmy in employer.myArmies)
                {
                    if (thisArmy.GetLeader() == this)
                    {
                        myArmies.Add(thisArmy);
                    }
                }
            }

            return myArmies;
        }


        /// <summary>
        /// Preforms conditional checks prior to examining armies in a fief
        /// </summary>
        /// <returns>bool indicating whether to proceed with examination</returns>
        public bool ChecksBefore_ExamineArmies(out ProtoMessage error)
        {
            error = null;
            bool proceed = true;
            int reconDays = 0;
            PlayerCharacter player = null;
            if (this is PlayerCharacter)
            {
                player = (PlayerCharacter)this;
            }
            else
            {
                player = this.GetHeadOfFamily();
                if (player == null)
                {
                    player = (this as NonPlayerCharacter).GetEmployer();
                }
            }
            
            // check if has minimum days
            if (this.days < 1)
            {
                proceed = false;
                if (player!=null)
                {
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                }
            }

            // has minimum days
            else
            {
                // see how long reconnaissance takes
                reconDays = Globals_Game.myRand.Next(1, 4);

                // check if runs out of time
                if (this.days < reconDays)
                {
                    proceed = false;

                    // set days to 0
                    this.AdjustDays(this.days);

                    if (player!=null)
                    {
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericPoorOrganisation;
                    }
                }
                else
                {
                    // if observer NPC, remove from entourage if necessary
                    if (this is NonPlayerCharacter)
                    {
                        if ((this as NonPlayerCharacter).inEntourage)
                        {
                            player.RemoveFromEntourage(this as NonPlayerCharacter);
                        }
                    }

                    // adjust days for recon
                    this.AdjustDays(reconDays);
                }

            }

            return proceed;
        }
		//TODO replace with proto
        /// <summary>
        /// Retrieves information for Character display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="showFullDetails">bool indicating whether to display full character details</param>
        /// <param name="showTitles">bool indicating whether to display character's titles</param>
        /// <param name="observer">Character who is viewing this character's information</param>
        public string DisplayCharacter(bool showFullDetails, bool showTitles, Character observer)
        {
            string charText = "";

            // check to see if is army leader
            if (!String.IsNullOrWhiteSpace(this.armyID))
            {
                charText += "NOTE: This character is currently LEADING AN ARMY (" + this.armyID + ")\r\n\r\n";
            }

            // check to see if is under siege
            if (!String.IsNullOrWhiteSpace(this.location.siege))
            {
                if (this.inKeep)
                {
                    charText += "NOTE: This character is located in a KEEP UNDER SIEGE\r\n\r\n";
                }
            }

            // character ID
            charText += "Character ID: " + this.charID + "\r\n";

            // player ID
            if (this is PlayerCharacter)
            {
                if (!String.IsNullOrWhiteSpace((this as PlayerCharacter).playerID))
                {
                    charText += "Player ID: " + (this as PlayerCharacter).playerID + "\r\n";
                }
            }

            // name
            charText += "Name: " + this.firstName + " " + this.familyName + "\r\n";

            // age
            charText += "Age: " + this.CalcAge() + "\r\n";

            // sex

            charText += "Sex: ";
            if (this.isMale)
            {
                charText += "Male";
            }
            else
            {
                charText += "Female";
            }
            charText += "\r\n";

            // nationality
            charText += "Nationality: " + this.nationality.name + "\r\n";

            if ((this is PlayerCharacter) && (showFullDetails))
            {
                // home fief
                Fief homeFief = (this as PlayerCharacter).GetHomeFief();
                charText += "Home fief: " + homeFief.name + " (" + homeFief.id + ")\r\n";

                // ancestral home fief
                Fief ancHomeFief = (this as PlayerCharacter).GetAncestralHome();
                charText += "Ancestral Home fief: " + ancHomeFief.name + " (" + ancHomeFief.id + ")\r\n";
            }

            // health (& max. health)
            charText += "Health: ";
            if (!this.isAlive)
            {
                charText += "Oops - Dead!";
            }
            else
            {
                charText += this.CalculateHealth() + " (max. health: " + this.maxHealth + ")";
            }
            charText += "\r\n";

            // virility
            if (showFullDetails)
            {
                charText += "Virility: " + this.virility + "\r\n";
            }

            // location
            charText += "Current location: " + this.location.name + " (" + this.location.province.name + ")\r\n";

            // language
            charText += "Language: " + this.language.GetName() + "\r\n";

            // days left
            charText += "Days remaining: " + this.days + "\r\n";

            // stature
            charText += "Stature: " + this.CalculateStature() + "\r\n";
            charText += "  (base stature: " + this.CalculateStature(false) + " | modifier: " + this.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + this.management + "\r\n";

            // combat rating
            charText += "Combat: " + this.combat + "\r\n";

            // traits list
            charText += "Trait:\r\n";
            for (int i = 0; i < this.traits.Length; i++)
            {
                charText += "  - " + this.traits[i].Item1.name + " (level " + this.traits[i].Item2 + ")\r\n";
            }

            // whether inside/outside the keep
            if (this.inKeep)
            {
                charText += "Inside";
            }
            else
            {
                charText += "Outside";
            }
            charText += " the keep\r\n";

            if (showFullDetails)
            {
                // marital status
                NonPlayerCharacter thisSpouse = null;
                charText += "Marital status: ";
                if (!String.IsNullOrWhiteSpace(this.spouse))
                {
                    // get spouse
                    if (Globals_Game.npcMasterList.ContainsKey(this.spouse))
                    {
                        thisSpouse = Globals_Game.npcMasterList[this.spouse];
                    }

                    if (thisSpouse != null)
                    {
                        charText += "happily married to " + thisSpouse.firstName + " " + thisSpouse.familyName;
                        charText += " (ID: " + this.spouse + ").";
                    }
                    else
                    {
                        charText += "apparently married (but your spouse cannot be identified).";
                    }
                }
                else
                {
                    charText += "not married.";
                }
                charText += "\r\n";

                // if pregnant
                if (!this.isMale)
                {
                    charText += "Pregnancy status: ";
                    if (!this.isPregnant)
                    {
                        charText += "not ";
                    }
                    charText += "pregnant\r\n";
                }

                // if spouse pregnant
                else
                {
                    if (thisSpouse != null)
                    {
                        if (thisSpouse.isPregnant)
                        {
                            charText += "Your spouse is pregnant (congratulations!)\r\n";
                        }
                        else
                        {
                            charText += "Your spouse is not pregnant\r\n";
                        }
                    }
                }

                // engaged
                charText += "You are ";
                if (!String.IsNullOrWhiteSpace(this.fiancee))
                {
                    charText += "engaged to be married to ID " + this.fiancee;
                }
                else
                {
                    charText += "not engaged to be married";
                }
                charText += "\r\n";

                // father
                charText += "Father's ID: ";
                if (!String.IsNullOrWhiteSpace(this.father))
                {
                    charText += this.father;
                }
                else
                {
                    charText += "N/A";
                }
                charText += "\r\n";

                // mother
                charText += "Mother's ID: ";
                if (!String.IsNullOrWhiteSpace(this.mother))
                {
                    charText += this.mother;
                }
                else
                {
                    charText += "N/A";
                }
                charText += "\r\n";

                // head of family
                charText += "Head of family's ID: ";
                if (!String.IsNullOrWhiteSpace(this.familyID))
                {
                    charText += this.familyID;
                }
                else
                {
                    charText += "N/A";
                }
                charText += "\r\n";
            }

            // gather additional information for PC/NPC
            bool isPC = this is PlayerCharacter;
            if (isPC)
            {
                if (showFullDetails)
                {
                    charText += (this as PlayerCharacter).DisplayPlayerCharacter();
                }
            }
            else
            {
                charText += (this as NonPlayerCharacter).DisplayNonPlayerCharacter(observer);
            }


            // if TITLES are to be shown
            if (showTitles)
            {
                charText += "\r\n\r\n------------------ TITLES ------------------\r\n\r\n";

                // kingdoms
                foreach (string titleEntry in this.myTitles)
                {
                    // get kingdom
                    Place thisPlace = null;

                    if (Globals_Game.kingdomMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.kingdomMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName(this.language).ToUpper() + " (rank " + thisPlace.rank.id + ") of ";
                        // get kingdom details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }
                charText += "\r\n";

                // provinces
                charText += "PROVINCES:\r\n";
                int provCount = 0;
                foreach (string titleEntry in this.myTitles)
                {
                    // get province
                    Place thisPlace = null;

                    if (Globals_Game.provinceMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.provinceMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName(this.language) + " (rank " + thisPlace.rank.id + ") of ";

                        // get province details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";

                        provCount++;
                    }
                }
                if (provCount < 1)
                {
                    charText += "None\r\n";
                }
                charText += "\r\n";

                // fiefs
                // provinces
                charText += "FIEFS:\r\n";
                foreach (string titleEntry in this.myTitles)
                {
                    // get fief
                    Place thisPlace = null;

                    if (Globals_Game.fiefMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.fiefMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName((thisPlace as Fief).language) + " (rank " + thisPlace.rank.id + ") of ";
                        // get fief details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }
            }

            return charText;
        }

        /// <summary>
        /// Allows a character to propose marriage between himself and a female family member of another player 
        /// </summary>
        /// <returns>bool indicating whether proposal was processed successfully</returns>
        /// <param name="bride">The prospective bride</param>
        public bool ProposeMarriage(Character bride)
        {
            bool success = false;

            // get interested parties
            PlayerCharacter headOfFamilyGroom = this.GetHeadOfFamily();
            PlayerCharacter headOfFamilyBride = bride.GetHeadOfFamily();

            if ((headOfFamilyGroom != null) && (headOfFamilyBride != null))
            {
                // ID
                uint proposalID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
                string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
                string groomEntry = this.charID + "|groom";
                string brideEntry = bride.charID + "|bride";
                string[] myProposalPersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, brideEntry, groomEntry };


                // description
                string[] fields = new string[5];
                fields[0] = headOfFamilyGroom.firstName + " " + headOfFamilyGroom.familyName;
                fields[1] = headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName;
                if (headOfFamilyGroomEntry.Equals(groomEntry))
                {
                    fields[2]= "he";
                }
                else
                {
                    fields[2] =  this.firstName + " " + this.familyName;
                }
                fields[3] =  bride.firstName + " " + bride.familyName;

                // create and send a proposal (journal entry)
                ProtoMessage proposal = new ProtoMessage();
                proposal.ResponseType = DisplayMessages.JournalProposal;
                proposal.MessageFields = fields;
                JournalEntry myProposal = new JournalEntry(proposalID, year, season, myProposalPersonae, "proposalMade",proposal);
                success = Globals_Game.AddPastEvent(myProposal);
                if (success) this.GetHeadOfFamily().activeProposals.Add(this.charID, bride.charID);
            }

            return success;
        }

        //TODO prettify, if have time. A few if-elses goes a long way towards readability
        /// <summary>
        /// Implements conditional checks on the character and his proposed bride prior to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether proposal can proceed</returns>
        /// <param name="bride">The prospective bride</param>
        public bool ChecksBeforeProposal(Character bride,out ProtoMessage error)
        {
            error = null;
            string field  = "";
            PlayerCharacter player = null;
            if (this is PlayerCharacter)
            {
                player = (PlayerCharacter)this;
            }
            else
            {
                player = this.GetHeadOfFamily();
            }
            bool proceed = true;
            DisplayMessages message = DisplayMessages.None;

            // ============= BRIDE
            // check is female
            if (bride.isMale)
            {
                message = DisplayMessages.CharacterProposalMan;
                proceed = false;
            }

            // check is of age
            else
            {
                if (bride.CalcAge() < 14)
                {
                    message = DisplayMessages.CharacterProposalUnderage;
                    field = "bride";
                    proceed = false;
                }

                else
                {
                    // check isn't engaged
                    if (!String.IsNullOrWhiteSpace(bride.fiancee))
                    {
                        message = DisplayMessages.CharacterProposalEngaged;
                        field = "bride";
                        proceed = false;
                    }

                    else
                    {
                        // check isn't married
                        if (!String.IsNullOrWhiteSpace(bride.spouse))
                        {
                            message = DisplayMessages.CharacterProposalMarried;
                            field = "bride";
                            proceed = false;
                        }
                        else
                        {
                            // check is family member of player
                            //if ((bride.GetHeadOfFamily() == null) || (String.IsNullOrWhiteSpace(bride.GetHeadOfFamily().playerID)))
                            if ((bride.GetHeadOfFamily() == null))
                            {
                                message = DisplayMessages.CharacterProposalFamily;
                                field = "bride";
                                proceed = false;
                            }
                            else
                            {
                                // ============= GROOM
                                // check is male
                                if (!this.isMale)
                                {
                                    message = DisplayMessages.CharacterNotMale;
                                    proceed = false;
                                }
                                else
                                {
                                    // check is of age
                                    if (this.CalcAge() < 14)
                                    {
                                        message = DisplayMessages.CharacterProposalUnderage;
                                        field = "groom";
                                        proceed = false;
                                    }
                                    // Ensure not already proposed to someone else
                                    else if (this.GetHeadOfFamily().activeProposals.ContainsKey(this.charID))
                                    {
                                        message = DisplayMessages.CharacterProposalAlready;
                                        proceed = false;
                                    }
                                    else
                                    {
                                        // check is unmarried
                                        if (!String.IsNullOrWhiteSpace(this.spouse))
                                        {
                                            message = DisplayMessages.CharacterProposalMarried;
                                            field = "groom";
                                            proceed = false;
                                        }
                                        else
                                        {
                                            // check isn't engaged
                                            if (!String.IsNullOrWhiteSpace(this.fiancee))
                                            {
                                                message = DisplayMessages.CharacterProposalEngaged;
                                                field = "groom";
                                                proceed = false;
                                            }
                                            else
                                            {
                                                // check is family member of player OR is player themself
                                                if (String.IsNullOrWhiteSpace(this.familyID))
                                                {
                                                    message = DisplayMessages.CharacterProposalFamily;
                                                    field = "groom";
                                                    proceed = false;
                                                }
                                                else
                                                {
                                                    // check isn't in family same family as bride
                                                    if (this.familyID.Equals(bride.familyID))
                                                    {
                                                        message = DisplayMessages.CharacterProposalIncest;
                                                        proceed = false;
                                                    }
                                                }

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                }
            }

            if (!proceed)
            {
                error = new ProtoMessage();
                error.ResponseType = message;
                error.MessageFields = new string[] { field };
            }

            return proceed;
        }

        /// <summary>
        /// Moves character one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool RandomMoveNPC(out ProtoMessage error)
        {
            error = null;
            bool success = false;

            // generate random int 0-6 to see if moves
            int randomInt = Globals_Game.myRand.Next(7);

            if (randomInt > 0)
            {
                // get a destination
                Fief target = Globals_Game.gameMap.chooseRandomHex(this.location);

                // get travel cost
                double travelCost = this.location.getTravelCost(target);

                // perform move
                success = this.MoveCharacter(target, travelCost,out error);
            }

            return success;
        }

        /// <summary>
        /// Moves character sequentially through fiefs stored in goTo queue
        /// </summary>
        /// <returns>bool indicating success</returns>
       public bool CharacterMultiMove(out ProtoMessage error)
        {
            error = null;
            bool success = false;
            double travelCost = 0;
            int steps = this.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                // get travel cost
                travelCost = this.location.getTravelCost(this.goTo.Peek(), this.armyID);
                // attempt to move character
                success = this.MoveCharacter(this.goTo.Peek(), travelCost,out error);
                // if move successfull, remove fief from goTo queue
                if (success)
                {
                    this.goTo.Dequeue();
                }
                // if not successfull, exit loop
                else
                {
                    break;
                }
           }
           //ASK about this condition
           if (this is PlayerCharacter)
           {
               // if player has moved, indicate success
               if (this.goTo.Count < steps)
               {
                   success = true;
               }
           }

           return success;

        }

       /// <summary>
       /// Allows the character to remain in their current location for the specified
       /// number of days, incrementing bailiffDaysInFief if appropriate
       /// </summary>
       /// <returns>bool indicating success</returns>
       /// <param name="campDays">Number of days to camp</param>
       public bool CampWaitHere(byte campDays, out ProtoMessage campMessage)
       {
           campMessage = null;
           bool proceed = true;
           // get army
           Army thisArmy = null;
           thisArmy = this.GetArmy();
           PlayerCharacter player = null;
           if (this is PlayerCharacter)
           {
               player = this as PlayerCharacter;
           }
           else
           {
               player = this.GetHeadOfFamily();
               if (player == null)
               {
                   player = (this as NonPlayerCharacter).GetEmployer();
               }
           }
           // get siege
           Siege thisSiege = null;
           if (thisArmy != null)
           {
               thisSiege = thisArmy.GetSiege();
           }

           if (campDays > this.days)
           {
               campDays = Convert.ToByte(this.days);
           }

           if (proceed)
           {
               // check if player's entourage needs to camp
               bool entourageCamp = false;

               // if character is player, camp entourage
               if (this is PlayerCharacter)
               {
                   entourageCamp = true;
               }

               // if character NOT player
               else
               {
                   // if is in entourage, remove prior to camping
                   if ((this as NonPlayerCharacter).inEntourage)
                   {
                       if (player!=null)
                       {
                           Globals_Game.UpdatePlayer(player.playerID, DisplayMessages.CharacterRemovedFromEntourage, new string[] { this.firstName + " " + this.familyName });
                       }
                       player.RemoveFromEntourage((this as NonPlayerCharacter));
                   }
               }

               // check for siege
               // uses different method to adjust days of all objects involved and apply attrition)
               if (thisSiege != null)
               {
                   thisSiege.SyncSiegeDays(this.days - campDays);
               }

               // if no siege
               else
               {
                   // adjust character's days
                   if (this is PlayerCharacter)
                   {
                       (this as PlayerCharacter).AdjustDays(campDays);
                   }
                   else
                   {
                       this.AdjustDays(campDays);
                   }

                   // inform player
                   if (player!=null)
                   {
                       campMessage = new ProtoMessage();
                       campMessage.ResponseType = DisplayMessages.CharacterCamp;
                       campMessage.MessageFields=new string[] {this.firstName + " " + this.familyName,this.location.name,campDays.ToString()};
                   }

                   // check if character is army leader, if so check for army attrition
                   if (thisArmy != null)
                   {
                       // number of attrition checks
                       byte attritionChecks = 0;
                       attritionChecks = Convert.ToByte(campDays / 7);
                       // total attrition
                       uint totalAttrition = 0;

                       for (int i = 0; i < attritionChecks; i++)
                       {
                           // calculate attrition
                           double attritionModifer = thisArmy.CalcAttrition();
                           // apply attrition
                           if (attritionModifer > 0)
                           {
                               totalAttrition += thisArmy.ApplyTroopLosses(attritionModifer);
                               
                           }
                       }

                       // inform player
                       if (totalAttrition > 0)
                       {
                           if (player!=null)
                           {
                               Globals_Game.UpdatePlayer(player.playerID, DisplayMessages.CharacterCampAttrition, new string[] {this.armyID,totalAttrition.ToString()});
                           }
                       }
                   }
               }

               // keep track of bailiffDaysInFief before any possible increment
               Double bailiffDaysBefore = this.location.bailiffDaysInFief;

               // keep track of identity of bailiff
               Character myBailiff = null;

               // check if character is bailiff of this fief
               if (this.location.bailiff == this)
               {
                   myBailiff = this;
               }

               // if character not bailiff, if appropriate, check to see if anyone in entourage is
               else if (entourageCamp)
               {
                   // if player is bailiff
                   if (player == this.location.bailiff)
                   {
                       myBailiff = player;
                   }
                   // if not, check for bailiff in entourage
                   else
                   {
                       for (int i = 0; i < player.myNPCs.Count; i++)
                       {
                           if (player.myNPCs[i].inEntourage)
                           {
                               if (player.myNPCs[i] != this)
                               {
                                   if (player.myNPCs[i] == this.location.bailiff)
                                   {
                                       myBailiff = player.myNPCs[i];
                                   }
                               }
                           }
                       }
                   }

               }

               // if bailiff identified as someone who camped
               if (myBailiff != null)
               {
                   // increment bailiffDaysInFief
                   this.location.bailiffDaysInFief += campDays;
                   // if necessary, display message to player
                   if (this.location.bailiffDaysInFief >= 30)
                   {
                       // don't display this message if min bailiffDaysInFief was already achieved
                       if (!(bailiffDaysBefore >= 30))
                       {
                           if (player!=null)
                           {
                               Globals_Game.UpdatePlayer(player.playerID, DisplayMessages.CharacterBailiffDuty,new string[]{myBailiff.firstName + " " + myBailiff.familyName,this.location.name});
                           }
                       }
                   }
               }
           }

           return proceed;
       }

       /// <summary>
       /// Allows the character to be moved along a specific route by using direction codes
       /// </summary>
       /// <param name="directions">string[] containing list of sequential directions to follow</param>
       public void TakeThisRoute(string[] directions,out ProtoMessage error)
       {
           error = null;
           bool proceed;
           Fief source = null;
           Fief target = null;
           PlayerCharacter player;
           if (this is PlayerCharacter)
           {
               player = this as PlayerCharacter;
           }
           else
           {
               player = this.GetHeadOfFamily();
               if (player == null)
               {
                   player = (this as NonPlayerCharacter).GetEmployer();
               }
           }
           Queue<Fief> route = new Queue<Fief>();

           // remove from entourage, if necessary
           if (this is NonPlayerCharacter)
           {
               if ((this as NonPlayerCharacter).inEntourage)
               {
                   player.RemoveFromEntourage(this as NonPlayerCharacter);
               }
           }

           // convert to Queue of fiefs
           for (int i = 0; i < directions.Length; i++)
           {
               // source for first move is character's current location
               if (i == 0)
               {
                   source = this.location;
               }
               // source for all other moves is the previous target fief
               else
               {
                   source = target;
               }

               // get the target fief
               target = Globals_Game.gameMap.GetFief(source, directions[i].ToUpper());

               // if target successfully acquired, add to queue
               if (target != null)
               {
                   route.Enqueue(target);
               }
               // if no target acquired, display message and break
               else
               {
                   error = new ProtoMessage();
                   error.ResponseType = DisplayMessages.CharacterInvalidMovement;
                   break;
               }

           }

           // if there are any fiefs in the queue, overwrite the character's goTo queue
           // then process by calling characterMultiMove
           if (route.Count > 0)
           {
               this.goTo = route;
               proceed = this.CharacterMultiMove(out error);
           }
       }
       /// <summary>
       /// Moves the character to a specified fief using the shortest path
       /// </summary>
       /// <param name="fiefID">String containing the ID of the target fief</param>
       public void MoveTo(string fiefID, out ProtoMessage error)
       {
           error = null;
           PlayerCharacter player = null;
           if (this is PlayerCharacter)
           {
               player = this as PlayerCharacter;
           }
           // remove from entourage, if necessary
           if (this is NonPlayerCharacter)
           {
               player = this.GetHeadOfFamily();
               if (player == null)
               {
                   player = (this as NonPlayerCharacter).GetEmployer();
               }
               if ((this as NonPlayerCharacter).inEntourage)
               {
                   player.RemoveFromEntourage(this as NonPlayerCharacter);
               }
           }

           // check for existence of fief
           if (Globals_Game.fiefMasterList.ContainsKey(fiefID))
           {
               // retrieves target fief
               Fief target = Globals_Game.fiefMasterList[fiefID];

               // obtains goTo queue for shortest path to target
               this.goTo = Globals_Game.gameMap.GetShortestPath(this.location, target);

               // if retrieve valid path
               if (this.goTo.Count > 0)
               {
                   // perform move
                   this.CharacterMultiMove(out error);
               }

           }

           // if target fief not found
           else
           {
               error = new ProtoMessage();
               error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
           }

       }
       
       
       /// <summary>
       /// Spy on a fief to obtain information. Note: SpyCheck should be performed first
       /// </summary>
       /// <param name="fief">Fief to spy on</param>
       /// <param name="result"> Full details of spy result, including information if successful and spy status</param>
       /// <returns>boolean indicating spy success</returns>
       public bool SpyOn(Fief fief, out ProtoMessage result)
       {

           // Booleans indicating result
           bool isSuccessful=false;
           bool wasDetected=false;
           bool wasKilled=false;
            
           this.AdjustDays(10);
           result = new ProtoMessage();
           // TOOD Move to config
           // Threshold under which this character will be detected
           double detectedThreshold = 40;
           // Threshold under which this character will be killed //TODO add capture
           double killThreshold = 30;

           // Get random success and escape chances 
           double successChance= Utility_Methods.GetRandomDouble(85,15);
           double escapeChance = Utility_Methods.GetRandomDouble(75, 25);

           // Calculate total chance of success
           double success = GetSpySuccessChance(fief);
           // Check for success
           if (success > successChance)
           {
               isSuccessful=true;
           }
           else
           {
               isSuccessful=false;
           }
           // Check whether detected or killed
           if ((success + escapeChance) / 2 < detectedThreshold)
           {
               wasDetected = true;
           }
           if ((success + escapeChance) / 2 < killThreshold)
           {
               wasKilled = true;
               this.ProcessDeath("spy");
           }
           PlayerCharacter owner = this.GetPlayerCharacter();
           if (isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccessDetected, new string[]{this.firstName+ " "+this.familyName, fief.id});
               Globals_Game.UpdatePlayer(fief.owner.playerID, DisplayMessages.EnemySpySuccess, new string[] { fief.id, owner.firstName + " "+ owner.familyName });
               ProtoFief fiefDetails = new ProtoFief(fief);
               fiefDetails.includeSpy(fief);
                
               result = fiefDetails;
           }
           else if (isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccess, new string[] { this.firstName + " " + this.familyName, fief.id});
               ProtoFief fiefDetails = new ProtoFief(fief);
               fiefDetails.includeSpy(fief);
               result = fiefDetails;
           }
           else if (!isSuccessful && wasKilled)
           {
               Globals_Game.UpdatePlayer(fief.owner.playerID, DisplayMessages.EnemySpyKilled, new string[] { fief.id, owner.firstName + " " + owner.familyName });
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDead, new string[] { this.firstName + " " + this.familyName, fief.id });
           }
           else if (!isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(fief.owner.playerID, DisplayMessages.EnemySpyFail, new string[] { fief.id, owner.firstName + " " + owner.familyName });
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDetected, new string[] { this.firstName + " " + this.familyName,fief.id });
           }
           else if (!isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFail, new string[] { this.firstName + " " + this.familyName,fief.id });
           }
            result.ResponseType = DisplayMessages.Success;
           return isSuccessful;
       }


        // TODO use values from config
        /// <summary>
        /// Get the success chance for spying on a target
        /// </summary>
        /// <param name="target">Target to spy on- currently Fief, Character or Army</param>
        /// <returns>Chance of success</returns>
        public double GetSpySuccessChance(object target)
        {
#if DEBUG
            if (0<=fixedSuccessChance&&fixedSuccessChance <=100)
            {
                return fixedSuccessChance;
            }
#endif
            Type t = target.GetType();
            double baseChance;
            Character perceptiveCharacter = null;

            if (t.IsSubclassOf(typeof(Character)))
            {
                Character character = target as Character;
                if (character == null)
                {
                    return -1;
                }
                baseChance = 40;
                perceptiveCharacter = character;
            }
            else if (t == typeof(Fief))
            {
                Fief fief = target as Fief;
                if (fief == null)
                {
                    return -1;
                }
                baseChance = 40;
                if (fief.bailiff != null)
                {
                    perceptiveCharacter = fief.bailiff;
                }
            }
            else if (t == typeof(Army))
            {
                Army army = target as Army;
                if (army == null)
                {
                    return -1;
                }

                if (!string.IsNullOrWhiteSpace(army.leader))
                {
                    perceptiveCharacter = army.GetLeader();
                }
                baseChance = 30;
            }
            else
            {
                return -1;
            }
            double stealth = CalcTraitEffect(Globals_Game.Stats.STEALTH);
            double enemyPerception = 0;
            if (perceptiveCharacter != null)
            {
                enemyPerception = perceptiveCharacter.CalcTraitEffect(Globals_Game.Stats.PERCEPTION);
            }
            return baseChance + ((stealth - enemyPerception) * 100);
        }

        public bool SpyCheck(Character character, out ProtoMessage result)
        {
            result = null;
            // Cannot spy on captive
            if (!string.IsNullOrWhiteSpace(character.captorID))
            {
                result = new ProtoMessage(DisplayMessages.ErrorSpyCaptive);
                return false;
            }
            // Cannot spy on own character
            if (character.GetPlayerCharacter() == this.GetPlayerCharacter())
            {
                result = new ProtoMessage(DisplayMessages.ErrorSpyOwn);
                return false;
            }
            // Ensure spy is in same location
            if (!this.location.Equals(character.location))
            {
                result = new ProtoMessage(DisplayMessages.ErrorGenericNotInSameFief);
                return false;
            }
            if (this.days < 10)
            {
                result= new ProtoMessage(DisplayMessages.ErrorGenericNotEnoughDays);
                return false;
            }
            // Cannot spy on dead character
            if (!character.isAlive)
            {
                result = new ProtoMessage(DisplayMessages.ErrorSpyDead);
                return false;
            }
            return true;
        }


        public bool SpyCheck(Fief fief, out ProtoMessage result)
        {
            result = null;
            // Ensure spy is in same location
            if (!this.location.Equals(fief))
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                result = error;
                return false;
            }
            // Ensure not trying to spy on own army
            if (fief.owner == this.GetPlayerCharacter())
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorSpyOwn;
                error.MessageFields = new string[] { "fief" };
                result = error;
                return false;
            }
            if (this.days < 10)
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                result = error;
                return false;
            }
            return true;
        }

        public bool SpyCheck(Army army, out ProtoMessage result)
        {
            result = null;
            // Ensure spy is in same location
            if (this.location.id != (army.location))
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                result = error;
                return false;
            }
            // Ensure not trying to spy on own army
            if (army.GetOwner() == this.GetPlayerCharacter())
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorSpyOwn;
                error.MessageFields = new string[] { "army" };
                result = error;
                return false;

            }
            if (this.days < 10)
            {
                ProtoMessage error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                result = error;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Spy on a character to gain additional information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="character">Character to spy on</param>
        /// <param name="result">Returns protomessage containing the full spy result and any information gained</param>
        /// <returns>Bool indicating spy success</returns>
        public bool SpyOn(Character character, out ProtoMessage result)
       {
           // Booleans indicating result
           bool isSuccessful = false;
           bool wasDetected = false;
           bool wasKilled = false;

           // Threshold under which this character will be detected
           double detectedThreshold = 40;
           // Threshold under which this character will be killed //TODO add capture
           double killThreshold = 30;

           result = new ProtoMessage();
           this.AdjustDays(10);
           // Total chance of success
           double success = GetSpySuccessChance(character);
           
           // Get random success and escape chances 
           double successChance = Utility_Methods.GetRandomDouble(85, 15);
           double escapeChance = Utility_Methods.GetRandomDouble(75, 25);
            
           if (success > successChance)
           {
               isSuccessful = true;
           }
           else
           {
               isSuccessful = false;
           }
           // Check whether detected or killed
           if ((success + escapeChance) / 2 < detectedThreshold)
           {
               wasDetected = true;
           }
           if ((success + escapeChance) / 2 < killThreshold)
           {
               wasKilled = true;
               this.ProcessDeath("spy");
           }

           /***Send results**/
           PlayerCharacter owner = this.GetPlayerCharacter();
           PlayerCharacter enemyOwner = character.GetPlayerCharacter();
           if (isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccessDetected, new string[] { this.firstName + " " + this.familyName, character.firstName + " " + character.familyName });
               if (enemyOwner != null)
               {
                   Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpySuccess, new string[] { character.firstName + " " + character.familyName, owner.firstName + " " + owner.familyName });
               }
               if (character is NonPlayerCharacter)
               {
                   ProtoNPC charDetails = new ProtoNPC(character as NonPlayerCharacter);
                   charDetails.includeSpy(character);
                   result = charDetails;
               }
               else
               {
                   ProtoPlayerCharacter charDetails = new ProtoPlayerCharacter(character as PlayerCharacter);
                   charDetails.includeSpy(character);
                   result = charDetails;
               }
               
           }
           else if (isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccess, new string[] { this.firstName + " " + this.familyName,character.firstName + " " + character.familyName });
               if (character is NonPlayerCharacter)
               {
                   ProtoNPC charDetails = new ProtoNPC(character as NonPlayerCharacter);
                   charDetails.includeSpy(character);
                   result = charDetails;
               }
               else
               {
                   ProtoPlayerCharacter charDetails = new ProtoPlayerCharacter(character as PlayerCharacter);
                   charDetails.includeSpy(character);
                   result = charDetails;
               }
           }
           else if (!isSuccessful && wasKilled)
           {
               if (enemyOwner != null)
               {
                   Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpyKilled, new string[] { character.firstName + " " + character.familyName, owner.firstName + " " + owner.familyName });
               
               }
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDead, new string[] { this.firstName + " " + this.familyName, character.firstName + " " + character.familyName});
           }
           else if (!isSuccessful && wasDetected)
           {
               if (enemyOwner != null)
               {
                   Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpyFail, new string[] { character.firstName + " " + character.familyName, owner.firstName + " " + owner.familyName });
               }
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDetected, new string[] { this.firstName + " " + this.familyName,character.firstName + " " + character.familyName });
           }
           else if (!isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFail, new string[] { this.firstName +" "+ this.familyName,character.firstName  + " "+ character.familyName});
           }

            result.ResponseType = DisplayMessages.Success;
           return isSuccessful;
       }

        /// <summary>
        /// Spy on an army to obtain information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="army">Army to spy on</param>
        /// <param name="result">Result of spying, including additional information obtained</param>
        /// <returns>Bool for success</returns>
       public bool SpyOn(Army army, out ProtoMessage result)
       {
           // Booleans indicating result
           bool isSuccessful = false;
           bool wasDetected = false;
           bool wasKilled = false;

           // Threshold under which this character will be detected
           double detectedThreshold = 40;
           // Threshold under which this character will be killed //TODO add capture
           double killThreshold = 30;

           result = new ProtoMessage();
           this.AdjustDays(10);
            // Total chance of success
            double success = GetSpySuccessChance(army);

           // Get random success and escape chances 
           double successChance = Utility_Methods.GetRandomDouble(85, 15);
           double escapeChance = Utility_Methods.GetRandomDouble(75, 25);
            
           if (success > successChance)
           {
               isSuccessful = true;
           }
           else
           {
               isSuccessful = false;
           }
           // Check whether detected or killed
           if ((success + escapeChance) / 2 < detectedThreshold)
           {
               wasDetected = true;
           }
           if ((success + escapeChance) / 2 < killThreshold)
           {
               wasKilled = true;
               this.ProcessDeath("spy");
           }

           /***Send results**/
           PlayerCharacter owner = this.GetPlayerCharacter();
           PlayerCharacter enemyOwner = army.GetOwner();
           string armyDetails = enemyOwner.firstName + " " + enemyOwner.familyName + " (" + army.armyID + ")";
           string myArmyDetails = "your army (" + army.armyID + ")";
           if (isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccessDetected, new string[] { this.firstName + " " + this.familyName, armyDetails });
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpySuccess, new string[] {myArmyDetails, owner.firstName + " " + owner.familyName });
               ProtoArmy armyInfo = new ProtoArmy(army,this);
               armyInfo.includeSpy(army);
               result = armyInfo;
           }
           else if (isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpySuccess, new string[] { this.firstName + " " + this.familyName, armyDetails });
               ProtoArmy armyInfo = new ProtoArmy(army, this);
               armyInfo.includeSpy(army);
               result = armyInfo;
           }
           else if (!isSuccessful && wasKilled)
           {
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpyKilled, new string[] { myArmyDetails, owner.firstName + " " + owner.familyName });
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDead, new string[] { this.firstName + " " + this.familyName, armyDetails});
           }
           else if (!isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemySpyFail, new string[] { myArmyDetails, owner.firstName + " " + owner.familyName });
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFailDetected, new string[] { this.firstName + " " + this.familyName, armyDetails });
           }
           else if (!isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.SpyFail, new string[] { this.firstName + " " + this.familyName, armyDetails });
           }
            result.ResponseType = DisplayMessages.Success;
           return isSuccessful;
       }

        /// <summary>
        /// Kidnap a character
        /// </summary>
        /// <param name="target">Character to kidnap</param>
        /// <param name="result">Result of kidnapping attempt or any errors</param>
        /// <returns>Success</returns>
       public bool Kidnap(Character target, out ProtoMessage result)
       {
           // Cannot kidnap dead person
           if (!target.isAlive) {
               // error
               result = new ProtoMessage();
               result.ResponseType=DisplayMessages.KidnapDead;
               return false;
           }
           // Cannot kidnap own character
           if(target.GetPlayerCharacter()==this.GetPlayerCharacter()) {
               // error
               result=new ProtoMessage();
               result.ResponseType=DisplayMessages.KidnapOwnCharacter;
               return false;
           }
           // target must belong to a player
           // TODO use commented line in final
           //if(target.GetPlayerCharacter()==null||string.IsNullOrWhiteSpace(target.GetPlayerCharacter().playerID)) {
           if (target.GetPlayerCharacter() == null)
           {
               // error
               result=new ProtoMessage();
               result.ResponseType = DisplayMessages.KidnapNoPlayer;
               return false;
           }
           // Cannot already be a captive
           if (!string.IsNullOrWhiteSpace(target.captorID))
           {
               result = new ProtoMessage();
               result.ResponseType = DisplayMessages.CharacterHeldCaptive;
               return false;
           }
           // Booleans indicating result
           bool isSuccessful = false;
           bool wasDetected = false;
           bool wasKilled = false;

           // Threshold under which this character will be detected
           double detectedThreshold = 70;
           double killThreshold = 30;

           result = null;
           this.AdjustDays(10);
           // Get own stealth rating and enemy perception rating (if the army has a leader)
           double stealth = this.CalcTraitEffect(Globals_Game.Stats.STEALTH);
           double enemyPerception = 0;
           enemyPerception = target.CalcTraitEffect(Globals_Game.Stats.PERCEPTION);

           double baseKidnapChance = 30;
           // Total chance of success
           double success = ((stealth - enemyPerception) * 100) + baseKidnapChance;

           // Get random success and escape chances 
           double successChance = Utility_Methods.GetRandomDouble(85, 15);
           double escapeChance = Utility_Methods.GetRandomDouble(75, 25);

           double successModifier = 1;
           // If target is a playercharacter, decrease success by 15%
           if (target is PlayerCharacter)
           {
               successModifier -= 0.15;
           }
           // If target is a family member, decrease success by 10%
           else if (target.GetHeadOfFamily() != null)
           {
               successModifier -= 0.10;
           }
           // If target is in an entourage, decrease by a further 20%
           if (target.GetPlayerCharacter().myEntourage.Contains(target))
           {
               successModifier -= 0.20;
           }
           // If target is leading an army, decrease by 10%
           if (!string.IsNullOrWhiteSpace(target.armyID))
           {
               successModifier -= 0.1;
           }

           success = success * successModifier;
           Console.WriteLine("Kidnap success: " + success + ",SuccessChance: " + successChance + ",EscapeChance: " + escapeChance);
           if (success > successChance)
           {
               isSuccessful = true;
               // Add captive to home fief
               this.GetPlayerCharacter().AddCaptive(target, this.GetPlayerCharacter().GetHomeFief());
           }
           else
           {
               isSuccessful = false;
           }
           // Check whether detected or killed
           Console.WriteLine("Escape chance: " + escapeChance);
           if ((success + escapeChance) / 2 < detectedThreshold)
           {
               Console.WriteLine("Detected");
               wasDetected = true;
           }
           if ((success + escapeChance) / 2 < killThreshold)
           {
               Console.WriteLine("Killed");
               wasKilled = true;
               this.ProcessDeath("kidnap");
           }

           /***Send results**/
           PlayerCharacter owner = this.GetPlayerCharacter();
           PlayerCharacter enemyOwner = target.GetPlayerCharacter();
           string targetName = target.firstName + " " + target.familyName;
           string kidnapperName = this.firstName + " " + this.familyName;
           string kidnapperOwner = owner.firstName + " " + owner.familyName;
           if (isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.KidnapSuccessDetected, new string[] {kidnapperName, targetName });
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemyKidnapSuccessDetected, new string[] { targetName,kidnapperOwner });
           
           }
           else if (isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.KidnapSuccess, new string[] { kidnapperName,targetName });
           }
           else if (!isSuccessful && wasKilled)
           {
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemyKidnapKilled, new string[] { targetName,kidnapperOwner});
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.KidnapFailDead, new string[] {kidnapperName, targetName});
           }
           else if (!isSuccessful && wasDetected)
           {
               Globals_Game.UpdatePlayer(enemyOwner.playerID, DisplayMessages.EnemyKidnapFail, new string[] { targetName,kidnapperOwner});
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.KidnapFailDetected, new string[] { kidnapperName,targetName });
           }
           else if (!isSuccessful)
           {
               Globals_Game.UpdatePlayer(owner.playerID, DisplayMessages.KidnapFail, new string[] { kidnapperName,targetName});
           }
           return isSuccessful;

       }
        /// <summary>
        /// Calculates how much this character can be ransomed for
        /// </summary>
        /// <returns>ransom amount</returns>
       public uint CalculateRansom()
       {
           uint ransom = 0;
           if (this is PlayerCharacter)
           {
               // calculate ransom (10% of total GDP)
               ransom = Convert.ToUInt32(((this as PlayerCharacter).GetTotalGDP() * 0.1));
           }
           else
           {
               string thisFunction = (this as NonPlayerCharacter).GetFunction(this.GetPlayerCharacter());
               ransom = Convert.ToUInt32((this as NonPlayerCharacter).CalcFamilyAllowance(thisFunction));
           }
           return ransom;
       }

        public bool Equals(Character other)
        {
            return charID.Equals(other.charID);
        }
    }

    

    /// <summary>
    /// Class storing data on PlayerCharacter
    /// </summary>
    [ContractVerification(true)]
    public class PlayerCharacter : Character
    {
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// </summary>
        public string playerID { get; set; }
        /// <summary>
        /// Holds character outlawed status
        /// </summary>
        public bool outlawed { get; set; }
        /// <summary>
        /// Holds character's treasury
        /// </summary>
        public uint purse { get; set; }
        /// <summary>
        /// Holds character's employees and family (NonPlayerCharacter objects)
        /// </summary>
        public List<NonPlayerCharacter> myNPCs = new List<NonPlayerCharacter>();
        /// <summary>
        /// Holds character's owned fiefs
        /// </summary>
        public List<Fief> ownedFiefs = new List<Fief>();
        /// <summary>
        /// Holds character's owned provinces
        /// </summary>
        public List<Province> ownedProvinces = new List<Province>();
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public String homeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public String ancestralHomeFief { get; set; }
        /// <summary>
        /// Holds character's armies (Army objects)
        /// </summary>
        public List<Army> myArmies = new List<Army>();
        /// <summary>
        /// Holds character's sieges (siegeIDs)
        /// </summary>
        public List<string> mySieges = new List<string>();
        /// <summary>
        /// Holds Characters in entourage
        /// </summary>
        public List<Character> myEntourage = new List<Character>();
        /// <summary>
        /// Dictionary holding active proposals from family members to other NPCs. Each family member can only propose to one person at a time
        /// </summary>
        public Dictionary<string, string> activeProposals = new Dictionary<string, string>();
        /// <summary>
        /// Holds a list of all characters that have been taken captive (during battle, siege, kidnapping, failed spy attempts etc)
        /// </summary>
        public List<Character> myCaptives = new List<Character>();


        /// <summary>
        /// Constructor for PlayerCharacter
        /// </summary>
        /// <param name="outl">bool holding character outlawed status</param>
        /// <param name="pur">uint holding character purse</param>
        /// <param name="npcs">List(NonPlayerCharacter) holding employees and family of character</param>
        /// <param name="ownedF">List(Fief) holding fiefs owned by character</param>
        /// <param name="ownedP">List(Province) holding provinces owned by character</param>
        /// <param name="home">String holding character's home fief (fiefID)</param>
        /// <param name="anchome">String holding character's ancestral home fief (fiefID)</param>
        /// <param name="pID">String holding ID of player who is currently playing this PlayerCharacter</param>
        /// <param name="myA">List(Army) holding character's armies</param>
        /// <param name="myS">List(string) holding character's sieges (siegeIDs)</param>
        public PlayerCharacter(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, Nationality nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Language lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Trait, int>[] trt, bool inK, bool preg, String famID,
            String sp, String fath, String moth, bool outl, uint pur, List<NonPlayerCharacter> npcs, List<Fief> ownedF, List<Province> ownedP, String home, String ancHome, List<String> myTi, List<Army> myA,
            List<string> myS, string fia, Dictionary<string, Ailment> ails = null, Fief loc = null, String aID = null, String pID = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, trt, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc, aID)
        {
            // VALIDATION
            //TODO exception handling
            // HOME
            // trim and ensure is uppercase
            home = home.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(home))
            {
                throw new InvalidDataException("PlayerCharacter homeFief id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // ANCHOME
            // trim and ensure is uppercase
            ancHome = ancHome.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(ancHome))
            {
                throw new InvalidDataException("PlayerCharacter ancestral homeFief id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // MYSIEGES
            if (myS.Count > 0)
            {
                for (int i = 0; i < myS.Count; i++ )
                {
                    // trim and ensure 1st is uppercase
                    myS[i] = Utility_Methods.FirstCharToUpper(myS[i].Trim());

                    if (!Utility_Methods.ValidateSiegeID(myS[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter siege IDs must have the format 'Siege_' followed by some numbers");
                    }
                }
            }

            this.outlawed = outl;
            this.purse = pur;
            this.myNPCs = npcs;
            this.ownedFiefs = ownedF;
            this.ownedProvinces = ownedP;
            this.homeFief = home;
            this.ancestralHomeFief = ancHome;
            this.playerID = pID;
            this.myArmies = myA;
            this.mySieges = myS;
        }

        /// <summary>
        /// Constructor for PlayerCharacter taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public PlayerCharacter()
		{
		}

		/// <summary>
        /// Constructor for PlayerCharacter using PlayerCharacter_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="pcs">PlayerCharacter_Serialised object to use as source</param>
		public PlayerCharacter(PlayerCharacter_Serialised pcs)
			: base(pcs: pcs)
		{

			this.outlawed = pcs.isOutlawed;
			this.purse = pcs.purse;
            // create empty NPC List, to be populated later
			this.myNPCs = new List<NonPlayerCharacter> ();
            // create empty Fief List, to be populated later
            this.ownedFiefs = new List<Fief>();
            // create empty Province List, to be populated later
            this.ownedProvinces = new List<Province>();
            this.homeFief = pcs.homeFief;
            this.ancestralHomeFief = pcs.ancestralHomeFief;
            this.playerID = pcs.playerID;
            // create empty Army List, to be populated later
            this.myArmies = new List<Army>();
            this.mySieges = pcs.mySieges;
		}

        /// <summary>
        /// Constructor for PlayerCharacter using NonPlayerCharacter object and a PlayerCharacter object,
        /// for use when promoting a deceased PC's heir
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
		/// <param name="pc">PlayerCharacter object to use as source</param>
        public PlayerCharacter(NonPlayerCharacter npc, PlayerCharacter pc)
            : base(npc, "promote", pc.myTitles)
        {
            this.outlawed = false;
            this.purse = pc.purse;
            this.myNPCs = pc.myNPCs;
            this.ownedFiefs = pc.ownedFiefs;
            for (int i = 0; i < this.ownedFiefs.Count; i++ )
            {
                this.ownedFiefs[i].owner = this;
            }
            this.ownedProvinces = pc.ownedProvinces;
            for (int i = 0; i < this.ownedProvinces.Count; i++)
            {
                this.ownedProvinces[i].owner = this;
            }
            this.homeFief = pc.homeFief;
            this.ancestralHomeFief = pc.ancestralHomeFief;
            this.playerID = pc.playerID;
            this.myArmies = pc.myArmies;
            this.mySieges = pc.mySieges;
        }

		//TODO change to Proto
        /// <summary>
        /// Retrieves PlayerCharacter-specific information for Character display 
        /// </summary>
        /// <returns>String containing information to display</returns>
        public string DisplayPlayerCharacter()
        {
            string pcText = "";

            // whether outlawed
            pcText += "You are ";
            if (!this.outlawed)
            {
                pcText += "not ";
            }
            pcText += "outlawed\r\n";

            // purse
            pcText += "Purse: " + this.purse + "\r\n";

            // employees
            pcText += "Family and employees:\r\n";
            for (int i = 0; i < this.myNPCs.Count; i++)
            {
                pcText += "  - " + this.myNPCs[i].firstName + " " + this.myNPCs[i].familyName;
                if (this.myNPCs[i].inEntourage)
                {
                    pcText += " (travelling companion)";
                }
                pcText += "\r\n";
            }

            // owned fiefs
            pcText += "Fiefs owned:\r\n";
            for (int i = 0; i < this.ownedFiefs.Count; i++)
            {
                pcText += "  - " + this.ownedFiefs[i].name + "\r\n";
            }

            // owned provinces
            pcText += "Provinces owned:\r\n";
            for (int i = 0; i < this.ownedProvinces.Count; i++)
            {
                pcText += "  - " + this.ownedProvinces[i].name + "\r\n";
            }

            return pcText;
        }

        /// <summary>
        /// Identifies and returns the PlayerCharacter's heir
        /// </summary>
        /// <returns>The heir (NonPlayerCharacter)</returns>
        public NonPlayerCharacter GetHeir()
        {
            NonPlayerCharacter heir = null;
            List<NonPlayerCharacter> sons = new List<NonPlayerCharacter>();
            List<NonPlayerCharacter> brothers = new List<NonPlayerCharacter>();

            foreach (NonPlayerCharacter npc in this.myNPCs)
            {
                // check for assigned heir
                if (npc.isHeir)
                {
                    heir = npc;
                    break;
                }

                // take note of sons
                else if (npc.GetFunction(this).Equals("Son"))
                {
                    sons.Add(npc);
                }

                // take note of brothers
                else if (npc.GetFunction(this).Equals("Brother"))
                {
                    brothers.Add(npc);
                }
            }

            // if no assigned heir identified
            if (heir == null)
            {
                int age = 0;

                // if there are some sons
                if (sons.Count > 0)
                {
                    foreach (NonPlayerCharacter son in sons)
                    {
                        // if son is older, assign as heir
                        if (son.CalcAge() > age)
                        {
                            heir = son;
                            age = son.CalcAge();
                        }
                    }
                }

                // if there are some brothers
                else if (brothers.Count > 0)
                {
                    foreach (NonPlayerCharacter brother in brothers)
                    {
                        // if brother is older, assign as heir
                        if (brother.CalcAge() > age)
                        {
                            heir = brother;
                            age = brother.CalcAge();
                        }
                    }
                }
            }

            // make sure heir is properly identified
            if (heir != null)
            {
                if (!heir.isHeir)
                {
                    heir.isHeir = true;
                }
            }

            return heir;
        }

        /// <summary>
        /// Returns the siege object associated with the specified siegeID
        /// </summary>
        /// <returns>The siege object</returns>
        /// <param name="id">The siegeID of the siege</param>
        public Siege GetSiege(string id)
        {
            Siege thisSiege = null;

            if (Globals_Game.siegeMasterList.ContainsKey(id))
            {
                thisSiege = Globals_Game.siegeMasterList[id];
            }

            return thisSiege;
        }

        /// <summary>
        /// Returns the current total GDP for all fiefs owned by the PlayerCharacter
        /// </summary>
        /// <returns>The current total GDP</returns>
        public int GetTotalGDP()
        {
            int totalGDP = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                totalGDP += Convert.ToInt32(thisFief.keyStatsCurrent[1]);
            }

            return totalGDP;
        }

        /// <summary>
        /// Finds the highest ranking fief(s) in the PlayerCharacter's owned fiefs
        /// </summary>
        /// <returns>A list containing the highest ranking fief(s)</returns>
        public List<Fief> GetHighestRankingFief()
        {
            List<Fief> highestFiefs = new List<Fief>();
            int highestRank = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                if (thisFief.rank.id > highestRank)
                {
                    // clear existing fiefs
                    if (highestFiefs.Count > 0)
                    {
                        highestFiefs.Clear();
                    }

                    // add fief to list
                    highestFiefs.Add(thisFief);

                    // update highest rank
                    highestRank = thisFief.rank.id;
                }
            }

            return highestFiefs;
        }

        /// <summary>
        /// Processes an offer for employment
        /// </summary>
        /// <returns>bool indicating acceptance of offer</returns>
        /// <param name="npc">NPC receiving offer</param>
        /// <param name="offer">Proposed wage</param>
        public bool ProcessEmployOffer(NonPlayerCharacter npc, uint offer,out ProtoMessage result)
        {
            // The player must have sufficient funds
            if (offer > this.GetHomeFief().GetAvailableTreasury())
            {
                result = new ProtoMessage();
                result.ResponseType = DisplayMessages.ErrorGenericInsufficientFunds;
                return false;
            }
            
            bool accepted = false;

            // get NPC's potential salary
            double potentialSalary = npc.CalcSalary(this);

            // generate random (0 - 100) to see if accepts offer
            double chance = Utility_Methods.GetRandomDouble(100);

            // get 'npcHire' trait effect modifier (increase/decrease chance of offer being accepted)
            double hireTraits = this.CalcTraitEffect(Globals_Game.Stats.NPCHIRE);

            // convert to % to allow easy modification of chance
            hireTraits = (hireTraits * 100);

            // apply to chance
            chance = (chance + (hireTraits * -1));

            // ensure chance is a valid %
            if (chance < 0)
            {
                chance = 0;
            }
            else if (chance > 100)
            {
                chance = 100;
            }


            // get range of negotiable offers
            // minimum = 90% of potential salary, below which all offers rejected
            double minAcceptable = potentialSalary - (potentialSalary / 10);
            // maximum = 110% of potential salary, above which all offers accepted
            double maxAcceptable = potentialSalary + (potentialSalary / 10);
            // get range
            double rangeNegotiable = (maxAcceptable - minAcceptable);
            // ensure this offer is more than the last from this PC
            bool offerLess = false;
            if (npc.lastOffer.ContainsKey(this.charID))
            {
                if (!(offer > npc.lastOffer[this.charID]))
                {
                    offerLess = true;
                }
                // if new offer is greater, over-write previous offer
                else
                {
                    npc.lastOffer[this.charID] = offer;
                }
            }
            // if no previous offer, add new entry
            else
            {
                npc.lastOffer.Add(this.charID, offer);
            }

            // automatically accept if offer > 10% above potential salary
            if (offer > maxAcceptable)
            {
                accepted = true;
                result = new ProtoMessage();
                result.ResponseType = DisplayMessages.CharacterOfferHigh;
            }

            // automatically reject if offer < 10% below potential salary
            else if (offer < minAcceptable)
            {
                accepted = false;
                result = new ProtoMessage();
                result.ResponseType = DisplayMessages.CharacterOfferLow;
            }

            // automatically reject if offer < previous offer
            else if (offerLess)
            {
                accepted = false;
                result = new ProtoMessage();
                result.ResponseType = DisplayMessages.CharacterOfferHaggle;
                result.MessageFields = new string[] { npc.lastOffer[this.charID].ToString() };
            }

            else
            {
                // see where offer lies (as %) within rangeNegotiable
                double offerPercentage = ((offer - minAcceptable) / rangeNegotiable) * 100;
                // compare randomly generated % (chance) with offerPercentage
                if (chance <= offerPercentage)
                {
                    accepted = true;
                    result = new ProtoMessage();
                    result.ResponseType = DisplayMessages.CharacterOfferOk;
                }
                else
                {
                    result = new ProtoMessage();
                    result.ResponseType = DisplayMessages.CharacterOfferAlmost;
                }
            }

            if (accepted)
            {
                // hire this NPC
                this.HireNPC(npc, offer);
            }

            return accepted;
        }

        /// <summary>
        /// Hire an NPC
        /// </summary>
        /// <param name="npc">NPC to hire</param>
        /// <param name="wage">NPC's wage</param>
        public void HireNPC(NonPlayerCharacter npc, uint wage)
        {
            // if was in employ of another PC, fire from that position
            if (!String.IsNullOrWhiteSpace(npc.employer))
            {
                if (!npc.employer.Equals(this.charID))
                {
                    // get previous employer
                    PlayerCharacter oldBoss = npc.GetEmployer();

                    if (oldBoss != null)
                    {
                        oldBoss.FireNPC(npc);
                    }
                }
            }

            // add to employee list
            this.myNPCs.Add(npc);

            // set NPC wage
            npc.salary = wage;

            // set this PC as NPC's boss
            npc.employer = this.charID;

            // remove any offers by this PC from NPCs lastOffer list
            npc.lastOffer.Clear();
        }

        //TODO send success to client
        /// <summary>
        /// Fire an NPC
        /// </summary>
        /// <param name="npc">NPC to fire</param>
        public void FireNPC(NonPlayerCharacter npc)
        {
            // remove from bailiff duties
            List<Fief> fiefsBailiff = npc.GetFiefsBailiff();
            if (fiefsBailiff.Count > 0)
            {
                for (int i = 0; i < fiefsBailiff.Count; i++ )
                {
                    fiefsBailiff[i].bailiff = null;
                }
            }

            // remove from army duties
            List<Army> armiesLeader = npc.GetArmiesLeader();
            if (armiesLeader.Count > 0)
            {
                for (int i = 0; i < armiesLeader.Count; i++ )
                {
                    armiesLeader[i].leader = null;
                }
                npc.armyID = null;
            }

            // take back titles, if appropriate
            if (npc.myTitles.Count > 0)
            {
                List<string> titlesToRemove = new List<string>();
                foreach (string thisTitle in npc.myTitles)
                {
                    Fief titleFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(thisTitle))
                    {
                        titleFief = Globals_Game.fiefMasterList[thisTitle];
                    }

                    if (titleFief != null)
                    {
                        if (titleFief.owner == this)
                        {
                            // fief titleHolder
                            titleFief.titleHolder = this.charID;

                            // add to PC myTitles
                            this.myTitles.Add(thisTitle);

                            // mark title for removal
                            titlesToRemove.Add(thisTitle);
                        }
                    }
                }

                // remove from NPC titles
                if (titlesToRemove.Count > 0)
                {
                    foreach (string thisTitle in titlesToRemove)
                    {
                        npc.myTitles.Remove(thisTitle);
                    }
                }
                titlesToRemove.Clear();
            }

            // remove from employee list
            this.myNPCs.Remove(npc);

            // set NPC wage to 0
            npc.salary = 0;

            // remove this PC as NPC's boss
            npc.employer = null;

            // remove NPC from entourage
            RemoveFromEntourage(npc);

            // if NPC has entries in goTo, clear
            if (npc.goTo.Count > 0)
            {
                npc.goTo.Clear();
            }
        }

        /// <summary>
        /// Adds an NPC to the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be added</param>
        public void AddToEntourage(NonPlayerCharacter npc)
        {
            // if NPC has entries in goTo, clear
            if (npc.goTo.Count > 0)
            {
                npc.goTo.Clear();
            }
            lock (entourageLock)
            {
                // keep track of original days value for PC
                double myDays = this.days;

                // ensure days are synchronised
                double minDays = Math.Min(this.days, npc.days);
                this.days = minDays;
                npc.days = minDays;

                // add to entourage
                npc.setEntourage(true);
                this.myEntourage.Add(npc);
                // ensure days of entourage are synched with PC
                if (this.days != myDays)
                {
                    this.AdjustDays(0);
                }
            }
           
        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be removed</param>
        public void RemoveFromEntourage(NonPlayerCharacter npc)
        {
            lock (entourageLock)
            {
                //remove from entourage
                npc.setEntourage(false);
                this.myEntourage.Remove(npc);
            }
        }

        /// <summary>
        /// Adds a Fief to the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be added</param>
        public void AddToOwnedFiefs(Fief f)
        {
            // add fief
            this.ownedFiefs.Add(f);
        }

        /// <summary>
        /// Adds a Province to the character's list of owned provinces
        /// </summary>
        /// <param name="p">Province to be added</param>
        public void AddToOwnedProvinces(Province p)
        {
            // add fief
            this.ownedProvinces.Add(p);
        }

        /// <summary>
        /// Removes a Fief from the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be removed</param>
        public void RemoveFromOwnedFiefs(Fief f)
        {
            // remove fief
            this.ownedFiefs.Remove(f);
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to enter keep (if not barred).
        /// Then moves entourage (if not individually barred). Ignores nationality bar
        /// for entourage if PlayerCharacter allowed to enter
        /// </summary>
        /// <returns>bool indicating success</returns>
        public override bool EnterKeep(out ProtoMessage error)
        {
            error = null;
            // invoke base method for PlayerCharacter
            bool success = base.EnterKeep(out error);

            // if PlayerCharacter enters keep
            if (success)
            {
                // iterate through employees
                for (int i = 0; i < this.myNPCs.Count; i++)
                {
                    // if employee in entourage, allow to enter keep unless individually barred
                    if (this.myNPCs[i].inEntourage)
                    {
                        if (location.barredCharacters.Contains(this.myNPCs[i].charID))
                        {
                            this.myNPCs[i].inKeep = false;
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterBarredKeep;
                        }
                        else
                        {
                            this.myNPCs[i].inKeep = true;
                        }
                    }

                }
                
            }

            return success;

        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to exit keep. Then exits entourage.
        /// </summary>
        public override bool ExitKeep()
        {
            // invoke base method for PlayerCharacter
            bool success = base.ExitKeep();

            // iterate through employees
            for (int i = 0; i < this.myNPCs.Count; i++)
            {
                // if employee in entourage, exit keep
                if (this.myNPCs[i].inEntourage)
                {
                    this.myNPCs[i].inKeep = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to synchronise the days of their entourage
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public override void AdjustDays(Double daysToSubtract)
        {
            // use base method to subtract days from PlayerCharacter
            base.AdjustDays(daysToSubtract);

            // iterate through employees
            for (int i = 0; i < this.myNPCs.Count; i++)
            {
                // if employee in entourage, set NPC days to same as player
                if (this.myNPCs[i].inEntourage)
                {
                    this.myNPCs[i].days = this.days;
                }
            }

        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to target fief. Then moves entourage.
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public override bool MoveCharacter(Fief target, double cost, out ProtoMessage error, bool siegeCheck = true)
        {

            // use base method to move PlayerCharacter
            bool success = base.MoveCharacter(target, cost, out error,siegeCheck);

            // if PlayerCharacter move successfull
            if (success)
            {
                // iterate through employees
                for (int i = 0; i < this.myNPCs.Count; i++)
                {
                    // if employee in entourage, move employee
                    if (this.myNPCs[i].inEntourage)
                    {
                        this.MoveEntourageNPC(target, this.myNPCs[i]);
                    }
                }
            }

            return success;

        }

        /// <summary>
        /// Moves an NPC in a player's entourage (i.e. sets new location)
        /// </summary>
        /// <param name="target">Target fief</param>
        /// <param name="npc">NonPlayerCharacter to move</param>
        public void MoveEntourageNPC(Fief target, NonPlayerCharacter npc)
        {
            // remove character from current fief's character list
            npc.location.RemoveCharacter(npc);
            // set location to target fief
            npc.location = target;
            // add character to target fief's character list
            npc.location.AddCharacter(npc);
            // arrives outside keep
            npc.inKeep = false;
        }

        /// <summary>
        /// Carries out conditional checks prior to recruitment
        /// </summary>
        /// <returns>bool indicating whether recruitment can proceed</returns>
        public bool ChecksBeforeRecruitment(out ProtoMessage error)
        {
            error = null;
            bool proceed = true;
            int indivTroopCost = 0;

            // get home fief
            Fief homeFief = this.GetHomeFief();

            // calculate cost of individual soldier
            if (this.location.ancestralOwner == this)
            {
                indivTroopCost = 500;
            }
            else
            {
                indivTroopCost = 2000;
            }

            // 1. see if fief owned by player
            if (this.location.owner != this)
            {
                proceed = false;
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.CharacterRecruitOwn;
            }
            else
            {
                // 2. see if recruitment already occurred for this season
                if (this.location.hasRecruited)
                {
                    proceed = false;

                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.CharacterRecruitAlready;
                }
                else
                {
                    // 3. Check language and loyalty permit recruitment
                    if ((this.language.baseLanguage != this.location.language.baseLanguage)
                        && (this.location.loyalty < 7))
                    {
                        proceed = false;
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterLoyaltyLanguage;
                    }
                    else
                    {
                        // 4. check sufficient funds for at least 1 troop
                        if (!(homeFief.GetAvailableTreasury() > indivTroopCost))
                        {
                            proceed = false;
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ArmyMaintainInsufficientFunds;
                        }
                        else
                        {
                            // 5. check minimum days remaining
                            if (this.days < 1)
                            {
                                proceed = false;
                                error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                            }
                            else
                            {
                                // 6. check for siege
                                if (!String.IsNullOrWhiteSpace(this.location.siege))
                                {
                                    proceed = false;
                                    error = new ProtoMessage();
                                    error.ResponseType = DisplayMessages.CharacterRecruitSiege;
                                }
                                else
                                {
                                    // 7. check for rebellion
                                    if (this.location.status.Equals('R'))
                                    {
                                        proceed = false;
                                        error = new ProtoMessage();
                                        error.ResponseType = DisplayMessages.CharacterRecruitRebellion;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Recruits troops from the current fief
        /// </summary>
        /// <returns>uint containing number of troops recruited</returns>
        /// <param name="number">How many troops to recruit</param>
        /// <param name="thisArmy">Army to recruit into- null to create new army</param>
        /// <param name="isConfirm">Whether or not this action has been confirmed by client</param>
        public ProtoMessage RecruitTroops(uint number, Army thisArmy, bool isConfirm)
        {
            bool armyExists = (thisArmy != null);
            // used to record outcome of various checks
            bool proceed = true;

            int troopsRecruited = 0;
            int revisedRecruited = 0;
            int indivTroopCost = 0;
            int troopCost = 0;
            int daysUsed = 0;

            // get home fief
            Fief homeFief = this.GetHomeFief();

            // calculate cost of individual soldier
            if (this.location.ancestralOwner == this)
            {
                indivTroopCost = 500;
            }
            else
            {
                indivTroopCost = 2000;
            }


            // various checks to see whether to proceed
            ProtoMessage error = null;
            proceed = this.ChecksBeforeRecruitment(out error);

            // if have not passed all of checks above, return
            if (!proceed)
            {
                return error;
            }

            // actual days taken
            // see how long recuitment attempt will take: generate random int (1-5)
            daysUsed = Globals_Game.myRand.Next(1, 6);

            if (this.days < daysUsed)
            {
                proceed = false;
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericPoorOrganisation;
            }

            //TODO move troop calculatioms + confirmation to client
            if (proceed)
            {
                // calculate potential cost
                troopCost = Convert.ToInt32(number) * indivTroopCost;
                //TODO client confirm revised troop numbers
                // check to see if can afford the specified number of troops
                // if can't afford specified number
                if (!(homeFief.GetAvailableTreasury() >= troopCost))
                {
                    // work out how many troops can afford
                    double roughNumber = homeFief.GetAvailableTreasury() / indivTroopCost;
                    revisedRecruited = Convert.ToInt32(Math.Floor(roughNumber));
                    //Return revised number, ask client to submit new amount
                    ProtoRecruit recruitDetails = new ProtoRecruit();
                    recruitDetails.ResponseType = DisplayMessages.CharacterRecruitInsufficientFunds;
                    recruitDetails.MessageFields = new string[] { number.ToString(), revisedRecruited.ToString() };
                    recruitDetails.amount = Convert.ToUInt32(revisedRecruited);
                    recruitDetails.cost = revisedRecruited * indivTroopCost;
                    recruitDetails.armyID = thisArmy.armyID;
                    return recruitDetails;
                }

                if (proceed)
                {
                    // calculate number of troops responding to call (based on fief population)
                    troopsRecruited = this.location.CallUpTroops(minProportion: 0.4);

                    // adjust if necessary
                    if (troopsRecruited >= number)
                    {
                        troopsRecruited = Convert.ToInt32(number);

                        // calculate total cost
                        troopCost = troopsRecruited * indivTroopCost;

                    }
                    // if less than specified number respond to call
                    else
                    {
                        // calculate total cost
                        troopCost = troopsRecruited * indivTroopCost;
                    }
                    if (!isConfirm)
                    {
                        ProtoRecruit recruitmentDetails = new ProtoRecruit();
                        recruitmentDetails.armyID = thisArmy.armyID;
                        recruitmentDetails.ResponseType = DisplayMessages.CharacterRecruitOk;
                        recruitmentDetails.amount = Convert.ToUInt32(troopsRecruited);
                        recruitmentDetails.cost = troopCost;
                        recruitmentDetails.treasury = this.GetHomeFief().Treasury;
                        recruitmentDetails.MessageFields = new string[] { recruitmentDetails.amount.ToString(), recruitmentDetails.cost.ToString(), recruitmentDetails.treasury.ToString() };
                        return recruitmentDetails;
                    }
                    // if no existing army, create one
                    if (!armyExists)
                    {
                        // if necessary, exit keep (new armies are created outside keep)
                        if (this.inKeep)
                        {
                            this.ExitKeep();
                        }

                        thisArmy = new Army(Globals_Game.GetNextArmyID(), null, this.charID, this.days, this.location.id);
                        thisArmy.AddArmy();
                    }

                    // deduct cost of troops from treasury
                    homeFief.AdjustTreasury(-troopCost);

                    // get army nationality
                    string thisNationality = this.nationality.natID;

                    // work out how many of each type recruited
                    uint[] typesRecruited = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
                    uint totalSoFar = 0;
                    for (int i = 0; i < typesRecruited.Length; i++)
                    {
                        // work out 'trained' troops numbers
                        if (i < typesRecruited.Length - 1)
                        {
                            typesRecruited[i] = Convert.ToUInt32(troopsRecruited * Globals_Server.recruitRatios[thisNationality][i]);
                            totalSoFar += typesRecruited[i];
                        }
                        // fill up with rabble
                        else
                        {
                            typesRecruited[i] = Convert.ToUInt32(troopsRecruited) - totalSoFar;
                        }
                    }
                    for (int i = 0; i < thisArmy.troops.Length; i++)
                    {
                        thisArmy.troops[i] += typesRecruited[i];
                    }

                    // indicate recruitment has occurred in this fief
                    this.location.hasRecruited = true;
                }
            }

            // update character's days
            this.AdjustDays(daysUsed);

            {
                ProtoRecruit recruitDetails = new ProtoRecruit();
                recruitDetails.armyID = thisArmy.armyID;
                recruitDetails.ResponseType = DisplayMessages.Success;
                recruitDetails.MessageFields = new string[] { troopsRecruited.ToString(), troopCost.ToString() };
                return recruitDetails;
            }
        }

        /// <summary>
        /// Gets character's kingdom
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetKingdom()
        {
            Kingdom myKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                // get kingdom with matching nationality
                if (kingdomEntry.Value.nationality == this.nationality)
                {
                    myKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return myKingdom;
        }

        /// <summary>
        /// Gets PlayerCharacter's king
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetKing()
        {
            PlayerCharacter myKing = null;
            Kingdom myKingdom = this.GetKingdom();

            if (myKingdom != null)
            {
                if (myKingdom.owner != null)
                {
                    myKing = myKingdom.owner;
                }
            }

            return myKing;
        }

        /// <summary>
        /// Gets character's queen
        /// </summary>
        /// <returns>The queen</returns>
        public NonPlayerCharacter GetQueen()
        {
            NonPlayerCharacter myQueen = null;

            // get king
            PlayerCharacter myKing = this.GetKing();

            if (myKing != null)
            {
                // get queen
                if (!String.IsNullOrWhiteSpace(myKing.spouse))
                {
                    if (Globals_Game.npcMasterList.ContainsKey(myKing.spouse))
                    {
                        myQueen = Globals_Game.npcMasterList[myKing.spouse];
                    }
                }
            }

            return myQueen;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a king
        /// </summary>
        /// <returns>bool indicating whether is a king</returns>
        public bool CheckIsKing()
        {
            bool isKing = false;

            if ((this == Globals_Game.kingOne) || (this == Globals_Game.kingTwo))
            {
                isKing = true;
            }

            return isKing;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a prince
        /// </summary>
        /// <returns>bool indicating whether is a prince</returns>
        public bool CheckIsPrince()
        {
            bool isPrince = false;

            if ((this == Globals_Game.princeOne) || (this == Globals_Game.princeTwo))
            {
                isPrince = true;
            }

            return isPrince;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a herald
        /// </summary>
        /// <returns>bool indicating whether is a herald</returns>
        public bool CheckIsHerald()
        {
            bool isHerald = false;

            if ((this == Globals_Game.heraldOne) || (this == Globals_Game.heraldTwo))
            {
                isHerald = true;
            }

            return isHerald;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a sysAdmin
        /// </summary>
        /// <returns>bool indicating whether is a sysAdmin</returns>
        public bool CheckIsSysAdmin()
        {
            return (this == Globals_Game.sysAdmin);
        }

        /// <summary>
        /// Returns the PlayerCharacter's home fief
        /// </summary>
        /// <returns>The home fief</returns>
        public Fief GetHomeFief()
        {
            Fief thisHomeFief = null;

            if (!String.IsNullOrWhiteSpace(this.homeFief))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.homeFief))
                {
                    thisHomeFief = Globals_Game.fiefMasterList[this.homeFief];
                }
            }

            return thisHomeFief;
        }

        /// <summary>
        /// Returns the PlayerCharacter's ancestral home fief
        /// </summary>
        /// <returns>The ancestral home fief</returns>
        public Fief GetAncestralHome()
        {
            Fief ancestralHome = null;

            if (!String.IsNullOrWhiteSpace(this.ancestralHomeFief))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.ancestralHomeFief))
                {
                    ancestralHome = Globals_Game.fiefMasterList[this.ancestralHomeFief];
                }
            }

            return ancestralHome;
        }

        
        /// <summary>
        /// Transfers the specified title to the specified character
        /// </summary>
        /// <param name="newTitleHolder">The new title holder</param>
        /// <param name="titlePlace">The place to which the title refers</param>
        public void TransferTitle(Character newTitleHolder, Place titlePlace)
        {
            Character oldTitleHolder = titlePlace.GetTitleHolder();

            // remove title from existing holder
            if (oldTitleHolder != null)
            {
                oldTitleHolder.myTitles.Remove(titlePlace.id);
            }

            // add title to new holder
            newTitleHolder.myTitles.Add(titlePlace.id);
            titlePlace.titleHolder = newTitleHolder.charID;

            // CREATE JOURNAL ENTRY
            // get interested parties
            bool success = true;
            PlayerCharacter placeOwner = titlePlace.owner;

            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(placeOwner.charID + "|placeOwner");
            tempPersonae.Add(newTitleHolder.charID + "|newTitleHolder");
            if ((oldTitleHolder != null) && (oldTitleHolder != placeOwner))
            {
                tempPersonae.Add(oldTitleHolder.charID + "|oldTitleHolder");
            }
            if (titlePlace is Province)
            {
                tempPersonae.Add("all|all");
            }
            string[] thisPersonae = tempPersonae.ToArray();

            // type
            string type = "";
            if (titlePlace is Fief)
            {
                type += "grantTitleFief";
            }
            else if (titlePlace is Province)
            {
                type += "grantTitleProvince";
            }

            // location
            string location = titlePlace.id;

            // description
            string[] fields = new string[4];
            if (titlePlace is Fief)
            {
                fields[0]= "fief";
            }
            else if (titlePlace is Province)
            {
                fields[0] =  "province";
            }
            fields[1] = titlePlace.name;
            if ((newTitleHolder == placeOwner) && (oldTitleHolder != null))
            {
                fields[3] = ".";
                fields[2] = "removed by His Royal Highness " + this.firstName + " " + this.familyName + " from the previous holder "+ oldTitleHolder.firstName + " " + oldTitleHolder.familyName;
            }
            else
            {
                fields[2] =  "granted by its owner "+ placeOwner.firstName + " " + placeOwner.familyName + " to "+newTitleHolder.firstName + " " + newTitleHolder.familyName;
                if ((oldTitleHolder != null) && (oldTitleHolder != placeOwner))
                {
                    fields[3] = "; This has necessitated the removal of "+oldTitleHolder.firstName + " " + oldTitleHolder.familyName + " from the title";
                }
                else
                {
                    fields[3] = ".";
                }
            }

            // create and add a journal entry to the pastEvents journal
            ProtoMessage titleTrans = new ProtoMessage();
            titleTrans.ResponseType = DisplayMessages.CharacterTransferTitle;
            titleTrans.MessageFields = fields;
            JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type, titleTrans,loc: location);
            success = Globals_Game.AddPastEvent(thisEntry);
        }

        /// <summary>
        /// Transfers the title of a fief or province to another character
        /// </summary>
        /// <remarks>
        /// Predicate: assumes newHolder is male
        /// Predicate: assumes newHolder age 14 or over 
        /// </remarks>
        /// <returns>bool indicating success</returns>
        /// <param name="newHolder">The character receiving the title</param>
        /// <param name="titlePlace">The place to which the title refers</param>
        /// <param name="result">The result of attempting to grant</param>
        public bool GrantTitle(Character newHolder, Place titlePlace, out ProtoMessage result)
        {
            result = null;
            bool proceed = true;
            DisplayMessages toDisplay = DisplayMessages.None;

            // only fiefs or provinces
            if ((titlePlace is Fief) || (titlePlace is Province))
            {
                // CHECKS
                // ownership (must be owner)
                if (!(this == titlePlace.owner))
                {
                    toDisplay = DisplayMessages.CharacterTitleOwner;
                    proceed = false;
                }

                else
                {
                    // can't give away highest ranking place
                    List<Place> highestPlaces = this.GetHighestRankPlace();
                    if (highestPlaces.Count > 0)
                    {
                        if (highestPlaces[0].rank.stature == titlePlace.rank.stature)
                        {
                            if (highestPlaces.Count == 1)
                            {
                                toDisplay = DisplayMessages.CharacterTitleHighest;
                                proceed = false;
                            }
                        }
                    }

                    if (proceed)
                    {
                        // fief ancestral ownership (only king can give away fief ancestral titles)
                        if (titlePlace is Fief)
                        {
                            if (titlePlace.owner.charID.Equals((titlePlace as Fief).ancestralOwner.charID))
                            {
                                // check if king
                                if ((titlePlace as Fief).ancestralOwner != (titlePlace as Fief).province.kingdom.owner)
                                {
                                    toDisplay = DisplayMessages.CharacterTitleAncestral;
                                    proceed = false;
                                }
                            }
                        }

                        // provinces can only be given by king
                        else if (titlePlace is Province)
                        {
                            if ((titlePlace as Province).owner != (titlePlace as Province).kingdom.owner)
                            {
                                toDisplay = DisplayMessages.CharacterTitleKing;
                                proceed = false;
                            }
                        }
                    }
                }

                if (proceed)
                {
                    this.TransferTitle(newHolder, titlePlace);
                }
                else
                {
                    result = new ProtoMessage();
                    result.ResponseType = toDisplay;
                }
            }

            return proceed;
        }

        /// <summary>
        /// Gets the total population of fiefs governed by the PlayerCharacter
        /// </summary>
        /// <returns>int containing total population</returns>
        public int GetMyPopulation()
        {
            int totalPop = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                totalPop += thisFief.population;
            }

            return totalPop;
        }

        /// <summary>
        /// Gets the percentage of population in the game governed by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of population governed</returns>
        public double GetPopulationPercentage()
        {
            double popPercent = 0;

            popPercent = (Convert.ToDouble(this.GetMyPopulation()) / Globals_Game.GetTotalPopulation()) * 100;

            return popPercent;
        }

        /// <summary>
        /// Gets the percentage of total fiefs in the game owned by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of total fiefs owned</returns>
        public double GetFiefsPercentage()
        {
            double fiefPercent = 0;

            fiefPercent = (Convert.ToDouble(this.ownedFiefs.Count) / Globals_Game.GetTotalFiefs()) * 100;

            return fiefPercent;
        }

        /// <summary>
        /// Gets the percentage of total money in the game owned by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of total money owned</returns>
        public double GetMoneyPercentage()
        {
            double moneyPercent = 0;

            moneyPercent = (Convert.ToDouble(this.GetMyMoney()) / Globals_Game.GetTotalMoney()) * 100;

            return moneyPercent;
        }

        /// <summary>
        /// Calculates the total funds currently owned by the PlayerCharacter
        /// </summary>
        /// <returns>int containing the total funds</returns>
        public int GetMyMoney()
        {
            int totalFunds = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                totalFunds += thisFief.Treasury;
            }

            return totalFunds;
        }

        /// <summary>
        /// Elects a new leader from NPCs accompanying an army (upon death of PC leader)
        /// </summary>
        /// <returns>The new leader</returns>
        public NonPlayerCharacter ElectNewArmyLeader()
        {
            NonPlayerCharacter newLeader = null;

            double highestRating = 0;

            foreach (NonPlayerCharacter candidate in this.myNPCs)
            {
                double armyLeaderRating = candidate.CalcArmyLeadershipRating();

                if (armyLeaderRating > highestRating)
                {
                    highestRating = armyLeaderRating;
                    newLeader = candidate;
                }
            }

            return newLeader;
        }

        /// <summary>
        /// Add a captive to a fief's gaol
        /// </summary>
        /// <param name="captive"></param>
        /// <param name="fief"></param>
        public void AddCaptive(Character captive, Fief fief)
        {
            // Move captive and add to gaol
            myCaptives.Add(captive);
            captive.captorID = this.charID;
            ProtoMessage ignore;
            captive.MoveCharacter(fief, 0, out ignore);
            captive.inKeep = false;
            fief.gaol.Add(captive);

            // Remove char as bailiff;
            List<Fief> bailiffFiefs = captive.GetFiefsBailiff();
            foreach (Fief f in bailiffFiefs)
            {
                f.bailiff = null;
            }

            // Remove char as army leader
            if (!string.IsNullOrWhiteSpace(captive.armyID))
            {
                captive.GetArmy().leader = null;
                captive.armyID = null;
            }
        }

        /// <summary>
        /// Kill the specified captive and update the captive's family/employer of the death
        /// </summary>
        /// <param name="captive">Captive to be executted</param>
        public void ExecuteCaptive(Character captive)
        {
            captive.location.gaol.Remove(captive);
            myCaptives.Remove(captive);
            captive.ProcessDeath("execute");
            Globals_Game.UpdatePlayer(captive.GetPlayerCharacter().playerID, DisplayMessages.CharacterExecuted, new string[] { captive.firstName + " " + captive.familyName });
            
        }

        /// <summary>
        /// Send a ransom to the family/employer of one of your captives
        /// </summary>
        /// <param name="captive">Captive to be ransomed</param>
        public void RansomCaptive(Character captive)
        {
            uint ransom = captive.CalculateRansom();
            string captor = this.charID + "|Captor";
            string thisCaptive = captive.charID + "|Captive";
            string headOfCaptive = captive.GetPlayerCharacter().charID + "|HeadOfCaptiveFamily";
            string[] personae = new string[]{captor,thisCaptive, headOfCaptive};
            ProtoMessage ransomMessage = new ProtoMessage();
            ransomMessage.MessageFields=new string [] {captive.firstName + " " + captive.familyName, ransom.ToString()};
            JournalEntry entry = new JournalEntry(Globals_Game.GetNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, personae, "ransom", ransomMessage);
            Globals_Game.AddPastEvent(entry);
            captive.ransomDemand = ""+entry.jEntryID;
            // Update player and captive owner of ransom
            Globals_Game.UpdatePlayer(captive.GetPlayerCharacter().playerID, DisplayMessages.RansomReceived, new string[] { captive.firstName + " " + captive.familyName });
        }

        /// <summary>
        /// Releases one of your captives. The captive will immediately be transported to their employer/family's home fief
        /// </summary>
        /// <param name="captive">The captive to be released</param>
        public void ReleaseCaptive(Character captive)
        {
            captive.location.gaol.Remove(captive);
            myCaptives.Remove(captive);
            captive.captorID = null;
            // Send captive to home fief (avoids being recaptured repeatedly)
            ProtoMessage ignore;
            captive.ransomDemand = null;
            captive.MoveCharacter(captive.GetPlayerCharacter().GetHomeFief(), 0, out ignore, false);
            Globals_Game.UpdatePlayer(captive.GetPlayerCharacter().playerID, DisplayMessages.CharacterReleased, new string[] { captive.firstName + " " + captive.familyName });
        }
    }


    /// <summary>
    /// Class storing data on NonPlayerCharacter
    /// </summary>
    public class NonPlayerCharacter : Character
    {

        /// <summary>
        /// Holds NPC's employer (charID)
        /// </summary>
        public String employer { get; set; }
        /// <summary>
        /// Holds NPC's salary
        /// </summary>
        public uint salary { get; set; }
        /// <summary>
        /// Holds last wage offer from individual PCs
        /// </summary>
        public Dictionary<string, uint> lastOffer { get; set; }
        /// <summary>
        /// Denotes if in employer's entourage
        /// </summary>
        public bool inEntourage { get; protected set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool isHeir { get; set; }

        /// <summary>
        /// Constructor for NonPlayerCharacter
        /// </summary>
        /// <param name="empl">String holding NPC's employer (charID)</param>
        /// <param name="sal">string holding NPC's wage</param>
        /// <param name="inEnt">bool denoting if in employer's entourage</param>
        /// <param name="isH">bool denoting if is player's heir</param>
        public NonPlayerCharacter(String id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, Nationality nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Language lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Trait, int>[] trt, bool inK, bool preg, String famID,
            String sp, String fath, String moth, uint sal, bool inEnt, bool isH, List<String> myTi, string fia, Dictionary<string, Ailment> ails = null, Fief loc = null, String aID = null, String empl = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, trt, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc, aID)
        {
            // VALIDATION
            // EMPL
            if (!String.IsNullOrWhiteSpace(empl))
            {
                //TODO exception handling
                // trim and ensure 1st is uppercase
                empl = Utility_Methods.FirstCharToUpper(empl.Trim());

                if (!String.IsNullOrWhiteSpace(famID))
                {
                    throw new InvalidDataException("A NonPlayerCharacter with a familyID cannot have an employer ID");
                }

                else if (!Utility_Methods.ValidateCharacterID(empl))
                {
                    throw new InvalidDataException("NonPlayerCharacter employer ID must have the format 'Char_' followed by some numbers");
                }
            }

            this.employer = empl;
            this.salary = sal;
            this.inEntourage = inEnt;
            this.lastOffer = new Dictionary<string, uint>();
            this.isHeir = isH;
            Globals_Game.npcMasterList.Add(this.charID, this);
        }

        /// <summary>
        /// Constructor for NonPlayerCharacter taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public NonPlayerCharacter()
		{
		}

		/// <summary>
        /// Constructor for NonPlayerCharacter using NonPlayerCharacter_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="npcs">NonPlayerCharacter_Serialised object to use as source</param>
		public NonPlayerCharacter(NonPlayerCharacter_Serialised npcs)
			: base(npcs: npcs)
		{
            if ((!String.IsNullOrWhiteSpace(npcs.employer)) && (npcs.employer.Length > 0))
			{
				this.employer = npcs.employer;
			}
			this.salary = npcs.salary;
			this.inEntourage = npcs.inEntourage;
			this.lastOffer = npcs.lastOffer;
            this.isHeir = npcs.isHeir;
		}

        /// <summary>
        /// Constructor for NonPlayerCharacter using NonPlayerCharacter object,
        /// for use when respawning deceased NPCs
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        public NonPlayerCharacter(NonPlayerCharacter npc)
            : base(npc, "respawn")
        {
            this.employer =null;
            this.salary = 0;
            this.inEntourage = false;
            this.lastOffer = new Dictionary<string,uint>();
            this.isHeir = false;
        }

		//TODO replace with proto
        /// <summary>
        /// Retrieves NonPlayerCharacter-specific information for Character display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="observer">Character who is viewing this character's information</param>
        public string DisplayNonPlayerCharacter(Character observer)
        {
            string npcText = "";

            // boss
            if (!String.IsNullOrWhiteSpace(this.employer))
            {
                npcText += "Employer (ID): " + this.employer + "\r\n";
            }

            // salary-related information
            if (observer is PlayerCharacter)
            {
                // estimated salary level (if character is male)
                if (this.isMale)
                {
                    npcText += "Potential salary: " + this.CalcSalary(observer as PlayerCharacter) + "\r\n";

                    // most recent salary offer from player (if any)
                    npcText += "Last offer from this PC: ";
                    if (this.lastOffer.ContainsKey(observer.charID))
                    {
                        npcText += this.lastOffer[observer.charID];
                    }
                    else
                    {
                        npcText += "N/A";
                    }
                    npcText += "\r\n";

                    // current salary
                    npcText += "Current salary: " + this.salary + "\r\n";
                }
            }

            return npcText;
        }

        /// <summary>
        /// Sets entourage value
        /// </summary>
        /// <param name="inEntourage"></param>
        public void setEntourage(bool inEntourage)
        {
            this.inEntourage = inEntourage;
        }
        /// <summary>
        /// Removes character from entourage
        /// </summary>
        public void removeSelfFromEntourage()
        {
            PlayerCharacter pc = this.GetEmployer();
            if (pc == null)
            {
                pc = this.GetHeadOfFamily();
            }
            if (pc == null && this.inEntourage)
            {
                throw new Exception("Entourage discrepancy");
            }
            if (pc != null)
            {
                pc.RemoveFromEntourage(this);
            }
        }
        /// <summary>
        /// Performs conditional checks prior to assigning the NonPlayerCharacter as heir
        /// </summary>
        /// <returns>bool indicating NonPlayerCharacter's suitability as heir</returns>
        /// <param name="pc">The PlayerCharacter who is choosing the heir</param>
        public bool ChecksForHeir(PlayerCharacter pc, out ProtoMessage error)
        {
            bool suitableHeir = true;
            error = null;
            if (String.IsNullOrWhiteSpace(this.familyID) || this.familyID != pc.familyID || !this.isMale)
            {
                suitableHeir = false;
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.CharacterHeir;
            }

            return suitableHeir;
        }

        /// <summary>
        /// Calculates the family allowance of a family NPC, based on age and function
        /// </summary>
        /// <returns>uint containing family allowance</returns>
        /// <param name="func">NPC's function</param>
        public uint CalcFamilyAllowance(String func)
        {
            uint famAllowance = 0;
            double ageModifier = 1;

            // factor in family function
            if (func.ToLower().Equals("wife"))
            {
                famAllowance = 30000;
            }
            else
            {
                if (func.ToLower().Contains("heir"))
                {
                    famAllowance = 40000;
                }
                else if (func.ToLower().Equals("son"))
                {
                    famAllowance = 20000;
                }
                else if (func.ToLower().Equals("daughter"))
                {
                    famAllowance = 15000;
                }
                else
                {
                    famAllowance = 10000;
                }
                int age = this.CalcAge();
                // calculate age modifier
                if ((age <= 7))
                {
                    ageModifier = 0.25;
                }
                else if ((age > 7) && (age <= 14))
                {
                    ageModifier = 0.5;
                }
                else if ((age > 14) && (age <= 21))
                {
                    ageModifier = 0.75;
                }

                // apply age modifier
                famAllowance = Convert.ToUInt32(famAllowance * ageModifier);
            }

            return famAllowance;
        }

        /// <summary>
        /// Derives NPC function
        /// </summary>
        /// <returns>String containing NPC function</returns>
        /// <param name="pc">PlayerCharacter with whom NPC has relationship</param>
        public String GetFunction(PlayerCharacter pc)
        {
            String myFunction = "";

            // check for employees
            if (!String.IsNullOrWhiteSpace(this.employer))
            {
                if (this.employer.Equals(pc.charID))
                {
                    myFunction = "Employee";
                }
           }

            // check for family function
            else if ((!String.IsNullOrWhiteSpace(this.familyID)) && (this.familyID.Equals(pc.familyID)))
            {
                // default value
                myFunction = "Family Member";

                // get character's father
                Character thisFather = this.GetFather();
                // get PC's father
                Character pcFather = pc.GetFather();

                if (thisFather != null)
                {
                    // sons & daughters
                    if (thisFather == pc)
                    {
                        if (this.isMale)
                        {
                            myFunction = "Son";
                        }
                        else
                        {
                            myFunction = "Daughter";
                        }
                    }

                    // brothers and sisters
                    if (pcFather != null)
                    {
                        if (thisFather == pcFather)
                        {
                            if (this.isMale)
                            {
                                myFunction = "Brother";
                            }
                            else
                            {
                                myFunction = "Sister";
                            }
                        }
                    }

                    // uncles and aunts
                    if ((pcFather != null) && (!String.IsNullOrWhiteSpace(pcFather.father)))
                    {
                        if (this.father == pcFather.father)
                        {
                            if (this.isMale)
                            {
                                myFunction = "Uncle";
                            }
                            else
                            {
                                myFunction = "Aunt";
                            }
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(thisFather.father))
                    {
                        // grandsons & granddaughters
                        if (thisFather.father.Equals(pc.charID))
                        {
                            if (this.isMale)
                            {
                                myFunction = "Grandson";
                            }
                            else
                            {
                                myFunction = "Granddaughter";
                            }
                        }
                    }
                }

                // if haven't found function yet
                if (myFunction.Equals("Family Member"))
                {
                    // sons and daughters (just in case only mother recorded)
                    if ((!String.IsNullOrWhiteSpace(this.mother)) && (!String.IsNullOrWhiteSpace(pc.spouse)))
                    {
                        if (this.mother == pc.spouse)
                        {
                            if (this.isMale)
                            {
                                myFunction = "Son";
                            }
                            else
                            {
                                myFunction = "Daughter";
                            }
                        }
                    }

                    // grandmother
                    if (pcFather != null)
                    {
                        if ((!String.IsNullOrWhiteSpace(pcFather.mother)) && (pcFather.mother.Equals(this.charID)))
                        {
                            myFunction = "Grandmother";
                        }
                    }

                    if ((!String.IsNullOrWhiteSpace(pc.mother)) && (pc.mother.Equals(this.charID)))
                    {
                        // mother
                        myFunction = "Mother";
                    }

                    // wife
                    if ((!String.IsNullOrWhiteSpace(this.spouse)) && (this.spouse.Equals(pc.charID)))
                    {
                        if (this.isMale)
                        {
                            myFunction = "Husband";
                        }
                        else
                        {
                            myFunction = "Wife";
                        }
                    }

                    // daughter-in-law
                    Character mySpouse = this.GetSpouse();
                    if (mySpouse != null)
                    {
                        if (mySpouse.father.Equals(pc.charID))
                        {
                            myFunction = "Daughter-in-law";
                        }
                    }

                    // check for heir
                    if (this.isHeir)
                    {
                        myFunction += " & Heir";
                    }
                }
            }

            return myFunction;
        }

        /// <summary>
        /// Gets an NPC's employment responsibilities
        /// </summary>
        /// <returns>String containing NPC responsibilities</returns>
        /// <param name="pc">PlayerCharacter by whom NPC is employed</param>
        public String GetResponsibilities(PlayerCharacter pc)
        {
            String myResponsibilities = "";
            List<Fief> bailiffDuties = new List<Fief>();

            // check for employment function
            if (((!String.IsNullOrWhiteSpace(this.employer)) && (this.employer.Equals(pc.charID)))
                || ((!String.IsNullOrWhiteSpace(this.familyID)) && (this.familyID.Equals(pc.charID))))
            {
                // check PC's fiefs for bailiff
                foreach (Fief thisFief in pc.ownedFiefs)
                {
                    if (thisFief.bailiff == this)
                    {
                        bailiffDuties.Add(thisFief);
                    }
                }

                // create entry for bailiff duties
                if (bailiffDuties.Count > 0)
                {
                    myResponsibilities += "Bailiff (";
                    for (int i = 0; i < bailiffDuties.Count; i++ )
                    {
                        myResponsibilities += bailiffDuties[i].id;
                        if (i < (bailiffDuties.Count - 1))
                        {
                            myResponsibilities += ", ";
                        }
                    }
                    myResponsibilities += ")";
                }

                // check for army leadership
                if (!String.IsNullOrWhiteSpace(this.armyID))
                {
                    if (!String.IsNullOrWhiteSpace(myResponsibilities))
                    {
                        myResponsibilities += ". ";
                    }
                    myResponsibilities += "Army leader (" + this.armyID + ").";
                }

                // if employee who isn't bailiff or army leader = 'Unspecified'
                if (String.IsNullOrWhiteSpace(myResponsibilities))
                {
                    if (!String.IsNullOrWhiteSpace(this.employer))
                    {
                        if (this.employer.Equals(pc.charID))
                        {
                            myResponsibilities = "Unspecified";
                        }
                    }
                }
            }

            return myResponsibilities;
        }

        /// <summary>
        /// Checks if recently born NPC still needs to be named
        /// </summary>
        /// <returns>bool indicating whether NPC needs naming</returns>
        /// <param name="age">NPC age to check for</param>
        public bool HasBabyName(byte age)
        {
            bool hasBabyName = false;

            // look for NPC with age < 1 who has firstname of 'baby'
            if ((this.CalcAge() == age) && ((this.firstName).ToLower().Equals("baby")))
            {
                hasBabyName = true;
            }

            return hasBabyName;
        }

        /// <summary>
        /// Calculates the potential salary (per season) for the NonPlayerCharacter, based on his current salary
        /// </summary>
        /// <returns>double containing salary</returns>
        public double CalcSalary_BaseOnCurrent()
        {
            double salary = 0;

            // NPC will only accept a minimum offer of 5% above his current salary
            salary = this.salary + (this.salary * 0.05);

            // use minimum figure to calculate median salary to use as basis for negotiations
            // (i.e. the minimum figure is 90% of the median salary)
            salary = salary + (salary * 0.11);

            return salary;
        }

        /// <summary>
        /// Calculates the potential salary (per season) for the NonPlayerCharacter, based on his traits
        /// </summary>
        /// <returns>uint containing salary</returns>
        public double CalcSalary_BaseOnTraits()
        {
            double salary = 0;
            double basicSalary = 1500;

            // get fief management rating
            double fiefMgtRating = this.CalcFiefManagementRating();

            // get army leadership rating
            double armyLeaderRating = this.CalcArmyLeadershipRating();

            // determine lowest of 2 ratings
            double minRating = Math.Min(armyLeaderRating, fiefMgtRating);

            // determine highest of 2 ratings
            double maxRating = Math.Max(armyLeaderRating, fiefMgtRating);
            if (maxRating < 0)
            {
                maxRating = 0;
            }

            // calculate potential salary, based on highest rating
            salary = basicSalary * maxRating;
            // if appropriate, also including 'flexibility bonus' for lowest rating
            if (minRating > 0)
            {
                salary += (basicSalary * (minRating / 2));
            }

            return salary;
        }

        /// <summary>
        /// Gets the potential salary (per season) for the NonPlayerCharacter,
        /// taking into account the stature of the hiring PlayerCharacter
        /// </summary>
        /// <returns>uint containing salary</returns>
        /// <param name="hiringPlayer">Hiring player</param>
        public uint CalcSalary(PlayerCharacter hiringPlayer)
        {
            // get potential salary based on NPC's traits
            double salary_traits = this.CalcSalary_BaseOnTraits();

            // get potential salary based on NPC's current salary
            double salary_current = 0;
            if (this.salary > 0)
            {
                salary_current = this.CalcSalary_BaseOnCurrent();
            }

            // use maximum of the two salary calculations
            double salary = Math.Max(salary_traits, salary_current);

            // factor in hiring PC's stature and current employer's stature (if applicable)
            // (4% reduction in NPC's salary for each stature rank above 4)
            double statMod = 0;

            // hiring PC
            double hirerStatMod = 0;
            if (hiringPlayer.CalculateStature() > 4)
            {
                hirerStatMod = (hiringPlayer.CalculateStature() - 4) * 0.04;
            }

            // current employer (note: is made negative to counteract hiring PC's stature effect)
            double emplStatMod = 0;
            if (this.GetEmployer() != null)
            {
                if (this.GetEmployer().CalculateStature() > 4)
                {
                    emplStatMod = ((hiringPlayer.CalculateStature() - 4) * 0.04) * -1;
                }
            }

            // add together the 2 stature modifiers and invert
            statMod = 1 - (hirerStatMod + emplStatMod);

            // apply to salary
            salary = salary * statMod;

            return Convert.ToUInt32(salary);
        }

        /// <summary>
        /// Gets the character's head of family
        /// </summary>
        /// <returns>The head of family or null</returns>
        public PlayerCharacter GetHeadOfFamily()
        {
            PlayerCharacter myHeadOfFamily = null;

            if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.familyID))
                {
                    myHeadOfFamily = Globals_Game.pcMasterList[this.familyID];
                }
            }

            return myHeadOfFamily;
        }

        /// <summary>
        /// Gets character's kingdom
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetKingdom()
        {
            Kingdom myKingdom = null;
            Character nationalitySource = null;

            // get nationality source
            // head of family
            if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                nationalitySource = this.GetHeadOfFamily();
            }

            // employer
            else if (!String.IsNullOrWhiteSpace(this.employer))
            {
                nationalitySource = this.GetEmployer();
            }

            // self
            if (nationalitySource == null)
            {
                nationalitySource = this;
            }

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                // get kingdom with matching nationality
                if (kingdomEntry.Value.nationality == nationalitySource.nationality)
                {
                    myKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return myKingdom;
        }

        /// <summary>
        /// Gets character's king
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetKing()
        {
            PlayerCharacter myKing = null;

            // get kingdom
            Kingdom myKingdom = this.GetKingdom();

            // get king with matching nationality
            if (myKingdom != null)
            {
                if (myKingdom.owner != null)
                {
                    myKing = myKingdom.owner;
                }
            }

            return myKing;
        }

        /// <summary>
        /// Gets character's queen
        /// </summary>
        /// <returns>The queen</returns>
        public NonPlayerCharacter GetQueen()
        {
            NonPlayerCharacter myQueen = null;

            // get king
            PlayerCharacter myKing = this.GetKing();

            if (myKing != null)
            {
                // get queen
                if (!String.IsNullOrWhiteSpace(myKing.spouse))
                {
                    if (Globals_Game.npcMasterList.ContainsKey(myKing.spouse))
                    {
                        myQueen = Globals_Game.npcMasterList[myKing.spouse];
                    }
                }
            }

            return myQueen;
        }

        /// <summary>
        /// Gets the character's employer
        /// </summary>
        /// <returns>The employer or null</returns>
        public PlayerCharacter GetEmployer()
        {
            PlayerCharacter myEmployer = null;

            if (!String.IsNullOrWhiteSpace(this.employer))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.employer))
                {
                    myEmployer = Globals_Game.pcMasterList[this.employer];
                }
            }

            return myEmployer;
        }

        /// <summary>
        /// Checks to see if the character needs to be named and, if so, assigns regent's first name
        /// </summary>
        public void CheckNeedsNaming()
        {
            // if (age >= 1) && (firstName.Equals("Baby")), character firstname = king's/queen's
            if (!String.IsNullOrWhiteSpace(this.familyID))
            {
                if (this.HasBabyName(1))
                {
                    // boys = try to get king's firstName 
                    if (this.isMale)
                    {
                        if (this.GetKing() != null)
                        {
                            this.firstName = this.GetKing().firstName;
                        }
                    }
                    else
                    {
                        // girls = try to get queen's firstName 
                        if (this.GetQueen() != null)
                        {
                            this.firstName = this.GetQueen().firstName;
                        }
                    }
                }
            }
        }

    }

	/// <summary>
	/// Class used to convert Character to/from serialised format (JSON)
	/// </summary>
	public abstract class Character_Serialised
	{

		/// <summary>
		/// Holds character ID
		/// </summary>
		public string charID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
        public String firstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public String familyName { get; set; }
        /// <summary>
        /// Tuple holding character's year and season of birth
        /// </summary>
        public Tuple<uint, byte> birthDate { get; set; }
        /// <summary>
		/// Holds if character male
		/// </summary>
		public bool isMale { get; set; }
		/// <summary>
		/// Holds character nationality (ID)
		/// </summary>
		public String nationality { get; set; }
        /// <summary>
        /// bool indicating whether character is alive
        /// </summary>
        public bool isAlive { get; set; }
        /// <summary>
		/// Holds character maximum health
		/// </summary>
		public Double maxHealth { get; set; }
		/// <summary>
		/// Holds character virility
		/// </summary>
		public Double virility { get; set; }
		/// <summary>
        /// Queue of Fiefs (fiefID) to auto-travel to
		/// </summary>
		public List<String> goTo { get; set; }
		/// <summary>
		/// Holds character language and dialect
		/// </summary>
        public string language { get; set; }
		/// <summary>
		/// Holds character's remaining days in season
		/// </summary>
		public double days { get; set; }
		/// <summary>
		/// Holds character's stature
		/// </summary>
		public Double statureModifier { get; set; }
		/// <summary>
		/// Holds character's management rating
		/// </summary>
		public Double management { get; set; }
		/// <summary>
		/// Holds character's combat rating
		/// </summary>
		public Double combat { get; set; }
		/// <summary>
        /// Array holding character's traits (ID)
		/// </summary>
		public Tuple<String, int>[] traits { get; set; }
		/// <summary>
		/// bool indicating if character is in the keep
		/// </summary>
		public bool inKeep { get; set; }
		/// <summary>
		/// Holds character pregnancy status
		/// </summary>
		public bool isPregnant { get; set; }
		/// <summary>
		/// Holds current location (Fief ID)
		/// </summary>
		public String location { get; set; }
		/// <summary>
		/// Holds spouse (Character ID)
		/// </summary>
		public String spouse { get; set; }
		/// <summary>
		/// Holds father (Character ID)
		/// </summary>
		public String father { get; set; }
        /// <summary>
        /// Holds mother (Character ID)
        /// </summary>
        public String mother { get; set; }
        /// <summary>
        /// Holds charID of head of family with which character associated
		/// </summary>
		public String familyID { get; set; }
        /// <summary>
        /// Holds chaacter's fiancee (charID)
        /// </summary>
        public string fiancee { get; set; }
        /// <summary>
        /// Holds character's titles (fiefIDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }
        /// <summary>
        /// Holds ailments effecting character's health
        /// </summary>
        public Dictionary<string, Ailment> ailments = new Dictionary<string, Ailment>();
        /// <summary>
        /// Holds captor charID
        /// </summary>
        public string captorID { get; set; }
        /// <summary>
        /// Holds jentry id of ransom demand
        /// </summary>
        public string ransom { get; set; }

		/// <summary>
        /// Constructor for Character_Serialised
		/// </summary>
		/// <param name="pc">PlayerCharacter object to use as source</param>
		/// <param name="npc">NonPlayerCharacter object to use as source</param>
		public Character_Serialised(PlayerCharacter pc = null, NonPlayerCharacter npc = null)
		{
			Character charToUse = null;

			if (pc != null)
			{
				charToUse = pc;
			}
			else if (npc != null)
			{
				charToUse = npc;
			}

			if (charToUse != null)
			{
				this.charID = charToUse.charID;
				this.familyName = charToUse.familyName;
                this.firstName = charToUse.firstName;
				this.birthDate = charToUse.birthDate;
				this.isMale = charToUse.isMale;
				this.nationality = charToUse.nationality.natID;
                this.isAlive = charToUse.isAlive;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
				this.goTo = new List<string> ();
				if (charToUse.goTo.Count > 0)
				{
					foreach (Fief value in charToUse.goTo)
					{
						this.goTo.Add (value.id);
					}
				}
                this.language = charToUse.language.id;
				this.days = charToUse.days;
				this.statureModifier = charToUse.statureModifier;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
				this.traits = new Tuple<String, int>[charToUse.traits.Length];
				for (int i = 0; i < charToUse.traits.Length; i++)
				{
					this.traits [i] = new Tuple<string,int>(charToUse.traits [i].Item1.id, charToUse.traits [i].Item2);
				}
				this.inKeep = charToUse.inKeep;
				this.isPregnant = charToUse.isPregnant;
				this.location = charToUse.location.id;
                this.spouse = charToUse.spouse;
                this.father = charToUse.father;
                this.mother = charToUse.mother;
                this.familyID = charToUse.familyID;
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
                this.ailments = charToUse.ailments;
                this.fiancee = charToUse.fiancee;
                this.captorID = charToUse.captorID;
                this.ransom = charToUse.ransomDemand;
            }
		}

        /// <summary>
        /// Constructor for Character_Serialised taking seperate values.
        /// For creating Character_Serialised from CSV file.
        /// </summary>
        /// <param name="id">string holding character ID</param>
        /// <param name="firstNam">String holding character's first name</param>
        /// <param name="famNam">String holding character's family name</param>
        /// <param name="dob">Tuple(uint, byte) holding character's year and season of birth</param>
        /// <param name="isM">bool holding if character male</param>
        /// <param name="nat">string holding Character's Nationality (id)</param>
        /// <param name="alive">bool indicating whether character is alive</param>
        /// <param name="mxHea">Double holding character maximum health</param>
        /// <param name="vir">Double holding character virility rating</param>
        /// <param name="go">Queue (string) of Fiefs to auto-travel (id)</param>
        /// <param name="lang">string holding Language (id)</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">Double holding character stature rating</param>
        /// <param name="mngmnt">Double holding character management rating</param>
        /// <param name="cbt">Double holding character combat rating</param>
        /// <param name="trt">string array containing character's traits (id)</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famID">String holding charID of head of family with which character associated</param>
        /// <param name="sp">String holding spouse (charID)</param>
        /// <param name="fath">String holding father (charID)</param>
        /// <param name="moth">String holding mother (charID)</param>
        /// <param name="fia">string holding fiancee (charID)</param>
        /// <param name="loc">string holding current location (id)</param>
        /// <param name="myTi">List holding character's titles (fiefIDs)</param>
        /// <param name="aID">String holding armyID of army character is leading</param>
        /// <param name="ails">Dictionary (string, Ailment) holding ailments effecting character's health</param>
        public Character_Serialised(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, string nat, bool alive, Double mxHea, Double vir,
            List<string> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<string, int>[] trt, bool inK, bool preg,
            String famID, String sp, String fath, String moth, List<String> myTi, string fia, Dictionary<string, Ailment> ails = null, string loc = null, String aID = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.FirstCharToUpper(id.Trim());

            if (!Utility_Methods.ValidateCharacterID(id))
            {
                throw new InvalidDataException("Character_Serialised id must have the format 'Char_' followed by some numbers");
            }

            // FIRSTNAM
            // trim and ensure 1st is uppercase
            firstNam = Utility_Methods.FirstCharToUpper(firstNam.Trim());

            if (!Utility_Methods.ValidateName(firstNam))
            {
                throw new InvalidDataException("Character_Serialised firstname must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // FAMNAM
            // trim
            famNam = famNam.Trim();

            if (!Utility_Methods.ValidateName(famNam))
            {
                throw new InvalidDataException("Character_Serialised family name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // DOB
            if (!Utility_Methods.ValidateSeason(dob.Item2))
            {
                throw new InvalidDataException("Character_Serialised date-of-birth season must be a byte between 0-3");
            }

            // NAT
            // trim and ensure 1st is uppercase
            nat = Utility_Methods.FirstCharToUpper(nat.Trim());

            if (!Utility_Methods.ValidateNationalityID(nat))
            {
                throw new InvalidDataException("Character_Serialised nationality ID must be 1-3 characters long, and consist entirely of letters");
            }

            // MXHEA
            if (!Utility_Methods.ValidateCharacterStat(mxHea))
            {
                throw new InvalidDataException("Character_Serialised maxHealth must be a double between 1-9");
            }

            // VIR
            if (!Utility_Methods.ValidateCharacterStat(vir))
            {
                throw new InvalidDataException("Character_Serialised virility must be a double between 1-9");
            }

            // GOTO
            if (go.Count > 0)
            {
                string[] goQueue = go.ToArray();
                for (int i = 0; i < goQueue.Length; i++ )
                {
                    // trim and ensure is uppercase
                    goQueue[i] = goQueue[i].Trim().ToUpper();

                    if (!Utility_Methods.ValidatePlaceID(goQueue[i]))
                    {
                        throw new InvalidDataException("All IDs in Character_Serialised goTo queue must be 5 characters long, start with a letter, and end in at least 2 numbers");
                    }
                }
            }

            // LANG
            // trim
            lang = lang.Trim();
            if (!Utility_Methods.ValidateLanguageID(lang))
            {
                throw new InvalidDataException("Character_Serialised language ID must have the format 'lang_' followed by 1-2 letters, ending in 1-2 numbers");
            }

            // DAYS
            if (!Utility_Methods.ValidateDays(day))
            {
                throw new InvalidDataException("Character_Serialised days must be a double between 0-109");
            }

            // STAT
            if (!Utility_Methods.ValidateCharacterStat(stat, 0))
            {
                throw new InvalidDataException("Character_Serialised stature must be a double between 0-9");
            }

            // MNGMNT
            if (!Utility_Methods.ValidateCharacterStat(mngmnt))
            {
                throw new InvalidDataException("Character_Serialised management must be a double between 1-9");
            }

            // CBT
            if (!Utility_Methods.ValidateCharacterStat(cbt))
            {
                throw new InvalidDataException("Character_Serialised combat must be a double between 1-9");
            }

            // TRT
            for (int i = 0; i < trt.Length; i++)
            {
                if (!Utility_Methods.ValidateTraitID(trt[i].Item1))
                {
                    throw new InvalidDataException("Character_Serialised trait ID must have the format 'trait_' followed by some numbers");
                }

                else if (!Utility_Methods.ValidateCharacterStat(Convert.ToDouble(trt[i].Item2)))
                {
                    throw new InvalidDataException("Character_Serialised trait level must be an integer between 1-9");
                }
            }

            // PREG
            if (preg)
            {
                if (isM)
                {
                    throw new InvalidDataException("Character_Serialised cannot be pregnant if is male");
                }
            }

            // FAMID
            if (!String.IsNullOrWhiteSpace(famID))
            {
                // trim and ensure 1st is uppercase
                famID = Utility_Methods.FirstCharToUpper(famID.Trim());

                if (!Utility_Methods.ValidateCharacterID(famID))
                {
                    throw new InvalidDataException("Character_Serialised family id must have the format 'Char_' followed by some numbers");
                }
            }

            // SP
            if (!String.IsNullOrWhiteSpace(sp))
            {
                // trim and ensure 1st is uppercase
                sp = Utility_Methods.FirstCharToUpper(sp.Trim());

                if (!Utility_Methods.ValidateCharacterID(sp))
                {
                    throw new InvalidDataException("Character_Serialised spouse id must have the format 'Char_' followed by some numbers");
                }
            }

            // FATH
            if (!String.IsNullOrWhiteSpace(fath))
            {
                // trim and ensure 1st is uppercase
                fath = Utility_Methods.FirstCharToUpper(fath.Trim());

                if (!Utility_Methods.ValidateCharacterID(fath))
                {
                    throw new InvalidDataException("Character_Serialised father id must have the format 'Char_' followed by some numbers");
                }
            }

            // MOTH
            if (!String.IsNullOrWhiteSpace(moth))
            {
                // trim and ensure 1st is uppercase
                moth = Utility_Methods.FirstCharToUpper(moth.Trim());

                if (!Utility_Methods.ValidateCharacterID(moth))
                {
                    throw new InvalidDataException("Character_Serialised mother id must have the format 'Char_' followed by some numbers");
                }
            }

            // MYTI
            for (int i = 0; i < myTi.Count; i++)
            {
                // trim and ensure is uppercase
                myTi[i] = myTi[i].Trim().ToUpper();

                if (!Utility_Methods.ValidatePlaceID(myTi[i]))
                {
                    throw new InvalidDataException("All Character_Serialised title IDs must be 5 characters long, start with a letter, and end in at least 2 numbers");
                }
            }

            // FIA
            if (!String.IsNullOrWhiteSpace(fia))
            {
                // trim and ensure 1st is uppercase
                fia = Utility_Methods.FirstCharToUpper(fia.Trim());

                if (!Utility_Methods.ValidateCharacterID(fia))
                {
                    throw new InvalidDataException("Character_Serialised fiancee id must have the format 'Char_' followed by some numbers");
                }
            }

            // AILS
            if (ails.Count > 0)
            {
                string[] myAils = new string[ails.Count];
                ails.Keys.CopyTo(myAils, 0);
                for (int i = 0; i < myAils.Length; i++)
                {
                    // trim and ensure is uppercase
                    myAils[i] = myAils[i].Trim().ToUpper();

                    if (!Utility_Methods.ValidateAilmentID(myAils[i]))
                    {
                        throw new InvalidDataException("All IDs in Character_Serialised ailments must have the format 'Ail_' followed by some numbers");
                    }
                }
            }

            // LOC
            // trim and ensure is uppercase
            loc = loc.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(loc))
            {
                throw new InvalidDataException("Character_Serialised location id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // AID
            if (!String.IsNullOrWhiteSpace(aID))
            {
                // trim and ensure 1st is uppercase
                aID = Utility_Methods.FirstCharToUpper(aID.Trim());

                if (!Utility_Methods.ValidateArmyID(aID))
                {
                    throw new InvalidDataException("Character_Serialised army id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            this.charID = id;
            this.firstName = firstNam;
            this.familyName = famNam;
            this.birthDate = dob;
            this.isMale = isM;
            this.nationality = nat;
            this.isAlive = alive;
            this.maxHealth = mxHea;
            this.virility = vir;
            this.goTo = go;
            this.language = lang;
            this.days = day;
            this.statureModifier = stat;
            this.management = mngmnt;
            this.combat = cbt;
            this.traits = trt;
            this.inKeep = inK;
            this.isPregnant = preg;
			this.location = loc;
            this.spouse = sp;
            this.father = fath;
            this.mother = moth;
            this.familyID = famID;
            this.myTitles = myTi;
            this.armyID = aID;
            if (ails != null)
            {
                this.ailments = ails;
            }
            this.fiancee = fia;
        }

	}

	/// <summary>
	/// Class used to convert PlayerCharacter to/from serialised format (JSON)
	/// </summary>
	public class PlayerCharacter_Serialised : Character_Serialised
	{

		/// <summary>
		/// Holds character outlawed status
		/// </summary>
		public bool isOutlawed { get; set; }
		/// <summary>
		/// Holds character's finances
		/// </summary>
		public uint purse { get; set; }
		/// <summary>
		/// Holds character's employees and family (charID)
		/// </summary>
		public List<String> myNPCs = new List<String>();
		/// <summary>
		/// Holds character's owned fiefs (id)
		/// </summary>
		public List<String> ownedFiefs = new List<String>();
        /// <summary>
        /// Holds character's owned provinces (id)
        /// </summary>
        public List<string> ownedProvinces = new List<string>();
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public String homeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public String ancestralHomeFief { get; set; }
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// </summary>
        public String playerID { get; set; }
        /// <summary>
        /// Holds character's armies (Army objects)
        /// </summary>
        public List<String> myArmies = new List<String>();
        /// <summary>
        /// Holds character's sieges (Siege objects)
        /// </summary>
        public List<string> mySieges = new List<string>();

		/// <summary>
        /// Constructor for PlayerCharacter_Serialised
		/// </summary>
		/// <param name="pc">PlayerCharacter object to use as source</param>
		public PlayerCharacter_Serialised(PlayerCharacter pc)
			: base(pc: pc)
		{

			this.isOutlawed = pc.outlawed;
			this.purse = pc.purse;
			if (pc.myNPCs.Count > 0)
			{
				for (int i = 0; i < pc.myNPCs.Count; i++)
				{
					this.myNPCs.Add (pc.myNPCs[i].charID);
				}
			}
			if (pc.ownedFiefs.Count > 0)
			{
				for (int i = 0; i < pc.ownedFiefs.Count; i++)
				{
					this.ownedFiefs.Add (pc.ownedFiefs[i].id);
				}
			}
            if (pc.ownedProvinces.Count > 0)
            {
                foreach (Province thisProv in pc.ownedProvinces)
                {
                    this.ownedProvinces.Add(thisProv.id);
                }
            }
            this.homeFief = pc.homeFief;
            this.ancestralHomeFief = pc.ancestralHomeFief;
            this.playerID = pc.playerID;
            if (pc.myArmies.Count > 0)
            {
                for (int i = 0; i < pc.myArmies.Count; i++ )
                {
                    this.myArmies.Add(pc.myArmies[i].armyID);
                }
            }
            this.mySieges = pc.mySieges;
		}

        /// <summary>
        /// Constructor for PlayerCharacter_Serialised taking seperate values.
        /// For creating PlayerCharacter_Serialised from CSV file.
        /// </summary>
        /// <param name="outl">bool holding character outlawed status</param>
        /// <param name="pur">uint holding character purse</param>
        /// <param name="npcs">List (string) holding employees and family (id)</param>
        /// <param name="ownedF">List (string) holding fiefs owned (id)</param>
        /// <param name="ownedP">List (string) holding provinces owned (id)</param>
        /// <param name="home">String holding character's home fief (id)</param>
        /// <param name="anchome">String holding character's ancestral home fief (id)</param>
        /// <param name="pID">String holding ID of player who is currently playing this PlayerCharacter</param>
        /// <param name="myA">List (string) holding character's armies (id)</param>
        /// <param name="myS">List<string> holding character's sieges (id)</param>
        public PlayerCharacter_Serialised(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, string nat, bool alive, Double mxHea, Double vir,
            List<string> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<string, int>[] trt, bool inK, bool preg, String famID,
            String sp, String fath, String moth, List<String> myTi, string fia, bool outl, uint pur, List<string> npcs, List<string> ownedF, List<string> ownedP, String home, String ancHome, List<string> myA,
            List<string> myS, Dictionary<string, Ailment> ails = null, string loc = null, String aID = null, String pID = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, trt, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc, aID)
        {
            // VALIDATION

            // MYNPCS
            if (npcs.Count > 0)
            {
                for (int i = 0; i < npcs.Count; i++ )
                {
                    // trim and ensure 1st is uppercase
                    npcs[i] = Utility_Methods.FirstCharToUpper(npcs[i].Trim());

                    if (!Utility_Methods.ValidateCharacterID(npcs[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter_Serialised myNPC IDs must have the format 'Char_' followed by some numbers");
                    }
                }
            }

            // MYOWNEDFIEFS
            if (ownedF.Count > 0)
            {
                for (int i = 0; i < ownedF.Count; i++)
                {
                    // trim and ensure is uppercase
                    ownedF[i] = ownedF[i].Trim().ToUpper();

                    if (!Utility_Methods.ValidatePlaceID(ownedF[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter_Serialised ownedFief IDs must be 5 characters long, start with a letter, and end in at least 2 numbers");
                    }
                }
            }

            // MYOWNEDPROVS
            if (ownedP.Count > 0)
            {
                for (int i = 0; i < ownedP.Count; i++)
                {
                    // trim and ensure is uppercase
                    ownedP[i] = ownedP[i].Trim().ToUpper();

                    if (!Utility_Methods.ValidatePlaceID(ownedP[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter_Serialised ownedProvince IDs must be 5 characters long, start with a letter, and end in at least 2 numbers");
                    }
                }
            }

            // HOME
            // trim and ensure is uppercase
            home = home.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(home))
            {
                throw new InvalidDataException("PlayerCharacter_Serialised homeFief id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // ANCHOME
            // trim and ensure is uppercase
            ancHome = ancHome.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(ancHome))
            {
                throw new InvalidDataException("PlayerCharacter_Serialised ancestral homeFief id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // MYARMIES
            if (myA.Count > 0)
            {
                for (int i = 0; i < myA.Count; i++)
                {
                    // trim and ensure 1st is uppercase
                    myA[i] = Utility_Methods.FirstCharToUpper(myA[i].Trim());

                    if (!Utility_Methods.ValidateArmyID(myA[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter_Serialised army IDs must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                    }
                }
            }

            // MYSIEGES
            if (myS.Count > 0)
            {
                for (int i = 0; i < myS.Count; i++)
                {
                    // trim and ensure 1st is uppercase
                    myS[i] = Utility_Methods.FirstCharToUpper(myS[i].Trim());

                    if (!Utility_Methods.ValidateSiegeID(myS[i]))
                    {
                        throw new InvalidDataException("All PlayerCharacter_Serialised siege IDs must have the format 'Siege_' followed by some numbers");
                    }
                }
            }

            this.isOutlawed = outl;
            this.purse = pur;
            this.myNPCs = npcs;
            this.ownedFiefs = ownedF;
            this.ownedProvinces = ownedP;
            this.homeFief = home;
            this.ancestralHomeFief = ancHome;
            this.playerID = pID;
            this.myArmies = myA;
            this.mySieges = myS;
        }

        //TODO change serialised class to serialise method
        /// <summary>
        /// Constructor for PlayerCharacter_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public PlayerCharacter_Serialised()
		{
		}
	}

	/// <summary>
	/// Class used to convert NonPlayerCharacter to/from serialised format (JSON)
	/// </summary>
	public class NonPlayerCharacter_Serialised : Character_Serialised
	{

		/// <summary>
		/// Holds NPC's employer (charID)
		/// </summary>
		public String employer { get; set; }
		/// <summary>
		/// Holds NPC's wage
		/// </summary>
		public uint salary { get; set; }
		/// <summary>
		/// Holds last wage offer from individual PCs
		/// </summary>
		public Dictionary<string, uint> lastOffer { get; set; }
		/// <summary>
        /// Denotes if in employer's entourage
		/// </summary>
		public bool inEntourage { get; set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool isHeir { get; set; }


		/// <summary>
        /// Constructor for NonPlayerCharacter_Serialised
		/// </summary>
		/// <param name="npc">NonPlayerCharacter object to use as source</param>
		public NonPlayerCharacter_Serialised(NonPlayerCharacter npc)
			: base(npc: npc)
		{

            if (!String.IsNullOrWhiteSpace(npc.employer))
			{
				this.employer = npc.employer;
			}
			this.salary = npc.salary;
			this.inEntourage = npc.inEntourage;
			this.lastOffer = npc.lastOffer;
            this.isHeir = npc.isHeir;
		}

        /// <summary>
        /// Constructor for NonPlayerCharacter_Serialised taking seperate values.
        /// For creating NonPlayerCharacter_Serialised from CSV file.
        /// </summary>
        /// <param name="empl">String holding NPC's employer (charID)</param>
        /// <param name="sal">string holding NPC's wage</param>
        /// <param name="inEnt">bool denoting if in employer's entourage</param>
        /// <param name="isH">bool denoting if is player's heir</param>
        public NonPlayerCharacter_Serialised(String id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, string nat, bool alive, Double mxHea, Double vir,
            List<string> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<string, int>[] trt, bool inK, bool preg, String famID,
            String sp, String fath, String moth, List<String> myTi, string fia, uint sal, bool inEnt, bool isH, Dictionary<string, Ailment> ails = null, string loc = null, String aID = null, String empl = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, trt, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc, aID)
        {
            // VALIDATION

            // EMPL
            if (!String.IsNullOrWhiteSpace(empl))
            {
                // trim and ensure 1st is uppercase
                empl = Utility_Methods.FirstCharToUpper(empl.Trim());

                if (!String.IsNullOrWhiteSpace(famID))
                {
                    throw new InvalidDataException("A NonPlayerCharacter with a familyID cannot have an employer ID");
                }

                if (!Utility_Methods.ValidateCharacterID(empl))
                {
                    throw new InvalidDataException("NonPlayerCharacter employer ID must have the format 'Char_' followed by some numbers");
                }
            }

            this.employer = empl;
            this.salary = sal;
            this.inEntourage = inEnt;
            this.lastOffer = new Dictionary<string, uint>();
            this.isHeir = isH;
        }

        /// <summary>
        /// Constructor for NonPlayerCharacter_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public NonPlayerCharacter_Serialised()
		{
		}

	}

}
