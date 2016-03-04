// <copyright file="TestClientTest.cs">Copyright ©  2015</copyright>
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
//using Microsoft.Pex.Framework;
//using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using System.Linq;
using System.Security.Policy;

namespace hist_mmorpg.Tests
{
    /// <summary>This class contains parameterized unit tests for TestClient</summary>
  //  [PexClass(typeof(TestClient))]
  //  [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
 //   [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TestClientTest
    {

        /// <summary>
        /// The Game object for this test (contains and handles all game data)
        /// </summary>
        public static Game game;
        /// <summary>
        /// The Server object used for this test (contains connected client information
        /// </summary>
        public static Server server;
        /// <summary>
        /// The dummy Client to be used for this test
        /// </summary>
        public static TestClient client;

        public static Army OwnedArmy;
        public static Army NotOwnedArmy;
        public static Fief OwnedFief;
        public static Fief NotOwnedFief;
        // Username and Password of client who will be used during tests
        public static string Username;
        public static string Pass;
        // Username and Password of another user, which will be used to test LogIn features
        public static string OtherUsername;
        public static string OtherPass;
        public static string BadUsername;
        public static string BadPass;
        public static PlayerCharacter MyPlayerCharacter ;
        public static PlayerCharacter NotMyPlayerCharacter;
        public static NonPlayerCharacter MyFamily;
        public static NonPlayerCharacter MyEmployee;
        public static NonPlayerCharacter NotMyFamily;
        public static NonPlayerCharacter NotMyEmplployee;
        public static NonPlayerCharacter NobodysCharacter; 
        /// <summary>
        /// Initialise game state for the TestSuite
        /// </summary>
        /// <param name="ctx"></param>
        [ClassInitialize()]
        public static void InitialiseGameState(TestContext ctx)
        {
            Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt");
            Globals_Server.LogFile.AutoFlush = true;
            game = new Game();
            server = new Server();
            client = new TestClient();

            Username = "helen";
            Pass = "potato";
            OtherUsername = "test";
            OtherPass = "tomato";
            BadUsername = "notauser";
            BadPass = "notapass";
            client.LogInAndConnect(Username,Pass,new byte[]{1,2,3,4,5,6});
            MyPlayerCharacter = Globals_Game.userChars[Username];
            Dictionary<string,PlayerCharacter>.Enumerator e = Globals_Game.pcMasterList.GetEnumerator();
            e.MoveNext();
            NotMyPlayerCharacter = e.Current.Value;
            while (NotMyPlayerCharacter == MyPlayerCharacter)
            {
                e.MoveNext();
                NotMyPlayerCharacter = e.Current.Value;
            }
            foreach (NonPlayerCharacter npc in MyPlayerCharacter.myNPCs)
            {
                if (!string.IsNullOrWhiteSpace(npc.familyID))
                {
                    MyFamily = npc;
                }
                else if (!string.IsNullOrWhiteSpace(npc.employer))
                {
                    MyEmployee = npc;
                }
                if (MyEmployee != null && MyFamily != null)
                {
                    break;
                }
            }
            foreach (NonPlayerCharacter npc in NotMyPlayerCharacter.myNPCs)
            {
                if (!string.IsNullOrWhiteSpace(npc.familyID))
                {
                    NotMyFamily = npc;
                }
                else if (!string.IsNullOrWhiteSpace(npc.employer))
                {
                    NotMyEmplployee = npc;
                }
                if (NotMyEmplployee != null && NotMyFamily != null)
                {
                    break;
                }
            }
            if (MyPlayerCharacter.myArmies != null && MyPlayerCharacter.myArmies.Count > 0)
            {
                OwnedArmy = MyPlayerCharacter.myArmies[0];
            }
            else
            {
                Console.WriteLine("Set owned army");
                Army army = new Army(Globals_Game.GetNextArmyID(), null, MyPlayerCharacter.charID, 30, NotMyPlayerCharacter.location.id, false,trp:new uint[]{5,5,5,5,5,5});
                OwnedArmy = army;
                OwnedArmy.AddArmy();
            }
            if (NotMyPlayerCharacter.myArmies != null && NotMyPlayerCharacter.myArmies.Count > 0)
            {
                NotOwnedArmy = NotMyPlayerCharacter.myArmies[0];
            }
            else
            {
                Console.Write("Set not owned army");
                Army army = new Army(Globals_Game.GetNextArmyID(), null, NotMyPlayerCharacter.charID, 30, NotMyPlayerCharacter.location.id, false,trp: new uint[] { 5, 5, 5, 5, 5, 5 });
                NotOwnedArmy = army;
                NotOwnedArmy.AddArmy();
                
            }
            if (MyPlayerCharacter.ownedFiefs != null && MyPlayerCharacter.ownedFiefs.Count > 0)
            {
                OwnedFief = MyPlayerCharacter.ownedFiefs[0];
            }
            if (NotMyPlayerCharacter.ownedFiefs != null && NotMyPlayerCharacter.ownedFiefs.Count > 0)
            {
                NotOwnedFief = NotMyPlayerCharacter.ownedFiefs[0];
            }
            foreach (var npc in Globals_Game.npcMasterList)
            {
                if (npc.Value.GetPlayerCharacter() == null)
                {
                    NobodysCharacter = npc.Value;
                }
            }
        }

        [ClassCleanup()]
        public static void FinaliseGameState()
        {
            client.LogOut();
            server.Shutdown();
            Globals_Server.server.Shutdown("bye");
            Globals_Server.LogFile.Close();
        }

        public bool ValidClientState(TestClient testClient, out Client client)
        {
            if (testClient.net == null || !testClient.net.GetConnectionStatus().Equals("Connected"))
            {
                client = null;
                return false;
            }
            // If do not have a PlayerCharacter, are dead etc, expect to fail
            Globals_Server.clients.TryGetValue(testClient.playerID, out client);
            if (client == null)
            {
                Assert.AreEqual(testClient.net.GetConnectionStatus(), "Disconnected");
                return false;
            }

            if (client.myPlayerCharacter == null || !client.myPlayerCharacter.isAlive)
            {
                Task<string> ReplyTask = testClient.GetServerMessage();
                ReplyTask.Wait();
                string reply = ReplyTask.Result;
                Assert.AreEqual(reply, "You have no valid PlayerCharacter!");
                return false;
            }
            if(!Globals_Game.IsObserver(client))
            {
                Console.WriteLine("TEST: not an observer- not logged in");
                return false;
            }
            else
            {
                Console.WriteLine("TEST: client is observer");
            }
            return true;
        } 

        /// <summary>Test stub for AdjustExpenditure(String, Double, Double, Double, Double, Double)</summary>
        
        public void AdjustExpenditureTest(
            TestClient TestClient,
            string fiefID,
            double newTax,
            double newOff,
            double newGarr,
            double newKeep,
            double newInfra
        )
        {
            TestClient.AdjustExpenditure(fiefID, newTax, newOff, newGarr, newKeep, newInfra);
            
            Client client = null;
            if (!ValidClientState(TestClient, out client))
            {
                Task<string> ReplyTask = TestClient.GetServerMessage();
                ReplyTask.Wait();
                string reply = ReplyTask.Result;
                Assert.AreEqual( "Invalid message sequence-expecting login", reply);
                return;
            }
            // If not a valid fief, expect to fail
            if (string.IsNullOrWhiteSpace(fiefID) || !Globals_Game.fiefKeys.Contains(fiefID))
            {
                Console.Write("not a fief Id ");
                Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                ReplyTask.Wait();
                ProtoMessage reply = ReplyTask.Result;
                Assert.AreEqual(reply.ResponseType,DisplayMessages.ErrorGenericFiefUnidentified);
            }
            else
            {
                Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                ReplyTask.Wait();
                ProtoMessage reply = ReplyTask.Result;
                Console.WriteLine("TEST: Result is "+reply.ResponseType);
                // If not fief owner expect unauthorised
                Fief fief = null;
                Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
                if (fief.owner != client.myPlayerCharacter)
                {
                    
                    Assert.AreEqual(reply.ResponseType, DisplayMessages.ErrorGenericUnauthorised);
                }
                    // If numbers invalid expect an invalid exception
                else if (newTax < 0 || newOff < 0 || newGarr < 0 || newKeep < 0 || newInfra < 0)
                {
                    Assert.AreEqual(reply.ResponseType, DisplayMessages.ErrorGenericMessageInvalid);
                }
                else
                {
                    if (reply.GetType() == typeof (ProtoFief))
                    {
                        Assert.AreEqual(DisplayMessages.FiefExpenditureAdjusted, reply.ResponseType);
                    }
                    else
                    {
                        Assert.AreEqual(DisplayMessages.FiefExpenditureAdjustment,reply.ResponseType);
                    }
                }
            }

        }

        public void AttackTest(TestClient testClient, string armyID, string targetID)
        {
            testClient.Attack(armyID,targetID);
            
            Client client = null;
            if (!ValidClientState(testClient, out client))
            {
                Task<string> ReplyTask = testClient.GetServerMessage();
                ReplyTask.Wait();
                string reply = ReplyTask.Result;
                Assert.AreEqual("Invalid message sequence-expecting login", reply);
                return;
            }
            else
            {
                Task<ProtoMessage> ReplyTask = testClient.GetReply();
                ReplyTask.Wait();
                ProtoMessage reply = ReplyTask.Result;
                Army army = null;
                if (armyID != null)
                {
                   Globals_Game.armyMasterList.TryGetValue(armyID, out army); 
                }
                
                Army targetArmy = null;
                if (targetID != null)
                {
                    Globals_Game.armyMasterList.TryGetValue(targetID, out targetArmy);
                }
                
                ProtoMessage tmp = null;
                if (army == null || targetArmy == null)
                {
                    Assert.IsTrue(reply.ResponseType==DisplayMessages.ErrorGenericMessageInvalid||reply.ResponseType==DisplayMessages.ErrorGenericArmyUnidentified||reply.ResponseType==DisplayMessages.Error);
                }
                else if (army.CalcArmySize() == 0 || targetArmy.CalcArmySize() == 0)
                {
                    Assert.AreEqual(DisplayMessages.Error, reply.ResponseType);
                }
                else if (!client.myPlayerCharacter.myArmies.Contains(army))
                {
                    Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised,reply.ResponseType);
                }
                else if (client.myPlayerCharacter.myArmies.Contains(targetArmy))
                {
                    Assert.AreEqual(DisplayMessages.ArmyAttackSelf,reply.ResponseType);
                }
                else if (!army.ChecksBeforeAttack(targetArmy, out tmp))
                {
                    Assert.AreEqual(tmp.ResponseType, reply.ResponseType);
                }
                else
                {
                    Assert.IsTrue(reply.ResponseType == DisplayMessages.BattleBringSuccess || reply.ResponseType == DisplayMessages.BattleBringFail || reply.ResponseType == DisplayMessages.BattleResults);
                }
            }
        }

