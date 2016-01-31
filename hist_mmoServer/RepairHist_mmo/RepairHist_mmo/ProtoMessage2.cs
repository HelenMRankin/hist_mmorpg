using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace hist_mmorpg
{
    /// <summary>
    /// Will become ProtoMessage when finalized
    /// Used for communication between client and server
    /// Consists of more traditional header and payload
    /// 
    /// Strategy for use: Serialize and de-serialize with length prefix
    /// Deserialise with type object, then use the type provided in the header to properly cast/deserialize the payload
    /// Enables much less confusing communication, as well as flexibility
    /// </summary>
    [ProtoContract, Serializable]
    public class ProtoMessage2<E>
    {
        public Actions action;
        public DisplayMessages response;
        public Type type;
        public E payload;

        public ProtoMessage2(Actions a, DisplayMessages r, E p)
        {
            // Set the type of the message to the payload- this makes deserialization possible
            type = p.GetType();
            action = a;
            response = r;
            payload = p;
        }
    }
}
