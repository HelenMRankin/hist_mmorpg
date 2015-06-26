using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Lidgren.Network;
using System.Diagnostics;
namespace TestLidgren
{
    class Program
    {
        static void Main(string[] args)
        {
           /* IPAddress ip = NetUtility.Resolve("localhost");
            Console.WriteLine(ip.ToString());
            Server s = new Server();
            Client c = new Client("client");
            Thread t = new Thread(new ThreadStart(s.read));
            t.Start();
            c.Connect(ip.ToString(), 8000);
            Thread.Sleep(1000);
         //   s.toggleListen();

            c.Send("Hello? HELLO??");
            c.Send("Can you hear me?");
            c.Send("third times a charm");
            
            Thread.Sleep(5000);
            c.Send("are you getting these?");
         //   s.toggleListen();
            Console.Read();*/
			String hostname = Dns.GetHostName ();
			Console.WriteLine (hostname);
			IPAddress ip = NetUtility.Resolve("localhost");
			Console.WriteLine (ip.ToString ());
            Server s = new Server();
            Thread t_server = new Thread(new ThreadStart(s.read));
            t_server.Start();
            Stopwatch timer = new Stopwatch();
            Thread.Sleep(500);
            Thread t_timer = new Thread(new ThreadStart(timer.Start));   
            List<Client> clients = new List<Client>();
            t_timer.Start();
            Console.WriteLine("test");
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("Trying to create client");
                Client c = new Client("client" + i);
                c.Connect(ip.ToString(), 8000);
                clients.Add(c);
            }
            long connect = timer.ElapsedMilliseconds;
            Console.WriteLine("Connect time: " + connect);
            foreach (Client c in clients)
            {
                c.SendViaProto();
                //c.Send("a message or two");
                Thread.Sleep(10);
                //c.Send("or 3");
            }
            Console.WriteLine("TotalTime: " + timer.ElapsedMilliseconds);
            timer.Stop();
            //   s.toggleListen();
            Console.Read();
        }
    }

}
