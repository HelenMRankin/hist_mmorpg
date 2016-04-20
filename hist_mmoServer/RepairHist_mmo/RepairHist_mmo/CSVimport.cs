using System;
using System.Collections.Generic;
using System.IO;
using QuickGraph;
using System.Diagnostics;
//TODO Modify to use CSV file
namespace hist_mmorpg
{
    public static class CSVimport
    {
        /// <summary>
        /// Creates game objects necessary for a new game, mainly using data imported from a CSV file
        /// </summary>
        /// <param name="objectDataFile">Name of file containing game object CSV data</param>
        /// <param name="mapDataFile">Name of file containing map CSV data</param>
        public static void NewGameFromCSV(string objectDataFile, string mapDataFile, uint start = 1337)
        {
            // create GameClock
            Globals_Game.clock = new GameClock("clock_1", start);

            // load game objects from CSV
            CSVimport.ImportFromCSV(objectDataFile, synch: true, toDatabase: false);

            // load/create map from CSV
            CSVimport.CreateMapEdgesFromCSV(mapDataFile, toDatabase: false);

            // POPULATE GLOBALS
            // populate Globals_Server.gameTypes
            Globals_Server.gameTypes.Add(0, "Individual points");
            Globals_Server.gameTypes.Add(1, "Individual position");
            Globals_Server.gameTypes.Add(2, "Team historical");

            // populate Globals_Server.combatValues
            uint[] eCombatValues = new uint[] { 9, 9, 1, 9, 5, 3, 1 };
            Globals_Server.combatValues.Add("Eng", eCombatValues);
            uint[] fCombatValues = new uint[] { 7, 7, 3, 2, 4, 2, 1 };
            Globals_Server.combatValues.Add("Fr", fCombatValues);
            uint[] sCombatValues = new uint[] { 8, 8, 1, 2, 4, 4, 1 };
            Globals_Server.combatValues.Add("Sco", sCombatValues);
            uint[] oCombatValues = new uint[] { 7, 7, 3, 2, 4, 2, 1 };
            Globals_Server.combatValues.Add("Oth", oCombatValues);

            // populate Globals_Server.recruitRatios
            double[] eRecruitRatios = new double[] { 0.01, 0.02, 0, 0.12, 0.03, 0.32, 0.49 };
            Globals_Server.recruitRatios.Add("Eng", eRecruitRatios);
            double[] fRecruitRatios = new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 };
            Globals_Server.recruitRatios.Add("Fr", fRecruitRatios);
            double[] sRecruitRatios = new double[] { 0.01, 0.02, 0, 0, 0.04, 0.43, 0.49 };
            Globals_Server.recruitRatios.Add("Sco", sRecruitRatios);
            double[] oRecruitRatios = new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 };
            Globals_Server.recruitRatios.Add("Oth", oRecruitRatios);

            // populate Globals_Server.battleProbabilities
            double[] odds = new double[] { 2, 3, 4, 5, 6, 99 };
            Globals_Server.battleProbabilities.Add("odds", odds);
            double[] bChance = new double[] { 10, 30, 50, 70, 80, 90 };
            Globals_Server.battleProbabilities.Add("battle", bChance);
            double[] pChance = new double[] { 10, 20, 30, 40, 50, 60 };
            Globals_Server.battleProbabilities.Add("pillage", pChance);

