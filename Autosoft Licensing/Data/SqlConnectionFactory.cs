using System.Configuration;
using System.Data.SqlClient;

namespace Autosoft_Licensing.Data
{
    public sealed class SqlConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(string connectionStringName = "LicensingDb")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString
                  ?? throw new ConfigurationErrorsException($"Missing connection string: {connectionStringName}");
        }

        public SqlConnection Create()
        {
            var conn = new SqlConnection(_cs);
            conn.Open();
            return conn;
        }
    }
}   