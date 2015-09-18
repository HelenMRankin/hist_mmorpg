using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using hist_mmorpg;
using UnityEngine.UI;
using System;
public class ArmyManager : MonoBehaviour {
	// Selected army
	public ProtoArmy army;
	public GameObject list;
	public GameObject controls;
	public GameObject itemDetails;
	public GameObject scrollPanel;
	public GameObject dropOff;
	public GameObject appointLeaderBtn;
	public GameObject DetachmentList;
	public GameObject SiegeDisplay;
	private Dictionary<string, ProtoDetachment> detachments  = new Dictionary<string, ProtoDetachment>();
	// Holds revised recruitment details from server in case not enough money/troops
	public static ProtoRecruit revisedRecruit;
	public static ProtoSiegeDisplay currentSiege;
	public bool InDropOffState = false;
	// Load the scene
	void Awake() {
		while(GameStateManager.gameState.SceneLoadQueue.Count>0) {
			GameStateManager.gameState.SceneLoadQueue.Dequeue().Invoke ();
		}
		ProtoMessage requestArmies = new ProtoMessage();
		requestArmies.ActionType=Actions.ListArmies;
		NetworkScript.Send (requestArmies);
		controls.SetActive(true);
		appointLeaderBtn.SetActive (false);
		ShowArmyList();

	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	

	public static void DropOffForCharacter(string charID) {
		ArmyManager instance = FindObjectOfType<ArmyManager>();
		instance.InDropOffState=true;
		instance.ShowDropOff();
		instance.dropOff.transform.Find ("LeftFor").GetComponent<InputField>().text=charID;
	}

	public void SetUpSliders() {
		if(army!=null) {
			Slider aggro = controls.transform.Find ("Aggression").GetComponent<Slider>();
			Slider combat = controls.transform.Find ("CombatOdds").GetComponent<Slider>();
			aggro.value=(float)army.aggression;
			combat.value=(float)army.combatOdds;
		}
	}
	public void recruitTroops(bool isConfirm = false) {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		ProtoRecruit recruitDetails = new ProtoRecruit();
		recruitDetails.armyID=army.armyID;
		var inputObject = GameObject.Find ("TroopCount");

		InputField input = inputObject.GetComponent<InputField>();
		try {
			uint numTroops = Convert.ToUInt32(input.textComponent.text);
			recruitDetails.amount = numTroops;
			recruitDetails.isConfirm=isConfirm;
			recruitDetails.ActionType=Actions.RecruitTroops;
			NetworkScript.Send (recruitDetails);
		}
		catch(Exception e) {
			GameStateManager.gameState.DisplayMessage ("Please enter a valid number");
		}		 
	}

	public void maintainArmy() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		else {
			GameStateManager.gameState.DisplayMessage("It will cost £"+army.maintCost+" to maintain this army. \n Do you wish to proceed?","Yes","No", OnMaintainConfirm);

		}
	}

	public void OnMaintainConfirm(bool accept) {
		if(accept) {
			ProtoMessage maintain = new ProtoMessage();
			maintain.ActionType=Actions.MaintainArmy;
			maintain.Message=army.armyID;
			NetworkScript.Send (maintain);
		}
		else {
		}
	}

