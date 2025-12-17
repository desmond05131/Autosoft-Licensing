/*
PAGE: ManageUserPage.cs
ROLE: Dealer Admin (super-admin)
PURPOSE:
  Manage Dealer EXE user accounts and their access rights to the Dealer app.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
                    btnEdit.Click += btnEdit_Click;
                    btnDelete.Click += btnDelete_Click;
                    btnRefresh.Click += btnRefresh_Click; // <--- ADDED

                    // Navigation buttons
                    if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                    if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                    if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                    if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                    if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");

                    // Logout
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

                        // Double Click Event
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
                btnRefresh.Enabled = true; // Refresh always allowed if you can see the page

                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user?.CanGenerateLicense ?? false;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user?.CanViewRecords ?? false;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user?.CanManageProduct ?? false;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user?.CanManageUsers ?? false;

                // Settings Admin-only
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

                var row = view.GetFocusedRow() as UserRow;
                if (row != null) return row;

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

        private void btnRefresh_Click(object sender, EventArgs e) // <--- ADDED
        {
            RefreshData();
        }

        private void GrdUsers_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;

                var pt = view.GridControl.PointToClient(Control.MousePosition);
                var hit = view.CalcHitInfo(pt);

                if (hit.InRow || hit.InRowCell)
                {
                    var row = GetFocusedRow();
                    if (row != null)
                    {
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

        private void Navigate(string targetPage, int? userId = null, string mode = null)
        {
            try
            {
                FireNavigate(targetPage, userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageUserPage.Navigate error: {ex}");
            }
        }
    }
}