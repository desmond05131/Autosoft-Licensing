/*
PAGE: ManageUserPage.cs
ROLE: Dealer Admin (super-admin)
PURPOSE:
  Manage Dealer EXE user accounts and their access rights to the Dealer app (Generate License, License Records, Manage Product, Manage User).

KEY UI ELEMENTS:
  - GridControl: list of users (Username, DisplayName, IsActive, Roles)
  - Buttons: Create, View, Edit, Delete
  - User form: Username, DisplayName, Password, ConfirmPassword, Role checkboxes (GenerateLicense, LicenseRecord, ManageProduct, ManageUser), IsActive checkbox
  - Notes area: "Admin account cannot be deleted" hint

BACKEND SERVICE CALLS:
  - ServiceRegistry.Database.GetUsers(), InsertUser(user), UpdateUser(user), DeleteUser(id)

VALIDATION & RULES:
  - Username unique
  - Cannot delete 'Admin' account (enforce in UI and service guard)
  - Password rules per internal policy (min length; for prototype may be simple)
  - At least one Admin must remain

ACCESS CONTROL:
  - Only Admin role may access ManageUser page
  - Users with ManageUser permission can edit certain flags; only highest admin can assign ManageUser rights

UX NOTES:
  - Show a purple note "Admin cannot delete User Admin" per wireframe
  - Confirm delete with modal; if attempting to delete Admin account show warning and block

ACCEPTANCE CRITERIA:
  - Create/Edit/Delete operate as expected with permission enforcement
  - Admin account protected from deletion

COPILOT PROMPTS:
  - "// Implement LoadUsers to call ServiceRegistry.Database.GetUsers() and bind grid"
  - "// Implement DeleteUser to check against default Admin and call ServiceRegistry.Database.DeleteUser(userId)"
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms; // Required for Control.MousePosition
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class ManageUserPage : PageBase
    {
        private ILicenseDatabaseService _dbService;

        // Internal row model
        private class UserRow
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public bool IsActive { get; set; }
        }

        private BindingList<UserRow> _data = new BindingList<UserRow>();

        // REMOVED: Shadowing navigation event and args; use PageBase.NavigateRequested with unified NavigateEventArgs

        public ManageUserPage()
        {
            InitializeComponent();

            try
            {
                if (!DesignMode)
                {
                    try { if (_dbService == null) _dbService = ServiceRegistry.Database; } catch { }

                    // Wire actions
                    btnCreate.Click += btnCreate_Click;
                    btnView.Click += btnView_Click;
                    btnEdit.Click += btnEdit_Click;
                    btnDelete.Click += btnDelete_Click;

                    // Navigation buttons — identical to GenerateLicensePage
                    if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                    if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                    if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                    if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                    if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");

                    // Logout (panel + inner label + picture)
                    if (btnNav_Logout != null) BindNavigationEvent(btnNav_Logout, "Logout");
                    if (lblNav_Logout != null) BindNavigationEvent(lblNav_Logout, "Logout");
                    if (picNav_Logout != null) BindNavigationEvent(picNav_Logout, "Logout");

                    // Grid setup (read-only)
                    var view = grdUsers.MainView as GridView;
                    if (view != null)
                    {
                        view.OptionsBehavior.Editable = false;
                        view.OptionsView.ShowGroupPanel = false;
                        view.OptionsView.ShowIndicator = false;
                        view.FocusRectStyle = DrawFocusRectStyle.RowFocus;
                        view.BestFitColumns();

                        // --- ADDED: Double Click Event ---
                        view.DoubleClick += GrdUsers_DoubleClick;
                    }

                    // Initial load
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageUserPage ctor error: {ex}");
            }
        }

        public void Initialize(ILicenseDatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        public override void InitializeForRole(User user)
        {
            try
            {
                // Default: only users allowed to manage users can create/edit/delete
                bool canManage = user != null && (user.CanManageUsers || string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase));
                btnCreate.Enabled = canManage;
                btnEdit.Enabled = canManage;
                btnDelete.Enabled = canManage;
                btnView.Enabled = true;

                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user?.CanGenerateLicense ?? false;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user?.CanViewRecords ?? false;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user?.CanManageProduct ?? false;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user?.CanManageUsers ?? false;

                // Settings Admin-only (match GenerateLicensePage behavior)
                if (btnNav_GeneralSetting != null)
                    btnNav_GeneralSetting.Visible = string.Equals(user?.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                // Logout always visible
                if (btnNav_Logout != null) btnNav_Logout.Visible = true;
            }
            catch { /* ignore */ }
        }

        // DATA
        private void RefreshData()
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var users = _dbService.GetUsers() ?? Enumerable.Empty<User>();
                _data = new BindingList<UserRow>(
                    users.Select(u => new UserRow
                    {
                        Id = u.Id,
                        Username = u.Username ?? string.Empty,
                        IsActive = u.IsActive
                    }).OrderBy(r => r.Username, StringComparer.OrdinalIgnoreCase).ToList());

                grdUsers.DataSource = _data;

                try
                {
                    var view = grdUsers.MainView as GridView;
                    view?.BestFitColumns();
                }
                catch { }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load users.");
                System.Diagnostics.Debug.WriteLine($"ManageUserPage.RefreshData error: {ex}");
            }
        }

        private UserRow GetFocusedRow()
        {
            try
            {
                var view = grdUsers.MainView as GridView;
                if (view == null) return null;

                // Attempt strongly typed row first
                var row = view.GetFocusedRow() as UserRow;
                if (row != null) return row;

                // Fallback if bound directly to Model.User in the future
                var any = view.GetFocusedRow();
                if (any == null) return null;

                var idProp = any.GetType().GetProperty("Id");
                var usernameProp = any.GetType().GetProperty("Username");
                var activeProp = any.GetType().GetProperty("IsActive");
                if (idProp != null && usernameProp != null && activeProp != null)
                {
                    return new UserRow
                    {
                        Id = (int)(idProp.GetValue(any) ?? 0),
                        Username = usernameProp.GetValue(any)?.ToString(),
                        IsActive = (bool)(activeProp.GetValue(any) ?? false)
                    };
                }
                return null;
            }
            catch { return null; }
        }

        // ACTIONS
        private void btnCreate_Click(object sender, EventArgs e)
        {
            Navigate("UserDetailsPage", null, "Create");
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                ShowError("Please select a user.");
                return;
            }
            Navigate("UserDetailsPage", row.Id, "View");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                ShowError("Please select a user.");
                return;
            }
            Navigate("UserDetailsPage", row.Id, "Edit");
        }

        // --- ADDED: Double Click Handler ---
        private void GrdUsers_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;

                // Robust hit detection to ensure we clicked a valid row
                var pt = view.GridControl.PointToClient(Control.MousePosition);
                var hit = view.CalcHitInfo(pt);

                if (hit.InRow || hit.InRowCell)
                {
                    var row = GetFocusedRow();
                    if (row != null)
                    {
                        // Navigate to details; the destination page will handle ReadOnly permissions automatically
                        Navigate("UserDetailsPage", row.Id, "Edit");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GrdUsers_DoubleClick error: {ex}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var row = GetFocusedRow();
                if (row == null)
                {
                    ShowError("Please select a user.");
                    return;
                }

                // Guard default admin
                if (string.Equals(row.Username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("The default Administrator account cannot be deleted.");
                    return;
                }

                var result = XtraMessageBox.Show(
                    $"Are you sure you want to delete user '{row.Username}'?",
                    "Confirm Delete",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning);

                if (result != System.Windows.Forms.DialogResult.Yes) return;

                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                _dbService.DeleteUser(row.Id);
                ShowInfo("User deleted.", "Success");
                RefreshData();
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete user.");
                System.Diagnostics.Debug.WriteLine($"ManageUserPage.btnDelete_Click error: {ex}");
            }
        }

        // Helper to raise navigation event
        private void Navigate(string targetPage, int? userId = null, string mode = null)
        {
            try
            {
                // FIX: pass the selected user's Id to the unified navigation args
                FireNavigate(targetPage, userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageUserPage.Navigate error: {ex}");
            }
        }
    }
}