	public void Camp() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		if(string.IsNullOrEmpty (army.leader)) {
			GameStateManager.gameState.DisplayMessage("An army cannot camp without a leader");
			return;
		}
		else {
			ProtoMessage camp = new ProtoMessage();
			camp.Message=army.leaderID;
			var inputObject= GameObject.Find ("CampDays");
			InputField input = inputObject.GetComponent<InputField>();
			try {
				int campDays = Convert.ToInt32(input.textComponent.text);
				camp.MessageFields=new string[]{campDays.ToString ()};
				camp.ActionType= Actions.Camp;
				NetworkScript.Send (camp);
			}
			catch(Exception e) {
				GameStateManager.gameState.DisplayMessage ("Please enter a valid number");
			}
		}
	}

	public void getPotentialLeaders() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		list.SetActive (true);
		//itemDetails.SetActive (false);
		appointLeaderBtn.SetActive(true);
		controls.SetActive (false);
		ProtoMessage getLeaders = new ProtoMessage();
		getLeaders.ActionType=Actions.GetNPCList;
		getLeaders.Message= "Grant";
		getLeaders.MessageFields=new string[]{"leader",army.armyID};
		NetworkScript.Send (getLeaders);
	}

	public void appointLeader() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		string id = list.GetComponent<ListDisplay>().selectedItemID;
		ProtoMessage appoint = new ProtoMessage();
		appoint.Message = army.armyID;
		appoint.MessageFields= new string[] {id};
		appoint.ActionType=Actions.AppointLeader;
		appointLeaderBtn.SetActive (false);
		controls.SetActive (true);
		NetworkScript.Send (appoint);
	}

	public void disbandArmy() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		ProtoMessage disband = new ProtoMessage();
		disband.ActionType= Actions.DisbandArmy;
		disband.Message=army.armyID;
		NetworkScript.Send (disband);
	}

	public void ShowDropOff() {
		Debug.Log ("Show drop off");
		scrollPanel.SetActive (false);
		dropOff.SetActive (true);
		DetachmentList.SetActive (false);
		SiegeDisplay.SetActive (false);
	}

	public void ShowArmyList() {
		Debug.Log ("Show armies");
		scrollPanel.SetActive (true);
		dropOff.SetActive (false);
		DetachmentList.SetActive(false);
		list.SetActive (true);
		if(army!=null) {
			controls.SetActive (true);
		}
		else {
			controls.SetActive(false);
		}
		SiegeDisplay.SetActive (false);
	}

	public void ShowDetachmentList() {
		Debug.Log ("Show detachments");
		scrollPanel.SetActive (false);
		dropOff.SetActive (false);
		DetachmentList.SetActive (true);
		SiegeDisplay.SetActive (false);
	}

	public void ShowSieges() {
		Debug.Log ("Show sieges");
		scrollPanel.SetActive (false);
		dropOff.SetActive (false);
		DetachmentList.SetActive (false);
		SiegeDisplay.SetActive (true);
		list.SetActive (false);
		controls.SetActive (false);
	}
	public void DropOffTroops() {
		var leaveForObject = GameObject.Find ("LeftFor");
		var knightsObject = GameObject.Find ("Knights");
		var MAAObject = GameObject.Find ("MAA");
		var lCavObject = GameObject.Find ("LCav");
		var lBowObject = GameObject.Find ("LBow");
		var sBowObject = GameObject.Find ("CBow");
		var footObject = GameObject.Find ("Foot");
		var rabbleObject = GameObject.Find ("Rab");

		try {
			string leaveFor = leaveForObject.GetComponent<InputField>().textComponent.text;
			UInt32 knights;
			if(string.IsNullOrEmpty(knightsObject.GetComponent<InputField>().textComponent.text)) {
				knights=0;
			}
			else {
				knights = Convert.ToUInt32(knightsObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 MAA;
			if(string.IsNullOrEmpty(MAAObject.GetComponent<InputField>().textComponent.text)) {
				MAA=0;
			}
			else {
				MAA = Convert.ToUInt32(MAAObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 lCav;
			if(string.IsNullOrEmpty(lCavObject.GetComponent<InputField>().textComponent.text)) {
				lCav=0;
			}
			else {
				lCav = Convert.ToUInt32(lCavObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 lBow;
			if(string.IsNullOrEmpty(lBowObject.GetComponent<InputField>().textComponent.text)) {
				lBow=0;
			}
			else {
				lBow = Convert.ToUInt32(lBowObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 sBow;
			if(string.IsNullOrEmpty(sBowObject.GetComponent<InputField>().textComponent.text)) {
				sBow=0;
			}
			else {
				sBow = Convert.ToUInt32(sBowObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 foot;
			if(string.IsNullOrEmpty(footObject.GetComponent<InputField>().textComponent.text)) {
				foot=0;
			}
			else {
				foot = Convert.ToUInt32(footObject.GetComponent<InputField>().textComponent.text);
			}
			UInt32 rabble;
			if(string.IsNullOrEmpty(rabbleObject.GetComponent<InputField>().textComponent.text)) {
				rabble=0;
			}
			else {
				rabble = Convert.ToUInt32(rabbleObject.GetComponent<InputField>().textComponent.text);
			}
			ProtoDetachment detachment = new ProtoDetachment();
			detachment.armyID=army.armyID;
			detachment.leftFor=leaveFor;
			detachment.troops = new uint[] {knights,MAA,lCav,lBow,sBow,foot,rabble};
			detachment.ActionType=Actions.DropOffTroops;
			NetworkScript.Send (detachment);
		}
		catch(Exception e) {
			Debug.LogError (e.Message);
			GameStateManager.gameState.DisplayMessage ("Please enter a valid number");
			return;
		}
	}

	public void ListTransfers() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage("You must select an army first");
			return;
		}
		ProtoMessage requestTransferList = new ProtoMessage();
		requestTransferList.Message=army.armyID;
		requestTransferList.ActionType=Actions.ListDetachments;
		NetworkScript.Send (requestTransferList);
	}

	public void showDetachments(ProtoDetachment[] detachmentList) {
		ShowDetachmentList();
		var row = Resources.Load<GameObject> ("DetachmentDetails");
		var detachmentDisplay = DetachmentList.transform.FindChild("ScrollPanel").FindChild("PanelContents").gameObject;
		foreach(ProtoDetachment detachment in detachmentList) {
			detachments.Add (detachment.id,detachment);
			GameObject detachmentView = Instantiate (row);
			detachmentView.name = detachment.id;
			detachmentView.transform.Find ("Knights").GetComponent<Text>().text = detachment.troops[0].ToString ();
			detachmentView.transform.Find ("MAA").GetComponent<Text>().text = detachment.troops[1].ToString ();
			detachmentView.transform.Find ("LCav").GetComponent<Text>().text = detachment.troops[2].ToString ();
			detachmentView.transform.Find ("LBow").GetComponent<Text>().text = detachment.troops[3].ToString ();
			detachmentView.transform.Find ("CBow").GetComponent<Text>().text = detachment.troops[4].ToString ();
			detachmentView.transform.Find ("Foot").GetComponent<Text>().text = detachment.troops[5].ToString ();
			detachmentView.transform.Find ("Rabble").GetComponent<Text>().text = detachment.troops[6].ToString ();
			detachmentView.transform.Find ("Left").GetComponent<Text>().text = detachment.days.ToString ();
			detachmentView.transform.SetParent (detachmentDisplay.transform);
		}
		Debug.Log ("Finished displaying pickups");
	}
	public void AddSelectedDetachments() {
		if(army==null) {
			GameStateManager.gameState.DisplayMessage ("You must select an army before adding detachments");
			return;
		}
		List<string> selectedDetachments = new List<string>();
		var detachmentContainer = DetachmentList.transform.FindChild("ScrollPanel").FindChild("PanelContents").gameObject;
		// Iterate over objects in detachment list
		foreach(Transform transform in detachmentContainer.transform) {
			Debug.Log ("checking item: "+transform.gameObject.name);
			// Check current item is a valid detachment and is selected
			var toggle = transform.Find ("Toggle");
			if(toggle!=null) {
				Debug.Log ("toggle found ");
				if(toggle.GetComponent<Toggle>().isOn) {
					Debug.Log ("Toggle on");
					selectedDetachments.Add (transform.gameObject.name);
				}
			}
		}
		if(selectedDetachments.Count==0) {
			Debug.Log ("No pickups selected");
			GameStateManager.gameState.DisplayMessage("No detachments selected");
		}
		else {
			ProtoMessage requestPickups = new ProtoMessage();
			requestPickups.ActionType=Actions.PickUpTroops;
			requestPickups.MessageFields=selectedDetachments.ToArray ();
			requestPickups.Message=army.armyID;
			NetworkScript.Send (requestPickups);
		}
	}
	public static void OnRecruitConfirm(bool isConfirm) {
		if(isConfirm) {
			revisedRecruit.isConfirm=true;
			NetworkScript.Send (revisedRecruit);
		}
	}

	public void AdjustOdds() {
		Slider aggro = controls.transform.Find ("Aggression").GetComponent<Slider>();
		Slider combat = controls.transform.Find ("CombatOdds").GetComponent<Slider>();
		byte newAgg = Convert.ToByte (aggro.value);
		byte newCombat = Convert.ToByte(combat.value);
		ProtoCombatValues newValues = new ProtoCombatValues();
		newValues.armyID=army.armyID;
		Debug.Log ("Adjusting values for army: "+army.armyID);
		newValues.aggression=newAgg;
		newValues.odds=newCombat;
		newValues.ActionType=Actions.AdjustCombatValues;
		NetworkScript.Send (newValues);
	}

	public void ListSieges(ProtoSiegeOverview[] sieges) {
		ShowSieges ();
		var siegeItem = Resources.Load<GameObject>("SiegeOverview");
		if(siegeItem==null) {
			Debug.LogError ("Could not find SiegeOverview");
			return;
		}
		foreach(ProtoSiegeOverview siege in sieges) {
			var row = Instantiate (siegeItem);
			row.name=siege.siegeID;
			row.transform.Find ("Fief").GetComponent <Text>().text = Globals_Game.fiefMasterList[siege.besiegedFief].name;
			row.transform.Find ("Besieger").GetComponent<Text>().text = siege.besiegingPlayer;
			row.transform.Find ("Defender").GetComponent<Text>().text = siege.defendingPlayer;
			row.GetComponent<Button>().onClick.AddListener (()=>GetSiege(row.name));
			row.transform.SetParent (SiegeDisplay.transform.Find ("SiegeList").Find ("PanelContents"));
		}
	}

	public void GetSiege(string siegeID) {
		ProtoMessage viewSiege = new ProtoMessage();
		viewSiege.ActionType=Actions.ViewSiege;
		viewSiege.Message=siegeID;
		NetworkScript.Send (viewSiege);
	}

	public static void Besiege(string armyID) {
		ProtoMessage besiege = new ProtoMessage();
		besiege.ActionType=Actions.BesiegeFief;
		besiege.Message=armyID;
		NetworkScript.Send (besiege);
	}
	public static void RequestSieges() {
		ProtoMessage siegelist = new ProtoMessage();
		siegelist.ActionType=Actions.SiegeList;
		NetworkScript.Send (siegelist);
	}
			                                            
	public void ShowSiegeControls(bool show) {
		var siegeControls = SiegeDisplay.transform.Find ("SiegeControls").gameObject;
		siegeControls.SetActive (show);
	}

	public void DisplaySiege() {
		ShowSieges ();
		ShowSiegeControls (currentSiege.besiegingPlayer == Globals_Client.pcID);
		Text siegeText = SiegeDisplay.transform.Find ("SiegeDetails").Find("PanelContents").GetComponent<Text>();
		Fief location = Globals_Game.fiefMasterList[currentSiege.besiegedFief];
		string text = "Fief: "+ location.name + " ("+location.province.name+")\n";
		text+="Fief Owner: "+currentSiege.defendingPlayer;
		text+="\nBesieging Player: "+currentSiege.besiegingPlayer;
		text+="\nDefending army: "+currentSiege.defenderGarrison;
		if(currentSiege.defenderAdditional!=null) {
			text+=" (aided by "+currentSiege.defenderAdditional+")";
		}
		text+="Start date: ";
		if(currentSiege.startSeason==0) {
			text+="Spring";
		}
		else if(currentSiege.startSeason==1) {
			text+="Summer";
		}
		else if(currentSiege.startSeason==2) {
			text+="Autumn";
		}
		else {
			text+="Winter";
		}
		text+= " "+currentSiege.startYear;
		text+="\nDays used so far: "+currentSiege.totalDays;
		text+="\nDays left in season: "+currentSiege.days;

		text+="\nBesieging forces: "+currentSiege.besiegerArmy;
		text+="\nKeep Level: "+currentSiege.keepLevel+"/"+currentSiege.startKeepLevel;
		text+="\nDefender troops killed: "+currentSiege.totalCasualtiesDefender;
		text+="\nAttacker troops killed: "+currentSiege.totalCasualtiesDefender;

		//TODO finish display
		if(!string.IsNullOrEmpty (currentSiege.endDate)) {
			text+="\n\nOn "+currentSiege.endDate;
			if(currentSiege.besiegerWon) {
				text+=currentSiege.besiegingPlayer +" won the siege, ";
				text+="taking "+currentSiege.lootLost +" as spoils.";
				if(currentSiege.captivesTaken!=null) {
					text+="\nAdditionally, several members of "+currentSiege.defendingPlayer+" were taken hostage:";
					foreach(string charName in currentSiege.captivesTaken) {
						text+=charName+ ", ";
					}
					text+="have all been captured.";
				}
			}
			else {
				text+=currentSiege.defendingPlayer+ " repelled the forces of "+currentSiege.besiegingPlayer+ " to end the siege of "+Globals_Game.fiefMasterList[currentSiege.besiegedFief];

			}
				text+="\nOver the course of "+currentSiege.totalDays+", "+currentSiege.defendingPlayer+" lost "+currentSiege.totalCasualtiesDefender +" troops";
				text+=", and "+currentSiege.besiegingPlayer+ " lost "+currentSiege.totalCasualtiesAttacker+ "troops.";
				text+="\nThe ownership of "+Globals_Game.fiefMasterList[currentSiege.besiegedFief]+" has changed hands to "+currentSiege.besiegingPlayer;
			
		}
		siegeText.text= text;

	}
	
	public void StormRound() {
		ProtoMessage storm = new ProtoMessage();
		storm.ActionType=Actions.SiegeRoundStorm;
		storm.Message=currentSiege.siegeID;
		NetworkScript.Send (storm);
	}

	public void NegotiationRound() {
		ProtoMessage negotiate = new ProtoMessage();
		negotiate.ActionType=Actions.SiegeRoundNegotiate;
		negotiate.Message=currentSiege.siegeID;
		NetworkScript.Send (negotiate);
	}

	public void ReductionRound() {
		ProtoMessage reduce = new ProtoMessage();
		reduce.ActionType=Actions.SiegeRoundReduction;
		reduce.Message=currentSiege.siegeID;
		NetworkScript.Send (reduce);
	}

	public void EndSiege() {
		ProtoMessage end = new ProtoMessage();
		end.ActionType=Actions.EndSiege;
		end.Message=currentSiege.siegeID;
		NetworkScript.Send (end);
	}

	public void SiegeHasEnded(string siegeID) {
		// Find relevant row in list
		var row = SiegeDisplay.transform.Find ("SiegeList").Find ("PanelContents").Find (siegeID);
		if(row!=null) {
			// Destroy row
			Destroy (row.gameObject);
		}
		// if is active siege, hide controls
		if(currentSiege.siegeID.Equals (siegeID)) {
			SiegeDisplay.transform.Find ("SiegeControls").gameObject.SetActive (false);
		}
	}

	public static string DisplayBattle(ProtoBattle battle) {
		string battleText;
		// Ordinary battle
		if(battle.circumstance==(byte)0) {
			battleText = "On this day of our lord the army, owned by "+battle.attackerOwner;
			if(!String.IsNullOrEmpty (battle.attackerLeader) ){
				battleText+=" and led by "+battle.attackerLeader;
			}
			if(!battle.battleTookPlace) {
				battleText+= ", tried and failed to bring "+battle.defenderOwner+"'s army";
				if(battle.defenderLeader!=null) {
					battleText+=", led by "+battle.defenderLeader+",";
				}
				battleText+=" to battle in "+Globals_Game.fiefMasterList[battle.battleLocation].name +".";
				battleText+="\n"+battle.defenderOwner+"'s army successfully refused battle and retreated from the fief";
				return battleText;
			}
			else {
				battleText+=", successfully brought "  +battle.defenderOwner+"'s army";
				if(battle.defenderLeader!=null) {
					battleText+=", led by "+battle.defenderLeader+",";
				}
				battleText+=" to battle in "+Globals_Game.fiefMasterList[battle.battleLocation].name +".";
				battleText+="\n\nRESULTS: ";
			}
		}
		// Pillage
		else if(battle.circumstance==(byte)1) {
			battleText="The fief garrison and militia of "+Globals_Game.fiefMasterList[battle.battleLocation].name+", led by "+battle.attackerLeader+", sallied forth to bring the pillaging army owned by "+battle.defenderOwner+" to battle.";
			battleText+="\n\nRESULTS: ";
		}
		// Siege
		else {
			battleText="The fief garrison and militia of "+Globals_Game.fiefMasterList[battle.battleLocation].name+", led by "+battle.attackerLeader+", sallied forth to bring the besieging army owned by "+battle.defenderOwner+" to battle.";
			battleText+="\n\nRESULTS: \n";
		}
		// If battle occurred, show results
		if(battle.battleTookPlace) {
			// Victory
			if(battle.attackerVictorious) {
				battleText+= battle.attackerOwner+"'s army was victorious.\n";
			}
			else {
				battleText+= battle.defenderOwner+"'s army was victorious.\n";
			}
			// Casualties
			// attacker
			battleText+=battle.attackerOwner+"'s army suffered "+battle.attackerCasualties + " casualties\n";
			battleText+=battle.defenderOwner+"'s army suffered "+battle.defenderCasualties+" casualties\n";
			
			// Disbands
			if(battle.disbandedArmies!=null && battle.disbandedArmies.Length>0)  {
				battleText+=battle.disbandedArmies[0];
				if(battle.disbandedArmies.Length>1) {
					for(int i  = 1;i<battle.disbandedArmies.Length-1;i++) {
						battleText+= ", "+battle.disbandedArmies[i];
					}
					battleText+= " and " +battle.disbandedArmies[battle.disbandedArmies.Length-1];
				}
				battleText+="'s forces disbanded due to heavy casualties.\n";
			}
			
			// Retreats
			if(battle.retreatedArmies!=null && battle.retreatedArmies.Length>0) {
				battleText+=battle.retreatedArmies[0];
				if(battle.retreatedArmies.Length>1) {
					for(int i  = 1;i<battle.retreatedArmies.Length-1;i++) {
						battleText+= ", "+battle.retreatedArmies[i];
					}
					battleText+= " and " +battle.retreatedArmies[battle.retreatedArmies.Length-1];
				}
				battleText+="'s forces retreated from the fief.\n";
			}
			
			// Deaths
			if(battle.deaths!=null && battle.deaths.Length>0) {
				battleText+=battle.deaths[0];
				if(battle.deaths.Length>1) {
					for(int i  = 1;i<battle.deaths.Length-1;i++) {
						battleText+= ", "+battle.deaths[i];
					}
					battleText+= " and " +battle.deaths[battle.deaths.Length-1];
				}
				battleText+=" died due to injuries received during battle.\n";
			}
			if(battle.circumstance==(byte)1) {
				if(battle.attackerVictorious) {
					battleText+="The pillage of "+Globals_Game.fiefMasterList[battle.battleLocation].name + " has been prevented!\n";
				}
			}
			if(battle.circumstance==(byte)2) {
				if(battle.siegeRaised) {
					battleText+="The siege in "+ Globals_Game.fiefMasterList[battle.battleLocation].name +" has been raised.\n";
				}
				else if(battle.DefenderDeadNoHeir) {
					battleText+="The siege in "+ Globals_Game.fiefMasterList[battle.battleLocation].name +" has been raised due to the death of the besieging leader, who leaves no heirs to take his place.\n";
				}
			}
		}
		else {
			if(battle.circumstance!=(byte)0) {
				battleText+="The defending forces failed to bring the attacking army to battle.\n";
			}
		}
		return battleText;
		
	}
}
