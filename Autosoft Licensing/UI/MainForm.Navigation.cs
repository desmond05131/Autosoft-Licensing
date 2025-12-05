using DevExpress.XtraBars.Navigation;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.UI.Pages;

namespace Autosoft_Licensing
{
    // Keep runtime-only UI construction away from InitializeComponent so the designer can load.
    partial class MainForm
    {
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Skip when the form is hosted by the designer
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            BuildAccordion();
        }

        private void BuildAccordion()
        {
            // Idempotent guard: if already built, don't add duplicates
            if (this.accordion != null && this.contentPanel != null &&
                this.Controls.Contains(this.accordion) && this.Controls.Contains(this.contentPanel))
            {
                UpdateRoleVisibility();
                return;
            }

            // Create and configure accordion (left)
            this.accordion = new AccordionControl();
            this.accordion.Dock = DockStyle.Left;
            this.accordion.ViewType = AccordionControlViewType.HamburgerMenu;
            this.accordion.Name = "accordion";
            this.accordion.Width = 260;

            AccordionControlElement navGroup = new AccordionControlElement
            {
                Text = "Navigation",
                Name = "aceNavigation",
                Expanded = true
            };

            // NOTE: Text changed to "Generate License" to match UI automation expectations used by E2E tests.
            AccordionControlElement aceDashboard = new AccordionControlElement { Text = "Dashboard", Style = ElementStyle.Item, Name = "aceDashboard" };
            AccordionControlElement aceGenerateRequest = new AccordionControlElement { Text = "Generate License", Style = ElementStyle.Item, Name = "aceGenerateRequest" };
            AccordionControlElement aceRequestHistory = new AccordionControlElement { Text = "Request History", Style = ElementStyle.Item, Name = "aceRequestHistory" };
            AccordionControlElement aceImportActivate = new AccordionControlElement { Text = "Import / Activate", Style = ElementStyle.Item, Name = "aceImportActivate" };
            AccordionControlElement aceLicenseList = new AccordionControlElement { Text = "License List", Style = ElementStyle.Item, Name = "aceLicenseList" };
            AccordionControlElement aceLicenseDetails = new AccordionControlElement { Text = "License Details", Style = ElementStyle.Item, Name = "aceLicenseDetails" };

            // NEW: Manage Product element (inserted before User Management)
            AccordionControlElement aceManageProduct = new AccordionControlElement { Text = "Manage Product", Style = ElementStyle.Item, Name = "aceManageProduct" };

            AccordionControlElement aceUserManagement = new AccordionControlElement { Text = "User Management", Style = ElementStyle.Item, Name = "aceUserManagement" };
            AccordionControlElement aceSettingsSecurity = new AccordionControlElement { Text = "Settings / Security", Style = ElementStyle.Item, Name = "aceSettingsSecurity" };

            navGroup.Elements.AddRange(new AccordionControlElement[]
            {
                aceDashboard,
                aceGenerateRequest,
                aceRequestHistory,
                aceImportActivate,
                aceLicenseList,
                aceLicenseDetails,
                // Insert new Manage Product before User Management
                aceManageProduct,
                aceUserManagement,
                aceSettingsSecurity
            });

            this.accordion.Elements.Add(navGroup);

            // Create the right-side host panel for pages
            this.contentPanel = new PanelControl();
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.Name = "contentPanel";

            // Add host panel and accordion to the form and ensure docking order (accordion at left, panel fills)
            this.SuspendLayout();
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.accordion);

            // Ensure the accordion remains at z-order 0 so it docks left and panel fills remaining space
            this.Controls.SetChildIndex(this.accordion, 0);
            this.ResumeLayout(performLayout: false);

            // Wire navigation clicks directly to loader
            this.accordion.ElementClick += Accordion_ElementClick;

