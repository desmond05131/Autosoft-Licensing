using System;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
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

            if (!_db.TryGetLatestLicenseSummary(productId, companyName, out var type, out var from, out var to, out var status))
            {
                message = UiMessages.InvalidLicenseFile;
                hidePluginTab = true; // no valid license found => hide to be safe
                return false;
            }

            var now = _clock.UtcNow;
            if (to < now || string.Equals(status, "Expired", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(type, "Demo", StringComparison.OrdinalIgnoreCase))
                {
                    message = UiMessages.DemoLicenseExpired;
                    hidePluginTab = true;
                }
                else
                {
                    message = UiMessages.LicenseExpired;
                    hidePluginTab = false;
                }
                return false;
            }

            // Valid path
            message = string.Empty;
            hidePluginTab = false;
            return true;
        }
    }
}