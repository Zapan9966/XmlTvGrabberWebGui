using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Channel
{
    [Serializable]
    [XmlRoot(ElementName = "channel")]
    public class Channel
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("display-name")]
        public List<ValueLang> DisplayNames { get; set; }

        [XmlElement("url")]
        public List<Url> Urls { get; set; }

        [XmlElement("icon")]
        public List<Icon> Icons { get; set; }
    }
}
