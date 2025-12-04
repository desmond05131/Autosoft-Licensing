using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.UI.Pages;
using System.Text;

namespace Autosoft_Licensing
{
    public partial class MainForm : XtraForm
    {
        // Cache singletons per page element name
        private readonly Dictionary<string, PageBase> _pageCache = new Dictionary<string, PageBase>(StringComparer.OrdinalIgnoreCase);

        // Currently logged-in user (set this from your login flow)
        public User LoggedInUser { get; private set; }

        // When true suppress modal message boxes (used by automated UI smoke tests)
        private bool _suppressMessageBoxes = false;

        public MainForm()
        {
            InitializeComponent();

   
        }

        public void SetLoggedInUser(User user)
        {
            LoggedInUser = user;
            UpdateRoleVisibility();
        }

        public void NavigateToDefaultPage()
        {
            // Ensure runtime UI exists
            if (this.accordion == null || this.contentPanel == null)
            {
                this.BuildAccordion();
            }

            var navGroup = (accordion != null && accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
            if (navGroup == null) return;

            foreach (var el in navGroup.Elements)
            {
                if (el == null) continue;

                bool isAdminElement = string.Equals(el.Name, "aceUserManagement", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(el.Name, "aceSettingsSecurity", StringComparison.OrdinalIgnoreCase);

                if (isAdminElement && (LoggedInUser == null || !string.Equals(LoggedInUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)))
                    continue;

                LoadPage(el.Name, el.Text);
                return;
            }
        }

        public void NavigateToElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName))
                return;

            // Ensure runtime UI exists
            if (this.accordion == null || this.contentPanel == null)
            {
                this.BuildAccordion();
            }

            // Find element by name under the first nav group
            var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
            if (navGroup == null) return;

            foreach (var el in navGroup.Elements)
            {
                if (el == null) continue;
                if (string.Equals(el.Name, elementName, StringComparison.OrdinalIgnoreCase))
                {
                    LoadPage(el.Name, el.Text);
                    return;
                }
            }
        }

        public IReadOnlyList<(string Name, string Text)> GetNavigationElements()
        {
            var list = new List<(string, string)>();

            if (this.accordion == null || this.contentPanel == null)
            {
                BuildAccordion();
            }

            var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
            if (navGroup == null) return list;

            foreach (var el in navGroup.Elements)
            {
                if (el == null) continue;
                list.Add((el.Name ?? string.Empty, el.Text ?? string.Empty));
            }

            return list;
        }