        /// <summary>Test stub for AdjustOddsAndAgression(String, Byte, Byte)</summary>
        
        public void AdjustOddsAndAgressionTest(
            TestClient testClient,
            string armyID,
            byte newOdds,
            byte newAgg
        )
        {
            testClient.AdjustOddsAndAgression(armyID, newOdds, newAgg);
            Client client = null;
            if (!ValidClientState(testClient, out client))
            {
                return;
            }
            // If armyID is null or invalid, should get a message about invalid army
            // If don't own army should get an unauthorised message
            
            // TODO: add assertions to method TestClientTest.AdjustOddsAndAgressionTest(TestClient, String, Byte, Byte)
        }

        /// <summary>Test stub for AppointArmyLeader(String, String)</summary>
        
        public void AppointArmyLeaderTest(
            TestClient target,
            string armyID,
            string charID
        )
        {
            target.AppointArmyLeader(armyID, charID);
            // TODO: add assertions to method TestClientTest.AppointArmyLeaderTest(TestClient, String, String)
        }

        /// <summary>Test stub for AppointBailiff(String, String)</summary>
        
        public void AppointBailiffTest(
            TestClient target,
            string charID,
            string fiefID
        )
        {
            target.AppointBailiff(charID, fiefID);
            // TODO: add assertions to method TestClientTest.AppointBailiffTest(TestClient, String, String)
        }

