using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Diagnostics;


using System.Linq;

namespace hist_mmo_TestSet
{
    /// <summary>
    /// Summary description for ActionsTest
    /// </summary>
    [TestClass]
    public class ActionsTest
    {
        public static PlayerCharacter sysAdmin { get; set; }
        public static PlayerCharacter king1 { get; set; }
        public static PlayerCharacter king2 { get; set; }
        public static PlayerCharacter player { get; set; }
        public static NonPlayerCharacter ownedNPC { get; set; }
        public static NonPlayerCharacter notOwnedNPC { get; set; }
        public static Fief ownedFief { get; set; }
        public static Fief notOwnedFief { get; set; }
        public ActionsTest()
        {
            
        }
        [AssemblyInitialize()]
        public static void Initialize(TestContext context)
        {
            Game game = new Game();
            sysAdmin = Globals_Game.sysAdmin;
            king1 = Globals_Game.kingOne;
            king2 = Globals_Game.kingTwo;
            player = Globals_Game.pcMasterList["Char_256"];
            ownedNPC = Globals_Game.npcMasterList["Char_889"];
            if (ownedNPC == null)
            {
                Trace.WriteLine("NPC is null");
            }
            notOwnedNPC = Globals_Game.npcMasterList["Char_361"];
            ownedFief = Globals_Game.fiefMasterList["EWK04"];
            notOwnedFief = Globals_Game.fiefMasterList["ESU07"];
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestViewChar()
        {
            
            // id of Character owned by pc
            ProtoMessage validAuthorised = new ProtoMessage();
            validAuthorised.MessageType = hist_mmorpg.Actions.ViewChar;
            validAuthorised.Message = ownedNPC.charID;

            hist_mmorpg.ProtoMessage validUnauthorised = new hist_mmorpg.ProtoMessage();
            validUnauthorised.MessageType = hist_mmorpg.Actions.ViewChar;
            validUnauthorised.Message = notOwnedNPC.charID;

            ProtoMessage invalidChar = new ProtoMessage();
            invalidChar.MessageType=Actions.ViewChar;
            invalidChar.Message = "SPADERSH";

            ProtoMessage result1 = Game.ActionController(validAuthorised, player);
            Trace.WriteLine(result1.GetType());
            Assert.AreEqual(result1.GetType(), typeof(ProtoCharacter));
            Trace.WriteLine((result1 as ProtoCharacter).management);

            ProtoMessage result2 = Game.ActionController(invalidChar, player);
            Assert.AreEqual(DisplayMessages.ErrorGenericCharacterUnidentified, result2.MessageType);
        }

        [TestMethod]
        public void TestGetNPClist()
        {
            
            string entourageNPCs = "entourage";
            string familyNPCs = "family";
            string employees = "employ";
            string all = "entouragefamilyemploy";
            string none = "";
            ProtoMessage messageIn = new ProtoMessage();
            messageIn.MessageType = Actions.GetNPCList;
            messageIn.Message = all;

            ProtoMessage result = Game.ActionController(messageIn, player);
            Trace.WriteLine(result.GetType());

            Assert.AreEqual(typeof(ProtoGenericArray<ProtoCharacterOverview>),result.GetType());
            foreach (ProtoCharacterOverview overview in (result as ProtoGenericArray<ProtoCharacterOverview>).fields)
            {
                Trace.WriteLine(overview.charName);
            }
            List<string> charIDs = new List<string>();
            foreach (NonPlayerCharacter npc in player.myNPCs)
            {
                charIDs.Add(npc.charID);
            }
            foreach (ProtoCharacterOverview overview in (result as ProtoGenericArray<ProtoCharacterOverview>).fields)
            {
                CollectionAssert.Contains(charIDs, overview.charID);
            }
        }

        [TestMethod]
        public void TestTravelTo()
        {
            string invalidChar = "notvalid";
            string invalidFief = "notafief";
            string validFief = "EDR03";
            string[] directions = { "E", "NW" };
            ProtoTravelTo instructions = new ProtoTravelTo();
            instructions.travelTo = validFief;
            instructions.characterID = ownedNPC.charID;
            ProtoMessage result = Game.ActionController(instructions, player);
            Trace.WriteLine(result.GetType());
            Assert.AreEqual(typeof(ProtoFief), result.GetType());

            instructions.travelTo = invalidFief;
            result = Game.ActionController(instructions, player);
            Assert.AreEqual(DisplayMessages.ErrorGenericFiefUnidentified, result.MessageType);

            instructions.travelTo = validFief;
            instructions.characterID = invalidChar;
            result = Game.ActionController(instructions, player);
            Assert.AreEqual(DisplayMessages.ErrorGenericCharacterUnidentified, result.MessageType);

            instructions.characterID = notOwnedNPC.charID;
            result = Game.ActionController(instructions, player);
            Trace.WriteLine(result.GetType());
            Assert.AreEqual(result.MessageType, DisplayMessages.ErrorGenericUnauthorised);

        }

        [TestMethod]
        public void TestViewFief()
        {
            string fiefIn = player.location.id;
            ProtoMessage viewFief = new ProtoMessage();
            viewFief.MessageType = Actions.ViewFief;
            viewFief.Message = fiefIn;

            ProtoMessage result = Game.ActionController(viewFief, player);
            Assert.AreEqual(typeof(ProtoFief), result.GetType());
            if (player.ownedFiefs.Contains<Fief>(player.location))
            {
                Assert.IsNotNull((result as ProtoFief).treasury);
            }
            else
            {
                Assert.IsNull((result as ProtoFief).treasury);
            }

            viewFief.Message = ownedFief.id;
            result = Game.ActionController(viewFief, player);
            Assert.IsNotNull((result as ProtoFief).treasury);

            viewFief.Message = notOwnedFief.id;
            result = Game.ActionController(viewFief, player);
            Assert.AreEqual(DisplayMessages.ErrorGenericTooFarFromFief, result.MessageType);
        }

        [TestMethod]
        public void TestAppointBailiff()
        {
            ProtoMessage appointBailiff = new ProtoMessage();
            appointBailiff.MessageType = Actions.AppointBailiff;
            appointBailiff.Message = ownedFief.id;
            appointBailiff.MessageFields[0] = ownedNPC.charID;
            int age = ownedNPC.CalcAge();
            // Forcibly age NPC
            if (age < 14)
            {
                ownedNPC.birthDate = (player.birthDate);
            }
            ProtoMessage result = Game.ActionController(appointBailiff, player);
            Assert.AreEqual(typeof(ProtoFief),result.GetType());
            appointBailiff.Message = notOwnedFief.id;
            result = Game.ActionController(appointBailiff, player);
            Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, result.MessageType);
            appointBailiff.Message = ownedFief.id;
            appointBailiff.MessageFields[0] = notOwnedNPC.charID;

            Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, result.MessageType);
        }
        
    }
}
