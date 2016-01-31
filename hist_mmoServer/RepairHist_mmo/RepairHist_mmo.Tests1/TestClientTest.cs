// <copyright file="TestClientTest.cs">Copyright ©  2015</copyright>
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using System.Linq;
using System.Security.Policy;

namespace hist_mmorpg.Tests
{
    /// <summary>This class contains parameterized unit tests for TestClient</summary>
    [PexClass(typeof(TestClient))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
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
        public static string Username;
        public static string Pass;
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
            BadUsername = "notauser";
            BadPass = "notapass";
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
            if (NotMyPlayerCharacter.myArmies != null && NotMyPlayerCharacter.myArmies.Count > 0)
            {
                NotOwnedArmy = NotMyPlayerCharacter.myArmies[0];
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
            server.Shutdown();
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
            return true;
        } 

        /// <summary>Test stub for AdjustExpenditure(String, Double, Double, Double, Double, Double)</summary>
        [PexMethod]
        public void AdjustExpenditureTest(
            [PexAssumeUnderTest]TestClient TestClient,
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
                return;
            }
            // If not a valid fief, expect to fail
            if (string.IsNullOrWhiteSpace(fiefID) || !Globals_Game.fiefKeys.Contains(fiefID))
            {
                Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                ReplyTask.Wait();
                ProtoMessage reply = ReplyTask.Result;
                Assert.AreEqual(reply.ResponseType,DisplayMessages.ErrorGenericFiefUnidentified);
            }
            else
            {
                // If not fief owner expect unauthorised
                Fief fief = null;
                Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
                if (fief.owner != client.myPlayerCharacter)
                {
                    Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                    ReplyTask.Wait();
                    ProtoMessage reply = ReplyTask.Result;
                    Assert.AreEqual(reply.ResponseType, DisplayMessages.ErrorGenericUnauthorised);
                }
                    // If numbers invalid expect an invalid exception
                else if (newTax < 0 || newOff < 0 || newGarr < 0 || newKeep < 0 || newInfra < 0)
                {
                    Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                    ReplyTask.Wait();
                    ProtoMessage reply = ReplyTask.Result;
                    Assert.AreEqual(reply.ResponseType, DisplayMessages.ErrorGenericMessageInvalid);
                }
                else
                {
                   
                    // On success expect either a ProtoFief or a ProtoMessage
                    Task<ProtoMessage> ReplyTask = TestClient.GetReply();
                    ReplyTask.Wait();
                    ProtoMessage reply = ReplyTask.Result;
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

        /// <summary>Test stub for AdjustOddsAndAgression(String, Byte, Byte)</summary>
        [PexMethod]
        public void AdjustOddsAndAgressionTest(
            [PexAssumeUnderTest]TestClient testClient,
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
        [PexMethod]
        public void AppointArmyLeaderTest(
            [PexAssumeUnderTest]TestClient target,
            string armyID,
            string charID
        )
        {
            target.AppointArmyLeader(armyID, charID);
            // TODO: add assertions to method TestClientTest.AppointArmyLeaderTest(TestClient, String, String)
        }

        /// <summary>Test stub for AppointBailiff(String, String)</summary>
        [PexMethod]
        public void AppointBailiffTest(
            [PexAssumeUnderTest]TestClient target,
            string charID,
            string fiefID
        )
        {
            target.AppointBailiff(charID, fiefID);
            // TODO: add assertions to method TestClientTest.AppointBailiffTest(TestClient, String, String)
        }

        /// <summary>Test stub for AutoAdjustExpenditure(String)</summary>
        [PexMethod]
        public void AutoAdjustExpenditureTest([PexAssumeUnderTest]TestClient target, string fiefID)
        {
            target.AutoAdjustExpenditure(fiefID);
            // TODO: add assertions to method TestClientTest.AutoAdjustExpenditureTest(TestClient, String)
        }

        /// <summary>Test stub for BarCharacter(String, String)</summary>
        [PexMethod]
        public void BarCharacterTest(
            [PexAssumeUnderTest]TestClient target,
            string fiefID,
            string charID
        )
        {
            target.BarCharacter(fiefID, charID);
            // TODO: add assertions to method TestClientTest.BarCharacterTest(TestClient, String, String)
        }

        /// <summary>Test stub for BarNationality(String, String)</summary>
        [PexMethod]
        public void BarNationalityTest(
            [PexAssumeUnderTest]TestClient target,
            string natID,
            string fiefID
        )
        {
            target.BarNationality(natID, fiefID);
            // TODO: add assertions to method TestClientTest.BarNationalityTest(TestClient, String, String)
        }

        /// <summary>Test stub for Besiege(String)</summary>
        [PexMethod]
        public void BesiegeTest([PexAssumeUnderTest]TestClient target, string armyID)
        {
            target.Besiege(armyID);
            // TODO: add assertions to method TestClientTest.BesiegeTest(String)
        }

        /// <summary>Test stub for Camp(Int32, String)</summary>
        [PexMethod]
        public void CampTest(
            [PexAssumeUnderTest]TestClient target,
            int days,
            string charID
        )
        {
            target.Camp(days, charID);
            // TODO: add assertions to method TestClientTest.CampTest(TestClient, Int32, String)
        }

        /// <summary>Test stub for DisbandArmy(String)</summary>
        [PexMethod]
        public void DisbandArmyTest([PexAssumeUnderTest]TestClient target, string armyID)
        {
            target.DisbandArmy(armyID);
            // TODO: add assertions to method TestClientTest.DisbandArmyTest(TestClient, String)
        }

        /// <summary>Test stub for DropOffTroops(String, String, UInt32[])</summary>
        [PexMethod]
        public void DropOffTroopsTest(
            [PexAssumeUnderTest]TestClient target,
            string armyID,
            string playerID,
            uint[] troops
        )
        {
            target.DropOffTroops(armyID, playerID, troops);
            // TODO: add assertions to method TestClientTest.DropOffTroopsTest(TestClient, String, String, UInt32[])
        }

        /// <summary>Test stub for EndSiege(String)</summary>
        [PexMethod]
        public void EndSiegeTest([PexAssumeUnderTest]TestClient target, string siegeID)
        {
            target.EndSiege(siegeID);
            // TODO: add assertions to method TestClientTest.EndSiegeTest(TestClient, String)
        }

        /// <summary>Test stub for EnterExitKeep(String)</summary>
        [PexMethod]
        public void EnterExitKeepTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.EnterExitKeep(charID);
            // TODO: add assertions to method TestClientTest.EnterExitKeepTest(TestClient, String)
        }

        /// <summary>Test stub for ExamineArmies(String)</summary>
        [PexMethod]
        public void ExamineArmiesTest([PexAssumeUnderTest]TestClient target, string fiefID)
        {
            target.ExamineArmies(fiefID);
            // TODO: add assertions to method TestClientTest.ExamineArmiesTest(TestClient, String)
        }

        /// <summary>Test stub for ExecuteCaptive(String)</summary>
        [PexMethod]
        public void ExecuteCaptiveTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.ExecuteCaptive(charID);
            // TODO: add assertions to method TestClientTest.ExecuteCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for GetCaptive(String)</summary>
        [PexMethod]
        public void GetCaptiveTest([PexAssumeUnderTest]TestClient target, string captiveID)
        {
            target.GetCaptive(captiveID);
            // TODO: add assertions to method TestClientTest.GetCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for GetFiefList()</summary>
        [PexMethod]
        public void GetFiefListTest([PexAssumeUnderTest]TestClient target)
        {
            target.GetFiefList();
            // TODO: add assertions to method TestClientTest.GetFiefListTest(TestClient)
        }

        /// <summary>Test stub for GetNPCList(String, String, String)</summary>
        [PexMethod]
        public void GetNPCListTest(
            [PexAssumeUnderTest]TestClient target,
            string type,
            string role,
            string itemID
        )
        {
            target.GetNPCList(type, role, itemID);
            // TODO: add assertions to method TestClientTest.GetNPCListTest(TestClient, String, String, String)
        }

        /// <summary>Test stub for GetPlayerList()</summary>
        [PexMethod]
        public void GetPlayerListTest([PexAssumeUnderTest]TestClient target)
        {
            target.GetPlayerList();
            // TODO: add assertions to method TestClientTest.GetPlayerListTest(TestClient)
        }

        /// <summary>Test stub for GetReply()</summary>
        [PexMethod]
        public Task<ProtoMessage> GetReplyTest([PexAssumeUnderTest]TestClient target)
        {
            Task<ProtoMessage> result = target.GetReply();
            return result;
            // TODO: add assertions to method TestClientTest.GetReplyTest(TestClient)
        }

        /// <summary>Test stub for GetSiegeList()</summary>
        [PexMethod]
        public void GetSiegeListTest([PexAssumeUnderTest]TestClient target)
        {
            target.GetSiegeList();
            // TODO: add assertions to method TestClientTest.GetSiegeListTest(TestClient)
        }

        /// <summary>Test stub for GetSiege(String)</summary>
        [PexMethod]
        public void GetSiegeTest([PexAssumeUnderTest]TestClient target, string siegeID)
        {
            target.GetSiege(siegeID);
            // TODO: add assertions to method TestClientTest.GetSiegeTest(TestClient, String)
        }

        /// <summary>Test stub for ListCharactersInPlace(String, String)</summary>
        [PexMethod]
        public void ListCharactersInPlaceTest(
            [PexAssumeUnderTest]TestClient target,
            string charID,
            string place
        )
        {
            target.ListCharactersInPlace(charID, place);
            // TODO: add assertions to method TestClientTest.ListCharactersInPlaceTest(TestClient, String, String)
        }

        /// <summary>Test stub for ListDetachments(String)</summary>
        [PexMethod]
        public void ListDetachmentsTest([PexAssumeUnderTest]TestClient target, string armyID)
        {
            target.ListDetachments(armyID);
            // TODO: add assertions to method TestClientTest.ListDetachmentsTest(TestClient, String)
        }

        /// <summary>Test stub for LogIn(String, String, Byte[])</summary>
        [PexMethod]
        public void LogInTest(
            [PexAssumeUnderTest]TestClient client,
            string user,
            string pass,
            byte[] key
        )
        {
            client.LogIn(user, pass, key);
            // If username not recognised, expect to be disconnected
            if (string.IsNullOrEmpty(user) || !Utility_Methods.CheckStringValid("combined", user) || !server.logInManager.users.ContainsKey(user))
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
                return;
            }
            // If password is incorrect, expect an error
            Tuple<byte[], byte[]> hashNsalt = server.logInManager.users[user];
            byte[] hash = server.logInManager.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pass), hashNsalt.Item2);
            if (!hashNsalt.Item1.SequenceEqual(hash))
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
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
            }
        }

