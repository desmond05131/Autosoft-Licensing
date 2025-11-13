using System;

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
        public string LicenseType { get; set; } // Demo / Paid
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidToUtc { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; } // Valid / Expired / Invalid
        public DateTime ImportedOnUtc { get; set; }
        public int? ImportedByUserId { get; set; }
        public string RawAslBase64 { get; set; } // optional
    }
}
