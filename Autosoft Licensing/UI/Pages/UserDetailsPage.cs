/*
PAGE: UserDetailsPage.cs
ROLE: Dealer Admin
PURPOSE:
  Create or edit a single user account and assign access rights for Dealer app capabilities.

KEY UI ELEMENTS:
  - TextBox: Username (readonly for edit), DisplayName
  - Password and Confirm Password fields (for create or change)
  - Checkbox list: Access rights (Generate License, License record, Manage Product, Manage User)
  - Buttons: Save, Cancel

BACKEND SERVICE CALLS:
  - On load (edit): ServiceRegistry.Database.GetUserById(id) or ServiceRegistry.Database.GetUserByUsername(username)
  - On Save: ServiceRegistry.Database.InsertUser(user) or ServiceRegistry.Database.UpdateUser(user)

VALIDATION & RULES:
  - Require non-empty username and displayname
  - Password required for new users; for updates optional unless changing password
  - Only Admin can assign ManageUser right
  - Prevent deleting or demoting the default Admin (this logic enforced in ManageUserPage Delete; here just prevent editing default admin to remove all admin roles)

ACCESS CONTROL:
  - Only users with ManageUser permission can use/save this form

UX NOTES:
  - If password fields left blank on edit, keep existing password
  - Show message on success (UI transient toast)

ACCEPTANCE CRITERIA:
  - New users created in DB and appear on ManageUserPage
  - Edits persist and role changes reflected in app permissions

COPILOT PROMPTS:
  - "// Implement SaveUser -> validate fields, optionally hash password, and call ServiceRegistry.Database.InsertUser(user) for new users or ServiceRegistry.Database.UpdateUser(user) for updates."
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms; // <-- add this
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class UserDetailsPage : PageBase
    {
        private ILicenseDatabaseService _dbService;
        private IEncryptionService _encryptionService;

        private BindingList<PermissionRow> _rows = new BindingList<PermissionRow>();
        private int? _userId;
        private User _existingUser;
        private User _currentUser; // host can set via InitializeForRole

        // Removed: manual UI fields now provided by Designer:
        // lblRole, cmbRole, lblEmail, txtEmail, lblCreated, txtCreatedUtc

        // Host can listen and navigate back to ManageUserPage after save
        public event EventHandler NavigateBackRequested;

        internal class PermissionRow
        {
            public string Key { get; set; }
            public string Description { get; set; }
            public bool IsChecked { get; set; }
        }

        public UserDetailsPage()
        {
            InitializeComponent();
            try
            {
                if (!DesignMode)
                {
                    // grid visual defaults
                    var view = grdPermissions.MainView as GridView;
                    if (view != null)
                    {
                        view.OptionsView.ShowGroupPanel = false;
                        view.OptionsSelection.EnableAppearanceFocusedCell = false;
                        view.FocusRectStyle = DrawFocusRectStyle.RowFocus;
                    }
                    if (btnSave != null)
                    {
                        btnSave.Click += btnSave_Click;
                    }
                }
            }
            catch { /* ignore design-time issues */ }

            // Removed: EnsureExtraFields(); all controls are now in Designer

            // NEW: Wire Cancel navigation
            try
            {
                if (btnCancel != null)
                {
                    btnCancel.Click += (s, e) =>
                    {
                        // Navigate back to ManageUserPage (host wires this event in MainForm)
                        NavigateBackRequested?.Invoke(this, EventArgs.Empty);
                    };
                }
            }
            catch { /* ignore */ }
        }

        private void EnforceDefaultAdminLock()
        {
            try
            {
                if (_existingUser != null && string.Equals(_existingUser.Username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    // Keep everything locked and Save disabled for the default admin record.
                    btnSave.Enabled = false;

                    txtUsername.ReadOnly = true;
                    txtDisplayName.ReadOnly = true;
                    txtPassword.Enabled = false;

                    if (cmbRole != null) cmbRole.Enabled = false;
                    if (txtEmail != null) txtEmail.Enabled = false;

                    var view = grdPermissions.MainView as GridView;
                    if (view != null) view.OptionsBehavior.Editable = false;
                }
            }
            catch { /* ignore */ }
        }

        public override void InitializeForRole(User user)
        {
            _currentUser = user;
            // Only users with ManageUser permission (or Admin role) can save/modify.
            var canManage = user != null && (user.CanManageUsers || string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase));
            btnSave.Enabled = canManage;

            // NEW: Cancel is always enabled to allow leaving without saving
            try { if (btnCancel != null) btnCancel.Enabled = true; } catch { /* ignore */ }

            // If cannot manage, make form read-only for clarity.
            try
            {
                if (!canManage)
                {
                    txtUsername.ReadOnly = true;
                    txtDisplayName.ReadOnly = true;
                    txtPassword.Enabled = false;

                    if (cmbRole != null) cmbRole.Enabled = false;
                    if (txtEmail != null) txtEmail.Enabled = false;

                    var view = grdPermissions.MainView as GridView;
                    if (view != null) view.OptionsBehavior.Editable = false;
                }
                else
                {
                    // Respect edit-mode rules (LoadExisting sets default admin to read-only and disables Save).
                    var view = grdPermissions.MainView as GridView;
                    if (view != null) view.OptionsBehavior.Editable = true;

                    if (cmbRole != null) cmbRole.Enabled = true;
                    if (txtEmail != null) txtEmail.Enabled = true;
                }

                // Ensure default admin stays locked even if InitializeForRole runs after LoadExisting
                EnforceDefaultAdminLock();
            }
            catch { /* ignore */ }
        }

        // Overload with explicit DI
        public void Initialize(ILicenseDatabaseService db, IEncryptionService encryption, int? userId)
        {
            _dbService = db ?? throw new ArgumentNullException(nameof(db));
            _encryptionService = encryption ?? throw new ArgumentNullException(nameof(encryption));
            Initialize(userId);
        }

        // Default: resolve from ServiceRegistry
        public void Initialize(int? userId)
        {
            try
            {
                _dbService ??= ServiceRegistry.Database;
            }
            catch { /* allow tests to inject later */ }

            try
            {
                _encryptionService ??= ServiceRegistry.Encryption;
            }
            catch { /* allow tests to inject later */ }

            _userId = userId;

            BuildPermissionRows();
            BindPermissionGrid();

            if (userId.HasValue)
                LoadExisting(userId.Value);
            else
                PrepareNew();
        }

        private void BuildPermissionRows()
        {
            _rows = new BindingList<PermissionRow>(new List<PermissionRow>
            {
                new PermissionRow { Key = "GEN",  Description = "Generate License", IsChecked = false },
                new PermissionRow { Key = "REC",  Description = "License record",   IsChecked = false },
                new PermissionRow { Key = "PROD", Description = "Manage Product",   IsChecked = false },
                new PermissionRow { Key = "USER", Description = "Manage User",      IsChecked = false }
            });
        }

        private void BindPermissionGrid()
        {
            grdPermissions.DataSource = _rows;
            try { (grdPermissions.MainView as GridView)?.BestFitColumns(); } catch { }
        }

        private void PrepareNew()
        {
            try
            {
                _existingUser = null;
                txtUsername.ReadOnly = false;
                txtUsername.Text = string.Empty;
                txtDisplayName.Text = string.Empty;
                txtPassword.Text = string.Empty;
                chkIsActive.Checked = true;

                if (txtEmail != null) txtEmail.Text = string.Empty;
                if (txtCreatedUtc != null) txtCreatedUtc.Text = DateTime.Now.ToString("g");
                if (cmbRole != null) cmbRole.SelectedIndex = 1; // Support

                foreach (var r in _rows) r.IsChecked = false;
                grdPermissions.RefreshDataSource();
                btnSave.Enabled = btnSave.Enabled && true; // keep role gating
            }
            catch { /* ignore */ }
        }

        private void LoadExisting(int id)
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                _existingUser = _dbService.GetUserById(id);
                if (_existingUser == null)
                {
                    ShowError("User not found.");
                    return;
                }

                txtUsername.Text = _existingUser.Username ?? string.Empty;
                txtDisplayName.Text = _existingUser.DisplayName ?? string.Empty;
                txtPassword.Text = string.Empty; // do not show hash
                chkIsActive.Checked = _existingUser.IsActive;

                if (txtEmail != null) txtEmail.Text = _existingUser.Email ?? string.Empty;
                if (txtCreatedUtc != null) txtCreatedUtc.Text = ToLocal(_existingUser.CreatedUtc).ToString("g");
                if (cmbRole != null) cmbRole.EditValue = _existingUser.Role;

                MapUserToPermissions(_existingUser);
                grdPermissions.RefreshDataSource(); // ensure grid reflects mapped permissions

                // If default admin, lock everything and force all permissions true.
                if (string.Equals(_existingUser.Username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var r in _rows) r.IsChecked = true;
                    grdPermissions.RefreshDataSource();

                    txtUsername.ReadOnly = true;
                    txtDisplayName.ReadOnly = true;
                    txtPassword.Enabled = false;

                    if (cmbRole != null) { cmbRole.EditValue = "Admin"; cmbRole.Enabled = false; }
                    if (txtEmail != null) txtEmail.Enabled = false;

                    var view = grdPermissions.MainView as GridView;
                    if (view != null) view.OptionsBehavior.Editable = false;

                    btnSave.Enabled = false;
                }
                else
                {
                    txtUsername.ReadOnly = true; // username immutable on edit
                    txtDisplayName.ReadOnly = false;
                    txtPassword.Enabled = true;

                    // Role gating may further restrict editability (handled in InitializeForRole)
                    var view = grdPermissions.MainView as GridView;
                    if (view != null) view.OptionsBehavior.Editable = btnSave.Enabled;
                }

                try { (grdPermissions.MainView as GridView)?.BestFitColumns(); } catch { }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.LoadExisting error: {ex}");
                ShowError("Failed to load user.");
            }
        }

        private void MapUserToPermissions(User u)
        {
            SetPerm("GEN", u.CanGenerateLicense);
            SetPerm("REC", u.CanViewRecords);
            SetPerm("PROD", u.CanManageProduct);
            SetPerm("USER", u.CanManageUsers);
        }

        private void SetPerm(string key, bool value)
        {
            var row = _rows.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            if (row != null) row.IsChecked = value;
        }

        private bool GetPerm(string key)
        {
            var row = _rows.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            return row != null && row.IsChecked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveUser();
        }

        // Prevent editing Manage User checkbox if current user is not allowed (unless editing self-admin lock handled separately).
        private void viewPermissions_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null || view.FocusedColumn == null) return;

                if (!string.Equals(view.FocusedColumn.FieldName, "IsChecked", StringComparison.OrdinalIgnoreCase))
                    return;

                var row = view.GetFocusedRow() as PermissionRow;
                if (row == null) return;

                bool isManageUserRow = string.Equals(row.Key, "USER", StringComparison.OrdinalIgnoreCase);
                bool currentCanAssignManageUser = _currentUser != null &&
                    (_currentUser.CanManageUsers || string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));

                if (isManageUserRow && !currentCanAssignManageUser)
                    e.Cancel = true;

                // Also block all edits if default admin user is being edited (handled earlier), defensive here.
                if (_existingUser != null && string.Equals(_existingUser.Username, "admin", StringComparison.OrdinalIgnoreCase))
                    e.Cancel = true;
            }
            catch { /* ignore */ }
        }

        private void SaveUser()
        {
            try
            {
                if (_dbService == null || _encryptionService == null)
                {
                    ShowError("Service not initialized.");
                    return;
                }

                var username = (txtUsername.Text ?? string.Empty).Trim();
                var displayName = (txtDisplayName.Text ?? string.Empty).Trim();
                var password = txtPassword.Text ?? string.Empty;
                var isActive = chkIsActive.Checked;
                var email = txtEmail != null ? (txtEmail.Text ?? string.Empty).Trim() : null;
                var selectedRole = cmbRole != null ? (cmbRole.Text ?? string.Empty).Trim() : string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowError("Username is required.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    ShowError("Display Name is required.");
                    return;
                }

                // Optional email validation: if provided, require a basic '@'
                if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
                {
                    ShowError("Email format looks invalid.");
                    return;
                }

                var isNew = !_userId.HasValue || _existingUser == null;

                // Unique username on create
                if (isNew)
                {
                    var existingByUsername = _dbService.GetUserByUsername(username);
                    if (existingByUsername != null)
                    {
                        ShowError("Username already exists.");
                        return;
                    }
                }

                // Only Admin can assign ManageUser right via grid (defensive)
                bool currentCanAssignManageUser = _currentUser != null &&
                    (_currentUser.CanManageUsers || string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));
                if (!currentCanAssignManageUser)
                {
                    SetPerm("USER", _existingUser != null ? _existingUser.CanManageUsers : false);
                }

                // Password hashing rules
                string passwordHash;
                if (isNew)
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        ShowError("Password is required for a new user.");
                        return;
                    }
                    passwordHash = _encryptionService.ComputeSha256Hex(Encoding.UTF8.GetBytes(password));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(password))
                        passwordHash = _encryptionService.ComputeSha256Hex(Encoding.UTF8.GetBytes(password));
                    else
                        passwordHash = _existingUser.PasswordHash; // keep existing
                }

                // Build user object
                var user = isNew ? new User() : new User { Id = _existingUser.Id };
                user.Username = username;
                user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName;
                user.Email = email;
                user.PasswordHash = passwordHash;
                user.IsActive = isActive;

                // Map permissions from grid (initial)
                user.CanGenerateLicense = GetPerm("GEN");
                user.CanViewRecords = GetPerm("REC");
                user.CanManageProduct = GetPerm("PROD");
                user.CanManageUsers = GetPerm("USER");

                // Role mapping from UI combo
                // CRITICAL: enforce role?permissions consistency
                var roleFromUi = string.IsNullOrWhiteSpace(selectedRole) ? "Support" : selectedRole;
                if (string.Equals(roleFromUi, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    user.Role = "Admin";
                    user.CanManageUsers = true; // force Admin implies Manage Users
                }
                else
                {
                    user.Role = "Support";
                    user.CanManageUsers = false; // Support cannot manage users
                }

                // Guard: at least one Admin must remain (covers self or others)
                if (!isNew && _existingUser != null && _existingUser.CanManageUsers && !user.CanManageUsers)
                {
                    var anyOtherAdmin = (_db_service_getusers_safe() ?? Enumerable.Empty<User>())
                        .Any(u => u.Id != _existingUser.Id && (u.CanManageUsers || string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase)));
                    if (!anyOtherAdmin)
                    {
                        ShowError("At least one Admin must remain. Assign another Admin before removing Admin rights.");
                        return;
                    }
                }

                if (isNew)
                {
                    user.CreatedUtc = DateTime.UtcNow;
                    var newId = _dbService.InsertUser(user);
                    _userId = newId;
                }
                else
                {
                    _dbService.UpdateUser(user);
                }

                ShowInfo("User saved.", "Success");
                NavigateBackRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.SaveUser error: {ex}");
                ShowError("Failed to save user.");
            }
        }

        // Safe wrapper to avoid null-conditional on every call site
        private IEnumerable<User> _db_service_getusers_safe()
        {
            try { return _dbService.GetUsers() ?? Enumerable.Empty<User>(); }
            catch { return Enumerable.Empty<User>(); }
        }
    }
}