using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Diagnostics;
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
        /// <summary>
        /// Used for including all subtypes of ProtoMessage
        /// </summary>
      /*  public void initialiseTypes()
        {
            List<Type> subMessages = typeof(ProtoMessage).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ProtoMessage))).ToList();
            int i = 50;
            RuntimeTypeModel testmodel = new RuntimeTypeModel();
            foreach (Type message in subMessages)
            {
                if (message != typeof(ProtoPlayerCharacter) && message != typeof(ProtoNPC)&& !message.ToString().Contains("GenericArray"))
                {
                    
                    Console.WriteLine("adding type:" + message.ToString());
                    RuntimeTypeModel.Default.Add(typeof(ProtoMessage), true).AddSubType(i, message);
                    testmodel.Add(typeof(ProtoMessage), true).AddSubType(i, message);
                    i++;
                }
            }
            testmodel.Compile("compiledTypeModel", "E:\\");
        }*/

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
			this.InitGameObjects(gameID: "testBucket", objectDataFile: gameObjects, mapDataFile: mapData,
            start: 1194, king1: "Char_47", king2: "Char_40", herald1: "Char_1", sysAdmin: null);
           // initialiseTypes();
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
                DatabaseRead.DatabaseReadAll(gameID);

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

        public void addTestCharacter()
        {
            Nationality nat =  Globals_Game.nationalityMasterList["Sco"];
            NonPlayerCharacter proposalChar = new NonPlayerCharacter("Char_626", "Mairi", "Meah", new Tuple<uint, byte>(1162, 3), false, Globals_Game.nationalityMasterList["Sco"], true, 9, 9,new Queue<Fief>(), Globals_Game.languageMasterList["lang_C1"], 90, 9, 9, 9,new Tuple<Trait,int>[0], true, false, "Char_126", null, "Char_126", null, 0, false, false,new List<string>(), null, null,Globals_Game.fiefMasterList["ESW05"]);
            PlayerCharacter pc = Globals_Game.pcMasterList["Char_126"];
            pc.myNPCs.Add(proposalChar);
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
            Dictionary<Globals_Game.Stats, double> effectsCommand = new Dictionary<Globals_Game.Stats, double>();
            effectsCommand.Add(Globals_Game.Stats.BATTLE, 0.4);
            effectsCommand.Add(Globals_Game.Stats.SIEGE, 0.4);
            effectsCommand.Add(Globals_Game.Stats.NPCHIRE, 0.2);
            // create trait
            Trait command = new Trait("trait_1", "Command", effectsCommand);
            // add to traitCollection
            Globals_Game.traitMasterList.Add(command.id, command);

            Dictionary<Globals_Game.Stats, double> effectsChivalry = new Dictionary<Globals_Game.Stats, double>();
            effectsChivalry.Add(Globals_Game.Stats.FAMEXPENSE, 0.2);
            effectsChivalry.Add(Globals_Game.Stats.FIEFEXPENSE, 0.1);
            effectsChivalry.Add(Globals_Game.Stats.FIEFLOY, 0.2);
            effectsChivalry.Add(Globals_Game.Stats.NPCHIRE, 0.1);
            effectsChivalry.Add(Globals_Game.Stats.SIEGE, 0.1);
            Trait chivalry = new Trait("trait_2", "Chivalry", effectsChivalry);
            Globals_Game.traitMasterList.Add(chivalry.id, chivalry);

            Dictionary<Globals_Game.Stats, double> effectsAbrasiveness = new Dictionary<Globals_Game.Stats, double>();
            effectsAbrasiveness.Add(Globals_Game.Stats.BATTLE, 0.15);
            effectsAbrasiveness.Add(Globals_Game.Stats.DEATH, 0.05);
            effectsAbrasiveness.Add(Globals_Game.Stats.FIEFEXPENSE, -0.05);
            effectsAbrasiveness.Add(Globals_Game.Stats.FAMEXPENSE, 0.05);
            effectsAbrasiveness.Add(Globals_Game.Stats.TIME, 0.05);
            effectsAbrasiveness.Add(Globals_Game.Stats.SIEGE, -0.1);
            Trait abrasiveness = new Trait("trait_3", "Abrasiveness", effectsAbrasiveness);
            Globals_Game.traitMasterList.Add(abrasiveness.id, abrasiveness);

            Dictionary<Globals_Game.Stats, double> effectsAccountancy = new Dictionary<Globals_Game.Stats, double>();
            effectsAccountancy.Add(Globals_Game.Stats.TIME, 0.1);
            effectsAccountancy.Add(Globals_Game.Stats.FIEFEXPENSE, -0.2);
            effectsAccountancy.Add(Globals_Game.Stats.FAMEXPENSE, -0.2);
            effectsAccountancy.Add(Globals_Game.Stats.FIEFLOY, -0.05);
            Trait accountancy = new Trait("trait_4", "Accountancy", effectsAccountancy);
            Globals_Game.traitMasterList.Add(accountancy.id, accountancy);

            Dictionary<Globals_Game.Stats, double> effectsStupidity = new Dictionary<Globals_Game.Stats, double>();
            effectsStupidity.Add(Globals_Game.Stats.BATTLE, -0.4);
            effectsStupidity.Add(Globals_Game.Stats.DEATH, 0.05);
            effectsStupidity.Add(Globals_Game.Stats.FIEFEXPENSE, 0.2);
            effectsStupidity.Add(Globals_Game.Stats.FAMEXPENSE, 0.2);
            effectsStupidity.Add(Globals_Game.Stats.FIEFLOY, -0.1);
            effectsStupidity.Add(Globals_Game.Stats.NPCHIRE, -0.1);
            effectsStupidity.Add(Globals_Game.Stats.TIME, -0.1);
            effectsStupidity.Add(Globals_Game.Stats.SIEGE, -0.4);
            effectsStupidity.Add(Globals_Game.Stats.STEALTH, -0.6);
            Trait stupidity = new Trait("trait_5", "Stupidity", effectsStupidity);
            Globals_Game.traitMasterList.Add(stupidity.id, stupidity);

            Dictionary<Globals_Game.Stats, double> effectsRobust = new Dictionary<Globals_Game.Stats, double>();
            effectsRobust.Add(Globals_Game.Stats.VIRILITY, 0.2);
            effectsRobust.Add(Globals_Game.Stats.NPCHIRE, 0.05);
            effectsRobust.Add(Globals_Game.Stats.FIEFLOY, 0.05);
            effectsRobust.Add(Globals_Game.Stats.DEATH, -0.2);
            Trait robust = new Trait("trait_6", "Robust", effectsRobust);
            Globals_Game.traitMasterList.Add(robust.id, robust);

            Dictionary<Globals_Game.Stats, double> effectsPious = new Dictionary<Globals_Game.Stats, double>();
            effectsPious.Add(Globals_Game.Stats.VIRILITY, -0.2);
            effectsPious.Add(Globals_Game.Stats.NPCHIRE, 0.1);
            effectsPious.Add(Globals_Game.Stats.FIEFLOY, 0.1);
            effectsPious.Add(Globals_Game.Stats.TIME, -0.1);
            Trait pious = new Trait("trait_7", "Pious", effectsPious);
            Globals_Game.traitMasterList.Add(pious.id, pious);

            Dictionary<Globals_Game.Stats, double> effectsParanoia = new Dictionary<Globals_Game.Stats, double>();
            effectsParanoia.Add(Globals_Game.Stats.VIRILITY, - 0.3);
            effectsParanoia.Add(Globals_Game.Stats.PERCEPTION, 0.4);
            effectsParanoia.Add(Globals_Game.Stats.FIEFLOY, -0.05);
            Trait paranoia = new Trait("trait_8", "Paranoia", effectsParanoia);
            Globals_Game.traitMasterList.Add(paranoia.id,paranoia);
            
            Dictionary<Globals_Game.Stats, double> effectsCunning = new Dictionary<Globals_Game.Stats, double>();
            effectsCunning.Add(Globals_Game.Stats.PERCEPTION, 0.1);
            effectsCunning.Add(Globals_Game.Stats.STEALTH, 0.3);
            Trait cunning = new Trait("trait_9", "Cunning", effectsCunning);
            Globals_Game.traitMasterList.Add(cunning.id, cunning);

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
            Dictionary<string, ProtoDetachment> transfers001 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers002 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers003 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers004 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers005 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers006 = new Dictionary<string, ProtoDetachment>();
            Dictionary<string, ProtoDetachment> transfers007 = new Dictionary<string, ProtoDetachment>();

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
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Game.pcMasterList.ToList())
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
            List<Army> disbandedArmies = new List<Army>();
            bool hasDissolved = false;

            // iterate through armies
            foreach (KeyValuePair<string, Army> armyEntry in Globals_Game.armyMasterList)
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
                for (int i = 0; i < disbandedArmies.Count; i++)
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
                        c.activeChar = pc;
                        c.myPlayerCharacter = pc;
                        c.fiefToView = pc.location;
                    }
                }
            }
        }

        public static ProtoMessage ActionController(ProtoMessage msgIn, PlayerCharacter pc)
        {
            switch (msgIn.ActionType)
            {
                // Switch to using another character (performing actions with NPC
                case Actions.UseChar:
                    {
                        Character c;
                        if (string.IsNullOrEmpty(msgIn.Message))
                        {
                             c = pc;
                        }
                        else
                        {
                            c = Globals_Game.getCharFromID(msgIn.Message);
                            if (c == null)
                            {
                                ProtoMessage msg = new ProtoMessage();
                                msg.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                                return msg;
                            }
							if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, c))
                            {
                                ProtoMessage unauthorised = new ProtoMessage();
                                unauthorised.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return unauthorised;
                            }
                            // Cannot use a character that is dead
						if (!c.isAlive)
                            {
                                ProtoMessage dead = new ProtoMessage();
							// TODO add "This person is dead" message
							dead.ResponseType=DisplayMessages.ErrorGenericUnauthorised;
                                return dead;
                            }
                        }
                        Client client = null;
                        Globals_Server.clients.TryGetValue(pc.playerID, out client);
                        if (client != null)
                        {
                            client.activeChar = c;
                            ProtoCharacter success = new ProtoCharacter(c);
                            success.includeAll(c);
                            success.ActionType = Actions.UseChar;
                            success.ResponseType = DisplayMessages.Success;
                            return success;
                        }
                        else
                        {
                            Console.Error.WriteLine("error: client unidentified for player: " + pc.playerID);
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType=DisplayMessages.Error;
                            return error;
                        }
                    }
                    break;
                case Actions.GetPlayers:
                    {
                        ProtoGenericArray<ProtoPlayer> players = new ProtoGenericArray<ProtoPlayer>();
                        List<ProtoPlayer> playerList = new List<ProtoPlayer>();
                        foreach (Client c in Globals_Server.clients.Values)
                        {
                            if (!c.myPlayerCharacter.isAlive) continue;
                            ProtoPlayer player = new ProtoPlayer();
                            player.playerID = c.user;
                            player.pcID = c.myPlayerCharacter.charID;
                            player.pcName = c.myPlayerCharacter.firstName + " " + c.myPlayerCharacter.familyName;
                            player.natID = c.myPlayerCharacter.nationality.natID;
                            playerList.Add(player);
                        }
                        players.fields = playerList.ToArray();
                        return players;
                    }
                    break;
                // View a specific character
                case Actions.ViewChar:
                    {
                        Character c = Globals_Game.getCharFromID(msgIn.Message);
                        // Ensure character exists
                        if(c==null)
                        {
                            ProtoMessage msg = new ProtoMessage();
                            msg.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return msg;
                        }
                        // Check whether player owns character, include additional information if so
                        bool viewAll = PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, c);
                        bool seeLocation = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin,pc,c.location);
                        if (c is NonPlayerCharacter)
                        {
                            NonPlayerCharacter npc = c as NonPlayerCharacter;
                            ProtoNPC characterDetails = new ProtoNPC(npc);
                            characterDetails.ResponseType = DisplayMessages.Success;
                            // If unemployed include hire details
                            if (string.IsNullOrWhiteSpace(npc.familyID) && string.IsNullOrWhiteSpace(npc.employer))
                            {
                                characterDetails.IncludeHire(npc, pc.charID);
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
                            if (!string.IsNullOrWhiteSpace(c.captorID)&&!c.captorID.Equals(pc.charID))
                            {
                                characterDetails.location = null;   
                            }
                            return characterDetails;
                        }
                        else
                        {
                            ProtoPlayerCharacter characterDetails = new ProtoPlayerCharacter(c as PlayerCharacter);
                            characterDetails.ResponseType = DisplayMessages.Success;
                            if (viewAll)
                            {
                                characterDetails.includeAll(c as PlayerCharacter);
                            }
                            if (!string.IsNullOrWhiteSpace(c.captorID) && !c.captorID.Equals(pc.charID))
                            {
                                characterDetails.location = null;
                            }
                            return characterDetails;
                        }
                    }
                // Hire an NPC
                case Actions.HireNPC:
                    {
                        NonPlayerCharacter toHire = Globals_Game.getCharFromID(msgIn.Message) as NonPlayerCharacter;
                        // Validate character to hire
                        if (toHire == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Ensure player is near character
                        if (!PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, pc, toHire.location))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericTooFarFromFief;
                            return error;
                        }
                        if (!string.IsNullOrWhiteSpace(pc.captorID))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                            return error;
                        }
                        if(!toHire.CheckCanHire(pc)) {
                            ProtoMessage result = new ProtoMessage();
                            result.ResponseType = DisplayMessages.CharacterHireNotEmployable;
                            return result;
                        }
                        uint bid = 0;
                        // Ensure bid included and convert bid to uint
                        if (msgIn.MessageFields == null || msgIn.MessageFields.Length != 1 || !uint.TryParse(msgIn.MessageFields[0], out bid))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericPositiveInteger;
                            return error;
                        }
                        else
                        {
                            // Send result and updated character detail
                            // Result contains character details, the result of hiring (response type) and any necessary fields for filling in response
                            ProtoMessage result = null;
                            pc.ProcessEmployOffer(toHire, bid, out result);
                            ProtoNPC viewCharacter = new ProtoNPC(toHire);
                            viewCharacter.ResponseType = result.ResponseType;
                            viewCharacter.Message = result.Message;
                            viewCharacter.MessageFields = result.MessageFields;
                            return viewCharacter;
                        }
                    }
                    break;
                // Fire an NPC
                case Actions.FireNPC:
                    {
                        Character character = Globals_Game.getCharFromID(msgIn.Message);
                        // If character to hire unidentified, error
                        if (character == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        NonPlayerCharacter npc = character as NonPlayerCharacter;
                        // if is not npc, or is not employed by player, error
                        if (npc == null||npc.GetEmployer() != pc)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterFireNotEmployee;
                            return error;
                        }
                        pc.FireNPC(npc);
                        ProtoMessage result = new ProtoMessage();
                        // Include char id to let client know which npc has been fired
                        result.Message = npc.charID;
                        result.ResponseType = DisplayMessages.Success;
                        return result;
                    }
                    break;
                // View an army
                case Actions.ViewArmy:
                    {
                        Army a = null;
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out a);
                        if (a == null)
                        {
                            ProtoMessage msg = new ProtoMessage();
                            msg.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return msg;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.canSeeArmyOrAdmin, pc, a))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        bool viewAll = PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, a);
                        ProtoArmy armyDetails = new ProtoArmy(a, pc);
                        armyDetails.ResponseType = DisplayMessages.Success;
                        if (viewAll)
                        {
                            armyDetails.includeAll(a);
                        }
                        return armyDetails;
                    }
                case Actions.DisbandArmy:
                    {
                        Army army = null;
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        army.DisbandArmy();
                        ProtoMessage result = new ProtoMessage();
                        result.ResponseType = DisplayMessages.Success;
                        return result;
                    }
                    break;
                // Get a list of NPCs- if listing chars in meeting place use "ListCharsInMeetingPlace"
                case Actions.GetNPCList:
                    {
                        List<ProtoCharacterOverview> listOfChars = new List<ProtoCharacterOverview>();
                       // List<string> listOfChars = new List<string>();
                        if (msgIn.Message.Equals("Entourage"))
                        {
                            foreach (Character entourageChar in pc.myEntourage)
                            {
                              //  listOfChars.Add(entourageChar.firstName);
                                listOfChars.Add(new ProtoCharacterOverview(entourageChar));
                            }
                        }
                        // List of characters that can be granted a title
                        else if (msgIn.Message.Equals("Grant"))
                        {
                            Army army = null;
                            if (msgIn.MessageFields == null)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                return error;
                            }
                            else
                            {
                                string role = msgIn.MessageFields[0];
                                // ensure leaders have army
                                if(role.Equals("leader")) {
                                    string armyID = null;
                                    if (msgIn.MessageFields.Length == 2)
                                    {
                                        armyID = msgIn.MessageFields[1];
                                        if (string.IsNullOrEmpty(armyID))
                                        {
                                            ProtoMessage error = new ProtoMessage();
                                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                            return error;
                                        }
                                        // Check player owns army
                                        army = null;
                                        Globals_Game.armyMasterList.TryGetValue(armyID, out army);
                                        if (army == null)
                                        {
                                            ProtoMessage error = new ProtoMessage();
                                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                                            return error;
                                        }
                                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                                        {
                                            ProtoMessage error = new ProtoMessage();
                                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                            return error;
                                        }
                                    }
                                    else
                                    {
                                        ProtoMessage error = new ProtoMessage();
                                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                        return error;
                                    }
                                }

                                foreach (NonPlayerCharacter npc in pc.myNPCs)
                                {
                                    ProtoMessage ignore = null;
                                    if (army == null)
                                    {
                                        if (npc.ChecksBeforeGranting(pc, role, true, out ignore))
                                        {

                                            listOfChars.Add(new ProtoCharacterOverview(npc));
                                        }
                                    }
                                    else
                                    {
                                        if (npc.ChecksBeforeGranting(pc, role, true, out ignore, army.armyID))
                                        {
                                        
                                            listOfChars.Add(new ProtoCharacterOverview(npc));
                                        }
                                    }
                                    
                                }
                            }
                        }
                        else {
                            if (msgIn.Message.Contains("Family"))
                            {
                                listOfChars.Add(new ProtoCharacterOverview(pc));
                                foreach (NonPlayerCharacter family in pc.myNPCs)
                                {
                                    // ensure character is employee
                                    if (!string.IsNullOrWhiteSpace(family.familyID))
                                    {
                                        //   listOfChars.Add(family.firstName);
                                        listOfChars.Add(new ProtoCharacterOverview(family));
                                    }
                                }
                            }
                            if (msgIn.Message.Contains("Employ"))
                            {
                                foreach (NonPlayerCharacter employee in pc.myNPCs)
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
                        
                        ProtoGenericArray<ProtoCharacterOverview> result = new ProtoGenericArray<ProtoCharacterOverview>();
                        result.fields = listOfChars.ToArray();
                        result.ResponseType = DisplayMessages.Success;
                        result.Message = msgIn.Message;
                        result.MessageFields = msgIn.MessageFields;
                        return result;
                    }
                // Can travel to fief if are/own character, and have valid fief. Days etc taken into account elsewhere
                case Actions.TravelTo:
                    // Attempt to convert message to ProtoTravelTo
                    ProtoTravelTo travelTo = msgIn as ProtoTravelTo;
                    if (travelTo != null)
                    {
                        // Identify character to move
                        Character charToMove = null;
                        if (travelTo.characterID != null)
                        {
                            charToMove = Globals_Game.getCharFromID(travelTo.characterID);
                        }
                        // If not character id specified PC is moving self
                        else {
                            charToMove=pc;
                        }
                        if (charToMove == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        else
                        {
                            // Check permissions
                            if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, charToMove))
                            {
                                // Identify fief
                                Fief fief = null;
                                if (travelTo.travelTo != null)
                                {
                                    Globals_Game.fiefMasterList.TryGetValue(travelTo.travelTo, out fief);
                                    if (fief != null)
                                    {
                                        double travelCost = charToMove.location.getTravelCost(fief, charToMove.armyID);
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
                                            ProtoFief reply = new ProtoFief(fief);
                                            reply.ResponseType = DisplayMessages.Success ;
                                            return reply;
                                        }
                                        else
                                        {
                                            //Return error message obtained from MoveCharacter
                                            return error;
                                        }
                                    }
                                    else
                                    {
                                        ProtoMessage error = new ProtoMessage();
                                        error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                                        return error;
                                    }
                                }
                                // If specifying a route, attempt to move character and return final location
                                else if (travelTo.travelVia != null)
                                {
                                    ProtoMessage error;
                                    charToMove.TakeThisRoute(travelTo.travelVia,out error);
                                    if (error != null)
                                    {
                                        Globals_Game.UpdatePlayer(pc.playerID, error);
                                    }
                                    ProtoFief reply = new ProtoFief(charToMove.location);
                                    reply.ResponseType = DisplayMessages.Success;
                                    return reply;
                                }
                                // If fief unidentified return error
                                else
                                {
                                    ProtoMessage msg = new ProtoMessage();
                                    msg.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                                    return msg;
                                }
                            }
                            // If unauthorised return error
                            else
                            {
                                Trace.WriteLine("returning unauthorised");
                                ProtoMessage unauthorised = new ProtoMessage();
                                unauthorised.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return unauthorised;
                            }
                        }
                    }
                    // Message was not valid
                    else
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                        error.Message = "InvalidTravelMessage";
                        return error;
                    }

                // View a fief. Can view a fief if in it, own it, or have a character in it
                case Actions.ViewFief:
                    {
                        Fief f = null;
                        if (msgIn.Message==null||msgIn.Message.Equals("home"))
                        {
                            f = pc.GetHomeFief();
                        }
                        else
                        {
                            Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out f);
                        }
                        if (f == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        else
                        {
                            ProtoFief fiefToView = new ProtoFief(f);
                            if (pc.ownedFiefs.Contains(f))
                            {
                                fiefToView.includeAll(f);
                                return fiefToView;
                            }
                            bool hasCharInFief = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, pc, f);
                            if (hasCharInFief)
                            {
                                return fiefToView;
                            }
                            else
                            {
                                ProtoMessage m = new ProtoMessage();
                                m.ResponseType = DisplayMessages.ErrorGenericTooFarFromFief;
                                return m;
                            }
                        }
                    }
                case Actions.ViewMyFiefs:
                    {
                        ProtoGenericArray<ProtoFief> fiefList = new ProtoGenericArray<ProtoFief>();
                        fiefList.fields=new ProtoFief[pc.ownedFiefs.Count];
                        int i = 0;
                        foreach(Fief f in pc.ownedFiefs) {
                            ProtoFief fief = new ProtoFief(f);
                            fief.includeAll(f);
                            fiefList.fields[i]=fief;
                            i++;
                        }
                        return fiefList;
                    }
                    break;
                
                // Appoint a character as a bailiff to a fief
                case Actions.AppointBailiff:
                    {
                        // Fief
                        Fief f = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out f);
                        // Character to become bailiff
                        Character c = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                        if (c != null&&f!=null)
                        {
                            // Ensure character owns fief and character, or is admin
                            if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, c) && PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, f))
                            {
                                // Check character can become bailiff
                                ProtoMessage error = null;
                                if (c.ChecksBeforeGranting(pc, "bailiff", false,out error))
                                {
                                    // set bailiff, return fief
                                    f.bailiff = c;
                                    ProtoFief fiefToView = new ProtoFief(f);
                                    fiefToView.ResponseType = DisplayMessages.Success;
                                    fiefToView.includeAll(f);
                                    return fiefToView;
                                }
                                else
                                {
                                    // error message returned from ChecksBeforeGranting
                                    return error;
                                }
                            }
                            // User unauthorised
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                        }
                        // If character or fief not recognised
                        else
                        {
                            if (c == null)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                                return error;
                            }
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                                return error;
                            }
                        }
                    }
                // Remove bailiff from fief
                case Actions.RemoveBailiff:
                    {
                        // Fief
                        Fief f = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out f);
                        if(f!=null) {
                            // Ensure player is authorized
                            bool hasPermission = PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, f);
                            if (hasPermission)
                            {
                                // Remove bailiff and return fief details
                                if (f.bailiff != null)
                                {
                                    f.bailiff = null;
                                    f.bailiffDaysInFief = 0;
                                    ProtoFief fiefToView = new ProtoFief(f);
                                    fiefToView.ResponseType = DisplayMessages.Success;
                                    fiefToView.includeAll(f);
                                    return fiefToView;
                                }
                                // Error- no bailiff
                                else
                                {
                                    ProtoMessage error = new ProtoMessage();
                                    error.ResponseType = DisplayMessages.FiefNoBailiff;
                                    return error;
                                }
                            }
                            // Unauthorised
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                        }
                        // Could not identify fief
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Bar a number of characters from a fief
                case Actions.BarCharacters:
                    {
                        // check fief is valid
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief != null)
                        {
                            // Check player owns fief
                            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                            // List of characters that for whatever reason could not be barred
                            List<string> couldNotBar = new List<string>();
                            // Bar characters
                            foreach (string charID in msgIn.MessageFields)
                            {
                                Character charToBar = Globals_Game.getCharFromID(charID);

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
                            ProtoFief fiefToReturn = new ProtoFief(fief);

                            if (couldNotBar.Count > 0)
                            {
                                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                                fiefToReturn.MessageFields = couldNotBar.ToArray();
                            }
                            fiefToReturn.includeAll(fief);
                            return fiefToReturn;
                        }
                        // if could not identify fief
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Unbar a number of characters from fief
                case Actions.UnbarCharacters:
                    {
                        // check fief is valid
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief != null)
                        {
                            // Check player owns fief
                            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                            // List of characters that for whatever reason could not be barred
                            List<string> couldNotUnbar = new List<string>();
                            // Bar characters
                            foreach (string charID in msgIn.MessageFields)
                            {
                                Character charToUnbar = Globals_Game.getCharFromID(charID);
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
                            ProtoFief returnFiefState = new ProtoFief(fief);
                            if (couldNotUnbar.Count > 0)
                            {
                                returnFiefState.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                                returnFiefState.MessageFields = couldNotUnbar.ToArray();
                            }
                            returnFiefState.includeAll(fief);
                            return returnFiefState;
                        }
                        // error if fief unidentified
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Bar a number of nationalities from fief
                case Actions.BarNationalities:
                    {
                        // check fief is valid
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief != null)
                        {
                            // Check player owns fief
                            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                            // Attempt to bar nationalities
                            List<string> failedToBar = new List<string>();
                            foreach (string natID in msgIn.MessageFields)
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
                            ProtoFief fiefToReturn = new ProtoFief(fief);
                            if (failedToBar.Count > 0)
                            {
                                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                                fiefToReturn.MessageFields = failedToBar.ToArray();
                            }
                            fiefToReturn.includeAll(fief);
                            return fiefToReturn;
                        }
                        // Fief is invalid
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Unbar a number of nationalities from fief
                case Actions.UnbarNationalities:
                    {
                        // check fief is valid
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief != null)
                        {
                            // Check player owns fief
                            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                            // Attempt to bar nationalities
                            List<string> failedToUnbar = new List<string>();
                            foreach (string natID in msgIn.MessageFields)
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
                            ProtoFief fiefToReturn = new ProtoFief(fief);
                            if (failedToUnbar.Count > 0)
                            {
                                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                                fiefToReturn.MessageFields = failedToUnbar.ToArray();
                            }
                            fiefToReturn.includeAll(fief);
                            return fiefToReturn;
                        }
                        // Fief is invalid
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Grant a fief title to a character
                case Actions.GrantFiefTitle:
                    {
                        // Get fief
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief != null)
                        {
                            // Ensure player has permission to grant title
                            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                return error;
                            }
                            // Get Character
                            Character charToGrant = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                            if (charToGrant != null)
                            {
                                ProtoMessage error;
                                bool canGrant = charToGrant.ChecksBeforeGranting(pc, "title", false,out error);
                                if (canGrant)
                                {
                                    bool granted = pc.GrantTitle(charToGrant, fief,out error);
                                    if (granted)
                                    {
                                        ProtoFief f = new ProtoFief(fief);
                                        f.Message = "grantedTitle";
                                        f.includeAll(fief);
                                        return f;
                                    }
                                    // If granting fails message is returned from GrantTitle
                                    else
                                    {
                                        return error;
                                    }
                                }
                                //Permission denied
                                else
                                {
                                    return error;
                                }
                            }
                            // Character not valid
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                                return error;
                            }
                        }
                        // Fief not valid
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                    }
                // Adjust fief expenditure
                case Actions.AdjustExpenditure:
                    {
                        
                        // Get fief
                        Fief fief = null;
                        if (msgIn.Message == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                        if (fief == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        // Check permissions
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Try to convert message to ProtoGenericArray
                        ProtoGenericArray<double> newRates = msgIn as ProtoGenericArray<double>;
                        if (newRates == null)
                        {
                            Console.WriteLine("Null rates-auto adjust");
                            int overspend = fief.GetAvailableTreasury(true);
                            if(overspend<0) {
                                fief.AutoAdjustExpenditure(Convert.ToUInt32(Math.Abs(overspend)));
                            }
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                return error;
                            }
                        }
                        else
                        {
                            // Check array is right length
                            Double[] adjustedValues = newRates.fields;
                            if (adjustedValues.Length != 5)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                error.Message = "Expected array:5";
                                return error;
                            }

                            ProtoMessage adjust = fief.AdjustExpenditures(adjustedValues[0], Convert.ToUInt32(adjustedValues[1]), Convert.ToUInt32(adjustedValues[2]), Convert.ToUInt32(adjustedValues[3]), Convert.ToUInt32(adjustedValues[4]));
                            return adjust;
                   
                        }

                        // Return fief after adjusting expenditures
                        ProtoFief fiefToView = new ProtoFief(fief);

                        fiefToView.ResponseType = DisplayMessages.FiefExpenditureAdjusted;
                        fiefToView.includeAll(fief);
                        return fiefToView;
                    }
                    break;
                // Transfer funds between fiefs
                case Actions.TransferFunds:
                    {
                        // Cast message to ProtoTranfer
                        ProtoTransfer transferDetails = msgIn as ProtoTransfer;
                        if (msgIn == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        // Check both fiefs are valid
                        Fief fiefFrom = null;
                        Fief fiefTo = null;
                        if (string.IsNullOrWhiteSpace(transferDetails.fiefFrom))
                        {
                            fiefFrom = pc.GetHomeFief();
                        }
                        else if (string.IsNullOrWhiteSpace(transferDetails.fiefTo))
                        {
                            Globals_Game.fiefMasterList.TryGetValue(transferDetails.fiefFrom, out fiefFrom);
                            fiefTo = pc.GetHomeFief();
                        }
                        else
                        {
                            Globals_Game.fiefMasterList.TryGetValue(transferDetails.fiefTo, out fiefTo);
                        }
                        if (fiefFrom == null || fiefTo == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        // Check amount is valid
                        int amount = transferDetails.amount;
                        if (amount < 0)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericPositiveInteger;
                            return error;
                        }
                        // Ensure fief has sufficient funds
                        if (amount > fiefFrom.GetAvailableTreasury(true))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericInsufficientFunds;
                            return error;
                        }
                        else
                        {
                            // return success
                            ProtoMessage error;
                            if (fiefFrom.TreasuryTransfer(fiefTo, amount, out error))
                            {
                                ProtoFief success = new ProtoFief(fiefFrom);
                                success.includeAll(fiefFrom);
                                success.Message = "transferfunds";
                                success.ResponseType = DisplayMessages.Success;
                                return success;
                            }
                            // an error message will be sent from within TreasuryTransfer
                            else
                            {
                                return error;
                            }

                        }
                    }
                // Transfer funds from pc's home fief to another player's home fief
                case Actions.TransferFundsToPlayer:
                    {
                        // Try to convert to ProtoTransferPlayer
                        ProtoTransferPlayer transferDetails = msgIn as ProtoTransferPlayer;
                        // If failed return error
                        if (transferDetails == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        // Get player to transfer to
                        PlayerCharacter transferTo = null;
                        Globals_Game.pcMasterList.TryGetValue(transferDetails.playerTo, out transferTo);
                        if (transferTo == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Confirm both players have a home fief
                        if (pc.GetHomeFief() == null || transferTo.GetHomeFief() == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericNoHomeFief;
                            return error;
                        }
                        // Perform treasury transfer, update
                        ProtoMessage TransferError;
                        bool success = pc.GetHomeFief().TreasuryTransfer(transferTo.GetHomeFief(),transferDetails.amount,out TransferError);
                        if(success) {
                            Globals_Game.UpdatePlayer(transferTo.playerID, DisplayMessages.GenericReceivedFunds, new string[] { transferDetails.amount.ToString(), pc.firstName + " " + pc.familyName });
                            ProtoMessage result = new ProtoMessage();
                            result.ResponseType = DisplayMessages.Success;
                            result.Message = "transferfundsplayer";
                            return result;
                        }
                        else
                        {
                            return TransferError;
                        }
                    }
                // Instruct a character to enter or exit keep
                case Actions.EnterExitKeep:
                    {
                        Character c=null;
                        // get character, check is valid
                        if (msgIn.Message == null)
                        {
                            c = pc;
                        }
                        else
                        {
                            c = Globals_Game.getCharFromID(msgIn.Message);
                        }
                        
                        if (c == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // check authorization
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, c))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage EnterError;
                        bool success = c.ExitEnterKeep(out EnterError);
                        if (success)
                        {
                            ProtoMessage m = new ProtoMessage();
                            m.Message = c.inKeep.ToString();
                            m.ResponseType = DisplayMessages.Success;
                            return m;
                        }
                        else
                        {
                            // Error messages handled within enter/exit keep
                            return EnterError;
                        }
                    }
                // List chars in court, tavern or outside fief
                case Actions.ListCharsInMeetingPlace:
                    {
                        // Character to use
                        Character character;
                        // fief is current player fief- May change later to allow a player's characters to scout out NPCs in fiefs
                        if (msgIn.MessageFields != null && msgIn.MessageFields.Length == 1)
                        {
                            character = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                            if (character == null)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                                return error;
                            }
                            
                        }
                        else
                        {
                            character = pc;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, character))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        Fief fief = character.location;
                        if (msgIn.Message == null)
                        {
                            ProtoMessage m = new ProtoMessage();
                            m.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return m;
                        }
                        // Enter/exit keep as appropriate depending on whether viewing court
                        if (msgIn.Message.Equals("court"))
                        {
                            if (!character.inKeep) {
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
                        ProtoGenericArray<ProtoCharacterOverview> charsInPlace = new ProtoGenericArray<ProtoCharacterOverview>(fief.ListCharsInMeetingPlace(msgIn.Message, pc));
                        charsInPlace.Message = msgIn.Message;
                        charsInPlace.ResponseType = DisplayMessages.Success;
                        return charsInPlace;
                        
                    }
                // Instruct a character to camp where they are for a number of days
                case Actions.Camp:
                    {
                        Character c;
                        // Validate character
                        if (msgIn.Message != null)
                        {
                            c = Globals_Game.getCharFromID(msgIn.Message);
                        }
                        else
                        {
                            c = pc;
                        }
                        
                        if(c==null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Perform authorization
                        if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, c))
                        {
                            // Attempt to camp
                            try
                            {
                                ProtoMessage campMessage;
                                c.CampWaitHere(Convert.ToByte(msgIn.MessageFields[0]),out campMessage);
 
                                return campMessage;
                            }
                            catch (Exception e)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                error.Message = e.ToString();
                                return error;
                            }
                            
                        }
                        // if unauthorised, error
                        else
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                    }
                // add or remove a character to the entourage
                case Actions.AddRemoveEntourage:
                    {
                        // validate character
                        Character c = Globals_Game.getCharFromID(msgIn.Message);
                        NonPlayerCharacter myNPC = (c as NonPlayerCharacter);
                        if (myNPC == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // ensure player is authorized to add to entourage
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, myNPC))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Ensure playercharacter is not captured
                        if (!string.IsNullOrWhiteSpace(pc.captorID))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                            return error;
                        }
                        // If in entourage, remove- otherwise, add
                        if ((c as NonPlayerCharacter).inEntourage == true)
                        {
                            pc.RemoveFromEntourage(myNPC);
                        }
                        else
                        {
                            // Player character must be in same location as npc to add
                            if (pc.location != myNPC.location)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                                return error;
                            }
                            pc.AddToEntourage(myNPC);
                        }
                        // return entourage
                        List<ProtoCharacterOverview> newEntourage = new List<ProtoCharacterOverview>();
                        foreach (Character entourageChar in pc.myEntourage)
                        {
                            newEntourage.Add(new ProtoCharacterOverview(entourageChar));
                        }
                        ProtoGenericArray<ProtoCharacterOverview> result = new ProtoGenericArray<ProtoCharacterOverview>(newEntourage.ToArray());
                        result.ResponseType = DisplayMessages.Success;
                        return result;
                    }
                // Propose marriage between two characters
                case Actions.ProposeMarriage:
                    Character charFromProposer = Globals_Game.getCharFromID(msgIn.Message);
                    if (charFromProposer == null)
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                        return error;
                    }
                    if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, pc, charFromProposer))
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericUnauthorised ;
                        return error;
                    }
                    Character charProposeTo = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                    if (charProposeTo == null)
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                        return error;
                    }
                    bool madeProposal = false;
                    if (!string.IsNullOrWhiteSpace(charFromProposer.captorID )|| !string.IsNullOrWhiteSpace(charProposeTo.captorID))
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                        return error;
                    }
                    ProtoMessage proposalError;
                    if (charFromProposer.ChecksBeforeProposal(charProposeTo,out proposalError))
                    {
                        madeProposal = charFromProposer.ProposeMarriage(charProposeTo);
                    }
                    if (!madeProposal)
                    {
                        return proposalError;
                    }
                    else
                    {
                        ProtoMessage success = new ProtoMessage();
                        success.ResponseType = DisplayMessages.Success;
                        success.Message = "Proposal success";
                        return success;
                    }
                // Reply to a proposal (accept or reject)
                case Actions.AcceptRejectProposal:
                    {
                        // Attempt to get Journal entry
                        JournalEntry jEntry = null;
                        Client client = Globals_Server.clients[pc.playerID];
                        // Player must be logged in (but if not logged on, nobody to send result to
                        if (client == null)
                        {
                            return null;
                        }
                        client.myPastEvents.entries.TryGetValue(Convert.ToUInt32(msgIn.Message),out jEntry);
                        if (jEntry == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.JournalEntryUnrecognised;
                            return error;
                        }
                        if (!jEntry.CheckForProposal(pc))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.JournalEntryNotProposal;
                            return error;
                        }
                        try
                        {
                            bool success = jEntry.ReplyToProposal(Convert.ToBoolean(msgIn.MessageFields[0]));
                            if (success)
                            {
                                ProtoMessage proposeresult = new ProtoMessage();
                                proposeresult.ResponseType = DisplayMessages.Success;
                                return proposeresult;
                            }
                            else
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.Error;
                                return error;
                            }
                        }
                        catch (Exception e)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                       
                    }
                // Appoint a new heir
                case Actions.AppointHeir:
                    {
                        // Cannot appioint an heir if captured
                        if (!string.IsNullOrWhiteSpace(pc.captorID))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                            return error;
                        }
                        // validate character
                        Character heirTemp = Globals_Game.getCharFromID(msgIn.Message);
                        if (heirTemp == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                    
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, pc, heirTemp))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        NonPlayerCharacter heir = (heirTemp as NonPlayerCharacter);
                        ProtoMessage heirError;
                        if (heir==null || !heir.ChecksForHeir(pc,out heirError))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.CharacterHeir;
                            return error;
                        }
                        else {
                            // check for an existing heir and remove
                            foreach (NonPlayerCharacter npc in pc.myNPCs)
                            {
                                if (npc.isHeir)
                                {
                                    npc.isHeir = false;
                                }
                            }
                            heir.isHeir = true;
                            ProtoCharacter result = new ProtoCharacter(heir);
                            result.ResponseType = DisplayMessages.Success;
                            result.includeAll(heir);
                            return result;
                        }
                    }
                // Attempt to have a child
                case Actions.TryForChild:
                    {
                        // Get father
                        Character father = Globals_Game.getCharFromID(msgIn.Message);
                        if (father == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Authorize
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, father))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Confirm can get pregnant
                        ProtoMessage birthError;
                        if (!Birth.ChecksBeforePregnancyAttempt(father,out birthError))
                        {
                            return birthError;
                        }
                        // Move so that both husband and wife are in/out of keep
                        father.GetSpouse().inKeep = father.inKeep;
                        ProtoMessage birthMessage;
                        bool pregnant = father.GetSpousePregnant(father.GetSpouse(),out birthMessage);
                        // At this point the rest is handled by the GetSpousePregnant method
                        return birthMessage;
                    }
                // Recruit troops from fief
                case Actions.RecruitTroops:
                    {
                        // convert incoming message to ProtoRecruit
                        ProtoRecruit recruitmentDetails = (msgIn as ProtoRecruit);
                        if (recruitmentDetails == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        // Get army to recruit into
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(recruitmentDetails.armyID, out army);
                        if (army == null || !pc.myArmies.Contains(army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        // recruit troops
                        uint numTroops = recruitmentDetails.amount ;
                        return pc.RecruitTroops(numTroops, army,recruitmentDetails.isConfirm);
                    }
                // Maintain an army
                case Actions.MaintainArmy:
                    {
                        // get army
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage result;
                        
                        army.MaintainArmy(out result);
                        return result ;
                    }
                // Appoint a new army leader
                case Actions.AppointLeader:
                    {
                        // Get army
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        // Get leader
                        Character newLeader = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                        if (newLeader == null )
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Authorize
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army) || (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, newLeader)))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Check char can be leader, grant
                        ProtoMessage grantError;
                        
                        bool canBeLeader = newLeader.ChecksBeforeGranting(pc, "leader", false, out grantError, army.armyID);
                        if (canBeLeader)
                        {
                            army.AssignNewLeader(newLeader);
                            return new ProtoArmy(army,pc);
                        }
                        // Checks before granting will return own error messages
                        else
                        {
                            return grantError;
                        }
                    }
                // Drop off troops in fief 
                case Actions.DropOffTroops:
                    {
                        // Convert to ProtoDetachment
                        ProtoDetachment detachmentDetails = (msgIn as ProtoDetachment);
                        if (detachmentDetails == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        // Get army
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(detachmentDetails.armyID, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        // Authorize
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Create a detachment
                        ProtoMessage detachmentResult;
                        if (army.CreateDetachment(detachmentDetails.troops, detachmentDetails.leftFor,out detachmentResult))
                        {
                            ProtoArmy success = new ProtoArmy(army, pc);
                            success.includeAll(army);
                            success.ResponseType = DisplayMessages.Success;
                            return success;
                        }
                        else
                        {
                            return detachmentResult;
                        }
                    }
                case Actions.ListArmies:
                    {
                        ProtoGenericArray<ProtoArmyOverview> armies = new ProtoGenericArray<ProtoArmyOverview>();
                        armies.fields = new ProtoArmyOverview[pc.myArmies.Count];
                        int i = 0;
                        foreach (Army army in pc.myArmies)
                        {
                            ProtoArmyOverview armyDetails = new ProtoArmyOverview(army);
                            armyDetails.includeAll(army);
                            armies.fields[i] = armyDetails;
                            i++;
                        }
                        return armies;
                    }
                    break;
                // List detachments in fief
                case Actions.ListDetachments:
                    {
                        // Get army to pick up detachments
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        // check permissions
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // List available transfers
                        List<ProtoDetachment> myAvailableTransfers = new List<ProtoDetachment>();
                        foreach (ProtoDetachment transferDetails in army.GetLocation().troopTransfers.Values)
                        {
                            if(transferDetails.leftFor.Equals(pc.charID)) {
                               myAvailableTransfers.Add(transferDetails);
                            }
                        }
                        ProtoGenericArray<ProtoDetachment> detachmentList = new ProtoGenericArray<ProtoDetachment>(myAvailableTransfers.ToArray());
                        detachmentList.ResponseType = DisplayMessages.Success;
                        return detachmentList;
                    }
                // Pick up detachments 
                // TODO add ability to create new army by picking up troops
                case Actions.PickUpTroops:
                    {
                        // Army to pick up detachments
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null )
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage pickupMessage = army.ProcessPickups(msgIn.MessageFields);
                        if (pickupMessage != null)
                        {
                            if ((DisplayMessages)pickupMessage.ResponseType == DisplayMessages.ArmyPickupsNotEnoughDays)
                            {
                                ProtoArmy armyDetails = new ProtoArmy(army, pc);
                                armyDetails.ResponseType = pickupMessage.ResponseType;
                                return armyDetails;
                            }
                            else
                            {
                                return pickupMessage;
                            }
                        }
                        ProtoArmy updatedArmy = new ProtoArmy(army, pc);
                        updatedArmy.includeAll(army);
                        updatedArmy.ResponseType = DisplayMessages.Success;
                        return updatedArmy;
                    }
                // Pillage a fief
                case Actions.PillageFief:
                    {
                        // Get army
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null )
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage pillageError;
                        bool canPillage = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(),out pillageError);
                        if (canPillage)
                        {
                            ProtoMessage result = Pillage_Siege.PillageFief(army, army.GetLocation());
                            result.ResponseType = DisplayMessages.Success;
                            result.Message = "pillage";
                            return result;
                        }
                        // CheckBeforePillage returns own error messages
                        return pillageError;
                    }
                // Besiege a fief
                case Actions.BesiegeFief:
                    {
                        // Get army
                        if (string.IsNullOrEmpty(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out army);
                        if (army == null )
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, army))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage pillageError;
                        bool canSiege = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(),out pillageError,"siege");
                        if (canSiege)
                        {
                            Siege newSiege = Pillage_Siege.SiegeStart(army, army.GetLocation());
                            ProtoSiegeDisplay result = new ProtoSiegeDisplay(newSiege);
                            result.ResponseType = DisplayMessages.Success;
                            result.Message = "siege";
                            return result;
                        }
                        else
                        {
                            return pillageError;
                        }
                    }
                // Perform siege negotiation round
                case Actions.SiegeRoundNegotiate:
                    {
                        // Get siege
                        Siege siege = pc.GetSiege(msgIn.Message);
                        if (siege == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                            return error;
                        }
                        // Check besieger is pc
                        if (siege.besiegingPlayer != pc.charID)
                        {
                            ProtoMessage error = new ProtoMessage();
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
                // Perform a siege reduction round
                case Actions.SiegeRoundReduction:
                    {
                        // get siege
                        Siege siege = pc.GetSiege(msgIn.Message);
                        if (siege == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                            return error;
                        }
                        // check player is besieger
                        if (siege.besiegingPlayer != pc.charID)
                        {
                            ProtoMessage error = new ProtoMessage();
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
                // Perform siege storm round
                case Actions.SiegeRoundStorm:
                    {
                        // Get siege
                        Siege siege = pc.GetSiege(msgIn.Message);
                        if (siege == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                            return error;
                        }
                        // check player is besieger
                        if (siege.besiegingPlayer != pc.charID)
                        {
                            ProtoMessage error = new ProtoMessage();
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
                case Actions.EndSiege:
                    {
                        // Get siege
                        Siege siege = pc.GetSiege(msgIn.Message);
                        if (siege == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                            return error;
                        }
                        // check player is besieger
                        if (siege.besiegingPlayer != pc.charID)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.SiegeNotBesieger;
                            return error;
                        }
                        ProtoMessage siegeError = null;
                        if (!siege.ChecksBeforeSiegeOperation(out siegeError,"end"))
                        {
                            return siegeError;
                        }
                        siege.SiegeEnd(false);
                        ProtoMessage reply = new ProtoMessage();
                        reply.ResponseType = DisplayMessages.Success;
                        reply.Message = siege.siegeID;
                        return reply;
                    }
                    break;
                // List player's sieges
                case Actions.SiegeList:
                    {
                        List<ProtoSiegeOverview> sieges = new List<ProtoSiegeOverview>();
                        foreach(String siegeID in pc.mySieges) {
                            sieges.Add(new ProtoSiegeOverview(pc.GetSiege(siegeID)));
                        }
                        ProtoGenericArray<ProtoSiegeOverview> siegeList = new ProtoGenericArray<ProtoSiegeOverview>(sieges.ToArray());
                        return siegeList;
                    }
                case Actions.ViewSiege:
                    {
                        string siegeID = msgIn.Message;
                        if (!pc.mySieges.Contains(siegeID))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        else
                        {
                            ProtoSiegeDisplay siegeDetails = new ProtoSiegeDisplay(pc.GetSiege(siegeID));
                            siegeDetails.ResponseType = DisplayMessages.Success;
                            return siegeDetails;
                        }
                    }
                    break;
                // Adjust combat values
                case Actions.AdjustCombatValues:
                    {
                        // Convert incoming message to ProtoCombatValues
                        ProtoCombatValues newValues = (msgIn as ProtoCombatValues);
                        if (newValues == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        // Get army
                        Army army = null;
                        Globals_Game.armyMasterList.TryGetValue(newValues.armyID, out army);
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin,pc, army)) {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Attempt to adjust standing orders
                        bool success= army.AdjustStandingOrders(newValues.aggression, newValues.odds);
                        if (success)
                        {
                            ProtoCombatValues result = new ProtoCombatValues(army.aggression,army.combatOdds,army.armyID);
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                        else {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.Error;
                            error.Message = "adjust standing orders";
                            return error;
                        }
                    }
                // Examine the armies in a fief
                case Actions.ExamineArmiesInFief:
                    {
                        // get fief
                        Fief fief = null;
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.Message,out fief);
                        if(fief == null) {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        // Check character is in fief, owns fief, or is admin
                        if(!PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin,pc,fief))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // get list of armies
                        List<ProtoArmyOverview> armies = new List<ProtoArmyOverview>();
                        foreach(string armyID in fief.armies) {
                            Army army = null;
                            Globals_Game.armyMasterList.TryGetValue(armyID,out army);
                            if(army!=null) {
                                armies.Add(new ProtoArmyOverview(army));
                            }
                        }
                        // Return array of overviews
                        ProtoGenericArray<ProtoArmyOverview> armyList = new ProtoGenericArray<ProtoArmyOverview>(armies.ToArray());
                        armyList.ResponseType =DisplayMessages.Armies;
                        return armyList;
                    }
                // Attack an army
                case Actions.Attack:
                    {
                        // Get attacker and defender
                        Army armyAttacker = null;
                        Army armyDefender = null;
                        Globals_Game.armyMasterList.TryGetValue(msgIn.Message, out armyAttacker);
                        Globals_Game.armyMasterList.TryGetValue(msgIn.MessageFields[0], out armyDefender);
                        if (armyAttacker == null || armyDefender == null)
                        {

                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, pc, armyAttacker))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        ProtoMessage attackResult=null;
                        if (armyAttacker.ChecksBeforeAttack(armyDefender,out attackResult))
                        {
                            // TODO refactor battle
                            // GiveBattle returns necessary messages
                            ProtoBattle battleResults = null;
                            bool isVictorious = Battle.GiveBattle(armyAttacker, armyDefender,out battleResults);
                          /*  if (isVictorious)
                            {
                                attackResult = new ProtoMessage();
                                attackResult.ResponseType=DisplayMessages.BattleBringSuccess;
                                attackResult.MessageFields = new string[] { "You", "the defending army" };
                                return attackResult;
                            }
                            else
                            {
                                attackResult = new ProtoMessage();
                                attackResult.ResponseType = DisplayMessages.BattleBringFail;
                                attackResult.MessageFields = new string[] { "You", "the defending army" };
                                return attackResult;
                            }*/
                            battleResults.ResponseType = DisplayMessages.BattleResults;
                            return battleResults;
                        }
                        else
                        {
                            return attackResult;
                        }
                    }
                // View journal entries
                case Actions.ViewJournalEntries:
                    {
                        // Get client
                        Client client = null;
                        Globals_Server.clients.TryGetValue(pc.playerID, out client);
                        if (client != null)
                        {
                            // Get list of journal entries for scope
                            string scope = msgIn.Message;
                            var entries = client.myPastEvents.getJournalEntrySet(scope, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
                            Console.WriteLine("There are :" + entries.Count + " entries in journal");
                            ProtoJournalEntry[] entryList = new ProtoJournalEntry[entries.Count];
                            int i = 0;
                            foreach (var entry in entries)
                            {
                                ProtoJournalEntry newEntry = new ProtoJournalEntry(entry.Value);
                                entryList[i] = newEntry;
                                i++;
                            }
                            ProtoGenericArray<ProtoJournalEntry> result = new ProtoGenericArray<ProtoJournalEntry>(entryList);
                            result.ResponseType = DisplayMessages.JournalEntries;
                            return result;
                        }
                        // if no client, nobody to send error to
                        else
                        {
                            Globals_Server.logError("ViewJournalEntries: entries requested from non-existant client " + pc.playerID);
                            return null;
                        }
                    }
                    break;
                case Actions.ViewJournalEntry:
                    {
                        Client client = null;
                        Globals_Server.clients.TryGetValue(pc.playerID, out client);
                        if (client != null)
                        {
                            try {
                                uint id = Convert.ToUInt32(msgIn.Message);
                                JournalEntry jEntry= null;
                                client.myPastEvents.entries.TryGetValue(id,out jEntry);
                                if(jEntry==null) {
                                    ProtoMessage error = new ProtoMessage();
                                    error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                    return error;
                                }
                                else {
                                    ProtoJournalEntry reply = new ProtoJournalEntry(jEntry);
                                    reply.ResponseType=DisplayMessages.Success;
                                    return reply;
                                }
                            }
                            catch(Exception e) {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                                return error;
                            }
                        }
                        else
                        {
                            Globals_Server.logError("ViewJournalEntry: entry requested from non-existant client " + pc.playerID);
                            return null;
                        }
                    }
                case Actions.SpyArmy:
                    {
                        Army army;
                        Character spy;
                        spy = Globals_Game.getCharFromID(msgIn.Message);
                        if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1 || string.IsNullOrWhiteSpace(msgIn.MessageFields[0]))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        Globals_Game.armyMasterList.TryGetValue(msgIn.MessageFields[0], out army);
                        // Ensure character and army are valid
                        if (spy == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        if (army == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericArmyUnidentified;
                            return error;
                        }
                        // Ensure spy is pc's character
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin,pc,spy))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Ensure spy is in same location
                        if(spy.location.id != (army.location)) {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                            return error;
                        }
                        // Ensure not trying to spy on own army
                        if (army.GetOwner() == pc)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorSpyOwn;
                            error.MessageFields = new string[] { "army" };
                            return error;
                        }
                        ProtoMessage result = null;
                        if (spy.SpyOn(army, out result))
                        {
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                        else
                        {
                            ProtoMessage message = new ProtoMessage();
                            message.ResponseType = DisplayMessages.None;
                            return message;
                        }
                    }
                    break;
                case Actions.SpyCharacter:
                    {
                        Character target;
                        Character spy;
                        spy = Globals_Game.getCharFromID(msgIn.Message);
                        if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1 || string.IsNullOrWhiteSpace(msgIn.MessageFields[0]))
                        {
                            Console.WriteLine("Invalid here");
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        target = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                        // Ensure character and army are valid
                        if (spy == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        if (target == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        // Ensure spy is pc's character
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, spy))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Ensure spy is in same location
                        if (!spy.location.Equals(target.location))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                            return error;
                        }
                        // Ensure not trying to spy on own army
                        if (target.GetPlayerCharacter() == pc)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorSpyOwn;
                            error.MessageFields = new string[] { "personnel" };
                            return error;
                        }
                        ProtoMessage result = null;
                        spy.SpyOn(target, out result);
                        if (result == null)
                        {
                            result = new ProtoMessage();
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                        else
                        {
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                    }
                    break;
                case Actions.SpyFief:
                    {
                        Fief fief;
                        Character spy;
                        spy = Globals_Game.getCharFromID(msgIn.Message);
                        if (msgIn.MessageFields == null || msgIn.MessageFields.Length < 1 || string.IsNullOrWhiteSpace(msgIn.MessageFields[0]))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        Globals_Game.fiefMasterList.TryGetValue(msgIn.MessageFields[0],out fief);
                        // Ensure character and army are valid
                        if (spy == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        if (fief == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                            return error;
                        }
                        // Ensure spy is pc's character
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, spy))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                            return error;
                        }
                        // Ensure spy is in same location
                        if (!spy.location.Equals(fief))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                            return error;
                        }
                        // Ensure not trying to spy on own army
                        if (fief.owner == pc)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorSpyOwn;
                            error.MessageFields = new string[] { "fief" };
                            return error;
                        }
                        ProtoMessage result = null;
                        if (spy.SpyOn(fief, out result))
                        {
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                        else
                        {
                            ProtoMessage message = new ProtoMessage();
                            message.ResponseType = DisplayMessages.None;
                            return message;
                        }
                    }
                    break;
                case Actions.Kidnap:
                    {
                        Character target;
                        Character kidnapper;
                        kidnapper = Globals_Game.getCharFromID(msgIn.Message);
                        if(msgIn.MessageFields==null||string.IsNullOrWhiteSpace(msgIn.MessageFields[0])) {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType=DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        target = Globals_Game.getCharFromID(msgIn.MessageFields[0]);
                        if (kidnapper == null || target == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        Console.WriteLine("PC: " + pc.firstName + " " + pc.familyName + " " + pc.charID + ", Kidnapper " + kidnapper.firstName + " " + kidnapper.familyName + " " + kidnapper.charID);
                        if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, pc, kidnapper))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericUnauthorised; ;
                            return error;
                        }
                        ProtoMessage result;
                        kidnapper.Kidnap(target, out result);
                        if (result == null)
                        {
                            result = new ProtoMessage();
                            result.ResponseType = DisplayMessages.Success;
                            return result;
                        }
                        else
                        {
                            return result;
                        }
                    }
                    break;
                case Actions.ViewCaptives:
                    {
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        List<Character> captiveList=null;
                        if (msgIn.Message.Equals("all"))
                        {
                            captiveList = pc.myCaptives;
                        }
                        else
                        {
                            // If not all captives, check for fief captives
                            Fief fief;
                            Globals_Game.fiefMasterList.TryGetValue(msgIn.Message, out fief);
                            if (fief != null)
                            {
                                // Ensure has permission
                                if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, pc, fief))
                                {
                                    ProtoMessage error = new ProtoMessage();
                                    error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                                    return error;
                                }
                                captiveList = fief.gaol;
                            }
                            else
                            {
                                // error
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                                return error;
                            }
                        }
                        if (captiveList != null && captiveList.Count > 0)
                        {
                            ProtoGenericArray<ProtoCharacterOverview> captives = new ProtoGenericArray<ProtoCharacterOverview>();
                            captives.fields = new ProtoCharacterOverview[captiveList.Count];
                            int i = 0;
                            foreach (Character captive in captiveList)
                            {
                                ProtoCharacterOverview captiveDetails = new ProtoCharacterOverview(captive);
                                captiveDetails.showLocation(captive);
                                captives.fields[i] = captiveDetails;
                                i++;
                            }
                            captives.ResponseType = DisplayMessages.Success;
                            return captives;
                        }
                        else
                        {
                            ProtoMessage NoCaptives = new ProtoMessage();
                            NoCaptives.ResponseType = DisplayMessages.FiefNoCaptives;
                            return NoCaptives;
                        }
                    }
                    break;
                case Actions.ViewCaptive:
                    {
                        Character captive = Globals_Game.getCharFromID(msgIn.Message);
                        if(captive==null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        if (!pc.myCaptives.Contains(captive))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.NotCaptive;
                            return error;
                        }
                        else
                        {
                            ProtoCharacter captiveDetails = new ProtoCharacter(captive);
                            captiveDetails.ResponseType = DisplayMessages.Success;
                            return captiveDetails;
                        }
                    }
                    break;
                case Actions.RansomCaptive:
                    {
                        Character captive = null;
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        captive=Globals_Game.getCharFromID(msgIn.Message);
                        if (captive == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        else
                        {
                            if (!pc.myCaptives.Contains(captive))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.NotCaptive;
                                return error;
                            }
                            if (!string.IsNullOrWhiteSpace(captive.ransomDemand))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.RansomAlready;
                                return error;
                            }
                            pc.RansomCaptive(captive);
                            ProtoMessage success = new ProtoMessage();
                            success.ResponseType = DisplayMessages.Success;
                            success.Message = captive.CalculateRansom().ToString();
                            return success;
                        }
                    }
                    break;
                case Actions.ReleaseCaptive:
                    {
                        Character captive = null;
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        captive = Globals_Game.getCharFromID(msgIn.Message);
                        if (captive == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        else
                        {
                            if (!pc.myCaptives.Contains(captive))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.NotCaptive;
                                return error;
                            }
                            pc.ReleaseCaptive(captive);
                            ProtoMessage success = new ProtoMessage();
                            success.ResponseType = DisplayMessages.Success;
                            return success;
                        }
                    }
                    break;
                case Actions.ExecuteCaptive:
                    {
                        Character captive = null;
                        if (string.IsNullOrWhiteSpace(msgIn.Message))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        captive = Globals_Game.getCharFromID(msgIn.Message);
                        if (captive == null)
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                            return error;
                        }
                        else
                        {
                            if (!pc.myCaptives.Contains(captive))
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.NotCaptive;
                                return error;
                            }
                            pc.ExecuteCaptive(captive);
                            ProtoMessage success = new ProtoMessage();
                            success.ResponseType = DisplayMessages.Success;
                            return success;
                        }
                    }
                    break;
                case Actions.RespondRansom:
                    {
                        JournalEntry jEntry =null;
                        bool pay = false;
                        if (string.IsNullOrWhiteSpace(msgIn.Message) || msgIn.MessageFields == null || string.IsNullOrWhiteSpace(msgIn.MessageFields[0]))
                        {
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        try {
                            UInt32 entryID = Convert.ToUInt32(msgIn.Message);
                            Globals_Game.pastEvents.entries.TryGetValue(entryID, out jEntry);
                            if (jEntry == null)
                            {
                                ProtoMessage error = new ProtoMessage();
                                error.ResponseType = DisplayMessages.JournalEntryUnrecognised;
                                return error;
                            }
                            pay = Convert.ToBoolean(msgIn.MessageFields[0]);
                            ProtoMessage ransomResult = null;
                            if (jEntry.RansomResponse(pay, out ransomResult))
                            {
                                ransomResult = new ProtoMessage();
                                ransomResult.ResponseType = DisplayMessages.Success;
                                return ransomResult;
                            }
                            else
                            {
                                return ransomResult;
                            }
                        }
                        catch (Exception e)
                        {
                            Globals_Server.logError("Conversion error in RespondRansom: " + e.Message);
                            ProtoMessage error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                            return error;
                        }
                        
                    }
                    break;
                case Actions.SeasonUpdate:
                    {
                        Globals_Game.game.SeasonUpdate();
                        ProtoMessage updated = new ProtoMessage();
                        updated.ResponseType = DisplayMessages.Success;
                        return updated;
                    }
                    break;
                default:
                    {
                        ProtoMessage error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                        return error;
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
