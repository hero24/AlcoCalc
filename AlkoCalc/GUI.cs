using System.Windows.Forms;
using System;

namespace AlkoCalc
{
    /*
     * Wine is constant proof that God loves us and loves to see us happy.
     * ~ Benjamin Franklin
     */
    public partial class GUI : Form
    {
        private const string PG_LABEL = "Previous graivty";
        private const string NW_LABEL = "New gravity";
        private const string ABV_VAL = "Drink ABV";
        private const string DRNKVOL = "Drink volume";
        private const string ACF_FILE = "datafile.dat";
        private enum FieldsIndexes
        {
            OGS,
            FGS,
            DRINKS,
            DRABV
        }
        private TextBox[][] fields = new TextBox[(int)FieldsIndexes.DRABV + 1][];
        private NotesPanel notespanel;
        private RecipePanel projectPanel;
        private Filehandler filehandle;
        private RecipePanel openedProject;

        private void constructorBody()
        {
            InitializeComponent();
            filehandle = new Filehandler(ACF_FILE);
            notespanel = new NotesPanel(newNote, filehandle.Notes);
            projectPanel = new RecipePanel(filehandle.Projects);
            projectTab.Controls.Add(projectPanel);
            notes.Controls.Add(notespanel);
        }
        public GUI()
        {
            constructorBody();
        }

        public GUI(string projectFile)
        {
            constructorBody();
            Notes<Recipe> rec = Filehandler.openRecipe(projectFile);
            if (rec == null)
                return;
            openedProject = new RecipePanel(rec);
            openedProjectTab.Controls.Add(openedProject);
            loadPrFF.Visible = false;
            sprjTF.Visible = true;
            tabs.SelectedTab = tabs.TabPages["OpenedProjectTab"];
        }

