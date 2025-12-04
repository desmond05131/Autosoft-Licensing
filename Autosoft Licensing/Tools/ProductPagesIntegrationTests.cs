// TASK: Create integration tests for ManageProductPage and ProductDetailsPage.
// CONTEXT: WinForms app, DevExpress controls, ServiceRegistry pattern, UiTestHost available.
//
// INSTRUCTIONS:
// 1. Create a new class "ProductPagesIntegrationTests" in namespace "Autosoft_Licensing.Tools".
// 2. Setup [TestInitialize] to reset ServiceRegistry, create InMemoryLicenseDatabaseService, and start UiTestHost.
// 3. Include the Reflection helpers (GetPrivateControl, etc.) to access private UI elements.
//
// IMPLEMENT THE FOLLOWING TESTS:
//
// TEST 1: Backend_Persistence_Check
//    - Purpose: Verify the Service layer handles parent-child saving (Product + Modules).
//    - Action: Create a Product model with 2 Modules. Call _db.InsertProduct(p).
//    - Assert: Call _db.GetModulesForProduct(p.ProductID).Count() should be 2.
//    - Note: If this fails, the UI tests will likely fail too.
//
// TEST 2: ManagePage_Search_And_Role
//    - Setup: Seed 2 products: "Alpha System" (ID: A1) and "Beta System" (ID: B1).
//    - Act: Load ManageProductPage.
//    - Assert: Grid has 2 rows.
//    - Act: Set Search text to "Alpha". Click Search/Refresh.
//    - Assert: Grid has 1 row.
//    - Act: Set User to "Support". Verify "btnDelete" is Disabled.
//    - Act: Set User to "Admin". Verify "btnDelete" is Enabled.
//
// TEST 3: DetailsPage_Create_Flow
//    - Act: Load ProductDetailsPage. Initialize(null) for Create mode.
//    - Action: Set txtProductId="NEW-001", txtProductName="New App".
//    - Action: Click "btnAdd".
//    - Logic: Access the GridControl.DataSource (cast to BindingList or List) and modify the added row to set ModuleName="Mod1".
//    - Action: Click "btnSave".
//    - Assert: Query _db.GetProductByProductId("NEW-001"). Assert it exists.
//    - Assert: Query _db.GetModulesForProduct("NEW-001"). Assert count is 1.
//
// TEST 4: DetailsPage_Edit_Flow
//    - Setup: Seed Product "OLD-001" with 1 module "OldMod".
//    - Act: Load ProductDetailsPage. Initialize(id).
//    - Assert: Grid has 1 row.
//    - Action: Click "btnMinus" (remove module).
//    - Action: Click "btnSave".
//    - Assert: Query _db.GetModulesForProduct("OLD-001"). Assert count is 0.
//
// IMPORTANT IMPLEMENTATION DETAILS:
// - When interacting with the GridControl in tests, prefer casting grid.DataSource to dynamic/IList to verify data, 
//   as visual row handles might not fully render in the test thread context.
// - Ensure ServiceRegistry.Product is initialized in Setup (new ProductService(_db)).
// - Handle null checks strictly in Reflection helpers.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Tests.Helpers;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class ProductPagesIntegrationTests
    {
        private UiTestHost _ui;
        private InMemoryLicenseDatabaseService _db;

        [TestInitialize]
        public void Init()
        {
            _db = new InMemoryLicenseDatabaseService();

            // Reset ServiceRegistry caches and bind fresh DB
            ResetServiceRegistryCaches();
            ServiceRegistry.Database = _db;

            // Ensure Product service is initialized against current DB
            // Accessing the lazy property will create ProductService(Database)
            var _ = ServiceRegistry.Product;

            _ui = new UiTestHost("ProductsTestsHost");
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { _ui?.Dispose(); } catch { /* ignore */ }
            _ui = null;

            try
            {
                ResetServiceRegistryCaches();
                var backingDb = typeof(ServiceRegistry).GetField("_database", BindingFlags.Static | BindingFlags.NonPublic);
                backingDb?.SetValue(null, null);
            }
            catch { /* ignore */ }
        }

        // TEST 1: Backend_Persistence_Check
        [TestMethod]
        public void Backend_Persistence_Check()
        {
            var p = new Product
            {
                ProductID = "CHK-001",
                Name = "Check Product",
                CreatedBy = "tester",
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow,
                Modules = new List<Autosoft_Licensing.Models.Module>
                {
                    new Autosoft_Licensing.Models.Module { ModuleCode = "M1", Name = "Module One", Description = "Desc1", IsActive = true },
                    new Autosoft_Licensing.Models.Module { ModuleCode = "M2", Name = "Module Two", Description = "Desc2", IsActive = true }
                }
            };

            var newId = _db.InsertProduct(p);
            Assert.IsTrue(newId > 0, "InsertProduct should return a new Id.");

            var modules = _db.GetModulesForProduct(p.ProductID) ?? Enumerable.Empty<ModuleDto>();
            Assert.AreEqual(2, modules.Count(), "Service layer should persist 2 modules for the product.");
        }

        // TEST 2: ManagePage_Search_And_Role
        [TestMethod]
        public void ManagePage_Search_And_Role()
        {
            // Seed two products
            _db.InsertProduct(new Product
            {
                ProductID = "A1",
                Name = "Alpha System",
                CreatedBy = "seed",
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
            });
            _db.InsertProduct(new Product
            {
                ProductID = "B1",
                Name = "Beta System",
                CreatedBy = "seed",
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
            });

            // Remove pre-seeded SAMPLE-PRODUCT so grid shows only the two seeded rows
            var sample = _db.GetProductByProductId("SAMPLE-PRODUCT");
            if (sample != null) _db.DeleteProduct(sample.Id);

            ManageProductPage page = null;
            GridControl grd = null;
            ButtonEdit txtSearch = null;
            SimpleButton btnDelete = null;

            _ui.Invoke(() =>
            {
                page = new ManageProductPage();
                page.Initialize(ServiceRegistry.Database);

                _ui.ShowControl(page);

                grd = GetPrivateControl<GridControl>(page, "grdProducts");
                txtSearch = GetPrivateControl<ButtonEdit>(page, "txtSearch");
                btnDelete = GetPrivateControl<SimpleButton>(page, "btnDelete");
            });

            // Assert grid has 2 rows initially
            var initialCount = _ui.Invoke(() => GetGridView(grd)?.RowCount ?? 0);
            Assert.AreEqual(2, initialCount, "Manage grid should show 2 rows for seeded products.");

            // Filter by "Alpha"
            _ui.Invoke(() =>
            {
                txtSearch.Text = "Alpha";
                var button = txtSearch.Properties?.Buttons != null && txtSearch.Properties.Buttons.Count > 0
                    ? txtSearch.Properties.Buttons[0]
                    : null;
                Assert.IsNotNull(button, "Search ButtonEdit should have at least one button.");
                txtSearch.PerformClick(button);
                Application.DoEvents();
            });

            var filteredCount = _ui.Invoke(() => GetGridView(grd)?.RowCount ?? 0);
            Assert.AreEqual(1, filteredCount, "Manage grid should filter to 1 row for 'Alpha'.");

            // Role checks
            var support = new User { Id = 2, Username = "support", DisplayName = "Support User", Role = "Support" };
            var admin = _db.GetUserByUsername("admin");
            Assert.IsNotNull(admin, "Seeded admin user should exist.");

            _ui.Invoke(() =>
            {
                page.InitializeForRole(support);
            });

            var canDeleteSupport = _ui.Invoke(() => btnDelete.Enabled);
            Assert.IsFalse(canDeleteSupport, "Support role should not have Delete permission.");

            _ui.Invoke(() =>
            {
                page.InitializeForRole(admin);
            });

            var canDeleteAdmin = _ui.Invoke(() => btnDelete.Enabled);
            Assert.IsTrue(canDeleteAdmin, "Admin role should have Delete permission.");
        }

        // TEST 3: DetailsPage_Create_Flow
        [TestMethod]
        public void DetailsPage_Create_Flow()
        {
            ProductDetailsPage page = null;
            TextEdit txtProductId = null;
            TextEdit txtProductName = null;
            GridControl grdModules = null;
            SimpleButton btnAdd = null;
            SimpleButton btnSave = null;

            _ui.Invoke(() =>
            {
                page = new ProductDetailsPage();
                _ui.ShowControl(page);

                page.Initialize(null, ServiceRegistry.Database, ServiceRegistry.Product);

                txtProductId = GetPrivateControl<TextEdit>(page, "txtProductId");
                txtProductName = GetPrivateControl<TextEdit>(page, "txtProductName");
                grdModules = GetPrivateControl<GridControl>(page, "grdModules");
                btnAdd = GetPrivateControl<SimpleButton>(page, "btnAdd");
                btnSave = GetPrivateControl<SimpleButton>(page, "btnSave");

                // Fill product fields
                txtProductId.Text = "NEW-001";
                txtProductName.Text = "New App";

                // Add a module row
                btnAdd.PerformClick();
                Application.DoEvents();

                // Set ModuleName = "Mod1" via DataSource
                var ds = grdModules.DataSource as IEnumerable;
                Assert.IsNotNull(ds, "Modules grid DataSource should be enumerable.");
                var list = grdModules.DataSource as IList;
                if (list == null)
                {
                    var tmp = new List<object>();
                    foreach (var item in ds) tmp.Add(item);
                    list = tmp;
                }

                Assert.IsTrue(list.Count >= 1, "Modules list should contain the newly added row.");

                var row = list[0];
                SetPropertyIfExists(row, "ModuleName", "Mod1");
            });

            _ui.Invoke(() =>
            {
                btnSave.PerformClick();
                Application.DoEvents();
            });

            var savedProduct = _db.GetProductByProductId("NEW-001");
            Assert.IsNotNull(savedProduct, "Product NEW-001 should be persisted.");

            var savedModules = _db.GetModulesForProduct("NEW-001") ?? Enumerable.Empty<ModuleDto>();
            Assert.AreEqual(1, savedModules.Count(), "NEW-001 should have exactly 1 module persisted.");
        }

        // TEST 4: DetailsPage_Edit_Flow
        [TestMethod]
        public void DetailsPage_Edit_Flow()
        {
            // Seed product OLD-001 with 1 module
            var prod = new Product
            {
                ProductID = "OLD-001",
                Name = "Old App",
                CreatedBy = "seed",
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow,
                Modules = new List<Autosoft_Licensing.Models.Module>
                {
                    new Autosoft_Licensing.Models.Module { ModuleCode = "OLDMOD", Name = "OldMod", Description = "Legacy", IsActive = true }
                }
            };
            var id = _db.InsertProduct(prod);

            ProductDetailsPage page = null;
            GridControl grdModules = null;
            SimpleButton btnMinus = null;
            SimpleButton btnSave = null;

            _ui.Invoke(() =>
            {
                page = new ProductDetailsPage();
                _ui.ShowControl(page);

                page.Initialize(id, ServiceRegistry.Database, ServiceRegistry.Product);

                grdModules = GetPrivateControl<GridControl>(page, "grdModules");
                btnMinus = GetPrivateControl<SimpleButton>(page, "btnMinus");
                btnSave = GetPrivateControl<SimpleButton>(page, "btnSave");
            });

            var initialRows = _ui.Invoke(() => GetGridView(grdModules)?.RowCount ?? 0);
            Assert.AreEqual(1, initialRows, "Edit mode should load the single existing module.");

            _ui.Invoke(() =>
            {
                btnMinus.PerformClick();
                Application.DoEvents();

                btnSave.PerformClick();
                Application.DoEvents();
            });

            var remaining = _db.GetModulesForProduct("OLD-001") ?? Enumerable.Empty<ModuleDto>();
            Assert.AreEqual(0, remaining.Count(), "After removing and saving, OLD-001 should have 0 modules.");
        }

        // ---------- Helpers ----------

        private static GridView GetGridView(GridControl grid) => grid?.MainView as GridView;

        private static T GetPrivateControl<T>(Control parent, string name) where T : class
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

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

        private static void SetPropertyIfExists(object target, string propertyName, object value)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            var t = target.GetType();
            var pi = t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (pi == null || !pi.CanWrite)
                throw new InvalidOperationException($"Property '{propertyName}' not found or not writable on '{t.Name}'.");
            pi.SetValue(target, value);
        }

        private static void ResetServiceRegistryCaches()
        {
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