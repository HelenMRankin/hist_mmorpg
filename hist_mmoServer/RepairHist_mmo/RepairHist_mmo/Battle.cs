using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
namespace hist_mmorpg
{
    [ContractVerification(true)] 
    public static class Battle
    {
        /// <summary>
        /// Calculates whether the attacking army is able to successfully bring the defending army to battle
        /// </summary>
        /// <returns>bool indicating whether battle has commenced</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        private static bool BringToBattle(uint attackerValue, uint defenderValue, string circumstance = "battle")
        {
            bool battleHasCommenced = false;
            double[] combatOdds = Globals_Server.battleProbabilities["odds"];
            double[] battleChances = Globals_Server.battleProbabilities[circumstance];
            double thisChance = 0;

            for (int i = 0; i < combatOdds.Length; i++)
            {
                if (i < combatOdds.Length - 1)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    if (attackerValue / defenderValue < combatOdds[i])
                    {
                        thisChance = battleChances[i];
                        break;
                    }
                }
                else
                {
                    thisChance = battleChances[i];
                    break;
                }
            }

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to battleChance
            if (randomPercentage <= thisChance)
            {
                battleHasCommenced = true;
            }
            return battleHasCommenced;
        }

        /// <summary>
        /// Determines whether the attacking army is victorious in a battle
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public static bool DecideBattleVictory(uint attackerValue, uint defenderValue)
        {
            bool attackerVictorious = false;

            // calculate chance of victory
            double attackerVictoryChance = Battle.CalcVictoryChance(attackerValue, defenderValue);

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to attackerVictoryChance
            if (randomPercentage <= attackerVictoryChance)
            {
                attackerVictorious = true;
            }

            return attackerVictorious;
        }

        /// <summary>
        /// Calculates chance that the attacking army will be victorious in a battle
        /// </summary>
        /// <returns>double containing percentage chance of victory</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public static double CalcVictoryChance(uint attackerValue, uint defenderValue)
        {
            return (attackerValue / (Convert.ToDouble(attackerValue + defenderValue))) * 100;
        }

