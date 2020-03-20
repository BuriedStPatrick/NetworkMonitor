using System.Data.SqlClient;

namespace NetworkMonitor.Api.Extensions
{
    public static class SqlExtensions
    {
        public static SqlCommand AddParameter(this SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.Add(new SqlParameter(name, value));
            return cmd;
        }

        public static SqlCommand CreateCommand(this SqlConnection conn, string text)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = text;
            return cmd;
        }
    }
}
