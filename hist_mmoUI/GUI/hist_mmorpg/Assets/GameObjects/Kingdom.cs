using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
/// <summary>
/// Class storing data on kingdom
/// </summary>
[Serializable()]
public class Kingdom : Place, ISerializable
{
	/// <summary>
	/// Holds Kingdom nationality
	/// </summary>
	public Nationality nationality { get; set; }
	
	/// <summary>
	/// Constructor for Kingdom
	/// </summary>
	/// <param name="nat">Kingdom's Nationality object</param>
	public Kingdom(String id, String nam, Nationality nat, Rank r = null)
		: base(id, nam, r)
	{
		this.nationality = nat;
	}
	
	/// <summary>
	/// Constructor for Kingdom taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Kingdom()
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("nat", this.nationality.natID, typeof(string));
	}
	
	public Kingdom(SerializationInfo info, StreamingContext context):base(info,context)
	{
		var tmpNat = info.GetString("nat");
		this.nationality = Globals_Game.nationalityMasterList[tmpNat];
		Globals_Game.kingdomMasterList.Add (this.id, this);
	}
}

