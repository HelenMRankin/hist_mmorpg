﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hist_mmorpg
{
    /// <summary>
    /// User interface component for selecting and passing data back to Form1
    /// </summary>
    public partial class SelectionForm : Form
    {
        /// <summary>
        /// Holds parent form, allowing access to its public variables
        /// </summary>
        public Form1 parent;
        /// <summary>
        /// Holds current function for the form
        /// </summary>
        public string function { get; set; }
        /// <summary>
        /// Holds armyID (if appointing an army leader)
        /// </summary>
        public string armyID { get; set; }
        /// <summary>
        /// Holds observer (if examining armies in fief)
        /// </summary>
        public Character observer { get; set; }
        /// <summary>
        /// Holds place id (if granting place title)
        /// </summary>
        public string placeID { get; set; }
        /// <summary>
        /// Holds position id (if bestowing a position)
        /// </summary>
        public byte positionID { get; set; }

        /// <summary>
        /// Constructor for SelectionForm
        /// </summary>
        /// <param name="par">Parent Form1 object</param>
        /// <param name="funct">String indicating function to be performed</param>
        /// <param name="armID">String indicating ID of army (if choosing leader)</param>
        /// <param name="observer">Observer (if examining armies in fief)</param>
        public SelectionForm(Form1 par, String funct, String armID = null, Character obs = null, String place = null, byte posID = 0)
        {
            // initialise form elements
            InitializeComponent();

            this.parent = par;
            this.function = funct;
            this.armyID = armID;
            this.observer = obs;
            this.placeID = place;
            this.positionID = posID;

            // initialise NPC display
            this.initDisplay();
        }

        /// <summary>
        /// Initialises display screen
        /// </summary>
        private void initDisplay()
        {
            if (((this.function.Equals("bailiff")) || (this.function.Equals("leader")))
                || (this.function.Equals("titleHolder")) || (this.function.Contains("royalGift")))
            {
                // format list display
                this.SetUpNpcList();

                // refresh information 
                this.RefreshNPCdisplay();

                // show container
                this.npcContainer.BringToFront();
            }

            if (this.function.Equals("lockout"))
            {
                // format list display
                this.SetUpBarredList();

                // refresh information 
                this.RefreshBarredDisplay();

                // show container
                this.lockOutContainer.BringToFront();
            }

            if (this.function.Equals("transferTroops"))
            {
                // format list display
                this.SetUpTransferList();

                // refresh information 
                this.RefreshTransferDisplay();

                // show container
                this.transferContainer.BringToFront();
            }

            if (this.function.Equals("transferFunds"))
            {
                // format list display
                this.SetUpTransferFundsList();

                // refresh information 
                this.RefreshTransferFundsDisplay();

                // show container
                this.transferFundsContainer.BringToFront();
            }

            if (this.function.Equals("armies"))
            {
                // format list display
                this.SetUpArmiesList();

                // refresh information 
                this.RefreshArmiesDisplay();

                // show container
                this.armiesContainer.BringToFront();
            }
        }

        /// <summary>
        /// Configures UI display for list of household NPCs
        /// </summary>
        public void SetUpNpcList()
        {
            // add necessary columns
            this.npcListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("Character ID", -2, HorizontalAlignment.Left);

            switch (this.function)
            {
                case "bailiff":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Appoint This Person As Bailiff";
                    break;
                case "leader":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Appoint This Person As Leader";
                    break;
                case "titleHolder":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Grant Title To This Person";
                    break;
                case "royalGiftTitle":
                    // add necessary columns
                    this.npcListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
                    this.npcListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Grant Title To This Person";
                    break;
                case "royalGiftFief":
                    // add necessary columns
                    this.npcListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
                    this.npcListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Gift Fief To This Person";
                    break;
                case "royalGiftPosition":
                    // add necessary columns
                    this.npcListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
                    this.npcListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Bestow Position Upon This Person";
                    break;
                default:
                    break;
            }

            // disable button (until NPC selected)
            this.chooseNpcBtn.Enabled = false;
        }

        /// <summary>
        /// Configures UI display for list of players
        /// </summary>
        public void SetUpTransferFundsList()
        {
            // add necessary columns
            this.transferFundsListView.Columns.Add("PC Name", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("PC ID", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);

            // disable controls (until PC selected)
            this.transferFundsBtn.Enabled = false;
            this.transferFundsTextBox.Enabled = false;
        }

        /// <summary>
        /// Refreshes NPC list
        /// </summary>
        public void RefreshNPCdisplay()
        {
            bool addItem = true;
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();

            // remove any previously displayed characters
            this.npcDetailsTextBox.ReadOnly = true;
            this.npcDetailsTextBox.Text = "";

            // clear existing items in list
            this.npcListView.Items.Clear();

            ListViewItem myCharItem = null;

            // for royal gifts, ITERATE THROUGH PLAYERS
            if (this.function.Contains("royalGift"))
            {
                foreach (KeyValuePair<string, PlayerCharacter> thisPlayer in Globals_Game.pcMasterList)
                {
                    myCharItem = null;

                    if (thisPlayer.Value.ChecksBeforeGranting(this.function, true))
                    {
                        addItem = true;

                        // Create an item and subitems for each character
                        // name
                        myCharItem = new ListViewItem(thisPlayer.Value.firstName + " " + thisPlayer.Value.familyName);

                        // PC ID
                        myCharItem.SubItems.Add(thisPlayer.Value.charID);

                        // player ID
                        myCharItem.SubItems.Add(thisPlayer.Value.playerID);

                        // nationality
                        myCharItem.SubItems.Add(thisPlayer.Value.nationality.name);
                    }

                    // check for null items
					if (myCharItem == null)
					{
						addItem = false;
					}

					if (addItem)
					{
						itemsToAdd.Add(myCharItem);
					}
                }
            }

            // else ITERATE THROUGH EMPLOYEES/FAMILY
            else
            {
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    myCharItem = null;

                    if (Globals_Client.myPlayerCharacter.myNPCs[i].ChecksBeforeGranting(this.function, true, this.armyID))
                    {
                        addItem = true;

                        // Create an item and subitems for each character
                        // name
                        myCharItem = new ListViewItem(Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName);

                        // charID
                        myCharItem.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].charID);

                        // location
                        myCharItem.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].location.id);
                    }

					// check for null items
					if (myCharItem == null)
					{
						addItem = false;
					}

					if (addItem)
					{
						// add item to temporary list
						itemsToAdd.Add(myCharItem);
					}
                }
            }

			// add items to ListView
			if (itemsToAdd.Count > 0)
			{
				this.npcListView.Items.AddRange(itemsToAdd.ToArray());
			}
        }

        /// <summary>
        /// Refreshes transfer funds PC list
        /// </summary>
        public void RefreshTransferFundsDisplay()
        {
            // remove any previously displayed characters
            this.transferFundsTextBox.Text = "";
            this.transferFundsLabel.Text = "";

            // clear existing items in list
            this.transferFundsListView.Items.Clear();

            ListViewItem myPlayerItem = null;

            // iterate through players
            foreach (KeyValuePair<string, PlayerCharacter> thisPlayer in Globals_Game.pcMasterList)
            {
                // only show 'played' PCs
                if (!String.IsNullOrWhiteSpace(thisPlayer.Value.playerID))
                {
                    // don't show this player
                    if (thisPlayer.Value != Globals_Client.myPlayerCharacter)
                    {
                        // Create an item and subitems for each character

                        // name
                        myPlayerItem = new ListViewItem(thisPlayer.Value.firstName + " " + thisPlayer.Value.familyName);

                        // PC ID
                        myPlayerItem.SubItems.Add(thisPlayer.Value.charID);

                        // player ID
                        myPlayerItem.SubItems.Add(thisPlayer.Value.playerID);

                        // nationality
                        myPlayerItem.SubItems.Add(thisPlayer.Value.nationality.name);

                        // add item to list
                        this.transferFundsListView.Items.Add(myPlayerItem);
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the npcListView
        /// which then displays the selected NPC's details in the npcDetailsTextBox
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void npcListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Character charToDisplay = null;

            string textToDisplay = "";

            if (npcListView.SelectedItems.Count > 0)
            {
                // set textbox to read only
                this.npcDetailsTextBox.ReadOnly = true;

                // get character
                if (this.function.Contains("royalGift"))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Game.pcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }
                else
                {
                    if (Globals_Game.npcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Game.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }

                if (charToDisplay != null)
                {
                    // enable 'appoint this NPC' button
                    this.chooseNpcBtn.Enabled = true;

                    // get details
                    textToDisplay += this.DisplayCharacter(charToDisplay);

                    // display details
                    this.npcDetailsTextBox.Text = textToDisplay;
                }
            }
        }

        /// <summary>
        /// Retrieves army details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="a">Army whose information is to be displayed</param>
        public string DisplayArmy(Army a)
        {
            bool isMyArmy = false;
            uint[] troopNumbers = new uint[7];
            uint totalTroops = 0;
            string armyText = "";

            // get owner
            PlayerCharacter thisOwner = a.GetOwner();

            // ID
            armyText += "ID: " + a.armyID + "\r\n\r\n";

            // nationality
            armyText += "Nationality: " + thisOwner.nationality.name + "\r\n\r\n";

            // owner
            armyText += "Owner: " + thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")\r\n\r\n";

            // check if is your army (will effect display of troop numbers)
            if (thisOwner == Globals_Client.myPlayerCharacter)
            {
                isMyArmy = true;
            }

            // leader
            Character armyLeader = a.GetLeader();
            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!\r\n\r\n";
            }
            else
            {
                armyText += armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")";
            }
            armyText += "\r\n\r\n";

            // get troop numbers
            if (isMyArmy)
            {
                // actual troop numbers if is player's army
                troopNumbers = a.troops;
            }
            else
            {
                // estimated troop numbers if is NOT player's army
                troopNumbers = a.GetTroopsEstimate(observer);
            }

            armyText += "Troop numbers";
            if (!isMyArmy)
            {
                armyText += " (ESTIMATE)";
            }
            armyText += ":\r\n";

            // labels for troop types
            string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Longbowmen: ", " - Crossbowmen: ", " - Foot: ", " - Rabble: " };

            // display numbers for each troop type
            for (int i = 0; i < troopNumbers.Length; i++ )
            {
                armyText += troopTypeLabels[i] + troopNumbers[i];
                totalTroops += troopNumbers[i];
                armyText += "\r\n";
            }

            // display total
            armyText += "   ==================\r\n";
            armyText += " - TOTAL: " + totalTroops + "\r\n\r\n";

            return armyText;
        }

        /// <summary>
        /// Retrieves character details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="thisChar">Character whose information is to be displayed</param>
        public string DisplayCharacter(Character thisChar)
        {
            string charText = "";

            // ID
            charText += "ID: " + thisChar.charID + "\r\n";

            // name
            charText += "Name: " + thisChar.firstName + " " + thisChar.familyName + "\r\n";

            // age
            charText += "Age: " + thisChar.CalcAge() + "\r\n";

            // nationality
            charText += "Nationality: " + thisChar.nationality + "\r\n";

            // health (& max. health)
            charText += "Health: ";
            if (!thisChar.isAlive)
            {
                charText += "Blimey, you're Dead!";
            }
            else
            {
                charText += thisChar.CalculateHealth() + " (max. health: " + thisChar.maxHealth + ")";
            }
            charText += "\r\n";

            // any death modifiers (from traits)
            charText += "  (Death modifier from traits: " + thisChar.CalcTraitEffect("death") + ")\r\n";

            // location
            charText += "Current location: " + thisChar.location.name + " (" + thisChar.location.province.name + ")\r\n";

            // if in process of auto-moving, display next hex
            if (thisChar.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + thisChar.goTo.Peek().id + "\r\n";
            }

            // language
            charText += "Language: " + thisChar.language + "\r\n";

            // days left
            charText += "Days remaining: " + thisChar.days + "\r\n";

            // stature
            charText += "Stature: " + thisChar.CalculateStature() + "\r\n";
            charText += "  (base stature: " + thisChar.CalculateStature(false) + " | modifier: " + thisChar.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + thisChar.management + "\r\n";

            // combat rating
            charText += "Combat: " + thisChar.combat + "\r\n";

            // traits list
            charText += "Traits:\r\n";
            for (int i = 0; i < thisChar.traits.Length; i++)
            {
                charText += "  - " + thisChar.traits[i].Item1.name + " (level " + thisChar.traits[i].Item2 + ")\r\n";
            }

            // gather additional NPC-specific information
            if (thisChar is NonPlayerCharacter)
            {
                charText += this.DisplayNonPlayerCharacter(thisChar as NonPlayerCharacter);
            }

            return charText;
        }

        /// <summary>
        /// Retrieves NonPlayerCharacter-specific details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string DisplayNonPlayerCharacter(NonPlayerCharacter npc)
        {
            string npcText = "";

            // current salary
            npcText += "Current salary: " + npc.salary + "\r\n";

            return npcText;
        }

        /// <summary>
        /// Responds to the click event of the chooseNpcBtn button, assigning a character to
        /// the selected function (various)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void chooseNpcBtn_Click(object sender, EventArgs e)
        {
            if (npcListView.SelectedItems.Count > 0)
            {
                bool refreshMenus = false;

                // if royal gift, get selected PlayerCharacter
                Character selectedCharacter = null;
                if (this.function.Contains("royalGift"))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        selectedCharacter = Globals_Game.pcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }

                // if not a royal gift, get an NPC
                else
                {
                    if (Globals_Game.npcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        selectedCharacter = Globals_Game.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }

                // get place or position (if royal gift)
                Place thisPlace = null;
                Position thisPosition = null;
                if (this.function.Contains("royalGift"))
                {
                    // if gifting place or place title
                    if (!this.function.Equals("royalGiftPosition"))
                    {
                        // get place type and id
                        string[] placeDetails = this.placeID.Split('|');

                        // get place associated with title
                        if (placeDetails[0].Equals("province"))
                        {
                            if (Globals_Game.provinceMasterList.ContainsKey(placeDetails[1]))
                            {
                                thisPlace = Globals_Game.provinceMasterList[placeDetails[1]];
                            }
                        }
                        else if (placeDetails[0].Equals("fief"))
                        {
                            if (Globals_Game.fiefMasterList.ContainsKey(placeDetails[1]))
                            {
                                thisPlace = Globals_Game.fiefMasterList[placeDetails[1]];
                            }
                        }
                    }

                    // if gifting a position
                    else
                    {
                        if (Globals_Game.positionMasterList.ContainsKey(this.positionID))
                        {
                            thisPosition = Globals_Game.positionMasterList[this.positionID];
                        }
                    }
                }

                // appoint character to position
                // if appointing a bailiff
                if (this.function.Equals("bailiff"))
                {
                    if (selectedCharacter != null)
                    {
                        if (selectedCharacter.ChecksBeforeGranting(this.function, false, this.armyID))
                        {
                            // set the selected NPC as bailiff
                            Globals_Client.fiefToView.bailiff = selectedCharacter;

                            // refresh the fief information (in the main form)
                            this.parent.RefreshFiefContainer(Globals_Client.fiefToView);
                        }
                    }
                }

                // if making a royal gift (title)
                else if (this.function.Equals("royalGiftTitle"))
                {
                    if (thisPlace != null)
                    {
                        if (selectedCharacter != null)
                        {
                            if (selectedCharacter.ChecksBeforeGranting(this.function, false))
                            {
                                // set the selected NPC as title holder
                                refreshMenus = Globals_Client.myPlayerCharacter.GrantTitle(selectedCharacter, thisPlace);

                                // check if menus need to be refreshed (due to ownership changes)
                                if ((thisPlace is Province) && (refreshMenus))
                                {
                                    this.parent.InitMenuPermissions();
                                }

                                // refresh the fief information (in the main form)
                                this.parent.RefreshCurrentScreen();
                            }
                        }
                    }
                }

                // if making a royal gift (fief)
                else if (this.function.Equals("royalGiftFief"))
                {
                    if (thisPlace != null)
                    {
                        if (selectedCharacter != null)
                        {
                            if (selectedCharacter.ChecksBeforeGranting(this.function, false))
                            {
                                // process the change of ownership
                                (thisPlace as Fief).ChangeOwnership((selectedCharacter as PlayerCharacter), "voluntary");

                                // refresh the fief information (in the main form)
                                this.parent.RefreshCurrentScreen();
                            }
                        }
                    }
                }

                // if making a royal gift (position)
                else if (this.function.Equals("royalGiftPosition"))
                {
                    if (thisPosition != null)
                    {
                        if (selectedCharacter != null)
                        {
                            if (selectedCharacter.ChecksBeforeGranting(this.function, false))
                            {
                                // process the change of officeHolder
                                thisPosition.BestowPosition(selectedCharacter as PlayerCharacter);

                                // refresh the fief information (in the main form)
                                this.parent.RefreshCurrentScreen();
                            }
                        }
                    }
                }

                // if appointing fief title holder
                else if (this.function.Equals("titleHolder"))
                {
                    if (selectedCharacter != null)
                    {
                        if (selectedCharacter.ChecksBeforeGranting(this.function, false))
                        {
                            // set the selected NPC as title holder
                            Globals_Client.myPlayerCharacter.GrantTitle(selectedCharacter, Globals_Client.fiefToView);

                            // refresh the fief information (in the main form)
                            this.parent.RefreshCurrentScreen();
                        }
                    }
                }

                // if appointing an army leader
                else
                {
                    if (selectedCharacter != null)
                    {
                        if (selectedCharacter.ChecksBeforeGranting(this.function, false, this.armyID))
                        {
                            // get army
                            if (!String.IsNullOrWhiteSpace(this.armyID))
                            {
                                if (Globals_Game.armyMasterList.ContainsKey(this.armyID))
                                {
                                    Army thisArmy = Globals_Game.armyMasterList[this.armyID];

                                    thisArmy.AssignNewLeader(selectedCharacter);

                                    // refresh the army information (in the main form)
                                    this.parent.RefreshArmyContainer(thisArmy);
                                }
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("No army specified.");
                                }
                            }
                        }
                    }
                }
            }

            // close this form
            this.Close();
        }

        /// <summary>
        /// Configures UI display for list of barred characters
        /// </summary>
        public void SetUpBarredList()
        {
            // add necessary columns (barred characters)
            this.barredCharsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.barredCharsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.barredCharsListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);

            // add necessary columns (nationalities)
            this.barredNatsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.barredNatsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes barred characters list
        /// </summary>
        public void RefreshBarredDisplay()
        {
            // clear existing items in lists
            this.barredCharsListView.Items.Clear();
            this.barredNatsListView.Items.Clear();

            // CHARACTERS
            // iterates through employees
            for (int i = 0; i < Globals_Client.fiefToView.barredCharacters.Count; i++)
            {
                // retrieve character
                Character myBarredChar = null;

                if (Globals_Game.pcMasterList.ContainsKey(Globals_Client.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = Globals_Game.pcMasterList[Globals_Client.fiefToView.barredCharacters[i]];
                }
                else if (Globals_Game.npcMasterList.ContainsKey(Globals_Client.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = Globals_Game.npcMasterList[Globals_Client.fiefToView.barredCharacters[i]];
                }

                if (myBarredChar != null)
                {
                    // Create an item and subitems for each character

                    // name
                    ListViewItem charItem = new ListViewItem(myBarredChar.firstName + " " + myBarredChar.familyName);

                    // charID
                    charItem.SubItems.Add(myBarredChar.charID);

                    // if is in player's nationality
                    charItem.SubItems.Add(myBarredChar.nationality.name);

                    // add item to fiefsListView
                    this.barredCharsListView.Items.Add(charItem);
                }

            }

            // NATIONALITIES
            // iterates through nationalities
            foreach (KeyValuePair<string, Nationality> thisNat in Globals_Game.nationalityMasterList)
            {
                // Create an item and subitems for each nationality
                // ID
                ListViewItem natItem = new ListViewItem(thisNat.Value.natID);

                // name
                natItem.SubItems.Add(thisNat.Value.name);

                // ensure item is checked if appropriate
                if (Globals_Client.fiefToView.barredNationalities.Contains(thisNat.Value.natID))
                {
                    natItem.Checked = true;
                }

                // add item to fiefsListView
                this.barredNatsListView.Items.Add(natItem);

            }

            // disable 'UnBar Character' button until a list item is selected
            this.unbarCharBtn.Enabled = false;
        }

        /// <summary>
        /// Configures UI display for list of troop detachments
        /// </summary>
        public void SetUpTransferList()
        {
            // add necessary columns
            this.transferListView.Columns.Add("   ID", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Knights", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("M-A-A", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("LightCav", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Longbowmen", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Crossbowmen", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Foot", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Rabble", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Days", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("For", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Configures UI display for list of armies in feif
        /// </summary>
        public void SetUpArmiesList()
        {
            // add necessary columns
            this.armiesListView.Columns.Add("   ID", -2, HorizontalAlignment.Left);
            this.armiesListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);

            // disable button
            this.armiesAttackBtn.Enabled = false;
        }

        /// <summary>
        /// Refreshes troop transfer list
        /// </summary>
       public void RefreshTransferDisplay()
        {
            ListViewItem thisDetachment = null;

            // get army
            Army thisArmy = Globals_Game.armyMasterList[this.armyID];

            // get fief
            Fief thisFief = thisArmy.GetLocation();

            // add troop detachments to list
            foreach (KeyValuePair<string, string[]> troopDetachment in thisFief.troopTransfers)
            {
                if ((troopDetachment.Value[0] == thisArmy.owner) || (troopDetachment.Value[1] == thisArmy.owner))
                {
                    // Create an item and subitems for each detachment

                    // ID
                    thisDetachment = new ListViewItem(troopDetachment.Key);

                    // knights
                    thisDetachment.SubItems.Add(troopDetachment.Value[2]);
                    // menAtArms
                    thisDetachment.SubItems.Add(troopDetachment.Value[3]);
                    // lightCav
                    thisDetachment.SubItems.Add(troopDetachment.Value[4]);
                    // longbowmen
                    thisDetachment.SubItems.Add(troopDetachment.Value[5]);
                    // crossbowmen
                    thisDetachment.SubItems.Add(troopDetachment.Value[6]);
                    // foot
                    thisDetachment.SubItems.Add(troopDetachment.Value[7]);
                    // rabble
                    thisDetachment.SubItems.Add(troopDetachment.Value[8]);

                    // days
                    thisDetachment.SubItems.Add(troopDetachment.Value[9]);

                    // owner
                    thisDetachment.SubItems.Add(troopDetachment.Value[0]);

                    // for
                    thisDetachment.SubItems.Add(troopDetachment.Value[1]);

                    // add item to fiefsListView
                    this.transferListView.Items.Add(thisDetachment);

                }
            }
        }

        /// <summary>
        /// Refreshes list of armies in fief
        /// </summary>
        public void RefreshArmiesDisplay()
        {
            ListViewItem armyEntry = null;

            // get fief
            Fief thisFief = this.observer.location;

            // Create an item and subitems for each army in the fief and add to list
            foreach (string armyID in thisFief.armies)
            {
                // get army
                Army thisArmy = Globals_Game.armyMasterList[armyID];

                // ID
                armyEntry = new ListViewItem(armyID);

                // owner
                armyEntry.SubItems.Add(thisArmy.owner);

                // add item to fiefsListView
                this.armiesListView.Items.Add(armyEntry);
            }
        }

        /// <summary>
        /// Responds to the click event of the barThisCharBtn button,
        /// adding the charID in the barThisCharTextBox to the barred characters list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barThisCharBtn_Click(object sender, EventArgs e)
        {
            // get character
            Character thisCharacter = null;

            if (Globals_Game.pcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                thisCharacter = Globals_Game.pcMasterList[this.barThisCharTextBox.Text];
            }
            else if (Globals_Game.npcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                thisCharacter = Globals_Game.npcMasterList[this.barThisCharTextBox.Text];
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Character could not be identified.  Please ensure charID is valid.");
                }
            }

            if (thisCharacter != null)
            {
                // ensure can't bar self
                if (thisCharacter != Globals_Client.myPlayerCharacter)
                {
                    // add ID to barred characters
                    Globals_Client.fiefToView.barredCharacters.Add(thisCharacter.charID);

                    // check if is currently in keep of barring fief, and remove if necessary
                    if ((thisCharacter.inKeep) && (thisCharacter.location == Globals_Client.fiefToView))
                    {
                        thisCharacter.inKeep = false;
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(thisCharacter.firstName + " " + thisCharacter.familyName + " has been ejected from the keep.");
                        }

                    }

                    // refresh display
                    this.RefreshBarredDisplay();

                    // refresh the parent's fief container
                    this.parent.RefreshFiefContainer(Globals_Client.fiefToView);
                }

                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You cannot bar yourself from your own fief!");
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the unbarCharBtn button,
        /// removing the selected character in the ListView from the barred characters list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void unbarCharBtn_Click(object sender, EventArgs e)
        {
            if (barredCharsListView.SelectedItems.Count > 0)
            {
                // if selected character is in pcMasterList
                if (Globals_Game.pcMasterList.ContainsKey(this.barredCharsListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    Globals_Client.fiefToView.barredCharacters.Remove(this.barredCharsListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.RefreshBarredDisplay();
                }
                // if selected character is in pcMasterList
                else if (Globals_Game.npcMasterList.ContainsKey(this.barredCharsListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    Globals_Client.fiefToView.barredCharacters.Remove(this.barredCharsListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.RefreshBarredDisplay();
                }
                // if selected character not found
                else
                {
                    // display error message
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Selected character could not be identified.");
                    }
                }

            }
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the barredListView,
        /// enabling the 'unBar Character' button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barredListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (barredCharsListView.SelectedItems.Count > 0)
            {
                // enable 'unBar Character' button
                this.unbarCharBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the closeBtn button,
        /// closing the Lock Out Menu
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the click event of the transferPickupBtn button,
        /// allowing any selected detachments to be added to the currently displayed army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferPickupBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;
            bool adjustDays = true;
            double daysTaken = 0;
            double minDays = 0;
            bool displayNotAllMsg = false;
            uint[] totTroopsToAdd = new uint[] {0, 0, 0, 0, 0, 0, 0};
            string toDisplay = "";

            // get army
            Army thisArmy = Globals_Client.armyToView;

            // set minDays to thisArmy.days (as default value)
            minDays = thisArmy.days;

            // get leader and owner
            Character myLeader = thisArmy.GetLeader();
            PlayerCharacter myOwner = thisArmy.GetOwner();

            // get checked items in listview
            ListView.CheckedListViewItemCollection checkedItems = this.transferListView.CheckedItems;

            // if no detachment selected, do nothing except display message
            if (checkedItems.Count < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No detachments have been selected.");
                }
            }

            // if any detachments selected, proceed
            else
            {
                // check have minimum days necessary for transfer
                if (thisArmy.days < 10)
                {
                    if (Globals_Client.showMessages)
                    {
                        toDisplay = "You don't have enough days left for this transfer.";
                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                    }
                    proceed = false;
                    adjustDays = false;
                }
                else
                {
                    // calculate time taken for transfer
                    daysTaken = Globals_Game.myRand.Next(10, 31);

                    // check if have enough days for transfer in this instance
                    if (daysTaken > thisArmy.days)
                    {
                        daysTaken = thisArmy.days;
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                        }
                        proceed = false;
                    }
                    else
                    {
                        // make sure collecting army is owned by recipient or donator
                        foreach (ListViewItem item in checkedItems)
                        {
                            // get donating player
                            PlayerCharacter pcFrom = null;
                            if (Globals_Game.pcMasterList.ContainsKey(item.SubItems[9].Text))
                            {
                                pcFrom = Globals_Game.pcMasterList[item.SubItems[9].Text];
                            }

                            // get donating player
                            PlayerCharacter pcFor = null;
                            if (Globals_Game.pcMasterList.ContainsKey(item.SubItems[10].Text))
                            {
                                pcFor = Globals_Game.pcMasterList[item.SubItems[10].Text];
                            }

                            // check for appropriate collecting player
                            if ((myOwner != pcFrom) && (myOwner != pcFor))
                                {
                                    if (Globals_Client.showMessages)
                                    {
                                        toDisplay = "You are not permitted to collect at least one of the selected detachments.";
                                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                    }
                                    proceed = false;
                                    adjustDays = false;
                                }
                        }
                    }
                }

                if (proceed)
                {
                    // get fief
                    Fief thisFief = thisArmy.GetLocation();

                    // check for minimum days
                    foreach (ListViewItem item in checkedItems)
                    {
                        double thisDays = Convert.ToDouble(item.SubItems[8].Text);

                        // check if detachment has enough days for transfer in this instance
                        // if not, flag display of message at end of process, but do nothing else
                        if (thisDays < daysTaken)
                        {
                            displayNotAllMsg = true;
                            toDisplay = "At least one detachment could not be added due to poor organisation.";
                        }
                        else
                        {
                            if (thisDays < minDays)
                            {
                                minDays = thisDays;
                            }
                        }
                    }

                    foreach (ListViewItem item in checkedItems)
                    {
                        double thisDays = Convert.ToDouble(item.SubItems[8].Text);

                        // get numbers of each type to add
                        uint[] thisTroops = new uint[] { Convert.ToUInt32(item.SubItems[1].Text), Convert.ToUInt32(item.SubItems[2].Text),
                            Convert.ToUInt32(item.SubItems[3].Text), Convert.ToUInt32(item.SubItems[4].Text),
                            Convert.ToUInt32(item.SubItems[5].Text), Convert.ToUInt32(item.SubItems[6].Text),
                            Convert.ToUInt32(item.SubItems[7].Text) };

                        // if does have enough days, proceed
                        if (thisDays >= daysTaken)
                        {
                            // create temporary army to check attrition
                            Army tempArmy = new Army(Globals_Game.GetNextArmyID(), null, item.SubItems[9].Text,
                                thisDays, thisArmy.location, trp: thisTroops);

                            // compare days to minDays, apply attrition if necessary
                            // then add to troopsToAdd
                            if (thisDays > minDays)
                            {
                                // check for attrition (to bring it down to minDays)
                                byte attritionChecks = 0;
                                attritionChecks = Convert.ToByte((thisDays - minDays) / 7);
                                double attritionModifier = 0;

                                for (int i = 0; i < attritionChecks; i++)
                                {
                                    attritionModifier = tempArmy.CalcAttrition();

                                    // apply attrition
                                    if (attritionModifier > 0)
                                    {
                                        tempArmy.ApplyTroopLosses(attritionModifier);
                                    }
                                }
                            }

                            for (int i = 0; i < totTroopsToAdd.Length; i++)
                            {
                                totTroopsToAdd[i] += tempArmy.troops[i];
                            }

                            // remove detachment from fief
                            thisFief.troopTransfers.Remove(item.SubItems[0].Text);

                            // nullify tempArmy
                            tempArmy = null;
                        }

                    }

                }

                if (adjustDays)
                {
                    if (thisArmy.days == minDays)
                    {
                        // add troops to army (this could be 0)
                        for (int i = 0; i < thisArmy.troops.Length; i++ )
                        {
                            thisArmy.troops[i] += totTroopsToAdd[i];
                        }

                        // adjust days
                        myLeader.AdjustDays(daysTaken);

                        // calculate attrition for army
                        byte attritionChecks = Convert.ToByte(daysTaken / 7);
                        double attritionModifier = 0;

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = thisArmy.CalcAttrition();
                            if (attritionModifier > 0)
                            {
                                thisArmy.ApplyTroopLosses(attritionModifier);
                            }
                        }
                    }
                    else
                    {
                        // any days army has had to 'wait' should go towards days taken
                        // for the transfer (daysTaken)
                        double differenceToMin = (thisArmy.days - minDays);
                        if (differenceToMin >= daysTaken)
                        {
                            daysTaken = 0;
                        }
                        else
                        {
                            daysTaken = daysTaken - differenceToMin;
                        }

                        // adjust days
                        myLeader.AdjustDays(differenceToMin);

                        // calculate attrition for army (to bring it down to minDays)
                        byte attritionChecks = Convert.ToByte(differenceToMin / 7);
                        double attritionModifier = 0;

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = thisArmy.CalcAttrition();
                            if (attritionModifier > 0)
                            {
                                thisArmy.ApplyTroopLosses(attritionModifier);
                            }
                        }

                       // add troops to army
                        for (int i = 0; i < thisArmy.troops.Length; i++ )
                        {
                            thisArmy.troops[i] += totTroopsToAdd[i];
                        }

                        // check if are any remaining days taken for the transfer (daysTaken) 
                        if (daysTaken > 0)
                        {
                            // adjust days
                            myLeader.AdjustDays(daysTaken);

                            // calculate attrition for army for days taken for transfer
                            attritionChecks = Convert.ToByte(daysTaken / 7);

                            for (int i = 0; i < attritionChecks; i++)
                            {
                                attritionModifier = thisArmy.CalcAttrition();
                                if (attritionModifier > 0)
                                {
                                    thisArmy.ApplyTroopLosses(attritionModifier);
                                }
                            }
                        }
                    }

                    // if not all selected detachments could be picked up (not enough days), show message
                    if (displayNotAllMsg)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(toDisplay);
                        }
                    }
                }

                // refresh the army screen (in the main form) and close form
                this.parent.RefreshArmyContainer(thisArmy);
                this.Close();
            }

        }

        /// <summary>
        /// Responds to the click event of the transferCancelBtn button,
        /// closing the selection form
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the armiesListView,
        /// displaying the details of the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            string textToDisplay = "";

            if (armiesListView.SelectedItems.Count > 0)
            {
                // set textbox to read only
                this.armiesTextBox.ReadOnly = true;

                // get details
                Army otherArmy = Globals_Game.armyMasterList[this.armiesListView.SelectedItems[0].SubItems[0].Text];
                textToDisplay += this.DisplayArmy(otherArmy);

                // display details
                this.armiesTextBox.Text = textToDisplay;

                // get owner of selected army
                PlayerCharacter otherArmyOwner = otherArmy.GetOwner();

                // if selected army is not owned by player && observer is an army leader, enable attack button
                if ((otherArmyOwner != Globals_Client.myPlayerCharacter) && (!String.IsNullOrWhiteSpace(this.observer.armyID)))
                {
                    this.armiesAttackBtn.Enabled = true;
                }
                else
                {
                    this.armiesAttackBtn.Enabled = false;
                }

            }
        }

        /// <summary>
        /// Responds to the click event of the armiesCloseBtn button,
        /// closing the armies list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the click event of the armiesAttackBtn button,
        /// instigating an attack on the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesAttackBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;
            string toDisplay = "";

            // get armies
            Army attacker = this.observer.GetArmy();
            Army defender = null;
            if (Globals_Game.armyMasterList.ContainsKey(this.armiesListView.SelectedItems[0].SubItems[0].Text))
            {
                defender = Globals_Game.armyMasterList[this.armiesListView.SelectedItems[0].SubItems[0].Text];
            }

            // check the observer is leading an army
            if (attacker == null)
            {
                if (Globals_Client.showMessages)
                {
                    toDisplay = "You are not leading an army and so cannot attack.";
                    System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                    proceed = false;
                }
            }

            else
            {
                if (defender == null)
                {
                    if (Globals_Client.showMessages)
                    {
                        toDisplay = "Error: Defending army could not be identified.";
                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                        proceed = false;
                    }
                }

                else
                {
                    // check if attacking army is besieging a keep
                    string siegeID = attacker.CheckIfBesieger();
                    if (!String.IsNullOrWhiteSpace(siegeID))
                    {
                        // display warning and get decision
                        DialogResult dialogResult = MessageBox.Show("Your army is besieging a keep and this action would end the siege.\r\nClick 'OK' to proceed.", "Proceed with attack?", MessageBoxButtons.OKCancel);

                        // if choose to cancel
                        if (dialogResult == DialogResult.Cancel)
                        {
                            if (Globals_Client.showMessages)
                            {
                                toDisplay = "Attack cancelled.";
                                System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                proceed = false;
                            }
                        }

                        // if choose to proceed
                        else
                        {
                            Siege thisSiege = null;
                            thisSiege = Globals_Game.siegeMasterList[siegeID];

                            // construct event description to be passed into siegeEnd
                            string siegeDescription = "On this day of Our Lord the forces of ";
                            siegeDescription += thisSiege.GetBesiegingPlayer().firstName + " " + thisSiege.GetBesiegingPlayer().familyName;
                            siegeDescription += " have chosen to abandon the siege of " + thisSiege.GetFief().name;
                            siegeDescription += ". " + thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;
                            siegeDescription += " retains ownership of the fief.";

                            parent.SiegeEnd(thisSiege, false, siegeDescription);
                        }
                    }
                }
            }

            if (proceed)
            {
                // do various conditional checks
                proceed = attacker.ChecksBeforeAttack(defender);
            }

            if (proceed)
            {
                // let slip the dogs of war
                parent.GiveBattle(attacker, defender);
            }

            // close form
            this.Close();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the transferFundsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayerCharacter transferTo = null;

            if (this.transferFundsListView.SelectedItems.Count > 0)
            {
                // get player
                if (Globals_Game.pcMasterList.ContainsKey(this.transferFundsListView.SelectedItems[0].SubItems[1].Text))
                {
                    transferTo = Globals_Game.pcMasterList[this.transferFundsListView.SelectedItems[0].SubItems[1].Text];
                }

                if (transferTo != null)
                {
                    // enable controls
                    this.transferFundsBtn.Enabled = true;
                    this.transferFundsTextBox.Enabled = true;

                    // get home fief
                    Fief home = Globals_Client.myPlayerCharacter.GetHomeFief();

                    // get home treasury
                    int homeTreasury = home.GetAvailableTreasury(true);

                    // populate transferFundsLabel to show treasury available
                    this.transferFundsLabel.Text = homeTreasury.ToString();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the transferFundsBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                PlayerCharacter playerTo = null;

                if (this.transferFundsListView.SelectedItems.Count > 0)
                {
                    // get player
                    if (Globals_Game.pcMasterList.ContainsKey(this.transferFundsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        playerTo = Globals_Game.pcMasterList[this.transferFundsListView.SelectedItems[0].SubItems[1].Text];
                    }

                    if (playerTo != null)
                    {
                        // get home fief
                        Fief fiefFrom = Globals_Client.myPlayerCharacter.GetHomeFief();

                        // get playerTo's home fief
                        Fief fiefTo = playerTo.GetHomeFief();

                        // get amount to transfer
                        int amount = Convert.ToInt32(this.transferFundsTextBox.Text);

                        if (((fiefFrom != null) && (fiefTo != null)) && (amount > 0))
                        {
                            // make sure are enough funds to cover transfer
                            if (amount > fiefFrom.GetAvailableTreasury(true))
                            {
                                // if not, inform player and adjust amount downwards
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("Too few funds available for this transfer.");
                                }
                            }

                            else
                            {
                                // make the transfer
                                this.parent.TreasuryTransfer(fiefFrom, fiefTo, amount);

                                // close form
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
        }

        /// <summary>
        /// Responds to the ItemCheck event of the barredNatsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barredNatsListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // get nationality ID
            string natID = this.barredNatsListView.Items[e.Index].SubItems[0].Text;

            // if item original state is UNCHECKED, add nationality to barredNationalities
            if (e.CurrentValue == CheckState.Unchecked)
            {
                // check to make sure aren't barring the fief's own nataionality
                if (!(Globals_Client.fiefToView.owner.nationality.natID == natID))
                {
                    if (!Globals_Client.fiefToView.barredNationalities.Contains(natID))
                    {
                        Globals_Client.fiefToView.barredNationalities.Add(natID);
                    }
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You cannot bar the fief's own nationality!");
                    }
                    e.NewValue = CheckState.Unchecked;
                }
            }

            // if item original state is CHECKED, remove nationality from barredNationalities
            else
            {
                if (Globals_Client.fiefToView.barredNationalities.Contains(natID))
                {
                    Globals_Client.fiefToView.barredNationalities.Remove(natID);
                }
            }

            // refresh the parent's fief container
            this.parent.RefreshFiefContainer(Globals_Client.fiefToView);
        }
    }

}
