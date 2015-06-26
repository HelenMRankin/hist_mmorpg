using System;
using System.Collections.Generic;
using System.Linq;
namespace RiakTest
{
	public class DataSet
	{
		public static int DATA_SIZE = 500;
		public static int THREADCOUNT;
		public int thread_num;
		public Dictionary<string, int> intMap;
		public Dictionary<string,string> stringMap;
		public DataSet (int threads, int thread_num)
		{
			THREADCOUNT = threads;
			this.thread_num = thread_num;
			intMap = new Dictionary<string,int> ();
			stringMap = new Dictionary<string,string> ();
			populate ();
		}

		public void populate() {
			for (int i = 0; i < DATA_SIZE; i++) {
				int j = i+(DATA_SIZE*thread_num);
				intMap.Add ("iKey_" + j,j);
				stringMap.Add ("sKey_" + j, "string" + j);
			}

		}

		public Dictionary<string,int> getInts() {
			return intMap;
		}

		public Dictionary<string,string>getStrings() {
			return stringMap;
		}
	}
}

