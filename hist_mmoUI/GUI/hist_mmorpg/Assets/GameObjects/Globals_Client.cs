using UnityEngine;
using System.Collections;
using hist_mmorpg;
public static class Globals_Client {
	/// <summary>
	/// The current location.
	/// </summary>
	public static Fief currentLocation;
	/// <summary>
	/// The travel modifier. Based on current season and army movement stats
	/// </summary>
	public static double TravelModifier;
	/// <summary>
	/// Checks whether own character is in keep
	/// </summary>
	public static bool inKeep;
	public static double days;
	public static uint purse;
	public static string[] goTo;
	public static double homeTreasury;
	public static string pcID;
	/// <summary>
	/// Holds ID of character currently performing actions with (e.g. camping, moving etc). 
	/// </summary>
	public static string activeChar;
	public static string activeCharName;
	public static ProtoCharacter activeCharacter;
	public static ProtoCharacter playerCharacter;
	// Holds number of unread entries
	public static int unreadEntries=0;
}
