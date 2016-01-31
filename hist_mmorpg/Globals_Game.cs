using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required game-wide static variables and related methods
    /// </summary>
    public static class Globals_Game
    {
        /// <summary>
        /// Dictionary mapping users to player characters
        /// </summary>
        public static Dictionary<string, PlayerCharacter> userChars = new Dictionary<string, PlayerCharacter>();
        /// <summary>
        /// Holds current challenges for ownership of provinces or kingdoms
        /// </summary>
        public static Dictionary<string, OwnershipChallenge> ownershipChallenges = new Dictionary<string, OwnershipChallenge>();
        /// <summary>
        /// Holds data for all players required for the calculation of individual victory
        /// </summary>
        public static Dictionary<string, VictoryData> victoryData = new Dictionary<string,VictoryData>();
        /// <summary>
        /// Holds keys for VictoryData objects (used when retrieving from database)
        /// </summary>
        public static List<String> victoryDataKeys = new List<String>();
        /// <summary>
        /// Holds PlayerCharacter associated with the position of sysAdmin for the game
        /// </summary>
        public static PlayerCharacter sysAdmin;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of king for kingdom one
        /// </summary>
        public static PlayerCharacter kingOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of king for kingdom two
        /// </summary>
        public static PlayerCharacter kingTwo;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of prince for kingdom one
        /// </summary>
        public static PlayerCharacter princeOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of prince for kingdom two
        /// </summary>
        public static PlayerCharacter princeTwo;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of herald for kingdom one
        /// </summary>
        public static PlayerCharacter heraldOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of herald for kingdom two
        /// </summary>
        public static PlayerCharacter heraldTwo;
        /// <summary>
        /// List of registered observers
        /// </summary>
        private static List<Client> registeredObservers = new List<Client>();
        /// <summary>
        /// Holds all NonPlayerCharacter objects
        /// </summary>
        public static Dictionary<string, NonPlayerCharacter> npcMasterList = new Dictionary<string, NonPlayerCharacter>();
        /// <summary>
        /// Holds keys for NonPlayerCharacter objects (used when retrieving from database)
        /// </summary>
        public static List<String> npcKeys = new List<String>();
        /// <summary>
        /// Holds all PlayerCharacter objects <charID, PlayerCharacter>
        /// </summary>
        public static Dictionary<string, PlayerCharacter> pcMasterList = new Dictionary<string, PlayerCharacter>();
        /// <summary>
        /// Holds keys for PlayerCharacter objects (used when retrieving from database)
        /// </summary>
        public static List<String> pcKeys = new List<String>();
        /// <summary>
        /// Holds all Fief objects
        /// </summary>
        public static Dictionary<string, Fief> fiefMasterList = new Dictionary<string, Fief>();
        /// <summary>
        /// Holds keys for Fief objects (used when retrieving from database)
        /// </summary>
        public static List<String> fiefKeys = new List<String>();
        /// <summary>
        /// Holds all Province objects
        /// </summary>
        public static Dictionary<string, Province> provinceMasterList = new Dictionary<string, Province>();
        /// <summary>
        /// Holds keys for Province objects (used when retrieving from database)
        /// </summary>
        public static List<String> provKeys = new List<String>();
        /// <summary>
        /// Holds all Kingdom objects
        /// </summary>
        public static Dictionary<string, Kingdom> kingdomMasterList = new Dictionary<string, Kingdom>();
        /// <summary>
        /// Holds keys for Kingdom objects (used when retrieving from database)
        /// </summary>
        public static List<String> kingKeys = new List<String>();
        /// <summary>
        /// Holds all Rank objects
        /// </summary>
        public static Dictionary<byte, Rank> rankMasterList = new Dictionary<byte, Rank>();
        /// <summary>
        /// Holds keys for Rank objects (used when retrieving from database)
        /// </summary>
        public static List<byte> rankKeys = new List<byte>();
        /// <summary>
        /// Holds all Terrain objects
        /// </summary>
        public static Dictionary<string, Terrain> terrainMasterList = new Dictionary<string, Terrain>();
        /// <summary>
        /// Holds keys for Terrain objects (used when retrieving from database)
        /// </summary>
        public static List<String> terrKeys = new List<String>();
        /// <summary>
        /// Holds all BaseLanguage objects
        /// </summary>
        public static Dictionary<string, BaseLanguage> baseLanguageMasterList = new Dictionary<string, BaseLanguage>();
        /// <summary>
        /// Holds keys for BaseLanguage objects (used when retrieving from database)
        /// </summary>
        public static List<String> baseLangKeys = new List<String>();
        /// <summary>
        /// Holds all Language objects
        /// </summary>
        public static Dictionary<string, Language> languageMasterList = new Dictionary<string, Language>();
        /// <summary>
        /// Holds keys for Language objects (used when retrieving from database)
        /// </summary>
        public static List<String> langKeys = new List<String>();
        /// <summary>
        /// Holds all Trait objects
        /// </summary>
        public static Dictionary<string, Trait> traitMasterList = new Dictionary<string, Trait>();
        /// <summary>
        /// Holds keys for Trait objects (used when retrieving from database)
        /// </summary>
        public static List<String> traitKeys = new List<String>();
        /// <summary>
        /// Holds all army objects
        /// </summary>
        public static Dictionary<string, Army> armyMasterList = new Dictionary<string, Army>();
        /// <summary>
        /// Holds keys for army objects (used when retrieving from database)
        /// </summary>
        public static List<String> armyKeys = new List<String>();
        /// <summary>
        /// Holds all siege objects
        /// </summary>
        public static Dictionary<string, Siege> siegeMasterList = new Dictionary<string, Siege>();
        /// <summary>
        /// Holds keys for siege objects (used when retrieving from database)
        /// </summary>
        public static List<String> siegeKeys = new List<String>();
        /// <summary>
        /// Holds all nationality objects
        /// </summary>
        public static Dictionary<string, Nationality> nationalityMasterList = new Dictionary<string, Nationality>();
        /// <summary>
        /// Holds keys for nationality objects (used when retrieving from database)
        /// </summary>
        public static List<string> nationalityKeys = new List<string>();
        /// <summary>
        /// Holds all position objects
        /// </summary>
        public static Dictionary<byte, Position> positionMasterList = new Dictionary<byte, Position>();
        /// <summary>
        /// Holds keys for position objects (used when retrieving from database)
        /// </summary>
        public static List<byte> positionKeys = new List<byte>();
        /// <summary>
        /// Holds Character_Serialised objects with existing goTo queues (used during load from database)
        /// </summary>
        public static List<Character_Serialised> goToList = new List<Character_Serialised>();
        /// <summary>
        /// Holds random for use with various methods
        /// </summary>
        public static Random myRand = new Random();
        /// <summary>
        /// Holds next value for character ID
        /// </summary>
        public static uint newCharID = 7000;
        /// <summary>
        /// Holds next value for army ID
        /// </summary>
        public static uint newArmyID = 1;
        /// <summary>
        /// Holds next value for detachment ID
        /// </summary>
        public static uint newDetachmentID = 1;
        /// <summary>
        /// Holds next value for ailment ID
        /// </summary>
        public static uint newAilmentID = 1;
        /// <summary>
        /// Holds next value for siege ID
        /// </summary>
        public static uint newSiegeID = 1;
        /// <summary>
        /// Holds next value for JournalEntry ID
        /// </summary>
        public static uint newJournalEntryID = 1;
        /// <summary>
        /// Holds next value for OwnershipChallenge ID
        /// </summary>
        public static uint newOwnChallengeID = 1;
        /// <summary>
        /// Holds HexMapGraph for this game
        /// </summary>
        public static HexMapGraph gameMap;
        /// <summary>
        /// Holds GameClock for this game
        /// </summary>
        public static GameClock clock { get; set; }
        /// <summary>
        /// Holds scheduled events
        /// </summary>
        public static Journal scheduledEvents = new Journal();
        /// <summary>
        /// Holds past events
        /// </summary>
        public static Journal pastEvents = new Journal();
        /// <summary>
        /// Holds priorities for types of JournalEntry
        /// Key = JournalEntry type
        /// Value = 0-2 byte indicating priority level
        /// </summary>
        public static Dictionary<string[], byte> jEntryPriorities = new Dictionary<string[], byte>();
        /// <summary>
        /// Holds newly promoted NPCs to be added to pcMasterList (during seasonUpdate)
        /// </summary>
        public static List<PlayerCharacter> promotedNPCs = new List<PlayerCharacter>();
        /// <summary>
        /// Holds type of game (sets victory conditions)
        /// 0 = individual point game
        /// 1 = individual position game
        /// 2 = team historical game
        /// </summary>
        public static uint gameType = 0;
        /// <summary>
        /// Holds duration (number of turns) for the current game
        /// </summary>
        public static uint duration = 100;
        /// <summary>
        /// Holds start year for current game
        /// </summary>
        public static uint startYear = 1337;
        /// <summary>
        /// Holds bool indicating whether or not to load initial object data from database on startup
        /// </summary>
		public static bool loadFromDatabase = false;
        /// <summary>
        /// Holds bool indicating whether or not to load initial object data from CSV file
        /// </summary>
        public static bool loadFromCSV = true;
        /// <summary>
        /// Holds bool indicating whether or not to write current object data to database on exit
        /// </summary>
		public static bool writeToDatabase = false;
        /// <summary>
        /// Holds bool indicating whether or not a cap on characters' stature modifier is in force
        /// </summary>
        public static bool statureCapInForce = true;

        /// <summary>
        /// Gets the game's end date (year)
        /// </summary>
        /// <returns>uint containing end year</returns>
        public static uint GetGameEndDate()
        {
            return Globals_Game.startYear + Globals_Game.duration;
        }
        
        /// <summary>
        /// Gets the current scores for all players
        /// </summary>
        /// <returns>SortedList<double, string> containing current scores</returns>
        public static SortedList<double, string> GetCurrentScores()
        {
            SortedList<double, string> currentScores = new SortedList<double, string>();
            double thisScore = 0;

            foreach (KeyValuePair<string, VictoryData> scoresEntry in Globals_Game.victoryData)
            {
                // reset score
                thisScore = 0;

                // get score
                thisScore += scoresEntry.Value.CalcStatureScore();
                thisScore += scoresEntry.Value.CalcPopulationScore();
                thisScore += scoresEntry.Value.CalcFiefScore();
                thisScore += scoresEntry.Value.CalcMoneyScore();

                // add to list
                currentScores.Add(thisScore, scoresEntry.Value.playerID);
            }

            return currentScores;
        }

        /// <summary>
        /// Gets the total money owned by all PlayerCharacters in the game
        /// </summary>
        /// <returns>int containing total money</returns>
        public static int GetTotalMoney()
        {
            int totMoney = 0;

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                totMoney += fiefEntry.Value.treasury;
            }

            return totMoney;
        }
        
        /// <summary>
        /// Gets the total population for all fiefs in the game
        /// </summary>
        /// <returns>int containing total population</returns>
        public static int GetTotalPopulation()
        {
            int totPop = 0;

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                totPop += fiefEntry.Value.population;
            }

            return totPop;
        }

        /// <summary>
        /// Gets the total number of fiefs in the game
        /// </summary>
        /// <returns>int containing number of fiefs</returns>
        public static int GetTotalFiefs()
        {
            return Globals_Game.fiefMasterList.Count;
        }

        /// <summary>
        /// Gets the next available newCharID, then increments it
        /// </summary>
        /// <returns>string containing newCharID</returns>
        public static string GetNextCharID()
        {
            string charID = "Char_" + Globals_Game.newCharID;
            Globals_Game.newCharID++;
            return charID;
        }

        /// <summary>
        /// Gets the next available newArmyID, then increments it
        /// </summary>
        /// <returns>string containing newArmyID</returns>
        public static string GetNextArmyID()
        {
            string armyID = "Army_" + Globals_Game.newArmyID;
            Globals_Game.newArmyID++;
            return armyID;
        }

        /// <summary>
        /// Gets the next available newDetachmentID, then increments it
        /// </summary>
        /// <returns>string containing newDetachmentID</returns>
        public static string GetNextDetachmentID()
        {
            string detachmentID = "Det_" + Globals_Game.newDetachmentID;
            Globals_Game.newDetachmentID++;
            return detachmentID;
        }

        /// <summary>
        /// Gets the next available newAilmentID, then increments it
        /// </summary>
        /// <returns>string containing newAilmentID</returns>
        public static string GetNextAilmentID()
        {
            string ailmentID = "Ail_" + Globals_Game.newAilmentID;
            Globals_Game.newAilmentID++;
            return ailmentID;
        }

        /// <summary>
        /// Gets the next available newSiegeID, then increments it
        /// </summary>
        /// <returns>string containing newSiegeID</returns>
        public static string GetNextSiegeID()
        {
            string siegeID = "Siege_" + Globals_Game.newSiegeID;
            Globals_Game.newSiegeID++;
            return siegeID;
        }

        /// <summary>
        /// Gets the next available newJournalEntryID, then increments it
        /// </summary>
        /// <returns>uint containing newJournalEntryID</returns>
        public static uint GetNextJournalEntryID()
        {
            uint newID = 0;

            // get newJournalEntryID
            newID = Globals_Game.newJournalEntryID;

            // increment newJournalEntryID
            Globals_Game.newJournalEntryID++;

            return newID;
        }

        /// <summary>
        /// Adds a new JournalEntry to the scheduledEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public static bool AddScheduledEvent(JournalEntry jEvent)
        {
            bool success = false;

            success = Globals_Game.scheduledEvents.AddNewEntry(jEvent);

            return success;

        }

        /// <summary>
        /// Adds a new JournalEntry to the pastEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public static bool AddPastEvent(JournalEntry jEvent)
        {
            bool success = false;

            success = Globals_Game.pastEvents.AddNewEntry(jEvent);
            if (success)
            {
                Globals_Game.NotifyObservers("newEvent|" + jEvent.jEntryID);
            }

            return success;

        }

        /// <summary>
        /// Gets the next available newOwnChallengeID, then increments it
        /// </summary>
        /// <returns>string containing newOwnChallengeID</returns>
        public static string GetNextOwnChallengeID()
        {
            string challengeID = "Challenge_" + Globals_Game.newOwnChallengeID;
            Globals_Game.newOwnChallengeID++;
            return challengeID;
        }

        /// <summary>
        /// Checks for the existence of a challenge for the same Place
        /// </summary>
        /// <returns>bool indicating the existence of a challenge</returns>
        /// <param name="placeID">ID of Place for new challenge</param>
        public static bool CheckForExistingChallenge(string newPlaceID)
        {
            bool isExistingChallenge = false;

            foreach (KeyValuePair<string, OwnershipChallenge> challengeEntry in Globals_Game.ownershipChallenges)
            {
                if (challengeEntry.Value.placeID.Equals(newPlaceID))
                {
                    isExistingChallenge = true;
                    break;
                }
            }

            return isExistingChallenge;
        }

        /// <summary>
        /// Adds a new OwnershipChallenge to the ownershipChallenges collection
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="challenge">OwnershipChallenge to be added</param>
        public static bool AddOwnershipChallenge(OwnershipChallenge challenge)
        {
            bool success = true;
            string toDisplay = "";

            if (Globals_Game.CheckForExistingChallenge(challenge.placeID))
            {
                success = false;
                PlayerCharacter player = challenge.GetChallenger();
                if (player != null)
                {
                    toDisplay = "There is already a challenge for the ownership of " + challenge.GetPlace().name + ". Only one challenge is permissable at a time.";
                    UpdatePlayer(player.playerID, toDisplay);
                }
            }

            else
            {
                Globals_Game.ownershipChallenges.Add(challenge.id, challenge);
            }

            return success;
        }

        /// <summary>
        /// Processes all challenges in the ownershipChallenges collection
        /// </summary>
        /// <returns>bool indicating at least one place has changed ownership</returns>
        public static bool ProcessOwnershipChallenges()
        {
            bool ownershipChanged = false;
            List<OwnershipChallenge> toBeRemoved = new List<OwnershipChallenge>();
            PlayerCharacter challenger = null;
            Place contestedPlace = null;
            PlayerCharacter currentOwner = null;

            foreach (KeyValuePair<string, OwnershipChallenge> challenge in Globals_Game.ownershipChallenges)
            {
                // get challenger and place
                challenger = challenge.Value.GetChallenger();
                contestedPlace = challenge.Value.GetPlace();

                // prepare JOURNAL ENTRY
                bool createJournalEntry = false;

                // get interested parties
                currentOwner = contestedPlace.owner;

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // location
                string entryLoc = contestedPlace.id;

                // personae
                string currentOwnerEntry = "";
                string challengerEntry = "";
                string[] entryPersonae = new string[2];

                // description
                string description = "";

                // entry type
                string entryType = "";

                // CALCULATE SUCCESS
                // variables needed for calculation
                int challengerTally = 0;
                int totalParts = 0;
                double successThreshold = 0;

                // get challenger's ownership tally
                if ((contestedPlace != null) && (challenger != null))
                {
                    if (challenge.Value.placeType.Equals("province"))
                    {
                        // iterate through all fiefs
                        foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                        {
                            if (fiefEntry.Value.province.id.Equals(contestedPlace.id))
                            {
                                // update total fiefs in province
                                totalParts++;

                                // update challenger's ownership tally
                                if (fiefEntry.Value.owner == challenger)
                                {
                                    challengerTally++;
                                }
                            }
                        }

                        // work out success threshold
                        successThreshold = totalParts / 2.0;
                    }

                    if (challenge.Value.placeType.Equals("kingdom"))
                    {
                        // iterate through all provinces
                        foreach (KeyValuePair<string, Province> provEntry in Globals_Game.provinceMasterList)
                        {
                            if (provEntry.Value.GetCurrentKingdom().id.Equals(contestedPlace.id))
                            {
                                // update total provinces in kingdom
                                totalParts++;

                                // update challenger's ownership tally
                                if (provEntry.Value.owner == challenger)
                                {
                                    challengerTally++;
                                }
                            }
                        }

                        // work out success threshold
                        successThreshold = totalParts / 2.0;
                    }

                    // check to see if ownership condition has been met
                    // ownership condition MET
                    if (challengerTally > successThreshold)
                    {
                        // increment challenge counter
                        challenge.Value.counter++;

                        // check to see if challenge has succeeded
                        if (challenge.Value.counter == 4)
                        {
                            // province
                            if (challenge.Value.placeType.Equals("province"))
                            {
                                // process success
                                (contestedPlace as Province).TransferOwnership(challenger);
                                createJournalEntry = true;

                                // journal entry personae
                                string allEntry = "all|all";
                                currentOwnerEntry = currentOwner.charID + "|oldOwner";
                                challengerEntry = challenger.charID + "|newOwner";
                                entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                                // entry type
                                entryType = "ownershipChallenge_success";

                                // journal entry description
                                description = "On this day of Our Lord a challenge for the ownership of " + contestedPlace.name + " (" + contestedPlace.id + ")";
                                description += " was SUCCESSFUL.  " + challenger.firstName + " " + challenger.familyName + " succeeds ";
                                description += currentOwner.firstName + " " + currentOwner.familyName + " as owner.";
                            }

                            // kingdom
                            else if (challenge.Value.placeType.Equals("kingdom"))
                            {
                                // process success
                                (contestedPlace as Kingdom).TransferOwnership(challenger);
                                createJournalEntry = true;

                                // journal entry personae
                                string allEntry = "all|all";
                                currentOwnerEntry = currentOwner.charID + "|oldKing";
                                challengerEntry = challenger.charID + "|newKing";
                                entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                                // entry type
                                entryType = "depose_success";

                                // journal entry description
                                description = "On this day of Our Lord a challenge for the crown of " + contestedPlace.name + " (" + contestedPlace.id + ")";
                                description += " was SUCCESSFUL.  His highness " + challenger.firstName + " " + challenger.familyName + " succeeds ";
                                description += currentOwner.firstName + " " + currentOwner.familyName + " as King of " + contestedPlace.name + ".  Long may he reign.";
                            }

                            // mark challenge for removal
                            toBeRemoved.Add(challenge.Value);

                            // update ownershipChanged
                            ownershipChanged = true;
                        }
                    }

                    // ownership condition NOT met
                    else
                    {
                        // mark challenge for removal
                        toBeRemoved.Add(challenge.Value);

                        // province
                        if (challenge.Value.placeType.Equals("province"))
                        {
                            createJournalEntry = true;

                            // journal entry personae
                            currentOwnerEntry = currentOwner.charID + "|owner";
                            challengerEntry = challenger.charID + "|challenger";
                            entryPersonae = new string[] { currentOwnerEntry, challengerEntry };

                            // entry type
                            entryType = "ownershipChallenge_failure";

                            // journal entry description
                            description = "On this day of Our Lord a challenge for the ownership of " + contestedPlace.name + " (" + contestedPlace.id + ")";
                            description += " was UNSUCCESSFUL.  " + challenger.firstName + " " + challenger.familyName + " was unable to rest ownership from ";
                            description += currentOwner.firstName + " " + currentOwner.familyName + ".";
                        }

                        else if (challenge.Value.placeType.Equals("kingdom"))
                        {
                            createJournalEntry = true;

                            // journal entry personae
                            string allEntry = "all|all";
                            currentOwnerEntry = currentOwner.charID + "|king";
                            challengerEntry = challenger.charID + "|pretender";
                            entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                            // entry type
                            entryType = "depose_failure";

                            // journal entry description
                            description = "On this day of Our Lord a challenge for the crown of " + contestedPlace.name + " (" + contestedPlace.id + ")";
                            description += " was UNSUCCESSFUL.  The pretender " + challenger.firstName + " " + challenger.familyName + " was unable to press his claim and ";
                            description += "His Highness " + currentOwner.firstName + " " + currentOwner.familyName + " remains King of " + contestedPlace.name + "; long may he reign.";
                        }
                    }

                    // create and send a proposal (journal entry)
                    if (createJournalEntry)
                    {
                        // entry ID
                        uint entryID = Globals_Game.GetNextJournalEntryID();

                        JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType, descr: description, loc: entryLoc);
                        Globals_Game.AddPastEvent(myEntry);
                    }
                }
            }

            // clear challenges
            if (toBeRemoved.Count > 0)
            {
                foreach (OwnershipChallenge thisChallenge in toBeRemoved)
                {
                    Globals_Game.ownershipChallenges.Remove(thisChallenge.id);
                }
            }

            return ownershipChanged;
        }

        /// <summary>
        /// Checks for victory / end game
        /// </summary>
        /// <returns>bool indicating game end (victory achieved)</returns>
        public static bool CheckForVictory()
        {
            bool gameEnded = false;

            SortedList<double, string> currentScores = new SortedList<double, string>();
            if (Globals_Game.gameType == 0)
            {
                //update scores
                foreach (KeyValuePair<string, VictoryData> scoresEntry in Globals_Game.victoryData)
                {
                    scoresEntry.Value.UpdateData();
                }

                // get scores
                currentScores = Globals_Game.GetCurrentScores();
            }

            // CHECK FOR END GAME
            string gameResults = "";
            bool endDateReached = false;
            bool absoluteVictory = false;
            Kingdom victor = null;

            // absolute victory (all fiefs owned by one kingdom)
            victor = Globals_Game.CheckTeamAbsoluteVictory();
            if (victor != null)
            {
                absoluteVictory = true;
                gameResults += "The kingdom of " + victor.name + " under the valiant leadership of ";
                gameResults += victor.owner.firstName + " " + victor.owner.familyName;
                gameResults += " is victorious, having taken all fiefs under its control.";
            }

            // if no absolute victory
            else
            {
                // check if game end date reached
                if (Globals_Game.GetGameEndDate() == Globals_Game.clock.currentYear)
                {
                    endDateReached = true;
                }
            }

            // individual points game
            if (Globals_Game.gameType == 0)
            {
                if ((endDateReached) || (absoluteVictory))
                {
                    // get top scorer (ID)
                    string topScorer = currentScores.Last().Value;

                    foreach (KeyValuePair<double, string> scoresEntry in currentScores.Reverse())
                    {
                        // get PC
                        PlayerCharacter thisPC = Globals_Game.pcMasterList[Globals_Game.victoryData[scoresEntry.Value].playerCharacterID];

                        if (absoluteVictory)
                        {
                            gameResults += "\r\n\r\n";
                        }

                        // check for top scorer
                        if (thisPC.playerID.Equals(topScorer))
                        {
                            gameResults += "The individual winner is " + thisPC.firstName + " " + thisPC.familyName + " (player: " + thisPC.playerID + ")";
                            gameResults += " with a score of " + scoresEntry.Key + ".\r\n\r\nThe rest of the scores are:\r\n";
                        }

                        else
                        {
                            gameResults += thisPC.firstName + " " + thisPC.familyName + " (player: " + thisPC.playerID + ")";
                            gameResults += " with a score of " + scoresEntry.Key + ".\r\n";
                        }
                    }
                }
            }

            // individual position game
            else if (Globals_Game.gameType == 1)
            {
                if ((endDateReached) || (absoluteVictory))
                {
                    if (absoluteVictory)
                    {
                        gameResults += "\r\n\r\n";
                    }

                    gameResults += "The individual winners are ";
                    gameResults += Globals_Game.kingOne.firstName + " " + Globals_Game.kingOne.familyName + " (King of Kingdom One)";
                    gameResults += " and " + Globals_Game.kingTwo.firstName + " " + Globals_Game.kingTwo.familyName + " (King of Kingdom Two).";
                }
            }

            // team historical game
            else if (Globals_Game.gameType == 2)
            {
                if ((endDateReached) && (!absoluteVictory))
                {
                    victor = Globals_Game.CheckTeamHistoricalVictory();
                    gameResults += "The kingdom of " + victor.name + " under the valiant leadership of ";
                    gameResults += victor.owner.firstName + " " + victor.owner.familyName + " is victorious.";
                    if (victor.nationality.natID.Equals("Fr"))
                    {
                        gameResults += "  It has managed to eject the English from its sovereign territory.";
                    }
                    else if (victor.nationality.natID.Equals("Eng"))
                    {
                        gameResults += "  It has managed to retain control of at least one fief in French sovereign territory.";
                    }
                }
            }
            //TODO find way to send game results to clients
            // announce winners
            if ((endDateReached) || (absoluteVictory))
            {
                //send gameResults to client
                gameEnded = true;
            }

            return gameEnded;
        }

        /// <summary>
        /// Checks for a historical team victory (victory depends on whether the English own any French fiefs)
        /// </summary>
        /// <remarks>Very early test version - not properly functional</remarks>
        /// <returns>Kingdom object belonging to victor</returns>
        public static Kingdom CheckTeamHistoricalVictory()
        {
            Kingdom victor = null;

            // get France and England
            Kingdom france = Globals_Game.kingdomMasterList["Fr"];
            Kingdom england = Globals_Game.kingdomMasterList["Eng"];

            // set France as victor by default
            victor = france;

            // check each French fief for enemy occupation
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                if (fiefEntry.Value.GetRightfulKingdom() == france)
                {
                    if (fiefEntry.Value.GetCurrentKingdom() == england)
                    {
                        victor = england;
                    }
                }
            }

            return victor;
        }

        /// <summary>
        /// Checks for absolute victory (all fiefs owned by one kingdom)
        /// </summary>
        /// <returns>Kingdom object belonging to victor</returns>
        public static Kingdom CheckTeamAbsoluteVictory()
        {
            Kingdom victor = null;
            int fiefCount = 0;

            // iterate through kingdoms
            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                // reset fiefCount
                fiefCount = 0;

                // iterate through fiefs, checking if owned by this kingdom
                foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                {
                    if (fiefEntry.Value.GetCurrentKingdom() == kingdomEntry.Value)
                    {
                        // if owned by this kingdom, increment count
                        fiefCount++;
                    }
                }

                // check if kingdom owns all fiefs
                if (fiefCount == Globals_Game.fiefMasterList.Count)
                {
                    victor = kingdomEntry.Value;
                    break;
                }
            }

            return victor;
        }

        /// <summary>
        /// Iterates through the scheduledEvents journal, implementing the appropriate actions
        /// </summary>
        /// <returns>List<JournalEntry> containing journal entries to be removed</returns>
        public static List<JournalEntry> ProcessScheduledEvents()
        {
            List<JournalEntry> forRemoval = new List<JournalEntry>();
            bool proceed = true;

            // iterate through scheduled events
            foreach (KeyValuePair<uint, JournalEntry> jEntry in Globals_Game.scheduledEvents.entries)
            {
                proceed = true;

                if ((jEntry.Value.year == Globals_Game.clock.currentYear) && (jEntry.Value.season == Globals_Game.clock.currentSeason))
                {
                    //BIRTH
                    if ((jEntry.Value.type).ToLower().Equals("birth"))
                    {
                        // get parents
                        NonPlayerCharacter mummy = null;
                        Character daddy = null;
                        for (int i = 0; i < jEntry.Value.personae.Length; i++)
                        {
                            string thisPersonae = jEntry.Value.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');

                            if (thisPersonaeSplit.Length > 1)
                            {
                                switch (thisPersonaeSplit[1])
                                {
                                    case "mother":
                                        mummy = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                        break;
                                    case "father":
                                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                        {
                                            daddy = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                                        }
                                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                        {
                                            daddy = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        // do conditional checks
                        // death of mother or father
                        if ((!mummy.isAlive) || (!daddy.isAlive))
                        {
                            proceed = false;
                        }


                        if (proceed)
                        {
                            // run childbirth procedure
                            mummy.GiveBirth(daddy);
                        }

                        // add entry to list for removal
                        forRemoval.Add(jEntry.Value);
                    }

                    // MARRIAGE
                    else if ((jEntry.Value.type).ToLower().Equals("marriage"))
                    {
                        // get bride and groom
                        Character bride = null;
                        Character groom = null;

                        for (int i = 0; i < jEntry.Value.personae.Length; i++)
                        {
                            string thisPersonae = jEntry.Value.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');

                            switch (thisPersonaeSplit[1])
                            {
                                case "bride":
                                    bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                    break;
                                case "groom":
                                    if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                                    }
                                    else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        // CONDITIONAL CHECKS
                        // death of bride or groom
                        if ((!bride.isAlive) || (!groom.isAlive))
                        {
                            proceed = false;

                            // add entry to list for removal
                            forRemoval.Add(jEntry.Value);
                        }

                        // separated by siege
                        else
                        {
                            // if are in different fiefs OR in same fief but not both in keep
                            if ((bride.location != groom.location)
                                || ((bride.location == groom.location) && (bride.inKeep != groom.inKeep)))
                            {
                                // if there's a siege in the fief where the character is in the keep
                                if (((!String.IsNullOrWhiteSpace(bride.location.siege)) && (bride.inKeep))
                                    || ((!String.IsNullOrWhiteSpace(groom.location.siege)) && (groom.inKeep)))
                                {
                                    proceed = false;

                                    // postpone marriage until next season
                                    if (jEntry.Value.season == 3)
                                    {
                                        jEntry.Value.season = 0;
                                        jEntry.Value.year++;
                                    }
                                    else
                                    {
                                        jEntry.Value.season++;
                                    }
                                }
                            }
                        }

                        if (proceed)
                        {
                            // process marriage
                            jEntry.Value.ProcessMarriage();

                            // add entry to list for removal
                            forRemoval.Add(jEntry.Value);
                        }

                    }
                }
            }

            return forRemoval;

        }
        /// <summary>
        /// Sends an update to a particular user
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        public static void UpdatePlayer(string player, string message)
        {
            if(string.IsNullOrWhiteSpace(player)||(string.IsNullOrWhiteSpace(message))) {
                return;
            }
            // if a user is currently signed in send the message direct to client
            if (Globals_Server.clients.ContainsKey(player))
            {
                Globals_Server.clients[player].Update(message);
            }
            // if user is away store message in database to view when user next logs in
            else
            {
                //TODO user message log
            }
        }
        /// <summary>
        /// Adds an observer (Client object) to the list of registered observers
        /// </summary>
        /// <param name="obs">Observer to be added</param>
        public static void RegisterObserver(Client obs)
        {
            // add new observer to list
            registeredObservers.Add(obs);
        }

        /// <summary>
        /// Removes an observer (Form1 object) from the list of registered observers
        /// </summary>
        /// <param name="obs">Observer to be removed</param>
        public static void RemoveObserver(Client obs)
        {
            // remove observer from list
            registeredObservers.Remove(obs);
        }

        //TODO possibly move to Game, send updated globals
        /// <summary>
        /// Notifies all observers (Form1 objects) in the list of registered observers
        /// that a change has occured to the data
        /// </summary>
        /// <param name="info">String indicating the nature of the change</param>
        public static void NotifyObservers(String info)
        {
            // iterate through list of observers
            foreach (Client obs in registeredObservers)
            {
                // call observer's update method to perform the required actions
                // based on the string passed
                obs.Update(info);
            }
        }
    }

    /// <summary>
    /// Class storing data on which to base individual victory
    /// </summary>
    public class VictoryData
    {
        /// <summary>
        /// Holds player ID
        /// </summary>
        public string playerID;
        /// <summary>
        /// Holds PlayerCharacter ID
        /// </summary>
        public string playerCharacterID;
        /// <summary>
        /// Holds player's stature at start of game
        /// </summary>
        public double startStature;
        /// <summary>
        /// Holds player's current stature
        /// </summary>
        public double currentStature;
        /// <summary>
        /// Holds percentage of population under player's control at start of game
        /// </summary>
        public double startPopulation;
        /// <summary>
        /// Holds percentage of population currently under player's control
        /// </summary>
        public double currentPopulation;
        /// <summary>
        /// Holds percentage of fiefs under player's control at start of game
        /// </summary>
        public double startFiefs;
        /// <summary>
        /// Holds percentage of fiefs currently under player's control
        /// </summary>
        public double currentFiefs;
        /// <summary>
        /// Holds the percentage of total funds owned by the player at start of game
        /// </summary>
        public double startMoney;
        /// <summary>
        /// Holds the percentage of total funds currently owned by the player
        /// </summary>
        public double currentMoney;

        /// <summary>
        /// Constructor for VictoryData
        /// </summary>
        /// <param name="player">string holding Language ID</param>
        /// <param name="pc">string holding PlayerCharacter ID</param>
        /// <param name="stat">double player's stature at start of game</param>
        /// <param name="pop">double holding percentage of population under player's control at start of game</param>
        /// <param name="fiefs">double holding percentage of fiefs under player's control at start of game</param>
        /// <param name="money">double holding the percentage of total funds owned by the player at start of game</param>
        public VictoryData(string player, string pc, double stat, double pop, double fiefs, double money)
        {
            // VALIDATION

            // PC
            // trim and ensure 1st is uppercase
            pc = Utility_Methods.FirstCharToUpper(pc.Trim());

            if (!Utility_Methods.ValidateCharacterID(pc))
            {
                throw new InvalidDataException("VictoryData playerCharacterID must have the format 'Char_' followed by some numbers");
            }

            // STAT
            if (!Utility_Methods.ValidateCharacterStat(stat, 0))
            {
                throw new InvalidDataException("VictoryData startStature must be a double between 0-9");
            }

            // POP
            if (!Utility_Methods.ValidatePercentage(pop))
            {
                throw new InvalidDataException("VictoryData startPopulation must be a double between 0 and 100");
            }

            // FIEFS
            if (!Utility_Methods.ValidatePercentage(fiefs))
            {
                throw new InvalidDataException("VictoryData startFiefs must be a double between 0 and 100");
            }

            // MONEY
            if (!Utility_Methods.ValidatePercentage(money))
            {
                throw new InvalidDataException("VictoryData startMoney must be a double between 0 and 100");
            }

            this.playerID = player;
            this.playerCharacterID = pc;
            this.startStature = stat;
            this.currentStature = 0;
            this.startPopulation = pop;
            this.currentPopulation = 0;
            this.startFiefs = fiefs;
            this.currentFiefs = 0;
            this.startMoney = money;
            this.currentMoney = 0;
        }

        /// <summary>
        /// Constructor for VictoryData taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public VictoryData()
        {
        }

        /// <summary>
        /// Updates the current data
        /// </summary>
        public void UpdateData()
        {
            // get PlayerCharacter
            PlayerCharacter thisPC = null;
            if (Globals_Game.pcMasterList.ContainsKey(this.playerCharacterID))
            {
                thisPC = Globals_Game.pcMasterList[this.playerCharacterID];
            }

            // update data
            if (thisPC != null)
            {
                // stature
                this.currentStature = thisPC.CalculateStature();

                // population governed
                this.currentPopulation = thisPC.GetPopulationPercentage();

                // fiefs owned
                this.currentFiefs = thisPC.GetFiefsPercentage();

                // money
                this.currentMoney = thisPC.GetMoneyPercentage();
            }
        }
        
        /// <summary>
        /// Calculates the current stature score
        /// </summary>
        /// <returns>double containing the stature score</returns>
        public double CalcStatureScore()
        {
            double statScore = 0;

            statScore = this.currentStature + (this.currentStature - this.startStature);

            return statScore;
        }

        /// <summary>
        /// Calculates the current population  score
        /// </summary>
        /// <returns>double containing the population score</returns>
        public double CalcPopulationScore()
        {
            double popScore = 0;

            popScore = (this.currentPopulation + (this.currentPopulation - this.startPopulation)) / 10;

            return popScore;
        }

        /// <summary>
        /// Calculates the current fief score
        /// </summary>
        /// <returns>double containing the fief score</returns>
        public double CalcFiefScore()
        {
            double fiefScore = 0;

            fiefScore = (this.currentFiefs + (this.currentFiefs - this.startFiefs)) / 10;

            return fiefScore;
        }

        /// <summary>
        /// Calculates the current money score
        /// </summary>
        /// <returns>double containing the money score</returns>
        public double CalcMoneyScore()
        {
            double moneyScore = 0;

            moneyScore = (this.currentMoney + (this.currentMoney - this.startMoney)) / 10;

            return moneyScore;
        }
    }

    /// <summary>
    /// Struct storing data on ownership challenges (for Province or Kingdom)
    /// </summary>
    public class OwnershipChallenge
    {
        /// <summary>
        /// Holds ID of challenge
        /// </summary>
        public string id;
        /// <summary>
        /// Holds ID of challenger
        /// </summary>
        public string challengerID;
        /// <summary>
        /// Holds type of place
        /// </summary>
        public string placeType;
        /// <summary>
        /// Holds ID of place
        /// </summary>
        public string placeID;
        /// <summary>
        /// Holds number of seasons so far that challenger has met ownership conditions
        /// </summary>
        public int counter;

        /// <summary>
        /// Constructor for OwnershipChallenge
        /// </summary>
        /// <param name="chalID">string holding challenge ID</param>
        /// <param name="chID">string holding ID of challenger</param>
        /// <param name="type">string holding type of place</param>
        /// <param name="place">string holding ID of place</param>
        public OwnershipChallenge(string chalID, string chID, string type, string place)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            chalID = Utility_Methods.FirstCharToUpper(chalID.Trim());

            if (!Utility_Methods.ValidateChallengeID(chalID))
            {
                throw new InvalidDataException("OwnershipChallenge id must have the format 'Challenge_' followed by some numbers");
            }

            // CHID
            // trim and ensure 1st is uppercase
            chID = Utility_Methods.FirstCharToUpper(chID.Trim());

            if (!Utility_Methods.ValidateCharacterID(chID))
            {
                throw new InvalidDataException("OwnershipChallenge challenger id must have the format 'Char_' followed by some numbers");
            }

            // TYPE
            // trim and convert to lowercase
            type = type.Trim().ToLower();

            if ((!type.Equals("province")) && (!type.Equals("kingdom")))
            {
                throw new InvalidDataException("OwnershipChallenge type must be either 'province' or 'kingdom'");
            }

            // PLACE
            // trim and ensure is uppercase
            place = place.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(place))
            {
                throw new InvalidDataException("OwnershipChallenge place id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            this.id = chalID;
            this.challengerID = chID;
            this.placeType = type;
            this.placeID = place;
            this.counter = 0;
        }

        /// <summary>
        /// Gets the PlayerCharacter who has issued the challenge
        /// </summary>
        /// <returns>The challenger (PlayerCharacter)</returns>
        public PlayerCharacter GetChallenger()
        {
            PlayerCharacter challenger = null;

            if (!String.IsNullOrWhiteSpace(this.challengerID))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.challengerID))
                {
                    challenger = Globals_Game.pcMasterList[this.challengerID];
                }
            }

            return challenger;
        }

        /// <summary>
        /// Gets the Place being contested
        /// </summary>
        /// <returns>The Place</returns>
        public Place GetPlace()
        {
            Place contestedPlace = null;

            if (!String.IsNullOrWhiteSpace(this.placeID))
            {
                // get province
                if (this.placeType.Equals("province"))
                {
                    if (Globals_Game.provinceMasterList.ContainsKey(this.placeID))
                    {
                        contestedPlace = Globals_Game.provinceMasterList[this.placeID];
                    }
                }

                // get kingdom
                else if (this.placeType.Equals("kingdom"))
                {
                    if (Globals_Game.kingdomMasterList.ContainsKey(this.placeID))
                    {
                        contestedPlace = Globals_Game.kingdomMasterList[this.placeID];
                    }
                }
            }

            return contestedPlace;
        }

        /// <summary>
        /// Increments the season counter
        /// </summary>
        public void IncrementCounter()
        {
            this.counter++;
        }
    }

}
