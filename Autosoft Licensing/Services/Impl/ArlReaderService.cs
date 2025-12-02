using System;
using System.Linq;
using System.Collections.Generic;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services.Impl
{
    /// <summary>
    /// Adapter that delegates ARL parsing to the existing ILicenseRequestService implementation.
    /// Keeps a small, UI-friendly interface separate from the core LicenseRequest service contract.
    /// </summary>
    public class ArlReaderService : IArlReaderService
    {
        private readonly ILicenseRequestService _inner;

        public ArlReaderService(ILicenseRequestService inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public ArlRequest ParseArl(string path)
        {
            // Delegate to core license request service which already performs validation and throws
            // the expected ValidationException with the exact message when appropriate.
            var licenseRequest = _inner.ParseArl(path);
            return MapToArlRequest(licenseRequest);
        }

        public ArlRequest ParseArlFromBase64(string base64)
        {
            var licenseRequest = _inner.ParseArlFromBase64(base64);
            return MapToArlRequest(licenseRequest);
        }

        private ArlRequest MapToArlRequest(LicenseRequest licenseRequest)
        {
            if (licenseRequest == null)
                throw new ArgumentNullException(nameof(licenseRequest));

            // IMPORTANT: ignore any ModuleCodes — ARL must not drive module selection.
            return new ArlRequest
            {
                CompanyName = licenseRequest.CompanyName,
                ProductID = licenseRequest.ProductID,
                ProductName = null,
                DealerCode = licenseRequest.DealerCode,
                RequestedPeriodMonths = licenseRequest.RequestedPeriodMonths,
                LicenseType = licenseRequest.LicenseType, // pass through string ("Demo" or "Paid")
                LicenseKey = licenseRequest.LicenseKey,
                CurrencyCode = licenseRequest.CurrencyCode,
                RequestDateUtc = licenseRequest.RequestDateUtc,
                ModuleCodes = Enumerable.Empty<string>() // deliberately empty
            };
        }
    }
}
