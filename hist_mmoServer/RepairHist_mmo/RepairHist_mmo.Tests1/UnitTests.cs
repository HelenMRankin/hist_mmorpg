using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;

namespace hist_mmorpg.Tests1
{
    public partial class TestClientTest
    {

        /// <summary>
        /// Try to log in with valid credentials
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTestValid()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            while (!s0.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            s0.LogOut();

        }

        /// <summary>
        /// Try logging out before logging in
        /// </summary>
        [TestMethod]
        [Timeout(20000)]
        public void LogOutBeforeLogIn()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6 });
            while (s0.net.GetConnectionStatus()!=NetConnectionStatus.Connected||!Server.ContainsConnection(OtherUser))
            {
                Thread.Sleep(0);
            }
            s0.LogOut();
            Console.WriteLine("Connection status: " + s0.net.GetConnectionStatus().ToString() + " Server contains connection? " + Server.ContainsConnection(OtherUser));
            while ((s0.net.GetConnectionStatus()!=NetConnectionStatus.Disconnected)||(Server.ContainsConnection(OtherUser)))
            {
                Thread.Sleep(0);
            }
            Console.WriteLine("Connection status: " + s0.net.GetConnectionStatus().ToString() + " Server contains connection? " + Server.ContainsConnection(OtherUser));
            Assert.IsFalse(Server.ContainsConnection(OtherUser));
        }

        /// <summary>
        /// Log in with an invalid password
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTestBadPassword()
        {
            TestClient s0 = new TestClient();
            this.LogInTest(s0, OtherUser,"Notpass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(OtherUser));
            s0.LogOut();
        }

        /// <summary>
        /// Log in with no password
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTestNullPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUser, null, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.IsFalse(Server.ContainsConnection(OtherUser));
            s0.LogOut();
        }

        /// <summary>
        /// Log in without using an encryption key. This is valid, it simply will result in unencrypted messages
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTestNoKey()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, OtherUser, OtherPass, new byte[] { });
            s0.LogOut();
        }

        /// <summary>
        /// Log in with an unrecognised username
        /// </summary>
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

        /// <summary>
        /// Log in with no username
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void LogInTestNullUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, null, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
        }

        /// <summary>
        /// Log in with an empty string as a username
        /// </summary>
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

        /// <summary>
        /// Try logging in twice. The second log in should cause the server to terminate the connection
        /// </summary>
        [TestMethod]
        public void DoubleLogIn()
        {
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
            Assert.AreEqual(s0.net.GetConnectionStatusString(), "Disconnected");
            s0.LogOut();
        }

        /// <summary>
        /// Attempt to perform an action (in this case, adjusting expenditure) without logging in
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTestNotLoggedIn()
        {
            TestClient s0 = new TestClient();
            s0.ConnectNoLogin(OtherUser, OtherPass);
            while (!s0.net.GetConnectionStatusString().Equals("Connected"))
            {
                Thread.Sleep(0);
            }
            Console.WriteLine("TEST: Sending adjust expenditure data");
            this.AdjustExpenditureTest(s0, OwnedFief.id, 50, 50, 50, 50, 50);
            s0.LogOut();
        }

        /// <summary>
        /// Try to adjust the expenditure for a non-existent fief
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTestBadFief()
        {
            this.AdjustExpenditureTest(client, "notafief", 50, 50, 50, 50, 50);
            
        }

        /// <summary>
        /// Try to adjust the expenditure of someone else's fief
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTestNotOwnedFief()
        {
            this.AdjustExpenditureTest(client, NotOwnedFief.id, 50, 50, 50, 50, 50);
            
        }

        /// <summary>
        /// Try to adjust the expenditure using invalid numbers
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureBadData()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, -1, -1, -2, -1, -1);
            
        }

        /// <summary>
        /// Adjust the expenditure of your own fief (should succeed)
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureSuccess()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, 1, 1, 1, 1, 1);
            
        }

        /// <summary>
        /// Try to adjust your expenditure past what funds you have
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void AdjustExpenditureTooMuch()
        {
            this.AdjustExpenditureTest(client, OwnedFief.id, 100000, 1000000, 11000000, 11000000, 11000000);
            
        }

        /// <summary>
        /// Attack another army (should succeed)
        /// </summary>
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

        [TestMethod]
        [Timeout(15000)]
        public void MaintainArmyValid()
        {
            // Add to treasury to ensure the army can be maintained
            int treasuryOld = MyPlayerCharacter.GetHomeFief().GetAvailableTreasury();
            MyPlayerCharacter.GetHomeFief().AdjustTreasury(200000);
            this.MaintainArmyTest(client, OwnedArmy.armyID);
            // Set army maintenance status to unmaintained
            OwnedArmy.isMaintained = false;
            // Reset treasury to old amount
            MyPlayerCharacter.GetHomeFief().AdjustTreasury(treasuryOld-MyPlayerCharacter.GetHomeFief().GetAvailableTreasury());

        }

        [TestMethod]
        [Timeout(15000)]
        public void MaintainArmyNotOwned()
        {
            this.MaintainArmyTest(client, NotOwnedArmy.armyID);
        }

        [TestMethod]
        [Timeout(15000)]
        public void MaintainArmyBadID()
        {
            this.MaintainArmyTest(client, null);
            this.MaintainArmyTest(client, "");
            this.MaintainArmyTest(client, "\0");
            this.MaintainArmyTest(client,"NotarmyID");
        }
        [TestMethod]
        [Timeout(15000)]
        public void MaintainArmyAlreadyMaintained()
        {
            OwnedArmy.isMaintained = true;
            this.MaintainArmyTest(client, OwnedArmy.armyID);
        }

        [TestMethod]
        [Timeout(15000)]
        public void MaintainArmyInsufficientFunds()
        {
            // Remove from treasury to ensure the army can be maintained
            int treasuryOld = MyPlayerCharacter.GetHomeFief().GetAvailableTreasury();
            MyPlayerCharacter.GetHomeFief().AdjustTreasury(-treasuryOld);
            // Ensure army has not been maintained already
            OwnedArmy.isMaintained = false;
            Assert.AreEqual(DisplayMessages.ArmyMaintainInsufficientFunds,this.MaintainArmyTest(client, OwnedArmy.armyID));

            // Reset treasury
            MyPlayerCharacter.GetHomeFief().AdjustTreasury(treasuryOld);
        }
        [TestMethod]
        [Timeout(15000)]
        public void RecruitValidCancel()
        {
           
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

        /// <summary>
        /// Spy on an army with a character you do not own
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyNotOwned()
        {
           
            NotOwnedArmy.location = MyPlayerCharacter.location.ToString();
            Character leader = NotOwnedArmy.GetLeader();
            if (leader != null)
            {
                leader.location = MyPlayerCharacter.location;
            }
            this.SpyArmyTest(client, NotMyPlayerCharacter.charID, NotOwnedArmy.armyID);
        }

        /// <summary>
        /// Spy on an army which is not in the same location as your spy
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyArmyTooFar()
        {
            this.SpyArmyTest(client, MyPlayerCharacter.charID, NotOwnedArmy.armyID);
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefValid()
        {
            
            MyPlayerCharacter.location = NotOwnedFief;
            this.SpyFiefTest(client, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }


        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefNotOwned()
        {
            
            this.SpyFiefTest(client, NotMyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyFiefTooFar()
        {
            
            this.SpyFiefTest(client, MyPlayerCharacter.charID, NotOwnedFief.id, true);
        }

        /// <summary>
        /// Spy on a character
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterValid()
        {
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }


        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterNotOwned()
        {
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            NotMyFamily.MoveTo(NotMyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, NotMyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyOnOwnCharacter()
        {
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            MyFamily.MoveTo(MyPlayerCharacter.location.id, out ignore);
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, MyFamily.charID, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyOnOwnFief()
        {
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            MyPlayerCharacter.MoveTo(OwnedFief.id, out ignore);
            this.SpyFiefTest(client, MyPlayerCharacter.charID, OwnedFief.id, true);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyOnOwnArmy()
        {
            ProtoMessage ignore = null;
            MyPlayerCharacter.MoveTo(OwnedArmy.location, out ignore);
            // (need to move the character to the same location as the playercharacter)
            this.SpyArmyTest(client, MyPlayerCharacter.charID, OwnedArmy.armyID);
        }

        [TestMethod]
        [Timeout(15000)]
        public void SpyCharacterTooFar()
        {
            ProtoMessage ignore = null;
            // (need to move the character to the same location as the playercharacter)
            this.SpyCharacterTest(client, MyPlayerCharacter.charID, NotMyFamily.charID, true);
        }

        /// <summary>
        /// Travel to a fief indicated by fief ID
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelValidFief()
        {
            this.TravelTest(client, MyPlayerCharacter.charID,NotOwnedFief.id,null);
        }

        /// <summary>
        /// Travel using instructions
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelValidInstructions()
        {
            this.TravelTest(client, MyPlayerCharacter.charID,null, new string[]{"E","W"});
        }

        /// <summary>
        /// Attempt to travel to an invalid fief
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelInvalidFief()
        {
            this.TravelTest(client, MyPlayerCharacter.charID, "Absolutelynotafief", null);
        }

        /// <summary>
        /// Attempt to travel with unrecognised instructions
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelInvalidInstructions()
        {
            this.TravelTest(client, MyPlayerCharacter.charID,null, new string[] {"X","Y","Z"});
        }

        /// <summary>
        /// Attempt to travel with a null fief id and no instructions
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelNoFiefOrInstructions()
        {
            this.TravelTest(client, MyPlayerCharacter.charID, null, null);
            this.TravelTest(client, MyPlayerCharacter.charID, "", null);
            this.TravelTest(client, MyPlayerCharacter.charID, "\0", null);
        }

        /// <summary>
        /// Attempt to travel using a character you do not own
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelNotOwnedCharacter()
        {
            this.TravelTest(client,NotMyPlayerCharacter.charID,NotOwnedFief.id,null);
        }

        /// <summary>
        /// Attempt to travel with invalid character IDs
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelInvalidCharacter()
        {
            this.TravelTest(client, null, NotOwnedFief.id, null);
            this.TravelTest(client, "", NotOwnedFief.id, null);
            this.TravelTest(client, "\0", NotOwnedFief.id, null);
        }

        /// <summary>
        /// Attempt to travel without enough days
        /// </summary>
        [TestMethod]
        [Timeout(15000)]
        public void TravelNotEnoughDays()
        {
            double oldDays = MyPlayerCharacter.days;
            MyPlayerCharacter.days = 1;
            this.TravelTest(client, MyPlayerCharacter.charID, NotOwnedFief.id,null);
            MyPlayerCharacter.days = oldDays;
        }

        [TestMethod]
        [Timeout(15000)]
        public void LogOutLogin()
        {
            client.LogOut();
            client.LogInAndConnect(Username,Pass,new byte[]{1,2,3,4,5,6,7});
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            Assert.IsTrue(client.IsConnectedAndLoggedIn());
        }

        [TestMethod]
        [Timeout(15000)]
        public void LogInTwiceAsDifferentUsers()
        {
            // Tell the client it isn't logged in
            client.net.loggedIn = false;
            // Try to log in as anothr user
            client.LogInAndConnect(OtherUser, OtherPass, new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            Assert.IsFalse(Globals_Game.IsObserver(OtherUser));
        }
    }
}