        /// <summary>Test stub for AutoAdjustExpenditure(String)</summary>
        
        public void AutoAdjustExpenditureTest(TestClient target, string fiefID)
        {
            target.AutoAdjustExpenditure(fiefID);
            // TODO: add assertions to method TestClientTest.AutoAdjustExpenditureTest(TestClient, String)
        }

        /// <summary>Test stub for BarCharacter(String, String)</summary>
        
        public void BarCharacterTest(
            TestClient target,
            string fiefID,
            string charID
        )
        {
            target.BarCharacter(fiefID, charID);
            // TODO: add assertions to method TestClientTest.BarCharacterTest(TestClient, String, String)
        }

        /// <summary>Test stub for BarNationality(String, String)</summary>
        
        public void BarNationalityTest(
            TestClient target,
            string natID,
            string fiefID
        )
        {
            target.BarNationality(natID, fiefID);
            // TODO: add assertions to method TestClientTest.BarNationalityTest(TestClient, String, String)
        }

        /// <summary>Test stub for Besiege(String)</summary>
        
        public void BesiegeTest(TestClient target, string armyID)
        {
            target.Besiege(armyID);
            // TODO: add assertions to method TestClientTest.BesiegeTest(String)
        }

        /// <summary>Test stub for Camp(Int32, String)</summary>
        
