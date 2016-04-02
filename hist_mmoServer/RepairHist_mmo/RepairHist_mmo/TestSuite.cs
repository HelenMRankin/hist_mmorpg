using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using ProtoBuf;
using System.IO;
using System.Diagnostics.Contracts;
namespace hist_mmorpg
{
    public class TestSuite
    {
        public static StreamWriter LogFile;
        public Game game;
        public Server server;
        public TestSuite()
        {
            LogFile=new StreamWriter("TestLog"+System.DateTime.Today.ToString("dd_MM_yy")+".txt");
            LogFile.AutoFlush = true;
            
            try
            {
                using (Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt"))
                {
                    Globals_Server.LogFile.AutoFlush = true;
                    Globals_Server.logEvent("Server start");

                    game = new Game();
                    server = new Server();
                    //client.LogIn("helen", "potato");
                    String s = Console.ReadLine();
                    if (s != null && s.Equals("exit"))
                    {
                        Globals_Server.logEvent("Server exits");
                        Globals_Server.server.Shutdown("Server exits");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered an error:" +e.Message+ "\n TRACE: "+ e.StackTrace);
                Console.ReadLine();
            } 
        }

        public static void LogData(string type, string data)
        {
            LogFile.WriteAsync(type + ":\t" + data + "\n");
        }
        /// <summary>
        /// Get approximate memory used by the program
        /// </summary>
        /// <param name="vMem">Virtual Memory</param>
        /// <param name="wMem">Working Memory</param>
        public void GetMemoryUsage(out long vMem, out long wMem)
        {
            Process p = Process.GetCurrentProcess();
            wMem = p.WorkingSet64;
            vMem = p.VirtualMemorySize64;
            p.Dispose();
        }


        /// <summary>
        /// Test the LogIn functionality
        /// If successful user should be logged in and in Server's client connections
        /// if the username is not in the database, the user should get a message stating it is not recognised
        /// if the password is wrong, the user should 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task TestLogIn(TestClient client, string user, string pass, byte[] key)
        {
            Contract.Requires(client != null);
            long vMem, wMem;
            // Get initial memory usage
            GetMemoryUsage(out vMem, out wMem);
            // Use NetPeer to send login to server
            try {
                client.LogInAndConnect(user, pass);
                int timeout = 1000;
                var reply = client.GetReply();
                if (await Task.WhenAny(reply, Task.Delay(timeout)) == reply)
                {
                    Contract.Assert(reply.Result.GetType() == typeof(ProtoLogIn));
                }
                else {
                    reply.Dispose();
                    
                }
                reply = client.GetReply();
                if (await Task.WhenAny(reply, Task.Delay(timeout)) == reply)
                {
                    Contract.Assert(reply.Result.GetType() == typeof(ProtoClient));
                }
                else {
                    reply.Dispose();

                }
            }
            catch(Exception e)
            {
                LogData("LogInException", e.Message);
            }
            GetMemoryUsage(out vMem, out wMem);
        }
    }
}
