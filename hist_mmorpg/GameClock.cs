using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing season data
    /// </summary>
    public class GameClock
    {
		/// <summary>
		/// Holds clock ID
		/// </summary>
		public String id { get; set; }
        /// <summary>
        /// Holds seasons
        /// </summary>
        public string[] seasons = new string[4] { "Spring", "Summer", "Autumn", "Winter" };
        /// <summary>
        /// Holds current year
        /// </summary>
        public uint currentYear { get; set; }
        /// <summary>
        /// Holds current season
        /// </summary>
        public byte currentSeason { get; set; }

        /// <summary>
        /// Constructor for GameClock
        /// </summary>
		/// <param name="id">String holding clock ID</param>
        /// <param name="yr">uint holding starting year</param>
        /// <param name="s">byte holding current season (default: 0)</param>
        public GameClock(String id, uint yr, byte s = 0)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new InvalidDataException("GameClock ID must be a string > 0 characters in length");
            }
            else
            {
                this.id = id;
            }

            this.currentYear = yr;

            // ensure season within correct values
            if (s > 3)
            {
                s = 3;
            }
            else if (s < 0)
            {
                s = 0;
            }

            this.currentSeason = s;
        }

        /// <summary>
        /// Constructor for GameClock taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public GameClock()
		{
		}

        /// <summary>
        /// Calculates travel modifier for season
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public double CalcSeasonTravMod()
        {
            double travelModifier = 0;

            switch (this.currentSeason)
            {
                // spring
                case 0:
                    travelModifier = 1.5;
                    break;
                // winter
                case 3:
                    travelModifier = 2;
                    break;
                // summer & autumn
                default:
                    travelModifier = 1;
                    break;
            }

            return travelModifier;
        }

        /// <summary>
        /// Advances GameClock to next season
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public void AdvanceSeason()
        {
            if (this.currentSeason == 3)
            {
                this.currentYear++;
                this.currentSeason = 0;
            }
            else
            {
                this.currentSeason++;
            }
        }
    }
}
