using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Autosoft_Licensing.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection Create();
    }

    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(string connectionStringName = "LicensingDb")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_cs))
                throw new InvalidOperationException($"Missing connection string '{connectionStringName}'.");
        }

        public SqlConnection Create()
        {
            var con = new SqlConnection(_cs);
            con.Open();
            return con;
        }
    }
}