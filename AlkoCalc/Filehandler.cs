using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AlkoCalc
{
    [Serializable()]
    public struct ACFile
    {
        public string filename;
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
                    notes = filemeta.notes;
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
            filemeta.filename = this.filename;
            filemeta.creationDate = DateTime.Now;
            filemeta.notes = new Notes<Note>();
            filemeta.projects = new Notes<Recipe>();
        }

        public void saveFile()
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, filemeta);
            stream.Close();
        }
    }
}