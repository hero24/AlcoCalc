using System;
using System.Windows.Forms;

// TODO: 
// allow for saving single project
//  --> need to figure saving and reading procedure.

namespace AlkoCalc
{
    public enum Types : uint
    {
        BEER = 0,
        WINE,
        SPIRIT,
        MEAD
    }
    [Serializable()]
    public class Recipe
    {
        public enum Index : uint
        {
            Type = 0, Name, Abv, Blg, Ssg, Fsg,
            Water, Spirit, SprStr, Notes, Desc, Ingr, Prop,
            CrD, EdiD
        }
        public bool[] editable = {
            false, false, false, false,
            true, true, true, true,
            true, true, true, true,
            true, false, false
        };
        public static readonly bool[] beer_states =  {
            true, true, true, true, true, true, false, false, 
            false, true, true, true, true, true, true    
        };
        public static readonly bool[] wine_states = {
            true, true, true, true, true, true, false, false,
            false, true, true, true, true, true, true
        };
        public static readonly bool[] spirit_states = {
            true, true, false, false, false, false, true, true,
            true, true, true, true, true, true, true
        };
        public static readonly bool[] mead_states = {
            true, true, true, true, true, true, false, false,
            false, true, true, true, true, true, true
        };
        public static readonly bool[][] enabled = { 
            beer_states, wine_states, spirit_states, mead_states
        };
        private const short INITIAL = 10;
        private const short INCR = 5;
        private uint ing_idx;
        private uint prop_idx;
        private Types type;
        private float abv;
        private float blg;
        private decimal starting_sg;
        private decimal finishing_sg;
        private decimal water;
        private decimal spirit;
        private decimal strength;
        private string name;
        private string notes;
        private string description;
        private string[] ingredients;
        private string[] properties;
        private DateTime created;
        private DateTime lastEdited;
        private LbxLabel label;

        public Recipe(Types type, string name, string[] ingredients)
        {
            this.type = type;
            this.name = name;
            this.ingredients = ingredients;
            this.created = DateTime.Now;
            this.created = lastEdited;
            ing_idx = (uint) ingredients.Length;
            this.properties = new string[0];
            for (int i = 0; i < editable.Length; i++)
            {
                if(editable[i] && !enabled[(int)type][i])
                {
                    editable[i] = !editable[i];
                }
            }
        }

        public String this[Index idx]
        {
            get 
            {
                switch (idx)
                {
                    case Index.Type:
                        return type.ToString();
                    case Index.Abv:
                        return abv.ToString();
                    case Index.Blg:
                        return Blg.ToString();
                    case Index.CrD:
                        return Created.ToString();
                    case Index.EdiD:
                        return Edited.ToString();
                    case Index.Name:
                        return Name;
                    case Index.Desc:
                        return Description;
                    case Index.Fsg:
                        return finishingGravity.ToString();
                    case Index.Ingr:
                        return String.Join(",", ingredients);
                    case Index.Notes:
                        return Notes;
                    case Index.Prop:
                        return String.Join(",", Properties);
                    case Index.Spirit:
                        return Spirit.ToString();
                    case Index.Ssg:
                        return startingGravity.ToString();
                    case Index.Water:
                        return Water.ToString();
                    case Index.SprStr:
                        return strength.ToString();

                }
                return "";
            }
            set
            {
                char[] splitChar = { ',', '\n' };
                switch (idx)
                {
                    case Index.Type:
                    case Index.Abv:
                    case Index.Blg:
                    case Index.CrD:
                    case Index.EdiD:
                    case Index.Name:
                        // these fields are readonly.
                        break;
                    case Index.Desc:
                        Description = value;
                        break;
                    case Index.Fsg:
                        finishingGravity = decimal.Parse(value);
                        break;
                    case Index.Ingr:
                        ingredients = value.Split(splitChar);
                        break;
                    case Index.Notes:
                        Notes = value;
                        break;
                    case Index.Prop:
                        properties = value.Split(splitChar);
                        break;
                    case Index.Spirit:
                        Spirit = decimal.Parse(value);
                        break;
                    case Index.Ssg:
                        startingGravity = decimal.Parse(value);
                        setSpiritProp(water, spirit, strength);
                        break;
                    case Index.Water:
                        Water = decimal.Parse(value);
                        setSpiritProp(water, spirit, strength);
                        break;
                    case Index.SprStr:
                        strength = decimal.Parse(value);
                        setSpiritProp(water, spirit, strength);
                        break;
                }
            }
        }

