using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlTvGrabberWebGui.Helpers.GlobalProperties
{
    public class GlobalProperties : IGlobalProperties
    {
        public bool IsDisposed { get; protected set; }

        public string CurrentXmlTvUrl { get; set; }
        public string TempFolder { get; set; } = "tmp";
        public int? FileProcessingId { get; set; }

        public void ClearAll()
        {
            typeof(GlobalProperties)
                .GetProperties()
                .ToList()
                .ForEach(p => p.SetValue(this, default));

            TempFolder = "tmp";
        }

        public void ClearUrl()
        {
            CurrentXmlTvUrl = null;
        }
        public void ClearProcessingId()
        {
            FileProcessingId = null;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            if (!IsDisposed)
            {
                try
                {
                    Dispose(true);
                }
                catch
                { }

                IsDisposed = true;
                GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor   
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearAll();
            }
        }

        #endregion
    }
}
