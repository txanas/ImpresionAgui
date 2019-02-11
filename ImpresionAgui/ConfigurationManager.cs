using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace ImpresionAgui
{
    class ConfigurationManager
    {

        public static ConfigurationManager configurationManager;

        public static readonly string CONFIG_DIR = "config/";
        public static readonly string PAIR_DATA = "pair_data.xml";
        public static readonly string PAIR_DATA_FILE = CONFIG_DIR + PAIR_DATA;

        public ConfigurationManager()
        {
            Directory.CreateDirectory(CONFIG_DIR);
        }

        public static ConfigurationManager getInstance()
        {
            if (configurationManager == null)
            {
                configurationManager = new ConfigurationManager();
            }
            return configurationManager;
        }

        private void saveConfiguration<T>(string fileName, T saveData)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                new XmlSerializer(typeof(T)).Serialize(stream, saveData);
                stream.Flush();
                Thread.Sleep(100);
            }
        }

        public void savePairData(PairData pairData)
        {
            saveConfiguration(PAIR_DATA_FILE, pairData);
        }

        public PairData getPairData()
        {
            if (File.Exists(PAIR_DATA_FILE))
            {
                try
                {
                    return Load<PairData>(PAIR_DATA_FILE);
                }
                catch (Exception exception)
                {
                    // Hay un problema con el documento. Se borrara el documento
                    //File.Delete(PAIR_DATA_FILE);
                    return null;
                }
            }
            else
            {
                crearXML(PAIR_DATA_FILE);

                try
                {
                    return Load<PairData>(PAIR_DATA_FILE);
                }
                catch (Exception exception)
                {
                    // Hay un problema con el documento. Se borrara el documento
                    //File.Delete(PAIR_DATA_FILE);
                    return null;
                }
            }
        }

        public static T Load<T>(string fileName)
        {
            T result;
            using (FileStream stream = File.OpenRead(fileName))
            {
                result = (T)new XmlSerializer(typeof(T)).Deserialize(stream);
            }
            return result;
        }

        public void crearXML(String file_path)
        {
            XmlTextWriter writer;
            writer = new XmlTextWriter(file_path, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("PairData");
            writer.WriteElementString("IP", "192.168.1.200");
            writer.WriteElementString("Port", "9100");
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }


    }
}
