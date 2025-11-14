using DevExpress.XtraBars.Navigation;
using System;
using System.ComponentModel;
using System.Windows.Forms;

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
            this.accordion = new AccordionControl();
            this.accordion.Dock = DockStyle.Left;
            this.accordion.ViewType = AccordionControlViewType.HamburgerMenu;
            this.accordion.Name = "accordion";
            this.accordion.Width = 260;

            AccordionControlElement navGroup = new AccordionControlElement();
            navGroup.Text = "Navigation";
            navGroup.Name = "aceNavigation";
            navGroup.Expanded = true;

            AccordionControlElement aceDashboard = new AccordionControlElement { Text = "Dashboard",           Style = ElementStyle.Item, Name = "aceDashboard" };
            AccordionControlElement aceGenerateRequest = new AccordionControlElement { Text = "Generate Request",    Style = ElementStyle.Item, Name = "aceGenerateRequest" };
            AccordionControlElement aceRequestHistory = new AccordionControlElement { Text = "Request History",     Style = ElementStyle.Item, Name = "aceRequestHistory" };
            AccordionControlElement aceImportActivate = new AccordionControlElement { Text = "Import / Activate",   Style = ElementStyle.Item, Name = "aceImportActivate" };
            AccordionControlElement aceLicenseList = new AccordionControlElement { Text = "License List",        Style = ElementStyle.Item, Name = "aceLicenseList" };
            AccordionControlElement aceLicenseDetails = new AccordionControlElement { Text = "License Details",     Style = ElementStyle.Item, Name = "aceLicenseDetails" };
            AccordionControlElement aceUserManagement = new AccordionControlElement { Text = "User Management",     Style = ElementStyle.Item, Name = "aceUserManagement" };
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

            // Add accordion to the form and ensure docking order (accordion first, then Fill panel)
            this.SuspendLayout();
            this.Controls.Add(this.accordion);
            this.Controls.SetChildIndex(this.accordion, 0);
            this.ResumeLayout(performLayout: false);
        }
    }
}