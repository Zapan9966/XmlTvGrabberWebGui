using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    public class Rating
    {
        private string _value;

        [XmlAttribute("system")]
        public string System { get; set; }

        [XmlElement("value")]
        public string Value 
        {
            get { return _value; }
            set { _value = value.Replace("-", ""); }
        }

        [XmlElement("icon")]
        public List<Icon> Icons { get; set; }
    }
}
