using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
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
            // VALIDATION

            // DIALECT
            if (dial < 1)
            {
                throw new InvalidDataException("Language dialect code must be an integer >= 0");
            }

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
        /// Constructor for Language using Language_Serialised.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ls">Language_Serialised object to use as source</param>
        public Language(Language_Serialised ls)
        {
            this.id = ls.id;
            this.dialect = ls.dialect;
            // baseLanguage to be inserted later
            this.baseLanguage = null;
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
            // VALIDATION

            // ID
            // trim
            id = id.Trim();

            if (!Utility_Methods.ValidateLanguageID(id, "baseLang"))
            {
                throw new InvalidDataException("BaseLanguage ID must have the format 'lang_' followed by 1-2 letters");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("BaseLanguage name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

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

    /// <summary>
    /// Class used to convert Language to/from serialised format (JSON)
    /// </summary>
    public class Language_Serialised
    {
        /// <summary>
        /// Holds language ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds base language
        /// </summary>
        public string baseLanguage { get; set; }
        /// <summary>
        /// Holds language dialect code
        /// </summary>
        public int dialect { get; set; }

        /// <summary>
        /// Constructor for Language_Serialised
        /// </summary>
        /// <param name="pc">PlayerCharacter object to use as source</param>
        public Language_Serialised(Language l)
        {
            this.id = l.id;
            this.dialect = l.dialect;
            this.baseLanguage = l.baseLanguage.id;
        }

        /// <summary>
        /// Constructor for Language_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Language_Serialised()
        {
        }

        /// <summary>
        /// Constructor for Language_Serialised taking seperate values.
        /// For creating Language_Serialised from CSV file.
        /// </summary>
        /// <param name="id">string holding Language ID</param>
        /// <param name="bLang">string holding BaseLanguage (ID)</param>
        /// <param name="dial">int holding language dialect code</param>
        public Language_Serialised(string id, string bLang, int dial)
        {
            // VALIDATION

            // ID
            // trim id and bLang
            id = id.Trim();
            bLang = bLang.Trim();

            if (!id.Contains(bLang))
            {
                throw new InvalidDataException("Language_Serialised ID be based on its BaseLanguage ID");
            }

            else if (!Utility_Methods.ValidateLanguageID(id))
            {
                throw new InvalidDataException("Language_Serialised ID must have the format 'lang_' followed by 1-2 letters, ending in 1-2 numbers");
            }

            // BLANG
            if (!Utility_Methods.ValidateLanguageID(bLang, "baseLang"))
            {
                throw new InvalidDataException("Language_Serialised BaseLanguage ID must have the format 'lang_' followed by 1-2 letters");
            }

            // DIALECT
            if (dial < 1)
            {
                throw new InvalidDataException("Language dialect code must be an integer >= 0");
            }

            this.id = id;
            this.baseLanguage = bLang;
            this.dialect = dial;
        }
    }
}
