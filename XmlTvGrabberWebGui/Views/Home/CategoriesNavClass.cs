using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace XmlTvGrabberWebGui.Views.Home
{
    public static class CategoriesNavClass
    {
        public static string Xml => "Xml";
        public static string TvHeadend => "TvHeadend";

        public static string XmlPageClass(ViewContext viewContext) => PageNavClass(viewContext, Xml);
        public static string TvHeadendPageClass(ViewContext viewContext) => PageNavClass(viewContext, TvHeadend);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["CategoriesActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
