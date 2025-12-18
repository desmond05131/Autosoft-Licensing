using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.UI.Pages;
using System.Linq;
using DevExpress.XtraBars.Navigation;

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
            // Update the hosting state and role visibility first
            LoggedInUser = user;
            _currentUser = user;

            try
            {
                // Update navigation / accordion visibility based on the new user
                UpdateRoleVisibility();
            }
            catch { /* best-effort */ }

            try
            {
                // Enforce "blank slate" behaviour:
                // - If the user has at least one permission, navigate to the highest-priority allowed page.
                // - If the user has no permissions, DO NOT leave the previous page visible; show a safe blank/welcome page.
                if (user == null)
                {
                    // No user - show login
                    ShowLogin();
                    return;
                }

                bool canAny = user.CanGenerateLicense || user.CanViewRecords || user.CanManageProduct || user.CanManageUsers;
                if (!canAny)
                {
                    ShowBlankState("Welcome", "You do not have access to any modules.");
                    return;
                }

                // If host code explicitly sets a user (Program.cs launch-as-admin), navigate to appropriate start page.
                if (user.CanGenerateLicense)
                {
                    NavigateToPage("GenerateLicensePage");
                }
                else if (user.CanViewRecords)
                {
                    NavigateToPage("LicenseRecordsPage");
                }
                else if (user.CanManageProduct)
                {
                    NavigateToPage("ManageProductPage");
                }
                else if (user.CanManageUsers)
                {
                    NavigateToPage("ManageUserPage");
                }
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Failed to set logged in user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

                // Ensure navigation/visibility is updated immediately
                UpdateRoleVisibility();

                // Decide start page according to permissions. If none -> show blank slate.
                if (user == null)
                {
                    ShowLogin();
                    return;
                }

                string startPage = null;
                if (user.CanGenerateLicense) startPage = "GenerateLicensePage";
                else if (user.CanViewRecords) startPage = "LicenseRecordsPage";
                else if (user.CanManageProduct) startPage = "ManageProductPage";
                else if (user.CanManageUsers) startPage = "ManageUserPage";
                else
                {
                    // CRITICAL FIX: user has no permissions -> present blank/welcome state instead of leaving previous page visible.
                    ShowBlankState("Welcome", "You do not have access to any modules.");
                    return;
                }

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
            else if (elementName.Equals("aceSettingsSecurity", StringComparison.OrdinalIgnoreCase))
                key = "GeneralSettingPage";

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

                    case "GeneralSettingPage":
                    {
                        var p = new GeneralSettingPage();
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

                // NEW: Handle License Record Details and pass LicenseId + wire back navigation
                if (string.Equals(e.TargetPage, "LicenseRecordDetailsPage", StringComparison.OrdinalIgnoreCase))
                {
                    var details = new LicenseRecordDetailsPage();
                    try
                    {
                        // 1. Pass the RecordId (License ID) from the grid event
                        if (e.RecordId.HasValue)
                        {
                            details.Initialize(e.RecordId.Value);
                        }

                        // 2. Wire the navigation event so "Back" works
                        details.NavigateRequested += OnPageNavigate;

                        // 3. Apply roles
                        details.InitializeForRole(_currentUser);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error init details: {ex}");
                    }

                    // 4. Show the page
                    contentPanel.Controls.Clear();
                    details.Dock = DockStyle.Fill;
                    contentPanel.Controls.Add(details);
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
                // If accordion isn't ready yet, ensure BuildAccordion will configure visibility later.
                if (this.accordion == null)
                {
                    // Nothing to update now
                    return;
                }

                // If no user logged in, hide/disable navigation
                if (_currentUser == null)
                {
                    try
                    {
                        this.accordion.Visible = false;
                        this.accordion.Enabled = false;
                    }
                    catch { }
                    return;
                }

                // Ensure accordion shown for logged in users
                try
                {
                    this.accordion.Visible = true;
                    this.accordion.Enabled = true;
                }
                catch { }

                // Locate navigation group and its elements (best-effort)
                try
                {
                    var navGroup = this.accordion.Elements.FirstOrDefault(e => string.Equals(e.Name, "aceNavigation", StringComparison.OrdinalIgnoreCase));
                    if (navGroup != null)
                    {
                        // Helper to set element visibility by element name
                        void SetVisible(string elementName, bool visible)
                        {
                            try
                            {
                                var el = navGroup.Elements.FirstOrDefault(x => string.Equals(x.Name, elementName, StringComparison.OrdinalIgnoreCase));
                                if (el != null) el.Visible = visible;
                            }
                            catch { /* ignore element-level failures */ }
                        }

                        SetVisible("aceGenerateRequest", _currentUser.CanGenerateLicense);
                        SetVisible("aceLicenseList", _currentUser.CanViewRecords);
                        SetVisible("aceManageProduct", _currentUser.CanManageProduct);
                        SetVisible("aceUserManagement", _currentUser.CanManageUsers);
                        SetVisible("aceSettingsSecurity", string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));

                        // Always keep Dashboard visible as a neutral landing page
                        SetVisible("aceDashboard", true);
                    }
                }
                catch { /* ignore */ }
            }
            catch { }
        }

        /// <summary>
        /// Show a minimal blank/welcome page and hide interactive navigation to enforce a "blank slate" for users with no permissions.
        /// </summary>
        private void ShowBlankState(string title, string message)
        {
            try
            {
                if (this.contentPanel == null)
                {
                    BuildAccordion();
                }

                // Hide/disable the accordion so there is no visible navigation to other pages
                try
                {
                    if (this.accordion != null)
                    {
                        this.accordion.Visible = false;
                        this.accordion.Enabled = false;
                    }
                }
                catch { }

                contentPanel.Controls.Clear();

                // Use GenericPage to present a neutral page. Put the message in the page title for clarity.
                var welcomeTitle = string.IsNullOrWhiteSpace(message) ? title ?? "Welcome" : $"{title} - {message}";
                var page = new GenericPage(welcomeTitle);
                page.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(page);

                // Apply role information (if any) so page can decide what to show
                try { page.InitializeForRole(_currentUser); } catch { }
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Failed to show blank state: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}