using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
namespace hist_mmorpg
{
    /// <summary>
    /// Class for translating objects to ProtoBuf constructs
    /// Object fields can be hidden from clients by setting the desired field to null
    /// </summary>
    [ProtoContract]
    public class ProtoMessage<T>
    {
        /// <summary>
        /// Contains the underlying type of the message. Used to easily distinguish between messages
        /// </summary>
        [ProtoMember(1)]
        String MessageType {get;set;}

        /// <summary>
        /// Contains a message or messageID for the client
        /// Used when sending error messages
        /// </summary>
        [ProtoMember(2)]
        String Message;

        /// <summary>
        /// Contains any fields that need to be sent along with the message
        /// e.g. amount of overspend in fief
        /// </summary>
        [ProtoMember(3)]
        T[] MessageFields;

        public string getMsgType()
        {
            return MessageType;
        }
        public string getMessage()
        {
            return this.Message;
        }
        public T[] getFields()
        {
            return this.MessageFields;
        }

        /**************** MESSAGES TO CLIENT ***********************/


        /// <summary>
        /// Class for serializing an Ailment
        /// (At present, only minimumEffect is hidden)
        /// </summary>
        [ProtoContract]
        public class ProtoAilment: ProtoMessage<T>
        {
            /// <summary>
            /// Holds ailmentID
            /// </summary>
            [ProtoMember(1)]
            string _AilmentID { get; set; }
            /// <summary>
            /// Holds ailment description
            /// </summary>
            [ProtoMember(2)]
            public String _description { get; set; }
            /// <summary>
            /// Holds ailment date
            /// </summary>
            [ProtoMember(3)]
            public string _when { get; set; }
            /// <summary>
            /// Holds current ailment effect
            /// </summary>
            [ProtoMember(4)]
            public uint _effect { get; set; }
            /// <summary>
            /// Holds minimum ailment effect
            /// </summary>
            [ProtoMember(5)]
            public uint _minimumEffect { get; set; }

            /// <summary>
            /// Create a ProtoAilment from an Ailment
            /// </summary>
            /// <param name="a"></param>
            public ProtoAilment(Ailment a)
            {
                this._AilmentID = a.ailmentID;
                this._description = a.description;
                this._when = a.when;
                this._effect = a.effect;
                this._minimumEffect = 0;
            }
            public bool ShouldSerializeminimumEffect() {
                return (_minimumEffect!=0);
            }
        }
        /// <summary>
        /// Class for serializing an Army
        /// The amount of information a player can view about an army depends on whether that player 
        /// ownes the army, how close the player is etc. 
        /// Can be tuned later to include information obtained via methods such as spying, interrogation, or defection
        /// </summary>
        [ProtoContract]
        public class ProtoArmy : ProtoMessage<T>
        {
            /// <summary>
            /// Holds army ID
            /// </summary>
            [ProtoMember(1)]
            public String _armyID { get; set; }
            /// <summary>
            /// Holds troops in army
            /// 0 = knights
            /// 1 = menAtArms
            /// 2 = lightCav
            /// 3 = longbowmen
            /// 4 = crossbowmen
            /// 5 = foot
            /// 6 = rabble
            /// </summary>
            [ProtoMember(2)]
            public uint[] _troops = new uint[7] { 0, 0, 0, 0, 0, 0, 0 };
            /// <summary>
            /// Holds army leader ID and name in the format:
            /// ID|first_name|last_name
            /// </summary>
            [ProtoMember(3)]
            public string _leader { get; set; }
            /// <summary>
            /// Holds army owner ID and name in the format:
            /// ID|first_name|last_name
            /// </summary>
            [ProtoMember(4)]
            public string _owner { get; set; }
            /// <summary>
            /// Holds army's remaining days in season
            /// </summary>
            [ProtoMember(5)]
            public double _days { get; set; }
            /// <summary>
            /// Holds army location in the format:
            /// fiefID|fiefName|provinceName|kingdomName
            /// </summary>
            [ProtoMember(6)]
            public string _location { get; set; }
            /// <summary>
            /// Indicates whether army is being actively maintained by owner
            /// </summary>
            [ProtoMember(7)]
            public bool _isMaintained { get; set; }
            /// <summary>
            /// Indicates army's aggression level (automated response to combat)
            /// </summary>
            [ProtoMember(8)]
            public byte _aggression { get; set; }
            /// <summary>
            /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
            /// </summary>
            [ProtoMember(9)]
            public byte _combatOdds { get; set; }
            /// <summary>
            /// String indicating army nationality
            /// </summary>
            [ProtoMember(10)]
            public string _nationality { get; set; }
            /// <summary>
            /// Holds siege status of army
            /// One of BESIEGING, BESIEGED, FIEF or none
            /// BESIEGING: army is currently besieging fief
            /// BESIEGED: army is under siege
            /// FIEF: the fief the army is in is under siege
            /// </summary>
            [ProtoMember(11)]
            public string _siegeStatus { get; set; }

