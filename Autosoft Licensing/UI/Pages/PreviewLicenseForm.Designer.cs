// Designer only — layout changes for PreviewLicenseForm.
// DO NOT modify code-behind or event handler signatures.

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;

namespace Autosoft_Licensing.UI
{
    partial class PreviewLicenseForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private DevExpress.XtraEditors.GroupControl grpHeader;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.LabelControl lblExpiryLarge;

        // Main layout
        private DevExpress.XtraLayout.LayoutControl layoutMain;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupMain;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemSummary;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemJson;

        // Left summary (must keep existing names)
        private DevExpress.XtraEditors.GroupControl grpSummary;
        private DevExpress.XtraEditors.LabelControl lblCompany;
        private DevExpress.XtraEditors.LabelControl lblProduct;
        private DevExpress.XtraEditors.LabelControl lblLicenseType;
        private DevExpress.XtraEditors.LabelControl lblValidFromUtc;
        private DevExpress.XtraEditors.LabelControl lblValidFromLocal;
        private DevExpress.XtraEditors.LabelControl lblValidToUtc;
        private DevExpress.XtraEditors.LabelControl lblValidToLocal;
        private DevExpress.XtraEditors.TextEdit txtLicenseKeySummary;
        private DevExpress.XtraGrid.GridControl grdModulesSummary;
        private DevExpress.XtraGrid.Views.Grid.GridView viewModulesSummary;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleEnabled;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleName;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit;

        // Right JSON / checksum (existing names preserved)
        private DevExpress.XtraEditors.GroupControl grpJson;
        private DevExpress.XtraEditors.SimpleButton btnCopyJson;
        private DevExpress.XtraEditors.SimpleButton btnExportJson;
        private DevExpress.XtraEditors.SimpleButton btnCopyChecksum;
        private DevExpress.XtraEditors.SimpleButton btnVerifyChecksum;
        private DevExpress.XtraEditors.MemoEdit memCanonicalJson;
        private DevExpress.XtraEditors.LabelControl lblChecksum;
        private DevExpress.XtraEditors.LabelControl lblVerifyResult;

