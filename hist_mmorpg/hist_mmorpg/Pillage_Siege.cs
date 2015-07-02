﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    public static class Pillage_Siege
    {
        /// <summary>
        /// Calculates the outcome of the pillage of a fief by an army
        /// </summary>
        /// <param name="f">The fief being pillaged</param>
        /// <param name="a">The pillaging army</param>
        /// <param name="circumstance">The circumstance under which the fief is being pillaged</param>
        public static void ProcessPillage(Fief f, Army a, string circumstance = "pillage")
        {
            ProtoPillageResult pillageResult = new ProtoPillageResult();
            double thisLoss = 0;
            double moneyPillagedTotal = 0;
            double moneyPillagedOwner = 0;
            double pillageMultiplier = 0;
            // get army leader
            Character armyLeader = a.GetLeader();

            // get pillaging army owner (receives a proportion of total spoils)
            PlayerCharacter armyOwner = a.GetOwner();

            pillageResult.fiefID = f.id;
            // get garrison leader (to add to journal entry)
            Character defenderLeader = null;
            if (f.bailiff != null)
            {
                defenderLeader = f.bailiff;
            }

            // calculate pillageMultiplier (based on no. pillagers per 1000 population)
            pillageMultiplier = a.CalcArmySize() / (f.population / 1000);

            // calculate days taken for pillage
            double daysTaken = Globals_Game.myRand.Next(7, 16);
            if (daysTaken > a.days)
            {
                daysTaken = a.days;
            }

            // update army days
            armyLeader.AdjustDays(daysTaken);
            pillageResult.daysTaken = daysTaken;

            // % population loss
            thisLoss = (0.007 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply population loss
            pillageResult.populationLoss = Convert.ToInt32((f.population * (thisLoss / 100)));
            f.population -= Convert.ToInt32((f.population * (thisLoss / 100)));

            // % treasury loss
            if (!circumstance.Equals("quellRebellion"))
            {
                thisLoss = (0.2 * pillageMultiplier);
                // ensure is between 1%-80%
                if (thisLoss < 1)
                {
                    thisLoss = 1;
                }
                else if (thisLoss > 80)
                {
                    thisLoss = 80;
                }
                // apply treasury loss
                if (f.treasury > 0)
                {
                    pillageResult.treasuryLoss = Convert.ToInt32((f.treasury * (thisLoss / 100)));
                    f.treasury -= Convert.ToInt32((f.treasury * (thisLoss / 100)));
                }
            }

            // % loyalty loss
            thisLoss = (0.33 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply loyalty loss
            pillageResult.loyaltyLoss = (f.loyalty * (thisLoss / 100));

            f.loyalty -= (f.loyalty * (thisLoss / 100));

            // % fields loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply fields loss
            pillageResult.fieldsLoss = (f.fields * (thisLoss / 100));
            f.fields -= (f.fields * (thisLoss / 100));

            // % industry loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply industry loss
            pillageResult.industryLoss = (f.industry * (thisLoss / Convert.ToDouble(100)));
            f.industry -= (f.industry * (thisLoss / 100));

            // money pillaged (based on GDP)
            thisLoss = (0.032 * pillageMultiplier);
            // ensure is between 1%-50%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 50)
            {
                thisLoss = 50;
            }
            // calculate base amount pillaged based on fief GDP
            double baseMoneyPillaged = (f.keyStatsCurrent[1] * (thisLoss / 100));
            moneyPillagedTotal = baseMoneyPillaged;
            pillageResult.baseMoneyPillaged = baseMoneyPillaged;

            // factor in no. days spent pillaging (get extra 5% per day > 7)
            int daysOver7 = Convert.ToInt32(daysTaken) - 7;
            if (daysOver7 > 0)
            {
                for (int i = 0; i < daysOver7; i++)
                {
                    moneyPillagedTotal += (baseMoneyPillaged * 0.05);
                }
                pillageResult.bonusMoneyPillaged = moneyPillagedTotal - baseMoneyPillaged;
                pillageResult.daysTaken = daysOver7;
            }

            // check for jackpot
            // generate randomPercentage to see if hit the jackpot
            int myRandomPercent = Globals_Game.myRand.Next(101);
            if (myRandomPercent <= 30)
            {
                // generate random int to multiply amount pillaged
                int myRandomMultiplier = Globals_Game.myRand.Next(3, 11);
                pillageResult.jackpot = moneyPillagedTotal * myRandomMultiplier - moneyPillagedTotal;
                moneyPillagedTotal = moneyPillagedTotal * myRandomMultiplier;
            }

            // check proportion of money pillaged goes to army owner (based on stature)
            double proportionForOwner = 0.05 * armyOwner.CalculateStature();
            moneyPillagedOwner = (moneyPillagedTotal * proportionForOwner);
            pillageResult.moneyPillagedOwner = moneyPillagedOwner;

            // apply to army owner's home fief treasury
            armyOwner.GetHomeFief().treasury += Convert.ToInt32(moneyPillagedOwner);

            // apply loss of stature to army owner if fief has same language
            if (armyOwner.language.id == f.language.id)
            {
                armyOwner.AdjustStatureModifier(-0.3);
                pillageResult.statureModifier = (-0.3);
            }
            else if (armyOwner.language.baseLanguage.id == f.language.baseLanguage.id)
            {
                armyOwner.AdjustStatureModifier(-0.2);
                pillageResult.statureModifier = (-0.2);
            }

            // set isPillaged for fief
            f.isPillaged = true;

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(f.owner.charID + "|fiefOwner");
            tempPersonae.Add(armyOwner.charID + "|attackerOwner");
            if (armyLeader != null)
            {
                tempPersonae.Add(armyLeader.charID + "|attackerLeader");
            }
            if ((defenderLeader != null) && (!circumstance.Equals("quellRebellion")))
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderLeader");
            }
            if (circumstance.Equals("quellRebellion"))
            {
                tempPersonae.Add("all|all");
            }
            string[] pillagePersonae = tempPersonae.ToArray();

            // location
            string pillageLocation = f.id;

            // type
            string type = "";
            if (circumstance.Equals("pillage"))
            {
                type += "pillage";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                type += "rebellionQuelled";
            }

            if (circumstance.Equals("pillage"))
            {
                pillageResult.isPillage = true;
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageResult.isPillage = false;
            }
            pillageResult.fiefName = f.name;
            pillageResult.fiefOwner=f.owner.firstName + " " + f.owner.familyName;

            if ((circumstance.Equals("pillage")) && (defenderLeader != null))
            {
                if (f.owner != defenderLeader)
                {
                    pillageResult.defenderLeader=defenderLeader.firstName + " " + defenderLeader.familyName;
                }
            }
            
            pillageResult.armyOwner = armyOwner.firstName + " " + armyOwner.familyName;
            if (armyLeader != null)
            {
                if (armyOwner != armyLeader)
                {
                    pillageResult.armyLeader = armyLeader.firstName + " " + armyLeader.familyName;
                }
            }

            // put together new journal entry
            JournalEntry pillageEntry = new JournalEntry(pillageResult, entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, pillagePersonae, type, loc: pillageLocation);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(pillageEntry);

            //TODO message handling
            Globals_Game.UpdatePlayer(armyOwner.playerID,pillageResult);
            Globals_Game.UpdatePlayer(f.owner.playerID, pillageResult);
        }

        /// <summary>
        /// Implements the processes involved in the pillage of a fief by an army
        /// </summary>
        /// <param name="a">The pillaging army</param>
        /// <param name="f">The fief being pillaged</param>
        public static void PillageFief(Army a, Fief f)
        {
            bool pillageCancelled = false;
            bool bailiffPresent = false;
            Army fiefArmy = null;

            // check if bailiff present in fief (he'll lead the army)
            if (f.bailiff != null)
            {
                for (int i = 0; i < f.charactersInFief.Count; i++)
                {
                    if (f.charactersInFief[i] == f.bailiff)
                    {
                        bailiffPresent = true;
                        break;
                    }
                }
            }

            // if bailiff is present, create an army and attempt to give battle
            // no bailiff = no leader = pillage is unopposed by defending forces
            if (bailiffPresent)
            {
                // create temporary army for battle
                fiefArmy = f.CreateDefendingArmy();

                // give battle and get result
                pillageCancelled = Battle.GiveBattle(fiefArmy, a, circumstance: "pillage");

                if (pillageCancelled)
                {
                    string toDisplay = "The pillaging force has been forced to retreat by the fief's defenders!";
                    Globals_Game.UpdatePlayer(f.owner.playerID, DisplayMessages.PillageRetreat);
                    Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageRetreat);
                }

                else
                {
                    // check still have enough days left
                    if (a.days < 7)
                    {
                        Globals_Game.UpdatePlayer(f.owner.playerID, DisplayMessages.PillageDays);
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageDays);
                        
                        pillageCancelled = true;
                    }
                }

            }

            if (!pillageCancelled)
            {
                // process pillage
                Pillage_Siege.ProcessPillage(f, a);
            }

        }


        /// <summary>
        /// Implements conditional checks prior to the pillage or siege of a fief
        /// </summary>
        /// <returns>bool indicating whether pillage/siege can proceed</returns>
        /// <param name="f">The fief being pillaged/besieged</param>
        /// <param name="a">The pillaging/besieging army</param>
        /// <param name="circumstance">The circumstance - pillage or siege</param>
        public static bool ChecksBeforePillageSiege(Army a, Fief f, string circumstance = "pillage")
        {
            bool proceed = true;
            string operation = "";

            // check if is your own fief
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if (f.owner == a.GetOwner())
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageOwnFief,new string[] {"pillage"});
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageOwnFief,new string[] {"siege"});
                    }
                }
            }

            // check if fief is under siege
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if ((!String.IsNullOrWhiteSpace(f.siege)) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageUnderSiege);
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageSiegeAlready);
                    }
                }
            }

            // check if fief already pillaged
            // note: not necessary for quell rebellion (get a 'free' pillage)
            if (!circumstance.Equals("quellRebellion"))
            {
                if (circumstance.Equals("pillage"))
                {
                    // check isPillaged = false
                    if ((f.isPillaged) && (proceed))
                    {
                        proceed = false;
                        Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageAlready);
                    }
                }
            }

            // check if your army has a leader
            if (a.GetLeader() == null)
            {
                proceed = false;

                if (circumstance.Equals("quellRebellion"))
                {
                    operation = "Operation";
                }
                if (circumstance.Equals("pillage"))
                {
                    operation = "Pillage";
                }
                else if (circumstance.Equals("siege"))
                {
                    operation = "Siege";
                }
                Globals_Game.UpdatePlayer(a.owner, DisplayMessages.ArmyNoLeader);
            }

            // check has min days required
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("quellRebellion")))
            {
                // pillage = min 7
                if ((a.days < 7) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("quellRebellion"))
                    {
                        operation = "Quell rebellion";
                    }
                    else
                    {
                        operation = "Pillage";
                    }
                    Globals_Game.UpdatePlayer(a.owner, DisplayMessages.ErrorGenericNotEnoughDays);
                }
            }
            else if (circumstance.Equals("siege"))
            {
                // siege = 1 (to set up siege)
                if ((a.days < 1) && (proceed))
                {
                    proceed = false;
                    Globals_Game.UpdatePlayer(a.owner, DisplayMessages.ErrorGenericNotEnoughDays);
                }
            }

            // check for presence of armies belonging to fief owner
            if (proceed)
            {
                // iterate through armies in fief
                for (int i = 0; i < f.armies.Count; i++)
                {
                    // get army
                    Army armyInFief = Globals_Game.armyMasterList[f.armies[i]];

                    // check if owned by fief owner
                    if (armyInFief.owner.Equals(f.owner.charID))
                    {
                        // army must be outside keep
                        if (!armyInFief.GetLeader().inKeep)
                        {
                            // army must have correct aggression settings
                            if (armyInFief.aggression > 1)
                            {
                                proceed = false;
                                if (circumstance.Equals("pillage"))
                                {
                                    operation = "Pillage";
                                }
                                else if (circumstance.Equals("siege"))
                                {
                                    operation = "Siege";
                                }
                                else if (circumstance.Equals("quellRebellion"))
                                {
                                    operation = "Quell rebellion";
                                }
                                Globals_Game.UpdatePlayer(a.owner, DisplayMessages.PillageArmyDefeat,new string[]{ armyInFief.armyID });

                                break;
                            }
                        }
                    }
                }

                // check if fief in rebellion
                if ((circumstance.Equals("siege")) && (proceed))
                {
                    if (f.status.Equals('R'))
                    {
                        proceed = false;
                        Globals_Game.UpdatePlayer(a.owner,DisplayMessages.PillageSiegeRebellion);
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Allows an attacking army to lay siege to an enemy fief
        /// </summary>
        /// <param name="attacker">The attacking army</param>
        /// <param name="target">The fief to be besieged</param>
        public static Siege SiegeStart(Army attacker, Fief target)
        {
            Army defenderGarrison = null;
            Army defenderAdditional = null;

            // check for existence of army in keep
            for (int i = 0; i < target.armies.Count; i++)
            {
                // get army
                Army armyInFief = Globals_Game.armyMasterList[target.armies[i]];

                // check is in keep
                Character armyLeader = armyInFief.GetLeader();
                if (armyLeader != null)
                {
                    if (armyLeader.inKeep)
                    {
                        // check owner is same as that of fief (i.e. can help in siege)
                        if (armyInFief.GetOwner() == target.owner)
                        {
                            defenderAdditional = armyInFief;
                            break;
                        }
                    }
                }
            }

            // create defending force
            defenderGarrison = target.CreateDefendingArmy();

            // get the minumum days of all army objects involved
            double minDays = Math.Min(attacker.days, defenderGarrison.days);
            if (defenderAdditional != null)
            {
                minDays = Math.Min(minDays, defenderAdditional.days);
            }

            // get defenderAdditional ID, or null if no defenderAdditional
            string defAddID = null;
            if (defenderAdditional != null)
            {
                defAddID = defenderAdditional.armyID;
            }

            // create siege object
            Siege mySiege = new Siege(Globals_Game.GetNextSiegeID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, attacker.GetOwner().charID, target.owner.charID, attacker.armyID, defenderGarrison.armyID, target.id, minDays, target.keepLevel, defAdd: defAddID);

            // add to master list
            Globals_Game.siegeMasterList.Add(mySiege.siegeID, mySiege);

            // add to siege owners
            mySiege.GetBesiegingPlayer().mySieges.Add(mySiege.siegeID);
            mySiege.GetDefendingPlayer().mySieges.Add(mySiege.siegeID);

            // add to fief
            target.siege = mySiege.siegeID;

            // reduce expenditures in fief, except for garrison
            target.infrastructureSpendNext = 0;
            target.keepSpendNext = 0;
            target.officialsSpendNext = 0;

            // update days (NOTE: siege.days will be updated in syncDays)
            mySiege.totalDays++;

            // sychronise days
            mySiege.SyncSiegeDays(mySiege.days - 1);

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(mySiege.GetDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(mySiege.GetBesiegingPlayer().charID + "|attackerOwner");
            tempPersonae.Add(attacker.GetLeader().charID + "|attackerLeader");
            // get defenderLeader
            Character defenderLeader = defenderGarrison.GetLeader();
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
            string siegeLocation = mySiege.GetFief().id;

            // description
            string[] fields = new string[6];
            fields[0]= mySiege.GetBesiegingPlayer().firstName + " " + mySiege.GetBesiegingPlayer().familyName;
            fields[1] =  attacker.GetLeader().firstName + " " + attacker.GetLeader().familyName;
            fields[2]=  mySiege.GetFief().name;
            fields[3]= mySiege.GetDefendingPlayer().firstName + " " + mySiege.GetDefendingPlayer().familyName;
            fields[4]=fields[5]="";
            if (defenderLeader != null)
            {
                fields[4] = "The defending garrison is led by " + defenderLeader.firstName + " " + defenderLeader.familyName+".";
            }
            if (addDefendLeader != null)
            {
                fields[5] = "Additional defending forces are led by " + addDefendLeader.firstName + " " + addDefendLeader.familyName+".";
            }

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siege",fields, loc: siegeLocation, messageIdentifier: DisplayMessages.PillageInitiateSiege);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(siegeResult);

            return mySiege;
        }
    }
}
