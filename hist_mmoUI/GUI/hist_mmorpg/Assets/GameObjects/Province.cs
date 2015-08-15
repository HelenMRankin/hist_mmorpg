using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
/// <summary>
/// Class storing data on province
/// </summary>
[Serializable()]
public class Province : Place, ISerializable
{

	/// <summary>
	/// Holds province kingdom object
	/// </summary>
	public Kingdom kingdom { get; set; }
	
	/// <summary>
	/// Constructor for Province
	/// </summary>
	/// <param name="otax">Double holding province tax rate</param>
	/// <param name="king">Province's Kingdom object</param>
	public Province(String id, String nam, Kingdom king = null, Rank r = null)
		: base(id, nam, r)
	{
		this.kingdom = king;
	}
	
	/// <summary>
	/// Constructor for Province taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Province()
	{
	}

	//temp for serializing to Client side Fief object
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("king", this.kingdom.id, typeof(string));
	}
	
	public Province(SerializationInfo info, StreamingContext context): base(info,context)
	{
		var tmpKing = info.GetString("king");
		this.kingdom = Globals_Game.kingdomMasterList[tmpKing];
		Globals_Game.provinceMasterList.Add (this.id, this);
	}
	
}
