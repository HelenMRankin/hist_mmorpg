using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ProtoBuf;

namespace hist_mmorpg 
{
    /// <summary>
    /// Class storing data on army 
    /// </summary>
    /// 
    [ProtoContract]
    public class Army 
    {
        //TODO Fix boolean logic 
		/// <summary>
		/// Holds army ID
		/// </summary>
        
        [ProtoMember(1)]
		public String armyID { get; set; }
        /// <summary>
        /// Holds troops in army
        /// 0 = knights
        /// 1 = menAtArms
        /// 2 = lightCav
        /// 3 = longbowmen
        /// 4 = crossbowmen
        /// 5 = foot
        /// 6 = rabble
        /// </summary>
        [ProtoMember(2)]
        public uint[] troops = new uint[7] {0, 0, 0, 0, 0, 0, 0};
        /// <summary>
        /// Holds army leader (ID)
        /// </summary>
        [ProtoMember(3)]
        public string leader { get; set; }
        /// <summary>
        /// Holds army owner (ID)
        /// </summary>
        [ProtoMember(4)]
        public string owner { get; set; }
        /// <summary>
        /// Holds army's remaining days in season
        /// </summary>
        [ProtoMember(5)]
        public double days { get; set; }
        /// <summary>
        /// Holds army location (fiefID)
        /// </summary>
        [ProtoMember(6)]
        public string location { get; set; }
        /// <summary>
        /// Indicates whether army is being actively maintained by owner
        /// </summary>
        [ProtoMember(7)]
        public bool isMaintained { get; set; }
        /// <summary>
        /// Indicates army's aggression level (automated response to combat)
        /// </summary>
        [ProtoIgnore]
        public byte aggression { get; set; }
        /// <summary>
        /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
        /// </summary>
        [global::ProtoBuf.ProtoMember(9)]
        public byte combatOdds { get; set; }

