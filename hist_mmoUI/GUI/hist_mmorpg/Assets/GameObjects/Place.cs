using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;

public abstract class Place
{
	/// <summary>
	/// Holds place ID
	/// </summary>
	public String id { get; set; }
	/// <summary>
	/// Holds place name
	/// </summary>
	public String name { get; set; }
	/// <summary>
	/// Holds place rank (Rank object)
	/// </summary>
	public Rank rank { get; set; }
	
	/// <summary>
	/// Constructor for Place
	/// </summary>
	/// <param name="id">String holding place ID</param>
	/// <param name="nam">String holding place name</param>
	/// <param name="tiHo">String holding place title holder (charID)</param>
	/// <param name="own">Place owner (PlayerCharacter)</param>
	/// <param name="rnk">Place rank (Rank object)</param>
	public Place(String id, String nam, Rank r)
	{
		this.id = id;
		this.name = nam;
		this.rank = r;
		
	}
	
	
	/// <summary>
	/// Constructor for Place taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Place()
	{
	}

	// Serialise Place for client
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		
		// Use the AddValue method to specify serialized values.
		info.AddValue("id", this.id, typeof(string));
		info.AddValue("nam", this.name, typeof(string));
		info.AddValue("rank", this.rank.id, typeof(byte));
	}
	
	public Place(SerializationInfo info, StreamingContext context)
	{
		this.id = info.GetString("id");
		this.name = info.GetString("nam");
		var tmpRank = info.GetByte("rank");
		this.rank = Globals_Game.rankMasterList[tmpRank];
	}
}