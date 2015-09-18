using UnityEngine;
using System.Collections;
using ProtoBuf;
using hist_mmorpg;
using System;
public class HouseholdManager : MonoBehaviour {

	void Awake() {
		/*GameStateManager.gameState.DaysLeft = GameObject.Find ("DaysLeft");
		ProtoMessage requestHousehold = new ProtoMessage();
		requestHousehold.ActionType= Actions.GetNPCList;
		requestHousehold.Message=GameStateManager.gameState.preLoadState;
		NetworkScript.Send (requestHousehold);*/
		while(GameStateManager.gameState.SceneLoadQueue.Count>0) {
			GameStateManager.gameState.SceneLoadQueue.Dequeue().Invoke ();
		}
		//GameStateManager.gameState.menuBar.transform.SetParent (this.gameObject.transform);
	}
	

	public static void RequestHouseholdList(string type) {
		ProtoMessage requestHousehold = new ProtoMessage();
		requestHousehold.ActionType= Actions.GetNPCList;
		requestHousehold.Message=type;
		NetworkScript.Send (requestHousehold);
	}

	public static void viewCharacter(string charID) {
		ProtoMessage viewChar = new ProtoMessage();
		viewChar.ActionType=Actions.ViewChar;
		viewChar.Message=charID;
		NetworkScript.Send (viewChar);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static string CharacterText(ProtoCharacter character) {
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
			if(character.maxHealth!=0) {
				display+="\nHealth: "+character.health.ToString ()+ "/"+character.maxHealth.ToString ();
			}

		}
		else {
			display+="\nThis character is DEAD";
		}
		
		if(!String.IsNullOrEmpty (character.captor)) {
			display+="\nThis character is being held CAPTIVE";
		}
		if(!string.IsNullOrEmpty (character.location)) {
			display+="\nLocation: " +Globals_Game.fiefMasterList[character.location].name;
			if(character.inKeep) {
				display+=" (keep)";
			}
		}
		display+="\nNationality: "+Globals_Game.nationalityMasterList[character.nationality].name;
		display+="\nLanguage: "+Globals_Game.languageMasterList[character.language].GetName();
		if(!string.IsNullOrEmpty (character.spouse)) {
			display+="\nMarried to: "+character.spouse;
		}
		if(!string.IsNullOrEmpty(character.fiancee)) {
			display+="\nEngaged to: "+character.fiancee;
		}
		if(character is ProtoPlayerCharacter)  {
			ProtoPlayerCharacter pc = character as ProtoPlayerCharacter;
			display+="Purse: "+pc.purse.ToString ();
			if(pc!=null &&pc.myNPCs!=null) {
				display+="\n\nHousehold: ";
				foreach(ProtoCharacterOverview member in pc.myNPCs) {
					display+="\n"+member.charName + ", "+ member.role;
				}
			}
		}
		if(character.traits!=null) {
			display+="\nTraits: ";
			foreach(Pair trait in character.traits) {
				display+="\n"+trait.key + " " +trait.value;
			}
		}
		return display;
	}
}
