using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace XmlTvGrabberWebGui.Helpers.Logger
{
    public class SqliteLoggerProvider : IDisposable, ILoggerProvider, ISupportExternalScope
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<string, SqliteLogger> _loggers;

        private IExternalScopeProvider _scopeProvider;
        private protected IDisposable _settingsChangeToken;

        public bool IsDisposed { get; protected set; }

        public SqliteLoggerProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _loggers = new ConcurrentDictionary<string, SqliteLogger>();
        }

        ~SqliteLoggerProvider()
        {
            if (!IsDisposed)
                Dispose(false);
        }

        internal IExternalScopeProvider ScopeProvider
        {
            get
            {
                if (_scopeProvider == null)
                    _scopeProvider = new LoggerExternalScopeProvider();

                return _scopeProvider;
            }
        }

        #region ISupportExternalScope implementation

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        #endregion

        #region ILoggerProvider implementation

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new SqliteLogger(this, _serviceScopeFactory, category));
        }

        #endregion

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
                {
                }

                IsDisposed = true;
                GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor   
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_settingsChangeToken != null)
            {
                _settingsChangeToken.Dispose();
                _settingsChangeToken = null;
            }
        }

        #endregion

    }
}
