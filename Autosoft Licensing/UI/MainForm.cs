using System;
using System.Collections.Generic;
using System.Linq;
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

            // Update navigation visibility immediately
            UpdateRoleVisibility();

            // Enforce "blank slate" for users with no permissions
            try
            {
                bool hasAnyPermission = _currentUser != null &&
                                        (_currentUser.CanGenerateLicense ||
                                         _currentUser.CanViewRecords ||
                                         _currentUser.CanManageProduct ||
                                         _currentUser.CanManageUsers ||
                                         string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));

                if (!hasAnyPermission)
                {
                    // Clear any cached pages that might expose UI
                    _pageCache.Clear();

                    // Hide/disable navigation
                    try
                    {
                        if (this.accordion != null)
                        {
                            this.accordion.Visible = false;
                            this.accordion.Enabled = false;
                        }
                    }
                    catch { }

                    // Show a safe blank/welcome page
                    var blank = new GenericPage("Welcome - You do not have access to any modules.");
                    ShowPage(blank);
                    try { blank.InitializeForRole(_currentUser); } catch { }
                }
            }
            catch { /* best-effort */ }
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

                // Update role visibility on UI
                UpdateRoleVisibility();

                // Decide start page based on permissions.
                // CRITICAL: If the user has no permissions, show a blank/welcome page instead of the default records view.
                bool hasGenerate = user?.CanGenerateLicense ?? false;
                bool hasView = user?.CanViewRecords ?? false;
                bool hasManageProduct = user?.CanManageProduct ?? false;
                bool hasManageUsers = user?.CanManageUsers ?? false;
                bool isAdmin = string.Equals(user?.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                if (hasGenerate)
                {
                    LoadPage("GenerateLicensePage", "Generate License");
                }
                else if (hasView)
                {
                    LoadPage("LicenseRecordsPage", "License List");
                }
                else if (hasManageProduct)
                {
                    LoadPage("ManageProductPage", "Manage Product");
                }
                else if (hasManageUsers)
                {
                    LoadPage("ManageUserPage", "User Management");
                }
                else if (isAdmin)
                {
                    // Admin fallback - show ManageUser to allow user admin actions
                    LoadPage("ManageUserPage", "User Management");
                }
                else
                {
                    // No permissions: enforce blank slate
                    _pageCache.Clear();

                    try
                    {
                        if (this.accordion != null)
                        {
                            this.accordion.Visible = false;
                            this.accordion.Enabled = false;
                        }
                    }
                    catch { }

                    var blank = new GenericPage("Welcome - You do not have access to any modules.");
                    ShowPage(blank);
                    try { blank.InitializeForRole(_currentUser); } catch { }
                }
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
                if (this.accordion == null)
                    return;

                // When no user is set, hide navigation entirely
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

                // Find the Navigation group and adjust each element's visibility by permission
                try
                {
                    var navGroup = this.accordion.Elements
                        .FirstOrDefault(e => string.Equals(e.Name, "aceNavigation", StringComparison.OrdinalIgnoreCase));

                    if (navGroup == null)
                    {
                        // Fallback: make accordion visible if user has any rights
                        this.accordion.Visible = (_currentUser.CanGenerateLicense || _currentUser.CanViewRecords ||
                                                  _currentUser.CanManageProduct || _currentUser.CanManageUsers ||
                                                  string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));
                        this.accordion.Enabled = this.accordion.Visible;
                        return;
                    }

                    // Helper to set element visibility safely
                    void SetVisible(string elementName, bool visible)
                    {
                        try
                        {
                            var el = navGroup.Elements
                                .FirstOrDefault(x => string.Equals(x.Name, elementName, StringComparison.OrdinalIgnoreCase));
                            if (el != null) el.Visible = visible;
                        }
                        catch { }
                    }

                    SetVisible("aceGenerateRequest", _currentUser.CanGenerateLicense);
                    SetVisible("aceGenerateLicense", _currentUser.CanGenerateLicense);
                    SetVisible("aceLicenseList", _currentUser.CanViewRecords);
                    SetVisible("aceLicenseDetails", _currentUser.CanViewRecords);
                    SetVisible("aceManageProduct", _currentUser.CanManageProduct);
                    SetVisible("aceUserManagement", _currentUser.CanManageUsers);
                    SetVisible("aceSettingsSecurity", string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));
                    SetVisible("aceDashboard", (_currentUser.CanGenerateLicense || _currentUser.CanViewRecords || _currentUser.CanManageProduct || _currentUser.CanManageUsers));

                    // Finally, show/hide the accordion based on whether any items are visible
                    bool anyVisible = navGroup.Elements.Any(x => x.Style == DevExpress.XtraBars.Navigation.ElementStyle.Item && x.Visible);
                    this.accordion.Visible = anyVisible;
                    this.accordion.Enabled = anyVisible;
                }
                catch
                {
                    // If any failure occurs, fallback to hiding accordion to avoid exposing UI
                    try { this.accordion.Visible = false; this.accordion.Enabled = false; } catch { }
                }
            }
            catch { }
        }
    }
}