            public ProtoArmy()
            {

            }
            public ProtoArmy(Army a, Character observer)
            {
                this.MessageType = "Army";
                bool isOwner = (a.GetOwner()==observer)||(observer==Globals_Game.sysAdmin);
                this._armyID = a.armyID;
                this._leader = a.leader+"|"+a.GetLeader().firstName+"|"+a.GetLeader().familyName;
                this._owner = a.owner+"|"+a.GetOwner().firstName+"|"+a.GetOwner().familyName;
                this._location = a.location + "|" + a.GetLocation().name + "|" + a.GetLocation().province.name + "|" + a.GetLocation().province.kingdom.name;
                // check if is garrison in a siege
                string siegeID = a.CheckIfSiegeDefenderGarrison();
                if (String.IsNullOrWhiteSpace(siegeID))
                {
                    // check if is additional defender in a siege
                    siegeID = a.CheckIfSiegeDefenderAdditional();
                }

                // if is defender in a siege, indicate
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    _siegeStatus = "BESIEGED";
                }

                else
                {
                    // check if is besieger in a siege
                    siegeID = a.CheckIfBesieger();

                    // if is besieger in a siege, indicate
                    if (!String.IsNullOrWhiteSpace(siegeID))
                    {
                        _siegeStatus = "BESIEGER";
                    }

                    // check if is siege in fief (but army not involved)
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(a.GetLocation().siege))
                        {
                            _siegeStatus = "FIEF";
                        }
                    }
                }
                if (isOwner)
                {
                    this._aggression = a.aggression;
                    this._combatOdds = a.combatOdds;
                    this._troops = a.troops;
                    this._days = a.days;
                    this._isMaintained = a.isMaintained;
                    this._nationality = a.GetOwner().nationality.name;
                }
                else
                {
                    if (observer.location == a.GetLocation())
                    {
                        this._troops = a.GetTroopsEstimate(observer);
                    }
                    
                }
            }
        }
        /// <summary>
        /// Class for sending details of a character
        /// </summary>
        [ProtoContract]
        [ProtoInclude(14,typeof(ProtoPlayerCharacter))]
        [ProtoInclude(15,typeof(ProtoNPC))]
        public class ProtoCharacter : ProtoMessage<T>
        {
            /* BASIC CHARACTER DETAILS */
            /// <summary>
            /// Holds character ID
            /// </summary>
            [ProtoMember(1)]
            public string charID { get; set; }
            /// <summary>
            /// Holds character's first name
            /// </summary>
            [ProtoMember(2)]
            public string firstName { get; set; }
            /// <summary>
            /// Holds character's family name
            /// </summary>
            [ProtoMember(3)]
            public string familyName { get; set; }
            /// <summary>
            /// Tuple holding character's year and season of birth
            /// </summary>
            [ProtoMember(4)]
            public Tuple<uint, byte> birthDate { get; set; }
            /// <summary>
            /// Holds if character male
            /// </summary>
            [ProtoMember(5)]
            public bool isMale { get; set; }
            /// <summary>
            /// Holds the string representation of this character's nationality
            /// </summary>
            [ProtoMember(6)]
            public string nationality { get; set; }
            /// <summary>
            /// Indicates whether a character is alive
            /// </summary>
            [ProtoMember(7)]
            public bool isAlive { get; set; }
            /// <summary>
            /// Character's max health
            /// </summary>
            [ProtoMember(8)]
            public double maxHealth { get; set; }
            /// <summary>
            /// Character's virility
            /// </summary>
            [ProtoMember(9)]
            public double virility { get; set; }
            /// <summary>
            /// IDs of fiefs in goTo list
            /// </summary>
            [ProtoMember(10)]
            public bool inKeep { get; set; }
            [ProtoMember(11)]
            public string language {get;set;}
            /// <summary>
            /// number of days left in season
            /// </summary>
            [ProtoMember(12)]
            public double days { get; set; }
            [ProtoMember(13)]
            public String familyID { get; set; }
            //14 and 15 reserved for PC and NPC
            [ProtoMember(16)]
            public String spouse { get; set; }
            [ProtoMember(17)]
            public String father { get; set; }
            [ProtoMember(18)]
            public String mother { get; set; }
            [ProtoMember(19)]
            public string fiancee { get; set; }
            [ProtoMember(20)]
            public string location { get; set; }
            [ProtoMember(21)]
            public double statureModifier { get; set; }
            [ProtoMember(22)]
            public double management { get; set; }
            [ProtoMember(23)]
            public double combat { get; set; }
            [ProtoMember(24)]
            public Tuple<string, double> traits { get; set; }
            [ProtoMember(25)]
            public bool isPregnant { get; set; }
            [ProtoMember(26)]
            public string[] titles { get; set; }
            [ProtoMember(27)]
            public string armyID { get; set; }
            [ProtoMember(28)]
            public Tuple<string, string>[] ailments { get; set; }
            [ProtoMember(29)]
            public string[] goTo { get; set; }
        }
        /// <summary>
        /// Class for sending details of a PlayerCharacter
        /// </summary>
        [ProtoContract]
        [ProtoInclude(1,typeof(ProtoCharacter))]
        public class ProtoPlayerCharacter : ProtoMessage<T>
        {
            /// <summary>
            /// Holds ID of player who is currently playing this PlayerCharacter
            /// Note that list of sieges and list of armies is stored elsewhere- see ProtoSiegeList and ProtoArmyList
            /// </summary>
            [ProtoMember(2)]
            public string playerID { get; set; }
            /// <summary>
            /// Holds character outlawed status
            /// </summary>
            [ProtoMember(3)]
            public bool outlawed { get; set; }
            /// <summary>
            /// Holds character's treasury
            /// </summary>
            [ProtoMember(4)]
            public uint purse { get; set; }
            /// <summary>
            /// Holds IDs and names of character's employees and family
            /// </summary>
            [ProtoMember(5)]
            public Tuple<string, string>[] myNPCs { get; set; }
            /// <summary>
            /// Holds IDs and names character's owned fiefs
            /// </summary>
            [ProtoMember(6)]
            public Tuple<string, string>[] ownedFiefs { get; set; }
            /// <summary>
            /// Holds IDs and names character's owned provinces
            /// </summary>
            [ProtoMember(7)]
            public Tuple<string, string>[] provinces { get; set; }
            /// <summary>
            /// Holds character's home fief (fiefID)
            /// </summary>
            [ProtoMember(8)]
            public String homeFief { get; set; }
            /// <summary>
            /// Holds character's ancestral home fief (fiefID)
            /// </summary>
            [ProtoMember(9)]
            public String ancestralHomeFief { get; set; }
        }
        /// <summary>
        /// Class for sending details of Non-PlayerCharacter
        /// </summary>
        [ProtoContract]
        [ProtoInclude(1, typeof(ProtoCharacter))]
        public class ProtoNPC : ProtoMessage<T>
        {
            /// <summary>
            /// Holds NPC's employer (charID)
            /// </summary>
            public string employer { get; set; }
            /// <summary>
            /// Holds NPC's salary
            /// </summary>
            public uint salary { get; set; }
            /// <summary>
            /// Holds last wage offer from individual PCs
            /// </summary>
            public Tuple<string, uint> lastOffer { get; set; }
            /// <summary>
            /// Denotes if in employer's entourage
            /// </summary>
            public bool inEntourage { get; set; }
            /// <summary>
            /// Denotes if is player's heir
            /// </summary>
            public bool isHeir { get; set; }

        }
        /// <summary>
        /// Class for sending fief details
        /// </summary>
        [ProtoContract]
        public class ProtoFief : ProtoMessage<T>
        {
            /// <summary>
            /// Holds fief's Province reference
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// Holds fief population
            /// </summary>
            public int population { get; set; }
            /// <summary>
            /// Holds fief field level
            /// </summary>
            public double fields { get; set; }
            /// <summary>
            /// Holds fief industry level
            /// </summary>
            public double industry { get; set; }
            /// <summary>
            /// Holds number of troops in fief
            /// </summary>
            public uint troops { get; set; }
            /// <summary>
            /// Holds fief tax rate
            /// </summary>
            public double taxRate { get; set; }
            /// <summary>
            /// Holds fief tax rate (next season)
            /// </summary>
            public double taxRateNext { get; set; }
            /// <summary>
            /// Holds expenditure on officials (next season)
            /// </summary>
            public uint officialsSpendNext { get; set; }
            /// <summary>
            /// Holds expenditure on garrison (next season)
            /// </summary>
            public uint garrisonSpendNext { get; set; }
            /// <summary>
            /// Holds expenditure on infrastructure (next season)
            /// </summary>
            public uint infrastructureSpendNext { get; set; }
            /// <summary>
            /// Holds expenditure on keep (next season)
            /// </summary>
            public uint keepSpendNext { get; set; }
            /// <summary>
            /// Holds key data for current season.
            /// 0 = loyalty,
            /// 1 = GDP,
            /// 2 = tax rate,
            /// 3 = official expenditure,
            /// 4 = garrison expenditure,
            /// 5 = infrastructure expenditure,
            /// 6 = keep expenditure,
            /// 7 = keep level,
            /// 8 = income,
            /// 9 = family expenses,
            /// 10 = total expenses,
            /// 11 = overlord taxes,
            /// 12 = overlord tax rate,
            /// 13 = bottom line
            /// </summary>
            public double[] keyStatsCurrent = new double[14];
            /// <summary>
            /// Holds key data for previous season
            /// </summary>
            public double[] keyStatsPrevious = new double[14];
            /// <summary>
            /// Holds fief keep level
            /// </summary>
            public double keepLevel { get; set; }
            /// <summary>
            /// Holds fief loyalty
            /// </summary>
            public double loyalty { get; set; }
            /// <summary>
            /// Holds fief status (calm, unrest, rebellion)
            /// </summary>
            public char status { get; set; }
            /// <summary>
            /// Holds reference to fief language and dialect
            /// </summary>
            public string languageID { get; set; }
            /// <summary>
            /// Holds reference to fief terrain
            /// </summary>
            public string terrainID { get; set; }
            /// <summary>
            /// Holds overviews of characters present in fief
            /// </summary>
            public ProtoCharacterOverview[] charactersInFief { get;set; }
            /// <summary>
            /// Holds characters banned from keep (charIDs)
            /// </summary>
            public ProtoCharacterOverview[] barredCharacters { get; set; }
            /// <summary>
            /// Holds nationalities banned from keep (IDs)
            /// </summary>
            public string[] barredNationalities { get; set; }
            /// <summary>
            /// Holds fief ancestral owner (PlayerCharacter object)
            /// </summary>
            public ProtoCharacterOverview ancestralOwner { get; set; }
            /// <summary>
            /// Holds fief bailiff (Character object)
            /// </summary>
            public ProtoCharacterOverview bailiff { get; set; }
            /// <summary>
            /// Number of days the bailiff has been resident in the fief (this season)
            /// </summary>
            public Double bailiffDaysInFief { get; set; }
            /// <summary>
            /// Holds fief treasury
            /// </summary>
            public int treasury { get; set; }
            /// <summary>
            /// Holds overviews of armies present in the fief (armyIDs)
            /// </summary>
            public ProtoArmyOverview[] armies { get; set; }
            /// <summary>
            /// Identifies if recruitment has occurred in the fief in the current season
            /// </summary>
            public bool hasRecruited { get; set; }
            /// <summary>
            /// Identifies if pillage has occurred in the fief in the current season
            /// </summary>
            public bool isPillaged { get; set; }
            /// <summary>
            /// Siege (siegeID) of active siege
            /// </summary>
            public String siege { get; set; }
        }

        /// <summary>
        /// Class for summarising the important details of an army
        /// Can be used in conjunction with byte arrays to create a list of armies
        /// </summary>
        [ProtoContract]
        public class ProtoArmyOverview : ProtoMessage<T>
        {
            // Holds army id
            [ProtoMember(1)]
            public string armyID { get; set; }
            [ProtoMember(2)]
            public string leaderID { get; set; }
            [ProtoMember(3)]
            public string leaderName { get; set; }
            [ProtoMember(4)]
            public string locationID { get; set; }
            [ProtoMember(5)]
            public uint armySize { get; set; }
        }
        /// <summary>
        /// Class for summarising the basic details of a character
        /// Can be used in conjunction with byte arrays to create a list of characters
        /// </summary>
        [ProtoContract]
        public class ProtoCharacterOverview : ProtoMessage<T>
        {
            // Contains character ID
            [ProtoMember(1)]
            public string charID { get; set; }
            // Contains character name (first name + delimiter + family name)
            [ProtoMember(2)]
            public string charName { get; set; }
            // Contains character role or function (son, wife, employee etc)
            [ProtoMember(3)]
            public string role { get; set; }
            // Contains location ID
            [ProtoMember(4)]
            public string locationID { get; set; }
        }

        [ProtoContract]
        public class ProtoPillageResult : ProtoMessage<T>
        {
            [ProtoMember(1)]
            public string fiefID { get; set; }
            [ProtoMember(2)]
            public double daysTaken { get; set; }
            [ProtoMember(3)]
            public uint populationLoss { get; set; }
            [ProtoMember(4)]
            public int treasuryLoss { get; set; }
            [ProtoMember(5)]
            public double loyaltyLoss { get; set; }
            [ProtoMember(6)]
            public double fieldsLoss { get; set; }
            [ProtoMember(7)]
            public double baseMoneyPillaged { get; set; }
            [ProtoMember(8)]
            public double bonusMoneyPillaged { get; set; }
            [ProtoMember(9)]
            public double jackpot { get; set; }
            [ProtoMember(10)]
            public double statuteModifier { get; set; }
            
        }

        [ProtoContract]
        public class ProtoSiegeDisplay : ProtoMessage<T>
        {

        }

        /**************** MESSAGES FROM CLIENT ***********************/

        /// <summary>
        /// Class for sending details of a detachment
        /// Character ID of PlayerCharacter leaving detachment is obtained via connection details
        /// </summary>
        [ProtoContract]
        public class ProtoDetachment : ProtoMessage<T>
        {
            /// <summary>
            /// Array of troops (size = 7)
            /// </summary>
            [ProtoMember(1)]
            public uint[] troops;
            [ProtoMember(2)]
            public string leftFor { get; set; }

            public ProtoDetachment(uint[] troops, string leftFor)
            {
                this.troops = troops;
                this.leftFor = leftFor;
            }
        }
        /// <summary>
        /// Class for handling the transfer of money between fiefs
        /// </summary>
        [ProtoContract]
        public class ProtoTransfer : ProtoMessage<T>
        {
            // ID of the fief transferring the funds
            [ProtoMember(1)]
            public string fiefTo { get; set; }
            // ID of the fief receiving the funds
            [ProtoMember(2)]
            public string fiefFrom { get; set; }
            [ProtoMember(3)]
            // amount being transferred
            public int amount { get; set; }
        }
        /// <summary>
        /// Class for transferring money between players (player sending money obtained from connection
        /// </summary>
        [ProtoContract]
        public class ProtoTransferPlayer : ProtoMessage<T>
        {
            // ID of player to receive money- will transfer the funds to their PlayerCharacter
            [ProtoMember(1)]
            public string playerTo { get; set; }
            // amount to transfer
            [ProtoMember(2)]
            public int amount { get; set; }
        }

        /// <summary>
        /// Class for specifying which fief to travel to, via which route and which character
        /// Essentially handles TravelTo, MoveCharacter and multimoves
        /// </summary>
        [ProtoContract]
        public class ProtoTravelTo : ProtoMessage<T>
        {
            // Fief to travel to
            [ProtoMember(1)]
            public string travelTo { get; set; }
            // Route to take, if any
            [ProtoMember(2)]
            public string[] travelVia { get; set; }
            // character who will be travelling
            [ProtoMember(3)]
            public string characterID { get; set; }
        }
        /// <summary>
        /// Used for including all subtypes of ProtoMessage
        /// </summary>
        public void initialiseTypes()
        {
            List<Type> subMessages = typeof(ProtoMessage<T>).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ProtoMessage<T>))).ToList();
            int i = 4;
            foreach (Type message in subMessages)
            {
                RuntimeTypeModel.Default.Add(typeof(ProtoMessage<T>), true).AddSubType(i, message);
                i++;
            }
        }
    }

}
