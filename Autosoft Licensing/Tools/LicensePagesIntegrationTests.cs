// TASK: Create robust integration tests for LicenseRecordsPage and LicenseRecordDetailsPage.
// CONTEXT: WinForms app using DevExpress, ServiceRegistry for DI, and UiTestHost for threading.
//
// INSTRUCTIONS:
// 1. Create a new class "LicensePagesIntegrationTests" in namespace "Autosoft_Licensing.Tools".
// 2. Use "InMemoryLicenseDatabaseService" to seed data (Users, Products, Licenses).
// 3. Use "UiTestHost" to run UI logic on the correct thread.
// 4. Since UI controls (like grdLicenses, txtCompanyName) are private, use Reflection to access them.
// 5. Implement the following test methods:
//
//    TEST 1: RecordsPage_LoadsData_And_Filters
//      - Setup: Seed 2 active licenses (CompanyA, CompanyB).
//      - Act: Load LicenseRecordsPage.
//      - Assert: Grid has 2 rows.
//      - Act: Filter CompanyName to "CompanyA" and click Refresh.
//      - Assert: Grid has 1 row.
//
//    TEST 2: RecordsPage_Delete_AdminOnly
//      - Setup: Seed an admin user and a support user.
//      - Act: Initialize page with Support user.
//      - Assert: "btnDelete" is Disabled.
//      - Act: Initialize page with Admin user.
//      - Assert: "btnDelete" is Enabled.
//
//    TEST 3: DetailsPage_Populates_Correctly
//      - Setup: Seed a license with specific Modules and Validity dates.
//      - Act: Create LicenseRecordDetailsPage and call Initialize(licenseId).
//      - Assert: txtCompanyName, txtProductCode, dtIssueDate match seed data.
//      - Assert: grdModules has correct row count.
//
//    TEST 4: DetailsPage_VerifyChecksum_Flow
//      - Setup: Use real EncryptionService to generate a valid ASL string for a seeded license.
//      - Store this ASL in the seeded LicenseMetadata.RawAslBase64.
//      - Act: Load details page, click "btnVerifyChecksum".
//      - Assert: lblChecksumStatus text contains "Valid" or color is Green.
//
// 6. Add a helper method "GetPrivateControl<T>(Control parent, string name)" to simplify reflection access.
// 7. Ensure ServiceRegistry is properly initialized and reset in a [TestInitialize] / [TestCleanup] block.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;
using Autosoft_Licensing.Tests.Helpers;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class LicensePagesIntegrationTests
    {
        private UiTestHost _ui;
        private InMemoryLicenseDatabaseService _db;

        [TestInitialize]
        public void Init()
        {
            // Initialize in-memory DB and wire into ServiceRegistry BEFORE creating any pages.
            _db = new InMemoryLicenseDatabaseService();

            // Reset ServiceRegistry singletons that cache DB-backed services (Product, KeyGen, AslGen, ArlReader)
            ResetServiceRegistryCaches();

            ServiceRegistry.Database = _db;

            // Create UI host (STA thread + message loop)
            _ui = new UiTestHost("TestsHost");
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { _ui?.Dispose(); } catch { /* ignore */ }
            _ui = null;

            // Reset ServiceRegistry to avoid leaking state across test runs
            try
            {
                ResetServiceRegistryCaches();
                // Also clear the backing Database field to avoid accidental reuse
                var backingDb = typeof(ServiceRegistry).GetField("_database", BindingFlags.Static | BindingFlags.NonPublic);
                backingDb?.SetValue(null, null);
            }
            catch { /* ignore */ }
        }

        // TEST 1: RecordsPage_LoadsData_And_Filters
        [TestMethod]
        public void RecordsPage_LoadsData_And_Filters()
        {
            // Seed two active licenses (future expiry so not filtered out)
            var vf = DateTime.UtcNow.Date.AddDays(-1);
            var vt = DateTime.UtcNow.Date.AddDays(60);

            var l1 = new LicenseMetadata
            {
                CompanyName = "CompanyA",
                ProductID = "SAMPLE-PRODUCT",
                DealerCode = "DEALER-001",
                LicenseKey = "KEY-A",
                LicenseType = LicenseType.Subscription,
                ValidFromUtc = vf,
                ValidToUtc = vt,
                CurrencyCode = "MYR",
                Status = LicenseStatus.Valid,
                ModuleCodes = new List<string> { "MODULE-001" }
            };
            _db.InsertLicense(l1);

            var l2 = new LicenseMetadata
            {
                CompanyName = "CompanyB",
                ProductID = "SAMPLE-PRODUCT",
                DealerCode = "DEALER-001",
                LicenseKey = "KEY-B",
                LicenseType = LicenseType.Subscription,
                ValidFromUtc = vf,
                ValidToUtc = vt,
                CurrencyCode = "MYR",
                Status = LicenseStatus.Valid,
                ModuleCodes = new List<string> { "MODULE-002" }
            };
            _db.InsertLicense(l2);

            LicenseRecordsPage page = null;
            GridControl grid = null;
            ComboBoxEdit cmbCompany = null;
            SimpleButton btnRefresh = null;

            _ui.Invoke(() =>
            {
                page = new LicenseRecordsPage();
                _ui.ShowControl(page);

                grid = GetPrivateControl<GridControl>(page, "grdLicenses");
                cmbCompany = GetPrivateControl<ComboBoxEdit>(page, "cmbCompanyName");
                btnRefresh = GetPrivateControl<SimpleButton>(page, "btnRefresh");
            });

            // Assert grid has 2 rows initially
            var initialCount = _ui.Invoke(() => GetGridView(grid).RowCount);
            Assert.AreEqual(2, initialCount, "Initial grid row count should be 2.");

            // Filter by CompanyA and refresh
            _ui.Invoke(() =>
            {
                // Select "CompanyA" in the combo
                var idx = -1;
                for (int i = 0; i < cmbCompany.Properties.Items.Count; i++)
                {
                    var itemText = Convert.ToString(cmbCompany.Properties.Items[i]);
                    if (string.Equals(itemText, "CompanyA", StringComparison.OrdinalIgnoreCase))
                    {
                        idx = i;
                        break;
                    }
                }
                Assert.IsTrue(idx >= 0, "CompanyA option should be present in the CompanyName filter.");
                cmbCompany.SelectedIndex = idx;

                // Click Refresh
                btnRefresh.PerformClick();
                Application.DoEvents();
            });

            var filteredCount = _ui.Invoke(() => GetGridView(grid).RowCount);
            Assert.AreEqual(1, filteredCount, "Filtered grid row count should be 1 after selecting CompanyA.");
        }

        // TEST 2: RecordsPage_Delete_AdminOnly
        [TestMethod]
        public void RecordsPage_Delete_AdminOnly()
        {
            // InMemory DB already seeds an Admin user (username: admin, Role: Admin)
            var admin = _db.GetUserByUsername("admin");
            Assert.IsNotNull(admin, "Seeded admin user should exist.");

            // Seed a support user
            var support = new User
            {
                Username = "support",
                DisplayName = "Support User",
                Role = "Support",
                Email = "support@example.com",
                PasswordHash = "dummy",
                CreatedUtc = DateTime.UtcNow
            };
            support.Id = _db.InsertUser(support);

            LicenseRecordsPage page = null;
            SimpleButton btnDelete = null;

            _ui.Invoke(() =>
            {
                page = new LicenseRecordsPage();
                _ui.ShowControl(page);

                btnDelete = GetPrivateControl<SimpleButton>(page, "btnDelete");

                // Initialize for Support role -> Delete should be disabled
                page.InitializeForRole(support);
            });

            var supportCanDelete = _ui.Invoke(() => btnDelete.Enabled);
            Assert.IsFalse(supportCanDelete, "Support user should not be able to delete.");

            _ui.Invoke(() =>
            {
                // Initialize for Admin role -> Delete should be enabled
                page.InitializeForRole(admin);
            });

            var adminCanDelete = _ui.Invoke(() => btnDelete.Enabled);
            Assert.IsTrue(adminCanDelete, "Admin user should be able to delete.");
        }

        // TEST 3: DetailsPage_Populates_Correctly
        [TestMethod]
        public void DetailsPage_Populates_Correctly()
        {
            // Seed a license with modules and specific dates (use SAMPLE-PRODUCT; modules pre-seeded in InMemory service)
            var issueUtc = new DateTime(DateTime.UtcNow.Year, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var expiryUtc = issueUtc.AddMonths(6);

            var meta = new LicenseMetadata
            {
                CompanyName = "Contoso Ltd",
                ProductID = "SAMPLE-PRODUCT",
                DealerCode = "DEALER-001",
                LicenseKey = "CONTOSO-001",
                LicenseType = LicenseType.Subscription,
                ValidFromUtc = issueUtc,
                ValidToUtc = expiryUtc,
                CurrencyCode = "USD",
                Status = LicenseStatus.Valid,
                ModuleCodes = new List<string> { "MODULE-001", "MODULE-002" }
            };
            var licenseId = _db.InsertLicense(meta);

            LicenseRecordDetailsPage page = null;
            TextEdit txtCompany = null;
            TextEdit txtProductCode = null;
            DateEdit dtIssue = null;
            GridControl grdModules = null;

            _ui.Invoke(() =>
            {
                page = new LicenseRecordDetailsPage();
                _ui.ShowControl(page);

                // Initialize page with license id
                page.Initialize(licenseId);

                // Access private controls
                txtCompany = GetPrivateControl<TextEdit>(page, "txtCompanyName");
                txtProductCode = GetPrivateControl<TextEdit>(page, "txtProductCode");
                dtIssue = GetPrivateControl<DateEdit>(page, "dtIssueDate");
                grdModules = GetPrivateControl<GridControl>(page, "grdModules");
            });

            var actualCompany = _ui.Invoke(() => txtCompany.Text);
            var actualProductCode = _ui.Invoke(() => txtProductCode.Text);
            var actualIssueDate = _ui.Invoke(() => dtIssue.DateTime.Date);
            var moduleRowCount = _ui.Invoke(() => GetGridView(grdModules)?.RowCount ?? 0);

            Assert.AreEqual("Contoso Ltd", actualCompany, "Company name mismatch.");
            Assert.AreEqual("SAMPLE-PRODUCT", actualProductCode, "Product code mismatch.");
            Assert.AreEqual(issueUtc.ToLocalTime().Date, actualIssueDate, "Issue date mismatch.");
            Assert.AreEqual(2, moduleRowCount, "Modules grid row count mismatch.");
        }

        // TEST 4: DetailsPage_VerifyChecksum_Flow
        [TestMethod]
        public void DetailsPage_VerifyChecksum_Flow()
        {
            // Build a LicenseData and encrypt to a valid ASL so details page can verify checksum
            var issueUtc = DateTime.UtcNow.Date.AddDays(-5);
            var expiryUtc = DateTime.UtcNow.Date.AddMonths(1);

            var data = new LicenseData
            {
                CompanyName = "Tailspin Toys",
                ProductID = "SAMPLE-PRODUCT",
                DealerCode = "DEALER-001",
                CurrencyCode = "USD",
                LicenseType = LicenseType.Subscription,
                ValidFromUtc = issueUtc,
                ValidToUtc = expiryUtc,
                LicenseKey = "TAIL-KEY-001",
                ModuleCodes = new List<string> { "MODULE-001", "MODULE-003" }
            };

            // Compute canonical JSON with checksum, then encrypt to ASL using real EncryptionService
            var jsonWithChecksum = ServiceRegistry.Encryption.BuildJsonWithChecksum(data);
            var aslBase64 = ServiceRegistry.Encryption.EncryptJsonToAsl(jsonWithChecksum, CryptoConstants.AesKey, CryptoConstants.AesIV);

            // Persist metadata with ASL stored (must be present for details page to enable verification)
            var meta = new LicenseMetadata
            {
                CompanyName = data.CompanyName,
                ProductID = data.ProductID,
                DealerCode = data.DealerCode,
                LicenseKey = data.LicenseKey,
                LicenseType = data.LicenseType,
                ValidFromUtc = data.ValidFromUtc,
                ValidToUtc = data.ValidToUtc,
                CurrencyCode = data.CurrencyCode,
                Status = LicenseStatus.Valid,
                ImportedOnUtc = DateTime.UtcNow,
                ImportedByUserId = 1,
                RawAslBase64 = aslBase64,
                ModuleCodes = new List<string>(data.ModuleCodes)
            };
            var licenseId = _db.InsertLicense(meta);

            LicenseRecordDetailsPage page = null;
            SimpleButton btnVerify = null;
            LabelControl lblStatus = null;

            _ui.Invoke(() =>
            {
                page = new LicenseRecordDetailsPage();
                _ui.ShowControl(page);
                page.Initialize(licenseId);

                btnVerify = GetPrivateControl<SimpleButton>(page, "btnVerifyChecksum");
                lblStatus = GetPrivateControl<LabelControl>(page, "lblChecksumStatus");

                Assert.IsTrue(btnVerify.Enabled, "Verify Checksum button should be enabled when ASL contains a checksum.");
                btnVerify.PerformClick();
                Application.DoEvents();
            });

            var statusText = _ui.Invoke(() => lblStatus.Text ?? string.Empty);
            var statusColor = _ui.Invoke(() => lblStatus.Appearance.ForeColor);

            // Accept either the exact text or the green color as success indicator per instructions
            Assert.IsTrue(statusText.IndexOf("Valid", StringComparison.OrdinalIgnoreCase) >= 0 || statusColor == Color.Green,
                $"Checksum status should indicate Valid. Actual: '{statusText}', Color: {statusColor}.");
        }

        // ---------- Helpers ----------

        private static GridView GetGridView(GridControl grid)
        {
            return grid?.MainView as GridView;
        }

        // Helper to fetch private designer fields or by Name recursively
        private static T GetPrivateControl<T>(Control parent, string name) where T : class
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            // 1) Try private field on the control type (Designer-generated fields)
            var type = parent.GetType();
            while (type != null)
            {
                var fi = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fi != null)
                {
                    var val = fi.GetValue(parent) as T;
                    if (val != null) return val;
                }
                type = type.BaseType;
            }

            // 2) Fallback: walk Controls tree by Name (recursive)
            var found = FindByNameRecursive(parent, name) as T;
            if (found != null) return found;

            throw new InvalidOperationException($"Control '{name}' of type '{typeof(T).Name}' not found on '{parent.GetType().Name}'.");
        }

        private static Control FindByNameRecursive(Control root, string name)
        {
            foreach (Control c in root.Controls)
            {
                if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
                    return c;
                var sub = FindByNameRecursive(c, name);
                if (sub != null) return sub;
            }
            return null;
        }

        private static void ResetServiceRegistryCaches()
        {
            // Null out private static cached services so each test starts clean and binds to current DB
            var t = typeof(ServiceRegistry);
            TrySetStaticFieldNull(t, "_product");
            TrySetStaticFieldNull(t, "_keyGen");
            TrySetStaticFieldNull(t, "_aslGen");
            TrySetStaticFieldNull(t, "_arlReader");
        }

        private static void TrySetStaticFieldNull(Type t, string fieldName)
        {
            try
            {
                var f = t.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
                f?.SetValue(null, null);
            }
            catch { /* ignore */ }
        }
    }
}
