using System;
using MySql.Data.MySqlClient;
using MySql.Data;
using MySql.Data.Entity;
using System.Data.Common;
using System.Diagnostics;
namespace RiakTest
{
	public class MySQLDatabase
	{
		private MySqlConnection con;
		private string host;
		private string user;
		private string pass;
		private string database;
		private readonly int MAX_PROFILES = 100;
		public MySQLDatabase ()
		{
			host = "localhost";
			user = "root";
			database = "helen";
			pass = "mup1020trup";
			string connectionString = "SERVER=" + host + ";" + "DATABASE=" + 
				database + ";" + "UID=" + user + ";" + "PASSWORD=" + pass + ";" + "maximumpoolsize=200;";
			con = new MySqlConnection (connectionString);
			con.Open ();
			if (con.Ping ()) {
				Console.WriteLine ("SQL ping success");
				MySqlCommand cmd = new MySqlCommand ("SET PROFILING = 1",con);
				cmd.ExecuteNonQuery ();
				MySqlCommand cmd2 = new MySqlCommand ("SET profiling_history_size = "+MAX_PROFILES,con);
				cmd2.ExecuteNonQuery ();
			} else {
				Console.Write ("no ping :( ");
			}
		}

		public void viewResults() {
			string query = "SHOW PROFILEs";
			MySqlCommand cmd = new MySqlCommand (query, con);
			MySqlDataReader result = cmd.ExecuteReader ();
			double total = 0;
			while(result.Read()) {
				total += Double.Parse (result ["Duration"].ToString ());
			//	Console.Write (result ["Query"]);
			//	Console.Write(" " + result["Duration"]);
			}
			result.Close ();
			Console.WriteLine ("Time per query: "+ ((total*1000) / MAX_PROFILES) +"(ms)");
		}

		public void insertInts(string[] keys, int[] values) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				string query = "INSERT INTO ints VALUES('" + keys [i] + "','" + values [i] + "')";
				MySqlCommand cmd = new MySqlCommand (query, con);
				try {
					cmd.ExecuteNonQuery ();

				}
				catch(MySqlException e) {
					Console.WriteLine (e.Message);
				}
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("MySQL int insert total time: "+time+" , per item: "+time_per);
			viewResults ();
		}

		public void getInts(string[] keys) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				string query = "SELECT * FROM ints WHERE text = '" + keys [i] + "'";
				MySqlCommand cmd = new MySqlCommand (query, con);
				try {
					MySqlDataReader read = cmd.ExecuteReader ();
					read.Close();
				} catch (MySqlException e) {
					Console.WriteLine (e.Message);
				}
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("MySQL int select total time: "+time+" , per item: "+time_per);
			viewResults ();
		}
		public void deleteInts(string[] keys) {
			Stopwatch timer = Stopwatch.StartNew ();
			for (int i = 0; i < keys.Length; i++) {
				string query = "DELETE FROM ints WHERE text = '" + keys [i] + "'";
				MySqlCommand cmd = new MySqlCommand (query, con);
				try {
					MySqlDataReader read = cmd.ExecuteReader ();
					read.Close();
				} catch (MySqlException e) {
					Console.WriteLine (e.Message);
				}
			}
			long time = timer.ElapsedMilliseconds;
			double time_per = time / (double)(keys.Length);
			Console.WriteLine ("MySQL int delete total time: "+time+" , per item: "+time_per);
			viewResults ();
		}
			
	}
}

