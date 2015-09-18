using UnityEngine;
using System.Collections;
using System;
using hist_mmorpg;
using ProtoBuf;
using UnityEngine.UI;
public class MenuBarScript : MonoBehaviour {

	void Awake() {
		if(GameStateManager.gameState.menuBar==null) {
			GameStateManager.gameState.menuBar=this.gameObject;
			Debug.Log ("Keeping menu bar: "+this.gameObject.GetInstanceID().ToString ());
			DontDestroyOnLoad(this.gameObject.transform.root.gameObject);
			GameStateManager.gameState.Treasury=GameObject.Find ("Treasury");
			GameStateManager.gameState.CurrentChar =GameObject.Find ("CurrentChar");
			GameStateManager.gameState.DaysLeft=GameObject.Find ("DaysLeft");
			GameObject.Find ("Update").GetComponent<Button>().onClick.AddListener (()=>GameStateManager.SeasonUpdate ());
		}
		else {
			if(GameStateManager.gameState.menuBar!=this.gameObject) {
				Debug.Log("Destroying menu bar");
				Destroy (this.gameObject.transform.root.gameObject);
			}
		}
	}
	// Use this for initialization
	void Start () {
		//GameStateManager.gameState.menuBar=this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		Debug.Log ("Menu bar destroyed: "+this.gameObject.GetInstanceID ().ToString ());
	}

	public void MenuBarClick(string option) {
		if(option.Equals ("Travel")){
			Application.LoadLevel ("Travel");
		}
		else if(option.Equals ("Character") ) {

			Application.LoadLevel ("Character");
		}
		else if(option.Contains ("Household")) {
			
			GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("FamilyEmploy"));
			GameStateManager.gameState.preLoadState="FamilyEmploy";
			// load household
			if(option.Contains ("Family")) {
				// restrict to family
				GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Family"));
				GameStateManager.gameState.preLoadState = "Family";
			}
			else if (option.Contains ("Employ")) {
				GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Employ"));
				GameStateManager.gameState.preLoadState = "Employ";
			}
			else if(option.Contains ("Entourage")) {
				GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList("Entourage"));
				GameStateManager.gameState.preLoadState = "Entourage";
			}
			Application.LoadLevel ("Household");
		}
		else if(option.Contains ("Journal")) {
			GameStateManager.gameState.preLoadState="all";
			if(option.Contains ("year")) {
				GameStateManager.gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("year"));
				GameStateManager.gameState.preLoadState = "year";
			}
			else if (option.Contains ("season")) {
				GameStateManager.gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("season"));
				GameStateManager.gameState.preLoadState = "season";
			}
			else if (option.Contains ("unread")) {
				GameStateManager.gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("unread"));
				GameStateManager.gameState.preLoadState = "unread";
			}
			else {
				GameStateManager.gameState.SceneLoadQueue.Enqueue(()=>JournalController.RequestEntries("all"));
			}
			Application.LoadLevel ("Journal");
		}
		else if(option.Contains ("Fief")) {
			GameStateManager.gameState.preLoadState = "home";
			if(option.Contains ("current")) {
				GameStateManager.gameState.preLoadState=Globals_Client.currentLocation.id;
			}
			if(option.Contains("all")) {
				GameStateManager.gameState.preLoadState="all";
			}
			Application.LoadLevel ("Fief");
		}
		else if(option.Contains("Combat")) {
			// Choose armies or sieges-defaults to armies
			Application.LoadLevel ("Combat");
		}
	}
}
