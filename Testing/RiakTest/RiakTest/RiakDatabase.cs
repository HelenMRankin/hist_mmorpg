using System;
using RiakClient;
using RiakClient.Models;
using System.IO;
using System.Diagnostics;
namespace RiakTest
{
	public class RiakDatabase
	{
		IRiakClient client;
		public RiakDatabase (IRiakClient client)
		{
			this.client = client;
		}

		public void insertStrings(string[] keys, string[] values) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				RiakObject pair = new RiakObject ("strings", keys [i], values [i]);
				client.Put (pair);
			/*	if (client.Put (pair).IsSuccess) {
					Console.WriteLine ("Wrote string " + values [i]);
				}*/
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("Riak string insert total time: "+time+" , per item: "+time_per);
		}

		public void insertInts(string[] keys, int[] values) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				RiakObject pair = new RiakObject ("ints", keys [i], values [i]);
				RiakResult result= client.Put (pair);
				if (result.ResultCode != ResultCode.Success) {
					Console.WriteLine ("error in insert!" +result.ErrorMessage);
					break;
				}
				/*if (client.Put (pair).IsSuccess) {
					Console.WriteLine ("Wrote int " + values [i]);
				}*/
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("Riak int insert total time: "+time+" , per item: "+time_per);
		}

		public void getInts(string[] keys) {
			Stopwatch timer = Stopwatch.StartNew ();
			for(int i = 0;i<keys.Length;i++) {
				RiakResult result = client.Get("ints",keys[i]);
				if (result.ResultCode != ResultCode.Success) {
					Console.WriteLine ("error in insert!");
					break;
				}
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("Riak int get total time: "+time+" , per item: "+time_per);
		}

		public void deleteInts(string[] keys) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				RiakResult result = client.Delete ("ints", keys [i]);
				if (result.ErrorMessage != null) {
					Console.WriteLine ("error in insert!" + result.ErrorMessage);
					break;
				}
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("Riak int delete total time: "+time+" , per item: "+time_per);
		}
	}
}

