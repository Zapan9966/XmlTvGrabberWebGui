using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "review")]
    public class Review
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("reviewer")]
        public string Reviewer { get; set; }

        [XmlAttribute("lang")]
        public string Lang { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
