using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "previously-shown")]
    public class PreviouslyShown
    {
        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("channel")]
        public string Channel { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
