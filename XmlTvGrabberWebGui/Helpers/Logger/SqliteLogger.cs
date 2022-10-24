using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using XmlTvGrabberWebGui.Data;
using XmlTvGrabberWebGui.Models;

namespace XmlTvGrabberWebGui.Helpers.Logger
{
    public class SqliteLogger : ILogger
    {
        private readonly SqliteLoggerProvider _provider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _categoryName;

        public SqliteLogger(SqliteLoggerProvider provider, IServiceScopeFactory serviceScopeFactory, string categoryName)
        {
            _provider = provider;
            _serviceScopeFactory = serviceScopeFactory;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _provider.ScopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<GrabberContext>();
            var globals = serviceScope.ServiceProvider.GetService<GlobalProperties.IGlobalProperties>();
            if (IsEnabled(logLevel))
            {
                context.Traces.Add(new Trace
                {
                    Category = _categoryName,
                    LogLevel = logLevel,
                    EventId = eventId.Id,
                    Date = DateTime.Now,
                    Message = $"{(state != null ? $"{state.ToString()}" : null)}" +
                        $"{(exception != null ? $"{Environment.NewLine}Exception: {exception.Message}{Environment.NewLine}Stacktrace: {TranslateStack(exception)}" : null)}",
                    Filename = globals?.CurrentXmlTvUrl,
                    FileProcessingId = globals?.FileProcessingId
                });
                context.SaveChanges();
            }
        }
        
        private string TranslateStack(Exception exception)
        {
            string format = "{0}; {1}; {2}; {3}; l.{4}";
            StringBuilder builder = new StringBuilder();
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
            var stackFrames = trace.GetFrames();
            if (stackFrames == null) return builder.ToString();
            foreach (System.Diagnostics.StackFrame frame in stackFrames)
            {
                string name = frame.GetMethod().Name;
                StringBuilder subBuilder = new StringBuilder();
                string fileName = frame.GetFileName();
                string str = frame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture);
                if ((frame.GetMethod() == null) || (frame.GetMethod().DeclaringType == null)) continue;
                string fullName = frame.GetMethod().DeclaringType.FullName;
                foreach (ParameterInfo info in frame.GetMethod().GetParameters())
                {
                    if (subBuilder.Length != 0)
                        subBuilder.Append(", ");

                    subBuilder.Append($"{info.ParameterType.Name} {info.Name}");
                }

                if (builder.Length != 0)
                    builder.Append(Environment.NewLine);

                builder.AppendFormat(CultureInfo.CurrentCulture, format, new object[] { name, subBuilder, fullName, fileName, str });
            }
            return builder.ToString();
        }
    }
}
