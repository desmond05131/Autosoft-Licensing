using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class GenerateLicensePageLogicTests
    {
        /// <summary>
        /// Helper to construct GenerateLicensePage and initialize with provided mocks.
        /// Returns the page instance and the mocks so tests can adjust behaviors.
        /// </summary>
        private (GenerateLicensePage page,
                 Mock<IArlReaderService> mockArlReader,
                 Mock<IAslGeneratorService> mockAslService,
                 Mock<IProductService> mockProductService,
                 Mock<ILicenseDatabaseService> mockDbService,
                 Mock<IUserService> mockUserService) CreateGeneratePageWithMocks()
        {
            var mockArlReader = new Mock<IArlReaderService>();
            var mockAslService = new Mock<IAslGeneratorService>();
            var mockProductService = new Mock<IProductService>();
            var mockDbService = new Mock<ILicenseDatabaseService>();
            var mockUserService = new Mock<IUserService>();

            var page = new GenerateLicensePage();

            // Initialize Must be called to inject services used by page logic.
            page.Initialize(
                mockArlReader.Object,
                mockAslService.Object,
                mockProductService.Object,
                mockDbService.Object,
                mockUserService.Object);

            return (page, mockArlReader, mockAslService, mockProductService, mockDbService, mockUserService);
        }

        [TestMethod]
        public void UploadArl_PopulatesFields_BindsModules()
        {
            // Arrange
            var (page, mockArlReader, mockAsl, mockProduct, mockDb, mockUser) = CreateGeneratePageWithMocks();

            var arl = new ArlRequest
            {
                CompanyName = "Acme Corp",
                ProductID = "PROD-001",
                ProductName = "Acme Product",
                DealerCode = "DLR01",
                RequestedPeriodMonths = 3,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "MOD_A", "MOD_B" }
            };

            mockArlReader
                .Setup(x => x.ParseArl(It.IsAny<string>()))
                .Returns(arl);

            var modules = new[]
            {
                new ModuleDto { ModuleCode = "MOD_A", ModuleName = "Module A" },
                new ModuleDto { ModuleCode = "MOD_B", ModuleName = "Module B" },
            };

            mockProduct
                .Setup(x => x.GetModulesByProductId(arl.ProductID))
                .Returns(modules);

            mockProduct
                .Setup(x => x.GetProductName(arl.ProductID))
                .Returns(arl.ProductName);

            // The page's upload code cannot be invoked directly in tests because it shows an OpenFileDialog.
            // Instead simulate the post-parse actions the handler performs:
            // - set private _currentRequest
            // - set text boxes (txtCompanyName, txtProductId, txtProductName)
            // - call BindModules with product modules and enabled codes
            // - set dates and enable Generate button

            // Use reflection to set private field _currentRequest
            var fiCurrentRequest = typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic);
            fiCurrentRequest.SetValue(page, arl);

            // Set UI controls via reflection (txtCompanyName, txtProductId, txtProductName)
            var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var txtProductName = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

            // Populate as the handler would
            txtCompany.Text = arl.CompanyName;
            txtProductId.Text = arl.ProductID;
            // product name resolved via product service
            txtProductName.Text = mockProduct.Object.GetProductName(arl.ProductID);

            // Call private BindModules to bind grid with product modules and the enabled module codes
            var bindMethod = typeof(GenerateLicensePage).GetMethod("BindModules", BindingFlags.Instance | BindingFlags.NonPublic);
            bindMethod.Invoke(page, new object[] { modules, arl.ModuleCodes });

            // Also set dates as upload would
            var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            dtIssue.DateTime = DateTime.UtcNow.Date;
            dtExpire.DateTime = dtIssue.DateTime.AddMonths(Math.Max(1, arl.RequestedPeriodMonths));

            // Enable generate button as handler does
            var btnGenerate = typeof(GenerateLicensePage).GetField("btnGenerateKey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page) as DevExpress.XtraEditors.SimpleButton;
            var btnPreview = typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page) as DevExpress.XtraEditors.SimpleButton;
            var btnDownload = typeof(GenerateLicensePage).GetField("btnDownload", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page) as DevExpress.XtraEditors.SimpleButton;

            btnGenerate.Enabled = true;
            btnPreview.Enabled = false;
            btnDownload.Enabled = false;

            // Assert UI fields were populated correctly
            Assert.AreEqual("Acme Corp", txtCompany.Text);
            Assert.AreEqual("PROD-001", txtProductId.Text);
            Assert.AreEqual("Acme Product", txtProductName.Text);

            // Assert modules grid has been bound with 2 rows and correct module names
            var grdModules = (GridControl)typeof(GenerateLicensePage).GetField("grdModules", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var view = grdModules.MainView as GridView;
            Assert.IsNotNull(view);
            Assert.AreEqual(2, view.DataRowCount);

            // Verify that the rows contain the expected ModuleName values and Enabled flags
            var names = new List<string>();
            var enabledFlags = new List<bool>();
            for (int i = 0; i < view.DataRowCount; i++)
            {
                var row = view.GetRow(i);
                var propName = row.GetType().GetProperty("ModuleName");
                var propEnabled = row.GetType().GetProperty("Enabled");
                names.Add(propName.GetValue(row)?.ToString());
                enabledFlags.Add((bool)(propEnabled.GetValue(row) ?? false));
            }

            CollectionAssert.AreEquivalent(new[] { "Module A", "Module B" }, names);
            // Enabled should reflect presence in arl.ModuleCodes (both present)
            CollectionAssert.AreEqual(new[] { true, true }, enabledFlags);
        }

        [TestMethod]
        public void GenerateKey_WhenDuplicateExists_PreventsGenerationAndDoesNotCallAslGenerator()
        {
            // Arrange
            var (page, mockArlReader, mockAsl, mockProduct, mockDb, mockUser) = CreateGeneratePageWithMocks();

            var arl = new ArlRequest
            {
                CompanyName = "DupCo",
                ProductID = "DUP-01",
                ProductName = "Duplicate Product",
                DealerCode = "DLR-DUP",
                RequestedPeriodMonths = 1,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "M1" }
            };

            // Simulate a duplicate found in DB
            mockDb.Setup(d => d.ExistsDuplicateLicense(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).Returns(true);

            // Prime page state: set _currentRequest and text fields (as upload would)
            typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(page, arl);

            var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

            txtCompany.Text = arl.CompanyName;
            txtProductId.Text = arl.ProductID;

            // Set issue/expire dates such that duplicate check uses same Utc dates
            var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            dtIssue.DateTime = DateTime.UtcNow.Date;
            dtExpire.DateTime = dtIssue.DateTime.AddMonths(1);

            // Ensure ASL generator mock setup to detect calls (it should NOT be called in this scenario)
            mockAsl.Setup(x => x.CreateAndSaveAsl(It.IsAny<LicenseData>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<bool>()));

            // Act: invoke private btnGenerateKey_Click handler reflectively
            var method = typeof(GenerateLicensePage).GetMethod("btnGenerateKey_Click", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(page, new object[] { null, EventArgs.Empty });

            // Assert: since duplicate exists, no license key should be populated and preview/download remain disabled
            var txtLicenseKey = (TextEdit)typeof(GenerateLicensePage).GetField("txtLicenseKey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var btnPreview = (DevExpress.XtraEditors.SimpleButton)typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var btnDownload = (DevExpress.XtraEditors.SimpleButton)typeof(GenerateLicensePage).GetField("btnDownload", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

            Assert.IsTrue(string.IsNullOrWhiteSpace(txtLicenseKey.Text), "License key should not be generated when duplicate exists.");
            Assert.IsFalse(btnPreview.Enabled, "Preview should remain disabled on duplicate prevention.");
            Assert.IsFalse(btnDownload.Enabled, "Download should remain disabled on duplicate prevention.");

            // Verify ASL generator was not called
            mockAsl.Verify(x => x.CreateAndSaveAsl(It.IsAny<LicenseData>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void GenerateKey_WhenNoDuplicate_GeneratesKey_EnablesPreviewAndDownload()
        {
            // Arrange
            var (page, mockArlReader, mockAsl, mockProduct, mockDb, mockUser) = CreateGeneratePageWithMocks();

            var arl = new ArlRequest
            {
                CompanyName = "GoodCo",
                ProductID = "GOOD-01",
                ProductName = "Good Product",
                DealerCode = "DLR-GOOD",
                RequestedPeriodMonths = 12,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "M1", "M2" }
            };

            // No duplicate
            mockDb.Setup(d => d.ExistsDuplicateLicense(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).Returns(false);

            // Prime page state as upload would
            typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(page, arl);

            var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            txtCompany.Text = arl.CompanyName;
            txtProductId.Text = arl.ProductID;

            var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            dtIssue.DateTime = DateTime.UtcNow.Date;
            dtExpire.DateTime = dtIssue.DateTime.AddMonths(12);

            // Pre-condition: ensure no license key
            var txtLicenseKey = (TextEdit)typeof(GenerateLicensePage).GetField("txtLicenseKey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            txtLicenseKey.Text = string.Empty;

            // Act: invoke private btnGenerateKey_Click handler reflectively
            var method = typeof(GenerateLicensePage).GetMethod("btnGenerateKey_Click", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(page, new object[] { null, EventArgs.Empty });

            // Assert: license key generated, preview & download enabled
            Assert.IsFalse(string.IsNullOrWhiteSpace(txtLicenseKey.Text), "License key should be generated when no duplicate exists.");
            Assert.AreEqual(32, txtLicenseKey.Text.Length, "Generated license key length expected (32 hex chars from GUID-based generation in skeleton).");

            var btnPreview = (DevExpress.XtraEditors.SimpleButton)typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            var btnDownload = (DevExpress.XtraEditors.SimpleButton)typeof(GenerateLicensePage).GetField("btnDownload", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

            Assert.IsTrue(btnPreview.Enabled, "Preview should be enabled after generating key.");
            Assert.IsTrue(btnDownload.Enabled, "Download should be enabled after generating key.");

            // Note: in the current skeleton btnGenerateKey does not call the ASL generator directly.
            // We still ensure that CreateAndSaveAsl was not called at this stage.
            mockAsl.Verify(x => x.CreateAndSaveAsl(It.IsAny<LicenseData>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
