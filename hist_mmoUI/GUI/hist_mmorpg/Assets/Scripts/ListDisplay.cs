using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;
using System;
using hist_mmorpg;
public class ListDisplay : MonoBehaviour {
	// Title of the current list screen
	public Text TitleText;
	// Container representing list. Can add rows
	public GameObject itemList;
	// Which object to use as a row for populating the list
	public GameObject row;
	// GameObject holding controls for the selected item, such as hiring controls for NPCs
	public GameObject controls;
	// Current state- contains object type (e.g. character, army) and circumstance (e.g. tavern)
	public string state;
	// id of selected item, i.e. character or army (for hiring, firing, attacking etc)
	public string selectedItemID;
	public Dictionary<string,GameObject> rows = new Dictionary<string,GameObject>();
	// Stores attack message in the event that an attack first needs to be confirmed
	private ProtoMessage AttackConfirm;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	// Adds a character to the list of characters
	public void AddCharacter(ProtoCharacterOverview details) {
		var item = Instantiate (row);
		if(!rows.ContainsKey(details.charID)) {
			rows.Add (details.charID,item);
		}
		item.transform.SetParent(itemList.transform);
		item.name=details.charID;
		var name= item.transform.Find("charName");
		name.GetComponent<Text>().text=details.charName;
		var role = item.transform.Find ("charRole");
		role.GetComponent<Text>().text=details.role;
		var gender=item.transform.Find ("charGender");
		if(details.isMale) {
			gender.GetComponent<Text>().text="Male";
		}
		else {
			gender.GetComponent<Text>().text="Female";
		}
	}

	public void RemoveItem(string rowID) {
		GameObject row;
		rows.TryGetValue (rowID,out row);
		if(row!=null) {
			rows.Remove (rowID);
			Destroy (row);
			Debug.Log ("Removed and destroyed row");
		}
	}

