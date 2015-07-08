using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public abstract class Place
{
	/// <summary>
	/// Holds place ID
	/// </summary>
	public String id { get; set; }
	/// <summary>
	/// Holds place name
	/// </summary>
	public String name { get; set; }
	/// <summary>
	/// Holds place title holder (charID)
	/// </summary>
	public String titleHolder { get; set; }
	/// <summary>
	/// Holds place rank (Rank object)
	/// </summary>
	public Rank rank { get; set; }
	
	/// <summary>
	/// Constructor for Place
	/// </summary>
	/// <param name="id">String holding place ID</param>
	/// <param name="nam">String holding place name</param>
	/// <param name="tiHo">String holding place title holder (charID)</param>
	/// <param name="own">Place owner (PlayerCharacter)</param>
	/// <param name="rnk">Place rank (Rank object)</param>
	public Place(String id, String nam, String tiHo, Rank r)
	{
		this.id = id;
		this.name = nam;
		this.titleHolder = tiHo;
		this.rank = r;
		
	}
	
	
	/// <summary>
	/// Constructor for Place taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Place()
	{
	}
}