using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RiakClient;
using RiakClient.Models;
using Lidgren.Network;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required static variables for server-side
    /// </summary>
    public static class Globals_Server
    {
        /// <summary>
        /// Holds the usernames and Client objects of all players
        /// </summary>
        public static Dictionary<string, Client> Clients = new Dictionary<string, Client>();
        /// <summary>
        /// Holds all usernames/playerIDs (used in DatabaseRead)
        /// </summary>
        public static List<string> client_keys = new List<string>();
        /// <summary>
        /// Holds target RiakCluster 
        /// </summary>
        public static IRiakEndPoint rCluster;
        /// <summary>
        /// Holds RiakClient to communicate with RiakCluster
        /// </summary>
        public static IRiakClient rClient;
        /// <summary>
        /// Holds next value for game ID
        /// </summary>
        public static uint newGameID = 1;
        /// <summary>
        /// Holds combat values for different troop types and nationalities
        /// Key = nationality & Value = combat value for knights, menAtArms, lightCavalry, longbowmen, crossbowmen, foot, rabble
        /// </summary>
        public static Dictionary<string, uint[]> combatValues = new Dictionary<string, uint[]>();
        /// <summary>
        /// Dictionary mapping two troop types to a value representing one's effectiveness against the other
        /// </summary>
        public static Dictionary<Tuple<uint,uint>, double> troopTypeAdvantages = new Dictionary<Tuple<uint,uint>,double>();
        /// <summary>
        /// Holds recruitment ratios for different troop types and nationalities
        /// Key = nationality & Value = % ratio for knights, menAtArms, lightCavalry, longbowmen, crossbowmen, foot, rabble
        /// </summary>
        public static Dictionary<string, double[]> recruitRatios = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds probabilities for battle occurring at certain combat odds under certain conditions
        /// Key = 'odds', 'battle', 'pillage'
        /// Value = percentage likelihood of battle occurring
        /// </summary>
        public static Dictionary<string, double[]> battleProbabilities = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds type of game  (sets victory conditions)
        /// </summary>
        public static Dictionary<uint, string> gameTypes = new Dictionary<uint, string>();
        /// <summary>
        /// Holds NetServer used for hosting game
        /// </summary>
        public static NetServer server;
        /// <summary>
        /// Gets the next available newGameID, then increments it
        /// </summary>
        /// <returns>string containing newGameID</returns>
        public static string GetNextGameID()
        {
            string gameID = "Game_" + Globals_Server.newGameID;
            Globals_Server.newGameID++;
            return gameID;
        }

		public static StreamWriter LogFile;
        /// <summary>
        /// Writes any errors encountered to a logfile
        /// </summary>
        /// <param name="error"></param>
        public static void logError(String error)
        {
            try
            {
                LogFile.WriteLine("Run-time error: " + error);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failure to write to log file: "+e.ToString());
            }
#if DEBUG
			Console.WriteLine ("Run-time error: " + error);
#endif
        }

        public static void logEvent(String eventDetails)
        {
            try
            {
                LogFile.WriteLine(eventDetails);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failure to write to log file: "+e.ToString());
            }
#if DEBUG
            Console.WriteLine(eventDetails);
#endif
        }
    }
}
