using DevExpress.XtraBars.Navigation;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

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
    }
}