using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
/// <summary>
/// enum representing all valid actions in the gane
/// </summary>
public enum Actions
{
	ViewChar, GetNPCList, TravelTo, MoveCharacter, ViewFief, AppointBailiff, RemoveBailiff, BarCharacters, BarNationalities, UnbarCharacters, UnbarNationalities, GrantFiefTitle, AdjustExpenditure, TransferFunds,
	TransferFundsToPlayer, EnterExitKeep, ListCharsInMeetingPlace, TakeThisRoute, Camp, AddRemoveEntourage, ProposeMarriage, AcceptRejectProposal, RejectProposal, AppointHeir, TryForChild, RecruitTroops, MaintainArmy, AppointLeader, DropOffTroops,
	ListDetachments, PickUpTroops, PillageFief, BesiegeFief, AdjustCombatValues, ExamineArmiesInFief, Attack, ViewJournalEntries, SiegeRoundReduction, SiegeRoundStorm, SiegeRoundNegotiate, SiegeList
}
/// <summary>
/// enum representing all strings that may be sent to a client,
///  mapped to string from enum on client side
/// </summary>
public enum DisplayMessages
{
	None = 0, Success, Error, Armies, Fiefs, Characters, JournalEntries, ArmyMaintainInsufficientFunds, ArmyMaintainCost, ArmyMaintainConfirm, JournalProposal, JournalProposalReply, JournalMarriage, ChallengeKingSuccess, ChallengeKingFail, ChallengeProvinceSuccess, ChallengeProvinceFail, newEvent,
	SwitchPlayerErrorNoID, SwitchPlayerErrorIDInvalid, ChallengeErrorExists, SiegeNegotiateSuccess, SiegeNegotiateFail, SiegeStormSuccess, SiegeStormFail, SiegeEndDefault, SiegeErrorDays, SiegeRaised, SiegeReduction, ArmyMove,
	ArmyAttritionDebug, ArmyDetachmentArrayWrongLength, ArmyDetachmentNotEnoughTroops, ArmyDetachmentNotSelected, ErrorGenericNotEnoughDays, ErrorGenericPoorOrganisation, ErrorGenericUnidentifiedRecipient, ArmyNoLeader, ArmyBesieged,
	ArmyAttackSelf, ArmyPickupsDenied, ArmyPickupsNotEnoughDays, BattleBringSuccess, BattleBringFail, BattleResults, ErrorGenericNotInSameFief, BirthAlreadyPregnant, BirthSiegeSeparation, BirthNotMarried, CharacterMarriageDeath, CharacterDeath, CharacterEnterArmy,
	CharacterAlreadyArmy, CharacterNationalityBarred, CharacterBarred, CharacterRoyalGiftPlayer, CharacterRoyalGiftSelf, CharacterNotMale, CharacterNotOfAge, CharacterLeaderLocation, CharacterLeadingArmy, CharacterDaysJourney, CharacterSpousePregnant, CharacterSpouseNotPregnant, CharacterSpouseNeverPregnant,
	CharacterBirthOK, CharacterBirthChildDead, CharacterBirthMumDead, CharacterBirthAllDead, RankTitleTransfer, CharacterCombatInjury, CharacterProposalMan, CharacterProposalUnderage, CharacterProposalEngaged, CharacterProposalMarried, CharacterProposalFamily, CharacterProposalIncest, CharacterRemovedFromEntourage, CharacterCamp,
	CharacterCampAttrition, CharacterBailiffDuty, CharacterInvalidMovement, ErrorGenericFiefUnidentified, CharacterOfferLow, CharacterOfferHigh, CharacterOfferOk, CharacterOfferAlmost, CharacterOfferHaggle, CharacterBarredKeep, CharacterRecruitOwn, CharacterRecruitAlready, CharacterLoyaltyLanguage, ErrorGenericInsufficientFunds, CharacterRecruitSiege, CharacterRecruitRebellion ,
	CharacterTransferTitle, CharacterTitleOwner, CharacterTitleHighest, CharacterTitleKing, CharacterTitleAncestral, CharacterHeir, PillageInitiateSiege, PillageRetreat, PillageDays, PillageOwnFief, PillageUnderSiege, PillageSiegeAlready, PillageAlready, PillageArmyDefeat, PillageSiegeRebellion, FiefExpenditureAdjustment, FiefExpenditureAdjusted, FiefStatus, FiefOwnershipHome ,
	FiefOwnershipNewHome, FiefOwnershipNoFiefs, FiefChangeOwnership, FiefQuellRebellionFail, FiefEjectCharacter, ProvinceAlreadyOwn, KingdomAlreadyKing, KingdomOwnershipChallenge, ProvinceOwnershipChallenge, ErrorGenericCharacterUnidentified, ErrorGenericUnauthorised, ErrorGenericMessageInvalid, ErrorGenericTooFarFromFief, FiefNoBailiff, FiefCouldNotBar, FiefCouldNotUnbar,
	ErrorGenericBarOwnNationality, ErrorGenericPositiveInteger, GenericReceivedFunds, ErrorGenericNoHomeFief, CharacterRecruitInsufficientFunds, CharacterRecruitOk, SiegeNotBesieger, JournalEntryUnrecognised, JournalEntryNotProposal, ErrorGenericArmyUnidentified, ErrorGenericSiegeUnidentified
}

