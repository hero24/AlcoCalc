﻿using System.Windows.Forms;

namespace AlkoCalc
{
    /*
     * Wine is constant proof that God loves us and loves to see us happy.
     * ~ Benjamin Franklin
     */
    public partial class GUI : Form
    {
        private const string PG_LABEL ="Previous graivty";
        private const string NW_LABEL = "New gravity";
        private const string ABV_VAL = "Drink ABV";
        private const string DRNKVOL = "Drink volume";
        private enum FieldsIndexes
        {
            OGS,
            FGS,
            DRINKS, 
            DRABV
        }
        TextBox[][] fields = new TextBox[(int) FieldsIndexes.DRABV+1][];
        public GUI()
        {
            InitializeComponent();
        }

        private decimal getNumberFromField(TextBox box)
        {
            try
            {
                return decimal.Parse(box.Text);
            } catch (System.FormatException)
            {
                MessageBox.Show("Please input a valid number");
            }
            catch (System.ArgumentNullException)
            {
                MessageBox.Show("Please input a valid number");
            }
            return 1;
        }

        private int getIntrFromField(TextBox box)
        {
            try
            {
                return int.Parse(box.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Please input a valid number");
            }
            catch (System.ArgumentNullException)
            {
                MessageBox.Show("Please input a valid number");
            }
            return 1;
        }
        private void doCalculation(CalculationI calculator, TextBox resultbox)
        {
            calculator.Calculate();
            resultbox.Text = calculator.GetStringResult();
        }
        private void calculateResult_Click(object sender, System.EventArgs e)
        {
            doCalculation(new Diluter(getNumberFromField(this.alcoholStrength),
                 getNumberFromField(this.waterQuantity),
                 getNumberFromField(this.alcoholQuantity)), dilutionResult);
        }

        private void calculateWater(GetWaterI waterCalc, TextBox waterBottle)
        {
            waterBottle.Text = waterCalc.GetWater().ToString();
        }

        private void calculateOutcome_Click(object sender, System.EventArgs e)
        {
            DiluterVolume diluter = new DiluterVolume(getNumberFromField(this.alcoholStrengthDO),
                getNumberFromField(this.alcoholVolumeDO), getNumberFromField(this.abvWantedDO));
            doCalculation(diluter, totalDO);
            calculateWater(diluter, this.waterDO);
        }

        private void calculageDilutionP_Click(object sender, System.EventArgs e)
        {
            var prop = new Proportioner(getNumberFromField(this.alcoholStrengthP),
                getNumberFromField(this.totalQuantityP), getNumberFromField(this.wantedABVP));
            doCalculation(prop, alcoholResultP);
            calculateWater(prop, waterResultP);
        }

        private void calculateABV_Click(object sender, System.EventArgs e)
        {
            doCalculation(new SugarGravityAbv(getNumberFromField(abvSG),
                getNumberFromField(abvFG)), abvResult);
        }

        private void addFieldsMatrix(int mtx_index, int snd_index,
            int noOfAdditions, TableLayoutPanel tlp, string label1, string label2)
        {
            if (fields[mtx_index] is null)
            {
                fields[mtx_index] = new TextBox[noOfAdditions];
                fields[snd_index] = new TextBox[noOfAdditions];
                for (int i = 0; i < noOfAdditions; i++)
                {
                    addFields(fields[mtx_index], fields[snd_index],
                        i, tlp, label1, label2);
                }
            }
            else if (noOfAdditions > fields[mtx_index].Length)
            {
                var resizeFlds = new TextBox[noOfAdditions];
                var resizeFlds_2 = new TextBox[noOfAdditions];
                int i = 0;
                for (; i < fields[mtx_index].Length; i++)
                {
                    resizeFlds[i] = fields[mtx_index][i];
                    resizeFlds_2[i] = fields[snd_index][i];
                }
                for (; i < noOfAdditions; i++)
                {
                    addFields(resizeFlds, resizeFlds_2,
                        i, tlp, label1, label2);
                }
                fields[mtx_index] = resizeFlds;
                fields[snd_index] = resizeFlds_2;
            }
        }
        private void addFields(TextBox[] ogs, TextBox[] fgs,
            int i, TableLayoutPanel tlp, string label1, string label2)
        {
            var lab = new Label();
            lab.Text = label1;
            tlp.Controls.Add(lab);
            ogs[i] = new TextBox();
            tlp.Controls.Add(ogs[i]);
            lab = new Label();
            lab.Text = label2;
            tlp.Controls.Add(lab);
            fgs[i] = new TextBox();
            tlp.Controls.Add(fgs[i]);
        }

        private decimal[][] populateValues(int idx_fs, int idx_snd)
        {
            const int valuesFs = 0;
            const int valuesSnd = 1;
            decimal[][] values = new decimal[2][];
            if (fields[idx_fs] is null)
                return values;
            int filledFields = 0;
            for (; filledFields < fields[idx_fs].Length; filledFields++)
            {
                if (fields[idx_fs][filledFields].Text == "" ||
                    fields[idx_snd][filledFields].Text is null)
                {
                    break;
                }
            }
            values[valuesFs] = new decimal[filledFields];
            values[valuesSnd] = new decimal[filledFields];
            for (int i = 0; i < filledFields; i++)
            {
                values[valuesFs][i] = getNumberFromField(fields[idx_fs][i]);
                values[valuesSnd][i] = getNumberFromField(fields[idx_snd][i]);
            }
            return values;
        }
        private void submitNumberOfAdditions_Click(object sender, System.EventArgs e)
        {
            int noOfAdditions = getIntrFromField(this.ssgNOD);
            addFieldsMatrix((int)FieldsIndexes.OGS, (int)FieldsIndexes.FGS,
                noOfAdditions, gravityPanel, PG_LABEL, NW_LABEL);
        }

        private void calcualteSSG_Click(object sender, System.EventArgs e)
        {
            decimal sgAFloat = 0;
            decimal sg = getNumberFromField(ssgSG);
            // values 0 = ogs and 1 = fgs
            const int ogs = 0, fgs = 1;

            decimal[][] values = populateValues((int)FieldsIndexes.OGS,
                (int)FieldsIndexes.FGS);
            for (int i= 0; i < values[0].Length; i++)
            {
                sg += values[fgs][i] - values[ogs][i];
                if (sgAFloat > 0)
                    sg += sgAFloat - values[fgs][i];
                sgAFloat = values[ogs][i];
            }
            ssgResult.Text = sg.ToString();
        }

        private void numberOfDrinksBtn_Click(object sender, System.EventArgs e)
        {
            int noOfAdditions = getIntrFromField(this.numberOFDrinks);
            addFieldsMatrix((int) FieldsIndexes.DRINKS, (int) FieldsIndexes.DRABV,
                noOfAdditions, drinksPanel, DRNKVOL, ABV_VAL);
        }

        private void calculateMDABV_Click(object sender, System.EventArgs e)
        {
            decimal volume = 0;
            decimal alcCont = 0;
            decimal finalAbv;
            // values = 0 for volume, 1 for abv
            const int vol = 0, abv = 1;
            decimal[][] values = populateValues((int) FieldsIndexes.DRINKS,
                (int) FieldsIndexes.DRABV);

            for (int i = 0; i < values[vol].Length; i++)
            {
                volume += values[vol][i];
                alcCont += (values[abv][i] / 100) * values[vol][i];
            }
            finalAbv = alcCont / volume * 100;
            mdabvResult.Text = finalAbv.ToString(); 
        }
    }
}