        /// <summary>
        /// Calculates casualties from a battle for both sides
        /// </summary>
        /// <returns>double[] containing percentage loss modifier for each side</returns>
        /// <param name="attackerTroops">uint containing attacking army troop numbers</param>
        /// <param name="defenderTroops">uint containing defending army troop numbers</param>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="attackerVictorious">bool indicating whether attacking army was victorious</param>
        public static double[] CalculateBattleCasualties(uint attackerTroops, uint defenderTroops, uint attackerValue, uint defenderValue, bool attackerVictorious)
        {
            double[] battleCasualties = new double[2];
            double largeArmyModifier = 0;
            bool largestWon = true;

            // determine highest/lowest battle value
            double maxBV = Math.Max(attackerValue, defenderValue);
            double minBV = Math.Min(attackerValue, defenderValue);

            // use BVs to determine high mark for base casualty rate of army with smallest battle value (see below)
            double highCasualtyRate = maxBV / (maxBV + minBV);

            // determine base casualty rate for army with smallest battle value
            double smallestModifier = Utility_Methods.GetRandomDouble(highCasualtyRate, min: 0.1);

            // determine if army with largest battle value won
            if (maxBV == attackerValue)
            {
                if (!attackerVictorious)
                {
                    largestWon = false;
                }
            }
            else
            {
                if (attackerVictorious)
                {
                    largestWon = false;
                }
            }

            // if army with largest battle value won
            if (largestWon)
            {
                // calculate casualty modifier for army with largest battle value
                // (based on adapted version of Lanchester's Square Law - i.e. largest army loses less troops than smallest)
                largeArmyModifier = (1 + ((minBV * minBV) / (maxBV * maxBV))) / 2;

                // attacker is large army
                if (attackerVictorious)
                {
                    battleCasualties[1] = smallestModifier;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
                // defender is large army
                else
                {
                    battleCasualties[0] = smallestModifier;
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
            }

            // if army with smallest battle value won
            else
            {
                // calculate casualty modifier for army with largest battle value
                // this ensures its losses will be roughly the same as the smallest army (because it lost)
                largeArmyModifier = Utility_Methods.GetRandomDouble(1.20, min: 0.8);

                // defender is large army
                if (attackerVictorious)
                {
                    // smallest army losses reduced because they won
                    battleCasualties[0] = smallestModifier / 2;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
                // attacker is large army
                else
                {
                    battleCasualties[1] = smallestModifier / 2;
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
            }


            return battleCasualties;
        }

        /// <summary>
        /// Calculates whether either army has retreated due to the outcome of a battle
        /// </summary>
        /// <returns>int[] indicating the retreat distance (fiefs) of each army. First index is attacker, second is defender</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="aCasualties">The attacking army casualty modifier</param>
        /// <param name="dCasualties">The defending army casualty modifier</param>
        /// <param name="attackerVictorious">bool indicating if attacking army was victorious</param>
        public static int[] CheckForRetreat(Army attacker, Army defender, double aCasualties, double dCasualties, bool attackerVictorious)
        {
            Contract.Requires(defender!=null);
            bool[] hasRetreated = { false, false };
            int[] retreatDistance = { 0, 0 };

            // check if loser retreats due to battlefield casualties
            if (!attackerVictorious)
            {
                // if have >= 20% casualties
                if (aCasualties >= 0.2)
                {
                    // indicate attacker has retreated
                    hasRetreated[0] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[0] = Globals_Game.myRand.Next(1, 3);
                }
            }
            else
            {
                // if have >= 20% casualties
                if (dCasualties >= 0.2)
                {
                    // indicate defender has retreated
                    hasRetreated[1] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[1] = Globals_Game.myRand.Next(1, 3);
                }
            }

            // check to see if defender retreats due to aggression setting (i.e. was forced into battle)
            // NOTE: this will only happen if attacker and defender still present in fief
            if ((defender.aggression == 0) && (!hasRetreated[0] && !hasRetreated[1]))
            {
                // indicate defender has retreated
                hasRetreated[1] = true;

                // indicate retreat distance
                retreatDistance[1] = 1;
            }

            return retreatDistance;
        }

        /// <summary>
        /// Calculates rough battle odds between two armies (i.e ratio of attacking army combat
        /// value to defending army combat value).  NOTE: does not involve leadership values
        /// </summary>
        /// <returns>int containing battle odds</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        public static int GetBattleOdds(Army attacker, Army defender)
        {
            Contract.Requires(attacker!=null&&defender!=null);
            double battleOdds = 0;

            battleOdds = Math.Floor(attacker.CalculateCombatValue() / defender.CalculateCombatValue());

            return Convert.ToInt32(battleOdds);
        }

        /// <summary>
        /// Return a string describing the results of a battle
        /// </summary>
        /// <param name="battle">Results of battle</param>
        /// <returns>String description</returns>
        public static String DisplayBattleResults(ProtoBattle battle)
        {
            Contract.Requires(battle!=null);
            // Battle introduction
            string toDisplay = "The fief garrison and militia";
            if (battle.attackerLeader != null)
            {
                toDisplay += ", led by " + battle.attackerLeader + ",";
            }
            switch (battle.circumstance)
            {
                // Normal battle
                case 0:
                {
                    toDisplay+=" moved to attack ";
                }
                    break;
                // Pillage
                case 1:
                {
                    toDisplay += " sallied forth to bring the pillaging army,";
                    
                }
                    break;
                // Siege
                case 2:
                {
                    toDisplay += ", sallied forth to bring the besieging army, ";
                }
                    break;
                default:
                    toDisplay = "Unrecognised circumstance";
                    break;
            }
            if (battle.defenderLeader != null)
            {
                toDisplay += " led by " + battle.defenderLeader + " and";
            }
            toDisplay += " owned by " + battle.defenderOwner
                         + ", to battle in the fief of " + Globals_Game.fiefMasterList[battle.battleLocation].name+"."
                + "\r\n\r\nOutcome: ";
            if (battle.battleTookPlace)
            {
                if (battle.attackerVictorious)
                {
                    toDisplay += battle.attackerOwner;
                }
                else
                {
                    toDisplay += battle.defenderOwner;
                }

                // Victory status
                toDisplay += "'s army was victorious.\r\n\r\n";

                // Casualties
                toDisplay += battle.attackerOwner + "'s army suffered " + battle.attackerCasualties +
                             " troop casualties.\n";
                toDisplay += battle.defenderLeader + "'s army suffered " + battle.defenderCasualties +
                             " troop casualties.\n";

                // Retreats
                foreach (var retreater in battle.retreatedArmies)
                {
                    toDisplay += retreater + "'s army retreated from the fief.\n";
                }

                // Disbands
                foreach (var disbander in battle.disbandedArmies)
                {
                    toDisplay += disbander + "'s army disbanded due to heavy casualties.\n";
                }

                toDisplay += string.Join(", ", battle.deaths) + " all died due to injuries received.\n";
                if (battle.circumstance == 1)
                {
                    toDisplay += "The pillage in " + Globals_Game.fiefMasterList[battle.battleLocation].name +
                                 " has been prevented";
                }

                // Siege results
                if (battle.circumstance == 2)
                {
                    if (battle.attackerVictorious || battle.retreatedArmies.Contains(battle.attackerOwner))
                    {
                        toDisplay += battle.attackerOwner + "'s defenders have defeated the forces of " +
                                     battle.defenderOwner + ", relieving the siege of " +
                                     Globals_Game.fiefMasterList[battle.battleLocation].name
                                     + ". " + battle.defenderOwner +
                                     " retains ownership of the fief. The siege has been raised.\n";

                    }
                    else if(battle.DefenderDeadNoHeir)
                    {
                        // add to message
                        toDisplay += "The siege in " + Globals_Game.fiefMasterList[battle.battleLocation].name + " has been raised";
                        toDisplay += " due to the death of the besieging party, ";
                        toDisplay += battle.siegeBesieger+ ".";
                    }
                }
            }
            else
            {
                if (battle.circumstance > 0)
                {
                    toDisplay += battle.attackerOwner + "'s forces failed to bring their aggressors to battle.\n";
                }
                else
                {
                    toDisplay += battle.defenderOwner +
                                 "'s forces successfully refused battle and retreated from the fief.";
                }
            }
            return toDisplay; 
        }

        /// <summary>
        /// Display the results of a siege (that has been resolved due to a battle) in a human readable format
        /// </summary>
        /// <param name="battle">Results of battle</param>
        /// <returns>String describing battle</returns>
        public static String DisplaySiegeResults(ProtoBattle battle)
        {
            Contract.Requires(battle!=null);
            string siegeDescription = "";
            if (battle.attackerVictorious || battle.retreatedArmies.Contains(battle.attackerOwner))
            {
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += battle.siegeDefender;
                siegeDescription += " have defeated the forces of " + battle.siegeBesieger;
                siegeDescription += ", relieving the siege of " + Globals_Game.fiefMasterList[battle.battleLocation].name + ".";
                siegeDescription += " " + battle.siegeDefender;
                siegeDescription += " retains ownership of the fief.";
            }
            if (battle.DefenderDeadNoHeir)
            {
                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += battle.siegeBesieger;
                siegeDescription += " attacked the forces of " + battle.siegeDefender;
                siegeDescription += ", who was killed during the battle.";
                siegeDescription += "  Thus, despite losing the battle, " + battle.siegeBesieger;
                siegeDescription += " has succeeded in relieving the siege of " + Globals_Game.fiefMasterList[battle.battleLocation].name + ".";
                siegeDescription += " " + battle.siegeDefender;
                siegeDescription += " retains ownership of the fief.";
            }
            return siegeDescription;

        }

        /// <summary>
        /// Implements the processes involved in a battle between two armies in the field
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <remarks>
        /// Predicate: assumes attacker has sufficient days
        /// Predicate: assumes attacker has leader
        /// Predicate: assumes attacker in same fief as defender
        /// Predicate: assumes defender not besieged in keep
        /// Predicate: assumes attacker and defender not same army
        /// </remarks>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        public static bool GiveBattle(Army attacker, Army defender, out ProtoBattle battleResults, string circumstance = "battle")
        {
            Contract.Requires(attacker!=null&&defender!=null&&circumstance!=null);
            battleResults = new ProtoBattle();
            bool attackerVictorious = false;
            bool battleHasCommenced = false;
            bool attackerLeaderDead = false;
            bool defenderLeaderDead = false;
            // check if losing army has disbanded
            bool attackerDisbanded = false;
            bool defenderDisbanded = false;

            bool siegeRaised = false;
            uint[] battleValues = new uint[2];
            double[] casualtyModifiers = new double[2];
            double statureChange = 0;

            // if applicable, get siege
            Siege thisSiege = null;
            string thisSiegeID = defender.CheckIfBesieger();
            if (!String.IsNullOrWhiteSpace(thisSiegeID))
            {
                // get siege
                thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
            }

            // get starting troop numbers
            uint attackerStartTroops = attacker.CalcArmySize();
            uint defenderStartTroops = defender.CalcArmySize();
            uint attackerCasualties = 0;
            uint defenderCasualties = 0;

            // get leaders
            Character attackerLeader = attacker.GetLeader();
            Character defenderLeader = defender.GetLeader();

           // if(attackerLeader!=null) {
                battleResults.attackerLeader = attackerLeader.firstName + " " + attackerLeader.familyName;
          //  }
          //  if(defenderLeader!=null) {
                battleResults.defenderLeader=defenderLeader.firstName+ " " + defenderLeader.familyName;
         //   }
            
            battleResults.attackerOwner = attacker.GetOwner().firstName + "  " + attacker.GetOwner().familyName;
            battleResults.defenderOwner = defender.GetOwner().firstName + " " + defender.GetOwner().familyName;
            battleResults.battleLocation = attacker.GetLocation().id;
            // introductory text for message
            switch (circumstance)
            {
                case "pillage":
                    battleResults.circumstance = 1;
                    break;
                case "siege":
                    battleResults.circumstance = 2;
                    break;
                default:
                    battleResults.circumstance = 0;
                    break;
            }

            // get battle values for both armies
            battleValues = attacker.CalculateBattleValues(defender);

            // check if attacker has managed to bring defender to battle
            // case 1: defending army sallies during siege to attack besieger = battle always occurs
            if (circumstance.Equals("siege"))
            {
                battleHasCommenced = true;
            }
            // case 2: defending militia attacks pillaging army during pollage = battle always occurs
            else if (circumstance.Equals("pillage"))
            {
                battleHasCommenced = true;
            }
            // case 3: defender aggression and combatOdds allows battle
            else if (defender.aggression != 0)
            {
                if (defender.aggression == 1)
                {
                    // get odds
                    int battleOdds = Battle.GetBattleOdds(attacker, defender);

                    // if odds OK, give battle
                    if (battleOdds <= defender.combatOdds)
                    {
                        battleHasCommenced = true;
                    }

                    // if not, check for battle
                    else
                    {
                        battleHasCommenced = Battle.BringToBattle(battleValues[0], battleValues[1], circumstance);

                        if (!battleHasCommenced)
                        {
                            defender.ProcessRetreat(1);
                            
                        }
                    }
                }

                else
                {
                    battleHasCommenced = true;
                }
            }

            // otherwise, check to see if the attacker can bring the defender to battle
            else
            {
                battleHasCommenced = Battle.BringToBattle(battleValues[0], battleValues[1], circumstance);
                if (!battleHasCommenced)
                {
                    defender.ProcessRetreat(1);
                }
            }
            battleResults.battleTookPlace = battleHasCommenced;
            if (battleHasCommenced)
            {
                List<string> disbandedArmies = new List<string>();
                List<string> retreatedArmies = new List<string>();
                List<string> deadCharacters = new List<string>();
                // WHO HAS WON?
                // calculate if attacker has won
                attackerVictorious = Battle.DecideBattleVictory(battleValues[0], battleValues[1]);

                // UPDATE STATURE
                if (attackerVictorious)
                {
                    statureChange = 0.8 * (defender.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.statureChangeAttacker = statureChange;
                    attacker.GetOwner().AdjustStatureModifier(statureChange);
                    statureChange = -0.5 * (attacker.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.statureChangeDefender = statureChange;
                    defender.GetOwner().AdjustStatureModifier(statureChange);
                }
                else
                {
                    statureChange = 0.8 * (attacker.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.statureChangeDefender = statureChange;
                    defender.GetOwner().AdjustStatureModifier(statureChange);
                    statureChange = -0.5 * (defender.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.statureChangeAttacker = statureChange;
                    attacker.GetOwner().AdjustStatureModifier(statureChange);
                }

                // CASUALTIES
                // calculate troop casualties for both sides
                casualtyModifiers = Battle.CalculateBattleCasualties(attackerStartTroops, defenderStartTroops, battleValues[0], battleValues[1], attackerVictorious);


                uint totalAttackTroopsLost = 0;
                uint totalDefendTroopsLost = 0;

                // if losing side sustains >= 50% casualties, disbands
                if (attackerVictorious)
                {
                    // either indicate losing army to be disbanded
                    if (casualtyModifiers[1] >= 0.5)
                    {
                        defenderDisbanded = true;
                        disbandedArmies.Add(defender.owner);
                        totalDefendTroopsLost = defender.CalcArmySize();
                    }
                    // OR apply troop casualties to losing army
                    else
                    {
                        totalDefendTroopsLost = defender.ApplyTroopLosses(casualtyModifiers[1]);
                    }

                    // apply troop casualties to winning army
                    totalAttackTroopsLost = attacker.ApplyTroopLosses(casualtyModifiers[0]);
                }
                else
                {
                    if (casualtyModifiers[0] >= 0.5)
                    {
                        attackerDisbanded = true;
                        disbandedArmies.Add(attacker.owner);
                        totalAttackTroopsLost = attacker.CalcArmySize();
                    }
                    else
                    {
                        totalAttackTroopsLost = attacker.ApplyTroopLosses(casualtyModifiers[0]);
                    }

                    totalDefendTroopsLost = defender.ApplyTroopLosses(casualtyModifiers[1]);
                }
                battleResults.attackerCasualties = totalAttackTroopsLost;
                battleResults.defenderCasualties = totalDefendTroopsLost;
                // UPDATE TOTAL SIEGE LOSSES, if appropriate
                // NOTE: the defender in this battle is the attacker in the siege and v.v.
                if (thisSiege != null)
                {
                    // update total siege attacker (defender in this battle) losses
                    thisSiege.totalCasualtiesAttacker += Convert.ToInt32(totalDefendTroopsLost);

                    // update total siege defender (attacker in this battle) losses
                    if (circumstance.Equals("siege"))
                    {
                        thisSiege.totalCasualtiesDefender += Convert.ToInt32(totalAttackTroopsLost);
                    }
                }

                // get casualty figures (for message)
                if (!attackerDisbanded)
                {
                    // get attacker casualties
                    attackerCasualties = totalAttackTroopsLost;
                }
                if (!defenderDisbanded)
                {
                    // get defender casualties
                    defenderCasualties = totalDefendTroopsLost;
                }

                // DAYS
                // adjust days
                // NOTE: don't adjust days if is a siege (will be deducted elsewhere)
                if (!circumstance.Equals("siege"))
                {
                    if (attackerLeader != null)
                    {
                        attackerLeader.AdjustDays(1);
                    }
                    // need to check for defender having no leader
                    if (defenderLeader != null)
                    {
                        defenderLeader.AdjustDays(1);
                    }
                    else
                    {
                        defender.days -= 1;
                    }
                }

                // RETREATS
                // create array of armies (for easy processing)
                Army[] bothSides = { attacker, defender };

                // check if either army needs to retreat
                int[] retreatDistances = Battle.CheckForRetreat(attacker, defender, casualtyModifiers[0], casualtyModifiers[1], attackerVictorious);

                // if is pillage or siege, attacking army (the fief's army) doesn't retreat
                // if is pillage, the defending army (the pillagers) always retreats if has lost
                if (circumstance.Equals("pillage") || circumstance.Equals("siege"))
                {
                    retreatDistances[0] = 0;
                }

                if (circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        retreatDistances[1] = 1;
                    }
                }

                // if have retreated, perform it
                for (int i = 0; i < retreatDistances.Length; i++)
                {
                    if (retreatDistances[i] > 0)
                    {
                        bothSides[i].ProcessRetreat(retreatDistances[i]);
                    }
                }
                // If attacker has retreated add to retreat list
                if (retreatDistances[0] > 0)
                {
                    retreatedArmies.Add(battleResults.attackerOwner);
                }
                // If defender retreated add to retreat list
                if (retreatDistances[1] > 0)
                {
                    retreatedArmies.Add(battleResults.defenderOwner);
                }
                // PC/NPC INJURIES/DEATHS
                // check if any PCs/NPCs have been wounded or killed
                bool characterDead = false;

                // 1. ATTACKER
                uint friendlyBV = battleValues[0];
                uint enemyBV = battleValues[1];

                // if army leader a PC, check entourage
                if (attackerLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (attackerLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        if ((attackerLeader as PlayerCharacter).myNPCs[i].inEntourage)
                        {
                            characterDead = (attackerLeader as PlayerCharacter).myNPCs[i].CalculateCombatInjury(casualtyModifiers[0]);
                        }

                        // process death, if applicable
                        if (characterDead)
                        {
                            (attackerLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");             
                        }
                    }
                }

                // check army leader
                if (attackerLeader != null)
                {
                    attackerLeaderDead = attackerLeader.CalculateCombatInjury(casualtyModifiers[0]);
                }
                

                // process death, if applicable
                if (attackerLeaderDead)
                {
                    deadCharacters.Add(attackerLeader.firstName + " " + attackerLeader.familyName);
                    Character newLeader = null;

                    // if is pillage, do NOT elect new leader for attacking army
                    if (!circumstance.Equals("pillage"))
                    {
                        // if possible, elect new leader from entourage
                        if (attackerLeader is PlayerCharacter)
                        {
                            if ((attackerLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = (attackerLeader as PlayerCharacter).ElectNewArmyLeader();
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        attacker.AssignNewLeader(newLeader);
                    }
                }
                else
                {
                    // if pillage, if fief's army loses, make sure bailiff always returns to keep
                    if (circumstance.Equals("pillage"))
                    {
                        if (!attackerVictorious)
                        {
                            attackerLeader.inKeep = true;
                        }
                    }
                }

                // 2. DEFENDER

                // need to check if defending army had a leader
                if (defenderLeader != null)
                {
                    // if army leader a PC, check entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs[i].inEntourage)
                            {
                                characterDead = (defenderLeader as PlayerCharacter).myNPCs[i].CalculateCombatInjury(casualtyModifiers[1]);
                            }

                            // process death, if applicable
                            if (characterDead)
                            {
                                (defenderLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");
                            }
                        }
                    }

                    // check army leader
                    defenderLeaderDead = defenderLeader.CalculateCombatInjury(casualtyModifiers[1]);

                    // process death, if applicable
                    if (defenderLeaderDead)
                    {
                        deadCharacters.Add(defenderLeader.firstName + " " + defenderLeader.familyName);
                        Character newLeader = null;

                        // if possible, elect new leader from entourage
                        if (defenderLeader is PlayerCharacter)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = (defenderLeader as PlayerCharacter).ElectNewArmyLeader();
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        defender.AssignNewLeader(newLeader);
                    }
                }

                battleResults.deaths = deadCharacters.ToArray();
                battleResults.retreatedArmies = retreatedArmies.ToArray();
                
                battleResults.attackerVictorious = attackerVictorious;

                // check for SIEGE RELIEF
                if (thisSiege != null)
                {
                    battleResults.isSiege = true;
                    battleResults.siegeBesieger = thisSiege.GetBesiegingPlayer().firstName + " " +thisSiege.GetBesiegingPlayer().familyName;
                    battleResults.siegeDefender = thisSiege.GetDefendingPlayer().firstName + " " + thisSiege.GetDefendingPlayer().familyName;
                    // attacker (relieving army) victory or defender (besieging army) retreat = relief
                    if ((attackerVictorious) || (retreatDistances[1] > 0))
                    {
                        // indicate siege raised
                        siegeRaised = true;
                        battleResults.siegeRaised = true;
                    }

                    // check to see if siege raised due to death of siege owner with no heir
                    else if ((defenderLeaderDead) && ((defenderLeader as PlayerCharacter) == thisSiege.GetBesiegingPlayer()))
                    {
                        // get siege owner's heir
                        Character thisHeir = (defenderLeader as PlayerCharacter).GetHeir();

                        if (thisHeir == null)
                        {
                            battleResults.DefenderDeadNoHeir = true;
                            // indicate siege raised
                            siegeRaised = true;

                        }
                    }
                }

            }
            
            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            // personae tags vary depending on circumstance
            string attackOwnTag = "|attackerOwner";
            string attackLeadTag = "|attackerLeader";
            string defendOwnTag = "|defenderOwner";
            string defendLeadTag = "|defenderLeader";
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("siege")))
            {
                attackOwnTag = "|sallyOwner";
                attackLeadTag = "|sallyLeader";
                defendOwnTag = "|defenderAgainstSallyOwner";
                defendLeadTag = "|defenderAgainstSallyLeader";
            }
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(defender.GetOwner().charID + defendOwnTag);
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + attackLeadTag);
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + defendLeadTag);
            }
            tempPersonae.Add(attacker.GetOwner().charID + attackOwnTag);
            tempPersonae.Add(attacker.GetLocation().owner.charID + "|fiefOwner");
            if ((!circumstance.Equals("pillage")) && (!circumstance.Equals("siege")))
            {
                tempPersonae.Add("all|all");
            }
            string[] battlePersonae = tempPersonae.ToArray();

            // location
            string battleLocation = attacker.GetLocation().id;


            // put together new journal entry
            JournalEntry battleResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, battlePersonae, "battle",battleResults, loc: battleLocation);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(battleResult);

            // display pop-up informational message
            battleResults.ActionType = Actions.Update;
            battleResults.ResponseType = DisplayMessages.BattleResults;
            if (battleHasCommenced)
            {
                Globals_Game.UpdatePlayer(defender.GetOwner().playerID, DisplayMessages.BattleBringSuccess, new string[] { battleResults.attackerOwner });
            }
            else
            {
                Globals_Game.UpdatePlayer(defender.GetOwner().playerID, DisplayMessages.BattleBringFail, new string[] { battleResults.attackerOwner });
            }

            // end siege if appropriate
            if (siegeRaised)
            {
                //HACK
                thisSiege.SiegeEnd(false, DisplayMessages.BattleResults,new string[]{DisplaySiegeResults(battleResults)});
                thisSiege = null;

                // ensure if siege raised correct value returned to Form1.siegeReductionRound method
                if (circumstance.Equals("siege"))
                {
                    attackerVictorious = true;
                }
            }

            // process leader deaths
            if (defenderLeaderDead)
            {
                defenderLeader.ProcessDeath("injury");
            }
            else if (attackerLeaderDead)
            {
                attackerLeader.ProcessDeath("injury");
            }


            // DISBANDMENT

            // if is pillage, attacking (temporary) army always disbands after battle
            if (circumstance.Equals("pillage"))
            {
                attackerDisbanded = true;
            }

            // process army disbandings (after all other functions completed)
            if (attackerDisbanded)
            {
                attacker.DisbandArmy();
                attacker = null;
            }

            if (defenderDisbanded)
            {
                defender.DisbandArmy();
                defender = null;
            }

            return attackerVictorious;

        }

    }
}
