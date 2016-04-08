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
            this.LogInTest(s0, OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
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
            s0.ConnectNoLogin(OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(Username));
        }

        [TestMethod]
        [Timeout(15000)]
        public void LogInTestBadPassword()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUser,OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
            Assert.IsFalse(Server.ContainsConnection(Username));
        }

        [TestMethod]
        [Timeout(15000)]
        public void LogInTestNullPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUser, null, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(Username));
            s0.LogOut();
        }

        [TestMethod]
        [Timeout(15000)]
        public void LogInTestNoKey()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUser, OtherPass, new byte[] { });
            Assert.IsFalse(Server.ContainsConnection(Username));
            s0.LogOut();
        }

        [TestMethod]
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
        [Timeout(10000)]
        public void DoubleLogIn()
        {
            client.ClearMessageQueues();
            client.SendDummyLogIn("helen", "potato", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, });
            Thread.Sleep(1000);
            Assert.IsTrue(client.IsConnectedAndLoggedIn() == false);
            
        }


        /// <summary>
        /// Tests that a log in will time out correctly
        /// </summary>
        [TestMethod]
        [Timeout(40000)]
        public void LogInTimeout()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
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
            s0.ConnectNoLogin(OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            while (!s0.net.GetConnectionStatus().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            this.AdjustExpenditureTest(s0, OwnedFief.id, 50, 50, 50, 50, 50);
            s0.LogOut();
        }


        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTestBadFief()
        {
            this.AdjustExpenditureTest(client, "notafief", 50, 50, 50, 50, 50);
            
        }

        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTestNotOwnedFief()
        {
            this.AdjustExpenditureTest(client, NotOwnedFief.id, 50, 50, 50, 50, 50);
            
        }

        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureBadData()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, -1, -1, -2, -1, -1);
            
        }

        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureSuccess()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, 1, 1, 1, 1, 1);
            
        }

        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTooMuch()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, 100000, 1000000, 11000000, 11000000, 11000000);
            
        }

        [TestMethod]
        [Timeout(15000)]
        public void AttackValid()
        {
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, NotOwnedArmy.armyID);
        }


        [TestMethod]
        [Timeout(8000)]
        public void AttackNotOwnedArmy()
        {
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
            client.ClearMessageQueues();
            OwnedArmy.location = NotOwnedArmy.location;
            if (OwnedArmy.GetLeader() != null)
            {
                OwnedArmy.GetLeader().location.id = NotOwnedArmy.location;
            }
            this.AttackTest(client, OwnedArmy.armyID, null);
        }

        [TestMethod]
        [Timeout(30000)]
        public void AttackTooFarFromArmy()
        {
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
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                this.RecruitTroopsTest(client, null, 50, false);
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
                Console.WriteLine("Do not own an army!");
                Console.WriteLine("PlayerCharacter " + MyPlayerCharacter.charID + "( " + MyPlayerCharacter.firstName +
                                  " " + MyPlayerCharacter.familyName + " has " + MyPlayerCharacter.myArmies.Count +
                                  " armies");
                this.RecruitTroopsTest(client, null, 50, true);
            }
            else
            {
                this.RecruitTroopsTest(client, OwnedArmy.armyID, 50, true);
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
            this.RecruitTroopsTest(client, OwnedArmy.armyID, 50, true);
            this.RecruitTroopsTest(client, OwnedArmy.armyID , 50, true); 
            
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
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID);
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
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID);
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
            this.SpyArmyTest(client, NotMyPlayerCharacter.charID, NotOwnedArmy.armyID);
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
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID);
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
        public void SpyOnOwnCharacter()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            MyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, MyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyOnOwnFief()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            MyPlayerCharacter.MoveTo(OwnedFief.id, out ignore);
            this.SpyFiefTest(client, MyPlayerCharacter.charID, OwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyOnOwnArmy()
        {
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            client.ClearMessageQueues();
            ProtoMessage ignore = null;
            MyPlayerCharacter.MoveTo(OwnedArmy.location, out ignore);
            // (need to move the character to the same location as the playercharacter)
            this.SpyArmyTest(client, MyPlayerCharacter.charID, OwnedArmy.armyID);
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

        
    }
}