/**************A Note on ProtoBufs ************
     * This class is used to transmit information to players. As such, it is responsible for deciding which information a client can see
     * If there are certain details you want to hide from certain clients, there are several options:
     * 
     * For fields that you want no clients to see, set the visibility to clients. Note that this will also hide the information from admin.
     * 
     * For cases where there are a few distinct groups of players, having a constructor for each group may be the best way to quickly and easily control information visibility.
     * 
     * Another option is to have one or two constructors, and then to have a method that can be called to include additional information.
     * This is the approach I have used, as it offers good tunability. However, feel free to adjust to suit
     * 
     * Finally, having a constructor with multiple default values would allow you to control visibility by passing in however much information you want to include and leaving the rest null or 0
     * However, the above approach would prevent the ability to pass in an entire object and initialise fields by using the object's accessor methods.
     * 
     * In the rare case that a field should only be included/excluded when it is a certain value, look at the documentation for XML ShouldSerialize_fieldname
     * 
     * It may also be possible to use reflection to get the constructor and invoke it with whatever parameters you like, but this comes with significant overheads
     *
     *****************/
/// <summary>
/// Class for translating objects to ProtoBuf constructs
/// Object fields can be hidden from clients by setting the desired field to null
/// </summary>
[ProtoContract]
public class ProtoMessage
{
	/// <summary>
	/// Contains the underlying type of the message. Used to easily distinguish between messages
	/// </summary>
	[ProtoMember(1)]
	public Enum MessageType {get;set;}
	
	/// <summary>
	/// Contains a message or messageID for the client
	/// Used when sending error messages
	/// </summary>
	[ProtoMember(2)]
	public String Message;
	
	/// <summary>
	/// Contains any fields that need to be sent along with the message
	/// e.g. amount of overspend in fief
	/// </summary>
	[ProtoMember(3)]
	public string[] MessageFields;
	
	public Enum getMsgType()
	{
		return MessageType;
	}
	public string getMessage()
	{
		return this.Message;
	}
	public string[] getFields()
	{
		return this.MessageFields;
	}
	public ProtoMessage()
	{
		MessageFields = new string[5];
	}
	/// <summary>
	/// Used for including all subtypes of ProtoMessage
	/// </summary>
	
