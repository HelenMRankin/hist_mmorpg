using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
/// <summary>
/// Class storing data on terrain
/// </summary>
[Serializable()]
public class GameTerrain : ISerializable
{
	/// <summary>
	/// Holds terrain ID
	/// </summary>
	public String id { get; set; }
	/// <summary>
	/// Holds terrain description
	/// </summary>
	public String description { get; set; }
	/// <summary>
	/// Holds terrain travel cost
	/// </summary>
	public double travelCost { get; set; }
	
	/// <summary>
	/// Constructor for GameTerrain
	/// </summary>
	/// <param name="id">String holding terrain code</param>
	/// <param name="desc">String holding terrain description</param>
	/// <param name="tc">double holding terrain travel cost</param>
	public GameTerrain(String id, string desc, double tc)
	{
		// VALIDATION
		
		// ID
		// trim
		id = id.Trim();
		
		this.id = id;
		this.description = desc;
		this.travelCost = tc;
	}
	
	/// <summary>
	/// Constructor for GameTerrain taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public GameTerrain()
	{
	}
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("id", this.id, typeof(string));
		info.AddValue("des", this.description, typeof(string));
		info.AddValue("cost", this.travelCost, typeof(double));
	}
	
	public GameTerrain(SerializationInfo info, StreamingContext context)
	{
		this.id = info.GetString("id");
		this.description = info.GetString("des");
		this.travelCost = info.GetDouble("cost");
		Globals_Game.terrainMasterList.Add (this.id, this);
	}
}