using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace XmlTvGrabberWebGui.Models
{
    public class XmlCategoriesViewModel
    {
        public bool? ShowAll { get; set; }
        public IEnumerable<XmlCategory> XmlCategories { get; set; }
        public List<TvHeadendCategory> TvHeadendCategories { get; set; }
    }
}
