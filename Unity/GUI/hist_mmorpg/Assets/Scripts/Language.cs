using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
/// <summary>
/// Class storing data on language
/// </summary>
public class Language
{
	/// <summary>
	/// Holds language ID
	/// </summary>
	public String id { get; set; }
	/// <summary>
	/// Holds base language
	/// </summary>
	public BaseLanguage baseLanguage { get; set; }
	/// <summary>
	/// Holds language dialect code
	/// </summary>
	public int dialect { get; set; }
	
	/// <summary>
	/// Constructor for Language
	/// </summary>
	/// <param name="bLang">BaseLanguage for the language</param>
	/// <param name="dial">int holding language dialect code</param>
	public Language(BaseLanguage bLang, int dial)
	{
		this.baseLanguage = bLang;
		this.dialect = dial;
		this.id = this.baseLanguage.id + this.dialect;
	}
	
	/// <summary>
	/// Constructor for Language taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public Language()
	{
	}
	
	
	/// <summary>
	/// Gets the name of the language
	/// </summary>
	/// <returns>string containing the name</returns>
	public string GetName()
	{
		return this.baseLanguage.name + " (dialect " + this.dialect + ")";
	}
}

/// <summary>
/// Class storing base langauge data
/// </summary>
public class BaseLanguage
{
	/// <summary>
	/// Holds base langauge ID
	/// </summary>
	public String id { get; set; }
	/// <summary>
	/// Holds base language name
	/// </summary>
	public String name { get; set; }
	
	/// <summary>
	/// Constructor for BaseLanguage
	/// </summary>
	/// <param name="id">String holding language ID</param>
	/// <param name="nam">String holding language name</param>
	public BaseLanguage(String id, String nam)
	{
		this.id = id;
		this.name = nam;
	}
	
	/// <summary>
	/// Constructor for BaseLanguage taking no parameters.
	/// For use when de-serialising.
	/// </summary>
	public BaseLanguage()
	{
	}
}