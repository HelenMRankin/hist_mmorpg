using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIComponents : MonoBehaviour {
	public GameObject HexMap;
	public GameObject TravelControls;
	public GameObject ListDisplay;
	public GameObject DaysLeft;
	// Use this for initialization
	void Start () {
		/*
		GameStateManager.gameState.HexMap=HexMap;
		GameStateManager.gameState.ListDisplay=ListDisplay;
		GameStateManager.gameState.TravelControls=TravelControls; */

	}
	
	// Update is called once per frame
	void Update () {
		DaysLeft.GetComponent<Text>().text = "Days left: "+Globals_Client.days;
	}
}
