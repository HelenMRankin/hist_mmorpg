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
    /// Partial class for Form1, containing functionality specific to engagement and marriage
    /// </summary>
    partial class Form1
    {

        /// <summary>
        /// Allows a character to reply to a marriage proposal
        /// This doesn't seem like it needs to be here....
        /// </summary>
        /// <returns>bool indicating whether reply was processed successfully</returns>
        /// <param name="jEntry">The proposal</param>
        /// <param name="proposalAccepted">bool indicating whether proposal accepted</param>
        public bool ReplyToProposal(JournalEntry jEntry, bool proposalAccepted)
        {
            // process reply
            bool success = jEntry.ReplyToProposal(proposalAccepted);

            // refresh screen
            this.RefreshCurrentScreen();

            return success;
        }

      
    }
}
