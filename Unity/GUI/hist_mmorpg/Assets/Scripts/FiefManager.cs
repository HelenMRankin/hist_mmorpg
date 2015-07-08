using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FiefManager : MonoBehaviour {
	private Dictionary<string,string> movementInstructions = new Dictionary<string, string>(); 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void EnterExitKeep() {
		ProtoMessage message = new ProtoMessage ();
		message.MessageType = Actions.EnterExitKeep;
		NetworkScript.Send (message);
	}

	public void VisitCourt() {
		ProtoMessage message = new ProtoMessage ();
		message.MessageType = Actions.ListCharsInMeetingPlace;
		message.Message = "court";
		NetworkScript.Send (message);
	}

	public void VisitTavern() {
		
		ProtoMessage message = new ProtoMessage ();
		message.MessageType = Actions.ListCharsInMeetingPlace;
		message.Message = "tavern";
		NetworkScript.Send (message);
	}
	public void ListOutsideKeep() {
		
		ProtoMessage message = new ProtoMessage ();
		message.MessageType = Actions.ListCharsInMeetingPlace;
		message.Message = "outside";
		NetworkScript.Send (message);
	}

	public void MoveTo(Object fiefID) {
		string text = (fiefID as GUIText).text;
		ProtoTravelTo message = new ProtoTravelTo ();
		message.travelTo = text;
		message.MessageType = Actions.TravelTo;
		NetworkScript.Send (message);
	}

	public void Camp(Object daySlider) {
		int days_int = Mathf.FloorToInt((daySlider as Slider).value);
		ProtoMessage message = new ProtoMessage ();
		message.MessageFields [0] = days_int.ToString ();
		message.MessageType = Actions.Camp;
		NetworkScript.Send (message);
	}

	public void TakeRoute(Object route) {
		string textContent = (route as GUIText).text;
		string[] instructions = textContent.Split (',');
		ProtoTravelTo message = new ProtoTravelTo ();
		message.travelVia = instructions;
		message.MessageType = Actions.TravelTo;
	}

	public void goHere(string direction) {
		string[] instructions = new string[]{direction};
		ProtoTravelTo message = new ProtoTravelTo ();
		message.travelVia = instructions;
		message.MessageType = Actions.TravelTo;
	}
	//TODO
	public void ExamineArmies() {

	}
}