	public void initializeContentsCharacters(string circumstance, ProtoGenericArray<ProtoCharacterOverview> items) {
		row=(GameObject)Resources.Load ("CharacterOverview");
		state="character";
		Debug.Log ("Initialising Character list");
		// Clear previous contents
		foreach(Transform child in itemList.transform) {
			Destroy (child.gameObject);
		}
		
		switch(circumstance) {
		case "tavern": {
			state+="tavern";
			// Set title
			TitleText.text = "Persons in the Tavern of "+Globals_Client.currentLocation.name;
			// Populate table
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "court": {
			state+="court";
			// Set title
			TitleText.text = "Persons in the Esteemed Court of "+Globals_Client.currentLocation.name;
			// Populate table
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "outside": {
			state+="outside";
			// Set title
			TitleText.text = "Persons outside the keep in "+Globals_Client.currentLocation.name;
			// Populate table
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "Family": {
			state+="Family";
			TitleText.text = "Your Family Members";
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "Employ": {
			state +="Employ";
			TitleText.text = "Persons in your employ";
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "Entourage": {
			state +="Entourage";
			TitleText.text= "Your Entourage";
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}

		}
			break;
		case "FamilyEmploy": {
			state+="FamilyEmploy";
			TitleText.text="Employees and Family Members in your service";
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
		}
			break;
		case "bailiff": {
			state= "character";
			if(items !=null&&items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields)  {
					AddCharacter(charDetails);
				}
			}
			var SelectItem = GameObject.Find ("SelectItem");
			SelectItem.GetComponent<Button>().GetComponentInChildren<Text>().text = "Appoint Bailiff";

			SelectItem.GetComponent<Button>().onClick.RemoveAllListeners ();
			SelectItem.GetComponent<Button>().onClick.AddListener (()=>FiefController.appointBailiff(selectedItemID));
		}
			break;
		case "leader":{
			state= "character";
			Debug.Log ("Got leader");
			if(items !=null && items.fields!=null) {
				foreach(ProtoCharacterOverview charDetails in items.fields) {
					Debug.Log ("Adding character to list");
					AddCharacter(charDetails);
				}
			}
			if(items.fields==null) {
				Debug.Log ("No suitable canditates?");
			}
		}
			break;
		default: {
			Debug.LogError("ListDisplay: character type not recognised");
		}
			break;
		}
	}

	public void initializeContentsArmies(ProtoGenericArray<ProtoArmyOverview> armies) {
		state = "army";
		TitleText.text = "Armies in "+Globals_Client.currentLocation.name;
		row=(GameObject)Resources.Load ("ArmyOverview");
		if(armies!=null&&armies.fields!=null) {
			foreach(ProtoArmyOverview army in armies.fields) {
				AddArmy(army);
			}
		}
	}

	public void initializeContentsJournal(ProtoGenericArray<ProtoJournalEntry> entries) {
		state = "journal";
		TitleText.text = GameStateManager.gameState.preLoadState + " Journal Entries";
		row = (GameObject)Resources.Load ("JournalEntry");
		if(entries!=null&&entries.fields!=null) {
			foreach(ProtoJournalEntry entry in entries.fields) {
				AddEntry(entry);
			}
		}

		Debug.Log ("Done");
	}
	public void AddArmy(ProtoArmyOverview army) {
		var item = Instantiate (row);
		item.transform.SetParent(itemList.transform);
		item.name = army.armyID;
		var id= item.transform.Find("ArmyID");
		id.GetComponent<Text>().text = army.armyID;
		var owner = item.transform.Find("ArmyOwn");
		owner.GetComponent<Text>().text = army.ownerName;
		var leader = item.transform.Find("ArmyLead");
		leader.GetComponent<Text>().text = army.leaderName;
	}

	public void AddEntry(ProtoJournalEntry entry) {
		Debug.Log ("adding entry: type = "+ entry.type);
		var item = Instantiate (row);
		item.transform.SetParent(itemList.transform);
		item.name = entry.jEntryID.ToString ();
		var type = item.transform.Find ("type");
		type.GetComponent<Text>().text= entry.type;
		var year = entry.year;
		var season = entry.season;
		var date = item.transform.Find ("EntryDate");
		string seasonText;
		if(season==0) seasonText="Spring";
		else if(season==1)seasonText="Summer";
		else if(season==2)seasonText="Autumn";
		else seasonText="Winter";
		date.GetComponent<Text>().text = seasonText+" "+year.ToString ();
	}


	public void getDetails(string itemID) {
		if(state.Contains ("character")) {
			ProtoMessage viewChar = new ProtoMessage();
			viewChar.ActionType=Actions.ViewChar;
			viewChar.Message=itemID;
			NetworkScript.Send (viewChar);
		}
		else if(state.Contains ("army")) {
			ProtoMessage viewArmy = new ProtoMessage();
			viewArmy.ActionType=Actions.ViewArmy;
			viewArmy.Message=itemID;
			NetworkScript.Send (viewArmy);
		}
		else if (state.Contains("journal")) {
			ProtoMessage viewEntry = new ProtoMessage();
			viewEntry.ActionType=Actions.ViewJournalEntry;
			viewEntry.Message=itemID;
			NetworkScript.Send (viewEntry);
		}
	}

	public void DisplayDetails(ProtoArmy army) {
		//Destroy existing controls
		//Destroy (controls);
		var details = GameObject.Find ("ItemDetails");
		bool isOwner = army.ownerID.Equals (Globals_Client.pcID);
		Text armyText = details.GetComponent<Text>();
		string display = "Army ID: "+army.armyID;
		Nationality nat;
		Globals_Game.nationalityMasterList.TryGetValue (army.nationality,out nat);
		display+= "\nNationality: " + nat.name;
		display+="\nOwner: "+army.owner;
		display+="\nLeader: "+army.leader;
		display+="\nLocation: "+army.location;
		display +="\nTroops ";
		if(!isOwner) {
			display+="(Estimate)";
		}
		uint totalTroops = 0;
		string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Longbowmen: ", " - Crossbowmen: ", " - Foot: ", " - Rabble: " };
		// display numbers for each troop type
		Debug.Log ("Army troops length" + army.troops.Length);
		Debug.Log ("labels length " + troopTypeLabels.Length);
		for (int i = 0; i < army.troops.Length; i++)
		{
			display += "\n";
			display += troopTypeLabels[i] + army.troops[i];
			totalTroops += army.troops[i];
		}
		display+="\nTotal Troops: "+totalTroops;

		if(isOwner) {
			if(army.isMaintained) {
				display+="\nThis army is currently being maintained";
			}
			else {
				display+="\nThis army is NOT currently being maintained";
			}
			display+="\nAggression level: "+army.aggression;
			display+="\nSally value: "+army.combatOdds;
		}
		armyText.text=display;
		if(Application.loadedLevelName.Equals ("Combat")) {
			ArmyManager armyManger= FindObjectOfType<ArmyManager>();
			if(armyManger.InDropOffState) {
				Debug.Log ("In display details dropoffstate works");
				armyManger.ShowDropOff();
			}
			else {
				armyManger.ShowArmyList();
			}

		}
		else {
			// If is own army
			if(army.ownerID==Globals_Client.playerCharacter.charID) {
				// Show manage army button
				var ownControls = Instantiate(Resources.Load<GameObject> ("OwnedArmyControls"));
				ownControls.transform.SetParent (this.transform.Find ("ItemControls"));
				ownControls.transform.localPosition = new Vector2(0,0);
				ownControls.transform.Find ("ManageArmy").GetComponent<Button>().onClick.AddListener (()=>ManageArmy(army.armyID));
			}
			else {
				// Show attack controls
				var enemyControls = Instantiate(Resources.Load<GameObject> ("EnemyArmyControls"));
				enemyControls.transform.SetParent (this.transform.Find ("ItemControls"));
				enemyControls.transform.localPosition = new Vector2(0,0);
				enemyControls.transform.Find ("Troops").GetComponent<Button>().onClick.AddListener (()=>DropOffForChar(army.ownerID));
				enemyControls.transform.Find ("Attack").GetComponent<Button>().onClick.AddListener(()=>Attack(army.armyID));
				enemyControls.transform.Find ("Spy").GetComponent<Button>().onClick.AddListener(()=>Spy(army.armyID, Actions.SpyArmy));
			}
		}
	}

	private void ManageArmy(string armyID ) {
		getDetails(armyID);
		Application.LoadLevel ("Combat");
	}

	private void Attack(string armyID) {
		Debug.Log ("Active armyID = "+Globals_Client.activeCharacter.armyID);
		if(string.IsNullOrEmpty (Globals_Client.activeCharacter.armyID)) {
			GameStateManager.gameState.DisplayMessage ("You must have an army in order to attack!");
			return;
		}
		if (string.IsNullOrEmpty (armyID) ) {
			GameStateManager.gameState.DisplayMessage ("You must select an army to attack");
			return;
		}
		ProtoMessage attack = new ProtoMessage();
		attack.ActionType=Actions.Attack;
		attack.Message=Globals_Client.activeCharacter.armyID;
		attack.MessageFields=new string[]{armyID};
		if(Globals_Client.activeCharacter.siegeRole == ProtoCharacter.SiegeRole.Besieger) {
			AttackConfirm=attack;
			GameStateManager.gameState.DisplayMessage("You are currently besieging this fief. Attacking an army will end the siege. \n\nContinue?","Yes","No",OnAttackConfirm);
		}
		else {
			NetworkScript.Send (attack);
		}
	}
	private void DropOffForChar(string charID) {
		getDetails (Globals_Client.activeCharacter.armyID);
		GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>ArmyManager.DropOffForCharacter(charID));
		Application.LoadLevel ("Combat");
	}

	private void Spy(string armyID, Actions action) {
		ProtoMessage spyOn = new ProtoMessage();
		spyOn.Message=Globals_Client.activeCharacter.charID;
		spyOn.MessageFields=new string[]{armyID};
		spyOn.ActionType=action;
		NetworkScript.Send (spyOn);
	}

	private void OnAttackConfirm(bool confirm) {
		if(confirm) {
			NetworkScript.Send (AttackConfirm);
		}
	}
	/// <summary>
	/// Will either display the details and controls for a character or remove them, depending on whether row exists
	/// </summary>
	/// <param name="character">Character.</param>
	public void DisplayDetails(ProtoCharacter character, bool displayControls = true) {
		if(controls!=null) {
			Destroy (controls);
		}
		var details = this.gameObject.transform.Find ("ItemDetails");
		Text charText = details.GetComponentInChildren<Text>();
		// If row has been deleted, clear text and remove controls
		if(!rows.ContainsKey(character.charID)) {
			Debug.Log ("Clearing details...");
			// clear text
			charText.text=null;
			// remove all controls
			if(controls!=null) {
				Destroy (controls);
			}
		}

		else{
			string display = "Character ID: "+character.charID;
			display+= "\nName: " + character.firstName + " " + character.familyName; 
			if(character.isMale) {
				display += "\nMale";
			}
			else {
				display+="\nFemale";
			}
			display+="\nBorn: ";
			if(character.birthSeason==(byte)0) {
				display+="Spring";
			}
			else if (character.birthSeason==(byte)1) {
				display+="Summer";
			}
			else if (character.birthSeason==(byte)2) {
				display+="Autumn";
			}
			else {
				display+="Winter";
			}
			display+=character.birthYear.ToString ();
			display+="\nMother: "+character.mother;
			display+="\nFather: "+character.father;
			if(character.isAlive) {
				display+="\nHealth: "+character.health.ToString ()+ "/"+character.maxHealth.ToString ();
			}
			else {
				display+="\nThis character is DEAD";
			}

			if(!String.IsNullOrEmpty (character.captor)) {
				display+="\nThis character is being held CAPTIVE";
			}
			if(character.traits!=null) {
				display+="\nTraits: ";
				foreach(Pair trait in character.traits) {
					display+="\n"+trait.key + " " +trait.value;
				}
			}
			charText.text = display;
			// If is family member, display family controls
			if(character.familyID.Equals (Globals_Client.pcID)) {
				if(displayControls) {
					var prefab = (GameObject)Resources.Load ("FamilyControls");
					var familyControls = (GameObject)Instantiate (prefab);
					controls=familyControls;
					familyControls.transform.SetParent (GameObject.Find ("ItemControls").transform);
					familyControls.transform.localPosition = new Vector2(0,0);

				}
				Debug.Log ("is family");

			}
			else if(!string.IsNullOrEmpty (character.familyID)) {
				Debug.Log ("is family (but not yours");
				Debug.Log (character.familyID);
				var prefab = (GameObject)Resources.Load ("EnemyCharacterControls");
				var enemyControls = (GameObject)Instantiate (prefab);
				controls=enemyControls;
				enemyControls.transform.SetParent (this.gameObject.transform.Find ("ItemControls"));
				enemyControls.transform.localPosition = new Vector2(-62,0);
				Button kidnap = enemyControls.transform.Find ("Kidnap").GetComponent<Button>();
				kidnap.onClick.AddListener (() =>KidnapNPC());
				Button spy = enemyControls.transform.Find ("Spy").GetComponent<Button>();
				spy.onClick.AddListener (()=>Spy(character.charID,Actions.SpyCharacter));
			}
			else {
				Debug.Log ("is NPC: "+character.charID);
				// If character is unemployed, show unemployed controls
				ProtoNPC npc = character as ProtoNPC;
				if(npc==null) {
					Debug.LogError ("Could not convert to NPC");
					// error
					return;
				}
				if(npc.employer==null ) {
					Debug.Log ("Creating unemployed controls");
					var prefab = (GameObject)Resources.Load ("UnemployedControls");
					var unemployedControls = (GameObject)Instantiate (prefab);
					controls=unemployedControls;
					unemployedControls.transform.SetParent (this.gameObject.transform.Find ("ItemControls"));
					unemployedControls.transform.localPosition = new Vector2(-62,0);
					Button hire = unemployedControls.transform.Find ("Hire").GetComponent<Button>();
					InputField input = unemployedControls.GetComponentInChildren<InputField>();
					hire.onClick.AddListener (() =>HireNPC(input));
					Button spy = unemployedControls.transform.Find ("Spy").GetComponent<Button>();
					spy.onClick.AddListener (()=>Spy(npc.charID,Actions.SpyCharacter));
				}
				else if(npc.employer.Equals (Globals_Client.pcID)) {
					Debug.Log ("Creating employee controls");
					if(displayControls) {
						var prefab = (GameObject)Resources.Load ("EmployeeControls");
						var employedControls = (GameObject)Instantiate (prefab);
						controls=employedControls;
						employedControls.transform.SetParent (this.gameObject.transform);
						employedControls.transform.localPosition = new Vector2(83,-120);
						Button FireButton = employedControls.transform.Find ("Fire").GetComponent<Button>();
						Button EntourageButton =employedControls.transform.Find ("Entourage").GetComponent<Button>();
						Button Travel = employedControls.transform.Find ("Travel").GetComponent<Button>();
						FireButton.onClick.AddListener (() =>FireNPC());
						EntourageButton.onClick.AddListener (()=>Entourage());
						Travel.onClick.AddListener (()=>NPCTravel());
					}
				}
				else {
					var prefab = (GameObject)Resources.Load ("EnemyCharacterControls");
					var enemyControls = (GameObject)Instantiate (prefab);
					controls=enemyControls;
					enemyControls.transform.SetParent (this.gameObject.transform.Find ("ItemControls"));
					enemyControls.transform.localPosition = new Vector2(-62,0);
					Button kidnap = enemyControls.transform.Find ("Kidnap").GetComponent<Button>();
					kidnap.onClick.AddListener (() =>KidnapNPC());
					Button spy = enemyControls.transform.Find ("Spy").GetComponent<Button>();
					spy.onClick.AddListener (()=>Spy(character.charID,Actions.SpyCharacter));
				}
			}
		}
	}

	public void KidnapNPC() {
		Debug.Log ("In kidnap");
		ProtoMessage kidnap = new ProtoMessage();
		kidnap.ActionType=Actions.Kidnap;
		kidnap.Message=Globals_Client.activeCharacter.charID;
		kidnap.MessageFields=new string[]{selectedItemID};
		NetworkScript.Send (kidnap);
	}
	public void DisplayDetails(ProtoJournalEntry entry) {

		Destroy (controls);
		var details = this.gameObject.transform.Find ("ItemDetails");
		if(details==null) {
			Debug.Log ("details is null");
		}
		Text charText = details.GetComponentInChildren<Text>();
		string seasonText;
		if(entry.season==0) seasonText="Spring";
		else if(entry.season==1)seasonText="Summer";
		else if(entry.season==2)seasonText="Autumn";
		else seasonText="Winter";
		charText.text = seasonText+" "+entry.year;
		charText.text +="\n"+entry.type;
		charText.text +="\n\nPersonae:";
		foreach(ProtoCharacterOverview persona in entry.personae) {
			charText.text+="\n"+persona.charName;
		}

		charText.text+="\n\nDescription:\n";
		if(entry.type.Equals ("battle")){
			string battleResults = ArmyManager.DisplayBattle(entry.eventDetails as ProtoBattle);
			charText.text+=battleResults;
		}

		else {
			ProtoMessage m = entry.eventDetails;
			string message=null;
			if(entry.type.Equals ("deathOfFamily")) {
				if(m.MessageFields!=null) {
					Debug.Log ("Message: "+message);
					Debug.Log ("Fields : "+m.MessageFields.Length);
					foreach(string field in m.MessageFields) {
						Debug.Log (field);
					}
					message = string.Format (message,m.MessageFields);
					message=message.Replace ("\\n",Environment.NewLine);
					if(m.MessageFields.Length>3) {
						message+="He is succeeded by his "+m.MessageFields[3];
					}
				}
			}
			else if(m.GetType()==typeof(ProtoPillageResult)) {
				message = JournalController.PillageMessage( m as ProtoPillageResult);
			}
			else {
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
						Debug.Log ("Message: "+message);
						Debug.Log ("Fields : "+m.MessageFields.Length);
						foreach(string field in m.MessageFields) {
							Debug.Log (field);
						}
						message = string.Format (message,m.MessageFields);
						message=message.Replace ("\\n",Environment.NewLine);
					}
				}
			}

			charText.text+=message;
		}
		// If an entry is a proposal, the first persona is the head of family groom. Ensure cannot reply to own proposal
		if(entry.type.Equals ("proposalMade")&&entry.replied==false&&!entry.personae[0].charID.Equals (Globals_Client.pcID) ) {
			// Display controls
			var proposalControls = Resources.Load ("ProposalControls");
			GameObject propControls = (GameObject)Instantiate(proposalControls,new Vector2(183,-94), Quaternion.identity);
			propControls.transform.parent= (this.gameObject.transform);
			propControls.transform.localPosition= new Vector2(182,-94);
			GameObject.Find ("Accept").GetComponent<Button>().onClick.AddListener (()=>JournalController.acceptProposal(selectedItemID));
			GameObject.Find ("Reject").GetComponent<Button>().onClick.AddListener (()=>JournalController.rejectProposal(selectedItemID));

		}
	}
	
