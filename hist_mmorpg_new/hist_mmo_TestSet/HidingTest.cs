using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace hist_mmo_TestSet
{
    [ProtoContract]
    class HidingTest
    {
        [ProtoMember(1)]
        public string visible {get;set;}

        [ProtoMember(2)]
        public string invisible { get; set; }

        public HidingTest(string visible)
        {
            this.visible = visible;
        }

        public HidingTest()
        {

        }
    }

    [ProtoContract]
    class HiddenField : HidingTest
    {
        [ProtoMember(2)]
        string invisible;

        public HiddenField(string visible) :base(visible)
        {
            
        }

        public HiddenField(string invisible, string visible)
            : this(visible)
        {
            this.invisible = invisible;
        }

        public string getInvisible()
        {
            return invisible;
        }
    }
}
