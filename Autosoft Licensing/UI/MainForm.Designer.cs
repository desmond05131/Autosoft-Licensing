using DevExpress.XtraEditors;
using System.ComponentModel;

namespace Autosoft_Licensing
{
    partial class MainForm
    {
        private IContainer components = null;
        private ListBoxControl navList;
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
            this.navList = new ListBoxControl();
            this.contentPanel = new PanelControl();

            ((ISupportInitialize)(this.navList)).BeginInit();
            ((ISupportInitialize)(this.contentPanel)).BeginInit();
            this.SuspendLayout();

            // navList
            this.navList.Dock = System.Windows.Forms.DockStyle.Left;
            this.navList.Name = "navList";
            this.navList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.navList.ItemHeight = 28;
            this.navList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.navList.Size = new System.Drawing.Size(260, 450);
            this.navList.Items.AddRange(new object[]
            {
                "Dashboard",
                "Generate Request",
                "Request History",
                "Import / Activate",
                "License List",
                "License Details",
                "User Management",
                "Settings / Security"
            });
            this.navList.SelectedIndexChanged += new System.EventHandler(this.navList_SelectedIndexChanged);

            // contentPanel
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Name = "contentPanel";

            // MainForm
            this.Text = "AutoSoft Licensing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            // add LEFT first, then FILL last
            this.Controls.Add(this.navList);
            this.Controls.Add(this.contentPanel);
            this.Name = "MainForm";

            ((ISupportInitialize)(this.navList)).EndInit();
            ((ISupportInitialize)(this.contentPanel)).EndInit();
            this.ResumeLayout(false);
        }
    }
}