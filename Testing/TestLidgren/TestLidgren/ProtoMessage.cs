using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace TestLidgren
{
    [ProtoContract]
    public class ProtoMessage
    {
        [ProtoMember(1)]
        String name;

        [ProtoMember(2)]
        int age;
        public ProtoMessage(String s,int age)
        {
            this.name = s;
            this.age = age;
        }
        public ProtoMessage()
        {

        }
        public string getName()
        {
            return this.name;
        }
        public int getAge()
        {
            return this.age;
        }
    }
}
