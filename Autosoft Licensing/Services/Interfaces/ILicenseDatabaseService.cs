using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models;

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
}