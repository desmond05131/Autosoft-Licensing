using System.Configuration;
using System.Data.SqlClient;
using Autosoft_Licensing.Properties; // Add this

namespace Autosoft_Licensing.Data
{
    public static class SqlConnectionFactory
    {
        public static string GetConnectionString()
        {
            // 1. Check if we have dynamic settings saved
            string server = Settings.Default.DbServer;

            if (!string.IsNullOrWhiteSpace(server))
            {
                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = server;
                builder.InitialCatalog = Settings.Default.DbName;
                builder.UserID = Settings.Default.DbUser;
                builder.Password = Settings.Default.DbPassword;
                builder.IntegratedSecurity = false; // Always SQL Auth for remote
                return builder.ConnectionString;
            }

            // 2. Fallback to App.config (Local Dev or first run fallback)
            // Note: This might return null/error if App.config is empty, handled in Program.cs
            return ConfigurationManager.ConnectionStrings["LicensingDb"]?.ConnectionString;
        }
    }
}