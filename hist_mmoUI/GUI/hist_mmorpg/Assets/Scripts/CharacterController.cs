using UnityEngine;
using System.Collections;
using hist_mmorpg;
using ProtoBuf;
using UnityEngine.UI;
using System;
public class CharacterController : MonoBehaviour {
	private string selectedCaptive;
	public GameObject gaol;
	public GameObject itemDetails;
	void Awake() {
		ProtoMessage viewChar = new ProtoMessage();
		viewChar.ActionType=Actions.ViewChar;
		viewChar.Message=Globals_Client.playerCharacter.charID;
		NetworkScript.Send (viewChar);
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowCaptiveControls() {
		gaol.SetActive (true);
		itemDetails.SetActive(true);
	}
	public void DisplayPlayerCharacter(ProtoPlayerCharacter pc ){
		string text = HouseholdManager.CharacterText(pc);
		string kingdomTitles = "";
		string provinceTitles = "";
		string fiefTitles="";
		Language charLanguage = Globals_Game.languageMasterList[pc.language];
		foreach(string id in pc.titles) {
			if(Globals_Game.provinceMasterList.ContainsKey (id)) {
				provinceTitles+="\n"+Globals_Game.provinceMasterList[id].rank.GetName (charLanguage) + " of "+Globals_Game.provinceMasterList[id].name;
			}
			else if(Globals_Game.kingdomMasterList.ContainsKey (id)) {
				kingdomTitles+="\n"+Globals_Game.kingdomMasterList[id].rank.GetName (charLanguage) + " of " + Globals_Game.kingdomMasterList[id].name;
			}
			else if(Globals_Game.fiefMasterList.ContainsKey(id)) {
				fiefTitles+="\n"+Globals_Game.fiefMasterList[id].rank.GetName (charLanguage) + " of " + Globals_Game.fiefMasterList[id].name;
			}
		}
		text+="\n\nTitles: \n";
		if(!string.IsNullOrEmpty (kingdomTitles)) {
			text+= "\nKingdoms: " + kingdomTitles;
		}
		if(!string.IsNullOrEmpty (provinceTitles)) {
			text+="\nProvinces: "+provinceTitles;
		}
		if(!string.IsNullOrEmpty (fiefTitles)) {
			text+="\nFiefs: "+fiefTitles;
		}
		GameObject.Find ("ScrollPanel").GetComponentInChildren<Text>().text = text;
	}

	public void DisplayCaptives(ProtoCharacterOverview[] captives) {
		ShowCaptiveControls ();
		foreach(ProtoCharacterOverview captive in captives) {
			GameObject captiveOverview = Resources.Load<GameObject> ("CaptiveOverview");
			var row = Instantiate (captiveOverview);
			row.name=captive.charID;
			row.transform.SetParent (this.gameObject.transform.Find ("Gaol").Find ("PanelContents"));
			row.transform.Find ("charName").GetComponent<Text>().text = captive.charName;
			if(captive.isMale) {
				row.transform.Find ("charGender").GetComponent<Text>().text = "Male";
			}
			else {
				row.transform.Find ("charGender").GetComponent<Text>().text = "Female";
			}
			row.transform.Find ("charOwner").GetComponent<Text>().text = captive.owner;
			row.GetComponent<Button>().onClick.AddListener (()=>GetCaptive(row.name));
		}
	}

	public void GetCaptives() {
		ProtoMessage getCaptives = new ProtoMessage();
		getCaptives.ActionType= Actions.ViewCaptives;
		getCaptives.Message="all";
		NetworkScript.Send (getCaptives);
	}
	public void GetCaptive(string captiveID) {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.ViewCaptive;
		message.Message=captiveID;
		NetworkScript.Send (message);
	}
	public void ShowCaptive(ProtoCharacter captive) {
		Debug.Log ("Showing captive controls");
		this.selectedCaptive=captive.charID;
		this.gameObject.transform.Find ("ItemDetails").GetComponentInChildren<Text>().text= HouseholdManager.CharacterText (captive);
		ShowCaptiveControls();
	}
}
	