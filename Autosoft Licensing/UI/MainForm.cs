using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.UI.Pages;

namespace Autosoft_Licensing
{
    public partial class MainForm : XtraForm
    {
        private readonly Dictionary<string, PageBase> _pageCache = new Dictionary<string, PageBase>(StringComparer.OrdinalIgnoreCase);
        private User _currentUser;
        private bool _suppressMessageBoxes = false;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                if (this.accordion != null)
                {
                    this.accordion.Visible = false;
                    this.accordion.Enabled = false;
                }
            }
            catch { }

            ShowLogin();
        }

        public User LoggedInUser { get; private set; }

        public void SetLoggedInUser(User user)
        {
            LoggedInUser = user;
            _currentUser = user;
            UpdateRoleVisibility();
        }

        // NEW: Public navigation method for external callers (e.g., Program.cs)
        public void NavigateToPage(string pageName)
        {
            if (string.IsNullOrWhiteSpace(pageName))
                return;

            LoadPage(pageName, pageName);
        }

        private void ShowLogin()
        {
            try
            {
                _currentUser = null;
                LoggedInUser = null;

                if (this.contentPanel == null)
                {
                    BuildAccordion();
                }

                contentPanel.Controls.Clear();

                var login = new LoginPage();
                login.Initialize(ServiceRegistry.Database, ServiceRegistry.Encryption);
                login.InitializeForRole(null);

                login.LoginSuccess += OnLoginSuccess;

                contentPanel.Controls.Add(login);
                login.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Failed to show login: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnLoginSuccess(object sender, User user)
        {
            try
            {
                _currentUser = user;
                LoggedInUser = user;

                string startPage = null;
                if (user.CanGenerateLicense) startPage = "GenerateLicensePage";
                else if (user.CanViewRecords) startPage = "LicenseRecordsPage";
                else if (user.CanManageProduct) startPage = "ManageProductPage";
                else if (user.CanManageUsers) startPage = "ManageUserPage";
                else startPage = "LicenseRecordsPage";

                LoadPage(startPage, startPage);
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Login succeeded but navigation failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadPage(string elementName, string elementText)
        {
            if (string.IsNullOrEmpty(elementName))
                return;

            if (this.contentPanel == null)
            {
                BuildAccordion();
            }

            string key = elementName;
            if (elementName.Equals("aceManageProduct", StringComparison.OrdinalIgnoreCase))
                key = "ManageProductPage";
            else if (elementName.Equals("aceUserManagement", StringComparison.OrdinalIgnoreCase))
                key = "ManageUserPage";
            else if (elementName.Equals("aceLicenseList", StringComparison.OrdinalIgnoreCase))
                key = "LicenseRecordsPage";
            else if (elementName.Equals("aceGenerateRequest", StringComparison.OrdinalIgnoreCase) ||
                     elementName.Equals("aceGenerateLicense", StringComparison.OrdinalIgnoreCase) ||
                     elementName.Equals("btnNav_GenerateLicense", StringComparison.OrdinalIgnoreCase))
                key = "GenerateLicensePage";

            if (!_pageCache.TryGetValue(key, out var page))
            {
                switch (key)
                {
                    case "GenerateLicensePage":
                    {
                        var p = new GenerateLicensePage();
                        try
                        {
                            // Verified: Initialize with ArlReader, AslGenerator, Product, Database, User
                            p.Initialize(
                                ServiceRegistry.ArlReader,
                                ServiceRegistry.AslGenerator,
                                ServiceRegistry.Product,
                                ServiceRegistry.Database,
                                ServiceRegistry.User);
                        }
                        catch { }
                        page = p;
                        break;
                    }

                    case "LicenseRecordsPage":
                    {
                        var p = new LicenseRecordsPage();
                        try
                        {
                            p.Initialize(ServiceRegistry.Database, ServiceRegistry.User);
                        }
                        catch { }
                        page = p;
                        break;
                    }

                    case "LicenseRecordDetailsPage":
                        page = new LicenseRecordDetailsPage();
                        break;

                    case "ManageProductPage":
                    {
                        var p = new ManageProductPage();
                        try
                        {
                            p.Initialize(ServiceRegistry.Database);
                        }
                        catch { }
                        page = p;
                        break;
                    }

                    case "ManageUserPage":
                    {
                        var p = new ManageUserPage();
                        try
                        {
                            p.Initialize(ServiceRegistry.Database);
                        }
                        catch { }
                        page = p;
                        break;
                    }

                    default:
                        page = new GenericPage(elementText ?? key);
                        break;
                }

                // Verified: subscribe using Autosoft_Licensing.UI.Pages.NavigateEventArgs
                page.NavigateRequested += new EventHandler<Autosoft_Licensing.UI.Pages.NavigateEventArgs>(OnPageNavigate);

                _pageCache[key] = page;
            }

            contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(page);

            try
            {
                page.InitializeForRole(_currentUser);
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Failed to initialize page: " + ex.Message, "Page error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnPageNavigate(object sender, Autosoft_Licensing.UI.Pages.NavigateEventArgs e)
        {
            try
            {
                if (e == null || string.IsNullOrEmpty(e.TargetPage))
                    return;

                // Handle Logout
                if (string.Equals(e.TargetPage, "Logout", StringComparison.OrdinalIgnoreCase))
                {
                    _currentUser = null;
                    LoggedInUser = null;
                    _pageCache.Clear();
                    ShowLogin();
                    return;
                }

                // Handle Product Details
                if (string.Equals(e.TargetPage, "ProductDetailsPage", StringComparison.OrdinalIgnoreCase))
                {
                    var details = new ProductDetailsPage();
                    try
                    {
                        // Initialize with the unified RecordId
                        details.Initialize(e.RecordId);

                        // Wire back navigation to ManageProductPage
                        details.NavigateBackRequested += (s, args) => LoadPage("ManageProductPage", "ManageProductPage");

                        // Role-based setup
                        details.InitializeForRole(_currentUser);
                    }
                    catch { /* best-effort */ }

                    ShowPage(details);
                    return;
                }

                // Handle User Details
                if (string.Equals(e.TargetPage, "UserDetailsPage", StringComparison.OrdinalIgnoreCase))
                {
                    var details = new UserDetailsPage();
                    try
                    {
                        // Initialize with the unified RecordId
                        details.Initialize(e.RecordId);

                        // Wire back navigation to ManageUserPage
                        details.NavigateBackRequested += (s, args) => LoadPage("ManageUserPage", "ManageUserPage");

                        // Role-based setup
                        details.InitializeForRole(_currentUser);
                    }
                    catch { /* best-effort */ }

                    ShowPage(details);
                    return;
                }

                // Default: Load standard menu pages by name
                LoadPage(e.TargetPage, e.TargetPage);
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show($"Navigation failed: {ex.Message}", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateRoleVisibility()
        {
            try
            {
                if (this.accordion != null)
                {
                    this.accordion.Visible = false;
                    this.accordion.Enabled = false;
                }
            }
            catch { }
        }
    }
}