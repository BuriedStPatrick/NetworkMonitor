using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Api.Extensions;

namespace NetworkMonitor.Api.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class PingStatsController : Controller
    {
        private readonly ILogger<PingStatsController> _logger;
        private readonly IConfiguration _configuration;

        public PingStatsController(
            ILogger<PingStatsController> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sql = @"
                SELECT * FROM NM_Log
            ";

            await using var connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:LogDatabase"));
            await connection.OpenAsync();

            var command = connection.CreateCommand(sql);

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<LogEntry>();
            while (reader.Read())
            {
                results.Add(new LogEntry
                {
                    Message = reader.GetString(1),
                    TimeStamp = reader.GetDateTime(4)
                });
            }

            return Json(results);
        }

        public class LogEntry
        {
            public string Message { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
