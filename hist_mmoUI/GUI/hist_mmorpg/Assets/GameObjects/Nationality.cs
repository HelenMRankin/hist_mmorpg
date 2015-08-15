﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
/// <summary>
/// Class storing data on nationality
/// </summary>
[Serializable()]
public class Nationality : ISerializable
{
	/// <summary>
	/// Holds nationality ID
	/// </summary>
	public String natID { get; set; }
	/// <summary>
	/// Holds nationality name
	/// </summary>
	public String name { get; set; }
	
	/// <summary>
	/// Constructor for Nationality
	/// </summary>
	/// <param name="id">String holding nationality ID</param>
	/// <param name="nam">String holding nationality name</param>
	public Nationality(String id, String nam)
	{
		
		this.natID = id;
		this.name = nam;
		
	}
	
	/// <summary>
	/// Constructor for Nationality taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Nationality()
	{
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("natID", this.natID, typeof(string));
		info.AddValue("natName", this.name, typeof(string));
	}
	
	public Nationality(SerializationInfo info, StreamingContext context)
	{
		this.natID = info.GetString("natID");
		this.name = info.GetString("natName");
		Globals_Game.nationalityMasterList.Add (this.natID, this);
	}
}