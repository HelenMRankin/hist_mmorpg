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

using hist_mmorpg.Properties;

namespace hist_mmorpg
{
    /// <summary>
    /// Main user interface component
    /// </summary>
    public partial class Form1 : Form, Game_Observer
    {

        

        // ------------------- TEST METHODS

        /// <summary>
 

        /// <summary>
        /// Responds to the click event of any of the 'edit trait effects' buttons
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditTraitEffBtn_Click(object sender, EventArgs e)
        {
            string effName = null;
            double effLvl = 0;
            bool effectsChanged = false;

            // get button and tag
            Button thisButton = (sender as Button);
            string operation = thisButton.Tag.ToString();

            try
            {
                // get effects collection
                Dictionary<string, double> effects = new Dictionary<string, double>();
                for (int i = 0; i < this.adminEditTraitEffsListView.Items.Count; i++)
                {
                    effects.Add(this.adminEditTraitEffsListView.Items[i].SubItems[0].Text,
                        Convert.ToDouble(this.adminEditTraitEffsListView.Items[i].SubItems[1].Text));
                }

                // get selected effect
                effName = this.adminEditTraitEffTextBox.Text;
                if (!String.IsNullOrWhiteSpace(effName))
                {
                    effLvl = Convert.ToDouble(this.adminEditTraitEfflvlTextBox.Text);
                }

                if (effLvl > 0)
                {
                    // perform operation
                    switch (operation)
                    {
                        // change selected effect
                        case "chaEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects[effName] = effLvl;
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        case "addEffect":
                            // check effect present in collection
                            if (!effects.ContainsKey(effName))
                            {
                                effects.Add(effName, effLvl);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " already exists.  Operation cancelled.");
                                }
                            }
                            break;
                        case "delEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects.Remove(effName);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (effectsChanged)
                    {
                        this.RefreshTraitEffectsList(effects);
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Effects updated.");
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
        /// Responds to the click event of the adminEditSaveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSaveBtn_Click(object sender, EventArgs e)
        {
            bool success = false;

            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    success = this.SaveCharacterEdit(objectType);
                    break;

                case "NPC":
                    success = this.SaveCharacterEdit(objectType);
                    break;

                case "Fief":
                    success = this.SavePlaceEdit(objectType);
                    break;

                case "Province":
                    success = this.SavePlaceEdit(objectType);
                    break;

                case "Kingdom":
                    success = this.SavePlaceEdit(objectType);
                    break;

                case "Trait":
                    success = this.SaveTraitEdit();
                    break;

                case "Army":
                    success = this.SaveArmyEdit();
                    break;

                default:
                    break;
            }

            if (success)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Object saved.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'edit object' MenuItems
        /// displaying the appropriate screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditObjectMenuItem_Click(object sender, EventArgs e)
        {
            // get MenuItem
            ToolStripMenuItem thisItem = (sender as ToolStripMenuItem);
            string objectType = thisItem.Tag.ToString();

            // display edit object screen
            Globals_Client.containerToView = this.adminEditContainer;
            Globals_Client.containerToView.BringToFront();

            // set get/save button tag to identify object type (for retrieving and saving object)
            this.adminEditGetBtn.Tag = objectType;
            this.adminEditSaveBtn.Tag = objectType;
            this.adminEditCancelBtn.Tag = objectType;

            // clear previous object ID from TextBox
            this.adminEditTextBox.Text = "";

            // change admin edit control properties to match object type
            this.adminEditGetBtn.Text = "Get " + objectType;
            this.adminEditSaveBtn.Text = "Save " + objectType;
            this.adminEditLabel.Text = objectType + " ID";

            // display appropriate panel
            switch (objectType)
            {
                case "PC":
                    // clear previous data
                    this.RefreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit pc panel
                    this.adminEditCharPcPanel.BringToFront();
                    break;
                case "NPC":
                    // clear previous data
                    this.RefreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit npc panel
                    this.adminEditCharNpcPanel.BringToFront();
                    break;
                case "Fief":
                    // clear previous data
                    this.RefreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit fief panel
                    this.adminEditFiefPanel.BringToFront();
                    break;
                case "Province":
                    // clear previous data
                    this.RefreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit province panel
                    this.adminEditProvPanel.BringToFront();
                    break;
                case "Kingdom":
                    // clear previous data
                    this.RefreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit kingdom panel
                    this.adminEditKingPanel.BringToFront();
                    break;
                case "Trait":
                    // clear previous data
                    this.RefreshTraitEdit();
                    this.adminEditTraitIdTextBox.ReadOnly = true;
                    // display edit trait panel
                    this.adminEditTraitPanel.BringToFront();
                    break;
                case "Army":
                    // clear previous data
                    this.RefreshArmyEdit();
                    this.adminEditArmyIdTextBox.ReadOnly = true;
                    // display edit army panel
                    this.adminEditArmyPanel.BringToFront();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to the click event of any of the adminEditGetBtn button
        /// retrieving the specified object
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditGetBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // get specified object
            switch (objectType)
            {
                case "PC":
                    // get PC
                    PlayerCharacter thisPC = null;
                    if (Globals_Game.pcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisPC = Globals_Game.pcMasterList[this.adminEditTextBox.Text];
                    }

                    // display PC details
                    if (thisPC != null)
                    {
                        this.RefreshCharEdit(thisPC);
                    }
                    break;
                case "NPC":
                    // get NPC
                    NonPlayerCharacter thisNPC = null;
                    if (Globals_Game.npcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisNPC = Globals_Game.npcMasterList[this.adminEditTextBox.Text];
                    }

                    // display NPC details
                    if (thisNPC != null)
                    {
                        this.RefreshCharEdit(thisNPC);
                    }
                    break;

                case "Fief":
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.adminEditTextBox.Text];
                    }

                    // display fief details
                    if (thisFief != null)
                    {
                        this.RefreshPlaceEdit(thisFief);
                    }
                    break;

                case "Province":
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.adminEditTextBox.Text];
                    }

                    // display province details
                    if (thisProv != null)
                    {
                        this.RefreshPlaceEdit(thisProv);
                    }
                    break;

                case "Kingdom":
                    // get kingdom
                    Kingdom thiskingdom = null;
                    if (Globals_Game.kingdomMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thiskingdom = Globals_Game.kingdomMasterList[this.adminEditTextBox.Text];
                    }

                    // display kingdom details
                    if (thiskingdom != null)
                    {
                        this.RefreshPlaceEdit(thiskingdom);
                    }
                    break;

                case "Trait":
                    // get trait
                    Trait thisTrait = null;
                    if (Globals_Game.traitMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisTrait = Globals_Game.traitMasterList[this.adminEditTextBox.Text];
                    }

                    // display trait details
                    if (thisTrait != null)
                    {
                        this.RefreshTraitEdit(thisTrait);
                    }
                    break;

                case "Army":
                    // get army
                    Army thisArmy = null;
                    if (Globals_Game.armyMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisArmy = Globals_Game.armyMasterList[this.adminEditTextBox.Text];
                    }

                    // display army details
                    if (thisArmy != null)
                    {
                        this.RefreshArmyEdit(thisArmy);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to the click event of the adminEditCancelBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditCancelBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    this.RefreshCharEdit();
                    break;

                case "NPC":
                    this.RefreshCharEdit();
                    break;

                case "Fief":
                    this.RefreshPlaceEdit();
                    break;

                case "Province":
                    this.RefreshPlaceEdit();
                    break;

                case "Kingdom":
                    this.RefreshPlaceEdit();
                    break;

                case "Trait":
                    this.RefreshTraitEdit();
                    break;

                case "Army":
                    this.RefreshArmyEdit();
                    break;

                default:
                    break;
            }

            // clear ID box
            this.adminEditTextBox.Clear();
        }

        // ------------------- SIEGE/PILLAGE/REBELLION

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the siegeListView object,
        /// allowing details of the selected siege to be displayed
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get siege to view
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                Globals_Client.siegeToView = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];
            }

