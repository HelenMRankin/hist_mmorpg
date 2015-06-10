using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using CorrugatedIron;
using CorrugatedIron.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Partial class for Form1, containing functionality specific to household management
    /// Something like this will exist on client
    /// I feel like the method for grabbing the information to do with family members belongs in playercharacter
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Refreshes Household display
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to display</param>
        public void RefreshHouseholdDisplay(NonPlayerCharacter npc = null)
        {
            // set main character display as read only
            this.houseCharTextBox.ReadOnly = true;

            // disable controls until NPC selected in ListView
            this.houseCampBtn.Enabled = false;
            this.houseCampDaysTextBox.Enabled = false;
            this.familyNameChildButton.Enabled = false;
            this.familyNameChildTextBox.Enabled = false;
            this.familyNpcSpousePregBtn.Enabled = false;
            this.houseHeirBtn.Enabled = false;
            this.houseMoveToBtn.Enabled = false;
            this.houseMoveToTextBox.Enabled = false;
            this.houseRouteBtn.Enabled = false;
            this.houseEntourageBtn.Enabled = false;
            this.houseFireBtn.Enabled = false;
            this.houseExamineArmiesBtn.Enabled = false;

            // remove any previously displayed text
            this.houseCharTextBox.Text = "";
            this.houseCampDaysTextBox.Text = "";
            this.familyNameChildTextBox.Text = "";
            this.houseMoveToTextBox.Text = "";
            this.houseRouteTextBox.Text = "";
            this.houseProposeBrideTextBox.Text = "";

            // clear existing items in characters list
            this.houseCharListView.Items.Clear();

            // variables needed for name check (to see if NPC needs naming)
            string nameWarning = "The following offspring need to be named:\r\n\r\n";
            bool showNameWarning = false;

            // iterates through household characters adding information to ListView
            // and checking if naming is required
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                ListViewItem houseChar = null;

                // name
                houseChar = new ListViewItem(Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName);

                // charID
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].charID);

                // function (e.g. employee, son, wife, etc.)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].GetFunction(Globals_Client.myPlayerCharacter));

                // responsibilities (i.e. jobs)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].GetResponsibilities(Globals_Client.myPlayerCharacter));

                // location
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].location.id + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].location.name + ")");

                // show whether is in player's entourage
                if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                {
                    houseChar.SubItems.Add("Yes");
                }

                // check if needs to be named
                if (!String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.myNPCs[i].familyID))
                {
                    bool nameRequired = Globals_Client.myPlayerCharacter.myNPCs[i].HasBabyName(0);

                    if (nameRequired)
                    {
                        showNameWarning = true;
                        nameWarning += " - " + Globals_Client.myPlayerCharacter.myNPCs[i].charID + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName + ")\r\n";
                    }
                }

                if (houseChar != null)
                {
                    // if NPC passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == npc)
                    {
                        houseChar.Selected = true;
                    }

                    // add item to fiefsListView
                    this.houseCharListView.Items.Add(houseChar);
                }

            }

            // always enable marriage proposal controls
            this.houseProposeBtn.Enabled = true;
            this.houseProposeBrideTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Text = Globals_Client.myPlayerCharacter.charID;

            this.houseCharListView.HideSelection = false;
            this.houseCharListView.Focus();

            if (showNameWarning)
            {
                nameWarning += "\r\nAny children who are not named by the age of one will,\r\nwhere possible, be named after their royal highnesses the king and queen.";
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(nameWarning);
                }
            }
        }
    }
}
