using System;

namespace AlkoCalc
{
    /*
     * Give a man a beer, waste an hour. Teach a man to brew, and waste a lifetime!
     * ~ Bill Owen
     */
    interface CalculationI
    {
        decimal Calculate();
        string ToString();
    }
    interface GetWaterI: CalculationI
    {
        decimal GetWater();
    }
    abstract class Calculator : CalculationI
    {
        protected decimal result;
        public override string ToString()
        {
            return result.ToString();
        }
        public abstract decimal Calculate();
    }
    class SugarGravityAbv : Calculator
    {
        protected decimal startingGravity;
        protected decimal finishingGravity;
        protected const decimal MULTIPLIER = 131.25m;
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

    class AbvToGravity : SugarGravityAbv
    {
        public AbvToGravity(decimal abv, decimal fg) : base(abv, fg) {}

        public override decimal Calculate()
        {
            result = (startingGravity / MULTIPLIER) + finishingGravity;
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
            result = total / (abv / wantedAbv);
            return result;
        }

        public decimal GetWater()
        {
            return total - Calculate();
        }
    }

    class TemperatureCorrection : Calculator
    {
        // hydrometer calibration temperature in degrees F
        private readonly double CALIBRATION = celciusFarenheit(20);
        double temp;
        decimal reading;
        public TemperatureCorrection(decimal reading, decimal temp)
        {
            this.reading = reading;
            this.temp = celciusFarenheit((double)temp);
        }
        public override decimal Calculate()
        {
            decimal t1, t2, t3;
            decimal c1, c2, c3;
            decimal div1, div2;

            t1 = (decimal)(0.000134722124 * temp);
            t2 = (decimal)(0.00000204052596 * Math.Pow(temp,2));
            t3 = (decimal)(0.00000000232820948 * Math.Pow(temp, 3));

            c1 = (decimal)(0.000134722124 * CALIBRATION);
            c2 = (decimal)(0.00000204052596 * Math.Pow(CALIBRATION, 2));
            c3 = (decimal)(0.00000000232820948 * Math.Pow(CALIBRATION, 3));

            div1 = (decimal)1.00130346 - t1 + t2 - t3;
            div2 = (decimal)1.00130346 - c1 + c2 - c3;
            result = reading * (div1 / div2);
            return result;
        }
        private static double celciusFarenheit(double celcius)
        {
            return (celcius * 1.8) + 32;
        }
    }

    class BallingConverter : Calculator
    {
        decimal gravity;
        public BallingConverter(decimal gravity)
        {
            this.gravity = gravity;
        }
        public override decimal Calculate()
        {
            result = (this.gravity - 1) / (decimal)0.004;
            return result;
        }
    }

    class UnitCalc : Calculator
    {
        protected decimal abv, volume;
        public UnitCalc(decimal abv, decimal volume)
        {
            this.abv = abv;
            this.volume = volume;
        }
        public override decimal Calculate()
        {
            result = volume * abv * ((decimal) 0.000789);
            return result;
        }
    }

    class UKUnit : UnitCalc
    {
        public UKUnit(decimal abv, decimal vol) : base(abv, vol)
        {}

        public override decimal Calculate()
        {
            result = (this.abv * this.volume) / 1000;
            return result;
        }
    }

    class RatioCalculator : Calculator
    {
        decimal volume;
        decimal ratio;
        public RatioCalculator(decimal ratio, decimal volume)
        {
            this.volume = volume;
            this.ratio = ratio;
        }
        public override decimal Calculate()
        {
            result = volume / ratio;
            return result;
        }
    }

    class AlcoMass : Calculator
    {
        decimal volume, abv;
        public AlcoMass(decimal volume, decimal abv)
        {
            this.volume = volume;
            this.abv = abv;
        }

        public override decimal Calculate()
        {
            result = (volume/1000) * (abv / 100) * 789;
            return result;
        }
    }

    class BACCalculator : Calculator
    {
        decimal mass;
        decimal weight;
        decimal time;
        decimal bwr;
        readonly private decimal metabolism = 0.017m;

        public BACCalculator(decimal mass, decimal weight,
            decimal time, decimal bwr)
        {
            this.mass = mass;
            this.weight = weight;
            this.time = time;
            if (bwr == -1m)
                this.bwr = 0.68m; // man
            else if (bwr == -2m)
                this.bwr = 0.55m; // woman
            else
                this.bwr = bwr;
        }

        public override decimal Calculate()
        {
            result = ((mass / 100) / (bwr * weight)) * 100m - (metabolism * time);
            return result;
        }
    }
}

