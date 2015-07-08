using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Class storing data on province
/// </summary>
public class Province : Place
{
	/// <summary>
	/// Holds province tax rate
	/// </summary>
	public Double taxRate { get; set; }
	/// <summary>
	/// Holds province kingdom object
	/// </summary>
	public Kingdom kingdom { get; set; }
	
	/// <summary>
	/// Constructor for Province
	/// </summary>
	/// <param name="otax">Double holding province tax rate</param>
	/// <param name="king">Province's Kingdom object</param>
	public Province(String id, String nam, Double otax, String tiHo = null, Kingdom king = null, Rank r = null)
		: base(id, nam, tiHo, r)
	{
		this.taxRate = otax;
		this.kingdom = king;
	}
	
	/// <summary>
	/// Constructor for Province taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Province()
	{
	}
	
	
}
