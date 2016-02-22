using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using hist_mmorpg.Tests;
using System.Threading.Tasks;
using System.Threading;

namespace hist_mmorpg.Tests
{
    public partial class TestClientTest
    {


        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestValid()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, Username, Pass, new byte[] { 1,2,3,4,5,6,7,8,9});
            while (!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            s0.LogOut();
            
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogOutBeforeLogIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.ConnectNoLogin(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(Username));
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestBadPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, BadPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(Username));
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestNullPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, null, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(Username));
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestNoKey()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, Pass, new byte[] { });
            Assert.IsFalse(Server.ContainsConnection(Username));
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestInvalidUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, BadUsername, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(BadUsername));
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestNullUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, null, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestEmptyUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, "", Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.IsFalse(Server.ContainsConnection(""));
            s0.LogOut();
        }

        [TestMethod]
        [Timeout (10000)]
        public void DoubleLogIn()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            while (!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            s0.SendDummyLogIn("helen","potato", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, });
            Task<ProtoMessage> reply = s0.GetReply();
            reply.Wait();
            Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, reply.Result.ResponseType);
            s0.LogOut();
        }

        /// <summary>
        /// Tests that a log in will time out correctly
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTimeout()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Task<string> reply = s0.GetServerMessage();
            reply.Wait();
            Assert.AreEqual("Failed to login due to timeout", reply.Result);
            Assert.AreEqual(s0.net.GetConnectionStatus(), "Disconnected");
            s0.LogOut();
        }


        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestNotLoggedIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.ConnectNoLogin(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            while(!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            this.AdjustExpenditureTest(s0,"notafief",50,50,50,50,50);
            s0.LogOut();
        }

        
        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestBadFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username,Pass,new byte[]{1,2,3,4,5,6});
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.AdjustExpenditureTest(s0, "notafief", 50, 50, 50, 50, 50);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestNotOwnedFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.AdjustExpenditureTest(s0, NotOwnedFief.id, 50, 50, 50, 50, 50);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureBadData()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.AdjustExpenditureTest(s0, OwnedFief.id,-1,-1,-2,-1,-1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureSuccess()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.AdjustExpenditureTest(s0, OwnedFief.id, 1,1,1,1,1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTooMuch()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.AdjustExpenditureTest(s0, OwnedFief.id, 100000, 1000000, 11000000, 11000000, 11000000);
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackValid()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(s0,OwnedArmy.armyID,NotOwnedArmy.armyID);
            s0.LogOut();
        }


        [TestMethod]
        [Timeout(8000)]
        public void AttackNotOwnedArmy()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(s0, NotOwnedArmy.armyID, OwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackMyself()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(s0, OwnedArmy.armyID, OwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackBadArmy()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(s0, OwnedArmy.armyID, "NotanArmyId");
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(8000)]
        public void AttackNullArmy()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(s0, OwnedArmy.armyID, null);
            s0.LogOut();
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
        [Timeout(15000)]
        public void RecruitValidCancel()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                this.RecruitTroopsTest(s0,null,50,false);
            }
            else
            {
                this.RecruitTroopsTest(s0, OwnedArmy.armyID, 50, false);
            }
            
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitValidConfirm()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                this.RecruitTroopsTest(s0, null, 50, true);
            }
            else
            {
                this.RecruitTroopsTest(s0, OwnedArmy.armyID, 50, true);
            }

            s0.LogOut();
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitInvalidAlreadyRecruited()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                this.RecruitTroopsTest(s0, null, 50, true);
            }
            else
            {
                this.RecruitTroopsTest(s0, OwnedArmy.armyID, 50, true);
            }
            this.RecruitTroopsTest(s0, null, 50, true);
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(15000)]
        public void RecruitThenTravel()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            if (OwnedArmy == null)
            {
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                s0.RecruitTroops(null, 50, true);
            }
            else
            {
                s0.RecruitTroops(OwnedArmy.armyID, 50, true);
            }
            Thread.Sleep(1000);
            s0.ClearMessageQueues();
            s0.Move(MyPlayerCharacter.charID, OwnedFief.id);
            Task<ProtoMessage> responseTask = s0.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
#if STRICT
            Assert.AreEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
#else
            Assert.AreNotEqual(DisplayMessages.ErrorGenericMessageInvalid, response.ResponseType);
#endif
            s0.LogOut();
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyValid()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
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
            this.SpyArmyTest(s0, MyPlayerCharacter.charID, NotOwnedArmy.armyID ,true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyCancel()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            NotOwnedArmy.location = MyPlayerCharacter.location.ToString();
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(s0, MyPlayerCharacter.charID, NotOwnedArmy.armyID, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyNotOwned()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            NotOwnedArmy.location = MyPlayerCharacter.location.ToString();
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(s0, NotMyPlayerCharacter.charID, NotOwnedArmy.armyID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyTooFar()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.SpyArmyTest(s0, MyPlayerCharacter.charID, NotOwnedArmy.armyID, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(35000)]
        public void SpyArmyTimeout()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            // (need to move the character to the same location as the playercharacter)
            NotOwnedArmy.location = MyPlayerCharacter.location.id;
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            s0.SpyOnArmy(MyPlayerCharacter.charID, NotOwnedArmy.armyID);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = s0.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = s0.GetReply();
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
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            MyPlayerCharacter.location = NotOwnedFief;
            this.SpyFiefTest(s0, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefCancel()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();

            MyPlayerCharacter.location = NotOwnedFief;
            this.SpyFiefTest(s0, MyPlayerCharacter.charID, NotOwnedFief.id, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefNotOwned()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.SpyFiefTest(s0, NotMyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefTooFar()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            this.SpyFiefTest(s0, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(35000)]
        public void SpyFiefTimeout()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            MyPlayerCharacter.location = NotOwnedFief;
            s0.SpyOnFief(MyPlayerCharacter.charID, NotOwnedFief.id);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = s0.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = s0.GetReply();
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
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(s0, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterCancel()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(s0, MyPlayerCharacter.charID, NotMyFamily.charID, false);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterNotOwned()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(NotMyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(s0, NotMyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterTooFar()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            this.SpyCharacterTest(s0, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        /// <summary>
        /// Attempt to spy on a character, then timeout
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterTimeout()
        {
            TestClient s0 = new TestClient();
            s0.LogInAndConnect(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            s0.SpyOnCharacter(MyPlayerCharacter.charID, NotMyFamily.charID);
            Thread.Sleep(31000);
            Task<ProtoMessage> responseTask = s0.GetReply();
            responseTask.Wait();
            ProtoMessage response = responseTask.Result;
            Assert.AreEqual(DisplayMessages.SpyChance, response.ResponseType);
            responseTask = s0.GetReply();
            responseTask.Wait();
            response = responseTask.Result;

            Assert.AreEqual(DisplayMessages.Timeout, response.ResponseType);
        }
    }
}
