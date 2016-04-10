using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using ProtoBuf;
namespace hist_mmorpg
{
    /// <summary>
    /// Serializes several server-side objects into a a format more appropriate for client-side
	/// Any data which does not change and will not give players an unfair advantage should be serialized and sent to the client in order to reduce the amount of data transferred over network
    /// </summary>
    public class ClientSerializer
    {
		
        String path = "F:/CloneBackup/Unity/GUI/hist_mmorpg/Assets/ClientObjects/";
        
		public ClientSerializer() {
			path = Directory.GetCurrentDirectory ();
		}
        public void SerializeRanks()
        {
            List<ClientRank> ranks = new List<ClientRank>();
            foreach (Rank r in Globals_Game.rankMasterList.Values)
            {
                ranks.Add(new ClientRank(r));
            }
            ClientRank[] rankarray = ranks.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientRank[]>(File.Create(path+"Ranks.bin"), rankarray, ProtoBuf.PrefixStyle.Fixed32);
        }

        public void SerializeNationalities()
        {
            List<ClientNationality> nats = new List<ClientNationality>();
            foreach (Nationality n in Globals_Game.nationalityMasterList.Values)
            {
                nats.Add(new ClientNationality(n));
            }
            ClientNationality[] natArray = nats.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientNationality[]>(File.Create(path+"Nationalities.bin"), natArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeBaseLangs()
        {
            List<ClientBaseLanguage> baseLangs = new List<ClientBaseLanguage>();
            foreach (BaseLanguage bl in Globals_Game.baseLanguageMasterList.Values)
            {
                baseLangs.Add(new ClientBaseLanguage(bl));
            }
            ClientBaseLanguage[] blArray = baseLangs.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientBaseLanguage[]>(File.Create(path+"BaseLanguages.bin"), blArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeTerrains()
        {
            List<ClientTerrain> terrains = new List<ClientTerrain>();
            foreach (Terrain t in Globals_Game.terrainMasterList.Values)
            {
                terrains.Add(new ClientTerrain(t));
            }
            ClientTerrain[] terrainArray = terrains.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientTerrain[]>(File.Create(path + "Terrains.bin"), terrainArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeLangs()
        {
            List<ClientLanguage> langs = new List<ClientLanguage>();
            foreach (Language l in Globals_Game.languageMasterList.Values)
            {
                langs.Add(new ClientLanguage(l));
            }
            ClientLanguage[] langArray = langs.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientLanguage[]>(File.Create(path + "Languages.bin"), langArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeKingdoms()
        {
            List<ClientKingdom> kingdoms = new List<ClientKingdom>();
            foreach (Kingdom k in Globals_Game.kingdomMasterList.Values)
            {
                kingdoms.Add(new ClientKingdom(k));
            }
            ClientKingdom[] kingArray = kingdoms.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientKingdom[]>(File.Create(path + "Kingdoms.bin"), kingArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeProvinces()
        {
            List<ClientProvince> provinces = new List<ClientProvince>();
            foreach (Province p in Globals_Game.provinceMasterList.Values)
            {
                provinces.Add(new ClientProvince(p));
            }
            ClientProvince[] provArray = provinces.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientProvince[]>(File.Create(path + "Provinces.bin"), provArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeFiefs()
        {
            List<ClientFief> fiefs = new List<ClientFief>();
            foreach (Fief f in Globals_Game.fiefMasterList.Values)
            {
                fiefs.Add(new ClientFief(f));
            }
            ClientFief[] fiefArray = fiefs.ToArray();
            Serializer.SerializeWithLengthPrefix<ClientFief[]>(File.Create(path + "Fiefs.bin"), fiefArray, ProtoBuf.PrefixStyle.Fixed32);
        
        }

        public void SerializeAll()
        {
			path = Directory.GetCurrentDirectory ();
            SerializeRanks();
            SerializeNationalities();
            SerializeBaseLangs();
            SerializeTerrains();
            SerializeLangs();
            SerializeKingdoms();
            SerializeProvinces();
            SerializeFiefs();
        }

        public Rank[] DeserializeRanks()
        {
            FileStream inStream = new FileStream(path + "Ranks.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Rank[]));
            Rank[] newRanks = (Rank[])bf.Deserialize(inStream);
            return newRanks;
        }
        public Nationality[] DeserializeNationalities()
        {
            FileStream inStream = new FileStream(path + "Nationalities.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Nationality[]));
            Nationality[] newNats = (Nationality[])bf.Deserialize(inStream);
            return newNats;
        }
        public BaseLanguage[] DeserializeBaseLangs()
        {
            FileStream inStream = new FileStream(path + "BaseLanguages.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(BaseLanguage[]));
            BaseLanguage[] newBaseLangs = (BaseLanguage[])bf.Deserialize(inStream);
            return newBaseLangs;
        }

        public Terrain[] DeserializeTerrains()
        {
            FileStream inStream = new FileStream(path + "Terrains.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Terrain[]));
            Terrain[] newTerrs = (Terrain[])bf.Deserialize(inStream);
            return newTerrs;
        }

        public Language[] DeserializeLanguages()
        {
            FileStream inStream = new FileStream(path + "Languages.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Language[]));
            Language[] newLangs = (Language[])bf.Deserialize(inStream);
            return newLangs;
        }

        public Kingdom[] DeserializeKingdoms()
        {
            FileStream inStream = new FileStream(path + "Kingdoms.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Kingdom[]));
            Kingdom[] newKings = (Kingdom[])bf.Deserialize(inStream);
            return newKings;
        }

        public Province[] DeserializeProvinces()
        {
            FileStream inStream = new FileStream(path + "Provinces.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Province[]));
            Province[] newProvs = (Province[])bf.Deserialize(inStream);
            return newProvs;
        }

        public Fief[] DeserializeFiefs()
        {
            FileStream inStream = new FileStream(path + "Fiefs.bin", FileMode.Open);
            XmlSerializer bf = new XmlSerializer(typeof(Fief[]));
            Fief[] newFiefs = (Fief[])bf.Deserialize(inStream);
            return newFiefs;
        }

        public void DeserializeAll()
        {
            DeserializeRanks();
            DeserializeNationalities();
            DeserializeBaseLangs();
            DeserializeTerrains();
            DeserializeLanguages();
            DeserializeKingdoms();
            DeserializeProvinces();
            DeserializeFiefs();
        }
    }
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientFief {
		public string id { get; set; }
		public string name { get; set; }
		public byte rank { get; set; }
		public string terrain { get; set; }
		public string province { get; set; }
		public string lang { get; set; }
		public ClientFief()
		{

		}

		public ClientFief(Fief f)
		{
			this.id = f.id;
			this.name = f.name;
			this.rank = f.rank.id;
			this.terrain = f.terrain.id;
			this.province = f.province.id;
			this.lang = f.language.id;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientTerrain
	{
		public string id { get; set; }
		public string des { get; set; }
		public double cost { get; set; }

		public ClientTerrain()
		{

		}

		public ClientTerrain(Terrain t)
		{
			this.id = t.id;
			this.des = t.description;
			this.cost = t.travelCost;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientProvince
	{
		public string id { get; set; }
		public string name { get; set; }
		public byte rank { get; set; }
		public string kingdom { get; set; }
		public ClientProvince()
		{

		}

		public ClientProvince(Province p)
		{
			this.id = p.id;
			this.name = p.name;
			this.rank = p.rank.id;
			this.kingdom = p.kingdom.id;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientKingdom
	{
		public string id { get; set; }
		public string name { get; set; }
		public byte rank { get; set; }
		public string nat { get; set; }
		public ClientKingdom()
		{

		}
		public ClientKingdom(Kingdom k)
		{
			this.id = k.id;
			this.name = k.name;
			this.rank = k.rank.id;
			this.nat = k.nationality.natID;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientNationality
	{
		public string natID { get; set; }
		public string natName { get; set; }
		public ClientNationality()
		{

		}

		public ClientNationality(Nationality n)
		{
			this.natID = n.natID ;
			this.natName = n.name;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientLanguage
	{
		public string id { get; set; }
		public string baselang { get; set; }
		public int dia { get; set; }
		public ClientLanguage()
		{

		}

		public ClientLanguage(Language l)
		{
			this.id = l.id;
			this.baselang = l.baseLanguage.id;
			this.dia = l.dialect;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientBaseLanguage
	{
		public string id { get; set; }
		public string name { get; set; }
		public ClientBaseLanguage()
		{

		}

		public ClientBaseLanguage(BaseLanguage b)
		{
			this.id = b.id;
			this.name = b.name;
		}
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientRank
	{
		public byte id { get; set; }
		public TitleName[] titles { get; set; }
		public byte stature { get; set; }
		public ClientRank()
		{

		}

		public ClientRank(Rank r)
		{
			this.id = r.id;
			this.titles = r.title;
			this.stature = r.stature;
		}
	}
}