        public void CampTest(
            TestClient target,
            int days,
            string charID
        )
        {
            target.Camp(days, charID);
            // TODO: add assertions to method TestClientTest.CampTest(TestClient, Int32, String)
        }

        /// <summary>Test stub for DisbandArmy(String)</summary>
        
        public void DisbandArmyTest(TestClient target, string armyID)
        {
            target.DisbandArmy(armyID);
            // TODO: add assertions to method TestClientTest.DisbandArmyTest(TestClient, String)
        }

        /// <summary>Test stub for DropOffTroops(String, String, UInt32[])</summary>
        
        public void DropOffTroopsTest(
            TestClient target,
            string armyID,
            string playerID,
            uint[] troops
        )
        {
            target.DropOffTroops(armyID, playerID, troops);
            // TODO: add assertions to method TestClientTest.DropOffTroopsTest(TestClient, String, String, UInt32[])
        }

        /// <summary>Test stub for EndSiege(String)</summary>
        
        public void EndSiegeTest(TestClient target, string siegeID)
        {
            target.EndSiege(siegeID);
            // TODO: add assertions to method TestClientTest.EndSiegeTest(TestClient, String)
        }

        /// <summary>Test stub for EnterExitKeep(String)</summary>
        
        public void EnterExitKeepTest(TestClient target, string charID)
        {
            target.EnterExitKeep(charID);
            // TODO: add assertions to method TestClientTest.EnterExitKeepTest(TestClient, String)
        }

        /// <summary>Test stub for ExamineArmies(String)</summary>
        
        public void ExamineArmiesTest(TestClient target, string fiefID)
        {
            target.ExamineArmies(fiefID);
            // TODO: add assertions to method TestClientTest.ExamineArmiesTest(TestClient, String)
        }

        /// <summary>Test stub for ExecuteCaptive(String)</summary>
        