        // Footer
        private DevExpress.XtraEditors.PanelControl grpFooter;
        private DevExpress.XtraEditors.SimpleButton btnExportAsl;
        private DevExpress.XtraEditors.SimpleButton btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpHeader = new DevExpress.XtraEditors.GroupControl();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblExpiryLarge = new DevExpress.XtraEditors.LabelControl();
            this.layoutMain = new DevExpress.XtraLayout.LayoutControl();
            this.grpSummary = new DevExpress.XtraEditors.GroupControl();
            this.lblCompany = new DevExpress.XtraEditors.LabelControl();
            this.lblProduct = new DevExpress.XtraEditors.LabelControl();
            this.lblLicenseType = new DevExpress.XtraEditors.LabelControl();
            this.lblValidFromUtc = new DevExpress.XtraEditors.LabelControl();
            this.lblValidFromLocal = new DevExpress.XtraEditors.LabelControl();
            this.lblValidToUtc = new DevExpress.XtraEditors.LabelControl();
            this.lblValidToLocal = new DevExpress.XtraEditors.LabelControl();
            this.txtLicenseKeySummary = new DevExpress.XtraEditors.TextEdit();
            this.grdModulesSummary = new DevExpress.XtraGrid.GridControl();
            this.viewModulesSummary = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colModuleEnabled = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colModuleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grpJson = new DevExpress.XtraEditors.GroupControl();
            this.btnCopyJson = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportJson = new DevExpress.XtraEditors.SimpleButton();
            this.btnCopyChecksum = new DevExpress.XtraEditors.SimpleButton();
            this.btnVerifyChecksum = new DevExpress.XtraEditors.SimpleButton();
            this.memCanonicalJson = new DevExpress.XtraEditors.MemoEdit();
            this.lblChecksum = new DevExpress.XtraEditors.LabelControl();
            this.lblVerifyResult = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroupMain = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutItemSummary = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutItemJson = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpFooter = new DevExpress.XtraEditors.PanelControl();
            this.btnExportAsl = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpHeader)).BeginInit();
            this.grpHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
            this.layoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpSummary)).BeginInit();
            this.grpSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKeySummary.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModulesSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModulesSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpJson)).BeginInit();
            this.grpJson.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memCanonicalJson.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutItemSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutItemJson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFooter)).BeginInit();
            this.grpFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpHeader
            // 
            this.grpHeader.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.grpHeader.Appearance.Options.UseBackColor = true;
            this.grpHeader.Controls.Add(this.lblTitle);
            this.grpHeader.Controls.Add(this.lblExpiryLarge);
            this.grpHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHeader.Location = new System.Drawing.Point(0, 0);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(1000, 80);
            this.grpHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitle.Location = new System.Drawing.Point(12, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(380, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Preview License";
            // 
            // lblExpiryLarge
            // 
            this.lblExpiryLarge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExpiryLarge.Appearance.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblExpiryLarge.Appearance.Options.UseFont = true;
            this.lblExpiryLarge.Appearance.Options.UseTextOptions = true;
            this.lblExpiryLarge.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblExpiryLarge.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblExpiryLarge.Location = new System.Drawing.Point(1476, 16);
            this.lblExpiryLarge.Name = "lblExpiryLarge";
            this.lblExpiryLarge.Size = new System.Drawing.Size(300, 44);
            this.lblExpiryLarge.TabIndex = 1;
            // 
            // layoutMain
            // 
            this.layoutMain.Controls.Add(this.grpSummary);
            this.layoutMain.Controls.Add(this.grpJson);
            this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMain.Location = new System.Drawing.Point(0, 80);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.Padding = new System.Windows.Forms.Padding(8);
            this.layoutMain.Root = this.layoutControlGroupMain;
            this.layoutMain.Size = new System.Drawing.Size(1000, 564);
            this.layoutMain.TabIndex = 0;
            // 
            // grpSummary
            // 
            this.grpSummary.Controls.Add(this.lblCompany);
            this.grpSummary.Controls.Add(this.lblProduct);
            this.grpSummary.Controls.Add(this.lblLicenseType);
            this.grpSummary.Controls.Add(this.lblValidFromUtc);
            this.grpSummary.Controls.Add(this.lblValidFromLocal);
            this.grpSummary.Controls.Add(this.lblValidToUtc);
            this.grpSummary.Controls.Add(this.lblValidToLocal);
            this.grpSummary.Controls.Add(this.txtLicenseKeySummary);
            this.grpSummary.Controls.Add(this.grdModulesSummary);
            this.grpSummary.Location = new System.Drawing.Point(12, 12);
            this.grpSummary.Name = "grpSummary";
            this.grpSummary.Padding = new System.Windows.Forms.Padding(8);
            this.grpSummary.Size = new System.Drawing.Size(356, 540);
            this.grpSummary.TabIndex = 4;
            this.grpSummary.Text = "Summary";
            // 
            // lblCompany
            // 
            this.lblCompany.Appearance.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblCompany.Appearance.Options.UseFont = true;
            this.lblCompany.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCompany.Location = new System.Drawing.Point(12, 28);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(320, 30);
            this.lblCompany.TabIndex = 0;
            this.lblCompany.Text = "Company Name";
            // 
            // lblProduct
            // 
            this.lblProduct.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblProduct.Appearance.Options.UseFont = true;
            this.lblProduct.Location = new System.Drawing.Point(12, 64);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(109, 17);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "Product ID / Name";
            // 
            // lblLicenseType
            // 
            this.lblLicenseType.Location = new System.Drawing.Point(12, 90);
            this.lblLicenseType.Name = "lblLicenseType";
            this.lblLicenseType.Size = new System.Drawing.Size(66, 13);
            this.lblLicenseType.TabIndex = 2;
            this.lblLicenseType.Text = "License Type:";
            // 
            // lblValidFromUtc
            // 
            this.lblValidFromUtc.Location = new System.Drawing.Point(12, 110);
            this.lblValidFromUtc.Name = "lblValidFromUtc";
            this.lblValidFromUtc.Size = new System.Drawing.Size(84, 13);
            this.lblValidFromUtc.TabIndex = 3;
            this.lblValidFromUtc.Text = "Valid From (UTC):";
            // 
            // lblValidFromLocal
            // 
            this.lblValidFromLocal.Location = new System.Drawing.Point(12, 126);
            this.lblValidFromLocal.Name = "lblValidFromLocal";
            this.lblValidFromLocal.Size = new System.Drawing.Size(88, 13);
            this.lblValidFromLocal.TabIndex = 4;
            this.lblValidFromLocal.Text = "Valid From (Local):";
            // 
            // lblValidToUtc
            // 
            this.lblValidToUtc.Location = new System.Drawing.Point(12, 146);
            this.lblValidToUtc.Name = "lblValidToUtc";
            this.lblValidToUtc.Size = new System.Drawing.Size(72, 13);
            this.lblValidToUtc.TabIndex = 5;
            this.lblValidToUtc.Text = "Valid To (UTC):";
            // 
            // lblValidToLocal
            // 
            this.lblValidToLocal.Location = new System.Drawing.Point(12, 162);
            this.lblValidToLocal.Name = "lblValidToLocal";
            this.lblValidToLocal.Size = new System.Drawing.Size(76, 13);
            this.lblValidToLocal.TabIndex = 6;
            this.lblValidToLocal.Text = "Valid To (Local):";
            // 
            // txtLicenseKeySummary
            // 
            this.txtLicenseKeySummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseKeySummary.Location = new System.Drawing.Point(12, 186);
            this.txtLicenseKeySummary.Name = "txtLicenseKeySummary";
            this.txtLicenseKeySummary.Properties.ReadOnly = true;
            this.txtLicenseKeySummary.Size = new System.Drawing.Size(324, 20);
            this.txtLicenseKeySummary.TabIndex = 7;
            // 
            // grdModulesSummary
            // 
            this.grdModulesSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdModulesSummary.Location = new System.Drawing.Point(12, 218);
            this.grdModulesSummary.MainView = this.viewModulesSummary;
            this.grdModulesSummary.Name = "grdModulesSummary";
            this.grdModulesSummary.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit});
            this.grdModulesSummary.Size = new System.Drawing.Size(324, 304);
            this.grdModulesSummary.TabIndex = 8;
            this.grdModulesSummary.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewModulesSummary});
            // 
            // viewModulesSummary
            // 
            this.viewModulesSummary.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colModuleEnabled,
            this.colModuleName});
            this.viewModulesSummary.GridControl = this.grdModulesSummary;
            this.viewModulesSummary.Name = "viewModulesSummary";
            this.viewModulesSummary.OptionsBehavior.Editable = false;
            this.viewModulesSummary.OptionsView.ShowGroupPanel = false;
            // 
            // colModuleEnabled
            // 
            this.colModuleEnabled.ColumnEdit = this.repositoryItemCheckEdit;
            this.colModuleEnabled.FieldName = "Enabled";
            this.colModuleEnabled.Name = "colModuleEnabled";
            this.colModuleEnabled.Visible = true;
            this.colModuleEnabled.VisibleIndex = 0;
            this.colModuleEnabled.Width = 48;
            // 
            // repositoryItemCheckEdit
            // 
            this.repositoryItemCheckEdit.AutoHeight = false;
            this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
            // 
            // colModuleName
            // 
            this.colModuleName.Caption = "Module";
            this.colModuleName.FieldName = "ModuleName";
            this.colModuleName.Name = "colModuleName";
            this.colModuleName.Visible = true;
            this.colModuleName.VisibleIndex = 1;
            this.colModuleName.Width = 240;
            // 
            // grpJson
            // 
            this.grpJson.Controls.Add(this.btnCopyJson);
            this.grpJson.Controls.Add(this.btnExportJson);
            this.grpJson.Controls.Add(this.btnCopyChecksum);
            this.grpJson.Controls.Add(this.btnVerifyChecksum);
            this.grpJson.Controls.Add(this.memCanonicalJson);
            this.grpJson.Controls.Add(this.lblChecksum);
            this.grpJson.Controls.Add(this.lblVerifyResult);
            this.grpJson.Location = new System.Drawing.Point(372, 12);
            this.grpJson.Name = "grpJson";
            this.grpJson.Padding = new System.Windows.Forms.Padding(8);
            this.grpJson.Size = new System.Drawing.Size(616, 540);
            this.grpJson.TabIndex = 5;
            this.grpJson.Text = "Canonical JSON & Checksum";
            // 
            // btnCopyJson
            // 
            this.btnCopyJson.Location = new System.Drawing.Point(12, 28);
            this.btnCopyJson.Name = "btnCopyJson";
            this.btnCopyJson.Size = new System.Drawing.Size(100, 28);
            this.btnCopyJson.TabIndex = 0;
            this.btnCopyJson.Text = "Copy JSON";
            this.btnCopyJson.Click += new System.EventHandler(this.btnCopyJson_Click);
            // 
            // btnExportJson
            // 
            this.btnExportJson.Location = new System.Drawing.Point(120, 28);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(100, 28);
            this.btnExportJson.TabIndex = 1;
            this.btnExportJson.Text = "Export JSON";
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            // 
            // btnCopyChecksum
            // 
            this.btnCopyChecksum.Location = new System.Drawing.Point(228, 28);
            this.btnCopyChecksum.Name = "btnCopyChecksum";
            this.btnCopyChecksum.Size = new System.Drawing.Size(120, 28);
            this.btnCopyChecksum.TabIndex = 2;
            this.btnCopyChecksum.Text = "Copy Checksum";
            this.btnCopyChecksum.Click += new System.EventHandler(this.btnCopyChecksum_Click);
            // 
            // btnVerifyChecksum
            // 
            this.btnVerifyChecksum.Location = new System.Drawing.Point(356, 28);
            this.btnVerifyChecksum.Name = "btnVerifyChecksum";
            this.btnVerifyChecksum.Size = new System.Drawing.Size(120, 28);
            this.btnVerifyChecksum.TabIndex = 3;
            this.btnVerifyChecksum.Text = "Verify Checksum";
            this.btnVerifyChecksum.Click += new System.EventHandler(this.btnVerifyChecksum_Click);
            // 
            // memCanonicalJson
            // 
            this.memCanonicalJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memCanonicalJson.Location = new System.Drawing.Point(12, 64);
            this.memCanonicalJson.Name = "memCanonicalJson";
            this.memCanonicalJson.Properties.Appearance.Font = new System.Drawing.Font("Consolas", 10F);
            this.memCanonicalJson.Properties.Appearance.Options.UseFont = true;
            this.memCanonicalJson.Properties.ReadOnly = true;
            this.memCanonicalJson.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.memCanonicalJson.Properties.WordWrap = false;
            this.memCanonicalJson.Size = new System.Drawing.Size(588, 424);
            this.memCanonicalJson.TabIndex = 4;
            // 
            // lblChecksum
            // 
            this.lblChecksum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChecksum.Location = new System.Drawing.Point(12, 496);
            this.lblChecksum.Name = "lblChecksum";
            this.lblChecksum.Size = new System.Drawing.Size(0, 13);
            this.lblChecksum.TabIndex = 5;
            // 
            // lblVerifyResult
            // 
            this.lblVerifyResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVerifyResult.Location = new System.Drawing.Point(12, 516);
            this.lblVerifyResult.Name = "lblVerifyResult";
            this.lblVerifyResult.Size = new System.Drawing.Size(0, 13);
            this.lblVerifyResult.TabIndex = 6;
            // 
            // layoutControlGroupMain
            // 
            this.layoutControlGroupMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupMain.GroupBordersVisible = false;
            this.layoutControlGroupMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSummary,
            this.layoutItemJson});
            this.layoutControlGroupMain.Name = "layoutControlGroupMain";
            this.layoutControlGroupMain.Size = new System.Drawing.Size(1000, 564);
            // 
            // layoutItemSummary
            // 
            this.layoutItemSummary.Control = this.grpSummary;
            this.layoutItemSummary.Location = new System.Drawing.Point(0, 0);
            this.layoutItemSummary.MaxSize = new System.Drawing.Size(360, 0);
            this.layoutItemSummary.MinSize = new System.Drawing.Size(360, 100);
            this.layoutItemSummary.Name = "layoutItemSummary";
            this.layoutItemSummary.Size = new System.Drawing.Size(360, 544);
            this.layoutItemSummary.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutItemSummary.TextSize = new System.Drawing.Size(0, 0);
            this.layoutItemSummary.TextVisible = false;
            // 
            // layoutItemJson
            // 
            this.layoutItemJson.Control = this.grpJson;
            this.layoutItemJson.Location = new System.Drawing.Point(360, 0);
            this.layoutItemJson.Name = "layoutItemJson";
            this.layoutItemJson.Size = new System.Drawing.Size(620, 544);
            this.layoutItemJson.TextSize = new System.Drawing.Size(0, 0);
            this.layoutItemJson.TextVisible = false;
            // 
            // grpFooter
            // 
            this.grpFooter.Controls.Add(this.btnExportAsl);
            this.grpFooter.Controls.Add(this.btnClose);
            this.grpFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpFooter.Location = new System.Drawing.Point(0, 644);
            this.grpFooter.Name = "grpFooter";
            this.grpFooter.Padding = new System.Windows.Forms.Padding(8);
            this.grpFooter.Size = new System.Drawing.Size(1000, 56);
            this.grpFooter.TabIndex = 1;
            // 
            // btnExportAsl
            // 
            this.btnExportAsl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportAsl.Enabled = false;
            this.btnExportAsl.Location = new System.Drawing.Point(680, 12);
            this.btnExportAsl.Name = "btnExportAsl";
            this.btnExportAsl.Size = new System.Drawing.Size(140, 32);
            this.btnExportAsl.TabIndex = 0;
            this.btnExportAsl.Text = "Export ASL";
            this.btnExportAsl.Click += new System.EventHandler(this.btnExportAsl_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(840, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // PreviewLicenseForm
            // 
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.layoutMain);
            this.Controls.Add(this.grpFooter);
            this.Controls.Add(this.grpHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreviewLicenseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preview License";
            ((System.ComponentModel.ISupportInitialize)(this.grpHeader)).EndInit();
            this.grpHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
            this.layoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpSummary)).EndInit();
            this.grpSummary.ResumeLayout(false);
            this.grpSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKeySummary.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModulesSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModulesSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpJson)).EndInit();
            this.grpJson.ResumeLayout(false);
            this.grpJson.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memCanonicalJson.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutItemSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutItemJson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFooter)).EndInit();
            this.grpFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