            // Update role-based visibility (best-effort; SetLoggedInUser may be called later)
            UpdateRoleVisibility();
        }

        private void Accordion_ElementClick(object sender, ElementClickEventArgs e)
        {
            if (e?.Element == null) return;

            try
            {
                // direct call to the private loader in the same partial class
                LoadPage(e.Element.Name, e.Element.Text);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Failed to navigate: " + ex.Message, "Navigation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Replace the content panel with the provided page instance.
        /// Exposed so pages or external callers may show ad-hoc pages.
        /// </summary>
        public void ShowPage(UserControl page)
        {
            if (page == null) return;

            if (this.contentPanel == null)
            {
                // Ensure runtime accordion/content built
                BuildAccordion();
            }

            try
            {
                contentPanel.Controls.Clear();
                page.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(page);
            }
            catch
            {
                // swallow to avoid crashing host
            }
        }

        /// <summary>
        /// Convenience helper to navigate to the Generate License page using the internal loader.
        /// This constructs a page instance and shows it; host should inject real services via Initialize(...)
        /// </summary>
        public void NavigateToGenerateLicensePage()
        {
            var page = new GenerateLicensePage();

            // TODO: inject real services here, for example:
            // page.Initialize(ServiceRegistry.ArlReader, ServiceRegistry.AslGenerator, ServiceRegistry.Product, ServiceRegistry.Database, ServiceRegistry.User);

            ShowPage(page);
        }

        /// <summary>
        /// Returns the interactive navigation elements (items) in the accordion.
        /// Used by the smoke/demo harness to iterate and navigate.
        /// </summary>
        public AccordionControlElement[] GetNavigationElements()
        {
            // Ensure accordion exists
            if (this.accordion == null || !this.Controls.Contains(this.accordion))
            {
                BuildAccordion();
            }

            try
            {
                // Find the "Navigation" group and return its item elements
                var navGroup = this.accordion.Elements
                    .FirstOrDefault(e => string.Equals(e.Name, "aceNavigation", StringComparison.OrdinalIgnoreCase));

                if (navGroup == null)
                    return Array.Empty<AccordionControlElement>();

                return navGroup.Elements
                    .Where(e => e.Style == ElementStyle.Item)
                    .ToArray();
            }
            catch
            {
                return Array.Empty<AccordionControlElement>();
            }
        }

        /// <summary>
        /// Programmatic navigation helper; routes to the appropriate page via internal loader.
        /// Accepts either a known element Name or a page key (e.g., "GenerateLicensePage").
        /// </summary>
        public void NavigateToElement(string elementNameOrPageKey)
        {
            if (string.IsNullOrWhiteSpace(elementNameOrPageKey))
                return;

            // Ensure runtime UI exists
            if (this.accordion == null || !this.Controls.Contains(this.accordion) || this.contentPanel == null)
            {
                BuildAccordion();
            }

            // If the caller passed a page key, map it to loader keys directly
            string key = elementNameOrPageKey;

            // Normalize known aliases to page keys (mirrors LoadPage name mapping)
            if (key.Equals("aceManageProduct", StringComparison.OrdinalIgnoreCase))
                key = "ManageProductPage";
            else if (key.Equals("aceUserManagement", StringComparison.OrdinalIgnoreCase))
                key = "ManageUserPage";
            else if (key.Equals("aceLicenseList", StringComparison.OrdinalIgnoreCase))
                key = "LicenseRecordsPage";
            else if (key.Equals("aceGenerateRequest", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("aceGenerateLicense", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("btnNav_GenerateLicense", StringComparison.OrdinalIgnoreCase))
                key = "GenerateLicensePage";

            try
            {
                // Try to find a matching accordion element to preserve element text
                var all = GetNavigationElements();
                var match = all.FirstOrDefault(e =>
                    string.Equals(e.Name, elementNameOrPageKey, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.Text, elementNameOrPageKey, StringComparison.OrdinalIgnoreCase));

                var elementText = match?.Text ?? key;
                LoadPage(key, elementText);
            }
            catch (Exception ex)
            {
                if (!_suppressMessageBoxes)
                {
                    XtraMessageBox.Show("Navigation failed: " + ex.Message, "Navigation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Lightweight UI smoke test that iterates known navigation items and attempts to load each page.
        /// Returns a simple result with aggregated messages.
        /// </summary>
        public UiSmokeResult RunUiSmokeTest()
        {
            var sb = new StringBuilder();
            bool success = true;

            try
            {
                // Ensure accordion/content are ready
                BuildAccordion();

                var elements = GetNavigationElements();
                if (elements.Length == 0)
                {
                    sb.AppendLine("No navigation elements found.");
                    success = false;
                }
                else
                {
                    sb.AppendLine("Starting UI smoke test...");
                    foreach (var el in elements)
                    {
                        try
                        {
                            sb.AppendLine($"Navigating: {el.Text} ({el.Name})");
                            LoadPage(el.Name, el.Text);
                            // Give the UI a chance to layout; avoid blocking
                            Application.DoEvents();
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"Failed to load '{el.Text}' ({el.Name}): {ex.Message}");
                            success = false;
                        }
                    }
                    sb.AppendLine("UI smoke test complete.");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Smoke test encountered an exception: " + ex.Message);
                success = false;
            }

            return new UiSmokeResult { Success = success, Message = sb.ToString() };
        }

        /// <summary>
        /// Result value for RunUiSmokeTest.
        /// </summary>
        public struct UiSmokeResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}