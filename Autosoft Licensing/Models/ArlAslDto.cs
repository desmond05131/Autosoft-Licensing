using System;
using System.Collections.Generic;

namespace Autosoft_Licensing.Models
{
    // Minimal DTO representing a parsed .ARL request
    public class ArlRequest
    {
        public string CompanyName { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }

        // Added DealerCode - ARL contract includes DealerCode (used by Generate page)
        public string DealerCode { get; set; }

        public int RequestedPeriodMonths { get; set; }
        public string LicenseType { get; set; }
        public string LicenseKey { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public IEnumerable<string> ModuleCodes { get; set; }
    }

    // Minimal DTO representing the in-memory payload before encryption (.ASL)
    // NOTE: the UI now uses the canonical LicenseData for ASL generation, so this type
    // remains for any lightweight adapters but is not required by the Generate page.
    public class AslPayload
    {
        public string CompanyName { get; set; }
        public string ProductID { get; set; }
        public string DealerCode { get; set; }
        public string LicenseKey { get; set; }
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidToUtc { get; set; }
        public IEnumerable<string> ModuleCodes { get; set; }
        public string LicenseType { get; set; }
    }
}