        /// <summary>Test stub for MaintainArmy(String)</summary>
        [PexMethod]
        public void MaintainArmyTest([PexAssumeUnderTest]TestClient target, string armyID)
        {
            target.MaintainArmy(armyID);
            // TODO: add assertions to method TestClientTest.MaintainArmyTest(TestClient, String)
        }

        /// <summary>Test stub for Marry(String, String)</summary>
        [PexMethod]
        public void MarryTest(
            [PexAssumeUnderTest]TestClient target,
            string groomID,
            string brideID
        )
        {
            target.Marry(groomID, brideID);
            // TODO: add assertions to method TestClientTest.MarryTest(TestClient, String, String)
        }

        /// <summary>Test stub for Move(String, String, String[])</summary>
        [PexMethod]
        public void MoveTest(
            [PexAssumeUnderTest]TestClient target,
            string character,
            string location,
            string[] travelInstructions
        )
        {
            target.Move(character, location, travelInstructions);
            // TODO: add assertions to method TestClientTest.MoveTest(TestClient, String, String, String[])
        }

        /// <summary>Test stub for NameHeir(String)</summary>
        [PexMethod]
        public void NameHeirTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.NameHeir(charID);
            // TODO: add assertions to method TestClientTest.NameHeirTest(TestClient, String)
        }

        /// <summary>Test stub for NegotiationRound(String)</summary>
        [PexMethod]
        public void NegotiationRoundTest([PexAssumeUnderTest]TestClient target, string siegeID)
        {
            target.NegotiationRound(siegeID);
            // TODO: add assertions to method TestClientTest.NegotiationRoundTest(TestClient, String)
        }

        /// <summary>Test stub for PickUpDetachments(String[], String)</summary>
        [PexMethod]
        public void PickUpDetachmentsTest(
            [PexAssumeUnderTest]TestClient target,
            string[] selectedDetachments,
            string armyID
        )
        {
            target.PickUpDetachments(selectedDetachments, armyID);
            // TODO: add assertions to method TestClientTest.PickUpDetachmentsTest(TestClient, String[], String)
        }

        /// <summary>Test stub for PillageFief(String, String)</summary>
        [PexMethod]
        public void PillageFiefTest(
            [PexAssumeUnderTest]TestClient target,
            string fiefID,
            string armyID
        )
        {
            target.PillageFief(fiefID, armyID);
            // TODO: add assertions to method TestClientTest.PillageFiefTest(TestClient, String, String)
        }

        /// <summary>Test stub for RansomCaptive(String)</summary>
        [PexMethod]
        public void RansomCaptiveTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.RansomCaptive(charID);
            // TODO: add assertions to method TestClientTest.RansomCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for RecruitTroops(String, UInt32, Boolean)</summary>
        [PexMethod]
        public void RecruitTroopsTest(
            [PexAssumeUnderTest]TestClient target,
            string armyID,
            uint numTroops,
            bool isConfirm
        )
        {
            target.RecruitTroops(armyID, numTroops, isConfirm);
            // TODO: add assertions to method TestClientTest.RecruitTroopsTest(TestClient, String, UInt32, Boolean)
        }

        /// <summary>Test stub for ReductionRound(String)</summary>
        [PexMethod]
        public void ReductionRoundTest([PexAssumeUnderTest]TestClient target, string siegeID)
        {
            target.ReductionRound(siegeID);
            // TODO: add assertions to method TestClientTest.ReductionRoundTest(TestClient, String)
        }

        /// <summary>Test stub for ReleaseCaptive(String)</summary>
        [PexMethod]
        public void ReleaseCaptiveTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.ReleaseCaptive(charID);
            // TODO: add assertions to method TestClientTest.ReleaseCaptiveTest(TestClient, String)
        }

        /// <summary>Test stub for RemoveBailiff(String)</summary>
        [PexMethod]
        public void RemoveBailiffTest([PexAssumeUnderTest]TestClient target, string fiefID)
        {
            target.RemoveBailiff(fiefID);
            // TODO: add assertions to method TestClientTest.RemoveBailiffTest(TestClient, String)
        }

        /// <summary>Test stub for SeasonUpdate()</summary>
        [PexMethod]
        public void SeasonUpdateTest()
        {
            TestClient.SeasonUpdate();
            // TODO: add assertions to method TestClientTest.SeasonUpdateTest()
        }

        /// <summary>Test stub for SpyOnFief(String, String)</summary>
        [PexMethod]
        public void SpyOnFiefTest(
            [PexAssumeUnderTest]TestClient target,
            string charID,
            string fiefID
        )
        {
            target.SpyOnFief(charID, fiefID);
            // TODO: add assertions to method TestClientTest.SpyOnFiefTest(TestClient, String, String)
        }

        /// <summary>Test stub for StormRound(String)</summary>
        [PexMethod]
        public void StormRoundTest([PexAssumeUnderTest]TestClient target, string siegeID)
        {
            target.StormRound(siegeID);
            // TODO: add assertions to method TestClientTest.StormRoundTest(TestClient, String)
        }

        /// <summary>Test stub for SwitchCharacter(String)</summary>
        [PexMethod]
        public void SwitchCharacterTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.SwitchCharacter(charID);
            // TODO: add assertions to method TestClientTest.SwitchCharacterTest(TestClient, String)
        }

        /// <summary>Test stub for TransferFunds(Int32, String, Boolean)</summary>
        [PexMethod]
        public void TransferFundsTest(
            [PexAssumeUnderTest]TestClient target,
            int amount,
            string fiefID,
            bool toHome
        )
        {
            target.TransferFunds(amount, fiefID, toHome);
            // TODO: add assertions to method TestClientTest.TransferFundsTest(TestClient, Int32, String, Boolean)
        }

        /// <summary>Test stub for TransferFundsToPlayer(Int32, String)</summary>
        [PexMethod]
        public void TransferFundsToPlayerTest(
            [PexAssumeUnderTest]TestClient target,
            int amount,
            string playerID
        )
        {
            target.TransferFundsToPlayer(amount, playerID);
            // TODO: add assertions to method TestClientTest.TransferFundsToPlayerTest(TestClient, Int32, String)
        }

        /// <summary>Test stub for TryForChild(String)</summary>
        [PexMethod]
        public void TryForChildTest([PexAssumeUnderTest]TestClient target, string charID)
        {
            target.TryForChild(charID);
            // TODO: add assertions to method TestClientTest.TryForChildTest(TestClient, String)
        }

        /// <summary>Test stub for UnBarNationality(String, String)</summary>
        [PexMethod]
        public void UnBarNationalityTest(
            [PexAssumeUnderTest]TestClient target,
            string natID,
            string fiefID
        )
        {
            target.UnBarNationality(natID, fiefID);
            // TODO: add assertions to method TestClientTest.UnBarNationalityTest(TestClient, String, String)
        }

        /// <summary>Test stub for UnbarCharacter(String, String)</summary>
        [PexMethod]
        public void UnbarCharacterTest(
            [PexAssumeUnderTest]TestClient target,
            string fiefID,
            string charID
        )
        {
            target.UnbarCharacter(fiefID, charID);
            // TODO: add assertions to method TestClientTest.UnbarCharacterTest(TestClient, String, String)
        }

        /// <summary>Test stub for ViewCaptives()</summary>
        [PexMethod]
        public void ViewCaptivesTest([PexAssumeUnderTest]TestClient target)
        {
            target.ViewCaptives();
            // TODO: add assertions to method TestClientTest.ViewCaptivesTest(TestClient)
        }

        /// <summary>Test stub for ViewCaptives(String)</summary>
        [PexMethod]
        public void ViewCaptivesTest01([PexAssumeUnderTest]TestClient target, string fiefID)
        {
            target.ViewCaptives(fiefID);
            // TODO: add assertions to method TestClientTest.ViewCaptivesTest01(TestClient, String)
        }

        /// <summary>Test stub for ViewCharacter(String)</summary>
        [PexMethod]
        public void ViewCharacterTest(string charID)
        {
            TestClient.ViewCharacter(charID);
            // TODO: add assertions to method TestClientTest.ViewCharacterTest(String)
        }

        /// <summary>Test stub for ViewFief(String)</summary>
        [PexMethod]
        public void ViewFiefTest([PexAssumeUnderTest]TestClient target, string fiefID)
        {
            target.ViewFief(fiefID);
            // TODO: add assertions to method TestClientTest.ViewFiefTest(TestClient, String)
        }
    }
}
