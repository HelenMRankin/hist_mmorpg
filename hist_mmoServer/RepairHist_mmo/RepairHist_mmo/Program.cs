#define TESTSUITE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using RiakClient;
using System.Threading;
using RiakClient.Models;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace hist_mmorpg
{

    static class Program
    {
        /// <summary>
        /// The Game object for this test (contains and handles all game data)
        /// </summary>
        public static Game game;
        /// <summary>
        /// The Server object used for this test (contains connected client information
        /// </summary>
        public static Server server;
        /// <summary>
        /// The dummy Client to be used for this test
        /// </summary>
        public static TestClient client;

        public static Army OwnedArmy;
        public static Army NotOwnedArmy;
        public static Fief OwnedFief;
        public static Fief NotOwnedFief;
        public static string Username;
        public static string Pass;
        public static string BadUsername;
        public static string BadPass;
        public static PlayerCharacter MyPlayerCharacter;
        public static PlayerCharacter NotMyPlayerCharacter;
        public static NonPlayerCharacter MyFamily;
        public static NonPlayerCharacter MyEmployee;
        public static NonPlayerCharacter NotMyFamily;
        public static NonPlayerCharacter NotMyEmplployee;
        public static NonPlayerCharacter NobodysCharacter;

        /// <summary>
        /// Initialise game state for the TestSuite
        /// </summary>
        /// <param name="ctx"></param>
        public static void InitialiseGameState()
        {
            Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt");
            Globals_Server.LogFile.AutoFlush = true;
            game = new Game();
            server = new Server();
            client = new TestClient();

            Username = "helen";
            Pass = "potato";
            BadUsername = "notauser";
            BadPass = "notapass";
            MyPlayerCharacter = Globals_Game.userChars[Username];
            Dictionary<string, PlayerCharacter>.Enumerator e = Globals_Game.pcMasterList.GetEnumerator();
            e.MoveNext();
            NotMyPlayerCharacter = e.Current.Value;
            while (NotMyPlayerCharacter == MyPlayerCharacter)
            {
                e.MoveNext();
                NotMyPlayerCharacter = e.Current.Value;
            }
            foreach (NonPlayerCharacter npc in MyPlayerCharacter.myNPCs)
            {
                if (!string.IsNullOrWhiteSpace(npc.familyID))
                {
                    MyFamily = npc;
                }
                else if (!string.IsNullOrWhiteSpace(npc.employer))
                {
                    MyEmployee = npc;
                }
                if (MyEmployee != null && MyFamily != null)
                {
                    break;
                }
            }
            foreach (NonPlayerCharacter npc in NotMyPlayerCharacter.myNPCs)
            {
                if (!string.IsNullOrWhiteSpace(npc.familyID))
                {
                    NotMyFamily = npc;
                }
                else if (!string.IsNullOrWhiteSpace(npc.employer))
                {
                    NotMyEmplployee = npc;
                }
                if (NotMyEmplployee != null && NotMyFamily != null)
                {
                    break;
                }
            }
            if (MyPlayerCharacter.myArmies != null && MyPlayerCharacter.myArmies.Count > 0)
            {
                OwnedArmy = MyPlayerCharacter.myArmies[0];
            }
            else
            {
                Console.WriteLine("Set owned army");
                Army army = new Army(Globals_Game.GetNextArmyID(), null, MyPlayerCharacter.charID, 30, NotMyPlayerCharacter.location.id, false, trp: new uint[] { 5, 5, 5, 5, 5, 5 });
                OwnedArmy = army;
                OwnedArmy.AddArmy();
            }
            if (NotMyPlayerCharacter.myArmies != null && NotMyPlayerCharacter.myArmies.Count > 0)
            {
                NotOwnedArmy = NotMyPlayerCharacter.myArmies[0];
            }
            else
            {
                Console.Write("Set not owned army");
                Army army = new Army(Globals_Game.GetNextArmyID(), null, NotMyPlayerCharacter.charID, 30, NotMyPlayerCharacter.location.id, false, trp: new uint[] { 5, 5, 5, 5, 5, 5 });
                NotOwnedArmy = army;
                NotOwnedArmy.AddArmy();

            }
            if (MyPlayerCharacter.ownedFiefs != null && MyPlayerCharacter.ownedFiefs.Count > 0)
            {
                OwnedFief = MyPlayerCharacter.ownedFiefs[0];
            }
            if (NotMyPlayerCharacter.ownedFiefs != null && NotMyPlayerCharacter.ownedFiefs.Count > 0)
            {
                NotOwnedFief = NotMyPlayerCharacter.ownedFiefs[0];
            }
            foreach (var npc in Globals_Game.npcMasterList)
            {
                if (npc.Value.GetPlayerCharacter() == null)
                {
                    NobodysCharacter = npc.Value;
                }
            }
        }


        public static void FinaliseGameState()
        {
            server.Shutdown();
            Globals_Server.LogFile.Close();
        }

        public static void Main()
        {
            InitialiseGameState();
            TestRun();

            
        }

        public static async void TestRun()
        {
            client.LogInAndConnect("helen", "potato", new byte[] { 1, 2, 3, 4, 5, 6 });
            while(!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.Move(MyPlayerCharacter.charID, NotOwnedFief.id);
            client.ExamineArmies(NotOwnedFief.id);
            client.Camp(1);
            
            ProtoMessage reply = null;
            Task<ProtoMessage> MessageTask = client.GetReply();
            MessageTask.Wait();
            reply = MessageTask.Result;
            while(reply.ActionType!=Actions.Camp)
            {
                MessageTask = client.GetReply();
                MessageTask.Wait();
                reply = MessageTask.Result;
            }
            Console.WriteLine("End of test run");
            client.LogOut();
            FinaliseGameState();
        }
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //static void Main()
        //{

        //    try
        //    {
        //        using (Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt"))
        //        {

        //            //Globals_Server.rCluster = RiakCluster.FromConfig("riakConfig","app.config");
        //            //Globals_Server.rClient = Globals_Server.rCluster.CreateClient();
        //            Globals_Server.LogFile.AutoFlush = true;
        //            Globals_Server.logEvent("Server start");

        //            Game game = new Game();
        //            SetUpForDemo();
        //            /*
        //            //DatabaseWrite.DatabaseWriteAll ("testBucket");

        //            /*if (Globals_Server.rClient.Ping ().IsSuccess) {
        //                Console.WriteLine ("Database connection successful");
        //                string gameID = "testBucket";
        //                foreach (string trait in Globals_Game.traitKeys) {
        //                    Console.WriteLine (trait);
        //                }

        //                // Test can read from database
        //                var newClient = Globals_Server.rCluster.CreateClient();
        //                RiakObject newObj = new RiakObject (gameID, "superawesome3", Globals_Game.traitKeys.ToArray ());
        //                newClient.Put (newObj);
        //                Thread.Sleep (5000);
        //                var testRead =newClient.Get (gameID, "superawesome3");
        //                if (!testRead.IsSuccess) {
        //                    Console.WriteLine ("FAIL :(" + testRead.ErrorMessage);
        //                } else {
        //                    Console.WriteLine ("Got traitkeys:");
        //                }
        //                //DatabaseRead.DatabaseReadAll (gameID);
        //            } else {
        //                Console.WriteLine ("Could not connect to database :( ");
        //            } */

        //            //testCaptives();



        //                    Server server = new Server();
        //            try
        //            {
        //                //TestSuite testSuite = new TestSuite();
        //                TestClient client = new TestClient();
        //                client.LogInAndConnect("helen", "potato");
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);
        //            }
        //            //client.LogIn("helen", "potato");
        //                    String s = Console.ReadLine();
        //                    if (s != null && s.Equals("exit"))
        //                    {
        //                        Globals_Server.logEvent("Server exits");
        //                        server.isListening = false;
        //                        Globals_Server.server.Shutdown("Server exits");
        //                    }

        //            //testArmy();
        //            //testSpying();
        //            /*
        //                    while (true)
        //                    {

        //                        if (s != null && s.Equals("exit"))
        //                        {
        //                            Globals_Server.logEvent("Server exits");
        //                            server.isListening = false;
        //                            Globals_Server.server.Shutdown("Server exits");
        //                            break;
        //                        }

        //                    }

        //                    * */
        //            Globals_Server.LogFile.Close();

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Globals_Server.LogFile.Close();
        //        Console.WriteLine("Encountered an error:" +e.StackTrace);
        //        Console.ReadLine();
        //    }


        //}

        public static void SetUpForDemo()
        {
            // Make Anselm Marshal very sneaky
            Character Anselm = Globals_Game.getCharFromID("Char_390");
            Character Bishop = Globals_Game.getCharFromID("Char_391");
            Tuple<Trait, int>[] newTraits = new Tuple<Trait, int>[2];
            newTraits[0] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_9"], 9);
            newTraits[1] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_8"], 9);
            Anselm.traits = newTraits;
            // Make Bishop Henry Marshal not sneaky
            Tuple<Trait, int>[] newTraits2 = new Tuple<Trait, int>[1];
            newTraits2[0] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_5"], 2);
            Bishop.traits = newTraits2;
            // Add funds to home treasury
            (Globals_Game.getCharFromID("Char_158") as PlayerCharacter).GetHomeFief().AdjustTreasury(100000);

            // create and add army
            uint[] myArmyTroops1 = new uint[] { 8, 10, 0, 30, 60, 100, 220 };
            Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
            myArmy1.AddArmy();
            // create and add army
            uint[] myArmyTroops2 = new uint[] { 5, 10, 0, 30, 40, 80, 220 };
            Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
            myArmy2.AddArmy();

            // Add single lady appropriate for marriage
            Nationality nat = Globals_Game.nationalityMasterList["Sco"];
            NonPlayerCharacter proposalChar = new NonPlayerCharacter("Char_626", "Mairi", "Meah", new Tuple<uint, byte>(1162, 3), false, Globals_Game.nationalityMasterList["Sco"], true, 9, 9, new Queue<Fief>(), Globals_Game.languageMasterList["lang_C1"], 90, 9, 9, 9, new Tuple<Trait, int>[0], true, false, "Char_126", null, "Char_126", null, 0, false, false, new List<string>(), null, null, Globals_Game.fiefMasterList["ESW05"]);
            PlayerCharacter pc = Globals_Game.pcMasterList["Char_126"];
            pc.myNPCs.Add(proposalChar);
            proposalChar.inKeep = false;
        }
        public static void testCaptives()
        {
            Character captive = Globals_Game.getCharFromID("Char_626");
            PlayerCharacter captor = Globals_Game.pcMasterList["Char_158"];
            Fief location = Globals_Game.fiefMasterList["EPM02"];
            captor.AddCaptive(captive, location);
            //captor.ReleaseCaptive(captive);
            //captor.ExecuteCaptive(captive);
            Console.WriteLine(captive.CalculateRansom());
            Console.WriteLine(captive.isAlive.ToString());
            Console.WriteLine(captive.location.id);
        }
        public static void testArmy()
        {
            Army A = Globals_Game.armyMasterList["Army_1"];
            Army B = Globals_Game.armyMasterList["Army_2"];
            Console.WriteLine("Army 1 troops:");
            for (int i = 0; i < 7; i++)
            {
                Console.Write(i + ": " + A.troops[i] + ", ");
            }
            Console.WriteLine("\nArmy 2 troops:");
            for (int i = 0; i < 7; i++)
            {
                Console.Write(i + ": " + B.troops[i] + ", ");
            }
            Console.WriteLine("\nCombat values A: " + A.CalculateCombatValue());
            Console.WriteLine("Combat values B: " + B.CalculateCombatValue());
            Console.WriteLine("Combat advantages A: " + A.CalculateTroopTypeAdvatages(B.troops));
            Console.WriteLine("Combat advantages B: " + B.CalculateTroopTypeAdvatages(A.troops));
           
        }

        public static void testSpying()
        {
            Fief f = Globals_Game.fiefMasterList["ESW04"];
            Character bailiff = Globals_Game.pcMasterList["Char_158"];
            bailiff.traits = new Tuple<Trait, int>[] { new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_8"], 9) };
            f.bailiff = bailiff;
            Character c = Globals_Game.npcMasterList["Char_626"];
            c.traits = new Tuple<Trait, int>[] { new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_9"], 1) };
            ProtoMessage ignore;
            c.SpyOn(f,out ignore);
        }
    }
}