        public void ExecuteCaptiveTest(TestClient target, string charID)
        {
            target.ExecuteCaptive(charID);
            // TODO: add assertions to method TestClientTest.ExecuteCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for GetCaptive(String)</summary>
        
        public void GetCaptiveTest(TestClient target, string captiveID)
        {
            target.GetCaptive(captiveID);
            // TODO: add assertions to method TestClientTest.GetCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for GetFiefList()</summary>
        
        public void GetFiefListTest(TestClient target)
        {
            target.GetFiefList();
            // TODO: add assertions to method TestClientTest.GetFiefListTest(TestClient)
        }

        /// <summary>Test stub for GetNPCList(String, String, String)</summary>
        
        public void GetNPCListTest(
            TestClient target,
            string type,
            string role,
            string itemID
        )
        {
            target.GetNPCList(type, role, itemID);
            // TODO: add assertions to method TestClientTest.GetNPCListTest(TestClient, String, String, String)
        }

        /// <summary>Test stub for GetPlayerList()</summary>
        
        public void GetPlayerListTest(TestClient target)
        {
            target.GetPlayerList();
            // TODO: add assertions to method TestClientTest.GetPlayerListTest(TestClient)
        }

        /// <summary>Test stub for GetReply()</summary>
        
        public Task<ProtoMessage> GetReplyTest(TestClient target)
        {
            Task<ProtoMessage> result = target.GetReply();
            return result;
            // TODO: add assertions to method TestClientTest.GetReplyTest(TestClient)
        }

        /// <summary>Test stub for GetSiegeList()</summary>
        
        public void GetSiegeListTest(TestClient target)
        {
            target.GetSiegeList();
            // TODO: add assertions to method TestClientTest.GetSiegeListTest(TestClient)
        }

        /// <summary>Test stub for GetSiege(String)</summary>
        
        public void GetSiegeTest(TestClient target, string siegeID)
        {
            target.GetSiege(siegeID);
            // TODO: add assertions to method TestClientTest.GetSiegeTest(TestClient, String)
        }

        /// <summary>Test stub for ListCharactersInPlace(String, String)</summary>
        
        public void ListCharactersInPlaceTest(
            TestClient target,
            string charID,
            string place
        )
        {
            target.ListCharactersInPlace(charID, place);
            // TODO: add assertions to method TestClientTest.ListCharactersInPlaceTest(TestClient, String, String)
        }

        /// <summary>Test stub for ListDetachments(String)</summary>
        
        public void ListDetachmentsTest(TestClient target, string armyID)
        {
            target.ListDetachments(armyID);
            // TODO: add assertions to method TestClientTest.ListDetachmentsTest(TestClient, String)
        }

        /// <summary>Test stub for LogIn(String, String, Byte[])</summary>
        
        public void LogInTest(
            TestClient client,
            string user,
            string pass,
            byte[] key
        )
        {
            client.LogInAndConnect(user, pass, key);
            // If username not recognised, expect to be disconnected
            if (string.IsNullOrEmpty(user) || !Utility_Methods.CheckStringValid("combined", user) || !LogInManager.users.ContainsKey(user))
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
                return;
            }
            // If password is incorrect, expect an error
            Tuple<byte[], byte[]> hashNsalt = LogInManager.users[user];
            byte[] hash;
            if (pass==null)
            {
                hash = null;
            }
            else
            {
                hash = LogInManager.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pass), hashNsalt.Item2);
            }
            if (hash ==null || !hashNsalt.Item1.SequenceEqual(hash)||key==null||key.Length<5)
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
                Assert.IsFalse(Server.ContainsConnection(user));
            }
            else
            {
                // If the login was successful, expecting a ProtoLogin followed by a ProtoClient back
                Task<ProtoMessage> getReply = client.GetReply();
                getReply.Wait();
                ProtoMessage reply = getReply.Result;
                Assert.AreEqual(reply.GetType(), typeof(ProtoLogIn));

                getReply = client.GetReply();
                getReply.Wait();
                reply = getReply.Result;
                Assert.AreEqual(reply.GetType(), typeof(ProtoClient));

                // If login was successful, the client should be in the list of registered observers
                Assert.IsTrue(Globals_Game.IsObserver(Globals_Server.clients[user]));
                Assert.IsTrue(Server.ContainsConnection(user));
            }
        }


        /// <summary>Test stub for MaintainArmy(String)</summary>
        
        public void MaintainArmyTest(TestClient target, string armyID)
        {
            target.MaintainArmy(armyID);
            // TODO: add assertions to method TestClientTest.MaintainArmyTest(TestClient, String)
        }

        /// <summary>Test stub for Marry(String, String)</summary>
        
        public void MarryTest(
            TestClient target,
            string groomID,
            string brideID
        )
        {
            target.Marry(groomID, brideID);
            // TODO: add assertions to method TestClientTest.MarryTest(TestClient, String, String)
        }

        /// <summary>Test stub for Move(String, String, String[])</summary>
        
        public void MoveTest(
            TestClient target,
            string character,
            string location,
            string[] travelInstructions
        )
        {
            target.Move(character, location, travelInstructions);
            // TODO: add assertions to method TestClientTest.MoveTest(TestClient, String, String, String[])
        }

        /// <summary>Test stub for NameHeir(String)</summary>
        
        public void NameHeirTest(TestClient target, string charID)
        {
            target.NameHeir(charID);
            // TODO: add assertions to method TestClientTest.NameHeirTest(TestClient, String)
        }

        /// <summary>Test stub for NegotiationRound(String)</summary>
        
        public void NegotiationRoundTest(TestClient target, string siegeID)
        {
            target.NegotiationRound(siegeID);
            // TODO: add assertions to method TestClientTest.NegotiationRoundTest(TestClient, String)
        }

        /// <summary>Test stub for PickUpDetachments(String[], String)</summary>
        
        public void PickUpDetachmentsTest(
            TestClient target,
            string[] selectedDetachments,
            string armyID
        )
        {
            target.PickUpDetachments(selectedDetachments, armyID);
            // TODO: add assertions to method TestClientTest.PickUpDetachmentsTest(TestClient, String[], String)
        }

        /// <summary>Test stub for PillageFief(String, String)</summary>
        
        public void PillageFiefTest(
            TestClient target,
            string fiefID,
            string armyID
        )
        {
            target.PillageFief(fiefID, armyID);
            // TODO: add assertions to method TestClientTest.PillageFiefTest(TestClient, String, String)
        }

        /// <summary>Test stub for RansomCaptive(String)</summary>
        
        public void RansomCaptiveTest(TestClient target, string charID)
        {
            target.RansomCaptive(charID);
            // TODO: add assertions to method TestClientTest.RansomCaptiveTest(TestClient, String)
        }


        /// <summary>
        /// Test stub for spy character
        /// </summary>
        /// <param name="testClient"></param>
        /// <param name="charID"></param>
        /// <param name="targetID"></param>
        /// <param name="DoSpy"></param>
        
        public void SpyCharacterTest(TestClient testClient, string charID, string targetID,bool DoSpy)
        {
            testClient.SpyOnCharacter(charID,targetID);
            Task<ProtoMessage> responseTask = testClient.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
#if SESSIONTYPES
            if(string.IsNullOrWhiteSpace(charID)||string.IsNullOrWhiteSpace(targetID))
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
                return;
            }
            Character spy = Globals_Game.getCharFromID(charID);
            Character target = Globals_Game.getCharFromID(targetID);
            if(spy== null||target==null)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericCharacterUnidentified, response.ResponseType);
                return;
            }
            Client client = Globals_Server.clients[testClient.playerID];
            if (spy.GetPlayerCharacter() != client.myPlayerCharacter)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, response.ResponseType);
                return;
            }
            if(spy.location!=target.location)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericNotInSameFief, response.ResponseType);
                return;
            }
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            testClient.SendMessage(Actions.SpyCharacter, DoSpy.ToString());
            responseTask = testClient.GetReply();
            responseTask.Wait();
            response = responseTask.Result;
            return;

