using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels
{
    [Serializable]
    [XmlRoot(ElementName = "tv")]
    public class TV
    {
        [XmlAttribute("source-info-url")]
        public string SourceInfoUrl { get; set; }

        [XmlAttribute("source-info-name")]
        public string SourceInfoName { get; set; }

        [XmlAttribute("source-data-url")]
        public string SourceDataUrl { get; set; }

        [XmlAttribute("generator-info-name")]
        public string GeneratorInfoName { get; set; }

        [XmlAttribute("generator-info-url")]
        public string GeneratorInfoUrl { get; set; }

        [XmlElement("channel")]
        public List<Channel.Channel> Channels { get; set; }

        [XmlElement("programme")]
        public List<Programme.Programme> Programmes { get; set; }

    }
}