	public void initialiseTypes()
	{
		List<Type> subMessages = typeof(ProtoMessage).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ProtoMessage))).ToList();
		int i = 4;
		foreach (Type message in subMessages)
		{
			RuntimeTypeModel.Default.Add(typeof(ProtoMessage), true).AddSubType(i, message);
			i++;
		}
	}
}

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoGenericArray<T> : ProtoMessage
{
	public T[] fields { get; set; }
	
	public ProtoGenericArray()
		: base()
	{
		
	}
}
/// <summary>
/// Class for serializing an Ailment
/// (At present, only minimumEffect is hidden)
/// </summary>
[ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
public class ProtoAilment: ProtoMessage
{
	/// <summary>
	/// Holds ailmentID
	/// </summary>
	string _AilmentID { get; set; }
	/// <summary>
	/// Holds ailment description
	/// </summary>
	public String _description { get; set; }
	/// <summary>
	/// Holds ailment date
	/// </summary>
	public string _when { get; set; }
	/// <summary>
	/// Holds current ailment effect
	/// </summary>
	public uint _effect { get; set; }
	/// <summary>
	/// Holds minimum ailment effect
	/// </summary>
	public uint _minimumEffect { get; set; }
	
	public ProtoAilment() {
	}
}
/// <summary>
/// Class for serializing an Army
/// The amount of information a player can view about an army depends on whether that player 
/// ownes the army, how close the player is etc. 
/// Can be tuned later to include information obtained via methods such as spying, interrogation, or defection
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoArmy : ProtoMessage
{
	/// <summary>
	/// Holds army ID
	/// </summary>
	public String armyID { get; set; }
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
	public uint[] troops = new uint[7] { 0, 0, 0, 0, 0, 0, 0 };
	/// <summary>
	/// Holds army leader ID and name in the format:
	/// ID|first_name|last_name
	/// </summary>
	public string leader { get; set; }
	/// <summary>
	/// Holds army owner ID and name in the format:
	/// ID|first_name|last_name
	/// </summary>
	public string owner { get; set; }
	/// <summary>
	/// Holds army's remaining days in season
	/// </summary>
	public double days { get; set; }
	/// <summary>
	/// Holds army location in the format:
	/// fiefID|fiefName|provinceName|kingdomName
	/// </summary>
	public string location { get; set; }
	/// <summary>
	/// Indicates whether army is being actively maintained by owner
	/// </summary>
	public bool isMaintained { get; set; }
	/// <summary>
	/// Indicates army's aggression level (automated response to combat)
	/// </summary>
	public byte aggression { get; set; }
	/// <summary>
	/// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
	/// </summary>
	public byte combatOdds { get; set; }
	/// <summary>
	/// String indicating army nationality
	/// </summary>
	public string nationality { get; set; }
	/// <summary>
	/// Holds siege status of army
	/// One of BESIEGING, BESIEGED, FIEF or none
	/// BESIEGING: army is currently besieging fief
	/// BESIEGED: army is under siege
	/// FIEF: the fief the army is in is under siege
	/// </summary>
	public string siegeStatus { get; set; }
	
	public ProtoArmy()
	{
		
	}
}
/// <summary>
/// Class for sending details of a character
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[ProtoInclude(14,typeof(ProtoPlayerCharacter))]
[ProtoInclude(15,typeof(ProtoNPC))]
public class ProtoCharacter : ProtoMessage
{
	/* BASIC CHARACTER DETAILS */
	/// <summary>
	/// Holds character ID
	/// </summary>
	public string charID { get; set; }
	/// <summary>
	/// Holds character's first name
	/// </summary>
	public string firstName { get; set; }
	/// <summary>
	/// Holds character's family name
	/// </summary>
	public string familyName { get; set; }
	/// <summary>
	/// Character's year of birth
	/// </summary>
	public uint birthYear{ get; set; }
	/// <summary>
	/// Character's birth season
	/// </summary>
	public byte birthSeason {get;set;}
	/// <summary>
	/// Holds if character male
	/// </summary>
	public bool isMale { get; set; }
	/// <summary>
	/// Holds the string representation of this character's nationality
	/// </summary>
	public string nationality { get; set; }
	/// <summary>
	/// Indicates whether a character is alive
	/// </summary>
	public bool isAlive { get; set; }
	/// <summary>
	/// Character's max health
	/// </summary>
	public double maxHealth { get; set; }
	/// <summary>
	/// Character's virility
	/// </summary>
	public double virility { get; set; }
	/// <summary>
	/// Bool detclaring whether character is in keep
	/// </summary>
	public bool inKeep { get; set; }
	/// <summary>
	/// Character's language ID
	/// </summary>
	public string language {get;set;}
	/// <summary>
	/// number of days left in season
	/// </summary>
	public double days { get; set; }
	/// <summary>
	/// Character's family ID
	/// </summary>
	public String familyID { get; set; }
	/// <summary>
	/// Character spouse charID
	/// </summary>
	public String spouse { get; set; }
	/// <summary>
	/// Character father charID
	/// </summary>
	public String father { get; set; }
	/// <summary>
	/// Character mother charID
	/// </summary>
	public String mother { get; set; }
	/// <summary>
	/// Character mother charID
	/// </summary>
	public string fiancee { get; set; }
	/// <summary>
	/// Character location (FiefID)
	/// </summary>
	public string location { get; set; }
	/// <summary>
	/// Character statureModifier
	/// </summary>
	public double statureModifier { get; set; }
	/// <summary>
	/// Character management skill
	/// </summary>
	public double management { get; set; }
	/// <summary>
	/// Character combat skill
	/// </summary>
	public double combat { get; set; }
	/// <summary>
	/// Holds character's traits
	/// </summary>
	public Pair[] traits { get; set; }
	/// <summary>
	/// Bool to indicate whether char is pregnant
	/// </summary>
	public bool isPregnant { get; set; }
	/// <summary>
	/// Holds char's title
	/// </summary>
	public string[] titles { get; set; }
	/// <summary>
	/// ArmyID, if char leads army
	/// </summary>
	public string armyID { get; set; }
	/// <summary>
	/// Character's ailments
	/// </summary>
	public Pair[] ailments { get; set; }
	/// <summary>
	/// IDs of Fiefs in char's GoTo list
	/// </summary>
	public string[] goTo { get; set; }
	
	public ProtoCharacter()
	{
	}
}
/// <summary>
/// Class for sending details of a PlayerCharacter
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoPlayerCharacter : ProtoCharacter
{
	/// <summary>
	/// Holds ID of player who is currently playing this PlayerCharacter
	/// Note that list of sieges and list of armies is stored elsewhere- see ProtoSiegeList and ProtoArmyList
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
	/// Holds IDs and names of character's employees and family
	/// </summary>
	public ProtoCharacterOverview[] myNPCs { get; set; }
	/// <summary>
	/// Holds IDs of character's owned fiefs
	/// </summary>
	public string[] ownedFiefs { get; set; }
	/// <summary>
	/// Holds IDs of character's owned provinces
	/// </summary>
	public string[] provinces { get; set; }
	/// <summary>
	/// Holds character's home fief (fiefID)
	/// </summary>
	public String homeFief { get; set; }
	/// <summary>
	/// Holds character's ancestral home fief (fiefID)
	/// </summary>
	public String ancestralHomeFief { get; set; }
	
	public ProtoPlayerCharacter() {
	}
}
/// <summary>
/// Class for sending details of Non-PlayerCharacter
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoNPC : ProtoCharacter
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
	public string lastOfferID { get; set; }
	public uint lastOfferAmount { get; set; }
	/// <summary>
	/// Denotes if in employer's entourage
	/// </summary>
	public bool inEntourage { get; set; }
	/// <summary>
	/// Denotes if is player's heir
	/// </summary>
	public bool isHeir { get; set; }
	
	public ProtoNPC() {
		
	}
}
/// <summary>
/// Class for sending fief details
/// Province, language and terrain are all stored client side- unless this changes there is no need to send these
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoFief : ProtoMessage
{
	/// <summary>
	/// ID of the fief
	/// </summary>
	public string fiefID { get; set; }
	/// <summary>
	/// CharID and name of fief title holder
	/// </summary>
	public string titleHolder { get; set; }
	/// <summary>
	/// CharID and name of fief owner
	/// </summary>
	public string owner { get; set; }
	/// <summary>
	/// Fief rank
	/// </summary>
	public string rank { get; set; }
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
	/// Holds number of troops that can be recruited in this fief
	/// </summary>
	public int militia { get; set; }
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
	/// Holds key data for next season
	/// </summary>
	public double[] keyStatsNext = new double[14];
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
	private bool hasRecruited { get; set; }
	/// <summary>
	/// Identifies if pillage has occurred in the fief in the current season
	/// </summary>
	public bool isPillaged { get; set; }
	/// <summary>
	/// Siege (siegeID) of active siege
	/// </summary>
	public String siege { get; set; }
	
	public ProtoFief() {
	}
}

