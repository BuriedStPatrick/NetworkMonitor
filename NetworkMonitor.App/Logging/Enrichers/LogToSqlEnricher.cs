using Serilog.Core;
using Serilog.Events;

namespace NetworkMonitor.App.Logging
{
    public class LogToSqlEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("LogToSql", true));
        }
    }
}
