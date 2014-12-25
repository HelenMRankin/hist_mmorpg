﻿using System;
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
    /// Partial class for Form1, containing functionality specific to the meeting place screen
    /// (court, tavern, outside keep)
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Creates UI display for list of characters present in Court, Tavern, outside keep
        /// </summary>
        public void setUpMeetingPLaceCharsList()
        {
            // add necessary columns
            this.meetingPlaceCharsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Sex", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Household", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes UI Court, Tavern, outside keep display
        /// </summary>
        /// <param name="place">string specifying whether court, tavern, outside keep</param>
        public void refreshMeetingPlaceDisplay(string place)
        {
            // refresh general information
            this.meetingPlaceDisplayText();

            // remove any previously displayed characters
            this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
            this.meetingPlaceCharDisplayTextBox.Text = "";

            // disable controls until character selected in list
            this.hireNPC_Btn.Enabled = false;
            this.hireNPC_TextBox.Text = "";
            this.hireNPC_TextBox.Enabled = false;
            this.meetingPlaceMoveToBtn.Enabled = false;
            this.meetingPlaceMoveToTextBox.Text = "";
            this.meetingPlaceMoveToTextBox.Enabled = false;
            this.meetingPlaceRouteBtn.Enabled = false;
            this.meetingPlaceRouteTextBox.Text = "";
            this.meetingPlaceRouteTextBox.Enabled = false;
            this.meetingPlaceEntourageBtn.Enabled = false;
            this.meetingPlaceProposeBtn.Enabled = false;
            this.meetingPlaceProposeTextBox.Text = "";
            this.meetingPlaceProposeTextBox.Enabled = false;

            // set label
            switch (place)
            {
                case "tavern":
                    this.meetingPlaceLabel.Text = "Ye Olde Tavern Of " + Globals_Client.fiefToView.name;
                    break;
                case "court":
                    this.meetingPlaceLabel.Text = "The Esteemed Court Of " + Globals_Client.fiefToView.name;
                    break;
                case "outsideKeep":
                    this.meetingPlaceLabel.Text = "Persons Present Outwith\r\nThe Keep Of " + Globals_Client.fiefToView.name;
                    break;
                default:
                    this.meetingPlaceLabel.Text = "A Generic Meeting Place";
                    break;
            }
            // refresh list of characters
            this.meetingPlaceDisplayList(place);
        }

        /// <summary>
        /// Refreshes general information displayed in Court, Tavern, outside keep
        /// </summary>
        public void meetingPlaceDisplayText()
        {
            string textToDisplay = "";
            // date/season and main character's days left
            textToDisplay += Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear + ".  Your days left: " + Globals_Client.myPlayerCharacter.days + "\r\n\r\n";
            // Fief name/ID and province name
            textToDisplay += "Fief: " + Globals_Client.myPlayerCharacter.location.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")  in " + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name + "\r\n\r\n";
            // Fief owner
            textToDisplay += "Owner: " + Globals_Client.myPlayerCharacter.location.owner.firstName + " " + Globals_Client.myPlayerCharacter.location.owner.familyName + "\r\n";
            // Fief overlord
            textToDisplay += "Overlord: " + Globals_Client.myPlayerCharacter.location.getOverlord().firstName + " " + Globals_Client.myPlayerCharacter.location.getOverlord().familyName + "\r\n";

            this.meetingPlaceTextBox.ReadOnly = true;
            this.meetingPlaceTextBox.Text = textToDisplay;
        }

        /// <summary>
        /// Refreshes display of Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        public void meetingPlaceDisplayList(string place)
        {
            // clear existing items in list
            this.meetingPlaceCharsListView.Items.Clear();

            // select which characters to display - i.e. in the keep (court) or not (tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            // iterates through characters
            for (int i = 0; i < Globals_Client.myPlayerCharacter.location.charactersInFief.Count; i++)
            {
                ListViewItem charsInCourt = null;

                // only display characters in relevant location (in keep, or not)
                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] != Globals_Client.myPlayerCharacter)
                    {

                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((Globals_Client.myPlayerCharacter.location.charactersInFief[i] as NonPlayerCharacter).salary == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                break;
                        }

                    }
                }

                // add item to fiefsListView
                if (charsInCourt != null)
                {
                    this.meetingPlaceCharsListView.Items.Add(charsInCourt);
                }
            }
        }

        /// <summary>
        /// Creates item for Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        /// <returns>ListViewItem containing character details</returns>
        public ListViewItem createMeetingPlaceListItem(Character ch)
        {
            // name
            ListViewItem myItem = new ListViewItem(ch.firstName + " " + ch.familyName);

            // charID
            myItem.SubItems.Add(ch.charID);

            // sex
            if (ch.isMale)
            {
                myItem.SubItems.Add("Male");
            }
            else
            {
                myItem.SubItems.Add("Female");
            }

            // to store household and type data
            string myHousehold = "";
            string myType = "";
            bool isEmployee = false;
            bool isFamily = false;

            // household
            if (!String.IsNullOrWhiteSpace(ch.familyID))
            {
                myHousehold = ch.getHeadOfFamily().familyName + " (ID: " + ch.familyID + ")";

                if (ch.familyID.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isFamily = true;
                }
            }
            else if (!String.IsNullOrWhiteSpace((ch as NonPlayerCharacter).employer))
            {
                myHousehold = (ch as NonPlayerCharacter).getEmployer().familyName + " (ID: " + (ch as NonPlayerCharacter).employer + ")";
            }

            myItem.SubItems.Add(myHousehold);

            // type (e.g. family, NPC, player)

            // check for players and PCs
            if (ch is PlayerCharacter)
            {
                if (!String.IsNullOrWhiteSpace((ch as PlayerCharacter).playerID))
                {
                    myType = "PC (player)";
                }
                else
                {
                    myType = "PC (inactive)";
                }
            }
            else
            {
                // check for employees
                if ((!String.IsNullOrWhiteSpace((ch as NonPlayerCharacter).employer)) && (ch as NonPlayerCharacter).employer.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isEmployee = true;
                }

                // allocate NPC type
                if ((isFamily) || (isEmployee))
                {
                    myType = "My ";
                }
                if (isFamily)
                {
                    myType += "Family";
                }
                else if (isEmployee)
                {
                    myType += "Employee";
                }
                else
                {
                    myType = "NPC";
                }
            }

            myItem.SubItems.Add(myType);

            // show whether is in player's entourage
            bool isCompanion = false;

            if (ch is NonPlayerCharacter)
            {
                // iterate through employees checking for character
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == ch)
                    {
                        if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                        {
                            isCompanion = true;
                        }
                    }
                }
            }

            if (isCompanion)
            {
                myItem.SubItems.Add("Yes");
            }

            return myItem;
        }
    }
}