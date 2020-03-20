using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using NetworkMonitor.App.Logging;
using NetworkMonitor.App.Logging.Enrichers;
using NetworkMonitor.App.Pinging;
using Serilog.Context;

namespace NetworkMonitor.App.Invocables
{
    public class PingInvocable : IInvocable
    {
        private readonly IPingConfiguration _pingConfiguration;
        private readonly ILogger<PingInvocable> _logger;

        public PingInvocable(
            IPingConfiguration pingConfiguration,
            ILogger<PingInvocable> logger
        )
        {
            _pingConfiguration = pingConfiguration;
            _logger = logger;
        }

        public async Task Invoke()
        {
            await foreach (var reply in GetReplies(_pingConfiguration.Addresses))
            {
                using (LogContext.Push(new PingReplyEnricher(reply)))
                {
                    var status = reply.Status;
                    if (reply.Status != IPStatus.Success)
                    {
                        using (LogContext.Push(new LogToSqlEnricher()))
                        {
                            _logger.LogError(status.ToString());
                        }
                    }
                    else
                    {
                        _logger.LogInformation(status.ToString());
                    }
                }
            }
        }

        private static async IAsyncEnumerable<PingReply> GetReplies(string[] addresses)
        {
            foreach (var address in addresses)
            {
                yield return await new Ping().SendPingAsync(address);
            }
        }
    }
}
