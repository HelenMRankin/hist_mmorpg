using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuickGraph;
using QuickGraph.Algorithms;

namespace hist_mmorpg
{
    /// <summary>
    /// Class defining HexMapGraph
    /// </summary>
	public class HexMapGraph
    {

		/// <summary>
		/// Holds map ID
		/// </summary>
		public String mapID { get; set; }
        /// <summary>
        /// Holds map object AdjacencyGraph (from QuickGraph library), 
        /// specifying edge type (tagged)
        /// </summary>
        public AdjacencyGraph<Fief, TaggedEdge<Fief, string>> myMap { get; set; }
        /// <summary>
        /// Dictionary holding edge costs, for use when calculating shortest path
        /// </summary>
        private Dictionary<TaggedEdge<Fief, string>, double> costs { get; set; }

		/// <summary>
        /// Constructor for HexMapGraph
        /// </summary>
		/// <param name="id">String holding map ID</param>
		public HexMapGraph(String id)
        {
			this.mapID = id;
            myMap = new AdjacencyGraph<Fief, TaggedEdge<Fief, string>>();
            costs = new Dictionary<TaggedEdge<Fief, string>, double>();
        }

        /// <summary>
        /// Constructor for HexMapGraph, allowing map to be constructed from an array of edges
        /// </summary>
        /// <param name="id">String holding map ID</param>
        /// <param name="id">Array of edges</param>
        public HexMapGraph(String id, TaggedEdge<Fief, string>[] myEdges)
		{
			this.mapID = id;
            // construct new graph from array of edges
			myMap = myEdges.ToAdjacencyGraph<Fief, TaggedEdge<Fief, string>>();
			costs = new Dictionary<TaggedEdge<Fief, string>, double>();
            // populate costs, based on target and source terrain costs of each edge
            foreach (var e in this.myMap.Edges)
			{
				this.AddCost (e, (e.Source.terrain.travelCost + e.Target.terrain.travelCost) / 2);
			}
		}

        /// <summary>
        /// Constructor for HexMapGraph taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public HexMapGraph()
		{
		}

        /// <summary>
        /// Adds hex (vertex) and route (edge) in one operation.
        /// Existing hexes and routes will be ignored
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="s">Source hex (Fief)</param>
        /// <param name="t">Target hex (Fief)</param>
        /// <param name="tag">String tag for route</param>
        /// <param name="cost">Cost for route</param>
        public bool AddHexesAndRoute(Fief s, Fief t, string tag, double cost)
        {
            bool success = false;

            // create route
            TaggedEdge<Fief, string> myEdge = this.CreateEdge(s, t, tag);

            // use route as source to add route and hex to graph
            success = this.myMap.AddVerticesAndEdge(myEdge);

            // if successful, add route cost
            if (success)
            {
                this.AddCost(myEdge, cost);
            }

            return success;
        }

        /// <summary>
        /// Adds route
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="s">Source hex (Fief)</param>
        /// <param name="t">Target hex (Fief)</param>
        /// <param name="tag">String tag for route</param>
        /// <param name="cost">Cost for route</param>
        public bool AddRoute(Fief s, Fief t, string tag, double cost)
        {
            bool success = false;
            // create route
            TaggedEdge<Fief, string> myEdge = this.CreateEdge(s, t, tag);
            // add route
            success = this.myMap.AddEdge(myEdge);
            // if successful, add route cost
            if (success)
            {
                this.AddCost(myEdge, cost);
            }
            return success;
        }

