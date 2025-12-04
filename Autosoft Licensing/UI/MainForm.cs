using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
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

        /// <summary>
        /// Host-facing API to set the currently logged-in user and update UI visibility.
        /// Call this after authentication succeeds.
        /// </summary>
        public void SetLoggedInUser(User user)
        {
            LoggedInUser = user;
            UpdateRoleVisibility();
        }

        /// <summary>
        /// Navigate to a sensible default page for the current user (called after login).
        /// Default strategy: pick the first navigation element the current user is allowed to access.
        /// </summary>
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

                // Skip admin-only items if current user is not admin
                bool isAdminElement = string.Equals(el.Name, "aceUserManagement", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(el.Name, "aceSettingsSecurity", StringComparison.OrdinalIgnoreCase);

                if (isAdminElement && (LoggedInUser == null || !string.Equals(LoggedInUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)))
                    continue;

                // Load first allowed page
                LoadPage(el.Name, el.Text);
                return;
            }
        }

        /// <summary>
        /// Programmatic helper used by external hosts/tests to navigate to a named element.
        /// Uses the same loader as normal UI clicks. Safe no-op when element not found.
        /// </summary>
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
                    // Call private loader
                    LoadPage(el.Name, el.Text);
                    return;
                }
            }
        }

        /// <summary>
        /// Programmatic helper to enumerate the navigation elements (Name, Text).
        /// Used by smoke/demo flows to iterate navigation.
        /// </summary>
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

        /// <summary>
        /// Load a page by navigation element name (elementName) and display text (for placeholder pages).
        /// Pages are cached so each page is a singleton instance.
        /// </summary>
        private void LoadPage(string elementName, string elementText)
        {
            if (string.IsNullOrEmpty(elementName))
                return;

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
                // Instantiate specific pages for known navigation element names.
                switch (elementName)
                {
                    // Map the accordion element used for Generate License to the real page.
                    // BuildAccordion sets the element Name = "aceGenerateRequest".
                    case "aceGenerateRequest":
                    // Also accept alternative automation-friendly name if used by tests/automation.
                    case "btnNav_GenerateLicense":
                    case "aceGenerateLicense":
                    case "GenerateLicensePage":
                        page = new GenerateLicensePage();
                        break;

                    // License Records page
                    case "aceLicenseList":
                    case "btnNav_LicenseRecords":
                    case "LicenseRecordsPage":
                        page = new LicenseRecordsPage();
                        // Wire navigation event
                        if (page is LicenseRecordsPage recordsPage)
                        {
                            recordsPage.NavigateRequested += OnPageNavigationRequested;
                        }
                        break;

                    // License Details page
                    case "LicenseRecordDetailsPage":
                        page = new LicenseRecordDetailsPage();
                        // Wire back navigation
                        if (page is LicenseRecordDetailsPage detailsPage)
                        {
                            detailsPage.NavigateRequested += OnDetailsPageNavigationRequested;
                        }
                        break;

                    default:
                        page = new GenericPage(elementText ?? elementName);
                        break;
                }
                _pageCache[elementName] = page;
            }

            // Ensure page docked and shown
            contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(page);

            // Let page initialize based on current user role
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

        /// <summary>
        /// Handle navigation requests from child pages.
        /// </summary>
        private void OnPageNavigationRequested(object sender, LicenseRecordsPage.NavigateEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.TargetPage))
                    return;

                // Handle different target pages
                switch (e.TargetPage)
                {
                    case "LicenseRecordDetailsPage":
                        if (e.LicenseId.HasValue)
                        {
                            // Load or get cached details page
                            if (!_pageCache.TryGetValue("LicenseRecordDetailsPage", out var page))
                            {
                                page = new LicenseRecordDetailsPage();
                                
                                // FIXED: Wire back navigation event for newly created page
                                if (page is LicenseRecordDetailsPage detailsPage)
                                {
                                    detailsPage.NavigateRequested += OnDetailsPageNavigationRequested;
                                }
                                
                                _pageCache["LicenseRecordDetailsPage"] = page;
                            }

                            // Initialize with license ID
                            if (page is LicenseRecordDetailsPage detailsPageToInit)
                            {
                                detailsPageToInit.Initialize(e.LicenseId.Value);
                            }

                            // Show the page
                            contentPanel.Controls.Clear();
                            page.Dock = DockStyle.Fill;
                            contentPanel.Controls.Add(page);
                            page.InitializeForRole(LoggedInUser);
                        }
                        break;

                    case "GenerateLicensePage":
                        LoadPage("GenerateLicensePage", "Generate License");
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

        /// <summary>
        /// Handle navigation from details page (primarily back navigation).
        /// </summary>
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

        /// <summary>
        /// Toggle visibility of admin-only accordion elements.
        /// Called after SetLoggedInUser or on form load.
        /// </summary>
        private void UpdateRoleVisibility()
        {
            try
            {
                var isAdmin = LoggedInUser != null && string.Equals(LoggedInUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                if (accordion == null) return;

                // Attempt to find elements by name and set visibility
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

        // Public helper: run a non-interactive UI smoke test of the navigation shell.
        // Returns (success, message) where message contains human-readable details.
        public (bool Success, string Message) RunUiSmokeTest()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Starting UI smoke test...");

            try
            {
                // Ensure the runtime-built accordion and contentPanel exist.
                if (this.accordion == null || this.contentPanel == null)
                {
                    // Call the runtime builder implemented in the Navigation partial
                    BuildAccordion();
                }

                if (accordion == null || contentPanel == null)
                {
                    return (false, "Failed to construct navigation or content host controls.");
                }

                // Suppress message boxes while running automated checks
                _suppressMessageBoxes = true;

                var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
                if (navGroup == null)
                {
                    _suppressMessageBoxes = false;
                    return (false, "No navigation group found in accordion.");
                }

                sb.AppendLine($"Found navigation group '{navGroup.Text}' with {navGroup.Elements.Count} elements.");

                // Keep track of page instances to validate singleton behaviour
                var seenInstances = new Dictionary<string, PageBase>(StringComparer.OrdinalIgnoreCase);

                foreach (var el in navGroup.Elements)
                {
                    if (el == null) continue;
                    var name = el.Name ?? string.Empty;
                    var text = el.Text ?? name;

                    sb.AppendLine($"- Checking element: Name='{name}', Text='{text}'");

                    bool isAdminElement = name.Equals("aceUserManagement", StringComparison.OrdinalIgnoreCase) ||
                                          name.Equals("aceSettingsSecurity", StringComparison.OrdinalIgnoreCase);

                    // Ensure no user is set for access-denied checks
                    var savedUser = this.LoggedInUser;
                    this.LoggedInUser = null;
                    UpdateRoleVisibility();

                    // Simulate click by calling the loader
                    try
                    {
                        LoadPage(name, text);
                    }
                    catch (Exception ex)
                    {
                        // restore and fail
                        this.LoggedInUser = savedUser;
                        UpdateRoleVisibility();
                        _suppressMessageBoxes = false;
                        return (false, $"Exception while loading page '{name}': {ex.Message}");
                    }

                    // For admin-only elements we expect NO page to be created when no user is set.
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
                        // Non-admin: expect a page exists in cache and the shown control is that instance.
                        if (!_pageCache.TryGetValue(name, out var createdPage) || createdPage == null)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not create a cached page instance.");
                        }

                        // Ensure some control is displayed and it is the cached instance
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

                        // Validate caching (repeat navigation and ensure same instance reused)
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

                    // restore user and continue
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

        // Placeholder if you later want to handle accordion clicks:
        // private void OnAccordionElementClick(object sender, ElementClickEventArgs e)
        // {
        //     // TODO: swap user controls into contentPanel based on e.Element.Text
        // }
    }
}