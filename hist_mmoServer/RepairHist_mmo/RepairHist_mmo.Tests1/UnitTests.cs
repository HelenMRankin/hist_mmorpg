using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using hist_mmorpg.Tests;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;

namespace hist_mmorpg.Tests
{
    public partial class TestClientTest
    {


        [TestMethod] 
        [TestCategory("LogIn")]
		[Timeout(15000)]
        public void LogInTestValid()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUsername, OtherPass, new byte[] { 1,2,3,4,5,6,7,8,9});
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogOutBeforeLogIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.ConnectNoLogin(OtherUsername, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(OtherUsername));
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestBadPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUsername, BadPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(OtherUsername));
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestNullPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUsername, null, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(OtherUsername));
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestNoKey()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUsername, OtherPass,null);
#if !ALLOW_UNENCRYPT
            Assert.IsFalse(Server.ContainsConnection(OtherUsername));
#endif
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestInvalidUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, BadUsername, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(BadUsername));
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestNullUsername()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, null, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTestEmptyUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, "", OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.IsFalse(Server.ContainsConnection(""));
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout (10000)]
        public void DoubleLogIn()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUsername, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            while (!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            s0.SendDummyLogIn(OtherUsername,OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, });
            Task<ProtoMessage> reply = s0.GetReply();
            reply.Wait();
            Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, reply.Result.ResponseType);
            s0.LogOut();
        }

        /// <summary>
        /// Tests that a log in will time out correctly
        /// </summary>
        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTimeout()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(OtherUsername, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Task<string> reply = s0.GetServerMessage();
            reply.Wait();
            Assert.AreEqual("Failed to login due to timeout", reply.Result);
            Assert.AreEqual(s0.net.GetConnectionStatus(), "Disconnected");
            s0.LogOut();
        }

        /// <summary>
        /// Tries to log in with two different clients on same credentials
        /// </summary>
        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void LogInTwoClients()
        {
            //client.LogOut();
            TestClient s0 = new TestClient();
            TestClient s1 = new TestClient();
            this.LogInTest(s0,OtherUsername,OtherPass, new byte[] {1,2,3,4,5,6,7});
            while (!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            s1.LogInAndConnect(OtherUsername, OtherPass, new byte[]{1,2,3,4,5,6,6,8});
            Assert.IsTrue(s1.net.GetConnectionStatus().Equals("Disconnected"));
            s0.LogOut();
            s1.LogOut();
        }
        
        [TestMethod]
        [TestCategory("LogIn")]
        [Timeout(15000)]
        public void AdjustExpenditureTestNotLoggedIn()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(OtherUsername, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            while(!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            this.AdjustExpenditureTest(s0,"notafief",50,50,50,50,50);
            s0.LogOut();
        }

        [TestMethod]
        [TestCategory("General")]
        [Timeout(30000)]
        public void MessageWaitingTest()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            int n = 20;
            Task<ProtoMessage>[] replyTasks= new Task<ProtoMessage>[n];
            // Set up reply receivers 
            for (int i = 0; i < n; i++)
            {
                replyTasks[i] = client.GetReply(i.ToString());
            }
            // Send messages
            for (int i = 0; i < n; i++)
            {
                client.net.Send(new ProtoMessage() { ActionType = Actions.ViewChar, Message = MyPlayerCharacter.charID });
            }
            // Wait on all replies
            for (int i = 0; i < n; i++)
            {
                replyTasks[i].Wait(10000);
            }
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(15000)]
        public void AdjustExpenditureTestBadFief()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.AdjustExpenditureTest(client, "notafief", 50, 50, 50, 50, 50);
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(15000)]
        public void AdjustExpenditureTestNotOwnedFief()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.AdjustExpenditureTest(client, NotOwnedFief.id, 50, 50, 50, 50, 50);
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(15000)]
        public void AdjustExpenditureBadData()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.AdjustExpenditureTest(client, OwnedFief.id,-1,-1,-2,-1,-1);
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(15000)]
        public void AdjustExpenditureSuccess()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.AdjustExpenditureTest(client, OwnedFief.id, 1,1,1,1,1);
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(15000)]
        public void AdjustExpenditureTooMuch()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.AdjustExpenditureTest(client, OwnedFief.id, 100000, 1000000, 11000000, 11000000, 11000000);
        }

        [TestMethod]
        [TestCategory("AdjustExpenditure")]
        [Timeout(8000)]
        public void AttackValid()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client,OwnedArmy.armyID,NotOwnedArmy.armyID);
        }


        [TestMethod]
        [Timeout(8000)]
        public void AttackNotOwnedArmy()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, NotOwnedArmy.armyID, OwnedArmy.armyID);
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackMyself()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, OwnedArmy.armyID);
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackBadArmy()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, "NotanArmyId");
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackNullArmy()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, null);
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackTooFarFromArmy()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            OwnedArmy.location = OwnedFief.id;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = OwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, NotOwnedArmy.armyID);
        }

        /*
        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionBadArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, "defonotanarmy",1,1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionNullArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, null, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionEmptyArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, "", 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionNotMyArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, NotOwnedArmy.armyID, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });

            this.AdjustOddsAndAgressionTest(s0, OwnedArmy.armyID, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionBadBytes()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });

            this.AdjustOddsAndAgressionTest(s0, OwnedArmy.armyID, 15, 20);

            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNotLoggedIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;

            this.AppointArmyLeaderTest(s0, OwnedArmy.armyID, MyEmployee.charID);

        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username,Pass,new byte[]{1,2,3,4,5,6});
            this.AppointArmyLeaderTest(s0,OwnedArmy.armyID, MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNotOwnedArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0,NotOwnedArmy.armyID, MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNotOwnedCharacer()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0,OwnedArmy.armyID, NotMyEmplployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNullArmyID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, null, MyEmployee.charID);
            s0.LogOut();
        }
        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderEmptyArmyID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, "", MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNullCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, OwnedArmy.armyID, null);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderEmptyCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, OwnedArmy.armyID, "");
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNotOwnedCharacter()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, NotMyEmplployee.charID, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNotOwnedFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, NotOwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNullFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, null);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffEmptyFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, "");
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNullCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, null, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffEmptyCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, "", OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		 
		[Timeout(15000)]
        public void BesiegeOwnFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] {1, 2, 3, 4, 5, 6});
            this.BesiegeTest(s0,OwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void BesiegeNotOwnedArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.BesiegeTest(s0, NotOwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void BesiegeSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            s0.Move(OwnedArmy.leader,NotOwnedFief.id);
            this.BesiegeTest(s0, OwnedArmy.armyID);
            s0.LogOut();
        }
        */
        [TestMethod]
        [TestCategory("Recruit")]
        [Timeout(15000)]
        public void RecruitValidCancel()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                this.RecruitTroopsTest(client,null,50,false);
            }
            else
            {
                this.RecruitTroopsTest(client, OwnedArmy.armyID, 50, false);
            }
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitValidConfirm()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                this.RecruitTroopsTest(client, null, 50, true);
            }
            else
            {
                this.RecruitTroopsTest(client, OwnedArmy.armyID, 50, true);
            }
        }
        [TestMethod]
        [Timeout(15000)]
        public void RecruitTooMany()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                this.RecruitTroopsTest(client, null, 50000, true);
            }
            else
            {
                this.RecruitTroopsTest(client, OwnedArmy.armyID, 50000, true);
            }
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitInvalidAlreadyRecruited()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                this.RecruitTroopsTest(client, null, 50, true);
            }
            else
            {
                this.RecruitTroopsTest(client, OwnedArmy.armyID, 50, true);
            }
            this.RecruitTroopsTest(client, null, 50, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitThenTravel()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                client.RecruitTroops(null, 50, true);
            }
            else
            {
                client.RecruitTroops(OwnedArmy.armyID, 50, true);
            }
            Thread.Sleep(1000);
            client.ClearMessageQueues();
            client.Move(MyPlayerCharacter.charID, OwnedFief.id);
            Task<ProtoMessage> responseTask = client.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
