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

namespace hist_mmorpg
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
           
            try
            {
                using (Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt"))
                {
                    
                    //Globals_Server.rCluster = RiakCluster.FromConfig("riakConfig","app.config");
                    //Globals_Server.rClient = Globals_Server.rCluster.CreateClient();
                    Globals_Server.LogFile.AutoFlush = true;
                    Globals_Server.logEvent("Server start");
                    
                    Game game = new Game();
                    SetUpForDemo();
                    /*
                    //DatabaseWrite.DatabaseWriteAll ("testBucket");
                    
                    /*if (Globals_Server.rClient.Ping ().IsSuccess) {
                        Console.WriteLine ("Database connection successful");
                        string gameID = "testBucket";
                        foreach (string trait in Globals_Game.traitKeys) {
                            Console.WriteLine (trait);
                        }

                        // Test can read from database
                        var newClient = Globals_Server.rCluster.CreateClient();
                        RiakObject newObj = new RiakObject (gameID, "superawesome3", Globals_Game.traitKeys.ToArray ());
                        newClient.Put (newObj);
                        Thread.Sleep (5000);
                        var testRead =newClient.Get (gameID, "superawesome3");
                        if (!testRead.IsSuccess) {
                            Console.WriteLine ("FAIL :(" + testRead.ErrorMessage);
                        } else {
                            Console.WriteLine ("Got traitkeys:");
                        }
                        //DatabaseRead.DatabaseReadAll (gameID);
                    } else {
                        Console.WriteLine ("Could not connect to database :( ");
                    } */

                    //testCaptives();

                    
                    
                            Server server = new Server();
                    try
                    {
                        //TestSuite testSuite = new TestSuite();
                        TestClient client = new TestClient();
                        client.LogIn("helen", "potato");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    //client.LogIn("helen", "potato");
                            String s = Console.ReadLine();
                            if (s != null && s.Equals("exit"))
                            {
                                Globals_Server.logEvent("Server exits");
                                server.isListening = false;
                                Globals_Server.server.Shutdown("Server exits");
                            }

                    //testArmy();
                    //testSpying();
                    /*
                            while (true)
                            {
                                
                                if (s != null && s.Equals("exit"))
                                {
                                    Globals_Server.logEvent("Server exits");
                                    server.isListening = false;
                                    Globals_Server.server.Shutdown("Server exits");
                                    break;
                                }

                            }
                     
                            * */
                    Globals_Server.LogFile.Close();

                }
             
            }
            catch (Exception e)
            {
                Globals_Server.LogFile.Close();
                Console.WriteLine("Encountered an error:" +e.StackTrace);
                Console.ReadLine();
            }

            
        }

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