/// <summary>
/// Class for summarising the important details of an army
/// Can be used in conjunction with byte arrays to create a list of armies
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoArmyOverview : ProtoMessage
{
	// Holds army id
	public string armyID { get; set; }
	public string leaderID { get; set; }
	public string leaderName { get; set; }
	public string locationID { get; set; }
	public uint armySize { get; set; }
	
	public ProtoArmyOverview() {
	}
	
}
/// <summary>
/// Class for summarising the basic details of a character
/// Can be used in conjunction with byte arrays to create a list of characters
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoCharacterOverview : ProtoMessage
{
	// Contains character ID
	public string charID { get; set; }
	// Contains character name (first name + delimiter + family name)
	public string charName { get; set; }
	// Contains character role or function (son, wife, employee etc)
	public string role { get; set; }
	// Contains location ID
	public string locationID { get; set; }
	// boolean for character gender
	public bool isMale { get; set; }
	
	public ProtoCharacterOverview() {
	}
}

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoPillageResult : ProtoMessage
{
	public string fiefID { get; set; }
	public string fiefName { get; set; }
	public bool isPillage { get; set; }
	
	public string fiefOwner { get; set; }
	public string defenderLeader { get; set; }
	public string armyOwner { get; set; }
	public string armyLeader { get; set; }
	
	public double daysTaken { get; set; }
	public int populationLoss { get; set; }
	public int treasuryLoss { get; set; }
	public double industryLoss { get; set; }
	public double loyaltyLoss { get; set; }
	public double fieldsLoss { get; set; }
	public double baseMoneyPillaged { get; set; }
	public double bonusMoneyPillaged { get; set; }
	public double moneyPillagedOwner { get; set; }
	public double jackpot { get; set; }
	public double statureModifier { get; set; }
	
	
	
	public ProtoPillageResult()
	{
		
	}
}
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoSiegeOverview : ProtoMessage
{
	/// <summary>
	/// Holds siege ID
	/// </summary>
	public String siegeID { get; set; }
	/// <summary>
	/// Holds fief being besieged (fiefID)
	/// </summary>
	public String besiegedFief { get; set; }
	/// <summary>
	/// Holds besieging player
	/// </summary>
	public string besiegingPlayer { get; set; }
	/// <summary>
	/// Holds defending player
	/// </summary>
	public string defendingPlayer { get; set; }
	
	
	public ProtoSiegeOverview()
	{
		
	}
	
}
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoSiegeDisplay : ProtoMessage
{
	/// <summary>
	/// Holds siege ID
	/// </summary>
	public String siegeID { get; set; }
	/// <summary>
	/// Holds year the siege started
	/// </summary>
	public uint startYear { get; set; }
	/// <summary>
	/// Holds season the siege started
	/// </summary>
	public byte startSeason { get; set; }
	/// <summary>
	/// Holds besieging player
	/// </summary>
	public string besiegingPlayer { get; set; }
	/// <summary>
	/// Holds defending player
	/// </summary>
	public string defendingPlayer { get; set; }
	/// <summary>
	/// Holds besieging army (armyID)
	/// </summary>
	public string besiegerArmy { get; set; }
	/// <summary>
	/// Holds defending garrison (armyID)
	/// </summary>
	public string defenderGarrison { get; set; }
	/// <summary>
	/// Holds fief being besieged (fiefID)
	/// </summary>
	public String besiegedFief { get; set; }
	/// <summary>
	/// Holds days left in current season
	/// </summary>
	public double days { get; set; }
	/// <summary>
	/// Holds the keep level at the start of the siege
	/// </summary>
	public double startKeepLevel { get; set; }
	/// <summary>
	/// Holds current keep level
	/// </summary>
	public double keepLevel { get; set; }
	/// <summary>
	/// Casualties for attacker this round
	/// </summary>
	public int casualtiesAttacker { get; set; }
	/// <summary>
	/// Casualties for defender this round
	/// </summary>
	public int casualtiesDefender { get; set; }
	/// <summary>
	/// Total casualties this siege suffered by attacker
	/// </summary>
	public int totalCasualtiesAttacker { get; set; }
	/// <summary>
	/// Holds total casualties suffered so far by defender
	/// </summary>
	public int totalCasualtiesDefender { get; set; }
	/// <summary>
	/// Holds total duration of siege so far (days)
	/// </summary>
	public double totalDays { get; set; }
	/// <summary>
	/// Holds additional defending army 
	/// </summary>
	public string defenderAdditional { get; set; }
	/// <summary>
	/// Holds season and year the siege ended
	/// </summary>
	public String endDate { get; set; }
	
	public string[] captivesTaken { get; set; }
	
	public int totalRansom { get; set; }
	
	public bool besiegerWon { get; set; }
	public int lootLost { get; set; }
	
	public ProtoSiegeDisplay() {
	}
}


