using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
public static class Globals_Game
{
    /// <summary>
    /// Holds all Fief objects
    /// </summary>
    public static Dictionary<string, Fief> fiefMasterList = new Dictionary<string, Fief>();
    /// <summary>
    /// Holds all Province objects
    /// </summary>
    public static Dictionary<string, Province> provinceMasterList = new Dictionary<string, Province>();
    /// <summary>
    /// Holds all Kingdom objects
    /// </summary>
    public static Dictionary<string, Kingdom> kingdomMasterList = new Dictionary<string, Kingdom>();
    /// <summary>
    /// Holds all Rank objects
    /// </summary>
    public static Dictionary<byte, Rank> rankMasterList = new Dictionary<byte, Rank>();
    /// <summary>
    /// Holds all GameTerrain objects
    /// </summary>
    public static Dictionary<string, GameTerrain> terrainMasterList = new Dictionary<string, GameTerrain>();
    /// <summary>
    /// Holds all BaseLanguage objects
    /// </summary>
    public static Dictionary<string, BaseLanguage> baseLanguageMasterList = new Dictionary<string, BaseLanguage>();
    /// <summary>
    /// Holds all Language objects
    /// </summary>
    public static Dictionary<string, Language> languageMasterList = new Dictionary<string, Language>();
    /// <summary>
    /// Holds all nationality objects
    /// </summary>
    public static Dictionary<string, Nationality> nationalityMasterList = new Dictionary<string, Nationality>();
    /// <summary>
    /// Holds all position objects
    /// </summary>
    public static Dictionary<byte, Position> positionMasterList = new Dictionary<byte, Position>();
	/// <summary>
	/// Holds HexMapGraph for this game
	/// </summary>
	public static HexMapGraph gameMap;
	/// <summary>
	/// Obtains a message to display from an enum 
	/// </summary>
	public static Dictionary<DisplayMessages,string> displayMessages = new Dictionary<DisplayMessages,string>();

	public static void LoadStrings() {
		try {
			string line;
			StreamReader reader = new StreamReader(Application.dataPath+"/DataFiles/strings.txt",Encoding.UTF7);
			using(reader) {
				line = reader.ReadLine ();
				while(line!=null) {
					Debug.Log ("Splitting line: "+line);
					string[] tokenised = line.Split ('~');
					Debug.Log ("Key: "+tokenised[0] + ", Val: "+tokenised[1]);
					DisplayMessages messageType = (DisplayMessages) Enum.Parse (typeof(DisplayMessages),tokenised[0]);
					displayMessages.Add(messageType,tokenised[1]);
					line = reader.ReadLine ();
				}
			}
		}
		catch(Exception e) {
			Debug.LogError("Exception in LoadStrings: "+e.Message);
		}
	}
}
