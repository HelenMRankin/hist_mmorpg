using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using RiakClient;
using RiakClient.Models;
namespace hist_mmorpg
{
    public static class DatabaseWrite
    {
        //TODO implement server error log
        //TODO add loop to write all client things
        /// <summary>
        /// Writes all game objects to the database
        /// </summary>
        /// <param name="gameID">ID of game (used for database bucket)</param>
        public static void DatabaseWriteAll(string gameID, Client c = null)
        {
            // ========= write CLOCK
            DatabaseWrite.DatabaseWrite_Clock(gameID, Globals_Game.clock);

            // ========= write GLOBALS_SERVER/GAME DICTIONARIES
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "combatValues", Globals_Server.combatValues);
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "recruitRatios", Globals_Server.recruitRatios);
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "battleProbabilities", Globals_Server.battleProbabilities);
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "gameTypes", Globals_Server.gameTypes);
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "ownershipChallenges", Globals_Game.ownershipChallenges);
            // convert jEntryPriorities prior to writing
            Dictionary<string, byte> jEntryPriorities_serialised = DatabaseWrite.JentryPriorities_serialise(Globals_Game.jEntryPriorities);
            DatabaseWrite.DatabaseWrite_Dictionary(gameID, "jEntryPriorities", jEntryPriorities_serialised);

            // ========= write JOURNALS
            DatabaseWrite.DatabaseWrite_Journal(gameID, "serverScheduledEvents", Globals_Game.scheduledEvents);
            DatabaseWrite.DatabaseWrite_Journal(gameID, "serverPastEvents", Globals_Game.pastEvents);

            // ========= write GLOBALS_GAME/CLIENT/SERVER CHARACTER VARIABLES

            // Globals_Game.sysAdmin
            if (Globals_Game.sysAdmin != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "sysAdmin", Globals_Game.sysAdmin.charID);
            }
            // Globals_Game.kingOne
            if (Globals_Game.kingOne != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "kingOne", Globals_Game.kingOne.charID);
            }
            // Globals_Game.kingTwo
            if (Globals_Game.kingTwo != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "kingTwo", Globals_Game.kingTwo.charID);
            }
            // Globals_Game.princeOne
            if (Globals_Game.princeOne != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "princeOne", Globals_Game.princeOne.charID);
            }
            // Globals_Game.princeTwo
            if (Globals_Game.princeTwo != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "princeTwo", Globals_Game.princeTwo.charID);
            }
            // Globals_Game.heraldOne
            if (Globals_Game.heraldOne != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "heraldOne", Globals_Game.heraldOne.charID);
            }
            // Globals_Game.heraldTwo
            if (Globals_Game.heraldTwo != null)
            {
                DatabaseWrite.DatabaseWrite_String(gameID, "heraldTwo", Globals_Game.heraldTwo.charID);
            }

            // ========= write GLOBALS_GAME/CLIENT/SERVER newID VARIABLES
            // newCharID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newCharID", Globals_Game.newCharID);
            // newArmyID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newArmyID", Globals_Game.newArmyID);
            // newDetachmentID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newDetachmentID", Globals_Game.newDetachmentID);
            // newAilmentID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newAilmentID", Globals_Game.newAilmentID);
            // newSiegeID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newSiegeID", Globals_Game.newSiegeID);
            // newJournalEntryID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newJournalEntryID", Globals_Game.newJournalEntryID);
            // gameType
            DatabaseWrite.DatabaseWrite_newID(gameID, "gameType", Globals_Game.gameType);
            // duration
            DatabaseWrite.DatabaseWrite_newID(gameID, "duration", Globals_Game.duration);
            // startYear
            DatabaseWrite.DatabaseWrite_newID(gameID, "startYear", Globals_Game.startYear);
            // newGameID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newGameID", Globals_Server.newGameID);
            // newOwnChallengeID
            DatabaseWrite.DatabaseWrite_newID(gameID, "newOwnChallengeID", Globals_Game.newOwnChallengeID);

            // ========= write GLOBALS_GAME/CLIENT/SERVER BOOL VARIABLES
            // Globals_Game.loadFromDatabase
            DatabaseWrite.DatabaseWrite_Bool(gameID, "loadFromDatabase", Globals_Game.loadFromDatabase);
            // Globals_Game.loadFromCSV
            DatabaseWrite.DatabaseWrite_Bool(gameID, "loadFromCSV", Globals_Game.loadFromCSV);
            // Globals_Game.writeToDatabase
            DatabaseWrite.DatabaseWrite_Bool(gameID, "writeToDatabase", Globals_Game.writeToDatabase);
            // Globals_Game.statureCapInForce
            DatabaseWrite.DatabaseWrite_Bool(gameID, "statureCapInForce", Globals_Game.statureCapInForce);

            // ========= write TRAITS
            // clear existing key list
            if (Globals_Game.traitKeys.Count > 0)
            {
                Globals_Game.traitKeys.Clear();
            }

            // write each object in traitMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Trait> pair in Globals_Game.traitMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Trait(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.traitKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "traitKeys", Globals_Game.traitKeys);

            // ========= write BASELANGUAGES
            // clear existing key list
            if (Globals_Game.baseLangKeys.Count > 0)
            {
                Globals_Game.baseLangKeys.Clear();
            }

            // write each object in baseLanguageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, BaseLanguage> pair in Globals_Game.baseLanguageMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_BaseLanguage(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.baseLangKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "baseLangKeys", Globals_Game.baseLangKeys);

            // ========= write LANGUAGES
            // clear existing key list
            if (Globals_Game.langKeys.Count > 0)
            {
                Globals_Game.langKeys.Clear();
            }

            // write each object in languageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Language> pair in Globals_Game.languageMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Language(gameID, l: pair.Value);
                if (success)
                {
                    Globals_Game.langKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "langKeys", Globals_Game.langKeys);

            // ========= write NATIONALITY OBJECTS
            // clear existing key list
            if (Globals_Game.nationalityKeys.Count > 0)
            {
                Globals_Game.nationalityKeys.Clear();
            }

            // write each object in nationalityMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Nationality> pair in Globals_Game.nationalityMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Nationality(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.nationalityKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "nationalityKeys", Globals_Game.nationalityKeys);

            // ========= write RANKS
            // clear existing key list
            if (Globals_Game.rankKeys.Count > 0)
            {
                Globals_Game.rankKeys.Clear();
            }

            // write each object in rankMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Rank> pair in Globals_Game.rankMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Rank(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.rankKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "rankKeys", Globals_Game.rankKeys);

            // ========= write POSITIONS
            // clear existing key list
            if (Globals_Game.positionKeys.Count > 0)
            {
                Globals_Game.positionKeys.Clear();
            }

            // write each object in positionMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Position> pair in Globals_Game.positionMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Position(gameID, p: pair.Value);
                if (success)
                {
                    Globals_Game.positionKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "positionKeys", Globals_Game.positionKeys);

            // ========= write NPCs
            // clear existing key list
            if (Globals_Game.npcKeys.Count > 0)
            {
                Globals_Game.npcKeys.Clear();
            }

            // write each object in npcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, NonPlayerCharacter> pair in Globals_Game.npcMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_NPC(gameID, npc: pair.Value);
                if (success)
                {
                    Globals_Game.npcKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "npcKeys", Globals_Game.npcKeys);

            // ========= write PCs
            // clear existing key list
            if (Globals_Game.pcKeys.Count > 0)
            {
                Globals_Game.pcKeys.Clear();
            }

            // write each object in pcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, PlayerCharacter> pair in Globals_Game.pcMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_PC(gameID, pc: pair.Value);
                if (success)
                {
                    Globals_Game.pcKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "pcKeys", Globals_Game.pcKeys);

            // ========= write KINGDOMS
            // clear existing key list
            if (Globals_Game.kingKeys.Count > 0)
            {
                Globals_Game.kingKeys.Clear();
            }

            // write each object in kingdomMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Kingdom> pair in Globals_Game.kingdomMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Kingdom(gameID, k: pair.Value);
                if (success)
                {
                    Globals_Game.kingKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "kingKeys", Globals_Game.kingKeys);

            // ========= write PROVINCES
            // clear existing key list
            if (Globals_Game.provKeys.Count > 0)
            {
                Globals_Game.provKeys.Clear();
            }

            // write each object in provinceMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Province> pair in Globals_Game.provinceMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Province(gameID, p: pair.Value);
                if (success)
                {
                    Globals_Game.provKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "provKeys", Globals_Game.provKeys);

            // ========= write TERRAINS
            // clear existing key list
            if (Globals_Game.terrKeys.Count > 0)
            {
                Globals_Game.terrKeys.Clear();
            }

            // write each object in terrainMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Terrain> pair in Globals_Game.terrainMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Terrain(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.terrKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "terrKeys", Globals_Game.terrKeys);

            // ========= write VICTORYDATA OBJECTS
            // clear existing key list
            if (Globals_Game.victoryDataKeys.Count > 0)
            {
                Globals_Game.victoryDataKeys.Clear();
            }

            // write each object in victoryData, whilst also repopulating key list
            foreach (KeyValuePair<string, VictoryData> pair in Globals_Game.victoryData)
            {
                bool success = DatabaseWrite.DatabaseWrite_VictoryData(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.victoryDataKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "victoryDataKeys", Globals_Game.victoryDataKeys);

            // ========= write FIEFS
            // clear existing key list
            if (Globals_Game.fiefKeys.Count > 0)
            {
                Globals_Game.fiefKeys.Clear();
            }

            // write each object in fiefMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Fief> pair in Globals_Game.fiefMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Fief(gameID, f: pair.Value);
                if (success)
                {
                    Globals_Game.fiefKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "fiefKeys", Globals_Game.fiefKeys);

            // ========= write ARMIES
            // clear existing key list
            if (Globals_Game.armyKeys.Count > 0)
            {
                Globals_Game.armyKeys.Clear();
            }

            // write each object in armyMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Army> pair in Globals_Game.armyMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Army(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.armyKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "armyKeys", Globals_Game.armyKeys);

            // ========= write SIEGES
            // clear existing key list
            if (Globals_Game.siegeKeys.Count > 0)
            {
                Globals_Game.siegeKeys.Clear();
            }

            // write each object in siegeMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Siege> pair in Globals_Game.siegeMasterList)
            {
                bool success = DatabaseWrite.DatabaseWrite_Siege(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.siegeKeys.Add(pair.Key);
                }
            }

            // write key list to database
            DatabaseWrite.DatabaseWrite_KeyList(gameID, "siegeKeys", Globals_Game.siegeKeys);

            // ========= write MAP (edges collection)
            DatabaseWrite.DatabaseWrite_MapEdges(gameID, map: Globals_Game.gameMap);

            foreach (KeyValuePair<string, Client> pair in Globals_Server.clients)
            {
                DatabaseWrite.DatabaseWrite_Journal(gameID, pair.Value.user+"_journal", pair.Value.myPastEvents);
                // Globals_Client.myPlayerCharacter
                if (pair.Value.myPlayerCharacter != null)
                {
                    DatabaseWrite.DatabaseWrite_String(gameID, pair.Value.user+"_playerChar", pair.Value.myPlayerCharacter.charID);
                }

                // Globals_Client.showMessages
                DatabaseWrite.DatabaseWrite_Bool(gameID, pair.Value.user+"_showMessages", pair.Value.showMessages);
                // Globals_Client.showDebugMessages
                DatabaseWrite.DatabaseWrite_Bool(gameID, pair.Value.user+"_showDebugMessages", pair.Value.showDebugMessages);
            }

        }

        /// <summary>
        /// Writes a key list (List object) to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">key of key list</param>
        /// <param name="kl">key list to write</param>
        public static bool DatabaseWrite_KeyList<T>(string gameID, string k, List<T> kl)
        {

            RiakClient.Models.RiakObject rList = new RiakObject(gameID, k, kl);
            var putListResult = Globals_Server.rClient.Put(rList);

            if (!putListResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Key list " + rList.Key + " to bucket " + rList.Bucket);
            }

            return putListResult.IsSuccess;
        }

        /// <summary>
        /// Writes a GameClock object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="gc">GameClock to write</param>
        public static bool DatabaseWrite_Clock(string gameID, GameClock gc)
        {
            var rClock = new RiakObject(gameID, "gameClock", gc);
            var putClockResult = Globals_Server.rClient.Put(rClock);

            if (!putClockResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: GameClock to bucket " + rClock.Bucket);
            }

            return putClockResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Dictionary object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="dictionary">Dictionary to write</param>
        public static bool DatabaseWrite_Dictionary<T>(string gameID, string key, T dictionary)
        {
            var rDict = new RiakObject(gameID, key, dictionary);
            var putDictResult = Globals_Server.rClient.Put(rDict);

            if (!putDictResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Dictionary " + key + " to bucket " + rDict.Bucket);
            }

            return putDictResult.IsSuccess;
        }

        /// <summary>
        /// Serialises jEntryPriorities Dictionary
        /// </summary>
        /// <returns>Dictionary(string, byte) for database storage</returns>
        /// <param name="dictToConvert">The Dictionary to convert</param>
        public static Dictionary<string, byte> JentryPriorities_serialise(Dictionary<string[], byte> dictToConvert)
        {
            Dictionary<string, byte> dictOut = new Dictionary<string, byte>();

            if (dictToConvert.Count > 0)
            {
                foreach (KeyValuePair<string[], byte> thisEntry in dictToConvert)
                {
                    dictOut.Add(thisEntry.Key[0] + "|" + thisEntry.Key[1], thisEntry.Value);
                }
            }

            return dictOut;
        }

        /// <summary>
        /// Writes a Journal object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="journal">Journal to write</param>
        public static bool DatabaseWrite_Journal(string gameID, string key, Journal journal)
        {
            var rJournal = new RiakObject(gameID, key, journal);
            var putJournalResult = Globals_Server.rClient.Put(rJournal);

            if (!putJournalResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Journal " + key + " to bucket " + rJournal.Bucket);
            }

            return putJournalResult.IsSuccess;
        }

        public static bool DatabaseWrite_User(string gameID, string key, string pc)
        {
            var rString = new RiakObject(gameID, key, pc);
            var putStringResult = Globals_Server.rClient.Put(rString);
            if (!putStringResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: String variable " + key + " to bucket " + rString.Bucket);
            }

            return putStringResult.IsSuccess;
        }
        /// <summary>
        /// Writes a string variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="pcID">string to write</param>
        public static bool DatabaseWrite_String(string gameID, string key, string pcID)
        {
            pcID = "\"" + pcID + "\"";

            var rString = new RiakObject(gameID, key, pcID);
            var putStringResult = Globals_Server.rClient.Put(rString);

            if (!putStringResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: String variable " + key + " to bucket " + rString.Bucket);
            }

            return putStringResult.IsSuccess;
        }

        /// <summary>
        /// Writes a newID variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="newID">newID to write</param>
        public static bool DatabaseWrite_newID(string gameID, string key, uint newID)
        {
            var rCharVar = new RiakObject(gameID, key, newID);
            var putCharVarResult = Globals_Server.rClient.Put(rCharVar);

            if (!putCharVarResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: newID variable " + key + " to bucket " + rCharVar.Bucket);
            }

            return putCharVarResult.IsSuccess;
        }

        /// <summary>
        /// Writes a bool variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="thisBool">bool to write</param>
        public static bool DatabaseWrite_Bool(string gameID, string key, bool thisBool)
        {
            var rBool = new RiakObject(gameID, key, thisBool);
            var putBoolResult = Globals_Server.rClient.Put(rBool);

            if (!putBoolResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Bool variable " + key + " to bucket " + rBool.Bucket);
            }

            return putBoolResult.IsSuccess;
        }

        /// <summary>
        /// Writes aTtrait object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="t">Trait to write</param>
        public static bool DatabaseWrite_Trait(string gameID, Trait t)
        {
            var rTrait = new RiakObject(gameID, t.id, t);
            var putTraitResult = Globals_Server.rClient.Put(rTrait);

            if (!putTraitResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Trait " + rTrait.Key + " to bucket " + rTrait.Bucket);
            }

            return putTraitResult.IsSuccess;
        }

        /// <summary>
        /// Writes a BaseLanguage object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="bl">BaseLanguage to write</param>
        public static bool DatabaseWrite_BaseLanguage(string gameID, BaseLanguage bl)
        {

            var rBaseLanguage = new RiakObject(gameID, bl.id, bl);
            var putBaseLanguageResult = Globals_Server.rClient.Put(rBaseLanguage);

            if (!putBaseLanguageResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: BaseLanguage " + rBaseLanguage.Key + " to bucket " + rBaseLanguage.Bucket);
            }

            return putBaseLanguageResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Language or Language_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="l">Language to write</param>
        /// <param name="ls">Language_Serialised to write</param>
        public static bool DatabaseWrite_Language(string gameID, Language l = null, Language_Serialised ls = null)
        {
            if (l != null)
            {
                // convert Language into Language_Serialised
                ls = DatabaseWrite.Language_serialise(l);
            }

            // write Language_Serialised to database
            var rLanguage = new RiakObject(gameID, ls.id, ls);
            var putLanguageResult = Globals_Server.rClient.Put(rLanguage);

            if (!putLanguageResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Language " + rLanguage.Key + " to bucket " + rLanguage.Bucket);
            }

            return putLanguageResult.IsSuccess;
        }

        /// <summary>
        /// serialises a Language object
        /// </summary>
        /// <returns>Language_Serialised object</returns>
        /// <param name="l">Language to be converted</param>
        public static Language_Serialised Language_serialise(Language l)
        {
            Language_Serialised langSerialisedOut = null;
            langSerialisedOut = new Language_Serialised(l);
            return langSerialisedOut;
        }

        /// <summary>
        /// Writes a Nationality object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="n">Nationality to write</param>
        public static bool DatabaseWrite_Nationality(string gameID, Nationality n)
        {

            var rNationality = new RiakObject(gameID, n.natID, n);
            var putNationalityResult = Globals_Server.rClient.Put(rNationality);

            if (!putNationalityResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Nationality " + rNationality.Key + " to bucket " + rNationality.Bucket);
            }

            return putNationalityResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Rank object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="r">Rank to write</param>
        public static bool DatabaseWrite_Rank(string gameID, Rank r)
        {

            var rRank = new RiakObject(gameID, r.id.ToString(), r);
            var putRankResult = Globals_Server.rClient.Put(rRank);

            if (!putRankResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Rank " + rRank.Key + " to bucket " + rRank.Bucket);
            }

            return putRankResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Position or Position_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Position to write</param>
        /// <param name="ps">Position_Serialised to write</param>
        public static bool DatabaseWrite_Position(string gameID, Position p = null, Position_Serialised ps = null)
        {
            if (p != null)
            {
                // convert Position into Position_Serialised
                ps = DatabaseWrite.Position_serialise(p);
            }

            var rPos = new RiakObject(gameID, ps.id.ToString(), ps);
            var putPosResult = Globals_Server.rClient.Put(rPos);

            if (!putPosResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Position " + rPos.Key + " to bucket " + rPos.Bucket);
            }

            return putPosResult.IsSuccess;
        }

        /// <summary>
        /// serialises a Position object
        /// </summary>
        /// <returns>Position_Serialised object</returns>
        /// <param name="p">Position to be converted</param>
        public static Position_Serialised Position_serialise(Position p)
        {
            Position_Serialised posOut = null;
            posOut = new Position_Serialised(p);
            return posOut;
        }

        /// <summary>
        /// Writes a NonPlayerCharacter or NonPlayerCharacter_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="npc">NonPlayerCharacter to write</param>
        /// <param name="npcs">NonPlayerCharacter_Serialised to write</param>
        public static bool DatabaseWrite_NPC(string gameID, NonPlayerCharacter npc = null, NonPlayerCharacter_Serialised npcs = null)
        {
            if (npc != null)
            {
                // convert NonPlayerCharacter into NonPlayerCharacter_Serialised
                npcs = DatabaseWrite.NPC_serialise(npc);
            }

            // write NonPlayerCharacter_Serialised to database
            var rNPC = new RiakObject(gameID, npcs.charID, npcs);
            var putNPCresult = Globals_Server.rClient.Put(rNPC);

            if (!putNPCresult.IsSuccess)
            {
                Globals_Server.logError("Write failed: NPC " + rNPC.Key + " to bucket " + rNPC.Bucket);
            }

            return putNPCresult.IsSuccess;
        }

        /// <summary>
        /// Serialises a NonPlayerCharacter object
        /// </summary>
        /// <returns>NonPlayerCharacter_Serialised object</returns>
        /// <param name="npc">NonPlayerCharacter to be converted</param>
        public static NonPlayerCharacter_Serialised NPC_serialise(NonPlayerCharacter npc)
        {
            NonPlayerCharacter_Serialised npcSerialisedOut = null;
            npcSerialisedOut = new NonPlayerCharacter_Serialised(npc);
            return npcSerialisedOut;
        }

        /// <summary>
        /// Writes a PlayerCharacter or PlayerCharacter_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="pc">PlayerCharacter to write</param>
        /// <param name="pcs">PlayerCharacter_Serialised to write</param>
        public static bool DatabaseWrite_PC(string gameID, PlayerCharacter pc = null, PlayerCharacter_Serialised pcs = null)
        {
            if (pc != null)
            {
                // convert PlayerCharacter into PlayerCharacter_Serialised
                pcs = DatabaseWrite.PC_serialise(pc);
            }

            // write PlayerCharacter_Serialised to database
            var rPC = new RiakObject(gameID, pcs.charID, pcs);
            var putPCresult = Globals_Server.rClient.Put(rPC);

            if (!putPCresult.IsSuccess)
            {
                Globals_Server.logError("Write failed: PC " + rPC.Key + " to bucket " + rPC.Bucket);
            }

            return putPCresult.IsSuccess;
        }

        /// <summary>
        /// Serialises a PlayerCharacter object
        /// </summary>
        /// <returns>PlayerCharacter_Serialised object</returns>
        /// <param name="pc">PlayerCharacter to be converted</param>
        public static PlayerCharacter_Serialised PC_serialise(PlayerCharacter pc)
        {
            PlayerCharacter_Serialised pcSerialisedOut = null;
            pcSerialisedOut = new PlayerCharacter_Serialised(pc);
            return pcSerialisedOut;
        }

        /// <summary>
        /// Writes a Kingdom or Kingdom_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">Kingdom to write</param>
        /// <param name="ks">Kingdom_Serialised to write</param>
        public static bool DatabaseWrite_Kingdom(string gameID, Kingdom k = null, Kingdom_Serialised ks = null)
        {
            if (k != null)
            {
                // convert Kingdom into Kingdom_Serialised
                ks = DatabaseWrite.Kingdom_serialise(k);
            }

            var rKing = new RiakObject(gameID, ks.id, ks);
            var putKingResult = Globals_Server.rClient.Put(rKing);

            if (!putKingResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Kingdom " + rKing.Key + " to bucket " + rKing.Bucket);
            }

            return putKingResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Kingdom object
        /// </summary>
        /// <returns>Kingdom_Serialised object</returns>
        /// <param name="k">Kingdom to be converted</param>
        public static Kingdom_Serialised Kingdom_serialise(Kingdom k)
        {
            Kingdom_Serialised kingOut = null;
            kingOut = new Kingdom_Serialised(k);
            return kingOut;
        }

        /// <summary>
        /// Writes a Province or Province_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Province to write</param>
        /// <param name="ps">Province_Serialised to write</param>
        public static bool DatabaseWrite_Province(string gameID, Province p = null, Province_Serialised ps = null)
        {
            if (p != null)
            {
                // convert Province into Province_Serialised
                ps = DatabaseWrite.Province_serialise(p);
            }

            var rProv = new RiakObject(gameID, ps.id, ps);
            var putProvResult = Globals_Server.rClient.Put(rProv);

            if (!putProvResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Province " + rProv.Key + " to bucket " + rProv.Bucket);
            }

            return putProvResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Province object
        /// </summary>
        /// <returns>Province_Serialised object</returns>
        /// <param name="p">Province to be converted</param>
        public static Province_Serialised Province_serialise(Province p)
        {
            Province_Serialised oOut = null;
            oOut = new Province_Serialised(p);
            return oOut;
        }

        /// <summary>
        /// Writes a Terrain object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="t">Terrain to write</param>
        public static bool DatabaseWrite_Terrain(string gameID, Terrain t)
        {

            var rTerrain = new RiakObject(gameID, t.id, t);
            var putTerrainResult = Globals_Server.rClient.Put(rTerrain);

            if (!putTerrainResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Terrain " + rTerrain.Key + " to bucket " + rTerrain.Bucket);
            }

            return putTerrainResult.IsSuccess;
        }

        /// <summary>
        /// Writes a VictoryData object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="vicDat">VictoryData to write</param>
        public static bool DatabaseWrite_VictoryData(string gameID, VictoryData vicDat)
        {

            var rVictoryData = new RiakObject(gameID, vicDat.playerID, vicDat);
            var putVictoryDataResult = Globals_Server.rClient.Put(rVictoryData);

            if (!putVictoryDataResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: VictoryData " + rVictoryData.Key + " to bucket " + rVictoryData.Bucket);
            }

            return putVictoryDataResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Fief or Fief_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="f">Fief to write</param>
        /// <param name="fs">Fief_Serialised to write</param>
        public static bool DatabaseWrite_Fief(string gameID, Fief f = null, Fief_Serialised fs = null)
        {
            if (f != null)
            {
                // convert Fief to Fief_Serialised
                fs = DatabaseWrite.Fief_serialise(f);
            }

            var rFief = new RiakObject(gameID, fs.id, fs);
            var putFiefResult = Globals_Server.rClient.Put(rFief);

            if (!putFiefResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Fief " + rFief.Key + " to bucket " + rFief.Bucket);
            }

            return putFiefResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Fief object
        /// </summary>
        /// <returns>Fief_Serialised object</returns>
        /// <param name="f">Fief to be converted</param>
        public static Fief_Serialised Fief_serialise(Fief f)
        {
            Fief_Serialised fOut = null;
            fOut = new Fief_Serialised(f);
            return fOut;
        }

        /// <summary>
        /// Writes an Army object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="a">Army to write</param>
        public static bool DatabaseWrite_Army(string gameID, Army a)
        {
            var rArmy = new RiakObject(gameID, a.armyID, a);
            var putArmyResult = Globals_Server.rClient.Put(rArmy);

            if (!putArmyResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Army " + rArmy.Key + " to bucket " + rArmy.Bucket);
            }

            return putArmyResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Siege object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="s">Siege to write</param>
        public static bool DatabaseWrite_Siege(string gameID, Siege s)
        {
            var rSiege = new RiakObject(gameID, s.siegeID, s);
            var putSiegeResult = Globals_Server.rClient.Put(rSiege);

            if (!putSiegeResult.IsSuccess)
            {
                Globals_Server.logError("Write failed: Siege " + rSiege.Key + " to bucket " + rSiege.Bucket);
            }

            return putSiegeResult.IsSuccess;
        }

        /// <summary>
        /// Writes a HexMapGraph edges collection to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="map">HexMapGraph containing edges collection to write</param>
        /// <param name="edges">Edges collection to write</param>
        public static bool DatabaseWrite_MapEdges(string gameID, HexMapGraph map = null, List<TaggedEdge<string, string>> edges = null)
        {
            if (map != null)
            {
                // convert Language into Language_Serialised
                edges = DatabaseWrite.EdgeCollection_serialise(map.myMap.Edges.ToList());
            }

            var rMapE = new RiakObject(gameID, "mapEdges", edges);
            var putMapResultE = Globals_Server.rClient.Put(rMapE);

            if (!putMapResultE.IsSuccess)
            {
                Globals_Server.logError("Write failed: Map edges collection " + rMapE.Key + " to bucket " + rMapE.Bucket);
            }

            return putMapResultE.IsSuccess;
        }

        /// <summary>
        /// Serialises HexMapGraph edges collection
        /// </summary>
        /// <returns>Serialised edges collection</returns>
        /// <param name="edgesIn">Edges collection to be converted</param>
        public static List<TaggedEdge<string, string>> EdgeCollection_serialise(List<TaggedEdge<Fief, string>> edgesIn)
        {
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();

            foreach (TaggedEdge<Fief, string> element in edgesIn)
            {
                // convert each Fief object to string ID
                edgesOut.Add(DatabaseWrite.EdgeFief_serialise(element));
            }

            return edgesOut;
        }

        /// <summary>
        /// Serialises HexMapGraph edge object
        /// </summary>
        /// <returns>Serialised edge</returns>
        /// <param name="te">HexMapGraph edge to be converted</param>
        public static TaggedEdge<string, string> EdgeFief_serialise(TaggedEdge<Fief, string> te)
        {
            TaggedEdge<string, string> edgeOut = new TaggedEdge<string, string>(te.Source.id, te.Target.id, te.Tag);
            return edgeOut;
        }
    }
}
