/*
PAGE: ManageProductPage.cs
ROLE: Dealer Admin (Product Manager)
PURPOSE:
  CRUD management of products and their modules so GenerateLicensePage can reference product metadata (modules list, descriptions, release notes).

KEY UI ELEMENTS:
  - GridControl: list of products (ProductID, ProductName, CreatedBy, DateCreated, LastModified)
  - Buttons: Create, View, Edit, Delete, Search

BACKEND SERVICE CALLS:
  - ServiceRegistry.Database.GetProducts(), GetProductById(id)/GetProductByProductId(productId), InsertProduct(product), UpdateProduct(product), DeleteProduct(id)

VALIDATION & RULES:
  - ProductID uniqueness enforced
  - Module names non-empty
  - Only Admin role or product manager role can Create/Edit/Delete
  - When deleting a product, warn that existing licenses may reference this ProductID (deletion allowed but recommended to avoid)

ACCESS CONTROL:
  - ManageProduct page only visible to users with ManageProduct permission

UX NOTES:
  - Use inline validation for required fields
  - Modules grid supports add/remove rows easily (simple UI)

ACCEPTANCE CRITERIA:
  - Products persist to DB and appear in GenerateLicensePage module dropdown on refresh.
  - Module changes reflect in Generate license modules list.
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
    public partial class ManageProductPage : PageBase
    {
        private ILicenseDatabaseService _dbService;
        private BindingList<ProductRow> _data = new BindingList<ProductRow>();

        // Internal row model for grid binding
        private class ProductRow
        {
            public int Id { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string CreatedBy { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime LastModified { get; set; }
            public bool IsDeleted { get; set; } // NEW
        }

        public ManageProductPage()
        {
            InitializeComponent();

            try
            {
                if (!DesignMode)
                {
                    // Best-effort registry wiring
                    try { _dbService ??= ServiceRegistry.Database; } catch { }

                    // Wire events
                    btnCreate.Click += btnCreate_Click;
                    btnView.Click += btnView_Click;
                    btnEdit.Click += btnEdit_Click;

                    // Logic for Delete/Restore is handled in the handler now
                    btnDelete.Click += btnDelete_Click;

                    // NEW: Refresh button wires to RefreshData
                    if (btnRefresh != null)
                        btnRefresh.Click += (s, e) => RefreshData();

                    var view = grdProducts.MainView as GridView;
                    if (view != null)
                    {
                        view.DoubleClick += Grid_DoubleClick;
                        // NEW: row styling
                        view.RowStyle += GridView_RowStyle;

                        // --- FIX 2: Update Button Text on Row Change ---
                        view.FocusedRowChanged += (s, e) => UpdateActionButtons();
                    }

                    // Search interactions
                    txtSearch.ButtonClick += (s, e) => RefreshData();
                    txtSearch.KeyDown += (s, e) =>
                    {
                        if (e.KeyCode == Keys.Enter)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            RefreshData();
                        }
                    };

                    // NEW: Show Deleted checkbox toggle
                    if (chkShowDeleted != null)
                        chkShowDeleted.CheckedChanged += (s, e) => RefreshData();

                    // Initial load
                    RefreshData();

                    // Navigation buttons -> REPLACED with helper, matching GenerateLicensePage
                    if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                    if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                    if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                    if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                    if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");
                    // Logout bindings for panel + inner controls
                    if (btnNav_Logout != null) BindNavigationEvent(btnNav_Logout, "Logout");
                    if (lblNav_Logout != null) BindNavigationEvent(lblNav_Logout, "Logout");
                    if (picNav_Logout != null) BindNavigationEvent(picNav_Logout, "Logout");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageProductPage ctor error: {ex}");
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
                if (user == null) return;

                var role = user.Role ?? string.Empty;
                bool canEdit = role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                               || role.Equals("ProductManager", StringComparison.OrdinalIgnoreCase);

                btnCreate.Enabled = canEdit;
                btnEdit.Enabled = canEdit;
                btnDelete.Enabled = canEdit;
                btnView.Enabled = true;

                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user.CanGenerateLicense;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user.CanViewRecords;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user.CanManageProduct;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user.CanManageUsers;

                // Settings visible only for Admin (consistent with GeneralSettingPage)
                bool isAdmin = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);
                if (btnNav_GeneralSetting != null) btnNav_GeneralSetting.Visible = isAdmin;

                // Logout always visible
                if (btnNav_Logout != null) btnNav_Logout.Visible = true;

                // Ensure button state reflects current selection
                UpdateActionButtons();
            }
            catch { }
        }

        private void RefreshData()
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var all = _dbService.GetProducts() ?? Enumerable.Empty<Product>();

                var rows = all.Select(p => new ProductRow
                {
                    Id = p.Id,
                    ProductID = p.ProductID ?? string.Empty,
                    ProductName = p.Name ?? string.Empty,
                    CreatedBy = p.CreatedBy ?? string.Empty,
                    DateCreated = ToLocal(p.CreatedUtc),
                    LastModified = ToLocal(p.LastModifiedUtc == default ? p.CreatedUtc : p.LastModifiedUtc),
                    IsDeleted = p.IsDeleted // NEW
                });

                var query = rows;

                // Search filter
                var term = (txtSearch?.Text ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(r =>
                        (!string.IsNullOrEmpty(r.ProductID) && r.ProductID.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(r.ProductName) && r.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(r.CreatedBy) && r.CreatedBy.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
                }

                // NEW: Deleted filter
                if (!(chkShowDeleted?.Checked ?? false))
                {
                    query = query.Where(r => !r.IsDeleted);
                }

                _data = new BindingList<ProductRow>(query.ToList());
                grdProducts.DataSource = _data;

                // Best fit once after bind
                try
                {
                    var view = grdProducts.MainView as GridView;
                    view?.BestFitColumns();
                }
                catch { }

                // Update button label/state based on current selection
                UpdateActionButtons();
            }
            catch (Exception ex)
            {
                ShowError("Failed to load products.");
                System.Diagnostics.Debug.WriteLine($"ManageProductPage.RefreshData error: {ex}");
            }
        }

        private ProductRow GetFocusedRow()
        {
            try
            {
                var view = grdProducts.MainView as GridView;
                if (view == null) return null;
                return view.GetFocusedRow() as ProductRow;
            }
            catch { return null; }
        }

        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var row = GetFocusedRow();
                if (row == null) return;

                // Default to View on double-click
                Navigate("ProductDetailsPage", row.Id, "View");
            }
            catch (Exception ex)
            {
                ShowError("Failed to open product details.");
                System.Diagnostics.Debug.WriteLine($"Grid_DoubleClick error: {ex}");
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            Navigate("ProductDetailsPage", null, "Create");
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                ShowError("Please select a product.");
                return;
            }
            Navigate("ProductDetailsPage", row.Id, "View");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                ShowError("Please select a product.");
                return;
            }
            Navigate("ProductDetailsPage", row.Id, "Edit");
        }

        // --- FIX 3: Dynamic Button Logic ---
        private void UpdateActionButtons()
        {
            try
            {
                var row = GetFocusedRow();
                if (row == null)
                {
                    btnDelete.Enabled = false;
                    btnDelete.Text = "Delete";
                    btnEdit.Enabled = false;
                    return;
                }

                // Keep existing permission logic if you have it, otherwise default to true
                btnDelete.Enabled = true;

                if (row.IsDeleted)
                {
                    btnDelete.Text = "Restore"; // Change label
                    btnEdit.Enabled = false;    // Prevent editing while deleted
                }
                else
                {
                    btnDelete.Text = "Delete";
                    btnEdit.Enabled = true;
                }
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var row = GetFocusedRow();
                if (row == null) return;

                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                if (row.IsDeleted)
                {
                    // --- RESTORE LOGIC ---
                    var result = XtraMessageBox.Show(
                        $"Do you want to RESTORE product '{row.ProductID}'?\nIt will become active again.",
                        "Confirm Restore",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes) return;

                    var product = _dbService.GetProductById(row.Id);
                    if (product != null)
                    {
                        product.IsDeleted = false; // Undelete
                        product.LastModifiedUtc = DateTime.UtcNow;
                        _dbService.UpdateProduct(product);

                        ShowInfo("Product restored successfully.", "Success");
                        RefreshData();
                    }
                }
                else
                {
                    // --- DELETE LOGIC (Existing) ---
                    int licenseCount = 0;
                    try
                    {
                        // Check if licenses exist to show specific warning
                        licenseCount = (_dbService.GetLicenses(row.ProductID) ?? Enumerable.Empty<LicenseMetadata>()).Count();
                    }
                    catch { }

                    var warning = licenseCount > 0
                        ? $"\n\nWarning: {licenseCount} existing license(s) reference this ProductID."
                        : string.Empty;

                    var result = XtraMessageBox.Show(
                        $"Are you sure you want to delete product '{row.ProductID}'?{warning}",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes) return;

                    _dbService.DeleteProduct(row.Id);
                    ShowInfo("Product deleted.", "Success");
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Failed to update product status: {ex.Message}");
            }
        }

        // NEW: Row styling for deleted products
        private void GridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            var row = view.GetRow(e.RowHandle) as ProductRow;
            if (row == null) return;

            if (row.IsDeleted)
            {
                e.Appearance.BackColor = Color.LightCoral;
                e.Appearance.ForeColor = Color.DarkRed;
            }
        }

        // UPDATED: Use base class navigation event and map ProductId -> RecordId
        private void Navigate(string targetPage, int? productId, string mode)
        {
            try
            {
                FireNavigate(targetPage, productId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageProductPage.Navigate error: {ex}");
            }
        }
    }
}