#else
#endif
        }

        /// <summary>
        /// Test stub for spy character
        /// </summary>
        /// <param name="testClient"></param>
        /// <param name="charID"></param>
        /// <param name="targetID"></param>
        /// <param name="DoSpy"></param>
        
        public void SpyArmyTest(TestClient testClient, string charID, string targetID, bool DoSpy)
        {
            testClient.SpyOnArmy(charID, targetID);
            Task<ProtoMessage> responseTask = testClient.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
#if SESSIONTYPES
            if (string.IsNullOrWhiteSpace(charID) || string.IsNullOrWhiteSpace(targetID))
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
                return;
            }
            Character spy = Globals_Game.getCharFromID(charID);
            Army target = null;
            Globals_Game.armyMasterList.TryGetValue(targetID, out target);
            if (spy == null || target == null)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericCharacterUnidentified, response.ResponseType);
                return;
            }
            Client client = Globals_Server.clients[testClient.playerID];
            if (spy.GetPlayerCharacter() != client.myPlayerCharacter)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, response.ResponseType);
                return;
            }
            if (spy.location != target.GetLocation())
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericNotInSameFief, response.ResponseType);
                return;
            }
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            testClient.SendMessage(Actions.SpyCharacter, DoSpy.ToString());
            responseTask = testClient.GetReply();
            responseTask.Wait();
            response = responseTask.Result;
            return;

