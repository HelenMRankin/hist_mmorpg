using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace hist_mmorpg
{
    public static class DatabaseRead
    {
        /// <summary>
        /// Reads all objects for a particular game from database
        /// </summary>
        /// <param name="gameID">ID of game (database bucket)</param>
        public static void DatabaseReadAll(string gameID)
        {

            // ========= load KEY LISTS (to ensure efficient retrieval of specific game objects)
            DatabaseRead.DatabaseRead_keyLists(gameID);

            // ========= load CLOCK
            Globals_Game.clock = DatabaseRead.DatabaseRead_clock(gameID, "gameClock");

            // ========= load GLOBAL_SERVER/GAME DICTIONARIES
            Globals_Server.combatValues = DatabaseRead.DatabaseRead_dictStringUint(gameID, "combatValues");
            Globals_Server.recruitRatios = DatabaseRead.DatabaseRead_dictStringDouble(gameID, "recruitRatios");
            Globals_Server.battleProbabilities = DatabaseRead.DatabaseRead_dictStringDouble(gameID, "battleProbabilities");
            Globals_Server.gameTypes = DatabaseRead.DatabaseRead_dictIntString(gameID, "gameTypes");
            Globals_Game.jEntryPriorities = DatabaseRead.DatabaseRead_dictStringByte(gameID, "jEntryPriorities");
            Globals_Game.ownershipChallenges = DatabaseRead.DatabaseRead_dictStringOwnCh(gameID, "ownershipChallenges");

            // ========= load GLOBAL_GAME/SERVER newID VARIABLES
            // newCharID
            Globals_Game.newCharID = DatabaseRead.DatabaseRead_newIDs(gameID, "newCharID");
            // newArmyID
            Globals_Game.newArmyID = DatabaseRead.DatabaseRead_newIDs(gameID, "newArmyID");
            // newDetachmentID
            Globals_Game.newDetachmentID = DatabaseRead.DatabaseRead_newIDs(gameID, "newDetachmentID");
            // newAilmentID
            Globals_Game.newAilmentID = DatabaseRead.DatabaseRead_newIDs(gameID, "newAilmentID");
            // newSiegeID
            Globals_Game.newSiegeID = DatabaseRead.DatabaseRead_newIDs(gameID, "newSiegeID");
            // newJournalEntryID
            Globals_Game.newJournalEntryID = DatabaseRead.DatabaseRead_newIDs(gameID, "newJournalEntryID");
            // gameType
            Globals_Game.gameType = DatabaseRead.DatabaseRead_newIDs(gameID, "currentVictoryType");
            // duration
            Globals_Game.duration = DatabaseRead.DatabaseRead_newIDs(gameID, "duration");
            // startYear
            Globals_Game.startYear = DatabaseRead.DatabaseRead_newIDs(gameID, "startYear");
            // newGameID
            Globals_Server.newGameID = DatabaseRead.DatabaseRead_newIDs(gameID, "newGameID");
            // newOwnChallengeID
            Globals_Game.newOwnChallengeID = DatabaseRead.DatabaseRead_newIDs(gameID, "newOwnChallengeID");

            // ========= load JOURNALS
            Globals_Game.scheduledEvents = DatabaseRead.DatabaseRead_journal(gameID, "serverScheduledEvents");
            Globals_Game.pastEvents = DatabaseRead.DatabaseRead_journal(gameID, "serverPastEvents");

            // ========= load victoryData
            foreach (string element in Globals_Game.victoryDataKeys)
            {
                VictoryData vicData = DatabaseRead.DatabaseRead_victoryData(gameID, element);
                // add VictoryData to Globals_Game.victoryData
                Globals_Game.victoryData.Add(vicData.playerID, vicData);
            }

            // ========= read GLOBALS_GAME/CLIENT/SERVER BOOL VARIABLES
            Globals_Game.loadFromDatabase = DatabaseRead.DatabaseRead_bool(gameID, "loadFromDatabase");
            Globals_Game.loadFromCSV = DatabaseRead.DatabaseRead_bool(gameID, "loadFromCSV");
            Globals_Game.writeToDatabase = DatabaseRead.DatabaseRead_bool(gameID, "writeToDatabase");
            Globals_Game.statureCapInForce = DatabaseRead.DatabaseRead_bool(gameID, "statureCapInForce");

            // ========= load TRAITS
            foreach (string element in Globals_Game.traitKeys)
            {
                Trait trait = DatabaseRead.DatabaseRead_trait(gameID, element);
                // add Trait to traitMasterList
                Globals_Game.traitMasterList.Add(trait.id, trait);
            }

            // ========= load BASELANGUAGES
            foreach (string element in Globals_Game.baseLangKeys)
            {
                BaseLanguage bLang = DatabaseRead.DatabaseRead_baseLanguage(gameID, element);
                // add BaseLanguage to baseLanguageMasterList
                Globals_Game.baseLanguageMasterList.Add(bLang.id, bLang);
            }

            // ========= load LANGUAGES
            foreach (string element in Globals_Game.langKeys)
            {
                Language lang = DatabaseRead.DatabaseRead_language(gameID, element);
                // add Language to languageMasterList
                Globals_Game.languageMasterList.Add(lang.id, lang);
            }

            // ========= load NATIONALITY OBJECTS
            foreach (string element in Globals_Game.nationalityKeys)
            {
                Nationality nat = DatabaseRead.DatabaseRead_nationality(gameID, element);
                // add Nationality to nationalityMasterList
                Globals_Game.nationalityMasterList.Add(nat.natID, nat);
            }

            // ========= load RANKS
            foreach (byte element in Globals_Game.rankKeys)
            {
                Rank rank = DatabaseRead.DatabaseRead_rank(gameID, element.ToString());
                // add Rank to rankMasterList
                Globals_Game.rankMasterList.Add(rank.id, rank);
            }

            // ========= load POSITIONS
            foreach (byte element in Globals_Game.positionKeys)
            {
                Position pos = DatabaseRead.DatabaseRead_position(gameID, element.ToString());
                // add Position to positionMasterList
                Globals_Game.positionMasterList.Add(pos.id, pos);
            }

            // ========= load SIEGES
            foreach (string element in Globals_Game.siegeKeys)
            {
                Siege s = DatabaseRead.DatabaseRead_Siege(gameID, element);
                // add Siege to siegeMasterList
                Globals_Game.siegeMasterList.Add(s.siegeID, s);
            }

            // ========= load ARMIES
            foreach (string element in Globals_Game.armyKeys)
            {
                Army a = DatabaseRead.DatabaseRead_Army(gameID, element);
                // add Army to armyMasterList
                Globals_Game.armyMasterList.Add(a.armyID, a);
            }

            // ========= load NPCs
            foreach (string element in Globals_Game.npcKeys)
            {
                NonPlayerCharacter npc = DatabaseRead.DatabaseRead_NPC(gameID, element);
                // add NPC to npcMasterList
                Globals_Game.npcMasterList.Add(npc.charID, npc);
            }

            // ========= load PCs
            foreach (string element in Globals_Game.pcKeys)
            {
                PlayerCharacter pc = DatabaseRead.DatabaseRead_PC(gameID, element);
                // add PC to pcMasterList
                Globals_Game.pcMasterList.Add(pc.charID, pc);
            }

            // ========= load KINGDOMS
            foreach (string element in Globals_Game.kingKeys)
            {
                Kingdom king = DatabaseRead.DatabaseRead_Kingdom(gameID, element);
                // add Kingdom to kingdomMasterList
                Globals_Game.kingdomMasterList.Add(king.id, king);
            }

            // ========= load PROVINCES
            foreach (string element in Globals_Game.provKeys)
            {
                Province prov = DatabaseRead.DatabaseRead_Province(gameID, element);
                // add Province to provinceMasterList
                Globals_Game.provinceMasterList.Add(prov.id, prov);
            }

            // ========= load TERRAINS
            foreach (string element in Globals_Game.terrKeys)
            {
                Terrain terr = DatabaseRead.DatabaseRead_terrain(gameID, element);
                // add Terrain to terrainMasterList
                Globals_Game.terrainMasterList.Add(terr.id, terr);
            }

            // ========= load FIEFS
            foreach (string element in Globals_Game.fiefKeys)
            {
                Fief f = DatabaseRead.DatabaseRead_Fief(gameID, element);
                // add Fief to fiefMasterList
                Globals_Game.fiefMasterList.Add(f.id, f);
            }

            // ========= process any CHARACTER goTo QUEUES containing entries
            if (Globals_Game.goToList.Count > 0)
            {
                for (int i = 0; i < Globals_Game.goToList.Count; i++)
                {
                    DatabaseRead.Populate_goTo(Globals_Game.goToList[i]);
                }
                Globals_Game.goToList.Clear();
            }

            // ========= read GLOBALS_GAME CHARACTER VARIABLES
            Globals_Game.sysAdmin = DatabaseRead.DatabaseRead_PcVariable(gameID, "sysAdmin");
            Globals_Game.kingOne = DatabaseRead.DatabaseRead_PcVariable(gameID, "kingOne");
            Globals_Game.kingTwo = DatabaseRead.DatabaseRead_PcVariable(gameID, "kingTwo");
            Globals_Game.princeOne = DatabaseRead.DatabaseRead_PcVariable(gameID, "princeOne");
            Globals_Game.princeTwo = DatabaseRead.DatabaseRead_PcVariable(gameID, "princeTwo");
            Globals_Game.heraldOne = DatabaseRead.DatabaseRead_PcVariable(gameID, "heraldOne");
            Globals_Game.heraldTwo = DatabaseRead.DatabaseRead_PcVariable(gameID, "heraldTwo");

            // ========= load MAP
            Globals_Game.gameMap = DatabaseRead.DatabaseRead_map(gameID, "mapEdges");
        }

        //TODO
        public static Client DatabaseRead_Client(string gameID, string clientID)
        {
            var newClient = Globals_Server.rClient.Get(gameID, clientID);
            if (newClient.IsSuccess)
            {
                Client_Serialized client_ser = newClient.Value.GetObject<Client_Serialized>();

                return client_ser.deserialise();
            }
            else
            {
                Globals_Server.logError("Failed to read client: " + clientID);
                return null;
            }
        }

        /// <summary>
        /// Reads all key lists from the database
        /// </summary>
        /// <param name="gameID">Game for which key lists to be retrieved</param>
        public static void DatabaseRead_keyLists(string gameID)
        {
            // populate traitKeys
            var traitKeyResult = Globals_Server.rClient.Get(gameID, "traitKeys");
            if (traitKeyResult.IsSuccess)
            {
                Globals_Game.traitKeys = traitKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve traitKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate nationalityKeys
            var natKeyResult = Globals_Server.rClient.Get(gameID, "nationalityKeys");
            if (natKeyResult.IsSuccess)
            {
                Globals_Game.nationalityKeys = natKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve nationalityKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate langKeys
            var langKeyResult = Globals_Server.rClient.Get(gameID, "langKeys");
            if (langKeyResult.IsSuccess)
            {
                Globals_Game.langKeys = langKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve langKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate baseLangKeys
            var bLangKeyResult = Globals_Server.rClient.Get(gameID, "baseLangKeys");
            if (bLangKeyResult.IsSuccess)
            {
                Globals_Game.baseLangKeys = bLangKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve baseLangKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate rankKeys
            var rankKeyResult = Globals_Server.rClient.Get(gameID, "rankKeys");
            if (rankKeyResult.IsSuccess)
            {
                Globals_Game.rankKeys = rankKeyResult.Value.GetObject<List<byte>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve rankKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate positionKeys
            var positionKeyResult = Globals_Server.rClient.Get(gameID, "positionKeys");
            if (positionKeyResult.IsSuccess)
            {
                Globals_Game.positionKeys = positionKeyResult.Value.GetObject<List<byte>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve positionKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate npcKeys
            var npcKeyResult = Globals_Server.rClient.Get(gameID, "npcKeys");
            if (npcKeyResult.IsSuccess)
            {
                Globals_Game.npcKeys = npcKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve npcKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate pcKeys
            var pcKeyResult = Globals_Server.rClient.Get(gameID, "pcKeys");
            if (pcKeyResult.IsSuccess)
            {
                Globals_Game.pcKeys = pcKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve pcKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate kingKeys
            var kingKeyResult = Globals_Server.rClient.Get(gameID, "kingKeys");
            if (kingKeyResult.IsSuccess)
            {
                Globals_Game.kingKeys = kingKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve kingKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate provKeys
            var provKeyResult = Globals_Server.rClient.Get(gameID, "provKeys");
            if (provKeyResult.IsSuccess)
            {
                Globals_Game.provKeys = provKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve provKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate terrKeys
            var terrKeyResult = Globals_Server.rClient.Get(gameID, "terrKeys");
            if (terrKeyResult.IsSuccess)
            {
                Globals_Game.terrKeys = terrKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve terrKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate victoryDataKeys
            var vicDataResult = Globals_Server.rClient.Get(gameID, "victoryDataKeys");
            if (vicDataResult.IsSuccess)
            {
                Globals_Game.victoryDataKeys = vicDataResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve victoryDataKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate fiefKeys
            var fiefKeyResult = Globals_Server.rClient.Get(gameID, "fiefKeys");
            if (fiefKeyResult.IsSuccess)
            {
                Globals_Game.fiefKeys = fiefKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve fiefKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate armyKeys
            var armyKeyResult = Globals_Server.rClient.Get(gameID, "armyKeys");
            if (armyKeyResult.IsSuccess)
            {
                Globals_Game.armyKeys = armyKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve armyKeys from database.";
                Globals_Server.logError(toDisplay);
            }

            // populate siegeKeys
            var siegeKeyResult = Globals_Server.rClient.Get(gameID, "siegeKeys");
            if (siegeKeyResult.IsSuccess)
            {
                Globals_Game.siegeKeys = siegeKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                string toDisplay ="InitialDBload: Unable to retrieve siegeKeys from database.";
                Globals_Server.logError(toDisplay);
            }

        }

        /// <summary>
        /// Reads a GameClock object from the database
        /// </summary>
        /// <returns>GameClock object</returns>
        /// <param name="gameID">Game for which clock to be retrieved</param>
        /// <param name="clockID">ID of clock to be retrieved</param>
        public static GameClock DatabaseRead_clock(string gameID, string clockID)
        {
            var clockResult = Globals_Server.rClient.Get(gameID, clockID);
            var newClock = new GameClock();

            if (clockResult.IsSuccess)
            {
                newClock = clockResult.Value.GetObject<GameClock>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve GameClock " + clockID;
                Globals_Server.logError(toDisplay);
            }

            return newClock;
        }

        /// <summary>
        /// Reads a Dictionary(string, uint[]) from the database
        /// </summary>
        /// <returns>Dictionary(string, uint[]) object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public static Dictionary<string, uint[]> DatabaseRead_dictStringUint(string gameID, string dictID)
        {
            var dictResult = Globals_Server.rClient.Get(gameID, dictID);
            var newDict = new Dictionary<string, uint[]>();

            if (dictResult.IsSuccess)
            {
                newDict = dictResult.Value.GetObject<Dictionary<string, uint[]>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Dictionary " + dictID;
                Globals_Server.logError(toDisplay);
            }

            return newDict;
        }

        /// <summary>
        /// Reads a Dictionary(string, double[]) from the database
        /// </summary>
        /// <returns>Dictionary(string, double[]) object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public static Dictionary<string, double[]> DatabaseRead_dictStringDouble(string gameID, string dictID)
        {
            var dictResult = Globals_Server.rClient.Get(gameID, dictID);
            var newDict = new Dictionary<string, double[]>();

            if (dictResult.IsSuccess)
            {
                newDict = dictResult.Value.GetObject<Dictionary<string, double[]>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Dictionary " + dictID;
                Globals_Server.logError(toDisplay);
            }

            return newDict;
        }

        /// <summary>
        /// Reads a Dictionary(uint, string) from the database
        /// </summary>
        /// <returns>Dictionary(uint, string) object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public static Dictionary<uint, string> DatabaseRead_dictIntString(string gameID, string dictID)
        {
            var dictResult = Globals_Server.rClient.Get(gameID, dictID);
            var newDict = new Dictionary<uint, string>();

            if (dictResult.IsSuccess)
            {
                newDict = dictResult.Value.GetObject<Dictionary<uint, string>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Dictionary " + dictID;
                Globals_Server.logError(toDisplay);
            }

            return newDict;
        }

        /// <summary>
        /// Reads a Dictionary(string, byte) from the database
        /// </summary>
        /// <returns>Dictionary(string, byte) object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public static Dictionary<string[], byte> DatabaseRead_dictStringByte(string gameID, string dictID)
        {
            Dictionary<string[], byte> dictOut = new Dictionary<string[], byte>();
            var dictResult = Globals_Server.rClient.Get(gameID, dictID);
            var tempDict = new Dictionary<string, byte>();

            if (dictResult.IsSuccess)
            {
                tempDict = dictResult.Value.GetObject<Dictionary<string, byte>>();
                dictOut = DatabaseRead.JentryPriorities_deserialise(tempDict);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Dictionary " + dictID;
                Globals_Server.logError(toDisplay);
            }

            return dictOut;
        }

        /// <summary>
        /// Deserialises a jEntryPriorities Dictionary
        /// </summary>
        /// <returns>Dictionary<string[], byte> for game use</returns>
        /// <param name="dictToConvert">The Dictionary to convert</param>
        public static Dictionary<string[], byte> JentryPriorities_deserialise(Dictionary<string, byte> dictToConvert)
        {
            Dictionary<string[], byte> dictOut = new Dictionary<string[], byte>();

            if (dictToConvert.Count > 0)
            {
                foreach (KeyValuePair<string, byte> thisEntry in dictToConvert)
                {
                    string[] thisKey = thisEntry.Key.Split('|');
                    dictOut.Add(thisKey, thisEntry.Value);
                }
            }

            return dictOut;
        }

        /// <summary>
        /// Reads a Dictionary(string, OwnershipChallenge) from the database
        /// </summary>
        /// <returns>Dictionary(string, OwnershipChallenge) object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public static Dictionary<string, OwnershipChallenge> DatabaseRead_dictStringOwnCh(string gameID, string dictID)
        {
            Dictionary<string, OwnershipChallenge> dictOut = new Dictionary<string, OwnershipChallenge>();
            var dictResult = Globals_Server.rClient.Get(gameID, dictID);

            if (dictResult.IsSuccess)
            {
                dictOut = dictResult.Value.GetObject<Dictionary<string, OwnershipChallenge>>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Dictionary " + dictID;
                Globals_Server.logError(toDisplay);
            }

            return dictOut;
        }

        /// <summary>
        /// Reads a newID variable from the database
        /// </summary>
        /// <returns>uint</returns>
        /// <param name="gameID">Game for which newID variable to be retrieved</param>
        /// <param name="clockID">newID variable to be retrieved</param>
        public static uint DatabaseRead_newIDs(string gameID, string newID)
        {
            var newIDResult = Globals_Server.rClient.Get(gameID, newID);
            uint newIDout = 0;

            if (newIDResult.IsSuccess)
            {
                newIDout = newIDResult.Value.GetObject<uint>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve newID variable " + newID;
                Globals_Server.logError(toDisplay);
            }

            return newIDout;
        }

        /// <summary>
        /// Reads a Journal object from the database
        /// </summary>
        /// <returns>Journal object</returns>
        /// <param name="gameID">Game for which Journal to be retrieved</param>
        /// <param name="journalID">ID of Journal to be retrieved</param>
        public static Journal DatabaseRead_journal(string gameID, string journalID)
        {
            var journalResult = Globals_Server.rClient.Get(gameID, journalID);
            var newJournal = new Journal();

            if (journalResult.IsSuccess)
            {
                newJournal = journalResult.Value.GetObject<Journal>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Journal " + journalID;
                Globals_Server.logError(toDisplay);
            }

            return newJournal;
        }

        /// <summary>
        /// Reads a VictoryData object from the database
        /// </summary>
        /// <returns>VictoryData object</returns>
        /// <param name="gameID">Game for which VictoryData to be retrieved</param>
        /// <param name="vicDataID">ID of VictoryData to be retrieved</param>
        public static VictoryData DatabaseRead_victoryData(string gameID, string vicDataID)
        {
            var vicDataResult = Globals_Server.rClient.Get(gameID, vicDataID);
            var newVictoryData = new VictoryData();

            if (vicDataResult.IsSuccess)
            {
                newVictoryData = vicDataResult.Value.GetObject<VictoryData>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve VictoryData " + vicDataID;
                Globals_Server.logError(toDisplay);
            }

            return newVictoryData;
        }

        /// <summary>
        /// Reads a Trait object from the database
        /// </summary>
        /// <returns>Trait object</returns>
        /// <param name="gameID">Game for which trait to be retrieved</param>
        /// <param name="traitID">ID of trait to be retrieved</param>
        public static Trait DatabaseRead_trait(string gameID, string traitID)
        {
            var traitResult = Globals_Server.rClient.Get(gameID, traitID);
            var newTrait = new Trait();

            if (traitResult.IsSuccess)
            {
                newTrait = traitResult.Value.GetObject<Trait>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve trait " + traitID;
                Globals_Server.logError(toDisplay);
            }

            return newTrait;
        }

        /// <summary>
        /// Reads a BaseLanguage object from the database
        /// </summary>
        /// <returns>BaseLanguage object</returns>
        /// <param name="gameID">Game for which BaseLanguage to be retrieved</param>
        /// <param name="bLangID">ID of Language to be retrieved</param>
        public static BaseLanguage DatabaseRead_baseLanguage(string gameID, string bLangID)
        {
            var bLangResult = Globals_Server.rClient.Get(gameID, bLangID);
            var newBaseLang = new BaseLanguage();

            if (bLangResult.IsSuccess)
            {
                newBaseLang = bLangResult.Value.GetObject<BaseLanguage>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve BaseLanguage " + bLangID;
                Globals_Server.logError(toDisplay);
            }

            return newBaseLang;
        }

        /// <summary>
        /// Reads a Language object from the database
        /// </summary>
        /// <returns>Language object</returns>
        /// <param name="gameID">Game for which Language to be retrieved</param>
        /// <param name="langID">ID of Language to be retrieved</param>
        public static Language DatabaseRead_language(string gameID, string langID)
        {
            var languageResult = Globals_Server.rClient.Get(gameID, langID);
            var langSer = new Language_Serialised();
            var newLanguage = new Language();

            if (languageResult.IsSuccess)
            {
                // extract Language_Serialised
                langSer = languageResult.Value.GetObject<Language_Serialised>();

                // create Language from Language_Serialised
                newLanguage = DatabaseRead.Language_deserialise(langSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Language " + langID;
                Globals_Server.logError(toDisplay);
            }

            return newLanguage;
        }

        /// <summary>
        /// Deserialises a Language object
        /// </summary>
        /// <returns>Language object</returns>
        /// <param name="ls">Language_Serialised object to be converted</param>
        public static Language Language_deserialise(Language_Serialised ls)
        {
            Language langOut = null;

            // create NonPlayerCharacter from NonPlayerCharacter_Serialised
            langOut = new Language(ls);

            // insert BaseLanguage
            langOut.baseLanguage = Globals_Game.baseLanguageMasterList[ls.baseLanguage];

            return langOut;
        }

        /// <summary>
        /// Reads a Nationality object from the database
        /// </summary>
        /// <returns>Nationality object</returns>
        /// <param name="gameID">Game for which Nationality to be retrieved</param>
        /// <param name="natID">ID of Nationality to be retrieved</param>
        public static Nationality DatabaseRead_nationality(string gameID, string natID)
        {
            var natResult = Globals_Server.rClient.Get(gameID, natID);
            var newNat = new Nationality();

            if (natResult.IsSuccess)
            {
                newNat = natResult.Value.GetObject<Nationality>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Nationality " + natID;
                Globals_Server.logError(toDisplay);
            }

            return newNat;
        }

        /// <summary>
        /// Reads a Rank object from the database
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="gameID">Game for which Rank to be retrieved</param>
        /// <param name="rankID">ID of Rank to be retrieved</param>
        public static Rank DatabaseRead_rank(string gameID, string rankID)
        {
            var rankResult = Globals_Server.rClient.Get(gameID, rankID);
            var newRank = new Rank();

            if (rankResult.IsSuccess)
            {
                newRank = rankResult.Value.GetObject<Rank>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Rank " + rankID;
                Globals_Server.logError(toDisplay);
            }

            return newRank;
        }

        /// <summary>
        /// Reads a Position object from the database
        /// </summary>
        /// <returns>Position object</returns>
        /// <param name="gameID">Game for which Position to be retrieved</param>
        /// <param name="posID">ID of Position to be retrieved</param>
        public static Position DatabaseRead_position(string gameID, string posID)
        {
            var posResult = Globals_Server.rClient.Get(gameID, posID);
            var posSer = new Position_Serialised();
            var newPos = new Position();

            if (posResult.IsSuccess)
            {
                // extract Position_Serialised object
                posSer = posResult.Value.GetObject<Position_Serialised>();

                // create Position from Position_Serialised
                newPos = DatabaseRead.Position_deserialise(posSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Position " + posID;
                Globals_Server.logError(toDisplay);
            }

            return newPos;
        }

        /// <summary>
        /// Deserialises a Position object
        /// </summary>
        /// <returns>Position object</returns>
        /// <param name="ps">Position_Serialised object to be converted</param>
        public static Position Position_deserialise(Position_Serialised ps)
        {
            Position posOut = null;

            // create Position from Position_Serialised
            posOut = new Position(ps);

            // insert nationality
            if (Globals_Game.nationalityMasterList.ContainsKey(ps.nationality))
            {
                posOut.nationality = Globals_Game.nationalityMasterList[ps.nationality];
            }

            return posOut;
        }

        /// <summary>
        /// Reads a Siege object from the database
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="gameID">Game for which Siege to be retrieved</param>
        /// <param name="siegeID">ID of Siege to be retrieved</param>
        public static Siege DatabaseRead_Siege(string gameID, string siegeID)
        {
            var siegeResult = Globals_Server.rClient.Get(gameID, siegeID);
            Siege mySiege = new Siege();

            if (siegeResult.IsSuccess)
            {
                // extract Army object
                mySiege = siegeResult.Value.GetObject<Siege>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Siege " + siegeID;
                Globals_Server.logError(toDisplay);
            }

            return mySiege;
        }

        /// <summary>
        /// Reads an Army object from the database
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="gameID">Game for which Army to be retrieved</param>
        /// <param name="armyID">ID of Army to be retrieved</param>
        public static Army DatabaseRead_Army(string gameID, string armyID)
        {
            var armyResult = Globals_Server.rClient.Get(gameID, armyID);
            Army myArmy = new Army();

            if (armyResult.IsSuccess)
            {
                // extract Army object
                myArmy = armyResult.Value.GetObject<Army>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Army " + armyID;
                Globals_Server.logError(toDisplay);
            }

            return myArmy;
        }

        /// <summary>
        /// Reads an NPC object from the database
        /// </summary>
        /// <returns>NonPlayerCharacter object</returns>
        /// <param name="gameID">Game for which NPC to be retrieved</param>
        /// <param name="npcID">ID of NPC to be retrieved</param>
        public static NonPlayerCharacter DatabaseRead_NPC(string gameID, string npcID)
        {
            var npcResult = Globals_Server.rClient.Get(gameID, npcID);
            var npcSer = new NonPlayerCharacter_Serialised();
            NonPlayerCharacter myNPC = new NonPlayerCharacter();

            if (npcResult.IsSuccess)
            {
                // extract NonPlayerCharacter_Serialised object
                npcSer = npcResult.Value.GetObject<NonPlayerCharacter_Serialised>();

                // if NonPlayerCharacter_Serialised goTo queue contains entries, store for later processing
                if (npcSer.goTo.Count > 0)
                {
                    Globals_Game.goToList.Add(npcSer);
                }

                // create NonPlayerCharacter from NonPlayerCharacter_Serialised
                myNPC = DatabaseRead.NPC_deserialise(npcSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve NonPlayerCharacter " + npcID;
                Globals_Server.logError(toDisplay);
            }

            return myNPC;
        }

        /// <summary>
        /// Deserialises a NonPlayerCharacter object
        /// </summary>
        /// <returns>NonPlayerCharacter object</returns>
        /// <param name="npcs">NonPlayerCharacter_Serialised object to be converted</param>
        public static NonPlayerCharacter NPC_deserialise(NonPlayerCharacter_Serialised npcs)
        {
            NonPlayerCharacter npcOut = null;
            // create NonPlayerCharacter from NonPlayerCharacter_Serialised
            npcOut = new NonPlayerCharacter(npcs);

            // insert language
            npcOut.language = Globals_Game.languageMasterList[npcs.language];

            // insert nationality
            npcOut.nationality = Globals_Game.nationalityMasterList[npcs.nationality];

            // insert traits
            if (npcs.traits.Length > 0)
            {
                for (int i = 0; i < npcs.traits.Length; i++)
                {
                    npcOut.traits[i] = new Tuple<Trait, int>(Globals_Game.traitMasterList[npcs.traits[i].Item1], npcs.traits[i].Item2);
                }
            }

            return npcOut;
        }

        /// <summary>
        /// Reads a PC object from the database
        /// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="gameID">Game for which PC to be retrieved</param>
        /// <param name="pcID">ID of PC to be retrieved</param>
        public static PlayerCharacter DatabaseRead_PC(string gameID, string pcID)
        {
            var pcResult = Globals_Server.rClient.Get(gameID, pcID);
            var pcSer = new PlayerCharacter_Serialised();
            PlayerCharacter myPC = new PlayerCharacter();

            if (pcResult.IsSuccess)
            {
                // extract PlayerCharacter_Serialised object
                pcSer = pcResult.Value.GetObject<PlayerCharacter_Serialised>();

                // if PlayerCharacter_Serialised goTo queue contains entries, store for later processing
                if (pcSer.goTo.Count > 0)
                {
                    Globals_Game.goToList.Add(pcSer);
                }

                // create PlayerCharacter from PlayerCharacter_Serialised
                myPC = DatabaseRead.PC_deserialise(pcSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve PlayerCharacter " + pcID;
                Globals_Server.logError(toDisplay);
            }

            return myPC;
        }

        /// <summary>
        /// Deserialises a PlayerCharacter object
        /// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="pcs">PlayerCharacter_Serialised object to be converted</param>
        public static PlayerCharacter PC_deserialise(PlayerCharacter_Serialised pcs)
        {
            PlayerCharacter pcOut = null;

            // create PlayerCharacter from PlayerCharacter_Serialised
            pcOut = new PlayerCharacter(pcs);

            // insert language
            pcOut.language = Globals_Game.languageMasterList[pcs.language];

            // insert nationality
            pcOut.nationality = Globals_Game.nationalityMasterList[pcs.nationality];

            // insert traits
            if (pcs.traits.Length > 0)
            {
                for (int i = 0; i < pcs.traits.Length; i++)
                {
                    pcOut.traits[i] = new Tuple<Trait, int>(Globals_Game.traitMasterList[pcs.traits[i].Item1], pcs.traits[i].Item2);
                }
            }

            // insert employees/family
            if (pcs.myNPCs.Count > 0)
            {
                for (int i = 0; i < pcs.myNPCs.Count; i++)
                {
                    pcOut.myNPCs.Add(Globals_Game.npcMasterList[pcs.myNPCs[i]]);
                }
            }

            // insert armies
            if (pcs.myArmies.Count > 0)
            {
                for (int i = 0; i < pcs.myArmies.Count; i++)
                {
                    pcOut.myArmies.Add(Globals_Game.armyMasterList[pcs.myArmies[i]]);
                }
            }

            return pcOut;
        }

        /// <summary>
        /// Reads a Kingdom object from the database
        /// </summary>
        /// <returns>Kingdom object</returns>
        /// <param name="gameID">Game for which Kingdom to be retrieved</param>
        /// <param name="kingID">ID of Kingdom to be retrieved</param>
        public static Kingdom DatabaseRead_Kingdom(string gameID, string kingID)
        {
            var kingResult = Globals_Server.rClient.Get(gameID, kingID);
            var kingSer = new Kingdom_Serialised();
            Kingdom myKing = new Kingdom();

            if (kingResult.IsSuccess)
            {
                // extract Kingdom_Serialised object
                kingSer = kingResult.Value.GetObject<Kingdom_Serialised>();

                // create Kingdom from Kingdom_Serialised
                myKing = DatabaseRead.Kingdom_deserialise(kingSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Province " + kingID;
                Globals_Server.logError(toDisplay);
            }

            return myKing;
        }

        /// <summary>
        /// Deserialises a Kingdom object
        /// </summary>
        /// <returns>Kingdom object</returns>
        /// <param name="ks">Kingdom_Serialised to be converted</param>
        public static Kingdom Kingdom_deserialise(Kingdom_Serialised ks)
        {
            Kingdom kOut = null;
            kOut = new Kingdom(ks);

            // insert king
            if (!String.IsNullOrWhiteSpace(ks.owner))
            {
                if (Globals_Game.pcMasterList.ContainsKey(ks.owner))
                {
                    kOut.owner = Globals_Game.pcMasterList[ks.owner];
                }
                else
                {
                    string toDisplay = "Kingdom " + ks.id + ": King not found (" + ks.owner + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            // insert rank
            if (ks.rank > 0)
            {
                if (Globals_Game.rankMasterList.ContainsKey(ks.rank))
                {
                    kOut.rank = Globals_Game.rankMasterList[ks.rank];
                }
                else
                {
                    string toDisplay = "Kingdom " + ks.id + ": Rank not found (" + ks.rank + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            // insert nationality
            if (!String.IsNullOrWhiteSpace(ks.nationality))
            {
                if (Globals_Game.nationalityMasterList.ContainsKey(ks.nationality))
                {
                    kOut.nationality = Globals_Game.nationalityMasterList[ks.nationality];
                }
                else
                {
                    string toDisplay = "Kingdom " + ks.id + ": Nationality not found (" + ks.nationality + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            return kOut;
        }

        /// <summary>
        /// Reads a Province object from the database
        /// </summary>
        /// <returns>Province object</returns>
        /// <param name="gameID">Game for which Province to be retrieved</param>
        /// <param name="provID">ID of Province to be retrieved</param>
        public static Province DatabaseRead_Province(string gameID, string provID)
        {
            var provResult = Globals_Server.rClient.Get(gameID, provID);
            var provSer = new Province_Serialised();
            Province myProv = new Province();

            if (provResult.IsSuccess)
            {
                // extract Province_Serialised object
                provSer = provResult.Value.GetObject<Province_Serialised>();

                // create Province from Province_Serialised
                myProv = DatabaseRead.Province_deserialise(provSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Province " + provID;
                Globals_Server.logError(toDisplay);
            }

            return myProv;
        }

        /// <summary>
        /// Deserialises a Province object
        /// </summary>
        /// <returns>Province object</returns>
        /// <param name="ps">Province_Serialised to be converted</param>
        public static Province Province_deserialise(Province_Serialised ps)
        {
            Province provOut = null;
            provOut = new Province(ps);

            // insert overlord using overlordID
            if (!String.IsNullOrWhiteSpace(ps.owner))
            {
                if (Globals_Game.pcMasterList.ContainsKey(ps.owner))
                {
                    provOut.owner = Globals_Game.pcMasterList[ps.owner];
                }
                else
                {
                    string toDisplay = "Province " + ps.id + ": Overlord not found (" + ps.owner + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            // check if province is in owner's list of provinces owned
            bool provInList = provOut.owner.ownedProvinces.Any(item => item.id == provOut.id);
            // if not, add it
            if (!provInList)
            {
                provOut.owner.ownedProvinces.Add(provOut);
            }

            // insert kingdom using kingdomID
            if (!String.IsNullOrWhiteSpace(ps.kingdom))
            {
                if (Globals_Game.kingdomMasterList.ContainsKey(ps.kingdom))
                {
                    provOut.kingdom = Globals_Game.kingdomMasterList[ps.kingdom];
                }
                else
                {
                    string toDisplay = "Province " + ps.id + ": Kingdom not found (" + ps.kingdom + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            // insert rank using rankID
            if (ps.rank > 0)
            {
                if (Globals_Game.rankMasterList.ContainsKey(ps.rank))
                {
                    provOut.rank = Globals_Game.rankMasterList[ps.rank];
                }
                else
                {
                    string toDisplay = "Province " + ps.id + ": Rank not found (" + ps.rank + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            return provOut;
        }

        /// <summary>
        /// Reads a Terrain object from the database
        /// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="gameID">Game for which Terrain to be retrieved</param>
        /// <param name="terrID">ID of Terrain to be retrieved</param>
        public static Terrain DatabaseRead_terrain(string gameID, string terrID)
        {
            var terrainResult = Globals_Server.rClient.Get(gameID, terrID);
            var newTerrain = new Terrain();

            if (terrainResult.IsSuccess)
            {
                newTerrain = terrainResult.Value.GetObject<Terrain>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Terrain " + terrID;
                Globals_Server.logError(toDisplay);
            }

            return newTerrain;
        }

        /// <summary>
        /// Reads a Fief object from the database
        /// </summary>
        /// <returns>Fief object</returns>
        /// <param name="gameID">Game for which Fief to be retrieved</param>
        /// <param name="fiefID">ID of Fief to be retrieved</param>
        public static Fief DatabaseRead_Fief(string gameID, string fiefID)
        {
            var fiefResult = Globals_Server.rClient.Get(gameID, fiefID);
            var fiefSer = new Fief_Serialised();
            Fief myFief = new Fief();

            if (fiefResult.IsSuccess)
            {
                // extract Fief_Serialised object
                fiefSer = fiefResult.Value.GetObject<Fief_Serialised>();

                // create Fief from Fief_Serialised
                myFief = DatabaseRead.Fief_deserialise(fiefSer);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Fief " + fiefID;
                Globals_Server.logError(toDisplay);
            }

            return myFief;
        }

        /// <summary>
        /// Deserialises a Fief object
        /// Also inserts Fief into appropriate PlayerCharacter and NonPlayerCharacter objects.
        /// </summary>
        /// <returns>Fief object</returns>
        /// <param name="fs">Fief_Serialised object to be converted</param>
        public static Fief Fief_deserialise(Fief_Serialised fs)
        {
            Fief fOut = null;
            // create Fief from Fief_Serialised
            fOut = new Fief(fs);

            // insert province
            fOut.province = Globals_Game.provinceMasterList[fs.province];

            // insert language
            fOut.language = Globals_Game.languageMasterList[fs.language];

            // insert owner
            fOut.owner = Globals_Game.pcMasterList[fs.owner];
            // check if fief is in owner's list of fiefs owned
            bool fiefInList = fOut.owner.ownedFiefs.Any(item => item.id == fOut.id);
            // if not, add it
            if (!fiefInList)
            {
                fOut.owner.ownedFiefs.Add(fOut);
            }

            // insert ancestral owner
            fOut.ancestralOwner = Globals_Game.pcMasterList[fs.ancestralOwner];

            // insert bailiff (PC or NPC)
            if (!String.IsNullOrWhiteSpace(fs.bailiff))
            {
                if (Globals_Game.npcMasterList.ContainsKey(fs.bailiff))
                {
                    fOut.bailiff = Globals_Game.npcMasterList[fs.bailiff];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(fs.bailiff))
                {
                    fOut.bailiff = Globals_Game.pcMasterList[fs.bailiff];
                }
                else
                {
                    fOut.bailiff = null;
                    string toDisplay = "Unable to identify bailiff (" + fs.bailiff + ") for Fief " + fOut.id;
                    Globals_Server.logError(toDisplay);
                }
            }

            //insert terrain
            fOut.terrain = Globals_Game.terrainMasterList[fs.terrain];

            // insert characters
            if (fs.charactersInFief.Count > 0)
            {
                for (int i = 0; i < fs.charactersInFief.Count; i++)
                {
                    if (Globals_Game.npcMasterList.ContainsKey(fs.charactersInFief[i]))
                    {
                        fOut.charactersInFief.Add(Globals_Game.npcMasterList[fs.charactersInFief[i]]);
                        Globals_Game.npcMasterList[fs.charactersInFief[i]].location = fOut;
                    }
                    else if (Globals_Game.pcMasterList.ContainsKey(fs.charactersInFief[i]))
                    {
                        fOut.charactersInFief.Add(Globals_Game.pcMasterList[fs.charactersInFief[i]]);
                        Globals_Game.pcMasterList[fs.charactersInFief[i]].location = fOut;
                    }
                    else
                    {
                        string toDisplay = "Unable to identify character (" + fs.charactersInFief[i] + ") for Fief " + fOut.id;
                        Globals_Server.logError(toDisplay);
                    }

                }
            }

            // insert rank using rankID
            if (fs.rank > 0)
            {
                if (Globals_Game.rankMasterList.ContainsKey(fs.rank))
                {
                    fOut.rank = Globals_Game.rankMasterList[fs.rank];
                }
                else
                {
                    string toDisplay = "Fief " + fs.id + ": Rank not found (" + fs.rank + ")";
                    Globals_Server.logError(toDisplay);
                }
            }

            return fOut;
        }

        /// <summary>
        /// Inserts Fief objects into a Character's goTo Queue
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="cs">Character_Serialised containing goTo Queue</param>
        public static bool Populate_goTo(Character_Serialised cs)
        {
            bool success = false;
            Character myCh = null;

            if (cs is PlayerCharacter_Serialised)
            {
                if (Globals_Game.pcMasterList.ContainsKey(cs.charID))
                {
                    myCh = Globals_Game.pcMasterList[cs.charID];
                    success = true;
                }
            }
            else if (cs is NonPlayerCharacter_Serialised)
            {
                if (Globals_Game.npcMasterList.ContainsKey(cs.charID))
                {
                    myCh = Globals_Game.npcMasterList[cs.charID];
                    success = true;
                }
            }
            else
            {
                string toDisplay = "goTo queue processing: Character not found (" + cs.charID + ")";
                Globals_Server.logError(toDisplay);
            }

            if (success)
            {
                foreach (string value in cs.goTo)
                {
                    myCh.goTo.Enqueue(Globals_Game.fiefMasterList[value]);
                }
            }

            return success;
        }

        /// <summary>
        /// Reads a PlayerCharacter variable from the database
        /// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="gameID">Game for which PlayerCharacter variable to be retrieved</param>
        /// <param name="charVarID">ID of PlayerCharacter variable to be retrieved</param>
        public static PlayerCharacter DatabaseRead_PcVariable(string gameID, string charVarID)
        {
            var charVarResult = Globals_Server.rClient.Get(gameID, charVarID);
            String pcID = "";
            PlayerCharacter newPC = null;

            if (charVarResult.IsSuccess)
            {
                pcID = charVarResult.Value.GetObject<String>();
                if (pcID != "")
                {
                    if (Globals_Game.pcMasterList.ContainsKey(pcID))
                    {
                        newPC = Globals_Game.pcMasterList[pcID];
                    }
                }
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve Character variable " + charVarID;
                Globals_Server.logError(toDisplay);
            }

            return newPC;
        }

        /// <summary>
        /// Reads a bool variable from the database
        /// </summary>
        /// <returns>bool variable</returns>
        /// <param name="gameID">Game for which bool variable to be retrieved</param>
        /// <param name="boolID">ID of bool variable to be retrieved</param>
        public static bool DatabaseRead_bool(string gameID, string boolID)
        {
            var boolResult = Globals_Server.rClient.Get(gameID, boolID);
            bool newBool = true; ;

            if (boolResult.IsSuccess)
            {
                newBool = boolResult.Value.GetObject<bool>();
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve bool variable " + boolID;
                Globals_Server.logError(toDisplay);
            }

            return newBool;
        }

        /// <summary>
        /// Reads a map edge collection from the database and uses it to create a HexMapGraph
        /// </summary>
        /// <returns>HexMapGraph object</returns>
        /// <param name="gameID">Game for which map to be created</param>
        /// <param name="mapEdgesID">ID of map edges collection to be retrieved</param>
        public static HexMapGraph DatabaseRead_map(string gameID, string mapEdgesID)
        {
            var mapResult = Globals_Server.rClient.Get(gameID, mapEdgesID);
            List<TaggedEdge<string, string>> edgesList = new List<TaggedEdge<string, string>>();
            var newMap = new HexMapGraph();

            if (mapResult.IsSuccess)
            {
                edgesList = mapResult.Value.GetObject<List<TaggedEdge<string, string>>>();
                TaggedEdge<Fief, string>[] edgesArray = DatabaseRead.EdgeCollection_deserialise(edgesList);

                // create map from edges collection
                newMap = new HexMapGraph("map001", edgesArray);
            }
            else
            {
                string toDisplay = "InitialDBload: Unable to retrieve map edges " + mapEdgesID;
                Globals_Server.logError(toDisplay);
            }

            return newMap;
        }

        /// <summary>
        /// Converts serialised edges collection into HexMapGraph edges collection
        /// </summary>
        /// <returns>HexMapGraph edges collection</returns>
        /// <param name="edgesIn">'String-ified' edges collection to be converted</param>
        public static TaggedEdge<Fief, string>[] EdgeCollection_deserialise(List<TaggedEdge<string, string>> edgesIn)
        {
            TaggedEdge<Fief, string>[] edgesOut = new TaggedEdge<Fief, string>[edgesIn.Count];

            int i = 0;
            foreach (TaggedEdge<string, string> element in edgesIn)
            {
                // convert to HexMapGraph edge
                edgesOut[i] = DatabaseRead.EdgeString_deserialise(element);
                i++;
            }

            return edgesOut;
        }

        /// <summary>
        /// Converts serialised edge into HexMapGraph edge
        /// </summary>
        /// <returns>HexMapGraph edge</returns>
        /// <param name="te">'String-ified' edge to be converted</param>
        public static TaggedEdge<Fief, string> EdgeString_deserialise(TaggedEdge<string, string> te)
        {
            TaggedEdge<Fief, string> edgeOut = new TaggedEdge<Fief, string>(Globals_Game.fiefMasterList[te.Source], Globals_Game.fiefMasterList[te.Target], te.Tag);
            return edgeOut;
        }
    }
}
