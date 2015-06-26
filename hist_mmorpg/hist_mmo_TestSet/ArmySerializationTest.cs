using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using ProtoBuf;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
namespace hist_mmo_TestSet
{
    [TestClass]
    public class ArmySerializationTest
    {
        
        [TestMethod]
        public void SerializeTest()
        {
            ProtoTest m = new ProtoTest("A test",24, "another test");
            m._age2 = 14;
            Serializer.SerializeWithLengthPrefix<ProtoTest>(File.Create("noName.bin"), m, ProtoBuf.PrefixStyle.Fixed32);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            ProtoMessage test;
            using (var file = File.OpenRead("noName.bin"))
            {
                test = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(file, ProtoBuf.PrefixStyle.Fixed32);
                if (test == null) return;
                if (test.type.Equals("test"))
                {
                    Trace.WriteLine("Is prototest!");
                    ProtoTest message = (ProtoTest)test;
                    Trace.WriteLine(message._name2);
                }
                else
                {
                    Trace.WriteLine("failed: type = " + test.type);
                }
            }
            
        }
        [TestMethod]
        public void StringReplace()
        {
            String start = "Want to insert name here: {0} and age here: {1}";
            string param1 = "Helen";
            string param2 = "21";
            string result = string.Format(start, param1, param2);
            Trace.Write(result);
        }

        [TestMethod]
        public void ArmyDetails()
        {
            Game g = new Game();
            PlayerCharacter pc1 = Globals_Game.pcMasterList["Char_158"];
            PlayerCharacter pc2 = Globals_Game.pcMasterList["Char_40"];
            Trace.WriteLine("Starting to iterate through armies");
            if (pc1 != null)
            {
                Trace.WriteLine("PlayerCharacter:" +pc1.firstName + " "+pc1.familyName);
            }
            foreach(Army myArmy in pc1.myArmies) {
                Trace.WriteLine(myArmy.DisplayArmyData(pc1));
                Trace.WriteLine(myArmy.DisplayArmyData(pc2));
                hist_mmorpg.ProtoMessage proto = new hist_mmorpg.ProtoMessage.ProtoArmy(myArmy, pc1);
                hist_mmorpg.ProtoMessage proto2 = new hist_mmorpg.ProtoMessage.ProtoArmy(myArmy, pc2);
                var file = File.Create("armyTest.bin");
                var file2 = File.Create("armyTest2.bin");
                Serializer.Serialize<hist_mmorpg.ProtoMessage>(file, proto);
                Serializer.Serialize<hist_mmorpg.ProtoMessage>(file2, proto2);
                file.Close();
                file2.Close();

            }
            hist_mmorpg.ProtoMessage test = Serializer.Deserialize<hist_mmorpg.ProtoMessage>(File.OpenRead("armyTest.bin"));

            hist_mmorpg.ProtoMessage test2 = Serializer.Deserialize<hist_mmorpg.ProtoMessage>(File.OpenRead("armyTest2.bin"));
            if (test != null)
            {
                Trace.WriteLine("########NotNull#####");
                Trace.WriteLine(test.getMsgType());
                if (test.getMsgType().Equals("Army"))
                {
                    Trace.WriteLine("########is army#####");
                    hist_mmorpg.ProtoMessage.ProtoArmy armyMsg = (hist_mmorpg.ProtoMessage.ProtoArmy)test;
                    Trace.WriteLine(armyMsg._owner);
                    Trace.WriteLine(armyMsg._leader);
                    Trace.WriteLine(armyMsg._days);
                }
            }
            if (test2 != null)
            {
                Trace.WriteLine("########NotNull#####");
                Trace.WriteLine(test2.getMsgType());
                if (test2.getMsgType().Equals("Army"))
                {
                    Trace.WriteLine("########is army#####");
                    hist_mmorpg.ProtoMessage.ProtoArmy armyMsg2 = (hist_mmorpg.ProtoMessage.ProtoArmy)test2;
                    Trace.WriteLine(armyMsg2._owner);
                    Trace.WriteLine(armyMsg2._leader);
                    Trace.WriteLine(armyMsg2._days);
                }
            }

        }
    }
}