#else
#endif
        }
        /// <summary>
        /// Test stub for spy character
        /// </summary>
        /// <param name="testClient"></param>
        /// <param name="charID"></param>
        /// <param name="targetID"></param>
        /// <param name="DoSpy"></param>
        
        public void SpyFiefTest(TestClient testClient, string charID, string targetID, bool DoSpy)
        {
            testClient.SpyOnFief(charID, targetID);
            Task<ProtoMessage> responseTask = testClient.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
#if SESSIONTYPES
            if (string.IsNullOrWhiteSpace(charID) || string.IsNullOrWhiteSpace(targetID))
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
                return;
            }
            Character spy = Globals_Game.getCharFromID(charID);
            Fief target = null;
            Globals_Game.fiefMasterList.TryGetValue(targetID, out target);
            if (spy == null || target == null)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericCharacterUnidentified, response.ResponseType);
                return;
            }
            Client client = Globals_Server.clients[testClient.playerID];
            if (spy.GetPlayerCharacter() != client.myPlayerCharacter)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, response.ResponseType);
                return;
            }
            if (spy.location != target)
            {
                Assert.AreEqual(DisplayMessages.ErrorGenericNotInSameFief, response.ResponseType);
                return;
            }
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            testClient.SendMessage(Actions.SpyCharacter, DoSpy.ToString());
            responseTask = testClient.GetReply();
            responseTask.Wait();
            response = responseTask.Result;
            return;

