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
    /// Partial class for Form1, containing functionality specific to movement and the travel screen
    /// </summary>
    partial class Form1
    {
        /*
        /// <summary>
        /// Moves an NPC without a boss one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="npc">NPC to move</param>
        public bool RandomMoveNPC(NonPlayerCharacter npc)
        {
            bool success = false;

            // generate random int 0-6 to see if moves
            int randomInt = Globals_Game.myRand.Next(7);

            if (randomInt > 0)
            {
                // get a destination
                Fief target = Globals_Game.gameMap.chooseRandomHex(npc.location);

                // get travel cost
                double travelCost = npc.location.getTravelCost(target);

                // perform move
                success = npc.MoveCharacter(target, travelCost);
            }

            return success;
        }

        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to move</param>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public bool MoveCharacter(Character ch, Fief target, double cost, bool siegeCheck = true)
        {
            bool success = false;
            bool proceedWithMove = true;

            // check to see if character is leading a besieging army
            if (siegeCheck)
            {
                Army myArmy = ch.GetArmy();
                if (myArmy != null)
                {
                    string thisSiegeID = myArmy.CheckIfBesieger();
                    if (!String.IsNullOrWhiteSpace(thisSiegeID))
                    {
                        // give player fair warning of consequences to siege
                        DialogResult dialogResult = MessageBox.Show("Your army is currently besieging this fief.  Moving will end the siege.\r\nClick 'OK' to proceed.", "Proceed with move?", MessageBoxButtons.OKCancel);

                        // if choose to cancel
                        if (dialogResult == DialogResult.Cancel)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Move cancelled.");
                            }
                            proceedWithMove = false;
                        }

                        // if choose to proceed
                        else
                        {
                            // end the siege
                            Siege thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Siege (" + thisSiegeID + ") ended.");
                            }

                            // construct event description to be passed into siegeEnd
                            string siegeDescription = "On this day of Our Lord the forces of ";
                            siegeDescription += thisSiege.GetBesiegingPlayer().firstName + " " + thisSiege.GetBesiegingPlayer().familyName;
                            siegeDescription += " have chosen to abandon the siege of " + thisSiege.GetFief().name;
                            siegeDescription += ". " + thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;
                            siegeDescription += " retains ownership of the fief.";

                            this.SiegeEnd(thisSiege, false, siegeDescription);
                        }

                    }
                }
            }

            if (proceedWithMove)
            {
                // move character
                success = ch.MoveCharacter(target, cost);
            }

            return success;
        }

        /// <summary>
        /// Gets travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="source">Source fief</param>
        /// <param name="target">Target fief</param>
        private double getTravelCost(Fief source, Fief target, string armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (source.terrain.travelCost + target.terrain.travelCost) / 2;

            // apply season modifier
            cost = cost * Globals_Game.clock.CalcSeasonTravMod();

            // if necessary, apply army modifier
            if (!String.IsNullOrWhiteSpace(armyID))
            {
                cost = cost * Globals_Game.armyMasterList[armyID].CalcMovementModifier();
            }

            return cost;
        } */

        /// <summary>
        /// Refreshes travel display screen
        /// </summary>
        private void RefreshTravelContainer()
        {
            // get current fief
            Fief thisFief = Globals_Client.myPlayerCharacter.location;

            // string[] to hold direction text
            string[] directions = new string[] { "NE", "E", "SE", "SW", "W", "NW" };
            // Button[] to hold corresponding travel buttons
            Button[] travelBtns = new Button[] { travel_NE_btn, travel_E_btn, travel_SE_btn, travel_SW_btn, travel_W_btn, travel_NW_btn };

            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + thisFief.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")" + "\r\n" + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name;

            for (int i = 0; i < directions.Length; i++)
            {
                // retrieve target fief for that direction
                Fief target = Globals_Game.gameMap.GetFief(thisFief, directions[i]);
                // display fief details and travel cost
                if (target != null)
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\n";
                    travelBtns[i].Text += target.name + " (" + target.id + ")\r\n";
                    travelBtns[i].Text += target.province.name + ", " + target.province.kingdom.name + "\r\n\r\n";
                    travelBtns[i].Text += "Cost: " + thisFief.getTravelCost(target);
                }
                else
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\nNo fief present";
                }
            }

            // set text for informational labels
            this.travelLocationLabel.Text = "You are here: " + thisFief.name + " (" + thisFief.id + ")";
            this.travelDaysLabel.Text = "Your remaining days: " + Globals_Client.myPlayerCharacter.days;

            // set text for 'enter/exit keep' button, depending on whether player in/out of keep
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
            }

            // enable all controls
            this.travel_E_btn.Enabled = true;
            this.travel_Home_btn.Enabled = true;
            this.travel_NE_btn.Enabled = true;
            this.travel_NW_btn.Enabled = true;
            this.travel_SE_btn.Enabled = true;
            this.travel_SW_btn.Enabled = true;
            this.travel_W_btn.Enabled = true;
            this.travelCampBtn.Enabled = true;
            this.travelCampDaysTextBox.Enabled = true;
            this.travelExamineArmiesBtn.Enabled = true;
            this.travelMoveToBtn.Enabled = true;
            this.travelMoveToTextBox.Enabled = true;
            this.travelRouteBtn.Enabled = true;
            this.travelRouteTextBox.Enabled = true;
            this.enterKeepBtn.Enabled = true;
            this.listOutsideKeepBtn.Enabled = true;
            this.visitCourtBtn1.Enabled = true;
            this.visitTavernBtn.Enabled = true;

            // clear existing data in TextBoxes
            this.travelMoveToTextBox.Text = "";
            this.travelCampDaysTextBox.Text = "";
            this.travelRouteTextBox.Text = "";

            // check to see if fief is besieged and, if so, disable various controls
            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                // check to see if are inside/outside keep
                if (Globals_Client.myPlayerCharacter.inKeep)
                {
                    // if inside keep, disable all controls except tavern and court
                    this.travel_E_btn.Enabled = false;
                    this.travel_Home_btn.Enabled = false;
                    this.travel_NE_btn.Enabled = false;
                    this.travel_NW_btn.Enabled = false;
                    this.travel_SE_btn.Enabled = false;
                    this.travel_SW_btn.Enabled = false;
                    this.travel_W_btn.Enabled = false;
                    this.travelCampBtn.Enabled = false;
                    this.travelCampDaysTextBox.Enabled = false;
                    this.travelExamineArmiesBtn.Enabled = false;
                    this.travelMoveToBtn.Enabled = false;
                    this.travelMoveToTextBox.Enabled = false;
                    this.travelRouteBtn.Enabled = false;
                    this.travelRouteTextBox.Enabled = false;
                    this.enterKeepBtn.Enabled = false;
                    this.listOutsideKeepBtn.Enabled = false;
                }

                else
                {
                    // if outside keep, disable tavern, court and 'enter keep' but leave all others enabled
                    this.enterKeepBtn.Enabled = false;
                    this.visitCourtBtn1.Enabled = false;
                    this.visitTavernBtn.Enabled = false;
                }
            }

        }

        // Is this needed?

        /*
        /// <summary>
        /// Moves character to the target fief, through intervening fiefs (stored in goTo queue)
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to be moved</param>
        private bool CharacterMultiMove(Character ch)
        {
            bool success = false;
            double travelCost = 0;
            int steps = ch.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                // get travel cost
                travelCost = ch.location.getTravelCost(ch.goTo.Peek(), ch.armyID);
                // attempt to move character
                success = ch.MoveCharacter(ch.goTo.Peek(), travelCost);
                // if move successfull, remove fief from goTo queue
                if (success)
                {
                    ch.goTo.Dequeue();
                }
                // if not successfull, exit loop
                else
                {
                    break;
                }
            }

            if (ch == Globals_Client.myPlayerCharacter)
            {
                // if player has moved, indicate success
                if (ch.goTo.Count < steps)
                {
                    success = true;
                }
            }

            return success;

        } */

        /// <summary>
        /// Moves a character to a specified fief using the shortest path
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void MoveTo(string whichScreen)
        {
            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceMoveToTextBox;
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseMoveToTextBox;
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelMoveToTextBox;
            }

            // perform move
            Globals_Client.charToView.MoveTo(myTextBox.Text.ToUpper());

            // refresh appropriate screen
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                this.RefreshMeetingPlaceDisplay(whichScreen); ;
            }
            else if (whichScreen.Equals("house"))
            {
                this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
            else if (whichScreen.Equals("travel"))
            {
                this.RefreshTravelContainer();
            }

        }

        /// <summary>
        /// Allows the character to remain in their current location for the specified
        /// number of days, incrementing bailiffDaysInFief if appropriate
        /// </summary>
        /// <param name="ch">The Character who wishes to camp</param>
        /// <param name="campDays">Number of days to camp</param>
        public void CampWaitHere(Character ch, byte campDays)
        {
            // perform camp
            bool proceed = ch.CampWaitHere(campDays);

            // refresh display
            if (proceed)
            {
                if (ch == Globals_Client.myPlayerCharacter)
                {
                    this.RefreshTravelContainer();
                }
                else
                {
                    this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                }
            }

        }

        /// <summary>
        /// Allows a character to be moved along a specific route by using direction codes
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void TakeThisRoute(string whichScreen)
        {
            // get appropriate TextBox
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceRouteTextBox;
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseRouteTextBox;
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelRouteTextBox;
            }

            // get list of directions
            string[] directions = myTextBox.Text.Split(',').ToArray<string>();

            // perform movement
            Globals_Client.charToView.TakeThisRoute(directions);

            // refresh appropriate screen
            this.RefreshCurrentScreen();
        }
    }
}
