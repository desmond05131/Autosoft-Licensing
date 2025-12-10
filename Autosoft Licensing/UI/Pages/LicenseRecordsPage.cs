/*
PAGE: LicenseRecordsPage.cs
ROLE: Dealer Admin / Support
PURPOSE:
  Master view and search/filter UI for previously generated license records. Supports filtering, color-coded status display, and actions: View, Edit, Delete, Create.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class LicenseRecordsPage : PageBase
    {
        // Services (injected or pulled from ServiceRegistry)
        private ILicenseDatabaseService _dbService;
        private IUserService _userService;

        // Internal data
        private List<LicenseRecordRow> _dataSource;

        // Internal row model for grid binding
        private class LicenseRecordRow
        {
            public int LicenseId { get; set; }
            public string CompanyName { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string LicenseType { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpiryDate { get; set; }
            public string Status { get; set; }
            public string Countdown { get; set; }
            public int CountdownDays { get; set; } // for coloring logic
        }

        public LicenseRecordsPage()
        {
            InitializeComponent();

            try
            {
                if (!DesignMode)
                {
                    // FIX 1: Initialize Services
                    try { _dbService ??= ServiceRegistry.Database; } catch { }
                    try { _userService ??= ServiceRegistry.User; } catch { }

                    // Standardized navigation wiring
                    if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                    if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                    if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                    if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                    if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");

                    // Logout (panel + inner label + picture)
                    if (btnNav_Logout != null) BindNavigationEvent(btnNav_Logout, "Logout");
                    if (lblNav_Logout != null) BindNavigationEvent(lblNav_Logout, "Logout");
                    if (picNav_Logout != null) BindNavigationEvent(picNav_Logout, "Logout");

                    // FIX 2: Wire up Action Buttons
                    if (btnRefresh != null) btnRefresh.Click += btnRefresh_Click;
                    if (btnCreate != null) btnCreate.Click += btnCreate_Click;
                    if (btnView != null) btnView.Click += btnView_Click;
                    if (btnEdit != null) btnEdit.Click += btnEdit_Click;
                    if (btnDelete != null) btnDelete.Click += btnDelete_Click;

                    // FIX 3: Wire up Grid Events
                    if (grdLicenses != null && grdLicenses.MainView is GridView view)
                    {
                        view.DoubleClick += grdLicenses_DoubleClick;
                    }

                    // FIX 4: Load Data
                    InitializeFilterCombos();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LicenseRecordsPage ctor error: {ex}");
            }
        }

        /// <summary>
        /// Initialize filter combos with data from database.
        /// </summary>
        private void InitializeFilterCombos()
        {
            try
            {
                // Company Name combo: populate from distinct companies in Licenses
                if (_dbService != null)
                {
                    try
                    {
                        var licenses = _dbService.GetLicenses() ?? Enumerable.Empty<LicenseMetadata>();
                        var companies = licenses
                            .Where(l => !string.IsNullOrWhiteSpace(l.CompanyName))
                            .Select(l => l.CompanyName)
                            .Distinct()
                            .OrderBy(c => c)
                            .ToList();

                        cmbCompanyName.Properties.Items.Clear();
                        cmbCompanyName.Properties.Items.Add("All");
                        if (companies.Any())
                            cmbCompanyName.Properties.Items.AddRange(companies);
                        cmbCompanyName.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"InitializeFilterCombos companies error: {ex}");
                        // Fallback to minimal setup
                        cmbCompanyName.Properties.Items.Clear();
                        cmbCompanyName.Properties.Items.Add("All");
                        cmbCompanyName.SelectedIndex = 0;
                    }
                }
                else
                {
                    // No database service - set up minimal filter
                    cmbCompanyName.Properties.Items.Clear();
                    cmbCompanyName.Properties.Items.Add("All");
                    cmbCompanyName.SelectedIndex = 0;
                }

                // Product Code combo: populate from Products table
                if (_dbService != null)
                {
                    try
                    {
                        var products = _dbService.GetProducts() ?? Enumerable.Empty<Product>();
                        var codes = products
                            .Where(p => !string.IsNullOrWhiteSpace(p.ProductID))
                            .Select(p => p.ProductID)
                            .OrderBy(c => c)
                            .ToList();

                        cmbProductCode.Properties.Items.Clear();
                        cmbProductCode.Properties.Items.Add("All");
                        if (codes.Any())
                            cmbProductCode.Properties.Items.AddRange(codes);
                        cmbProductCode.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"InitializeFilterCombos products error: {ex}");
                        // Fallback to minimal setup
                        cmbProductCode.Properties.Items.Clear();
                        cmbProductCode.Properties.Items.Add("All");
                        cmbProductCode.SelectedIndex = 0;
                    }
                }
                else
                {
                    // No database service - set up minimal filter
                    cmbProductCode.Properties.Items.Clear();
                    cmbProductCode.Properties.Items.Add("All");
                    cmbProductCode.SelectedIndex = 0;
                }

                // License Type combo: All, Demo, Subscription, Permanent
                cmbLicenseType.Properties.Items.Clear();
                cmbLicenseType.Properties.Items.AddRange(new[] { "All", "Demo", "Subscription", "Permanent" });
                cmbLicenseType.SelectedIndex = 0;

                // Issue Date / Expiry Date mode combos
                cmbIssueDateMode.Properties.Items.Clear();
                cmbIssueDateMode.Properties.Items.AddRange(new[] { "By date", "By range" });
                cmbIssueDateMode.SelectedIndex = 0;

                cmbExpiryDateMode.Properties.Items.Clear();
                cmbExpiryDateMode.Properties.Items.AddRange(new[] { "By date", "By range" });
                cmbExpiryDateMode.SelectedIndex = 0;

                // Ensure date editors start empty so no date filters are applied by default
                dtIssueDateSingle.EditValue = null;
                dtIssueDateFrom.EditValue = null;
                dtIssueDateTo.EditValue = null;
                dtExpiryDateSingle.EditValue = null;
                dtExpiryDateFrom.EditValue = null;
                dtExpiryDateTo.EditValue = null;

                // Show/hide date controls based on mode
                UpdateDateControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeFilterCombos error: {ex}");
            }
        }

        /// <summary>
        /// Show/hide date controls based on selected mode (By date vs By range).
        /// </summary>
        private void UpdateDateControls()
        {
            try
            {
                // Issue Date
                bool issueDateByRange = cmbIssueDateMode.SelectedIndex == 1;
                dtIssueDateSingle.Visible = !issueDateByRange;
                dtIssueDateFrom.Visible = issueDateByRange;
                dtIssueDateTo.Visible = issueDateByRange;
                lblIssueDateFrom.Visible = issueDateByRange;
                lblIssueDateTo.Visible = issueDateByRange;

                // Expiry Date
                bool expiryDateByRange = cmbExpiryDateMode.SelectedIndex == 1;
                dtExpiryDateSingle.Visible = !expiryDateByRange;
                dtExpiryDateFrom.Visible = expiryDateByRange;
                dtExpiryDateTo.Visible = expiryDateByRange;
                lblExpiryDateFrom.Visible = expiryDateByRange;
                lblExpiryDateTo.Visible = expiryDateByRange;
            }
            catch { }
        }

        private void DateModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDateControls();
        }

        /// <summary>
        /// Refresh grid data with current filter values.
        /// </summary>
        private void RefreshData()
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                // Fetch all licenses (or filter by product if needed)
                var allLicenses = _dbService.GetLicenses().ToList();

                // Apply filters
                var filtered = allLicenses.AsEnumerable();

                // Company Name filter
                if (cmbCompanyName.SelectedIndex > 0 && cmbCompanyName.Text != "All")
                {
                    var selectedCompany = cmbCompanyName.Text;
                    filtered = filtered.Where(l => string.Equals(l.CompanyName, selectedCompany, StringComparison.OrdinalIgnoreCase));
                }

                // Product Code filter
                if (cmbProductCode.SelectedIndex > 0 && cmbProductCode.Text != "All")
                {
                    var selectedProduct = cmbProductCode.Text;
                    filtered = filtered.Where(l => string.Equals(l.ProductID, selectedProduct, StringComparison.OrdinalIgnoreCase));
                }

                // License Type filter
                if (cmbLicenseType.SelectedIndex > 0 && cmbLicenseType.Text != "All")
                {
                    var selectedType = cmbLicenseType.Text;
                    filtered = filtered.Where(l => string.Equals(l.LicenseType.ToString(), selectedType, StringComparison.OrdinalIgnoreCase));
                }

                // Issue Date filter
                if (cmbIssueDateMode.SelectedIndex == 0) // By date
                {
                    if (dtIssueDateSingle.EditValue != null)
                    {
                        var issueDate = dtIssueDateSingle.DateTime.Date;
                        filtered = filtered.Where(l => l.ValidFromUtc.ToLocalTime().Date == issueDate);
                    }
                }
                else // By range
                {
                    if (dtIssueDateFrom.EditValue != null && dtIssueDateTo.EditValue != null)
                    {
                        var from = dtIssueDateFrom.DateTime.Date;
                        var to = dtIssueDateTo.DateTime.Date;
                        filtered = filtered.Where(l =>
                        {
                            var localDate = l.ValidFromUtc.ToLocalTime().Date;
                            return localDate >= from && localDate <= to;
                        });
                    }
                }

                // Expiry Date filter
                if (cmbExpiryDateMode.SelectedIndex == 0) // By date
                {
                    if (dtExpiryDateSingle.EditValue != null)
                    {
                        var expiryDate = dtExpiryDateSingle.DateTime.Date;
                        filtered = filtered.Where(l => l.ValidToUtc.ToLocalTime().Date == expiryDate);
                    }
                }
                else // By range
                {
                    if (dtExpiryDateFrom.EditValue != null && dtExpiryDateTo.EditValue != null)
                    {
                        var from = dtExpiryDateFrom.DateTime.Date;
                        var to = dtExpiryDateTo.DateTime.Date;
                        filtered = filtered.Where(l =>
                        {
                            var localDate = l.ValidToUtc.ToLocalTime().Date;
                            return localDate >= from && localDate <= to;
                        });
                    }
                }

                // Show expired license checkbox
                if (!chkShowExpired.Checked)
                {
                    var todayLocal = DateTime.Now.Date; // compare using local date since we convert ValidToUtc to local
                    filtered = filtered.Where(l => l.ValidToUtc.ToLocalTime().Date >= todayLocal);
                }

                // Filter out deleted licenses (soft delete)
                filtered = filtered.Where(l => l.Status != LicenseStatus.Deleted);

                // Map to row model
                var todayLocal2 = DateTime.Now.Date;
                _dataSource = filtered.Select(l =>
                {
                    var expiryLocal = l.ValidToUtc.ToLocalTime().Date;
                    var issueLocal = l.ValidFromUtc.ToLocalTime().Date;
                    var countdownDays = (expiryLocal - todayLocal2).Days;

                    // Use persisted status string from DB
                    var status = l.Status.ToString();

                    // If DB status is Valid but expiry has passed, reflect Expired for UI clarity
                    if (expiryLocal < todayLocal2)
                        status = "Expired";

                    var countdownText = countdownDays < 0 ? "Expired" : countdownDays.ToString();

                    // Get product name from ProductService (best-effort)
                    string productName = l.ProductID;
                    try
                    {
                        if (ServiceRegistry.Product != null)
                            productName = ServiceRegistry.Product.GetProductName(l.ProductID) ?? l.ProductID;
                    }
                    catch { }

                    return new LicenseRecordRow
                    {
                        LicenseId = l.Id,
                        CompanyName = l.CompanyName,
                        ProductCode = l.ProductID,
                        ProductName = productName,
                        LicenseType = l.LicenseType.ToString(),
                        IssueDate = issueLocal,
                        ExpiryDate = expiryLocal,
                        Status = status,
                        Countdown = countdownText,
                        CountdownDays = countdownDays
                    };
                }).ToList();

                // Bind to grid
                grdLicenses.DataSource = _dataSource;

                // Apply row coloring
                var view = grdLicenses.MainView as GridView;
                if (view != null)
                {
                    view.RowStyle -= GridView_RowStyle; // remove old handler
                    view.RowStyle += GridView_RowStyle;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load license records.");
                System.Diagnostics.Debug.WriteLine($"RefreshData error: {ex}");
            }
        }

        /// <summary>
        /// Apply row coloring: Expired = Red, Soon-to-expire (≤ selected countdown days) = Yellow, Active = Normal.
        /// </summary>
        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null || e.RowHandle < 0) return;

                var row = view.GetRow(e.RowHandle) as LicenseRecordRow;
                if (row == null) return;

                if (row.Status == "Deleted")
                {
                    e.Appearance.BackColor = Color.LightGray;
                    e.Appearance.ForeColor = Color.DimGray;
                }
                else if (row.Status == "Expired")
                {
                    e.Appearance.BackColor = Color.LightCoral;
                    e.Appearance.ForeColor = Color.DarkRed;
                }
                else
                {
                    var threshold = (int)numCountdownDays.Value;
                    if (threshold > 0 && row.CountdownDays <= threshold && row.CountdownDays >= 0)
                    {
                        e.Appearance.BackColor = Color.LightYellow;
                        e.Appearance.ForeColor = Color.DarkOrange;
                    }
                }
            }
            catch { }
        }

        #region Event Handlers

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void grdLicenses_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = grdLicenses.MainView as GridView;
                if (view == null) return;

                var row = view.GetFocusedRow() as LicenseRecordRow;
                if (row == null) return;

                // Navigate to LicenseRecordDetailsPage with row.LicenseId
                Navigate("LicenseRecordDetailsPage", row.LicenseId, isReadOnly: true);
            }
            catch (Exception ex)
            {
                ShowError("Failed to open license details.");
                System.Diagnostics.Debug.WriteLine($"grdLicenses_DoubleClick error: {ex}");
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                // Navigate to GenerateLicensePage
                Navigate("GenerateLicensePage");
            }
            catch (Exception ex)
            {
                ShowError("Failed to navigate to Generate License page.");
                System.Diagnostics.Debug.WriteLine($"btnCreate_Click error: {ex}");
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                var view = grdLicenses.MainView as GridView;
                if (view == null) return;

                var row = view.GetFocusedRow() as LicenseRecordRow;
                if (row == null)
                {
                    ShowError("Please select a license record.");
                    return;
                }

                // Navigate to LicenseRecordDetailsPage (read-only mode)
                Navigate("LicenseRecordDetailsPage", row.LicenseId, isReadOnly: true);
            }
            catch (Exception ex)
            {
                ShowError("Failed to view license details.");
                System.Diagnostics.Debug.WriteLine($"btnView_Click error: {ex}");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var view = grdLicenses.MainView as GridView;
                if (view == null) return;

                var row = view.GetFocusedRow() as LicenseRecordRow;
                if (row == null)
                {
                    ShowError("Please select a license record.");
                    return;
                }

                // Navigate to LicenseRecordDetailsPage with edit mode
                Navigate("LicenseRecordDetailsPage", row.LicenseId, isReadOnly: false);
            }
            catch (Exception ex)
            {
                ShowError("Failed to edit license.");
                System.Diagnostics.Debug.WriteLine($"btnEdit_Click error: {ex}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var view = grdLicenses.MainView as GridView;
                if (view == null) return;

                var row = view.GetFocusedRow() as LicenseRecordRow;
                if (row == null)
                {
                    ShowError("Please select a license record.");
                    return;
                }

                // Confirm delete (Admin only)
                var result = XtraMessageBox.Show(
                    $"Are you sure you want to delete the license for {row.CompanyName} - {row.ProductCode}?\n\nThis will mark the license as deleted and it will no longer be active.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                // Soft delete
                if (_dbService != null)
                {
                    _dbService.UpdateLicenseStatus(row.LicenseId, LicenseStatus.Deleted.ToString());
                    ShowInfo("License deleted successfully.", "Success");
                    RefreshData();
                }
                else
                {
                    ShowError("Database service not initialized.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete license.");
                System.Diagnostics.Debug.WriteLine($"btnDelete_Click error: {ex}");
            }
        }

        #endregion

        /// <summary>
        /// Initialize services (called by host if needed).
        /// </summary>
        public void Initialize(ILicenseDatabaseService dbService, IUserService userService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Apply role-based visibility (Admin vs Support).
        /// </summary>
        public override void InitializeForRole(User user)
        {
            if (user == null) return;

            if (btnNav_Logout != null) btnNav_Logout.Visible = true;
            if (btnNav_GeneralSetting != null) btnNav_GeneralSetting.Visible = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

            // Delete only allowed for Admin
            if (btnDelete != null)
            {
                btnDelete.Enabled = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Helper to trigger navigation using base class event.
        /// </summary>
        private void Navigate(string targetPage, int? licenseId = null, bool isReadOnly = false)
        {
            try
            {
                FireNavigate(targetPage, licenseId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigate error: {ex}");
                ShowError($"Failed to navigate to {targetPage}.");
            }
        }
    }
}