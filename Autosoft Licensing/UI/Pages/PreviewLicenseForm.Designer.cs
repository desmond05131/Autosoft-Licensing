using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Autosoft_Licensing.UI
{
    partial class PreviewLicenseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        // Top summary controls (left column)
        private GroupControl grpSummary;
        private TextEdit txtCompanyName;
        private TextEdit txtProductId;
        private TextEdit txtProductName;
        private TextEdit txtLicenseKey;
        private TextEdit txtLicenseType;
        private TextEdit txtValidFrom;
        private TextEdit txtValidTo;
        private GridControl grdModules;
        private GridView viewModules;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleName;
        private DevExpress.XtraGrid.Columns.GridColumn colEnabled;

        // Right column: expiry/status + tabs
        private PanelControl panelRight;
        private LabelControl lblExpiryDate;
        private LabelControl lblStatusBadge;
        private XtraTabControl tabControl;
        private XtraTabPage tabCanonicalJson;
        private XtraTabPage tabLicenseInfo;

        // Canonical JSON tab controls
        private MemoEdit memoCanonicalJson;
        private TextEdit txtChecksum;
        private LabelControl lblChecksumStatus;
        private SimpleButton btnCopyJson;
        private SimpleButton btnExportJson;
        private SimpleButton btnValidateChecksum;

        // License Information tab placeholder grid
        private GridControl grdLicenseInfo;
        private GridView viewLicenseInfo;

        // Footer
        private PanelControl panelFooter;
        private SimpleButton btnBackToGenerate;
        private SimpleButton btnClose;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Designer-safe InitializeComponent that avoids risky constructs.
        /// Keep method body simple so the CodeDom parser can parse it.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();

            // Basic form properties
            this.SuspendLayout();
            this.Text = "Preview License";
            this.Name = "PreviewLicenseForm";
            this.ClientSize = new Size(1000, 640);
            this.StartPosition = FormStartPosition.CenterParent;

            // Left summary group
            grpSummary = new GroupControl();
            grpSummary.Name = "grpSummary";
            grpSummary.Text = "Summary";
            grpSummary.Dock = DockStyle.Left;
            grpSummary.Width = 340;
            this.Controls.Add(grpSummary);

            // Simple inner panel in summary to hold read-only TextEdits
            var summaryPanel = new Panel();
            summaryPanel.Dock = DockStyle.Fill;
            grpSummary.Controls.Add(summaryPanel);

            txtCompanyName = new TextEdit();
            txtCompanyName.Name = "txtCompanyName";
            txtCompanyName.ReadOnly = true;
            txtCompanyName.Dock = DockStyle.Top;
            txtCompanyName.Height = 24;

            txtProductId = new TextEdit();
            txtProductId.Name = "txtProductId";
            txtProductId.ReadOnly = true;
            txtProductId.Dock = DockStyle.Top;
            txtProductId.Height = 24;

            txtProductName = new TextEdit();
            txtProductName.Name = "txtProductName";
            txtProductName.ReadOnly = true;
            txtProductName.Dock = DockStyle.Top;
            txtProductName.Height = 24;

            txtLicenseKey = new TextEdit();
            txtLicenseKey.Name = "txtLicenseKey";
            txtLicenseKey.ReadOnly = true;
            txtLicenseKey.Dock = DockStyle.Top;
            txtLicenseKey.Height = 24;

            txtLicenseType = new TextEdit();
            txtLicenseType.Name = "txtLicenseType";
            txtLicenseType.ReadOnly = true;
            txtLicenseType.Dock = DockStyle.Top;
            txtLicenseType.Height = 24;

            txtValidFrom = new TextEdit();
            txtValidFrom.Name = "txtValidFrom";
            txtValidFrom.ReadOnly = true;
            txtValidFrom.Dock = DockStyle.Top;
            txtValidFrom.Height = 24;

            txtValidTo = new TextEdit();
            txtValidTo.Name = "txtValidTo";
            txtValidTo.ReadOnly = true;
            txtValidTo.Dock = DockStyle.Top;
            txtValidTo.Height = 24;

            // Add in stacked order
            summaryPanel.Controls.Add(txtValidTo);
            summaryPanel.Controls.Add(txtValidFrom);
            summaryPanel.Controls.Add(txtLicenseType);
            summaryPanel.Controls.Add(txtLicenseKey);
            summaryPanel.Controls.Add(txtProductName);
            summaryPanel.Controls.Add(txtProductId);
            summaryPanel.Controls.Add(txtCompanyName);

            // Modules grid: create minimal grid and view using designer pattern
            grdModules = new GridControl();
            grdModules.Name = "grdModules";
            grdModules.Dock = DockStyle.Bottom;
            grdModules.Height = 160;

            viewModules = new GridView();
            viewModules.Name = "viewModules";

            // Designer pattern: assign main view and add to ViewCollection via AddRange (explicit types)
            grdModules.MainView = viewModules;
            grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
                viewModules
            });

            colModuleName = new DevExpress.XtraGrid.Columns.GridColumn();
            colModuleName.FieldName = "ModuleName";
            colModuleName.Caption = "Module";
            colModuleName.Visible = true;
            colModuleName.VisibleIndex = 0;

            colEnabled = new DevExpress.XtraGrid.Columns.GridColumn();
            colEnabled.FieldName = "Enabled";
            colEnabled.Caption = "Enabled";
            colEnabled.Visible = true;
            colEnabled.VisibleIndex = 1;

            // Use explicit typed array (designer-friendly)
            viewModules.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colModuleName, colEnabled });

            summaryPanel.Controls.Add(grdModules);

            // Right side panel
            panelRight = new PanelControl();
            panelRight.Name = "panelRight";
            panelRight.Dock = DockStyle.Fill;
            this.Controls.Add(panelRight);

            lblExpiryDate = new LabelControl();
            lblExpiryDate.Name = "lblExpiryDate";
            lblExpiryDate.Text = "Valid To: -";
            lblExpiryDate.AutoSizeMode = LabelAutoSizeMode.None;
            lblExpiryDate.Width = 420;
            lblExpiryDate.Location = new Point(8, 8);
            panelRight.Controls.Add(lblExpiryDate);

            lblStatusBadge = new LabelControl();
            lblStatusBadge.Name = "lblStatusBadge";
            lblStatusBadge.Text = "Status";
            lblStatusBadge.Location = new Point(440, 12);
            panelRight.Controls.Add(lblStatusBadge);

            // Tab control with two pages
            tabControl = new XtraTabControl();
            tabControl.Name = "tabControl";
            tabControl.Dock = DockStyle.Fill;

            tabCanonicalJson = new XtraTabPage();
            tabCanonicalJson.Name = "tabCanonicalJson";
            tabCanonicalJson.Text = "Canonical JSON";

            tabLicenseInfo = new XtraTabPage();
            tabLicenseInfo.Name = "tabLicenseInfo";
            tabLicenseInfo.Text = "License Information";

            tabControl.TabPages.Add(tabCanonicalJson);
            tabControl.TabPages.Add(tabLicenseInfo);

            panelRight.Controls.Add(tabControl);

            // Canonical JSON tab controls
            memoCanonicalJson = new MemoEdit();
            memoCanonicalJson.Name = "memoCanonicalJson";
            memoCanonicalJson.Dock = DockStyle.Top;
            memoCanonicalJson.Height = 260;
            // Simple property assignment so designer can serialize
            memoCanonicalJson.Properties.ReadOnly = true;
            tabCanonicalJson.Controls.Add(memoCanonicalJson);

            // checksum panel (simple)
            var checksumPanel = new Panel();
            checksumPanel.Dock = DockStyle.Bottom;
            checksumPanel.Height = 72;
            tabCanonicalJson.Controls.Add(checksumPanel);

            var lblChecksum = new Label();
            lblChecksum.Text = "Checksum (SHA-256):";
            lblChecksum.Location = new Point(8, 10);
            checksumPanel.Controls.Add(lblChecksum);

            txtChecksum = new TextEdit();
            txtChecksum.Name = "txtChecksum";
            txtChecksum.ReadOnly = true;
            txtChecksum.Location = new Point(150, 6);
            txtChecksum.Width = 420;
            checksumPanel.Controls.Add(txtChecksum);

            lblChecksumStatus = new LabelControl();
            lblChecksumStatus.Name = "lblChecksumStatus";
            lblChecksumStatus.Text = "";
            lblChecksumStatus.Location = new Point(150, 36);
            checksumPanel.Controls.Add(lblChecksumStatus);

            btnCopyJson = new SimpleButton();
            btnCopyJson.Name = "btnCopyJson";
            btnCopyJson.Text = "Copy JSON";
            btnCopyJson.Location = new Point(600, 6);
            btnCopyJson.Width = 100;
            checksumPanel.Controls.Add(btnCopyJson);

            btnExportJson = new SimpleButton();
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Text = "Export JSON...";
            btnExportJson.Location = new Point(600, 36);
            btnExportJson.Width = 100;
            checksumPanel.Controls.Add(btnExportJson);

            btnValidateChecksum = new SimpleButton();
            btnValidateChecksum.Name = "btnValidateChecksum";
            btnValidateChecksum.Text = "Validate Checksum";
            btnValidateChecksum.Location = new Point(720, 6);
            btnValidateChecksum.Width = 140;
            checksumPanel.Controls.Add(btnValidateChecksum);

            // License information tab grid (designer pattern)
            grdLicenseInfo = new GridControl();
            grdLicenseInfo.Name = "grdLicenseInfo";
            grdLicenseInfo.Dock = DockStyle.Fill;
            viewLicenseInfo = new GridView();
            viewLicenseInfo.Name = "viewLicenseInfo";

            grdLicenseInfo.MainView = viewLicenseInfo;
            grdLicenseInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
                viewLicenseInfo
            });

            tabLicenseInfo.Controls.Add(grdLicenseInfo);

            // Footer
            panelFooter = new PanelControl();
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Height = 48;
            this.Controls.Add(panelFooter);

            btnBackToGenerate = new SimpleButton();
            btnBackToGenerate.Name = "btnBackToGenerate";
            btnBackToGenerate.Text = "Back to Generate";
            btnBackToGenerate.Width = 140;
            btnBackToGenerate.Anchor = AnchorStyles.Right;
            btnBackToGenerate.Location = new Point(680, 8);
            panelFooter.Controls.Add(btnBackToGenerate);

            btnClose = new SimpleButton();
            btnClose.Name = "btnClose";
            btnClose.Text = "Close";
            btnClose.Width = 100;
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.Location = new Point(840, 8);
            panelFooter.Controls.Add(btnClose);

            // Wire event handlers (designer-friendly simple assignments)
            btnCopyJson.Click += btnCopyJson_Click;
            btnExportJson.Click += btnExportJson_Click;
            btnValidateChecksum.Click += btnValidateChecksum_Click;
            btnBackToGenerate.Click += btnBackToGenerate_Click;
            btnClose.Click += btnClose_Click;

            this.ResumeLayout(false);
        }

        #endregion
    }
}
