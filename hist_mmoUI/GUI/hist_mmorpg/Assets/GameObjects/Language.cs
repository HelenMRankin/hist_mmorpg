using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
/// <summary>
/// Class storing data on language
/// </summary>
[Serializable()]
public class Language : ISerializable
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

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("LangID", this.id, typeof(string));
		this.baseLanguage.GetObjectData(info, context);
		info.AddValue("dia", this.dialect, typeof(int));
	}
	
	public Language(SerializationInfo info, StreamingContext context)
	{
		this.id = info.GetString("LangID");
		this.baseLanguage = new BaseLanguage(info, context);
		this.dialect = info.GetInt32("dia");
		Globals_Game.languageMasterList.Add (this.id, this);
	}
}

/// <summary>
/// Class storing base langauge data
/// </summary>
[Serializable()]
public class BaseLanguage : ISerializable 
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
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("baseID", this.id, typeof(string));
		info.AddValue("baseNam", this.name, typeof(string));
	}
	
	public BaseLanguage(SerializationInfo info, StreamingContext context)
	{
		this.id = info.GetString("baseID");
		this.name = info.GetString("baseNam");
		Globals_Game.baseLanguageMasterList.Add (this.id, this);
	}
}