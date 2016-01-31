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
    /// Partial class for Form1, containing functionality specific to fief management
    /// Again, this probably won't exist on the server side. These kind of methods will be implemented on the client
    /// There will be methods to get all the details of a fief on client request though
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Refreshes display of PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void RefreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            // disable controls until fief selected
            this.DisableControls(this.fiefsOwnedContainer.Panel1);

            ListViewItem[] fiefsOwned = new ListViewItem[Globals_Client.myPlayerCharacter.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < Globals_Client.myPlayerCharacter.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                // name
                fiefsOwned[i] = new ListViewItem(Globals_Client.myPlayerCharacter.ownedFiefs[i].name);

                // ID
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].id);

                // current location
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i] == Globals_Client.myPlayerCharacter.location)
                {
                    fiefsOwned[i].SubItems.Add("You are here");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // home fief
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i].id.Equals(Globals_Client.myPlayerCharacter.homeFief))
                {
                    fiefsOwned[i].SubItems.Add("Home");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // province name
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].province.name);

                // province ID
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].province.id);

                // add item to fiefsListView
                this.fiefsListView.Items.Add(fiefsOwned[i]);
            }
        }

        /// <summary>
        /// Refreshes main Fief display screen
        /// </summary>
        /// <param name="f">Fief whose information is to be displayed</param>
        public void RefreshFiefContainer(Fief f = null)
        {
			// if fief not specified, default to player's current location
            if (f == null)
            {
                f = Globals_Client.myPlayerCharacter.location;
            }

            Globals_Client.fiefToView = f;

            bool isOwner = Globals_Client.myPlayerCharacter.ownedFiefs.Contains(Globals_Client.fiefToView);
            bool displayWarning = false;
            string toDisplay = "";

            // set name label text
            this.fiefLabel.Text = Globals_Client.fiefToView.name + " (" + Globals_Client.fiefToView.id + ")";
            // set siege label text
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                this.fiefSiegeLabel.Text = "Fief under siege";
            }
            else
            {
                this.fiefSiegeLabel.Text = "";
            }

            // refresh main fief TextBox with updated info
            this.fiefTextBox.Text = Globals_Client.fiefToView.DisplayFiefGeneralData(isOwner);

            // ensure textboxes aren't interactive
            this.fiefTextBox.ReadOnly = true;
            this.fiefPrevKeyStatsTextBox.ReadOnly = true;
            this.fiefCurrKeyStatsTextBox.ReadOnly = true;
            this.fiefNextKeyStatsTextBox.ReadOnly = true;
            this.fiefTransferAmountTextBox.Text = "";

            // if fief is NOT owned by player, disable fief management buttons and TextBoxes 
            if (!isOwner)
            {
                this.DisableControls(this.fiefContainer.Panel1);
                this.DisableControls(this.fiefFinanceContainer1.Panel1);
                this.DisableControls(this.fiefFinanceContainer2.Panel1);
                this.DisableControls(this.fiefFinanceContainer2.Panel2);
            }

            // if fief IS owned by player, enable fief management buttons and TextBoxes 
            else
            {
                // get home fief
                Fief home = Globals_Client.myPlayerCharacter.GetHomeFief();

                // get home treasury
                int homeTreasury = 0;
                if (f == home)
                {
                    homeTreasury = home.GetAvailableTreasury();
                }
                else
                {
                    homeTreasury = home.GetAvailableTreasury(true);
                }

                // get this fief's treasury
                int fiefTreasury = f.GetAvailableTreasury();

                // if fief UNDER SIEGE, leave most controls disabled
                if (!String.IsNullOrWhiteSpace(f.siege))
                {
                    // allow view bailiff
                    this.viewBailiffBtn.Enabled = true;

                    // allow financial data TextBoxes to show appropriate data
                    this.fiefPrevKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsPrev();
                    this.fiefCurrKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsCurr();
                    this.fiefNextKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsNext();
                }

                // if NOT under siege, enable usual controls
                else
                {
                    this.EnableControls(this.fiefContainer.Panel1);
                    this.fiefPrevKeyStatsTextBox.Enabled = true;
                    this.fiefCurrKeyStatsTextBox.Enabled = true;
                    this.fiefNextKeyStatsTextBox.Enabled = true;

                    // don't enable 'appoint self' button if you're already the bailiff
                    if (f.bailiff == Globals_Client.myPlayerCharacter)
                    {
                        this.selfBailiffBtn.Enabled = false;
                    }
                    else
                    {
                        this.selfBailiffBtn.Enabled = true;
                    }
						
					// don't enable treasury transfer controls if in Home Fief (can't transfer to self)
                    if (f == home)
                    {
                        this.fiefTransferToFiefBtn.Enabled = false;
                        this.fiefTransferToHomeBtn.Enabled = false;
                        this.fiefTransferAmountTextBox.Enabled = false;
                        this.FiefTreasTextBox.Enabled = false;
                        this.FiefTreasTextBox.Text = "";
                    }
                    else
                    {
                        this.fiefTransferToFiefBtn.Enabled = true;
                        this.fiefTransferToHomeBtn.Enabled = true;
                        this.fiefTransferAmountTextBox.Enabled = true;
                        this.fiefHomeTreasTextBox.Enabled = true;
                        this.FiefTreasTextBox.Enabled = true;
                        this.FiefTreasTextBox.Text = fiefTreasury.ToString();
                    }

                    // set TextBoxes to show appropriate data
                    this.adjGarrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.garrisonSpendNext);
                    this.adjInfrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.infrastructureSpendNext);
                    this.adjOffSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.officialsSpendNext);
                    this.adjustKeepSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.keepSpendNext);
                    this.adjustTaxTextBox.Text = Convert.ToString(Globals_Client.fiefToView.taxRateNext);
                    this.fiefPrevKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsPrev();
                    this.fiefCurrKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsCurr();
                    this.fiefNextKeyStatsTextBox.Text = Globals_Client.fiefToView.DisplayFiefKeyStatsNext();

                    // display home treasury
                    this.fiefHomeTreasTextBox.Text = homeTreasury.ToString();

                    // check to see if proposed expenditure level doesn't exceed fief treasury
                    // get fief expenses (includes bailiff modifiers)
                    uint totalSpend = Convert.ToUInt32(Globals_Client.fiefToView.CalcNewExpenses());

                    // make sure expenditure can be supported by the treasury
                    // if it can't, display a message and enable the overspend auto-adjust button
                    if (!Globals_Client.fiefToView.CheckExpenditureOK(totalSpend))
                    {
                        this.fiefAutoAdjustBtn.Enabled = true;

                        int difference = Convert.ToInt32(totalSpend - fiefTreasury);
                        toDisplay = "Your proposed expenditure exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                        toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                        toDisplay += "\r\n\r\nAny unsupportable expenditure levels will be automatically adjusted during the seasonal update.";
                        displayWarning = true;
                    }
                    else
                    {
                        // disable the overspend auto-adjust button
                        this.fiefAutoAdjustBtn.Enabled = false;
                    }
                }

            }

            Globals_Client.containerToView = this.fiefContainer;
            Globals_Client.containerToView.BringToFront();

            if (displayWarning)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay, "WARNING: CANNOT SUPPORT PROPOSED EXPENDITURE");
                }
            }
        }

        /// <summary>
        /// Transfers funds between the home treasury and the fief treasury
        /// Yeah this belongs in Fief ;D
        /// </summary>
        /// <param name="from">The Fief from which funds are to be transferred</param>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        public void TreasuryTransfer(Fief from, Fief to, int amount)
        {
            // conduct transfer
            from.TreasuryTransfer(to, amount);

            // refresh fief display
            this.RefreshCurrentScreen();
        }

    }
}
