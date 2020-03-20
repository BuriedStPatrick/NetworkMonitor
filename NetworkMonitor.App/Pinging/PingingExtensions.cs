using System;
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
                .EverySecond()
                .PreventOverlapping(typeof(PingInvocable).FullName);
        }

        public static void AddPinging(
            this IServiceCollection serviceCollection,
            Action<IPingConfiguration> pingConfigurationDelegate
        )
        {
            
        }

        public static PingingBuilder AddPinging(this IServiceCollection serviceCollection)
        {
            return new PingingBuilder(serviceCollection);
        }
    }

    public class PingingBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public PingingBuilder(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<PingInvocable>();
            _serviceCollection = serviceCollection;
        }

        public PingingBuilder FromConfig(IConfiguration configuration)
        {
            _serviceCollection.AddSingleton<IPingConfiguration>(configuration.GetSection("Ping").Get<PingConfiguration>());
            return this;
        }
    }
}
