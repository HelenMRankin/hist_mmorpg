using NUnit.Framework;
using System;

namespace hist_mmorpg
{
	[TestFixture ()]
	public class UnityTests
	{
		/**** Test every action from Client ****/

		[Test ()]
		/// <summary>
		/// Tests the UseChar action
		/// 
		/// </summary>
		public void TestUseChar () {
			Game game = new Game();
			Server server = new Server();
			Client client = Globals_Server.clients["test"];

			Character captive_char = Globals_Game.npcMasterList["Char_626"];
			Character dead_char = Globals_Game.npcMasterList["Char_754"];
			Character ok_char = Globals_Game.npcMasterList["Char_731"];
			Program.testCaptives ();
			Character notOwned_Char = Globals_Game.pcMasterList["Char_158"]; 
			// Predicates
			// Character belongs to Player
			// Character is alive
			// Character is not a captive
			ProtoMessage testMessage = new ProtoMessage();
			testMessage.ActionType = Actions.UseChar;
			testMessage.Message = ok_char.charID;
			ProtoMessage response = Game.ActionController (testMessage, client.myPlayerCharacter);
			Assert.AreEqual (DisplayMessages.Success, response.ResponseType);

			testMessage.Message = captive_char.charID;
			response = Game.ActionController (testMessage, client.myPlayerCharacter);
			Assert.AreNotEqual (DisplayMessages.Success, response.ResponseType);

			dead_char.ProcessDeath ();
			testMessage.Message = dead_char.charID;
			response = Game.ActionController (testMessage, client.myPlayerCharacter);
			Assert.AreNotEqual (DisplayMessages.Success, response.ResponseType);

			testMessage.Message = notOwned_Char.charID;
			response = Game.ActionController (testMessage, client.myPlayerCharacter);;
			Assert.AreNotEqual (DisplayMessages.Success, response.ResponseType);
		}

		[Test ()]
		public void TestCase ()
		{

		}
	}
}

