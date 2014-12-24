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
    /// Partial class for Form1, containing functionality specific to the journal
    /// </summary>
    partial class Form1
    {

        // ------------------- JOURNAL

        /// <summary>
        /// Populates Globals_Client.eventSetToView with the appropriate JournalEvent set
        /// </summary>
        /// <param name="setScope">The JournalEvent set to fetch</param>
        private void viewJournalEntries(string setScope)
        {
            // get current season and year
            uint thisYear = Globals_Game.clock.currentYear;
            byte thisSeason = Globals_Game.clock.currentSeason;

            switch (setScope)
            {
                // get entries for current year
                case "year":
                    Globals_Client.eventSetToView = Globals_Client.myPastEvents.getEventsOnDate(yr: thisYear);
                    break;
                // get entries for current season
                case "season":
                    Globals_Client.eventSetToView = Globals_Client.myPastEvents.getEventsOnDate(yr: thisYear, seas: thisSeason);
                    break;
                // get unread entries
                case "unread":
                    Globals_Client.eventSetToView = Globals_Client.myPastEvents.getUnviewedEntries();
                    break;
                // get all entries
                default:
                    Globals_Client.eventSetToView = Globals_Client.myPastEvents.getEventsOnDate();
                    break;
            }

            // get max index position
            Globals_Client.jEntryMax = Globals_Client.eventSetToView.Count - 1;

            // set default index position
            Globals_Client.jEntryToView = -1;

            // display journal screen
            this.refreshJournalContainer();
        }

        /// <summary>
        /// Creates UI display for list of journal entries
        /// </summary>
        public void setUpJournalList()
        {
            // add necessary columns
            this.journalListView.Columns.Add("Entry ID", -2, HorizontalAlignment.Left);
            this.journalListView.Columns.Add("Date", -2, HorizontalAlignment.Left);
            this.journalListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Retrieves information for journal display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="indexPosition">The index position of the journal entry to be displayed</param>
        public string displayJournalEntry(int indexPosition)
        {
            string jentryText = "";

            // get journal entry
            JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(indexPosition).Value;

            // get text
            jentryText = thisJentry.getJournalEntryDetails();

            return jentryText;
        }

        /// <summary>
        /// Refreshes main journal display screen
        /// </summary>
        /// <param name="a">JournalEntry to be displayed</param>
        public void refreshJournalContainer(int jEntryIndex = -1)
        {
            // get JournalEntry
            JournalEntry jEntryPassedIn = null;
            if ((jEntryIndex >= 0) && (!(jEntryIndex > Globals_Client.jEntryMax)))
            {
                jEntryPassedIn = Globals_Client.eventSetToView.ElementAt(jEntryIndex).Value;
            }

            // clear existing information
            this.journalTextBox.Text = "";

            // ensure textboxes aren't interactive
            this.journalTextBox.ReadOnly = true;

            // disable controls until JournalEntry selected

            // clear existing items in journal list
            this.journalListView.Items.Clear();

            // iterates through journal entries adding information to ListView
            foreach (KeyValuePair<uint, JournalEntry> thisJentry in Globals_Client.eventSetToView)
            {
                ListViewItem thisEntry = null;

                // jEntryID
                thisEntry = new ListViewItem(Convert.ToString(thisJentry.Value.jEntryID));

                // date
                string entrySeason = Globals_Game.clock.seasons[thisJentry.Value.season];
                thisEntry.SubItems.Add(entrySeason + ", " + thisJentry.Value.year);

                // type
                thisEntry.SubItems.Add(thisJentry.Value.type);

                if (thisEntry != null)
                {
                    // if journal entry passed in as parameter, show as selected
                    if (thisJentry.Value == jEntryPassedIn)
                    {
                        thisEntry.Selected = true;
                    }

                    // add item to journalListView
                    this.journalListView.Items.Add(thisEntry);
                }

            }

            // switch off 'unread entries' alert
            this.setJournalAlert(false);

            Globals_Client.containerToView = this.journalContainer;
            Globals_Client.containerToView.BringToFront();
            this.journalListView.Focus();
        }

        /// <summary>
        /// Adds a new JournalEntry to the myPastEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public bool addMyPastEvent(JournalEntry jEntry)
        {
            bool success = false;
            byte priority = 0;

            success = Globals_Client.myPastEvents.addNewEntry(jEntry);

            if (success)
            {
                // check for entry priority
                priority = jEntry.checkEventForPriority();

                // set alert
                this.setJournalAlert(true, priority);
            }

            return success;

        }

        /// <summary>
        /// Sets the myPastEvents Journal's areNewEntries setting to the appropriate value
        /// and sets the journal menu alert to the desired priority
        /// </summary>
        /// <param name="setAlert">The desired bool value</param>
        /// <param name="newPriority">The desired priority</param>
        public void setJournalAlert(bool setAlert, byte newPriority = 0)
        {
            // set Journal menu alert (BackColor) and priority as appropriate
            if (!setAlert)
            {
                // if no new entries, set to default colour
                this.journalToolStripMenuItem.BackColor = Control.DefaultBackColor;
                // set journal priority
                Globals_Client.myPastEvents.priority = 0;
            }
            else
            {
                // only change alert colour if new priority higher than current
                if (newPriority >= Globals_Client.myPastEvents.priority)
                {
                    // set journal priority
                    Globals_Client.myPastEvents.priority = newPriority;

                    // set to appropriate alert colour
                    switch (newPriority)
                    {
                        case 1:
                            this.journalToolStripMenuItem.BackColor = Color.Orange;
                            break;
                        case 2:
                            this.journalToolStripMenuItem.BackColor = Color.Red;
                            break;
                        default:
                            this.journalToolStripMenuItem.BackColor = Color.GreenYellow;
                            break;
                    }
                }
            }

            // set areNewEntries value
            if (Globals_Client.myPastEvents.areNewEntries != setAlert)
            {
                Globals_Client.myPastEvents.areNewEntries = setAlert;
            }

        }
    }
}
