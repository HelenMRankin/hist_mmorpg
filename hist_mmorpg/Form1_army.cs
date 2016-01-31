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
    /// Partial class for Form1, containing functionality specific to army management
    /// In all likelihood these methods will be implemented  in client side
    /// these ones will be replaced with methods to gather appropriate data to send to client
    /// </summary>
    partial class Form1
    {

        /// <summary>
        /// Refreshes main Army display screen
        /// </summary>
        /// <param name="a">Army whose information is to be displayed</param>
        public void RefreshArmyContainer(Army a = null)
        {
            // disable controls until army selected
            // Pretty sure all this will be controlled entirely from client side
            this.DisableControls(this.armyManagementPanel);
            this.DisableControls(this.armyCombatPanel);

            // always enable switch between management and combat panels
            this.armyDisplayCmbtBtn.Enabled = true;
            this.armyDisplayMgtBtn.Enabled = true;

            // ensure main textbox is empty and isn't interactive
            this.armyTextBox.Text = "";
            this.armyTextBox.ReadOnly = true;

            // clear existing items in armies list
            this.armyListView.Items.Clear();

            // iterates through player's armies adding information to ListView
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myArmies.Count; i++)
            {
                ListViewItem thisArmy = null;

                // armyID
                thisArmy = new ListViewItem(Globals_Client.myPlayerCharacter.myArmies[i].armyID);

                // leader
                Character armyLeader = Globals_Client.myPlayerCharacter.myArmies[i].GetLeader();
                if (armyLeader != null)
                {
                    thisArmy.SubItems.Add(armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")");
                }
                else
                {
                    thisArmy.SubItems.Add("No leader");
                }

                // location
                Fief armyLocation = Globals_Client.myPlayerCharacter.myArmies[i].GetLocation();
                thisArmy.SubItems.Add(armyLocation.name + " (" + armyLocation.id + ")");

                // size
                thisArmy.SubItems.Add(Globals_Client.myPlayerCharacter.myArmies[i].CalcArmySize().ToString());

                if (thisArmy != null)
                {
                    // if army passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myArmies[i] == a)
                    {
                        thisArmy.Selected = true;
                    }

                    // add item to armyListView
                    this.armyListView.Items.Add(thisArmy);
                }

            }

            if (a == null)
            {
                // if player is not leading any armies, set button text to 'recruit new' and enable
                if (String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.armyID))
                {
                    this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                    this.armyRecruitBtn.Tag = "new";
                    this.armyRecruitBtn.Enabled = true;
                    this.armyRecruitTextBox.Enabled = true;
                }
            }

            Globals_Client.containerToView = this.armyContainer;
            Globals_Client.containerToView.BringToFront();

            // check which panel to display
            string armyPanelTag = this.armyContainer.Panel1.Tag.ToString();
            if (armyPanelTag.Equals("combat"))
            {
                this.armyCombatPanel.BringToFront();
            }
            else
            {
                this.armyManagementPanel.BringToFront();
            }

            this.armyListView.Focus();
        }

        /// <summary>
        /// Retrieves details of all armies in observer's current fief
        /// </summary>
        /// <param name="observer">The observer</param>
        private void ExamineArmiesInFief(Character observer)
        {
            bool proceed = observer.ChecksBefore_ExamineArmies();

            // refresh display to show amended observer days
            if (Globals_Client.containerToView == this.armyContainer)
            {
                this.RefreshArmyContainer(Globals_Client.armyToView);
            }
            else if (Globals_Client.containerToView == this.houseContainer)
            {
                this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
            else if (Globals_Client.containerToView == this.travelContainer)
            {
                this.RefreshTravelContainer();
            }

            if (proceed)
            {
                // check for previously opened SelectionForm and close if necessary
                if (Application.OpenForms.OfType<SelectionForm>().Any())
                {
                    Application.OpenForms.OfType<SelectionForm>().First().Close();
                }

                // display armies list
                SelectionForm examineArmies = new SelectionForm(this, "armies", obs: observer);
                examineArmies.Show();
            }


        }
    }
}
