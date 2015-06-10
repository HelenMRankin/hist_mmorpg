using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    public static class Birth
    {
        /// <summary>
        /// Generates a new NPC based on parents' statistics
        /// </summary>
        /// <returns>NonPlayerCharacter or null</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public static NonPlayerCharacter GenerateNewNPC(Character mummy, Character daddy)
        {
            NonPlayerCharacter newNPC = new NonPlayerCharacter();

            // charID
            newNPC.charID = Globals_Game.GetNextCharID();
            // first name
            newNPC.firstName = "Baby";
            // family name
            newNPC.familyName = daddy.familyName;
            // date of birth
            newNPC.birthDate = new Tuple<uint, byte>(Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
            // sex
            newNPC.isMale = Birth.GenerateSex();
            // nationality
            newNPC.nationality = daddy.nationality;
            // whether is alive
            newNPC.isAlive = true;
            // maxHealth
            newNPC.maxHealth = Birth.GenerateKeyCharacteristics(mummy.maxHealth, daddy.maxHealth);
            // virility
            newNPC.virility = Birth.GenerateKeyCharacteristics(mummy.virility, daddy.virility);
            // goTo queue
            newNPC.goTo = new Queue<Fief>();
            // language
            newNPC.language = daddy.language;
            // days left
            newNPC.days = 90;
            // stature modifier
            newNPC.statureModifier = 0;
            // management
            newNPC.management = Birth.GenerateKeyCharacteristics(mummy.management, daddy.management);
            // combat
            newNPC.combat = Birth.GenerateKeyCharacteristics(mummy.combat, daddy.combat);
            // traits
            newNPC.traits = Birth.GenerateTraitSetFromParents(mummy.traits, daddy.traits, newNPC.isMale);
            // if in keep
            newNPC.inKeep = mummy.inKeep;
            // if pregnant
            newNPC.isPregnant = false;
            // familyID
            newNPC.familyID = daddy.familyID;
            // spouse
            newNPC.spouse = null;
            // father
            newNPC.father = daddy.charID;
            // mother
            newNPC.mother = mummy.charID;
            // fiancee
            newNPC.fiancee = null;
            // location
            newNPC.location = null;
            // titles
            newNPC.myTitles = new List<string>();
            // armyID
            newNPC.armyID = null;
            // ailments
            newNPC.ailments = new Dictionary<string, Ailment>();
            // employer
            newNPC.employer = null;
            // salary/allowance
            newNPC.salary = 0;
            // lastOffer (will remain empty for family members)
            newNPC.lastOffer = new Dictionary<string, uint>();
            // inEntourage
            newNPC.inEntourage = false;
            // isHeir
            newNPC.isHeir = false;

            return newNPC;
        }

        /// <summary>
        /// Generates a random sex for a Character
        /// </summary>
        /// <returns>bool indicating whether is male</returns>
        public static bool GenerateSex()
        {
            bool isMale = false;

            // generate random (0-1) to see if male or female
            if (Globals_Game.myRand.Next(0, 2) == 0)
            {
                isMale = true;
            }

            return isMale;
        }

        /// <summary>
        /// Generates a characteristic stat for a Character, based on parent stats
        /// </summary>
        /// <returns>Double containing characteristic stat</returns>
        /// <param name="mummyStat">The mother's characteristic stat</param>
        /// <param name="daddyStat">The father's characteristic stat</param>
        public static Double GenerateKeyCharacteristics(Double mummyStat, Double daddyStat)
        {
            Double newStat = 0;

            // get average of parents' stats
            Double parentalAverage = (mummyStat + daddyStat) / 2;

            // generate random (0 - 100) to determine relationship of new stat to parentalAverage
            double randPercentage = Utility_Methods.GetRandomDouble(100);

            // calculate new stat
            if (randPercentage <= 35)
            {
                newStat = parentalAverage;
            }
            else if (randPercentage <= 52.5)
            {
                newStat = parentalAverage - 1;
            }
            else if (randPercentage <= 70)
            {
                newStat = parentalAverage + 1;
            }
            else if (randPercentage <= 80)
            {
                newStat = parentalAverage - 2;
            }
            else if (randPercentage <= 90)
            {
                newStat = parentalAverage + 2;
            }
            else if (randPercentage <= 95)
            {
                newStat = parentalAverage - 3;
            }
            else
            {
                newStat = parentalAverage + 3;
            }

            // make sure new stat falls within acceptable range
            if (newStat < 1)
            {
                newStat = 1;
            }
            else if (newStat > 9)
            {
                newStat = 9;
            }

            return newStat;
        }

        /// <summary>
        /// Generates a trait set for a Character, based on parent traits
        /// </summary>
        /// <returns>Array containing trait set</returns>
        /// <param name="mummyTraits">The mother's traits</param>
        /// <param name="daddyTraits">The father's traits</param>
        /// <param name="isMale">Whether character is a male</param>
        public static Tuple<Trait, int>[] GenerateTraitSetFromParents(Tuple<Trait, int>[] mummyTraits, Tuple<Trait, int>[] daddyTraits, bool isMale)
        {
            // store all unique traitKeys from both parents
            List<string> uniqueTraitKeys = new List<string>();

            // mummy's traits
            for (int i = 0; i < mummyTraits.Length; i++)
            {
                uniqueTraitKeys.Add(mummyTraits[i].Item1.id);
            }

            // daddy's traits
            for (int i = 0; i < daddyTraits.Length; i++)
            {
                if (!uniqueTraitKeys.Contains(daddyTraits[i].Item1.id))
                {
                    uniqueTraitKeys.Add(daddyTraits[i].Item1.id);
                }
            }

            // create new traits using uniqueTraitKeys
            Tuple<Trait, int>[] newTraits = Utility_Methods.GenerateTraitSet(uniqueTraitKeys);

            return newTraits;
        }

        /// <summary>
        /// Performs standard conditional checks before a pregnancy attempt
        /// </summary>
        /// <returns>bool indicating whether or not to proceed with pregnancy attempt</returns>
        /// <param name="husband">The husband</param>
        public static bool ChecksBeforePregnancyAttempt(Character husband)
        {
            bool proceed = true;
            bool isPlayer = husband is PlayerCharacter;
            bool isSonOfPlayer = (husband.GetFather()) is PlayerCharacter;
            if (!isPlayer && !isSonOfPlayer)
            {
                //TODO error log
                return false;
            }
            // check is married
            // get spouse
            Character wife = husband.GetSpouse();

            if (wife != null)
            {
                // check to make sure is in same fief
                if (!(wife.location == husband.location))
                {
                    if (isPlayer)
                    {  
                        string message = "You have to be in the same fief to do that!";
                        Globals_Game.UpdatePlayer(((PlayerCharacter)husband).playerID, message);
                    }
                    proceed = false;
                }

                else
                {
                    // make sure wife not already pregnant
                    if (wife.isPregnant)
                    {
                        if (isPlayer)
                        {
                            string message = "error:birth:" + wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!";
                            Globals_Game.UpdatePlayer(((PlayerCharacter)husband).playerID,message);
                        }
                        proceed = false;
                    }

                    // check if are kept apart by siege
                    else
                    {
                        if ((!String.IsNullOrWhiteSpace(husband.location.siege)) && (husband.inKeep != wife.inKeep))
                        {
                            if (isPlayer)
                            {
                                string message = "error:birth:I'm afraid the husband and wife are being separated by the ongoing siege.";
                                Globals_Game.UpdatePlayer(((PlayerCharacter)husband).playerID, message);
                            }
                            proceed = false;
                        }

                        else
                        {
                            // ensure player and spouse have at least 1 day remaining
                            double minDays = Math.Min(husband.days, wife.days);

                            if (minDays < 1)
                            {
                                if (isPlayer)
                                {
                                    string message = "error:birth:Sorry, you don't have enough time left for this in the current season.";
                                    Globals_Game.UpdatePlayer(((PlayerCharacter)husband).playerID, message);
                                }
                                proceed = false;
                            }
                            else
                            {
                                // ensure days are synchronised
                                if (husband.days != wife.days)
                                {
                                    if (husband.days != minDays)
                                    {
                                        if (husband is PlayerCharacter)
                                        {
                                            (husband as PlayerCharacter).AdjustDays(husband.days - minDays);
                                        }
                                        else
                                        {
                                            husband.AdjustDays(husband.days - minDays);
                                        }
                                    }
                                    else
                                    {
                                        wife.AdjustDays(wife.days - minDays);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // If the husband is a player, alert the husband's user
            // otherwise, the husband is the son of a player- alert the husband's father's user
            else
            {
                string toDisplay;
                string whoThisIs = "";
                if (isPlayer)
                {
                    whoThisIs = "You are ";
                }
                else
                {
                    whoThisIs = "This man is ";
                }
                toDisplay = "error:birth:," + whoThisIs + "not married, my lord.";
                if(isPlayer) Globals_Game.UpdatePlayer(((PlayerCharacter)husband).playerID, toDisplay);
                else
                {
                    Globals_Game.UpdatePlayer(((PlayerCharacter)husband.GetFather()).playerID, toDisplay);
                }
                proceed = false;
            }

            return proceed;
        }

    }
}