        private string[] resizeArray(string[] array)
        {
            string[] ing = new string[array.Length + INCR];
            for (uint i = 0; i < array.Length; i++)
            {
                ing[i] = array[i];
            }
            return ing;
        }
        public void addIngredient(string ingredient)
        {
            if (ing_idx == ingredients.Length)
                ingredients = resizeArray(ingredients);
            ingredients[ing_idx++] = ingredient;
            lastEdited = DateTime.Now;
        }
        public void addProperty(string property)
        {
            if (properties is null)
                properties = new string[INITIAL];
            if (prop_idx == properties.Length)
                properties = resizeArray(properties);
            properties[prop_idx++] = property;
            lastEdited = DateTime.Now;

        }
        public decimal startingGravity 
        { 
            set
            {
                starting_sg = value;
                var calc = new BallingConverter(starting_sg);
                blg = (float) calc.Calculate();
                lastEdited = DateTime.Now;
            }
            get { return starting_sg; } 
        }

        public decimal finishingGravity
        {
            set
            {
                finishing_sg = value;
                if (type == AlkoCalc.Types.SPIRIT)
                    return;
                var calc = new SugarGravityAbv(starting_sg, finishing_sg);
                this.abv = (float)calc.Calculate();
                lastEdited = DateTime.Now;
            }
            get { return finishing_sg; }
        }

        public void setSpiritProp(decimal water, decimal alc, decimal alc_abv)
        {
            if ((this.type != Types.SPIRIT || (water <= 0 || alc <= 0)) && alc_abv <= 0)
                return;
            this.water = water;
            this.spirit = alc;
            var calc = new Diluter(alc_abv, water, alc);
            this.abv = (float) calc.Calculate();
            lastEdited = DateTime.Now;
        }
        public float Abv { get { return abv; } }
        public float Blg { get { return blg; } }
        public string Type { get { return type.ToString(); } }
        public string Name { get { return name; } }
        public string Notes
        { 
            get { return notes; } 
            set 
            { 
                notes = value;
                lastEdited = DateTime.Now;
            }
        }
        public string Description
        { 
            get { return description; }
            set
            { 
                description = value;
                lastEdited = DateTime.Now;
            }
        }
        public string[] Ingredients { get { return ingredients; } }
        public string[] Properties { get { return properties; } }
        public decimal Water
        { 
            get { return water; } 
            set 
            { 
                water = value;
                lastEdited = DateTime.Now;
            } 
        }
        public decimal Spirit
        { 
            get { return spirit; } 
            set 
            { 
                spirit = value;
                lastEdited = DateTime.Now;
            } 
        }
        public DateTime Created { get { return created; } }
        public DateTime Edited { get { return lastEdited; } }
    }
    
