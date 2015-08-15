using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ProtoBuf;
using UnityEngine;
/// <summary>
/// Serializes server-side game objects into client-side game objects
/// </summary>
public class ClientSerializer
{
	String path = "G:/Unity/hist_mmoUI/GUI/hist_mmorpg/Assets/DataFiles/";
	
	public void SerializeRanks()
	{
		FileStream outStream = new FileStream(path + "Ranks.bin", FileMode.Create);
		Rank[] ranks = Globals_Game.rankMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream,ranks);
		outStream.Close();
	}
	
	public void SerializeNationalities()
	{
		FileStream outStream = new FileStream(path + "Nationalities.bin", FileMode.Create);
		Nationality[] nats = Globals_Game.nationalityMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, nats);
		outStream.Close();
	}
	
	public void SerializeBaseLangs()
	{
		FileStream outStream = new FileStream(path + "BaseLanguages.bin", FileMode.Create);
		BaseLanguage[] baseLangs = Globals_Game.baseLanguageMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, baseLangs);
		outStream.Close();
	}
	
	public void SerializeGameTerrains()
	{
		FileStream outStream = new FileStream(path + "GameTerrains.bin", FileMode.Create);
		GameTerrain[] terrs = Globals_Game.terrainMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, terrs);
		outStream.Close();
	}
	
	public void SerializeLangs()
	{
		FileStream outStream = new FileStream(path + "Languages.bin", FileMode.Create);
		Language[] langs = Globals_Game.languageMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, langs);
		outStream.Close();
	}
	
	public void SerializeKingdoms()
	{
		FileStream outStream = new FileStream(path + "Kingdoms.bin", FileMode.Create);
		Kingdom[] kingdoms = Globals_Game.kingdomMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, kingdoms);
		outStream.Close();
	}
	
	public void SerializeProvinces()
	{
		FileStream outStream = new FileStream(path + "Provinces.bin", FileMode.Create);
		Province[] provinces = Globals_Game.provinceMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, provinces);
		outStream.Close();
	}
	
	public void SerializeFiefs()
	{
		FileStream outStream = new FileStream(path + "Fiefs.bin", FileMode.Create);
		Fief[] fiefs = Globals_Game.fiefMasterList.Values.ToArray();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(outStream, fiefs);
		outStream.Close();
	}
	
	public void SerializeAll()
	{
		SerializeRanks();
		SerializeNationalities();
		SerializeBaseLangs();
		SerializeGameTerrains();
		SerializeLangs();
		SerializeKingdoms();
		SerializeProvinces();
		SerializeFiefs();
	}
	
	public void DeserializeRanks()
	{
		ClientSerializationItems.ClientRank[] ranks;
		using (var file = File.OpenRead(path+"Ranks.bin"))
		{
			ranks = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientRank[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientRank rank in ranks) {
			Rank r = new Rank();
			r.id=rank.id;
			r.stature=rank.stature;
			r.title=rank.titles;
			Globals_Game.rankMasterList.Add (r.id,r);
			Debug.Log ("Added rank: "+r.id+"\n");
		}
		Debug.Log ("Completed rank deserialization\n");
	}
	public void DeserializeNationalities()
	{
		ClientSerializationItems.ClientNationality[] nats;
		using (var file = File.OpenRead(path+"Nationalities.bin"))
		{
			nats = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientNationality[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientNationality nat in nats) {
			Nationality n = new Nationality();
			n.natID=nat.natID;
			n.name=nat.natName;
			Globals_Game.nationalityMasterList.Add (n.natID,n);
			Debug.Log ("Added nationality: "+n.name+"\n");
		}
		Debug.Log ("Completed nationality deserialization \n");
	}
	public void DeserializeBaseLangs()
	{
		ClientSerializationItems.ClientBaseLanguage[] baselangs;
		using (var file = File.OpenRead(path+"BaseLanguages.bin"))
		{
			baselangs = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientBaseLanguage[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientBaseLanguage baselang in baselangs) {
			BaseLanguage bl = new BaseLanguage();
			bl.id=baselang.id;
			bl.name=baselang.name;
			Globals_Game.baseLanguageMasterList.Add (bl.id,bl);
			Debug.Log ("Added baselang: "+bl.name+"\n");
		}
		Debug.Log ("Completed BaseLang deserialization\n");
	}
	
	public void DeserializeGameTerrains()
	{
		ClientSerializationItems.ClientTerrain[] terrs;
		using (var file = File.OpenRead(path+"Terrains.bin"))
		{
			terrs = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientTerrain[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientTerrain terr in terrs) {
			GameTerrain t = new GameTerrain();
			t.id=terr.id;
			t.description=terr.des;
			t.travelCost=terr.cost;
			Globals_Game.terrainMasterList.Add (t.id,t);
			Debug.Log ("Added terrain: "+t.description+"\n");
		}
		Debug.Log ("Completed terrain deserialization\n");
	}
	
	public void DeserializeLanguages()
	{
		ClientSerializationItems.ClientLanguage[] langs;
		using (var file = File.OpenRead(path+"Languages.bin"))
		{
			langs = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientLanguage[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientLanguage lang in langs) {
			Language l = new Language();
			l.id=lang.id;
			l.dialect=lang.dia;
			l.baseLanguage=Globals_Game.baseLanguageMasterList[lang.baselang];
			Globals_Game.languageMasterList.Add (l.id,l);
			Debug.Log ("Added language: "+l.id+"\n");
		}
		Debug.Log ("Completed language deserialization \n");
	}
	
	public void DeserializeKingdoms()
	{
		ClientSerializationItems.ClientKingdom[] kingdoms;
		using (var file = File.OpenRead(path+"Kingdoms.bin"))
		{
			kingdoms = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientKingdom[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientKingdom kingdom in kingdoms) {
			Nationality nat = Globals_Game.nationalityMasterList[kingdom.nat];
			Rank r = null;
			Globals_Game.rankMasterList.TryGetValue (kingdom.rank,out r);
			Kingdom k = new Kingdom(kingdom.id,kingdom.name,nat,r);
			Globals_Game.kingdomMasterList.Add (k.id,k);
			Debug.Log ("Added kingdom: "+k.name+"\n");
		}
		Debug.Log ("Completed kingdom deserialization\n");
	}
	
	public void DeserializeProvinces()
	{
		ClientSerializationItems.ClientProvince[] provinces;
		using (var file = File.OpenRead(path+"Provinces.bin"))
		{
			provinces = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientProvince[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientProvince prov in provinces) {
			Kingdom k = null;
			Rank r = null;
			Globals_Game.kingdomMasterList.TryGetValue (prov.kingdom, out k);
			Globals_Game.rankMasterList.TryGetValue (prov.rank,out r);
			Province p = new Province(prov.id,prov.name,k,r);
			Globals_Game.provinceMasterList.Add (p.id,p);
			Debug.Log ("Added province: "+p.name+"\n");
		}
		Debug.Log ("Completed province deserialization\n");
	}
	
	public void DeserializeFiefs()
	{
		ClientSerializationItems.ClientFief[] fiefs;
		using (var file = File.OpenRead(path+"Fiefs.bin"))
		{
			fiefs = Serializer.DeserializeWithLengthPrefix<ClientSerializationItems.ClientFief[]>(file, ProtoBuf.PrefixStyle.Fixed32);
		}
		foreach(ClientSerializationItems.ClientFief fief in fiefs) {
			GameTerrain terrain;
			Language lang;
			Province prov;
			Rank rank;
			Globals_Game.terrainMasterList.TryGetValue (fief.terrain,out terrain);
			Globals_Game.languageMasterList.TryGetValue (fief.lang,out lang);
			Globals_Game.provinceMasterList.TryGetValue (fief.province,out prov);
			Globals_Game.rankMasterList.TryGetValue (fief.rank,out rank);
			Fief f = new Fief(fief.id,fief.name,rank,prov,lang,terrain);
			Globals_Game.fiefMasterList.Add (f.id,f);
			Debug.Log ("Added fief: "+f.name+"\n");
		}
		Debug.Log ("Completed fief deserialization");
	}
	
	public void DeserializeAll()
	{
		DeserializeRanks();
		DeserializeNationalities();
		DeserializeBaseLangs();
		DeserializeGameTerrains();
		DeserializeLanguages();
		DeserializeKingdoms();
		DeserializeProvinces();
		DeserializeFiefs();
	}
}
