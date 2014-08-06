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
        /// Constructor for SelectionForm
        /// </summary>
        /// <param name="par">Parent Form1 object</param>
        /// <param name="myFunction">String indicating function to be performed</param>
        public SelectionForm(Form1 par, String myFunction)
        {
            // initialise form elements
            InitializeComponent();

            this.parent = par;

            // initialise NPC display
            this.initDisplay(myFunction);
        }

        /// <summary>
        /// Initialises NPC display screen
        /// </summary>
        /// <param name="myFunction">String indicating function to be performed</param>
        private void initDisplay(String myFunction)
        {
            if (myFunction.Equals("bailiff"))
            {
                // format list display
                this.setUpNpcList();

                // refresh information 
                this.refreshNPCdisplay();

                // show container
                this.npcContainer.BringToFront();
            }

            if (myFunction.Equals("lockout"))
            {
                // format list display
                this.setUpBarredList();

                // refresh information 
                this.refreshBarredDisplay();

                // show container
                this.lockOutContainer.BringToFront();
            }
        }

        /// <summary>
        /// Configures UI display for list of household NPCs
        /// </summary>
        public void setUpNpcList()
        {
            // add necessary columns
            this.npcListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes NPC list
        /// </summary>
        public void refreshNPCdisplay()
        {
            // remove any previously displayed characters
            this.npcDetailsTextBox.ReadOnly = true;
            this.npcDetailsTextBox.Text = "";

            // clear existing items in list
            this.npcListView.Items.Clear();

            ListViewItem[] myNPCs = new ListViewItem[this.parent.myChar.myNPCs.Count];

            // iterates through employees
            for (int i = 0; i < this.parent.myChar.myNPCs.Count; i++)
            {
                // Create an item and subitems for each character

                // name
                myNPCs[i] = new ListViewItem(this.parent.myChar.myNPCs[i].name);

                // charID
                myNPCs[i].SubItems.Add(this.parent.myChar.myNPCs[i].charID);

                // if is in player's entourage
                if (this.parent.myChar.myNPCs[i].inEntourage)
                {
                    myNPCs[i].SubItems.Add("Yes");
                }

                // add item to fiefsListView
                this.npcListView.Items.Add(myNPCs[i]);
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
            NonPlayerCharacter npcToDisplay = null;

            // loop through employees
            for (int i = 0; i < this.parent.myChar.myNPCs.Count; i++)
            {
                if (npcListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (this.parent.myChar.myNPCs[i].charID.Equals(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        npcToDisplay = this.parent.myChar.myNPCs[i];
                    }

                }

            }

            // retrieve and display character information
            if (npcToDisplay != null)
            {
                string textToDisplay = "";
                // get details
                textToDisplay += this.displayNPC(npcToDisplay);
                this.npcDetailsTextBox.ReadOnly = true;
                // display details
                this.npcDetailsTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Retrieves NPC details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string displayNPC(NonPlayerCharacter npc)
        {
            string charText = "";

            // ID
            charText += "ID: " + npc.charID + "\r\n";

            // name
            charText += "Name: " + npc.name + "\r\n";

            // age
            charText += "Age: " + npc.calcCharAge() + "\r\n";

            // nationality
            charText += "Nationality: " + npc.nationality + "\r\n";

            // health (& max. health)
            charText += "Health: ";
            if (npc.health == 0)
            {
                charText += "Blimey, you're Dead!";
            }
            else
            {
                charText += npc.health + " (max. health: " + npc.maxHealth + ")";
            }
            charText += "\r\n";

            // any death modifiers (from skills)
            charText += "  (Death modifier from skills: " + npc.calcSkillEffect("death") + ")\r\n";

            // location
            charText += "Current location: " + npc.location.name + " (" + npc.location.province.name + ")\r\n";

            // if in process of auto-moving, display next hex
            if (npc.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + npc.goTo.Peek().fiefID + "\r\n";
            }

            // language
            charText += "Language: " + npc.language + "\r\n";

            // days left
            charText += "Days remaining: " + npc.days + "\r\n";

            // stature
            charText += "Stature: " + npc.stature + "\r\n";

            // management rating
            charText += "Management: " + npc.management + "\r\n";

            // combat rating
            charText += "Combat: " + npc.combat + "\r\n";

            // skills list
            charText += "Skills:\r\n";
            for (int i = 0; i < npc.skills.Length; i++)
            {
                charText += "  - " + npc.skills[i].Item1.name + " (level " + npc.skills[i].Item2 + ")\r\n";
            }

            // gather additional NPC-specific information
            charText += this.displayNonPlayerCharacter(npc);

            return charText;
        }

        /// <summary>
        /// Retrieves NonPlayerCharacter-specific details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string displayNonPlayerCharacter(NonPlayerCharacter npc)
        {
            string npcText = "";

            // current salary
            npcText += "Current salary: " + npc.wage + "\r\n";

            return npcText;
        }

        /// <summary>
        /// Responds to the click event of the chooseNpcBtn button
        /// which appoints the selected NPC as the bailiff of the fief displayed in the main UI
        /// and closes the child (this) form
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void chooseNpcBtn_Click(object sender, EventArgs e)
        {
            // string to hold function
            String funct = "";

            if (npcListView.SelectedItems.Count > 0)
            {
                // if the fief has an existing bailiff, relieve him of his duties
                if (this.parent.fiefToView.bailiff != null)
                {
                    // set ex-bailiff's function
                    (this.parent.fiefToView.bailiff as NonPlayerCharacter).function = "Unspecified";
                    // remove from bailiff position
                    this.parent.fiefToView.bailiff = null;
                }

                // set the selected NPC as bailiff
                this.parent.fiefToView.bailiff = this.parent.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                
                // set new bailiff's function
                funct = "Bailiff of " + this.parent.fiefToView.name + " (" + this.parent.fiefToView.fiefID + ")";
                (this.parent.fiefToView.bailiff as NonPlayerCharacter).function = funct;

                // refresh the fief information (in the main form)
                this.parent.refreshFiefContainer(this.parent.fiefToView);
            }

            // close this form
            this.Close();
        }

        /// <summary>
        /// Configures UI display for list of barred characters
        /// </summary>
        public void setUpBarredList()
        {
            // add necessary columns
            this.barredListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.barredListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.barredListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes barred characters list
        /// </summary>
        public void refreshBarredDisplay()
        {
            // clear existing items in list
            this.barredListView.Items.Clear();

            ListViewItem[] barredChars = new ListViewItem[this.parent.fiefToView.barredCharacters.Count];

            // iterates through employees
            for (int i = 0; i < this.parent.fiefToView.barredCharacters.Count; i++)
            {
                // retrieve character
                //PlayerCharacter myBarredpc = null;
                //NonPlayerCharacter myBarrednpc = null;
                Character myBarredChar = null;

                if (this.parent.pcMasterList.ContainsKey(this.parent.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = this.parent.pcMasterList[this.parent.fiefToView.barredCharacters[i]];
                }
                else if (this.parent.npcMasterList.ContainsKey(this.parent.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = this.parent.npcMasterList[this.parent.fiefToView.barredCharacters[i]];
                }

                if (myBarredChar != null)
                {
                    // Create an item and subitems for each character

                    // name
                    barredChars[i] = new ListViewItem(myBarredChar.name);

                    // charID
                    barredChars[i].SubItems.Add(myBarredChar.charID);

                    // if is in player's nationality
                    barredChars[i].SubItems.Add(myBarredChar.nationality);

                    // add item to fiefsListView
                    this.barredListView.Items.Add(barredChars[i]);
                }

            }

            // disable 'UnBar Character' button until a list item is selected
            this.unbarCharBtn.Enabled = false;
            // ensure the nationality bar CheckBoxes are displaying correctly
            this.barEnglishCheckBox.Checked = this.parent.fiefToView.englishBarred;
            this.barFrenchCheckBox.Checked = this.parent.fiefToView.frenchBarred;
        }

        /// <summary>
        /// Responds to the click event of the barThisCharBtn button,
        /// adding the charID in the barThisCharTextBox to the barred characters list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barThisCharBtn_Click(object sender, EventArgs e)
        {
            // if input ID is in pcMasterList
            if (this.parent.pcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                // add ID to barred characters
                this.parent.fiefToView.barredCharacters.Add(this.barThisCharTextBox.Text);
                // refresh display
                this.refreshBarredDisplay();
            }
            // if input ID is in npcMasterList
            else if (this.parent.npcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                // add ID to barred characters
                this.parent.fiefToView.barredCharacters.Add(this.barThisCharTextBox.Text);
                // refresh display
                this.refreshBarredDisplay();
            }
            // if input ID not found
            else
            {
                // ask player to check entry
                System.Windows.Forms.MessageBox.Show("Character could not be identified.  Please ensure charID is valid.");
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
            if (barredListView.SelectedItems.Count > 0)
            {
                // if selected character is in pcMasterList
                if (this.parent.pcMasterList.ContainsKey(this.barredListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    this.parent.fiefToView.barredCharacters.Remove(this.barredListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.refreshBarredDisplay();
                }
                // if selected character is in pcMasterList
                else if (this.parent.npcMasterList.ContainsKey(this.barredListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    this.parent.fiefToView.barredCharacters.Remove(this.barredListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.refreshBarredDisplay();
                }
                // if selected character not found
                else
                {
                    // display error message
                    System.Windows.Forms.MessageBox.Show("Selected character could not be identified.");
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
            if (barredListView.SelectedItems.Count > 0)
            {
                // enable 'unBar Character' button
                this.unbarCharBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the barFrenchCheckBox,
        /// either barring or unbarring all French characters
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barFrenchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // bar/unbar French, depending on whether CheckBox is checked
            this.parent.fiefToView.frenchBarred = this.barFrenchCheckBox.Checked;
            // refresh the parent's fief container
            this.parent.refreshFiefContainer(this.parent.fiefToView);
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the barEnglishCheckBox,
        /// either barring or unbarring all English characters
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barEnglishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // bar/unbar English, depending on whether CheckBox is checked
            this.parent.fiefToView.englishBarred = this.barEnglishCheckBox.Checked;
            // refresh the parent's fief container
            this.parent.refreshFiefContainer(this.parent.fiefToView);
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

    }

}