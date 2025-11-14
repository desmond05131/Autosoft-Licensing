using System;
using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Models.Enums;

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

            var meta = _db.GetActiveLicense(productId, companyName);
            if (meta == null)
            {
                message = UiMessages.InvalidLicenseFile;
                hidePluginTab = true; // no license -> hide
                return false;
            }

            var now = _clock.UtcNow;
            if (meta.ValidToUtc < now || meta.Status == LicenseStatus.Expired || meta.Status == LicenseStatus.Invalid)
            {
                if (meta.LicenseType == LicenseType.Demo)
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
}