using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace XmlTvGrabberWebGui.Views.Shared
{
    public static class AppPages
    {
        public static string Categories => "Categories";
        public static string Grabber => "Grabber";
        public static string Configuration => "Configuration";
        public static string LogViewer => "LogViewer";

        public static string CategoriesPageClass(ViewContext viewContext) => PageNavClass(viewContext, Categories);
        public static string GrabberPageClass(ViewContext viewContext) => PageNavClass(viewContext, Grabber);
        public static string ConfigurationPageClass(ViewContext viewContext) => PageNavClass(viewContext, Configuration);
        public static string LogViewerPageClass(ViewContext viewContext) => PageNavClass(viewContext, LogViewer);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["AppActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