#if STRICT
            Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
#else
            Assert.AreNotEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
#endif
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyValid()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            if (NotOwnedArmy == null)
            {
                Assert.Fail("NotOwnedArmy is null");
                return;
            }
            NotOwnedArmy.location = MyPlayerCharacter.location.id;
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID ,true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyCancel()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            NotOwnedArmy.location = MyPlayerCharacter.location.ToString();
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyNotOwned()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            NotOwnedArmy.location = MyPlayerCharacter.location.ToString();
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(client, NotMyPlayerCharacter.charID, NotOwnedArmy.armyID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyTooFar()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(35000)]
        public void SpyArmyTimeout()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            // (need to move the character to the same location as the playercharacter)
            NotOwnedArmy.location = MyPlayerCharacter.location.id;
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            client.SpyOnArmy(MyPlayerCharacter.charID, NotOwnedArmy.armyID);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = client.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = client.GetReply();
            responseTask.Wait();
            response = responseTask.Result;

            Assert.AreEqual(DisplayMessages.Timeout, response.ResponseType);
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefValid()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            MyPlayerCharacter.location = NotOwnedFief;
            this.SpyFiefTest(client, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefCancel()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();

            MyPlayerCharacter.location = NotOwnedFief;
            this.SpyFiefTest(client, MyPlayerCharacter.charID, NotOwnedFief.id, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefNotOwned()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.SpyFiefTest(client, NotMyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefTooFar()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            this.SpyFiefTest(client, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(35000)]
        public void SpyFiefTimeout()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            MyPlayerCharacter.location = NotOwnedFief;
            client.SpyOnFief(MyPlayerCharacter.charID, NotOwnedFief.id);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = client.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = client.GetReply();
            responseTask.Wait();
            response = responseTask.Result;

            Assert.AreEqual(DisplayMessages.Timeout, response.ResponseType);
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterValid()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterCancel()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, NotMyFamily.charID, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterNotOwned()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(NotMyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, NotMyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterTooFar()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(40000)]
        public void SpyCharacterTimeout()
        {
            
            //client.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            client.SpyOnCharacter(MyPlayerCharacter.charID, NotMyFamily.charID);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = client.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = client.GetReply();
            responseTask.Wait();
            response = responseTask.Result;

            Assert.AreEqual(DisplayMessages.Timeout, response.ResponseType);
        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelValid()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            Fief f = MyPlayerCharacter.location;
            Fief adjacent = Globals_Game.gameMap.GetFief(f, "N");
            if (adjacent == null)
            {
                adjacent = Globals_Game.gameMap.GetFief(f, "S");
            }
            if (adjacent == null)
            {
                adjacent = Globals_Game.gameMap.GetFief(f, "E");
            }
            this.MoveTest(client,MyPlayerCharacter.charID,adjacent.id,null);
        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelSiegeConfirm()
        {
            // Forcibly add a siege
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            Fief f = MyPlayerCharacter.location;
            Fief adjacent = Globals_Game.gameMap.GetFief(f, "E");
            Siege s = Pillage_Siege.SiegeStart(OwnedArmy, NotOwnedFief);

            if (s == null||string.IsNullOrWhiteSpace(OwnedArmy.CheckIfBesieger())||OwnedArmy.GetSiege()==null||MyPlayerCharacter.GetArmy().GetSiege()==null)
            {
                Assert.Fail();
            }
            client.ClearMessageQueues();
            this.MoveTest(client,MyPlayerCharacter.charID,f.id,null);
            client.SendMessage(Actions.TravelTo,true);
            Thread.Sleep(1000);
        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelSiegeCancel()
        {

        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelTooFar()
        {

        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelBadCharacter()
        {
            
        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelNotOwnedCharacter()
        {

        }

        [TestMethod]
        [Timeout(10000)]
        public void TravelBadFief()
        {

        }
    }
}
