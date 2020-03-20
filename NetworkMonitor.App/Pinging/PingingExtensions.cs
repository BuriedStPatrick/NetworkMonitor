using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkMonitor.App.Invocables;

namespace NetworkMonitor.App.Pinging
{
    public static class PingingExtensions
    {
        public static void SchedulePinging(this IScheduler scheduler)
        {
            scheduler.Schedule<PingInvocable>()
                .EveryFiveSeconds()
                .PreventOverlapping(typeof(PingInvocable).FullName);
        }

        public static PingingConfigurationBuilder AddPinging(this IServiceCollection serviceCollection)
        {
            return new PingingConfigurationBuilder(serviceCollection);
        }
    }

    public class PingingConfigurationBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public PingingConfigurationBuilder(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<PingInvocable>();
            _serviceCollection = serviceCollection;
        }

        public PingingConfigurationBuilder FromConfig(IConfiguration configuration)
        {
            _serviceCollection.AddSingleton<IPingConfiguration>(configuration.GetSection("Ping").Get<PingConfiguration>());
            return this;
        }
    }
}
