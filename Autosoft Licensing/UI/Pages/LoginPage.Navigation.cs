using DevExpress.XtraBars.Navigation;
using System;
using System.ComponentModel;
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

            AccordionControlElement aceDashboard = new AccordionControlElement { Text = "Dashboard", Style = ElementStyle.Item, Name = "aceDashboard" };
            AccordionControlElement aceGenerateRequest = new AccordionControlElement { Text = "Generate Request", Style = ElementStyle.Item, Name = "aceGenerateRequest" };
            AccordionControlElement aceRequestHistory = new AccordionControlElement { Text = "Request History", Style = ElementStyle.Item, Name = "aceRequestHistory" };
            AccordionControlElement aceImportActivate = new AccordionControlElement { Text = "Import / Activate", Style = ElementStyle.Item, Name = "aceImportActivate" };
            AccordionControlElement aceLicenseList = new AccordionControlElement { Text = "License List", Style = ElementStyle.Item, Name = "aceLicenseList" };
            AccordionControlElement aceLicenseDetails = new AccordionControlElement { Text = "License Details", Style = ElementStyle.Item, Name = "aceLicenseDetails" };
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
        /// Helper to show a UserControl page inside the main content panel.
        /// Clears the current content, docks the new page and calls InitializeForRole if available.
        /// </summary>
        /// <param name="page">UserControl to display</param>
        public void ShowPage(UserControl page)
        {
            if (page == null) return;
            if (this.contentPanel == null) return;

            this.contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            this.contentPanel.Controls.Add(page);

            // If page implements PageBase, call InitializeForRole when possible.
            if (page is UI.Pages.PageBase pb)
            {
                try
                {
                    pb.InitializeForRole(this.LoggedInUser);
                }
                catch
                {
                    // best-effort; do not crash the host
                }
            }
        }

        /// <summary>
        /// Navigate to the application's default landing page after login.
        /// TODO: Replace direct constructor with DI/resolved instance when wiring services.
        /// </summary>
        public void NavigateToDefaultPage()
        {
            try
            {
                // TODO: Replace with DI-created page (inject services required by GenerateLicensePage).
                // Using GenerateLicensePage as default landing. If unavailable or requires parameters, replace with appropriate page.
                var defaultPage = new GenerateLicensePage();
                ShowPage(defaultPage);
            }
            catch (Exception ex)
            {
                // If default page creation fails, show a placeholder so UI remains usable.
                ShowPage(new UI.Pages.GenericPage("Home"));
                // Log or surface the issue as needed (left as TODO).
            }
        }
    }
}
