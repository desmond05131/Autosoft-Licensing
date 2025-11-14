using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class LicenseDatabaseService : ILicenseDatabaseService
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IClock _clock;

        public LicenseDatabaseService(SqlConnectionFactory connectionFactory, IClock clock)
        {
            _connectionFactory = connectionFactory;
            _clock = clock;
        }

        public int InsertLicenseRequest(LicenseRequest request, IEnumerable<int> moduleIds, int createdByUserId, string requestFileBase64)
        {
            using var conn = _connectionFactory.Create();
            using var tx = conn.BeginTransaction();

            try
            {
                var sql = @"
INSERT INTO dbo.LicenseRequests
(CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId)
VALUES (@CompanyName, @ProductID, @DealerCode, @LicenseType, @RequestedPeriodMonths, @LicenseKey, @CurrencyCode, @RequestDateUtc, @RequestFileBase64, @CreatedByUserId);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using var cmd = new SqlCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@CompanyName", request.CompanyName);
                cmd.Parameters.AddWithValue("@ProductID", request.ProductID);
                cmd.Parameters.AddWithValue("@DealerCode", request.DealerCode);
                cmd.Parameters.AddWithValue("@LicenseType", request.LicenseType);
                cmd.Parameters.AddWithValue("@RequestedPeriodMonths", request.RequestedPeriodMonths);
                cmd.Parameters.AddWithValue("@LicenseKey", request.LicenseKey);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object?)request.CurrencyCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequestDateUtc", request.RequestDateUtc);
                // store payload only if feature enabled; prefer Base64 as per column name
                cmd.Parameters.AddWithValue("@RequestFileBase64",
                    CryptoConstants.StoreRawFiles ? (object?)requestFileBase64 ?? DBNull.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);

                var requestId = (int)cmd.ExecuteScalar();

                if (moduleIds != null)
                {
                    foreach (var moduleId in moduleIds)
                    {
                        var linkSql = "INSERT INTO dbo.LicenseRequestModules (LicenseRequestId, ModuleId) VALUES (@Rid, @Mid);";
                        using var linkCmd = new SqlCommand(linkSql, conn, tx);
                        linkCmd.Parameters.AddWithValue("@Rid", requestId);
                        linkCmd.Parameters.AddWithValue("@Mid", moduleId);
                        linkCmd.ExecuteNonQuery();
                    }
                }

                tx.Commit();
                return requestId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public bool LicenseKeyExists(string licenseKey)
        {
            using var conn = _connectionFactory.Create();
            var sql = "SELECT TOP(1) 1 FROM dbo.Licenses WHERE LicenseKey = @k";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@k", licenseKey);
            var obj = cmd.ExecuteScalar();
            return obj != null;
        }

        public int InsertLicense(LicenseData data, IEnumerable<int> moduleIds, int? importedByUserId, string rawAslBase64)
        {
            using var conn = _connectionFactory.Create();
            using var tx = conn.BeginTransaction();

            try
            {
                var status = data.ValidToUtc >= _clock.UtcNow ? "Valid" : "Expired";

                var sql = @"
INSERT INTO dbo.Licenses
(CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64)
VALUES (@CompanyName, @ProductID, @DealerCode, @LicenseKey, @LicenseType, @ValidFromUtc, @ValidToUtc, @CurrencyCode, @Status, SYSUTCDATETIME(), @ImportedByUserId, @RawAslBase64);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using var cmd = new SqlCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@CompanyName", data.CompanyName);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@DealerCode", data.DealerCode);
                cmd.Parameters.AddWithValue("@LicenseKey", data.LicenseKey);
                cmd.Parameters.AddWithValue("@LicenseType", data.LicenseType);
                cmd.Parameters.AddWithValue("@ValidFromUtc", data.ValidFromUtc);
                cmd.Parameters.AddWithValue("@ValidToUtc", data.ValidToUtc);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object?)data.CurrencyCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ImportedByUserId", (object?)importedByUserId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawAslBase64", CryptoConstants.StoreRawFiles ? (object?)rawAslBase64 ?? DBNull.Value : DBNull.Value);

                var licenseId = (int)cmd.ExecuteScalar();

                if (moduleIds != null)
                {
                    foreach (var moduleId in moduleIds)
                    {
                        var linkSql = "INSERT INTO dbo.LicenseModules (LicenseId, ModuleId) VALUES (@Lid, @Mid);";
                        using var linkCmd = new SqlCommand(linkSql, conn, tx);
                        linkCmd.Parameters.AddWithValue("@Lid", licenseId);
                        linkCmd.Parameters.AddWithValue("@Mid", moduleId);
                        linkCmd.ExecuteNonQuery();
                    }
                }

                tx.Commit();
                return licenseId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void UpdateLicenseStatus(int licenseId, string status)
        {
            using var conn = _connectionFactory.Create();
            var sql = "UPDATE dbo.Licenses SET Status=@s WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", licenseId);
            cmd.ExecuteNonQuery();
        }

        public bool TryGetLatestLicenseSummary(string productId, string companyName, out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status)
        {
            licenseType = string.Empty;
            validFromUtc = DateTime.MinValue;
            validToUtc = DateTime.MinValue;
            status = string.Empty;

            using var conn = _connectionFactory.Create();
            var sql = @"
SELECT TOP(1) LicenseType, ValidFromUtc, ValidToUtc, Status
FROM dbo.Licenses
WHERE ProductID=@pid AND CompanyName=@cn
ORDER BY ValidToUtc DESC, Id DESC;";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@cn", companyName);

            using var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!rdr.Read()) return false;

            licenseType = rdr.GetString(0);
            validFromUtc = rdr.GetDateTime(1);
            validToUtc = rdr.GetDateTime(2);
            status = rdr.GetString(3);
            return true;
        }
    }
}