[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoBattle : ProtoMessage
{
	
}
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoJournalEntry
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
	/// Holds characters associated with event and their role
	/// </summary>
	public ProtoCharacterOverview[] personae { get; set; }
	/// <summary>
	/// Holds type of event (e.g. battle, birth)
	/// </summary>
	public String type { get; set; }
	/// <summary>
	/// Holds location of event (fiefID)
	/// </summary>
	public String location { get; set; }
	/// <summary>
	/// Holds message enum
	/// </summary>
	public Enum messageIdentifier { get; set; }
	/// <summary>
	/// Message description
	/// </summary>
	public string description { get; set; }
	/// <summary>
	/// Indicates whether entry has been viewed
	/// </summary>
	public bool viewed { get; set; }
	/// <summary>
	/// Indicates whether entry has been replied to (e.g. for Proposals)
	/// </summary>
	public bool replied { get; set; }
	/// <summary>
	/// Holds fields to be included with message
	/// </summary>
	public string[] fields { get; set; }
	/// <summary>
	/// Holds ProtoMessage containing details of event. More flexible than strings.
	/// </summary>
	public ProtoMessage dataFields { get; set; }
	
	public ProtoJournalEntry()
	{
		
	}
}
/// <summary>
/// ProtoMessage for sending an entire Journal
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoJournal : ProtoMessage  {
	/// <summary>
	/// Holds entries
	/// </summary>
	public ProtoJournalEntry[] entries;
	/// <summary>
	/// Indicates presence of new (unread) entries
	/// </summary>
	public bool areNewEntries = false;
	/// <summary>
	/// Priority level of new (unread) entries
	/// </summary>
	public byte priority = 0;
	
	public ProtoJournal()
	{
		
	}
}




