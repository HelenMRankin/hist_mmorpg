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
    /// Partial class for Form1, containing functionality specific to royalty and province overlords
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Refreshes royal gifts display
        /// </summary>
        public void RefreshRoyalGiftsContainer()
        {
            // get PlayerCharacter (to allow for herald viewing king's finances)
            PlayerCharacter thisKing = null;
            if (Globals_Client.myPlayerCharacter.CheckIsKing())
            {
                thisKing = Globals_Client.myPlayerCharacter;
            }
            else if (Globals_Client.myPlayerCharacter.CheckIsHerald())
            {
                thisKing = Globals_Client.myPlayerCharacter.GetKing();
            }

            if (thisKing != null)
            {
                // to store financial data
                int totGDP = 0;
                int provTaxInc = 0;
                int totTaxInc = 0;
                int totTreas = 0;
                double taxRate = 0;

                // disable controls until place selected in ListView
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
                this.royalGiftsPositionRemoveBtn.Enabled = false;

                // remove any previously displayed text

                // clear existing items in places lists
                this.royalGiftsProvListView.Items.Clear();
                this.royalGiftsFiefListView.Items.Clear();
                this.royalGiftsPositionListView.Items.Clear();

                // iterates through owned provinces and fiefs, adding information to appropriate ListView
                // PROVINCES
                foreach (Province thisProvince in thisKing.ownedProvinces)
                {
                    ListViewItem provItem = null;

                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // title holder
                    // get character
                    PlayerCharacter thisHolder = null;
                    //System.Windows.Forms.MessageBox.Show("Got here!");
                    if (Globals_Game.pcMasterList.ContainsKey(thisProvince.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisProvince.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        provItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate and total tax income
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            taxRate = fiefEntry.Value.keyStatsCurrent[12];
                            // update tax income total for province
                            provTaxInc += Convert.ToInt32(fiefEntry.Value.keyStatsCurrent[11]);
                        }
                    }

                    // add tax rate subitem
                    provItem.SubItems.Add(taxRate.ToString());

                    // add tax income subitem
                    provItem.SubItems.Add("£" + provTaxInc);

                    // update total tax income for all provinces
                    totTaxInc += provTaxInc;

                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsProvListView.Items.Add(provItem);
                    }

                }

                // add listviewitem with total tax income (all provinces)
                string[] provItemTotalSubs = new string[] { "", "", "", "", "£" + totTaxInc };
                ListViewItem provItemTotal = new ListViewItem(provItemTotalSubs);
                this.royalGiftsProvListView.Items.Add(provItemTotal);

                // FIEFS
                totTaxInc = 0;
                foreach (Fief thisFief in thisKing.ownedFiefs)
                {
                    ListViewItem fiefItem = null;

                    // id
                    fiefItem = new ListViewItem(thisFief.id);

                    // name
                    fiefItem.SubItems.Add(thisFief.name);

                    // province name
                    fiefItem.SubItems.Add(thisFief.province.name);

                    // title holder
                    // get character
                    Character thisHolder = null;
                    if (Globals_Game.pcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisFief.titleHolder];
                    }
                    else if (Globals_Game.npcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.npcMasterList[thisFief.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        fiefItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // gdp
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[1]);
                    // update GDP total
                    totGDP += Convert.ToInt32(thisFief.keyStatsCurrent[1]);

                    // last tax income
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[11]);
                    // update tax income total
                    totTaxInc += Convert.ToInt32(thisFief.keyStatsCurrent[11]);

                    // treasury
                    fiefItem.SubItems.Add("£" + thisFief.treasury);
                    // update treasury total
                    totTreas += thisFief.treasury;

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsFiefListView.Items.Add(fiefItem);
                    }

                }

                // add listviewitem with total GDP and tax income (all fiefs)
                string[] fiefItemTotalSubs = new string[] { "", "", "", "", "£" + totGDP, "£" + totTaxInc, "£" + totTreas };
                ListViewItem fiefItemTotal = new ListViewItem(fiefItemTotalSubs);
                this.royalGiftsFiefListView.Items.Add(fiefItemTotal);

                // POSITIONS
                foreach (KeyValuePair<byte, Position> thisPos in Globals_Game.positionMasterList)
                {
                    // only list posistions for this nationality
                    if (thisPos.Value.nationality == thisKing.nationality)
                    {
                        ListViewItem posItem = null;

                        // id
                        posItem = new ListViewItem(thisPos.Value.id.ToString());

                        // name
                        posItem.SubItems.Add(thisPos.Value.GetName(thisKing.language));

                        // stature
                        posItem.SubItems.Add(thisPos.Value.stature.ToString());

                        // holder
                        // get character
                        Character thisHolder = null;
                        if (!String.IsNullOrWhiteSpace(thisPos.Value.officeHolder))
                        {
                            if (Globals_Game.pcMasterList.ContainsKey(thisPos.Value.officeHolder))
                            {
                                thisHolder = Globals_Game.pcMasterList[thisPos.Value.officeHolder];
                            }
                        }

                        // title holder name & id
                        if (thisHolder != null)
                        {
                            posItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                        }
                        else
                        {
                            posItem.SubItems.Add(" - ");
                        }

                        if (posItem != null)
                        {
                            // add item to royalGiftsPositionListView
                            this.royalGiftsPositionListView.Items.Add(posItem);
                        }
                    }
                }

                Globals_Client.containerToView = this.royalGiftsContainer;
                Globals_Client.containerToView.BringToFront();
            }

        }

        /// <summary>
        /// Refreshes overlord province management display
        /// </summary>
        /// <param name="province">Province to display</param>
        public void RefreshProvinceContainer(Province province = null)
        {
            // disable controls until place selected in ListView
            this.DisableControls(this.provinceContainer.Panel1);

            // clear existing items in places lists
            this.provinceProvListView.Items.Clear();
            this.provinceFiefListView.Items.Clear();

            // iterates through provinces where the character holds the title, adding information to ListView
            foreach (string placeID in Globals_Client.myPlayerCharacter.myTitles)
            {
                ListViewItem provItem = null;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisProvince = Globals_Game.provinceMasterList[placeID];
                }

                if (thisProvince != null)
                {
                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // owner
                    PlayerCharacter thisOwner = thisProvince.owner;
                    if (thisOwner != null)
                    {
                        provItem.SubItems.Add(thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate
                    // get a fief
                    Fief thisFief = null;
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            thisFief = fiefEntry.Value;
                            break;
                        }
                    }

                    // get tax rate from fief
                    if (thisFief != null)
                    {
                        provItem.SubItems.Add(thisFief.keyStatsCurrent[12].ToString());
                    }

                    // kingdom ID
                    provItem.SubItems.Add(thisProvince.GetCurrentKingdom().id);

                    // kingdom name
                    provItem.SubItems.Add(thisProvince.GetCurrentKingdom().name);

                    // see if province to view has been passed in
                    if (province != null)
                    {
                        if (province == thisProvince)
                        {
                            provItem.Selected = true;
                        }
                    }

                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceProvListView.Items.Add(provItem);
                    }
                }

            }

            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Refreshes information the fief list in the overlord's province management display
        /// </summary>
        public void RefreshProvinceFiefList(Province p)
        {
            bool underOccupation = false;

            // clear existing items in list
            this.provinceFiefListView.Items.Clear();

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                ListViewItem fiefItem = null;

                if (fiefEntry.Value.province == p)
                {
                    // check for enemy occupation
                    underOccupation = fiefEntry.Value.CheckEnemyOccupation();

                    // id
                    fiefItem = new ListViewItem(fiefEntry.Value.id);

                    // name
                    fiefItem.SubItems.Add(fiefEntry.Value.name);

                    // owner name & id
                    if (fiefEntry.Value.owner != null)
                    {
                        fiefItem.SubItems.Add(fiefEntry.Value.owner.firstName + " " + fiefEntry.Value.owner.familyName + " (" + fiefEntry.Value.owner.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // GDP
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[1]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // last tax income
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[11]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // check if underOccupation message needed
                    if (underOccupation)
                    {
                        fiefItem.SubItems.Add("Under enemy occupation!");
                        fiefItem.ForeColor = Color.Red;
                    }

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceFiefListView.Items.Add(fiefItem);
                    }
                }

            }
        }
    }
}
