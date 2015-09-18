using UnityEngine;
using System.Collections;
using System;
using hist_mmorpg;
public class JournalController : MonoBehaviour {
	ProtoJournalEntry entry;
	void Awake() {
		while(GameStateManager.gameState.SceneLoadQueue.Count>0) {
			GameStateManager.gameState.SceneLoadQueue.Dequeue().Invoke ();
		}
	}
	// Use this for initialization
	void Start () {
		/*ProtoMessage requestEntries = new ProtoMessage();
		requestEntries.ActionType=Actions.ViewJournalEntries;
		requestEntries.Message=GameStateManager.gameState.preLoadState;
		NetworkScript.Send (requestEntries); */

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void RequestEntries(string entryType) {
		ProtoMessage requestEntries = new ProtoMessage();
		requestEntries.ActionType=Actions.ViewJournalEntries;
		requestEntries.Message=entryType;
		NetworkScript.Send (requestEntries);
	}

	public static void acceptProposal(string jId) {
		ProtoMessage accept = new ProtoMessage();
		accept.ActionType=Actions.AcceptRejectProposal;
		accept.Message=jId;
		accept.MessageFields=new string[]{"True"};
		NetworkScript.Send (accept);
	}

	public static void rejectProposal(string jId) {
		ProtoMessage accept = new ProtoMessage();
		accept.ActionType=Actions.AcceptRejectProposal;
		accept.Message=jId;
		accept.MessageFields=new string[]{"False"};
		NetworkScript.Send (accept);
	}

	public static void acceptRansom(string jId) {
		ProtoMessage accept = new ProtoMessage();
		accept.ActionType=Actions.RespondRansom;
		accept.Message=jId;
		accept.MessageFields=new string[]{"True"};
		NetworkScript.Send (accept);
	}
	
	public static void rejectRansom(string jId) {
		ProtoMessage accept = new ProtoMessage();
		accept.ActionType=Actions.RespondRansom;
		accept.Message=jId;
		accept.MessageFields=new string[]{"False"};
		NetworkScript.Send (accept);
	}


	public static string PillageMessage(ProtoPillageResult details) {
		string message = "On this the day of Our Lord the fief of "+details.fiefName + " owned by "+details.fiefOwner;
		message+=" and defended by "+details.defenderLeader + " was pillaged by the forces of "+ details.armyOwner;
		if(!string.IsNullOrEmpty(details.armyLeader)&&!details.armyLeader.Equals (details.armyOwner)) {
			message+= ",led by "+details.armyLeader+", ";
		}
		message+=".\r\nResults:\r\n";
		message+="- Days taken: " + details.daysTaken + "\r\n";
		message+="- Population loss: "+details.populationLoss + "\r\n";
		message+="- Treasury loss: " + details.treasuryLoss+"\r\n";
		message+="- Loyalty loss: "+details.loyaltyLoss+"\r\n";
		message+= "- Fields loss: "+details.fieldsLoss+"\r\n";
		message+="- Industry loss: "+details.industryLoss+"\r\n";
		message+="- Base Money Pillaged: "+details.baseMoneyPillaged+"\r\n";
		if(details.bonusMoneyPillaged>0) {
			message+="\t- and a bonus of "+details.bonusMoneyPillaged+"\r\n";
		}
		message+="- Money pillaged by attacking player: " +details.moneyPillagedOwner;
		return message;
	}
}
