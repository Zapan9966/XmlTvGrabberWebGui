using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels
{
    [Serializable]
    public class ValueLang
    {
        [XmlAttribute("lang")]
        public string Lang { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
