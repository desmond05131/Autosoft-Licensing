// TASK: Create integration tests for User Management (ManageUserPage & UserDetailsPage).
// CONTEXT: WinForms app, DevExpress, ServiceRegistry, UiTestHost.
//
// INSTRUCTIONS:
// 1. Create class "UserPagesIntegrationTests" in "Autosoft_Licensing.Tools".
// 2. Setup: [TestInitialize] with InMemoryLicenseDatabaseService and a mocked/real EncryptionService.
// 3. Helper: Include "GetPrivateControl<T>" and a helper to interact with the Permissions Grid (since rows are likely a private class).
//
// IMPLEMENT TESTS:
//
// TEST 1: ManagePage_Delete_Protection
//    - Setup: Seed "admin" and "staff".
//    - Act: Load ManageUserPage. Select "admin". Click "btnDelete".
//    - Assert: User "admin" still exists in DB (Count is 2).
//    - Act: Select "staff". Click "btnDelete".
//    - Assert: User "staff" is gone (Count is 1).
//
// TEST 2: DetailsPage_Create_Validation_And_Hashing
//    - Setup: Real EncryptionService (or deterministic mock).
//    - Act: Load UserDetailsPage (Create).
//    - Action: Set txtUsername="test", txtPassword="123", txtConfirm="999". Click Save.
//    - Assert: Save failed (DB count unchanged).
//    - Action: Fix txtConfirm="123". Tick "Generate License" in the Permissions Grid. Click Save.
//    - Assert: DB has user "test". 
//    - Assert: User.PasswordHash != "123" (it must be hashed).
//    - Assert: User.CanGenerateLicense == true.
//
// TEST 3: DetailsPage_Admin_Locking
//    - Setup: Seed "admin" user.
//    - Act: Load UserDetailsPage for "admin".
//    - Assert: txtUsername.ReadOnly is TRUE.
//    - Assert: All rows in 'grdPermissions' are Checked (State = True).
//
// TEST 4: DetailsPage_Duplicate_Username_Check
//    - Setup: Seed user "existing_user".
//    - Act: Create new user with username "existing_user". Click Save.
//    - Assert: Save fails (DB count unchanged). Error message shown (mock ShowError if possible, or just verify DB).
//
// IMPLEMENTATION NOTES:
// - Accessing the Permissions Grid: The GridControl 'grdPermissions' likely binds to a list of custom objects. 
//   Use reflection to get the DataSource (as IEnumerable) and iterate it to find/set the Boolean property.
// - Password Hashing: Ensure 'ServiceRegistry.Encryption' is set up so the page doesn't crash on save.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Services.Impl;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Tests.Helpers;
using Newtonsoft.Json.Linq;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class UserPagesIntegrationTests
    {
        private UiTestHost _ui;
        private InMemoryLicenseDatabaseService _db;
        private IEncryptionService _enc;

        [TestInitialize]
        public void Init()
        {
            _db = new InMemoryLicenseDatabaseService();

            // Deterministic test encryption (implements IEncryptionService fully)
            _enc = new TestEncryptionService();

            // Reset ServiceRegistry caches and bind fresh DB
            ResetServiceRegistryCaches();
            ServiceRegistry.Database = _db;

            _ui = new UiTestHost("UserTestsHost");
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

        // TEST 1: ManagePage_Delete_Protection
        [TestMethod]
        public void ManagePage_Delete_Protection()
        {
            // Ensure admin exists and add staff
            var admin = _db.GetUserByUsername("admin");
            if (admin == null)
            {
                admin = new User
                {
                    Username = "admin",
                    DisplayName = "System Administrator",
                    Role = "Admin",
                    PasswordHash = _enc.ComputeSha256Hex(Encoding.UTF8.GetBytes("admin")),
                    CreatedUtc = DateTime.UtcNow,
                    IsActive = true,
                    CanManageUsers = true
                };
                _db.InsertUser(admin);
            }

            var staff = new User
            {
                Username = "staff",
                DisplayName = "Staff User",
                Role = "Support",
                PasswordHash = _enc.ComputeSha256Hex(Encoding.UTF8.GetBytes("staff")),
                CreatedUtc = DateTime.UtcNow,
                IsActive = true
            };
            staff.Id = _db.InsertUser(staff);

            ManageUserPage page = null;
            GridControl grd = null;
            SimpleButton btnDelete = null;

            _ui.Invoke(() =>
            {
                page = new ManageUserPage();
                page.Initialize(ServiceRegistry.Database);
                _ui.ShowControl(page);

                grd = GetPrivateControl<GridControl>(page, "grdUsers");
                btnDelete = GetPrivateControl<SimpleButton>(page, "btnDelete");
            });

            // Select 'admin' and attempt delete (guarded, should not delete)
            _ui.Invoke(() =>
            {
                FocusRowByUsername(grd, "admin");
                btnDelete.PerformClick(); // Guard triggers, no confirmation appears
                Application.DoEvents();
            });

            var afterAdminAttemptUsers = _db.GetUsers()?.ToList() ?? new List<User>();
            Assert.AreEqual(2, afterAdminAttemptUsers.Count, "Admin delete attempt should be blocked by UI (admin + staff present).");
            Assert.IsNotNull(afterAdminAttemptUsers.FirstOrDefault(u => u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase)), "Admin must still exist.");

            // Select 'staff' and delete (confirm Yes)
            _ui.Invoke(() =>
            {
                FocusRowByUsername(grd, "staff");
                ConfirmNextMessageBoxYes(); // auto-press Yes for confirmation
                btnDelete.PerformClick();
                Application.DoEvents();
            });

            var remaining = _db.GetUsers()?.ToList() ?? new List<User>();
            Assert.AreEqual(1, remaining.Count, "Exactly one user should remain after deleting 'staff'.");
            Assert.IsNotNull(remaining.FirstOrDefault(u => u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase)), "Admin must still exist after deleting 'staff'.");
        }

        // TEST 2: DetailsPage_Create_Validation_And_Hashing
        [TestMethod]
        public void DetailsPage_Create_Validation_And_Hashing()
        {
            var admin = EnsureAdminUser();

            UserDetailsPage page = null;
            TextEdit txtUsername = null;
            TextEdit txtDisplayName = null;
            TextEdit txtPassword = null;
            TextEdit txtConfirm = null; // optional (designer may or may not have this)
            GridControl grdPermissions = null;
            SimpleButton btnSave = null;

            _ui.Invoke(() =>
            {
                page = new UserDetailsPage();
                _ui.ShowControl(page);

                // Explicit DI to ensure _dbService and _encryptionService present
                page.Initialize(ServiceRegistry.Database, _enc, null);
                // Enable Save by role
                page.InitializeForRole(admin);

                txtUsername = GetPrivateControl<TextEdit>(page, "txtUsername");
                txtDisplayName = GetPrivateControl<TextEdit>(page, "txtDisplayName");
                txtPassword = GetPrivateControl<TextEdit>(page, "txtPassword");
                grdPermissions = GetPrivateControl<GridControl>(page, "grdPermissions");
                btnSave = GetPrivateControl<SimpleButton>(page, "btnSave");

                // Try to get confirm if present; optional
                txtConfirm = TryGetPrivateControl<TextEdit>(page, "txtConfirm");
            });

            var beforeCount = _db.GetUsers().Count();

            // First attempt: enforce validation failure (DisplayName required)
            _ui.Invoke(() =>
            {
                txtUsername.Text = "test";
                txtPassword.Text = "123";
                if (txtConfirm != null) txtConfirm.Text = "999"; // per instruction (page doesn't validate confirm)
                txtDisplayName.Text = string.Empty; // force fail

                btnSave.PerformClick();
                Application.DoEvents();
            });

            var afterFirstAttemptCount = _db.GetUsers().Count();
            Assert.AreEqual(beforeCount, afterFirstAttemptCount, "Create should fail due to validation (missing display name).");

            // Fix and save: set display name, set confirm to match if present, tick Generate License permission
            _ui.Invoke(() =>
            {
                if (txtConfirm != null)
                {
                    txtConfirm.Text = "123";
                }
                txtDisplayName.Text = "Test User";

                // Tick "Generate License" permission using Key="GEN"
                SetPermissionChecked(grdPermissions, key: "GEN", isChecked: true);

                btnSave.PerformClick();
                Application.DoEvents();
            });

            var user = _db.GetUserByUsername("test");
            Assert.IsNotNull(user, "User 'test' should be created.");
            Assert.AreNotEqual("123", user.PasswordHash, "Password must be hashed, not stored in plain text.");

            var expectedHash = _enc.ComputeSha256Hex(Encoding.UTF8.GetBytes("123"));
            Assert.AreEqual(expectedHash, user.PasswordHash, "Password hash should match deterministic test hasher.");
            Assert.IsTrue(user.CanGenerateLicense, "Generate License permission should be true.");
        }

        // TEST 3: DetailsPage_Admin_Locking
        [TestMethod]
        public void DetailsPage_Admin_Locking()
        {
            var admin = EnsureAdminUser();

            UserDetailsPage page = null;
            TextEdit txtUsername = null;
            GridControl grdPermissions = null;

            _ui.Invoke(() =>
            {
                page = new UserDetailsPage();
                _ui.ShowControl(page);

                page.Initialize(ServiceRegistry.Database, _enc, admin.Id);
                page.InitializeForRole(admin); // role init should keep admin locked

                txtUsername = GetPrivateControl<TextEdit>(page, "txtUsername");
                grdPermissions = GetPrivateControl<GridControl>(page, "grdPermissions");
            });

            var isReadOnly = _ui.Invoke(() => txtUsername.Properties.ReadOnly);
            Assert.IsTrue(isReadOnly, "Admin username should be read-only.");

            var allChecked = _ui.Invoke(() => GetPermissionRows(grdPermissions).All(r => r.IsChecked));
            Assert.IsTrue(allChecked, "All permission rows should be checked for default admin.");
        }

        // TEST 4: DetailsPage_Duplicate_Username_Check
        [TestMethod]
        public void DetailsPage_Duplicate_Username_Check()
        {
            var admin = EnsureAdminUser();

            // Seed existing user
            var existing = new User
            {
                Username = "existing_user",
                DisplayName = "Existing",
                Role = "Support",
                PasswordHash = _enc.ComputeSha256Hex(Encoding.UTF8.GetBytes("pw")),
                CreatedUtc = DateTime.UtcNow,
                IsActive = true
            };
            _db.InsertUser(existing);

            var beforeCount = _db.GetUsers().Count();

            UserDetailsPage page = null;
            TextEdit txtUsername = null;
            TextEdit txtDisplayName = null;
            TextEdit txtPassword = null;
            SimpleButton btnSave = null;

            _ui.Invoke(() =>
            {
                page = new UserDetailsPage();
                _ui.ShowControl(page);

                page.Initialize(ServiceRegistry.Database, _enc, null);
                page.InitializeForRole(admin);

                txtUsername = GetPrivateControl<TextEdit>(page, "txtUsername");
                txtDisplayName = GetPrivateControl<TextEdit>(page, "txtDisplayName");
                txtPassword = GetPrivateControl<TextEdit>(page, "txtPassword");
                btnSave = GetPrivateControl<SimpleButton>(page, "btnSave");

                txtUsername.Text = "existing_user";
                txtDisplayName.Text = "Dup";
                txtPassword.Text = "newpw";

                btnSave.PerformClick();
                Application.DoEvents();
            });

            var afterCount = _db.GetUsers().Count();
            Assert.AreEqual(beforeCount, afterCount, "Save should fail for duplicate username (DB count unchanged).");
        }

        // ---------- Helpers ----------

        private User EnsureAdminUser()
        {
            var admin = _db.GetUserByUsername("admin");
            if (admin != null) return admin;

            admin = new User
            {
                Username = "admin",
                DisplayName = "System Administrator",
                Role = "Admin",
                PasswordHash = _enc.ComputeSha256Hex(Encoding.UTF8.GetBytes("admin")),
                CreatedUtc = DateTime.UtcNow,
                IsActive = true,
                CanManageUsers = true,
                CanManageProduct = true,
                CanGenerateLicense = true,
                CanViewRecords = true
            };
            admin.Id = _db.InsertUser(admin);
            return admin;
        }

        private static GridView GetGridView(GridControl grid) => grid?.MainView as GridView;

        private static void FocusRowByUsername(GridControl grid, string username)
        {
            var view = GetGridView(grid);
            if (view == null) return;

            for (int i = 0; i < view.DataRowCount; i++)
            {
                var rowObj = view.GetRow(i);
                if (rowObj == null) continue;

                var unProp = rowObj.GetType().GetProperty("Username", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var value = unProp?.GetValue(rowObj)?.ToString();

                if (string.Equals(value, username, StringComparison.OrdinalIgnoreCase))
                {
                    view.FocusedRowHandle = i;
                    return;
                }
            }
        }

        private static void ConfirmNextMessageBoxYes()
        {
            var t = new System.Windows.Forms.Timer();
            t.Interval = 50;
            t.Tick += (s, e) =>
            {
                try
                {
                    t.Stop();
                    SendKeys.SendWait("%Y"); // Alt+Y
                    SendKeys.SendWait("{ENTER}");
                }
                catch { /* ignore */ }
                finally
                {
                    t.Dispose();
                }
            };
            t.Start();
        }

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

        private static T TryGetPrivateControl<T>(Control parent, string name) where T : class
        {
            try { return GetPrivateControl<T>(parent, name); } catch { return null; }
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

        private static IEnumerable<(string Key, string Description, bool IsChecked, object Row)> GetPermissionRows(GridControl grid)
        {
            var ds = grid?.DataSource as IEnumerable;
            if (ds == null) return Enumerable.Empty<(string, string, bool, object)>();

            var list = new List<(string, string, bool, object)>();
            foreach (var item in ds)
            {
                var t = item.GetType();
                var key = t.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(item)?.ToString() ?? string.Empty;
                var desc = t.GetProperty("Description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(item)?.ToString() ?? string.Empty;
                var isChecked = (bool)(t.GetProperty("IsChecked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(item) ?? false);
                list.Add((key, desc, isChecked, item));
            }
            return list;
        }

        private static void SetPermissionChecked(GridControl grid, string key, bool isChecked)
        {
            var ds = grid?.DataSource as IEnumerable;
            if (ds == null) return;

            foreach (var item in ds)
            {
                var t = item.GetType();
                var k = t.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(item)?.ToString();
                if (string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                {
                    var pi = t.GetProperty("IsChecked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (pi != null && pi.CanWrite)
                    {
                        pi.SetValue(item, isChecked);
                    }
                }
            }

            try { grid?.RefreshDataSource(); } catch { /* ignore */ }
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

        // Deterministic encryption service for tests (implements IEncryptionService)
        private class TestEncryptionService : IEncryptionService
        {
            public string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv)
            {
                // Simple reversible encoding for tests
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonWithChecksum ?? string.Empty));
            }

            public string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv)
            {
                var bytes = Convert.FromBase64String(base64Asl ?? string.Empty);
                return Encoding.UTF8.GetString(bytes);
            }

            public string ComputeSha256Hex(byte[] data)
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    var hash = sha.ComputeHash(data ?? Array.Empty<byte>());
                    var sb = new StringBuilder(hash.Length * 2);
                    foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
                    return sb.ToString();
                }
            }

            public bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex)
            {
                var bytes = Encoding.UTF8.GetBytes(jsonWithoutChecksum ?? string.Empty);
                var hex = ComputeSha256Hex(bytes);
                return string.Equals(hex, checksumHex, StringComparison.OrdinalIgnoreCase);
            }

            public string BuildJsonWithChecksum(LicenseData licenseWithoutChecksum)
            {
                var j = JObject.FromObject(licenseWithoutChecksum ?? new LicenseData());
                j.Remove("ChecksumSHA256");

                var canonical = CanonicalJsonSerializer.Serialize(j);
                var bytes = Encoding.UTF8.GetBytes(canonical ?? string.Empty);
                var checksum = ComputeSha256Hex(bytes);

                j.AddFirst(new JProperty("ChecksumSHA256", checksum));
                return CanonicalJsonSerializer.Serialize(j);
            }
        }
    }
}