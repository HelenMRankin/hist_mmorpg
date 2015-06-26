using System;
using RiakClient;
using RiakClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace RiakTest
{
	class MainClass
	{
		static void Main(string[] args)
		{
			Tester t = new Tester ();
			t.runProgram ();
		}
	}
}
