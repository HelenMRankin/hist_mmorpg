using System;
using RiakClient;
using RiakClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RiakTest
{
	public class Tester
	{
		IRiakEndPoint cluster;
		static int THREAD_COUNT =200;

		void t_RiakInsert(Object intsObj) {
			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			IRiakClient client = cluster.CreateClient();
			var pingResult = client.Ping();
			if (pingResult.IsSuccess)
			{
				RiakDatabase rDB = new RiakDatabase (client);
				rDB.insertInts (ints.Keys.ToArray (), ints.Values.ToArray ());
			}
		}
		void t_RiakFetch(Object intsObj) {

			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			IRiakClient client = cluster.CreateClient();
			var pingResult = client.Ping();
			if (pingResult.IsSuccess)
			{
				RiakDatabase rDB = new RiakDatabase (client);
				rDB.getInts (ints.Keys.ToArray ());
			}
		}

		void t_RiakDelete(Object intsObj) {
			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			IRiakClient client = cluster.CreateClient();
			var pingResult = client.Ping();
			if (pingResult.IsSuccess)
			{
				RiakDatabase rDB = new RiakDatabase (client);
				rDB.deleteInts (ints.Keys.ToArray ());
			}
		}
		void t_MySQLInsert(Object intsObj) {
			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			MySQLDatabase sqlDB = new MySQLDatabase ();
			sqlDB.insertInts (ints.Keys.ToArray (), ints.Values.ToArray ());
		}

		void t_MySQLFetch(Object intsObj) {
			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			MySQLDatabase sqlDB = new MySQLDatabase ();
			sqlDB.getInts (ints.Keys.ToArray ());
		}

		void t_MySQLDelete(Object intsObj) {
			Dictionary<string,int> ints = (Dictionary<string,int>)intsObj;
			MySQLDatabase sqlDB = new MySQLDatabase ();
			sqlDB.deleteInts (ints.Keys.ToArray ());
		}

		public void runProgram()
		{
			this.cluster = RiakCluster.FromConfig("riakConfig","app.config");
			for (int i = 0; i < THREAD_COUNT; i++) {
				DataSet data = new DataSet (THREAD_COUNT, i);
				Object ints = data.getInts();
				Thread tR = new Thread (new ParameterizedThreadStart(this.t_RiakInsert));
			 	Thread tM = new Thread (new ParameterizedThreadStart(this.t_MySQLInsert));
				Thread tR_f = new Thread (new ParameterizedThreadStart(this.t_RiakFetch));
				Thread tM_f = new Thread (new ParameterizedThreadStart(this.t_MySQLFetch));
				Thread tM_d = new Thread (new ParameterizedThreadStart (this.t_MySQLDelete));
				Thread tR_d = new Thread (new ParameterizedThreadStart (this.t_RiakDelete));

				//tR.Start(ints);	
				//tR_f.Start (ints);
				//tR_d.Start(ints);

				tM.Start (ints);
				//tM_f.Start (ints);
				//tM_d.Start (ints);
			}
		}
	}
}

