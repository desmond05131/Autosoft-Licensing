using System.Configuration;
using System.Data.SqlClient;
using Autosoft_Licensing.Properties; // Add this

namespace Autosoft_Licensing.Data
{
    public static class SqlConnectionFactory
    {
        public static string GetConnectionString()
        {
            string server = Settings.Default.DbServer;

            // STRICT CHECK: Only return a string if we actually have user settings.
            if (!string.IsNullOrWhiteSpace(server))
            {
                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = server;
                builder.InitialCatalog = Settings.Default.DbName;
                builder.UserID = Settings.Default.DbUser;
                builder.Password = Settings.Default.DbPassword;
                builder.IntegratedSecurity = false;
                builder.TrustServerCertificate = true; // Add this
                builder.Encrypt = false;               // Add this
                return builder.ConnectionString;
            }

            // CRITICAL CHANGE: Return NULL instead of fallback.
            // This forces Program.cs to throw the "No settings" exception and show the Settings Form.
            return null;
        }
    }
}