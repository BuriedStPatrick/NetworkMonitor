using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Serilog.Core;

namespace NetworkMonitor.App.Logging
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration LoadConfig(this LoggerConfiguration loggerConfiguration, IConfiguration config)
        {
            loggerConfiguration.ReadFrom.Configuration(config);

            // Custom implementation to support additional columns
            var serilogMsSqlServerConfig = config.GetSection("Serilog.MSSqlServer").Get<SerilogMsSqlServerConfigModel>();
            if (serilogMsSqlServerConfig == null)
            {
                return loggerConfiguration;
            }

            var columnOptions = new ColumnOptions();

            // Add any custom columns
            if (serilogMsSqlServerConfig.AdditionalColumns.Any())
            {
                columnOptions.AdditionalColumns = serilogMsSqlServerConfig.AdditionalColumns?
                    .Select(column => new SqlColumn(column.Name, GetTypeForColumn(column)))
                    .ToList();
            }

            loggerConfiguration
                    .WriteTo.Logger(
                        logger => logger
                            .WriteTo.MSSqlServer(
                                serilogMsSqlServerConfig.ConnectionString,
                                serilogMsSqlServerConfig.TableName,
                                autoCreateSqlTable: serilogMsSqlServerConfig.AutoCreateSqlTable,
                                columnOptions: columnOptions
                            ).Filter.ByIncludingOnly(logEvent =>
                                logEvent.Properties.ContainsKey("LogToSql") || logEvent.Exception != null
                            )
                    );

            return loggerConfiguration;
        }

        private static SqlDbType GetTypeForColumn(ColumnConfig columnConfig)
        {
            return columnConfig.DataType switch
            {
                "string" => SqlDbType.VarChar,
                "decimal" => SqlDbType.Decimal,
                "int" => SqlDbType.Int,
                _ => throw new ArgumentOutOfRangeException(nameof(columnConfig), "DataType " + columnConfig.DataType + " not supported")
            };
        }

        public static void LogLoggerConfig(this ILogger serilogLogger)
        {
            if (!(serilogLogger is Logger logger))
            {
                return;
            }

            var sinkField = typeof(Logger).GetField("_sink", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(sinkField?.GetValue(logger) is ILogEventSink sink))
            {
                return;
            }

            var sinksField = sink.GetType().GetField("_sinks", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(sinksField?.GetValue(sink) is ILogEventSink[] sinks) || !sinks.Any())
            {
                return;
            }

            foreach (var eventSink in sinks)
            {
                serilogLogger.Debug("Logger config. Sink: {sinkName}", eventSink.GetType().FullName);
            }
        }
    }

    public class SerilogMsSqlServerConfigModel
    {
        public string? ConnectionString { get; set; }
        public string? TableName { get; set; }
        public bool AutoCreateSqlTable { get; set; }
        public ColumnConfig[]? AdditionalColumns { get; set; }
    }

    public class ColumnConfig
    {
        public string? Name { get; set; }
        public string? DataType { get; set; }
    }
}
