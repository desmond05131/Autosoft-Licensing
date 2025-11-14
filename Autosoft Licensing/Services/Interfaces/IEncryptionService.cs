using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;
using System.Data.SqlClient;
using System.Configuration;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseDatabaseService
    {
        // Users
        User GetUserByUsername(string username);
        User GetUserById(int id);

        // License Requests
        int InsertLicenseRequest(LicenseRequestRecord record);
        LicenseRequestRecord GetLicenseRequestById(int id);
        IEnumerable<LicenseRequestRecord> GetLicenseRequests(string productId = null);

        // Licenses
        int InsertLicense(LicenseMetadata meta);
        LicenseMetadata GetLicenseById(int id);
        LicenseMetadata GetActiveLicense(string productId, string companyName);
        IEnumerable<LicenseMetadata> GetLicenses(string productId = null);

        // Modules association
        void SetRequestModules(int requestId, IEnumerable<string> moduleCodes);
        void SetLicenseModules(int licenseId, IEnumerable<string> moduleCodes);

        // Duplicate license key check
        bool LicenseKeyExists(string licenseKey);

        // Latest summary for validation at startup
        bool TryGetLatestLicenseSummary(string productId, string companyName,
            out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status);
    }

    public sealed class LicenseValidationFacade : ILicenseValidationFacade
    {
        private readonly ILicenseDatabaseService _db;
        private readonly IClock _clock;

        public LicenseValidationFacade(ILicenseDatabaseService db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public bool IsLicenseValid(string productId, string companyName, out string message, out bool hidePluginTab)
        {
            message = string.Empty;
            hidePluginTab = false;

            if (!_db.TryGetLatestLicenseSummary(productId, companyName,
                out var type, out var from, out var to, out var status))
            {
                message = UiMessages.InvalidLicenseFile;
                hidePluginTab = true;
                return false;
            }

            var now = _clock.UtcNow;
            if (to < now || string.Equals(status, "Expired", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(type, "Demo", StringComparison.OrdinalIgnoreCase))
                {
                    message = UiMessages.DemoExpired;
                    hidePluginTab = true;
                }
                else
                {
                    message = UiMessages.LicenseExpired;
                    hidePluginTab = false;
                }
                return false;
            }

            return true;
        }
    }

    public interface IEncryptionService
    {
        string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv);
        string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv);
        string ComputeSha256Hex(byte[] data);
        bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex);

        // Build canonical JSON (sorted) with SHA-256 checksum injected
        string BuildJsonWithChecksum(LicenseData licenseWithoutChecksum);
    }
}