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

        // Navigation event - host form can subscribe
        public event EventHandler<NavigateEventArgs> NavigateRequested;

        public class NavigateEventArgs : EventArgs
        {
            public string TargetPage { get; set; }
            public int? ProductId { get; set; }
            public string Mode { get; set; } // "Create", "View", "Edit"
        }

        // Internal row model for grid binding
        private class ProductRow
        {
            public int Id { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string CreatedBy { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime LastModified { get; set; }
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
                    btnDelete.Click += btnDelete_Click;

                    // NEW: Refresh button wires to RefreshData
                    if (btnRefresh != null)
                        btnRefresh.Click += (s, e) => RefreshData();

                    var view = grdProducts.MainView as GridView;
                    if (view != null)
                    {
                        view.DoubleClick += Grid_DoubleClick;
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

                    // Initial load
                    RefreshData();
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
                if (user == null)
                    return;

                // Allow Admin (and optional "ProductManager") to modify
                var role = user.Role ?? string.Empty;
                bool canEdit = role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                               || role.Equals("ProductManager", StringComparison.OrdinalIgnoreCase);

                btnCreate.Enabled = canEdit;
                btnEdit.Enabled = canEdit;
                btnDelete.Enabled = canEdit;
                btnView.Enabled = true;
            }
            catch { /* ignore */ }
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
                    LastModified = ToLocal(p.LastModifiedUtc == default ? p.CreatedUtc : p.LastModifiedUtc)
                });

                var query = rows;

                var term = (txtSearch?.Text ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(r =>
                        (!string.IsNullOrEmpty(r.ProductID) && r.ProductID.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(r.ProductName) && r.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(r.CreatedBy) && r.CreatedBy.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var row = GetFocusedRow();
                if (row == null)
                {
                    ShowError("Please select a product.");
                    return;
                }

                // Optional warning: check if licenses exist for this ProductID
                int licenseCount = 0;
                try
                {
                    if (_dbService != null)
                        licenseCount = (_dbService.GetLicenses(row.ProductID) ?? Enumerable.Empty<LicenseMetadata>()).Count();
                }
                catch { /* ignore */ }

                var warning = licenseCount > 0
                    ? $"\n\nWarning: {licenseCount} existing license(s) reference this ProductID."
                    : string.Empty;

                var result = XtraMessageBox.Show(
                    $"Are you sure you want to delete product '{row.ProductID}'?{warning}",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                _dbService.DeleteProduct(row.Id);
                ShowInfo("Product deleted.", "Success");
                RefreshData();
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete product.");
                System.Diagnostics.Debug.WriteLine($"btnDelete_Click error: {ex}");
            }
        }

        private void Navigate(string targetPage, int? productId, string mode)
        {
            try
            {
                NavigateRequested?.Invoke(this, new NavigateEventArgs
                {
                    TargetPage = targetPage,
                    ProductId = productId,
                    Mode = mode
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ManageProductPage.Navigate error: {ex}");
                ShowError($"Failed to navigate to {targetPage}.");
            }
        }
    }
}
