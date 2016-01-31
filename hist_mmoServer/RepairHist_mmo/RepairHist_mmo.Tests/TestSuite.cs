// <copyright file="TestSuiteTest.cs">Copyright ©  2015</copyright>
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hist_mmorpg;
namespace hist_mmorpg.Tests
{
    /// <summary>This class contains parameterized unit tests for TestSuite</summary>
    [PexClass(typeof(TestSuite))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TestSuite
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
        }

        [ClassCleanup()]
        public static void FinaliseGameState()
        {
            server.Shutdown();
            Globals_Server.LogFile.Close();
        }

        [PexMethod]
        internal void LogInTest(
            [PexAssumeUnderTest] TestClient client,
            string user,
            string pass,
            byte[] key
            )
        {
            
            client.LogIn(user,pass,key);
            // If username not recognised, expect to be disconnected
            
            if (user == null ||!server.logInManager.users.ContainsKey(user))
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
                return;
            }
            // If password is incorrect, expect an error
            Tuple<byte[],byte[]> hashNsalt = server.logInManager.users[user];
            byte[] hash = server.logInManager.ComputeHash(Encoding.UTF8.GetBytes(pass), hashNsalt.Item2);
            if (!hashNsalt.Item1.SequenceEqual(hash))
            {
                Assert.AreEqual("Disconnected", client.net.GetConnectionStatus());
            }
            else
            {
                // If the login was successful, expecting a ProtoClient back
                Task<ProtoMessage> getReply = client.GetReply();
                getReply.Wait();
                ProtoMessage reply = getReply.Result;
                Assert.AreEqual(reply.GetType(), typeof (ProtoClient));

                // If login was successful, the client should be in the list of registered observers
                Assert.IsTrue(Globals_Game.IsObserver(Globals_Server.clients[user]));
            }
        }


    }
}
