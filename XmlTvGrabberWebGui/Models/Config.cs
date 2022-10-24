using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XmlTvGrabberWebGui.Models
{
    public class Config
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ConfigId { get; set; }

        [Required]
        [Display(Name = "Nom fichier XML local")]
        public string OutputFilename { get; set; }

        [Required]
        [Display(Name = "Chemin du sock Unix")]
        public string SockPath { get; set; }

        [Required]
        [Display(Name = "Chemin base de données EPG")]
        public string EpgDatabasePath { get; set; }

    }
}
