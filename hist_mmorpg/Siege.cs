using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on a siege
    /// </summary>
    public class Siege
    {
        /// <summary>
        /// Holds siege ID
        /// </summary>
        public String siegeID { get; set; }
        /// <summary>
        /// Holds year the siege started
        /// </summary>
        public uint startYear { get; set; }
        /// <summary>
        /// Holds season the siege started
        /// </summary>
        public byte startSeason { get; set; }
        /// <summary>
        /// Holds besieging player (charID)
        /// </summary>
        public String besiegingPlayer { get; set; }
        /// <summary>
        /// Holds defending player (charID)
        /// </summary>
        public String defendingPlayer { get; set; }
        /// <summary>
        /// Holds besieging army (armyID)
        /// </summary>
        public String besiegerArmy { get; set; }
        /// <summary>
        /// Holds defending garrison (armyID)
        /// </summary>
        public String defenderGarrison { get; set; }
        /// <summary>
        /// Holds fief being besieged (fiefID)
        /// </summary>
        public String besiegedFief { get; set; }
        /// <summary>
        /// Holds days left in current season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds the keep level at the start of the siege
        /// </summary>
        public double startKeepLevel { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by attacker
        /// </summary>
        public int totalCasualtiesAttacker { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by defender
        /// </summary>
        public int totalCasualtiesDefender { get; set; }
        /// <summary>
        /// Holds total duration of siege so far (days)
        /// </summary>
        public double totalDays { get; set; }
        /// <summary>
        /// Holds additional defending army (armyID)
        /// </summary>
        public String defenderAdditional { get; set; }
        /// <summary>
        /// Holds season and year the siege ended
        /// </summary>
        public String endDate { get; set; }

        /// <summary>
        /// Constructor for Siege
        /// </summary>
		/// <param name="id">String holding ID of siege</param>
        /// <param name="startYr">uint holding year the siege started</param>
        /// <param name="startSeas">byte holding season the siege started</param>
        /// <param name="bsgPlayer">String holding besieging player (charID)</param>
        /// <param name="defPlayer">String holding defending player (charID)</param>
        /// <param name="bsgArmy">String holding besieging army (armyID)</param>
        /// <param name="defGarr">String holding defending garrison (armyID)</param>
        /// <param name="fief">String holding fief being besieged (fiefID)</param>
        /// <param name="day">double containing days left in current season</param>
        /// <param name="kpLvl">double containing the keep level at the start of the siege</param>
        /// <param name="totAtt">int containing total attacker casualties so far</param>
        /// <param name="totDef">int containing total defender casualties so far</param>
        /// <param name="totday">double containing days used by siege so far</param>
        /// <param name="defAdd">String holding additional defending army (armyID)</param>
        /// <param name="end">string holding season and year the siege ended</param>
        public Siege(String id, uint startYr, byte startSeas, string bsgPlayer, string defPlayer, string bsgArmy,
            string defGarr, string fief, double day, double kpLvl, int totAtt = 0, int totDef = 0, double totDay = 1,
            string defAdd = null, string end = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.FirstCharToUpper(id.Trim());

            if (!Utility_Methods.ValidateSiegeID(id))
            {
                throw new InvalidDataException("Siege ID must have the format 'Siege_' followed by some numbers");
            }

            // STARTSEAS
            if (!Utility_Methods.ValidateSeason(startSeas))
            {
                throw new InvalidDataException("Siege startSeason must be a byte between 0-3");
            }

            // BSGPLAYER
            // trim and ensure 1st is uppercase
            bsgPlayer = Utility_Methods.FirstCharToUpper(bsgPlayer.Trim());

            if (!Utility_Methods.ValidateCharacterID(bsgPlayer))
            {
                throw new InvalidDataException("Siege besiegingPlayer id must have the format 'Char_' followed by some numbers");
            }

            // DEFPLAYER
            // trim and ensure 1st is uppercase
            defPlayer = Utility_Methods.FirstCharToUpper(defPlayer.Trim());

            if (!Utility_Methods.ValidateCharacterID(defPlayer))
            {
                throw new InvalidDataException("Siege defendingPlayer id must have the format 'Char_' followed by some numbers");
            }

            // BSGARMY
            // trim and ensure 1st is uppercase
            bsgArmy = Utility_Methods.FirstCharToUpper(bsgArmy.Trim());

            if (!Utility_Methods.ValidateArmyID(bsgArmy))
            {
                throw new InvalidDataException("Siege besiegingArmy id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // DEFGARR
            // trim and ensure 1st is uppercase
            defGarr = Utility_Methods.FirstCharToUpper(defGarr.Trim());

            if (!Utility_Methods.ValidateArmyID(defGarr))
            {
                throw new InvalidDataException("Siege defendingGarrison id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // FIEF
            // trim and ensure is uppercase
            fief = fief.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(fief))
            {
                throw new InvalidDataException("Siege fief id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // DAY
            if (!Utility_Methods.ValidateDays(day))
            {
                throw new InvalidDataException("Siege days must be a double between 0-109");
            }

            // KPLVL
            if (!Utility_Methods.ValidateFiefDouble(kpLvl))
            {
                throw new InvalidDataException("Siege startKeepLevel must be a double >= 0");
            }

            // TOTATT
            if (totAtt < 0)
            {
                throw new InvalidDataException("Siege totalCasualtiesAttacker must be an integer >= 0");
            }

            // TOTDEF
            if (totDef < 0)
            {
                throw new InvalidDataException("Siege totalCasualtiesDefender must be an integer >= 0");
            }

            // TOTDAY
            if (totDay < 0)
            {
                throw new InvalidDataException("Siege totalDays must be a double >= 0");
            }

            // DEFADD
            if (!String.IsNullOrWhiteSpace(defAdd))
            {
                // trim and ensure 1st is uppercase
                defAdd = Utility_Methods.FirstCharToUpper(defAdd.Trim());

                if (!Utility_Methods.ValidateArmyID(defAdd))
                {
                    throw new InvalidDataException("Siege defenderAdditonal id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            this.siegeID = id;
            this.startYear = startYr;
            this.startSeason = startSeas;
            this.besiegingPlayer = bsgPlayer;
            this.defendingPlayer = defPlayer;
            this.besiegerArmy = bsgArmy;
            this.defenderGarrison = defGarr;
            this.besiegedFief = fief;
            this.days = day;
            this.startKeepLevel = kpLvl;
            this.totalCasualtiesAttacker = totAtt;
            this.totalCasualtiesDefender = totDef;
            this.totalDays = totDay;
            this.defenderAdditional = defAdd;
            if (String.IsNullOrWhiteSpace(end))
            {
                this.endDate = null;
            }
            else
            {
                this.endDate = end;
            }
        }

        /// <summary>
        /// Constructor for Siege taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Siege()
		{
		}
		
        /// <summary>
        /// Gets the fief being besieged
        /// </summary>
        /// <returns>The besieged fief</returns>
        public Fief GetFief()
        {
            Fief besiegedFief = null;

            if (!String.IsNullOrWhiteSpace(this.besiegedFief))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.besiegedFief))
                {
                    besiegedFief = Globals_Game.fiefMasterList[this.besiegedFief];
                }
            }

            return besiegedFief;
        }

        /// <summary>
        /// Gets the besieging army
        /// </summary>
        /// <returns>The besieging army</returns>
        public Army GetBesiegingArmy()
        {
            Army besieger = null;

            if (!String.IsNullOrWhiteSpace(this.besiegerArmy))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.besiegerArmy))
                {
                    besieger = Globals_Game.armyMasterList[this.besiegerArmy];
                }
            }

            return besieger;
        }

        /// <summary>
        /// Gets the defending garrison
        /// </summary>
        /// <returns>The defending garrison (Army)</returns>
        public Army GetDefenderGarrison()
        {
            Army defenderGarrison = null;

            if (!String.IsNullOrWhiteSpace(this.defenderGarrison))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.defenderGarrison))
                {
                    defenderGarrison = Globals_Game.armyMasterList[this.defenderGarrison];
                }
            }

            return defenderGarrison;
        }

        /// <summary>
        /// Gets the additional defending army
        /// </summary>
        /// <returns>The additional defending army</returns>
        public Army GetDefenderAdditional()
        {
            Army thisDefenderAdditional = null;

            if (!String.IsNullOrWhiteSpace(this.defenderAdditional))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.defenderAdditional))
                {
                    thisDefenderAdditional = Globals_Game.armyMasterList[this.defenderAdditional];
                }
            }

            return thisDefenderAdditional;
        }

        /// <summary>
        /// Gets the defending player
        /// </summary>
        /// <returns>The defending player</returns>
        public PlayerCharacter GetDefendingPlayer()
        {
            PlayerCharacter defendingPlyr = null;

            if (!String.IsNullOrWhiteSpace(this.defendingPlayer))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.defendingPlayer))
                {
                    defendingPlyr = Globals_Game.pcMasterList[this.defendingPlayer];
                }
            }

            return defendingPlyr;
        }

        /// <summary>
        /// Gets the besieging player
        /// </summary>
        /// <returns>The besieging player</returns>
        public PlayerCharacter GetBesiegingPlayer()
        {
            PlayerCharacter besiegingPlyr = null;

            if (!String.IsNullOrWhiteSpace(this.besiegingPlayer))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.besiegingPlayer))
                {
                    besiegingPlyr = Globals_Game.pcMasterList[this.besiegingPlayer];
                }
            }

            return besiegingPlyr;
        }

        /// <summary>
        /// Synchronises days for component objects
        /// </summary>
        /// <param name="newDays">double indicating new value for days</param>
        /// <param name="checkForAttrition">bool indicating whether to check for attrition</param>
        public void SyncSiegeDays(double newDays, bool checkForAttrition = true)
        {
            Army besieger = this.GetBesiegingArmy();
            Army defenderGarr = this.GetDefenderGarrison();
            Army defenderAdd = this.GetDefenderAdditional();
            bool defenderAttritonApplies = false;
            byte attritionChecks = 0;
            double difference = 0;

            // check to see if attrition checks are required
            // NOTE: no check required for seasonal update
            if (checkForAttrition)
            {
                // if the siege has had to 'wait' for some days
                if (this.days > newDays)
                {
                    // get number of days difference
                    difference = this.days - newDays;

                    // work out number of attrition checks needed
                    attritionChecks = Convert.ToByte(difference / 7);

                    // check if attrition has kicked in for defending forces
                    defenderAttritonApplies = this.CheckAttritionApplies();
                }

            }

            // adjust siege days to specified days
            this.days = newDays;

            // ATTACKING ARMY
            Character attackerLeader = besieger.GetLeader();
            if (attackerLeader != null)
            {
                attackerLeader.AdjustDays(attackerLeader.days - this.days);
            }
            else
            {
                besieger.days = this.days;
            }

            // check for attrition if required
            if (attritionChecks > 0)
            {
                uint totalAttackTroopsLost = 0;
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = besieger.CalcAttrition();
                    // apply attrition
                    totalAttackTroopsLost += besieger.ApplyTroopLosses(attritionModifer);
                }
                // update total attacker siege losses
                this.totalCasualtiesAttacker += Convert.ToInt32(totalAttackTroopsLost);
            }

            // DEFENDING GARRISON
            Character garrisonLeader = defenderGarr.GetLeader();
            if (garrisonLeader != null)
            {
                garrisonLeader.AdjustDays(garrisonLeader.days - this.days);
            }
            else
            {
                defenderGarr.days = this.days;
            }

            // check for attrition if required
            if (defenderAttritonApplies)
            {
                if (attritionChecks > 0)
                {
                    uint totalDefendTroopsLost = 0;
                    for (int i = 0; i < attritionChecks; i++)
                    {
                        // calculate attrition
                        double attritionModifer = defenderGarr.CalcAttrition();
                        // apply attrition
                        totalDefendTroopsLost += defenderGarr.ApplyTroopLosses(attritionModifer);
                    }
                    // update total defender siege losses
                    this.totalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                }
            }

            // ADDITIONAL DEFENDING ARMY
            if (defenderAdd != null)
            {
                Character defAddLeader = defenderAdd.GetLeader();
                if (defAddLeader != null)
                {
                    defAddLeader.AdjustDays(defAddLeader.days - this.days);
                }
                else
                {
                    defenderAdd.days = this.days;
                }

                // check for attrition if required
                if (defenderAttritonApplies)
                {
                    if (attritionChecks > 0)
                    {
                        uint totalDefendTroopsLost = 0;
                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = defenderAdd.CalcAttrition();
                            // apply attrition
                            totalDefendTroopsLost += defenderAdd.ApplyTroopLosses(attritionModifer);
                        }
                        // update total defender siege losses
                        this.totalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if attrition applies to the defending forces (based on bailiff management rating)
        /// </summary>
        /// <returns>bool indicating whether attrition applies</returns>
        public bool CheckAttritionApplies()
        {
            bool attritionApplies = false;
            Character thisBailiff = this.GetFief().bailiff;
            double bailiffManagement = 0;

            // get bailiff's management rating
            if (thisBailiff != null)
            {
                bailiffManagement = thisBailiff.management;
            }
            else
            {
                bailiffManagement = 4;
            }

            // check to see if attrition needs to be applied
            if ((this.totalDays / 60) > bailiffManagement)
            {
                attritionApplies = true;
            }

            return attritionApplies;
        }

        /// <summary>
        /// Updates siege at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating whether the siege has been dismantled</returns>
        public bool UpdateSiege()
        {
            bool siegeEnded = false;
            Character besiegerLeader = null;
            PlayerCharacter besiegerOwner = this.GetBesiegingPlayer(); ;

            // check if besieger still in field (i.e. has not been disbanded)
            if (String.IsNullOrWhiteSpace(this.besiegerArmy))
            {
                siegeEnded = true;
            }

            // check besieging player still alive
            else if ((besiegerOwner == null) || (!besiegerOwner.isAlive))
            {
                siegeEnded = true;
            }

            else
            {
                // DAYS
                // base allowance
                double newDays = 90;

                // get besieging leader
                besiegerLeader = this.GetBesiegingArmy().GetLeader();
                if (besiegerLeader != null)
                {
                    // set days to besieger leader's days (may be effected by traits)
                    newDays = besiegerLeader.days;
                }

                // synchronise days of all component objects
                this.SyncSiegeDays(newDays, false);
            }

            return siegeEnded;
        }

        /// <summary>
        /// Ends the siege
        /// </summary>
        /// <param name="siegeSuccessful">bool indicating whether the siege was successful</param>
        /// <param name="s">String containing circumstances under which the siege ended</param>
        public void SiegeEnd(bool siegeSuccessful, string circumstance = null)
        {
            // get principle objects
            PlayerCharacter defendingPlayer = this.GetDefendingPlayer();
            Army besiegingArmy = this.GetBesiegingArmy();
            Army defenderGarrison = this.GetDefenderGarrison();
            Character defenderLeader = defenderGarrison.GetLeader();
            PlayerCharacter besiegingPlayer = this.GetBesiegingPlayer();
            Army defenderAdditional = this.GetDefenderAdditional();
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
            }
            Fief besiegedFief = this.GetFief();
            Character besiegingArmyLeader = null;
            if (besiegingArmy != null)
            {
                besiegingArmyLeader = besiegingArmy.GetLeader();
            }

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(defendingPlayer.charID + "|fiefOwner");
            tempPersonae.Add(besiegingPlayer.charID + "|attackerOwner");
            // get besiegingArmyLeader
            if (besiegingArmyLeader != null)
            {
                tempPersonae.Add(besiegingArmyLeader.charID + "|attackerLeader");
            }
            // get defenderLeader
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            if (addDefendLeader != null)
            {
                tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = besiegedFief.id;

            // description
            string siegeDescription = "";
            if (String.IsNullOrWhiteSpace(circumstance))
            {
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += besiegingPlayer.firstName + " " + besiegingPlayer.familyName;
                siegeDescription += " have been forced to abandon the siege of " + besiegedFief.name;
                siegeDescription += ". The ownership of this fief is retained by ";
                siegeDescription += defendingPlayer.firstName + " " + defendingPlayer.familyName + ".";
            }
            else
            {
                siegeDescription = circumstance;
            }

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeEnd", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(siegeResult);

            // disband garrison
            defenderGarrison.DisbandArmy();
            defenderGarrison = null;

            // disband additional defending army (but only if siege was successful)
            if ((defenderAdditional != null) && (siegeSuccessful))
            {
                defenderAdditional.DisbandArmy();
                defenderAdditional = null;
            }

            // remove from PCs
            besiegingPlayer.mySieges.Remove(this.siegeID);
            defendingPlayer.mySieges.Remove(this.siegeID);

            // remove from fief
            besiegedFief.siege = null;

            // sync days of all effected objects (to remove influence of attacking leader's traits)
            // work out proportion of seasonal allowance remaining
            double daysProportion = 0;
            if (besiegingArmyLeader != null)
            {
                daysProportion = this.days / besiegingArmyLeader.GetDaysAllowance();
            }
            else
            {
                daysProportion = this.days / 90;
            }

            // iterate through characters in fief keep
            foreach (Character thisChar in besiegedFief.charactersInFief)
            {
                if (thisChar.inKeep)
                {
                    // reset character's days to reflect days spent in siege
                    thisChar.days = thisChar.GetDaysAllowance() * daysProportion;
                }
            }

            // remove from master list
            if (Globals_Game.siegeMasterList.ContainsKey(this.siegeID))
            {
                Globals_Game.siegeMasterList.Remove(this.siegeID);
            }
        }

        /// <summary>
        /// Retrieves information for Siege display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        public string DisplaySiegeData()
        {
            string siegeText = "";
            Fief siegeLocation = this.GetFief();
            PlayerCharacter fiefOwner = siegeLocation.owner;
            Army besieger = this.GetBesiegingArmy();
            PlayerCharacter besiegingPlayer = this.GetBesiegingPlayer();
            Army defenderGarrison = this.GetDefenderGarrison();
            Army defenderAdditional = this.GetDefenderAdditional();
            Character besiegerLeader = besieger.GetLeader();
            Character defGarrLeader = defenderGarrison.GetLeader();
            Character defAddLeader = null;
            if (defenderAdditional != null)
            {
                defAddLeader = defenderAdditional.GetLeader();
            }

            // ID
            siegeText += "ID: " + this.siegeID + "\r\n\r\n";

            // fief
            siegeText += "Fief: " + siegeLocation.name + " (Province: " + siegeLocation.province.name + ".  Kingdom: " + siegeLocation.province.kingdom.name + ")\r\n\r\n";

            // fief owner
            siegeText += "Fief owner: " + fiefOwner.firstName + " " + fiefOwner.familyName + " (ID: " + fiefOwner.charID + ")\r\n\r\n";

            // besieging player
            siegeText += "Besieging player: " + besiegingPlayer.firstName + " " + besiegingPlayer.familyName + " (ID: " + besiegingPlayer.charID + ")\r\n\r\n";

            // start date
            siegeText += "Start date: " + this.startYear + ", " + Globals_Game.clock.seasons[this.startSeason] + "\r\n\r\n";

            // duration so far
            siegeText += "Days used so far: " + this.totalDays + "\r\n\r\n";

            // days left in current season
            siegeText += "Days remaining in current season: " + this.days + "\r\n\r\n";

            // defending forces
            siegeText += "Defending forces: ";
            // only show details if player is defender
            string defenderText = siegeText + "\r\nGarrison: " + defenderGarrison.armyID + "\r\n";
            defenderText += "- Leader: ";
           
            if (defGarrLeader != null)
            {
                defenderText += defGarrLeader.firstName + " " + defGarrLeader.familyName + " (ID: " + defGarrLeader.charID + ")";
            }
            else
            {
                defenderText += "None";
            }
            defenderText += "\r\n";
            defenderText += "- [Kn: " + defenderGarrison.troops[0] + ";  MAA: " + defenderGarrison.troops[1]
                + ";  LCav: " + defenderGarrison.troops[2] + ";  Lng: " + defenderGarrison.troops[3]
                + ";  Crss: " + defenderGarrison.troops[4] + ";  Ft: " + defenderGarrison.troops[5]
                + ";  Rbl: " + defenderGarrison.troops[6] + "]";

            // additional army details
            if (defenderAdditional != null)
            {
                defenderText += "\r\n\r\nField army: " + defenderAdditional.armyID + "\r\n";
                defenderText += "- Leader: ";
                if (defAddLeader != null)
                {
                    defenderText += defAddLeader.firstName + " " + defAddLeader.familyName + " (ID: " + defAddLeader.charID + ")";
                }
                else
                {
                    defenderText += "None";
                }
                defenderText += "\r\n";
                defenderText += "- [Kn: " + defenderAdditional.troops[0] + ";  MAA: " + defenderAdditional.troops[1]
                    + ";  LCav: " + defenderAdditional.troops[2] + ";  Lng: " + defenderAdditional.troops[3]
                    + ";  Crss: " + defenderAdditional.troops[4] + ";  Ft: " + defenderAdditional.troops[5]
                    + ";  Rbl: " + defenderAdditional.troops[6] + "]";
            }

            defenderText += "\r\n\r\nTotal defender casualties so far (including attrition): " + this.totalCasualtiesDefender;


            // if player not defending, hide defending forces details
            string attackerText = siegeText + "Unknown";

            // besieging forces
            defenderText += "Besieging forces: ";
            attackerText += "Besieging forces: ";
            // only show details if player is besieger

            // besieging forces details
            attackerText += "\r\nField army: " + besieger.armyID + "\r\n";
            attackerText += "- Leader: ";
            if (besiegerLeader != null)
            {
                attackerText += besiegerLeader.firstName + " " + besiegerLeader.familyName + " (ID: " + besiegerLeader.charID + ")";
            }
            else
            {
                attackerText += "None";
            }
            attackerText += "\r\n";
            attackerText += "- [Kn: " + besieger.troops[0] + ";  MAA: " + besieger.troops[1]
                + ";  LCav: " + besieger.troops[2] + ";  Lng: " + besieger.troops[3] + ";  Crss: " + besieger.troops[4]
                + ";  Ft: " + besieger.troops[5] + ";  Rbl: " + besieger.troops[6] + "]";

            attackerText += "\r\n\r\nTotal attacker casualties so far (including attrition): " + this.totalCasualtiesAttacker;

            defenderText += "Unknown";
            siegeText += "\r\n\r\n";

            // keep level
            attackerText += "Keep level:\r\n - at start of siege: " + this.startKeepLevel + "\r\n";
            defenderText += "Keep level:\r\n - at start of siege: " + this.startKeepLevel + "\r\n";


            // current keep level
            attackerText += "- current: " + siegeLocation.keepLevel + "\r\n\r\n";
            defenderText += "- current: " + siegeLocation.keepLevel + "\r\n\r\n";

            attackerText += "Chance of success in next round:\r\n";
            // chance of storm success
            /* double keepLvl = this.calcStormKeepLevel(s);
            double successChance = this.calcStormSuccess(keepLvl); */
            // get battle values for both armies
            uint[] battleValues = besieger.CalculateBattleValues(defenderGarrison, Convert.ToInt32(siegeLocation.keepLevel));
            double successChance = Battle.CalcVictoryChance(battleValues[0], battleValues[1]);
            attackerText += "- storm: " + successChance + "\r\n";

            // chance of negotiated success
            attackerText += "- negotiated: " + successChance / 2 + "\r\n\r\n";

            return siegeText;
        }

        /// <summary>
        /// Implements conditional checks prior to a siege operation
        /// </summary>
        /// <returns>bool indicating whether siege operation can proceed</returns>
        /// <param name="operation">The operation - round or end</param>
        public bool ChecksBeforeSiegeOperation(string operation = "round")
        {
            bool proceed = true;
            int daysRequired = 0;

            if (operation.Equals("round"))
            {
                daysRequired = 10;
            }
            else if (operation.Equals("end"))
            {
                daysRequired = 1;
            }

            // check has min days required
            if (this.days < daysRequired)
            {
                proceed = false;
                string toDisplay = "There are not enough days remaining for this\r\na siege operation.  Operation cancelled.";
                Globals_Game.UpdatePlayer(GetBesiegingPlayer().playerID, toDisplay);
            }

            return proceed;
        }

        /// <summary>
        /// Processes the storming of the keep by attacking forces in the siege
        /// </summary>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public void SiegeStormRound(uint defenderCasualties, double originalKeepLvl)
        {
            bool stormSuccess = false;
            Fief besiegedFief = this.GetFief();
            Army besiegingArmy = this.GetBesiegingArmy();
            Army defenderGarrison = this.GetDefenderGarrison();
            Army defenderAdditional = this.GetDefenderAdditional();
            PlayerCharacter attackingPlayer = this.GetBesiegingPlayer();
            Character defenderLeader = defenderGarrison.GetLeader();
            Character attackerLeader = besiegingArmy.GetLeader();
            double statureChange = 0;

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(this.GetDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(this.GetBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = this.GetFief().id;

            // description
            string siegeDescription = "";

            // get STORM RESULT
            uint[] battleValues = besiegingArmy.CalculateBattleValues(defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel), true);
            stormSuccess = Battle.DecideBattleVictory(battleValues[0], battleValues[1]);

            // KEEP DAMAGE
            // base damage to keep level (10%)
            double keepDamageModifier = 0.1;

            // calculate further damage, based on comparative battle values (up to extra 15%)
            // divide attackerBV by defenderBV to get extraDamageMultiplier
            double extraDamageMultiplier = battleValues[0] / battleValues[1];

            // ensure extraDamageMultiplier is max 10
            if (extraDamageMultiplier > 10)
            {
                extraDamageMultiplier = 10;
            }

            // generate random double 0-1 to see what proportion of extraDamageMultiplier will apply
            double myRandomDouble = Utility_Methods.GetRandomDouble(1);
            extraDamageMultiplier = extraDamageMultiplier * myRandomDouble;

            keepDamageModifier += (0.015 * extraDamageMultiplier);
            keepDamageModifier = (1 - keepDamageModifier);

            // apply keep damage
            besiegedFief.keepLevel = besiegedFief.keepLevel * keepDamageModifier;

            // CASUALTIES, based on comparative battle values and keep level
            // 1. DEFENDER
            // defender base casualtyModifier
            double defenderCasualtyModifier = 0.01;
            defenderCasualtyModifier = defenderCasualtyModifier * (Convert.ToDouble(battleValues[0]) / battleValues[1]);

            // apply casualties
            defenderCasualties += defenderGarrison.ApplyTroopLosses(defenderCasualtyModifier);
            // update total defender siege losses
            this.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // 2. ATTACKER
            double attackerCasualtyModifier = 0.01;
            attackerCasualtyModifier = attackerCasualtyModifier * (Convert.ToDouble(battleValues[1]) / battleValues[0]);
            // for attacker, add effects of keep level, modified by storm success
            if (stormSuccess)
            {
                attackerCasualtyModifier += (0.005 * besiegedFief.keepLevel);
            }
            else
            {
                attackerCasualtyModifier += (0.01 * besiegedFief.keepLevel);
            }
            // apply casualties
            uint attackerCasualties = besiegingArmy.ApplyTroopLosses(attackerCasualtyModifier);
            // update total attacker siege losses
            this.totalCasualtiesAttacker += Convert.ToInt32(attackerCasualties);

            // PC/NPC INJURIES
            // NOTE: defender only (attacker leaders assumed not to have climbed the walls)
            bool characterDead = false;
            if (defenderLeader != null)
            {
                // if defenderLeader is PC, check for casualties amongst entourage
                if (defenderLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                        characterDead = thisNPC.CalculateCombatInjury(defenderCasualtyModifier);

                        if (characterDead)
                        {
                            // process death
                            (defenderLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");
                        }
                    }
                }

                // check defenderLeader
                characterDead = defenderLeader.CalculateCombatInjury(defenderCasualtyModifier);

                if (characterDead)
                {
                    // remove as leader
                    defenderGarrison.leader = null;

                    // process death
                    defenderLeader.ProcessDeath("injury");
                }
            }

            if (stormSuccess)
            {
                // pillage fief
                Pillage_Siege.ProcessPillage(besiegedFief, besiegingArmy);

                // CAPTIVES
                // identify captives - fief owner, his family, and any PCs of enemy nationality
                List<Character> captives = new List<Character>();
                foreach (Character thisCharacter in besiegedFief.charactersInFief)
                {
                    if (thisCharacter.inKeep)
                    {
                        // fief owner and his family
                        if (!String.IsNullOrWhiteSpace(thisCharacter.familyID))
                        {
                            if (thisCharacter.familyID.Equals(this.GetDefendingPlayer().charID))
                            {
                                captives.Add(thisCharacter);
                            }
                        }

                        // PCs of enemy nationality
                        else if (thisCharacter is PlayerCharacter)
                        {
                            if (!thisCharacter.nationality.Equals(attackingPlayer.nationality))
                            {
                                captives.Add(thisCharacter);
                            }
                        }
                    }
                }

                // collect ransom
                int thisRansom = 0;
                int totalRansom = 0;
                foreach (Character thisCharacter in captives)
                {
                    // PCs
                    if (thisCharacter is PlayerCharacter)
                    {
                        // calculate ransom (10% of total GDP)
                        thisRansom = Convert.ToInt32(((thisCharacter as PlayerCharacter).GetTotalGDP() * 0.1));
                        // remove from captive's home treasury
                        (thisCharacter as PlayerCharacter).GetHomeFief().treasury -= thisRansom;
                    }
                    // NPCs (family of fief's old owner)
                    else
                    {
                        // calculate ransom (family allowance)
                        string thisFunction = (thisCharacter as NonPlayerCharacter).GetFunction(this.GetDefendingPlayer());
                        thisRansom = Convert.ToInt32((thisCharacter as NonPlayerCharacter).CalcFamilyAllowance(thisFunction));
                        // remove from head of family's home treasury
                        this.GetDefendingPlayer().GetHomeFief().treasury -= thisRansom;
                    }

                    // add to besieger's home treasury
                    attackingPlayer.GetHomeFief().treasury += thisRansom;
                    totalRansom += thisRansom;
                }

                // calculate change to besieging player's stature
                statureChange = 0.1 * (this.GetFief().population / Convert.ToDouble(10000));

                // construct event description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY stormed the keep of " + this.GetFief().name + ".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescriptionFull += "and " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ".\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += this.GetDefendingPlayer().firstName + " " + this.GetDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureChange + " stature.";
                // details of ransoms
                if (totalRansom > 0)
                {
                    siegeDescriptionFull += "\r\n\r\nA number of persons (";
                    Character lastCaptive = captives.Last();
                    foreach (Character thisCharacter in captives)
                    {
                        siegeDescriptionFull += thisCharacter.firstName + " " + thisCharacter.familyName;
                        if (thisCharacter != lastCaptive)
                        {
                            siegeDescriptionFull += ", ";
                        }
                    }
                    siegeDescriptionFull += ") were ransomed for a total of £" + totalRansom + ".";
                }

                // end the siege
                this.SiegeEnd(true, siegeDescriptionFull);

                // change fief ownership
                besiegedFief.ChangeOwnership(attackingPlayer);
            }

            // storm unsuccessful
            else
            {
                // calculate change to besieging player's stature
                statureChange = -0.2 * (Convert.ToDouble(this.GetFief().population) / 10000);

                // description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescription += " were UNSUCCESSFULL in their attempt to storm the keep of " + this.GetFief().name;
                siegeDescription += ".\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescription += "and " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".\r\n\r\nThis failure has resulted in a loss of ";
                siegeDescription += statureChange + " for " + this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(siegeResult);

            // inform player of result
            System.Windows.Forms.MessageBox.Show(siegeDescription);

            // apply change to besieging player's stature
            this.GetBesiegingPlayer().AdjustStatureModifier(statureChange);

        }

        /// <summary>
        /// Processes a single negotiation round of the siege
        /// </summary>
        /// <returns>bool indicating whether negotiation was successful</returns>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public bool SiegeNegotiationRound(uint defenderCasualties, double originalKeepLvl)
        {
            bool negotiateSuccess = false;

            // get required objects
            Fief besiegedFief = this.GetFief();
            Army besieger = this.GetBesiegingArmy();
            Army defenderGarrison = this.GetDefenderGarrison();
            Character defenderLeader = defenderGarrison.GetLeader();
            Character attackerLeader = besieger.GetLeader();
            Army defenderAdditional = this.GetDefenderAdditional();

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(this.GetDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(this.GetBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = this.GetFief().id;

            // description
            string siegeDescription = "";

            // calculate success chance
            uint[] battleValues = besieger.CalculateBattleValues(defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel));
            double successChance = Battle.CalcVictoryChance(battleValues[0], battleValues[1]) / 2;

            // generate random double 0-100 to see if negotiation a success
            double myRandomDouble = Utility_Methods.GetRandomDouble(100);

            if (myRandomDouble <= successChance)
            {
                negotiateSuccess = true;
            }

            // negotiation successful
            if (negotiateSuccess)
            {
                // add to winning player's stature
                double statureIncrease = 0.2 * (this.GetFief().population / Convert.ToDouble(10000));
                this.GetBesiegingPlayer().AdjustStatureModifier(statureIncrease);

                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY negotiated an end to the siege of " + this.GetFief().name + ".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += this.GetDefendingPlayer().firstName + " " + this.GetDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureIncrease + " stature.";
                siegeDescriptionFull += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescriptionFull += besiegedFief.keepLevel + ".";

                // end the siege
                this.SiegeEnd(true, siegeDescriptionFull);

                // change fief ownership
                this.GetFief().ChangeOwnership(this.GetBesiegingPlayer());

            }

            // negotiation unsuccessful
            else
            {
                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                siegeDescription += " FAILED to negotiate an end to the siege of " + this.GetFief().name + ".";
                siegeDescription += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".";
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(siegeResult);

            // update total defender siege losses
            this.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // inform player of success
            Globals_Game.UpdatePlayer(GetDefendingPlayer().playerID, siegeDescription);

            return negotiateSuccess;
        }

        /// <summary>
        /// Processes a single reduction round of the siege
        /// </summary>
        /// <param name="type">The type of round - storm, negotiate, reduction (default)</param>
        public void SiegeReductionRound(string type = "reduction")
        {
            bool siegeRaised = false;
            Fief besiegedFief = this.GetFief();
            Army besieger = this.GetBesiegingArmy();
            Army defenderGarrison = this.GetDefenderGarrison();
            Army defenderAdditional = null;

            // check for sallying army
            if (!String.IsNullOrWhiteSpace(this.defenderAdditional))
            {
                defenderAdditional = this.GetDefenderAdditional();

                if (defenderAdditional.aggression > 0)
                {
                    // get odds
                    int battleOdds = Battle.GetBattleOdds(defenderAdditional, besieger);

                    // if odds OK, give battle
                    if (battleOdds >= defenderAdditional.combatOdds)
                    {
                        // process battle and apply results, if required
                        siegeRaised = Battle.GiveBattle(defenderAdditional, besieger, circumstance: "siege");

                        // check for disbandment of defenderAdditional and remove from siege if necessary
                        if (!siegeRaised)
                        {
                            if (!besiegedFief.armies.Contains(this.defenderAdditional))
                            {
                                defenderAdditional = null;
                            }
                        }

                    }
                }
            }

            if (siegeRaised)
            {
                // NOTE: if sally was success, siege is ended in Form1.giveBattle
                string toDisplay = "The defenders have successfully raised the siege!";
                Globals_Game.UpdatePlayer(GetDefendingPlayer().playerID, toDisplay);
                Globals_Game.UpdatePlayer(GetBesiegingPlayer().playerID, toDisplay);
            }

            else
            {
                Character defenderLeader = defenderGarrison.GetLeader();
                Character attackerLeader = besieger.GetLeader();

                // process results of siege round
                // reduce keep level by 8%
                double originalKeepLvl = besiegedFief.keepLevel;
                besiegedFief.keepLevel = (besiegedFief.keepLevel * 0.92);

                // apply combat losses to defenderGarrison
                // NOTE: attrition for both sides is calculated in siege.syncDays

                double combatLosses = 0.01;
                uint troopsLost = defenderGarrison.ApplyTroopLosses(combatLosses);

                // check for death of defending PCs/NPCs
                if (defenderLeader != null)
                {
                    bool characterDead = false;

                    // if defenderLeader is PC, check for casualties amongst entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                            characterDead = thisNPC.CalculateCombatInjury(combatLosses);

                            if (characterDead)
                            {
                                // process death
                                (defenderLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");
                            }
                        }
                    }

                    // check defenderLeader
                    characterDead = defenderLeader.CalculateCombatInjury(combatLosses);

                    if (characterDead)
                    {
                        // remove as leader
                        defenderGarrison.leader = null;

                        // process death
                        defenderLeader.ProcessDeath("injury");
                    }
                }

                // update total days (NOTE: siege.days will be updated in syncDays)
                this.totalDays += 10;

                // synchronise days
                this.SyncSiegeDays(this.days - 10);

                if (type.Equals("reduction"))
                {
                    // UPDATE SIEGE LOSSES
                    this.totalCasualtiesDefender += Convert.ToInt32(troopsLost);

                    // =================== construct and send JOURNAL ENTRY
                    // ID
                    uint entryID = Globals_Game.GetNextJournalEntryID();

                    // personae
                    List<string> tempPersonae = new List<string>();
                    tempPersonae.Add(this.GetDefendingPlayer().charID + "|fiefOwner");
                    tempPersonae.Add(this.GetBesiegingPlayer().charID + "|attackerOwner");
                    if (attackerLeader != null)
                    {
                        tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
                    }
                    if (defenderLeader != null)
                    {
                        tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
                    }
                    // get additional defending leader
                    Character addDefendLeader = null;
                    if (defenderAdditional != null)
                    {
                        addDefendLeader = defenderAdditional.GetLeader();
                        if (addDefendLeader != null)
                        {
                            tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                        }
                    }
                    string[] siegePersonae = tempPersonae.ToArray();

                    // location
                    string siegeLocation = this.GetFief().id;

                    // use popup text as description
                    string siegeDescription = "On this day of Our Lord the siege of " + this.GetFief().name + " by ";
                    siegeDescription += this.GetBesiegingPlayer().firstName + " " + this.GetBesiegingPlayer().familyName;
                    siegeDescription += " continued.  The besieged garrison lost a total of " + troopsLost + " troops, ";
                    siegeDescription += " and the keep level was reduced from " + originalKeepLvl + " to ";
                    siegeDescription += besiegedFief.keepLevel + ".";

                    // put together new journal entry
                    JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeReduction", loc: siegeLocation, descr: siegeDescription);

                    // add new journal entry to pastEvents
                    Globals_Game.AddPastEvent(siegeResult);
                }

                if (type.Equals("storm"))
                {
                    this.SiegeStormRound(troopsLost, originalKeepLvl);
                }
                else if (type.Equals("negotiation"))
                {
                    this.SiegeNegotiationRound(troopsLost, originalKeepLvl);
                }
            }

        }

    }
}
