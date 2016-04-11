using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.Serialization;
namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on fief
    /// </summary>
    [Serializable()]
    public class Fief : Place, ISerializable
    {
        /// <summary>
        /// Holds fief's Province object
        /// </summary>
        public Province province { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        public int population { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        public Double fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        public Double industry { get; set; }
        /// <summary>
        /// Holds number of troops in fief
        /// </summary>
        public uint troops { get; set; }
        /// <summary>
        /// Holds fief tax rate
        /// </summary>
        public Double taxRate { get; set; }
        /// <summary>
        /// Holds fief tax rate (next season)
        /// </summary>
        public Double taxRateNext { get; set; }
        /// <summary>
        /// Holds expenditure on officials (next season)
        /// </summary>
        public uint officialsSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on garrison (next season)
        /// </summary>
        public uint garrisonSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure (next season)
        /// </summary>
        public uint infrastructureSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on keep (next season)
        /// </summary>
        public uint keepSpendNext { get; set; }
        /// <summary>
        /// Holds key data for current season.
        /// 0 = loyalty,
        /// 1 = GDP,
        /// 2 = tax rate,
        /// 3 = official expenditure,
        /// 4 = garrison expenditure,
        /// 5 = infrastructure expenditure,
        /// 6 = keep expenditure,
        /// 7 = keep level,
        /// 8 = income,
        /// 9 = family expenses,
        /// 10 = total expenses,
        /// 11 = overlord taxes,
        /// 12 = overlord tax rate,
        /// 13 = bottom line
        /// </summary>
        public double[] keyStatsCurrent = new double[14];
        /// <summary>
        /// Holds key data for previous season
        /// </summary>
        public double[] keyStatsPrevious = new double[14];
        /// <summary>
        /// Holds fief keep level
        /// </summary>
        public Double keepLevel { get; set; }
        /// <summary>
        /// Holds fief loyalty
        /// </summary>
        public Double loyalty { get; set; }
        /// <summary>
        /// Holds fief status (calm, unrest, rebellion)
        /// </summary>
        public char status { get; set; }
        /// <summary>
        /// Holds fief language and dialect
        /// </summary>
        public Language language { get; set; }
        /// <summary>
        /// Holds terrain object
        /// </summary>
        public Terrain terrain { get; set; }
        /// <summary>
        /// Holds characters present in fief
        /// </summary>
        public List<Character> charactersInFief = new List<Character>();
        /// <summary>
        /// Stores captives in this fief
        /// </summary>
        public List<Character> gaol = new List<Character>();
        /// <summary>
		/// Holds characters banned from keep (charIDs)
        /// </summary>
        public List<string> barredCharacters = new List<string>();
        /// <summary>
        /// Holds nationalitie banned from keep (IDs)
        /// </summary>
        public List<string> barredNationalities = new List<string>();
        /// <summary>
        /// Holds fief ancestral owner (PlayerCharacter object)
        /// </summary>
		public PlayerCharacter ancestralOwner { get; set; }
        /// <summary>
        /// Holds fief bailiff (Character object)
        /// </summary>
        public Character bailiff { get; set; }
        /// <summary>
        /// Number of days the bailiff has been resident in the fief (this season)
        /// </summary>
        public Double bailiffDaysInFief { get; set; }
        /// <summary>
        /// Holds fief treasury
        /// </summary>
        private int _treasury;
        /// <summary>
        /// Public accessor for the treasury- adjusting the treasury should be done with the "AdjustTreasury" method 
        /// </summary>
        public int Treasury { get { return this._treasury; } private set { this._treasury = value; } }
        /// <summary>
        /// Holds armies present in the fief (armyIDs)
        /// </summary>
        public List<string> armies = new List<string>();
        /// <summary>
        /// Identifies if recruitment has occurred in the fief in the current season
        /// </summary>
        public bool hasRecruited { get; set; }
        /// <summary>
        /// Identifies if pillage has occurred in the fief in the current season
        /// </summary>
        public bool isPillaged { get; set; }
        /// <summary>
        /// Holds troop detachments in the fief awaiting transfer
        /// String[] contains from (charID), to (charID), troop numbers (each type), days left when detached
        /// </summary>
        public Dictionary<string, ProtoDetachment> troopTransfers = new Dictionary<string, ProtoDetachment>();
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public String siege { get; set; }

        /**************LOCKS ***************/

        /// <summary>
        /// Create a new object to use as a treasury lock between transfers
        /// </summary>
        private object treasuryTransferLock = new object();
        /// <summary>
        /// Create lock for manipulating treasury
        /// </summary>
        private object treasuryLock = new object();
        /// <summary>
        /// Constructor for Fief
        /// </summary>
        /// <param name="id">String holding fief ID</param>
        /// <param name="nam">String holding fief name</param>
        /// <param name="own">PlayerCharacter holding fief owner</param>
        /// <param name="r">Fief's rank object</param>
        /// <param name="tiHo">Fief title holder (charID)</param>
        /// <param name="prov">Fief's Province object</param>
        /// <param name="pop">uint holding fief population</param>
        /// <param name="fld">Double holding fief field level</param>
        /// <param name="fld">Double holding fief industry level</param>
        /// <param name="trp">uint holding no. of troops in fief</param>
        /// <param name="tx">Double holding fief tax rate</param>
        /// <param name="txNxt">Double holding fief tax rate (next season)</param>
        /// <param name="offNxt">uint holding officials expenditure (next season)</param>
        /// <param name="garrNxt">uint holding garrison expenditure (next season)</param>
        /// <param name="infraNxt">uint holding infrastructure expenditure (next season)</param>
        /// <param name="keepNxt">uint holding keep expenditure (next season)</param>
        /// <param name="finCurr">Double [] holding financial data for current season</param>
        /// <param name="finPrev">Double [] holding financial data for previous season</param>
        /// <param name="kpLvl">Double holding fief keep level</param>
        /// <param name="loy">Double holding fief loyalty rating</param>
        /// <param name="stat">char holding fief status</param>
        /// <param name="lang">Language object holding language and dialect</param>
        /// <param name="terr">Terrain object for fief</param>
        /// <param name="chars">List holding characters present in fief</param>
        /// <param name="barChars">List holding IDs of characters barred from keep</param>
        /// <param name="barNats">List holding IDs of nationalities barred from keep</param>
        /// <param name="ancOwn">PlayerCharacter holding fief ancestral owner</param>
        /// <param name="bail">Character holding fief bailiff</param>
        /// <param name="bailInF">byte holding days bailiff in fief</param>
        /// <param name="treas">int containing fief treasury</param>
        /// <param name="arms">List holding IDs of armies present in fief</param>
        /// <param name="rec">bool indicating whether recruitment has occurred in the fief (current season)</param>
        /// <param name="pil">bool indicating whether pillage has occurred in the fief (current season)</param>
        /// <param name="trans">Dictionary &lt string, string[] &rt containing troop detachments in the fief awaiting transfer</param>
        /// <param name="sge">String holding siegeID of active siege</param>
        public Fief(String id, String nam, string tiHo, PlayerCharacter own, Rank r, Province prov, int pop, Double fld, Double ind, uint trp, Double tx,
            Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, double[] finCurr, double[] finPrev,
            Double kpLvl, Double loy, char stat, Language lang, Terrain terr, List<Character> chars, List<string> barChars, List<string> barNats,
            double bailInF, int treas, List<string> arms, bool rec, Dictionary<string, ProtoDetachment> trans, bool pil,
            PlayerCharacter ancOwn = null, Character bail = null, string sge = null)
            : base(id, nam, tiHo, own, r)
        {
            // VALIDATION

            // POP
            if (pop < 1)
            {
                throw new InvalidDataException("Fief population must be an integer greater than 0");
            }

            // FLD
            if (!Utility_Methods.ValidateFiefDouble(fld))
            {
                throw new InvalidDataException("Fief field level must be a double >= 0");
            }

            // IND
            if (!Utility_Methods.ValidateFiefDouble(ind))
            {
                throw new InvalidDataException("Fief industry level must be a double >= 0");
            }

            // TAX
            if (!Utility_Methods.ValidatePercentage(tx))
            {
                throw new InvalidDataException("Fief taxrate must be a double between 0 and 100");
            }

            // TAXNEXT
            if (!Utility_Methods.ValidatePercentage(txNxt))
            {
                throw new InvalidDataException("Fief taxrate for next season must be a double between 0 and 100");
            }

            // FINCUR
            // 0 = loyalty
            if (!Utility_Methods.ValidateFiefDouble(finCurr[0], 9))
            {
                throw new InvalidDataException("finCurr[0] (loyalty) must be a double between 0 and 9");
            }

            // 1 = GDP
            int gdpCurr = Convert.ToInt32(finCurr[1]);

            // 2 = tax rate,
            if (!Utility_Methods.ValidatePercentage(finCurr[2]))
            {
                throw new InvalidDataException("finCurr[2] (taxrate) must be a double between 0 and 100");
            }

            // 3 = official expenditure,
            uint offCurr = Convert.ToUInt32(finCurr[3]);

            // 4 = garrison expenditure,
            uint garrCurr = Convert.ToUInt32(finCurr[4]);

            // 5 = infrastructure expenditure,
            uint infCurr = Convert.ToUInt32(finCurr[5]);

            // 6 = keep expenditure,
            uint kpCurr = Convert.ToUInt32(finCurr[6]);

            // 7 = keep level,
            if (!Utility_Methods.ValidateFiefDouble(finCurr[7]))
            {
                throw new InvalidDataException("finCurr[7] (keep level) must be a double >= 0");
            }

            // 8 = income,
            uint incCurr = Convert.ToUInt32(finCurr[8]);

            // 9 = family expenses,
            uint famCurr = Convert.ToUInt32(finCurr[9]);

            // 10 = total expenses,
            uint expCurr = Convert.ToUInt32(finCurr[10]);

            // 11 = overlord taxes,
            uint otaxCurr = Convert.ToUInt32(finCurr[11]);

            // 12 = overlord tax rate,
            if (!Utility_Methods.ValidatePercentage(finCurr[12]))
            {
                throw new InvalidDataException("finCurr[12] (overlord taxrate) must be a double between 0 and 100");
            }

            // 13 = bottom line
            int botCurr = Convert.ToInt32(finCurr[13]);

            // FINPREV
            // 0 = loyalty,
            if (!Utility_Methods.ValidateFiefDouble(finPrev[0], 9))
            {
                throw new InvalidDataException("finPrev[0] (loyalty) must be a double between 0 and 9");
            }

            // 1 = GDP
            int gdpPrev = Convert.ToInt32(finPrev[1]);

            // 2 = tax rate,
            if (!Utility_Methods.ValidatePercentage(finPrev[2]))
            {
                throw new InvalidDataException("finPrev[2] (taxrate) must be a double between 0 and 100");
            }

            // 3 = official expenditure,
            uint offPrev = Convert.ToUInt32(finPrev[3]);

            // 4 = garrison expenditure,
            uint garrPrev = Convert.ToUInt32(finPrev[4]);

            // 5 = infrastructure expenditure,
            uint infPrev = Convert.ToUInt32(finPrev[5]);

            // 6 = keep expenditure,
            uint kpPrev = Convert.ToUInt32(finPrev[6]);

            // 7 = keep level,
            if (!Utility_Methods.ValidateFiefDouble(finPrev[7]))
            {
                throw new InvalidDataException("finPrev[7] (keep level) must be a double >= 0");
            }

            // 8 = income,
            uint incPrev = Convert.ToUInt32(finPrev[8]);

            // 9 = family expenses,
            uint famPrev = Convert.ToUInt32(finPrev[9]);

            // 10 = total expenses,
            uint expPrev = Convert.ToUInt32(finPrev[10]);

            // 11 = overlord taxes,
            uint otaxPrev = Convert.ToUInt32(finPrev[11]);

            // 12 = overlord tax rate,
            if (!Utility_Methods.ValidatePercentage(finPrev[12]))
            {
                throw new InvalidDataException("finPrev[12] (overlord taxrate) must be a double between 0 and 100");
            }

            // 13 = bottom line
            int botPrev = Convert.ToInt32(finPrev[13]);
            
            // KPLVL
            if (!Utility_Methods.ValidateFiefDouble(kpLvl))
            {
                throw new InvalidDataException("Fief keep level must be a double >= 0");
            }

            // LOY
            if (!Utility_Methods.ValidateFiefDouble(loy, 9))
            {
                throw new InvalidDataException("Fief loyalty must be a double between 0 and 9");
            }

            // STAT
            // convert to uppercase
            stat = Convert.ToChar(stat.ToString().ToUpper());

            if (!(Regex.IsMatch(stat.ToString(), "[CRU]")))
            {
                throw new InvalidDataException("Fief status must be 'C', 'U' or 'R'");
            }

            // BARCHARS
            for (int i = 0; i < barChars.Count; i++ )
            {
                // trim and ensure 1st is uppercase
                barChars[i] = Utility_Methods.FirstCharToUpper(barChars[i].Trim());

                if (!Utility_Methods.ValidateCharacterID(barChars[i]))
                {
                    throw new InvalidDataException("All fief barred character IDs must have the format 'Char_' followed by some numbers");
                }
            }

            // BARNATS
            for (int i = 0; i < barNats.Count; i++)
            {
                // trim and ensure 1st is uppercase
                barNats[i] = Utility_Methods.FirstCharToUpper(barNats[i].Trim());

                if (!Utility_Methods.ValidateNationalityID(barNats[i]))
                {
                    throw new InvalidDataException("All fief barred nationality IDs must be 1-3 characters long, and consist entirely of letters");
                }
            }

            // BAILIFFDAYSINFIEF
            if (!Utility_Methods.ValidateDays(bailInF))
            {
                throw new InvalidDataException("Fief bailiffDaysInFief must be a double between 0-109");
            }

            // ARMS
            for (int i = 0; i < arms.Count; i++)
            {
                // trim and ensure 1st is uppercase
                arms[i] = Utility_Methods.FirstCharToUpper(arms[i].Trim());

                if (!Utility_Methods.ValidateArmyID(arms[i]))
                {
                    throw new InvalidDataException("All fief army IDs must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            // SIEGE
            if (!String.IsNullOrWhiteSpace(sge))
            {
                // trim and ensure 1st is uppercase
                sge = Utility_Methods.FirstCharToUpper(sge.Trim());

                if (!Utility_Methods.ValidateSiegeID(sge))
                {
                    throw new InvalidDataException("Fief siege IDs must have the format 'Siege_' followed by some numbers");
                }
            }

            this.province = prov;
            this.population = pop;
            this.ancestralOwner = ancOwn;
            this.bailiff = bail;
            this.fields = fld;
            this.industry = ind;
            this.troops = trp;
            this.taxRate = tx;
            this.taxRateNext = txNxt;
            this.officialsSpendNext = offNxt;
            this.garrisonSpendNext = garrNxt;
            this.infrastructureSpendNext = infraNxt;
            this.keepSpendNext = keepNxt;
            this.keyStatsCurrent = finCurr;
            this.keyStatsPrevious = finPrev;
            this.keepLevel = kpLvl;
            this.loyalty = loy;
            this.status = stat;
            this.language = lang;
            this.terrain = terr;
            this.charactersInFief = chars;
            this.barredCharacters = barChars;
            this.barredNationalities = barNats;
            this.bailiffDaysInFief = bailInF;
            this.Treasury = treas;
            this.armies = arms;
            this.hasRecruited = rec;
            this.troopTransfers = trans;
            this.isPillaged = pil;
            this.siege = sge;
        }

		/// <summary>
        /// Constructor for Fief using Fief_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="fs">Fief_Serialised object to use as source</param>
		public Fief(Fief_Serialised fs)
			: base(fs: fs)
		{
		
            // province to be added later
			this.province = null;
			this.population = fs.population;
            // ancestral owner to be added later
            this.ancestralOwner = null;
            // bailiff to be added later
            this.bailiff = null;
			this.fields = fs.fields;
			this.industry = fs.industry;
			this.troops = fs.troops;
			this.taxRate = fs.taxRate;
			this.taxRateNext = fs.taxRateNext;
			this.officialsSpendNext = fs.officialsSpendNext;
			this.garrisonSpendNext = fs.garrisonSpendNext;
			this.infrastructureSpendNext = fs.infrastructureSpendNext;
			this.keepSpendNext = fs.keepSpendNext;
            this.keyStatsCurrent = fs.keyStatsCurrent;
            this.keyStatsPrevious = fs.keyStatsPrevious;
			this.keepLevel = fs.keepLevel;
			this.loyalty = fs.loyalty;
			this.status = fs.status;
            this.language = null;
			this.terrain = null;
            // create empty list to be populated later
			this.charactersInFief = new List<Character>();
			this.barredCharacters = fs.barredCharacters;
			this.barredNationalities = fs.barredNationalities;
            this.bailiffDaysInFief = fs.bailiffDaysInFief;
            this.Treasury = fs.treasury;
            this.armies = fs.armies;
            this.hasRecruited = fs.hasRecruited;
            this.troopTransfers = fs.troopTransfers;
            this.isPillaged = fs.isPillaged;
            this.siege = fs.siege;
        }

        /// <summary>
        /// Constructor for Fief taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Fief()
		{
		}

        // would prefer to check that fief is currently viewed fief elsewhere
        /// <summary>
        /// Adjusts the fief's tax rate and expenditure levels (officials, garrison, infrastructure, keep)
        /// </summary>
        /// <param name="tx">Proposed tax rate</param>
        /// <param name="off">Proposed officials expenditure</param>
        /// <param name="garr">Proposed garrison expenditure</param>
        /// <param name="infr">Proposed infrastructure expenditure</param>
        /// <param name="kp">Proposed keep expenditure</param>
        public ProtoMessage AdjustExpenditures(double tx, uint off, uint garr, uint infr, uint kp)
        {
            ProtoMessage error = null;
            // keep track of whether any spends ahve changed
            bool spendChanged = false;

            // get total spend
            uint totalSpend = off + garr + infr + kp;

            // factor in bailiff traits modifier for fief expenses
            double bailiffModif = 0;

            // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
            bailiffModif = this.CalcBailExpModif(this.bailiffDaysInFief >= 30);
            if (bailiffModif != 0)
            {
                totalSpend = totalSpend + Convert.ToUInt32(totalSpend * bailiffModif);
            }

            // check that expenditure can be supported by the treasury
            // if it can't, display a message and cancel the commit
            if (!this.CheckExpenditureOK(totalSpend))
            {
                int difference = Convert.ToInt32(totalSpend - this.GetAvailableTreasury());
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.FiefExpenditureAdjustment;
                error.MessageFields = new string[] { this.name, difference.ToString() };
                return error;
                
            }
            // if treasury funds are sufficient to cover expenditure, do the commit
            else
            {
                // tax rate
                // check if amount/rate changed
                if (tx != this.taxRateNext)
                {
                    // adjust tax rate
                    this.AdjustTaxRate(tx);
                    spendChanged = true;
                }

                // officials spend
                // check if amount/rate changed
                if (off != this.officialsSpendNext)
                {
                    // adjust officials spend
                    this.AdjustOfficialsSpend(off);
                    spendChanged = true;
                }

                // garrison spend
                // check if amount/rate changed
                if (garr != this.garrisonSpendNext)
                {
                    // adjust garrison spend
                    this.AdjustGarrisonSpend(garr);
                    spendChanged = true;
                }

                // infrastructure spend
                // check if amount/rate changed
                if (infr != this.infrastructureSpendNext)
                {
                    // adjust infrastructure spend
                    this.AdjustInfraSpend(infr);
                    spendChanged = true;
                }

                // adjust keep spend
                // check if amount/rate changed
                if (kp != this.keepSpendNext)
                {
                    // adjust keep spend
                    this.AdjustKeepSpend(kp);
                    spendChanged = true;
                }

                // display appropriate message
                string toDisplay = "";
                if (spendChanged)
                {
                    toDisplay += "adjusted";
                }
                else
                {
                    toDisplay += "unchanged";
                }
                ProtoFief success = new ProtoFief(this);
                success.includeAll(this);
                success.ResponseType = DisplayMessages.FiefExpenditureAdjusted;
                success.MessageFields = new string[] { toDisplay };
                return success;
            }
        }

        /// <summary>
        /// Calculates fief GDP
        /// </summary>
        /// <returns>uint containing fief GDP</returns>
        public uint CalcNewGDP()
        {
            uint gdp = 0;
            uint fldProd = 0;
            uint indProd = 0;

            // calculate production from fields using next season's expenditure
            fldProd = Convert.ToUInt32((this.CalcNewFieldLevel() * 8997));
			indProd = Convert.ToUInt32(this.CalcNewIndustryLevel() * this.CalcNewPopulation());

            // calculate final gdp
            gdp = (fldProd + indProd) / (this.CalcNewPopulation() / 1000);

            gdp = (gdp * 1000);

            return gdp;
        }

        /// <summary>
        /// Calculates fief population increase
        /// </summary>
        /// <returns>uint containing new fief population</returns>
        public uint CalcNewPopulation()
        {
            uint newPop = Convert.ToUInt32(this.population + (this.population * 0.005));
            return newPop;
        }

        /// <summary>
        /// Calculates fief income (NOT including income loss due to unrest/rebellion)
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int CalcNewIncome()
        {
            int fiefBaseIncome = 0;
            int fiefIncome = 0;

            // check for siege
            if (String.IsNullOrWhiteSpace(this.siege))
            {
                // no siege = use next season's expenditure and tax rate
                fiefBaseIncome = Convert.ToInt32(this.CalcNewGDP() * (this.taxRateNext / 100));
            }
            else
            {
                // siege = use next season's expenditure and 0% tax rate = no income
                fiefBaseIncome = Convert.ToInt32(this.CalcNewGDP() * 0);
            }

            fiefIncome = fiefBaseIncome;

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.CalcBlfIncMod(this.bailiffDaysInFief >= 30));

            // factor in officials spend modifier
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.CalcOffIncMod());

            // ensure min income = 0
            if (fiefIncome < 0)
            {
                fiefIncome = 0;
            }

            return fiefIncome;
        }

        /// <summary>
        /// Calculates effect of financial controller on fief family expenses
        /// </summary>
        /// <returns>double containing fief family expenses modifier</returns>
        /// <param name="ch">The financial controller (Character)</param>
        public double CalcFamExpenseMod(Character ch = null)
        {
            double famExpMod = 0;

            // set default management rating
            double manRating = 3;

            if (ch != null)
            {
                manRating = ch.management;
            }

            // 2.5% decrease in family expenses per management level above 1
            famExpMod = (((manRating - 1) * 2.5) / 100) * -1;

            // factor in financial controller traits
            if (ch != null)
            {
                double famExpTraitsMod = ch.CalcTraitEffect(Globals_Game.Stats.FAMEXPENSE);

                // apply to famExpMod
                if (famExpTraitsMod != 0)
                {
                    famExpMod = (famExpMod + famExpTraitsMod);
                }
            }

            return famExpMod;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBlfIncMod(bool daysInFiefOK)
        {
            double incomeModif = 0;

            // check if auto-bailiff
            if ((this.bailiff == null) || (! daysInFiefOK))
            {
                // modifer = 0.025 per management level above 1
                // if auto-baliff set modifier at equivalent of management rating of 3
                incomeModif = 0.05;
            }
            else if (this.bailiff != null)
            {
                incomeModif = this.bailiff.CalcFiefIncMod();
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcOffIncMod()
        {
            double incomeModif = 0;

            // using next season's expenditure and population
            incomeModif = ((this.officialsSpendNext - ((double)this.CalcNewPopulation() * 2)) / (this.CalcNewPopulation() * 2)) / 10;

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of unrest/rebellion on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcStatusIncmMod()
        {
            double incomeModif = 0;

            switch (this.status)
            {
                // unrest = income reduced by 50%
                case 'U':
                    incomeModif = 0.5;
                    break;
                // unrest = income reduced by 100%
                case 'R':
                    incomeModif = 0;
                    break;
                // anything else = no reduction
                default:
                    incomeModif = 1;
                    break;
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates overlord taxes
        /// </summary>
        /// <returns>uint containing overlord taxes</returns>
        public uint CalcNewOlordTaxes()
        {
            // calculate tax, based on income of specified season
            uint oTaxes = Convert.ToUInt32(this.CalcNewIncome() * (this.province.taxRate / 100));
            return oTaxes;
        }

        /// <summary>
        /// Calculates fief expenses
        /// </summary>
        /// <returns>int containing fief expenses</returns>
        public int CalcNewExpenses()
        {
            int fiefExpenses = 0;

            // using next season's expenditure
            fiefExpenses = (int)this.officialsSpendNext + (int)this.infrastructureSpendNext + (int)this.garrisonSpendNext + (int)this.keepSpendNext;

            // factor in bailiff traits modifier for fief expenses
            double bailiffModif = 0;

            // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
            bailiffModif = this.CalcBailExpModif(this.bailiffDaysInFief >= 30);

            if (bailiffModif != 0 )
            {
                fiefExpenses = fiefExpenses + Convert.ToInt32(fiefExpenses * bailiffModif);
            }

            return fiefExpenses;
        }

        /// <summary>
        /// Calculates effect of bailiff traits on fief expenses
        /// </summary>
        /// <returns>double containing fief expenses modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBailExpModif(bool daysInFiefOK)
        {
            double expTraitsModifier = 0;

            if ((this.bailiff != null) && (daysInFiefOK))
            {
                expTraitsModifier = this.bailiff.CalcTraitEffect(Globals_Game.Stats.FIEFEXPENSE);
            }

            return expTraitsModifier;
        }
        
        /// <summary>
        /// Calculates fief bottom line
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int CalcNewBottomLine()
        {
            int fiefBottomLine = 0;

            // factor in effect of fief status on specified season's income
            int fiefIncome = Convert.ToInt32(this.CalcNewIncome() * this.CalcStatusIncmMod());

            // calculate bottom line using specified season's income, and expenses
            fiefBottomLine = fiefIncome - (int)this.CalcNewExpenses() - this.CalcFamilyExpenses();

            // check for occupation before deducting overlord taxes
            if (!this.CheckEnemyOccupation())
            {
                fiefBottomLine -= (int)this.CalcNewOlordTaxes();
            }

            return fiefBottomLine;
        }

        /// <summary>
        /// Calculates family expenses
        /// </summary>
        /// <returns>int containing family expenses</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int CalcFamilyExpenses()
        {
            int famExpenses = 0;
            Character thisController = null;

            // for all fiefs, get bailiff wages
            if (this.bailiff != null)
            {
                // ensure isn't family member (they don't get paid)
                if (String.IsNullOrWhiteSpace(this.bailiff.familyID))
                {
                    int bailiffSalary = Convert.ToInt32((this.bailiff as NonPlayerCharacter).salary);

                    // get total no. of fiefs this guy is bailiff in
                    List<Fief> myFiefs = this.bailiff.GetFiefsBailiff();

                    // if is bailiff in more than this fief, only pay proportion of salary
                    if (myFiefs.Count > 1)
                    {
                        bailiffSalary = bailiffSalary / myFiefs.Count;
                    }

                    famExpenses += bailiffSalary;
                }
            }

            // if home fief, also get all non-bailiff expenses
            // (i.e. family allowances, other employees' wages)
            if (this == this.owner.GetHomeFief())
            {
                // get whoever has highest management rating
                if ((!String.IsNullOrWhiteSpace(this.owner.spouse)) && (this.owner.management < this.owner.GetSpouse().management))
                {
                    thisController = this.owner.GetSpouse();
                }
                else
                {
                    thisController = this.owner;
                }

                foreach (NonPlayerCharacter element in this.owner.myNPCs)
                {
                    if (!((element.GetResponsibilities(this.owner).ToLower()).Contains("bailiff")))
                    {
                        // add wage of non-bailiff employees
                        if (String.IsNullOrWhiteSpace(element.familyID))
                        {
                            famExpenses += Convert.ToInt32(element.salary);
                        }
                        // add family allowance of family NPCs
                        else
                        {
                            // get allowance (based on family function)
                            int thisExpense = Convert.ToInt32(element.CalcFamilyAllowance(element.GetFunction(this.owner)));

                            // check for siege
                            if (!String.IsNullOrWhiteSpace(this.siege))
                            {
                                // siege = half allowance
                                thisExpense = thisExpense / 2;
                            }

                            // add to total family expenses
                            famExpenses += thisExpense;
                        }
                    }
                }

            }

            // if NOT home fief, use bailiff's stats/traits
            else
            {
                if ((this.bailiffDaysInFief >= 30) && (this.bailiff != null))
                {
                    thisController = this.bailiff;
                }
            }

            // factor in traits and stats of financial controller (player/spouse/bailiff)
            double famExpModifier = this.CalcFamExpenseMod(thisController);

            // apply to family expenses
            if (famExpModifier != 0)
            {
                famExpenses = famExpenses + Convert.ToInt32(famExpenses * famExpModifier);
            }

            return famExpenses;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// (rate adjustment messages done client side)
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void AdjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
            }
            else if (tx < 0)
            {
                tx = 0;
            }

            this.taxRateNext = tx;
        }

        /// <summary>
        /// Gtes the maximum permitted expenditure for the fief of the specified type
        /// </summary>
        /// <returns>uint containing maximum permitted expenditure</returns>
        /// <param name="type">string containing type of expenditure</param>
        public uint GetMaxSpend(string type)
        {
            uint maxSpend = 0;

            uint[] spendMultiplier = {14, 6, 13, 4 };
            uint thisMultiplier = 0;

            switch (type)
            {
                case "garrison":
                    thisMultiplier = spendMultiplier[0];
                    break;
                case "infrastructure":
                    thisMultiplier = spendMultiplier[1];
                    break;
                case "keep":
                    thisMultiplier = spendMultiplier[2];
                    break;
                case "officials":
                    thisMultiplier = spendMultiplier[3];
                    break;
                default:
                    break;
            }

            maxSpend = Convert.ToUInt32(this.population) * thisMultiplier;

            return maxSpend;
        }

        //TODO notify on client side that expeniture has been changed to min/max
        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</param>
        public void AdjustOfficialsSpend(uint os)
        {
            // ensure min 0
            if (os < 0)
            {
                os = 0;
            }

            // ensure doesn't exceed max permitted (4 per head of population)
            uint maxSpend = this.GetMaxSpend("officials");
            if (os > maxSpend)
            {
                os = maxSpend;
            }

            this.officialsSpendNext = os;
        }

        //TODO notify user of min/max adjustment
        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</param>
        public void AdjustInfraSpend(uint infs)
        {
            // ensure min 0
            if (infs < 0)
            {
                infs = 0;
                
            }

            // ensure doesn't exceed max permitted (6 per head of population)
            uint maxSpend = this.GetMaxSpend("infrastructure");
            if (infs > maxSpend)
            {
                infs = maxSpend;
            }
            this.infrastructureSpendNext = infs;
        }
        //TODO notify user of autoadjustment
        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</param>
        public void AdjustGarrisonSpend(uint gs)
        {
            // ensure min 0
            if (gs < 0)
            {
                gs = 0;
            }

            // ensure doesn't exceed max permitted (14 per head of population)
            uint maxSpend = this.GetMaxSpend("garrison");
            if (gs > maxSpend)
            {
                gs = maxSpend;
            }

            this.garrisonSpendNext = gs;
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</param>
        public void AdjustKeepSpend(uint ks)
        {;
            // ensure min 0
            if (ks < 0)
            {
                ks = 0;
            }

            // ensure doesn't exceed max permitted (13 per head of population)
            uint maxSpend = this.GetMaxSpend("keep");
            if (ks > maxSpend)
            {
                ks = maxSpend;
                
            }

            this.keepSpendNext = ks;
        }

        /// <summary>
        /// Calculates new fief field level (from next season's spend)
        /// </summary>
        /// <returns>double containing new field level</returns>
        public double CalcNewFieldLevel()
        {
            double fldLvl = 0;

            // if no expenditure, field level reduced by 1%
            if (this.infrastructureSpendNext == 0)
            {
                fldLvl = this.fields - (this.fields / 100);
            }
            // field level increases by 0.2 per 100k spent
            else
            {
                fldLvl = this.fields + (this.infrastructureSpendNext / 500000.00);
            }

            // ensure not < 0
            if (fldLvl < 0)
            {
                fldLvl = 0;
            }

            return fldLvl;
        }

        /// <summary>
        /// Calculates new fief industry level (from next season's spend)
        /// </summary>
        /// <returns>double containing new industry level</returns>
        public double CalcNewIndustryLevel()
        {
            double indLvl = 0;

            // if no expenditure, industry level reduced by 1%
            if (this.infrastructureSpendNext == 0)
            {
                indLvl = this.industry - (this.industry / 100);
            }
            // industry level increases by 0.1 per 150k spent
            else
            {
                indLvl = this.industry + (this.infrastructureSpendNext / 1500000.00);
            }

            // ensure not < 0
            if (indLvl < 0)
            {
                indLvl = 0;
            }

            return indLvl;
        }

        /// <summary>
        /// Calculates new fief keep level (from next season's spend)
        /// </summary>
        /// <returns>double containing new keep level</returns>
        public double CalcNewKeepLevel()
        {
            double kpLvl = 0;

            // if no expenditure, keep level reduced by 0.15
            if (this.keepSpendNext == 0)
            {
                kpLvl = this.keepLevel - 0.15;
            }
            // keep level increases by 0.25 per 100k spent
            else
            {
                kpLvl = this.keepLevel + (this.keepSpendNext / 400000.00);
            }

            // ensure not < 0
            if (kpLvl < 0)
            {
                kpLvl = 0;
            }

            return kpLvl;
        }

        /// <summary>
        /// Calculates new fief loyalty level (i.e. for next season)
        /// </summary>
        /// <returns>double containing new fief loyalty</returns>
        public double CalcNewLoyalty()
        {
            double newBaseLoy = 0;
            double newLoy = 0;

            // calculate effect of tax rate change = loyalty % change is direct inverse of tax % change
            newBaseLoy = this.loyalty + (this.loyalty * (((this.taxRateNext - this.taxRate) / 100) * -1));

            // calculate effect of surplus 
            if (this.CalcNewBottomLine() > 0)
            {
                // loyalty reduced in proportion to surplus divided by income
                newLoy = newBaseLoy - (this.CalcNewBottomLine() / Convert.ToDouble(this.CalcNewIncome()));
            }
            else
            {
                newLoy = newBaseLoy;
            }

            // calculate effect of officials spend
            newLoy = newLoy + (newBaseLoy * this.CalcOffLoyMod());

            // calculate effect of garrison spend
            newLoy = newLoy + (newBaseLoy * this.CalcGarrLoyMod());

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            newLoy = newLoy + (newBaseLoy * this.CalcBlfLoyAdjusted(this.bailiffDaysInFief >= 30));

            // ensure loyalty within limits
            if (newLoy < 0)
            {
                newLoy = 0;
            }
            else if (newLoy > 9)
            {
                newLoy = 9;
            }

            return newLoy;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief loyalty level
        /// Also includes effect of traits
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBlfLoyAdjusted(bool daysInFiefOK)
        {
            double loyModif = 0;
            double loyTraitModif = 0;

            // get bailiff stats (set to auto-bailiff values by default)
            double bailStature = 3;
            double bailMgmt = 3;
            Language bailLang = this.language;

            // if not auto-bailiff and if has served appropriate days in fief
            if ((this.bailiff != null) && (daysInFiefOK))
            {
                bailStature = this.bailiff.CalculateStature();
                bailMgmt = this.bailiff.management;
                bailLang = this.bailiff.language;
            }

            // get base bailiff loyalty modifier
            loyModif = this.CalcBaseFiefLoyMod(bailStature, bailMgmt, bailLang);

            // check for trait modifier, passing in daysInFief
            loyTraitModif = this.CalcBailLoyTraitMod(daysInFiefOK);

            loyModif = loyModif + (loyModif * loyTraitModif);

            return loyModif;
        }


        /// <summary>
        /// Calculates base effect of bailiff's stats on fief loyalty
        /// Takes bailiff language into account
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="stature">Bailiff's stature</param>
        /// <param name="mngt">Bailiff's management rating</param>
        /// <param name="lang">Bailiff's language</param>
        public double CalcBaseFiefLoyMod(double stature, double mngt, Language lang)
        {
            double loyModif = 0;
            double bailStats = 0;

            bailStats = (stature + mngt) / 2;

            // check for language effects
            if (this.language != lang)
            {
                bailStats -= 3;
                if (bailStats < 1)
                {
                    bailStats = 1;
                }
            }

            // 1.25% increase in fief loyalty per bailiff's stature/management average above 1
            loyModif = (bailStats - 1) * 0.0125;

            return loyModif;
        }

        /// <summary>
        /// Calculates bailiff's trait modifier for fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBailLoyTraitMod(bool daysInFiefOK)
        {
            double loyTraitsModifier = 0;

            if ((this.bailiff != null) && (daysInFiefOK))
            {
                loyTraitsModifier = this.bailiff.CalcTraitEffect(Globals_Game.Stats.FIEFLOY);
            }

            return loyTraitsModifier;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double CalcOffLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's officials spend and population
            loyaltyModif = ((this.officialsSpendNext - ((double)this.CalcNewPopulation() * 2)) / (this.CalcNewPopulation() * 2)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Calculates effect of garrison spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double CalcGarrLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's garrison spend and population
            loyaltyModif = ((this.garrisonSpendNext - ((double)this.CalcNewPopulation() * 7)) / (this.CalcNewPopulation() * 7)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Validates proposed expenditure levels, auto-adjusting where necessary
        /// </summary>
        public void ValidateFiefExpenditure()
        {
            // get total spend
            uint totalSpend = Convert.ToUInt32(this.CalcNewExpenses()); ;

            // check to see if proposed expenditure level doesn't exceed fief treasury
            bool isOK = this.CheckExpenditureOK(totalSpend);

            // if expenditure does exceed fief treasury
            if (!isOK)
            {
                // get available treasury
                int availTreasury = this.GetAvailableTreasury();
                // calculate amount by which treasury exceeded
                uint difference = Convert.ToUInt32(totalSpend - availTreasury);
                // auto-adjust expenditure
                this.AutoAdjustExpenditure(difference);
            }
        }

        /// <summary>
        /// Automatically adjusts expenditure at end of season, if exceeds treasury
        /// </summary>
        /// <param name="difference">The amount by which expenditure exceeds treasury</param>
        public void AutoAdjustExpenditure(uint difference)
        {
            // get available treasury
            int availTreasury = this.GetAvailableTreasury();

            // if treasury empty or in deficit, reduce all expenditure to 0
            if (availTreasury <= 0)
            {
                this.officialsSpendNext = 0;
                this.garrisonSpendNext = 0;
                this.infrastructureSpendNext = 0;
                this.keepSpendNext = 0;
            }

            else
            {
                // bool to control do while loop
                bool finished = false;
                // keep track of new difference
                uint differenceNew = difference;
                // get total expenditure
                uint totalSpend = this.officialsSpendNext + this.garrisonSpendNext + this.infrastructureSpendNext + this.keepSpendNext;
                // proportion to take off each spend
                double reduceByModifierOff = (double)this.officialsSpendNext / totalSpend;
                double reduceByModifierGarr = (double)this.garrisonSpendNext / totalSpend;
                double reduceByModifierInf = (double)this.infrastructureSpendNext / totalSpend;
                double reduceByModifierKeep = (double)this.keepSpendNext / totalSpend;
                // double to reduce spend by
                double reduceBy = 0;
                // uint to deduct from each spend
                uint takeOff = 0;
                // list to hold individual spends
                List<string> spends = new List<string>();

                do
                {
                    // update difference
                    difference = differenceNew;

                    // clear current spends list
                    if (spends.Count > 0)
                    {
                        spends.Clear();
                    }

                    // re-populate spends list with appropriate codes
                    // but only spends > 0
                    if (this.officialsSpendNext > 0)
                    {
                        spends.Add("off");
                    }
                    if (this.garrisonSpendNext > 0)
                    {
                        spends.Add("garr");
                    }
                    if (this.infrastructureSpendNext > 0)
                    {
                        spends.Add("inf");
                    }
                    if (this.keepSpendNext > 0)
                    {
                        spends.Add("keep");
                    }

                    // if no remaining spends, finish
                    if (spends.Count == 0)
                    {
                        finished = true;
                    }

                    // iterate through each entry in spends list
                    // (remember: only spends > 0)
                    for (int i = 0; i < spends.Count; i++)
                    {
                        switch (spends[i])
                        {
                            // officials
                            case "off":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierOff;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\noffSpend: " + this.officialsSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierOff: " + reduceByModifierOff + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        // if is enough in spend to subtract takeOff amount
                                        if (this.officialsSpendNext >= takeOff)
                                        {
                                            // subtract takeOff from spend
                                            this.officialsSpendNext -= takeOff;
                                            // subtract takeOff from remaining difference
                                            differenceNew -= takeOff;
                                        }
                                        // if is less in spend than takeOff amount
                                        else
                                        {
                                            // subtract spend from remaining difference
                                            differenceNew -= this.officialsSpendNext;
                                            // reduce spend to 0
                                            this.officialsSpendNext = 0;
                                        }
                                        // check to see if is any difference remaining 
                                        if (differenceNew == 0)
                                        {
                                            // if no remaining difference, signal exit from do while loop
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "garr":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierGarr;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ngarrSpend: " + this.garrisonSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierGarr: " + reduceByModifierGarr + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (this.garrisonSpendNext >= takeOff)
                                        {
                                            this.garrisonSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= this.garrisonSpendNext;
                                            this.garrisonSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "inf":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierInf;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ninfSpend: " + this.infrastructureSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierInf: " + reduceByModifierInf + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (this.infrastructureSpendNext >= takeOff)
                                        {
                                            this.infrastructureSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= this.infrastructureSpendNext;
                                            this.infrastructureSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "keep":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierKeep;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\nkeepSpend: " + this.keepSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierKeep: " + reduceByModifierKeep + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (this.keepSpendNext >= takeOff)
                                        {
                                            this.keepSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= this.keepSpendNext;
                                            this.keepSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                               break;
                            default:
                                break;
                        }
                    }
                } while (!finished);
            }
        }

        /// <summary>
        /// Calculates amount available in treasury for financial transactions
        /// </summary>
        /// <returns>int containing amount available</returns>
        /// <param name="deductFiefExpense">bool indicating whether to account for fief expenses in the calculation</param>
        public int GetAvailableTreasury(bool deductFiefExpense = false)
        {
            int amountAvail = 0;

            // get treasury
            amountAvail = this.Treasury;
            // deduct family expenses
            amountAvail -= this.CalcFamilyExpenses();
            // deduct overlord taxes
            amountAvail -= Convert.ToInt32(this.CalcNewOlordTaxes());

            if (deductFiefExpense)
            {
                // deduct fief expenditure
                amountAvail -= this.CalcNewExpenses();
            }

            return amountAvail;
        }
        
        /// <summary>
        /// Compares expenditure level with fief treasury
        /// </summary>
        /// <returns>bool indicating whether expenditure acceptable</returns>
        /// <param name="totalSpend">proposed total expenditure for next season</param>
        public bool CheckExpenditureOK(uint totalSpend)
        {
            bool spendLevelOK = true;

            // get available treasury
            int availTreasury = this.GetAvailableTreasury();

            // if there are funds in the treasury
            if (availTreasury > 0)
            {
                // expenditure should not exceed treasury
                if (totalSpend > availTreasury)
                {
                    spendLevelOK = false;
                }
            }
            // if treasury is empty or in deficit
            else
            {
                // expenditure should be 0
                if (totalSpend > 0)
                {
                    spendLevelOK = false;
                }
            }

            return spendLevelOK;
        }
        
        /// <summary>
        /// Updates fief data at the end/beginning of the season
        /// </summary>
        public void UpdateFief()
        {
            // update previous season's financial data
            this.keyStatsCurrent.CopyTo(this.keyStatsPrevious, 0);

            // update tax rate
            this.keyStatsCurrent[2] = this.taxRateNext;
            this.taxRate = this.taxRateNext;

            // update bailiffDaysInFief if appropriate
            if ((this.bailiff != null) && (this.bailiff.days > 0))
            {
                this.bailiff.UseUpDays();
            }

            // validate fief expenditure against treasury
            this.ValidateFiefExpenditure();

            // update loyalty level
            this.keyStatsCurrent[0] = this.CalcNewLoyalty();
            this.loyalty = this.keyStatsCurrent[0];

            // update GDP
            this.keyStatsCurrent[1] = this.CalcNewGDP();

            // update officials spend
            this.keyStatsCurrent[3] = this.officialsSpendNext;

            // update garrison spend
            this.keyStatsCurrent[4] = this.garrisonSpendNext;

            // update infrastructure spend
            this.keyStatsCurrent[5] = this.infrastructureSpendNext;

            // update keep spend
            this.keyStatsCurrent[6] = this.keepSpendNext;

            // update keep level (based on next season keep spend)
            this.keyStatsCurrent[7] = this.CalcNewKeepLevel();
            this.keepLevel = this.keyStatsCurrent[7];

            // update income
            this.keyStatsCurrent[8] = this.CalcNewIncome();

            // update family expenses
            this.keyStatsCurrent[9] = this.CalcFamilyExpenses();

            // update total expenses (including bailiff modifiers)
            this.keyStatsCurrent[10] = this.CalcFamilyExpenses() + this.CalcNewExpenses();

            // update overord taxes
            this.keyStatsCurrent[11] = this.CalcNewOlordTaxes();

            // update overord tax rate
            this.keyStatsCurrent[12] = this.province.taxRate;

            // update bottom line
            this.keyStatsCurrent[13] = this.CalcNewBottomLine();

            // check for unrest/rebellion
            this.status = this.CheckFiefStatus();

            // update fields level (based on next season infrastructure spend)
            this.fields = this.CalcNewFieldLevel();

            // update industry level (based on next season infrastructure spend)
            this.industry = this.CalcNewIndustryLevel();

            // update fief treasury with new bottom line
            this.Treasury += Convert.ToInt32(this.keyStatsCurrent[13]);

            // check for occupation before transfering overlord taxes into overlord's treasury
            if (!this.CheckEnemyOccupation())
            {
                // get overlord
                PlayerCharacter thisOverlord = this.GetOverlord();
                if (thisOverlord != null)
                {
                    // get overlord's home fief
                    Fief overlordHome = thisOverlord.GetHomeFief();

                    if (overlordHome != null)
                    {
                        // pay in taxes
                        overlordHome.Treasury += Convert.ToInt32(this.CalcNewOlordTaxes());
                    }
                }
            }

            // update population
            this.population = Convert.ToInt32(this.CalcNewPopulation());

            // reset bailiffDaysInFief
            this.bailiffDaysInFief = 0;

            // reset hasRecruited
            this.hasRecruited = false;

            // reset isPillaged
            this.isPillaged = false;

            // update transfers
            foreach (KeyValuePair<string, ProtoDetachment> transferEntry in this.troopTransfers)
            {
                // create temporary army to check attrition
                uint[] thisTroops = transferEntry.Value.troops;
                int days = transferEntry.Value.days;
                string owner = transferEntry.Value.leftBy;
                Army tempArmy = new Army(Globals_Game.GetNextArmyID(), null, owner, days, this.id, trp: thisTroops);

                // attrition checks
                byte attritionChecks = 0;
                attritionChecks = Convert.ToByte(days / 7);
                double attritionModifier = 0;

                for (int i = 0; i < attritionChecks; i++)
                {
                    attritionModifier = tempArmy.CalcAttrition();

                    // apply attrition
                    if (attritionModifier > 0)
                    {
                        tempArmy.ApplyTroopLosses(attritionModifier);
                    }
                }

                // update detachment days
                transferEntry.Value.days = 90;

                // set tempArmy to null
                tempArmy = null;
            }
        }

        /// <summary>
        /// Checks for transition from calm to unrest/rebellion, or from unrest to calm
        /// </summary>
        /// <returns>char indicating fief status</returns>
        public char CheckFiefStatus()
        {
            char originalStatus = this.status;
 
            // if fief in rebellion it can only be recovered by combat or bribe,
            // so don't run check
            if (!this.status.Equals('R'))
            {
                // method 1 (depends on tax rate and surplus)
                if ((this.taxRate > 20) && (this.keyStatsCurrent[13] > (this.keyStatsCurrent[8] * 0.1)))
                {
                    if (Utility_Methods.GetRandomDouble(100) <= (this.taxRate - 20))
                    {
                        this.status = 'R';
                    }
                }

                // method 2 (depends on fief loyalty level)
                if (!this.status.Equals('R'))
                {
                    double chance = Utility_Methods.GetRandomDouble(100);
                    
                    // loyalty 3-4
                    if ((this.loyalty > 3) && (this.loyalty <= 4))
                    {
                        if (chance <= 2)
                        {
                            this.status = 'R';
                        }
                        else if (chance <= 10)
                        {
                            this.status = 'U';
                        }
                        else
                        {
                            this.status = 'C';
                        }
                    }

                    // loyalty 2-3
                    else if ((this.loyalty > 2) && (this.loyalty <= 3))
                    {
                        if (chance <= 14)
                        {
                            this.status = 'R';
                        }
                        else if (chance <= 30)
                        {
                            this.status = 'U';
                        }
                        else
                        {
                            this.status = 'C';
                        }
                    }

                    // loyalty 1-2
                    else if ((this.loyalty > 1) && (this.loyalty <= 2))
                    {
                        if (chance <= 26)
                        {
                            this.status = 'R';
                        }
                        else if (chance <= 50)
                        {
                            this.status = 'U';
                        }
                        else
                        {
                            this.status = 'C';
                        }
                    }

                    // loyalty 0-1
                    else if ((this.loyalty > 0) && (this.loyalty <= 1))
                    {
                        if (chance <= 38)
                        {
                            this.status = 'R';
                        }
                        else if (chance <= 70)
                        {
                            this.status = 'U';
                        }
                        else
                        {
                            this.status = 'C';
                        }
                    }

                    // loyalty 0
                    else if (this.loyalty == 0)
                    {
                        if (chance <= 50)
                        {
                            this.status = 'R';
                        }
                        else if (chance <= 90)
                        {
                            this.status = 'U';
                        }
                        else
                        {
                            this.status = 'C';
                        }
                    }
                }
            }

            // if status changed
            if (this.status != originalStatus)
            {
                // if necessary, APPLY STATUS LOSS
                if (this.status.Equals('R'))
                {
                    this.owner.AdjustStatureModifier(-0.1);
                }

                // CREATE JOURNAL ENTRY
                // get old and new status
                string oldStatus = "";
                string newStatus = "";
                if (originalStatus.Equals('C'))
                {
                    oldStatus = "calm";
                }
                else if (originalStatus.Equals('U'))
                {
                    oldStatus = "unrest";
                }
                else if (originalStatus.Equals('R'))
                {
                    oldStatus = "rebellion";
                }
                if (this.status.Equals('C'))
                {
                    newStatus = "CALM";
                }
                else if (this.status.Equals('U'))
                {
                    newStatus = "UNREST";
                }
                else if (this.status.Equals('R'))
                {
                    newStatus = "REBELLION";
                }

                // get interested parties
                bool success = true;
                PlayerCharacter fiefOwner = this.owner;

                // ID
                uint entryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                List<string> tempPersonae = new List<string>();
                tempPersonae.Add(fiefOwner.charID + "|fiefOwner");
                if ((this.status.Equals('R')) || (oldStatus.Equals("R")))
                {
                    tempPersonae.Add("all|all");
                }
                string[] thisPersonae = tempPersonae.ToArray();

                // type
                string type = "";
                if (this.status.Equals('C'))
                {
                    type = "fiefStatusCalm";
                }
                else if (this.status.Equals('U'))
                {
                    type = "fiefStatusUnrest";
                }
                else if (this.status.Equals('R'))
                {
                    type = "fiefStatusRebellion";
                }

                // location
                string location = this.id;

                ProtoMessage fiefStatus = new ProtoMessage();
                fiefStatus.ResponseType = DisplayMessages.FiefStatus;
                fiefStatus.MessageFields = new string[] { this.name, fiefOwner.firstName + " " + fiefOwner.familyName, oldStatus, newStatus };
                // create and add a journal entry to the pastEvents journal
                JournalEntry newEntry = new JournalEntry(entryID, year, season, thisPersonae, type, fiefStatus,loc: location);
                success = Globals_Game.AddPastEvent(newEntry);
            }

            return this.status;
        }

        /// <summary>
        /// Adds character to characters list
        /// </summary>
        /// <param name="ch">Character to be inserted into characters list</param>
        internal void AddCharacter(Character ch)
        {
            // add character
            this.charactersInFief.Add(ch);
        }

        /// <summary>
        /// Removes character from characters list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character to be removed from characters list</param>
        internal bool RemoveCharacter(Character ch)
        {
            // remove character
            bool success = this.charactersInFief.Remove(ch);

            return success;
        }

        /// <summary>
        /// Adds army to armies list
        /// </summary>
        /// <param name="armyID">ID of army to be inserted</param>
        internal void AddArmy(String armyID)
        {
            // add army
            this.armies.Add(armyID);
        }

        /// <summary>
        /// Removes army from armies list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="armyID">ID of army to be removed</param>
        internal bool RemoveArmy(String armyID)
        {
            // remove army
            bool success = this.armies.Remove(armyID);

            return success;
        }

        /// <summary>
        /// Bar a specific character from the fief's keep
        /// </summary>
        /// <param name="ch">Character to be barred</param>
        internal void BarCharacter(string ch)
        {
            // bar character
            this.barredCharacters.Add(ch);
        }

        /// <summary>
        /// Removes a fief keep bar from a specific character
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character for whom bar to be removed</param>
        internal bool RemoveBarCharacter(string ch)
        {
            // remove character bar
            bool success = this.barredCharacters.Remove(ch);

            return success;
        }

        /// <summary>
        /// Calculates the result of a call for troops
        /// </summary>
        /// <returns>int containing number of troops responding to call</returns>
        /// <param name="minProportion">double specifying minimum proportion of total troop number required</param>
        /// <param name="maxProportion">double specifying maximum proportion of total troop number required</param>
        public int CallUpTroops(double minProportion = 0, double maxProportion = 1)
        {
            int numberTroops = 0;
            int maxNumber = this.CalcMaxTroops();

            // generate random double between min and max
            double myRandomDouble = Utility_Methods.GetRandomDouble(min: minProportion, max: maxProportion);

            // apply random double as modifier to maxNumber
            numberTroops = Convert.ToInt32(maxNumber * myRandomDouble);

            // check for effects of unrest (only get 50% of troops)
            if (this.status.Equals('U'))
            {
                numberTroops = numberTroops / 2;
            }

            return numberTroops;
        }

        /// <summary>
        /// Calculates the maximum number of troops available for call up in the fief
        /// </summary>
        /// <returns>int containing maximum number of troops</returns>
        public int CalcMaxTroops()
        {
            return Convert.ToInt32(this.population * 0.05);
        }

        /// <summary>
        /// Calculates the garrison size for the fief
        /// </summary>
        /// <returns>int containing the garrison size</returns>
        public int GetGarrisonSize()
        {
            int garrisonSize = 0;

            garrisonSize = Convert.ToInt32(this.keyStatsCurrent[4] / 1000);

            return garrisonSize;
        }

        /*
        /// <summary>
        /// Gets the fief's title holder
        /// </summary>
        /// <returns>the title holder</returns>
        public Character getTitleHolder()
        {
            Character myTitleHolder = null;

            if (this.titleHolder != null)
            {
                // get title holder from appropriate master list
                if (Globals_Game.npcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = Globals_Game.npcMasterList[this.titleHolder];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = v.pcMasterList[this.titleHolder];
                }
            }

            return myTitleHolder;
        } */

        /// <summary>
        /// Gets fief's overlord
        /// </summary>
        /// <returns>The overlord</returns>
        public PlayerCharacter GetOverlord()
        {
            PlayerCharacter myOverlord = null;

            if (!String.IsNullOrWhiteSpace(this.province.titleHolder))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.province.titleHolder))
                {
                    myOverlord = Globals_Game.pcMasterList[this.province.titleHolder];
                }
            }

            return myOverlord;
        }

        /// <summary>
        /// Gets the fief's siege object
        /// </summary>
        /// <returns>the siege</returns>
        public Siege GetSiege()
        {
            Siege mySiege = null;

            if (!String.IsNullOrWhiteSpace(this.siege))
            {
                // get siege
                if (Globals_Game.siegeMasterList.ContainsKey(this.siege))
                {
                    mySiege = Globals_Game.siegeMasterList[this.siege];
                }
            }

            return mySiege;
        }

        /*
        /// <summary>
        /// Transfers the fief title to the specified character
        /// </summary>
        /// <param name="newTitleHolder">The new title holder</param>
        public void transferTitle(Character newTitleHolder)
        {
            // remove title from existing holder
            Character oldTitleHolder = this.getTitleHolder();
            oldTitleHolder.myTitles.Remove(this.id);

            // add title to new owner
            newTitleHolder.myTitles.Add(this.id);
            this.titleHolder = newTitleHolder.charID;
        } */

        /// <summary>
        /// Gets the fief's rightful king (i.e. the king of the kingdom that the fief traditionally belongs to)
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetRightfulKing()
        {
            PlayerCharacter thisKing = null;

            if (this.province.kingdom.owner != null)
            {
                thisKing = this.province.kingdom.owner;
            }

            return thisKing;
        }

        /// <summary>
        /// Gets the fief's current king (i.e. the king of the current owner)
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetCurrentKing()
        {
            PlayerCharacter thisKing = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.owner.nationality)
                {
                    if (kingdomEntry.Value.owner != null)
                    {
                        thisKing = kingdomEntry.Value.owner;
                        break;
                    }
                }
            }

            return thisKing;
        }

        /// <summary>
        /// Checks if the fief is under enemy occupation
        /// </summary>
        /// <returns>bool indicating whether is under enemy occupation</returns>
        public bool CheckEnemyOccupation()
        {
            bool isOccupied = false;

            if (this.GetRightfulKing() != this.GetCurrentKing())
            {
                isOccupied = true;
            }

            return isOccupied;
        }

        /// <summary>
        /// Gets the fief's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (this.province.GetRightfulKingdom() != null)
            {
                thisKingdom = this.province.GetRightfulKingdom();
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the fief's current kingdom (i.e. the kingdom of the current owner)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetCurrentKingdom()
        {
            Kingdom thisKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.owner.nationality)
                {
                    thisKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return thisKingdom;
        }

        //ASK difference between owner and title holder, who to notify
        /// <summary>
        /// Processes the functions involved in a change of fief ownership
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newOwner">The new owner</param>
        /// <param name="circumstance">The circumstance under which the change of ownership is taking place</param>
        public bool ChangeOwnership(PlayerCharacter newOwner, out ProtoMessage error, string circumstance = "hostile")
        {
            error = null;
            string user = this.owner.playerID;
            bool success = true;

            // get old owner
            PlayerCharacter oldOwner = this.owner;

            // check if fief was old owner's home fief
            if (oldOwner.homeFief.Equals(this.id))
            {
                // cannot voluntarily give away home fief
                if (!circumstance.Equals("hostile"))
                {
                    success = false;
                    error = new ProtoMessage();
                    error.ResponseType = DisplayMessages.FiefOwnershipHome;
                }

                else
                {
                    List<Fief> candidateFiefs = new List<Fief>();
                    Fief newHome = null;

                    // remove from old owner
                    oldOwner.homeFief = null;

                    // check to see if old owner has any other fiefs
                    if (oldOwner.ownedFiefs.Count > 0)
                    {
                        // get currently owned ancestral fiefs
                        foreach (Fief thisFief in oldOwner.ownedFiefs)
                        {
                            if (thisFief.ancestralOwner == oldOwner)
                            {
                                candidateFiefs.Add(thisFief);
                            }
                        }

                        // check for highest ranking fief
                        if (candidateFiefs.Count > 0)
                        {
                            foreach (Fief thisFief in candidateFiefs)
                            {
                                if (newHome == null)
                                {
                                    newHome = thisFief;
                                }
                                else
                                {
                                    if (thisFief.rank.id > newHome.rank.id)
                                    {
                                        newHome = thisFief;
                                    }
                                }
                            }
                        }

                        // if no new home yet identified
                        if (newHome == null)
                        {
                            // get highest ranking owned fief and set as new home fief
                            candidateFiefs = oldOwner.GetHighestRankingFief();
                            if (candidateFiefs.Count > 0)
                            {
                                // if only one fief at this rank, make it
                                if (candidateFiefs.Count == 1)
                                {
                                    newHome = candidateFiefs[0];
                                }
                            }
                        }

                        if (newHome != null)
                        {
                            // set new home fief in character
                            oldOwner.homeFief = newHome.id;

                            // if old owner isn't new home fief's title holder, transfer title
                            Fief newHomeFief = oldOwner.GetHomeFief();
                            if (!newHomeFief.titleHolder.Equals(oldOwner.charID))
                            {
                                // remove title from existing holder and assign to old owner
                                oldOwner.TransferTitle(oldOwner, newHomeFief);
                            }
                        }

                        else
                        {
                            error = new ProtoMessage();
                            error.ResponseType = DisplayMessages.FiefOwnershipNewHome;
                            error.MessageFields = new string[] { oldOwner.firstName + " " + oldOwner.familyName };
                        }
                    }

                    // old owner has no more fiefs
                    else
                    {
                        error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.FiefOwnershipNoFiefs;
                        error.MessageFields = new string[] { oldOwner.firstName + " " + oldOwner.familyName };
                    }
                }
            }

            if (success)
            {
                // adjust loyalty
                // lose 10% if old owner was ancestral owner
                if (oldOwner == this.ancestralOwner)
                {
                    this.loyalty *= 0.9;
                }
                // gain 10% if new owner is ancestral owner
                else if (newOwner == this.ancestralOwner)
                {
                    this.loyalty *= 1.1;
                }

                // remove title from existing holder and assign to new owner
                oldOwner.TransferTitle(newOwner, this);

                // remove from existing owner
                oldOwner.ownedFiefs.Remove(this);

                // add to new owner
                newOwner.ownedFiefs.Add(this);
                this.owner = newOwner;

                // remove existing bailiff
                this.bailiff = null;

                // reset bailiffDaysInFief
                this.bailiffDaysInFief = 0;

                // check for status
                this.status = this.CheckFiefStatus();

                // make changes to barred characters, etc. if necessary
                // new owner
                if (this.barredCharacters.Contains(newOwner.charID))
                {
                    this.barredCharacters.Remove(newOwner.charID);
                }

                // new owner's NPCs
                for (int i = 0; i < newOwner.myNPCs.Count; i++)
                {
                    if (this.barredCharacters.Contains(newOwner.myNPCs[i].charID))
                    {
                        this.barredCharacters.Remove(newOwner.myNPCs[i].charID);
                    }
                }

                // new owner's nationality
                if (this.barredNationalities.Contains(newOwner.nationality.natID))
                {
                    this.barredNationalities.Remove(newOwner.nationality.natID);
                }

                // Transfer Captives
                foreach (Character captive in gaol)
                {
                    oldOwner.myCaptives.Remove(captive);
                    newOwner.myCaptives.Add(captive);
                }
                // CREATE JOURNAL ENTRY
                bool entryAdded = true;

                // ID
                uint entryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                string[] thisPersonae = new string[] { newOwner.charID + "|newOwner", oldOwner.charID + "|oldOwner" };

                // type
                string type = "";
                if (circumstance.Equals("hostile"))
                {
                    type = "fiefOwnership_Hostile";
                }
                else
                {
                    type = "fiefOwnership_Gift";
                }

                // location
                string location = this.id;

                // description
                string[] fields = new string[4];
                fields[0] = this.name;
                string oldOwnerTitle = "";
                if (circumstance.Equals("hostile"))
                {
                    fields[1] =  "has passed from";
                }
                else
                {
                    fields[1] = "was granted by";
                    oldOwnerTitle = "His Majesty ";
                }
				fields[2]= oldOwnerTitle + oldOwner.firstName + " " + oldOwner.familyName;
                fields[3] = newOwner.firstName + " " + newOwner.familyName;

                ProtoMessage changeOwner = new ProtoMessage();
                changeOwner.ResponseType = DisplayMessages.FiefChangeOwnership;
                changeOwner.MessageFields = fields;
                // create and add a journal entry to the pastEvents journal
                JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type, changeOwner, loc: location);
                entryAdded = Globals_Game.AddPastEvent(thisEntry);
            }

            return success;
        }

        /// <summary>
        /// Checks to see if there is currently a field army in the fief keep
        /// </summary>
        /// <returns>bool indicating presence of a field army</returns>
        public bool CheckFieldArmyInKeep()
        {
            bool armyInKeep = false;

            foreach (Character thisChar in this.charactersInFief)
            {
                if ((!String.IsNullOrWhiteSpace(thisChar.armyID)) && (thisChar.inKeep))
                {
                    armyInKeep = true;
                    break;
                }
            }

            return armyInKeep;
        }

        /// <summary>
        /// Checks to see if an attempts to quell a rebellion has been successful
        /// </summary>
        /// <returns>bool indicating quell success or failure</returns>
        /// <param name="a">The army trying to quell the rebellion</param>
        public bool Quell_checkSuccess(Army a)
        {
            bool rebellionQuelled = false;

            // calculate base chance of success, based on army size and fief population
            double successChance = a.CalcArmySize() / (this.population / Convert.ToDouble(1000));

            // get army leader
            Character aLeader = a.GetLeader();

            // get army owner
            Character aOwner = a.GetOwner();

            // apply any bonus for leadership traits
            if (aLeader != null)
            {
                successChance += aLeader.GetLeadershipValue();
            }

            // apply any bonus for ancestral ownership
            if (aOwner != null)
            {
                // only if army owner is ancestral owner & current owner isn't
                if ((aOwner == this.ancestralOwner) && (this.owner != this.ancestralOwner))
                {
                    successChance += (aOwner.CalculateStature() * 2.22);
                }
            }

            // ensure successChance always between 1 > 99 (to allow for minimum chance of success/failure)
            if (successChance < 1)
            {
                successChance = 1;
            }
            else if (successChance > 99)
            {
                successChance = 99;
            }

            // generate random double 0-100 to check for success
            rebellionQuelled = (Utility_Methods.GetRandomDouble(101) <= successChance);

            return rebellionQuelled;
        }

        /// <summary>
        /// Attempts to quell the rebellion in the fief using the specified army
        /// </summary>
        /// <returns>bool indicating quell success or failure</returns>
        /// <param name="a">The army trying to quell the rebellion</param>
        public bool QuellRebellion(Army a,out ProtoMessage message)
        {
            message = null;
            // check to see if quell attempt was successful
            bool quellSuccessful = this.Quell_checkSuccess(a);

            // if quell successful
            if (quellSuccessful)
            {
                // process change of ownership, if appropriate
                if (this.owner != a.GetOwner())
                {
                    this.ChangeOwnership(a.GetOwner(),out message);
                }

                // set status
                this.status = 'C';
            }

            // if quell not successful
            else
            {
                // CREATE JOURNAL ENTRY
                // get interested parties
                bool success = true;
                PlayerCharacter fiefOwner = this.owner;
                PlayerCharacter attackerOwner = a.GetOwner();
                Character attackerLeader = null;
                if (a.GetLeader() != null)
                {
                    attackerLeader = a.GetLeader();
                }

                // ID
                uint entryID = Globals_Game.GetNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                List<string> tempPersonae = new List<string>();
                tempPersonae.Add(fiefOwner.charID + "|fiefOwner");
                tempPersonae.Add(attackerOwner.charID + "|attackerOwner");
                if (attackerLeader != null)
                {
                    tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
                }
                string[] quellPersonae = tempPersonae.ToArray();

                // type
                string type = "rebellionQuellFailed";

                // location
                string location = this.id;

                // description
                string[] fields = new string[4];
                fields[0]  = attackerOwner.firstName + " " + attackerOwner.familyName;
                fields[1] = "";
                if (attackerLeader != null)
                {
                    fields[1] = ", led by " + attackerLeader.firstName + " " + attackerLeader.familyName + ",";
                }
                fields[2] =  this.name;
                fields[3] = fiefOwner.firstName + " " + fiefOwner.familyName;

                ProtoMessage quellRebellion = new ProtoMessage();
                quellRebellion.ResponseType = DisplayMessages.FiefQuellRebellionFail;
                quellRebellion.MessageFields = fields;
                // create and add a journal entry to the pastEvents journal
                JournalEntry quellEntry = new JournalEntry(entryID, year, season, quellPersonae, type,quellRebellion, loc: location);
                success = Globals_Game.AddPastEvent(quellEntry);
            }

            return quellSuccessful;
        }

        /// <summary>
        /// Gets travel cost (in days) to move to a nieghbouring fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="target">Target fief</param>
        public double getTravelCost(Fief target, string armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (this.terrain.travelCost + target.terrain.travelCost) / 2;

            // apply season modifier
            cost = cost * Globals_Game.clock.CalcSeasonTravMod();

            // if necessary, apply army modifier
            if (!String.IsNullOrWhiteSpace(armyID))
            {
                cost = cost * Globals_Game.armyMasterList[armyID].CalcMovementModifier();
            }

            return cost;
        }


        public ProtoFief DisplayFiefData(bool isOwner)
        {
            ProtoFief fiefstats = new ProtoFief(this);
            if (isOwner)
            {
                fiefstats.includeAll(this);
                bool displayData = true;
                if (!String.IsNullOrWhiteSpace(this.siege))
                {
                    Siege thisSiege = this.GetSiege();
                    displayData = this.CheckToShowFinancialData(-1, thisSiege);
                }

                // if not OK to display data, show message
                if (!displayData)
                {
                    fiefstats.keyStatsPrevious = null;
                    fiefstats.keyStatsNext = null;
                }
            }
            return fiefstats;
        }

        

        /// <summary>
        /// Checks to see if display of financial data for the specified financial period
        /// is permitted due to ongoing siege
        /// </summary>
        /// <returns>bool indicating whether display is permitted</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        /// <param name="s">The siege</param>
        public bool CheckToShowFinancialData(int relativeSeason, Siege s)
        {
            bool displayData = true;

            uint financialPeriodYear = this.GetFinancialYear(relativeSeason);
            if (financialPeriodYear > s.startYear)
            {
                displayData = false;
            }
            else if (financialPeriodYear == s.startYear)
            {
                byte financialPeriodSeason = this.GetFinancialSeason(relativeSeason);
                if (financialPeriodSeason > s.startSeason)
                {
                    displayData = false;
                }
            }

            return displayData;
        }

        /// <summary>
        /// Gets the year for the specified financial period
        /// </summary>
        /// <returns>The year</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public uint GetFinancialYear(int relativeSeason)
        {
            uint financialYear = 0;
            uint thisYear = Globals_Game.clock.currentYear;

            switch (relativeSeason)
            {
                case (-1):
                    if (Globals_Game.clock.currentSeason == 0)
                    {
                        financialYear = thisYear - 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                case (1):
                    if (Globals_Game.clock.currentSeason == 4)
                    {
                        financialYear = thisYear + 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                default:
                    financialYear = thisYear;
                    break;
            }

            return financialYear;
        }

        /// <summary>
        /// Gets the season for the specified financial period
        /// </summary>
        /// <returns>The season</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public byte GetFinancialSeason(int relativeSeason)
        {
            byte financialSeason = 0;
            byte thisSeason = Globals_Game.clock.currentSeason;

            switch (relativeSeason)
            {
                case (-1):
                    if (thisSeason == 0)
                    {
                        financialSeason = 4;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason--;
                    }
                    break;
                case (1):
                    if (thisSeason == 4)
                    {
                        financialSeason = 0;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason++;
                    }
                    break;
                default:
                    financialSeason = thisSeason;
                    break;
            }

            return financialSeason;
        }
        /// <summary>
        /// Transfers funds between the fief treasury and another fief's treasury
        /// </summary>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        public bool TreasuryTransfer(Fief to, int amount, out ProtoMessage error)
        {
            error = null;
            // ensure number is positive
            amount = Math.Abs(amount);
            // check enough for transfer
            if (this.Treasury < amount)
            {
                error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericInsufficientFunds;
                return false;
            }
            else
            {
                // Lock
                lock (treasuryTransferLock)
                {
                    // subtract from source treasury
                    this.AdjustTreasury(-amount);

                    // add to target treasury
                    to.AdjustTreasury(amount);
                }
                
                return true;
            }
        }

        public void AdjustTreasury(int amount)
        {
            lock (treasuryLock)
            {
                this.Treasury += amount;
            }
        }
        /// <summary>
        /// Creates a defending army for defence of a fief during pillage or siege
        /// </summary>
        /// <returns>The defending army</returns>
        public Army CreateDefendingArmy()
        {
            Army defender = null;
            Character armyLeader = null;
            string armyLeaderID = null;
            double armyLeaderDays = 90;

            // if present in fief, get bailiff and assign as army leader
            if (this.bailiff != null)
            {
                for (int i = 0; i < this.charactersInFief.Count; i++)
                {
                    if (this.charactersInFief[i] == this.bailiff)
                    {
                        armyLeader = this.bailiff;
                        armyLeaderID = armyLeader.charID;
                        armyLeaderDays = armyLeader.days;
                        break;
                    }
                }
            }

            // gather troops to create army
            uint garrisonSize = 0;
            uint militiaSize = 0;
            uint[] troopsForArmy = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint[] tempTroops = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint totalSoFar = 0;

            // get army nationality
            string thisNationality = this.owner.nationality.natID;

            // get size of fief garrison
            garrisonSize = Convert.ToUInt32(this.GetGarrisonSize());

            // get size of fief 'militia' responding to emergency
            militiaSize = Convert.ToUInt32(this.CallUpTroops(minProportion: 0.33, maxProportion: 0.66));

            // get defending troops based on following troop type proportions:
            // militia = Global_Server.recruitRatios for types 0-4, fill with rabble
            // garrison = Global_Server.recruitRatios * 2 for types 0-3, fill with foot

            // 1. militia (includes proportion of rabble)
            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = Convert.ToUInt32(militiaSize * Globals_Server.recruitRatios[thisNationality][i]);
                }
                // fill up with rabble
                else
                {
                    tempTroops[i] = militiaSize - totalSoFar;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // 2. garrison (all 'professional' troops)
            totalSoFar = 0;

            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 2)
                {
                    tempTroops[i] = Convert.ToUInt32(garrisonSize * (Globals_Server.recruitRatios[thisNationality][i] * 2));
                }
                // fill up with foot
                else if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = garrisonSize - totalSoFar;
                }
                // no rabble in garrison
                else
                {
                    tempTroops[i] = 0;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // create temporary army for battle/siege
            defender = new Army("Garrison" + Globals_Game.GetNextArmyID(), armyLeaderID, this.owner.charID, armyLeaderDays, this.id, trp: troopsForArmy);
            defender.AddArmy();

            return defender;
        }

        /// <summary>
        /// Bars a Character from the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeBarred">The Character to be barred</param>
        public bool BarCharacter(Character toBeBarred)
        {
            bool success = true;
            string user = this.owner.playerID;
            // add ID to barred characters
            this.barredCharacters.Add(toBeBarred.charID);

            // check if is currently in keep of barring fief, and remove if necessary
            if ((toBeBarred.inKeep) && (toBeBarred.location == this))
            {
                toBeBarred.inKeep = false;
                // update place owner
                if (!string.IsNullOrEmpty(user))
                {
                    Globals_Game.UpdatePlayer(user, DisplayMessages.FiefEjectCharacter, new string[] {toBeBarred.firstName + " " + toBeBarred.familyName,this.name});
                }
                // Get and notify owner of barred character that they have been ejected
                PlayerCharacter barredOwner = null;
                barredOwner = toBeBarred.GetHeadOfFamily();
                if (barredOwner == null)
                {
                    barredOwner = (toBeBarred as NonPlayerCharacter).GetEmployer();
                }
                if (barredOwner != null)
                {
                    if (barredOwner == toBeBarred) Globals_Game.UpdatePlayer(barredOwner.playerID, DisplayMessages.FiefEjectCharacter, new string[] { "You", this.name });
                    else Globals_Game.UpdatePlayer(barredOwner.playerID, DisplayMessages.FiefEjectCharacter, new string[] { toBeBarred.firstName + " " + toBeBarred.familyName, this.name });
                }
            }

            // check for success
            if (!this.barredCharacters.Contains(toBeBarred.charID))
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Unbars a Character
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeUnbarred">ID of Character to be unbarred</param>
        public bool UnbarCharacter(string toBeUnbarred)
        {
            bool success = true;

            // check for empty string
            if (!String.IsNullOrWhiteSpace(toBeUnbarred))
            {
                // remove ID from barred characters
                if (this.barredCharacters.Contains(toBeUnbarred))
                {
                    this.barredCharacters.Remove(toBeUnbarred);
                }

                // check for success
                if (this.barredCharacters.Contains(toBeUnbarred))
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Bars a Nationality from entering the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeBarred">The ID of the Nationality to be barred</param>
        public bool BarNationality(string toBeBarred)
        {
            bool success = true;

            // add ID to barred nationalities
            this.barredNationalities.Add(toBeBarred);

            // check for success
            if (!this.barredNationalities.Contains(toBeBarred))
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Unbars a Nationality from entering the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeUnbarred">The ID of the Nationality to be unbarred</param>
        public bool UnbarNationality(string toBeUnbarred)
        {
            bool success = true;

            // check for empty string
            if (!String.IsNullOrWhiteSpace(toBeUnbarred))
            {
                // remove ID from barred nationalities
                if (this.barredNationalities.Contains(toBeUnbarred))
                {
                    this.barredNationalities.Remove(toBeUnbarred);
                }

                // check for success
                if (this.barredNationalities.Contains(toBeUnbarred))
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Returns descriptions of characters in Court, Tavern, outside keep for this fief
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        /// <param name="ch">PlayerCharacter viewing these details </param>
        public ProtoCharacterOverview[] ListCharsInMeetingPlace(string place, Character pc)
        {
            List<ProtoCharacterOverview> charsToView = new List<ProtoCharacterOverview>();
            // select which characters to display - i.e. in the keep (court) or not (tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            // iterates through characters
            for (int i = 0; i < this.charactersInFief.Count; i++)
            {
                // only display characters in relevant location (in keep, or not)
                if (this.charactersInFief[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (this.charactersInFief[i] != pc&&string.IsNullOrWhiteSpace(charactersInFief[i].captorID))
                    {
                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (this.charactersInFief[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((this.charactersInFief[i] as NonPlayerCharacter).salary == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsToView.Add(new ProtoCharacterOverview(this.charactersInFief[i]));
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsToView.Add(new ProtoCharacterOverview(this.charactersInFief[i]));
                                break;
                        }

                    }
                }

            }
            return charsToView.ToArray();
        }


        //temp for serializing to Client side Fief object
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Use the AddValue method to specify serialized values.
            info.AddValue("ter", this.terrain.id, typeof(string));
            info.AddValue("prov", this.province.id, typeof(string));
            info.AddValue("lang", this.language.id, typeof(string));
        }

        public Fief(SerializationInfo info, StreamingContext context) : base(info,context)
        {
            var tmpTerr = info.GetString("ter");
            var tmpProv = info.GetString("prov");
            var tmpLang = info.GetString("lang");
            this.terrain = Globals_Game.terrainMasterList[tmpTerr];
            this.province = Globals_Game.provinceMasterList[tmpProv];
            this.language = Globals_Game.languageMasterList[tmpLang];
        }
    }

    

	/// <summary>
    /// Class used to convert Fief to/from serialised format (JSON)
	/// </summary>
	public class Fief_Serialised : Place_Serialised
	{
        /// <summary>
		/// Holds fief's Province object (provinceID)
		/// </summary>
		public String province { get; set; }
		/// <summary>
		/// Holds fief population
		/// </summary>
		public int population { get; set; }
		/// <summary>
		/// Holds fief field level
		/// </summary>
		public Double fields { get; set; }
		/// <summary>
		/// Holds fief industry level
		/// </summary>
		public Double industry { get; set; }
		/// <summary>
		/// Holds no. trrops in fief
		/// </summary>
		public uint troops { get; set; }
		/// <summary>
		/// Holds fief tax rate
		/// </summary>
		public Double taxRate { get; set; }
		/// <summary>
		/// Holds fief tax rate (next season)
		/// </summary>
		public Double taxRateNext { get; set; }
		/// <summary>
		/// Holds expenditure on officials (next season)
		/// </summary>
		public uint officialsSpendNext { get; set; }
		/// <summary>
		/// Holds expenditure on garrison (next season)
		/// </summary>
		public uint garrisonSpendNext { get; set; }
		/// <summary>
		/// Holds expenditure on infrastructure (next season)
		/// </summary>
		public uint infrastructureSpendNext { get; set; }
		/// <summary>
		/// Holds expenditure on keep (next season)
		/// </summary>
		public uint keepSpendNext { get; set; }
        /// <summary>
        /// Holds key data for current season
        /// 0 = loyalty
        /// 1 = GDP
        /// 2 = tax rate
        /// 3 = official expenditure
        /// 4 = garrison expenditure
        /// 5 = infrastructure expenditure
        /// 6 = keep expenditure
        /// 7 = keep level
        /// 8 = income
        /// 9 = family expenses
        /// 10 = total expenses
        /// 11 = overlord taxes
        /// 12 = overlord tax rate
        /// 13 = bottom line
        /// </summary>
        public double[] keyStatsCurrent = new double[14];
        /// <summary>
        /// Holds key data for previous season
        /// </summary>
        public double[] keyStatsPrevious = new double[14];
        /// <summary>
		/// Holds fief keep level
		/// </summary>
		public Double keepLevel { get; set; }
		/// <summary>
		/// Holds fief loyalty
		/// </summary>
		public Double loyalty { get; set; }
		/// <summary>
		/// Holds fief status (code)
		/// </summary>
		public char status { get; set; }
        /// <summary>
        /// Holds fief language (ID)
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// Holds terrain object (terrainCode)
		/// </summary>
		public String terrain { get; set; }
		/// <summary>
		/// Holds list of characters present in fief (charIDs)
		/// </summary>
		public List<String> charactersInFief = new List<String>();
        /// <summary>
        /// Characters in gaol
        /// </summary>
        public List<string> gaol = new List<string>();
		/// <summary>
		/// Holds list of characters banned from keep (charIDs)
		/// </summary>
		public List<string> barredCharacters = new List<string>();
        /// <summary>
        /// Holds nationalitie banned from keep (IDs)
        /// </summary>
        public List<string> barredNationalities = new List<string>();
        /// <summary>
		/// Holds fief ancestral owner (charID)
		/// </summary>
		public String ancestralOwner { get; set; }
		/// <summary>
		/// Holds fief bailiff (charID)
		/// </summary>
		public String bailiff { get; set; }
        /// <summary>
        /// Number of days the bailiff has been resident in the fief (this season)
        /// </summary>
        public Double bailiffDaysInFief { get; set; }
        /// <summary>
        /// Holds fief treasury
        /// </summary>
        public int treasury { get; set; }
        /// <summary>
        /// Holds armies present in the fief (armyIDs)
        /// </summary>
        public List<string> armies = new List<string>();
        /// <summary>
        /// Identifies if recruitment has occurred in the fief
        /// </summary>
        public bool hasRecruited { get; set; }
        /// <summary>
        /// Identifies if pillage has occurred in the fief in the current season
        /// </summary>
        public bool isPillaged { get; set; }
        /// <summary>
        /// Holds troop detachments in the fief awaiting transfer
        /// String[] contains from (charID), to (charID), size, days left when detached
        /// </summary>
        public Dictionary<string, ProtoDetachment> troopTransfers = new Dictionary<string, ProtoDetachment>();
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public String siege { get; set; }

		/// <summary>
        /// Constructor for Fief_Serialised
		/// </summary>
		/// <param name="f">Fief object to use as source</param>
		public Fief_Serialised(Fief f)
            : base(f: f)
		{
            this.province = f.province.id;
			this.population = f.population;
			this.ancestralOwner = f.ancestralOwner.charID;
			if (f.bailiff != null) {
				this.bailiff = f.bailiff.charID;
			} else {
				this.bailiff = null;
			}
			this.fields = f.fields;
			this.industry = f.industry;
			this.troops = f.troops;
			this.taxRate = f.taxRate;
			this.taxRateNext = f.taxRateNext;
			this.officialsSpendNext = f.officialsSpendNext;
			this.garrisonSpendNext = f.garrisonSpendNext;
			this.infrastructureSpendNext = f.infrastructureSpendNext;
			this.keepSpendNext = f.keepSpendNext;
            this.keyStatsCurrent = f.keyStatsCurrent;
            this.keyStatsPrevious = f.keyStatsPrevious;
			this.keepLevel = f.keepLevel;
			this.loyalty = f.loyalty;
			this.status = f.status;
            this.language = f.language.id;
			this.terrain = f.terrain.id;
			if (f.charactersInFief.Count > 0)
			{
				for (int i = 0; i < f.charactersInFief.Count; i++)
				{
					this.charactersInFief.Add (f.charactersInFief[i].charID);
				}
			}
            foreach (Character prisoner in f.gaol)
            {
                this.gaol.Add(prisoner.charID);
            }
			this.barredCharacters = f.barredCharacters;
			this.barredNationalities = f.barredNationalities;
            this.bailiffDaysInFief = f.bailiffDaysInFief;
            this.treasury = f.Treasury;
            this.armies = f.armies;
            this.hasRecruited = f.hasRecruited;
            this.troopTransfers = f.troopTransfers;
            this.isPillaged = f.isPillaged;
            this.siege = f.siege;
		}

        /// <summary>
        /// Constructor for Fief_Serialised taking seperate values.
        /// For creating Fief_Serialised from CSV file.
        /// </summary>
        /// <param name="prov">String holding Fief's Province object (id)</param>
        /// <param name="pop">uint holding fief population</param>
        /// <param name="fld">Double holding fief field level</param>
        /// <param name="fld">Double holding fief industry level</param>
        /// <param name="trp">uint holding no. of troops in fief</param>
        /// <param name="tx">Double holding fief tax rate</param>
        /// <param name="txNxt">Double holding fief tax rate (next season)</param>
        /// <param name="offNxt">uint holding officials expenditure (next season)</param>
        /// <param name="garrNxt">uint holding garrison expenditure (next season)</param>
        /// <param name="infraNxt">uint holding infrastructure expenditure (next season)</param>
        /// <param name="keepNxt">uint holding keep expenditure (next season)</param>
        /// <param name="finCurr">Double [] holding financial data for current season</param>
        /// <param name="finPrev">Double [] holding financial data for previous season</param>
        /// <param name="kpLvl">Double holding fief keep level</param>
        /// <param name="loy">Double holding fief loyalty rating</param>
        /// <param name="stat">char holding fief status</param>
        /// <param name="lang">String holding Language object (id)</param>
        /// <param name="terr">String holding Terrain object (id)</param>
        /// <param name="chars">List holding characters present (id)</param>
        /// <param name="barChars">List holding IDs of characters barred from keep</param>
        /// <param name="barNats">List holding IDs of nationalities barred from keep</param>
        /// <param name="ancOwn">String holding ancestral owner (id)</param>
        /// <param name="bail">String holding fief bailiff (id)</param>
        /// <param name="bailInF">double holding days bailiff in fief</param>
        /// <param name="treas">int containing fief treasury</param>
        /// <param name="arms">List holding IDs of armies present in fief</param>
        /// <param name="rec">bool indicating whether recruitment has occurred in the fief (current season)</param>
        /// <param name="pil">bool indicating whether pillage has occurred in the fief (current season)</param>
        /// <param name="trans">Dictionary<string, string[]> containing troop detachments in the fief awaiting transfer</param>
        /// <param name="sge">String holding siegeID of active siege</param>
        public Fief_Serialised(String id, String nam, string prov, int pop, Double fld, Double ind, uint trp, Double tx,
            Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, double[] finCurr, double[] finPrev,
            Double kpLvl, Double loy, char stat, string lang, string terr, List<string> chars, List<string> barChars, List<string> barNats,
            double bailInF, int treas, List<string> arms, bool rec, Dictionary<string, ProtoDetachment> trans, bool pil, byte r, String tiHo = null,
            string own = null, string ancOwn = null, string bail = null, string sge = null)
            : base(id, nam, own: own, r: r, tiHo: tiHo)
        {
            // VALIDATION

            //PROV
            // trim and ensure is uppercase
            prov = prov.Trim().ToUpper();

            if (!Utility_Methods.ValidatePlaceID(prov))
            {
                throw new InvalidDataException("Fief_Serialised province ID must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // POP
            if (pop < 1)
            {
                throw new InvalidDataException("Fief_Serialised population must be an integer greater than 0");
            }

            // FLD
            if (!Utility_Methods.ValidateFiefDouble(fld))
            {
                throw new InvalidDataException("Fief_Serialised field level must be a double >= 0");
            }

            // IND
            if (!Utility_Methods.ValidateFiefDouble(ind))
            {
                throw new InvalidDataException("Fief_Serialised industry level must be a double >= 0");
            }

            // TAX
            if (!Utility_Methods.ValidatePercentage(tx))
            {
                throw new InvalidDataException("Fief_Serialised taxrate must be a double between 0 and 100");
            }

            // TAXNEXT
            if (!Utility_Methods.ValidatePercentage(txNxt))
            {
                throw new InvalidDataException("Fief_Serialised taxrate for next season must be a double between 0 and 100");
            }

            // FINCUR
            // 0 = loyalty
            if (!Utility_Methods.ValidateFiefDouble(finCurr[0], 9))
            {
                throw new InvalidDataException("Fief_Serialised finCurr[0] (loyalty) must be a double between 0 and 9");
            }

            // 1 = GDP
            int gdpCurr = Convert.ToInt32(finCurr[1]);

            // 2 = tax rate,
            if (!Utility_Methods.ValidatePercentage(finCurr[2]))
            {
                throw new InvalidDataException("Fief_Serialised finCurr[2] (taxrate) must be a double between 0 and 100");
            }

            // 3 = official expenditure,
            uint offCurr = Convert.ToUInt32(finCurr[3]);

            // 4 = garrison expenditure,
            uint garrCurr = Convert.ToUInt32(finCurr[4]);

            // 5 = infrastructure expenditure,
            uint infCurr = Convert.ToUInt32(finCurr[5]);

            // 6 = keep expenditure,
            uint kpCurr = Convert.ToUInt32(finCurr[6]);

            // 7 = keep level,
            if (!Utility_Methods.ValidateFiefDouble(finCurr[7]))
            {
                throw new InvalidDataException("Fief_Serialised finCurr[7] (keep level) must be a double >= 0");
            }

            // 8 = income,
            uint incCurr = Convert.ToUInt32(finCurr[8]);

            // 9 = family expenses,
            uint famCurr = Convert.ToUInt32(finCurr[9]);

            // 10 = total expenses,
            uint expCurr = Convert.ToUInt32(finCurr[10]);

            // 11 = overlord taxes,
            uint otaxCurr = Convert.ToUInt32(finCurr[11]);

            // 12 = overlord tax rate,
            if (!Utility_Methods.ValidatePercentage(finCurr[12]))
            {
                throw new InvalidDataException("Fief_Serialised finCurr[12] (overlord taxrate) must be a double between 0 and 100");
            }

            // 13 = bottom line
            int botCurr = Convert.ToInt32(finCurr[13]);

            // FINPREV
            // 0 = loyalty,
            if (!Utility_Methods.ValidateFiefDouble(finPrev[0], 9))
            {
                throw new InvalidDataException("Fief_Serialised finPrev[0] (loyalty) must be a double between 0 and 9");
            }

            // 1 = GDP
            int gdpPrev = Convert.ToInt32(finPrev[1]);

            // 2 = tax rate,
            if (!Utility_Methods.ValidatePercentage(finPrev[2]))
            {
                throw new InvalidDataException("Fief_Serialised finPrev[2] (taxrate) must be a double between 0 and 100");
            }

            // 3 = official expenditure,
            uint offPrev = Convert.ToUInt32(finPrev[3]);

            // 4 = garrison expenditure,
            uint garrPrev = Convert.ToUInt32(finPrev[4]);

            // 5 = infrastructure expenditure,
            uint infPrev = Convert.ToUInt32(finPrev[5]);

            // 6 = keep expenditure,
            uint kpPrev = Convert.ToUInt32(finPrev[6]);

            // 7 = keep level,
            if (!Utility_Methods.ValidateFiefDouble(finPrev[7]))
            {
                throw new InvalidDataException("Fief_Serialised finPrev[7] (keep level) must be a double >= 0");
            }

            // 8 = income,
            uint incPrev = Convert.ToUInt32(finPrev[8]);

            // 9 = family expenses,
            uint famPrev = Convert.ToUInt32(finPrev[9]);

            // 10 = total expenses,
            uint expPrev = Convert.ToUInt32(finPrev[10]);

            // 11 = overlord taxes,
            uint otaxPrev = Convert.ToUInt32(finPrev[11]);

            // 12 = overlord tax rate,
            if (!Utility_Methods.ValidatePercentage(finPrev[12]))
            {
                throw new InvalidDataException("Fief_Serialised finPrev[12] (overlord taxrate) must be a double between 0 and 100");
            }

            // 13 = bottom line
            int botPrev = Convert.ToInt32(finPrev[13]);

            // KPLVL
            if (!Utility_Methods.ValidateFiefDouble(kpLvl))
            {
                throw new InvalidDataException("Fief_Serialised keep level must be a double >= 0");
            }

            // LOY
            if (!Utility_Methods.ValidateFiefDouble(loy, 9))
            {
                throw new InvalidDataException("Fief_Serialised loyalty must be a double between 0 and 9");
            }

            // STAT
            // convert to uppercase
            stat = Convert.ToChar(stat.ToString().ToUpper());

            if (!(Regex.IsMatch(stat.ToString(), "[CRU]")))
            {
                throw new InvalidDataException("Fief_Serialised status must be 'C', 'U' or 'R'");
            }

            // LANG
            // trim
            lang = lang.Trim();

            if (!Utility_Methods.ValidateLanguageID(lang))
            {
                throw new InvalidDataException("Fief_Serialised language ID must have the format 'lang_' followed by 1-2 letters, ending in 1-2 numbers");
            }

            // TERR
            // trim
            terr = terr.Trim();

            if (!Utility_Methods.ValidateTerrainID(terr))
            {
                throw new InvalidDataException("Fief_Serialised terrain ID must have the format 'terr_' followed by some letters");
            }

            // CHARS
            for (int i = 0; i < chars.Count; i++)
            {
                // trim and ensure 1st is uppercase
                chars[i] = Utility_Methods.FirstCharToUpper(chars[i].Trim());

                if (!Utility_Methods.ValidateCharacterID(chars[i]))
                {
                    throw new InvalidDataException("All Fief_Serialised character IDs must have the format 'Char_' followed by some numbers");
                }
            }

            // BARCHARS
            for (int i = 0; i < barChars.Count; i++)
            {
                // trim and ensure 1st is uppercase
                barChars[i] = Utility_Methods.FirstCharToUpper(barChars[i].Trim());

                if (!Utility_Methods.ValidateCharacterID(barChars[i]))
                {
                    throw new InvalidDataException("All Fief_Serialised barred character IDs must have the format 'Char_' followed by some numbers");
                }
            }

            // BARNATS
            for (int i = 0; i < barNats.Count; i++)
            {
                // trim and ensure 1st is uppercase
                barNats[i] = Utility_Methods.FirstCharToUpper(barNats[i].Trim());

                if (!Utility_Methods.ValidateNationalityID(barNats[i]))
                {
                    throw new InvalidDataException("All Fief_Serialised barred nationality IDs must be 1-3 characters long, and consist entirely of letters");
                }
            }

            // BAILIFFDAYSINFIEF
            if (!Utility_Methods.ValidateDays(bailInF))
            {
                throw new InvalidDataException("Fief_Serialised bailiffDaysInFief must be a double between 0-109");
            }

            // ARMS
            for (int i = 0; i < arms.Count; i++)
            {
                // trim and ensure 1st is uppercase
                arms[i] = Utility_Methods.FirstCharToUpper(arms[i].Trim());

                if (!Utility_Methods.ValidateArmyID(arms[i]))
                {
                    throw new InvalidDataException("All Fief_Serialised army IDs must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            // ANCOWN
            // trim and ensure 1st is uppercase
            ancOwn = Utility_Methods.FirstCharToUpper(ancOwn.Trim());

            if (!Utility_Methods.ValidateCharacterID(ancOwn))
            {
                throw new InvalidDataException("Fief_Serialised ancestral owner ID must have the format 'Char_' followed by some numbers");
            }

            // BAIL
            if (!String.IsNullOrWhiteSpace(bail))
            {
                // trim and ensure 1st is uppercase
                bail = Utility_Methods.FirstCharToUpper(bail.Trim());

                if (!Utility_Methods.ValidateCharacterID(bail))
                {
                    throw new InvalidDataException("Fief_Serialised bailiff ID must have the format 'Char_' followed by some numbers");
                }
            }

            // SIEGE
            if (!String.IsNullOrWhiteSpace(sge))
            {
                // trim and ensure 1st is uppercase
                sge = Utility_Methods.FirstCharToUpper(sge.Trim());

                if (!Utility_Methods.ValidateSiegeID(sge))
                {
                    throw new InvalidDataException("Fief_Serialised siege ID must have the format 'Siege_' followed by some numbers");
                }
            }

            this.province = prov;
            this.population = pop;
            this.ancestralOwner = ancOwn;
            this.bailiff = bail;
            this.fields = fld;
            this.industry = ind;
            this.troops = trp;
            this.taxRate = tx;
            this.taxRateNext = txNxt;
            this.officialsSpendNext = offNxt;
            this.garrisonSpendNext = garrNxt;
            this.infrastructureSpendNext = infraNxt;
            this.keepSpendNext = keepNxt;
            this.keyStatsCurrent = finCurr;
            this.keyStatsPrevious = finPrev;
            this.keepLevel = kpLvl;
            this.loyalty = loy;
            this.status = stat;
            this.language = lang;
            this.terrain = terr;
            this.charactersInFief = chars;
            this.barredCharacters = barChars;
            this.barredNationalities = barNats;
            this.bailiffDaysInFief = bailInF;
            this.treasury = treas;
            this.armies = arms;
            this.hasRecruited = rec;
            this.troopTransfers = trans;
            this.isPillaged = pil;
            this.siege = sge;
        }
        /// <summary>
        /// Constructor for Fief_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Fief_Serialised()
		{
		}
	}
}
