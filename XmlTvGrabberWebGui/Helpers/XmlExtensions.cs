using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using XmlTvGrabberWebGui.SerializeModels;

namespace XmlTvGrabberWebGui.Helpers
{
    public static class XmlExtensions
    {
        private static readonly XmlSerializerNamespaces _xns;
        private static readonly XmlSerializer _xs;

        static XmlExtensions()
        {
            _xns = new XmlSerializerNamespaces();
            _xns.Add(string.Empty, string.Empty);
            _xs = new XmlSerializer(typeof(TV));
        }

        public static TV Deserialize(this string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && Path.GetExtension(filePath) == ".xml")
            {
                string content = File.ReadAllText(filePath);
                content = Regex.Replace(content, "[\u0011\u0012\u0013\u0014]", "");
                File.WriteAllText(filePath, content);

                using StreamReader rd = new StreamReader(filePath);
                return _xs.Deserialize(rd) as TV;
            }
            return null;
        }

        public static void Serialize(this TV tv, string outputFilePath)
        {
            if (tv != null && !string.IsNullOrEmpty(outputFilePath) && Path.GetExtension(outputFilePath) == ".xml")
            {
                using StreamWriter wr = new StreamWriter(outputFilePath);
                _xs.Serialize(wr, tv, _xns);
            }
        }

    }
}
