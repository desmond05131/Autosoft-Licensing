using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Autosoft_Licensing.Data
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionStringName = "LicensingDb")
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString
                ?? throw new System.InvalidOperationException($"Missing connection string '{connectionStringName}'.");        }

        public SqlConnection Create() => new SqlConnection(_connectionString);
    }
}