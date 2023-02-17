using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace AlkoCalc
{
    [Serializable()]
    public struct ACFile
    {
        public string filename;
        public byte contents;
        public DateTime creationDate;
        public Notes<Note> notes;
        public Notes<Recipe> projects;
    }
    public class Filehandler
    {
        string filename;
        ACFile filemeta;
        Notes<Note> notes;
        Notes<Recipe> projects;
        public const byte ACF_NOTES_MASK = 1;
        public const byte ACF_PRJCT_MASK = 2;
        public const byte ACF_ALL_FLD = ACF_PRJCT_MASK | ACF_NOTES_MASK;

        public Filehandler(string filename)
        {
            this.filename = filename;
            loadFile();
        }

        public Notes<Note> Notes { get { return notes; } }
        public Notes<Recipe> Projects { get { return projects; } }
        private bool loadFile()
        {
            FileInfo notefile = new FileInfo(filename);
            if (notefile.Exists)
            {
                try
                {
                    Stream notesBinary = File.Open(filename, FileMode.Open);
                    BinaryFormatter deserialize = new BinaryFormatter();
                    filemeta = (ACFile)deserialize.Deserialize(notesBinary);
                    notesBinary.Close();
                    if (filemeta.filename == null)
                        goto cr_new;
                    if ((filemeta.contents & ACF_NOTES_MASK) == ACF_NOTES_MASK)
                        notes = filemeta.notes;
                    if ((filemeta.contents & ACF_PRJCT_MASK) == ACF_PRJCT_MASK)
                        projects = filemeta.projects;

                }
                catch (Exception e)
                {
                    goto cr_new;
                }
                return true;
            }
            else
                goto cr_new;
            cr_new:
            createFile();
            notes = filemeta.notes;
            projects = filemeta.projects;
            return false;
        }

        private void createFile()
        {
            filemeta = new ACFile();
            filemeta.contents = ACF_ALL_FLD;
            filemeta.filename = this.filename;
            filemeta.creationDate = DateTime.Now;
            filemeta.notes = new Notes<Note>();
            filemeta.projects = new Notes<Recipe>();
        }
        
        public void saveFile()
        {
            saveCustomFile(this.filename, this.filemeta);
        }
        private static void saveCustomFile(string filename, ACFile filemeta)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, filemeta);
            stream.Close();
        }

        public static void saveFileDialog(Recipe recipe)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "ACF file|*.acf";
            saveFileDialog1.Title = "Save an recipe File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                saveSingleProject(recipe, saveFileDialog1.FileName,fs);
                fs.Close();
            }
        }

        public static Notes<Recipe> openFileDialog()
        {
            ACFile fileContent;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "acf files (*.acf)|*.acf";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    try
                    {
                        Stream notesBinary = File.Open(filePath, FileMode.Open);
                        BinaryFormatter deserialize = new BinaryFormatter();
                        fileContent = (ACFile)deserialize.Deserialize(notesBinary);
                        notesBinary.Close();
                        if ((fileContent.contents & ACF_PRJCT_MASK) == ACF_PRJCT_MASK)
                            return fileContent.projects;
                    }
                    catch (Exception e)
                    {
                        goto nll;
                    }
                }
            }
        nll:
            return null;
        }

        private static void saveCustomFile(string filename, ACFile filemeta, Stream stream)
        {
            //Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, filemeta);
        }

        public static void saveSingleProject(Recipe recipe, string filename, Stream stream)
        {
            ACFile file = new ACFile();
            file.contents = ACF_PRJCT_MASK;
            file.filename = filename;
            file.creationDate = DateTime.Now;
            file.projects = new Notes<Recipe>();
            file.projects.addNote(recipe);
            saveCustomFile(file.filename, file, stream);
        }
    }
}