        private decimal getNumberFromField(TextBox box)
        {
            try
            {
                return decimal.Parse(box.Text);
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
            resultbox.Text = calculator.ToString();
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
            for (int i = 0; i < values[0].Length; i++)
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
            addFieldsMatrix((int)FieldsIndexes.DRINKS, (int)FieldsIndexes.DRABV,
                noOfAdditions, drinksPanel, DRNKVOL, ABV_VAL);
        }

        private void calculateMDABV_Click(object sender, System.EventArgs e)
        {
            decimal volume = 0;
            decimal alcCont = 0;
            decimal finalAbv;
            // values = 0 for volume, 1 for abv
            const int vol = 0, abv = 1;
            decimal[][] values = populateValues((int)FieldsIndexes.DRINKS,
                (int)FieldsIndexes.DRABV);

            for (int i = 0; i < values[vol].Length; i++)
            {
                volume += values[vol][i];
                alcCont += (values[abv][i] / 100) * values[vol][i];
            }
            finalAbv = alcCont / volume * 100;
            mdabvResult.Text = finalAbv.ToString();
        }
        private void newNote_Click(object sender, EventArgs e)
        {
            notespanel.newNoteAction();
        }

        protected override void OnFormClosing(FormClosingEventArgs ea)
        {
            filehandle.saveFile();
        }

        private void calculateTemp_Click(object sender, EventArgs e)
        {
            doCalculation(new TemperatureCorrection(decimal.Parse(tempGravity.Text),
                decimal.Parse(tempBox.Text)), tempResult);
        }

        private void convBtn_Click(object sender, EventArgs e)
        {
            doCalculation(new BallingConverter(decimal.Parse(gravityInBox.Text)),
                blgResult);
        }

        private void toggleProjectVisibility()
        {
            newProject.Visible =  !newProject.Visible;
            deleteProject.Visible = !deleteProject.Visible;
            typeBox.Visible = !typeBox.Visible;
            nameLabel.Visible = !nameLabel.Visible;
            nameBox.Visible = !nameBox.Visible;
            ingredientsLabel.Visible = !ingredientsLabel.Visible;
            ingredientsBox.Visible = !ingredientsBox.Visible;
            addProject.Visible = !addProject.Visible;
            saveFile.Visible = !saveFile.Visible;
        }

        private void newProject_Click(object sender, EventArgs e)
        {
            toggleProjectVisibility();
        }

        private void addProject_Click(object sender, EventArgs e)
        {
            Types projectType = (Types)typeBox.SelectedIndex;
            if (projectType > Types.MEAD)
                projectType = 0;
            var name = nameBox.Text;
            char[] splitChar =  {',', '\n' };
            var ingredients = ingredientsBox.Text.Split(splitChar);
            Recipe project = new Recipe(projectType, name, ingredients);
            projectPanel.addRecipe(project);
            toggleProjectVisibility();
        }
        private bool first_click_pr_dl = true;
        private void deleteProject_Click(object sender, EventArgs e)
        {
    
            PrIDl.Visible = !PrIDl.Visible;
            PrjID.Visible = !PrjID.Visible;    
            newProject.Visible = !newProject.Visible;
            saveFile.Visible = !saveFile.Visible;
            if(!first_click_pr_dl)
            {
                if (PrjID.Text == "")
                    return;
                projectPanel.removeProject(int.Parse(PrjID.Text));
            }
            first_click_pr_dl = !first_click_pr_dl;
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            PrIDl.Visible = !PrIDl.Visible;
            PrjID.Visible = !PrjID.Visible;
            newProject.Visible = !newProject.Visible;
            deleteProject.Visible = !saveFile.Visible;
            if(!first_click_pr_dl)
            {
                Recipe recipe = projectPanel.getRecipe(int.Parse(PrjID.Text));
                Filehandler.saveFileDialog(recipe);
            }
            first_click_pr_dl = !first_click_pr_dl;
        }

        private void loadPrFF_Click(object sender, EventArgs e)
        {
            Notes<Recipe> rec = Filehandler.openFileDialog();
            if (rec == null)
                return;
            openedProject = new RecipePanel(rec);
            openedProjectTab.Controls.Add(openedProject);
            loadPrFF.Visible = false;
            sprjTF.Visible = true;
        }

        private void sprjTF_Click(object sender, EventArgs e)
        {
            Recipe rec = openedProject.getRecipe(0);
            Filehandler.saveFileDialog(rec);
        }

        private void unitsBtn_Click(object sender, EventArgs e)
        {
            UnitCalc calc = new UnitCalc(decimal.Parse(abvUnits.Text), 
                decimal.Parse(volumeUnits.Text));
            doCalculation(calc, unitResult);
        }

        private void ukUnitsBtn_Click(object sender, EventArgs e)
        {
            UKUnit calc = new UKUnit(decimal.Parse(abvUkUn.Text),
    decimal.Parse(volUkUn.Text));
            doCalculation(calc, resultUkUm);
        }

        private void honeyWater_Click(object sender, EventArgs e)
        {
            decimal ratio;
            if (poltorak1.Checked)
                ratio = 1m / 0.5m;
            else if (dwojniak1.Checked)
                ratio = 1m / 1m;
            else if (trojniak1.Checked)
                ratio = 1m / 2m;
            else
                return;
            RatioCalculator rc = new RatioCalculator(ratio,
                decimal.Parse(honVol.Text));
            doCalculation(rc, honeyW);
        }

        private void calcWaterHon_Click(object sender, EventArgs e)
        {
            decimal ratio;
            if (poltorak2.Checked)
                ratio = 1m / 2m;
            else if (dwojniak2.Checked)
                ratio = 1m / 1m;
            else if (trojniak2.Checked)
                ratio = 2m / 1m;
            else
                return;
            RatioCalculator rc = new RatioCalculator(ratio,
                decimal.Parse(honwatervol.Text));
            doCalculation(rc, waterHonResult);
        }

        private void honWatTotCalc_Click(object sender, EventArgs e)
        {
            decimal honey, water, total, basediv;
            if (poltorak3.Checked)
            {
                honey = 2m;
                water = 1m;
            }
            else if (dwojniak3.Checked)
            {
                honey = 1m;
                water = 1m;
            }
            else if (trojniak3.Checked)
            {
                honey = 1m;
                water = 2m;
            }
            else
                return;
            total = decimal.Parse(totMeadVol.Text);
            basediv = total / (honey + water);
            meadHoneyResult.Text = (basediv * honey).ToString();
            meadWaterResult.Text = (basediv * water).ToString();

        }

        private void sgclcbtn_Click(object sender, EventArgs e)
        {
            AbvToGravity atg = new AbvToGravity(decimal.Parse(sgabvwntd.Text),
                decimal.Parse(sgfngv.Text));
            doCalculation(atg, sgrsltbx);
        }

        private void massCalculateBtn_Click(object sender, EventArgs e)
        {
            AlcoMass am = new AlcoMass(decimal.Parse(massVolBox.Text),
                decimal.Parse(abvMassBox.Text));
            doCalculation(am, resultMassBox);
        }

        private void calculateBAC_Click(object sender, EventArgs e)
        {
            decimal bwr;
            if (manBwr.Checked)
                bwr = -1m;
            else if (womanBwr.Checked)
                bwr = -2m;
            else
                bwr = decimal.Parse(custBWR.Text);
            BACCalculator bac = new BACCalculator(decimal.Parse(alcMassBac.Text),
                decimal.Parse(weightBac.Text), decimal.Parse(timeBAC.Text), bwr);
            doCalculation(bac, bacResult);
        }
    }
}
