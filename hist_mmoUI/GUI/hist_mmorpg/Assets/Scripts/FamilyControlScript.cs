using UnityEngine;
using System.Collections;
using hist_mmorpg;
using UnityEngine.UI;
//using UnityEditor;
public class FamilyControlScript : MonoBehaviour {
	ListDisplay listDisplay {get;set;}
	// Use this for initialization
	void Start () {
		listDisplay = FindObjectOfType<ListDisplay>();
		//var DaysLeft = GameObject.Find ("DaysLeft");
		//GameStateManager.gameState.DaysLeft=DaysLeft;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void NPCTravel() {
		listDisplay.NPCTravel ();
	}

	public void Entourage() {
		listDisplay.Entourage();
	}

	public void NameHeir() {
		if(listDisplay.selectedItemID!=null) {
			ProtoMessage message = new ProtoMessage();
			message.ActionType=Actions.AppointHeir;
			message.Message=listDisplay.selectedItemID;
			NetworkScript.Send (message);
		}
		else {
			GameStateManager.gameState.DisplayMessage ("You must select a character first");
		}
	}

	public void ProposeMarriage() {
		var groom = GameObject.Find ("Groom");
		var bride = GameObject.Find("Bride");
		string groomId = groom.GetComponent<InputField>().text;
		string brideId =bride.GetComponent<InputField>().text;
		Debug.Log ("Groom: "+groomId+", Bride: "+brideId);
		if(string.IsNullOrEmpty (groomId)||string.IsNullOrEmpty (brideId)) {
			GameStateManager.gameState.DisplayMessage ("You must select a character first");
			return;
		}
		else {
			ProtoMessage marry = new ProtoMessage();
			marry.ActionType=Actions.ProposeMarriage;
			marry.Message = groomId;
			marry.MessageFields=new string[] {brideId};
			NetworkScript.Send (marry);
		}
	}

	public void TryForChild() {
		ProtoMessage tryForChild= new ProtoMessage();
		tryForChild.ActionType=Actions.TryForChild;
		tryForChild.Message=Globals_Client.pcID;
		NetworkScript.Send(tryForChild);
	}



	public void EncouragePregnancy() {
		if(string.IsNullOrEmpty (listDisplay.selectedItemID)) {
			GameStateManager.gameState.DisplayMessage ("You must select a character first");
		}
		else {
			ProtoMessage tryForChild= new ProtoMessage();
			tryForChild.ActionType=Actions.TryForChild;
			tryForChild.Message=listDisplay.selectedItemID;
			NetworkScript.Send(tryForChild);
		}
	}
}
