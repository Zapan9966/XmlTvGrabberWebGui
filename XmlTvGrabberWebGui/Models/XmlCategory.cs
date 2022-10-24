using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XmlTvGrabberWebGui.Models
{
    public class XmlCategory
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int XmlCategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? TvHeadendCategoryId { get; set; }

        [ForeignKey("TvHeadendCategoryId")]
        public virtual TvHeadendCategory TvHeadendCategory { get; set; }

        public override string ToString()
        {
            return $"{XmlCategoryId} - {Name}{(!string.IsNullOrEmpty(TvHeadendCategory?.Name) ? $" - {TvHeadendCategory?.Name}" : null)}";
        }
    }
}
