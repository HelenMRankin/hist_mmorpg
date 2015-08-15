using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class HexMapManager : MonoBehaviour {

	public GameObject currentHex;
	public GameObject NEHex;
	public GameObject NWHex;
	public GameObject EHex;
	public GameObject WHex;
	public GameObject SEHex;
	public GameObject SWHex;

	private Text currentText;
	private Text NEText ;
	private Text NWText ;
	private Text EText ;
	private Text WText ;
	private Text SEText;
	private Text SWText;

	private string FiefText = "{0} Fief: \n\n{1}\n{2}";
	private string TravelText="\n\nCost: {3}";
	// Use this for initialization
	void Start () {
		currentText = currentHex.GetComponentInChildren<Text>() ;
		NEText = NEHex.GetComponentInChildren<Text>();
		NWText = NWHex.GetComponentInChildren<Text>();
		EText = EHex.GetComponentInChildren<Text>();
		WText = WHex.GetComponentInChildren<Text>();
		SEText = SEHex.GetComponentInChildren<Text>();
		SWText = SWHex.GetComponentInChildren<Text>();

		updateHexMap ();

	}

	public void updateHexMap() {
		Fief current = Globals_Client.currentLocation;
		currentText.text = string.Format (FiefText,"Current",current.name,current.province.name); 
		Fief f = Globals_Game.gameMap.GetFief(current,"NE");
		if(f==null) {
			NEText.text = "No fief present";
			NEHex.GetComponent<Button>().enabled=false;
		}
		else {
			NEText.text = string.Format (FiefText+TravelText,"NE",f.name,f.province.name,current.getTravelCost (f).ToString());
			NEHex.GetComponent<Button>().enabled=true;
		}
		f = Globals_Game.gameMap.GetFief (current,"NW");
		if(f==null) {
			NWText.text = "No fief present";
			NWHex.GetComponent<Button>().enabled=false;
		}
		else {
			NWText.text = string.Format (FiefText+TravelText,"NW",f.name,f.province.name,current.getTravelCost (f).ToString());
			NWHex.GetComponent<Button>().enabled=true;
		}
		f= Globals_Game.gameMap.GetFief (current,"E");
		if(f==null) {
			EText.text = "No fief present";
			EHex.GetComponent<Button>().enabled=false;
		}
		else {
			EText.text = string.Format (FiefText+TravelText,"E",f.name,f.province.name,current.getTravelCost (f).ToString());
			EHex.GetComponent<Button>().enabled=true;
		}
		f=Globals_Game.gameMap.GetFief(current,"W");
		if(f==null) {
			WText.text = "No fief present";
			WHex.GetComponent<Button>().enabled=false;
		}
		else {
			WText.text = string.Format (FiefText+TravelText,"W",f.name,f.province.name,current.getTravelCost (f).ToString());
			WHex.GetComponent<Button>().enabled=true;
		}
		f=Globals_Game.gameMap.GetFief(current,"SE");
		if(f==null) {
			SEText.text = "No fief present";
			SEHex.GetComponent<Button>().enabled=false;
		}
		else {
			SEText.text = string.Format (FiefText+TravelText,"SE",f.name,f.province.name,current.getTravelCost (f).ToString());
			SEHex.GetComponent<Button>().enabled=true;
		}
		f=Globals_Game.gameMap.GetFief (current,"SW");
		if(f==null) {
			SWText.text = "No fief present";
			SWHex.GetComponent<Button>().enabled=false;
		}
		else {
			SWText.text = string.Format (FiefText+TravelText,"SW",f.name,f.province.name,current.getTravelCost (f).ToString());
			SWHex.GetComponent<Button>().enabled=true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
