using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using RiakClient;
namespace hist_mmorpg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt"))
            {
				Globals_Server.rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig","app.config");
				Globals_Server.rClient = (RiakClient.RiakClient)Globals_Server.rCluster.CreateClient();
                Globals_Server.LogFile.AutoFlush = true;
                Globals_Server.logEvent("Server start");
                Game game = new Game();
				/*if (Globals_Server.rClient.Ping ().IsSuccess) {
					Console.WriteLine ("Database connection successful");
					string gameID = "testBucket";
					DatabaseWrite.DatabaseWriteAll (gameID);
					// Test can read from database
					PlayerCharacter testRead = DatabaseRead.DatabaseRead_PC (gameID, "Char_158");
					if (testRead == null) {
						Console.WriteLine ("FAIL :(");
					} else {
						Console.WriteLine ("Got Character: " + testRead.firstName + " " + testRead.familyName);
					}
					DatabaseRead.DatabaseReadAll (gameID);
				} else {
					Console.WriteLine ("Could not connect to database :( ");
				} */
                testCaptives();
                Server server = new Server();

                //testArmy();
                //testSpying();
                //Console.Read();
            }
            
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
