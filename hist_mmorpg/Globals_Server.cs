using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using CorrugatedIron;
//using CorrugatedIron.Models;
using RiakClient;
using RiakClient.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required static variables for server-side
    /// </summary>
    public static class Globals_Server
    {
        /// <summary>
        /// Holds the usernames and Client objects of connected players
        /// </summary>
        public static Dictionary<string, Client> clients = new Dictionary<string, Client>();
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
        /// Gets the next available newGameID, then increments it
        /// </summary>
        /// <returns>string containing newGameID</returns>
        public static string GetNextGameID()
        {
            string gameID = "Game_" + Globals_Server.newGameID;
            Globals_Server.newGameID++;
            return gameID;
        }

        //TODO
        /// <summary>
        /// Writes any errors encountered to a logfile
        /// </summary>
        /// <param name="error"></param>
        public static void logError(String error)
        {

        }

    }
}
