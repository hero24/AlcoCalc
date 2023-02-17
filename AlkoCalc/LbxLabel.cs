using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;

namespace AlkoCalc
{
    enum LabelType
    {
        BEERSTAMP = 0
    } 

    [Serializable()]
    class LbxLabel
    {
        private const string LABEL_XML = "label.xml";
        private const string PROP_XML = "prop.xml";
        private const string IMAGE_FL = "Object0.jpg";

        // make theese into passable array
        private const string TITLE_PLACEHOLDER = "BEER_NAME";
        private const string PERCENTAGE_PLACEHOLDER = "BP%";

        // templates
        private readonly string[] TEMPLATES = { 
            "beerstamp.aclf" 
        };

        private XmlDocument label_xml;
        private XmlDocument prop_xml;
        private Image Object0;

        public XmlDocument LX { get { return label_xml; } }
        public XmlDocument PX {  get { return prop_xml; } }
        public Image I { get { return Object0; } }
        public LbxLabel(string lbx_path)
        {
            ZipArchive lbxfile;
            lbxfile = ZipFile.Open(lbx_path, ZipArchiveMode.Read);
            lbxfile.GetEntry(LABEL_XML);
            label_xml = new XmlDocument();
            prop_xml = new XmlDocument();

            label_xml.Load(lbxfile.GetEntry(LABEL_XML).Open());
            prop_xml.Load(lbxfile.GetEntry(PROP_XML).Open());
            //image handling
            Stream img_stream = lbxfile.GetEntry(IMAGE_FL).Open();
            if (img_stream != null)
                Object0 = Image.FromStream(img_stream);
        }

        public LbxLabel() { }

        public LbxLabel(string title, float abv, LabelType type)
        {
            string perc;
            this.loadTemplate(TEMPLATES[(int)type]);
            this.Title = title;
            perc = abv.ToString("0.00");
            this.Abv = "${perc}%";
        }

        public void loadTemplate(string template)
        {
            LbxLabel lbx = new LbxLabel();
            Stream thisBinary = File.Open(template, FileMode.Open);
            BinaryFormatter deserialize = new BinaryFormatter();
            lbx = (LbxLabel)deserialize.Deserialize(thisBinary);
            thisBinary.Close();
            this.label_xml = lbx.LX;
            this.prop_xml = lbx.PX;
            this.Object0 = lbx.I;
        }

        public static void generateTemplate(LbxLabel label, string templateName)
        {
            Stream stream = File.Open(templateName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, label);
        }

        public void saveLbx()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
                {
                    var lbl_xml = archive.CreateEntry(LABEL_XML);
                    var prp_xml = archive.CreateEntry(PROP_XML);
                    using (var entryStream = lbl_xml.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(label_xml);
                    }
                    using(var entryStream = prp_xml.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(prop_xml);
                    }
                    if (Object0 != null)
                    {
                        var obj0 = archive.CreateEntry(IMAGE_FL);
                        using (var entryStream = obj0.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            Object0.Save(entryStream, ImageFormat.Jpeg);
                        }

                    }

                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Lbx file|*.lbx";
                saveFileDialog1.Title = "Save an Borther label File";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    var bytes = memoryStream.GetBuffer();
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
        }
        public string Title
        { 
            set 
            {
                replaceXml(TITLE_PLACEHOLDER, value);
            }
        }

        public string Abv
        {
            set
            {
                replaceXml(PERCENTAGE_PLACEHOLDER, value);
            }
        }

        private void replaceXml(string old, string new_)
        {
            XmlNodeList datalist = label_xml.GetElementsByTagName("pt:data");
            XmlNode data = datalist.Item(0);
            string text = data.InnerText;
            text.Replace(old, new_);
            data.InnerText = text;
        }
    }
}
