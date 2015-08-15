using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;
/// <summary>
/// Class storing data on rank and title
/// </summary>
[Serializable()]
public class Rank : ISerializable
{
	/// <summary>
	/// Holds rank ID
	/// </summary>
	public byte id { get; set; }
	/// <summary>
	/// Holds title name in various languages
	/// </summary>
	public TitleName[] title { get; set; }
	/// <summary>
	/// Holds base stature for this rank
	/// </summary>
	public byte stature { get; set; }
	
	/// <summary>
	/// Constructor for Rank
	/// </summary>
	/// <param name="id">byte holding rank ID</param>
	/// <param name="ti">TitleName[] holding title name in various languages</param>
	/// <param name="stat">byte holding base stature for rank</param>
	public Rank(byte id, TitleName[] ti, byte stat)
	{
		// VALIDATION
		
		// STATURE
		if (stat < 1)
		{
			stat = 1;
		}
		else if (stat > 6)
		{
			stat = 6;
		}
		
		this.id = id;
		this.title = ti;
		this.stature = stat;
		
	}
	
	/// <summary>
	/// Constructor for Rank taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Rank()
	{
	}
	
	/// <summary>
	/// Gets the correct name for the rank depending on the specified Language
	/// </summary>
	/// <returns>string containing the name</returns>
	/// <param name="l">The Language to be used</param>
	public string GetName(Language l)
	{
		string rankName = null;
		bool nameFound = false;
		
		// iterate through TitleNames and get correct name
		foreach (TitleName titleName in this.title)
		{
			if (titleName.langID == l.id)
			{
				rankName = titleName.name;
				nameFound = true;
				break;
			}
		}
		
		// if no name found for specified language
		if (!nameFound)
		{
			// iterate through TitleNames and get generic name
			foreach (TitleName titleName in this.title)
			{
				if ((titleName.langID.Equals("generic")) || (titleName.langID.Contains("lang_E")))
				{
					rankName = titleName.name;
					nameFound = true;
					break;
				}
			}
		}
		
		// if still no name found
		if (!nameFound)
		{
			// get first name
			rankName = this.title[0].name;
		}
		
		return rankName;
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("id", this.id, typeof(byte));
		info.AddValue("titles", this.title, typeof(TitleName[]));
		info.AddValue("stat", this.stature, typeof(byte));
	}
	
	public Rank(SerializationInfo info, StreamingContext context)
	{
		this.id = info.GetByte("id");
		this.title = (TitleName[])info.GetValue("titles", typeof(TitleName[]));
		this.stature = info.GetByte("stat");
		Globals_Game.rankMasterList.Add (this.id, this);
	}
	
}

/// <summary>
/// Class storing data on positions of power
/// </summary>
[Serializable()]
public class Position : Rank, ISerializable
{
	/// <summary>
	/// Holds nationality associated with the position
	/// </summary>
	public Nationality nationality { get; set; }
	
	/// <summary>
	/// Constructor for Position
	/// </summary>
	/// <param name="holder">string holding ID of the office holder</param>
	/// <param name="nat">Nationality associated with the position</param>
	public Position(byte id, TitleName[] ti, byte stat, Nationality nat)
		: base(id, ti, stat)
	{
		this.nationality = nat;
	}
	
	/// <summary>
	/// Constructor for Position taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Position()
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("PosNat", this.nationality.natID, typeof(string));
	}
	public Position(SerializationInfo info, StreamingContext context) :base(info,context)
	{
		var tmpNat = info.GetString("PosNat");
		this.nationality = Globals_Game.nationalityMasterList[tmpNat];
		Globals_Game.positionMasterList.Add (this.id, this);
	}
}

/// <summary>
/// Struct storing data on title name
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[Serializable()]

public struct TitleName : ISerializable
{
	/// <summary>
	/// Holds Language ID or "generic"
	/// </summary>
	public string langID;
	/// <summary>
	/// Holds title name associated with specific language
	/// </summary>
	public string name;
	
	/// <summary>
	/// Constructor for TitleName
	/// </summary>
	/// <param name="lang">string holding Language ID</param>
	/// <param name="nam">string holding title name associated with specific language</param>
	public TitleName(string lang, string nam)
	{
		this.langID = lang;
		this.name = nam;
	}
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		// Use the AddValue method to specify serialized values.
		info.AddValue("TiLang", this.langID, typeof(string));
		info.AddValue("TiName", this.name, typeof(string));
	}
	
	public TitleName(SerializationInfo info, StreamingContext context)
	{
		this.langID = info.GetString("TiLang");
		this.name = info.GetString("TiName");
	}
}
