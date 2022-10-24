using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "actor")]
    public class Actor
    {
        [XmlAttribute("role")]
        public string Role { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