	/// <summary>
	/// Hire the selected character
	/// </summary>
	/// <param name="input">Input field containing amount willing to bid for character</param>
	public void HireNPC(InputField input) {
		if(string.IsNullOrEmpty (selectedItemID)) {
			GameStateManager.gameState.DisplayMessage ("You must select a character to hire first");
			return;
		}
		string amount = input.textComponent.text;
		if(string.IsNullOrEmpty(amount)) {
			GameStateManager.gameState.DisplayMessage ("Please enter an amount");
			return;
		}
		uint bid = 0;
		if(!uint.TryParse (amount,out bid)) {
			GameStateManager.gameState.DisplayMessage ("Please enter a valid number");
			return;
		}
		else {
			ProtoMessage hireMessage = new ProtoMessage();
			hireMessage.ActionType=Actions.HireNPC;
			hireMessage.Message=selectedItemID;
			hireMessage.MessageFields=new string[] {amount.ToString ()};
			NetworkScript.Send (hireMessage);
		}
	}

	/// <summary>
	/// Fire the NPC
	/// </summary>
	public void FireNPC() {
		ProtoMessage fireMessage = new ProtoMessage();
		fireMessage.ActionType=Actions.FireNPC;
		fireMessage.Message=selectedItemID;
		NetworkScript.Send (fireMessage);
	}


	/// <summary>
	/// Add this character to entourage
	/// </summary>
	public void Entourage() {
		ProtoMessage entourageMessage = new ProtoMessage();
		entourageMessage.ActionType=Actions.AddRemoveEntourage;
		entourageMessage.Message=selectedItemID;
		NetworkScript.Send (entourageMessage);
	}

	/// <summary>
	/// Perform travel actions (move, camp) with NPC
	/// </summary>
	public void NPCTravel() {
		GameStateManager.UseCharacter (selectedItemID);

		GameStateManager.ExecutionQueue.Enqueue (()=> {Application.LoadLevel ("Travel");});
	}
	
}
