using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Channel
{
    [Serializable]
    [XmlRoot(ElementName = "url")]
    public class Url
    {
        [XmlText]
        public string Value { get; set; }
    }
}
