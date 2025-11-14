using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Services
{
    public sealed class LicenseDatabaseService : ILicenseDatabaseService
    {
        private readonly ISqlConnectionFactory _factory;
        public LicenseDatabaseService(ISqlConnectionFactory factory) => _factory = factory;

        // -------- Users
        public User GetUserByUsername(string username)
        {
            using var con = _factory.Create();
            using var cmd = new SqlCommand(@"SELECT TOP 1 Id, Username, DisplayName, Role, Email, PasswordHash, CreatedUtc
                                             FROM dbo.Users WHERE Username = @u", con);
            cmd.Parameters.AddWithValue("@u", username);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;     
            return MapUser(r);
        }

        public User GetUserById(int id)
        {
            using var con = _factory.Create();
            using var cmd = new SqlCommand(@"SELECT TOP 1 Id, Username, DisplayName, Role, Email, PasswordHash, CreatedUtc
                                             FROM dbo.Users WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapUser(r);
        }

        // -------- License Requests
        public int InsertLicenseRequest(LicenseRequestRecord rec)
        {
            using var con = _factory.Create();
            using var tx = con.BeginTransaction();

            int newId;
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.LicenseRequests
(CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId)
VALUES (@CompanyName, @ProductID, @DealerCode, @LicenseType, @RequestedPeriodMonths, @LicenseKey, @CurrencyCode, @RequestDateUtc, @RequestFileBase64, @CreatedByUserId);
SELECT CAST(SCOPE_IDENTITY() AS INT);
", con, tx))
            {
                cmd.Parameters.AddWithValue("@CompanyName", rec.CompanyName);
                cmd.Parameters.AddWithValue("@ProductID", rec.ProductID);
                cmd.Parameters.AddWithValue("@DealerCode", rec.DealerCode);
                cmd.Parameters.AddWithValue("@LicenseType", rec.LicenseType.ToString());
                cmd.Parameters.AddWithValue("@RequestedPeriodMonths", rec.RequestedPeriodMonths);
                cmd.Parameters.AddWithValue("@LicenseKey", rec.LicenseKey);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object)rec.CurrencyCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequestDateUtc", rec.RequestDateUtc);
                cmd.Parameters.AddWithValue("@RequestFileBase64", (object)rec.RequestFileBase64 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedByUserId", rec.CreatedByUserId);
                newId = (int)cmd.ExecuteScalar();
            }

            if (rec.ModuleCodes != null && rec.ModuleCodes.Count > 0)
                UpsertRequestModules(con, tx, newId, rec.ModuleCodes);

            tx.Commit();
            return newId;
        }

        public LicenseRequestRecord GetLicenseRequestById(int id)
        {
            using var con = _factory.Create();
            LicenseRequestRecord rec = null;
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId
FROM dbo.LicenseRequests WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    rec = new LicenseRequestRecord
                    {
                        Id = r.GetInt32(0),
                        CompanyName = r.GetString(1),
                        ProductID = r.GetString(2),
                        DealerCode = r.GetString(3),
                        LicenseType = Enum.Parse<LicenseType>(r.GetString(4), true),
                        RequestedPeriodMonths = r.GetInt32(5),
                        LicenseKey = r.GetString(6),
                        CurrencyCode = r.IsDBNull(7) ? null : r.GetString(7),
                        RequestDateUtc = r.GetDateTime(8),
                        RequestFileBase64 = r.IsDBNull(9) ? null : r.GetString(9),
                        CreatedByUserId = r.GetInt32(10),
                        ModuleCodes = new List<string>()
                    };
                }
            }
            if (rec == null) return null;

            rec.ModuleCodes = ReadRequestModules(con, rec.Id);
            return rec;
        }

        public IEnumerable<LicenseRequestRecord> GetLicenseRequests(string productId = null)
        {
            using var con = _factory.Create();
            var list = new List<LicenseRequestRecord>();
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId
FROM dbo.LicenseRequests
WHERE (@pid IS NULL OR ProductID = @pid)
ORDER BY RequestDateUtc DESC", con))
            {
                cmd.Parameters.AddWithValue("@pid", (object)productId ?? DBNull.Value);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var rec = new LicenseRequestRecord
                    {
                        Id = r.GetInt32(0),
                        CompanyName = r.GetString(1),
                        ProductID = r.GetString(2),
                        DealerCode = r.GetString(3),
                        LicenseType = Enum.Parse<LicenseType>(r.GetString(4), true),
                        RequestedPeriodMonths = r.GetInt32(5),
                        LicenseKey = r.GetString(6),
                        CurrencyCode = r.IsDBNull(7) ? null : r.GetString(7),
                        RequestDateUtc = r.GetDateTime(8),
                        RequestFileBase64 = r.IsDBNull(9) ? null : r.GetString(9),
                        CreatedByUserId = r.GetInt32(10),
                        ModuleCodes = new List<string>()
                    };
                    list.Add(rec);
                }
            }

            // Optionally load modules per request (second query per row to keep SQL simple)
            foreach (var rec in list)
                rec.ModuleCodes = ReadRequestModules(con, rec.Id);

            return list;
        }

        // -------- Licenses
        public int InsertLicense(LicenseMetadata meta)
        {
            using var con = _factory.Create();
            using var tx = con.BeginTransaction();
            int newId;

            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Licenses
(CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64)
VALUES (@CompanyName, @ProductID, @DealerCode, @LicenseKey, @LicenseType, @ValidFromUtc, @ValidToUtc, @CurrencyCode, @Status, @ImportedOnUtc, @ImportedByUserId, @RawAslBase64);
SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
            {
                cmd.Parameters.AddWithValue("@CompanyName", meta.CompanyName);
                cmd.Parameters.AddWithValue("@ProductID", meta.ProductID);
                cmd.Parameters.AddWithValue("@DealerCode", meta.DealerCode);
                cmd.Parameters.AddWithValue("@LicenseKey", meta.LicenseKey);
                cmd.Parameters.AddWithValue("@LicenseType", meta.LicenseType.ToString());
                cmd.Parameters.AddWithValue("@ValidFromUtc", meta.ValidFromUtc);
                cmd.Parameters.AddWithValue("@ValidToUtc", meta.ValidToUtc);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object)meta.CurrencyCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", meta.Status.ToString());
                cmd.Parameters.AddWithValue("@ImportedOnUtc", meta.ImportedOnUtc);
                cmd.Parameters.AddWithValue("@ImportedByUserId", (object)meta.ImportedByUserId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawAslBase64", (object)meta.RawAslBase64 ?? DBNull.Value);
                newId = (int)cmd.ExecuteScalar();
            }

            if (meta.ModuleCodes != null && meta.ModuleCodes.Count > 0)
                UpsertLicenseModules(con, tx, newId, meta.ModuleCodes);

            tx.Commit();
            return newId;
        }

        public LicenseMetadata GetLicenseById(int id)
        {
            using var con = _factory.Create();
            LicenseMetadata meta = null;
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64
FROM dbo.Licenses WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    meta = MapLicense(r);
                }
            }
            if (meta == null) return null;
            meta.ModuleCodes = ReadLicenseModules(con, meta.Id);
            return meta;
        }

        public LicenseMetadata GetActiveLicense(string productId, string companyName)
        {
            using var con = _factory.Create();
            LicenseMetadata meta = null;
            using (var cmd = new SqlCommand(@"
SELECT TOP 1 Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64
FROM dbo.Licenses
WHERE ProductID = @pid AND CompanyName = @c
ORDER BY ImportedOnUtc DESC", con))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@c", companyName);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                    meta = MapLicense(r);
            }
            if (meta == null) return null;
            meta.ModuleCodes = ReadLicenseModules(con, meta.Id);
            return meta;
        }

        public IEnumerable<LicenseMetadata> GetLicenses(string productId = null)
        {
            using var con = _factory.Create();
            var list = new List<LicenseMetadata>();
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64
FROM dbo.Licenses
WHERE (@pid IS NULL OR ProductID = @pid)
ORDER BY ImportedOnUtc DESC", con))
            {
                cmd.Parameters.AddWithValue("@pid", (object)productId ?? DBNull.Value);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(MapLicense(r));
            }

            foreach (var m in list)
                m.ModuleCodes = ReadLicenseModules(con, m.Id);

            return list;
        }

        // -------- Modules association
        public void SetRequestModules(int requestId, IEnumerable<string> moduleCodes)
        {
            using var con = _factory.Create();
            using var tx = con.BeginTransaction();
            ClearRequestModules(con, tx, requestId);
            if (moduleCodes != null)
                UpsertRequestModules(con, tx, requestId, moduleCodes);
            tx.Commit();
        }

        public void SetLicenseModules(int licenseId, IEnumerable<string> moduleCodes)
        {
            using var con = _factory.Create();
            using var tx = con.BeginTransaction();
            ClearLicenseModules(con, tx, licenseId);
            if (moduleCodes != null)
                UpsertLicenseModules(con, tx, licenseId, moduleCodes);
            tx.Commit();
        }

        public bool LicenseKeyExists(string licenseKey)
        {
            using var con = _factory.Create();
            using var cmd = new SqlCommand("SELECT 1 FROM dbo.Licenses WHERE LicenseKey = @k", con);
            cmd.Parameters.AddWithValue("@k", licenseKey);
            var obj = cmd.ExecuteScalar();
            return obj != null;
        }

        public bool TryGetLatestLicenseSummary(string productId, string companyName,
            out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status)
        {
            licenseType = status = string.Empty;
            validFromUtc = validToUtc = DateTime.MinValue;

            using var con = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 LicenseType, ValidFromUtc, ValidToUtc, Status
FROM dbo.Licenses
WHERE ProductID = @pid AND CompanyName = @c
ORDER BY ImportedOnUtc DESC", con);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@c", companyName);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return false;

            licenseType = r.GetString(0);
            validFromUtc = r.GetDateTime(1);
            validToUtc = r.GetDateTime(2);
            status = r.GetString(3);
            return true;
        }

        // -------- Helpers
        private static User MapUser(SqlDataReader r) => new User
        {
            Id = r.GetInt32(0),
            Username = r.GetString(1),
            DisplayName = r.GetString(2),
            Role = r.GetString(3),
            Email = r.IsDBNull(4) ? null : r.GetString(4),
            PasswordHash = r.GetString(5),
            CreatedUtc = r.GetDateTime(6)
        };

        private static LicenseMetadata MapLicense(SqlDataReader r) => new LicenseMetadata
        {
            Id = r.GetInt32(0),
            CompanyName = r.GetString(1),
            ProductID = r.GetString(2),
            DealerCode = r.GetString(3),
            LicenseKey = r.GetString(4),
            LicenseType = Enum.Parse<LicenseType>(r.GetString(5), true),
            ValidFromUtc = r.GetDateTime(6),
            ValidToUtc = r.GetDateTime(7),
            CurrencyCode = r.IsDBNull(8) ? null : r.GetString(8),
            Status = Enum.Parse<LicenseStatus>(r.GetString(9), true),
            ImportedOnUtc = r.GetDateTime(10),
            ImportedByUserId = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
            RawAslBase64 = r.IsDBNull(12) ? null : r.GetString(12),
            ModuleCodes = new List<string>()
        };

        private void UpsertRequestModules(SqlConnection con, SqlTransaction tx, int requestId, IEnumerable<string> moduleCodes)
        {
            // Find ProductId for request
            int productSqlId = GetProductSqlIdForRequest(con, tx, requestId);
            var moduleIds = ResolveModuleIds(con, tx, productSqlId, moduleCodes);

            foreach (var mid in moduleIds)
            {
                using var cmd = new SqlCommand(@"INSERT INTO dbo.LicenseRequestModules (LicenseRequestId, ModuleId) VALUES (@r, @m)", con, tx);
                cmd.Parameters.AddWithValue("@r", requestId);
                cmd.Parameters.AddWithValue("@m", mid);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpsertLicenseModules(SqlConnection con, SqlTransaction tx, int licenseId, IEnumerable<string> moduleCodes)
        {
            // Find ProductId for license
            int productSqlId = GetProductSqlIdForLicense(con, tx, licenseId);
            var moduleIds = ResolveModuleIds(con, tx, productSqlId, moduleCodes);

            foreach (var mid in moduleIds)
            {
                using var cmd = new SqlCommand(@"INSERT INTO dbo.LicenseModules (LicenseId, ModuleId) VALUES (@l, @m)", con, tx);
                cmd.Parameters.AddWithValue("@l", licenseId);
                cmd.Parameters.AddWithValue("@m", mid);
                cmd.ExecuteNonQuery();
            }
        }

        private void ClearRequestModules(SqlConnection con, SqlTransaction tx, int requestId)
        {
            using var cmd = new SqlCommand("DELETE FROM dbo.LicenseRequestModules WHERE LicenseRequestId = @id", con, tx);
            cmd.Parameters.AddWithValue("@id", requestId);
            cmd.ExecuteNonQuery();
        }

        private void ClearLicenseModules(SqlConnection con, SqlTransaction tx, int licenseId)
        {
            using var cmd = new SqlCommand("DELETE FROM dbo.LicenseModules WHERE LicenseId = @id", con, tx);
            cmd.Parameters.AddWithValue("@id", licenseId);
            cmd.ExecuteNonQuery();
        }

        private List<string> ReadRequestModules(SqlConnection con, int requestId)
        {
            using var cmd = new SqlCommand(@"
SELECT m.ModuleCode
FROM dbo.LicenseRequestModules rm
INNER JOIN dbo.Modules m ON rm.ModuleId = m.Id
WHERE rm.LicenseRequestId = @id", con);
            cmd.Parameters.AddWithValue("@id", requestId);
            using var r = cmd.ExecuteReader();
            var list = new List<string>();
            while (r.Read()) list.Add(r.GetString(0));
            return list;
        }

        private List<string> ReadLicenseModules(SqlConnection con, int licenseId)
        {
            using var cmd = new SqlCommand(@"
SELECT m.ModuleCode
FROM dbo.LicenseModules lm
INNER JOIN dbo.Modules m ON lm.ModuleId = m.Id
WHERE lm.LicenseId = @id", con);
            cmd.Parameters.AddWithValue("@id", licenseId);
            using var r = cmd.ExecuteReader();
            var list = new List<string>();
            while (r.Read()) list.Add(r.GetString(0));
            return list;
        }

        private int GetProductSqlIdForRequest(SqlConnection con, SqlTransaction tx, int requestId)
        {
            using var cmd = new SqlCommand("SELECT ProductID FROM dbo.LicenseRequests WHERE Id = @id", con, tx);
            cmd.Parameters.AddWithValue("@id", requestId);
            var productId = (string)cmd.ExecuteScalar();
            using var cmd2 = new SqlCommand("SELECT Id FROM dbo.Products WHERE ProductID = @pid", con, tx);
            cmd2.Parameters.AddWithValue("@pid", productId);
            return (int)cmd2.ExecuteScalar();
        }

        private int GetProductSqlIdForLicense(SqlConnection con, SqlTransaction tx, int licenseId)
        {
            using var cmd = new SqlCommand("SELECT ProductID FROM dbo.Licenses WHERE Id = @id", con, tx);
            cmd.Parameters.AddWithValue("@id", licenseId);
            var productId = (string)cmd.ExecuteScalar();
            using var cmd2 = new SqlCommand("SELECT Id FROM dbo.Products WHERE ProductID = @pid", con, tx);
            cmd2.Parameters.AddWithValue("@pid", productId);
            return (int)cmd2.ExecuteScalar();
        }

        private List<int> ResolveModuleIds(SqlConnection con, SqlTransaction tx, int productSqlId, IEnumerable<string> moduleCodes)
        {
            var ids = new List<int>();
            using var cmd = new SqlCommand(@"
SELECT Id FROM dbo.Modules WHERE ProductId = @p AND ModuleCode IN (SELECT value FROM STRING_SPLIT(@codes, ','))", con, tx);
            cmd.Parameters.AddWithValue("@p", productSqlId);
            cmd.Parameters.AddWithValue("@codes", string.Join(",", moduleCodes ?? Array.Empty<string>()));
            using var r = cmd.ExecuteReader();
            while (r.Read()) ids.Add(r.GetInt32(0));
            return ids;
        }
    }
}