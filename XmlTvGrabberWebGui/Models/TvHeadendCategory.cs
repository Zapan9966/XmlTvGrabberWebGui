using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XmlTvGrabberWebGui.Models
{
    public class TvHeadendCategory
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int TvHeadendCategoryId { get; set; }

        [Required]
        [Display(Name = "Groupe")]
        public string Group { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        public virtual ICollection<XmlCategory> XmlCategories { get; set; }

        public override string ToString()
        {
            return $"{TvHeadendCategoryId} - {Group} - {Name}";
        }
    }
}
