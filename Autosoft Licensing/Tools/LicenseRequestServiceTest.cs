using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.Services;
using Autosoft_Licensing;
using Autosoft_Licensing.Services.Impl;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class LicenseRequestServiceTests
    {
        [TestMethod]
        public void ParseArlFromBase64_InvalidBase64_ThrowsValidationException_WithExpectedMessage()
        {
            // Arrange
            var svc = new LicenseRequestService(ServiceRegistry.Validation);

            // Act & Assert
            try
            {
                svc.ParseArlFromBase64("this-is-not-base64!!");
                Assert.Fail("Expected ValidationException was not thrown.");
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual("Invalid license request file.", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected exception type thrown: " + ex.GetType().FullName);
            }
        }

        [TestMethod]
        public void ParseArlFromBase64_MissingRequiredFields_ThrowsValidationException()
        {
            var svc = new LicenseRequestService(ServiceRegistry.Validation);

            // JSON missing ProductID
            var json = @"{
                ""CompanyName"": ""Acme"",
                ""RequestedPeriodMonths"": 1,
                ""DealerCode"": ""D01"",
                ""LicenseType"": ""Demo"",
                ""LicenseKey"": ""K1"",
                ""RequestDateUtc"": ""2025-12-01T00:00:00Z""
            }";
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            try
            {
                svc.ParseArlFromBase64(base64);
                Assert.Fail("Expected ValidationException was not thrown for missing field.");
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual("Invalid license request file.", ex.Message);
            }
        }

        [TestMethod]
        public void ParseArlFromBase64_InvalidLicenseType_ThrowsValidationException()
        {
            var svc = new LicenseRequestService(ServiceRegistry.Validation);

            var json = @"{
                ""CompanyName"": ""Acme"",
                ""RequestedPeriodMonths"": 1,
                ""DealerCode"": ""D01"",
                ""ProductID"": ""P01"",
                ""LicenseType"": ""Trial"",
                ""LicenseKey"": ""K1"",
                ""RequestDateUtc"": ""2025-12-01T00:00:00Z""
            }";
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            try
            {
                svc.ParseArlFromBase64(base64);
                Assert.Fail("Expected ValidationException for invalid LicenseType.");
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual("Invalid license request file.", ex.Message);
            }
        }

        [TestMethod]
        public void ParseArlFromBase64_DemoWithMonthsNotOne_ThrowsValidationException()
        {
            var svc = new LicenseRequestService(ServiceRegistry.Validation);

            var json = @"{
                ""CompanyName"": ""Acme"",
                ""RequestedPeriodMonths"": 3,
                ""DealerCode"": ""D01"",
                ""ProductID"": ""P01"",
                ""LicenseType"": ""Demo"",
                ""LicenseKey"": ""K1"",
                ""RequestDateUtc"": ""2025-12-01T00:00:00Z""
            }";
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            try
            {
                svc.ParseArlFromBase64(base64);
                Assert.Fail("Expected ValidationException for Demo with months != 1.");
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual("Invalid license request file.", ex.Message);
            }
        }

        [TestMethod]
        public void ArlReader_ParseArlFromBase64_ModuleCodesInArl_IgnoredByAdapter()
        {
            // Use LicenseRequestService + ArlReaderService to ensure adapter ignores ModuleCodes
            var licenseSvc = new LicenseRequestService(ServiceRegistry.Validation);
            var arlReader = new ArlReaderService(licenseSvc);

            var json = @"{
                ""CompanyName"": ""Acme"",
                ""RequestedPeriodMonths"": 1,
                ""DealerCode"": ""D01"",
                ""ProductID"": ""P01"",
                ""LicenseType"": ""Demo"",
                ""LicenseKey"": ""K1"",
                ""CurrencyCode"": ""USD"",
                ""RequestDateUtc"": ""2025-12-01T00:00:00Z"",
                ""ModuleCodes"": [""MOD1"", ""MOD2""]
            }";
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            var arl = arlReader.ParseArlFromBase64(base64);

            // Adapter must ignore ModuleCodes and return empty collection
            Assert.IsNotNull(arl);
            CollectionAssert.AreEqual(new string[0], new System.Collections.Generic.List<string>(arl.ModuleCodes ?? System.Linq.Enumerable.Empty<string>()).ToArray());
        }
    }
}
