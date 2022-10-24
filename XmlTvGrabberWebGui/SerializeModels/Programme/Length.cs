using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "length")]
    public class Length
    {
        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
