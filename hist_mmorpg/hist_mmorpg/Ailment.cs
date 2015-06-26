using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on ailments effecting character health
    /// </summary>
    public class Ailment
    {
        /// <summary>
        /// Holds ailment ID
        /// </summary>
        public String ailmentID { get; set; }
        /// <summary>
        /// Holds ailment description
        /// </summary>
        public String description { get; set; }
        /// <summary>
        /// Holds ailment date
        /// </summary>
        public string when { get; set; }
        /// <summary>
        /// Holds current ailment effect
        /// </summary>
        public uint effect { get; set; }
        /// <summary>
        /// Holds minimum ailment effect
        /// </summary>
        public uint minimumEffect { get; set; }

        /// <summary>
        /// Constructor for Ailment
        /// </summary>
        /// <param name="id">String holding ailment ID</param>
        /// <param name="descr">string holding ailment description</param>
        /// <param name="wh">string holding ailment date</param>
        /// <param name="eff">uint holding current ailment effect</param>
        /// <param name="minEff">uint holding minimum ailment effect</param>
        public Ailment(String id, string descr, string wh, uint eff, uint minEff)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.FirstCharToUpper(id.Trim());

            if (!Utility_Methods.ValidateAilmentID(id))
            {
                throw new InvalidDataException("Ailment ID must have the format 'Ail_' followed by some numbers");
            }

            // DESCR
            // trim and ensure 1st is uppercase
            descr = Utility_Methods.FirstCharToUpper(descr.Trim());

            if (!Utility_Methods.ValidateName(descr))
            {
                throw new InvalidDataException("Ailment description must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // WHEN
            // trim and ensure 1st is uppercase
            wh = Utility_Methods.FirstCharToUpper(wh.Trim());

            // check contains season
            if (!wh.Contains("Spring"))
            {
                if (!wh.Contains("Summer"))
                {
                    if (!wh.Contains("Autumn"))
                    {
                        if (!wh.Contains("Winter"))
                        {
                            throw new InvalidDataException("Ailment 'when' must specify the season and year in which the ailment occurred");
                        }
                    }
                }
            }

            // EFF
            // check must be 1-5
            if ((eff < 1) || (eff > 5))
            {
                throw new InvalidDataException("Ailment effect must be a uint between 1-5");
            }

            // MINEFF
            // check not > 1
            if (minEff > 1)
            {
                throw new InvalidDataException("Ailment minimumEffect must be a uint less than 2");
            }

            this.ailmentID = id;
            this.description = descr;
            this.when = wh;
            this.effect = eff;
            this.minimumEffect = minEff;
        }

        /// <summary>
        /// Updates the ailment, reducing effect where approprite
        /// </summary>
        /// <returns>bool indicating whether ailment should be deleted</returns>
        public bool UpdateAilment()
        {
            bool deleteAilment = false;

            // reduce effect, if appropriate
            if (effect > minimumEffect)
            {
                effect--;
            }

            // remove effect if has reached 0
            if (effect == 0)
            {
                deleteAilment = true;
            }

            return deleteAilment;
        }
    }
}
