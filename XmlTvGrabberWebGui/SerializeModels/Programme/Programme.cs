using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace XmlTvGrabberWebGui.SerializeModels.Programme
{
    [Serializable]
    [XmlRoot(ElementName = "programme")]
    public class Programme
    {
        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("stop")]
        public string Stop { get; set; }

        [XmlAttribute("pdc-start")]
        public string PdcStart { get; set; }

        [XmlAttribute("vps-start")]
        public string VpsStart { get; set; }

        [XmlAttribute("showview")]
        public string ShowView { get; set; }

        [XmlAttribute("videoplus")]
        public string VideoPlus { get; set; }

        [XmlAttribute("channel")]
        public string Channel { get; set; }

        [XmlAttribute("clumpidx")]
        public string ClumpIdx { get; set; }

        // *******************************************************

        [XmlElement("title")]
        public List<ValueLang> Titles { get; set; }

        [XmlElement("sub-title")]
        public List<ValueLang> SubTitle { get; set; }

        [XmlElement("desc")]
        public List<ValueLang> Descriptions { get; set; }

        [XmlElement("credits")]
        public Credits Credits { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("category")]
        public List<ValueLang> Categories { get; set; }

        [XmlElement("keyword")]
        public List<ValueLang> Keywords { get; set; }

        [XmlElement("language")]
        public ValueLang Language { get; set; }

        [XmlElement("orig-language")]
        public ValueLang OrigLanguage { get; set; }

        [XmlElement("length")]
        public Length Length { get; set; }

        [XmlElement("icon")]
        public List<Icon> Icons { get; set; }

        [XmlElement("country")]
        public List<ValueLang> Countries { get; set; }

        [XmlElement("episode-num")]
        public List<EpisodeNum> EpisodeNums { get; set; }

        [XmlElement("video")]
        public Video Video { get; set; }

        [XmlElement("audio")]
        public Audio Audio { get; set; }

        [XmlElement("previously-shown")]
        public PreviouslyShown PreviouslyShown { get; set; }

        [XmlElement("premiere")]
        public ValueLang Premiere { get; set; }

        [XmlElement("last-chance")]
        public ValueLang LastChance { get; set; }

        [XmlElement("new")]
        public string New { get; set; }

        [XmlElement("subtitles")]
        public List<SubTitles> SubTitles { get; set; }

        [XmlElement("rating")]
        public List<Rating> Ratings { get; set; }

        [XmlElement("star-rating")]
        public List<Rating> StarRatings { get; set; }

        [XmlElement("review")]
        public List<Review> Reviews { get; set; }
    }
}
