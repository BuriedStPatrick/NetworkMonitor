using System.Net.NetworkInformation;
using Serilog.Core;
using Serilog.Events;

namespace NetworkMonitor.App.Logging.Enrichers
{
    public class PingReplyEnricher : ILogEventEnricher
    {
        private readonly PingReply _pingReply;

        public PingReplyEnricher(PingReply pingReply)
        {
            _pingReply = pingReply;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RoundtripTime", _pingReply.RoundtripTime));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Address", _pingReply.Address.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TTL", _pingReply.Options.Ttl));
        }
    }
}
