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
                // Instantiate placeholder pages here. Replace with real page types when available.
                switch (elementName)
                {
                    // e.g. case "aceGenerateRequest": page = new GenerateLicensePage(); break;
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
            var initialContentCount = 0;

            try
            {
                // Ensure the runtime-built accordion and contentPanel exist.
                // BuildAccordion is in the other partial class; call it only if controls not already created.
                if (this.accordion == null || this.contentPanel == null)
                {
                    // The BuildAccordion method is private; it's in another partial file but accessible from within this class.
                    // Call it to construct the navigation and host panel.
                    this.BuildAccordion();
                }

                if (accordion == null || contentPanel == null)
                {
                    return (false, "Failed to construct navigation or content host controls.");
                }

                // Suppress message boxes while running automated checks
                _suppressMessageBoxes = true;

                initialContentCount = contentPanel.Controls.Count;

                var navGroup = (accordion.Elements.Count > 0) ? accordion.Elements[0] : null;
                if (navGroup == null)
                    return (false, "No navigation group found in accordion.");

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

                    // Record content count prior to navigation
                    var beforeCount = contentPanel.Controls.Count;

                    // Simulate click by calling the loader
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

                    var afterCount = contentPanel.Controls.Count;

                    if (isAdminElement)
                    {
                        // Expect no new page created when no user or non-admin
                        if (_pageCache.ContainsKey(name) || afterCount != beforeCount)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Admin element '{name}' should not be accessible when no user is set, but it appears to have been loaded.");
                        }
                        sb.AppendLine($"  -> Admin element '{name}' correctly blocked when no user is set.");
                    }
                    else
                    {
                        // Non-admin: expect a single page loaded into contentPanel
                        if (afterCount == beforeCount)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not load a page into the content panel.");
                        }

                        if (contentPanel.Controls.Count != 1)
                        {
                            // Unexpected: ensure contentPanel is cleared and only the page is present
                            sb.AppendLine($"  -> Warning: contentPanel contains {contentPanel.Controls.Count} controls after loading '{name}'. Expected 1.");
                        }

                        var ctrl = contentPanel.Controls[0] as PageBase;
                        if (ctrl == null)
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Loaded control for '{name}' is not a PageBase-derived control.");
                        }

                        // Validate caching (repeat navigation and ensure same instance reused)
                        var firstInstance = ctrl;
                        // navigate again
                        LoadPage(name, text);
                        var secondInstance = (contentPanel.Controls.Count > 0) ? contentPanel.Controls[0] as PageBase : null;

                        if (!object.ReferenceEquals(firstInstance, secondInstance))
                        {
                            this.LoggedInUser = savedUser;
                            UpdateRoleVisibility();
                            _suppressMessageBoxes = false;
                            return (false, $"Element '{name}' did not reuse the same page instance on repeated navigation.");
                        }

                        // track for reporting
                        seenInstances[name] = firstInstance;
                        sb.AppendLine($"  -> '{name}' loaded and reused instance (Type={firstInstance.GetType().Name}).");
                    }

                    // restore user (none) and continue
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