    public class RecipeBox: TextBox
    {
        Recipe recipe;
        public RecipeBox[] boxGroup;
        Recipe.Index index;
        public RecipeBox(Recipe recipe, RecipeBox[] boxes, Recipe.Index idx)
        {
            this.recipe = recipe;
            this.boxGroup = boxes;
            this.index = idx;
            Leave += eventhandler;
        }
        public void refreshRecipeBoxes()
        {
            boxGroup[(int)Recipe.Index.Type].Text = (string)recipe.Type;
            boxGroup[(int)Recipe.Index.Name].Text = recipe.Name;
            boxGroup[(int)Recipe.Index.Abv].Text = recipe.Abv.ToString();
            boxGroup[(int)Recipe.Index.Blg].Text = recipe.Blg.ToString();
            boxGroup[(int)Recipe.Index.CrD].Text = recipe.Created.ToString();
            boxGroup[(int)Recipe.Index.Desc].Text = recipe.Description;
            boxGroup[(int)Recipe.Index.EdiD].Text = recipe.Edited.ToString();
            boxGroup[(int)Recipe.Index.Fsg].Text = recipe.finishingGravity.ToString();
            boxGroup[(int)Recipe.Index.Ingr].Text = String.Join(" ", recipe.Ingredients);
            boxGroup[(int)Recipe.Index.Notes].Text = recipe.Notes;
            boxGroup[(int)Recipe.Index.Prop].Text = String.Join(" ", recipe.Properties);
            boxGroup[(int)Recipe.Index.Spirit].Text = recipe.Spirit.ToString();
            boxGroup[(int)Recipe.Index.Ssg].Text = recipe.startingGravity.ToString();
            boxGroup[(int)Recipe.Index.Water].Text = recipe.Water.ToString();
            boxGroup[(int)Recipe.Index.SprStr].Text = recipe[Recipe.Index.SprStr];
        }
        
        private void eventhandler(object sender, System.EventArgs ea)
        {
            this.recipe[index] = this.Text;
            refreshRecipeBoxes();
        }
    }

    public class RecipePanel : TableLayoutPanel
    {
        private static readonly string[] labels = {
            "Type", "Name", "Abv", "Blg", "Starting gravity",
            "Finishing gravity", "Water", "Spirit", "Spirit(%) strength", "Notes",
            "Description", "Ingredients", "Properties", "Created date",
            "Last edited"
        };
        Notes<Recipe> projectBox;
        public RecipePanel(Notes<Recipe> projectBox)
        {
            initPanel();
            this.projectBox = projectBox;
            this.AutoScroll = true;
            loadFromNotes();
        }

        private void initPanel()
        {
            float per_sz = 100 / labels.Length;
            this.Width = 1064;
            this.Height = 362;
            this.Location = new System.Drawing.Point(6, 6);
            this.ColumnCount = labels.Length;
            this.ColumnStyles.Clear();
            for (int i = 0; i < labels.Length; i++)
            {
                ColumnStyle cs = new ColumnStyle(SizeType.Percent, per_sz);
                this.ColumnStyles.Add(cs);
            }
            foreach (ColumnStyle style in this.ColumnStyles)
            {
                style.SizeType = SizeType.Percent;
                style.Width = per_sz;
            }
            foreach (RowStyle style in this.RowStyles)
            {
                style.SizeType = SizeType.AutoSize;
            }
            initLabels();
        }

        public Recipe getRecipe(int i)
        {
            return projectBox.getNote(i);
        }
        private void loadFromNotes()
        {
            for(int i = 0; i < projectBox.notesQuantity(); i++)
            {
                addRecipeControls(projectBox.getNote(i));
            }
        }
        private void initLabels()
        {
            foreach (string title in labels)
            {
                Label label = new Label();
                label.Text = title;
                Controls.Add(label);
            }
        }

        private void addRecipeControls(Recipe recipe)
        {
            int i;
            int fieldsQ = Enum.GetNames(typeof(Recipe.Index)).Length;
            RecipeBox[] boxes = new RecipeBox[fieldsQ];
            for (i = 0; i < boxes.Length; i++)
            {
                boxes[i] = new RecipeBox(recipe, boxes, (Recipe.Index)i);
                this.Controls.Add(boxes[i]);
                boxes[i].Enabled = recipe.editable[i];
            }
            boxes[--i].refreshRecipeBoxes();
        }
        public void addRecipe(Recipe recipe)
        {
            projectBox.addNote(recipe);
            addRecipeControls(recipe);
        }
        
        public void removeProject(int id)
        {
            Recipe recipe = projectBox.getNote(id);
            id += labels.Length;
            RecipeBox box = (RecipeBox) this.Controls[id];
            foreach (RecipeBox b in box.boxGroup)
                this.Controls.Remove(b);
            projectBox.deleteNote(recipe);
        }
    }
}
