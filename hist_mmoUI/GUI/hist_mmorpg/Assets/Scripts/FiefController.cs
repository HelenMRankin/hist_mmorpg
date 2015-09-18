using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using hist_mmorpg;
using System;
public class FiefController : MonoBehaviour {

	public GameObject ownedControls;
	public GameObject otherControls;
	public GameObject list;
	public GameObject fiefList;
	public GameObject itemDetails;
	public GameObject controls;
	public GameObject BarredItems;
	public GameObject playerTransfer;
	public GameObject gaol;
	public GameObject armyDisplay;
	public Slider taxSlider;
	public Text garrisonSpend;
	public Text officialSpend;
	public Text infraSpend;
	public Text keepSpend;

	private static string currentViewedFief;
	public ProtoFief currentFiefDetails;
	private static ProtoCharacterOverview selectedBarredCharacter = null;
	private string selectedPlayer;
	private string selectedFief;
	private ProtoCharacter selectedCaptive;
	// TODO rewrite to use pr-load queue
	// Use this for initialization
	void Start () {
		ProtoMessage m = new ProtoMessage();
		if(GameStateManager.gameState.preLoadState.Equals ("all")) {
			GetFiefList();
			return;
		}
		else {
			m.ActionType=Actions.ViewFief;
			m.Message = GameStateManager.gameState.preLoadState;
		}
		//var DaysLeft = GameObject.Find ("DaysLeft");
		//GameStateManager.gameState.DaysLeft=DaysLeft;
		NetworkScript.Send (m);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowFiefList() {
		list.SetActive (false);
		gaol.SetActive (false);
		itemDetails.SetActive (false);
		fiefList.SetActive (true);
		itemDetails.SetActive (false);
		BarredItems.SetActive (false);
		armyDisplay.SetActive (false);
		controls.SetActive (false);
	}

	public void ShowFiefDetails() {
		list.SetActive (false);
		gaol.SetActive (false);
		itemDetails.SetActive (true);
		controls.SetActive (true);
		fiefList.SetActive (false);
		BarredItems.SetActive (false);
		armyDisplay.SetActive (false);
	}

	public void ShowBailiffList() {
		list.SetActive (true);
		gaol.SetActive (false);
		itemDetails.SetActive (false);
		controls.SetActive (false);
		fiefList.SetActive(false);
		BarredItems.SetActive (false);
		armyDisplay.SetActive (false);
	}

	public void ShowArmyList() {
		list.SetActive (false);
		gaol.SetActive (false);
		itemDetails.SetActive (false);
		controls.SetActive (false);
		fiefList.SetActive(false);
		BarredItems.SetActive (false);
		armyDisplay.SetActive(true);
	}
	public void ShowBarredDetails() {
		list.SetActive (false);
		gaol.SetActive (false);
		itemDetails.SetActive (false);
		controls.SetActive (false);
		fiefList.SetActive(false);
		BarredItems.SetActive (true);
		armyDisplay.SetActive (false);
	}

	public void ShowPlayerTransfer(bool show) {
		playerTransfer.SetActive (show);
	}

	public void ShowCaptiveControls() {
		gaol.SetActive (true);
		list.SetActive (false);
		controls.SetActive (false);
		fiefList.SetActive(false);
		BarredItems.SetActive (false);
		armyDisplay.SetActive (false);
	}
	public void GetFief(string fiefID) {
		if(string.IsNullOrEmpty (this.selectedFief)) {
			GameStateManager.gameState.DisplayMessage ("You must select a fief");
			return;
		}
		ProtoMessage requestFief = new ProtoMessage();
		requestFief.ActionType = Actions.ViewFief;
		requestFief.Message=fiefID;
		NetworkScript.Send (requestFief);
	}

	public void DisplayFief(ProtoFief fief = null) {
		Debug.Log ("Displaying Fief");
		ShowFiefDetails ();
		if(fief!=null) {
			currentFiefDetails=fief;
			currentViewedFief=fief.fiefID;
		}
		else {
			fief = currentFiefDetails;
		}
		var details = this.gameObject.transform.Find ("ItemDetails");
		Fief basicFief = Globals_Game.fiefMasterList[fief.fiefID];
		Text fiefText = details.GetComponentInChildren<Text>();
		fiefText.text=basicFief.name+", "+basicFief.province.name+ ", " + basicFief.province.kingdom.name + " ("+fief.fiefID+")";
		if(!string.IsNullOrEmpty (fief.siege)) {
			fiefText.text+="\nUNDER SIEGE";
		}
		fiefText.text+="\nTitle: "+basicFief.rank.ToString ();
		fiefText.text+="\nPopulation: "+fief.population;
		fiefText.text+="\nFields: "+fief.fields;
		fiefText.text+="\nIndustry: "+fief.industry;
		fiefText.text+="\nOwner: "+fief.owner + " Ancestral Owner: "+fief.ancestralOwner.charName;
		fiefText.text+="\nTitle Holder: "+fief.titleHolder;
		fiefText.text+="\nBailiff: ";
		if(fief.bailiff==null) {
			fiefText.text+="None (auto-bailiff)";
		}
		else {
			fiefText.text+= fief.bailiff.charName;
		}
		fiefText.text+="\nStatus: ";
		if(fief.status=='C') fiefText.text+="Calm";
		else if(fief.status=='U') fiefText.text+="Unrest";
		else fiefText.text+="Rebellion";
		fiefText.text+="\nLanguage: "+basicFief.language.GetName() + "(dialect "+basicFief.language.dialect.ToString ()+")";
		fiefText.text+="\nTerrain: "+basicFief.terrain.description;
		fiefText.text+="\nBarred Characters: ";
		if(fief.barredCharacters!=null) {
			foreach(ProtoCharacterOverview character in fief.barredCharacters) {
				fiefText.text+=character.charName + ", ";
			}
		}
		else {
			fiefText.text+="None";
		}

		fiefText.text+="\nBarred Nationalities: ";
		if(fief.barredNationalities!=null) {
			foreach(string nat in fief.barredNationalities) {
				Nationality nationality=null;
				Globals_Game.nationalityMasterList.TryGetValue (nat,out nationality);
				if(nationality!=null) {
					fiefText.text+=nationality.name+ ", ";
				}
			}
		}
		else {
			fiefText.text+="None";
		}

		if(fief.ownerID.Equals (Globals_Client.pcID)) {
			taxSlider.value= (float)fief.keyStatsCurrent[2];
			garrisonSpend.text = fief.keyStatsCurrent[4].ToString();
			officialSpend.text = fief.keyStatsCurrent[3].ToString ();
			keepSpend.text = fief.keyStatsCurrent[6].ToString ();
			infraSpend.text = fief.keyStatsCurrent[5].ToString ();
			ownedControls.SetActive (true);
			otherControls.SetActive (false);
			fiefText.text+="\nGarrison: " + Convert.ToInt32(fief.keyStatsCurrent[4] / 1000).ToString () + " troops";
			fiefText.text+="\nMilitia: up to " + fief.militia.ToString() + " available for call in this fief"; 
			var previous = GameObject.Find ("Previous");
			var current = GameObject.Find ("Current");
			var next = GameObject.Find ("Next");
			previous.GetComponentInChildren<Text>().text = DisplayFiefKeyStatsPrev(fief);
			current.GetComponentInChildren<Text>().text = DisplayFiefKeyStatsCurr(fief);
			next.GetComponentInChildren<Text>().text = DisplayFiefKeyStatsNext(fief);
			var bailiffBtn = GameObject.Find ("AppointRemoveBailiff");
			if(fief.bailiff==null) {
				bailiffBtn.GetComponentInChildren<Text>().text="Appoint Bailiff";
				var viewBailiffBtn = GameObject.Find ("ViewBailiff");
				viewBailiffBtn.GetComponent<Button>().enabled=false;
				var thisFiefTreasury = GameObject.Find ("ThisFief");
				thisFiefTreasury.GetComponent<Text>().text = "This Treasury: " +fief.treasury.ToString ();
				var homeFiefTreasury = GameObject.Find("HomeFief");
				homeFiefTreasury.GetComponent<Text>().text = "Home Treasury: "+ Globals_Client.homeTreasury.ToString ();
				var findlist = list.GetComponentsInChildren (typeof(Button),true);
				foreach (var item in findlist) {
					if(item.name.Equals ("Appoint Self")) {
						((Button)item).onClick.AddListener (()=>appointBailiff(Globals_Client.pcID));
					}
				}
			}
			else {
				bailiffBtn.GetComponentInChildren<Text>().text="Remove Bailiff";
				var viewBailiffBtn = GameObject.Find ("ViewBailiff");
				viewBailiffBtn.GetComponent<Button>().enabled=true;
			}
		}
		else {
			otherControls.SetActive (true);
			ownedControls.SetActive (false);
		}
	}

	/// <summary>
	/// Retrieves previous season's key information for Fief display screen
	/// </summary>
	/// <returns>String containing information to display</returns>
	public string DisplayFiefKeyStatsPrev(ProtoFief fief)
	{

		string fiefText = "PREVIOUS SEASON\r\n=================\r\n\r\n";
		
		// if under siege, check to see if display data (based on siege start date)
		if (fief.keyStatsPrevious==null)
		{
			fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
		}

		// if is OK, display as normal
		else
		{
			// loyalty
			fiefText += "Loyalty: " + fief.keyStatsPrevious[0] + "\r\n\r\n";
			
			// GDP
			fiefText += "GDP: " + fief.keyStatsPrevious[1] + "\r\n\r\n";
			
			// tax rate
			fiefText += "Tax rate: " + fief.keyStatsPrevious[2] + "%\r\n\r\n";
			
			// officials spend
			fiefText += "Officials expenditure: " + fief.keyStatsPrevious[3] + "\r\n\r\n";
			
			// garrison spend
			fiefText += "Garrison expenditure: " + fief.keyStatsPrevious[4] + "\r\n\r\n";
			
			// infrastructure spend
			fiefText += "Infrastructure expenditure: " + fief.keyStatsPrevious[5] + "\r\n\r\n";
			
			// keep spend
			fiefText += "Keep expenditure: " + fief.keyStatsPrevious[6] + "\r\n";
			// keep level
			fiefText += "   (Keep level: " + fief.keyStatsPrevious[7] + ")\r\n\r\n";
			
			// income
			fiefText += "Income: " + fief.keyStatsPrevious[8] + "\r\n\r\n";
			
			// family expenses
			fiefText += "Family expenses: " + fief.keyStatsPrevious[9] + "\r\n\r\n";
			
			// total expenses
			fiefText += "Total fief expenses: " + fief.keyStatsPrevious[10] + "\r\n\r\n";
			
			// overlord taxes
			fiefText += "Overlord taxes: " + fief.keyStatsPrevious[11] + "\r\n";
			// overlord tax rate
			fiefText += "   (tax rate: " + fief.keyStatsPrevious[12] + "%)\r\n\r\n";
			
			// surplus
			fiefText += "Bottom line: " + fief.keyStatsPrevious[13];
		}
		
		return fiefText;
	}

	/// <summary>
	/// Retrieves current season's key information for Fief display screen
	/// </summary>
	/// <returns>String containing information to display</returns>
	public string DisplayFiefKeyStatsCurr(ProtoFief fief)
	{
		
		string fiefText = "CURRENT SEASON\r\n=================\r\n\r\n";
		
		// if under siege, check to see if display data (based on siege start date)
		if (fief.keyStatsCurrent==null)
		{
			fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
		}

		// if is OK, display as normal
		else
		{
			// loyalty
			fiefText += "Loyalty: " + fief.keyStatsCurrent[0] + "\r\n\r\n";
			
			// GDP
			fiefText += "GDP: " + fief.keyStatsCurrent[1] + "\r\n\r\n";
			
			// tax rate
			fiefText += "Tax rate: " + fief.keyStatsCurrent[2] + "%\r\n\r\n";
			
			// officials spend
			fiefText += "Officials expenditure: " + fief.keyStatsCurrent[3] + "\r\n\r\n";
			
			// garrison spend
			fiefText += "Garrison expenditure: " + fief.keyStatsCurrent[4] + "\r\n\r\n";
			
			// infrastructure spend
			fiefText += "Infrastructure expenditure: " + fief.keyStatsCurrent[5] + "\r\n\r\n";
			
			// keep spend
			fiefText += "Keep expenditure: " + fief.keyStatsCurrent[6] + "\r\n";
			// keep level
			fiefText += "   (Keep level: " + fief.keyStatsCurrent[7] + ")\r\n\r\n";
			
			// income
			fiefText += "Income: " + fief.keyStatsCurrent[8] + "\r\n\r\n";
			
			// family expenses
			fiefText += "Family expenses: " + fief.keyStatsCurrent[9] + "\r\n\r\n";
			
			// total expenses
			fiefText += "Total fief expenses: " + fief.keyStatsCurrent[10] + "\r\n\r\n";
			
			// overlord taxes
			fiefText += "Overlord taxes: " + fief.keyStatsCurrent[11] + "\r\n";
			// overlord tax rate
			fiefText += "   (tax rate: " + fief.keyStatsCurrent[12] + "%)\r\n\r\n";
			
			// surplus
			fiefText += "Bottom line: " + fief.keyStatsCurrent[13];
		}
		
		return fiefText;
	}

	/// <summary>
	/// Retrieves next season's key information for Fief display screen
	/// </summary>
	/// <returns>String containing information to display</returns>
	public string DisplayFiefKeyStatsNext(ProtoFief fief)
	{
		Debug.Log ("Adjusting stats next");
		string fiefText = "NEXT SEASON (ESTIMATE)\r\n========================\r\n\r\n";
		
		// if under siege, don't display data
		if (fief.keyStatsNext==null)
		{
			fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";

		}
		
		// if NOT under siege
		else
		{
			Debug.Log("Next tax rate: "+fief.keyStatsNext[2]);
			// loyalty
			fiefText += "Loyalty: " + fief.keyStatsNext[0] + "\r\n";
			
			// GDP
			fiefText += "GDP: " + fief.keyStatsNext[1] + "\r\n\r\n";
			
			// tax rate
			fiefText += "Tax rate: " + fief.keyStatsNext[2] + "%\r\n\r\n";
			
			// officials expenditure
			fiefText += "Officials expenditure: " + fief.keyStatsNext[3] + "\r\n\r\n";
			
			// Garrison expenditure
			fiefText += "Garrison expenditure: " + fief.keyStatsNext[4] + "\r\n\r\n";
			
			// Infrastructure expenditure
			fiefText += "Infrastructure expenditure: " + fief.keyStatsNext[5] + "\r\n\r\n";
			
			// keep expenditure
			fiefText += "Keep expenditure: " + fief.keyStatsNext[6] + "\r\n";
			// keep level
			fiefText += "   (keep level: " + fief.keyStatsNext[7]+ ")\r\n\r\n";
			
			// income
			fiefText += "Income: " + fief.keyStatsNext[8] + "\r\n";
			
			// family expenses
			fiefText += "Family expenses  (may include a famExpense modifier): " + fief.keyStatsNext[9];
			fiefText += "\r\n\r\n";
			
			// total expenses (fief and family)
			fiefText += "Total fief expenses  (may include expenses modifiers): " + fief.keyStatsNext[10];
			fiefText += "\r\n\r\n";
			
			// overlord taxes
			fiefText += "Overlord taxes: " + fief.keyStatsNext[11] + "\r\n";
			// overlord tax rate
			fiefText += "   (tax rate: " + fief.keyStatsNext[12] + "%)\r\n\r\n";
			
			// bottom line
			fiefText += "Bottom line: " + fief.keyStatsNext[13];
		}
		
		return fiefText;
	}

	public void getPotentialBailiffs() {
		ShowBailiffList ();
		ProtoMessage getBailiffs = new ProtoMessage();
		getBailiffs.Message="Grant";
		getBailiffs.ActionType=Actions.GetNPCList;
		getBailiffs.MessageFields =new String[] {"bailiff"};
		NetworkScript.Send (getBailiffs);

	}

	public static void appointBailiff(string id) {
		ProtoMessage appoint = new ProtoMessage();
		appoint.ActionType=Actions.AppointBailiff;
		appoint.Message= currentViewedFief;
		appoint.MessageFields=new string[]{id};
		NetworkScript.Send (appoint);
	}

	public void AppointRemoveBailiff() {
		if(currentFiefDetails.bailiff==null) {
			getPotentialBailiffs();
		}
		else {
			ProtoMessage appoint = new ProtoMessage();
			appoint.ActionType=Actions.RemoveBailiff;
			appoint.Message=currentFiefDetails.fiefID;
			NetworkScript.Send (appoint);
		}
	}

	public void ViewBailiff() {
		if(currentFiefDetails.bailiff!=null) {
			//GameStateManager.gameState.preLoadState="FamilyEmploy";
			GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.RequestHouseholdList ("FamilyEmploy"));
			GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>HouseholdManager.viewCharacter(currentFiefDetails.bailiff.charID));
			Application.LoadLevel ("Household");
		}
		else {
			GameStateManager.gameState.DisplayMessage ("This fief has no bailiff sir!");
		}
	}
		
	public void TransferToHome() {
		ProtoTransfer transfer = new ProtoTransfer();
		int amount = 0;
		try {
			amount = Convert.ToInt32(GameObject.Find ("EnteredAmount").GetComponent<Text>().text);
		}
		catch (Exception e){
			GameStateManager.gameState.DisplayMessage ("Sorry, but this is not a number");
			return;
		}
		transfer.amount = amount;
		transfer.fiefFrom = currentFiefDetails.fiefID;
		transfer.ActionType=Actions.TransferFunds;
		NetworkScript.Send (transfer);
	}

	public void TransferFromHome() {
		ProtoTransfer transfer = new ProtoTransfer();
		int amount = 0;
		try {
			amount = Convert.ToInt32(GameObject.Find ("EnteredAmount").GetComponent<Text>().text);
		}
		catch (Exception e){
			GameStateManager.gameState.DisplayMessage ("Sorry, but this is not a number");
			return;
		}
		transfer.amount = amount;
		transfer.fiefTo = currentFiefDetails.fiefID;
		transfer.ActionType=Actions.TransferFunds;
		NetworkScript.Send (transfer);
	}

	public void VisitGaol() {
		ProtoMessage listCaptives =new ProtoMessage();
		listCaptives.ActionType=Actions.ViewCaptives;
		listCaptives.Message=currentFiefDetails.fiefID;
		NetworkScript.Send (listCaptives);
	}
	public void GetPlayerList() {
		ProtoMessage requestPlayers =new ProtoMessage();
		requestPlayers.ActionType=Actions.GetPlayers;
		NetworkScript.Send (requestPlayers);
	}

	public void ShowPlayers(ProtoPlayer[] players) {
		ShowPlayerTransfer(true);
		// Destroy previous list items
		List<GameObject> toDestroy = new List<GameObject>();
		foreach(Transform transform in this.gameObject.transform.Find ("TransferPlayer").Find("ScrollPanel").Find ("PanelContents")) {
			toDestroy.Add (transform.gameObject);
		}
		foreach(GameObject item in toDestroy) {
			Destroy (item);
		}
		var playerDetails = Resources.Load<GameObject> ("PlayerOverview");
		foreach(ProtoPlayer player in players) {
			var row = Instantiate (playerDetails);
			row.name=player.pcID;
			row.transform.SetParent (this.gameObject.transform.Find ("TransferPlayer").Find("ScrollPanel").Find ("PanelContents"));
			row.transform.Find ("PlayerID").GetComponent<Text>().text = player.playerID;
			row.transform.Find ("PlayerChar").GetComponent<Text>().text = player.pcName;
			row.transform.Find ("PlayerNat").GetComponent<Text>().text = Globals_Game.nationalityMasterList[player.natID].name;
			row.GetComponent<Button>().onClick.AddListener (()=>{this.selectedPlayer=row.name;});
		}
	}

	public void ShowCaptives(ProtoCharacterOverview[] captives) {
		ShowCaptiveControls();
		var captiveOverview = Resources.Load<GameObject>("CaptiveOverview");
		foreach(ProtoCharacterOverview captive in captives) {
			var row = Instantiate (captiveOverview);
			row.name=captive.charID;
			row.transform.SetParent (this.gameObject.transform.Find ("Gaol").Find("ScrollPanel").Find ("PanelContents"),false);

			row.transform.Find ("charName").GetComponent<Text>().text = captive.charName;
			if(captive.isMale) {
				row.transform.Find ("charGender").GetComponent<Text>().text = "Male";
			}
			else {
				row.transform.Find ("charGender").GetComponent<Text>().text = "Female";
			}
			row.transform.Find ("charOwner").GetComponent<Text>().text = captive.owner;
			row.GetComponent<Button>().onClick.AddListener (()=>GetCaptive(row.name));
			row.GetComponent<RectTransform>().sizeDelta=new Vector2(row.transform.parent.parent.GetComponent<RectTransform>().rect.width,row.GetComponent<RectTransform>().sizeDelta.y);
			row.GetComponent<LayoutElement>().preferredWidth=row.transform.parent.parent.GetComponent<RectTransform>().rect.width;
			row.GetComponent<BoxCollider2D>().size=new Vector2(row.GetComponent<LayoutElement>().preferredWidth,row.GetComponent<RectTransform>().sizeDelta.y);
		}
	}

	public void GetCaptive(string captiveID) {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.ViewCaptive;
		message.Message=captiveID;
		NetworkScript.Send (message);
	}

	public void ReleaseCaptive() {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.ReleaseCaptive;
		message.Message=selectedCaptive.charID;
		NetworkScript.Send (message);
	}
	public void ExecuteCaptive() {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.ExecuteCaptive;
		message.Message=selectedCaptive.charID;
		NetworkScript.Send (message);
	}
	public void RansomCaptive() {
		ProtoMessage message = new ProtoMessage();
		message.ActionType=Actions.RansomCaptive;
		message.Message=selectedCaptive.charID;
		NetworkScript.Send (message);
	}
	public void ShowCaptive(ProtoCharacter captive) {
		Debug.Log ("Showing captive controls");
		this.selectedCaptive=captive;
		itemDetails.transform.Find ("ItemDetails").GetComponent<Text>().text= HouseholdManager.CharacterText (captive);
		ShowCaptiveControls();
	}
	public void TransferToPlayer() {
		ProtoTransferPlayer transfer = new ProtoTransferPlayer();
		if(selectedPlayer==null) {
			GameStateManager.gameState.DisplayMessage ("Please select a player");
			return;
		}
		transfer.playerTo=selectedPlayer;
		try {
			transfer.amount = Convert.ToInt32 (this.gameObject.transform.Find ("TransferPlayer").Find("Amount").GetComponent<InputField>().text );
			if(transfer.amount<=0) {
				GameStateManager.gameState.DisplayMessage("You must enter a positive number to transfer");
			}
			else {
				transfer.ActionType=Actions.TransferFundsToPlayer;
				NetworkScript.Send (transfer);
				ShowPlayerTransfer (false);
			}
		}
		catch(Exception e) {
			GameStateManager.gameState.DisplayMessage("Please enter a number");
		}
	}
	public void adjustExpenditure() {
		try {
			double newTax = (double)taxSlider.value;
			double newOff = Convert.ToDouble (officialSpend.text);
			double newGarr = Convert.ToDouble (garrisonSpend.text);
			double newKeep = Convert.ToDouble (keepSpend.text);
			double newInfra = Convert.ToDouble (infraSpend.text);
			ProtoGenericArray<double> newExpenses = new ProtoGenericArray<double>();
			newExpenses.Message=currentViewedFief;
			newExpenses.fields=new double[] {newTax,newOff,newGarr,newInfra,newKeep};
			Debug.Log ("attempting to set expenses: "+newTax.ToString()+","+newOff.ToString ()+","+newGarr.ToString ()+","+newInfra+newKeep.ToString ());
			newExpenses.ActionType=Actions.AdjustExpenditure;
			NetworkScript.Send (newExpenses);
		}
		catch(Exception e) {
			
			GameStateManager.gameState.DisplayMessage ("Please ensure all expenditures are valid numbers");
			return;
		}

	}

	public void displayFiefs(ProtoFief[] fiefs) 
	{
		if(fiefs==null||fiefs.Length==0) {
			GameStateManager.gameState.DisplayMessage ("You own no fiefs!");
			return;
		}
		ShowFiefList();
		var row = Resources.Load <GameObject>("FiefDetails");
		foreach(ProtoFief fief in fiefs) {
			var fiefDisplay = Instantiate(row);

			fiefDisplay.name = fief.fiefID;
			try {
				Fief localFief = Globals_Game.fiefMasterList[fief.fiefID];
				fiefDisplay.transform.Find ("Fief").GetComponent<Text>().text=localFief.name;
				fiefDisplay.transform.Find ("Province").GetComponent<Text>().text=localFief.province.name;
				fiefDisplay.GetComponent<Button>().onClick.AddListener(()=>{this.selectedFief=fiefDisplay.name;});
				fiefDisplay.transform.SetParent (fiefList.transform.Find ("ScrollPanel").Find ("PanelContents"),false);
				/*
				fiefDisplay.GetComponent<RectTransform>().sizeDelta=new Vector2(fiefDisplay.transform.parent.parent.GetComponent<RectTransform>().rect.width,20);
				fiefDisplay.GetComponent<LayoutElement>().preferredWidth=fiefDisplay.transform.parent.parent.GetComponent<RectTransform>().rect.width;
				fiefDisplay.GetComponent<BoxCollider2D>().size=new Vector2(fiefDisplay.GetComponent<LayoutElement>().preferredWidth,20);*/
				fiefDisplay.GetComponent<RectTransform>().sizeDelta=new Vector2(fiefDisplay.transform.parent.parent.GetComponent<RectTransform>().rect.width,fiefDisplay.GetComponent<RectTransform>().sizeDelta.y);
				fiefDisplay.GetComponent<LayoutElement>().preferredWidth=fiefDisplay.transform.parent.parent.GetComponent<RectTransform>().rect.width;
				fiefDisplay.GetComponent<BoxCollider2D>().size=new Vector2(fiefDisplay.GetComponent<LayoutElement>().preferredWidth,fiefDisplay.GetComponent<RectTransform>().sizeDelta.y);
			}
			catch(Exception e) {
				Debug.LogError ("Could not recognise fief: "+fief.fiefID);
			}
		}
		GameObject.Find ("ViewFief").GetComponent<Button>().onClick.AddListener (()=>GetFief (selectedFief));


	}
	public static void GetFiefList() {
		ProtoMessage getFiefs = new ProtoMessage();
		getFiefs.ActionType= Actions.ViewMyFiefs;
		NetworkScript.Send (getFiefs);
	}
	public void autoAdjustExpenditure() {
		ProtoMessage adjust = new ProtoMessage();
		adjust.Message=currentViewedFief;
		adjust.ActionType=Actions.AdjustExpenditure;
		NetworkScript.Send (adjust);
	}

	public void ShowBarredItems() {
		ShowBarredDetails();
		BarredItemsCharacters();
		BarredItemsNationalities();

	}

	public void BarredItemsNationalities() {
		// Clear current list
		List<GameObject> currentChildren = new List<GameObject>();
		foreach(Transform child in BarredItems.transform.Find ("Nationalities").Find ("PanelContents")) {
			currentChildren.Add(child.gameObject);
		}
		foreach(GameObject child in currentChildren) {
			Destroy (child);
		}
		var nationalityRow = Resources.Load<GameObject> ("BarNationality");
		foreach(Nationality nat in Globals_Game.nationalityMasterList.Values) {
			var nationality = Instantiate (nationalityRow);
			nationality.name = nat.natID;
			nationality.transform.Find ("Nationality").GetComponent<Text>().text = nat.name;
			Toggle barredToggle = nationality.transform.Find ("Toggle").GetComponent<Toggle>();
			if(currentFiefDetails.barredNationalities!=null&&currentFiefDetails.barredNationalities.Length!=null && Array.Exists (currentFiefDetails.barredNationalities,element => element.Equals ( nat.natID))) {
				barredToggle.isOn=true;
			}
			else {
				barredToggle.isOn=false;
			}
			Debug.Log ("Item set to nationality "+nat.name);
			barredToggle.onValueChanged.AddListener ((value)=> {BarUnbarNationality(nationality.name, value);});
			nationality.transform.SetParent (BarredItems.transform.Find ("Nationalities").Find ("PanelContents"),false);
			nationality.GetComponent<RectTransform>().sizeDelta=new Vector2(nationality.transform.parent.parent.GetComponent<RectTransform>().rect.width,nationality.GetComponent<RectTransform>().sizeDelta.y);
			nationality.GetComponent<LayoutElement>().preferredWidth=nationality.transform.parent.parent.GetComponent<RectTransform>().rect.width;

		}
	}

	public void BarredItemsCharacters() {
		// Clear current list
		List<GameObject> currentChildren = new List<GameObject>();
		foreach(Transform child in BarredItems.transform.Find ("Characters").Find ("PanelContents")) {
			currentChildren.Add(child.gameObject);
		}
		foreach(GameObject child in currentChildren) {
			Destroy (child);
		}
		var barCharacterRow = Resources.Load<GameObject>("BarredCharacter");
		if(currentFiefDetails.barredCharacters==null||currentFiefDetails.barredCharacters.Length==0) {
			return;
		}
		foreach(ProtoCharacterOverview character in currentFiefDetails.barredCharacters) {
			var charDetails = Instantiate (barCharacterRow);
			charDetails.name = character.charID;
			charDetails.transform.Find ("Name").GetComponent<Text>().text= character.charName;
			charDetails.transform.Find ("Nationality").GetComponent<Text>().text = Globals_Game.nationalityMasterList[character.natID].name;
			charDetails.GetComponent<Button>().onClick.AddListener (()=>  {selectedBarredCharacter=character;});
			charDetails.transform.SetParent (BarredItems.transform.Find ("Characters").Find ("PanelContents"),false);
			charDetails.GetComponent<RectTransform>().sizeDelta=new Vector2(charDetails.transform.parent.parent.GetComponent<RectTransform>().rect.width,charDetails.GetComponent<RectTransform>().sizeDelta.y);
			charDetails.GetComponent<BoxCollider2D>().size=new Vector2(charDetails.GetComponent<RectTransform>().sizeDelta.x,charDetails.GetComponent<RectTransform>().sizeDelta.y);
		}
	}

	public void BarCharacter() {
		var characterInputField = BarredItems.transform.Find ("CharacterID").GetComponent<InputField>();
		string charID = characterInputField.text;
		if(string.IsNullOrEmpty (charID)) {
			GameStateManager.gameState.DisplayMessage ("Please enter a character id");
			return;
		}
		ProtoMessage bar = new ProtoMessage();
		bar.Message = currentFiefDetails.fiefID;
		bar.MessageFields=new string[] {charID};
		bar.ActionType = Actions.BarCharacters;
		NetworkScript.Send (bar);
	}

	public void UnbarCharacter() {
		if(selectedBarredCharacter ==null) {
			GameStateManager.gameState.DisplayMessage ("You must select a character first");
		}
		else {
			ProtoMessage unbarMessage = new ProtoMessage();
			unbarMessage.ActionType=Actions.UnbarCharacters;
			unbarMessage.Message = currentFiefDetails.fiefID;
			unbarMessage.MessageFields=new string[] {selectedBarredCharacter.charID};
			NetworkScript.Send (unbarMessage);
		}
	}

	public void BarUnbarNationality(string natID, bool bar) {
		if(string.IsNullOrEmpty (natID)) {
			GameStateManager.gameState.DisplayMessage ("You must select a nationality first");
		}
		else {
			ProtoMessage barMessage = new ProtoMessage();
			barMessage.Message=currentFiefDetails.fiefID;
			barMessage.MessageFields=new string[]{natID};
			if(bar) {
				Debug.Log("Attempting to bar: "+natID);
				barMessage.ActionType=Actions.BarNationalities;
			}
			else {
				Debug.Log("Attempting to unbar: "+natID);
				barMessage.ActionType=Actions.UnbarNationalities;
			}
			NetworkScript.Send (barMessage);
		}
	}

	public void PillageFief() {
		if(currentFiefDetails.ownerID.Equals (Globals_Client.pcID)) {
			GameStateManager.gameState.DisplayMessage ("It would be most unwise to pillage your own fief!");
		}
		else if(string.IsNullOrEmpty (Globals_Client.activeCharacter.armyID)) {
			GameStateManager.gameState.DisplayMessage ("You must be leading an army to do this");
		}
		else {
			ProtoMessage pillage = new ProtoMessage();
			pillage.ActionType=Actions.PillageFief;
			pillage.Message = Globals_Client.activeCharacter.armyID;
			NetworkScript.Send (pillage);
		}
	}
	public void ReturnToFiefView() {
		DisplayFief();
	}

	public void SpyOn() {
		ProtoMessage spy = new ProtoMessage();
		spy.ActionType=Actions.SpyFief;
		if(currentFiefDetails==null) {
			GameStateManager.gameState.DisplayMessage ("You must choose a fief to spy on");
			return;
		}
		spy.Message=Globals_Client.activeCharacter.charID;
		spy.MessageFields=new string[]{currentFiefDetails.fiefID};
		NetworkScript.Send (spy);
	}

	public void StartSiege() {
		GameStateManager.gameState.SceneLoadQueue.Enqueue(()=>ArmyManager.Besiege(Globals_Client.activeCharacter.armyID));
		GameStateManager.gameState.SceneLoadQueue.Enqueue (()=>ArmyManager.RequestSieges());
		Application.LoadLevel ("Combat");
	}

	public void ExamineArmies() {
		ProtoMessage message = new ProtoMessage();
		message.Message = Globals_Client.currentLocation.id;
		message.ActionType=Actions.ExamineArmiesInFief;
		NetworkScript.Send (message);
	}
}
