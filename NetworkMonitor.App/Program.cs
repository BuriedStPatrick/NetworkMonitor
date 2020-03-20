using System;
using System.IO;
using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkMonitor.App.Logging;
using NetworkMonitor.App.Pinging;
using Serilog;

namespace NetworkMonitor.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            // Set console title if not running as a Windows service
            Console.Title = "NetworkMonitor.App";
#endif
            // Set current directory to deployment folder
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            // Set BASEDIR for Serilog configuration
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);

            // Load environment variables from .env
            DotNetEnv.Env.Load();

            var configuration = CreateConfiguration(new ConfigurationBuilder());

            Log.Logger = new LoggerConfiguration()
                .LoadConfig(configuration)
                .CreateLogger();

            // Log Logger config details
            Log.Logger.LogLoggerConfig();

            var host = CreateHostBuilder(args).Build();

            // Scheduling
            host.Services.UseScheduler(scheduler =>
            {
                scheduler.SchedulePinging();
            });

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetService<ILogger<Program>>();

                logger.LogInformation("NM_ENVIRONMENT: " + Environment.GetEnvironmentVariable("NM_ENVIRONMENT"));
            }

            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Fatal error in Service");
            }
        }

        private static IConfiguration CreateConfiguration(IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("NM_ENVIRONMENT")}.json", true, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", true, true)
                .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration(configure => CreateConfiguration(configure))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScheduler();
                    services.AddPinging().FromConfig(hostContext.Configuration);
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .UseSerilog();
    }
}
