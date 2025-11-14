using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseDatabaseService
    {
        // Requests
        int InsertLicenseRequest(LicenseRequest request, IEnumerable<int> moduleIds, int createdByUserId, string requestFileBase64);

        // Licenses
        bool LicenseKeyExists(string licenseKey);
        int InsertLicense(LicenseData data, IEnumerable<int> moduleIds, int? importedByUserId, string rawAslBase64);
        void UpdateLicenseStatus(int licenseId, string status);
        // Minimal check used at startup or dashboard
        bool TryGetLatestLicenseSummary(string productId, string companyName, out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status);
    }
}