/**************** MESSAGES FROM CLIENT ***********************/

/// <summary>
/// Class for sending details of a detachment
/// Character ID of PlayerCharacter leaving detachment is obtained via connection details
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoDetachment : ProtoMessage
{
	/// <summary>
	/// Array of troops (size = 7)
	/// </summary>
	public uint[] troops;
	/// <summary>
	/// Character detachment is left for
	/// </summary>
	public string leftFor { get; set; }
	/// <summary>
	/// ArmyID of army from which detachment was created
	/// </summary>
	public string armyID { get; set; }
	/// <summary>
	/// Details of person who left this detachment (used in sending details of detachments to client)
	/// </summary>
	public string leftBy { get; set; }
	/// <summary>
	/// Days left of person who created detachment at time of creation
	/// </summary>
	public int days { get; set; }
	public ProtoDetachment(uint[] troops, string leftFor, string armyID)
	{
		this.troops = troops;
		this.leftFor = leftFor;
		this.armyID = armyID;
		this.MessageType = Actions.DropOffTroops;
	}
	public ProtoDetachment()
	{
		
	}
}
/// <summary>
/// Class for handling the transfer of money between fiefs
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoTransfer : ProtoMessage
{
	// ID of the fief transferring the funds
	public string fiefTo { get; set; }
	// ID of the fief receiving the funds
	public string fiefFrom { get; set; }
	// amount being transferred
	public int amount { get; set; }
	
	public ProtoTransfer(): base()
	{
		this.MessageType = Actions.TransferFunds;
	}
}
/// <summary>
/// Class for transferring money between players (player sending money obtained from connection)
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoTransferPlayer : ProtoMessage
{
	// ID of player to receive money- will transfer the funds to their PlayerCharacter
	public string playerTo { get; set; }
	// amount to transfer - sends between home fiefs
	public int amount { get; set; }
	
	public ProtoTransferPlayer() : base()
	{
		this.MessageType = Actions.TransferFundsToPlayer;
	}
}
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Pair {
	public string key { get; set; }
	public string value { get; set; }
	public Pair() {
	}
	public Pair(string key, string val) {
		this.key = key;
		this.value = val;
	}
}