            if (Globals_Client.siegeToView != null)
            {
                Army besiegingArmy = Globals_Client.siegeToView.GetBesiegingArmy();
                PlayerCharacter besiegingPlayer = Globals_Client.siegeToView.GetBesiegingPlayer();
                bool playerIsBesieger = (Globals_Client.myPlayerCharacter == besiegingPlayer);

                // display data for selected siege
                this.siegeTextBox.Text = Globals_Client.siegeToView.DisplaySiegeData();

                // if player is besieger
                if (playerIsBesieger)
                {
                    // enable various controls
                    this.siegeReduceBtn.Enabled = true;
                    this.siegeEndBtn.Enabled = true;

                    // if besieging army has a leader
                    if (!String.IsNullOrWhiteSpace(besiegingArmy.leader))
                    {
                        // enable proactive controls (storm, negotiate)
                        this.siegeNegotiateBtn.Enabled = true;
                        this.siegeStormBtn.Enabled = true;
                    }

                    // if besieging army has no leader
                    else
                    {
                        // disable proactive controls (storm, negotiate)
                        this.siegeNegotiateBtn.Enabled = false;
                        this.siegeStormBtn.Enabled = false;
                    }
                }

                // if player is defender
                else
                {
                    // disable various controls
                    this.siegeNegotiateBtn.Enabled = false;
                    this.siegeStormBtn.Enabled = false;
                    this.siegeReduceBtn.Enabled = false;
                    this.siegeEndBtn.Enabled = false;
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armySiegeBtn button
        /// instigating the siege of a fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armySiegeBtn_Click(object sender, EventArgs e)
        {
            // check army selected
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

                // get fief
                Fief thisFief = thisArmy.GetLocation();

                // do various checks
                proceed = Pillage_Siege.ChecksBeforePillageSiege(thisArmy, thisFief, "siege");

                // process siege
                if (proceed)
                {
                    this.SiegeStart(thisArmy, thisFief);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the viewMySiegesToolStripMenuItem
        /// displaying the siege management screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewMySiegesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.siegeToView = null;
            this.RefreshSiegeContainer();
        }

        /// <summary>
        /// Responds to any of the click events of the siegeRound buttons
        /// processing a single siege round of specified type
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeRoundBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get tag from button
                Button button = sender as Button;
                string roundType = button.Tag.ToString();

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = thisSiege.ChecksBeforeSiegeOperation();

                if (proceed)
                {
                    // process siege round of specified type
                    this.SiegeReductionRound(thisSiege, roundType);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No siege selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the siegeEndBtn button
        /// dismantling the selected siege
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeEndBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = thisSiege.ChecksBeforeSiegeOperation("end");

                if (proceed)
                {
                    // construct event description to be passed into siegeEnd
                    string siegeDescription = "On this day of Our Lord the forces of ";
                    siegeDescription += thisSiege.GetBesiegingPlayer().firstName + " " + thisSiege.GetBesiegingPlayer().familyName;
                    siegeDescription += " have chosen to abandon the siege of " + thisSiege.GetFief().name;
                    siegeDescription += ". " + thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;
                    siegeDescription += " retains ownership of the fief.";

                    // process siege reduction round
                    this.SiegeEnd(thisSiege, false, siegeDescription);

                    //refresh screen
                    this.RefreshCurrentScreen();
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No siege selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the armyPillageBtn button
        /// instigating the pillage of a fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyPillageBtn_Click(object sender, EventArgs e)
        {
            // check army selected
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

                // get fief
                Fief thisFief = thisArmy.GetLocation();

                // do various checks
                proceed = Pillage_Siege.ChecksBeforePillageSiege(thisArmy, thisFief);

                // process pillage
                if (proceed)
                {
                    Pillage_Siege.PillageFief(thisArmy, thisFief);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the Click event of the armyQuellRebellionBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyQuellRebellionBtn_Click(object sender, EventArgs e)
        {
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = null;
                if (Globals_Game.armyMasterList.ContainsKey(this.armyListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisArmy != null)
                {
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(thisArmy.location))
                    {
                        thisFief = Globals_Game.fiefMasterList[thisArmy.location];
                    }

                    if (thisFief != null)
                    {
                        // do various checks
                        proceed = Pillage_Siege.ChecksBeforePillageSiege(thisArmy, thisFief, circumstance: "quellRebellion");

                        if (proceed)
                        {
                            bool quellSuccess = thisFief.QuellRebellion(thisArmy);

                            // quell successful, pillage fief
                            if (quellSuccess)
                            {
                                // pillage the fief
                                Pillage_Siege.ProcessPillage(thisFief, thisArmy, "quellRebellion");
                            }

                            // if not successful, retreat army
                            else
                            {
                                // retreat army 1 hex
                                thisArmy.ProcessRetreat(1);
                            }
                        }
                    }
                }

            }
        }

        // ------------------- ROYAL/OVERLORD FUNCTIONS

        /// <summary>
        /// Responds to the click event of the royalGiftsToolStripMenuItem
        /// which displays royal gifts screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display royal gifts screen
            this.RefreshRoyalGiftsContainer();
            Globals_Client.containerToView = this.royalGiftsContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of either of the royal gifts ListViews
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Province thisProv = null;
            Fief thisFief = null;
            Position thisPos = null;

            // get ListView tag
            ListView listview = sender as ListView;
            string whichView = listview.Tag.ToString();

            // check for and correct 'loop backs' due to listview item deselection
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                }
            }

            // get selected place or position
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    if (Globals_Game.provinceMasterList.ContainsKey(this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    if (Globals_Game.fiefMasterList.ContainsKey(this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    // get position
                    if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                    {
                        thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                    }
                }
            }

            // deselect any selected items in other listView
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
            }

            // set button text and tag
            if (whichView.Equals("province"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Province Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Province Title";
                if (thisProv != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "province|" + thisProv.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "province|" + thisProv.id;
                }
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Fief Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Fief Title";
                if (thisFief != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsGiftFiefBtn.Tag = "fief|" + thisFief.id;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (thisPos != null)
                {
                    this.royalGiftsPositionBtn.Tag = thisPos.id;
                }
            }

            // enable/disable controls as appropriate

            // check to see if viewer is king or herald
            if (!Globals_Client.myPlayerCharacter.CheckIsHerald())
            {
                // provinces
                if (whichView.Equals("province"))
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        if (thisProv != null)
                        {
                            // revoke title button
                            if (thisProv.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }
                        }
                    }

                    // 'grant title' button
                    this.royalGiftsGrantTitleBtn.Enabled = true;

                    // gift fief button
                    this.royalGiftsGiftFiefBtn.Enabled = false;

                    // position buttons
                    this.royalGiftsPositionBtn.Enabled = false;
                    this.royalGiftsPositionRemoveBtn.Enabled = false;
                }

                // fiefs
                else if (whichView.Equals("fief"))
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        if (thisFief != null)
                        {
                            // revoke title button
                            if (thisFief.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = true;

                            // 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = true;

                            // position buttons
                            this.royalGiftsPositionBtn.Enabled = false;
                            this.royalGiftsPositionRemoveBtn.Enabled = false;
                        }
                    }
                }

                // positions
                else if (whichView.Equals("position"))
                {
                    if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        if (thisPos != null)
                        {
                            // bestow position button
                            this.royalGiftsPositionBtn.Enabled = true;

                            // remove position button, enabled if there is a current holder
                            if (!String.IsNullOrWhiteSpace(thisPos.officeHolder))
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = true;
                            }
                            else
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = false;
                            }

                            // revoke title button
                            this.royalGiftsRevokeTitleBtn.Enabled = false;

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = false;

                            // always enable 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = false;
                        }
                    }
                }
            }

            // don't enable controls if herald
            else
            {
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
            }

            // give focus back to appropriate listview
            if (whichView.Equals("province"))
            {
                this.royalGiftsProvListView.Focus();
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsFiefListView.Focus();
            }
            else if (whichView.Equals("position"))
            {
                this.royalGiftsPositionListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the click event of either the royalGiftsGrantTitleBtn button
        /// or the royalGiftsGiftFiefBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsBtn_Click(object sender, EventArgs e)
        {
            // get gift type and place id from button tag and name
            Button button = sender as Button;
            string giftID = button.Tag.ToString();
            string giftType = null;

            if (button.Name.ToString().Equals("royalGiftsGrantTitleBtn"))
            {
                giftType = "royalGiftTitle";
            }
            else if (button.Name.ToString().Equals("royalGiftsGiftFiefBtn"))
            {
                giftType = "royalGiftFief";
            }
            else if (button.Name.ToString().Equals("royalGiftsPositionBtn"))
            {
                giftType = "royalGiftPosition";
            }

            if (!String.IsNullOrWhiteSpace(giftType))
            {
                // check for previously opened SelectionForm and close if necessary
                if (Application.OpenForms.OfType<SelectionForm>().Any())
                {
                    Application.OpenForms.OfType<SelectionForm>().First().Close();
                }

                // open new SelectionForm
                SelectionForm royalGiftSelection = null;
                // if gifting place or place title
                if (!giftType.Equals("royalGiftPosition"))
                {
                    royalGiftSelection = new SelectionForm(this, giftType, place: giftID);
                }

                // if bestowing position
                else
                {
                    royalGiftSelection = new SelectionForm(this, giftType, posID: Convert.ToByte(giftID));
                }
                royalGiftSelection.Show();
            }

        }

        /// <summary>
        /// Responds to the Click event of the manageProvincesToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void manageProvincesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear existing provinceToView
            Globals_Client.provinceToView = null;

            // display royal gifts screen
            this.RefreshProvinceContainer();

            // display household affairs screen
            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the provinceProvListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceProvListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // refresh fief list
                    this.RefreshProvinceFiefList(thisProvince);

                    // populate provinceTaxTextBox
                    this.provinceTaxTextBox.Text = thisProvince.taxRate.ToString();

                    // enable controls
                    this.provinceTaxBtn.Enabled = true;
                    this.provinceTaxTextBox.Enabled = true;
                    this.provinceChallengeBtn.Enabled = true;

                    // set provinceToView
                    Globals_Client.provinceToView = thisProvince;
                }
            }
        }

        /// <summary>
        /// Responds to the Click event of the provinceTaxBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceTaxBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                bool rateChanged = false;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // keep track of whether tax has changed
                    double originalRate = thisProvince.taxRate;

                    try
                    {
                        // get new rate
                        Double newTax = Convert.ToDouble(this.provinceTaxTextBox.Text);

                        // if rate changed, commit new rate
                        if (newTax != originalRate)
                        {
                            // adjust tax rate
                            thisProvince.AdjustTaxRate(newTax);
                            rateChanged = true;

                            // display confirmation message
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Province tax rate changed.");
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
                    finally
                    {
                        // refresh screen if expenditure changed
                        if (rateChanged)
                        {
                            // refresh display
                            this.RefreshCurrentScreen();
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsPositionRemoveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsPositionRemoveBtn_Click(object sender, EventArgs e)
        {
            if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
            {
                // get position
                Position thisPos = null;
                if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                {
                    thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                }

                if (thisPos != null)
                {
                    // get current holder
                    PlayerCharacter currentHolder = thisPos.GetOfficeHolder();

                    // remove from position
                    if (currentHolder != null)
                    {
                        thisPos.RemoveFromOffice(currentHolder);

                        // refresh screen
                        this.RefreshCurrentScreen();
                    }
                }

            }
        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsRevokeTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsRevokeTitleBtn_Click(object sender, EventArgs e)
        {
            bool refreshMenus = false;

            // get place type and id from button tag
            Button button = sender as Button;
            string[] placeDetails = button.Tag.ToString().Split('|');

            // fiefs
            if (placeDetails[0].Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisFief = Globals_Game.fiefMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisFief != null)
                    {
                        Globals_Client.myPlayerCharacter.GrantTitle(Globals_Client.myPlayerCharacter, thisFief);

                        // refresh screen
                        this.RefreshCurrentScreen();
                    }
                }
            }

            // provinces
            else if (placeDetails[0].Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisProv = Globals_Game.provinceMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisProv != null)
                    {
                        refreshMenus = Globals_Client.myPlayerCharacter.GrantTitle(Globals_Client.myPlayerCharacter, thisProv);

                        // check if menus need to be refreshed (due to ownership changes)
                        if (refreshMenus)
                        {
                            this.InitMenuPermissions();
                        }

                        // refresh screen
                        this.RefreshCurrentScreen();
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the Click event of the provinceChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get kingdom
                Kingdom targetKingdom = null;
                if (Globals_Game.kingdomMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[4].Text))
                {
                    targetKingdom = Globals_Game.kingdomMasterList[this.provinceProvListView.SelectedItems[0].SubItems[4].Text];
                }

                if (targetKingdom != null)
                {
                    targetKingdom.LodgeOwnershipChallenge();
                }

                this.provinceProvListView.Focus();
            }
        }

        // ------------------- TRAVEL SCREEN

        /// <summary>
        /// Responds to the click event of any routeBtn buttons invoking the takeThisRoute method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void routeBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the movement command occurred
            string whichScreen = button.Tag.ToString();

            // perform move
            this.TakeThisRoute(whichScreen);
        }

        /// <summary>
        /// Responds to the click event of the travelExamineArmiesBtn button
        /// displaying a list of all armies in the Player's current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // examine armies
            this.ExamineArmiesInFief(Globals_Client.myPlayerCharacter);
        }

        /// <summary>
        /// Responds to the click event of the travelCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelCampBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get days to camp
                byte campDays = Convert.ToByte(this.travelCampDaysTextBox.Text);

                // camp
                this.CampWaitHere(Globals_Client.myPlayerCharacter, campDays);
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
            finally
            {
                // refresh display
                this.RefreshTravelContainer();
            }

        }

        /// <summary>
        /// Responds to the click event of any of the travel buttons
        /// which attempts to move the player to the target fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelBtnClick(object sender, EventArgs e)
        {
            bool success = false;
            // necessary in order to be able to access button tag
            Button button = sender as Button;
            // get target fief using travel button tag (contains direction string)
            Fief targetFief = Globals_Game.gameMap.GetFief(Globals_Client.myPlayerCharacter.location, button.Tag.ToString());

            if (targetFief != null)
            {
                // get travel cost
                double travelCost = Globals_Client.myPlayerCharacter.location.getTravelCost(targetFief, Globals_Client.myPlayerCharacter.armyID);

                // attempt to move player to target fief
                success = Globals_Client.myPlayerCharacter.MoveCharacter(targetFief, travelCost);

                // if move successfull, refresh travel display
                if (success)
                {
                    Globals_Client.fiefToView = targetFief;
                    this.RefreshTravelContainer();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the enterKeepBtn button
        /// which causes the player (and entourage) to enter/exit the keep and
        /// refreshes the travel screen, setting appropriate text for the enterKeepBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void enterKeepBtn_Click(object sender, EventArgs e)
        {
            // attempt to enter/exit keep
            bool success = Globals_Client.myPlayerCharacter.ExitEnterKeep();

            // if successful
            if (success)
            {
                // refresh display
                this.RefreshTravelContainer();
            }

        }

        /// <summary>
        /// Responds to the click event of any moveTo buttons invoking the moveTo method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void moveToBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the movement command occurred
            string whichScreen = button.Tag.ToString();

            // perform move
            this.MoveTo(whichScreen);
        }

        /// <summary>
        /// Responds to the click event of any of the 'visit meeting place' buttons
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void VisitMeetingPlace(object sender, EventArgs e)
        {
            bool success = true;

            // get button
            Button thisButton = (sender as Button);
            string place = thisButton.Tag.ToString();

            // enter/exit keep if required
            switch (place)
            {
                case "court":
                    if (!Globals_Client.myPlayerCharacter.inKeep)
                    {
                        success = Globals_Client.myPlayerCharacter.EnterKeep();
                    }
                    break;
                default:
                    if (Globals_Client.myPlayerCharacter.inKeep)
                    {
                        success = Globals_Client.myPlayerCharacter.ExitKeep();
                    }
                    break;
            }

            if (success)
            {
                // set button tags to reflect which meeting place
                this.hireNPC_Btn.Tag = place;
                this.meetingPlaceMoveToBtn.Tag = place;
                this.meetingPlaceRouteBtn.Tag = place;
                this.meetingPlaceEntourageBtn.Tag = place;

                // refresh outside keep screen 
                this.RefreshMeetingPlaceDisplay(place);

                // display tavern screen
                Globals_Client.containerToView = this.meetingPlaceContainer;
                Globals_Client.containerToView.BringToFront();
            }
        }

        /// <summary>
        /// Responds to the click event of the navigateToolStripMenuItem
        /// which refreshes and displays the navigation screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ensure reflects player's location
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;

            // refresh navigation data
            Globals_Client.fiefToView = Globals_Client.myPlayerCharacter.location;
            this.RefreshTravelContainer();

            // show navigation screen
            Globals_Client.containerToView = this.travelContainer;
            Globals_Client.containerToView.BringToFront();
        }

        // ------------------- MEETING PLACE SCREEN

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the meetingPlaceCharsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceCharsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in the fief
            for (int i = 0; i < Globals_Client.fiefToView.charactersInFief.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.fiefToView.charactersInFief[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.fiefToView.charactersInFief[i];

                        // check whether is this PC's employee or family
                        if (Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.fiefToView.charactersInFief[i]))
                        {
                            // see if is in entourage to set text of entourage button
                            if ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).inEntourage)
                            {
                                this.meetingPlaceEntourageBtn.Text = "Remove From Entourage";
                            }
                            else
                            {
                                this.meetingPlaceEntourageBtn.Text = "Add To Entourage";
                            }

                            // enable 'move to' controls
                            this.meetingPlaceMoveToBtn.Enabled = true;
                            this.meetingPlaceMoveToTextBox.Enabled = true;
                            this.meetingPlaceRouteBtn.Enabled = true;
                            this.meetingPlaceRouteTextBox.Enabled = true;
                            this.meetingPlaceEntourageBtn.Enabled = true;

                            // disable marriage proposals
                            this.meetingPlaceProposeBtn.Enabled = false;
                            this.meetingPlaceProposeTextBox.Text = "";
                            this.meetingPlaceProposeTextBox.Enabled = false;

                            // if is employee
                            if ((!String.IsNullOrWhiteSpace((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer))
                                && ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer.Equals(Globals_Client.myPlayerCharacter.charID)))
                            {
                                // set appropriate text for hire/fire button, and enable it
                                this.hireNPC_Btn.Text = "Fire NPC";
                                this.hireNPC_Btn.Enabled = true;
                                // disable 'salary offer' text box
                                this.hireNPC_TextBox.Visible = false;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }
                        }

                        // if is not employee or family
                        else
                        {
                            // set appropriate text for hire/fire controls, and enable them
                            this.hireNPC_Btn.Text = "Hire NPC";
                            this.hireNPC_TextBox.Visible = true;

                            // can only employ men (non-PCs)
                            if (charToDisplay.CheckCanHire(Globals_Client.myPlayerCharacter))
                            {
                                this.hireNPC_Btn.Enabled = true;
                                this.hireNPC_TextBox.Enabled = true;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }

                            // disable 'move to' and entourage controls
                            this.meetingPlaceMoveToBtn.Enabled = false;
                            this.meetingPlaceMoveToTextBox.Enabled = false;
                            this.meetingPlaceRouteBtn.Enabled = false;
                            this.meetingPlaceRouteTextBox.Enabled = false;
                            this.meetingPlaceEntourageBtn.Enabled = false;

                            // checks for enabling marriage proposals
                            if (((!String.IsNullOrWhiteSpace(charToDisplay.spouse)) || (charToDisplay.isMale)) || (!String.IsNullOrWhiteSpace(charToDisplay.fiancee)))
                            {
                                // disable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = false;
                                this.meetingPlaceProposeTextBox.Text = "";
                                this.meetingPlaceProposeTextBox.Enabled = false;
                            }
                            else
                            {
                                // enable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = true;
                                this.meetingPlaceProposeTextBox.Text = Globals_Client.myPlayerCharacter.charID;
                                this.meetingPlaceProposeTextBox.Enabled = true;
                            }
                        }
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.DisplayCharacter(charToDisplay, Globals_Client.myPlayerCharacter);
                this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
                this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Responds to the click event of any hireNPC button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void hireNPC_Btn_Click(object sender, EventArgs e)
        {
            bool amHiring = false;
            bool isHired = false;

            // get hireNPC_Btn tag (shows which meeting place are in)
            string place = Convert.ToString(((Button)sender).Tag);

            // if selected NPC is not a current employee
            if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.charToView))
            {
                amHiring = true;

                try
                {
                    // get offer amount
                    UInt32 newOffer = Convert.ToUInt32(this.hireNPC_TextBox.Text);
                    // submit offer
                    isHired = Globals_Client.myPlayerCharacter.ProcessEmployOffer((Globals_Client.charToView as NonPlayerCharacter), newOffer);

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

            // if selected NPC is already an employee
            else
            {
                // fire NPC
                Globals_Client.myPlayerCharacter.FireNPC(Globals_Client.charToView as NonPlayerCharacter);
            }

            // refresh appropriate screen
            // if firing an NPC
            if (!amHiring)
            {
                if (place.Equals("house"))
                {
                    this.RefreshHouseholdDisplay();
                }
                else
                {
                    this.RefreshMeetingPlaceDisplay(place, Globals_Client.charToView);
                }
            }
            // if hiring an NPC
            else
            {
                // if in the tavern and NPC is hired, refresh whole screen (NPC removed from list)
                if ((isHired) && (place.Equals("tavern")))
                {
                    this.RefreshMeetingPlaceDisplay(place);
                }
                else
                {
                    this.RefreshMeetingPlaceDisplay(place, Globals_Client.charToView);
                    //this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(Globals_Client.charToView);
                }
            }

        }

        /// <summary>
        /// Responds to the click event of any entourage button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void entourageBtn_Click(object sender, EventArgs e)
        {
            // for messages
            string toDisplay = "";

            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the command occurred
            string whichScreen = button.Tag.ToString();

            // check which action to perform
            // if is in entourage, remove
            if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
            {
                Globals_Client.myPlayerCharacter.RemoveFromEntourage((Globals_Client.charToView as NonPlayerCharacter));
            }

            // if is not in entourage, add
            else
            {
                // check to see if NPC is army leader
                // if not leader, proceed
                if (String.IsNullOrWhiteSpace(Globals_Client.charToView.armyID))
                {
                    // add to entourage
                    Globals_Client.myPlayerCharacter.AddToEntourage((Globals_Client.charToView as NonPlayerCharacter));

                }

                // if is army leader, can't add to entourage
                else
                {
                    toDisplay += "Sorry, milord, this person is an army leader\r\n";
                    toDisplay += "and, therefore, cannot be added to your entourage.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
            }

            // refresh appropriate screen
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                this.RefreshMeetingPlaceDisplay(whichScreen); ;
            }
            else if (whichScreen.Equals("house"))
            {
                this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
        }

        // ------------------- MARRIAGE

        /// <summary>
        /// Responds to the click event of the meetingPlaceProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceProposeBtn_Click(object sender, EventArgs e)
        {
            // get entry
            if (this.meetingPlaceCharsListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                Character bride = null;
                Character groom = null;
                string brideID = "";
                string groomID = "";

                if (String.IsNullOrWhiteSpace(this.meetingPlaceProposeTextBox.Text))
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                    }
                }
                else
                {
                    // get bride and groom IDs
                    brideID = this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text;
                    groomID = this.houseProposeGroomTextBox.Text;

                    // get bride
                    if (Globals_Game.npcMasterList.ContainsKey(brideID))
                    {
                        bride = Globals_Game.npcMasterList[brideID];
                    }

                    if (bride == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                        }
                    }
                    else
                    {
                        // get groom
                        if (Globals_Game.npcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.npcMasterList[groomID];
                        }
                        else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.pcMasterList[groomID];
                        }

                        if (groom == null)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                            }
                        }
                        else
                        {
                            // carry out conditional checks
                            proceed = groom.ChecksBeforeProposal(bride);

                            // if checks OK, process proposal
                            if (proceed)
                            {
                                groom.ProposeMarriage(bride);
                            }
                        }

                    }
                }
            }

            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a prospective bride.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of either of the proposal reply buttons,
        /// sending the appropriate reply
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalProposalReplyButton_Click(object sender, EventArgs e)
        {
            if (this.journalListView.SelectedItems.Count > 0)
            {
                bool proposalAccepted = false;

                // get tag from button
                Button button = sender as Button;
                string reply = button.Tag.ToString();

                // set appropriate response
                if (reply.Equals("accept"))
                {
                    proposalAccepted = true;
                }

                // get JournalEntry
                JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;

                // send reply
                this.ReplyToProposal(thisJentry, proposalAccepted);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No journal entry selected.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the houseProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseProposeBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;

            Character bride = null;
            Character groom = null;
            string brideID = "";
            string groomID = "";

            if (String.IsNullOrWhiteSpace(this.houseProposeBrideTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                }
            }
            else if (String.IsNullOrWhiteSpace(this.houseProposeGroomTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                }
            }
            else
            {
                // get bride and groom IDs
                brideID = this.houseProposeBrideTextBox.Text;
                groomID = this.houseProposeGroomTextBox.Text;

                // get bride
                if (Globals_Game.npcMasterList.ContainsKey(brideID))
                {
                    bride = Globals_Game.npcMasterList[brideID];
                }

                if (bride == null)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                    }
                }
                else
                {
                    // get groom
                    if (Globals_Game.npcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.npcMasterList[groomID];
                    }
                    else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.pcMasterList[groomID];
                    }

                    if (groom == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                        }
                    }
                    else
                    {
                        // carry out conditional checks
                        proceed = groom.ChecksBeforeProposal(bride);

                        // if checks OK, process proposal
                        if (proceed)
                        {
                            groom.ProposeMarriage(bride);

                            // refresh screen
                            this.RefreshCurrentScreen();
                        }
                    }

                }
            }

        }

        // ------------------- HOUSEHOLD MANAGEMENT

        /// <summary>
        /// Responds to the click event of the houseExamineArmiesBtn button
        /// displaying a list of all armies in the current NPC's fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // NPC
            Character thisObserver = Globals_Client.charToView;

            // examine armies
            this.ExamineArmiesInFief(thisObserver);
        }

        /// <summary>
        /// Responds to the click event of the familyNameChildButton
        /// allowing the player to name the selected child
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNameChildButton_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get NPC to name
                NonPlayerCharacter child = null;
                if (Globals_Game.npcMasterList.ContainsKey(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                {
                    child = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];
                }

                if (child != null)
                {
                    if (Regex.IsMatch(this.familyNameChildTextBox.Text.Trim(), @"^[a-zA-Z- ]+$"))
                    {
                        child.firstName = this.familyNameChildTextBox.Text;
                        this.RefreshHouseholdDisplay(child);
                    }
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("'" + this.familyNameChildTextBox.Text + "' is an unsuitable name, milord.");
                        }
                    }
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not retrieve details of NonPlayerCharacter.");
                    }
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a character from the list.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the houseHeirBtn button
        /// allowing the switch to another player (for testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseHeirBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];

                // check for suitability
                bool isSuitable = selectedNPC.ChecksForHeir(Globals_Client.myPlayerCharacter);

                if (isSuitable)
                {
                    // check for an existing heir and remove
                    foreach (NonPlayerCharacter npc in Globals_Client.myPlayerCharacter.myNPCs)
                    {
                        if (npc.isHeir)
                        {
                            npc.isHeir = false;
                        }
                    }

                    // appoint NPC as heir
                    selectedNPC.isHeir = true;

                    // refresh the household screen (in the main form)
                    this.RefreshHouseholdDisplay(selectedNPC);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the dealWithHouseholdAffairsToolStripMenuItem
        /// which causes the Household screen to display, listing the player's
        /// family and employed NPCs
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void dealWithHouseholdAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.charToView = null;

            // refresh household affairs screen 
            this.RefreshHouseholdDisplay();

            // display household affairs screen
            Globals_Client.containerToView = this.houseContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the houseCharListView object,
        /// invoking the displayCharacter method, passing a Character to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCharListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in employees
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                if (this.houseCharListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.myPlayerCharacter.myNPCs[i];
                        break;
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                this.houseCharTextBox.Text = this.DisplayCharacter(charToDisplay, Globals_Client.myPlayerCharacter);
                this.houseCharTextBox.ReadOnly = true;

                // see if is in entourage to set text of entourage button
                if ((charToDisplay as NonPlayerCharacter).inEntourage)
                {
                    this.houseEntourageBtn.Text = "Remove From Entourage";
                }
                else
                {
                    this.houseEntourageBtn.Text = "Add To Entourage";
                }

                // set text for 'enter/exit keep' button, depending on whether player in/out of keep
                if (Globals_Client.charToView.inKeep)
                {
                    this.houseEnterKeepBtn.Text = "Exit Keep";
                }
                else
                {
                    this.houseEnterKeepBtn.Text = "Enter Keep";
                }

                // FAMILY MATTERS CONTROLS
                // if family selected, enable 'choose heir' button, disbale 'fire' button
                if ((!String.IsNullOrWhiteSpace(Globals_Client.charToView.familyID)) && (Globals_Client.charToView.familyID.Equals(Globals_Client.myPlayerCharacter.charID)))
                {
                    this.houseHeirBtn.Enabled = true;
                    this.houseFireBtn.Enabled = false;

                    // if is male and married, enable NPC 'get wife with child' control
                    if ((Globals_Client.charToView.isMale) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.spouse)))
                    {
                        this.familyNpcSpousePregBtn.Enabled = true;
                    }
                    else
                    {
                        this.familyNpcSpousePregBtn.Enabled = false;
                    }
                }
                else
                {
                    this.houseHeirBtn.Enabled = false;
                    this.houseFireBtn.Enabled = true;
                    this.familyNpcSpousePregBtn.Enabled = false;
                }

                // if character firstname = "Baby", enable 'name child' controls
                if ((charToDisplay as NonPlayerCharacter).firstName.Equals("Baby"))
                {
                    this.familyNameChildButton.Enabled = true;
                    this.familyNameChildTextBox.Enabled = true;
                }
                // if not, ensure are disabled
                else
                {
                    this.familyNameChildButton.Enabled = false;
                    this.familyNameChildTextBox.Enabled = false;
                }

                // 'get wife with child' button always enabled
                this.familyGetSpousePregBtn.Enabled = true;

                // SIEGE CHECKS
                // check to see if is inside besieged keep
                if ((Globals_Client.charToView.inKeep) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.location.siege)))
                {
                    // if is inside besieged keep, disable most of controls
                    this.houseCampBtn.Enabled = false;
                    this.houseCampDaysTextBox.Enabled = false;
                    this.houseMoveToBtn.Enabled = false;
                    this.houseMoveToTextBox.Enabled = false;
                    this.houseRouteBtn.Enabled = false;
                    this.houseEntourageBtn.Enabled = false;
                    this.houseFireBtn.Enabled = false;
                    this.houseExamineArmiesBtn.Enabled = false;
                }

                // is NOT inside besieged keep
                else
                {
                    // re-enable controls
                    this.houseCampBtn.Enabled = true;
                    this.houseCampDaysTextBox.Enabled = true;
                    this.houseMoveToBtn.Enabled = true;
                    this.houseMoveToTextBox.Enabled = true;
                    this.houseRouteBtn.Enabled = true;
                    this.houseRouteTextBox.Enabled = true;
                    this.houseEntourageBtn.Enabled = true;
                    this.houseExamineArmiesBtn.Enabled = true;

                }

            }
        }

        /// <summary>
        /// Responds to the click event of the houseCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCampBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get days to camp
                byte campDays = Convert.ToByte(this.houseCampDaysTextBox.Text);

                // camp
                this.CampWaitHere(Globals_Client.charToView, campDays);
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
            finally
            {
                // refresh display
                this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }

        }

        // ------------------- FIEF MANAGEMENT

        /// <summary>
        /// Responds to the click event of the viewMyHomeFiefToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewMyHomeFiefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get home fief
            Fief homeFief = Globals_Client.myPlayerCharacter.GetHomeFief();

            if (homeFief != null)
            {
                // display home fief
                this.RefreshFiefContainer(homeFief);
            }

            // if have no home fief
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You have no home fief!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'Max' buttons inn the fief management screen,
        /// filling in the maximum expenditure for the selected field
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void maxSpendButton_Click(object sender, EventArgs e)
        {
            uint maxSpend = 0;

            // get tag from button
            Button button = sender as Button;
            string expType = button.Tag.ToString();

            // get max spend of specified type
            maxSpend = Globals_Client.fiefToView.GetMaxSpend(expType);

            if (maxSpend != 0)
            {
                switch (expType)
                {
                    case "garrison":
                        this.adjGarrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "infrastructure":
                        this.adjInfrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "keep":
                        this.adjustKeepSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "officials":
                        this.adjOffSpendTextBox.Text = maxSpend.ToString();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the fiefGrantTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefGrantTitleBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseTitleHolder = new SelectionForm(this, "titleHolder");
            chooseTitleHolder.Show();
        }

        /// <summary>
        /// Responds to the click event of the fiefTransferFundsPlayerBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefTransferFundsPlayerBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm transferFunds = new SelectionForm(this, "transferFunds");
            transferFunds.Show();
        }

        /// <summary>
        /// Responds to the click event of the selfBailiffBtn button,
        /// appointing the player as bailiff of the displayed fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void selfBailiffBtn_Click(object sender, EventArgs e)
        {
            // give player fair warning of bailiff commitments
            DialogResult dialogResult = MessageBox.Show("Being a bailiff will restrict your movement (as you need to remain in the fief for 30 days to have any effect).  Click 'OK' to proceed.", "Proceed with appointment?", MessageBoxButtons.OKCancel);

            // if choose to cancel
            if (dialogResult == DialogResult.Cancel)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Appointment cancelled.");
                }
            }

            // if choose to proceed
            else
            {
                // if the fief has an existing bailiff
                if (Globals_Client.fiefToView.bailiff != null)
                {
                    // relieve him of his duties
                    Globals_Client.fiefToView.bailiff = null;
                }

                // set player as bailiff
                Globals_Client.fiefToView.bailiff = Globals_Client.myPlayerCharacter;
            }

            // refresh fief display
            this.RefreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the click event of any of the 'transfer funds' buttons
        /// allowing players to transfer funds between treasuries
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get button
                Button button = sender as Button;
                // get transfer parameters from tag
                string transferType = button.Tag.ToString();

                Fief fiefFrom = null;
                Fief fiefTo = null;
                int amount = 0;

                switch (transferType)
                {
                    case "toFief":
                        fiefFrom = Globals_Client.myPlayerCharacter.GetHomeFief();
                        fiefTo = Globals_Client.fiefToView;
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    case "toHome":
                        fiefFrom = Globals_Client.fiefToView;
                        fiefTo = Globals_Client.myPlayerCharacter.GetHomeFief();
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    default:
                        break;
                }

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
                        this.TreasuryTransfer(fiefFrom, fiefTo, amount);
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
        /// Responds to the click event of the viewBailiffBtn button
        /// which refreshes and displays the character screen, showing details of the
        /// bailiff for the selected fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewBailiffBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // if player is bailiff, show in personal characteristics screen
                if (Globals_Client.fiefToView.bailiff == Globals_Client.myPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.myPlayerCharacter;
                    this.RefreshCharacterContainer(Globals_Client.charToView);
                }

                // if NPC is bailiff, show in household affairs screen
                else if (Globals_Client.fiefToView.bailiff is NonPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.fiefToView.bailiff;
                    // refresh household affairs screen 
                    this.RefreshHouseholdDisplay(Globals_Client.charToView as NonPlayerCharacter);
                    // display household affairs screen
                    Globals_Client.containerToView = this.houseContainer;
                    Globals_Client.containerToView.BringToFront();
                }
            }

            // display message that is no bailiff
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This fief currently has no bailiff.");
                }
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the fiefsListView object,
        /// invoking the displayFief method, passing a Fief to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // enable controls
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                this.fiefsChallengeBtn.Enabled = true;
                this.fiefsViewBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the setBailiffBtn button
        ///invoking and displaying the character selection screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void setBailiffBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseBailiff = new SelectionForm(this, "bailiff");
            chooseBailiff.Show();
        }

        /// <summary>
        /// Responds to the click event of the lockoutBtn button
        /// invoking and displaying the lockout screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void lockoutBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm lockOutOptions = new SelectionForm(this, "lockout");
            lockOutOptions.Show();
        }

        /// <summary>
        /// Responds to the click event of the removeBaliffBtn button,
        /// relieving the current bailiff of his duties
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void removeBaliffBtn_Click(object sender, EventArgs e)
        {
            // if the fief has an existing bailiff
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // relieve him of his duties
                Globals_Client.fiefToView.bailiff = null;

                // refresh fief display
                this.RefreshFiefContainer(Globals_Client.fiefToView);
            }
        }

        /// <summary>
        /// Responds to the click event of the adjustSpendBtn button
        /// which commits the expenditures and tax rate for the coming year
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adjustSpendBtn_Click(object sender, EventArgs e)
        {
            // keep track of whether any spends ahve changed
            bool spendChanged = false;

            try
            {
                // get new amounts
                Double newTax = Convert.ToDouble(this.adjustTaxTextBox.Text);
                UInt32 newOff = Convert.ToUInt32(this.adjOffSpendTextBox.Text);
                UInt32 newGarr = Convert.ToUInt32(this.adjGarrSpendTextBox.Text);
                UInt32 newInfra = Convert.ToUInt32(this.adjInfrSpendTextBox.Text);
                UInt32 newKeep = Convert.ToUInt32(this.adjustKeepSpendTextBox.Text);

                // process adjustments
                Globals_Client.fiefToView.AdjustExpenditures(newTax, newOff, newGarr, newInfra, newKeep);
                spendChanged = true;
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
            finally
            {
                // refresh screen if expenditure changed
                if (spendChanged)
                {
                    // refresh display
                    this.RefreshCurrentScreen();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the fiefAutoAdjustBtn button,
        /// displaying the armyManagementPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefAutoAdjustBtn_Click(object sender, EventArgs e)
        {
            int overspend = 0;

            // calculate overspend
            overspend = (Globals_Client.fiefToView.CalcNewExpenses() - Globals_Client.fiefToView.GetAvailableTreasury());

            // auto-adjust expenditure
            Globals_Client.fiefToView.AutoAdjustExpenditure(Convert.ToUInt32(overspend));

            // refresh screen
            this.RefreshCurrentScreen();
        }

        /// <summary>
        /// Responds to the click event of myFiefsToolStripMenuItem
        /// which refreshes and displays the owned fiefs screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void myFiefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.fiefToView = null;
            this.RefreshMyFiefs();
            Globals_Client.containerToView = this.fiefsOwnedContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the click event of the fiefManagementToolStripMenuItem
        /// which displays main Fief information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshFiefContainer(Globals_Client.myPlayerCharacter.location);
        }

        /// <summary>
        /// Responds to the Click event of the fiefsChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get province
                Province targetProv = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.fiefsListView.SelectedItems[0].SubItems[5].Text))
                {
                    targetProv = Globals_Game.provinceMasterList[this.fiefsListView.SelectedItems[0].SubItems[5].Text];
                }

                if (targetProv != null)
                {
                    targetProv.LodgeOwnershipChallenge(Globals_Client.myPlayerCharacter);
                }

                this.fiefsListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the Click event of the fiefsViewBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsViewBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get fief to view
                Globals_Client.fiefToView = Globals_Game.fiefMasterList[this.fiefsListView.SelectedItems[0].SubItems[1].Text];

                // go to fief display screen
                this.RefreshFiefContainer(Globals_Client.fiefToView);
                Globals_Client.containerToView = this.fiefContainer;
                Globals_Client.containerToView.BringToFront();
            }
        }

        // ------------------- CHARACTER DISPLAY

        /// <summary>
        /// Responds to the click event of the personalCharacteristicsAndAffairsToolStripMenuItem
        /// which displays main Character information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void personalCharacteristicsAndAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;
            this.RefreshCharacterContainer(Globals_Client.charToView);
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the characterTitlesCheckBox,
        /// displaying the player's titles/ranks
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void characterTitlesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshCharacterContainer(Globals_Client.charToView);
        }
        
        // ------------------- CHILDBIRTH

        /// <summary>
        /// Responds to the click event of the familyNpcSpousePregBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNpcSpousePregBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get spouse
                Character mySpouse = Globals_Client.charToView.GetSpouse();

                // perform standard checks
                if (Birth.ChecksBeforePregnancyAttempt(Globals_Client.charToView))
                {
                    // ensure are both in/out of keep
                    mySpouse.inKeep = Globals_Client.charToView.inKeep;

                    // attempt pregnancy
                    bool pregnant = Globals_Client.charToView.GetSpousePregnant(mySpouse);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No character selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the familyGetSpousePregBt button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyGetSpousePregBtn_Click(object sender, EventArgs e)
        {
            // perform standard checks
            if (Birth.ChecksBeforePregnancyAttempt(Globals_Client.myPlayerCharacter))
            {
                // get spouse
                Character mySpouse = Globals_Client.myPlayerCharacter.GetSpouse();

                // ensure are both in/out of keep
                mySpouse.inKeep = Globals_Client.myPlayerCharacter.inKeep;

                // attempt pregnancy
                bool pregnant = Globals_Client.myPlayerCharacter.GetSpousePregnant(mySpouse);
            }

            // refresh screen
            this.RefreshCurrentScreen();
        }

        // ------------------- JOURNAL

        /// <summary>
        /// Responds to the click event of the viewEntriesToolStripMenuItem
        /// displaying the journal screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get ToolStripMenuItem
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            // get entry scope from tag
            string entryScope = menuItem.Tag.ToString();

            // get and display entries
            this.ViewJournalEntries(entryScope);
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the journalListView
        /// displaying the selected journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get entry
            if (this.journalListView.SelectedItems.Count > 0)
            {
                uint jEntryID = Convert.ToUInt32(this.journalListView.SelectedItems[0].SubItems[0].Text);
                Globals_Client.jEntryToView = Globals_Client.eventSetToView.IndexOfKey(jEntryID);

                // retrieve and display character information
                this.journalTextBox.Text = this.DisplayJournalEntry(Globals_Client.jEntryToView);

                // check if marriage proposal controls should be enabled
                JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;
                bool enableProposalControls = thisJentry.CheckForProposalControlsEnabled();
                this.journalProposalAcceptBtn.Enabled = enableProposalControls;
                this.journalProposalRejectBtn.Enabled = enableProposalControls;

                // mark entry as viewed
                Globals_Client.myPastEvents.entries[jEntryID].viewed = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the journalPrevBtn button
        /// selecting and displaying the previous journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalPrevBtn_Click(object sender, EventArgs e)
        {
            // check if at beginning of index
            if (Globals_Client.jEntryToView == 0)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are no entries prior to this one.");
                }
            }

            else
            {
                // amend index position
                Globals_Client.jEntryToView--;
            }

            // refresh journal screen
            this.RefreshJournalContainer(Globals_Client.jEntryToView);
        }

        /// <summary>
        /// Responds to the click event of the journalNextBtn button
        /// selecting and displaying the next journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalNextBtn_Click(object sender, EventArgs e)
        {
            // check if at beginning of index
            if (Globals_Client.jEntryToView == Globals_Client.jEntryMax)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are no entries after this one.");
                }
            }

            else
            {
                // amend index position
                Globals_Client.jEntryToView++;
            }

            // refresh journal screen
            this.RefreshJournalContainer(Globals_Client.jEntryToView);
        }

        /// <summary>
        /// Responds to the Click event of the journalToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get and tally unread entries
            Globals_Client.eventSetToView = Globals_Client.myPastEvents.GetUnviewedEntries();
            int unreadEntries = Globals_Client.eventSetToView.Count;

            // indicate no. of unread entries in menu item text
            this.viewMyEntriesunreadToolStripMenuItem.Text = "View UNREAD entries (" + unreadEntries + ")";
        }

        // ------------------- ARMY MANAGEMENT

        /// <summary>
        /// Responds to the click event of the armyRecruitBtn button, allowing the player 
        /// to create a new army and/or recruit additional troops in the current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyRecruitBtn_Click(object sender, EventArgs e)
        {
            bool armyExists = true;

            // get tag from button
            Button button = sender as Button;
            string operation = button.Tag.ToString();

            if (operation.Equals("new"))
            {
                armyExists = false;
            }

            try
            {
                // get number of troops specified
                UInt32 numberWanted = Convert.ToUInt32(this.armyRecruitTextBox.Text);

                // recruit troops
                Globals_Client.myPlayerCharacter.RecruitTroops(numberWanted, armyExists);

                // get army
                Army myArmy = Globals_Client.myPlayerCharacter.GetArmy();

                // refresh display
                this.RefreshArmyContainer(myArmy);

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
        /// Responds to the click event of the armyMaintainBtn button
        /// allowing the player to maintain the army in the field
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyMaintainBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                // maintain army
                Globals_Client.armyToView.MantainArmy();

                // refresh display
                this.RefreshArmyContainer(Globals_Client.armyToView);
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the armyListView object,
        /// invoking the displayArmyData method and passing an Army to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get army to view
            if (this.armyListView.SelectedItems.Count > 0)
            {
                Globals_Client.armyToView = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];
            }

            if (Globals_Client.armyToView != null)
            {
                // display data for selected army
                this.armyTextBox.Text = Globals_Client.armyToView.DisplayArmyData(Globals_Client.myPlayerCharacter);

                // check if is defender in a siege
                string siegeID = Globals_Client.armyToView.CheckIfSiegeDefenderGarrison();
                if (String.IsNullOrWhiteSpace(siegeID))
                {
                    siegeID = Globals_Client.armyToView.CheckIfSiegeDefenderAdditional();
                }

                // if is defender in a siege, disable controls
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    this.DisableControls(this.armyManagementPanel);
                    this.DisableControls(this.armyCombatPanel);

                    // always enable switch between management and combat panels
                    this.armyDisplayCmbtBtn.Enabled = true;
                    this.armyDisplayMgtBtn.Enabled = true;
                }

                // if isn't defender in a siege, enable controls
                else
                {
                    // recruit controls
                    // if player is leading an army but not the one on view, disable 'recruit' button
                    if ((!String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                        && (!(Globals_Client.armyToView.leader.Equals(Globals_Client.myPlayerCharacter.charID))))
                    {
                        this.armyRecruitBtn.Enabled = false;
                        this.armyRecruitTextBox.Enabled = false;
                    }
                    // otherwise, enable 'recruit' button
                    else
                    {
                        this.armyRecruitBtn.Enabled = true;
                        this.armyRecruitTextBox.Enabled = true;

                        // if army on view is led by player, set button text to 'recruit additional'
                        if (Globals_Client.armyToView.leader == Globals_Client.myPlayerCharacter.charID)
                        {
                            this.armyRecruitBtn.Text = "Recruit Additional Troops From Current Fief";
                            this.armyRecruitBtn.Tag = "add";
                        }
                        // if player is not leading any armies, set button text to 'recruit new'
                        else if (String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.armyID))
                        {
                            this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                            this.armyRecruitBtn.Tag = "new";
                        }
                    }

                    // if has no leader
                    if (String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                    {
                        // set army aggression to 0
                        if (Globals_Client.armyToView.aggression > 0)
                        {
                            Globals_Client.armyToView.aggression = 0;
                        }

                        // disable 'proactive' army functions
                        this.armyExamineBtn.Enabled = false;
                        this.armyPillageBtn.Enabled = false;
                        this.armySiegeBtn.Enabled = false;
                        this.armyAutoCombatBtn.Enabled = false;
                        this.armyAggroTextBox.Enabled = false;
                        this.armyOddsTextBox.Enabled = false;
                        this.armyTransDropBtn.Enabled = false;
                        this.armyTransKnightTextBox.Enabled = false;
                        this.armyTransMAAtextBox.Enabled = false;
                        this.armyTransLCavTextBox.Enabled = false;
                        this.armyTransLongbowTextBox.Enabled = false;
                        this.armyTransCrossbowTextBox.Enabled = false;
                        this.armyTransFootTextBox.Enabled = false;
                        this.armyTransRabbleTextBox.Enabled = false;
                        this.armyTransDropWhoTextBox.Enabled = false;
                        this.armyTransPickupBtn.Enabled = false;
                    }

                    // has leader
                    else
                    {
                        this.armyExamineBtn.Enabled = true;
                        this.armyPillageBtn.Enabled = true;
                        this.armySiegeBtn.Enabled = true;
                        this.armyAutoCombatBtn.Enabled = true;
                        this.armyAggroTextBox.Enabled = true;
                        this.armyOddsTextBox.Enabled = true;
                        this.armyTransDropBtn.Enabled = true;
                        this.armyTransKnightTextBox.Enabled = true;
                        this.armyTransMAAtextBox.Enabled = true;
                        this.armyTransLCavTextBox.Enabled = true;
                        this.armyTransLongbowTextBox.Enabled = true;
                        this.armyTransCrossbowTextBox.Enabled = true;
                        this.armyTransFootTextBox.Enabled = true;
                        this.armyTransRabbleTextBox.Enabled = true;
                        this.armyTransDropWhoTextBox.Enabled = true;
                        this.armyTransPickupBtn.Enabled = true;
                    }

                    // other controls
                    this.armyMaintainBtn.Enabled = true;
                    this.armyAppointLeaderBtn.Enabled = true;
                    this.armyAppointSelfBtn.Enabled = true;
                    this.armyDisbandBtn.Enabled = true;
                    this.armyCampBtn.Enabled = true;
                    this.armyCampTextBox.Enabled = true;

                    // check to see if current fief is in rebellion and enable control as appropriate
                    // get fief
                    Fief thisFief = Globals_Client.armyToView.GetLocation();
                    if (thisFief.status.Equals('R'))
                    {
                        // only enable if army has leader
                        if (!String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                        {
                            this.armyQuellRebellionBtn.Enabled = true;
                        }
                    }
                    else
                    {
                        this.armyQuellRebellionBtn.Enabled = false;
                    }

                    // set auto combat values
                    this.armyAggroTextBox.Text = Globals_Client.armyToView.aggression.ToString();
                    this.armyOddsTextBox.Text = Globals_Client.armyToView.combatOdds.ToString();

                    // preload own ID in 'drop off to' textbox (assumes transferring between own armies)
                    this.armyTransDropWhoTextBox.Text = Globals_Client.myPlayerCharacter.charID;
                    // and set all troop transfer numbers to 0
                    this.armyTransKnightTextBox.Text = "0";
                    this.armyTransMAAtextBox.Text = "0";
                    this.armyTransLCavTextBox.Text = "0";
                    this.armyTransLongbowTextBox.Text = "0";
                    this.armyTransCrossbowTextBox.Text = "0";
                    this.armyTransFootTextBox.Text = "0";
                    this.armyTransRabbleTextBox.Text = "0";
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the listMToolStripMenuItem
        /// displaying the army management screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void listMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "management";
            Globals_Client.armyToView = null;
            this.RefreshArmyContainer();
        }

        /// <summary>
        /// Responds to the click event of the armyAppointLeaderBtn button
        /// invoking and displaying the character selection screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAppointLeaderBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            string thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseLeader = new SelectionForm(this, "leader", armID: thisArmyID);
                chooseLeader.Show();
            }

            // if no army selected
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the armyAppointSelfBtn button
        /// allowing the player to appoint themselves as army leader
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAppointSelfBtn_Click(object sender, EventArgs e)
        {
            // get army
            Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

            thisArmy.AssignNewLeader(Globals_Client.myPlayerCharacter);

            // refresh the army information (in the main form)
            this.RefreshArmyContainer(thisArmy);
        }

        /// <summary>
        /// Responds to the click event of the armyTransDropBtn button
        /// allowing the player to leave troops in the fief for transfer to other armies
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyTransDropBtn_Click(object sender, EventArgs e)
        {
            bool success = true;
            string toDisplay = "";

            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get number of troops to transfer
                    uint[] troopsToTransfer = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
                    troopsToTransfer[0] = Convert.ToUInt32(this.armyTransKnightTextBox.Text);
                    troopsToTransfer[1] = Convert.ToUInt32(this.armyTransMAAtextBox.Text);
                    troopsToTransfer[2] = Convert.ToUInt32(this.armyTransLCavTextBox.Text);
                    troopsToTransfer[3] = Convert.ToUInt32(this.armyTransLongbowTextBox.Text);
                    troopsToTransfer[4] = Convert.ToUInt32(this.armyTransCrossbowTextBox.Text);
                    troopsToTransfer[5] = Convert.ToUInt32(this.armyTransFootTextBox.Text);
                    troopsToTransfer[6] = Convert.ToUInt32(this.armyTransRabbleTextBox.Text);

                    // create detachment details
                    string[] detachmentDetails = new string[] {troopsToTransfer[0].ToString(), troopsToTransfer[1].ToString(),
                        troopsToTransfer[2].ToString(), troopsToTransfer[3].ToString(), troopsToTransfer[4].ToString(),
                        troopsToTransfer[5].ToString(), troopsToTransfer[6].ToString(), this.armyTransDropWhoTextBox.Text };

                    // create detachment and leave in fief
                    success = Globals_Client.armyToView.CreateDetachment(detachmentDetails);

                    // inform player
                    if (!success)
                    {
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "An error occurred that prevented the transfer.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "ERROR DETECTED");
                        }
                    }
                    else
                    {
                        toDisplay = "Transfer successful.";
                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION SUCCESSFUL");
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
                finally
                {
                    this.RefreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyTransPickupBtn button, invoking and displaying the
        /// transfer selection screen, allowing detachments in the current fief to be added to the army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyTransPickupBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            string thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseTroops = new SelectionForm(this, "transferTroops", armID: thisArmyID);
                chooseTroops.Show();
            }

            // if no army selected
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the armyManagementToolStripMenuItem
        /// which displays main Army information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get player's army
            Army thisArmy = Globals_Client.myPlayerCharacter.GetArmy();

            // display army mangement screen
            this.armyContainer.Panel1.Tag = "management";
            this.RefreshArmyContainer(thisArmy);
        }

        /// <summary>
        /// Responds to the click event of the armyDisbandBtn button, disbanding the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisbandBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                // disband army
                Globals_Client.armyToView.DisbandArmy();
                Globals_Client.armyToView = null;

                // refresh display
                this.RefreshArmyContainer();
            }
        }

        /// <summary>
        /// Responds to the click event of the armyAutoCombatBtn button, setting the army's
        /// aggression and combat odds values to those in the text fields
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAutoCombatBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get new aggression level
                    byte newAggroLevel = Convert.ToByte(this.armyAggroTextBox.Text);

                    // get new combat odds value
                    byte newOddsValue = Convert.ToByte(this.armyOddsTextBox.Text);

                    // check and adjust values
                    bool success = Globals_Client.armyToView.AdjustStandingOrders(newAggroLevel, newOddsValue);

                    // inform player
                    string toDisplay = "";
                    string msgTitle = "";
                    if (success)
                    {
                        toDisplay = "Auto-combat values successfully adjusted.";
                        msgTitle = "OPERATION SUCCESSFUL";
                    }
                    else
                    {
                        toDisplay = "I'm afraid there has been a problem adjusting the auto-combat values, my lord.";
                        msgTitle = "ERROR OCCURRED";
                    }
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay, msgTitle);
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
                finally
                {
                    // refresh display
                    this.RefreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyCampBtn button
        /// invoking the campWaitHere method for the army leader (and army)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyCampBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get days to camp
                    byte campDays = Convert.ToByte(this.armyCampTextBox.Text);

                    // get leader
                    Character thisLeader = Globals_Client.armyToView.GetLeader();

                    // camp
                    if (thisLeader != null)
                    {
                        this.CampWaitHere(thisLeader, campDays);
                    }

                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot conduct this operation without a leader.", "OPERATION CANCELLED");
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
                finally
                {
                    // refresh display
                    this.RefreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyExamineBtn button
        /// displaying a list of all armies in the current army's fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyExamineBtn_Click(object sender, EventArgs e)
        {
            // army leader
            Character thisLeader = Globals_Client.armyToView.GetLeader();

            // examine armies
            this.ExamineArmiesInFief(thisLeader);
        }

        /// <summary>
        /// Responds to the click event of the armyDisplayMgtBtn button,
        /// displaying the armyCombatPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisplayMgtBtn_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "management";
            this.armyManagementPanel.BringToFront();
            this.armyListView.Focus();
        }

        /// <summary>
        /// Responds to the click event of the armyDisplayCmbtBtn button,
        /// displaying the armyManagementPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisplayCmbtBtn_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "combat";
            this.armyCombatPanel.BringToFront();
            this.armyListView.Focus();
        }

        /// <summary>
        /// Responds to the click event of the murderThisCharacterToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void murderThisCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get character
            string charID = this.muderCharacterMenuTextBox1.Text;

            if (String.IsNullOrWhiteSpace(charID))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No Character ID entered.  Operation cancelled.");
                }
            }

            Character charToMurder = null;
             if (Globals_Game.pcMasterList.ContainsKey(charID))
            {
                charToMurder = Globals_Game.pcMasterList[charID];
            }
             else if (Globals_Game.npcMasterList.ContainsKey(charID))
            {
                charToMurder = Globals_Game.npcMasterList[charID];
            }

             if (charToMurder != null)
             {
                 charToMurder.ProcessDeath();
             }

             else
             {
                 if (Globals_Client.showMessages)
                 {
                     System.Windows.Forms.MessageBox.Show("Character could not be identified.  Operation cancelled.");
                 }
             }
        }

        /// <summary>
        /// Responds to the click event of the houseEnterKeepBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseEnterKeepBtn_Click(object sender, EventArgs e)
        {            
            // attempt to enter/exit keep
            bool success = Globals_Client.charToView.ExitEnterKeep();

            // if successful
            if (success)
            {
                // refresh display
                this.RefreshHouseholdDisplay(Globals_Client.charToView as NonPlayerCharacter);
            }
        }

    }

}
