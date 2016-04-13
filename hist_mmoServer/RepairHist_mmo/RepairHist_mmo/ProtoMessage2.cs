using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;
using System.Windows.Forms.VisualStyles;
using RiakClient.Commands;

namespace hist_mmorpg
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(10, typeof(ProtoMessage2<string[]>))]
    public class ProtoMessageBase
    {
        /// <summary>
        /// The Action to be carried out
        /// </summary>
        public Actions Action { get; set; }
        /// <summary>
        /// The response code
        /// </summary>
        public DisplayMessages Response { get; set; }
        /// <summary>
        /// Type of payload received- used for successful deserialisation
        /// </summary>
        public string TypeName { get; set; }
        public ProtoMessageBase()
        {
            
        }
        public ProtoMessageBase(Actions A, DisplayMessages R)
        {
            this.Action = A;
            this.Response = R;
        }
        public byte[] SerialiseMessage()
        {
            MemoryStream ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix<ProtoMessageBase>(ms, this, PrefixStyle.Fixed32);
            return ms.GetBuffer();
        }

        public static ProtoMessage2<object> DeserialiseMessage(MemoryStream ms)
        {
            ProtoMessage2<object> m = Serializer.DeserializeWithLengthPrefix<ProtoMessage2<object>>(ms, PrefixStyle.Fixed32);
            return m;
        }

        public System.Type GetType()
        {
            try
            {
                Console.WriteLine("Type name: "+TypeName);
                Type t = Type.GetType(TypeName);
                return t;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get type: "+e.ToString());
                return null;
            }
            
        }
    }
    /// <summary>
    /// Will become ProtoMessage when finalized
    /// Used for communication between client and server
    /// Consists of more traditional header and payload
    /// 
    /// Strategy for use: Serialize and de-serialize with length prefix
    /// Deserialise with type object, then use the type provided in the header to properly cast/deserialize the payload
    /// Enables much less confusing communication, as well as flexibility
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ProtoMessage2<E> : ProtoMessageBase
    {
       
        /// <summary>
        /// The payload, or main contents of the message
        /// </summary>
        public E payload { get; set; }

        /// <summary>
        /// Construct a new ProtoMessage
        /// </summary>
        /// <param name="a">Action type</param>
        /// <param name="r">Response type</param>
        /// <param name="p">Payload (TypeName is set automatically)</param>
        public ProtoMessage2(Actions a, DisplayMessages r, E p) : base(a,r)
        {
            // Set the type of the message to the payload- this makes deserialization possible
            TypeName = typeof(E).FullName;
            Action = a;
            Response = r;
            payload = p;
        }

        public ProtoMessage2() : base()
        {
        } 


        

        public E GetPayload()
        {
            return this.payload;
        }
    }
}
