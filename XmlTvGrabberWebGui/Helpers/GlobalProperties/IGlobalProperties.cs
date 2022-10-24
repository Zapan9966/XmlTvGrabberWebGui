using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlTvGrabberWebGui.Helpers.GlobalProperties
{
    public interface IGlobalProperties : IDisposable
    {
        public string CurrentXmlTvUrl { get; set; }
        public string TempFolder { get; set; }
        public int? FileProcessingId { get; set; }

        public void ClearAll();
        public void ClearUrl();
        public void ClearProcessingId();
    }
}
