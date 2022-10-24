using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "subtitles")]
    public class SubTitles
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }
    }
}
