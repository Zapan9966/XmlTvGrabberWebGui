using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XmlTvGrabberWebGui.Helpers
{
    public static class Extensions
    {
        public static string RemoveLineReturn(this string value, string replace = "")
        {
            return value?
                .Replace(Environment.NewLine, replace)
                .Replace("\r\n", replace)
                .Replace("\n", replace)
                .Replace("\r", replace)
                .Replace(((char)0x2028).ToString(), replace) // lineSeparator
                .Replace(((char)0x2029).ToString(), replace); // paragraphSeparator
        }

        public static string Nl2br(this string value)
        {
            return value?.RemoveLineReturn("<br />");
        }

        public static string Br2nl(this string value)
        {
            return value?
                .Replace("<br />", "\r\n")
                .Replace("<br/>", "\r\n")
                .Replace("<br>", "\r\n")
                .Replace("<BR />", "\r\n")
                .Replace("<BR/>", "\r\n")
                .Replace("<BR>", "\r\n");
        }

        public static string ApplyStyle(this LogLevel logLevel)
        {
            return logLevel == LogLevel.Information ? "table-info text-info"
                : logLevel == LogLevel.Warning ? "table-warning text-warning"
                : logLevel == LogLevel.Error ? "table-danger text-danger"
                : null;
        }

        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            return (T)inst?.Invoke(obj, null);
        }
    }
}