        /// <summary>
        /// Removes route
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="s">Source hex (Fief)</param>
        /// <param name="tag">String tag for route</param>
        public bool RemoveRoute(Fief s, string tag)
        {
            bool success = false;
            // iterate through routes
            foreach (var e in this.myMap.Edges)
            {
                // if source matches, check tag
                if (e.Source == s)
                {
                    // if tag matches, remove route
                    if (e.Tag.Equals(tag))
                    {
                        success = this.myMap.RemoveEdge(e);
                        // if route successfully removed, remove cost
                        if (success)
                        {
                            this.RemoveCost(e);
                        }
                        break;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Adds hex (Fief)
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="f">Hex (Fief) to add</param>
        public bool AddHex(Fief f)
        {
            bool success = false;
            // add hex
            success = this.myMap.AddVertex(f);
            return success;
        }

        /// <summary>
        /// Removes hex (Fief)
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="f">Hex (Fief) to remove</param>
        public bool RemoveHex(Fief f)
        {
            bool success = false;
            // remove hex
            success = this.myMap.RemoveVertex(f);
            return success;
        }

        /// <summary>
        /// Adds route (edge) cost to the costs collection
        /// </summary>
        /// <param name="e">Route (edge)</param>
        /// <param name="cost">Route cost to add</param>
        public void AddCost(TaggedEdge<Fief, string> e, double cost)
        {
            // add cost
            costs.Add(e, cost);
        }

        /// <summary>
        /// Removes route (edge) cost from the costs collection
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="e">Route (edge)</param>
        public bool RemoveCost(TaggedEdge<Fief, string> e)
        {
            // remove cost
            bool success = costs.Remove(e);

            return success;
        }

        /// <summary>
        /// Creates new route (edge)
        /// </summary>
        /// <returns>TaggedEdge</returns>
        /// <param name="s">Source hex (Fief)</param>
        /// <param name="t">Target hex (Fief)</param>
        /// <param name="tag">String tag for route</param>
        public TaggedEdge<Fief, string> CreateEdge(Fief s, Fief t, string tag)
        {
            // create route
            TaggedEdge<Fief, string> myEdge = new TaggedEdge<Fief, string>(s, t, tag);
            return myEdge;
        }

        /// <summary>
        /// Selects random adjoining hex (also equal chance to select current hex)
        /// </summary>
        /// <returns>Fief to move to (or null)</returns>
        /// <param name="from">Current fief</param>
        /// <param name="getOwned">bool indicating whether or not to try to return an owned fief</param>
        /// <param name="owner">owner, when looking for an owned fief</param>
        /// <param name="avoid">Fief to avoid (for retreats)</param>
        public Fief chooseRandomHex(Fief from, bool getOwned = false, PlayerCharacter fiefOwner = null, Fief avoid = null)
        {
            // list to store all edges
            List<TaggedEdge<Fief, string>> choices = new List<TaggedEdge<Fief, string>>();
            // list to store all edges to owned fiefs
            List<TaggedEdge<Fief, string>> ownedChoices = new List<TaggedEdge<Fief, string>>();
            // int to use in edge selection
            int selection = 0;
            // string to contain chosen move direction
            Fief goTo = null;

            // identify and store all target hexes from source hex
            foreach (var e in this.myMap.Edges)
            {
                bool okToAdd = true;
                if (e.Source == from)
                {
                    // no 'avoid' fief specified
                    if (avoid == null)
                    {
                        okToAdd = true;
                    }

                    // if 'avoid' fief specified
                    else
                    {
                        // if is NOT specified 'avoid' fief
                        if (e.Target != avoid)
                        {
                            okToAdd = true;
                        }

                        // if IS specified 'avoid' fief
                        else
                        {
                            okToAdd = false;
                        }
                    }

                    if (okToAdd)
                    {
                        choices.Add(e);

                        // if getOwned, also check for target ownership
                        if (getOwned)
                        {
                            if (e.Target.owner == fiefOwner)
                            {
                                ownedChoices.Add(e);
                            }
                        }
                    }
                }
            }

            // if looking for owned fief, get one if possible
            if ((getOwned) && (ownedChoices.Count > 0))
            {
                // choose fief by generating random int between 0 and no. of targets
                selection = Globals_Game.myRand.Next(0, ownedChoices.Count);

                // get Fief
                goTo = ownedChoices[selection].Target;
            }

            // if ownership not required, choose from all adjoining fiefs
            else if (choices.Count > 0)
            {
                // choose fief by generating random int between 0 and no. of targets
                selection = Globals_Game.myRand.Next(0, choices.Count);

                // get Fief
                goTo = choices[selection].Target;
            }

            return goTo;
        }

        /// <summary>
        /// Identify a route and retrieve the target fief
        /// </summary>
        /// <returns>Fief to move to (or null)</returns>
        /// <param name="f">Current location of NPC</param>
        /// <param name="f">Direction to move (route tag)</param>
        public Fief GetFief(Fief f, string direction)
        {
            Fief myFief = null;

            // check for correct direction codes
            string[] correctDirections = new string[6] { "E", "W", "SE", "SW", "NE", "NW" };
            bool dirCorrect = false;
            foreach (string correctDir in correctDirections)
            {
                if (direction.ToUpper().Equals(correctDir))
                {
                    dirCorrect = true;
                    break;
                }
            }

            // iterate through edges
            if (dirCorrect)
            {
                foreach (var e in this.myMap.Edges)
                {
                    // if matching source, check tag
                    if (e.Source == f)
                    {
                        // if matching tag, get target
                        if (e.Tag.Equals(direction))
                        {
                            myFief = e.Target;
                            break;
                        }
                    }
                }
            }

            return myFief;

        }

        /// <summary>
        /// Identify the shortest path between 2 hexes (Fiefs)
        /// </summary>
        /// <returns>Queue of Fiefs to move to</returns>
        /// <param name="from">Source Fief</param>
        /// <param name="to">Target Fief</param>
        public Queue<Fief> GetShortestPath(Fief @from, Fief to)
        {
            Queue<Fief> pathNodes = new Queue<Fief>();
            var edgeCost = AlgorithmExtensions.GetIndexer(costs);
            // get shortest route using Dijkstra algorithm
            var tryGetPath = myMap.ShortestPathsDijkstra(edgeCost, @from);

            IEnumerable<TaggedEdge<Fief, string>> path;
            // iterate through resulting routes (edges)
            if (tryGetPath(to, out path))
            {
                // extract target Fiefs and add to queue
                foreach (var e in path)
                {
                    pathNodes.Enqueue(e.Target);
                }
            }

            return pathNodes;
        }

        /// <summary>
        /// 'Helper' method to identify the shortest path between 2 hexes (Fiefs),
        /// then to convert path into a string for visual display
        /// </summary>
        /// <returns>String to display</returns>
        /// <param name="from">Source Fief</param>
        /// <param name="to">Target Fief</param>
        public string GetShortestPathString(Fief @from, Fief to)
        {
            string output = "";
            var edgeCost = AlgorithmExtensions.GetIndexer(costs);
            var tryGetPath = myMap.ShortestPathsDijkstra(edgeCost, @from);

            IEnumerable<TaggedEdge<Fief, string>> path;
            if (tryGetPath(to, out path))
            {
                output = PrintPath(@from, to, path);
            }
            else
            {
                output = "No path found from " + @from.id + " to " + to.id;
            }
            return output;
        }

        /// <summary>
        /// 'Helper' method allowing shortest path to be converted to text format.
        /// Used by getShortestPathString method
        /// </summary>
        /// <returns>String to display</returns>
        /// <param name="from">Source Fief</param>
        /// <param name="to">Target Fief</param>
        /// <param name="path">Collection containing path routes (edges)</param>
        private static string PrintPath(Fief @from, Fief to, IEnumerable<TaggedEdge<Fief, string>> path)
        {
            string output = "";
            output += "Path found from " + @from.id + " to " + to.id + "is\r\n";
            foreach (var e in path)
            {
                output += e.Tag + " to (" + e.Target.id + ") ";
            }
            return output;
        }
    }

}
