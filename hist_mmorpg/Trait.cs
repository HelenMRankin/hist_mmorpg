using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on a Trait
    /// </summary>
    public class Trait
    {
		/// <summary>
        /// Holds trait ID
		/// </summary>
		public String id { get; set; }
        /// <summary>
        /// Holds strait name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds trait effects
        /// </summary>
        public Dictionary<string, double> effects;

        /// <summary>
        /// Constructor for Trait
        /// </summary>
        /// <param name="id">String holding trait ID</param>
        /// <param name="nam">String holding trait name</param>
        /// <param name="effs">Dictionary(string, double) holding trait effects</param>
        public Trait(String id, String nam, Dictionary<string, double> effs)
        {
            // VALIDATION

            // ID
            // trim
            id = id.Trim();

            if (!Utility_Methods.ValidateTraitID(id))
            {
                throw new InvalidDataException("Trait ID must have the format 'trait_' followed by some numbers");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("Trait name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // EFFS
            // effect name
            string[] effNames = new string[effs.Count];
            effs.Keys.CopyTo(effNames, 0);

            for (int i = 0; i < effNames.Length; i++ )
            {
                if (!Utility_Methods.ValidateName(effNames[i]))
                {
                    throw new InvalidDataException("All Trait effect names must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
                }
            }

            // effect values
            double[] effVals = new double[effs.Count];
            effs.Values.CopyTo(effVals, 0);

            for (int i = 0; i < effVals.Length; i++)
            {
                if ((effVals[i] < -0.99) || (effVals[i] > 0.99))
                {
                    throw new InvalidDataException("All Trait effect values must be doubles between -0.99 and 0.99");
                }
            }

			this.id = id;
			this.name = nam;
            this.effects = effs;

        }

        /// <summary>
        /// Constructor for Trait taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Trait()
		{
		}
    }
}
