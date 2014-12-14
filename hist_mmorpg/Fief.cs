﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on fief
    /// </summary>
    public class Fief : Place
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
        public int treasury { get; set; }
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
        public Dictionary<string, string[]> troopTransfers = new Dictionary<string, string[]>();
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public String siege { get; set; }

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
        /// <param name="trans">Dictionary<string, string[]> containing troop detachments in the fief awaiting transfer</param>
        /// <param name="sge">String holding siegeID of active siege</param>
        public Fief(String id, String nam, Province prov, int pop, Double fld, Double ind, uint trp, Double tx,
            Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, double[] finCurr, double[] finPrev,
            Double kpLvl, Double loy, char stat, Language lang, Terrain terr, List<Character> chars, List<string> barChars, List<string> barNats,
            byte bailInF, int treas, List<string> arms, bool rec, Dictionary<string, string[]> trans, bool pil, String tiHo = null,
            PlayerCharacter own = null, PlayerCharacter ancOwn = null, Character bail = null, Rank r = null, string sge = null)
            : base(id, nam, own: own, r: r, tiHo: tiHo)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/01-19

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Fief name must be between 1 and 40 characters in length");
            }

            // TODO: validate prov ID = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate pop = 1-2000000
            if ((pop < 1) || (pop > 2000000))
            {
                throw new InvalidDataException("Fief population must be an integer between 1 and 2000000");
            }

            // TODO: validate own ID = 1-10000?
            // TODO: validate ancOwn ID = 1-10000?
            // TODO: validate bail ID = 1-10000?

            // validate fld >= 0
            if (fld < 0)
            {
                throw new InvalidDataException("Fief field level must be a double >= 0");
            }

            // validate ind >= 0
            if (ind < 0)
            {
                throw new InvalidDataException("Fief industry level must be a double >= 0");
            }

            // TODO: validate trp = (upper limit?)
            // validate tx = 0-100.00
            if ((tx < 0) || (tx > 100))
            {
                throw new InvalidDataException("Fief tax rate must be a double between 0 and 100");
            }

            // TODO: validate off = (upper limit?)
            // TODO: validate garr = (upper limit?)
            // TODO: validate infra = (upper limit?)
            // TODO: validate keep = (upper limit?)
            // validate tx = 0-100.00
            if ((txNxt < 0) || (txNxt > 100))
            {
                throw new InvalidDataException("Fief tax rate (next season) must be a double between 0 and 100");
            }

            // TODO: validate offNxt = (upper limit?)
            // TODO: validate garrNxt = (upper limit?)
            // TODO: validate infraNxt = (upper limit?)
            // TODO: validate keepNxt = (upper limit?)

            // validate keepLvl >= 0
            if (kpLvl < 0)
            {
                throw new InvalidDataException("Fief keep level must be a double >= 0");
            }

            // validate loy = 0-9.00
            if ((loy < 0) || (loy > 9))
            {
                throw new InvalidDataException("Fief loyalty must be a double between 0 and 9");
            }

            // validate stat = C/U/R
            if (((!stat.Equals('C')) && (!stat.Equals('U'))) && (!stat.Equals('R')))
            {
                throw new InvalidDataException("Fief status must be 'C', 'U' or 'R'");
            }

            /*
            this.id = id;
            this.name = nam;
            this.owner = own;
            this.rank = r;
            this.titleHolder = tiHo; */
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
		/// Constructor for Fief using Fief_Riak object.
        /// For use when de-serialising from Riak
        /// </summary>
		/// <param name="fr">Fief_Riak object to use as source</param>
		public Fief(Fief_Riak fr)
			: base(fr: fr)
		{
		
            // province to be added later
			this.province = null;
			this.population = fr.population;
            // ancestral owner to be added later
            this.ancestralOwner = null;
            // bailiff to be added later
            this.bailiff = null;
			this.fields = fr.fields;
			this.industry = fr.industry;
			this.troops = fr.troops;
			this.taxRate = fr.taxRate;
			this.taxRateNext = fr.taxRateNext;
			this.officialsSpendNext = fr.officialsSpendNext;
			this.garrisonSpendNext = fr.garrisonSpendNext;
			this.infrastructureSpendNext = fr.infrastructureSpendNext;
			this.keepSpendNext = fr.keepSpendNext;
            this.keyStatsCurrent = fr.keyStatsCurrent;
            this.keyStatsPrevious = fr.keyStatsPrevious;
			this.keepLevel = fr.keepLevel;
			this.loyalty = fr.loyalty;
			this.status = fr.status;
            this.language = null;
			this.terrain = null;
            // create empty list to be populated later
			this.charactersInFief = new List<Character>();
			this.barredCharacters = fr.barredCharacters;
			this.barredNationalities = fr.barredNationalities;
            this.bailiffDaysInFief = fr.bailiffDaysInFief;
            this.treasury = fr.treasury;
            this.armies = fr.armies;
            this.hasRecruited = fr.hasRecruited;
            this.troopTransfers = fr.troopTransfers;
            this.isPillaged = fr.isPillaged;
            this.siege = fr.siege;
        }

        /// <summary>
        /// Constructor for Fief taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Fief()
		{
		}

        /// <summary>
        /// Calculates fief GDP
        /// </summary>
        /// <returns>uint containing fief GDP</returns>
        public uint calcNewGDP()
        {
            uint gdp = 0;
            uint fldProd = 0;
            uint indProd = 0;

            // calculate production from fields using next season's expenditure
            fldProd = Convert.ToUInt32((this.calcNewFieldLevel() * 8997));

            // calculate production from industry using next season's expenditure
            indProd = Convert.ToUInt32(this.calcNewIndustryLevel() * (290 * Math.Pow(1.2, ((this.calcNewPop() / 1000) - 1))));
            
            // calculate final gdp
            gdp = (fldProd + indProd) / (this.calcNewPop() / 1000);

            gdp = (gdp * 1000);

            return gdp;
        }

        /// <summary>
        /// Calculates fief population increase
        /// </summary>
        /// <returns>uint containing new fief population</returns>
        public uint calcNewPop()
        {
            uint newPop = Convert.ToUInt32(this.population + (this.population * 0.035));
            return newPop;
        }

        /// <summary>
        /// Calculates fief income (NOT including income loss due to unrest/rebellion)
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int calcNewIncome()
        {
            int fiefBaseIncome = 0;
            int fiefIncome = 0;

            // check for siege
            if (this.siege == null)
            {
                // no siege = use next season's expenditure and tax rate
                fiefBaseIncome = Convert.ToInt32(this.calcNewGDP() * (this.taxRateNext / 100));
            }
            else
            {
                // siege = use next season's expenditure and 0% tax rate = no income
                fiefBaseIncome = Convert.ToInt32(this.calcNewGDP() * 0);
            }

            fiefIncome = fiefBaseIncome;

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.calcBlfIncMod(this.bailiffDaysInFief >= 30));

            // factor in officials spend modifier
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.calcOffIncMod());

            return fiefIncome;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double calcBlfIncMod(bool daysInFiefOK)
        {
            double incomeModif = 0;

            // check if auto-bailiff
            if ((this.bailiff == null) || (! daysInFiefOK))
            {
                // modifer = 0.025 per management level above 1
                // if auto-baliff set modifier at equivalent of management rating of 3
                incomeModif = 0.05;
            }
            else
            {
                incomeModif = this.bailiff.calcFiefIncMod();
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcOffIncMod()
        {
            double incomeModif = 0;

            // using next season's expenditure and population
            incomeModif = ((this.officialsSpendNext - ((double)this.calcNewPop() * 2)) / (this.calcNewPop() * 2)) / 10;

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of unrest/rebellion on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcStatusIncmMod()
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
        public uint calcNewOlordTaxes()
        {
            // calculate tax, based on income of specified season
            uint oTaxes = Convert.ToUInt32(this.calcNewIncome() * (this.province.taxRate / 100));
            return oTaxes;
        }

        /// <summary>
        /// Calculates fief expenses
        /// </summary>
        /// <returns>int containing family expenses</returns>
        public int calcNewExpenses()
        {
            int fiefExpenses = 0;

            // using next season's expenditure
            fiefExpenses = (int)this.officialsSpendNext + (int)this.infrastructureSpendNext + (int)this.garrisonSpendNext + (int)this.keepSpendNext;

            // factor in bailiff skills modifier for fief expenses
            double bailiffModif = 0;

            // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
            bailiffModif = this.calcBailExpModif(this.bailiffDaysInFief >= 30);

            if (bailiffModif != 0 )
            {
                fiefExpenses = fiefExpenses + Convert.ToInt32(fiefExpenses * bailiffModif);
            }

            return fiefExpenses;
        }

        /// <summary>
        /// Calculates effect of bailiff skills on fief expenses
        /// </summary>
        /// <returns>double containing fief expenses modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double calcBailExpModif(bool daysInFiefOK)
        {
            double expSkillsModifier = 0;

            if ((this.bailiff != null) && (daysInFiefOK))
            {
                expSkillsModifier = this.bailiff.calcSkillEffect("fiefExpense");
            }

            return expSkillsModifier;
        }
        
        /// <summary>
        /// Calculates fief bottom line
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int calcNewBottomLine()
        {
            int fiefBottomLine = 0;

            // factor in effect of fief status on specified season's income
            int fiefIncome = Convert.ToInt32(this.calcNewIncome() * this.calcStatusIncmMod());

            // calculate bottom line using specified season's income, and expenses
            fiefBottomLine = fiefIncome - (int)this.calcNewExpenses() - this.calcFamilyExpenses();

            // check for occupation before deducting overlord taxes
            if (!this.checkEnemyOccupation())
            {
                fiefBottomLine -= (int)this.calcNewOlordTaxes();
            }

            return fiefBottomLine;
        }

        /// <summary>
        /// Calculates family expenses
        /// </summary>
        /// <returns>int containing family expenses</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int calcFamilyExpenses()
        {
            int famExpenses = 0;

            // for all fiefs, get bailiff wages
            if (this.bailiff != null)
            {
                if (this.bailiff != this.owner)
                {
                    famExpenses += Convert.ToInt32((this.bailiff as NonPlayerCharacter).salary);
                }
            }

            // if home fief, also get all non-bailiff expenses
            // (i.e. family allowances, other employees' wages)
            if (this == this.owner.getHomeFief())
            {
                foreach (NonPlayerCharacter element in this.owner.myNPCs)
                {
                    if (!((element.getFunction(this.owner).ToLower()).Contains("bailiff")))
                    {
                        // add wage of non-bailiff employees
                        if (element.familyID == null)
                        {
                            famExpenses += Convert.ToInt32(element.salary);
                        }
                        // add family allowance of family NPCs
                        else
                        {
                            // check for siege
                            if (this.siege == null)
                            {
                                // no siege = normal allowance
                                famExpenses += Convert.ToInt32(element.calcFamilyAllowance(element.getFunction(this.owner)));
                            }
                            else
                            {
                                // siege = half allowance
                                famExpenses += (Convert.ToInt32(element.calcFamilyAllowance(element.getFunction(this.owner))) / 2);
                            }
                        }
                    }
                }

                // factor in skills of player or spouse (highest management rating)
                double famSkillsModifier = 0;

                // get famExpense rating of whoever has highest management rating
                if ((this.owner.spouse != null) && (this.owner.management < this.owner.getSpouse().management))
                {
                    famSkillsModifier = this.owner.getSpouse().calcSkillEffect("famExpense");
                }
                else
                {
                    famSkillsModifier = this.owner.calcSkillEffect("famExpense");
                }

                // apply to family expenses
                if (famSkillsModifier != 0)
                {
                    famExpenses = famExpenses + Convert.ToInt32(famExpenses * famSkillsModifier);
                }
            }

            return famExpenses;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void adjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum tax rate is 100%.  Rate adjusted.");
                }
            }
            else if (tx < 0)
            {
                tx = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum tax rate is 0%.  Rate adjusted.");
                }
            }

            this.taxRateNext = tx;
        }

        /// <summary>
        /// Gtes the maximum permitted expenditure for the fief of the specified type
        /// </summary>
        /// <returns>uint containing maximum permitted expenditure</returns>
        /// <param name="type">string containing type of expenditure</param>
        public uint getMaxSpend(string type)
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

        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</param>
        public void adjustOfficialsSpend(uint os)
        {
            // ensure min 0
            if (os < 0)
            {
                os = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum officials expenditure is 0.  Amount adjusted.");
                }
            }

            // ensure doesn't exceed max permitted (4 per head of population)
            uint maxSpend = this.getMaxSpend("officials");
            if (os > maxSpend)
            {
                os = maxSpend;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum officials expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
                }
            }

            this.officialsSpendNext = os;
        }

        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</param>
        public void adjustInfraSpend(uint infs)
        {
            // ensure min 0
            if (infs < 0)
            {
                infs = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum infrastructure expenditure is 0.  Amount adjusted.");
                }
            }

            // ensure doesn't exceed max permitted (6 per head of population)
            uint maxSpend = this.getMaxSpend("infrastructure");
            if (infs > maxSpend)
            {
                infs = maxSpend;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum infrastructure expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
                }
            }

            this.infrastructureSpendNext = infs;
        }

        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</param>
        public void adjustGarrisonSpend(uint gs)
        {
            // ensure min 0
            if (gs < 0)
            {
                gs = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum garrison expenditure is 0.  Amount adjusted.");
                }
            }

            // ensure doesn't exceed max permitted (14 per head of population)
            uint maxSpend = this.getMaxSpend("garrison");
            if (gs > maxSpend)
            {
                gs = maxSpend;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum garrison expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
                }
            }

            this.garrisonSpendNext = gs;
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</param>
        public void adjustKeepSpend(uint ks)
        {
            // ensure min 0
            if (ks < 0)
            {
                ks = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum keep expenditure is 0.  Amount adjusted.");
                }
            }

            // ensure doesn't exceed max permitted (13 per head of population)
            uint maxSpend = this.getMaxSpend("keep");
            if (ks > maxSpend)
            {
                ks = maxSpend;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum keep expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
                }
            }

            this.keepSpendNext = ks;
        }

        /// <summary>
        /// Calculates new fief field level (from next season's spend)
        /// </summary>
        /// <returns>double containing new field level</returns>
        public double calcNewFieldLevel()
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

            return fldLvl;
        }

        /// <summary>
        /// Calculates new fief industry level (from next season's spend)
        /// </summary>
        /// <returns>double containing new industry level</returns>
        public double calcNewIndustryLevel()
        {
            double indLvl = 0;

            // if no expenditure, industry level reduced by 1%
            if (this.infrastructureSpendNext == 0)
            {
                indLvl = this.industry - (this.industry / 100);
            }
            // industry level increases by 0.1 per 100k spent
            else
            {
                indLvl = this.industry + (this.infrastructureSpendNext / 1000000.00);
            }
            return indLvl;
        }

        /// <summary>
        /// Calculates new fief keep level (from next season's spend)
        /// </summary>
        /// <returns>double containing new keep level</returns>
        public double calcNewKeepLevel()
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

            return kpLvl;
        }

        /// <summary>
        /// Calculates new fief loyalty level (i.e. for next season)
        /// </summary>
        /// <returns>double containing new fief loyalty</returns>
        public double calcNewLoyalty()
        {
            double newBaseLoy = 0;
            double newLoy = 0;

            // calculate effect of tax rate change = loyalty % change is direct inverse of tax % change
            newBaseLoy = this.loyalty + (this.loyalty * (((this.taxRateNext - this.taxRate) / 100) * -1));

            // calculate effect of surplus 
            if (this.calcNewBottomLine() > 0)
            {
                // loyalty reduced in proportion to surplus divided by income
                newLoy = newBaseLoy - (this.calcNewBottomLine() / Convert.ToDouble(this.calcNewIncome()));
            }
            else
            {
                newLoy = newBaseLoy;
            }

            // calculate effect of officials spend
            newLoy = newLoy + (newBaseLoy * this.calcOffLoyMod());

            // calculate effect of garrison spend
            newLoy = newLoy + (newBaseLoy * this.calcGarrLoyMod());

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            newLoy = newLoy + (newBaseLoy * this.calcBlfLoyAdjusted(this.bailiffDaysInFief >= 30));

            return newLoy;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief loyalty level
        /// Also includes effect of skills
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double calcBlfLoyAdjusted(bool daysInFiefOK)
        {
            double loyModif = 0;
            double loySkillModif = 0;

            // get bailiff stats (set to auto-bailiff values by default)
            double bailStature = 3;
            double bailMgmt = 3;
            Language bailLang = this.language;

            // if not auto-bailiff and if has served appropriate days in fief
            if ((this.bailiff != null) && (daysInFiefOK))
            {
                bailStature = this.bailiff.calculateStature();
                bailMgmt = this.bailiff.management;
                bailLang = this.bailiff.language;
            }

            // get base bailiff loyalty modifier
            loyModif = this.calcBaseFiefLoyMod(bailStature, bailMgmt, bailLang);

            // check for skill modifier, passing in daysInFief
            loySkillModif = this.calcBailLoySkillMod(daysInFiefOK);

            loyModif = loyModif + (loyModif * loySkillModif);

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
        public double calcBaseFiefLoyMod(double stature, double mngt, Language lang)
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
        /// Calculates bailiff's skill modifier for fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double calcBailLoySkillMod(bool daysInFiefOK)
        {
            double loySkillsModifier = 0;

            if ((this.bailiff != null) && (daysInFiefOK))
            {
                loySkillsModifier = this.bailiff.calcSkillEffect("fiefLoy");
            }

            return loySkillsModifier;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcOffLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's officials spend and population
            loyaltyModif = ((this.officialsSpendNext - ((double)this.calcNewPop() * 2)) / (this.calcNewPop() * 2)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Calculates effect of garrison spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcGarrLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's garrison spend and population
            loyaltyModif = ((this.garrisonSpendNext - ((double)this.calcNewPop() * 7)) / (this.calcNewPop() * 7)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Validates proposed expenditure levels, auto-adjusting where necessary
        /// </summary>
        public void validateFiefExpenditure()
        {
            // get total spend
            uint totalSpend = Convert.ToUInt32(this.calcNewExpenses()); ;

            // check to see if proposed expenditure level doesn't exceed fief treasury
            bool isOK = this.checkExpenditureOK(totalSpend);

            // if expenditure does exceed fief treasury
            if (!isOK)
            {
                // get available treasury
                int availTreasury = this.getAvailableTreasury();
                // calculate amount by which treasury exceeded
                uint difference = Convert.ToUInt32(totalSpend - availTreasury);
                // auto-adjust expenditure
                this.autoAdjustExpenditure(difference);
            }
        }

        /// <summary>
        /// Automatically adjusts expenditure at end of season, if exceeds treasury
        /// </summary>
        /// <param name="difference">The amount by which expenditure exceeds treasury</param>
        public void autoAdjustExpenditure(uint difference)
        {
            // get available treasury
            int availTreasury = this.getAvailableTreasury();

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
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\noffSpend: " + this.officialsSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierOff: " + reduceByModifierOff + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }

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
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ngarrSpend: " + this.garrisonSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierGarr: " + reduceByModifierGarr + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }

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
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ninfSpend: " + this.infrastructureSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierInf: " + reduceByModifierInf + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }

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
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\nkeepSpend: " + this.keepSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierKeep: " + reduceByModifierKeep + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }

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
        public int getAvailableTreasury(bool deductFiefExpense = false)
        {
            int amountAvail = 0;

            // get treasury
            amountAvail = this.treasury;
            // deduct family expenses
            amountAvail -= this.calcFamilyExpenses();
            // deduct overlord taxes
            amountAvail -= Convert.ToInt32(this.calcNewOlordTaxes());

            if (deductFiefExpense)
            {
                // deduct fief expenditure
                amountAvail -= this.calcNewExpenses();
            }

            return amountAvail;
        }
        
        /// <summary>
        /// Compares expenditure level with fief treasury
        /// </summary>
        /// <returns>bool indicating whether expenditure acceptable</returns>
        /// <param name="totalSpend">proposed total expenditure for next season</param>
        public bool checkExpenditureOK(uint totalSpend)
        {
            bool spendLevelOK = true;

            // get available treasury
            int availTreasury = this.getAvailableTreasury();

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
        public void updateFief()
        {
            // update bailiffDaysInFief if appropriate
            if ((this.bailiff != null) && (this.bailiff.days > 0))
            {
                this.bailiff.useUpDays();
            }

            // update previous season's financial data
            this.keyStatsCurrent.CopyTo(this.keyStatsPrevious, 0);

            // validate fief expenditure against treasury
            this.validateFiefExpenditure();

            // update loyalty level
            this.keyStatsCurrent[0] = this.calcNewLoyalty();

            // update GDP
            this.keyStatsCurrent[1] = this.calcNewGDP();

            // update tax rate
            this.keyStatsCurrent[2] = this.taxRateNext;

            // update officials spend (updating total expenses)
            this.keyStatsCurrent[3] = this.officialsSpendNext;
            this.keyStatsCurrent[10] = 0;
            this.keyStatsCurrent[10] += this.keyStatsCurrent[3];

            // update garrison spend (updating total expenses)
            this.keyStatsCurrent[4] = this.garrisonSpendNext;
            this.keyStatsCurrent[10] += this.keyStatsCurrent[4];

            // update infrastructure spend (updating total expenses)
            this.keyStatsCurrent[5] = this.infrastructureSpendNext;
            this.keyStatsCurrent[10] += this.keyStatsCurrent[5];

            // update keep spend (updating total expenses)
            this.keyStatsCurrent[6] = this.keepSpendNext;
            this.keyStatsCurrent[10] += this.keyStatsCurrent[6];

            // update keep level (based on next season keep spend)
            this.keyStatsCurrent[7] = this.calcNewKeepLevel();

            // update income
            this.keyStatsCurrent[8] = this.calcNewIncome();

            // update family expenses (updating total expenses)
            this.keyStatsCurrent[9] = this.calcFamilyExpenses();
            this.keyStatsCurrent[10] += this.keyStatsCurrent[9];

            // update overord taxes
            this.keyStatsCurrent[11] = this.calcNewOlordTaxes();

            // update overord tax rate
            this.keyStatsCurrent[12] = this.province.taxRate;

            // update bottom line
            this.keyStatsCurrent[13] = this.calcNewBottomLine();

            // update fields level (based on next season infrastructure spend)
            this.fields = this.calcNewFieldLevel();

            // update industry level (based on next season infrastructure spend)
            this.industry = this.calcNewIndustryLevel();

            // update fief treasury with new bottom line
            this.treasury += Convert.ToInt32(this.keyStatsCurrent[13]);

            // check for occupation before transfering overlord taxes into overlord's treasury
            if (!this.checkEnemyOccupation())
            {
                // get overlord
                PlayerCharacter thisOverlord = this.getOverlord();
                if (thisOverlord != null)
                {
                    // get overlord's home fief
                    Fief overlordHome = thisOverlord.getHomeFief();

                    if (overlordHome != null)
                    {
                        // pay in taxes
                        overlordHome.treasury += Convert.ToInt32(this.calcNewOlordTaxes());
                    }
                }
            }

            // check for unrest/rebellion
            this.status = this.checkFiefStatus();

            // reset bailiffDaysInFief
            this.bailiffDaysInFief = 0;

            // reset hasRecruited
            this.hasRecruited = false;

            // reset isPillaged
            this.isPillaged = false;
        }

        /// <summary>
        /// Checks for transition from calm to unrest/rebellion, or from unrest to calm
        /// </summary>
        /// <returns>char indicating fief status</returns>
        public char checkFiefStatus()
        {
            char originalStatus = this.status;
 
            // if fief in rebellion it can only be recovered by combat or bribe,
            // so don't run check
            if (!this.status.Equals('R'))
            {
                // method 1 (depends on tax rate and surplus)
                if ((this.taxRate > 20) && (this.keyStatsCurrent[13] > (this.keyStatsCurrent[8] * 0.1)))
                {
                    if (Globals_Game.GetRandomDouble(100) <= (this.taxRate - 20))
                    {
                        this.status = 'R';
                    }
                }

                // method 2 (depends on fief loyalty level)
                if (!this.status.Equals('R'))
                {
                    double chance = Globals_Game.GetRandomDouble(100);
                    
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
                    this.owner.adjustStatureModifier(-0.1);
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
                uint entryID = Globals_Game.getNextJournalEntryID();

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

                // description
                string description = "On this day of Our Lord the status of the fief of " + this.name;
                description += ", owned by " + fiefOwner.firstName + " " + fiefOwner.familyName + ",";
                description += " changed from " + oldStatus + " to " + newStatus + ".";

                // create and add a journal entry to the pastEvents journal
                JournalEntry newEntry = new JournalEntry(entryID, year, season, thisPersonae, type, loc: location, descr: description);
                success = Globals_Game.addPastEvent(newEntry);
            }

            return this.status;
        }

        /// <summary>
        /// Adds character to characters list
        /// </summary>
        /// <param name="ch">Character to be inserted into characters list</param>
        internal void addCharacter(Character ch)
        {
            // add character
            this.charactersInFief.Add(ch);
        }

        /// <summary>
        /// Removes character from characters list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character to be removed from characters list</param>
        internal bool removeCharacter(Character ch)
        {
            // remove character
            bool success = this.charactersInFief.Remove(ch);

            return success;
        }

        /// <summary>
        /// Adds army to armies list
        /// </summary>
        /// <param name="armyID">ID of army to be inserted</param>
        internal void addArmy(String armyID)
        {
            // add army
            this.armies.Add(armyID);
        }

        /// <summary>
        /// Removes army from armies list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="armyID">ID of army to be removed</param>
        internal bool removeArmy(String armyID)
        {
            // remove army
            bool success = this.armies.Remove(armyID);

            return success;
        }

        /// <summary>
        /// Bar a specific character from the fief's keep
        /// </summary>
        /// <param name="ch">Character to be barred</param>
        internal void barCharacter(string ch)
        {
            // bar character
            this.barredCharacters.Add(ch);
        }

        /// <summary>
        /// Removes a fief keep bar from a specific character
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character for whom bar to be removed</param>
        internal bool removeBarCharacter(string ch)
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
        public int callUpTroops(double minProportion = 0, double maxProportion = 1)
        {
            int numberTroops = 0;
            int maxNumber = this.calcMaxTroops();

            // generate random double between min and max
            double myRandomDouble = Globals_Game.GetRandomDouble(min: minProportion, max: maxProportion);

            // apply random double as modifier to maxNumber
            numberTroops = Convert.ToInt32(maxNumber * myRandomDouble);

            return numberTroops;
        }

        /// <summary>
        /// Calculates the maximum number of troops available for call up in the fief
        /// </summary>
        /// <returns>int containing maximum number of troops</returns>
        public int calcMaxTroops()
        {
            return Convert.ToInt32(this.population * 0.05);
        }

        /// <summary>
        /// Calculates the garrison size for the fief
        /// </summary>
        /// <returns>int containing the garrison size</returns>
        public int getGarrisonSize()
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
        public PlayerCharacter getOverlord()
        {
            PlayerCharacter myOverlord = null;

            if (this.province.titleHolder != null)
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
        public Siege getSiege()
        {
            Siege mySiege = null;

            if (this.siege != null)
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
        public PlayerCharacter getRightfulKing()
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
        public PlayerCharacter getCurrentKing()
        {
            PlayerCharacter thisKing = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.owner.nationality)
                {
                    if (kingdomEntry.Value.owner != null)
                    {
                        thisKing = kingdomEntry.Value.owner;
                    }
                }
            }

            return thisKing;
        }

        /// <summary>
        /// Checks if the fief is under enemy occupation
        /// </summary>
        /// <returns>bool indicating whether is under enemy occupation</returns>
        public bool checkEnemyOccupation()
        {
            bool isOccupied = false;

            if (this.getRightfulKing() != this.getCurrentKing())
            {
                isOccupied = true;
            }

            return isOccupied;
        }

        /// <summary>
        /// Gets the fief's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom getRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (this.province.getRightfulKingdom() != null)
            {
                thisKingdom = this.province.getRightfulKingdom();
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the fief's current kingdom (i.e. the kingdom of the current owner)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom getCurrentKingdom()
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

        /// <summary>
        /// Processes the functions involved in a change of fief ownership
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newOwner">The new owner</param>
        /// <param name="circumstance">The circumstance under which the change of ownership is taking place</param>
        public bool changeOwnership(PlayerCharacter newOwner, string circumstance = "hostile")
        {
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
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You cannot give away your home fief.");
                    }
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
                            candidateFiefs = oldOwner.getHighestRankingFief();
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
                            Fief newHomeFief = oldOwner.getHomeFief();
                            if (!newHomeFief.titleHolder.Equals(oldOwner.charID))
                            {
                                // remove title from existing holder and assign to old owner
                                oldOwner.transferTitle(oldOwner, newHomeFief);
                            }
                        }

                        else
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show(oldOwner.firstName + " " + oldOwner.familyName + " must select a new home fief.");
                            }
                        }
                    }

                    // old owner has no more fiefs
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(oldOwner.firstName + " " + oldOwner.familyName + " doesn't own any fiefs!  Defeat?");
                        }
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
                oldOwner.transferTitle(newOwner, this);

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
                this.status = this.checkFiefStatus();

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

                // CREATE JOURNAL ENTRY
                bool entryAdded = true;

                // ID
                uint entryID = Globals_Game.getNextJournalEntryID();

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
                string oldOwnerTitle = "";
                string description = "On this day of Our Lord the ownership of the fief of " + this.name;
                if (circumstance.Equals("hostile"))
                {
                    description += " has passed from";
                }
                else
                {
                    description += " was granted by";
                    oldOwnerTitle = "His Majesty ";
                }
                description += " the previous owner, ";
				description += oldOwnerTitle + oldOwner.firstName + " " + oldOwner.familyName + ", to the NEW OWNER, ";
                description += newOwner.firstName + " " + newOwner.familyName + ".";

                // create and add a journal entry to the pastEvents journal
                JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type, loc: location, descr: description);
                entryAdded = Globals_Game.addPastEvent(thisEntry);
            }

            return success;
        }

        /// <summary>
        /// Checks to see if there is currently a field army in the fief keep
        /// </summary>
        /// <returns>bool indicating presence of a field army</returns>
        public bool checkFieldArmyInKeep()
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
    }

	/// <summary>
	/// Class used to convert Fief to/from format suitable for Riak (JSON)
	/// </summary>
	public class Fief_Riak : Place_Riak
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
        public Dictionary<string, string[]> troopTransfers = new Dictionary<string, string[]>();
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public String siege { get; set; }

		/// <summary>
		/// Constructor for Fief_Riak
		/// </summary>
		/// <param name="f">Fief object to use as source</param>
		public Fief_Riak(Fief f)
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
			this.terrain = f.terrain.terrainCode;
			if (f.charactersInFief.Count > 0)
			{
				for (int i = 0; i < f.charactersInFief.Count; i++)
				{
					this.charactersInFief.Add (f.charactersInFief[i].charID);
				}
			}
			this.barredCharacters = f.barredCharacters;
			this.barredNationalities = f.barredNationalities;
            this.bailiffDaysInFief = f.bailiffDaysInFief;
            this.treasury = f.treasury;
            this.armies = f.armies;
            this.hasRecruited = f.hasRecruited;
            this.troopTransfers = f.troopTransfers;
            this.isPillaged = f.isPillaged;
            this.siege = f.siege;
		}

        /// <summary>
        /// Constructor for Fief_Riak taking seperate values.
        /// For creating Fief_Riak from CSV file.
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
        /// <param name="bailInF">byte holding days bailiff in fief</param>
        /// <param name="treas">int containing fief treasury</param>
        /// <param name="arms">List holding IDs of armies present in fief</param>
        /// <param name="rec">bool indicating whether recruitment has occurred in the fief (current season)</param>
        /// <param name="pil">bool indicating whether pillage has occurred in the fief (current season)</param>
        /// <param name="trans">Dictionary<string, string[]> containing troop detachments in the fief awaiting transfer</param>
        /// <param name="sge">String holding siegeID of active siege</param>
        public Fief_Riak(String id, String nam, string prov, int pop, Double fld, Double ind, uint trp, Double tx,
            Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, double[] finCurr, double[] finPrev,
            Double kpLvl, Double loy, char stat, string lang, string terr, List<string> chars, List<string> barChars, List<string> barNats,
            byte bailInF, int treas, List<string> arms, bool rec, Dictionary<string, string[]> trans, bool pil, byte r, String tiHo = null,
            string own = null, string ancOwn = null, string bail = null, string sge = null)
            : base(id, nam, own: own, r: r, tiHo: tiHo)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/01-19

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Fief name must be between 1 and 40 characters in length");
            }

            // TODO: validate prov ID = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate pop = 1-2000000
            if ((pop < 1) || (pop > 2000000))
            {
                throw new InvalidDataException("Fief population must be an integer between 1 and 2000000");
            }

            // TODO: validate own ID = 1-10000?
            // TODO: validate ancOwn ID = 1-10000?
            // TODO: validate bail ID = 1-10000?

            // validate fld >= 0
            if (fld < 0)
            {
                throw new InvalidDataException("Fief field level must be a double >= 0");
            }

            // validate ind >= 0
            if (ind < 0)
            {
                throw new InvalidDataException("Fief industry level must be a double >= 0");
            }

            // TODO: validate trp = (upper limit?)
            // validate tx = 0-100.00
            if ((tx < 0) || (tx > 100))
            {
                throw new InvalidDataException("Fief tax rate must be a double between 0 and 100");
            }

            // TODO: validate off = (upper limit?)
            // TODO: validate garr = (upper limit?)
            // TODO: validate infra = (upper limit?)
            // TODO: validate keep = (upper limit?)
            // validate tx = 0-100.00
            if ((txNxt < 0) || (txNxt > 100))
            {
                throw new InvalidDataException("Fief tax rate (next season) must be a double between 0 and 100");
            }

            // TODO: validate offNxt = (upper limit?)
            // TODO: validate garrNxt = (upper limit?)
            // TODO: validate infraNxt = (upper limit?)
            // TODO: validate keepNxt = (upper limit?)

            // validate keepLvl >= 0
            if (kpLvl < 0)
            {
                throw new InvalidDataException("Fief keep level must be a double >= 0");
            }

            // validate loy = 0-9.00
            if ((loy < 0) || (loy > 9))
            {
                throw new InvalidDataException("Fief loyalty must be a double between 0 and 9");
            }

            // validate stat = C/U/R
            if (((!stat.Equals('C')) && (!stat.Equals('U'))) && (!stat.Equals('R')))
            {
                throw new InvalidDataException("Fief status must be 'C', 'U' or 'R'");
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
        /// Constructor for Fief_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Fief_Riak()
		{
		}
	}
}
