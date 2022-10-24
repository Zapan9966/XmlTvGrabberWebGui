using System;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "audio")]
    public class Audio
    {
        [XmlElement("present")]
        public string Present { get; set; }

        [XmlElement("stereo")]
        public string Stereo { get; set; }
    }
}
