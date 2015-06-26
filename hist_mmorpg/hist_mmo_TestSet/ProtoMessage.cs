using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace hist_mmo_TestSet
{
    [ProtoContract]
    [ProtoInclude (3,typeof(ProtoTest))]
    public class ProtoMessage
    {
        [ProtoMember(1)]
        public string type { get; set; }
    }

    [ProtoContract]
    public class ProtoTest : ProtoMessage
    {
        [ProtoMember(1)]
        public string _name2 { get; set; }

        [ProtoMember(2)]
        public int _age2 { get; set; }

        [ProtoMember(3)]
        public string test { get; set; }

        public ProtoTest(string name, int age, string test)
        {
            this.type = "test";
            this._name2 = name;
            this._age2 = age;
            this.test = test;
        }
        public ProtoTest()
        {

        }
    }
}
