using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Models
{
    // Record stored in DB for active/imported licenses
    public class LicenseMetadata
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ProductID { get; set; }
        public string DealerCode { get; set; }
        public string LicenseKey { get; set; }
        public LicenseType LicenseType { get; set; }
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidToUtc { get; set; }
        public string CurrencyCode { get; set; }
        public LicenseStatus Status { get; set; }
        public DateTime ImportedOnUtc { get; set; }
        public int? ImportedByUserId { get; set; }
        public string RawAslBase64 { get; set; }

        // Stored activated module codes
        public List<string> ModuleCodes { get; set; } = new List<string>();
    }
}