#else
#endif
        }
        
        /// <summary>Test stub for RecruitTroops(String, UInt32, Boolean)</summary>
        
        public void RecruitTroopsTest(
            TestClient target,
            string armyID,
            uint numTroops,
            bool isConfirm
        )
        {
#if SESSIONTYPES
            target.RecruitTroops(armyID, numTroops, isConfirm);
            Army army = null;
            Task<ProtoMessage> responseTask = target.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            if (!string.IsNullOrWhiteSpace(armyID))
            {
                if (!Globals_Game.armyMasterList.ContainsKey(armyID))
                {
                    // Expect army unrecognised
                    Assert.AreEqual(DisplayMessages.ErrorGenericArmyUnidentified, response.ResponseType);
                    return;
                }
                else
                {
                    army = Globals_Game.armyMasterList[armyID];
                }
            }
            if (!Globals_Server.clients.ContainsKey(target.playerID))
            {
                Assert.Fail();
            }
            Client client = Globals_Server.clients[target.playerID];
            if (army != null)
            {
                if (army.GetOwner() != client.myPlayerCharacter)
                {
                    Assert.AreEqual(DisplayMessages.ErrorGenericUnauthorised, response.ResponseType);
                }
                if (army.GetLocation().CalcMaxTroops() < numTroops)
                {
                    Assert.AreEqual(DisplayMessages.CharacterRecruitInsufficientFunds, response.ResponseType);
                }
            }
            ProtoMessage ignoreMessage;
            if (!client.myPlayerCharacter.ChecksBeforeRecruitment(out ignoreMessage))
            {
                Assert.IsFalse(response.ResponseType==DisplayMessages.CharacterRecruitInsufficientFunds||response.ResponseType==DisplayMessages.CharacterRecruitInsufficientFunds);
                Assert.IsFalse(response is ProtoRecruit);
                return;
            }
            ProtoRecruit recruitDetails=response as ProtoRecruit;
            if (recruitDetails == null)
            {
                Assert.Fail();
            }
            Console.WriteLine("Sending recruit troops confirmation: " + isConfirm.ToString());
            target.RecruitTroops(armyID,recruitDetails.amount,isConfirm);
            Task<ProtoMessage> confirmTask = target.GetReply();
            confirmTask.Wait();
            ProtoMessage confirm = confirmTask.Result;
            if (isConfirm)
            {
                Assert.AreEqual(DisplayMessages.Success, confirm.ResponseType);
            }
            else
            {
                Assert.AreEqual(DisplayMessages.RecruitCancelled,confirm.ResponseType);
            }
            
#else
            
#endif
            // TODO: add assertions to method TestClientTest.RecruitTroopsTest(TestClient, String, UInt32, Boolean)
        }

        /// <summary>Test stub for ReductionRound(String)</summary>
        
        public void ReductionRoundTest(TestClient target, string siegeID)
        {
            target.ReductionRound(siegeID);
            // TODO: add assertions to method TestClientTest.ReductionRoundTest(TestClient, String)
        }

        /// <summary>Test stub for ReleaseCaptive(String)</summary>
        
        public void ReleaseCaptiveTest(TestClient target, string charID)
        {
            target.ReleaseCaptive(charID);
            // TODO: add assertions to method TestClientTest.ReleaseCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for RemoveBailiff(String)</summary>
        
        public void RemoveBailiffTest(TestClient target, string fiefID)
        {
            target.RemoveBailiff(fiefID);
            // TODO: add assertions to method TestClientTest.RemoveBailiffTest(TestClient, String)
        }

        /// <summary>Test stub for SeasonUpdate()</summary>
        
        public void SeasonUpdateTest(TestClient client)
        {
            client.SeasonUpdate();
            // TODO: add assertions to method TestClientTest.SeasonUpdateTest()
        }

        /// <summary>Test stub for SpyOnFief(String, String)</summary>
        
        public void SpyOnFiefTest(
            TestClient target,
            string charID,
            string fiefID
        )
        {
            target.SpyOnFief(charID, fiefID);
            // TODO: add assertions to method TestClientTest.SpyOnFiefTest(TestClient, String, String)
        }

        /// <summary>Test stub for StormRound(String)</summary>
        
        public void StormRoundTest(TestClient target, string siegeID)
        {
            target.StormRound(siegeID);
            // TODO: add assertions to method TestClientTest.StormRoundTest(TestClient, String)
        }

        /// <summary>Test stub for SwitchCharacter(String)</summary>
        
        public void SwitchCharacterTest(TestClient target, string charID)
        {
            target.SwitchCharacter(charID);
            // TODO: add assertions to method TestClientTest.SwitchCharacterTest(TestClient, String)
        }

        /// <summary>Test stub for TransferFunds(Int32, String, Boolean)</summary>
        
        public void TransferFundsTest(
            TestClient target,
            int amount,
            string fiefID,
            bool toHome
        )
        {
            target.TransferFunds(amount, fiefID, toHome);
            // TODO: add assertions to method TestClientTest.TransferFundsTest(TestClient, Int32, String, Boolean)
        }

        /// <summary>Test stub for TransferFundsToPlayer(Int32, String)</summary>
        
        public void TransferFundsToPlayerTest(
            TestClient target,
            int amount,
            string playerID
        )
        {
            target.TransferFundsToPlayer(amount, playerID);
            // TODO: add assertions to method TestClientTest.TransferFundsToPlayerTest(TestClient, Int32, String)
        }

        /// <summary>Test stub for TryForChild(String)</summary>
        
        public void TryForChildTest(TestClient target, string charID)
        {
            target.TryForChild(charID);
            // TODO: add assertions to method TestClientTest.TryForChildTest(TestClient, String)
        }

        /// <summary>Test stub for UnBarNationality(String, String)</summary>
        
        public void UnBarNationalityTest(
            TestClient target,
            string natID,
            string fiefID
        )
        {
            target.UnBarNationality(natID, fiefID);
            // TODO: add assertions to method TestClientTest.UnBarNationalityTest(TestClient, String, String)
        }

        /// <summary>Test stub for UnbarCharacter(String, String)</summary>
        
        public void UnbarCharacterTest(
            TestClient target,
            string fiefID,
            string charID
        )
        {
            target.UnbarCharacter(fiefID, charID);
            // TODO: add assertions to method TestClientTest.UnbarCharacterTest(TestClient, String, String)
        }

        /// <summary>Test stub for ViewCaptives()</summary>
        
        public void ViewCaptivesTest(TestClient target)
        {
            target.ViewCaptives();
            // TODO: add assertions to method TestClientTest.ViewCaptivesTest(TestClient)
        }

        /// <summary>Test stub for ViewCaptives(String)</summary>
        
        public void ViewCaptivesTest01(TestClient target, string fiefID)
        {
            target.ViewCaptives(fiefID);
            // TODO: add assertions to method TestClientTest.ViewCaptivesTest01(TestClient, String)
        }

    }
}
