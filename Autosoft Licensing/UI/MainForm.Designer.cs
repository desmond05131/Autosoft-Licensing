using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System.ComponentModel;

namespace Autosoft_Licensing
{
    partial class MainForm
    {
        private IContainer components = null;
        private AccordionControl accordion;
        private PanelControl contentPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.accordion = new AccordionControl();
            this.contentPanel = new PanelControl();

            ((ISupportInitialize)(this.accordion)).BeginInit();
            ((ISupportInitialize)(this.contentPanel)).BeginInit();
            this.SuspendLayout();

            // accordion
            this.accordion.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordion.ViewType = AccordionControlViewType.HamburgerMenu;
            this.accordion.Name = "accordion";
            this.accordion.Width = 260;

            var navGroup = new AccordionControlElement()
            {
                Text = "Navigation",
                Name = "aceNavigation",
                Expanded = true
            };

            navGroup.Elements.AddRange(new[]
            {
                MakeItem("Dashboard"),
                MakeItem("Generate Request"),
                MakeItem("Request History"),
                MakeItem("Import / Activate"),
                MakeItem("License List"),
                MakeItem("License Details"),
                MakeItem("User Management"),
                MakeItem("Settings / Security"),
            });

            this.accordion.Elements.Add(navGroup);

            // contentPanel
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Name = "contentPanel";

            // MainForm
            this.Text = "AutoSoft Licensing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.accordion);
            this.Name = "MainForm";

            ((ISupportInitialize)(this.accordion)).EndInit();
            ((ISupportInitialize)(this.contentPanel)).EndInit();
            this.ResumeLayout(false);
        }
    }
}