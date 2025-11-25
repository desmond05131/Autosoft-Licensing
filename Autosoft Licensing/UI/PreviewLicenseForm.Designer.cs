using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Autosoft_Licensing.UI
{
    partial class PreviewLicenseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private LabelControl labelTitle;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerMain;
        private DevExpress.XtraEditors.GroupControl grpSummary;
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

        private PanelControl panelRightTop;
        private LabelControl lblExpiryDate;
        private LabelControl lblStatusBadge;

        private XtraTabControl tabControl;
        private XtraTabPage tabCanonical;
        private XtraTabPage tabInfo;

        private MemoEdit memoCanonicalJson;
        private TextEdit txtChecksum;
        private LabelControl lblChecksumStatus;
        private SimpleButton btnCopyJson;
        private SimpleButton btnExportJson;
        private SimpleButton btnValidateChecksum;

        private GridControl grdLicenseInfo;
        private GridView viewLicenseInfo;

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

        private void InitializeComponent()
        {
            this.components = new Container();

            // Form
            this.Text = "Preview License";
            this.ClientSize = new Size(980, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            labelTitle = new LabelControl
            {
                Text = "Preview License",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
                Location = new Point(12, 8),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(300, 28)
            };
            this.Controls.Add(labelTitle);

            // Split container main
            splitContainerMain = new DevExpress.XtraEditors.SplitContainerControl
            {
                Location = new Point(12, 44),
                Size = new Size(956, 420),
                SplitterPosition = 340,
                Horizontal = false,
                Dock = System.Windows.Forms.DockStyle.None
            };
            splitContainerMain.Panel1.Controls.Clear();
            splitContainerMain.Panel2.Controls.Clear();
            this.Controls.Add(splitContainerMain);

            // Top area: use another SplitContainerControl horizontally for left/right columns
            var topSplit = new DevExpress.XtraEditors.SplitContainerControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Horizontal = false
            };
            // Instead of reusing splitContainerMain panels, place topSplit manually and adjust below.
            // Create left GroupControl (summary)
            grpSummary = new GroupControl
            {
                Text = "Summary",
                Dock = System.Windows.Forms.DockStyle.Left,
                Width = 340
            };

            // Create summary fields placed vertically
            int labelX = 10, controlX = 110, y = 25, gap = 34;
            // CompanyName
            txtCompanyName = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblCN = new LabelControl { Text = "Company:", Location = new Point(labelX, y + 3), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblCN);
            grpSummary.Controls.Add(txtCompanyName);
            y += gap;

            // ProductId
            txtProductId = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblPID = new LabelControl { Text = "Product ID:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblPID);
            grpSummary.Controls.Add(txtProductId);
            y += gap;

            // ProductName
            txtProductName = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblPName = new LabelControl { Text = "Product Name:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblPName);
            grpSummary.Controls.Add(txtProductName);
            y += gap;

            // LicenseKey (monospace)
            txtLicenseKey = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            txtLicenseKey.Properties.Appearance.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            var lblKey = new LabelControl { Text = "License Key:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblKey);
            grpSummary.Controls.Add(txtLicenseKey);
            y += gap;

            // LicenseType
            txtLicenseType = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblType = new LabelControl { Text = "License Type:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblType);
            grpSummary.Controls.Add(txtLicenseType);
            y += gap;

            // ValidFrom
            txtValidFrom = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblFrom = new LabelControl { Text = "Valid From:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblFrom);
            grpSummary.Controls.Add(txtValidFrom);
            y += gap;

            // ValidTo
            txtValidTo = new TextEdit { Location = new Point(controlX, y), Width = 210, ReadOnly = true };
            var lblTo = new LabelControl { Text = "Valid To:", Location = new Point(labelX, y + 3), Size = new Size(90, 18) };
            grpSummary.Controls.Add(lblTo);
            grpSummary.Controls.Add(txtValidTo);
            y += gap + 6;

            // Modules grid
            grdModules = new GridControl { Location = new Point(10, y), Size = new Size(316, 180), MainView = null };
            viewModules = new GridView(grdModules) { Name = "viewModules", OptionsBehavior = { Editable = false }, OptionsView = { ShowGroupPanel = false } };
            grdModules.MainView = viewModules;
            grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewModules });
            colModuleName = new DevExpress.XtraGrid.Columns.GridColumn { FieldName = "ModuleName", Caption = "Module", Visible = true, VisibleIndex = 0 };
            colEnabled = new DevExpress.XtraGrid.Columns.GridColumn { FieldName = "Enabled", Caption = "Enabled", Visible = true, VisibleIndex = 1 };
            viewModules.Columns.AddRange(new[] { colModuleName, colEnabled });
            grpSummary.Controls.Add(grdModules);

            // Right top panel (expiry and badge)
            panelRightTop = new PanelControl { Dock = System.Windows.Forms.DockStyle.Fill, Location = new Point(360, 0), Size = new Size(580, 320) };
            lblExpiryDate = new LabelControl
            {
                Text = "Valid To (UTC):",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point),
                Location = new Point(16, 10),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(520, 48)
            };
            lblStatusBadge = new LabelControl
            {
                Text = "Status",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
                Location = new Point(16, 70),
                AutoSizeMode = LabelAutoSizeMode.Default
            };
            panelRightTop.Controls.Add(lblExpiryDate);
            panelRightTop.Controls.Add(lblStatusBadge);

            // Add left and right controls to splitContainerMain.Panel1
            splitContainerMain.Panel1.Controls.Add(grpSummary);
            splitContainerMain.Panel1.Controls.Add(panelRightTop);

            // Tab control in bottom area (splitContainerMain.Panel2)
            tabControl = new XtraTabControl { Dock = System.Windows.Forms.DockStyle.Fill };
            tabCanonical = new XtraTabPage { Text = "Canonical JSON" };
            tabInfo = new XtraTabPage { Text = "License Information" };
            tabControl.TabPages.AddRange(new[] { tabCanonical, tabInfo });

            // Canonical tab contents
            memoCanonicalJson = new MemoEdit { Dock = System.Windows.Forms.DockStyle.Top, Height = 300 };
            memoCanonicalJson.Properties.ReadOnly = true;
            memoCanonicalJson.Properties.Appearance.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            memoCanonicalJson.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tabCanonical.Controls.Add(memoCanonicalJson);

            txtChecksum = new TextEdit { Location = new Point(12, 312), Width = 520, ReadOnly = true };
            lblChecksumStatus = new LabelControl { Location = new Point(540, 315), Size = new Size(380, 24) };
            btnCopyJson = new SimpleButton { Text = "Copy JSON", Location = new Point(12, 345), Size = new Size(100, 28) };
            btnExportJson = new SimpleButton { Text = "Export JSON", Location = new Point(124, 345), Size = new Size(100, 28) };
            btnValidateChecksum = new SimpleButton { Text = "Validate Checksum", Location = new Point(236, 345), Size = new Size(140, 28) };

            tabCanonical.Controls.Add(txtChecksum);
            tabCanonical.Controls.Add(lblChecksumStatus);
            tabCanonical.Controls.Add(btnCopyJson);
            tabCanonical.Controls.Add(btnExportJson);
            tabCanonical.Controls.Add(btnValidateChecksum);

            // License Info grid (simple readonly)
            grdLicenseInfo = new GridControl { Dock = System.Windows.Forms.DockStyle.Fill };
            viewLicenseInfo = new GridView(grdLicenseInfo) { OptionsBehavior = { Editable = false }, OptionsView = { ShowGroupPanel = false } };
            grdLicenseInfo.MainView = viewLicenseInfo;
            grdLicenseInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewLicenseInfo });
            tabInfo.Controls.Add(grdLicenseInfo);

            // Add tabControl to Panel2
            splitContainerMain.Panel2.Controls.Add(tabControl);

            // Footer buttons
            btnBackToGenerate = new SimpleButton { Text = "Back to Generate", Location = new Point(12, 578), Size = new Size(140, 32) };
            btnClose = new SimpleButton { Text = "Close", Location = new Point(824, 578), Size = new Size(140, 32) };

            this.Controls.Add(btnBackToGenerate);
            this.Controls.Add(btnClose);

            // Wire up event handlers (actual event handler methods are in code-behind)
            btnCopyJson.Click += BtnCopyJson_Click;
            btnExportJson.Click += BtnExportJson_Click;
            btnValidateChecksum.Click += BtnValidateChecksum_Click;
            btnBackToGenerate.Click += BtnBackToGenerate_Click;
            btnClose.Click += BtnClose_Click;
        }

        #endregion

        // Designer stubs for event handler wiring; actual implementations in PreviewLicenseForm.cs
        private void BtnCopyJson_Click(object sender, EventArgs e) { /* wired in code-behind */ }
        private void BtnExportJson_Click(object sender, EventArgs e) { /* wired in code-behind */ }
        private void BtnValidateChecksum_Click(object sender, EventArgs e) { /* wired in code-behind */ }
        private void BtnBackToGenerate_Click(object sender, EventArgs e) { /* wired in code-behind */ }
        private void BtnClose_Click(object sender, EventArgs e) { /* wired in code-behind */ }
    }
}
