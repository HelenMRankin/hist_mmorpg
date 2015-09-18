using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using hist_mmorpg;
public class TravelManager : MonoBehaviour {


	// The Hex Map
	public GameObject HexMap;
	// The Travel Controls
	public GameObject TravelControls;
	// The Character Display 
	public GameObject ListDisplay;
	// Text on the enterExitKeep button
	public Text enterExitText;
	// The current cell in the hex map
	public GameObject currentCell;
	private Slider days;
	// Message containing travel instructions, for use when the player has to confirm whether they wish to travel
	private ProtoMessage travelMessage;
	void Awake() {
		GameStateManager.gameState.travelManager=this;
		//GameStateManager.gameState.DaysLeft = GameObject.Find("DaysLeft");
	}
	// Use this for initialization
	void Start () {
		UpdateEnterExitText();
		TravelState();
	}
	
	// Update is called once per frame
	void Update () {
	}

	// Enter the travel state (show hex map and movement controls)
	public void TravelState() {
		ListDisplay.SetActive(false);
		HexMap.SetActive (true);
		TravelControls.SetActive (true);
	}

	// Enter list view state- show list of armies or characters in fief/meeting place
	public void ListState() {
		HexMap.SetActive(false);
		TravelControls.SetActive (false);
		ListDisplay.SetActive (true);
	}

	public void EnterExitKeep() {
		ProtoMessage message = new ProtoMessage ();
		message.Message= Globals_Client.activeChar;
		message.ActionType = Actions.EnterExitKeep;
		NetworkScript.Send (message);
	}

	public void VisitCourt() {
		ProtoMessage message = new ProtoMessage ();
		message.ActionType = Actions.ListCharsInMeetingPlace;
		if(Globals_Client.activeChar!=null) {
			message.MessageFields=new string[] {Globals_Client.activeChar};
		}
		message.Message = "court";
		NetworkScript.Send (message);
	}

	public void VisitTavern() {
		
		ProtoMessage message = new ProtoMessage ();
		message.ActionType = Actions.ListCharsInMeetingPlace;
		if(Globals_Client.activeChar!=null) {
			message.MessageFields=new string[] {Globals_Client.activeChar};
		}
		message.Message = "tavern";
		NetworkScript.Send (message);
	}
	public void ListOutsideKeep() {
		
		ProtoMessage message = new ProtoMessage ();
		message.ActionType = Actions.ListCharsInMeetingPlace;
		if(Globals_Client.activeChar!=null) {
			message.MessageFields=new string[] {Globals_Client.activeChar};
		}
		message.Message = "outside";
		NetworkScript.Send (message);
	}

	public void MoveTo(GameObject fiefID) {
		string text = (fiefID.GetComponent<Text>()).text;
		ProtoTravelTo message = new ProtoTravelTo ();
		// Check which character is performing action
		if(Globals_Client.activeChar!=null) {
			message.characterID=Globals_Client.activeChar;
		}
		message.travelTo = text;
		message.ActionType = Actions.TravelTo;
		if(Globals_Client.activeCharacter.siegeRole==ProtoCharacter.SiegeRole.Besieger) {
			this.travelMessage=message;
			GameStateManager.gameState.DisplayMessage ("You are currently besieging this fief. Moving will end your siege. \n Do you wish to proceed?","Yes","No", OnTravelConfirm);
		}
		else {
			NetworkScript.Send (message);
		}
	}

	public void Camp(Slider daySlider) {
		days=daySlider;
		int days_int = Mathf.FloorToInt(daySlider.value);
		bool proceed = true;
		if(days_int > Globals_Client.days) {
			GameStateManager.gameState.DisplayMessage ("You only have " + Globals_Client.days + " days left this season.\nCamp for this number of days?","Camp","Cancel",onCampConfirm);
			proceed=false;
		}
		if(proceed) {
			ProtoMessage message = new ProtoMessage ();
			message.MessageFields = new string[] {days_int.ToString ()};
			// Check which character is performing action
			if(Globals_Client.activeChar!=null) {
				message.Message=Globals_Client.activeChar;
			}
			message.ActionType = Actions.Camp;
			NetworkScript.Send (message);
		}
		else {
			return;
		}
	}

	public void onCampConfirm(bool confirmed) {
		if(confirmed) {
			int days_int = Mathf.FloorToInt(days.value);
			ProtoMessage message = new ProtoMessage ();
			message.MessageFields = new string[] {days_int.ToString ()};
			// Check which character is performing action
			if(Globals_Client.activeChar!=null) {
				message.Message=Globals_Client.activeChar;
			}
			message.ActionType = Actions.Camp;
			NetworkScript.Send (message);
		}
	}
	public void TakeRoute(GameObject route) {
		string textContent = (route.GetComponent<Text>()).text;
		string[] instructions = textContent.Split (',');
		ProtoTravelTo message = new ProtoTravelTo ();
		// Check which character is performing action
		if(Globals_Client.activeChar!=null) {
			message.characterID=Globals_Client.activeChar;
		}
		message.travelVia = instructions;
		message.ActionType = Actions.TravelTo;
		if(Globals_Client.activeCharacter.siegeRole==ProtoCharacter.SiegeRole.Besieger) {
			this.travelMessage=message;
			GameStateManager.gameState.DisplayMessage ("You are currently besieging this fief. Moving will end your siege. \n Do you wish to proceed?","Yes","No", OnTravelConfirm);
		}
		else {
			NetworkScript.Send (message);
		}
	}

	public void goHere(string direction) {
		string[] instructions = new string[]{direction};
		ProtoTravelTo message = new ProtoTravelTo ();
		message.travelVia = instructions;
		message.ActionType = Actions.TravelTo;
		// Check which character is performing action
		if(Globals_Client.activeChar!=null) {
			message.characterID=Globals_Client.activeChar;
		}
		if(Globals_Client.activeCharacter.siegeRole==ProtoCharacter.SiegeRole.Besieger) {
			this.travelMessage=message;
			GameStateManager.gameState.DisplayMessage ("You are currently besieging this fief. Moving will end your siege. \n Do you wish to proceed?","Yes","No", OnTravelConfirm);
		}
		else {
			NetworkScript.Send (message);
		}
	}

	public void OnTravelConfirm(bool confirm) {
		if(confirm) {
			NetworkScript.Send (travelMessage);
		}
		else {
			GameStateManager.gameState.DisplayMessage("You chose to remain in the current fief.");
		}
	}


	public void ExamineArmies() {
		ProtoMessage message = new ProtoMessage();
		message.Message = Globals_Client.currentLocation.id;
		message.ActionType=Actions.ExamineArmiesInFief;
		NetworkScript.Send (message);
	}

	public void UpdateEnterExitText() {
		if(Globals_Client.inKeep) {
			enterExitText.text="Exit keep";
		}
		else {
			enterExitText.text = "Enter keep";
		}
	}

	// Note- a null charID will default to client's head of family
	public void UseCharacter(string charID=null) {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.UseChar;
		message.Message=charID;
		Debug.Log ("Attempting to use "+charID);
		NetworkScript.Send (message);
	}

}
