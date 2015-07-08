using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Class storing data on kingdom
/// </summary>
public class Kingdom : Place
{
	/// <summary>
	/// Holds Kingdom nationality
	/// </summary>
	public Nationality nationality { get; set; }
	
	/// <summary>
	/// Constructor for Kingdom
	/// </summary>
	/// <param name="nat">Kingdom's Nationality object</param>
	public Kingdom(String id, String nam, Nationality nat, String tiHo = null, Rank r = null)
		: base(id, nam, tiHo, r)
	{
		this.nationality = nat;
	}
	
	/// <summary>
	/// Constructor for Kingdom taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Kingdom()
	{
	}

}

