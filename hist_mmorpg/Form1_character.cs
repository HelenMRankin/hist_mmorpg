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
    /// Partial class for Form1, containing functionality specific to the character display
    /// This can be smooshed into Character to fetch the details required for the client
    /// </summary>
    partial class Form1
    {
       

        /// <summary>
        /// Retrieves information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="observed">Character whose information is to be displayed</param>
        /// <param name="observer">Character who is viewing this character's information</param>
        public string DisplayCharacter(Character observed, Character observer)
        {
            bool isMyNPC = false;

            // check if is in player's myNPCs
            if (observer is PlayerCharacter)
            {
                if ((observer as PlayerCharacter).myNPCs.Contains((observed as NonPlayerCharacter)) || (observed == Globals_Client.myPlayerCharacter))
                {
                    isMyNPC = true;
                }
            }

            string charText = observed.DisplayCharacter(isMyNPC, this.characterTitlesCheckBox.Checked, observer);

            return charText;
        }

        /// <summary>
        /// Refreshes main Character display screen
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        public void RefreshCharacterContainer(Character ch = null)
        {
            // if character not specified, default to player
            if (ch == null)
            {
                ch = Globals_Client.myPlayerCharacter;
            }

            // refresh Character display TextBox
            this.characterTextBox.ReadOnly = true;
            this.characterTextBox.Text = this.DisplayCharacter(ch, Globals_Client.myPlayerCharacter);

            // clear previous entries in Camp TextBox
            this.travelCampDaysTextBox.Text = "";

            // multimove button only enabled if is player or an employee
            if (ch != Globals_Client.myPlayerCharacter)
            {
                if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(ch))
                {
                    this.travelMoveToBtn.Enabled = false;
                }
            }

            Globals_Client.containerToView = this.characterContainer;
            Globals_Client.containerToView.BringToFront();
        }
    }
}
