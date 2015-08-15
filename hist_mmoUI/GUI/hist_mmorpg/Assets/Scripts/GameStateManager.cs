using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ProtoBuf;
using ProtoBuf.Meta;
using hist_mmorpg;
using System.Threading;
using UnityEngine.UI;
public enum Actions
{
	Update=0, LogIn, UseChar, GetPlayers, ViewChar,ViewArmy, GetNPCList, HireNPC, FireNPC, TravelTo, MoveCharacter, ViewFief,ViewMyFiefs, AppointBailiff, RemoveBailiff, BarCharacters, BarNationalities, UnbarCharacters, UnbarNationalities, GrantFiefTitle, AdjustExpenditure, TransferFunds,
	TransferFundsToPlayer, EnterExitKeep, ListCharsInMeetingPlace, TakeThisRoute, Camp, AddRemoveEntourage, ProposeMarriage, AcceptRejectProposal, RejectProposal, AppointHeir, TryForChild, RecruitTroops, MaintainArmy, AppointLeader, DropOffTroops,
	ListDetachments,ListArmies, PickUpTroops, PillageFief, BesiegeFief, AdjustCombatValues, ExamineArmiesInFief, Attack, ViewJournalEntries, ViewJournalEntry, SiegeRoundReduction, SiegeRoundStorm, SiegeRoundNegotiate, SiegeList, ViewSiege, EndSiege, DisbandArmy, SpyFief, SpyCharacter, SpyArmy, Kidnap, ViewCaptives, ViewCaptive, RansomCaptive, ReleaseCaptive, ExecuteCaptive, RespondRansom
}
/// <summary>
/// enum representing all strings that may be sent to a client,
///  mapped to string from enum on client side
/// </summary>
public enum DisplayMessages
{
	None = 0, Success, Error, Armies, Fiefs, Characters, JournalEntries, ArmyMaintainInsufficientFunds, ArmyMaintainCost, ArmyMaintainConfirm, ArmyMaintainedAlready, JournalProposal, JournalProposalReply, JournalMarriage, ChallengeKingSuccess, ChallengeKingFail, ChallengeProvinceSuccess, ChallengeProvinceFail, newEvent,
	SwitchPlayerErrorNoID, SwitchPlayerErrorIDInvalid, ChallengeErrorExists, SiegeNegotiateSuccess, SiegeNegotiateFail, SiegeStormSuccess, SiegeStormFail, SiegeEndDefault, SiegeErrorDays, SiegeRaised, SiegeReduction, ArmyMove,
	ArmyAttritionDebug, ArmyDetachmentArrayWrongLength, ArmyDetachmentNotEnoughTroops, ArmyDetachmentNotSelected, ArmyRetreat, ArmyDisband, ErrorGenericNotEnoughDays, ErrorGenericPoorOrganisation, ErrorGenericUnidentifiedRecipient, ArmyNoLeader, ArmyBesieged,
	ArmyAttackSelf, ArmyPickupsDenied, ArmyPickupsNotEnoughDays, BattleBringSuccess, BattleBringFail, BattleResults, ErrorGenericNotInSameFief, BirthAlreadyPregnant, BirthSiegeSeparation, BirthNotMarried, CharacterMarriageDeath, CharacterDeath, CharacterDeathNoHeir, CharacterEnterArmy,
	CharacterAlreadyArmy, CharacterNationalityBarred, CharacterBarred, CharacterRoyalGiftPlayer, CharacterRoyalGiftSelf, CharacterNotMale, CharacterNotOfAge, CharacterLeaderLocation, CharacterLeadingArmy, CharacterDaysJourney, CharacterSpousePregnant, CharacterSpouseNotPregnant, CharacterSpouseNeverPregnant,
	CharacterBirthOK, CharacterBirthChildDead, CharacterBirthMumDead, CharacterBirthAllDead, RankTitleTransfer, CharacterCombatInjury, CharacterProposalMan, CharacterProposalUnderage, CharacterProposalEngaged, CharacterProposalMarried, CharacterProposalFamily, CharacterProposalIncest,CharacterProposalAlready,CharacterRemovedFromEntourage, CharacterCamp,
	CharacterCampAttrition, CharacterBailiffDuty, CharacterInvalidMovement, ErrorGenericFiefUnidentified, CharacterHireNotEmployable, CharacterFireNotEmployee, CharacterOfferLow, CharacterOfferHigh, CharacterOfferOk, CharacterOfferAlmost, CharacterOfferHaggle, CharacterBarredKeep, CharacterRecruitOwn, CharacterRecruitAlready, CharacterLoyaltyLanguage, ErrorGenericInsufficientFunds, CharacterRecruitSiege, CharacterRecruitRebellion,
	CharacterTransferTitle, CharacterTitleOwner, CharacterTitleHighest, CharacterTitleKing, CharacterTitleAncestral, CharacterHeir, PillageInitiateSiege, PillageRetreat, PillageDays, PillageOwnFief, PillageUnderSiege, PillageSiegeAlready, PillageAlready, PillageArmyDefeat, PillageSiegeRebellion, FiefExpenditureAdjustment, FiefExpenditureAdjusted, FiefStatus, FiefOwnershipHome,
	FiefOwnershipNewHome, FiefOwnershipNoFiefs, FiefChangeOwnership, FiefQuellRebellionFail, FiefEjectCharacter, ProvinceAlreadyOwn, KingdomAlreadyKing, KingdomOwnershipChallenge, ProvinceOwnershipChallenge, ErrorGenericCharacterUnidentified, ErrorGenericUnauthorised, ErrorGenericMessageInvalid, ErrorGenericTooFarFromFief, FiefNoBailiff, FiefCouldNotBar, FiefCouldNotUnbar,
	ErrorGenericBarOwnNationality, ErrorGenericPositiveInteger, GenericReceivedFunds, ErrorGenericNoHomeFief, CharacterRecruitInsufficientFunds, CharacterRecruitOk, SiegeNotBesieger, JournalEntryUnrecognised, JournalEntryNotProposal, ErrorGenericArmyUnidentified, ErrorGenericSiegeUnidentified, ErrorSpyDead,ErrorSpyCaptive,ErrorSpyOwn, SpySuccess, SpyFail, SpySuccessDetected, SpyFailDetected, SpyFailDead, EnemySpySuccess, EnemySpyFail,EnemySpyKilled, CharacterHeldCaptive, RansomReceived, RansomPaid, RansonDenied, RansomRepliedAlready, RansomCaptiveDead, RansomAlready,NotCaptive, EntryNotRansom,KidnapOwnCharacter,KidnapDead,KidnapNoPlayer,KidnapSuccess,KidnapSuccessDetected,KidnapFailDetected,KidnapFailDead,KidnapFail,EnemyKidnapSuccess,EnemyKidnapSuccessDetected,EnemyKidnapFail,EnemyKidnapKilled, CharacterExecuted, CharacterReleased, LogInSuccess, LogInFail, YouDied, YouDiedNoHeir
}
	public class GameStateManager : MonoBehaviour {
		// Field for enforcing singleton pattern
	public static GameStateManager gameState;
	// Queue of Actions that must be executed on main thread. Work around for multithreading in Unity
	public readonly static Queue<Action> ExecutionQueue = new Queue<Action>();
	/**** Game objects representing the current objects being displayed ******/
	// The menu bar
	public GameObject menuBar;
	// The Object showing remaining days
	public GameObject DaysLeft;
	// Holds the TravelManager for when in Travel Scene
	public TravelManager travelManager {get;set;}
	// REPLACE WITH QUEUE
	// Stores a state before changing scenes so that the scene can begin in the correct state, such as listing only employees in "Household"
	public string preLoadState {get;set;}
	// Queue of actions to take place on scene load, useful if a scene needs to start in a certain state
	public Queue<Action> SceneLoadQueue= new Queue<Action>();
	public void Awake() {
		if(gameState==null) {
			Debug.Log ("INITIALIZING GAME STATE");
			gameState=this;
			DontDestroyOnLoad(this.gameObject);
			ClientSerializer cs = new ClientSerializer();
			cs.DeserializeAll();
			HexMapGraph mapgraph = new HexMapGraph();
			mapgraph.deserialize ();
			Globals_Game.gameMap = mapgraph;
			Globals_Game.LoadStrings ();
			//initialiseTypes();
			DummyStart ();
		}
		else {
			if(this!=gameState)
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {		
	}
	// Enter Login State
	// Update is called once per frame
	void Update () {
		while(ExecutionQueue.Count>0) {
			Debug.Log ("Performing action from queue");
			ExecutionQueue.Dequeue ().Invoke ();
		}
		if(DaysLeft) {
			DaysLeft.GetComponentInChildren<Text>().text = "Days left: "+Globals_Client.activeCharacter.days;
		}
	}
	// Test method for initializing data
	void DummyStart() {
		Globals_Client.TravelModifier=2.12;
		Globals_Client.currentLocation=Globals_Game.fiefMasterList["ESW04"];
	}


	// Note- a null charID will default to client's head of family
	public static void UseCharacter(string charID=null) {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.UseChar;
		message.Message=charID;
		Debug.Log ("Attempting to use "+charID);
		NetworkScript.Send (message);
	}

	public void ActionController(ProtoMessage m) {
		switch(m.ActionType) {
		// Updates the player or the game state, either by changing the client globals or displaying a message
		case Actions.Update: {

			Debug.Log ("Got update");
			// If the message is a ProtoClient, message contains updated game state
			ProtoClient clientDetails = m as ProtoClient;
			if(clientDetails!=null) {
				PerformUpdate(clientDetails);
			}
			// Otherwise, message contains some information for player. Display message
			else {
				if(m.ResponseType==DisplayMessages.newEvent) {
					// Display event
					Debug.Log("new event!");
					var Journal = menuBar.transform.Find ("Journal").gameObject;
					Journal.GetComponentInChildren<Text>().text="Journal\n(!)";
					Journal.GetComponent<Image>().color=Color.green;
					Globals_Client.unreadEntries++;
					var journalItems = Journal.GetComponentsInChildren<Transform>(true);
					foreach(Transform item in journalItems) {
						if(item.name.Equals("Unread")) {
							var text = item.gameObject.GetComponentsInChildren<Text>(true);
							text[0].text="Unread("+Globals_Client.unreadEntries+")";
							return;
						}
					}

				}
				else {
					DisplayMessage(m);
				}
			}
			break;
		}
		// Switches to using an NPC to perform actions
		case Actions.UseChar: 
		{
			Debug.Log ("Got use char");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ProtoCharacter activechar = m as ProtoCharacter;
				if(activechar==null) {
					Debug.LogError("Error converting message to ProtoCharacter in UseChar");
				}
				else {
					Globals_Client.activeCharacter = activechar;
				}
			}
			// On succcess all updates recieved will contain the active character details
		}
			break;
		case Actions.GetPlayers:
		{
			ProtoGenericArray<ProtoPlayer> players = m as ProtoGenericArray<ProtoPlayer>;
			if(players==null) {
				DisplayMessage("Could not obtain list of players.");
				Debug.LogError ("Error converting to ProtoGenericArray<ProtoPlayer> in GetPlayers");
			}
			else {
				if(Application.loadedLevelName.Equals ("Fief")) {
					FiefController fief = FindObjectOfType<FiefController>();
					if(fief!=null) {
						fief.ShowPlayers(players.fields);
					}
					else {
						Debug.LogError ("Could not find FiefController (from GetPlayers)");
					}
				}
			}
		}
			break;
		case Actions.HireNPC: 
		{
			Debug.Log ("Got hire npc");
			// If cannot convert incoming message, attempt to display message as an error
			ProtoNPC charDetails = m as ProtoNPC;
			if(charDetails==null) {
				DisplayMessage(m);
				return;
			}
			ListDisplay listDisplay = FindObjectOfType<ListDisplay>();
			if(m.ResponseType==DisplayMessages.CharacterOfferOk||m.ResponseType==DisplayMessages.CharacterOfferHigh) {
				Debug.Log ("Is high or ok");
				// If char has been hired, remove from tavern list
				if(listDisplay.state.Contains ("tavern")) {
					Debug.Log ("Attempting to remove row");
					listDisplay.RemoveItem(charDetails.charID);
				}
			}
			// Update the character details display (will clear display and remove controls if NPC successfully hired)
			listDisplay.DisplayDetails (charDetails);

			// Finally, display message to client (Incoming message has response type and fields indicating hiring result
			DisplayMessage (m);
		}
			break;
		case Actions.FireNPC:
		{
			Debug.Log ("Got fire npc");
			// If failed, show error message
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				// If displaying a list of characters, remove this character
				ListDisplay listDisplay = FindObjectOfType<ListDisplay>();
				if(listDisplay!=null) {
					listDisplay.RemoveItem (m.Message);
				}
				// If fired NPC was active character, set active char to main character
				if(Globals_Client.activeChar.Equals (m.Message)) {
					UseCharacter (Globals_Client.pcID);
				}
			}
		}
			break;
		case Actions.AppointBailiff:
		{
			Debug.Log ("Got appoint bailiff");
			if(m.ResponseType!=DisplayMessages.Success){
				DisplayMessage(m);
			}
			else {
				ProtoFief fief = m as ProtoFief;
				if(m!=null) {
					var fiefController = FindObjectOfType<FiefController>();
					fiefController.DisplayFief (fief);
				}
				else {
					Debug.LogError ("Could not convert to ProtoFief in AppointBailiff");
					return;
				}

			}
		}
			break;
		case Actions.RemoveBailiff:
		{
			Debug.Log ("Got remove bailiff");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ProtoFief fief = m as ProtoFief;
				if(m!=null) {
					var fiefController = FindObjectOfType<FiefController>();
					fiefController.DisplayFief (fief);
				}
				else {
					Debug.LogError ("Could not convert to ProtoFief in RemoveBailiff");
					return;
				}
			}
		}
			break;
		case Actions.AdjustExpenditure:
		{
			Debug.Log ("Got adjust expenditure");
			if(m.ResponseType!=DisplayMessages.FiefExpenditureAdjusted) {
				DisplayMessage(m);
			}
			else {
				ProtoFief fief = m as ProtoFief;
				if(m!=null) {
					var fiefController = FindObjectOfType<FiefController>();
					fiefController.DisplayFief (fief);
				}
				else {
					Debug.LogError ("Could not convert to ProtoFief in RemoveBailiff");
					return;
				}
			}
		}
			break;
		case Actions.TransferFunds: {
			Debug.Log ("Got transfer funds");
			if(m.ResponseType!= DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				DisplayMessage("Transfer Successful");
				FiefController controller = FindObjectOfType<FiefController>();
				ProtoFief fief = m as ProtoFief;
				controller.DisplayFief (fief);
				controller.currentFiefDetails=fief;
			}
		}
			break;
		case Actions.TransferFundsToPlayer: {
			Debug.Log ("Got transfer funds to player");
			if(m.ResponseType!= DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				DisplayMessage ("Transfer Successful");
			}
		}
			break;
		case Actions.LogIn:
		{
			Debug.Log ("GotLogIn");
			if(m.ResponseType==DisplayMessages.LogInSuccess) {
				ProtoClient clientDetails = m as ProtoClient;
				if(clientDetails!=null) {
					PerformUpdate (clientDetails);
				}
				else {
					Debug.LogError ("Invalid Log In Message");
					return;
				}
				ExecutionQueue.Enqueue (()=> {Application.LoadLevel ("Travel");});
			}
			else {
				// Show Error Message
			}
		}
			break;
			/****TRAVEL CONTROLS*****/
		case Actions.EnterExitKeep:
			Debug.Log ("Enter exit keep");
			//TravelManager travelManager  = FindObjectOfType<TravelManager>();
			if(m.ResponseType==DisplayMessages.Success) {
				Debug.Log ("Enter keep success");
				if(travelManager) {
					Globals_Client.inKeep=Convert.ToBoolean( m.Message);
					travelManager.UpdateEnterExitText();
				}
			}
			else {
				DisplayMessage (m);
			}
			break;
		case Actions.GetNPCList:{
			Debug.Log ("Got NPC List");
			ProtoGenericArray<ProtoCharacterOverview> characters = m as ProtoGenericArray<ProtoCharacterOverview>;
			if(characters!=null) {
				Debug.Log ("Showing character contents...");
				ListDisplay characterDisplay =FindObjectOfType<ListDisplay>();
				if(m.Message.Equals ("Grant")) {
					characterDisplay.initializeContentsCharacters(m.MessageFields[0],characters);
				}
				else {
					characterDisplay.initializeContentsCharacters(m.Message,characters);
				}

			}
		}
			break;
		case Actions.ListCharsInMeetingPlace: {
			Debug.Log ("Got meeting place list");
			// If action failed display message
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
				return;
			}
			ProtoGenericArray<ProtoCharacterOverview> characters = m as ProtoGenericArray<ProtoCharacterOverview>;
			if(characters!=null) {
				travelManager.ListState();
				ListDisplay characterDisplay =FindObjectOfType<ListDisplay>();
				characterDisplay.initializeContentsCharacters(m.Message,characters);
			}
			else {
				Debug.LogError ("Could not convert message to type ProtoGenericArray<ProtoCharacterOverview>");
				return;
			}
		}
			break;
		case Actions.TravelTo:
		{
			Debug.Log ("Got travel to");
			HexMapManager mapManager = FindObjectOfType<HexMapManager>();
			ProtoFief fiefDetails = m as ProtoFief;
			if(fiefDetails==null) {
				Debug.LogError ("Error converting message to type ProtoFief");
				return;
			}
			// TODO remove or re-add
			/*
			Globals_Client.days -= Globals_Client.currentLocation.getTravelCost (Globals_Game.fiefMasterList[fiefDetails.fiefID]);
			Globals_Client.currentLocation=Globals_Game.fiefMasterList[fiefDetails.fiefID];
			*/
			mapManager.updateHexMap();
		}
			break;
		case Actions.Camp:
			Debug.Log ("Got camp- response type: " +m.ResponseType.ToString ());
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			break;
		case Actions.ViewFief: {
			Debug.Log ("Got View Fief");
			var itemDetails = FindObjectOfType<FiefController>();
			ProtoFief fief = m as ProtoFief;
			if(fief==null) {
				Debug.LogError ("Error converting incoming message to type ProtoFief");
			}
			else  {
				itemDetails.DisplayFief (fief);
			}
		}
			break;
		case Actions.BarNationalities: {
			if(m.ResponseType==DisplayMessages.FiefCouldNotBar) {
				string message = "Apologies my lord, but the following could not be barred: ";
				foreach(string natID in m.MessageFields) {
					message+=Globals_Game.nationalityMasterList[natID].name;
				}
				DisplayMessage(message);
			}
			ProtoFief fief = m as ProtoFief;
			if(fief==null) {
				Debug.LogError ("Error converting incoming message to type ProtoFief");
				return;
			}
			FiefController fiefController = FindObjectOfType<FiefController>();
			fiefController.currentFiefDetails = fief;
			fiefController.ShowBarredItems();
		}
			break;
		case Actions.UnbarNationalities: {
			if(m.ResponseType==DisplayMessages.FiefCouldNotUnbar) {
				string message = "Apologies my lord, but the following could not be barred: ";
				foreach(string natID in m.MessageFields) {
					message+=Globals_Game.nationalityMasterList[natID].name;
				}
				DisplayMessage(message);
			}
			ProtoFief fief = m as ProtoFief;
			if(fief==null) {
				Debug.LogError ("Error converting incoming message to type ProtoFief");
				return;
			}
			FiefController fiefController = FindObjectOfType<FiefController>();
			fiefController.currentFiefDetails = fief;
			fiefController.ShowBarredItems();
		}
			break;
		case Actions.BarCharacters: {
			if(m.ResponseType==DisplayMessages.FiefCouldNotBar) {
				string message = "Apologies my lord, but the following could not be barred: ";
				foreach(string charID in m.MessageFields) {
					message+=charID;
				}
				DisplayMessage(message);
			}
			ProtoFief fief = m as ProtoFief;
			if(fief==null) {
				Debug.LogError ("Error converting incoming message to type ProtoFief");
				return;
			}
			FiefController fiefController = FindObjectOfType<FiefController>();
			fiefController.currentFiefDetails = fief;
			fiefController.ShowBarredItems();
		}
			break;
		case Actions.UnbarCharacters: {
			if(m.ResponseType==DisplayMessages.FiefCouldNotBar) {
				string message = "Apologies my lord, but the following could not be unbarred: ";
				foreach(string charID in m.MessageFields) {
					message+=charID;
				}
				DisplayMessage(message);
			}
			ProtoFief fief = m as ProtoFief;
			if(fief==null) {
				Debug.LogError ("Error converting incoming message to type ProtoFief");
				return;
			}
			FiefController fiefController = FindObjectOfType<FiefController>();
			fiefController.currentFiefDetails = fief;
			fiefController.ShowBarredItems();
		}
			break;
		case Actions.ViewMyFiefs: {
			Debug.Log ("Got view all fiefs!");
			ProtoGenericArray<ProtoFief> fiefs = m as ProtoGenericArray<ProtoFief>;
			if(fiefs ==null) {
				Debug.LogError ("Error converting incoming message to array of ProtoFief");
			}
			else {
				FiefController fiefManager = FindObjectOfType<FiefController>();
				if(fiefManager==null) {
					Debug.LogError ("FiefManager not initialised");
				}
				else {
					fiefManager.displayFiefs(fiefs.fields);
				}
			}
		}
			break;
		case Actions.ExamineArmiesInFief:
		{
			Debug.Log ("Got armies in fief");
			ProtoGenericArray<ProtoArmyOverview> armies = m as ProtoGenericArray<ProtoArmyOverview>;
			if(armies==null) {
				Debug.Log ("Error converting message to type ProtoArmyOverview");
				DisplayMessage(m);
				return;
			}
			else {
				if(travelManager) {
					travelManager.ListState ();
				}
				ListDisplay armyDisplay =FindObjectOfType<ListDisplay>();
				armyDisplay.initializeContentsArmies(armies);
			}
		}
			break;
		case Actions.ViewArmy:
		{
			Debug.Log ("Got view army");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
				return;
			}
			ProtoArmy army = m as ProtoArmy;
			if(army ==null) {
				Debug.Log ("Error converting message to type ProtoArmy");
				return;
			}
			else {
				// Get list display object
				ListDisplay armyDisplay =FindObjectOfType<ListDisplay>();
				armyDisplay.selectedItemID=army.armyID;
				armyDisplay.DisplayDetails(army);
				if(Application.loadedLevelName.Equals ("Combat")) {
					ArmyManager manager = FindObjectOfType<ArmyManager>();
					manager.army=army;
					manager.SetUpSliders ();
					if(!manager.InDropOffState) {
						manager.ShowArmyList();
					}

				}
			}
		}
			break;
		case Actions.ViewChar: 
		{
			Debug.Log ("Got view character");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
				return;
			}
			ProtoCharacter character = m as ProtoCharacter;
			if(character==null) {
				Debug.Log("Error converting message to type ProtoCharacter");
				return;
			}
			else {
				ListDisplay charDisplay =FindObjectOfType<ListDisplay>();
				charDisplay.selectedItemID = character.charID;
				// Don't display family controls for fief and combat levels
				if(Application.loadedLevelName.Equals ("Fief")||Application.loadedLevelName.Equals ("Combat")) {
					charDisplay.DisplayDetails(character,false);
				}
				else {
					charDisplay.DisplayDetails(character);
				}

			}
		}
			break;
		case Actions.AddRemoveEntourage:
		{
			Debug.Log ("Got add/remove entourage");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				// Get list if in a list view
				ListDisplay charDisplay =FindObjectOfType<ListDisplay>();
				// 
				if(charDisplay!=null&&charDisplay.state.Contains ("entourage")) {
					ProtoGenericArray<ProtoCharacterOverview> entouragelist = m as ProtoGenericArray<ProtoCharacterOverview>;
					if(entouragelist==null) {
						Debug.LogError("Could not convert message to entourage list");
						return;
					}
					charDisplay.initializeContentsCharacters("entourage",entouragelist);
				}
			}
		}
			break;
		case Actions.TryForChild:
		{
			DisplayMessage (m);
			if(m.ResponseType==DisplayMessages.Success) {
				// refresh character container;
			}
		}
			break;
		case Actions.AppointHeir:
		{
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				// Refresh list display item
			}
		}
			break;
		case Actions.ProposeMarriage:
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				DisplayMessage ("Your proposal of marriage has been sent, my lord!","Close");
			}
			// Should receive journal entry detailing proposal success
			break;
		case Actions.ViewJournalEntries:
			if(m.ResponseType!=DisplayMessages.JournalEntries) {
				DisplayMessage (m);
			}
			else {
				Debug.Log ("Got Journal Entries");
				ProtoGenericArray<ProtoJournalEntry> entries = m as ProtoGenericArray<ProtoJournalEntry>;
				if(entries ==null) {
					Debug.Log ("Error converting message to type ProtoArmy");
					return;
				}
				ListDisplay entryDisplay =FindObjectOfType<ListDisplay>();
				entryDisplay.initializeContentsJournal(entries);
			}
			break;
		case Actions.ViewJournalEntry:
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				ProtoJournalEntry entry = m as ProtoJournalEntry;
				if(entry==null) {
					// error
				}
				else {
					Debug.Log ("Got Journal Entry");
					ListDisplay entryDisplay =FindObjectOfType<ListDisplay>();
					entryDisplay.selectedItemID = entry.jEntryID.ToString ();
					entryDisplay.DisplayDetails(entry);
				}
			}
			break;
		case Actions.ListArmies: 
		{
			ProtoGenericArray<ProtoArmyOverview> armies = m as ProtoGenericArray<ProtoArmyOverview>;
			if(armies==null) {
				// error
			}
			else {
				ListDisplay entryDisplay =FindObjectOfType<ListDisplay>();
				if(entryDisplay!=null) {
					
					entryDisplay.initializeContentsArmies(armies);
				}
			}
		}
			break;

		case Actions.RecruitTroops: 
		{
			// Try to convert to ProtoRecruit
			ProtoRecruit recruitDetails = m as ProtoRecruit;
			if(recruitDetails==null) {
				DisplayMessage(m);
				return;
			}
			// If is ProtoRecruit check success
			else {
				if(recruitDetails.ResponseType==DisplayMessages.Success) {
					//do something
					DisplayMessage ("Recruited "+recruitDetails.MessageFields[0] + " troops for a total of £"+recruitDetails.MessageFields[1]);
				}
				else {
					ArmyManager.revisedRecruit=recruitDetails;
					DisplayMessage (recruitDetails,"Accept","Decline",ArmyManager.OnRecruitConfirm);
				}
			}
		}
			break;
		case Actions.MaintainArmy:
		{
			Debug.Log ("Got maintain army");
			// Display maintain message
			DisplayMessage(m);
		}
			break;
		
		case Actions.AppointLeader:
		{
			Debug.Log ("Got appoint leader");
			ProtoArmy army = m as ProtoArmy;
			if(army==null) {
				DisplayMessage(m);
			}
			else {
				Debug.Log ("success");
				// update army container
				ListDisplay list = GameObject.FindObjectOfType<ListDisplay>();
				list.DisplayDetails(army);
				ArmyManager manager = GameObject.FindObjectOfType<ArmyManager>();
				if(manager!=null) {
					manager.army = army;
				}
			}
		}
			break;
		case Actions.DisbandArmy:
		{
			Debug.Log ("Got disband army");
			if(m.ResponseType==DisplayMessages.Success) {
				// Update list
				Debug.Log ("success");
			}
			else {
				DisplayMessage(m);
			}
		}
			break;
		case Actions.DropOffTroops: 
		{
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				DisplayMessage ("Drop off successful");
				// Convert message to ProtoArmy and update display
				ProtoArmy army = m as ProtoArmy;
				if(army==null) {
					Debug.LogError("Error converting to ProtoArmy in DropOffTroops");
					return;
				}
				else {
					ListDisplay armyDisplay =FindObjectOfType<ListDisplay>();
					armyDisplay.selectedItemID=army.armyID;
					armyDisplay.DisplayDetails(army);
					ArmyManager armyManager = FindObjectOfType<ArmyManager>();
					armyManager.ShowArmyList();
					armyManager.army = army;
				}
			}
		}
			break;
		case Actions.ListDetachments:
		{
			Debug.Log ("Got list detachments");
			if(m.ResponseType!=DisplayMessages.Success) {
			   	DisplayMessage (m);
			}
			else {
				ProtoGenericArray<ProtoDetachment> detachments = m as ProtoGenericArray<ProtoDetachment>;
				if(detachments!=null) {
					ArmyManager armyManager = FindObjectOfType<ArmyManager>();
					if(armyManager==null) {
						Debug.LogError("Could not find ArmyManager");
					}
					else {
						if(detachments.fields==null) {
							DisplayMessage("There are no detachments in this fief.");
						}
						else {
							armyManager.showDetachments(detachments.fields);
						}
					}
				}
				else {
					Debug.LogError("Could not convert to array of ProtoDetachment at ListDetachments");
				}
			}
		}
			break;
		case Actions.PickUpTroops: 
		{
			Debug.Log ("Got pick up troops");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				ProtoArmy army = m as ProtoArmy;
				if(army==null) {
					Debug.LogError("Error converting to ProtoArmy in DropOffTroops");
					return;
				}
				else {
					ListDisplay armyDisplay =FindObjectOfType<ListDisplay>();
					armyDisplay.selectedItemID=army.armyID;
					armyDisplay.DisplayDetails(army);
					ArmyManager armyManager = FindObjectOfType<ArmyManager>();
					armyManager.ShowArmyList();
					armyManager.army = army;
				}
			}
		}
			break;
		case Actions.AdjustCombatValues:
		{
			Debug.Log ("Got adjust combat values");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ProtoCombatValues newValues = m as ProtoCombatValues;
				if(newValues==null) {
					Debug.LogError ("Error converting to ProtoCombatValues in AdjustCombatValues");
					return;
				}
				ArmyManager armyManager = FindObjectOfType<ArmyManager>();
				armyManager.army.aggression= newValues.aggression;
				armyManager.army.combatOdds=newValues.odds;
				armyManager.SetUpSliders();
			}
		}
			break;
		case Actions.Attack: 
		{
			Debug.Log ("Got attack");
			// If failed to attack
			if(m.ResponseType!=DisplayMessages.BattleResults) {
				DisplayMessage(m);
			}
			else {
				// Convert to ProtoBattle
				ProtoBattle battleResults = m as ProtoBattle;
				if(battleResults==null) {
					Debug.LogError ("Error converting to ProtoBattle in Attack");
				}
				else {
					// Display battle result
					String battleResult = ArmyManager.DisplayBattle(battleResults);
					DisplayMessage(battleResult);
				}
			}

		}
			break;
		case Actions.PillageFief: 
		{
			Debug.Log ("Got PillageFief");
			if(m.ResponseType==DisplayMessages.Success) {
				ProtoPillageResult result = m as ProtoPillageResult;
				if(result!=null) {
					string message = "You have successfully pillaged this fief.\n\nYou gained £"+result.moneyPillagedOwner;
					DisplayMessage (message,"Close");
				}
				else {
					Debug.LogError ("Error converting to ProtoPillageResult in PillageFief");
				}
			}
			else {
				DisplayMessage (m);
			}

		}
			break;
		case Actions.BesiegeFief: {
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
			else {
				ArmyManager armyManager = FindObjectOfType<ArmyManager>();
				if(armyManager!=null) {
					ProtoSiegeDisplay display = m as ProtoSiegeDisplay;
					if(display!=null) {
						ArmyManager.currentSiege=display;
						armyManager.DisplaySiege();
					}
					else {
						Debug.LogError("Error converting to ProtoSiegeDisplay");
					}
				}
				else {
					Debug.LogError ("Error- ArmyManager not initialised for BesiegeFief");
				}
			}
		}
			break;
		case Actions.SiegeRoundStorm: 
		case Actions.SiegeRoundReduction:
		case Actions.SiegeRoundNegotiate:
		{
			{
				ProtoSiegeDisplay siegeResult = m as ProtoSiegeDisplay;
				if(siegeResult==null) {
					DisplayMessage (m);
				}
				else {
					ArmyManager armyManager = FindObjectOfType<ArmyManager>();
					if(armyManager==null) {
						Debug.LogError ("ArmyManager not initialised for SiegeRound");
					}
					else {
						ArmyManager.currentSiege=siegeResult;
						armyManager.DisplaySiege();
						// If siege has ended, remove from list
						if(!string.IsNullOrEmpty(siegeResult.endDate)) {
							armyManager.SiegeHasEnded (siegeResult.siegeID);
						}
					}
				}
			}
			break;
		}
			break;
		case Actions.EndSiege: 
		{
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ArmyManager armyManager = FindObjectOfType<ArmyManager>();
				if(armyManager==null) {
					Debug.LogError ("ArmyManager not initialised for SiegeRound");
				}
				else {
					DisplayMessage ("You have chosen to end the siege.");
					armyManager.SiegeHasEnded (m.Message);
				}
			}
		}
			break;
		case Actions.SiegeList: {
			ProtoGenericArray<ProtoSiegeOverview> siegelist = m as ProtoGenericArray<ProtoSiegeOverview>;
			if(siegelist!=null&&siegelist.fields!=null) {
				ArmyManager armyManager = FindObjectOfType<ArmyManager>();
				if(armyManager!=null) {
					armyManager.ListSieges(siegelist.fields);
				}
				else {
					Debug.LogError ("Error- ArmyManager not initialised for SiegeList");
				}
			}
			else {
				Debug.Log ("No sieges to display");
			}
		}
			break;
		case Actions.ViewSiege: {
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ArmyManager armymanager = FindObjectOfType<ArmyManager>();
				if(armymanager!=null) {
					ProtoSiegeDisplay siege = m as ProtoSiegeDisplay;
					if(siege==null) {
						Debug.LogError ("Error converting message to ProtoSiegeDisplay in ViewSiege");
					}
					else {
						ArmyManager.currentSiege=siege;
						armymanager.DisplaySiege();
					}
				}
				else {
					Debug.LogError ("Error- ArmyManager not initialised for ViewSiege");
				}
			}
		}
			break;
		case Actions.SpyFief:
		{
			if(m.ResponseType==DisplayMessages.Success) {
				// Convert to ProtoFief
				ProtoFief fiefDetails = m as ProtoFief;
				if(fiefDetails==null) {
					Debug.LogError ("Error converting message to ProtoFief in SpyFief");
					return;
				}
				if(Application.loadedLevelName.Equals ("Fief")) {
					FiefController fiefController = FindObjectOfType<FiefController>();
					fiefController.DisplayFief (fiefDetails);
				}
			}
			// if there was an error, display error
			else if(m.ResponseType!=DisplayMessages.None)  {
				DisplayMessage(m);
			}
			// If unsuccessful, the update will show all the details necessary
		}
			break;
		case Actions.SpyArmy: 
		{
			if(m.ResponseType==DisplayMessages.Success) {
				ProtoArmy armyDetails = m as ProtoArmy;
				if(armyDetails==null) {
					Debug.LogError ("Error converting message to ProtoArmy in SpyArmy");
					return;
				}
				ListDisplay display = FindObjectOfType<ListDisplay>();
				if(display!=null) {
					display.DisplayDetails(armyDetails);
				}
			}
			// if there was an error, display error
			else if(m.ResponseType!=DisplayMessages.None)  {
				DisplayMessage(m);
			}
		}
			break;
		case Actions.SpyCharacter: 
		{
			if(m.ResponseType==DisplayMessages.Success) {
				ProtoCharacter characterDetails= m as ProtoCharacter;
				if(characterDetails==null) {
					return;
				}
				else {
					ListDisplay display = FindObjectOfType<ListDisplay>();
					if(display!=null) {
						display.DisplayDetails(characterDetails);
					}
				}

			}
			// if there was an error, display error
			else if(m.ResponseType!=DisplayMessages.None)  {
				DisplayMessage(m);
			}
		}
			break;
		case Actions.Kidnap: {
			Debug.Log ("Got kidnap!");
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.RansomCaptive: {
			if(m.ResponseType==DisplayMessages.Success) {
				DisplayMessage ("You sent a ransom demand for £"+m.Message);
			}
			else {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.ExecuteCaptive: {
			if(m.ResponseType==DisplayMessages.Success) {
				DisplayMessage ("This character has been executed");
			}
			else {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.ReleaseCaptive: {
			if(m.ResponseType==DisplayMessages.Success) {
				DisplayMessage ("You have released this captive");
			}
			else {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.RespondRansom: {
			if(m.ResponseType==DisplayMessages.Success) {
				DisplayMessage ("You successfully sent your response to the ransom demands");
			}
			else {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.ViewCaptives: {
			Debug.Log("Got view captives");
			if(m.ResponseType==DisplayMessages.None) {
				DisplayMessage ("There are no captives to display");
			}
			else if(m.ResponseType==DisplayMessages.Success) {
				ProtoGenericArray<ProtoCharacterOverview> captives = m as ProtoGenericArray<ProtoCharacterOverview>;
				if(captives==null) {
					Debug.LogError ("Error converting message to ProtoGenericArray<ProtoCharacterOverview> in ViewCaptives");
				}
				else {
					if(Application.loadedLevelName.Equals ("Fief")) {
						FiefController controller = FindObjectOfType<FiefController>();
						controller.ShowCaptives(captives.fields);
					}
				}
			}
			else {
				DisplayMessage (m);
			}
		}
			break;
		case Actions.ViewCaptive:{
			if(m.ResponseType!=DisplayMessages.Success) {
				DisplayMessage(m);
			}
			else {
				ProtoCharacter character = m as ProtoCharacter;
				if(character!=null) {
					Debug.Log ("character not null");
					if (Application.loadedLevelName.Equals ("Fief")) {
						Debug.Log ("is  fief");
						FiefController controller = FindObjectOfType<FiefController>();
						controller.ShowCaptive(character);
					}
				}
				else {
					Debug.LogError("error converting to ProtoCharacter in ViewCaptive");
				}
			}
		}
			break;
		default: Debug.LogError ("Unrecognised message"); break;
		}
	}
	public void PerformUpdate(ProtoClient clientDetails) {
		Globals_Client.TravelModifier=clientDetails.travelModifier;
		Globals_Client.activeCharacter=clientDetails.activeChar;
		Globals_Client.playerCharacter=clientDetails.playerChar;
		Globals_Client.currentLocation=Globals_Game.fiefMasterList[ clientDetails.activeChar.location];
		Globals_Client.days= clientDetails.activeChar.days;
		Globals_Client.inKeep=clientDetails.activeChar.inKeep;
		Globals_Client.purse = clientDetails.purse;
		Globals_Client.goTo = clientDetails.activeChar.goTo;
		Globals_Client.homeTreasury=clientDetails.homeFiefTreasury;
		Globals_Client.activeChar=clientDetails.activeChar.charID;
		Globals_Client.activeCharName=clientDetails.activeChar.firstName + " " +clientDetails.activeChar.familyName;
		Globals_Client.pcID=clientDetails.playerChar.charID;
		HexMapManager mapManager = FindObjectOfType<HexMapManager>();
		if(mapManager!=null) {
			mapManager.updateHexMap ();
		}
		if(travelManager) {
			travelManager.UpdateEnterExitText ();
		}
	}

	// Displays a message in a pop-out dialog
	public void DisplayMessage(ProtoMessage m,string confirm = null, string deny = null, PopUpDialog.clicked clickEvent = null) {
		string message=null;
		if(m.ResponseType==DisplayMessages.None) {
			Debug.LogError ("response is null");
			return;
		}
		if(Globals_Game.displayMessages==null) {
			Debug.LogError("Display messages is null");
			return;
		}
		Globals_Game.displayMessages.TryGetValue (m.ResponseType,out message);
		if(message==null) {
			message=m.Message;
			if(m.MessageFields!=null&&m.Message!=null) {
				Debug.Log (m.MessageFields[0]);
				message = string.Format (message,m.MessageFields);
				message=message.Replace ("\\n",Environment.NewLine);
				Debug.Log ("Formatted message");
			}
		}
		else {
			if(m.MessageFields!=null) {
				message = string.Format (message,m.MessageFields);
				message=message.Replace ("\\n",Environment.NewLine);
			}
		}
		var canvas = GameObject.Find ("Canvas");
		if(canvas==null) {
			Debug.LogError ("Could not identify Canvas for message display.");
			return;
		}
		DisplayMessage (message,confirm,deny,clickEvent);

		Debug.Log ("Finished display message code");
	}

	/// <summary>
	/// Displays a message in a popout dialog, with options change button text, add a cancel button and a callback
	/// </summary>
	/// <param name="message">The string to display</param>
	/// <param name="confirm">The text to show on the confirm button.</param>
	/// <param name="deny">Add a deny/cancel button with the corresponding string as text</param>
	/// <param name="clickEvent">Callback function (called on clicking a button. Must take in a bool and return void</param>
	public void DisplayMessage(string message,string confirm = null, string deny = null, PopUpDialog.clicked clickEvent = null) {
		Debug.Log ("In display message");
		GameObject dialog = (GameObject)Resources.Load ("PopUp");
		GameObject panel = (GameObject)Instantiate(dialog,new Vector2(0,0),Quaternion.identity);
		PopUpDialog popup = FindObjectOfType<PopUpDialog>();
		popup.ShowPopUpDialog (message,confirm,deny,clickEvent);
	}

	public static void MainMenuClick(String option) {
		if(option.Equals ("Travel")){
		   Application.LoadLevel ("Travel");
		}
		else if(option.Contains ("Household")) {
			gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("FamilyEmploy"));
			gameState.preLoadState="FamilyEmploy";
			// load household
			if(option.Contains ("Family")) {
				// restrict to family
				gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Family"));
				gameState.preLoadState = "Family";
			}
			else if (option.Contains ("Employ")) {
				gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Employ"));
				gameState.preLoadState = "Employ";
			}
			else if(option.Contains ("Entourage")) {
				gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Entourage"));
				gameState.preLoadState = "Entourage";
			}
			Application.LoadLevel ("Household");
		}
		else if(option.Contains ("Journal")) {
			gameState.preLoadState="all";
			if(option.Contains ("year")) {
				gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("year"));
				gameState.preLoadState = "year";
			}
			else if (option.Contains ("season")) {
				gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("season"));
				gameState.preLoadState = "season";
			}
			else if (option.Contains ("unread")) {
				gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("unread"));
				gameState.preLoadState = "unread";
			}
			else {
				gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("all"));
			}
			Application.LoadLevel ("Journal");
		}
		else if(option.Contains ("Fief")) {
			gameState.preLoadState = "home";
			if(option.Contains ("current")) {
				gameState.preLoadState=Globals_Client.currentLocation.id;
			}
			if(option.Contains("all")) {
				gameState.preLoadState="all";
			}
			Application.LoadLevel ("Fief");
		}
		else if(option.Contains("Combat")) {
			// Choose armies or sieges-defaults to armies
			Application.LoadLevel ("Combat");
		}
	}

	public void testRansom() {
		ProtoMessage ransom = new ProtoMessage();
		ransom.ActionType=Actions.RansomCaptive;
		ransom.Message="Char_626";
		NetworkScript.Send (ransom);
	}
}
