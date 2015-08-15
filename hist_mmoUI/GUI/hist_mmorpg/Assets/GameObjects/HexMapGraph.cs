using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Serialization;
using System.Xml;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Class defining HexMapGraph
/// </summary>
public class HexMapGraph
{
	string mapFilePath = Application.dataPath+"/DataFiles/Graph.bin";
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
		Trace.WriteLine("The edge tag is: " + tag);
		TaggedEdge<Fief, string> myEdge = new TaggedEdge<Fief, string>(s, t, tag);
		return myEdge;
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
	/// Returns a string representing the edge tag (used in serialization
	/// </summary>
	/// <param name="edge">Edge to get string from</param>
	/// <returns></returns>
	public string getStringFromEdge(TaggedEdge<Fief, string> edge)
	{
		return edge.Tag;
	}
	
	/// <summary>
	/// Returns the Fief id (used in serialization)
	/// </summary>
	/// <param name="f">Fief to get id from</param>
	/// <returns></returns>
	public string getIdFromFief(Fief f)
	{
		return f.id;
	}
	
	/// <summary>
	/// Returns the Fief from the relative id
	/// </summary>
	/// <param name="id">Fief id</param>
	/// <returns></returns>
	public Fief getFiefFromID(String id)
	{
		return Globals_Game.fiefMasterList[id];
	}
	
	public void deserialize()
	{
		AdjacencyGraph<Fief, TaggedEdge<Fief, string>> tmpGraph = new AdjacencyGraph<Fief, TaggedEdge<Fief, string>>();
		IdentifiableVertexFactory<Fief> fiefFactory = new IdentifiableVertexFactory<Fief>(getFiefFromID);
		IdentifiableEdgeFactory<Fief, TaggedEdge<Fief, string>> edgeFactory = new IdentifiableEdgeFactory<Fief, TaggedEdge<Fief, string>>(CreateEdge);
		using (var xwriter =  XmlReader.Create(mapFilePath))
		{
			tmpGraph.DeserializeFromGraphML<Fief,TaggedEdge<Fief,string>,AdjacencyGraph<Fief, TaggedEdge<Fief, string>>>(xwriter,fiefFactory,edgeFactory);
			
		}
		this.myMap = tmpGraph;
	}
}

