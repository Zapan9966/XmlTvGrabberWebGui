using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels
{
    [Serializable]
    [XmlRoot(ElementName = "icon")]
    public class Icon
    {
        [XmlAttribute("src")]
        public string Source { get; set; }

        [XmlAttribute("width")]
        public string Width { get; set; }

        [XmlAttribute("height")]
        public string Height { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
