using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting; // Make sure you have the MSTest NuGet package installed
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.Services;
using Autosoft_Licensing;

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
    }
}
