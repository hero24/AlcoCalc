namespace AlkoCalc
{
    /*
     * Give a man a beer, waste an hour. Teach a man to brew, and waste a lifetime!
     * ~ Bill Owen
     */
    interface CalculationI
    {
        decimal Calculate();
        string GetStringResult();
    }
    interface GetWaterI: CalculationI
    {
        decimal GetWater();
    }
    abstract class Calculator : CalculationI
    {
        protected decimal result;
        public string GetStringResult()
        {
            return result.ToString();
        }
        public abstract decimal Calculate();
    }
    class SugarGravityAbv : Calculator
    {
        decimal startingGravity;
        decimal finishingGravity;
        const decimal MULTIPLIER = 131.25m;
        public SugarGravityAbv(decimal sg, decimal fg)
        {
            startingGravity = sg;
            finishingGravity = fg;
        }

        public override decimal Calculate()
        {
            result = (startingGravity - finishingGravity) * MULTIPLIER;
            return result;
        }
    }

    class Diluter : Calculator
    {
        // calculates strength
        protected decimal abv, waterAlc, alcQ;
        public Diluter(decimal abv, decimal waterAlc, decimal alcQ)
        {
            // calculate result of diluting acohol
            this.abv = abv;
            this.waterAlc = waterAlc;
            this.alcQ = alcQ;
        }

        public override decimal Calculate()
        {
            result = (alcQ / (alcQ + waterAlc) * 100) / 100 * abv;
            return result;
        }
    }

    class DiluterVolume : Diluter, GetWaterI
    {
        public DiluterVolume(decimal abv, decimal waterAlc, decimal alcQ) :
            base(abv, waterAlc, alcQ) { }

        public override decimal Calculate()
        {
            result = waterAlc * abv / alcQ;
            return result;
        }
        public decimal GetWater() 
        {
            return result - waterAlc;
        }

    }

    class Proportioner : Calculator, GetWaterI
    {
        decimal abv, wantedAbv, total;
        public Proportioner(decimal abv, decimal total, decimal wantedAbv)
        {
            this.abv = abv;
            this.total = total;
            this.wantedAbv = wantedAbv;
        }

        public override decimal Calculate()
        {
            return total / (abv / wantedAbv);
        }

        public decimal GetWater()
        {
            return total - Calculate();
        }
    }
}

