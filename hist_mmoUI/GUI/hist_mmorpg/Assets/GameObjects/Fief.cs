using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;

[Serializable()]
public class Fief : Place, ISerializable {
	/// <summary>
	/// Holds fief's Province object
	/// </summary>
	public Province province { get; set; }
	/// <summary>
	/// Holds fief language and dialect
	/// </summary>
	public Language language { get; set; }
	/// <summary>
	/// Holds terrain object
	/// </summary>
	public GameTerrain terrain { get; set; }
	/// <summary>
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Fief(string id, string nam, Rank rank, Province prov, Language lang, GameTerrain terrain) : base(id,nam,rank) {
		this.province=prov;
		this.language=lang;
		this.terrain=terrain;
	}

	public double getTravelCost(Fief target) {
		double cost = 0;
		// calculate base travel cost based on terrain for both fiefs
		cost = (this.terrain.travelCost + target.terrain.travelCost) / 2;

		cost=cost*Globals_Client.TravelModifier;
		return cost;
	}
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		// Use the AddValue method to specify serialized values.
		info.AddValue("ter", this.terrain.id, typeof(string));
		info.AddValue("prov", this.province.id, typeof(string));
		info.AddValue("lang", this.language.id, typeof(string));
	}
	
	public Fief(SerializationInfo info, StreamingContext context) : base(info,context)
	{
		var tmpTerr = info.GetString("ter");
		var tmpProv = info.GetString("prov");
		var tmpLang = info.GetString("lang");
		this.terrain = Globals_Game.terrainMasterList[tmpTerr];
		this.province = Globals_Game.provinceMasterList[tmpProv];
		this.language = Globals_Game.languageMasterList[tmpLang];
		Globals_Game.fiefMasterList.Add (this.id, this);
	}
}