/// <summary>
/// Class for specifying which fief to travel to, via which route and which character
/// Essentially handles TravelTo, MoveCharacter and multimoves
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoTravelTo : ProtoMessage
{
	// Fief to travel to
	public string travelTo { get; set; }
	// Route to take, if any
	public string[] travelVia { get; set; }
	// character who will be travelling
	public string characterID { get; set; }
	
	public ProtoTravelTo()
	{
		this.MessageType = Actions.TravelTo;
	}
}

/// <summary>
/// Class for storing recruitment information
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoRecruit : ProtoMessage
{
	/// <summary>
	/// Army ID of army to recruit into (null = new army)
	/// </summary>
	public string armyID { get; set; }
	/// <summary>
	/// Amount of troops to recruit
	/// </summary>
	public uint amount { get; set; }
	/// <summary>
	/// Bool representing whether the player has confirmed that they are happy to purchase troops
	/// </summary>
	public bool isConfirm { get; set; }
	/// <summary>
	/// Holds amount in player's treasury, in the event that game has to send fail message to client
	/// </summary>
	public int treasury { get; set; }
	
	public int cost { get; set; }
	public ProtoRecruit() : base()
	{
		this.MessageType = Actions.RecruitTroops;
	}
}

/// <summary>
/// Class for sending combat values (bytes)
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ProtoCombatValues : ProtoMessage
{
	/// <summary>
	/// Army aggression value
	/// </summary>
	public byte aggression { get; set; }
	/// <summary>
	/// Army combat odds
	/// </summary>
	public byte odds { get; set; }
	/// <summary>
	/// Army whose combat values are to be adjusted
	/// </summary>
	public string armyID { get; set; }
	
	public ProtoCombatValues(byte aggr, byte odds, string armyID)
	{
		this.MessageType = Actions.AdjustCombatValues;
		this.aggression = aggr;
		this.odds = odds;
		this.armyID = armyID;
	}
	
	public ProtoCombatValues()
	{
		this.MessageType = Actions.AdjustCombatValues;
	}
}
