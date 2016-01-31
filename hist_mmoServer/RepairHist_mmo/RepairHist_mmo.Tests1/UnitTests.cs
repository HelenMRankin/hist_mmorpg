using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
using hist_mmorpg.Tests;
namespace hist_mmorpg.Tests
{
    public partial class TestClientTest
    {
        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestValid()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network) null;
            this.LogInTest(s0, Username, Pass, new byte[] { 1,2,3,4,5,6,7,8,9});
            //s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogOutBeforeLogIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogOut();
        }
        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestBadPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, BadPass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();

        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestNullPassword()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, null, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestNoKey()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, Username, Pass, new byte[] { });
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void LogInTestInvalidUsername()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.LogInTest(s0, BadUsername, Pass, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
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
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestNotLoggedIn()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            this.AdjustExpenditureTest(s0,"notafief",50,50,50,50,50);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestBadFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username,Pass,new byte[]{1,2,3,4,5,6});
            this.AdjustExpenditureTest(s0, "notafief", 50, 50, 50, 50, 50);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTestNotOwnedFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustExpenditureTest(s0, NotOwnedFief.id, 50, 50, 50, 50, 50);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureBadData()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustExpenditureTest(s0, OwnedFief.id,-1,-1,-2,-1,-1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustExpenditureTest(s0, OwnedFief.id, 1,1,1,1,1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustExpenditureTooMuch()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustExpenditureTest(s0, OwnedFief.id, 100000, 1000000, 11000000, 11000000, 11000000);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionBadArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, "defonotanarmy",1,1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionNullArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, null, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionEmptyArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, "", 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionNotMyArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AdjustOddsAndAgressionTest(s0, NotOwnedArmy.armyID, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });

            this.AdjustOddsAndAgressionTest(s0, OwnedArmy.armyID, 1, 1);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AdjustOddsAndAggressionBadBytes()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });

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
            s0.LogIn(Username,Pass,new byte[]{1,2,3,4,5,6});
            this.AppointArmyLeaderTest(s0,OwnedArmy.armyID, MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNotOwnedArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0,NotOwnedArmy.armyID, MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNotOwnedCharacer()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0,OwnedArmy.armyID, NotMyEmplployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNullArmyID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, null, MyEmployee.charID);
            s0.LogOut();
        }
        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderEmptyArmyID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, "", MyEmployee.charID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderNullCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, OwnedArmy.armyID, null);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointArmyLeaderEmptyCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointArmyLeaderTest(s0, OwnedArmy.armyID, "");
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNotOwnedCharacter()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, NotMyEmplployee.charID, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNotOwnedFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, NotOwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNullFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, null);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffEmptyFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, MyFamily.charID, "");
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffNullCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, null, OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void AppointBailiffEmptyCharID()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.AppointBailiffTest(s0, "", OwnedFief.id);
            s0.LogOut();
        }

        [TestMethod] 
		 
		[Timeout(15000)]
        public void BesiegeOwnFief()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] {1, 2, 3, 4, 5, 6});
            this.BesiegeTest(s0,OwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void BesiegeNotOwnedArmy()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            this.BesiegeTest(s0, NotOwnedArmy.armyID);
            s0.LogOut();
        }

        [TestMethod] 
		[Timeout(15000)]
        public void BesiegeSuccess()
        {
            TestClient s0 = new TestClient();
            s0.net = (TestClient.Network)null;
            s0.LogIn(Username, Pass, new byte[] { 1, 2, 3, 4, 5, 6 });
            s0.Move(OwnedArmy.leader,NotOwnedFief.id);
            this.BesiegeTest(s0, OwnedArmy.armyID);
            s0.LogOut();
        }


    }
}