            // populate Globals_Server.troopTypeAdvantages
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(0, 5), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(0, 6), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(1, 5), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(1, 6), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(2, 3), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(2, 4), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(3, 5), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(3, 6), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(4, 0), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(4, 1), 3);
            Globals_Server.troopTypeAdvantages.Add(new Tuple<uint, uint>(5, 2), 3);

            // populate Globals_Game.jEntryPriorities
            // marriage
            string[] thisPriorityKey001 = { "proposalMade", "headOfFamilyBride" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey001, 2);
            string[] thisPriorityKey002 = { "proposalRejected", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey002, 2);
            string[] thisPriorityKey003 = { "proposalAccepted", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey003, 2);
            string[] thisPriorityKey004 = { "marriage", "headOfFamilyBride" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey004, 1);
            string[] thisPriorityKey005 = { "marriage", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey005, 1);
            string[] thisPriorityKey016 = { "marriageCancelled", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey016, 2);
            string[] thisPriorityKey017 = { "marriageCancelled", "headOfFamilyBride" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey017, 1);
            // birth
            string[] thisPriorityKey006 = { "birth", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey006, 2);
            string[] thisPriorityKey007 = { "birth", "father" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey007, 2);
            // battle
            string[] thisPriorityKey008 = { "battle", "defenderOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey008, 2);
            string[] thisPriorityKey009 = { "battle", "sallyOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey009, 2);
            string[] thisPriorityKey010 = { "battle", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey010, 1);
            // siege
            string[] thisPriorityKey011 = { "siege", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey011, 2);
            string[] thisPriorityKey012 = { "siegeReduction", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey012, 1);
            string[] thisPriorityKey013 = { "siegeStorm", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey013, 1);
            string[] thisPriorityKey014 = { "siegeNegotiation", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey014, 1);
            string[] thisPriorityKey015 = { "siegeEnd", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey015, 2);
            // death
            string[] thisPriorityKey018 = { "deathOfHeir", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey018, 2);
            string[] thisPriorityKey019 = { "deathOfFamilyMember", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey019, 1);
            string[] thisPriorityKey020 = { "deathOfEmployee", "employer" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey020, 1);
            string[] thisPriorityKey021 = { "deathOfPlayer", "newHeadOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey021, 2);
            string[] thisPriorityKey022 = { "deathOfPlayer", "deceasedHeadOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey022, 2);
            // injury
            string[] thisPriorityKey023 = { "injury", "employer" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey023, 1);
            string[] thisPriorityKey024 = { "injury", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey024, 1);
            string[] thisPriorityKey025 = { "injury", "injuredCharacter" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey025, 2);
            // pillage
            string[] thisPriorityKey026 = { "pillage", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey026, 2);
            // fief status change
            string[] thisPriorityKey027 = { "fiefStatusUnrest", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey027, 1);
            string[] thisPriorityKey028 = { "fiefStatusRebellion", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey028, 2);
            // quelling rebellion
            string[] thisPriorityKey029 = { "rebellionQuelled", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey029, 1);
            string[] thisPriorityKey030 = { "rebellionQuellFailed", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey030, 1);
            // fief/province/position title holder change
            string[] thisPriorityKey031 = { "grantTitleFief", "newTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey031, 1);
            string[] thisPriorityKey032 = { "grantTitleFief", "oldTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey032, 1);
            string[] thisPriorityKey033 = { "grantTitleProvince", "newTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey033, 1);
            string[] thisPriorityKey034 = { "grantTitleProvince", "oldTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey034, 1);
            string[] thisPriorityKey035 = { "grantPosition", "newPositionHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey035, 1);
            string[] thisPriorityKey036 = { "grantPosition", "oldPositionHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey036, 1);
            // fief change of ownership
            string[] thisPriorityKey037 = { "fiefOwnership_Hostile", "oldOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey037, 2);
            string[] thisPriorityKey038 = { "fiefOwnership_Gift", "newOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey038, 2);
            // ownership/kingship challenges
            string[] thisPriorityKey039 = { "ownershipChallenge_new", "owner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey039, 2);
            string[] thisPriorityKey040 = { "ownershipChallenge_success", "newOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey040, 2);
            string[] thisPriorityKey041 = { "ownershipChallenge_success", "oldOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey041, 2);
            string[] thisPriorityKey042 = { "ownershipChallenge_failure", "owner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey042, 2);
            string[] thisPriorityKey043 = { "ownershipChallenge_failure", "challenger" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey043, 2);
            string[] thisPriorityKey044 = { "depose_success", "newKing" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey044, 2);
            string[] thisPriorityKey045 = { "depose_success", "oldKing" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey045, 2);
            string[] thisPriorityKey049 = { "depose_success", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey049, 2);
            string[] thisPriorityKey046 = { "depose_failure", "king" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey046, 2);
            string[] thisPriorityKey047 = { "depose_failure", "pretender" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey047, 2);
            string[] thisPriorityKey050 = { "depose_failure", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey050, 1);
            string[] thisPriorityKey048 = { "depose_new", "king" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey048, 2);
            string[] thisPriorityKey051 = { "depose_new", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey051, 2);
        }

        /// <summary>
        /// Creates game objects using data imported from a CSV file and writes them to the database
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        /// <param name="synch">bool indicating whether or not to synch the game objects' properties</param>
        /// <param name="toDatabase">bool indicating whether or not to save the game objects to database or game</param>
        public static bool ImportFromCSV(string filename, string bucketID = null, bool synch = false, bool toDatabase = true)
        {
            bool inputFileError = false;
            string lineIn;
            string[] lineParts;
            int lineNum = 0;
            StreamReader srObjects = null;

            // list for storing object keys
            List<string> fiefKeyList = new List<string>();
            List<string> provKeyList = new List<string>();
            List<string> kingKeyList = new List<string>();
            List<string> pcKeyList = new List<string>();
            List<string> npcKeyList = new List<string>();
            List<string> traitKeyList = new List<string>();
            List<string> armyKeyList = new List<string>();
            List<string> langKeyList = new List<string>();
            List<string> baseLangKeyList = new List<string>();
            List<string> natKeyList = new List<string>();
            List<byte> rankKeyList = new List<byte>();
            List<byte> posKeyList = new List<byte>();
            List<string> siegeKeyList = new List<string>();
            List<string> terrKeyList = new List<string>();

            // dictionaries for temporary storage of objects
            Dictionary<string, Fief_Serialised> fiefMasterList = new Dictionary<string, Fief_Serialised>();
            Dictionary<string, PlayerCharacter_Serialised> pcMasterList = new Dictionary<string, PlayerCharacter_Serialised>();
            Dictionary<string, NonPlayerCharacter_Serialised> npcMasterList = new Dictionary<string, NonPlayerCharacter_Serialised>();
            Dictionary<string, Province_Serialised> provinceMasterList = new Dictionary<string, Province_Serialised>();
            Dictionary<string, Kingdom_Serialised> kingdomMasterList = new Dictionary<string, Kingdom_Serialised>();
            Dictionary<string, Siege> siegeMasterList = new Dictionary<string, Siege>();
            Dictionary<string, Army> armyMasterList = new Dictionary<string, Army>();
            Dictionary<string, BaseLanguage> baseLanguageMasterList = new Dictionary<string, BaseLanguage>();
            Dictionary<string, Language_Serialised> languageMasterList = new Dictionary<string, Language_Serialised>();
            Dictionary<string, Trait> traitMasterList = new Dictionary<string, Trait>();
            Dictionary<string, Nationality> nationalityMasterList = new Dictionary<string, Nationality>();
            Dictionary<byte, Rank> rankMasterList = new Dictionary<byte, Rank>();
            Dictionary<byte, Position_Serialised> positionMasterList = new Dictionary<byte, Position_Serialised>();
            Dictionary<string, Terrain> terrainMasterList = new Dictionary<string, Terrain>();

            try
            {
                // opens StreamReader to read in  data from csv file
                srObjects = new StreamReader(filename);
            }
            // catch following IO exceptions that could be thrown by the StreamReader 
            catch (FileNotFoundException fnfe)
            {
                Globals_Server.logError(fnfe.Message);
                inputFileError = true;
            }
            catch (IOException ioe)
            {
                inputFileError = true;
                Globals_Server.logError(ioe.Message);
            }

            // while there is data in the line
            while ((lineIn = srObjects.ReadLine()) != null)
            {
                // increment lineNum
                lineNum++;

                // put the contents of the line into lineParts array, splitting on (char)9 (TAB)
                lineParts = lineIn.Split(',');

                try
                {

                    if (lineParts[0].Equals("fief"))
                    {
                        Fief_Serialised thisFiefSer = null;
                        thisFiefSer = CSVimport.ImportFromCSV_Fief(lineParts, lineNum);

                        if (thisFiefSer != null)
                        {
                            // add object to masterList
                            fiefMasterList.Add(thisFiefSer.id, thisFiefSer);

                            // add key to keylist
                            fiefKeyList.Add(thisFiefSer.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                           /* if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Fief object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("province"))
                    {
                        Province_Serialised thisProvSer = null;
                        thisProvSer = CSVimport.ImportFromCSV_Prov(lineParts, lineNum);

                        if (thisProvSer != null)
                        {
                            // add object to masterList
                            provinceMasterList.Add(thisProvSer.id, thisProvSer);

                            // add key to keylist
                            provKeyList.Add(thisProvSer.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Province object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("kingdom"))
                    {
                        Kingdom_Serialised thisKingSer = null;
                        thisKingSer = CSVimport.ImportFromCSV_Kingdom(lineParts, lineNum);

                        if (thisKingSer != null)
                        {
                            // add object to masterList
                            kingdomMasterList.Add(thisKingSer.id, thisKingSer);

                            // add key to keylist
                            kingKeyList.Add(thisKingSer.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Kingdom object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("pc"))
                    {
                        PlayerCharacter_Serialised thisPcSer = null;
                        thisPcSer = CSVimport.ImportFromCSV_PC(lineParts, lineNum);

                        if (thisPcSer != null)
                        {
                            // add object to masterList
                            pcMasterList.Add(thisPcSer.charID, thisPcSer);

                            // add key to keylist
                            pcKeyList.Add(thisPcSer.charID);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create PlayerCharacter object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("npc"))
                    {
                        NonPlayerCharacter_Serialised thisNpcSer = null;
                        thisNpcSer = CSVimport.importFromCSV_NPC(lineParts, lineNum);

                        if (thisNpcSer != null)
                        {
                            // add object to masterList
                            npcMasterList.Add(thisNpcSer.charID, thisNpcSer);

                            // add key to keylist
                            npcKeyList.Add(thisNpcSer.charID);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create NonPlayerCharacter object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("trait"))
                    {
                        Trait thisTrait = null;

                        thisTrait = CSVimport.ImportFromCSV_Trait(lineParts, lineNum);

                        if (thisTrait != null)
                        {
                            // add object to masterList
                            traitMasterList.Add(thisTrait.id, thisTrait);

                            // add key to keylist
                            traitKeyList.Add(thisTrait.id);
                        }
                        else
                        {
                            inputFileError = true;

                            Globals_Server.logError("Unable to create Trait object: " + lineParts[1]);
                        }
                    }

                    else if (lineParts[0].Equals("army"))
                    {
                        Army thisArmy = null;
                        thisArmy = CSVimport.ImportFromCSV_Army(lineParts, lineNum);

                        if (thisArmy != null)
                        {
                            // add object to masterList
                            armyMasterList.Add(thisArmy.armyID, thisArmy);

                            // add key to keylist
                            armyKeyList.Add(thisArmy.armyID);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Army object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("language"))
                    {
                        Language_Serialised thisLangSer = null;
                        thisLangSer = CSVimport.ImportFromCSV_Language(lineParts, lineNum);

                        if (thisLangSer != null)
                        {
                            // add object to masterList
                            languageMasterList.Add(thisLangSer.id, thisLangSer);

                            // add key to keylist
                            langKeyList.Add(thisLangSer.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Language object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("baseLanguage"))
                    {
                        BaseLanguage thisBaseLang = null;
                        thisBaseLang = CSVimport.ImportFromCSV_BaseLanguage(lineParts, lineNum);

                        if (thisBaseLang != null)
                        {
                            // add object to masterList
                            baseLanguageMasterList.Add(thisBaseLang.id, thisBaseLang);

                            // add key to keylist
                            baseLangKeyList.Add(thisBaseLang.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create BaseLanguage object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("nationality"))
                    {
                        Nationality thisNat = null;
                        thisNat = CSVimport.ImportFromCSV_Nationality(lineParts, lineNum);

                        if (thisNat != null)
                        {
                            // add object to masterList
                            nationalityMasterList.Add(thisNat.natID, thisNat);

                            // add key to keylist
                            natKeyList.Add(thisNat.natID);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Nationality object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("rank"))
                    {
                        Rank thisRank = null;

                        thisRank = CSVimport.ImportFromCSV_Rank(lineParts, lineNum);

                        if (thisRank != null)
                        {
                            // add object to masterList
                            rankMasterList.Add(thisRank.id, thisRank);

                            // add key to keylist
                            rankKeyList.Add(thisRank.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling#
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Rank object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("position"))
                    {
                        Position_Serialised thisPosSer = null;
                        thisPosSer = CSVimport.ImportFromCSV_Position(lineParts, lineNum);

                        if (thisPosSer != null)
                        {
                            // add object to masterList
                            positionMasterList.Add(thisPosSer.id, thisPosSer);

                            // add key to keylist
                            posKeyList.Add(thisPosSer.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Position object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("siege"))
                    {
                        Siege thisSiege = null;
                        thisSiege = CSVimport.ImportFromCSV_Siege(lineParts, lineNum);

                        if (thisSiege != null)
                        {
                            // add object to masterList
                            siegeMasterList.Add(thisSiege.siegeID, thisSiege);

                            // add key to keylist
                            siegeKeyList.Add(thisSiege.siegeID);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Siege object: " + lineParts[1]);
                            }*/
                        }
                    }

                    else if (lineParts[0].Equals("terrain"))
                    {
                        Terrain thisTerr = null;
                        thisTerr = CSVimport.ImportFromCSV_Terrain(lineParts, lineNum);

                        if (thisTerr != null)
                        {
                            // add object to masterList
                            terrainMasterList.Add(thisTerr.id, thisTerr);

                            // add key to keylist
                            terrKeyList.Add(thisTerr.id);
                        }
                        else
                        {
                            inputFileError = true;
                            //TODO error handling
                            /*
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Terrain object: " + lineParts[1]);
                            }*/
                        }
                    }

                    // non-recognised object prefix
                    else
                    {
                        throw new InvalidDataException("Object prefix not recognised");
                    }
                }
                // catch exception that could be thrown by an invalid object prefix
                catch (InvalidDataException ide)
                {
                    // create and add sysAdmin JournalEntry
                    JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                    if (importErrorEntry != null)
                    {
                        Trace.WriteLine("Line " + lineNum + ": " + ide.Message);
                        importErrorEntry.entryDetails.Message= "Line " + lineNum + ": " + ide.Message;
                        Globals_Game.AddPastEvent(importErrorEntry);
                    }
                    //TODO error handling
                    /*
                    if (Globals_Client.showDebugMessages)
                    {
                        MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                    }*/
                }
            }

            // SAVE KEYLISTS
            // fiefs
            if (fiefKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "fiefKeys", fiefKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.fiefKeys = fiefKeyList;
                }
            }

            // provinces
            if (provKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "provKeys", provKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.provKeys = provKeyList;
                }
            }

            // kingdoms
            if (kingKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "kingKeys", kingKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.kingKeys = kingKeyList;
                }
            }

            // PCs
            if (pcKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "pcKeys", pcKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.pcKeys = pcKeyList;
                }
            }

            // NPCs
            if (npcKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "npcKeys", npcKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.npcKeys = npcKeyList;
                }
            }

            // traits
            if (traitKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "traitKeys", traitKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.traitKeys = traitKeyList;
                }
            }

            // armies
            if (armyKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "armyKeys", armyKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.armyKeys = armyKeyList;
                }
            }

            // languages
            if (langKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "langKeys", langKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.langKeys = langKeyList;
                }
            }

            // baseLanguages
            if (baseLangKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "baseLangKeys", baseLangKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.baseLangKeys = baseLangKeyList;
                }
            }

            // nationalities
            if (natKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "nationalityKeys", natKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.nationalityKeys = natKeyList;
                }
            }

            // ranks
            if (rankKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "rankKeys", rankKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.rankKeys = rankKeyList;
                }
            }

            // positions
            if (posKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "positionKeys", posKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.positionKeys = posKeyList;
                }
            }

            // sieges
            if (siegeKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "siegeKeys", siegeKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.siegeKeys = siegeKeyList;
                }
            }

            // terrains
            if (terrKeyList.Count > 0)
            {
                if (toDatabase)
                {
                    // save keylist to database
                    DatabaseWrite.DatabaseWrite_KeyList(bucketID, "terrKeys", terrKeyList);
                }

                else
                {
                    // save to game
                    Globals_Game.terrKeys = terrKeyList;
                }
            }

            // SAVE OBJECTS NOT NEEDING SYNCHING
            // trait
            if (traitMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Trait> thisEntry in traitMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Trait(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.traitMasterList = traitMasterList;
                }
            }

            // baseLanguage
            if (baseLanguageMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, BaseLanguage> thisEntry in baseLanguageMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_BaseLanguage(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.baseLanguageMasterList = baseLanguageMasterList;
                }
            }

            // nationality
            if (nationalityMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Nationality> thisEntry in nationalityMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Nationality(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.nationalityMasterList = nationalityMasterList;
                }
            }

            // rank
            if (rankMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<byte, Rank> thisEntry in rankMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Rank(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.rankMasterList = rankMasterList;
                }
            }

            // terrain
            if (terrainMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Terrain> thisEntry in terrainMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Terrain(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.terrainMasterList = terrainMasterList;
                }
            }

            // langauge
            if (languageMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Language_Serialised> thisEntry in languageMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Language(bucketID, ls: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Language newObject = new Language();
                    foreach (KeyValuePair<string, Language_Serialised> thisEntry in languageMasterList)
                    {
                        // de-serialise 
                        newObject = DatabaseRead.Language_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.languageMasterList.Add(newObject.id, newObject);
                    }
                }
            }

            // position
            if (positionMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<byte, Position_Serialised> thisEntry in positionMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Position(bucketID, ps: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Position newObject = new Position();
                    foreach (KeyValuePair<byte, Position_Serialised> thisEntry in positionMasterList)
                    {
                        // de-serialise 
                        newObject = DatabaseRead.Position_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.positionMasterList.Add(newObject.id, newObject);
                    }
                }
            }

            // CHECK FOR OBJECTS NEEDING SYNCHING
            // if synching, pass remaining objects to SynchGameObjectCollections
            if (synch)
            {
                // pass objects for resynching/saving
                CSVimport.SynchGameObjectCollections(fiefMasterList, pcMasterList, npcMasterList, provinceMasterList,
                    kingdomMasterList, siegeMasterList, armyMasterList, bucketID, toDatabase);
            }

            // if not synching, save remaining objects
            else
            {
                // army
                if (armyMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, Army> thisEntry in armyMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_Army(bucketID, thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        Globals_Game.armyMasterList = armyMasterList;
                    }
                }

                // siege
                if (siegeMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, Siege> thisEntry in siegeMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_Siege(bucketID, thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        Globals_Game.siegeMasterList = siegeMasterList;
                    }
                }

                // NPC
                if (npcMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> thisEntry in npcMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_NPC(bucketID, npcs: thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        NonPlayerCharacter newObject = new NonPlayerCharacter();
                        foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> thisEntry in npcMasterList)
                        {
                            // de-serialise 
                            newObject = DatabaseRead.NPC_deserialise(thisEntry.Value);

                            // add to Globals_Game masterList
                            Globals_Game.npcMasterList.Add(newObject.charID, newObject);
                        }
                    }
                }

                // PC
                if (pcMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, PlayerCharacter_Serialised> thisEntry in pcMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_PC(bucketID, pcs: thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        PlayerCharacter newObject = new PlayerCharacter();
                        foreach (KeyValuePair<string, PlayerCharacter_Serialised> thisEntry in pcMasterList)
                        {
                            // de-serialise 
                            newObject = DatabaseRead.PC_deserialise(thisEntry.Value);

                            // add to Globals_Game masterList
                            Globals_Game.pcMasterList.Add(newObject.charID, newObject);
                        }
                    }
                }

                // kingdom
                if (kingdomMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, Kingdom_Serialised> thisEntry in kingdomMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_Kingdom(bucketID, ks: thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        Kingdom newObject = new Kingdom();
                        foreach (KeyValuePair<string, Kingdom_Serialised> thisEntry in kingdomMasterList)
                        {
                            // de-serialise 
                            newObject = DatabaseRead.Kingdom_deserialise(thisEntry.Value);

                            // add to Globals_Game masterList
                            Globals_Game.kingdomMasterList.Add(newObject.id, newObject);
                        }
                    }
                }

                // province
                if (provinceMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, Province_Serialised> thisEntry in provinceMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_Province(bucketID, ps: thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        Province newObject = new Province();
                        foreach (KeyValuePair<string, Province_Serialised> thisEntry in provinceMasterList)
                        {
                            // de-serialise 
                            newObject = DatabaseRead.Province_deserialise(thisEntry.Value);

                            // add to Globals_Game masterList
                            Globals_Game.provinceMasterList.Add(newObject.id, newObject);
                        }
                    }
                }

                // fief
                if (fiefMasterList.Count > 0)
                {
                    // save to database
                    if (toDatabase)
                    {
                        foreach (KeyValuePair<string, Fief_Serialised> thisEntry in fiefMasterList)
                        {
                            DatabaseWrite.DatabaseWrite_Fief(bucketID, fs: thisEntry.Value);
                        }
                    }
                    // save to game
                    else
                    {
                        Fief newObject = new Fief();
                        foreach (KeyValuePair<string, Fief_Serialised> thisEntry in fiefMasterList)
                        {
                            // de-serialise 
                            newObject = DatabaseRead.Fief_deserialise(thisEntry.Value);

                            // add to Globals_Game masterList
                            Globals_Game.fiefMasterList.Add(newObject.id, newObject);
                        }
                    }
                }
            }

            return inputFileError;
        }

        /// <summary>
        /// Creates a Fief_Serialised object using data in a string array
        /// </summary>
        /// <returns>Fief_Serialised object</returns>
        /// <param name="fiefData">string[] holding source data</param>
        public static Fief_Serialised ImportFromCSV_Fief(string[] fiefData, int lineNum)
        {
            Fief_Serialised thisFiefSer = null;

            try
            {
                // create empty lists for variable length collections
                // (characters, barredChars, armies)
                List<string> characters = new List<string>();
                List<string> barredChars = new List<string>();
                List<string> barredNats = new List<string>();
                List<string> armies = new List<string>();

                // check to see if any data present for variable length collections
                if (fiefData.Length > 57)
                {
                    // create variables to hold start/end index positions
                    int chStart, chEnd, barChStart, barChEnd, barNatStart, barNatEnd, arStart, arEnd;
                    chStart = chEnd = barChStart = barChEnd = barNatStart = barNatEnd = arStart = arEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 57; i < fiefData.Length; i++)
                    {
                        if (fiefData[i].Equals("chStart"))
                        {
                            chStart = i;
                        }
                        else if (fiefData[i].Equals("chEnd"))
                        {
                            chEnd = i;
                        }
                        else if (fiefData[i].Equals("barChStart"))
                        {
                            barChStart = i;
                        }
                        else if (fiefData[i].Equals("barChEnd"))
                        {
                            barChEnd = i;
                        }
                        else if (fiefData[i].Equals("barNatStart"))
                        {
                            barNatStart = i;
                        }
                        else if (fiefData[i].Equals("barNatEnd"))
                        {
                            barNatEnd = i;
                        }
                        else if (fiefData[i].Equals("arStart"))
                        {
                            arStart = i;
                        }
                        else if (fiefData[i].Equals("arEnd"))
                        {
                            arEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // characters
                    if ((chStart > -1) && (chEnd > -1))
                    {
                        for (int i = chStart + 1; i < chEnd; i++)
                        {
                            characters.Add(fiefData[i]);
                        }
                    }

                    // barredChars
                    if ((barChStart > -1) && (barChEnd > -1))
                    {
                        for (int i = barChStart + 1; i < barChEnd; i++)
                        {
                            barredChars.Add(fiefData[i]);
                        }
                    }

                    // barredNats
                    if ((barNatStart > -1) && (barNatEnd > -1))
                    {
                        for (int i = barNatStart + 1; i < barNatEnd; i++)
                        {
                            barredNats.Add(fiefData[i]);
                        }
                    }

                    // armies
                    if ((arStart > -1) && (arEnd > -1))
                    {
                        for (int i = arStart + 1; i < arEnd; i++)
                        {
                            armies.Add(fiefData[i]);
                        }
                    }
                }

                // create financial data arrays
                // current
                double[] finCurr = new double[] { Convert.ToDouble(fiefData[14]), Convert.ToDouble(fiefData[15]),
                    Convert.ToDouble(fiefData[16]), Convert.ToDouble(fiefData[17]), Convert.ToDouble(fiefData[18]),
                    Convert.ToDouble(fiefData[19]), Convert.ToDouble(fiefData[20]), Convert.ToDouble(fiefData[21]),
               Convert.ToDouble(fiefData[22]), Convert.ToDouble(fiefData[23]), Convert.ToDouble(fiefData[24]),
                Convert.ToDouble(fiefData[25]), Convert.ToDouble(fiefData[26]), Convert.ToDouble(fiefData[27]) };

                // previous
                double[] finPrev = new double[] { Convert.ToDouble(fiefData[28]), Convert.ToDouble(fiefData[29]),
                    Convert.ToDouble(fiefData[30]), Convert.ToDouble(fiefData[31]), Convert.ToDouble(fiefData[32]),
                    Convert.ToDouble(fiefData[33]), Convert.ToDouble(fiefData[34]), Convert.ToDouble(fiefData[35]),
               Convert.ToDouble(fiefData[36]), Convert.ToDouble(fiefData[37]), Convert.ToDouble(fiefData[38]),
                Convert.ToDouble(fiefData[39]), Convert.ToDouble(fiefData[40]), Convert.ToDouble(fiefData[41]) };

                // check for presence of conditional values
                string tiHo, own, ancOwn, bail, sge;
                tiHo = own = ancOwn = bail = sge = null;

                if (!String.IsNullOrWhiteSpace(fiefData[52]))
                {
                    tiHo = fiefData[52];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[53]))
                {
                    own = fiefData[53];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[54]))
                {
                    ancOwn = fiefData[54];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[55]))
                {
                    bail = fiefData[55];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[56]))
                {
                    sge = fiefData[56];
                }

                // create Fife_Serialised object
                thisFiefSer = new Fief_Serialised(fiefData[1], fiefData[2], fiefData[3], Convert.ToInt32(fiefData[4]),
                    Convert.ToDouble(fiefData[5]), Convert.ToDouble(fiefData[6]), Convert.ToUInt32(fiefData[7]),
                    Convert.ToDouble(fiefData[8]), Convert.ToDouble(fiefData[9]), Convert.ToUInt32(fiefData[10]),
                    Convert.ToUInt32(fiefData[11]), Convert.ToUInt32(fiefData[12]), Convert.ToUInt32(fiefData[13]),
                    finCurr, finPrev, Convert.ToDouble(fiefData[42]), Convert.ToDouble(fiefData[43]),
                    Convert.ToChar(fiefData[44]), fiefData[45], fiefData[46], characters, barredChars,
                    barredNats, Convert.ToDouble(fiefData[47]), Convert.ToInt32(fiefData[48]), armies,
                    Convert.ToBoolean(fiefData[49]), new Dictionary<string, ProtoDetachment>(), Convert.ToBoolean(fiefData[50]),
                    Convert.ToByte(fiefData[51]), tiHo: tiHo, own: own, ancOwn: ancOwn, bail: bail, sge: sge);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message= "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                //TODO error handling
                /*
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }*/
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                //TODO error handling
                /*
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }*/
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisFiefSer;
        }

        /// <summary>
        /// Creates a Province_Serialised object using data in a string array
        /// </summary>
        /// <returns>Province_Serialised object</returns>
        /// <param name="provData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Province_Serialised ImportFromCSV_Prov(string[] provData, int lineNum)
        {
            Province_Serialised thisProvSer = null;

            try
            {
                if (provData.Length != 8)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Province object.");
                }

                // check for presence of conditional values
                string tiHo, own, kingdom;
                tiHo = own = kingdom = null;

                if (!String.IsNullOrWhiteSpace(provData[5]))
                {
                    tiHo = provData[5];
                }
                if (!String.IsNullOrWhiteSpace(provData[6]))
                {
                    own = provData[6];
                }
                if (!String.IsNullOrWhiteSpace(provData[7]))
                {
                    kingdom = provData[7];
                }

                // create Province_Serialised object
                thisProvSer = new Province_Serialised(provData[1], provData[2], Convert.ToByte(provData[3]),
                    Convert.ToDouble(provData[4]), tiHo, own, kingdom);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisProvSer;
        }

        /// <summary>
        /// Creates a Kingdom_Serialised object using data in a string array
        /// </summary>
        /// <returns>Kingdom_Serialised object</returns>
        /// <param name="kingData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Kingdom_Serialised ImportFromCSV_Kingdom(string[] kingData, int lineNum)
        {
            Kingdom_Serialised thisKingSer = null;

            try
            {
                if (kingData.Length != 7)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Kingdom object.");
                }

                // check for presence of conditional values
                string tiHo, own;
                tiHo = own = null;

                if (!String.IsNullOrWhiteSpace(kingData[5]))
                {
                    tiHo = kingData[5];
                }
                if (!String.IsNullOrWhiteSpace(kingData[6]))
                {
                    own = kingData[6];
                }

                // create Kingdom_Serialised object
                thisKingSer = new Kingdom_Serialised(kingData[1], kingData[2], Convert.ToByte(kingData[3]), kingData[4],
                    tiHo, own);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisKingSer;
        }

        /// <summary>
        /// Creates a PlayerCharacter_Serialised object using data in a string array
        /// </summary>
        /// <returns>PlayerCharacter_Serialised object</returns>
        /// <param name="pcData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static PlayerCharacter_Serialised ImportFromCSV_PC(string[] pcData, int lineNum)
        {
            PlayerCharacter_Serialised thisPcSer = null;

            try
            {
                // create empty lists for variable length collections
                // (traits, myTitles, myNPCs, myOwnedFiefs, myOwnedProvinces, myArmies, mySieges)
                Tuple<string, int>[] traits = null;
                List<string> myTitles = new List<string>();
                List<string> myNPCs = new List<string>();
                List<string> myOwnedFiefs = new List<string>();
                List<string> myOwnedProvinces = new List<string>();
                List<string> myArmies = new List<string>();
                List<string> mySieges = new List<string>();

                // check to see if any data present for variable length collections
                if (pcData.Length > 30)
                {
                    // create variables to hold start/end index positions
                    int trStart, trEnd, tiStart, tiEnd, npcStart, npcEnd, fiStart, fiEnd, prStart, prEnd, arStart, arEnd,
                        siStart, siEnd;
                    trStart = trEnd = tiStart = tiEnd = npcStart = npcEnd = fiStart = fiEnd = prStart = prEnd = arStart
                        = arEnd = siStart = siEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 30; i < pcData.Length; i++)
                    {
                        if (pcData[i].Equals("trStart"))
                        {
                            trStart = i;
                        }
                        else if (pcData[i].Equals("trEnd"))
                        {
                            trEnd = i;
                        }
                        else if (pcData[i].Equals("tiStart"))
                        {
                            tiStart = i;
                        }
                        else if (pcData[i].Equals("tiEnd"))
                        {
                            tiEnd = i;
                        }
                        else if (pcData[i].Equals("npcStart"))
                        {
                            npcStart = i;
                        }
                        else if (pcData[i].Equals("npcEnd"))
                        {
                            npcEnd = i;
                        }
                        else if (pcData[i].Equals("fiStart"))
                        {
                            fiStart = i;
                        }
                        else if (pcData[i].Equals("fiEnd"))
                        {
                            fiEnd = i;
                        }
                        else if (pcData[i].Equals("prStart"))
                        {
                            prStart = i;
                        }
                        else if (pcData[i].Equals("prEnd"))
                        {
                            prEnd = i;
                        }
                        else if (pcData[i].Equals("arStart"))
                        {
                            arStart = i;
                        }
                        else if (pcData[i].Equals("arEnd"))
                        {
                            arEnd = i;
                        }
                        else if (pcData[i].Equals("siStart"))
                        {
                            siStart = i;
                        }
                        else if (pcData[i].Equals("siEnd"))
                        {
                            siEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // traits
                    List<Tuple<string, int>> tempTraits = new List<Tuple<string, int>>();

                    if ((trStart > -1) && (trEnd > -1))
                    {
                        // check to ensure all traits have accompanying trait level
                        if (Utility_Methods.IsOdd(trStart + trEnd))
                        {
                            for (int i = trStart + 1; i < trEnd; i = i + 2)
                            {
                                Tuple<string, int> thisTrait = new Tuple<string, int>(pcData[i], Convert.ToInt32(pcData[i + 1]));
                                tempTraits.Add(thisTrait);
                            }
                            // convert traits list to traits array
                            traits = tempTraits.ToArray();
                        }
                    }

                    // myTitles
                    if ((tiStart > -1) && (tiEnd > -1))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i++)
                        {
                            myTitles.Add(pcData[i]);
                        }
                    }

                    // myNPCs
                    if ((npcStart > -1) && (npcEnd > -1))
                    {
                        for (int i = npcStart + 1; i < npcEnd; i++)
                        {
                            myNPCs.Add(pcData[i]);
                        }
                    }

                    // myOwnedFiefs
                    if ((fiStart > -1) && (fiEnd > -1))
                    {
                        for (int i = fiStart + 1; i < fiEnd; i++)
                        {
                            myOwnedFiefs.Add(pcData[i]);
                        }
                    }

                    // myOwnedProvinces
                    if ((prStart > -1) && (prEnd > -1))
                    {
                        for (int i = prStart + 1; i < prEnd; i++)
                        {
                            myOwnedProvinces.Add(pcData[i]);
                        }
                    }

                    // myArmies
                    if ((arStart > -1) && (arEnd > -1))
                    {
                        for (int i = arStart + 1; i < arEnd; i++)
                        {
                            myArmies.Add(pcData[i]);
                        }
                    }

                    // mySieges
                    if ((siStart > -1) && (siEnd > -1))
                    {
                        for (int i = siStart + 1; i < siEnd; i++)
                        {
                            mySieges.Add(pcData[i]);
                        }
                    }
                }

                // if no traits, try to generate random set
                if (traits == null)
                {
                    if (Globals_Game.traitMasterList.Count > 2)
                    {
                        Tuple<Trait, int>[] generatedTraits = Utility_Methods.GenerateTraitSet();

                        // convert to format for saving to database
                        traits = new Tuple<String, int>[generatedTraits.Length];
                        for (int i = 0; i < generatedTraits.Length; i++)
                        {
                            traits[i] = new Tuple<string, int>(generatedTraits[i].Item1.id, generatedTraits[i].Item2);
                        }
                    }

                    // if can't generate set, create empty set
                    else
                    {
                        traits = new Tuple<string, int>[0];
                    }
                }

                // create DOB tuple
                if (String.IsNullOrWhiteSpace(pcData[5]))
                {
                    pcData[5] = Globals_Game.myRand.Next(4).ToString();
                }
                Tuple<uint, byte> dob = new Tuple<uint, byte>(Convert.ToUInt32(pcData[4]), Convert.ToByte(pcData[5]));

                // check for presence of CONDITIONAL VARIABLES
                string loc, aID, pID;
                loc = aID = pID = null;

                if (!String.IsNullOrWhiteSpace(pcData[27]))
                {
                    loc = pcData[27];
                }
                if (!String.IsNullOrWhiteSpace(pcData[28]))
                {
                    aID = pcData[28];
                }
                if (!String.IsNullOrWhiteSpace(pcData[29]))
                {
                    pID = pcData[29];
                }

                // create PlayerCharacter_Serialised object
                thisPcSer = new PlayerCharacter_Serialised(pcData[1], pcData[2], pcData[3], dob, Convert.ToBoolean(pcData[6]),
                    pcData[7], Convert.ToBoolean(pcData[8]), Convert.ToDouble(pcData[9]), Convert.ToDouble(pcData[10]),
                    new List<string>(), pcData[11], Convert.ToDouble(pcData[12]), Convert.ToDouble(pcData[13]),
                    Convert.ToDouble(pcData[14]), Convert.ToDouble(pcData[15]), traits, Convert.ToBoolean(pcData[16]),
                    Convert.ToBoolean(pcData[17]), pcData[18], pcData[19], pcData[20], pcData[21], myTitles,
                    pcData[22], Convert.ToBoolean(pcData[23]), Convert.ToUInt32(pcData[24]), myNPCs, myOwnedFiefs,
                    myOwnedProvinces, pcData[25], pcData[26], myArmies, mySieges, ails: new Dictionary<string, Ailment>(),
                    loc: loc, aID: aID, pID: pID);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisPcSer;
        }

        /// <summary>
        /// Creates a NonPlayerCharacter_Serialised object using data in a string array
        /// </summary>
        /// <returns>NonPlayerCharacter_Serialised object</returns>
        /// <param name="npcData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static NonPlayerCharacter_Serialised importFromCSV_NPC(string[] npcData, int lineNum)
        {
            NonPlayerCharacter_Serialised thisNpcSer = null;

            try
            {
                // create empty lists for variable length collections
                // (traits, myTitles)
                Tuple<string, int>[] traits = null;
                List<string> myTitles = new List<string>();

                // check to see if any data present for variable length collections
                if (npcData.Length > 29)
                {
                    // create variables to hold start/end index positions
                    int trStart, trEnd, tiStart, tiEnd;
                    trStart = trEnd = tiStart = tiEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 29; i < npcData.Length; i++)
                    {
                        if (npcData[i].Equals("trStart"))
                        {
                            trStart = i;
                        }
                        else if (npcData[i].Equals("trEnd"))
                        {
                            trEnd = i;
                        }
                        else if (npcData[i].Equals("tiStart"))
                        {
                            tiStart = i;
                        }
                        else if (npcData[i].Equals("tiEnd"))
                        {
                            tiEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // traits
                    List<Tuple<string, int>> tempTraits = new List<Tuple<string, int>>();

                    if ((trStart > -1) && (trEnd > -1))
                    {
                        // check to ensure all traits have accompanying trait level
                        if (Utility_Methods.IsOdd(trStart + trEnd))
                        {
                            for (int i = trStart + 1; i < trEnd; i = i + 2)
                            {
                                Tuple<string, int> thisTrait = new Tuple<string, int>(npcData[i], Convert.ToInt32(npcData[i + 1]));
                                tempTraits.Add(thisTrait);
                            }
                            // convert traits list to traits array
                            traits = tempTraits.ToArray();
                        }
                    }

                    // myTitles
                    if ((tiStart > -1) && (tiEnd > -1))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i++)
                        {
                            myTitles.Add(npcData[i]);
                        }
                    }
                }

                // if no traits, try to generate random set
                if (traits == null)
                {
                    if (Globals_Game.traitMasterList.Count > 2)
                    {
                        Tuple<Trait, int>[] generatedTraits = Utility_Methods.GenerateTraitSet();

                        // convert to format for saving to database
                        traits = new Tuple<String, int>[generatedTraits.Length];
                        for (int i = 0; i < generatedTraits.Length; i++)
                        {
                            traits[i] = new Tuple<string, int>(generatedTraits[i].Item1.id, generatedTraits[i].Item2);
                        }
                    }

                    // if can't generate set, create empty set
                    else
                    {
                        traits = new Tuple<string, int>[0];
                    }
                }

                // create DOB tuple
                if (String.IsNullOrWhiteSpace(npcData[5]))
                {
                    npcData[5] = Globals_Game.myRand.Next(4).ToString();
                }
                Tuple<uint, byte> dob = new Tuple<uint, byte>(Convert.ToUInt32(npcData[4]), Convert.ToByte(npcData[5]));

                // check for presence of CONDITIONAL VARIABLES
                string loc, aID, boss;
                loc = aID = boss = null;

                if (!String.IsNullOrWhiteSpace(npcData[26]))
                {
                    loc = npcData[26];
                }
                if (!String.IsNullOrWhiteSpace(npcData[27]))
                {
                    aID = npcData[27];
                }
                if (!String.IsNullOrWhiteSpace(npcData[28]))
                {
                    boss = npcData[28];
                }

                // create NonPlayerCharacter_Serialised object
                thisNpcSer = new NonPlayerCharacter_Serialised(npcData[1], npcData[2], npcData[3], dob, Convert.ToBoolean(npcData[6]),
                    npcData[7], Convert.ToBoolean(npcData[8]), Convert.ToDouble(npcData[9]), Convert.ToDouble(npcData[10]),
                    new List<string>(), npcData[11], Convert.ToDouble(npcData[12]), Convert.ToDouble(npcData[13]),
                    Convert.ToDouble(npcData[14]), Convert.ToDouble(npcData[15]), traits, Convert.ToBoolean(npcData[16]),
                    Convert.ToBoolean(npcData[17]), npcData[18], npcData[19], npcData[20], npcData[21], myTitles, npcData[22],
                    Convert.ToUInt32(npcData[23]), Convert.ToBoolean(npcData[24]), Convert.ToBoolean(npcData[25]),
                    ails: new Dictionary<string, Ailment>(), loc: loc, aID: aID, empl: boss);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisNpcSer;
        }

        /// <summary>
        /// Creates a Trait object using data in a string array
        /// </summary>
        /// <returns>Trait object</returns>
        /// <param name="traitData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Trait ImportFromCSV_Trait(string[] traitData, int lineNum)
        {
            Trait thisTrait = null;

            try
            {
                // create empty lists for variable length collections
                // (effects)
                Dictionary<Globals_Game.Stats, double> effects = new Dictionary<Globals_Game.Stats, double>();

                // check to see if any data present for variable length collections
                if (traitData.Length > 3)
                {
                    // create variables to hold start/end index positions
                    int effStart, effEnd;
                    effStart = effEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 3; i < traitData.Length; i++)
                    {
                        if (traitData[i].Equals("effStart"))
                        {
                            effStart = i;
                        }
                        else if (traitData[i].Equals("effEnd"))
                        {
                            effEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // effects
                    if ((effStart > -1) && (effEnd > -1))
                    {
                        // check to ensure all effects have accompanying effect level
                        if (Utility_Methods.IsOdd(effStart + effEnd))
                        {
                            for (int i = effStart + 1; i < effEnd; i = i + 2)
                            {
                                Globals_Game.Stats stat;
                                if (Enum.TryParse<Globals_Game.Stats>(traitData[i], true, out stat))
                                {
                                    effects.Add(stat, Convert.ToDouble(traitData[i + 1]));
                                }
                                else
                                {
                                    Globals_Server.logError("Trait name unrecognised: " + traitData[i]);
                                }
                            }
                        }
                    }
                }

                // create Trait object
                if (effects.Count > 0)
                {
                    thisTrait = new Trait(traitData[1], traitData[2], effects);
                }
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisTrait;
        }

        /// <summary>
        /// Creates a Army object using data in a string array
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="armyData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Army ImportFromCSV_Army(string[] armyData, int lineNum)
        {
            Army thisArmy = null;

            try
            {
                if (armyData.Length != 16)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Army object.");
                }

                // create troops array
                uint[] troops = new uint[] { Convert.ToUInt32(armyData[9]), Convert.ToUInt32(armyData[10]),
                    Convert.ToUInt32(armyData[11]), Convert.ToUInt32(armyData[12]), Convert.ToUInt32(armyData[13]),
                    Convert.ToUInt32(armyData[14]), Convert.ToUInt32(armyData[15])};

                // check for presence of conditional values
                string maint, aggr, odds;
                maint = aggr = odds = null;

                if (!String.IsNullOrWhiteSpace(armyData[6]))
                {
                    maint = armyData[6];
                }
                if (!String.IsNullOrWhiteSpace(armyData[7]))
                {
                    aggr = armyData[7];
                }
                if (!String.IsNullOrWhiteSpace(armyData[8]))
                {
                    odds = armyData[8];
                }

                // create Army object
                thisArmy = new Army(armyData[1], armyData[2], armyData[3], Convert.ToDouble(armyData[4]), armyData[5],
                    maint: Convert.ToBoolean(maint), aggr: Convert.ToByte(aggr), odds: Convert.ToByte(odds), trp: troops);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisArmy;
        }

        /// <summary>
        /// Creates a Language_Serialised object using data in a string array
        /// </summary>
        /// <returns>Language_Serialised object</returns>
        /// <param name="langData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Language_Serialised ImportFromCSV_Language(string[] langData, int lineNum)
        {
            Language_Serialised thisLangSer = null;

            try
            {
                if (langData.Length != 4)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Language object.");
                }

                // create Language_Serialised object
                thisLangSer = new Language_Serialised(langData[1], langData[2], Convert.ToInt32(langData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisLangSer;
        }

        /// <summary>
        /// Creates a BaseLanguage object using data in a string array
        /// </summary>
        /// <returns>BaseLanguage object</returns>
        /// <param name="baseLangData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static BaseLanguage ImportFromCSV_BaseLanguage(string[] baseLangData, int lineNum)
        {
            BaseLanguage thisBaseLang = null;

            try
            {
                if (baseLangData.Length != 3)
                {
                    throw new InvalidDataException("Incorrect number of data parts for BaseLanguage object.");
                }

                // create BaseLanguage object
                thisBaseLang = new BaseLanguage(baseLangData[1], baseLangData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisBaseLang;
        }

        /// <summary>
        /// Creates a Nationality object using data in a string array
        /// </summary>
        /// <returns>Nationality object</returns>
        /// <param name="natData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Nationality ImportFromCSV_Nationality(string[] natData, int lineNum)
        {
            Nationality thisNat = null;

            try
            {
                if (natData.Length != 3)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Nationality object.");
                }

                // create Nationality object
                thisNat = new Nationality(natData[1], natData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisNat;
        }

        /// <summary>
        /// Creates a Rank object using data in a string array
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="rankData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Rank ImportFromCSV_Rank(string[] rankData, int lineNum)
        {
            Rank thisRank = null;

            try
            {
                // create empty lists for variable length collections
                // (title)
                TitleName[] title = null;

                // create variables to hold start/end index positions
                int tiStart, tiEnd;
                tiStart = tiEnd = -1;

                // iterate through main list STORING START/END INDEX POSITIONS
                for (int i = 3; i < rankData.Length; i++)
                {
                    if (rankData[i].Equals("tiStart"))
                    {
                        tiStart = i;
                    }
                    else if (rankData[i].Equals("tiEnd"))
                    {
                        tiEnd = i;
                    }
                }

                // ADD ITEMS to appropriate list
                // title
                List<TitleName> tempTitle = new List<TitleName>();

                if ((tiStart > -1) && (tiEnd > -1))
                {
                    // check to ensure all effects have accompanying effect level
                    if (Utility_Methods.IsOdd(tiStart + tiEnd))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i = i + 2)
                        {
                            TitleName thisTitle = new TitleName(rankData[i], rankData[i + 1]);
                            tempTitle.Add(thisTitle);
                        }
                        // create title array from list
                        title = tempTitle.ToArray();
                    }
                }

                if (title.Length > 0)
                {
                    // create Rank object
                    thisRank = new Rank(Convert.ToByte(rankData[1]), title, Convert.ToByte(rankData[2]));
                }

            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisRank;
        }

        /// <summary>
        /// Creates a Position_Serialised object using data in a string array
        /// </summary>
        /// <returns>Position_Serialised object</returns>
        /// <param name="posData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Position_Serialised ImportFromCSV_Position(string[] posData, int lineNum)
        {
            Position_Serialised thisPosSer = null;

            try
            {
                // create empty lists for variable length collections
                // (title)
                TitleName[] title = null;

                // create variables to hold start/end index positions
                int tiStart, tiEnd;
                tiStart = tiEnd = -1;

                // iterate through main list STORING START/END INDEX POSITIONS
                for (int i = 5; i < posData.Length; i++)
                {
                    if (posData[i].Equals("tiStart"))
                    {
                        tiStart = i;
                    }
                    else if (posData[i].Equals("tiEnd"))
                    {
                        tiEnd = i;
                    }
                }

                // ADD ITEMS to appropriate list
                // title
                List<TitleName> tempTitle = new List<TitleName>();

                if ((tiStart > -1) && (tiEnd > -1))
                {
                    // check to ensure all effects have accompanying effect level
                    if (Utility_Methods.IsOdd(tiStart + tiEnd))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i = i + 2)
                        {
                            TitleName thisTitle = new TitleName(posData[i], posData[i + 1]);
                            tempTitle.Add(thisTitle);
                        }
                        // create title array from list
                        title = tempTitle.ToArray();
                    }
                }

                if (title.Length > 0)
                {
                    // create Rank object
                    thisPosSer = new Position_Serialised(Convert.ToByte(posData[1]), title, Convert.ToByte(posData[2]), posData[3], posData[4]);
                }

            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisPosSer;
        }

        /// <summary>
        /// Creates a Siege object using data in a string array
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="siegeData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Siege ImportFromCSV_Siege(string[] siegeData, int lineNum)
        {
            Siege thisSiege = null;

            try
            {
                if (siegeData.Length != 16)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Siege object.");
                }

                // check for presence of conditional values
                string totAttCas, totDefCas, totDays, defenderAdd, siegeEnd;
                totAttCas = totDefCas = totDays = defenderAdd = siegeEnd = null;

                if (!String.IsNullOrWhiteSpace(siegeData[11]))
                {
                    totAttCas = siegeData[11];
                }
                else
                {
                    totAttCas = "0";
                }
                if (!String.IsNullOrWhiteSpace(siegeData[12]))
                {
                    totDefCas = siegeData[12];
                }
                else
                {
                    totDefCas = "0";
                }
                if (!String.IsNullOrWhiteSpace(siegeData[13]))
                {
                    totDays = siegeData[13];
                }
                else
                {
                    totDays = "0";
                }
                if (!String.IsNullOrWhiteSpace(siegeData[14]))
                {
                    defenderAdd = siegeData[14];
                }
                if (!String.IsNullOrWhiteSpace(siegeData[15]))
                {
                    siegeEnd = siegeData[15];
                }

                // create Siege object
                thisSiege = new Siege(siegeData[1], Convert.ToUInt32(siegeData[2]), Convert.ToByte(siegeData[3]), siegeData[4],
                    siegeData[5], siegeData[6], siegeData[7], siegeData[8], Convert.ToDouble(siegeData[9]),
                    Convert.ToDouble(siegeData[10]), Convert.ToInt32(totAttCas), Convert.ToInt32(totDefCas),
                    Convert.ToDouble(totDays), defenderAdd, siegeEnd);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisSiege;
        }

        /// <summary>
        /// Creates a Terrain object using data in a string array
        /// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="terrData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public static Terrain ImportFromCSV_Terrain(string[] terrData, int lineNum)
        {
            Terrain thisTerr = null;

            try
            {
                if (terrData.Length != 4)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Terrain object.");
                }

                // create Terrain object
                thisTerr = new Terrain(terrData[1], terrData[2], Convert.ToDouble(terrData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + fe.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + aoore.Message);
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + ide.Message);
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.CreateSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.entryDetails.Message = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.AddPastEvent(importErrorEntry);
                }
                Globals_Server.logError("Line " + lineNum + ": " + oe.Message);
            }

            return thisTerr;
        }

        /// <summary>
        /// Uses individual game objects to populate variable-length collections within other game objects
        /// </summary>
        /// <param name="fiefMasterList">Fief_Serialised objects</param>
        /// <param name="pcMasterList">PlayerCharacter_Serialised objects</param>
        /// <param name="npcMasterList">NonPlayerCharacter_Serialised objects</param>
        /// <param name="provinceMasterList">Province_Serialised objects</param>
        /// <param name="kingdomMasterList">Kingdom_Serialised objects</param>
        /// <param name="siegeMasterList">Siege objects</param>
        /// <param name="armyMasterList">Army objects</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        /// <param name="toDatabase">bool indicating whether or not to save the game objects to database or game</param>
        public static void SynchGameObjectCollections(Dictionary<string, Fief_Serialised> fiefMasterList, Dictionary<string,
            PlayerCharacter_Serialised> pcMasterList, Dictionary<string, NonPlayerCharacter_Serialised> npcMasterList,
            Dictionary<string, Province_Serialised> provinceMasterList, Dictionary<string, Kingdom_Serialised> kingdomMasterList,
            Dictionary<string, Siege> siegeMasterList, Dictionary<string, Army> armyMasterList, string bucketID, bool toDatabase = true)
        {

            // iterate through FIEFS
            foreach (KeyValuePair<string, Fief_Serialised> fiefEntry in fiefMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(fiefEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
                    if (pcMasterList.ContainsKey(fiefEntry.Value.titleHolder))
                    {
                        thisTiHo = pcMasterList[fiefEntry.Value.titleHolder];
                    }
                    else if (npcMasterList.ContainsKey(fiefEntry.Value.titleHolder))
                    {
                        thisTiHo = npcMasterList[fiefEntry.Value.titleHolder];
                    }

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(fiefEntry.Key))
                        {
                            thisTiHo.myTitles.Add(fiefEntry.Key);
                        }
                    }
                }

                // get owner
                if (!String.IsNullOrWhiteSpace(fiefEntry.Value.owner))
                {
                    PlayerCharacter_Serialised thisOwner = null;
                    if (pcMasterList.ContainsKey(fiefEntry.Value.owner))
                    {
                        thisOwner = pcMasterList[fiefEntry.Value.owner];
                    }

                    // put fief in owner's ownedFiefs
                    if (thisOwner != null)
                    {
                        if (!thisOwner.ownedFiefs.Contains(fiefEntry.Key))
                        {
                            thisOwner.ownedFiefs.Add(fiefEntry.Key);
                        }
                    }
                }
            }

            // iterate through PROVINCES
            foreach (KeyValuePair<string, Province_Serialised> provEntry in provinceMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(provEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
                    if (pcMasterList.ContainsKey(provEntry.Value.titleHolder))
                    {
                        thisTiHo = pcMasterList[provEntry.Value.titleHolder];
                    }
                    else if (npcMasterList.ContainsKey(provEntry.Value.titleHolder))
                    {
                        thisTiHo = npcMasterList[provEntry.Value.titleHolder];
                    }

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(provEntry.Key))
                        {
                            thisTiHo.myTitles.Add(provEntry.Key);
                        }
                    }
                }

                // get owner
                if (!String.IsNullOrWhiteSpace(provEntry.Value.owner))
                {
                    PlayerCharacter_Serialised thisOwner = null;
                    if (pcMasterList.ContainsKey(provEntry.Value.owner))
                    {
                        thisOwner = pcMasterList[provEntry.Value.owner];
                    }

                    // put province in owner's ownedProvinces
                    if (!thisOwner.ownedProvinces.Contains(provEntry.Key))
                    {
                        thisOwner.ownedProvinces.Add(provEntry.Key);
                    }
                }
            }

            // iterate through KINGDOMS
            foreach (KeyValuePair<string, Kingdom_Serialised> kingEntry in kingdomMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(kingEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
                    if (pcMasterList.ContainsKey(kingEntry.Value.titleHolder))
                    {
                        thisTiHo = pcMasterList[kingEntry.Value.titleHolder];
                    }
                    else if (npcMasterList.ContainsKey(kingEntry.Value.titleHolder))
                    {
                        thisTiHo = npcMasterList[kingEntry.Value.titleHolder];
                    }

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(kingEntry.Key))
                        {
                            thisTiHo.myTitles.Add(kingEntry.Key);
                        }
                    }
                }
            }

            // iterate through PCs
            foreach (KeyValuePair<string, PlayerCharacter_Serialised> pcEntry in pcMasterList)
            {
                if (pcEntry.Value.isAlive)
                {
                    // get location
                    if (!String.IsNullOrWhiteSpace(pcEntry.Value.location))
                    {
                        Fief_Serialised thisFief = null;
                        if (fiefMasterList.ContainsKey(pcEntry.Value.location))
                        {
                            thisFief = fiefMasterList[pcEntry.Value.location];
                        }

                        // put PC in fief's characters
                        if (!thisFief.charactersInFief.Contains(pcEntry.Key))
                        {
                            thisFief.charactersInFief.Add(pcEntry.Key);
                        }
                    }
                }
            }

            // iterate through NPCs
            foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> npcEntry in npcMasterList)
            {
                if (npcEntry.Value.isAlive)
                {
                    // get location
                    if (!String.IsNullOrWhiteSpace(npcEntry.Value.location))
                    {
                        Fief_Serialised thisFief = null;
                        if (fiefMasterList.ContainsKey(npcEntry.Value.location))
                        {
                            thisFief = fiefMasterList[npcEntry.Value.location];
                        }

                        // put NPC in fief's characters
                        if (thisFief != null)
                        {
                            if (!thisFief.charactersInFief.Contains(npcEntry.Key))
                            {
                                thisFief.charactersInFief.Add(npcEntry.Key);
                            }
                        }
                    }

                    // get employer
                    if (!String.IsNullOrWhiteSpace(npcEntry.Value.employer))
                    {
                        PlayerCharacter_Serialised thisBoss = null;
                        if (pcMasterList.ContainsKey(npcEntry.Value.employer))
                        {
                            thisBoss = pcMasterList[npcEntry.Value.employer];
                        }

                        if (thisBoss != null)
                        {
                            // put NPC in employer's myNPCs
                            if (!thisBoss.myNPCs.Contains(npcEntry.Key))
                            {
                                thisBoss.myNPCs.Add(npcEntry.Key);
                            }
                        }
                    }

                    // get familyID
                    if (!String.IsNullOrWhiteSpace(npcEntry.Value.familyID))
                    {
                        PlayerCharacter_Serialised thisHeadOfFamily = null;
                        if (pcMasterList.ContainsKey(npcEntry.Value.familyID))
                        {
                            thisHeadOfFamily = pcMasterList[npcEntry.Value.familyID];
                        }

                        if (thisHeadOfFamily != null)
                        {
                            // put NPC in headOfFamily's myNPCs
                            if (!thisHeadOfFamily.myNPCs.Contains(npcEntry.Key))
                            {
                                thisHeadOfFamily.myNPCs.Add(npcEntry.Key);
                            }
                        }
                    }
                }
            }

            // iterate through SIEGES
            foreach (KeyValuePair<string, Siege> siegeEntry in siegeMasterList)
            {
                // ensure siege not ended
                if (String.IsNullOrWhiteSpace(siegeEntry.Value.endDate))
                {
                    // get attacking PC
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.besiegingPlayer))
                    {
                        PlayerCharacter_Serialised attacker = null;
                        if (pcMasterList.ContainsKey(siegeEntry.Value.besiegingPlayer))
                        {
                            attacker = pcMasterList[siegeEntry.Value.besiegingPlayer];
                        }

                        // put siege id in attacker's mySieges
                        if (attacker != null)
                        {
                            if (!attacker.mySieges.Contains(siegeEntry.Key))
                            {
                                attacker.mySieges.Add(siegeEntry.Key);
                            }
                        }
                    }

                    // get defending PC
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.defendingPlayer))
                    {
                        PlayerCharacter_Serialised defender = null;
                        if (pcMasterList.ContainsKey(siegeEntry.Value.defendingPlayer))
                        {
                            defender = pcMasterList[siegeEntry.Value.defendingPlayer];
                        }

                        // put siege id in defender's mySieges
                        if (defender != null)
                        {
                            if (!defender.mySieges.Contains(siegeEntry.Key))
                            {
                                defender.mySieges.Add(siegeEntry.Key);
                            }
                        }
                    }

                    // get defending Fief
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.besiegedFief))
                    {
                        Fief_Serialised besiegedFief = null;
                        if (fiefMasterList.ContainsKey(siegeEntry.Value.besiegedFief))
                        {
                            besiegedFief = fiefMasterList[siegeEntry.Value.besiegedFief];
                        }

                        // put siege id in fief's siege
                        if (besiegedFief != null)
                        {
                            if (!besiegedFief.siege.Equals(siegeEntry.Key))
                            {
                                besiegedFief.siege = siegeEntry.Key;
                            }
                        }
                    }
                }
            }

            // iterate through ARMIES
            foreach (KeyValuePair<string, Army> armyEntry in armyMasterList)
            {
                // get army owner
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.owner))
                {
                    PlayerCharacter_Serialised owner = null;
                    if (pcMasterList.ContainsKey(armyEntry.Value.owner))
                    {
                        owner = pcMasterList[armyEntry.Value.owner];
                    }

                    // put army in owner's myArmies
                    if (owner != null)
                    {
                        if (!owner.myArmies.Contains(armyEntry.Key))
                        {
                            owner.myArmies.Add(armyEntry.Key);
                        }
                    }
                }

                // get army leader
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.leader))
                {
                    Character_Serialised leader = null;
                    if (pcMasterList.ContainsKey(armyEntry.Value.leader))
                    {
                        leader = pcMasterList[armyEntry.Value.leader];
                    }
                    else if (npcMasterList.ContainsKey(armyEntry.Value.leader))
                    {
                        leader = npcMasterList[armyEntry.Value.leader];
                    }

                    // put army id in leader's armyID
                    if (leader != null)
                    {
                        if (!leader.armyID.Equals(armyEntry.Key))
                        {
                            leader.armyID = armyEntry.Key;
                        }
                    }
                }

                // get army location
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.location))
                {
                    Fief_Serialised thisFief = null;
                    if (fiefMasterList.ContainsKey(armyEntry.Value.location))
                    {
                        thisFief = fiefMasterList[armyEntry.Value.location];
                    }

                    // put army id in fief's armies
                    if (thisFief != null)
                    {
                        if (!thisFief.armies.Contains(armyEntry.Key))
                        {
                            thisFief.armies.Add(armyEntry.Key);
                        }
                    }
                }
            }

            // army
            if (armyMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Army> thisEntry in armyMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Army(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.armyMasterList = armyMasterList;
                }
            }

            // siege
            if (siegeMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Siege> thisEntry in siegeMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Siege(bucketID, thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Globals_Game.siegeMasterList = siegeMasterList;
                }
            }

            // NPC
            if (npcMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> thisEntry in npcMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_NPC(bucketID, npcs: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    NonPlayerCharacter newObject = new NonPlayerCharacter();
                    foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> thisEntry in npcMasterList)
                    {
                        // if no traits, try to generate random set
                        if (thisEntry.Value.traits.Length < 2)
                        {
                            if (Globals_Game.traitMasterList.Count > 2)
                            {
                                Tuple<Trait, int>[] generatedTraits = Utility_Methods.GenerateTraitSet();

                                // convert to format for saving to database
                                thisEntry.Value.traits = new Tuple<String, int>[generatedTraits.Length];
                                for (int i = 0; i < generatedTraits.Length; i++)
                                {
                                    thisEntry.Value.traits[i] = new Tuple<string, int>(generatedTraits[i].Item1.id, generatedTraits[i].Item2);
                                }
                            }
                        }

                        // de-serialise 
                        newObject = DatabaseRead.NPC_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.npcMasterList.Add(newObject.charID, newObject);
                    }
                }
            }

            // PC
            if (pcMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, PlayerCharacter_Serialised> thisEntry in pcMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_PC(bucketID, pcs: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    PlayerCharacter newObject = new PlayerCharacter();
                    foreach (KeyValuePair<string, PlayerCharacter_Serialised> thisEntry in pcMasterList)
                    {
                        // if no traits, try to generate random set
                        if (thisEntry.Value.traits.Length < 2)
                        {
                            if (Globals_Game.traitMasterList.Count > 2)
                            {
                                Tuple<Trait, int>[] generatedTraits = Utility_Methods.GenerateTraitSet();

                                // convert to format for saving to database
                                thisEntry.Value.traits = new Tuple<String, int>[generatedTraits.Length];
                                for (int i = 0; i < generatedTraits.Length; i++)
                                {
                                    thisEntry.Value.traits[i] = new Tuple<string, int>(generatedTraits[i].Item1.id, generatedTraits[i].Item2);
                                }
                            }
                        }

                        // de-serialise 
                        newObject = DatabaseRead.PC_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.pcMasterList.Add(newObject.charID, newObject);
                    }
                }
            }

            // kingdom
            if (kingdomMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Kingdom_Serialised> thisEntry in kingdomMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Kingdom(bucketID, ks: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Kingdom newObject = new Kingdom();
                    foreach (KeyValuePair<string, Kingdom_Serialised> thisEntry in kingdomMasterList)
                    {
                        // de-serialise 
                        newObject = DatabaseRead.Kingdom_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.kingdomMasterList.Add(newObject.id, newObject);
                    }
                }
            }

            // province
            if (provinceMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Province_Serialised> thisEntry in provinceMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Province(bucketID, ps: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Province newObject = new Province();
                    foreach (KeyValuePair<string, Province_Serialised> thisEntry in provinceMasterList)
                    {
                        // de-serialise 
                        newObject = DatabaseRead.Province_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.provinceMasterList.Add(newObject.id, newObject);
                    }
                }
            }

            // fief
            if (fiefMasterList.Count > 0)
            {
                // save to database
                if (toDatabase)
                {
                    foreach (KeyValuePair<string, Fief_Serialised> thisEntry in fiefMasterList)
                    {
                        DatabaseWrite.DatabaseWrite_Fief(bucketID, fs: thisEntry.Value);
                    }
                }
                // save to game
                else
                {
                    Fief newObject = new Fief();
                    foreach (KeyValuePair<string, Fief_Serialised> thisEntry in fiefMasterList)
                    {
                        // de-serialise 
                        newObject = DatabaseRead.Fief_deserialise(thisEntry.Value);

                        // add to Globals_Game masterList
                        Globals_Game.fiefMasterList.Add(newObject.id, newObject);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a map edges collection using data imported from a CSV file and either writes it to the database
        /// or creates a game map
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        /// <param name="toDatabase">bool indicating whether or not to save the game objects to database or game</param>
        public static bool CreateMapEdgesFromCSV(string filename, string bucketID = null, bool toDatabase = true)
        {
            bool success = true;
            List<TaggedEdge<string, string>> mapEdges = new List<TaggedEdge<string, string>>();
            string lineIn;
            string[] lineParts;
            StreamReader srHexes = null;
            string[,] mapHexes = null;
            List<string[]> customLinks = new List<string[]>();
            int row = 0;

            try
            {
                // opens StreamReader to read in  data from csv file
                srHexes = new StreamReader(filename);
            }
            // catch following IO exceptions that could be thrown by the StreamReader 
            catch (FileNotFoundException fnfe)
            {
                success = false;
                Globals_Server.logError(fnfe.Message);
            }
            catch (IOException ioe)
            {
                success = false;
                Globals_Server.logError(ioe.Message);
            }

            // CREATE HEXMAP ARRAY
            // while there is data in the line
            while ((lineIn = srHexes.ReadLine()) != null)
            {
                // put the contents of the line into lineParts array, splitting on (char)9 (TAB)
                lineParts = lineIn.Split(',');

                // first line should contain array dimensions
                if (lineParts[0].Equals("dimensions"))
                {
                    mapHexes = new string[Convert.ToInt32(lineParts[1]), Convert.ToInt32(lineParts[2])];
                }

                // unlabelled rows hold hex values (fiefIDs) for adjacent hexes
                else if (!lineParts[0].Equals("customLink"))
                {
                    for (int i = 0; i < mapHexes.GetLength(1); i++)
                    {
                        mapHexes[row, i] = lineParts[i];
                    }

                    // increment row
                    row++;
                }

                // 'customLink' rows hold data for additional links (e.g. non-adjacent hexes)
                else if (lineParts[0].Equals("customLink"))
                {
                    if (lineParts.Length == 4)
                    {
                        string[] newLink = new string[] { lineParts[1], lineParts[2], lineParts[3] };
                        customLinks.Add(newLink);
                    }
                }
            }

            // create list of map edges from array
            if (customLinks.Count > 0)
            {
                mapEdges = CSVimport.CreateMapEdgesList(mapHexes, customLinks: customLinks);
            }
            else
            {
                mapEdges = CSVimport.CreateMapEdgesList(mapHexes);
            }

            // SAVE TO DATABASE OR CREATE MAP FOR NEW GAME
            if (toDatabase)
            {
                // save to database
                DatabaseWrite.DatabaseWrite_MapEdges(bucketID, edges: mapEdges);
            }

            else
            {
                HexMapGraph newMap = new HexMapGraph();

                // creat map edges array from list
                TaggedEdge<Fief, string>[] edgesArray = DatabaseRead.EdgeCollection_deserialise(mapEdges);

                // create map from edges array
                newMap = new HexMapGraph("map_1", edgesArray);

                // set new map as current Globals_Game.gameMap
                Globals_Game.gameMap = newMap;
            }

            return success;
        }

        /// <summary>
        /// Creates list of serialised map edges
        /// </summary>
        /// <returns>List containing map edges</returns>
        /// <param name="mapArray">string[,] containing main map data</param>
        /// <param name="customLinks">List(string) containing custom hex links (optional)</param>
        public static List<TaggedEdge<string, string>> CreateMapEdgesList(string[,] mapArray, List<string[]> customLinks = null)
        {
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();
            TaggedEdge<string, string> thisEdge = null;

            // iterate row
            for (int i = 0; i < mapArray.GetLength(0); i++)
            {
                // iterate column
                for (int j = 0; j < mapArray.GetLength(1); j++)
                {
                    // don't process null entries
                    if (!String.IsNullOrWhiteSpace(mapArray[i, j]))
                    {
                        // if not first hex in row, ADD LINKS BETWEEN THIS HEX/FIEF AND PREVIOUS HEX/FIEF
                        if (j != 0)
                        {
                            if (!String.IsNullOrWhiteSpace(mapArray[i, j - 1]))
                            {
                                // add link to previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i, j - 1], "W");
                                edgesOut.Add(thisEdge);

                                // add link from previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j - 1], mapArray[i, j], "E");
                                edgesOut.Add(thisEdge);
                            }
                        }

                        // if not first row, ADD LINKS BETWEEN THIS HEX/FIEF AND HEX/FIEFS ABOVE
                        if (i != 0)
                        {
                            // keep track of target columns
                            int col = 0;

                            // if not first column in even-numbered row, add link between this hex/fief and hex/fief above left
                            if (!((!Utility_Methods.IsOdd(i)) && (j == 0)))
                            {
                                // target correct column (above left is different for odd/even numbered rows)
                                if (Utility_Methods.IsOdd(i))
                                {
                                    col = j;
                                }
                                else
                                {
                                    col = j - 1;
                                }

                                if (!String.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NW");
                                    edgesOut.Add(thisEdge);

                                    // add link from above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SE");
                                    edgesOut.Add(thisEdge);
                                }
                            }

                            // if not last column in odd-numbered row, add link between this hex/fief and hex/fief above right
                            if (!((Utility_Methods.IsOdd(i)) && (j == mapArray.GetLength(1) - 1)))
                            {
                                // target correct column (above right is different for odd/even numbered rows)
                                if (Utility_Methods.IsOdd(i))
                                {
                                    col = j + 1;
                                }
                                else
                                {
                                    col = j;
                                }

                                if (!String.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NE");
                                    edgesOut.Add(thisEdge);

                                    // add link from above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SW");
                                    edgesOut.Add(thisEdge);
                                }
                            }
                        }
                    }
                }
            }

            // check for custom links
            if (customLinks != null)
            {
                foreach (string[] linkEntry in customLinks)
                {
                    thisEdge = new TaggedEdge<string, string>(linkEntry[0], linkEntry[1], linkEntry[2]);
                    edgesOut.Add(thisEdge);
                }
            }

            return edgesOut;
        }
    }
}
