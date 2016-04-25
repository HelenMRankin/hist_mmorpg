using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace hist_mmorpg
{
    /// <summary>
    ///     Class representing game
    ///     It will initialise all the game objects and control the flow of the game
    ///     The control code from all the form classes will either be put in here or accessed from here
    ///     For example, if a server gets an incoming message from a client and finds that it is some game action to be taken,
    ///     it might send this to Game, where the precise action will be determined from the message, carried out and the
    ///     result send back via server
    /// </summary>
#if V_GAME
    [ContractVerification(true)]
#endif
    public class Game
    {
        /// <summary>
        ///     Create a new game
        /// </summary>
        public Game()
        {
            Globals_Game.game = this;
            // initialise game objects
            // This path handling should ensure that the correct path will be found in Linux, Windows or debug mode
            /*String dir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            // if the program is being run in debug mode, this will obtain the correct directory
            if (dir.Contains("bin"))
            {
                dir = Directory.GetParent(dir).FullName;
            }*/

            var dir = Directory.GetCurrentDirectory();
            dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            var path = Path.Combine(dir, "RepairHist_mmo", "CSVs");
            var gameObjects = Path.Combine(path, "gameObjects.csv");
            var mapData = Path.Combine(path, "map.csv");
            InitGameObjects("testBucket", gameObjects, mapData,
                start: 1194, king1: "Char_47", king2: "Char_40", herald1: "Char_1", sysAdmin: null);
        }

        /// <summary>
        ///     Initialises all game objects
        /// </summary>
        /// <param name="gameID">gameID of the game</param>
        /// <param name="objectDataFile">Name of file containing game object CSV data</param>
        /// <param name="mapDataFile">Name of file containing map CSV data</param>
        /// <param name="type">Game type</param>
        /// <param name="duration">Game duration (years)</param>
        /// <param name="start">Start year</param>
        /// <param name="king1">ID of PlayerCharacter in role of kingOne</param>
        /// <param name="king2">ID of PlayerCharacter in role of kingTwo</param>
        /// <param name="herald1">ID of PlayerCharacter in role of heraldOne</param>
        /// <param name="herald2">ID of PlayerCharacter in role of heraldTwo</param>
        /// <param name="sysAdmin">ID of PlayerCharacter in role of sysAdmin</param>
        [ContractVerification(false)]
        public void InitGameObjects(string gameID = null, string objectDataFile = null,
            string mapDataFile = null, uint type = 0, uint duration = 100, uint start = 1337, string king1 = null,
            string king2 = null, string herald1 = null, string herald2 = null, string sysAdmin = null)
        {
            var dataLoaded = false;

            // LOAD DATA
            // database
            if (Globals_Game.loadFromDatabase)
            {
                // load objects
                DatabaseRead.DatabaseReadAll(gameID);

                dataLoaded = true;
            }
            // CSV file
            else if (Globals_Game.loadFromCSV)
            {
                // load objects (mainly) from CSV file
                if (!string.IsNullOrWhiteSpace(objectDataFile))
                {
                    // load objects
                    CSVimport.NewGameFromCSV(objectDataFile, mapDataFile, start);

                    // initialise Globals_Game.victoryData
                    SynchroniseVictoryData();

                    dataLoaded = true;
                }
            }
            // from code (for quick testing)
            if (!dataLoaded)
            {
                // load objects
                LoadFromCode();

                // initialise Globals_Game.victoryData
                SynchroniseVictoryData();
            }

            if ((!Globals_Game.loadFromDatabase) || (!dataLoaded))
            {
                // INITIALISE ROLES
                // set kings
                if (!string.IsNullOrWhiteSpace(king1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king1))
                    {
                        Globals_Game.kingOne = Globals_Game.pcMasterList[king1];
                    }
                }
                if (!string.IsNullOrWhiteSpace(king2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king2))
                    {
                        Globals_Game.kingTwo = Globals_Game.pcMasterList[king2];
                    }
                }

                // set heralds
                if (!string.IsNullOrWhiteSpace(herald1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald1))
                    {
                        Globals_Game.heraldOne = Globals_Game.pcMasterList[herald1];
                    }
                }
                if (!string.IsNullOrWhiteSpace(herald2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald2))
                    {
                        Globals_Game.heraldTwo = Globals_Game.pcMasterList[herald2];
                    }
                }

                // set sysAdmin
                if (!string.IsNullOrWhiteSpace(sysAdmin))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(sysAdmin))
                    {
                        Globals_Game.sysAdmin = Globals_Game.pcMasterList[sysAdmin];
                    }
                }

                // SET GAME PARAMETERS
                // game type
                if (type != 0)
                {
                    Globals_Game.gameType = type;
                }

                // start date
                if (start != 1337)
                {
                    Globals_Game.startYear = start;
                }

                // duration
                if (duration != 100)
                {
                    Globals_Game.duration = duration;
                }
            }

            // =================== TESTING

            // create and add army
            /* uint[] myArmyTroops1 = new uint[] { 5, 10, 0, 30, 40, 4000, 6020 };
             Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
             myArmy1.AddArmy();
             addTestCharacter();
             // create and add army
             uint[] myArmyTroops2 = new uint[] { 5, 10, 0, 30, 40, 80, 220 };
             Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
             myArmy2.AddArmy(); */
            /*
                        uint[] myArmyTroops1 = new uint[] { 20, 20, 15, 5, 5, 100, 100 };
                        Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
                        myArmy1.AddArmy();

                        addTestCharacter();
                        // create and add army
                        uint[] myArmyTroops2 = new uint[] { 10, 10, 10, 10, 25, 0, 0 };
                        Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
                        myArmy2.AddArmy(); */
            /*
            // create ailment
            Ailment myAilment = new Ailment(Globals_Game.getNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, 1, 0);
            Globals_Game.pcMasterList["Char_196"].ailments.Add(myAilment.ailmentID, myAilment); */
            // =================== END TESTING
        }

        /// <summary>
        ///     Creates some game objects from code (temporary, for testing)
        /// </summary>
        /// <param name="start">Start year</param>
        [ContractVerification(false)]
        public void LoadFromCode(uint start = 1337)
        {
            // create GameClock
            Globals_Game.clock = new GameClock("clock_1", start);

            // create traits
            // Dictionary of trait effects
            var effectsCommand = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.BATTLE, 0.4},
                {Globals_Game.Stats.SIEGE, 0.4},
                {Globals_Game.Stats.NPCHIRE, 0.2}
            };
            // create trait
            var command = new Trait("trait_1", "Command", effectsCommand);
            // add to traitCollection
            Globals_Game.traitMasterList.Add(command.id, command);

            var effectsChivalry = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.FAMEXPENSE, 0.2},
                {Globals_Game.Stats.FIEFEXPENSE, 0.1},
                {Globals_Game.Stats.FIEFLOY, 0.2},
                {Globals_Game.Stats.NPCHIRE, 0.1},
                {Globals_Game.Stats.SIEGE, 0.1}
            };
            var chivalry = new Trait("trait_2", "Chivalry", effectsChivalry);
            Globals_Game.traitMasterList.Add(chivalry.id, chivalry);

            var effectsAbrasiveness = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.BATTLE, 0.15},
                {Globals_Game.Stats.DEATH, 0.05},
                {Globals_Game.Stats.FIEFEXPENSE, -0.05},
                {Globals_Game.Stats.FAMEXPENSE, 0.05},
                {Globals_Game.Stats.TIME, 0.05},
                {Globals_Game.Stats.SIEGE, -0.1}
            };
            var abrasiveness = new Trait("trait_3", "Abrasiveness", effectsAbrasiveness);
            Globals_Game.traitMasterList.Add(abrasiveness.id, abrasiveness);

            var effectsAccountancy = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.TIME, 0.1},
                {Globals_Game.Stats.FIEFEXPENSE, -0.2},
                {Globals_Game.Stats.FAMEXPENSE, -0.2},
                {Globals_Game.Stats.FIEFLOY, -0.05}
            };
            var accountancy = new Trait("trait_4", "Accountancy", effectsAccountancy);
            Globals_Game.traitMasterList.Add(accountancy.id, accountancy);

            var effectsStupidity = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.BATTLE, -0.4},
                {Globals_Game.Stats.DEATH, 0.05},
                {Globals_Game.Stats.FIEFEXPENSE, 0.2},
                {Globals_Game.Stats.FAMEXPENSE, 0.2},
                {Globals_Game.Stats.FIEFLOY, -0.1},
                {Globals_Game.Stats.NPCHIRE, -0.1},
                {Globals_Game.Stats.TIME, -0.1},
                {Globals_Game.Stats.SIEGE, -0.4},
                {Globals_Game.Stats.STEALTH, -0.6}
            };
            var stupidity = new Trait("trait_5", "Stupidity", effectsStupidity);
            Globals_Game.traitMasterList.Add(stupidity.id, stupidity);

            var effectsRobust = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.VIRILITY, 0.2},
                {Globals_Game.Stats.NPCHIRE, 0.05},
                {Globals_Game.Stats.FIEFLOY, 0.05},
                {Globals_Game.Stats.DEATH, -0.2}
            };
            var robust = new Trait("trait_6", "Robust", effectsRobust);
            Globals_Game.traitMasterList.Add(robust.id, robust);

            var effectsPious = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.VIRILITY, -0.2},
                {Globals_Game.Stats.NPCHIRE, 0.1},
                {Globals_Game.Stats.FIEFLOY, 0.1},
                {Globals_Game.Stats.TIME, -0.1}
            };
            var pious = new Trait("trait_7", "Pious", effectsPious);
            Globals_Game.traitMasterList.Add(pious.id, pious);

            var effectsParanoia = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.VIRILITY, -0.3},
                {Globals_Game.Stats.PERCEPTION, 0.4},
                {Globals_Game.Stats.FIEFLOY, -0.05}
            };
            var paranoia = new Trait("trait_8", "Paranoia", effectsParanoia);
            Globals_Game.traitMasterList.Add(paranoia.id, paranoia);

            var effectsCunning = new Dictionary<Globals_Game.Stats, double>
            {
                {Globals_Game.Stats.PERCEPTION, 0.1},
                {Globals_Game.Stats.STEALTH, 0.3}
            };
            var cunning = new Trait("trait_9", "Cunning", effectsCunning);
            Globals_Game.traitMasterList.Add(cunning.id, cunning);

            // add each traitsCollection key to traitsKeys
            foreach (var entry in Globals_Game.traitMasterList)
            {
                Globals_Game.traitKeys.Add(entry.Key);
            }

            // create BaseLanguage & Language objects
            var c = new BaseLanguage("lang_C", "Celtic");
            Globals_Game.baseLanguageMasterList.Add(c.id, c);
            var c1 = new Language(c, 1);
            Globals_Game.languageMasterList.Add(c1.id, c1);
            var c2 = new Language(c, 2);
            Globals_Game.languageMasterList.Add(c2.id, c2);
            var f = new BaseLanguage("lang_F", "French");
            Globals_Game.baseLanguageMasterList.Add(f.id, f);
            var f1 = new Language(f, 1);
            Globals_Game.languageMasterList.Add(f1.id, f1);
            var e = new BaseLanguage("lang_E", "English");
            Globals_Game.baseLanguageMasterList.Add(e.id, e);
            var e1 = new Language(e, 1);
            Globals_Game.languageMasterList.Add(e1.id, e1);

            // create terrain objects
            var plains = new Terrain("terr_P", "Plains", 1);
            Globals_Game.terrainMasterList.Add(plains.id, plains);
            var hills = new Terrain("terr_H", "Hills", 1.5);
            Globals_Game.terrainMasterList.Add(hills.id, hills);
            var forrest = new Terrain("terr_F", "Forrest", 1.5);
            Globals_Game.terrainMasterList.Add(forrest.id, forrest);
            var mountains = new Terrain("terr_M", "Mountains", 15);
            Globals_Game.terrainMasterList.Add(mountains.id, mountains);
            var impassable_mountains = new Terrain("terr_MX", "Impassable mountains", 91);
            Globals_Game.terrainMasterList.Add(impassable_mountains.id, impassable_mountains);

            // create keep barred lists for fiefs
            var keep1BarChars = new List<string>();
            var keep2BarChars = new List<string>();
            var keep3BarChars = new List<string>();
            var keep4BarChars = new List<string>();
            var keep5BarChars = new List<string>();
            var keep6BarChars = new List<string>();
            var keep7BarChars = new List<string>();

            // create chars lists for fiefs
            var fief1Chars = new List<Character>();
            var fief2Chars = new List<Character>();
            var fief3Chars = new List<Character>();
            var fief4Chars = new List<Character>();
            var fief5Chars = new List<Character>();
            var fief6Chars = new List<Character>();
            var fief7Chars = new List<Character>();

            // create ranks for kingdoms, provinces, fiefs
            var myTitleName03 = new TitleName[4];
            myTitleName03[0] = new TitleName("lang_C1", "King");
            myTitleName03[1] = new TitleName("lang_C2", "King");
            myTitleName03[2] = new TitleName("lang_E1", "King");
            myTitleName03[3] = new TitleName("lang_F1", "Roi");
            var myRank03 = new Rank(3, myTitleName03, 6);
            Globals_Game.rankMasterList.Add(myRank03.id, myRank03);

            var myTitleName09 = new TitleName[4];
            myTitleName09[0] = new TitleName("lang_C1", "Prince");
            myTitleName09[1] = new TitleName("lang_C2", "Prince");
            myTitleName09[2] = new TitleName("lang_E1", "Prince");
            myTitleName09[3] = new TitleName("lang_F1", "Prince");
            var myRank09 = new Rank(9, myTitleName09, 4);
            Globals_Game.rankMasterList.Add(myRank09.id, myRank09);

            var myTitleName11 = new TitleName[4];
            myTitleName11[0] = new TitleName("lang_C1", "Earl");
            myTitleName11[1] = new TitleName("lang_C2", "Earl");
            myTitleName11[2] = new TitleName("lang_E1", "Earl");
            myTitleName11[3] = new TitleName("lang_F1", "Comte");
            var myRank11 = new Rank(11, myTitleName11, 4);
            Globals_Game.rankMasterList.Add(myRank11.id, myRank11);

            var myTitleName15 = new TitleName[4];
            myTitleName15[0] = new TitleName("lang_C1", "Baron");
            myTitleName15[1] = new TitleName("lang_C2", "Baron");
            myTitleName15[2] = new TitleName("lang_E1", "Baron");
            myTitleName15[3] = new TitleName("lang_F1", "Baron");
            var myRank15 = new Rank(15, myTitleName15, 2);
            Globals_Game.rankMasterList.Add(myRank15.id, myRank15);

            var myTitleName17 = new TitleName[4];
            myTitleName17[0] = new TitleName("lang_C1", "Lord");
            myTitleName17[1] = new TitleName("lang_C2", "Lord");
            myTitleName17[2] = new TitleName("lang_E1", "Lord");
            myTitleName17[3] = new TitleName("lang_F1", "Sire");
            var myRank17 = new Rank(17, myTitleName17, 1);
            Globals_Game.rankMasterList.Add(myRank17.id, myRank17);

            // create Nationality objects for Kingdoms, Characters and positions
            var nationality01 = new Nationality("Fr", "French");
            Globals_Game.nationalityMasterList.Add(nationality01.natID, nationality01);
            var nationality02 = new Nationality("Eng", "English");
            Globals_Game.nationalityMasterList.Add(nationality02.natID, nationality02);

            // create Positions
            var myTiName01 = new TitleName("lang_C1", "Keeper of the Privy Seal");
            var myTiName011 = new TitleName("lang_C2", "Keeper of the Privy Seal");
            var myTiName012 = new TitleName("lang_E1", "Keeper of the Privy Seal");
            TitleName[] myTitles01 = {myTiName01, myTiName011, myTiName012};
            var myPos01 = new Position(100, myTitles01, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos01.id, myPos01);
            var myTiName02 = new TitleName("lang_C1", "Lord High Steward");
            var myTiName021 = new TitleName("lang_C2", "Lord High Steward");
            var myTiName022 = new TitleName("lang_E1", "Lord High Steward");
            TitleName[] myTitles02 = {myTiName02, myTiName021, myTiName022};
            var myPos02 = new Position(101, myTitles02, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos02.id, myPos02);

            // create kingdoms for provinces
            var myKingdom1 = new Kingdom("E0000", "England", nationality02, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom1.id, myKingdom1);
            var myKingdom2 = new Kingdom("F0000", "France", nationality01, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom2.id, myKingdom2);

            // create provinces for fiefs
            var myProv = new Province("ESX00", "Sussex", 6.2, king: myKingdom1, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv.id, myProv);
            var myProv2 = new Province("ESR00", "Surrey", 6.2, king: myKingdom2, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv2.id, myProv2);

            // create financial arrays for fiefs
            double[] prevFin001 =
            {
                6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2,
                340894
            };
            double[] currFin001 =
            {
                5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2,
                350274
            };
            var prevFin002 = new double[14];
            var currFin002 = new double[14];
            var prevFin003 = new double[14];
            var currFin003 = new double[14];
            var prevFin004 = new double[14];
            var currFin004 = new double[14];
            var prevFin005 = new double[14];
            var currFin005 = new double[14];
            double[] prevFin006 =
            {
                6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2,
                340894
            };
            double[] currFin006 =
            {
                5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2,
                350274
            };
            var prevFin007 = new double[14];
            var currFin007 = new double[14];

            // create armies lists for fiefs
            var armies001 = new List<string>();
            var armies002 = new List<string>();
            var armies003 = new List<string>();
            var armies004 = new List<string>();
            var armies005 = new List<string>();
            var armies006 = new List<string>();
            var armies007 = new List<string>();

            // create troop transfer lists for fiefs
            var transfers001 = new Dictionary<string, ProtoDetachment>();
            var transfers002 = new Dictionary<string, ProtoDetachment>();
            var transfers003 = new Dictionary<string, ProtoDetachment>();
            var transfers004 = new Dictionary<string, ProtoDetachment>();
            var transfers005 = new Dictionary<string, ProtoDetachment>();
            var transfers006 = new Dictionary<string, ProtoDetachment>();
            var transfers007 = new Dictionary<string, ProtoDetachment>();

            // create barredNationalities for fiefs
            var barredNats01 = new List<string>();
            var barredNats02 = new List<string>();
            var barredNats03 = new List<string>();
            var barredNats04 = new List<string>();
            var barredNats05 = new List<string>();
            var barredNats06 = new List<string>();
            var barredNats07 = new List<string>();

            var myFief1 = new Fief("ESX02", "Cuckfield", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000,
                42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'C', c1, plains, fief1Chars, keep1BarChars,
                barredNats01, 0, 2000000, armies001, false, transfers001, false);
            Globals_Game.fiefMasterList.Add(myFief1.id, myFief1);
            var myFief2 = new Fief("ESX03", "Pulborough", null, null, myRank15, myProv, 10000, 3.50, 0.20, 50, 10, 10,
                1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'C', c1, hills, fief2Chars, keep2BarChars,
                barredNats02, 0, 4000, armies002, false, transfers002, false);
            Globals_Game.fiefMasterList.Add(myFief2.id, myFief2);
            var myFief3 = new Fief("ESX01", "Hastings", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000,
                42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', c1, plains, fief3Chars, keep3BarChars,
                barredNats03, 0, 100000, armies003, false, transfers003, false);
            Globals_Game.fiefMasterList.Add(myFief3.id, myFief3);
            var myFief4 = new Fief("ESX04", "Eastbourne", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10,
                12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', c1, plains, fief4Chars, keep4BarChars,
                barredNats04, 0, 100000, armies004, false, transfers004, false);
            Globals_Game.fiefMasterList.Add(myFief4.id, myFief4);
            var myFief5 = new Fief("ESX05", "Worthing", null, null, myRank15, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000,
                42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', f1, plains, fief5Chars, keep5BarChars,
                barredNats05, 0, 100000, armies005, false, transfers005, false);
            Globals_Game.fiefMasterList.Add(myFief5.id, myFief5);
            var myFief6 = new Fief("ESR03", "Reigate", null, null, myRank17, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000,
                42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', f1, plains, fief6Chars, keep6BarChars,
                barredNats06, 0, 100000, armies006, false, transfers006, false);
            Globals_Game.fiefMasterList.Add(myFief6.id, myFief6);
            var myFief7 = new Fief("ESR04", "Guilford", null, null, myRank15, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000,
                42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', f1, forrest, fief7Chars, keep7BarChars,
                barredNats07, 0, 100000, armies007, false, transfers007, false);
            Globals_Game.fiefMasterList.Add(myFief7.id, myFief7);

            // create QuickGraph undirected graph
            // 1. create graph
            var myHexMap = new HexMapGraph("map001");
            Globals_Game.gameMap = myHexMap;
            // 2. Add edge and auto create vertices
            // from myFief1
            myHexMap.AddHexesAndRoute(myFief1, myFief2, "W", (myFief1.terrain.travelCost + myFief2.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief1, myFief3, "E", (myFief1.terrain.travelCost + myFief3.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief1, myFief6, "NE",
                (myFief1.terrain.travelCost + myFief6.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief1, myFief4, "SE",
                (myFief1.terrain.travelCost + myFief4.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief1, myFief5, "SW",
                (myFief1.terrain.travelCost + myFief5.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief1, myFief7, "NW",
                (myFief1.terrain.travelCost + myFief7.terrain.travelCost)/2);
            // from myFief2
            myHexMap.AddHexesAndRoute(myFief2, myFief1, "E", (myFief2.terrain.travelCost + myFief1.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief2, myFief7, "NE",
                (myFief2.terrain.travelCost + myFief7.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief2, myFief5, "SE",
                (myFief2.terrain.travelCost + myFief5.terrain.travelCost)/2);
            // from myFief3
            myHexMap.AddHexesAndRoute(myFief3, myFief4, "SW",
                (myFief3.terrain.travelCost + myFief4.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief3, myFief6, "NW",
                (myFief3.terrain.travelCost + myFief6.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief3, myFief1, "W", (myFief3.terrain.travelCost + myFief1.terrain.travelCost)/2);
            // from myFief4
            myHexMap.AddHexesAndRoute(myFief4, myFief3, "NE",
                (myFief4.terrain.travelCost + myFief3.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief4, myFief1, "NW",
                (myFief4.terrain.travelCost + myFief1.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief4, myFief5, "W", (myFief4.terrain.travelCost + myFief5.terrain.travelCost)/2);
            // from myFief5
            myHexMap.AddHexesAndRoute(myFief5, myFief1, "NE",
                (myFief5.terrain.travelCost + myFief1.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief5, myFief2, "NW",
                (myFief5.terrain.travelCost + myFief2.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief5, myFief4, "E", (myFief5.terrain.travelCost + myFief4.terrain.travelCost)/2);
            // from myFief6
            myHexMap.AddHexesAndRoute(myFief6, myFief3, "SE",
                (myFief6.terrain.travelCost + myFief3.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief6, myFief1, "SW",
                (myFief6.terrain.travelCost + myFief1.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief6, myFief7, "W", (myFief6.terrain.travelCost + myFief7.terrain.travelCost)/2);
            // from myFief7
            myHexMap.AddHexesAndRoute(myFief7, myFief6, "E", (myFief7.terrain.travelCost + myFief6.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief7, myFief1, "SE",
                (myFief7.terrain.travelCost + myFief1.terrain.travelCost)/2);
            myHexMap.AddHexesAndRoute(myFief7, myFief2, "SW",
                (myFief7.terrain.travelCost + myFief2.terrain.travelCost)/2);

            // create goTo queues for characters
            var myGoTo1 = new Queue<Fief>();
            var myGoTo2 = new Queue<Fief>();
            var myGoTo3 = new Queue<Fief>();
            var myGoTo4 = new Queue<Fief>();
            var myGoTo5 = new Queue<Fief>();
            var myGoTo6 = new Queue<Fief>();
            var myGoTo7 = new Queue<Fief>();
            var myGoTo8 = new Queue<Fief>();
            var myGoTo9 = new Queue<Fief>();
            var myGoTo10 = new Queue<Fief>();
            var myGoTo11 = new Queue<Fief>();
            var myGoTo12 = new Queue<Fief>();

            // add some goTo entries for myChar1
            //myGoTo1.Enqueue (myFief2);
            //myGoTo1.Enqueue (myFief7);

            // create entourages for PCs
            var myEmployees1 = new List<NonPlayerCharacter>();
            var myEmployees2 = new List<NonPlayerCharacter>();

            // create lists of fiefs owned by PCs and add some fiefs
            var myFiefsOwned1 = new List<Fief>();
            var myFiefsOwned2 = new List<Fief>();

            // create lists of provinces owned by PCs and add some fiefs
            var myProvsOwned1 = new List<Province>();
            var myProvsOwned2 = new List<Province>();

            // create DOBs for characters
            var myDob001 = new Tuple<uint, byte>(1161, 1);
            var myDob002 = new Tuple<uint, byte>(1134, 0);
            var myDob003 = new Tuple<uint, byte>(1152, 2);
            var myDob004 = new Tuple<uint, byte>(1169, 3);
            var myDob005 = new Tuple<uint, byte>(1167, 2);
            var myDob006 = new Tuple<uint, byte>(1159, 2);
            var myDob007 = new Tuple<uint, byte>(1159, 3);
            var myDob008 = new Tuple<uint, byte>(1181, 2);
            var myDob009 = new Tuple<uint, byte>(1179, 0);
            var myDob010 = new Tuple<uint, byte>(1179, 0);
            var myDob011 = new Tuple<uint, byte>(1176, 1);
            var myDob012 = new Tuple<uint, byte>(1177, 3);

            // create titles list for characters
            var myTitles001 = new List<string>();
            var myTitles002 = new List<string>();
            var myTitles003 = new List<string>();
            var myTitles004 = new List<string>();
            var myTitles005 = new List<string>();
            var myTitles006 = new List<string>();
            var myTitles007 = new List<string>();
            var myTitles008 = new List<string>();
            var myTitles009 = new List<string>();
            var myTitles010 = new List<string>();
            var myTitles011 = new List<string>();
            var myTitles012 = new List<string>();

            // create armies list for PCs
            var myArmies001 = new List<Army>();
            var myArmies002 = new List<Army>();

            // create sieges list for PCs
            var mySieges001 = new List<string>();
            var mySieges002 = new List<string>();

            // create some characters
            var myChar1 = new PlayerCharacter("Char_47", "Dave", "Bond", myDob001, true, nationality02, true, 8.50, 9.0,
                myGoTo1, c1, 90, 0, 7.2, 6.1, Utility_Methods.GenerateTraitSet(), false, false, "Char_47", "Char_403",
                null, null, false, 13000, myEmployees1, myFiefsOwned1, myProvsOwned1, "ESX02", "ESX02", myTitles001,
                myArmies001, mySieges001, null, loc: myFief1, pID: "libdab");
            Globals_Game.pcMasterList.Add(myChar1.charID, myChar1);
            var myChar2 = new PlayerCharacter("Char_40", "Bave", "Dond", myDob002, true, nationality01, true, 8.50, 6.0,
                myGoTo2, f1, 90, 0, 5.0, 4.5, Utility_Methods.GenerateTraitSet(), false, false, "Char_40", null, null,
                null, false, 13000, myEmployees2, myFiefsOwned2, myProvsOwned2, "ESR03", "ESR03", myTitles002,
                myArmies002, mySieges002, null, loc: myFief7, pID: "otherGuy");
            Globals_Game.pcMasterList.Add(myChar2.charID, myChar2);
            var myNPC1 = new NonPlayerCharacter("Char_401", "Jimmy", "Servant", myDob003, true, nationality02, true,
                8.50, 6.0, myGoTo3, c1, 90, 0, 3.3, 6.7, Utility_Methods.GenerateTraitSet(), false, false, null, null,
                null, null, 0, false, false, myTitles003, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC1.charID, myNPC1);
            var myNPC2 = new NonPlayerCharacter("Char_402", "Johnny", "Servant", myDob004, true, nationality02, true,
                8.50, 6.0, myGoTo4, c1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), false, false, null, null,
                null, null, 10000, true, false, myTitles004, null, empl: myChar1.charID, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC2.charID, myNPC2);
            var myNPC3 = new NonPlayerCharacter("Char_403", "Harry", "Bailiff", myDob005, true, nationality01, true,
                8.50, 6.0, myGoTo5, c1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), true, false, null, null,
                null, null, 10000, false, false, myTitles005, null, empl: myChar2.charID, loc: myFief6);
            Globals_Game.npcMasterList.Add(myNPC3.charID, myNPC3);
            var myChar1Wife = new NonPlayerCharacter("Char_404", "Bev", "Bond", myDob006, false, nationality02, true,
                2.50, 9.0, myGoTo6, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), false, false, "Char_47",
                "Char_47", null, null, 30000, false, false, myTitles006, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            var myChar2Son = new NonPlayerCharacter("Char_405", "Horatio", "Dond", myDob007, true, nationality01, true,
                8.50, 6.0, myGoTo7, f1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), true, false, "Char_40",
                "Char_406", "Char_40", null, 10000, false, true, myTitles007, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            var myChar2SonWife = new NonPlayerCharacter("Char_406", "Mave", "Dond", myDob008, false, nationality02, true,
                2.50, 9.0, myGoTo8, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_40",
                "Char_405", null, null, 30000, false, false, myTitles008, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);
            var myChar1Son = new NonPlayerCharacter("Char_407", "Rickie", "Bond", myDob009, true, nationality02, true,
                2.50, 9.0, myGoTo9, c1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_47",
                null, "Char_47", "Char_404", 30000, false, true, myTitles009, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Son.charID, myChar1Son);
            var myChar1Daughter = new NonPlayerCharacter("Char_408", "Elsie", "Bond", myDob010, false, nationality02,
                true, 2.50, 9.0, myGoTo10, c1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false,
                "Char_47", null, "Char_47", "Char_404", 30000, false, false, myTitles010, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Daughter.charID, myChar1Daughter);
            var myChar2Son2 = new NonPlayerCharacter("Char_409", "Wayne", "Dond", myDob011, true, nationality01, true,
                2.50, 9.0, myGoTo11, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_40",
                null, "Char_40", null, 30000, false, false, myTitles011, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son2.charID, myChar2Son2);
            var myChar2Daughter = new NonPlayerCharacter("Char_410", "Esmerelda", "Dond", myDob012, false, nationality01,
                true, 2.50, 9.0, myGoTo12, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false,
                "Char_40", null, "Char_40", null, 30000, false, false, myTitles012, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Daughter.charID, myChar2Daughter);

            /*
            // create and add a scheduled birth
            string[] birthPersonae = new string[] { myChar1Wife.familyID + "|headOfFamily", myChar1Wife.charID + "|mother", myChar1Wife.spouse + "|father" };
            JournalEntry myEntry = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 1, birthPersonae, "birth");
            Globals_Game.scheduledEvents.entries.Add(myEntry.jEntryID, myEntry); */

            // get character's correct days allowance
            myChar1.days = myChar1.days;
            myChar2.days = myChar2.days;
            myNPC1.days = myNPC1.days;
            myNPC2.days = myNPC2.days;
            myNPC3.days = myNPC3.days;
            myChar1Wife.days = myChar1Wife.days;
            myChar2Son.days = myChar2Son.days;
            myChar2SonWife.days = myChar2SonWife.days;
            myChar1Son.days = myChar1Son.days;
            myChar1Daughter.days = myChar1Daughter.days;
            myChar2Son2.days = myChar2Son2.days;
            myChar2Daughter.days = myChar2Daughter.days;

            // set kingdom owners
            Globals_Game.kingOne = myChar1;
            Globals_Game.kingTwo = myChar2;

            // set province owners
            myProv.owner = myChar2;
            myProv2.owner = myChar2;

            // Add provinces to list of provinces owned 
            myChar2.AddToOwnedProvinces(myProv);
            myChar2.AddToOwnedProvinces(myProv2);

            // set fief owners
            myFief1.owner = myChar1;
            myFief2.owner = myChar1;
            myFief3.owner = myChar1;
            myFief4.owner = myChar1;
            myFief5.owner = myChar2;
            myFief6.owner = myChar2;
            myFief7.owner = myChar2;

            // Add fiefs to list of fiefs owned 
            myChar1.AddToOwnedFiefs(myFief1);
            myChar1.AddToOwnedFiefs(myFief3);
            myChar1.AddToOwnedFiefs(myFief4);
            myChar2.AddToOwnedFiefs(myFief6);
            myChar1.AddToOwnedFiefs(myFief2);
            myChar2.AddToOwnedFiefs(myFief5);
            myChar2.AddToOwnedFiefs(myFief7);

            // set kingdom title holders
            myKingdom1.titleHolder = myChar1.charID;
            myKingdom2.titleHolder = myChar2.charID;

            // set province title holders
            myProv.titleHolder = myChar1.charID;
            myProv2.titleHolder = myChar1.charID;

            // set fief title holders
            myFief1.titleHolder = myChar1.charID;
            myFief2.titleHolder = myChar1.charID;
            myFief3.titleHolder = myChar1.charID;
            myFief4.titleHolder = myChar1.charID;
            myFief5.titleHolder = myChar2.charID;
            myFief6.titleHolder = myChar2.charID;
            myFief7.titleHolder = myChar2.charID;

            // add titles (all types of places) to myTitles lists
            myChar1.myTitles.Add(myKingdom1.id);
            myChar1.myTitles.Add(myProv.id);
            myChar1.myTitles.Add(myFief1.id);
            myChar1.myTitles.Add(myFief2.id);
            myChar1.myTitles.Add(myFief3.id);
            myChar1.myTitles.Add(myFief4.id);
            myChar2.myTitles.Add(myKingdom2.id);
            myChar2.myTitles.Add(myProv2.id);
            myChar2.myTitles.Add(myFief5.id);
            myChar2.myTitles.Add(myFief6.id);
            myChar2.myTitles.Add(myFief7.id);

            // set fief ancestral owners
            myFief1.ancestralOwner = myChar1;
            myFief2.ancestralOwner = myChar1;
            myFief3.ancestralOwner = myChar1;
            myFief4.ancestralOwner = myChar1;
            myFief5.ancestralOwner = myChar2;
            myFief6.ancestralOwner = myChar2;
            myFief7.ancestralOwner = myChar2;

            // set fief bailiffs
            myFief1.bailiff = myChar1;
            myFief2.bailiff = myChar1;
            myFief6.bailiff = myNPC3;

            // add NPC to employees
            myChar1.HireNPC(myNPC2, 12000);
            // set employee as travelling companion
            myChar1.AddToEntourage(myNPC2);
            // give player a wife
            myChar1.spouse = myChar1Wife.charID;
            // add NPC to employees/family
            myChar1.myNPCs.Add(myChar1Wife);
            myChar1.myNPCs.Add(myChar1Son);
            myChar1.myNPCs.Add(myChar1Daughter);
            myChar2.HireNPC(myNPC3, 10000);
            myChar2.myNPCs.Add(myChar2Son);
            myChar2.myNPCs.Add(myChar2SonWife);
            myChar2.myNPCs.Add(myChar2Son2);
            myChar2.myNPCs.Add(myChar2Daughter);

            // add some characters to myFief1
            myFief1.AddCharacter(myChar1);
            myFief1.AddCharacter(myChar2);
            myFief1.AddCharacter(myNPC1);
            myFief1.AddCharacter(myNPC2);
            myFief1.AddCharacter(myChar1Son);
            myFief1.AddCharacter(myChar1Daughter);
            myFief6.AddCharacter(myNPC3);
            myFief1.AddCharacter(myChar1Wife);
            myFief6.AddCharacter(myChar2Son);
            myFief6.AddCharacter(myChar2SonWife);
            myFief6.AddCharacter(myChar2Son2);
            myFief6.AddCharacter(myChar2Daughter);

            // populate Globals_Server.gameTypes
            Globals_Server.gameTypes.Add(0, "Individual points");
            Globals_Server.gameTypes.Add(1, "Individual position");
            Globals_Server.gameTypes.Add(2, "Team historical");

            // populate Globals_Server.combatValues
            uint[] eCombatValues = {9, 9, 1, 9, 5, 3, 1};
            Globals_Server.combatValues.Add("Eng", eCombatValues);
            uint[] fCombatValues = {7, 7, 3, 2, 4, 2, 1};
            Globals_Server.combatValues.Add("Fr", fCombatValues);
            uint[] sCombatValues = {8, 8, 1, 2, 4, 4, 1};
            Globals_Server.combatValues.Add("Sco", sCombatValues);
            uint[] oCombatValues = {7, 7, 3, 2, 4, 2, 1};
            Globals_Server.combatValues.Add("Oth", oCombatValues);

            // populate Globals_Server.recruitRatios
            double[] eRecruitRatios = {0.01, 0.02, 0, 0.12, 0.03, 0.32, 0.49};
            Globals_Server.recruitRatios.Add("Eng", eRecruitRatios);
            double[] fRecruitRatios = {0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49};
            Globals_Server.recruitRatios.Add("Fr", fRecruitRatios);
            double[] sRecruitRatios = {0.01, 0.02, 0, 0, 0.04, 0.43, 0.49};
            Globals_Server.recruitRatios.Add("Sco", sRecruitRatios);
            double[] oRecruitRatios = {0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49};
            Globals_Server.recruitRatios.Add("Oth", oRecruitRatios);

            // populate Globals_Server.battleProbabilities
            double[] odds = {2, 3, 4, 5, 6, 99};
            Globals_Server.battleProbabilities.Add("odds", odds);
            double[] bChance = {10, 30, 50, 70, 80, 90};
            Globals_Server.battleProbabilities.Add("battle", bChance);
            double[] pChance = {10, 20, 30, 40, 50, 60};
            Globals_Server.battleProbabilities.Add("pillage", pChance);

            // create an army and add in appropriate places
            uint[] myArmyTroops = {10, 10, 0, 100, 100, 200, 400};
            var myArmy = new Army(Globals_Game.GetNextArmyID(), null, myChar1.charID, 90, myChar1.location.id,
                trp: myArmyTroops);
            Globals_Game.armyMasterList.Add(myArmy.armyID, myArmy);
            myChar1.myArmies.Add(myArmy);
            // myChar1.armyID = myArmy.armyID;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = {10, 10, 30, 0, 40, 200, 0};
            var myArmy2 = new Army(Globals_Game.GetNextArmyID(), myChar2.charID, myChar2.charID, myChar2.days,
                myChar2.location.id, trp: myArmyTroops2, aggr: 1);
            Globals_Game.armyMasterList.Add(myArmy2.armyID, myArmy2);
            myChar2.myArmies.Add(myArmy2);
            myChar2.armyID = myArmy2.armyID;
            myChar2.location.armies.Add(myArmy2.armyID);

            // bar a character from the myFief1 keep
            myFief2.BarCharacter(myNPC1.charID);
            myFief2.BarCharacter(myChar2.charID);
            myFief2.BarCharacter(myChar1Wife.charID);

            /*
            // create VictoryDatas for PCs
            VictoryData myVicData01 = new VictoryData(myChar1.playerID, myChar1.charID, myChar1.calculateStature(), myChar1.getPopulationPercentage(), myChar1.getFiefsPercentage(), myChar1.getMoneyPercentage());
            Globals_Game.victoryData.Add(myVicData01.playerID, myVicData01);
            VictoryData myVicData02 = new VictoryData(myChar2.playerID, myChar2.charID, myChar2.calculateStature(), myChar2.getPopulationPercentage(), myChar2.getFiefsPercentage(), myChar2.getMoneyPercentage());
            Globals_Game.victoryData.Add(myVicData02.playerID, myVicData02);*/

            // try retrieving fief from masterlist using fiefID
            // Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");
        }

        /// <summary>
        ///     Ensures that the Globals_Game.victoryData is up-to-date
        /// </summary>
        [ContractVerification(false)]
        public void SynchroniseVictoryData()
        {
            var toRemove = new List<string>();
            var toAdd = new List<VictoryData>();

            // iterate through Globals_Game.victoryData
            foreach (var vicDataEntry in Globals_Game.victoryData)
            {
                // check that player still active
                PlayerCharacter thisPC = null;
                if (Globals_Game.pcMasterList.ContainsKey(vicDataEntry.Value.playerCharacterID))
                {
                    thisPC = Globals_Game.pcMasterList[vicDataEntry.Value.playerCharacterID];
                }

                // if PC exists
                if (thisPC != null)
                {
                    // check is active player
                    if ((string.IsNullOrWhiteSpace(thisPC.playerID)) || (!thisPC.isAlive))
                    {
                        toRemove.Add(vicDataEntry.Key);
                    }
                }

                // if PC doesn't exist
                else
                {
                    toRemove.Add(vicDataEntry.Key);
                }
            }

            // remove Globals_Game.victoryData entries if necessary
            if (toRemove.Count > 0)
            {
                foreach (var item in toRemove)
                    Globals_Game.victoryData.Remove(item);
            }
            toRemove.Clear();

            // iterate through pcMasterList
            foreach (var pcEntry in Globals_Game.pcMasterList)
            {
                // check for playerID (i.e. active player)
                if (!string.IsNullOrWhiteSpace(pcEntry.Value.playerID))
                {
                    // check if is in Globals_Game.victoryData
                    if (!Globals_Game.victoryData.ContainsKey(pcEntry.Value.playerID))
                    {
                        // create and add new VictoryData if necessary
                        var newVicData = new VictoryData(pcEntry.Value.playerID, pcEntry.Value.charID,
                            pcEntry.Value.CalculateStature(), pcEntry.Value.GetPopulationPercentage(),
                            pcEntry.Value.GetFiefsPercentage(), pcEntry.Value.GetMoneyPercentage());
                        toAdd.Add(newVicData);
                    }
                }
            }

            // add any new Globals_Game.victoryData entries
            if (toAdd.Count > 0)
            {
                for (var i = 0; i < toAdd.Count; i++)
                {
                    Globals_Game.victoryData.Add(toAdd[i].playerID, toAdd[i]);
                }
            }
            toAdd.Clear();
        }

        // ------------------- SEASONAL UPDATE

        /// <summary>
        ///     Updates game objects at end/start of season
        /// </summary>
        [ContractVerification(false)]
        public void SeasonUpdate()
        {
            // used to check if character update is necessary
            var performCharacterUpdate = true;

            // FIEFS
            foreach (var fiefEntry in Globals_Game.fiefMasterList)
            {
                fiefEntry.Value.UpdateFief();
            }

            // NONPLAYERCHARACTERS
            foreach (var npcEntry in Globals_Game.npcMasterList)
            {
                // check if NonPlayerCharacter is alive
                performCharacterUpdate = npcEntry.Value.isAlive;

                if (performCharacterUpdate)
                {
                    // updateCharacter includes checkDeath
                    npcEntry.Value.UpdateCharacter();

                    // check again if NonPlayerCharacter is alive (after checkDeath)
                    if (npcEntry.Value.isAlive)
                    {
                        // random move if has no boss and is not family member
                        if ((string.IsNullOrWhiteSpace(npcEntry.Value.employer)) &&
                            (string.IsNullOrWhiteSpace(npcEntry.Value.familyID)))
                        {
                            ProtoMessage ignore;
                            npcEntry.Value.RandomMoveNPC(out ignore);
                        }

                        // finish previously started multi-hex move if necessary
                        if (npcEntry.Value.goTo.Count > 0)
                        {
                            ProtoMessage ignore;
                            npcEntry.Value.CharacterMultiMove(out ignore);
                        }
                    }
                }
            }

            // PLAYERCHARACTERS
            foreach (var pcEntry in Globals_Game.pcMasterList.ToList())
            {
                // check if PlayerCharacter is alive
                performCharacterUpdate = pcEntry.Value.isAlive;

                if (performCharacterUpdate)
                {
                    // updateCharacter includes checkDeath
                    pcEntry.Value.UpdateCharacter();

                    // check again if PlayerCharacter is alive (after checkDeath)
                    if (pcEntry.Value.isAlive)
                    {
                        // finish previously started multi-hex move if necessary
                        if (pcEntry.Value.goTo.Count > 0)
                        {
                            ProtoMessage ignore;
                            pcEntry.Value.CharacterMultiMove(out ignore);
                        }
                    }
                }
            }

            // add any newly promoted NPCs to Globals_Game.pcMasterList
            if (Globals_Game.promotedNPCs.Count > 0)
            {
                foreach (var pc in Globals_Game.promotedNPCs)
                {
                    if (!Globals_Game.pcMasterList.ContainsKey(pc.charID))
                    {
                        Globals_Game.pcMasterList.Add(pc.charID, pc);
                    }
                }
            }

            // clear Globals_Game.promotedNPCs
            Globals_Game.promotedNPCs.Clear();

            // ARMIES
            // keep track of any armies requiring removal (if hav fallen below 100 men)
            var disbandedArmies = new List<Army>();
            var hasDissolved = false;

            // iterate through armies
            foreach (var armyEntry in Globals_Game.armyMasterList)
            {
                hasDissolved = armyEntry.Value.UpdateArmy();

                // add to dissolvedArmies if appropriate
                if (hasDissolved)
                {
                    disbandedArmies.Add(armyEntry.Value);
                }
            }

            // remove any dissolved armies
            if (disbandedArmies.Count > 0)
            {
                for (var i = 0; i < disbandedArmies.Count; i++)
                {
                    // disband army
                    disbandedArmies[i].DisbandArmy();
                    disbandedArmies[i] = null;
                }

                // clear dissolvedArmies
                disbandedArmies.Clear();
            }

            // SIEGES

            // keep track of any sieges requiring removal
            var hasEnded = false;

            // iterate through sieges
            foreach (var siegeEntry in Globals_Game.siegeMasterList)
            {
                hasEnded = siegeEntry.Value.UpdateSiege();

                // add to dissolvedSieges if appropriate
                if (hasEnded)
                {
                    siegeEntry.Value.SiegeEnd(false);
                    Globals_Game.siegeMasterList.Remove(siegeEntry.Key);
                }
            }

            // ADVANCE SEASON AND YEAR
            Globals_Game.clock.AdvanceSeason();

            // CHECK FOR VICTORY / END GAME
            var gameEnded = Globals_Game.CheckForVictory();

            //TODO either implement or remove
            /*
            // update and get scores for individual point game
            SortedList<double, string> currentScores = new SortedList<double,string>();
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
            victor = this.CheckTeamAbsoluteVictory();
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
                    victor = this.CheckTeamHistoricalVictory();
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

            // announce winners
            if ((endDateReached) || (absoluteVictory))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(gameResults);
                }
            } */

            if (!gameEnded)
            {
                // CHECK SCHEDULED EVENTS
                var entriesForRemoval = Globals_Game.ProcessScheduledEvents();

                // remove processed events from Globals_Game.scheduledEvents
                for (var i = 0; i < entriesForRemoval.Count; i++)
                {
                    Globals_Game.scheduledEvents.entries.Remove(entriesForRemoval[i].jEntryID);
                }
            }
        }

        /// <summary>
        ///     Allows a player to switch character
        /// </summary>
        /// <param name="user">userID of the player</param>
        /// <param name="charID">characterID of the playercharacter to be switched to</param>
        public void SwitchPlayerCharacter(string user, string charID)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                Globals_Server.logError("Null User detected in switchPlayerCharacter! charID: " + charID);
                return;
            }
            if (!Globals_Game.ownedPlayerCharacters.ContainsKey(user))
            {
                Globals_Server.logError("Invalid User detected in switchPlayerCharacter! charID: " + charID);
            }
            Client c;
            PlayerCharacter pc;
            if (string.IsNullOrWhiteSpace(charID))
            {
                Globals_Game.UpdatePlayer(user, DisplayMessages.SwitchPlayerErrorNoID);
                return;
            }
            // Checks that the character is available
            if (!Globals_Game.pcMasterList.ContainsKey(charID) ||
                Globals_Game.ownedPlayerCharacters.ContainsValue(Globals_Game.pcMasterList[charID]))
            {
                Globals_Game.UpdatePlayer(user, DisplayMessages.SwitchPlayerErrorIDInvalid);
            }
            else
            {
                pc = Globals_Game.pcMasterList[charID];

                Globals_Game.ownedPlayerCharacters[user] = pc;
                //TODO write new user character to database
                if (Globals_Server.Clients.ContainsKey(user))
                {
                    c = Globals_Server.Clients[user];
                    c.activeChar = pc;
                    c.myPlayerCharacter = pc;
                    c.fiefToView = pc.location;
                }
            }
        }

        /// <summary>
        ///     Processes a client request to switch to controlling a different Character
        /// </summary>
        /// <param name="charID">ID of character to control</param>
        /// <param name="client">Client who wishes to use this character</param>
        /// <returns>
        ///     ProtoCharacter message with response type "Success" on success, ProtoMessage with ErrorGenericCharacterUnidentified
        ///     for invalid character ID
        ///     ProtoMessage with ErrorGenericUnauthorised if do not own character, CharacterIsDead if trying to use a dead
        ///     character,
        /// </returns>
        /// <remarks>
        ///     On success the client's active character is changed. Empty or null charIDs default to the client's
        ///     PlayerCharacter
        /// </remarks>
        public static ProtoMessage UseChar(string charID, Client client)
        {
            Character c;
            DisplayMessages charErr;
            c = Utility_Methods.GetCharacter(charID, out charErr);

            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    c))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Cannot use a character that is dead
            if (!c.isAlive)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            client.activeChar = c;
            var success = new ProtoCharacter(c);
            success.includeAll(c);
            success.ActionType = Actions.UseChar;
            success.ResponseType = DisplayMessages.Success;
            return success;
        }

        /// <summary>
        ///     Processes a client request to get a list of all players, including usernames, nationality and PlayerCharacter names
        /// </summary>
        /// <param name="client">Client who is requesting list of players</param>
        /// <returns>Collection of other players wrapped in a ProtoMessage</returns>
        public static ProtoMessage GetPlayers(Client client)
        {
            var players = new ProtoGenericArray<ProtoPlayer>();
            var playerList = new List<ProtoPlayer>();
            foreach (var c in Globals_Server.Clients.Values)
            {
                // Do not include dead characters or self
                if (!c.myPlayerCharacter.isAlive || c.username.Equals(client.username)) continue;
                var player = new ProtoPlayer();
                player.playerID = c.username;
                player.pcID = c.myPlayerCharacter.charID;
                player.pcName = c.myPlayerCharacter.firstName + " " + c.myPlayerCharacter.familyName;
                player.natID = c.myPlayerCharacter.nationality.natID;
                playerList.Add(player);
            }
            players.fields = playerList.ToArray();
            players.ResponseType = DisplayMessages.Success;
            return players;
        }

        /// <summary>
        ///     Processes a client request to view a character. The level of detail varies based on who is viewing which character
        /// </summary>
        /// <param name="charID">ID of character to view</param>
        /// <param name="client">Client who wishes to view Character</param>
        /// <returns>
        ///     Details of character if successful (hides information if do not own character, hides location if character is
        ///     captured), CharacterUnidentified if invalid, MessageInvalid if null
        /// </returns>
        public static ProtoMessage ViewCharacter(string charID, Client client)
        {
            DisplayMessages charErr;
            var c = Utility_Methods.GetCharacter(charID, out charErr);
            // Ensure character exists
            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            // Check whether player owns character, include additional information if so
            var viewAll = PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin,
                client.myPlayerCharacter, c);
            var seeLocation = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin,
                client.myPlayerCharacter, c.location);
            if (c is NonPlayerCharacter)
            {
                var npc = c as NonPlayerCharacter;
                var characterDetails = new ProtoNPC(npc);
                characterDetails.ResponseType = DisplayMessages.Success;
                // If unemployed include hire details
                if (string.IsNullOrWhiteSpace(npc.familyID) && string.IsNullOrWhiteSpace(npc.employer))
                {
                    characterDetails.IncludeHire(npc, client.myPlayerCharacter.charID);
                }
                if (viewAll)
                {
                    characterDetails.includeAll(c as NonPlayerCharacter);
                }
                if (seeLocation)
                {
                    characterDetails.includeLocation(c);
                }
                // If captured, hide location
                if (!string.IsNullOrWhiteSpace(c.captorID) && !c.captorID.Equals(client.myPlayerCharacter.charID))
                {
                    characterDetails.location = null;
                }
                return characterDetails;
            }
            else
            {
                var characterDetails = new ProtoPlayerCharacter(c as PlayerCharacter);
                characterDetails.ResponseType = DisplayMessages.Success;
                if (viewAll)
                {
                    characterDetails.includeAll(c as PlayerCharacter);
                }
                if (!string.IsNullOrWhiteSpace(c.captorID) && !c.captorID.Equals(client.myPlayerCharacter.charID))
                {
                    characterDetails.location = null;
                }
                return characterDetails;
            }
        }

        /// <summary>
        ///     Processes a client request to hire an NPC by bidding
        /// </summary>
        /// <param name="charID">ID of character to hire</param>
        /// <param name="bid">Amount client wishes to bid</param>
        /// <param name="client">Client to hire NPC</param>
        /// <returns>
        ///     ProtoNPC containing updated hire details and hire status- may have bidded successfully but not bid high enough;
        ///     MessageInvalid if null character id; CharacterUnidentified if invalid character ID, ErrorGenericTooFarFromFief if
        ///     not in same fief as character to hire, CharacterHeldCaptive if your character is held captive,
        ///     CharacterHireNotEmployable if character cannot be employed. Within the ActionController will return PositiveInteger
        ///     if a bid is not included or is invalid.
        /// </returns>
        public static ProtoMessage HireNPC(string charID, uint bid, Client client)
        {
            DisplayMessages charErr;
            var toHire = Utility_Methods.GetCharacter(charID, out charErr) as NonPlayerCharacter;
            // Validate character to hire
            if (toHire == null)
            {
                return new ProtoMessage(charErr);
            }
            // Ensure player is near character
            if (
                !PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, client.myPlayerCharacter,
                    toHire.location))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericTooFarFromFief);
            }
            if (!string.IsNullOrWhiteSpace(client.myPlayerCharacter.captorID))
            {
                return new ProtoMessage(DisplayMessages.CharacterHeldCaptive);
            }
            if (!toHire.CheckCanHire(client.myPlayerCharacter))
            {
                return new ProtoMessage(DisplayMessages.CharacterHireNotEmployable);
            }

            // Send result and updated character detail
            // Result contains character details, the result of hiring (response type) and any necessary fields for filling in response
            ProtoMessage result = null;
            client.myPlayerCharacter.ProcessEmployOffer(toHire, bid, out result);
            var viewCharacter = new ProtoNPC(toHire);
            viewCharacter.ResponseType = result.ResponseType;
            viewCharacter.Message = result.Message;
            viewCharacter.MessageFields = result.MessageFields;
            return viewCharacter;
        }

        /// <summary>
        ///     Processes a client request to fire an NPC
        /// </summary>
        /// <param name="charID">Character ID of character to be fired</param>
        /// <param name="client">Client who wishes to fire NPC</param>
        /// <returns>
        ///     MessageInvalid if not a valid character ID; CharacterUnidentified if not a valid character;
        ///     CharacterFireNotEmployee if trying to fire someone who is not an employee; Success otherwise
        /// </returns>
        public static ProtoMessage FireNPC(string charID, Client client)
        {
            DisplayMessages charErr;
            var character = Utility_Methods.GetCharacter(charID, out charErr);

            // If character to hire unidentified, error
            if (character == null)
            {
                return new ProtoMessage(charErr);
            }
            var npc = character as NonPlayerCharacter;
            // if is not npc, or is not employed by player, error
            if (npc == null || npc.GetEmployer() != client.myPlayerCharacter)
            {
                return new ProtoMessage(DisplayMessages.CharacterFireNotEmployee);
            }
            client.myPlayerCharacter.FireNPC(npc);
            var result = new ProtoMessage();
            // Include char id to let client know which npc has been fired
            result.Message = npc.charID;
            result.ResponseType = DisplayMessages.Success;
            return result;
        }

        /// <summary>
        ///     Processes a client request to view an army. Details will vary based on whether army is owned by client or not
        /// </summary>
        /// <param name="armyID">Army ID of army to view</param>
        /// <param name="client">Client who wishes to view army</param>
        /// <returns>
        ///     ProtoArmy with all details if successful; MessageInvalid if no or invalid army id; ArmyUnidentified if not in
        ///     master army list; Unauthorised if too far from army
        /// </returns>
        public static ProtoMessage ViewArmy(string armyID, Client client)
        {
            //Check is a valid Army
            DisplayMessages armyErr;
            var army = Utility_Methods.GetArmy(armyID, out armyErr);
            if (army == null)
            {
                return new ProtoMessage(armyErr);
            }
            // Check whether client can see this army
            if (!PermissionManager.isAuthorized(PermissionManager.canSeeArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Determine how much information is displayed by checking if owns army
            var viewAll = PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter,
                army);
            var armyDetails = new ProtoArmy(army, client.myPlayerCharacter);
            armyDetails.ResponseType = DisplayMessages.Success;
            if (viewAll)
            {
                armyDetails.includeAll(army);
            }
            return armyDetails;
        }

        /// <summary>
        ///     Processes a client request to disband an army
        /// </summary>
        /// <param name="armyID">Army ID of army to be disbanded</param>
        /// <param name="client">Client who wishes to disband the army</param>
        /// <returns>
        ///     Success if completed without error; MessageInvalid if no or invalid army ID; ArmyUnidentified if army not in
        ///     army master list; Unauthorised if do not own army
        /// </returns>
        public static ProtoMessage DisbandArmy(string armyID, Client client)
        {
            //Check is a valid Army
            DisplayMessages armyErr;
            var army = Utility_Methods.GetArmy(armyID, out armyErr);
            if (army == null)
            {
                return new ProtoMessage(armyErr);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            army.DisbandArmy();
            return new ProtoMessage(DisplayMessages.Success);
        }

        /// <summary>
        ///     Processes a client request to get a list of all NPCs that meet the given type and role
        /// </summary>
        /// <param name="type">
        ///     Type of NPC grouping: can be Entourage, Grant, Family, Employ or a combination of Family and Employ
        ///     (for whole household)
        /// </param>
        /// <param name="role">
        ///     For use with "Grant" type, the role checks what role is to be granted. Presently only "leader" is
        ///     supported
        /// </param>
        /// <param name="item">
        ///     Item that is to be granted control of- in this case, as only army leadership is supported, the item
        ///     will be an army ID
        /// </param>
        /// <param name="client">Client who is performing this action</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage GetNPCList(string type, string role, string item, Client client)
        {
            var listOfChars = new List<ProtoCharacterOverview>();
            switch (type)
            {
                case "Entourage":
                {
                    foreach (var entourageChar in client.myPlayerCharacter.myEntourage)
                    {
                        //  listOfChars.Add(entourageChar.firstName);
                        listOfChars.Add(new ProtoCharacterOverview(entourageChar));
                    }
                    break;
                }
                case "Grant":
                {
                    switch (role)
                    {
                        case "leader":
                        {
                            var armyID = item;
                            //Check is a valid Army
                            DisplayMessages armyErr;
                            var army = Utility_Methods.GetArmy(armyID, out armyErr);
                            if (army == null)
                            {
                                return new ProtoMessage(armyErr);
                            }
                            if (
                                !PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin,
                                    client.myPlayerCharacter, army))
                            {
                                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
                            }
                            foreach (var npc in client.myPlayerCharacter.myNPCs)
                            {
                                ProtoMessage ignore = null;

                                if (npc.ChecksBeforeGranting(client.myPlayerCharacter, role, true, out ignore))
                                {
                                    listOfChars.Add(new ProtoCharacterOverview(npc));
                                }

                                else
                                {
                                    if (npc.ChecksBeforeGranting(client.myPlayerCharacter, role, true, out ignore,
                                        army.armyID))
                                    {
                                        listOfChars.Add(new ProtoCharacterOverview(npc));
                                    }
                                }
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                    break;
                }
                default:
                {
                    if (type.Contains("Family"))
                    {
                        listOfChars.Add(new ProtoCharacterOverview(client.myPlayerCharacter));
                        foreach (var family in client.myPlayerCharacter.myNPCs)
                        {
                            // ensure character is employee
                            if (!string.IsNullOrWhiteSpace(family.familyID))
                            {
                                //   listOfChars.Add(family.firstName);
                                listOfChars.Add(new ProtoCharacterOverview(family));
                            }
                        }
                    }
                    if (type.Contains("Employ"))
                    {
                        foreach (var employee in client.myPlayerCharacter.myNPCs)
                        {
                            // ensure character is employee
                            if (!string.IsNullOrWhiteSpace(employee.employer))
                            {
                                // listOfChars.Add(employee.firstName);
                                listOfChars.Add(new ProtoCharacterOverview(employee));
                            }
                        }
                    }
                }
                    break;
            }
            var result = new ProtoGenericArray<ProtoCharacterOverview>();
            result.fields = listOfChars.ToArray();
            result.ResponseType = DisplayMessages.Success;
            result.Message = type;
            result.MessageFields = new[] {role, item};
            return result;
        }

        /// <summary>
        ///     Processes a client request to travel to another fief
        /// </summary>
        /// <param name="charID"></param>
        /// <param name="fiefID"></param>
        /// <param name="travelInstructions"></param>
        /// <param name="client"></param>
        /// <returns>Result of request</returns>
        public static ProtoMessage TravelTo(string charID, string fiefID, string[] travelInstructions, Client client)
        {
            // Identify character to move
            Character charToMove = null;
            DisplayMessages charError;
            charToMove = Utility_Methods.GetCharacter(charID, out charError);
            if (charToMove == null)
            {
                return new ProtoMessage(charError);
            }
            // Check permissions
            if (
                PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    charToMove) && charToMove.isAlive)
            {
                // Identify fief

                if (!string.IsNullOrWhiteSpace(fiefID))
                {
                    DisplayMessages errorMessage;
                    var fief = Utility_Methods.GetFief(fiefID, out errorMessage);
                    if (fief == null) return new ProtoMessage(errorMessage);
                    var travelCost = charToMove.location.getTravelCost(fief, charToMove.armyID);
                    // If successful send details of fief
                    ProtoMessage error;
                    PlayerCharacter pcToMove;
                    bool success;
                    // Convert to player/non player character in order to perform correct movement
                    if (charToMove is PlayerCharacter)
                    {
                        pcToMove = charToMove as PlayerCharacter;
                        success = pcToMove.MoveCharacter(fief, travelCost, out error);
                    }
                    else
                    {
                        success = (charToMove as NonPlayerCharacter).MoveCharacter(fief, travelCost, out error);
                    }
                    if (success)
                    {
                        var reply = new ProtoFief(fief) {ResponseType = DisplayMessages.Success};
                        return reply;
                    }
                    //Return error message obtained from MoveCharacter
                    return error;
                }
                    // If specifying a route, attempt to move character and return final location
                if (travelInstructions != null)
                {
                    ProtoMessage error;
                    charToMove.TakeThisRoute(travelInstructions, out error);
                    if (error != null)
                    {
                        Globals_Game.UpdatePlayer(client.username, error);
                    }
                    var reply = new ProtoFief(charToMove.location);
                    reply.ResponseType = DisplayMessages.Success;
                    return reply;
                }
                    // If fief unidentified return error
                return new ProtoMessage(DisplayMessages.ErrorGenericFiefUnidentified);
            }
                // If unauthorised return error
            return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
        }

        /// <summary>
        ///     Processes a client request to view details of another fief. Amount of information returned depends on whether this
        ///     fief is owned by the client
        /// </summary>
        /// <param name="fiefID">ID of fief to be viewed</param>
        /// <param name="client">Client who sent the request to view the fief</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage ViewFief(string fiefID, Client client)
        {
            Fief f = null;
            // A null fief (or sending "home") gets the home fief
            if (fiefID == null || fiefID.Equals("home"))
            {
                f = client.myPlayerCharacter.GetHomeFief();
            }
            else
            {
                DisplayMessages fiefErr;
                f = Utility_Methods.GetFief(fiefID, out fiefErr);
            }
            if (f == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericFiefUnidentified);
            }
            // If the client owns the fief, include all information
            var fiefToView = new ProtoFief(f);
            if (client.myPlayerCharacter.ownedFiefs.Contains(f))
            {
                fiefToView.includeAll(f);
                return fiefToView;
            }
            // Can only see fief details if have a character in the fief
            var hasCharInFief = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin,
                client.myPlayerCharacter, f);
            if (hasCharInFief)
            {
                return fiefToView;
            }
            return new ProtoMessage(DisplayMessages.ErrorGenericTooFarFromFief);
        }

        /// <summary>
        ///     Processes a client request to view all fiefs they own
        /// </summary>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage ViewMyFiefs(Client client)
        {
            var fiefList = new ProtoGenericArray<ProtoFief>();
            fiefList.fields = new ProtoFief[client.myPlayerCharacter.ownedFiefs.Count];
            var i = 0;
            foreach (var f in client.myPlayerCharacter.ownedFiefs)
            {
                var fief = new ProtoFief(f);
                fief.includeAll(f);
                fiefList.fields[i] = fief;
                i++;
            }
            return fiefList;
        }

        /// <summary>
        ///     Processes a client request to appoint a new bailiff to a fief
        /// </summary>
        /// <param name="fiefID">The ID of the fief to appoint a bailiff to</param>
        /// <param name="charID">The ID of the character which will become the new bailiff</param>
        /// <param name="client">The client who sent the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage AppointBailiff(string fiefID, string charID, Client client)
        {
            // Fief
            DisplayMessages fiefErr, charErr;
            var f = Utility_Methods.GetFief(fiefID, out fiefErr);
            // Character to become bailiff
            var c = Utility_Methods.GetCharacter(charID, out charErr);
            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            if (f == null)
            {
                return new ProtoMessage(fiefErr);
            }

            // Ensure character owns fief and character, or is admin
            if (
                PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter, c) &&
                PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, f))
            {
                // Check character can become bailiff
                ProtoMessage error = null;
                if (c.ChecksBeforeGranting(client.myPlayerCharacter, "bailiff", false, out error))
                {
                    // set bailiff, return fief
                    f.bailiff = c;
                    f.bailiffDaysInFief = 0;
                    var fiefToView = new ProtoFief(f);
                    fiefToView.ResponseType = DisplayMessages.Success;
                    fiefToView.includeAll(f);
                    return fiefToView;
                }
                // error message returned from ChecksBeforeGranting
                return error;
            }
                // User unauthorised
            return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
        }

        /// <summary>
        ///     Process a client request to remove the current bailiff from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief from which to remove the bailiff</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage RemoveBailiff(string fiefID, Client client)
        {
            // Fief
            DisplayMessages fiefErr;
            var f = Utility_Methods.GetFief(fiefID, out fiefErr);
            if (f == null)
            {
                return new ProtoMessage(fiefErr);
            }
            // Ensure player is authorized
            var hasPermission = PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin,
                client.myPlayerCharacter, f);
            if (hasPermission)
            {
                // Remove bailiff and return fief details
                if (f.bailiff != null)
                {
                    f.bailiff = null;
                    f.bailiffDaysInFief = 0;
                    var fiefToView = new ProtoFief(f);
                    fiefToView.ResponseType = DisplayMessages.Success;
                    fiefToView.includeAll(f);
                    return fiefToView;
                }
                    // Error- no bailiff
                return new ProtoMessage(DisplayMessages.FiefNoBailiff);
            }
                // Unauthorised
            return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
        }

        /// <summary>
        ///     Process a client request to bar a number of characters from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        /// <param name="charIDs">Array of all IDs of characters to be banned</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage BarCharacters(string fiefID, string[] charIDs, Client client)
        {
            DisplayMessages fiefError;
            // check fief is valid
            var fief = Utility_Methods.GetFief(fiefID, out fiefError);

            if (fief != null)
            {
                // Check player owns fief
                if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
                {
                    return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
                }
                // List of characters that for whatever reason could not be barred
                var couldNotBar = new List<string>();
                // Bar characters
                foreach (var charID in charIDs)
                {
                    DisplayMessages charErr;
                    var charToBar = Utility_Methods.GetCharacter(charID, out charErr);

                    if (charToBar != null)
                    {
                        // Try to bar, if fail add to list of unbarrables
                        if (!fief.BarCharacter(charToBar))
                        {
                            couldNotBar.Add(charToBar.firstName + " " + charToBar.familyName);
                        }
                    }
                    // If could not identify character, add to list of unbarrables
                    else
                    {
                        couldNotBar.Add(charID);
                    }
                }
                // return fief, along with details of characters that could not be barred
                var fiefToReturn = new ProtoFief(fief);
                // If failed to bar some characters, return the IDs of the characters that could not be barred
                if (couldNotBar.Count > 0)
                {
                    fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                    fiefToReturn.MessageFields = couldNotBar.ToArray();
                }
                fiefToReturn.includeAll(fief);
                return fiefToReturn;
            }
                // if could not identify fief
            return new ProtoMessage(fiefError);
        }

        /// <summary>
        ///     Processes a client request to unbar a number of characters from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief to unbar the characters from</param>
        /// <param name="charIDs">List of character IDs of characters to be unbarred</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage UnbarCharacters(string fiefID, string[] charIDs, Client client)
        {
            // check fief is valid
            Fief fief = null;
            Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
            if (fief != null)
            {
                // Check player owns fief
                if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
                {
                    return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
                }
                // List of characters that for whatever reason could not be barred
                var couldNotUnbar = new List<string>();
                // Bar characters
                foreach (var charID in charIDs)
                {
                    DisplayMessages charErr;
                    var charToUnbar = Utility_Methods.GetCharacter(charID, out charErr);
                    if (charToUnbar != null)
                    {
                        if (!fief.UnbarCharacter(charToUnbar.charID))
                        {
                            couldNotUnbar.Add(charToUnbar.firstName + " " + charToUnbar.familyName);
                        }
                    }
                    else
                    {
                        couldNotUnbar.Add(charID);
                    }
                }
                // Return fief along with characters that could not be unbarred
                var returnFiefState = new ProtoFief(fief);
                if (couldNotUnbar.Count > 0)
                {
                    returnFiefState.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                    returnFiefState.MessageFields = couldNotUnbar.ToArray();
                }
                returnFiefState.includeAll(fief);
                return returnFiefState;
            }
                // error if fief unidentified
            return new ProtoMessage(DisplayMessages.ErrorGenericFiefUnidentified);
        }

        /// <summary>
        ///     Bar a number of nationalities from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief to bar nationalities from</param>
        /// <param name="natIDs">List of nationality IDs of nationalities to be banned</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage BarNationalities(string fiefID, string[] natIDs, Client client)
        {
            DisplayMessages fiefError;
            var fief = Utility_Methods.GetFief(fiefID, out fiefError);
            if (fief != null)
            {
                // Check player owns fief
                if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
                {
                    return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
                }
                // Attempt to bar nationalities
                var failedToBar = new List<string>();
                foreach (var natID in natIDs)
                {
                    Nationality natToBar = null;
                    Globals_Game.nationalityMasterList.TryGetValue(natID, out natToBar);
                    if (natToBar != null)
                    {
                        // Cannot ban self
                        if (natToBar == fief.owner.nationality)
                        {
                            failedToBar.Add(natID);
                            continue;
                        }
                        // Attempt to bar nationality
                        if (!fief.BarNationality(natID))
                        {
                            failedToBar.Add(natID);
                        }
                    }
                    else
                    {
                        failedToBar.Add(natID);
                    }
                }
                // Return fief + nationalities failed to bar
                var fiefToReturn = new ProtoFief(fief);
                if (failedToBar.Count > 0)
                {
                    fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                    fiefToReturn.MessageFields = failedToBar.ToArray();
                }
                fiefToReturn.includeAll(fief);
                return fiefToReturn;
            }
                // Fief is invalid
            return new ProtoMessage(fiefError);
        }

        /// <summary>
        ///     Processes a client request to unbar a number of nationalities from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief from which to unbar nationalities</param>
        /// <param name="natIDs">List of nationality IDs to unbar</param>
        /// <param name="client">Client who submitted request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage UnbarNationalities(string fiefID, string[] natIDs, Client client)
        {
            DisplayMessages fiefError;
            // check fief is valid
            var fief = Utility_Methods.GetFief(fiefID, out fiefError);
            if (fief != null)
            {
                // Check player owns fief
                if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
                {
                    return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
                }
                // Attempt to bar nationalities
                var failedToUnbar = new List<string>();
                foreach (var natID in natIDs)
                {
                    Nationality natToUnbar = null;
                    Globals_Game.nationalityMasterList.TryGetValue(natID, out natToUnbar);
                    if (natToUnbar != null)
                    {
                        // Cannot ban self
                        if (natToUnbar == fief.owner.nationality)
                        {
                            failedToUnbar.Add(natID);
                            continue;
                        }
                        // Attempt to bar nationality
                        if (!fief.UnbarNationality(natID))
                        {
                            failedToUnbar.Add(natID);
                        }
                    }
                    else
                    {
                        failedToUnbar.Add(natID);
                    }
                }
                // return fief along with nationalities that could not be unbarred
                var fiefToReturn = new ProtoFief(fief);
                if (failedToUnbar.Count > 0)
                {
                    fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                    fiefToReturn.MessageFields = failedToUnbar.ToArray();
                }
                fiefToReturn.includeAll(fief);
                return fiefToReturn;
            }
                // Fief is invalid
            return new ProtoMessage(fiefError);
        }

        /// <summary>
        ///     Processes a client request to grant the title of a fief to another character
        /// </summary>
        /// <param name="fiefID">ID of fief to grant the title of</param>
        /// <param name="charID">ID of character who will become the new title holder</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public static ProtoMessage GrantFiefTitle(string fiefID, string charID, Client client)
        {
            DisplayMessages fiefError;
            // Get fief
            var fief = Utility_Methods.GetFief(fiefID, out fiefError);
            if (fief == null)
            {
                return new ProtoMessage(fiefError);
            }

            // Ensure player has permission to grant title
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Get Character
            DisplayMessages charErr;
            var charToGrant = Utility_Methods.GetCharacter(charID, out charErr);
            if (charToGrant == null) return new ProtoMessage(charErr);

            ProtoMessage error;
            var canGrant = charToGrant.ChecksBeforeGranting(client.myPlayerCharacter, "title", false, out error);
            if (canGrant)
            {
                var granted = client.myPlayerCharacter.GrantTitle(charToGrant, fief, out error);
                if (granted)
                {
                    var f = new ProtoFief(fief);
                    f.Message = "grantedTitle";
                    f.includeAll(fief);
                    return f;
                }
                    // If granting fails message is returned from GrantTitle
                return error;
            }
                //Permission denied
            return error;
        }

        /// <summary>
        ///     Processes a client request to adjust the expenditure in a fief
        /// </summary>
        /// <param name="fiefID"></param>
        /// <param name="adjustedValues"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ProtoMessage AdjustExpenditure(string fiefID, double[] adjustedValues, Client client)
        {
            // Get fief
            Fief fief = null;
            if (string.IsNullOrWhiteSpace(fiefID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericFiefUnidentified);
            }
            Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
            if (fief == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericFiefUnidentified);
            }
            // Check permissions
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter, fief))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            if (adjustedValues == null)
            {
                var overspend = fief.GetAvailableTreasury(true);
                if (overspend < 0)
                {
                    fief.AutoAdjustExpenditure(Convert.ToUInt32(Math.Abs(overspend)));
                    var f = new ProtoFief(fief);
                    f.includeAll(fief);
                    f.ResponseType = DisplayMessages.FiefExpenditureAdjusted;
                    return f;
                }
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            if (adjustedValues.Length != 5)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                error.Message = "Expected array:5";
                return error;
            }
            // Perform conversion
            try
            {
                var adjust = fief.AdjustExpenditures(adjustedValues[0], Convert.ToUInt32(adjustedValues[1]),
                    Convert.ToUInt32(adjustedValues[2]), Convert.ToUInt32(adjustedValues[3]),
                    Convert.ToUInt32(adjustedValues[4]));
                return adjust;
            }
            catch (OverflowException e)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                error.Message = "Invalid values";
                return error;
            }
        }

        public static ProtoMessage TransferFunds(string fiefFromID, string fiefToID, int amount, Client client)
        {
            // Check both fiefs are valid
            Fief fiefFrom = null;
            Fief fiefTo = null;
            if (string.IsNullOrWhiteSpace(fiefFromID))
            {
                fiefFrom = client.myPlayerCharacter.GetHomeFief();
            }
            else if (string.IsNullOrWhiteSpace(fiefToID))
            {
                Globals_Game.fiefMasterList.TryGetValue(fiefFromID, out fiefFrom);
                fiefTo = client.myPlayerCharacter.GetHomeFief();
            }
            else
            {
                Globals_Game.fiefMasterList.TryGetValue(fiefToID, out fiefTo);
            }
            if (fiefFrom == null || fiefTo == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                return error;
            }
            if (amount < 0)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericPositiveInteger;
                return error;
            }
            // Ensure fief has sufficient funds
            if (amount > fiefFrom.GetAvailableTreasury(true))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericInsufficientFunds;
                return error;
            }
            else
            {
                // return success
                ProtoMessage error;
                if (fiefFrom.TreasuryTransfer(fiefTo, amount, out error))
                {
                    var success = new ProtoFief(fiefFrom);
                    success.includeAll(fiefFrom);
                    success.Message = "transferfunds";
                    success.ResponseType = DisplayMessages.Success;
                    return success;
                }
                    // an error message will be sent from within TreasuryTransfer
                return error;
            }
        }

        public static ProtoMessage TransferFundsToPlayer(string playerTo, int amount, Client client)
        {
            // Get player to transfer to
            PlayerCharacter transferTo = null;
            Globals_Game.pcMasterList.TryGetValue(playerTo, out transferTo);
            if (transferTo == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                return error;
            }
            // Confirm both players have a home fief
            if (client.myPlayerCharacter.GetHomeFief() == null || transferTo.GetHomeFief() == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericNoHomeFief;
                return error;
            }
            // Perform treasury transfer, update
            ProtoMessage TransferError;
            var success = client.myPlayerCharacter.GetHomeFief()
                .TreasuryTransfer(transferTo.GetHomeFief(), amount, out TransferError);
            if (success)
            {
                Globals_Game.UpdatePlayer(transferTo.playerID, DisplayMessages.GenericReceivedFunds,
                    new[]
                    {amount.ToString(), client.myPlayerCharacter.firstName + " " + client.myPlayerCharacter.familyName});
                var result = new ProtoMessage();
                result.ResponseType = DisplayMessages.Success;
                result.Message = "transferfundsplayer";
                return result;
            }
            return TransferError;
        }

        public static ProtoMessage EnterExitKeep(string charID, Client client)
        {
            Character c = null;
            var charErr = DisplayMessages.ErrorGenericCharacterUnidentified;
            // get character, check is valid
            if (string.IsNullOrWhiteSpace(charID))
            {
                c = client.myPlayerCharacter;
            }
            else
            {
                c = Utility_Methods.GetCharacter(charID, out charErr);
            }

            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            // check authorization
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    c))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage EnterError;
            var success = c.ExitEnterKeep(out EnterError);
            if (success)
            {
                var m = new ProtoMessage();
                m.Message = c.inKeep.ToString();
                m.ResponseType = DisplayMessages.Success;
                return m;
            }
            // Error messages handled within enter/exit keep
            return EnterError;
        }

        public static ProtoMessage ListCharsInMeetingPlace(string placeType, string charID, Client client)
        {
            // Character to use
            Character character;
            // fief is current player fief- May change later to allow a player's characters to scout out NPCs in fiefs
            if (string.IsNullOrWhiteSpace(charID))
            {
                DisplayMessages charErr;
                character = Utility_Methods.GetCharacter(charID, out charErr);
                if (character == null)
                {
                    return new ProtoMessage(charErr);
                }
            }
            else
            {
                character = client.myPlayerCharacter;
            }
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    character))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            var fief = character.location;
            if (string.IsNullOrWhiteSpace(placeType))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Enter/exit keep as appropriate depending on whether viewing court
            if (placeType.Equals("court"))
            {
                if (!character.inKeep)
                {
                    ProtoMessage error;
                    character.EnterKeep(out error);
                    if (error != null)
                    {
                        return error;
                    }
                }
            }
            else
            {
                if (character.inKeep)
                {
                    character.ExitKeep();
                }
            }
            // Get and return charcters in meeting place
            var charsInPlace =
                new ProtoGenericArray<ProtoCharacterOverview>(fief.ListCharsInMeetingPlace(placeType,
                    client.myPlayerCharacter));
            charsInPlace.Message = placeType;
            charsInPlace.ResponseType = DisplayMessages.Success;
            return charsInPlace;
        }

        public static ProtoMessage Camp(string charID, byte days, Client client)
        {
            Character c;
            var charErr = DisplayMessages.ErrorGenericCharacterUnidentified;
            // Validate character
            if (!string.IsNullOrWhiteSpace(charID))
            {
                c = Utility_Methods.GetCharacter(charID, out charErr);
            }
            else
            {
                c = client.myPlayerCharacter;
            }

            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            // Perform authorization
            if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter, c))
            {
                ProtoMessage campMessage;
                c.CampWaitHere(days, out campMessage);
                return campMessage;
            }
            // if unauthorised, error
            return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
        }

        public static ProtoMessage AddRemoveEntourage(string charID, Client client)
        {
            DisplayMessages charErr;
            // validate character
            var c = Utility_Methods.GetCharacter(charID, out charErr);
            if (c == null)
            {
                return new ProtoMessage(charErr);
            }
            var myNPC = (c as NonPlayerCharacter);
            if (myNPC == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericCharacterUnidentified);
            }
            var pc = client.myPlayerCharacter;
            // ensure player is authorized to add to entourage
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, myNPC))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Ensure playercharacter is not captured
            if (!string.IsNullOrWhiteSpace(pc.captorID))
            {
                return new ProtoMessage(DisplayMessages.CharacterHeldCaptive);
            }
            // If in entourage, remove- otherwise, add
            if ((c as NonPlayerCharacter).inEntourage)
            {
                pc.RemoveFromEntourage(myNPC);
            }
            else
            {
                // Player character must be in same location as npc to add
                if (pc.location != myNPC.location)
                {
                    var error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                    return error;
                }
                pc.AddToEntourage(myNPC);
            }
            // return entourage
            var newEntourage = new List<ProtoCharacterOverview>();
            foreach (var entourageChar in pc.myEntourage)
            {
                newEntourage.Add(new ProtoCharacterOverview(entourageChar));
            }
            var result =
                new ProtoGenericArray<ProtoCharacterOverview>(newEntourage.ToArray());
            result.ResponseType = DisplayMessages.Success;
            return result;
        }

        public static ProtoMessage ProposeMarriage(string groomID, string brideID, Client client)
        {
            DisplayMessages groomErr, brideErr;
            var groom = Utility_Methods.GetCharacter(groomID, out groomErr);
            if (groom == null)
            {
                return new ProtoMessage(groomErr);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, client.myPlayerCharacter, groom))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            var bride = Utility_Methods.GetCharacter(brideID, out brideErr);
            if (bride == null)
            {
                return new ProtoMessage(brideErr);
            }
            var madeProposal = false;
            if (!string.IsNullOrWhiteSpace(groom.captorID) || !string.IsNullOrWhiteSpace(bride.captorID))
            {
                return new ProtoMessage(DisplayMessages.CharacterHeldCaptive);
            }
            ProtoMessage proposalError;
            if (groom.ChecksBeforeProposal(bride, out proposalError))
            {
                madeProposal = groom.ProposeMarriage(bride);
            }
            if (!madeProposal)
            {
                return proposalError;
            }
            var success = new ProtoMessage();
            success.ResponseType = DisplayMessages.Success;
            success.Message = "Proposal success";
            return success;
        }

        public static ProtoMessage AcceptRejectProposal(uint jEntryID, bool accept, Client client)
        {
            // Attempt to get Journal entry
            JournalEntry jEntry = null;
            client.myPastEvents.entries.TryGetValue(jEntryID, out jEntry);
            if (jEntry == null)
            {
                return new ProtoMessage(DisplayMessages.JournalEntryUnrecognised);
            }
            if (!jEntry.CheckForProposal(client.myPlayerCharacter))
            {
                return new ProtoMessage(DisplayMessages.JournalEntryNotProposal);
            }
            var success = jEntry.ReplyToProposal(accept);
            if (success)
            {
                var proposeresult = new ProtoMessage();
                proposeresult.ResponseType = DisplayMessages.Success;
                return proposeresult;
            }
            return new ProtoMessage(DisplayMessages.Error);
        }

        public static ProtoMessage AppointHeir(string charID, Client client)
        {
            // Cannot appioint an heir if captured
            if (!string.IsNullOrWhiteSpace(client.myPlayerCharacter.captorID))
            {
                return new ProtoMessage(DisplayMessages.CharacterHeldCaptive);
            }
            // validate character
            DisplayMessages charErr;
            var heirTemp = Utility_Methods.GetCharacter(charID, out charErr);
            if (heirTemp == null)
            {
                return new ProtoMessage(charErr);
            }

            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, client.myPlayerCharacter, heirTemp))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            var heir = (heirTemp as NonPlayerCharacter);
            ProtoMessage heirError;
            if (heir == null || !heir.ChecksForHeir(client.myPlayerCharacter, out heirError))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.CharacterHeir;
                return error;
            }
            // check for an existing heir and remove
            foreach (var npc in client.myPlayerCharacter.myNPCs)
            {
                if (npc.isHeir)
                {
                    npc.isHeir = false;
                }
            }
            heir.isHeir = true;
            var result = new ProtoCharacter(heir);
            result.ResponseType = DisplayMessages.Success;
            result.includeAll(heir);
            return result;
        }

        public static ProtoMessage TryForChild(string charID, Client client)
        {
            // Get father
            DisplayMessages charErr;
            var father = Utility_Methods.GetCharacter(charID, out charErr);
            if (father == null)
            {
                return new ProtoMessage(charErr);
            }
            // Authorize
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    father))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // Confirm can get pregnant
            ProtoMessage birthError;
            if (!Birth.ChecksBeforePregnancyAttempt(father, out birthError))
            {
                return birthError;
            }
            // Move so that both husband and wife are in/out of keep
            father.GetSpouse().inKeep = father.inKeep;
            ProtoMessage birthMessage;
            var pregnant = father.GetSpousePregnant(father.GetSpouse(), out birthMessage);
            // At this point the rest is handled by the GetSpousePregnant method
            return birthMessage;
        }

        public static ProtoMessage RecruitTroops(string armyID, uint amount, bool isConfirm, Client client)
        {
            // Get army to recruit into
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null || !client.myPlayerCharacter.myArmies.Contains(army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericArmyUnidentified);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // recruit troops
            var numTroops = amount;
            return client.myPlayerCharacter.RecruitTroops(numTroops, army, isConfirm);
        }

        public static ProtoMessage MaintainArmy(string armyID, Client client)
        {
            DisplayMessages armyErrorMessage;
            var army = Utility_Methods.GetArmy(armyID, out armyErrorMessage);
            if (army == null)
            {
                return new ProtoMessage(armyErrorMessage);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage result;
            army.MaintainArmy(out result);
            return result;
        }

        public static ProtoMessage ListArmies(Client client)
        {
            var armies = new ProtoGenericArray<ProtoArmyOverview>();
            armies.fields = new ProtoArmyOverview[client.myPlayerCharacter.myArmies.Count];
            var i = 0;
            foreach (var army in client.myPlayerCharacter.myArmies)
            {
                var armyDetails = new ProtoArmyOverview(army);
                armyDetails.includeAll(army);
                armies.fields[i] = armyDetails;
                i++;
            }
            return armies;
        }

        public static ProtoMessage AppointLeader(string armyID, string charID, Client client)
        {
            // Get army
            DisplayMessages armyErr, charErr;
            var army = Utility_Methods.GetArmy(armyID, out armyErr);
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                return new ProtoMessage(armyErr);
            }
            // Get leader
            var newLeader = Utility_Methods.GetCharacter(charID, out charErr);
            if (newLeader == null)
            {
                return new ProtoMessage(charErr);
            }
            // Authorize
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army) ||
                (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    newLeader)))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // Check char can be leader, grant
            ProtoMessage grantError;

            var canBeLeader = newLeader.ChecksBeforeGranting(client.myPlayerCharacter, "leader", false, out grantError,
                army.armyID);
            if (canBeLeader)
            {
                army.AssignNewLeader(newLeader);
                return new ProtoArmy(army, client.myPlayerCharacter);
            }
                // Checks before granting will return own error messages
            return grantError;
        }

        public static ProtoMessage DropOffTroops(string armyID, uint[] troops, string charID, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID) || string.IsNullOrWhiteSpace(charID) || troops == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Get army
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                return error;
            }
            // Authorize
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // Create a detachment
            ProtoMessage detachmentResult;
            if (army.CreateDetachment(troops, charID, out detachmentResult))
            {
                var success = new ProtoArmy(army, client.myPlayerCharacter);
                success.includeAll(army);
                success.ResponseType = DisplayMessages.Success;
                return success;
            }
            return detachmentResult;
        }

        public static ProtoMessage ListDetachments(string armyID, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Get army to pick up detachments
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                return error;
            }
            // check permissions
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // List available transfers
            var myAvailableTransfers = new List<ProtoDetachment>();
            foreach (var transferDetails in army.GetLocation().troopTransfers.Values)
            {
                if (transferDetails.leftFor.Equals(client.myPlayerCharacter.charID))
                {
                    myAvailableTransfers.Add(transferDetails);
                }
            }
            var detachmentList = new ProtoGenericArray<ProtoDetachment>(myAvailableTransfers.ToArray());
            detachmentList.ResponseType = DisplayMessages.Success;
            return detachmentList;
        }

        public static ProtoMessage PickUpTroops(string armyID, string[] detachmentIDs, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID) || detachmentIDs == null || detachmentIDs.Length == 0)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Army to pick up detachments
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                return error;
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            var pickupMessage = army.ProcessPickups(detachmentIDs);
            if (pickupMessage != null)
            {
                if (pickupMessage.ResponseType == DisplayMessages.ArmyPickupsNotEnoughDays)
                {
                    var armyDetails = new ProtoArmy(army, client.myPlayerCharacter);
                    armyDetails.ResponseType = pickupMessage.ResponseType;
                    return armyDetails;
                }
                return pickupMessage;
            }
            var updatedArmy = new ProtoArmy(army, client.myPlayerCharacter);
            updatedArmy.includeAll(army);
            updatedArmy.ResponseType = DisplayMessages.Success;
            return updatedArmy;
        }

        public static ProtoMessage PillageFief(string armyID, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Get army
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                return error;
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            ProtoMessage pillageError;
            var canPillage = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(), out pillageError);
            if (canPillage)
            {
                var result = Pillage_Siege.PillageFief(army, army.GetLocation());
                result.ResponseType = DisplayMessages.Success;
                result.Message = "pillage";
                return result;
            }
            // CheckBeforePillage returns own error messages
            return pillageError;
        }

        public static ProtoMessage BesiegeFief(string armyID, Client client)
        {
            // Get army
            if (string.IsNullOrEmpty(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericArmyUnidentified);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage pillageError;
            var canSiege = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(), out pillageError, "siege");
            if (canSiege)
            {
                var newSiege = Pillage_Siege.SiegeStart(army, army.GetLocation());
                var result = new ProtoSiegeDisplay(newSiege);
                result.ResponseType = DisplayMessages.Success;
                result.Message = "siege";
                return result;
            }
            return pillageError;
        }

        public static ProtoMessage SiegeRoundNegotiate(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // Check besieger is pc
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            siege.SiegeReductionRound("negotiation");
            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage SiegeRoundReduction(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            siege.SiegeReductionRound();
            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage SiegeRoundStorm(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            siege.SiegeReductionRound("storm");

            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage EndSiege(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError, "end"))
            {
                return siegeError;
            }
            siege.SiegeEnd(false);
            var reply = new ProtoMessage();
            reply.ResponseType = DisplayMessages.Success;
            reply.Message = siege.siegeID;
            return reply;
        }

        public static ProtoMessage ViewSiege(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            if (!client.myPlayerCharacter.mySieges.Contains(siegeID))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            var siegeDetails = new ProtoSiegeDisplay(client.myPlayerCharacter.GetSiege(siegeID));
            siegeDetails.ResponseType = DisplayMessages.Success;
            return siegeDetails;
        }

        public static ProtoMessage AdjustCombatValues(string armyID, byte aggression, byte odds, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Get army
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericArmyUnidentified);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Attempt to adjust standing orders
            var success = army.AdjustStandingOrders(aggression, odds);
            if (success)
            {
                var result = new ProtoCombatValues(army.aggression, army.combatOdds, army.armyID);
                result.ResponseType = DisplayMessages.Success;
                return result;
            }
            return new ProtoMessage(DisplayMessages.Error) { Message ="adjust standing orders" };
        }

        public static ProtoMessage ExamineArmiesInFief(string fiefID, Client client)
        {
            if (string.IsNullOrWhiteSpace(fiefID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // get fief
            Fief fief = null;
            Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
            if (fief == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                return error;
            }
            // Check character is in fief, owns fief, or is admin
            if (!PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, client.myPlayerCharacter, fief))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // get list of armies
            var armies = new List<ProtoArmyOverview>();
            foreach (var armyID in fief.armies)
            {
                Army army = null;
                Globals_Game.armyMasterList.TryGetValue(armyID, out army);
                if (army != null)
                {
                    armies.Add(new ProtoArmyOverview(army));
                }
            }
            // Return array of overviews
            var armyList = new ProtoGenericArray<ProtoArmyOverview>(armies.ToArray());
            armyList.ResponseType = DisplayMessages.Armies;
            return armyList;
        }

        public static ProtoMessage Attack(string attackerID, string defenderID, Client client)
        {
            DisplayMessages attackerErrorMessage, defenderErrorMessage;
            // Get attacker and defender
            var armyAttacker = Utility_Methods.GetArmy(attackerID, out attackerErrorMessage);
            var armyDefender = Utility_Methods.GetArmy(defenderID, out defenderErrorMessage);
            if (armyAttacker == null)
            {
                return new ProtoMessage(attackerErrorMessage);
            }
            if (armyDefender == null)
            {
                return new ProtoMessage(defenderErrorMessage);
            }
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter,
                    armyAttacker))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // In the event an army has no troops, return error, log event and clean up
            if (armyAttacker.troops == null || armyAttacker.CalcArmySize() == 0 || armyDefender.troops == null ||
                armyDefender.CalcArmySize() == 0)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.Error;
                Globals_Server.logError("Found an army with no troops- Performing clean up");
                if (armyAttacker.troops == null || armyAttacker.CalcArmySize() == 0)
                {
                    armyAttacker.DisbandArmy();
                }
                else
                {
                    armyDefender.DisbandArmy();
                }
                return error;
            }
            ProtoMessage attackResult = null;
            if (armyAttacker.ChecksBeforeAttack(armyDefender, out attackResult))
            {
                // GiveBattle returns necessary messages
                ProtoBattle battleResults = null;
                try
                {
                    var isVictorious = Battle.GiveBattle(armyAttacker, armyDefender, out battleResults);
                    battleResults.ResponseType = DisplayMessages.BattleResults;
                    return battleResults;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }
            return attackResult;
        }

        public static ProtoMessage ViewJournalEntries(string scope, Client client)
        {
            // Get list of journal entries for scope
            var entries = client.myPastEvents.getJournalEntrySet(scope, Globals_Game.clock.currentYear,
                Globals_Game.clock.currentSeason);
            var entryList = new ProtoJournalEntry[entries.Count];
            var i = 0;
            foreach (var entry in entries)
            {
                var newEntry = new ProtoJournalEntry(entry.Value);
                entryList[i] = newEntry;
                i++;
            }
            var result = new ProtoGenericArray<ProtoJournalEntry>(entryList);
            result.ResponseType = DisplayMessages.JournalEntries;
            return result;
        }

        public static ProtoMessage ViewJournalEntry(uint journalID, Client client)
        {
            JournalEntry jEntry = null;
            client.myPastEvents.entries.TryGetValue(journalID, out jEntry);
            if (jEntry == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                return error;
            }
            var reply = new ProtoJournalEntry(jEntry);
            reply.ResponseType = DisplayMessages.Success;
            return reply;
        }

        public static ProtoMessage SpyArmy(string armyID, string charID, Client client)
        {
            DisplayMessages armyErr, charErr;
            var army = Utility_Methods.GetArmy(armyID, out armyErr);
            var spy = Utility_Methods.GetCharacter(charID, out charErr);

            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            // Ensure character and army are valid
            if (spy == null)
            {
                return new ProtoMessage(charErr);
            }
            if (army == null)
            {
                return new ProtoMessage(armyErr);
            }
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(army, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(army, out result);
            return result;
        }

        /// <summary>
        /// Process a client's request to spy on a character
        /// </summary>
        /// <param name="charID">The spy's character ID</param>
        /// <param name="targetID">The target's character ID</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of attempting to spy</returns>
        public static ProtoMessage SpyCharacter(string charID, string targetID, Client client)
        {
            DisplayMessages charErr, targetErr;
            var target = Utility_Methods.GetCharacter(targetID, out targetErr);
            var spy = Utility_Methods.GetCharacter(charID, out charErr);
            // Ensure character and army are valid
            if (spy == null)
            {
                return new ProtoMessage(charErr);
            }
            if (target == null)
            {
                return new ProtoMessage(targetErr);
            }
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(target, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(target, out result);
            return result;
        }

        /// <summary>
        /// Process a client's request to spy on a fief
        /// </summary>
        /// <param name="charID">Character ID of spy</param>
        /// <param name="fiefID">Fief ID of fief to spy on</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of attempt to spy</returns>
        public static ProtoMessage SpyFief(string charID, string fiefID, Client client)
        {
            DisplayMessages fiefErrMsg, charErrMsg;
            var fief = Utility_Methods.GetFief(fiefID, out fiefErrMsg);
            var spy = Utility_Methods.GetCharacter(charID, out charErrMsg);
            // Ensure character and army are valid
            if (spy == null)
                return new ProtoMessage(charErrMsg);
            if (fief == null)
                return new ProtoMessage(fiefErrMsg);
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(fief, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(fief, out result);
            return result;
        }


        /// <summary>
        ///     Kidnap a target character. Client must own kidnapper, and both targetID and kidnapperID must be valid Character IDs
        /// </summary>
        /// <param name="targetID">Character ID of target Character</param>
        /// <param name="kidnapperID">Character ID of kidnapper</param>
        /// <param name="client">Client who requested kidnapping (must own kidnapper)</param>
        /// <returns>Result of attempted kidnapping, or an error message</returns>
        public static ProtoMessage Kidnap(string targetID, string kidnapperID, Client client)
        {
            DisplayMessages targetErrMsg;
            DisplayMessages kidnapperErrMsg;
            var target = Utility_Methods.GetCharacter(targetID, out targetErrMsg);
            var kidnapper = Utility_Methods.GetCharacter(kidnapperID, out kidnapperErrMsg);
            if (kidnapper == null)
            {
                return new ProtoMessage(kidnapperErrMsg);
            }
            if (target == null)
            {
                return new ProtoMessage(targetErrMsg);
            }
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    kidnapper))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage result;
            kidnapper.Kidnap(target, out result);
            return result;
        }

        /// <summary>
        ///     Fief all captives in a location. Location can be "all" for all held captives across all Fiefs, or a Fief ID for
        ///     captives in that fief
        /// </summary>
        /// <param name="captiveLocation">Fief ID or "all"</param>
        /// <param name="client">Client who requested to view captives</param>
        /// <returns>ProtoGenericArray of ProtoCharacterOverview containing details of all captives, or an error message</returns>
        public static ProtoMessage ViewCaptives(string captiveLocation, Client client)
        {
            if (string.IsNullOrWhiteSpace(captiveLocation))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            List<Character> captiveList = null;
            if (captiveLocation.Equals("all"))
            {
                captiveList = client.myPlayerCharacter.myCaptives;
            }
            else
            {
                // If not all captives, check for fief captives
                DisplayMessages errorMsg;
                var fief = Utility_Methods.GetFief(captiveLocation, out errorMsg);
                if (fief != null)
                {
                    // Ensure has permission
                    if (
                        !PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter,
                            fief))
                    {
                        var error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                        return error;
                    }
                    captiveList = fief.gaol;
                }
                else
                {
                    // error
                    var error = new ProtoMessage();
                    error.ResponseType = errorMsg;
                    return error;
                }
            }
            if (captiveList != null && captiveList.Count > 0)
            {
                var captives = new ProtoGenericArray<ProtoCharacterOverview>();
                captives.fields = new ProtoCharacterOverview[captiveList.Count];
                var i = 0;
                foreach (var captive in captiveList)
                {
                    var captiveDetails = new ProtoCharacterOverview(captive);
                    captiveDetails.showLocation(captive);
                    captives.fields[i] = captiveDetails;
                    i++;
                }
                captives.ResponseType = DisplayMessages.Success;
                return captives;
            }
            var NoCaptives = new ProtoMessage();
            NoCaptives.ResponseType = DisplayMessages.FiefNoCaptives;
            return NoCaptives;
        }

        public static ProtoMessage ViewCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            var captiveDetails = new ProtoCharacter(captive);
            captiveDetails.ResponseType = DisplayMessages.Success;
            return captiveDetails;
        }

        public static ProtoMessage RansomCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            if (!string.IsNullOrWhiteSpace(captive.ransomDemand))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.RansomAlready;
                return error;
            }
            client.myPlayerCharacter.RansomCaptive(captive);
            var success = new ProtoMessage();
            success.ResponseType = DisplayMessages.Success;
            success.Message = captive.CalculateRansom().ToString();
            return success;
        }

        public static ProtoMessage ReleaseCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            Character captive = null;
            captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            client.myPlayerCharacter.ReleaseCaptive(captive);
            var success = new ProtoMessage();
            success.ResponseType = DisplayMessages.Success;
            return success;
        }

        /// <summary>
        ///     Process a client request to kill a captive
        /// </summary>
        /// <param name="charID">ID of character to execute</param>
        /// <param name="client">client who sent request</param>
        /// <returns>Result of this request</returns>
        public static ProtoMessage ExecuteCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                return new ProtoMessage(DisplayMessages.NotCaptive);
            }
            client.myPlayerCharacter.ExecuteCaptive(captive);
            return new ProtoMessage(DisplayMessages.Success);
        }

        public static ProtoMessage RespondRansom(uint jEntryID, bool pay, Client client)
        {
            JournalEntry jEntry = null;
            Globals_Game.pastEvents.entries.TryGetValue(jEntryID, out jEntry);
            if (jEntry == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.JournalEntryUnrecognised;
                return error;
            }
            ProtoMessage ransomResult = null;
            if (jEntry.RansomResponse(pay, out ransomResult))
            {
                ransomResult = new ProtoMessage();
                ransomResult.ResponseType = DisplayMessages.Success;
                return ransomResult;
            }
            return ransomResult;
        }

        /// <summary>
        /// Translates a message from a client into an action to be carried out
        /// </summary>
        /// <param name="msgIn">The client's request. Cannot be null</param>
        /// <param name="_client">The client who sent the request. Cannot be null</param>
        /// <returns>The result of attempting to process the client's request, ranging from the result of an action to an error message</returns>
        public static ProtoMessage ActionController(ProtoMessage msgIn, Client _client)
        {
            Contract.Requires(msgIn!=null && _client!=null);
            switch (msgIn.ActionType)
            {
                // Switch to using another character (performing actions with NPC
                case Actions.UseChar:
                {
                    return UseChar(msgIn.Message, _client);
                }
                case Actions.GetPlayers:
                {
                    return GetPlayers(_client);
                }
                // View a specific character
                case Actions.ViewChar:
                {
                    return ViewCharacter(msgIn.Message, _client);
                }
                // Hire an NPC
                case Actions.HireNPC:
                {
                    // Convert bid to UInt
                    uint bid = 0;
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length != 1 ||
                        !uint.TryParse(msgIn.MessageFields[0], out bid))
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericPositiveInteger);
                    }
                    return HireNPC(msgIn.Message, bid, _client);
                }
                // Fire an NPC
                case Actions.FireNPC:
                {
                    return FireNPC(msgIn.Message, _client);
                }
                // View an army
                case Actions.ViewArmy:
                {
                    return ViewArmy(msgIn.Message, _client);
                }
                case Actions.DisbandArmy:
                {
                    return DisbandArmy(msgIn.Message, _client);
                }
                // Get a list of NPCs- if listing chars in meeting place use "ListCharsInMeetingPlace"
                case Actions.GetNPCList:
                {
                    string type, role, item;
                    role = item = null;
                    type = msgIn.Message;
                    if (msgIn.MessageFields != null)
                    {
                        if (msgIn.MessageFields.Length == 2)
                        {
                            role = msgIn.MessageFields[0];
                            item = msgIn.MessageFields[1];
                        }
                        else if (msgIn.MessageFields.Length == 1)
                        {
                            role = msgIn.MessageFields[0];
                        }
                    }
                    return GetNPCList(type, role, item, _client);
                }
                // Can travel to fief if are/own character, and have valid fief. Days etc taken into account elsewhere
                case Actions.TravelTo:
                    // Attempt to convert message to ProtoTravelTo
                    var travelTo = msgIn as ProtoTravelTo;
                    if (travelTo != null)
                    {
                        return TravelTo(travelTo.characterID, travelTo.travelTo, travelTo.travelVia, _client);
                    }
                    // Message was not valid
                    return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);

                // View a fief. Can view a fief if in it, own it, or have a character in it
                case Actions.ViewFief:
                {
                    return ViewFief(msgIn.Message, _client);
                }
                case Actions.ViewMyFiefs:
                {
                    return ViewMyFiefs(_client);
                }
                // Appoint a character as a bailiff to a fief
                case Actions.AppointBailiff:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return AppointBailiff(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                // Remove bailiff from fief
                case Actions.RemoveBailiff:
                {
                    return RemoveBailiff(msgIn.Message, _client);
                }
                // Bar a number of characters from a fief
                case Actions.BarCharacters:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length == 0)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return BarCharacters(msgIn.Message, msgIn.MessageFields, _client);
                }
                // Unbar a number of characters from fief
                case Actions.UnbarCharacters:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length == 0)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return UnbarCharacters(msgIn.Message, msgIn.MessageFields, _client);
                }
                // Bar a number of nationalities from fief
                case Actions.BarNationalities:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return BarNationalities(msgIn.Message, msgIn.MessageFields, _client);
                }
                // Unbar a number of nationalities from fief
                case Actions.UnbarNationalities:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return UnbarNationalities(msgIn.Message, msgIn.MessageFields, _client);
                }
                // Grant a fief title to a character
                case Actions.GrantFiefTitle:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return GrantFiefTitle(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                // Adjust fief expenditure
                case Actions.AdjustExpenditure:
                {
                    var newRates = msgIn as ProtoGenericArray<double>;
                    if (newRates == null)
                    {
                        return AdjustExpenditure(msgIn.Message, null, _client);
                    }
                    return AdjustExpenditure(msgIn.Message, newRates.fields, _client);
                }
                // Transfer funds between fiefs
                case Actions.TransferFunds:
                {
                    var transferDetails = msgIn as ProtoTransfer;
                    if (transferDetails == null)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return TransferFunds(transferDetails.fiefFrom, transferDetails.fiefTo, transferDetails.amount,
                        _client);
                }
                // Transfer funds from pc's home fief to another player's home fief
                case Actions.TransferFundsToPlayer:
                {
                    // Try to convert to ProtoTransferPlayer
                    var transferDetails = msgIn as ProtoTransferPlayer;
                    // If failed return error
                    if (transferDetails == null)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return TransferFundsToPlayer(transferDetails.playerTo, transferDetails.amount, _client);
                }
                // Instruct a character to enter or exit keep
                case Actions.EnterExitKeep:
                {
                    return EnterExitKeep(msgIn.Message, _client);
                }
                // List chars in court, tavern or outside fief
                case Actions.ListCharsInMeetingPlace:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return ListCharsInMeetingPlace(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                // Instruct a character to camp where they are for a number of days
                case Actions.Camp:
                {
                    if (msgIn.MessageFields != null && msgIn.MessageFields.Length > 0)
                    {
                        try
                        {
                            return Camp(msgIn.Message, Convert.ToByte(msgIn.MessageFields[0]), _client);
                        }
                        catch (Exception e)
                        {
                            return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                        }
                    }
                    return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                }
                // add or remove a character to the entourage
                case Actions.AddRemoveEntourage:
                {
                    return AddRemoveEntourage(msgIn.Message, _client);
                }
                // Propose marriage between two characters
                case Actions.ProposeMarriage:
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return ProposeMarriage(msgIn.Message, msgIn.MessageFields[0], _client);
                // Reply to a proposal (accept or reject)
                case Actions.AcceptRejectProposal:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    try
                    {
                        return AcceptRejectProposal(Convert.ToUInt32(msgIn.Message),
                            Convert.ToBoolean(msgIn.MessageFields[0]), _client);
                    }
                    catch (Exception e)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                }
                // Appoint a new heir
                case Actions.AppointHeir:
                {
                    return AppointHeir(msgIn.Message, _client);
                }
                // Attempt to have a child
                case Actions.TryForChild:
                {
                    return TryForChild(msgIn.Message, _client);
                }
                // Recruit troops from fief
                case Actions.RecruitTroops:
                {
                    var details = msgIn as ProtoRecruit;
                    if (details == null)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return RecruitTroops(details.armyID, details.amount, details.isConfirm, _client);
                }
                // Maintain an army
                case Actions.MaintainArmy:
                {
                    return MaintainArmy(msgIn.Message, _client);
                }
                // Appoint a new army leader
                case Actions.AppointLeader:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return AppointLeader(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                // Drop off troops in fief 
                case Actions.DropOffTroops:
                {
                    // Convert to ProtoDetachment
                    var detachmentDetails = (msgIn as ProtoDetachment);
                    if (detachmentDetails == null)
                    {
                        var error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                        return error;
                    }
                    return DropOffTroops(detachmentDetails.armyID, detachmentDetails.troops,
                        detachmentDetails.leftFor, _client);
                }
                case Actions.ListArmies:
                {
                    return ListArmies(_client);
                }

                // List detachments in fief
                case Actions.ListDetachments:
                {
                    return ListDetachments(msgIn.Message, _client);
                }
                // Pick up detachments 
                // TODO add ability to create new army by picking up troops
                case Actions.PickUpTroops:
                {
                    return PickUpTroops(msgIn.Message, msgIn.MessageFields, _client);
                }
                // Pillage a fief
                case Actions.PillageFief:
                {
                    return PillageFief(msgIn.Message, _client);
                }
                // Besiege a fief
                case Actions.BesiegeFief:
                {
                    return BesiegeFief(msgIn.Message, _client);
                }
                // Perform siege negotiation round
                case Actions.SiegeRoundNegotiate:
                {
                    return SiegeRoundNegotiate(msgIn.Message, _client);
                }
                // Perform a siege reduction round
                case Actions.SiegeRoundReduction:
                {
                    return SiegeRoundReduction(msgIn.Message, _client);
                }
                // Perform siege storm round
                case Actions.SiegeRoundStorm:
                {
                    return SiegeRoundStorm(msgIn.Message, _client);
                }
                case Actions.EndSiege:
                {
                    return EndSiege(msgIn.Message, _client);
                }
                    break;
                // List player's sieges
                case Actions.SiegeList:
                {
                    var sieges = new List<ProtoSiegeOverview>();
                    foreach (var siegeID in _client.myPlayerCharacter.mySieges)
                    {
                        sieges.Add(new ProtoSiegeOverview(_client.myPlayerCharacter.GetSiege(siegeID)));
                    }
                    var siegeList = new ProtoGenericArray<ProtoSiegeOverview>(sieges.ToArray());
                    return siegeList;
                }
                case Actions.ViewSiege:
                {
                    var siegeID = msgIn.Message;
                    return ViewSiege(siegeID, _client);
                }
                    break;
                // Adjust combat values
                case Actions.AdjustCombatValues:
                {
                    // Convert incoming message to ProtoCombatValues
                    var newValues = (msgIn as ProtoCombatValues);
                    if (newValues == null)
                    {
                        var error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                        return error;
                    }
                    return AdjustCombatValues(newValues.armyID, newValues.aggression, newValues.odds, _client);
                }
                // Examine the armies in a fief
                case Actions.ExamineArmiesInFief:
                {
                    return ExamineArmiesInFief(msgIn.Message, _client);
                }
                // Attack an army
                case Actions.Attack:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return Attack(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                // View journal entries
                case Actions.ViewJournalEntries:
                {
                    return ViewJournalEntries(msgIn.Message, _client);
                }
                    break;
                case Actions.ViewJournalEntry:
                {
                    try
                    {
                        var jID = Convert.ToUInt32(msgIn.Message);
                        return ViewJournalEntry(jID, _client);
                    }
                    catch (Exception e)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                }
                case Actions.SpyArmy:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return SpyArmy(msgIn.MessageFields[0], msgIn.Message, _client);
                }
                    break;
                case Actions.SpyCharacter:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return SpyCharacter(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                case Actions.SpyFief:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return SpyFief(msgIn.MessageFields[0], msgIn.Message, _client);
                }
                case Actions.Kidnap:
                {
                    if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                    return Kidnap(msgIn.Message, msgIn.MessageFields[0], _client);
                }
                case Actions.ViewCaptives:
                {
                    return ViewCaptives(msgIn.Message, _client);
                }
                case Actions.ViewCaptive:
                {
                    return ViewCaptive(msgIn.Message, _client);
                }
                case Actions.RansomCaptive:
                {
                    return RansomCaptive(msgIn.Message, _client);
                }
                case Actions.ReleaseCaptive:
                {
                    return ReleaseCaptive(msgIn.Message, _client);
                }
                case Actions.ExecuteCaptive:
                {
                    return ExecuteCaptive(msgIn.Message, _client);
                }
                case Actions.RespondRansom:
                {
                    try
                    {
                        var entryID = Convert.ToUInt32(msgIn.Message);
                        return RespondRansom(entryID, Convert.ToBoolean(msgIn.MessageFields[0]), _client);
                    }
                    catch (Exception e)
                    {
                        return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
                    }
                }
                case Actions.SeasonUpdate:
                {
                    Globals_Game.game.SeasonUpdate();
                    var updated = new ProtoMessage();
                    updated.ResponseType = DisplayMessages.Success;
                    return updated;
                }
                    break;
                default:
                {
                    return null;
                }
            }
        }

        //TODO remove or implement
        /*
        /// <summary>
        /// Checks for a historical team victory (victory depends on whether the English own any French fiefs)
        /// </summary>
        /// <returns>Kingdom object belonging to victor</returns>
        public Kingdom CheckTeamHistoricalVictory()
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
        public Kingdom CheckTeamAbsoluteVictory()
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
        public List<JournalEntry> ProcessScheduledEvents()
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

                            // refresh household display
                            this.RefreshHouseholdDisplay();
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

        } */

        // ------------------- EXIT/CLOSE
    }
}