        /// <summary>
        /// Constructor for Army
        /// </summary>
		/// <param name="id">String holding ID of army</param>
        /// <param name="ldr">string holding ID of army leader</param>
        /// <param name="own">string holding ID of army owner</param>
        /// <param name="day">double holding remaining days in season for army</param>
        /// <param name="loc">string holding army location (fiefID)</param>
        /// <param name="maint">bool indicating whether army is being actively maintained by owner</param>
        /// <param name="aggr">byte indicating army's aggression level</param>
        /// <param name="odds">byte indicating army's combat odds value</param>
        /// <param name="trp">uint[] holding troops in army</param>
        public Army(String id, string ldr, string own, double day, string loc, bool maint = false, byte aggr = 1, byte odds = 9, uint[] trp = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.FirstCharToUpper(id.Trim());

            if (!Utility_Methods.ValidateArmyID(id))
            {
                throw new InvalidDataException("Army ID must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // LDR
            if (!String.IsNullOrWhiteSpace(ldr))
            {
                // trim and ensure 1st is uppercase
                ldr = Utility_Methods.FirstCharToUpper(ldr.Trim());

                if (!Utility_Methods.ValidateCharacterID(ldr))
                {
                    throw new InvalidDataException("Army leader ID must have the format 'Char_' followed by some numbers");
                }
            }

            // OWN
            // trim and ensure 1st is uppercase
            own = Utility_Methods.FirstCharToUpper(own.Trim());

            if (!Utility_Methods.ValidateCharacterID(own))
            {
                throw new InvalidDataException("Army owner id must have the format 'Char_' followed by some numbers");
            }

            // DAY
            if (!Utility_Methods.ValidateDays(day))
            {
                throw new InvalidDataException("Army days must be a double between 0-109");
            }

            // LOC
            // trim and ensure is uppercase
            loc = loc.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(loc))
            {
                throw new InvalidDataException("Army location id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // AGGR
            // check is < 3
            if (aggr > 2)
            {
                throw new InvalidDataException("Army aggression level must be a byte less than 3");
            }

            this.armyID = id;
            this.leader = ldr;
            this.owner = own;
            this.days = day;
            this.location = loc;
            this.isMaintained = maint;
            this.aggression = aggr;
            this.combatOdds = odds;
            if (trp != null)
            {
                this.troops = trp;
            }
        }
        /// <summary>
        /// Constructor for Army taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Army()
		{
		}

        /// <summary>
        /// Performs functions associated with creating a new army
        /// </summary>
        public void AddArmy()
        {
            // get leader
            Character armyLeader = this.GetLeader();

            // get owner
            PlayerCharacter armyOwner = this.GetOwner();

            // get location
            Fief armyLocation = this.GetLocation();

            // add to armyMasterList
            Globals_Game.armyMasterList.Add(this.armyID, this);

            // add to owner's myArmies
            armyOwner.myArmies.Add(this);

            // add to leader
            if (armyLeader != null)
            {
                armyLeader.armyID = this.armyID;
            }

            // add to fief's armies
            armyLocation.armies.Add(this.armyID);

        }

        /// <summary>
        /// Maintains the specified field army
        /// </summary>
        public void MantainArmy()
        {
            string toDisplay = "";

            // get cost
            uint maintCost = this.CalcArmySize() * 500;

            // get available treasury
            Fief homeFief = this.GetOwner().GetHomeFief();
            int availTreas = homeFief.GetAvailableTreasury();

            // check if army is already maintained
            if (!this.isMaintained)
            {
                // check if can afford maintenance
                if (maintCost > availTreas)
                {
                    // display 'no' message
                    toDisplay += "Sorry, milord, to maintain this army would cost £" + maintCost + "\r\n";
                    toDisplay += "and you only have £" + availTreas + " available in the home treasury.";
                    Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                }
                //TODO this will be done client side
                else
                {
                    // present alternative number and ask for confirmation
                    toDisplay = "It will cost £" + maintCost + " to maintain this army";
                    toDisplay += " and you have £" + homeFief.treasury + " in your home treasury.\r\n\r\n";
                    toDisplay += "Do you wish to proceed?";
                    DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with operation?", MessageBoxButtons.OKCancel);

                    // if choose to proceed
                    if (dialogResult != DialogResult.Cancel)
                    {
                        // set isMaintained
                        this.isMaintained = true;

                        // deduct funds from treasury
                        homeFief.treasury -= Convert.ToInt32(maintCost);

                        // display confirmation message
                        toDisplay = "Army maintained at a cost of £" + maintCost + ".";

                        Globals_Game.UpdatePlayer(owner, "confirm:army:" + toDisplay);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the army's aggression and combatOdds values
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newAggroLevel">The new aggression value</param>
        /// <param name="newOddsValue">The new combatOdds value</param>
        public bool AdjustStandingOrders(byte newAggroLevel, byte newOddsValue)
        {
            bool success = true;

            // check values and alter if appropriate
            if (newAggroLevel < 0)
            {
                newAggroLevel = 0;
            }
            else if (newAggroLevel > 2)
            {
                newAggroLevel = 2;
            }
            if (newOddsValue < 0)
            {
                newOddsValue = 0;
            }
            else if (newOddsValue > 9)
            {
                newOddsValue = 9;
            }

            // update army's values
            this.aggression = newAggroLevel;
            this.combatOdds = newOddsValue;

            if ((this.aggression != newAggroLevel) || (this.combatOdds != newOddsValue))
            {
                success = false;
            }

            return success;
        }
		
        /// <summary>
        /// Assigns a new leader to the army
        /// </summary>
        /// <remarks>
        /// Predicate: assumes leader is in same fief as army
        /// NOTE: you CAN assign a null character as leader (i.e. the army becomes leaderless)
        /// </remarks>
        /// /// <param name="newLeader">The new leader (can be null)</param>
        public void AssignNewLeader(Character newLeader)
        {
            // check if already leader of another army and remove if necessary
            Army otherArmy = null;
            if (!String.IsNullOrWhiteSpace(newLeader.armyID))
            {
                otherArmy = newLeader.GetArmy();

                if (otherArmy != null)
                {
                    if (otherArmy != this)
                    {
                        otherArmy.leader = null;
                    }
                }
            }

            // check if army is involved in a siege
            Siege mySiege = this.GetSiege();

            // Remove army from current leader
            Character oldLeader = this.GetLeader();
            if (oldLeader != null)
            {
                oldLeader.armyID = null;
            }

            // if no new leader (i.e. if just removing old leader)
            if (newLeader == null)
            {
                // in army, set new leader
                this.leader = null;
            }

            // if is new leader
            else
            {
                // add army to new leader
                newLeader.armyID = this.armyID;

                // in army, set new leader
                this.leader = newLeader.charID;

                // if new leader is NPC, remove from player's entourage
                if (newLeader is NonPlayerCharacter)
                {
                    (newLeader as NonPlayerCharacter).inEntourage = false;
                }

                // calculate days synchronisation
                double minDays = Math.Min(newLeader.days, this.days);
                double maxDays = Math.Max(newLeader.days, this.days);
                double difference = maxDays - minDays;

                if (newLeader.days != minDays)
                {
                    // synchronise days
                    newLeader.AdjustDays(difference);
                }
                else
                {
                    // if army not involved in siege, check for attrition in normal way
                    if (mySiege == null)
                    {
                        byte attritionChecks = 0;
                        attritionChecks = Convert.ToByte(difference / 7);

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = this.CalcAttrition();
                            // apply attrition
                            this.ApplyTroopLosses(attritionModifer);
                        }
                    }

                    // if army is involved in siege, attrition applied at siege level
                    else
                    {
                        mySiege.SyncSiegeDays(newLeader.days);
                    }

                }

            }
        }

        /// <summary>
        /// Calculates total army size
        /// </summary>
        /// <returns>uint containing army size</returns>
        public uint CalcArmySize()
        {
            uint armySize = 0;

            foreach (uint troopType in this.troops)
            {
                armySize += troopType;
            }

            return armySize;
        }

        /// <summary>
        /// Moves army to another fief
        /// </summary>
        /// <remarks>
        /// Predicate: assumes leader has enough days for movement
        /// </remarks>
        /// <returns>bool indicating success</returns>
        /// <param name="showAttrition">bool indicating whether to display message containing attrition losses</param>
        public bool MoveArmy(bool showAttrition = false)
        {
            bool success = false;

            // get leader
            Character myLeader = this.GetLeader();

            // get old fief
            Fief myOldFief = Globals_Game.fiefMasterList[this.location];
            // get new fief
            Fief myNewFief = Globals_Game.fiefMasterList[myLeader.location.id];

            // remove from old fief
            myOldFief.RemoveArmy(this.armyID);

            // add to new fief
            myNewFief.AddArmy(this.armyID);

            // change location
            this.location = myLeader.location.id;

            // update days
            this.days = myLeader.days;

            // calculate attrition
            double attritionModifer = this.CalcAttrition();
            // apply attrition
            uint troopsLost = this.ApplyTroopLosses(attritionModifer);

            // inform player of losses
            if (showAttrition)
            {
                if (troopsLost > 0)
                {
                    if (Globals_Game.userChars.ContainsKey(owner))
                    {
                        Globals_Game.UpdatePlayer(owner, "update:army:" + "Your army (" + this.armyID + ") has lost " + troopsLost + " from attrition in " + myNewFief.name);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Calculates movement modifier for the army
        /// </summary>
        /// <returns>uint containing movement modifier</returns>
        public uint CalcMovementModifier()
        {
            uint movementMod = 1;

            // generate random double (0-100)
            Double myRandomDouble = Globals_Game.myRand.NextDouble() * 100;

            // calculate chance of modifier based on army size
            Double modifierChance = (Math.Floor(this.CalcArmySize() / (Double)1000) * 3);

            // check to see if modifier required
            if (myRandomDouble <= modifierChance)
            {
                movementMod = 3;
            }

            return movementMod;
        }

        /// <summary>
        /// Calculates attrition for the army
        /// </summary>
        /// <returns>double containing casualty modifier to be applied troops</returns>
        public double CalcAttrition()
        {
            uint troopNumbers = this.CalcArmySize();
            double casualtyModifier = 0;
            Double attritionChance = 0;
            String toDisplay = "";

            // ensure is no attrition if army maintained
            if (!this.isMaintained)
            {
                // get fief
                Fief currentFief = this.GetLocation();

                // get leader
                Character myLeader = this.GetLeader();

                // calculate base chance of attrition
                attritionChance = (troopNumbers / Convert.ToDouble(currentFief.population)) * 100;
                toDisplay += "Base chance: " + attritionChance + "\r\n";

                // factor in effect of leader (need to check if army has leader)
                if (myLeader != null)
                {
                    // apply effect of leader
                    attritionChance = attritionChance - ((myLeader.CalculateStature() + myLeader.management) / 2);
                    toDisplay += "Leader effect: " + (myLeader.CalculateStature() + myLeader.management) / 2 + "\r\n";
                }

                // factor in effect of season (add 20 if is winter or spring)
                if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                {
                    attritionChance = attritionChance + 20;
                    toDisplay += "Season effect: 20\r\n";
                }

                // normalise chance of attrition
                if (attritionChance < 10)
                {
                    attritionChance = 10;
                }
                else if (attritionChance > 100)
                {
                    attritionChance = 100;
                }

                // generate random number (0-100) to check if attrition occurs
                Double randomPercent = Globals_Game.myRand.NextDouble() * 100;

                // check if attrition occurs
                if (randomPercent <= attritionChance)
                {
                    // calculate base casualtyModifier
                    casualtyModifier = (troopNumbers / Convert.ToDouble(currentFief.population)) / 10;
                    toDisplay += "casualtyModifier: " + casualtyModifier + "\r\n";

                    // factor in effect of season on potential losses (* 3 if is winter or spring)
                    if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                    {
                        casualtyModifier = casualtyModifier * 3;
                        toDisplay += "casualtyModifier after seasonal effect: " + casualtyModifier + "\r\n";
                    }

                }
            }

            if (casualtyModifier > 0)
            {
                Globals_Game.UpdatePlayer(owner, "update:army:" + toDisplay);
            }

            return casualtyModifier;
        }

        /// <summary>
        /// Applies troop losses after attrition, battle, siege, etc.
        /// </summary>
        /// <remarks>
        /// Predicate: assumes (lossModifier gt 0) && (lossModifier lt 1)
        /// </remarks>
        /// <returns>uint containing total number of troops lost</returns>
        /// <param name="lossModifier">modifier to be applied to each troop type</param>
        public uint ApplyTroopLosses(double lossModifier)
        {
            // keep track of total troops lost
            uint troopsLost = 0;

            for (int i = 0; i < this.troops.Length; i++ )
            {
                uint thisTypeLost = Convert.ToUInt32(this.troops[i] * lossModifier);
                troopsLost += thisTypeLost;
                this.troops[i] -= thisTypeLost;
            }

            return troopsLost;
        }

        /// <summary>
        /// Creates a detachment from the army's troops and leaves it in the fief
        /// </summary>
        /// <remarks>
        /// Predicate: assumes details[0] through details[details.Length-1] contain strings that can be converted to uint
        /// </remarks>
        /// <returns>bool indicating success of transfer</returns>
        /// <param name="details">string[] containing troop numbers and recipient (ID)</param>
        public bool CreateDetachment(string[] details)
        {
            bool proceed = true;
            bool adjustDays = true;
            int daysTaken = 0;
            uint totalTroopsToTransfer = 0;
            string toDisplay = "";
            string[] troopTypeLabels = new string[] { "knights", "men-at-arms", "light cavalry", "longbowmen", "crossbowmen", "foot", "rabble" };
            Character myLeader = null;

            // carry out CONDITIONAL CHECKS

            // 1. check arry length
            if (details.Length != 8)
            {
                proceed = false;
                adjustDays = false;
                toDisplay = "Not enough data parts in transfer details array.";
                Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
            }
            else
            {
                // 2. check each troop type; if not enough in army, cancel
                for (int i = 0; i < details.Length - 1; i++)
                {
                    if (Convert.ToUInt32(details[i]) > this.troops[i])
                    {
                        toDisplay = "You don't have enough " + troopTypeLabels[i] + " in your army for that transfer.";
                        Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        totalTroopsToTransfer += Convert.ToUInt32(details[i]);
                    }
                }

                if (proceed)
                {
                    // 3. if no troops selected for transfer, cancel
                    if (totalTroopsToTransfer < 1)
                    {
                        toDisplay = "You haven't selected any troops for transfer.";
                        Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        // 4. check have minimum days necessary for transfer
                        Client client = Globals_Server.clients[owner];
                        if (client == null) { } //TODO choose what to do when client unexpectedly null
                        if (client.armyToView.days < 10)
                        {
                            toDisplay = "You don't have enough days left for this transfer.";
                            Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                            proceed = false;
                            adjustDays = false;
                        }
                        else
                        {
                            // 5. check if have enough days for transfer in this instance

                            // calculate time taken for transfer
                            daysTaken = Globals_Game.myRand.Next(10, 31);

                            if (daysTaken > client.armyToView.days)
                            {
                                toDisplay = "Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.";
                                Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                                proceed = false;
                            }
                            else
                            {
                                // 6. check transfer recipient exists
                                if (!Globals_Game.pcMasterList.ContainsKey(details[7]))
                                {
                                    toDisplay = "Cannot identify transfer recipient.";
                                    Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                                    
                                    proceed = false;
                                    adjustDays = false;
                                }
                                else
                                {
                                    // 7. check army has a leader

                                    // get leader
                                    myLeader = this.GetLeader();

                                    if (myLeader == null)
                                    {
                                        toDisplay = "An army without a leader cannot make a transfer.";
                                        Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                                        proceed = false;
                                        adjustDays = false;
                                    }
                                    else
                                    {
                                        // 8. if reduces army to < 100 troops, display warning
                                        if (((this.CalcArmySize() - totalTroopsToTransfer) < 100) && (proceed))
                                        {
                                            toDisplay = "This transfer will reduce your army manpower to dangerous levels.  Click OK to proceed.";
                                            DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with transfer?", MessageBoxButtons.OKCancel);

                                            // if choose to cancel
                                            if (dialogResult == DialogResult.Cancel)
                                            {
                                                toDisplay = "Transfer cancelled.";
                                                Globals_Game.UpdatePlayer(owner, "error:army:" + toDisplay);
                                                proceed = false;
                                                adjustDays = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // remove troops from army
                for (int i = 0; i < this.troops.Length; i++)
                {
                    this.troops[i] -= Convert.ToUInt32(details[i]);
                }

                // get fief
                Fief thisFief = this.GetLocation();

                // create transfer entry
                string[] thisTransfer = new string[10] { this.owner, details[7], details[0], details[1], details[2],
                            details[3], details[4], details[5], details[6], (this.days - daysTaken).ToString() };

                // add to fief's troopTransfers list
                string tranferID = Globals_Game.GetNextDetachmentID();
                thisFief.troopTransfers.Add(tranferID, thisTransfer);

                // check detachment added to troopTransfers
                if (!thisFief.troopTransfers.ContainsKey(tranferID))
                {
                    proceed = false;
                }
            }

            if (adjustDays)
            {
                // adjust days
                myLeader.AdjustDays(daysTaken);

                // calculate possible attrition for army
                byte attritionChecks = Convert.ToByte(daysTaken / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = this.CalcAttrition();

                    // apply attrition
                    if (attritionModifer > 0)
                    {
                        this.ApplyTroopLosses(attritionModifer);
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Calculates the army's combat value for a combat engagement (NOTE: doesn't include leadership modifier)
        /// </summary>
        /// <returns>double containing combat value</returns>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        public double CalculateCombatValue(int keepLvl = 0)
        {
            double cv = 0;

            // get leader and owner
            Character myLeader = this.GetLeader();
            PlayerCharacter myOwner = this.GetOwner();

            // get nationality (effects combat values)
            string troopNationality = myOwner.nationality.natID;

            // get combat values for that nationality
            uint[] thisCombatValues = Globals_Server.combatValues[troopNationality];

            // get CV for each troop type
            for (int i = 0; i < this.troops.Length; i++)
            {
                cv += this.troops[i] * thisCombatValues[i];
            }

            // if calculating defender during keep storm, account for keep level
            // (1000 foot per level)
            if (keepLvl > 0)
            {
                cv += (keepLvl * 1000) * thisCombatValues[5];
            }

            // get leader's combat value
            if (myLeader != null)
            {
                cv += myLeader.GetCombatValue();

                // if leader is PC, get CV of entourage (male characters only)
                if (myLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (myLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        if ((myLeader as PlayerCharacter).myNPCs[i].inEntourage)
                        {
                            if ((myLeader as PlayerCharacter).myNPCs[i].isMale)
                            {
                                cv += (myLeader as PlayerCharacter).myNPCs[i].GetCombatValue();
                            }
                        }
                    }
                }
            }

            return cv;
        }

        /// <summary>
        /// Calculates the estimated number of troops of all types in the army
        /// </summary>
        /// <returns>uint[] containing estimated troop numbers for all types</returns>
        /// <param name="observer">The character making the estimate</param>
        public uint[] GetTroopsEstimate(Character observer)
        {
            uint[] troopNumbers = new uint[7] {0, 0, 0, 0, 0, 0, 0};

            if (observer != null)
            {
                // get random int (0-2) to decide whether to over- or under-estimate troop number
                int overUnder = Globals_Game.myRand.Next(3);

                // get observer's estimate variance (based on his leadership value)
                double estimateVariance = observer.GetEstimateVariance();

                // perform estimate for each troop type
                for (int i = 0; i < troopNumbers.Length; i++)
                {
                    // get troop number upon which to base estimate
                    troopNumbers[i] = this.troops[i];

                    // generate random double between 0 and estimate variance to decide variance in this case
                    double thisVariance = Utility_Methods.GetRandomDouble(estimateVariance);

                    // apply variance (negatively or positively) to troop number
                    // 0 = under-estimate, 1-2 = over-estimate
                    if (overUnder == 0)
                    {
                        troopNumbers[i] = troopNumbers[i] - Convert.ToUInt32(troopNumbers[i] * thisVariance);
                    }
                    else
                    {
                        troopNumbers[i] = troopNumbers[i] + Convert.ToUInt32(troopNumbers[i] * thisVariance);
                    }
                }
            }

            return troopNumbers;
        }

        /// <summary>
        /// Gets the army's location (fief)
        /// </summary>
        /// <returns>the fief</returns>
        public Fief GetLocation()
        {
            Fief thisFief = null;

            if (!String.IsNullOrWhiteSpace(this.location))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.location))
                {
                    thisFief = Globals_Game.fiefMasterList[this.location];
                }
            }

            return thisFief;
        }

        /// <summary>
        /// Gets the army's owner
        /// </summary>
        /// <returns>the owner</returns>
        public PlayerCharacter GetOwner()
        {
            PlayerCharacter myOwner = null;

            // get leader from PC master list
            if (!String.IsNullOrWhiteSpace(this.owner))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.owner))
                {
                    myOwner = Globals_Game.pcMasterList[this.owner];
                }
            }

            return myOwner;
        }

        /// <summary>
        /// Gets the army's leader
        /// </summary>
        /// <returns>the leader</returns>
        public Character GetLeader()
        {
            Character myLeader = null;

            if (!String.IsNullOrWhiteSpace(this.leader))
            {
                // get leader from appropriate master list
                if (Globals_Game.npcMasterList.ContainsKey(this.leader))
                {
                    myLeader = Globals_Game.npcMasterList[this.leader];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(this.leader))
                {
                    myLeader = Globals_Game.pcMasterList[this.leader];
                }
            }

            return myLeader;
        }

        /// <summary>
        /// Performs functions associated with army move for an army unaccompanied by a leader 
        /// </summary>
        /// <remarks>
        /// Predicate: assumes army has no leader
        /// Predicate: assumes army has sufficient days
        /// </remarks>
        /// <param name="target">The fief to move to</param>
        /// <param name="travelCost">The cost of moving to target fief</param>
        public void MoveWithoutLeader(Fief target, double travelCost)
        {
            // get current location
            Fief from = this.GetLocation();

            // remove from current fief
            from.armies.Remove(this.armyID);

            // add to target fief
            target.armies.Add(this.armyID);

            // change location
            this.location = target.id;

            // change days
            this.days = this.days - travelCost;

            // calculate attrition
            double attritionModifer = this.CalcAttrition();

            // apply attrition
            uint troopsLost = this.ApplyTroopLosses(attritionModifer);
        }

        /// <summary>
        /// Checks to see if army is besieging a fief/keep
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string CheckIfBesieger()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.GetLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.GetSiege();

                // check if this army is besieging army
                if (thisSiege.GetBesiegingArmy() == this)
                {
                    thisSiegeID = thisFief.siege;
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army is the defending garrison in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string CheckIfSiegeDefenderGarrison()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.GetLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.GetSiege();
                if (thisSiege.GetDefenderGarrison() == this)
                {
                    thisSiegeID = thisFief.siege;
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army is an additional defending army in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string CheckIfSiegeDefenderAdditional()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.GetLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.GetSiege();
                if (thisSiege.defenderAdditional != null)
                {
                    if (thisSiege.GetDefenderAdditional() == this)
                    {
                        thisSiegeID = thisFief.siege;
                    }
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army has any role (defending or besieging) in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string CheckForSiegeRole()
        {
            string thisSiegeID = null;

            // check if army is a defending garrison in a siege
            thisSiegeID = this.CheckIfSiegeDefenderGarrison();

            if (String.IsNullOrWhiteSpace(thisSiegeID))
            {
                // check if army is an additional defending army in a siege
                thisSiegeID = this.CheckIfSiegeDefenderAdditional();

                if (String.IsNullOrWhiteSpace(thisSiegeID))
                {
                    // check if army is besieger in a siege
                    thisSiegeID = this.CheckIfBesieger();
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Gets the siege object associated with the army (or null)
        /// </summary>
        /// <returns>The siege</returns>
        public Siege GetSiege()
        {
            Siege thisSiege = null;

            // check for siege ID associated with army
            string siegeID = this.CheckForSiegeRole();

            // get siege
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                if (Globals_Game.siegeMasterList.ContainsKey(siegeID))
                {
                    thisSiege = Globals_Game.siegeMasterList[siegeID];
                }
            }

            return thisSiege;
        }
        
        /// <summary>
        /// Updates army data at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating if army has dissolved</returns>
        public bool UpdateArmy()
        {
            bool hasDissolved = false;
            bool attritionApplies = true;
            string siegeID = null;
            bool isSiegeDefGarr = false;
            bool isSiegeDefAdd = false;
            Siege thisSiege = null;

            // check for SIEGE INVOLVEMENT
            // check that army is a defending garrison in a siege
            siegeID = this.CheckIfSiegeDefenderGarrison();
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                isSiegeDefGarr = true;
                thisSiege = this.GetSiege();
            }
            else
            {
                siegeID = this.CheckIfSiegeDefenderAdditional();
                // check that army is an additional defending army in a siege
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    isSiegeDefAdd = true;
                    thisSiege = this.GetSiege();
                }
            }

            // check to see if attrition applies to defending forces in siege
            // (based on besieged fief bailiff management rating)
            if ((isSiegeDefGarr) || (isSiegeDefAdd))
            {
                attritionApplies = thisSiege.CheckAttritionApplies();
            }

            // get leader
            Character myLeader = this.GetLeader();

            if (attritionApplies)
            {
                // check for attrition due to days remaining
                byte attritionChecks = Convert.ToByte(this.days / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = this.CalcAttrition();
                    // apply attrition
                    uint troopsLost = this.ApplyTroopLosses(attritionModifer);

                    // update siege losses, if applicable
                    if (thisSiege != null)
                    {
                        // siege defenders
                        if ((isSiegeDefGarr) || (isSiegeDefAdd))
                        {
                            thisSiege.totalCasualtiesDefender += Convert.ToInt32(troopsLost);
                        }

                        // siege attackers
                        else
                        {
                            thisSiege.totalCasualtiesAttacker += Convert.ToInt32(troopsLost);
                        }
                    }
                }
            }

            if (!((isSiegeDefGarr) || (isSiegeDefAdd)))
            {
                // check if army dissolves (less than 100 men)
                // NOTE: siege defenders do not dissolve in this way
                if (this.CalcArmySize() < 100)
                {
                    hasDissolved = true;
                }
            }

            // update army days
            if (!hasDissolved)
            {
                if (myLeader != null)
                {
                    this.days = myLeader.days;
                }
                else
                {
                    this.days = 90;
                }
            }

            // reset isMaintained
            this.isMaintained = false;

            return hasDissolved;
        }

        /// <summary>
        /// Runs conditional checks prior to the army launching an attack on another army
        /// </summary>
        /// <returns>bool indicating whether attack can proceed</returns>
        /// <param name="targetArmy">The army to be attacked</param>
        public bool ChecksBeforeAttack(Army targetArmy)
        {
            bool proceed = true;
            string toDisplay = "";

            // check has enough days to give battle (1)
            if (this.days < 1)
            {
                toDisplay = "error:army:Your army doesn't have enough days left to give battle.";
                Globals_Game.UpdatePlayer(owner, toDisplay);
                proceed = false;
            }
            else
            {
                // SIEGE INVOLVEMENT (DEFENDER)
                // check if defending army is the garrison in a siege
                string siegeID = targetArmy.CheckIfSiegeDefenderGarrison();
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    Globals_Game.UpdatePlayer(owner, "error:army:The defending army is currently being besieged and cannot be attacked.");

                }

                else
                {
                    // check if defending army is the additional defender in a siege
                    siegeID = targetArmy.CheckIfSiegeDefenderAdditional();
                    if (!String.IsNullOrWhiteSpace(siegeID))
                    {
                        toDisplay = "error:army:The defending army is currently being besieged and cannot be attacked.";
                        Globals_Game.UpdatePlayer(owner, toDisplay);
                        proceed = false;
                    }

                    else
                    {
                        // check if are attacking your own army
                        if (this.GetOwner() == targetArmy.GetOwner())
                        {
                            toDisplay = "error:army:You cannot attack one of your own armies.";
                            Globals_Game.UpdatePlayer(owner, toDisplay);
                            proceed = false;
                        }

                        else
                        {
                            // check if attacking king's army
                            if (this.GetOwner() == this.GetOwner().GetKing())
                            {
                                // display warning and get decision
                                DialogResult dialogResult = MessageBox.Show("You are about to attack an army owned by your king.\r\nClick 'OK' to proceed.", "Proceed with attack?", MessageBoxButtons.OKCancel);

                                // if choose to cancel
                                if (dialogResult == DialogResult.Cancel)
                                {
                                    toDisplay = "error:army:Attack cancelled.";
                                    Globals_Game.UpdatePlayer(owner, toDisplay);
                                    proceed = false;
                                }
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // SIEGE INVOLVEMENT (BESIEGER)
                // check if attacking army is besieging a keep
                string siegeID = this.CheckIfBesieger();
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    // display warning and get decision
                    DialogResult dialogResult = MessageBox.Show("Your army is besieging a keep and this action would end the siege.\r\nClick 'OK' to proceed.", "Proceed with attack?", MessageBoxButtons.OKCancel);

                    // if choose to cancel
                    if (dialogResult == DialogResult.Cancel)
                    {
                        toDisplay = "error:army:Attack cancelled.";
                        Globals_Game.UpdatePlayer(owner, toDisplay);
                        proceed = false;
                    }

                    // if choose to proceed
                    else
                    {
                        Siege thisSiege = null;
                        thisSiege = Globals_Game.siegeMasterList[siegeID];

                        // construct event description to be passed into siegeEnd
                        string siegeDescription = "On this day of Our Lord the forces of ";
                        siegeDescription += thisSiege.GetBesiegingPlayer().firstName + " " + thisSiege.GetBesiegingPlayer().familyName;
                        siegeDescription += " have chosen to abandon the siege of " + thisSiege.GetFief().name;
                        siegeDescription += ". " + thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;
                        siegeDescription += " retains ownership of the fief.";

                        thisSiege.SiegeEnd(false, siegeDescription);
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Disbands the army
        /// </summary>
        public void DisbandArmy()
        {
            // check for siege involvement
            Siege thisSiege = this.GetSiege();

            // remove from siege
            if (thisSiege != null)
            {
                // check if are additional defending army
                string whichRole = this.CheckIfSiegeDefenderAdditional();
                if (!String.IsNullOrWhiteSpace(whichRole))
                {
                    thisSiege.defenderAdditional = null;
                }

                // check if are besieging army
                else
                {
                    whichRole = this.CheckIfBesieger();
                    if (!String.IsNullOrWhiteSpace(whichRole))
                    {
                        // end siege
                        thisSiege.SiegeEnd(false);
                    }
                }
            }

            // remove from fief
            Fief thisFief = this.GetLocation();
            thisFief.armies.Remove(this.armyID);

            // remove from owner
            PlayerCharacter thisOwner = this.GetOwner();
            if (thisOwner != null)
            {
                thisOwner.myArmies.Remove(this);
            }

            // remove from leader
            Character thisLeader = this.GetLeader();
            if (thisLeader != null)
            {
                thisLeader.armyID = null;
            }

            // remove from armyMasterList
            Globals_Game.armyMasterList.Remove(this.armyID);
        }

        /// <summary>
        /// Retrieves information for Army display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="a">Army for which information is to be displayed</param>
        /// <param name="observer">Character observing the army</param>
        public string DisplayArmyData(Character observer)
        {
            bool isMyArmy = false;
            string armyText = "";
            uint[] troopNumbers = this.troops;
            uint totalTroops = 0;
            Fief armyLocation = this.GetLocation();

            // get owner
            PlayerCharacter thisOwner = this.GetOwner();

            // check if observer is owner (will effect display of details)
            if (thisOwner == observer)
            {
                isMyArmy = true;
            }

            // ID
            armyText += "ID: " + this.armyID + "\r\n\r\n";

            // nationality
            armyText += "Nationality: " + this.GetOwner().nationality.name + "\r\n\r\n";

            // owner
            armyText += "Owner: " + thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")\r\n\r\n";

            if (isMyArmy)
            {
                // days left
                armyText += "Days left: " + this.days + "\r\n\r\n";
            }

            // location
            armyText += "Location: " + armyLocation.name + " (Province: " + armyLocation.province.name + ".  Kingdom: " + armyLocation.province.kingdom.name + ")\r\n\r\n";

            // check if is garrison in a siege
            string siegeID = this.CheckIfSiegeDefenderGarrison();
            if (String.IsNullOrWhiteSpace(siegeID))
            {
                // check if is additional defender in a siege
                siegeID = this.CheckIfSiegeDefenderAdditional();
            }

            // if is defender in a siege, indicate
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                armyText += "NOTE: This army is currently UNDER SIEGE\r\n\r\n";
            }

            else
            {
                // check if is besieger in a siege
                siegeID = this.CheckIfBesieger();

                // if is besieger in a siege, indicate
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    armyText += "NOTE: This army is currently BESIEGING THIS FIEF\r\n\r\n";
                }

                // check if is siege in fief (but army not involved)
                else
                {
                    if (!String.IsNullOrWhiteSpace(armyLocation.siege))
                    {
                        armyText += "NOTE: This fief is currently UNDER SIEGE\r\n\r\n";
                    }
                }
            }

            // leader
            Character armyLeader = this.GetLeader();

            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!";

                if (isMyArmy)
                {
                    armyText += "  You should appoint one as soon as possible.";
                }
            }
            else
            {
                armyText += armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")";
            }
            armyText += "\r\n\r\n";

            // get troop numbers
            if (isMyArmy)
            {
                // actual troop numbers if is player's army
                troopNumbers = this.troops;
            }
            else
            {
                // estimated troop numbers if is NOT player's army
                troopNumbers = this.GetTroopsEstimate(observer);
            }

            armyText += "Troop numbers";
            if (!isMyArmy)
            {
                armyText += " (ESTIMATE)";
            }
            armyText += ":\r\n";

            // labels for troop types
            string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Longbowmen: ", " - Crossbowmen: ", " - Foot: ", " - Rabble: " };

            // display numbers for each troop type
            for (int i = 0; i < troopNumbers.Length; i++)
            {
                armyText += troopTypeLabels[i] + troopNumbers[i];
                totalTroops += troopNumbers[i];
                armyText += "\r\n";
            }

            // display total
            armyText += "   ==================\r\n";
            armyText += " - TOTAL: " + totalTroops + "\r\n\r\n";

            if (isMyArmy)
            {
                // whether is maintained (and at what cost)
                if (this.isMaintained)
                {
                    uint armyCost = this.CalcArmySize() * 500;

                    armyText += "This army is currently being maintained (at a cost of £" + armyCost + ")\r\n\r\n";
                }
                else
                {
                    armyText += "This army is NOT currently being maintained\r\n\r\n";
                }

                // aggression level
                armyText += "Aggression level: " + this.aggression + "\r\n\r\n";

                // sally value
                armyText += "Sally value: " + this.combatOdds + "\r\n\r\n";
            }

            return armyText;
        }

        /// <summary>
        /// Calculates battle values of both armies participating in a battle or siege
        /// </summary>
        /// <remarks>This method should be called by the attacking army</remarks>
        /// <returns>uint[] containing battle values of attacking & defending armies</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        /// <param name="isSiege">bool indicating if the circumstance is a siege storm</param>
        public uint[] CalculateBattleValues(Army defender, int keepLvl = 0, bool isSiegeStorm = false)
        {
            uint[] battleValues = new uint[2];
            double attackerLV = 0;
            double defenderLV = 0;

            // get leaders
            Character attackerLeader = this.GetLeader();
            Character defenderLeader = defender.GetLeader();

            // get leadership values for each army leader
            attackerLV = attackerLeader.GetLeadershipValue(isSiegeStorm);

            // defender may not have leader
            if (defenderLeader != null)
            {
                defenderLV = defenderLeader.GetLeadershipValue(isSiegeStorm);
            }
            else
            {
                defenderLV = 4;
            }

            // calculate battle modifier based on LVs
            // determine highest/lowest of 2 LVs
            double maxLV = Math.Max(attackerLV, defenderLV);
            double minLV = Math.Min(attackerLV, defenderLV);
            double battleModifier = maxLV / minLV;

            // get base combat value for each army
            uint attackerCV = Convert.ToUInt32(this.CalculateCombatValue());
            uint defenderCV = Convert.ToUInt32(defender.CalculateCombatValue(keepLvl));

            // apply battle modifer to the army CV corresponding to the highest LV
            if (attackerLV == maxLV)
            {
                attackerCV = Convert.ToUInt32(attackerCV * battleModifier);
            }
            else
            {
                defenderCV = Convert.ToUInt32(defenderCV * battleModifier);
            }

            battleValues[0] = attackerCV;
            battleValues[1] = defenderCV;

            return battleValues;
        }

        /// <summary>
        /// Process the retreat of the army
        /// </summary>
        /// <param name="retreatDistance">The retreat distance</param>
        public void ProcessRetreat(int retreatDistance)
        {
            // get starting fief
            Fief startingFief = this.GetLocation();

            // get army leader
            Character thisLeader = this.GetLeader();

            // get army owner
            PlayerCharacter thisOwner = this.GetOwner();

            // for each hex in retreatDistance, process retreat
            for (int i = 0; i < retreatDistance; i++)
            {
                // get current location
                Fief from = this.GetLocation();

                // get fief to retreat to
                Fief target = Globals_Game.gameMap.chooseRandomHex(from, true, thisOwner, startingFief);

                if (target != null)
                {
                    // get travel cost
                    double travelCost = from.getTravelCost(target);

                    // check for army leader (defender may not have had one)
                    if (thisLeader != null)
                    {
                        // ensure leader has enough days (retreats are immune to running out of days)
                        if (thisLeader.days < travelCost)
                        {
                            thisLeader.AdjustDays(thisLeader.days - travelCost);
                        }

                        // perform retreat
                        bool success = thisLeader.MoveCharacter(target, travelCost, false);
                    }

                    // if no leader
                    else
                    {
                        // ensure army has enough days (retreats are immune to running out of days)
                        if (this.days < travelCost)
                        {
                            this.days = travelCost;
                        }

                        // perform retreat
                        this.MoveWithoutLeader(target, travelCost);
                    }
                }

            }

        }

        /// <summary>
        /// Processes the addition of one or more detachments to the army
        /// </summary>
        /// <param name="detachments">The detachments to add</param>
        /// <param name="c">Client performing operation</param>
        public void ProcessPickups(ListView.CheckedListViewItemCollection detachments)
        {
            bool proceed = true;
            bool adjustDays = true;
            double daysTaken = 0;
            double minDays = 0;
            bool displayNotAllMsg = false;
            uint[] totTroopsToAdd = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            string toDisplay = "";

            // set minDays to thisArmy.days (as default value)
            minDays = this.days;

            // get leader and owner
            Character myLeader = this.GetLeader();
            PlayerCharacter myOwner = this.GetOwner();

            // check have minimum days necessary for transfer
            if (this.days < 10)
            {
                toDisplay = "error:army:You don't have enough days left for this transfer.";
                Globals_Game.UpdatePlayer(owner, toDisplay);
                proceed = false;
                adjustDays = false;
            }
            else
            {
                // calculate time taken for transfer
                daysTaken = Globals_Game.myRand.Next(10, 31);

                // check if have enough days for transfer in this instance
                if (daysTaken > this.days)
                {
                    daysTaken = this.days;
                    toDisplay = "error:army:Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.";
                    Globals_Game.UpdatePlayer(owner, toDisplay);
                    proceed = false;
                }
                else
                {
                    // make sure collecting army is owned by recipient or donator
                    foreach (ListViewItem item in detachments)
                    {
                        // get donating player
                        PlayerCharacter pcFrom = null;
                        if (Globals_Game.pcMasterList.ContainsKey(item.SubItems[9].Text))
                        {
                            pcFrom = Globals_Game.pcMasterList[item.SubItems[9].Text];
                        }

                        // get donating player
                        PlayerCharacter pcFor = null;
                        if (Globals_Game.pcMasterList.ContainsKey(item.SubItems[10].Text))
                        {
                            pcFor = Globals_Game.pcMasterList[item.SubItems[10].Text];
                        }

                        // check for appropriate collecting player
                        if ((myOwner != pcFrom) && (myOwner != pcFor))
                        {
                            toDisplay = "error:army:You are not permitted to collect at least one of the selected detachments.";
                            Globals_Game.UpdatePlayer(owner, toDisplay);
                            proceed = false;
                            adjustDays = false;
                        }
                    }
                }
            }

            if (proceed)
            {
                // get fief
                Fief thisFief = this.GetLocation();

                // check for minimum days
                foreach (ListViewItem item in detachments)
                {
                    double thisDays = Convert.ToDouble(item.SubItems[8].Text);

                    // check if detachment has enough days for transfer in this instance
                    // if not, flag display of message at end of process, but do nothing else
                    if (thisDays < daysTaken)
                    {
                        displayNotAllMsg = true;
                        toDisplay = "At least one detachment could not be added due to poor organisation.";
                    }
                    else
                    {
                        if (thisDays < minDays)
                        {
                            minDays = thisDays;
                        }
                    }
                }

                foreach (ListViewItem item in detachments)
                {
                    double thisDays = Convert.ToDouble(item.SubItems[8].Text);

                    // get numbers of each type to add
                    uint[] thisTroops = new uint[] { Convert.ToUInt32(item.SubItems[1].Text), Convert.ToUInt32(item.SubItems[2].Text),
                            Convert.ToUInt32(item.SubItems[3].Text), Convert.ToUInt32(item.SubItems[4].Text),
                            Convert.ToUInt32(item.SubItems[5].Text), Convert.ToUInt32(item.SubItems[6].Text),
                            Convert.ToUInt32(item.SubItems[7].Text) };

                    // if does have enough days, proceed
                    if (thisDays >= daysTaken)
                    {
                        // create temporary army to check attrition
                        Army tempArmy = new Army(Globals_Game.GetNextArmyID(), null, item.SubItems[9].Text,
                            thisDays, this.location, trp: thisTroops);

                        // compare days to minDays, apply attrition if necessary
                        // then add to troopsToAdd
                        if (thisDays > minDays)
                        {
                            // check for attrition (to bring it down to minDays)
                            byte attritionChecks = 0;
                            attritionChecks = Convert.ToByte((thisDays - minDays) / 7);
                            double attritionModifier = 0;

                            for (int i = 0; i < attritionChecks; i++)
                            {
                                attritionModifier = tempArmy.CalcAttrition();

                                // apply attrition
                                if (attritionModifier > 0)
                                {
                                    tempArmy.ApplyTroopLosses(attritionModifier);
                                }
                            }
                        }

                        for (int i = 0; i < totTroopsToAdd.Length; i++)
                        {
                            totTroopsToAdd[i] += tempArmy.troops[i];
                        }

                        // remove detachment from fief
                        thisFief.troopTransfers.Remove(item.SubItems[0].Text);

                        // nullify tempArmy
                        tempArmy = null;
                    }

                }

            }

            if (adjustDays)
            {
                if (this.days == minDays)
                {
                    // add troops to army (this could be 0)
                    for (int i = 0; i < this.troops.Length; i++)
                    {
                        this.troops[i] += totTroopsToAdd[i];
                    }

                    // adjust days
                    myLeader.AdjustDays(daysTaken);

                    // calculate attrition for army
                    byte attritionChecks = Convert.ToByte(daysTaken / 7);
                    double attritionModifier = 0;

                    for (int i = 0; i < attritionChecks; i++)
                    {
                        attritionModifier = this.CalcAttrition();
                        if (attritionModifier > 0)
                        {
                            this.ApplyTroopLosses(attritionModifier);
                        }
                    }
                }
                else
                {
                    // any days army has had to 'wait' should go towards days taken
                    // for the transfer (daysTaken)
                    double differenceToMin = (this.days - minDays);
                    if (differenceToMin >= daysTaken)
                    {
                        daysTaken = 0;
                    }
                    else
                    {
                        daysTaken = daysTaken - differenceToMin;
                    }

                    // adjust days
                    myLeader.AdjustDays(differenceToMin);

                    // calculate attrition for army (to bring it down to minDays)
                    byte attritionChecks = Convert.ToByte(differenceToMin / 7);
                    double attritionModifier = 0;

                    for (int i = 0; i < attritionChecks; i++)
                    {
                        attritionModifier = this.CalcAttrition();
                        if (attritionModifier > 0)
                        {
                            this.ApplyTroopLosses(attritionModifier);
                        }
                    }

                    // add troops to army
                    for (int i = 0; i < this.troops.Length; i++)
                    {
                        this.troops[i] += totTroopsToAdd[i];
                    }

                    // check if are any remaining days taken for the transfer (daysTaken) 
                    if (daysTaken > 0)
                    {
                        // adjust days
                        myLeader.AdjustDays(daysTaken);

                        // calculate attrition for army for days taken for transfer
                        attritionChecks = Convert.ToByte(daysTaken / 7);

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = this.CalcAttrition();
                            if (attritionModifier > 0)
                            {
                                this.ApplyTroopLosses(attritionModifier);
                            }
                        }
                    }
                }

                // if not all selected detachments could be picked up (not enough days), show message
                if (displayNotAllMsg)
                {
                    Globals_Game.UpdatePlayer(owner, toDisplay);
                }
            }
        }
  
     /*   public void Serialize()
        {
            Serializer.Serialize(File.Create("myTest.txt"), this);
        }*/
    }
}