        private void LoadPage(string elementName, string elementText)
        {
            if (string.IsNullOrEmpty(elementName))
                return;

            // Ensure runtime UI exists (avoid NRE when called before form Load)
            if (this.accordion == null || this.contentPanel == null)
            {
                this.BuildAccordion();
            }

            // Protect admin-only elements
            if ((elementName.Equals("aceUserManagement", StringComparison.OrdinalIgnoreCase) ||
                 elementName.Equals("aceSettingsSecurity", StringComparison.OrdinalIgnoreCase)) &&
                 (LoggedInUser == null || !string.Equals(LoggedInUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)))
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("You do not have permission to access this page.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            if (!_pageCache.TryGetValue(elementName, out var page))
            {
                switch (elementName)
                {
                    case "aceGenerateRequest":
                    case "btnNav_GenerateLicense":
                    case "aceGenerateLicense":
                    case "GenerateLicensePage":
                        page = new GenerateLicensePage();
                        break;

                    case "aceLicenseList":
                    case "btnNav_LicenseRecords":
                    case "LicenseRecordsPage":
                        page = new LicenseRecordsPage();
                        if (page is LicenseRecordsPage recordsPage)
                        {
                            recordsPage.NavigateRequested += OnPageNavigationRequested;
                        }
                        break;

                    case "LicenseRecordDetailsPage":
                        page = new LicenseRecordDetailsPage();
                        if (page is LicenseRecordDetailsPage detailsPage)
                        {
                            detailsPage.NavigateRequested += OnDetailsPageNavigationRequested;
                        }
                        break;

                    // NEW: Manage Product navigation
                    case "aceManageProduct":
                        page = new ManageProductPage();
                        if (page is ManageProductPage managePage)
                        {
                            // Initialize DB and wire navigation
                            managePage.Initialize(ServiceRegistry.Database);
                            managePage.NavigateRequested += OnManageProductNavigation;
                        }
                        break;

                    default:
                        page = new GenericPage(elementText ?? elementName);
                        break;
                }
                _pageCache[elementName] = page;
            }

            contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(page);

            try
            {
                page.InitializeForRole(LoggedInUser);
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Failed to initialize page: " + ex.Message, "Page error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Existing navigation handlers...
        private void OnPageNavigationRequested(object sender, Autosoft_Licensing.UI.Pages.LicenseRecordsPage.NavigateEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.TargetPage))
                    return;

                switch (e.TargetPage)
                {
                    case "LicenseRecordDetailsPage":
                        if (e.LicenseId.HasValue)
                        {
                            if (!_pageCache.TryGetValue("LicenseRecordDetailsPage", out var page))
                            {
                                page = new LicenseRecordDetailsPage();
                                if (page is LicenseRecordDetailsPage detailsPage)
                                    detailsPage.NavigateRequested += OnDetailsPageNavigationRequested;

                                _pageCache["LicenseRecordDetailsPage"] = page;
                            }

                            if (page is LicenseRecordDetailsPage detailsPageToInit)
                                detailsPageToInit.Initialize(e.LicenseId.Value);

                            contentPanel.Controls.Clear();
                            page.Dock = DockStyle.Fill;
                            contentPanel.Controls.Add(page);
                            page.InitializeForRole(LoggedInUser);
                        }
                        break;

                    case "GenerateLicensePage":
                        LoadPage("GenerateLicensePage", "Generate License");
                        break;

                    case "LicenseRecordsPage":
                        LoadPage("LicenseRecordsPage", "License Records");
                        break;

                    default:
                        LoadPage(e.TargetPage, e.TargetPage);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show($"Navigation failed: {ex.Message}", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnDetailsPageNavigationRequested(object sender, LicenseRecordDetailsPage.NavigateEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.TargetPage))
                    return;

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
                var isAdmin = LoggedInUser != null && string.Equals(LoggedInUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                if (accordion == null) return;

                var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
                if (navGroup == null) return;

                foreach (var el in navGroup.Elements)
                {
                    if (el.Name == "aceUserManagement" || el.Name == "aceSettingsSecurity")
                        el.Visible = isAdmin;
                }
            }
            catch
            {
                // best-effort; do not throw from UI update
            }
        }

        private void navList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: swap content based on selected item.
            // This stub resolves the missing handler compile error.
        }

        public (bool Success, string Message) RunUiSmokeTest()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Starting UI smoke test...");

            try
            {
                if (this.accordion == null || this.contentPanel == null)
                {
                    BuildAccordion();
                }

                if (accordion == null || contentPanel == null)
                {
                    return (false, "Failed to construct navigation or content host controls.");
                }

                _suppressMessageBoxes = true;

                var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
                if (navGroup == null)
                {
                    _suppressMessageBoxes = false;
                    return (false, "No navigation group found in accordion.");
                }

                sb.AppendLine($"Found navigation group '{navGroup.Text}' with {navGroup.Elements.Count} elements.");

                var seenInstances = new Dictionary<string, PageBase>(StringComparer.OrdinalIgnoreCase);

                foreach (var el in navGroup.Elements)
                {
                    if (el == null) continue;
                    var name = el.Name ?? string.Empty;
                    var text = el.Text ?? name;

                    sb.AppendLine($"- Checking element: Name='{name}', Text='{text}'");

                    bool isAdminElement = name.Equals("aceUserManagement", StringComparison.OrdinalIgnoreCase) ||
                                          name.Equals("aceSettingsSecurity", StringComparison.OrdinalIgnoreCase);

                    var savedUser = this.LoggedInUser;
                    this.LoggedInUser = null;
                    UpdateRoleVisibility();

                    try
                    {
                        LoadPage(name, text);
                    }
                    catch (Exception ex)
                    {
                        this.LoggedInUser = savedUser;
                        UpdateRoleVisibility();
                        _suppressMessageBoxes = false;
                        return (false, $"Exception while loading page '{name}': {ex.Message}");
                    }

                    if (isAdminElement)
                    {
                        if (_pageCache.ContainsKey(name))
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Admin element '{name}' should not be accessible when no user is set, but it was created in cache.");
                        }
                        sb.AppendLine($"  -> Admin element '{name}' correctly blocked when no user is set.");
                    }
                    else
                    {
                        if (!_pageCache.TryGetValue(name, out var createdPage) || createdPage == null)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not create a cached page instance.");
                        }

                        var displayed = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] as PageBase : null;
                        if (displayed == null)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not display a PageBase-derived control in the content panel.");
                        }

                        if (!object.ReferenceEquals(displayed, createdPage))
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' displayed a control that is not the cached instance.");
                        }

                        LoadPage(name, text);
                        var displayedAgain = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] as PageBase : null;

                        if (!object.ReferenceEquals(displayedAgain, createdPage))
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not reuse the same page instance on repeated navigation.");
                        }

                        seenInstances[name] = createdPage;
                        sb.AppendLine($"  -> '{name}' loaded and reused instance (Type={createdPage.GetType().Name}).");
                    }

                    this.LoggedInUser = savedUser;
                    UpdateRoleVisibility();
                }

                sb.AppendLine($"UI smoke test completed. Pages created: {seenInstances.Count} (non-admin).");
                _suppressMessageBoxes = false;
                return (true, sb.ToString());
            }
            catch (Exception ex)
            {
                _suppressMessageBoxes = false;
                return (false, "UI smoke test failed: " + ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        // NEW: Handle navigation events from ManageProductPage
        private void OnManageProductNavigation(object sender, ManageProductPage.NavigateEventArgs e)
        {
            try
            {
                if (e == null || string.IsNullOrEmpty(e.TargetPage))
                    return;

                if (string.Equals(e.TargetPage, "ProductDetailsPage", StringComparison.OrdinalIgnoreCase))
                {
                    var details = new ProductDetailsPage();
                    // Initialize with provided product id (null => create mode)
                    details.Initialize(e.ProductId, null, null);
                    details.NavigateBackRequested += OnProductDetailsBack;

                    ShowPage(details);
                    details.InitializeForRole(LoggedInUser);
                }
                else
                {
                    LoadPage(e.TargetPage, e.TargetPage);
                }
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show($"Navigation failed: {ex.Message}", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // NEW: Handle back from ProductDetailsPage
        private void OnProductDetailsBack(object sender, ProductDetailsPage.NavigateBackEventArgs e)
        {
            try
            {
                // Reload Manage Product page to refresh grid
                LoadPage("aceManageProduct", "Manage Product");

                if (e != null && e.Saved && !_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Product saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show($"Navigation failed: {ex.Message}", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Placeholder...
    }
}