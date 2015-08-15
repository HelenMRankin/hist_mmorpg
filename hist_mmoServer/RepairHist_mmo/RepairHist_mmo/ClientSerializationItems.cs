using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace hist_mmorpg
{
    
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
