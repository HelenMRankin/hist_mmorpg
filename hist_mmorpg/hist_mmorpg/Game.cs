﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace hist_mmorpg
{
    /// <summary>
    /// Class representing game
    /// It will initialise all the game objects and control the flow of the game
    /// The control code from all the form classes will either be put in here or accessed from here
    /// For example, if a server gets an incoming message from a client and finds that it is some game action to be taken,
    /// it might send this to Game, where the precise action will be determined from the message, carried out and the result send back via server
    /// </summary>
    public class Game
    {
        public Game()
        {
            Globals_Game.game = this;
            // initialise game objects
            // This path handling should ensure that the correct path will be found in Linux, Windows or debug mode
            String dir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            // if the program is being run in debug mode, this will obtain the correct directory
            if (dir.Contains("bin"))
            {
                dir = Directory.GetParent(dir).FullName;
            }
            String path = Path.Combine(dir, "CSVs");
            String gameObjects = Path.Combine(path, "gameObjects.csv");
            String mapData = Path.Combine(path, "map.csv");
            this.InitGameObjects(gameID: "fromCSV", objectDataFile: gameObjects, mapDataFile: mapData,
            start: 1194, king1: "Char_47", king2: "Char_40", herald1: "Char_1", sysAdmin: "Char_158");

            // this.ImportFromCSV("gameObjects.csv", bucketID: "fromCSV", synch: true, toDatabase: true);
            // this.CreateMapArrayFromCSV ("map.csv", bucketID: "fromCSV", toDatabase: true);
        }
        /// <summary>
        /// Initialises all game objects
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
        /// 
        //TODO need to refactor- should initialise game data, then add clients as player characters
        public void InitGameObjects(string gameID = null, string objectDataFile = null,
            string mapDataFile = null, uint type = 0, uint duration = 100, uint start = 1337, string king1 = null,
            string king2 = null, string herald1 = null, string herald2 = null, string sysAdmin = null)
        {
            bool dataLoaded = false;

            // LOAD DATA
            // database
            if (Globals_Game.loadFromDatabase)
            {
                // load objects
                //HACK readd
              //  DatabaseRead.DatabaseReadAll(gameID);

                dataLoaded = true;
            }
            // CSV file
            else if (Globals_Game.loadFromCSV)
            {
                // load objects (mainly) from CSV file
                if (!String.IsNullOrWhiteSpace(objectDataFile))
                {
                    // load objects
                    CSVimport.NewGameFromCSV(objectDataFile, mapDataFile, start);

                    // initialise Globals_Game.victoryData
                    this.SynchroniseVictoryData();

                    dataLoaded = true;
                }
            }
            // from code (for quick testing)
            if (!dataLoaded)
            {
                // load objects
                this.LoadFromCode();

                // initialise Globals_Game.victoryData
                this.SynchroniseVictoryData();
            }

            if ((!Globals_Game.loadFromDatabase) || (!dataLoaded))
            {
                // INITIALISE ROLES
                // set kings
                if (!String.IsNullOrWhiteSpace(king1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king1))
                    {
                        Globals_Game.kingOne = Globals_Game.pcMasterList[king1];
                    }
                }
                if (!String.IsNullOrWhiteSpace(king2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king2))
                    {
                        Globals_Game.kingTwo = Globals_Game.pcMasterList[king2];
                    }
                }

                // set heralds
                if (!String.IsNullOrWhiteSpace(herald1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald1))
                    {
                        Globals_Game.heraldOne = Globals_Game.pcMasterList[herald1];
                    }
                }
                if (!String.IsNullOrWhiteSpace(herald2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald2))
                    {
                        Globals_Game.heraldTwo = Globals_Game.pcMasterList[herald2];
                    }
                }

                // set sysAdmin
                if (!String.IsNullOrWhiteSpace(sysAdmin))
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
            uint[] myArmyTroops1 = new uint[] { 5, 10, 0, 30, 40, 4000, 6020 };
            Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
            myArmy1.AddArmy();

            // create and add army
            uint[] myArmyTroops2 = new uint[] { 5, 10, 0, 30, 40, 80, 220 };
            Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
            myArmy2.AddArmy();
            /*
            // create ailment
            Ailment myAilment = new Ailment(Globals_Game.getNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, 1, 0);
            Globals_Game.pcMasterList["Char_196"].ailments.Add(myAilment.ailmentID, myAilment); */
            // =================== END TESTING

        }
        /// <summary>
        /// Creates some game objects from code (temporary, for testing)
        /// </summary>
        /// <param name="start">Start year</param>
        public void LoadFromCode(uint start = 1337)
        {
            // create GameClock
            Globals_Game.clock = new GameClock("clock_1", start);

            // create traits
            // Dictionary of trait effects
            Dictionary<string, double> effectsCommand = new Dictionary<string, double>();
            effectsCommand.Add("battle", 0.4);
            effectsCommand.Add("siege", 0.4);
            effectsCommand.Add("npcHire", 0.2);
            // create trait
            Trait command = new Trait("trait_1", "Command", effectsCommand);
            // add to traitCollection
            Globals_Game.traitMasterList.Add(command.id, command);

            Dictionary<string, double> effectsChivalry = new Dictionary<string, double>();
            effectsChivalry.Add("famExpense", 0.2);
            effectsChivalry.Add("fiefExpense", 0.1);
            effectsChivalry.Add("fiefLoy", 0.2);
            effectsChivalry.Add("npcHire", 0.1);
            effectsChivalry.Add("siege", 0.1);
            Trait chivalry = new Trait("trait_2", "Chivalry", effectsChivalry);
            Globals_Game.traitMasterList.Add(chivalry.id, chivalry);

            Dictionary<string, double> effectsAbrasiveness = new Dictionary<string, double>();
            effectsAbrasiveness.Add("battle", 0.15);
            effectsAbrasiveness.Add("death", 0.05);
            effectsAbrasiveness.Add("fiefExpense", -0.05);
            effectsAbrasiveness.Add("famExpense", 0.05);
            effectsAbrasiveness.Add("time", 0.05);
            effectsAbrasiveness.Add("siege", -0.1);
            Trait abrasiveness = new Trait("trait_3", "Abrasiveness", effectsAbrasiveness);
            Globals_Game.traitMasterList.Add(abrasiveness.id, abrasiveness);

            Dictionary<string, double> effectsAccountancy = new Dictionary<string, double>();
            effectsAccountancy.Add("time", 0.1);
            effectsAccountancy.Add("fiefExpense", -0.2);
            effectsAccountancy.Add("famExpense", -0.2);
            effectsAccountancy.Add("fiefLoy", -0.05);
            Trait accountancy = new Trait("trait_4", "Accountancy", effectsAccountancy);
            Globals_Game.traitMasterList.Add(accountancy.id, accountancy);

            Dictionary<string, double> effectsStupidity = new Dictionary<string, double>();
            effectsStupidity.Add("battle", -0.4);
            effectsStupidity.Add("death", 0.05);
            effectsStupidity.Add("fiefExpense", 0.2);
            effectsStupidity.Add("famExpense", 0.2);
            effectsStupidity.Add("fiefLoy", -0.1);
            effectsStupidity.Add("npcHire", -0.1);
            effectsStupidity.Add("time", -0.1);
            effectsStupidity.Add("siege", -0.4);
            Trait stupidity = new Trait("trait_5", "Stupidity", effectsStupidity);
            Globals_Game.traitMasterList.Add(stupidity.id, stupidity);

            Dictionary<string, double> effectsRobust = new Dictionary<string, double>();
            effectsRobust.Add("virility", 0.2);
            effectsRobust.Add("npcHire", 0.05);
            effectsRobust.Add("fiefLoy", 0.05);
            effectsRobust.Add("death", -0.2);
            Trait robust = new Trait("trait_6", "Robust", effectsRobust);
            Globals_Game.traitMasterList.Add(robust.id, robust);

            Dictionary<string, double> effectsPious = new Dictionary<string, double>();
            effectsPious.Add("virility", -0.2);
            effectsPious.Add("npcHire", 0.1);
            effectsPious.Add("fiefLoy", 0.1);
            effectsPious.Add("time", -0.1);
            Trait pious = new Trait("trait_7", "Pious", effectsPious);
            Globals_Game.traitMasterList.Add(pious.id, pious);

            // add each traitsCollection key to traitsKeys
            foreach (KeyValuePair<string, Trait> entry in Globals_Game.traitMasterList)
            {
                Globals_Game.traitKeys.Add(entry.Key);
            }

            // create BaseLanguage & Language objects
            BaseLanguage c = new BaseLanguage("lang_C", "Celtic");
            Globals_Game.baseLanguageMasterList.Add(c.id, c);
            Language c1 = new Language(c, 1);
            Globals_Game.languageMasterList.Add(c1.id, c1);
            Language c2 = new Language(c, 2);
            Globals_Game.languageMasterList.Add(c2.id, c2);
            BaseLanguage f = new BaseLanguage("lang_F", "French");
            Globals_Game.baseLanguageMasterList.Add(f.id, f);
            Language f1 = new Language(f, 1);
            Globals_Game.languageMasterList.Add(f1.id, f1);
            BaseLanguage e = new BaseLanguage("lang_E", "English");
            Globals_Game.baseLanguageMasterList.Add(e.id, e);
            Language e1 = new Language(e, 1);
            Globals_Game.languageMasterList.Add(e1.id, e1);

            // create terrain objects
            Terrain plains = new Terrain("terr_P", "Plains", 1);
            Globals_Game.terrainMasterList.Add(plains.id, plains);
            Terrain hills = new Terrain("terr_H", "Hills", 1.5);
            Globals_Game.terrainMasterList.Add(hills.id, hills);
            Terrain forrest = new Terrain("terr_F", "Forrest", 1.5);
            Globals_Game.terrainMasterList.Add(forrest.id, forrest);
            Terrain mountains = new Terrain("terr_M", "Mountains", 15);
            Globals_Game.terrainMasterList.Add(mountains.id, mountains);
            Terrain impassable_mountains = new Terrain("terr_MX", "Impassable mountains", 91);
            Globals_Game.terrainMasterList.Add(impassable_mountains.id, impassable_mountains);

            // create keep barred lists for fiefs
            List<string> keep1BarChars = new List<string>();
            List<string> keep2BarChars = new List<string>();
            List<string> keep3BarChars = new List<string>();
            List<string> keep4BarChars = new List<string>();
            List<string> keep5BarChars = new List<string>();
            List<string> keep6BarChars = new List<string>();
            List<string> keep7BarChars = new List<string>();

            // create chars lists for fiefs
            List<Character> fief1Chars = new List<Character>();
            List<Character> fief2Chars = new List<Character>();
            List<Character> fief3Chars = new List<Character>();
            List<Character> fief4Chars = new List<Character>();
            List<Character> fief5Chars = new List<Character>();
            List<Character> fief6Chars = new List<Character>();
            List<Character> fief7Chars = new List<Character>();

            // create ranks for kingdoms, provinces, fiefs
            TitleName[] myTitleName03 = new TitleName[4];
            myTitleName03[0] = new TitleName("lang_C1", "King");
            myTitleName03[1] = new TitleName("lang_C2", "King");
            myTitleName03[2] = new TitleName("lang_E1", "King");
            myTitleName03[3] = new TitleName("lang_F1", "Roi");
            Rank myRank03 = new Rank(3, myTitleName03, 6);
            Globals_Game.rankMasterList.Add(myRank03.id, myRank03);

            TitleName[] myTitleName09 = new TitleName[4];
            myTitleName09[0] = new TitleName("lang_C1", "Prince");
            myTitleName09[1] = new TitleName("lang_C2", "Prince");
            myTitleName09[2] = new TitleName("lang_E1", "Prince");
            myTitleName09[3] = new TitleName("lang_F1", "Prince");
            Rank myRank09 = new Rank(9, myTitleName09, 4);
            Globals_Game.rankMasterList.Add(myRank09.id, myRank09);

            TitleName[] myTitleName11 = new TitleName[4];
            myTitleName11[0] = new TitleName("lang_C1", "Earl");
            myTitleName11[1] = new TitleName("lang_C2", "Earl");
            myTitleName11[2] = new TitleName("lang_E1", "Earl");
            myTitleName11[3] = new TitleName("lang_F1", "Comte");
            Rank myRank11 = new Rank(11, myTitleName11, 4);
            Globals_Game.rankMasterList.Add(myRank11.id, myRank11);

            TitleName[] myTitleName15 = new TitleName[4];
            myTitleName15[0] = new TitleName("lang_C1", "Baron");
            myTitleName15[1] = new TitleName("lang_C2", "Baron");
            myTitleName15[2] = new TitleName("lang_E1", "Baron");
            myTitleName15[3] = new TitleName("lang_F1", "Baron");
            Rank myRank15 = new Rank(15, myTitleName15, 2);
            Globals_Game.rankMasterList.Add(myRank15.id, myRank15);

            TitleName[] myTitleName17 = new TitleName[4];
            myTitleName17[0] = new TitleName("lang_C1", "Lord");
            myTitleName17[1] = new TitleName("lang_C2", "Lord");
            myTitleName17[2] = new TitleName("lang_E1", "Lord");
            myTitleName17[3] = new TitleName("lang_F1", "Sire");
            Rank myRank17 = new Rank(17, myTitleName17, 1);
            Globals_Game.rankMasterList.Add(myRank17.id, myRank17);

            // create Nationality objects for Kingdoms, Characters and positions
            Nationality nationality01 = new Nationality("Fr", "French");
            Globals_Game.nationalityMasterList.Add(nationality01.natID, nationality01);
            Nationality nationality02 = new Nationality("Eng", "English");
            Globals_Game.nationalityMasterList.Add(nationality02.natID, nationality02);

            // create Positions
            TitleName myTiName01 = new TitleName("lang_C1", "Keeper of the Privy Seal");
            TitleName myTiName011 = new TitleName("lang_C2", "Keeper of the Privy Seal");
            TitleName myTiName012 = new TitleName("lang_E1", "Keeper of the Privy Seal");
            TitleName[] myTitles01 = new TitleName[] { myTiName01, myTiName011, myTiName012 };
            Position myPos01 = new Position(100, myTitles01, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos01.id, myPos01);
            TitleName myTiName02 = new TitleName("lang_C1", "Lord High Steward");
            TitleName myTiName021 = new TitleName("lang_C2", "Lord High Steward");
            TitleName myTiName022 = new TitleName("lang_E1", "Lord High Steward");
            TitleName[] myTitles02 = new TitleName[] { myTiName02, myTiName021, myTiName022 };
            Position myPos02 = new Position(101, myTitles02, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos02.id, myPos02);

            // create kingdoms for provinces
            Kingdom myKingdom1 = new Kingdom("E0000", "England", nationality02, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom1.id, myKingdom1);
            Kingdom myKingdom2 = new Kingdom("F0000", "France", nationality01, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom2.id, myKingdom2);

            // create provinces for fiefs
            Province myProv = new Province("ESX00", "Sussex", 6.2, king: myKingdom1, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv.id, myProv);
            Province myProv2 = new Province("ESR00", "Surrey", 6.2, king: myKingdom2, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv2.id, myProv2);

            // create financial arrays for fiefs
            double[] prevFin001 = new double[] { 6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2, 340894 };
            double[] currFin001 = new double[] { 5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2, 350274 };
            double[] prevFin002 = new double[14];
            double[] currFin002 = new double[14];
            double[] prevFin003 = new double[14];
            double[] currFin003 = new double[14];
            double[] prevFin004 = new double[14];
            double[] currFin004 = new double[14];
            double[] prevFin005 = new double[14];
            double[] currFin005 = new double[14];
            double[] prevFin006 = new double[] { 6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2, 340894 };
            double[] currFin006 = new double[] { 5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2, 350274 };
            double[] prevFin007 = new double[14];
            double[] currFin007 = new double[14];

            // create armies lists for fiefs
            List<string> armies001 = new List<string>();
            List<string> armies002 = new List<string>();
            List<string> armies003 = new List<string>();
            List<string> armies004 = new List<string>();
            List<string> armies005 = new List<string>();
            List<string> armies006 = new List<string>();
            List<string> armies007 = new List<string>();

            // create troop transfer lists for fiefs
            Dictionary<string, string[]> transfers001 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers002 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers003 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers004 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers005 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers006 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers007 = new Dictionary<string, string[]>();

            // create barredNationalities for fiefs
            List<string> barredNats01 = new List<string>();
            List<string> barredNats02 = new List<string>();
            List<string> barredNats03 = new List<string>();
            List<string> barredNats04 = new List<string>();
            List<string> barredNats05 = new List<string>();
            List<string> barredNats06 = new List<string>();
            List<string> barredNats07 = new List<string>();

            Fief myFief1 = new Fief("ESX02", "Cuckfield", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'C', c1, plains, fief1Chars, keep1BarChars, barredNats01, 0, 2000000, armies001, false, transfers001, false);
            Globals_Game.fiefMasterList.Add(myFief1.id, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", null, null, myRank15, myProv, 10000, 3.50, 0.20, 50, 10, 10, 1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'C', c1, hills, fief2Chars, keep2BarChars, barredNats02, 0, 4000, armies002, false, transfers002, false);
            Globals_Game.fiefMasterList.Add(myFief2.id, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', c1, plains, fief3Chars, keep3BarChars, barredNats03, 0, 100000, armies003, false, transfers003, false);
            Globals_Game.fiefMasterList.Add(myFief3.id, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', c1, plains, fief4Chars, keep4BarChars, barredNats04, 0, 100000, armies004, false, transfers004, false);
            Globals_Game.fiefMasterList.Add(myFief4.id, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", null, null, myRank15, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', f1, plains, fief5Chars, keep5BarChars, barredNats05, 0, 100000, armies005, false, transfers005, false);
            Globals_Game.fiefMasterList.Add(myFief5.id, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", null, null, myRank17, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', f1, plains, fief6Chars, keep6BarChars, barredNats06, 0, 100000, armies006, false, transfers006, false);
            Globals_Game.fiefMasterList.Add(myFief6.id, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", null, null, myRank15, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', f1, forrest, fief7Chars, keep7BarChars, barredNats07, 0, 100000, armies007, false, transfers007, false);
            Globals_Game.fiefMasterList.Add(myFief7.id, myFief7);

            // create QuickGraph undirected graph
            // 1. create graph
            var myHexMap = new HexMapGraph("map001");
            Globals_Game.gameMap = myHexMap;
            // 2. Add edge and auto create vertices
            // from myFief1
            myHexMap.AddHexesAndRoute(myFief1, myFief2, "W", (myFief1.terrain.travelCost + myFief2.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief1, myFief3, "E", (myFief1.terrain.travelCost + myFief3.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief1, myFief6, "NE", (myFief1.terrain.travelCost + myFief6.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief1, myFief4, "SE", (myFief1.terrain.travelCost + myFief4.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief1, myFief5, "SW", (myFief1.terrain.travelCost + myFief5.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief1, myFief7, "NW", (myFief1.terrain.travelCost + myFief7.terrain.travelCost) / 2);
            // from myFief2
            myHexMap.AddHexesAndRoute(myFief2, myFief1, "E", (myFief2.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief2, myFief7, "NE", (myFief2.terrain.travelCost + myFief7.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief2, myFief5, "SE", (myFief2.terrain.travelCost + myFief5.terrain.travelCost) / 2);
            // from myFief3
            myHexMap.AddHexesAndRoute(myFief3, myFief4, "SW", (myFief3.terrain.travelCost + myFief4.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief3, myFief6, "NW", (myFief3.terrain.travelCost + myFief6.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief3, myFief1, "W", (myFief3.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            // from myFief4
            myHexMap.AddHexesAndRoute(myFief4, myFief3, "NE", (myFief4.terrain.travelCost + myFief3.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief4, myFief1, "NW", (myFief4.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief4, myFief5, "W", (myFief4.terrain.travelCost + myFief5.terrain.travelCost) / 2);
            // from myFief5
            myHexMap.AddHexesAndRoute(myFief5, myFief1, "NE", (myFief5.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief5, myFief2, "NW", (myFief5.terrain.travelCost + myFief2.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief5, myFief4, "E", (myFief5.terrain.travelCost + myFief4.terrain.travelCost) / 2);
            // from myFief6
            myHexMap.AddHexesAndRoute(myFief6, myFief3, "SE", (myFief6.terrain.travelCost + myFief3.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief6, myFief1, "SW", (myFief6.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief6, myFief7, "W", (myFief6.terrain.travelCost + myFief7.terrain.travelCost) / 2);
            // from myFief7
            myHexMap.AddHexesAndRoute(myFief7, myFief6, "E", (myFief7.terrain.travelCost + myFief6.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief7, myFief1, "SE", (myFief7.terrain.travelCost + myFief1.terrain.travelCost) / 2);
            myHexMap.AddHexesAndRoute(myFief7, myFief2, "SW", (myFief7.terrain.travelCost + myFief2.terrain.travelCost) / 2);

            // create goTo queues for characters
            Queue<Fief> myGoTo1 = new Queue<Fief>();
            Queue<Fief> myGoTo2 = new Queue<Fief>();
            Queue<Fief> myGoTo3 = new Queue<Fief>();
            Queue<Fief> myGoTo4 = new Queue<Fief>();
            Queue<Fief> myGoTo5 = new Queue<Fief>();
            Queue<Fief> myGoTo6 = new Queue<Fief>();
            Queue<Fief> myGoTo7 = new Queue<Fief>();
            Queue<Fief> myGoTo8 = new Queue<Fief>();
            Queue<Fief> myGoTo9 = new Queue<Fief>();
            Queue<Fief> myGoTo10 = new Queue<Fief>();
            Queue<Fief> myGoTo11 = new Queue<Fief>();
            Queue<Fief> myGoTo12 = new Queue<Fief>();

            // add some goTo entries for myChar1
            //myGoTo1.Enqueue (myFief2);
            //myGoTo1.Enqueue (myFief7);

            // create entourages for PCs
            List<NonPlayerCharacter> myEmployees1 = new List<NonPlayerCharacter>();
            List<NonPlayerCharacter> myEmployees2 = new List<NonPlayerCharacter>();

            // create lists of fiefs owned by PCs and add some fiefs
            List<Fief> myFiefsOwned1 = new List<Fief>();
            List<Fief> myFiefsOwned2 = new List<Fief>();

            // create lists of provinces owned by PCs and add some fiefs
            List<Province> myProvsOwned1 = new List<Province>();
            List<Province> myProvsOwned2 = new List<Province>();

            // create DOBs for characters
            Tuple<uint, byte> myDob001 = new Tuple<uint, byte>(1161, 1);
            Tuple<uint, byte> myDob002 = new Tuple<uint, byte>(1134, 0);
            Tuple<uint, byte> myDob003 = new Tuple<uint, byte>(1152, 2);
            Tuple<uint, byte> myDob004 = new Tuple<uint, byte>(1169, 3);
            Tuple<uint, byte> myDob005 = new Tuple<uint, byte>(1167, 2);
            Tuple<uint, byte> myDob006 = new Tuple<uint, byte>(1159, 2);
            Tuple<uint, byte> myDob007 = new Tuple<uint, byte>(1159, 3);
            Tuple<uint, byte> myDob008 = new Tuple<uint, byte>(1181, 2);
            Tuple<uint, byte> myDob009 = new Tuple<uint, byte>(1179, 0);
            Tuple<uint, byte> myDob010 = new Tuple<uint, byte>(1179, 0);
            Tuple<uint, byte> myDob011 = new Tuple<uint, byte>(1176, 1);
            Tuple<uint, byte> myDob012 = new Tuple<uint, byte>(1177, 3);

            // create titles list for characters
            List<string> myTitles001 = new List<string>();
            List<string> myTitles002 = new List<string>();
            List<string> myTitles003 = new List<string>();
            List<string> myTitles004 = new List<string>();
            List<string> myTitles005 = new List<string>();
            List<string> myTitles006 = new List<string>();
            List<string> myTitles007 = new List<string>();
            List<string> myTitles008 = new List<string>();
            List<string> myTitles009 = new List<string>();
            List<string> myTitles010 = new List<string>();
            List<string> myTitles011 = new List<string>();
            List<string> myTitles012 = new List<string>();

            // create armies list for PCs
            List<Army> myArmies001 = new List<Army>();
            List<Army> myArmies002 = new List<Army>();

            // create sieges list for PCs
            List<string> mySieges001 = new List<string>();
            List<string> mySieges002 = new List<string>();

            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("Char_47", "Dave", "Bond", myDob001, true, nationality02, true, 8.50, 9.0, myGoTo1, c1, 90, 0, 7.2, 6.1, Utility_Methods.GenerateTraitSet(), false, false, "Char_47", "Char_403", null, null, false, 13000, myEmployees1, myFiefsOwned1, myProvsOwned1, "ESX02", "ESX02", myTitles001, myArmies001, mySieges001, null, loc: myFief1, pID: "libdab");
            Globals_Game.pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("Char_40", "Bave", "Dond", myDob002, true, nationality01, true, 8.50, 6.0, myGoTo2, f1, 90, 0, 5.0, 4.5, Utility_Methods.GenerateTraitSet(), false, false, "Char_40", null, null, null, false, 13000, myEmployees2, myFiefsOwned2, myProvsOwned2, "ESR03", "ESR03", myTitles002, myArmies002, mySieges002, null, loc: myFief7, pID: "otherGuy");
            Globals_Game.pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("Char_401", "Jimmy", "Servant", myDob003, true, nationality02, true, 8.50, 6.0, myGoTo3, c1, 90, 0, 3.3, 6.7, Utility_Methods.GenerateTraitSet(), false, false, null, null, null, null, 0, false, false, myTitles003, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("Char_402", "Johnny", "Servant", myDob004, true, nationality02, true, 8.50, 6.0, myGoTo4, c1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), false, false, null, null, null, null, 10000, true, false, myTitles004, null, empl: myChar1.charID, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myNPC3 = new NonPlayerCharacter("Char_403", "Harry", "Bailiff", myDob005, true, nationality01, true, 8.50, 6.0, myGoTo5, c1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), true, false, null, null, null, null, 10000, false, false, myTitles005, null, empl: myChar2.charID, loc: myFief6);
            Globals_Game.npcMasterList.Add(myNPC3.charID, myNPC3);
            NonPlayerCharacter myChar1Wife = new NonPlayerCharacter("Char_404", "Bev", "Bond", myDob006, false, nationality02, true, 2.50, 9.0, myGoTo6, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), false, false, "Char_47", "Char_47", null, null, 30000, false, false, myTitles006, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            NonPlayerCharacter myChar2Son = new NonPlayerCharacter("Char_405", "Horatio", "Dond", myDob007, true, nationality01, true, 8.50, 6.0, myGoTo7, f1, 90, 0, 7.1, 5.2, Utility_Methods.GenerateTraitSet(), true, false, "Char_40", "Char_406", "Char_40", null, 10000, false, true, myTitles007, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            NonPlayerCharacter myChar2SonWife = new NonPlayerCharacter("Char_406", "Mave", "Dond", myDob008, false, nationality02, true, 2.50, 9.0, myGoTo8, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_40", "Char_405", null, null, 30000, false, false, myTitles008, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);
            NonPlayerCharacter myChar1Son = new NonPlayerCharacter("Char_407", "Rickie", "Bond", myDob009, true, nationality02, true, 2.50, 9.0, myGoTo9, c1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, true, myTitles009, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Son.charID, myChar1Son);
            NonPlayerCharacter myChar1Daughter = new NonPlayerCharacter("Char_408", "Elsie", "Bond", myDob010, false, nationality02, true, 2.50, 9.0, myGoTo10, c1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, false, myTitles010, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Daughter.charID, myChar1Daughter);
            NonPlayerCharacter myChar2Son2 = new NonPlayerCharacter("Char_409", "Wayne", "Dond", myDob011, true, nationality01, true, 2.50, 9.0, myGoTo11, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles011, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son2.charID, myChar2Son2);
            NonPlayerCharacter myChar2Daughter = new NonPlayerCharacter("Char_410", "Esmerelda", "Dond", myDob012, false, nationality01, true, 2.50, 9.0, myGoTo12, f1, 90, 0, 4.0, 6.0, Utility_Methods.GenerateTraitSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles012, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Daughter.charID, myChar2Daughter);

            /*
            // create and add a scheduled birth
            string[] birthPersonae = new string[] { myChar1Wife.familyID + "|headOfFamily", myChar1Wife.charID + "|mother", myChar1Wife.spouse + "|father" };
            JournalEntry myEntry = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 1, birthPersonae, "birth");
            Globals_Game.scheduledEvents.entries.Add(myEntry.jEntryID, myEntry); */

            // get character's correct days allowance
            myChar1.days = myChar1.GetDaysAllowance();
            myChar2.days = myChar2.GetDaysAllowance();
            myNPC1.days = myNPC1.GetDaysAllowance();
            myNPC2.days = myNPC2.GetDaysAllowance();
            myNPC3.days = myNPC3.GetDaysAllowance();
            myChar1Wife.days = myChar1Wife.GetDaysAllowance();
            myChar2Son.days = myChar2Son.GetDaysAllowance();
            myChar2SonWife.days = myChar2SonWife.GetDaysAllowance();
            myChar1Son.days = myChar1Son.GetDaysAllowance();
            myChar1Daughter.days = myChar1Daughter.GetDaysAllowance();
            myChar2Son2.days = myChar2Son2.GetDaysAllowance();
            myChar2Daughter.days = myChar2Daughter.GetDaysAllowance();

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

            // create an army and add in appropriate places
            uint[] myArmyTroops = new uint[] { 10, 10, 0, 100, 100, 200, 400 };
            Army myArmy = new Army(Globals_Game.GetNextArmyID(), null, myChar1.charID, 90, myChar1.location.id, trp: myArmyTroops);
            Globals_Game.armyMasterList.Add(myArmy.armyID, myArmy);
            myChar1.myArmies.Add(myArmy);
            // myChar1.armyID = myArmy.armyID;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = new uint[] { 10, 10, 30, 0, 40, 200, 0 };
            Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), myChar2.charID, myChar2.charID, myChar2.days, myChar2.location.id, trp: myArmyTroops2, aggr: 1);
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
        /// Ensures that the Globals_Game.victoryData is up-to-date
        /// </summary>
        public void SynchroniseVictoryData()
        {
            List<string> toRemove = new List<string>();
            List<VictoryData> toAdd = new List<VictoryData>();

            // iterate through Globals_Game.victoryData
            foreach (KeyValuePair<string, VictoryData> vicDataEntry in Globals_Game.victoryData)
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
                    if ((String.IsNullOrWhiteSpace(thisPC.playerID)) || (!thisPC.isAlive))
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
                for (int i = 0; i < toRemove.Count; i++)
                {
                    Globals_Game.victoryData.Remove(toRemove[i]);
                }
            }
            toRemove.Clear();

            // iterate through pcMasterList
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Game.pcMasterList)
            {
                // check for playerID (i.e. active player)
                if (!String.IsNullOrWhiteSpace(pcEntry.Value.playerID))
                {
                    // check if is in Globals_Game.victoryData
                    if (!Globals_Game.victoryData.ContainsKey(pcEntry.Value.playerID))
                    {
                        // create and add new VictoryData if necessary
                        VictoryData newVicData = new VictoryData(pcEntry.Value.playerID, pcEntry.Value.charID, pcEntry.Value.CalculateStature(), pcEntry.Value.GetPopulationPercentage(), pcEntry.Value.GetFiefsPercentage(), pcEntry.Value.GetMoneyPercentage());
                        toAdd.Add(newVicData);
                    }
                }
            }

            // add any new Globals_Game.victoryData entries
            if (toAdd.Count > 0)
            {
                for (int i = 0; i < toAdd.Count; i++)
                {
                    Globals_Game.victoryData.Add(toAdd[i].playerID, toAdd[i]);
                }
            }
            toAdd.Clear();
        }

        public void PerformAction(PlayerCharacter pc, string action, string object_ID = null)
        {
            switch (action)
            {

            }
        }
        // ------------------- SEASONAL UPDATE

        /// <summary>
        /// Updates game objects at end/start of season
        /// </summary>
        public void SeasonUpdate()
        {
            // used to check if character update is necessary
            bool performCharacterUpdate = true;

            // FIEFS
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                fiefEntry.Value.UpdateFief();
            }

            // NONPLAYERCHARACTERS
            foreach (KeyValuePair<string, NonPlayerCharacter> npcEntry in Globals_Game.npcMasterList)
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
                        if ((String.IsNullOrWhiteSpace(npcEntry.Value.employer)) && (String.IsNullOrWhiteSpace(npcEntry.Value.familyID)))
                        {
                            npcEntry.Value.RandomMoveNPC();
                        }

                        // finish previously started multi-hex move if necessary
                        if (npcEntry.Value.goTo.Count > 0)
                        {
                            npcEntry.Value.CharacterMultiMove();
                        }
                    }
                }

            }

            // PLAYERCHARACTERS
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Game.pcMasterList)
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
                            pcEntry.Value.CharacterMultiMove();
                        }
                    }
                }
            }

            // add any newly promoted NPCs to Globals_Game.pcMasterList
            if (Globals_Game.promotedNPCs.Count > 0)
            {
                foreach (PlayerCharacter pc in Globals_Game.promotedNPCs)
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
            bool hasDissolved = false;
            Dictionary<string, Army> updateArmy = new Dictionary<string, Army>();
            // iterate through armies
            foreach (KeyValuePair<string, Army> armyEntry in Globals_Game.armyMasterList)
            {
                hasDissolved = armyEntry.Value.UpdateArmy();

                // add to dissolvedArmies if appropriate
                if (hasDissolved)
                {
                    //armyEntry.Value.DisbandArmy();
                    updateArmy.Add(armyEntry.Key, armyEntry.Value);
                }
            }
            Globals_Game.armyMasterList = updateArmy;

            // SIEGES

            // keep track of any sieges requiring removal
            bool hasEnded = false;

            // iterate through sieges
            foreach (KeyValuePair<string, Siege> siegeEntry in Globals_Game.siegeMasterList)
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
            bool gameEnded = Globals_Game.CheckForVictory();

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
                List<JournalEntry> entriesForRemoval = Globals_Game.ProcessScheduledEvents();

                // remove processed events from Globals_Game.scheduledEvents
                for (int i = 0; i < entriesForRemoval.Count; i++)
                {
                    Globals_Game.scheduledEvents.entries.Remove(entriesForRemoval[i].jEntryID);
                }
            }
        }

        /// <summary>
        /// Allows a player to switch character
        /// </summary>
        /// <param name="user">userID of the player</param>
        /// <param name="charID">characterID of the playercharacter to be switched to</param>
        public void switchPlayerCharacter(string user, string charID)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                //TODO error log
            }
            if (!Globals_Game.userChars.ContainsKey(user))
            {
                //TODO error log
            }
            else
            {
                Client c;
                PlayerCharacter pc;
                if (string.IsNullOrWhiteSpace(charID))
                {
                    Globals_Game.UpdatePlayer(user, DisplayMessages.SwitchPlayerErrorNoID);
                }
                // Checks that the character is available
                if (!Globals_Game.pcMasterList.ContainsKey(charID) || Globals_Game.userChars.ContainsValue(Globals_Game.pcMasterList[charID]))
                {
                    Globals_Game.UpdatePlayer(user, DisplayMessages.SwitchPlayerErrorIDInvalid);
                }
                else
                {
                    pc = Globals_Game.pcMasterList[charID];

                    Globals_Game.userChars[user] = pc;
                    //TODO write new user character to database
                    if (Globals_Server.clients.ContainsKey(user))
                    {
                        c = Globals_Server.clients[user];
                        c.charToView = pc;
                        c.myPlayerCharacter = pc;
                        c.fiefToView = pc.location;
                    }
                }
            }
        }

        public static ProtoMessage ActionController(ProtoMessage msgIn, PlayerCharacter pc)
        {
            switch ((Actions)msgIn.MessageType)
            {
                case Actions.ViewChar:
                    Character c;
                    if (Globals_Game.pcMasterList.ContainsKey(msgIn.Message))
                    {
                        c = Globals_Game.pcMasterList[msgIn.Message];
                    }
                    else if (Globals_Game.npcMasterList.ContainsKey(msgIn.Message))
                    {
                        c = Globals_Game.npcMasterList[msgIn.Message];
                    }
                    else
                    {
                        ProtoMessage msg = new ProtoMessage();
                        msg.MessageType = Actions.Error;
                        msg.Message = "CharIDInvalid";
                        return msg;
                    }
                    bool viewAll = PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin,pc,c);
                    ProtoCharacter characterDetails = new ProtoCharacter(c);
                    if (viewAll)
                    {
                        characterDetails.includeAll(c);
                    }
                    return characterDetails;
                case Actions.GetNPCList:
                    if (msgIn.Message.Equals("FIEF"))
                    {
                        Fief fief = pc.location;
                        List<ProtoCharacterOverview> charList = new List<ProtoCharacterOverview>();
                        foreach (Character character in fief.charactersInFief) {
                            charList.Add(new ProtoCharacterOverview(character));
                        }
                        ProtoGenericArray< ProtoCharacterOverview> charsInFief= new ProtoGenericArray<ProtoCharacterOverview>(charList.ToArray());
                        return charsInFief;
                    }
                    else if (msgIn.Message.Equals("COURT")||msgIn.Message.Equals("TAVERN"))
                    {
                        Fief fief = pc.location;
                        List<ProtoCharacterOverview> charList = new List<ProtoCharacterOverview>();
                        foreach (Character character in fief.charactersInFief)
                        {
                            if (character != pc)
                            {
                                if (msgIn.Message.Equals("TAVERN"))
                                {
                                    if (string.IsNullOrWhiteSpace((character as NonPlayerCharacter).employer))
                                    {
                                        charList.Add(new ProtoCharacterOverview(character));
                                    }
                                }
                                else
                                {
                                    charList.Add(new ProtoCharacterOverview(character));
                                }
                            }
                        }
                        ProtoGenericArray<ProtoCharacterOverview> charsInFief = new ProtoGenericArray<ProtoCharacterOverview>(charList.ToArray());
                        return charsInFief;
                    }
                    
                    break;
                case Actions.TravelTo:
                    ProtoTravelTo travelTo = msgIn as ProtoTravelTo;
                    if (travelTo != null)
                    {
                        Character charToMove = Globals_Game.pcMasterList[travelTo.characterID];
                        if (charToMove == null)
                        {
                            charToMove = Globals_Game.npcMasterList[travelTo.characterID];
                        }
                        if (charToMove == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.MessageType = Actions.Error;
                            error.Message = "CharIDInvalid";
                            return error;
                        }
                        else
                        {
                            if (PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, pc, charToMove))
                            {
                                if (Globals_Game.fiefMasterList.ContainsKey(travelTo.travelTo))
                                {
                                    Fief fief = Globals_Game.fiefMasterList[msgIn.Message];
                                    double travelCost = charToMove.location.getTravelCost(fief, charToMove.armyID);
                                    if (charToMove.MoveCharacter(fief, travelCost))
                                    {
                                        return new ProtoFief(fief);
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                                else if (travelTo.travelVia != null)
                                {
                                    charToMove.TakeThisRoute(travelTo.travelVia);
                                    return new ProtoFief(charToMove.location);
                                }
                                else
                                {
                                    ProtoMessage msg = new ProtoMessage();
                                    msg.MessageType = Actions.Error;
                                    msg.Message = "FiefIDInvalid";
                                    return msg;
                                }
                            }
                            else
                            {
                                ProtoMessage unauthorised = new ProtoMessage();
                                unauthorised.MessageType = DisplayMessages.ErrorGenericUnauthorised;
                                return unauthorised;
                            }
                        }
                    }
                    else
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.MessageType = Actions.Error;
                        error.Message = "InvalidTravelMessage";
                        return error;
                    }

                case Actions.ViewFief:
                    break;
                case Actions.AppointBailiff:
                    break;
                case Actions.RemoveBailiff:
                    break;
                case Actions.BarCharacter:
                    break;
                case Actions.UnbarCharacter:
                    break;
                case Actions.BarNationality:
                    break;
                case Actions.GrantFiefTitle:
                    break;
                case Actions.AdjustExpenditure:
                    break;
                case Actions.TransferFunds:
                    break;
                case Actions.TransferFundsToPlayer:
                    break;
                case Actions.EnterExitKeep:
                    break;
                case Actions.ListCharsInMeetinPlace:
                    break;
                case Actions.TakeThisRoute:
                    break;
                case Actions.Camp:
                    break;
                case Actions.AddToEntourage:
                    break;
                case Actions.ProposeMarriage:
                    break;
                case Actions.AppointHeir:
                    break;
                case Actions.TryForChild:
                    break;
                case Actions.RecruitTroops:
                    break;
                case Actions.MaintainArmy:
                    break;
                case Actions.AppointLeader:
                    break;
                case Actions.DropOffTroops:
                    break;
                case Actions.ListDetachments:
                    break;
                case Actions.PickUpTroops:
                    break;
                case Actions.PillageFief:
                    break;
                case Actions.BesiegeFief:
                    break;
                case Actions.AdjustCombatValues:
                    break;
                case Actions.ExamineArmiesInFief:
                    break;
                case Actions.Attack:
                    break;
                case Actions.ViewJournalEntries:
                    break;
                default:
                    break;

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
