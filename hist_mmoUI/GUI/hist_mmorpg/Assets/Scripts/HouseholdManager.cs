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


}
