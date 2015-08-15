using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on terrain
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Holds terrain ID
        /// </summary>
		public String id { get; set; }
        /// <summary>
        /// Holds terrain description
        /// </summary>
        public String description { get; set; }
        /// <summary>
        /// Holds terrain travel cost
        /// </summary>
        public double travelCost { get; set; }

        /// <summary>
        /// Constructor for Terrain
        /// </summary>
		/// <param name="id">String holding terrain code</param>
        /// <param name="desc">String holding terrain description</param>
        /// <param name="tc">double holding terrain travel cost</param>
		public Terrain(String id, string desc, double tc)
        {
            // VALIDATION

            // ID
            // trim
            id = id.Trim();

            if (!Utility_Methods.ValidateTerrainID(id))
            {
                throw new InvalidDataException("Terrain ID must have the format 'terr_' followed by some letters");
            }

            // DESC
            // trim and ensure 1st is uppercase
            desc = Utility_Methods.FirstCharToUpper(desc.Trim());

            if (!Utility_Methods.ValidateName(desc))
            {
                throw new InvalidDataException("Terrain description must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // TC
            if (tc < 1)
            {
                throw new InvalidDataException("Terrain travelCost must be a double >= 1");
            }

            this.id = id;
            this.description = desc;
            this.travelCost = tc;
        }

        /// <summary>
        /// Constructor for Terrain taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Terrain()
		{
		}
    }
}
