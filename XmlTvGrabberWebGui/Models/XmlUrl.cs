using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XmlTvGrabberWebGui.Models
{
    public class XmlUrl
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int XmlUrlId { get; set; }

        [Required]
        [Display(Name = "URL fichier XMLTV")]
        public string Url { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int Index { get; set; }
    }
}
