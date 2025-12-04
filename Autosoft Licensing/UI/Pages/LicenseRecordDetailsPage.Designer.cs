using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Linq;

namespace Autosoft_Licensing.UI.Pages
{
    partial class LicenseRecordDetailsPage
    {
        private IContainer components = null;

        // Header
        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;

        // Info fields
        private LabelControl lblCompanyName;
        private TextEdit txtCompanyName;
        private LabelControl lblIssueDate;
        private DateEdit dtIssueDate;

        private LabelControl lblProductName;
        private TextEdit txtProductName;
        private LabelControl lblExpiryDate;
        private DateEdit dtExpiryDate;

        private LabelControl lblProductCode;
        private TextEdit txtProductCode;
        private LabelControl lblLicenseType;
        private TextEdit txtLicenseType;
        private LabelControl lblStatus;
        private TextEdit txtStatus;

        private LabelControl lblGeneratedBy;
        private TextEdit txtGeneratedBy;

        // NEW: Currency
        private LabelControl lblCurrency;
        private TextEdit txtCurrency;

        // Checksum controls
        private LabelControl lblChecksum;
        private TextEdit txtChecksum;
        private SimpleButton btnCopyChecksum;
        private SimpleButton btnVerifyChecksum;
        private LabelControl lblChecksumStatus;

        // Modules grid
        private LabelControl lblModules;
        private GridControl grdModules;
        private GridView viewModules;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleName;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleDescription;

        // Remark
        private LabelControl lblRemark;
        private MemoEdit memRemark;

        // Action buttons
        private SimpleButton btnBack;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblHeaderTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.txtCompanyName = new DevExpress.XtraEditors.TextEdit();
            this.lblIssueDate = new DevExpress.XtraEditors.LabelControl();
            this.dtIssueDate = new DevExpress.XtraEditors.DateEdit();
            this.lblProductName = new DevExpress.XtraEditors.LabelControl();
            this.txtProductName = new DevExpress.XtraEditors.TextEdit();
            this.lblExpiryDate = new DevExpress.XtraEditors.LabelControl();
            this.dtExpiryDate = new DevExpress.XtraEditors.DateEdit();
            this.lblProductCode = new DevExpress.XtraEditors.LabelControl();
            this.txtProductCode = new DevExpress.XtraEditors.TextEdit();
            this.lblLicenseType = new DevExpress.XtraEditors.LabelControl();
            this.txtLicenseType = new DevExpress.XtraEditors.TextEdit();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.txtStatus = new DevExpress.XtraEditors.TextEdit();
            this.lblGeneratedBy = new DevExpress.XtraEditors.LabelControl();
            this.txtGeneratedBy = new DevExpress.XtraEditors.TextEdit();
            this.lblCurrency = new DevExpress.XtraEditors.LabelControl();
            this.txtCurrency = new DevExpress.XtraEditors.TextEdit();
            this.lblChecksum = new DevExpress.XtraEditors.LabelControl();
            this.txtChecksum = new DevExpress.XtraEditors.TextEdit();
            this.btnCopyChecksum = new DevExpress.XtraEditors.SimpleButton();
            this.btnVerifyChecksum = new DevExpress.XtraEditors.SimpleButton();
            this.lblChecksumStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblModules = new DevExpress.XtraEditors.LabelControl();
            this.grdModules = new DevExpress.XtraGrid.GridControl();
            this.viewModules = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colModuleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModuleDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblRemark = new DevExpress.XtraEditors.LabelControl();
            this.memRemark = new DevExpress.XtraEditors.MemoEdit();
            this.btnBack = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeneratedBy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrency.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChecksum.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(211)))));
            this.headerPanel.Appearance.Options.UseBackColor = true;
            this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.headerPanel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1168, 60);
            this.headerPanel.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.Appearance.Options.UseFont = true;
            this.lblHeaderTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeaderTitle.Location = new System.Drawing.Point(18, 18);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(300, 28);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "License Record Details";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Location = new System.Drawing.Point(34, 80);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(82, 13);
            this.lblCompanyName.TabIndex = 1;
            this.lblCompanyName.Text = "Company Name :";
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(150, 78);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Properties.ReadOnly = true;
            this.txtCompanyName.Size = new System.Drawing.Size(320, 20);
            this.txtCompanyName.TabIndex = 2;
            // 
            // lblIssueDate
            // 
            this.lblIssueDate.Location = new System.Drawing.Point(570, 80);
            this.lblIssueDate.Name = "lblIssueDate";
            this.lblIssueDate.Size = new System.Drawing.Size(59, 13);
            this.lblIssueDate.TabIndex = 3;
            this.lblIssueDate.Text = "Issue Date :";
            // 
            // dtIssueDate
            // 
            this.dtIssueDate.EditValue = null;
            this.dtIssueDate.Location = new System.Drawing.Point(660, 78);
            this.dtIssueDate.Name = "dtIssueDate";
            this.dtIssueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtIssueDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtIssueDate.Properties.ReadOnly = true;
            this.dtIssueDate.Size = new System.Drawing.Size(150, 20);
            this.dtIssueDate.TabIndex = 4;
            // 
            // lblProductName
            // 
            this.lblProductName.Location = new System.Drawing.Point(34, 116);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(74, 13);
            this.lblProductName.TabIndex = 5;
            this.lblProductName.Text = "Product Name :";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(150, 114);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Properties.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(320, 20);
            this.txtProductName.TabIndex = 6;
            // 
            // lblExpiryDate
            // 
            this.lblExpiryDate.Location = new System.Drawing.Point(570, 116);
            this.lblExpiryDate.Name = "lblExpiryDate";
            this.lblExpiryDate.Size = new System.Drawing.Size(63, 13);
            this.lblExpiryDate.TabIndex = 7;
            this.lblExpiryDate.Text = "Expiry Date :";
            // 
            // dtExpiryDate
            // 
            this.dtExpiryDate.EditValue = null;
            this.dtExpiryDate.Location = new System.Drawing.Point(660, 114);
            this.dtExpiryDate.Name = "dtExpiryDate";
            this.dtExpiryDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiryDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiryDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtExpiryDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtExpiryDate.Properties.ReadOnly = true;
            this.dtExpiryDate.Size = new System.Drawing.Size(150, 20);
            this.dtExpiryDate.TabIndex = 8;
            // 
            // lblProductCode
            // 
            this.lblProductCode.Location = new System.Drawing.Point(34, 152);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(72, 13);
            this.lblProductCode.TabIndex = 9;
            this.lblProductCode.Text = "Product Code :";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(150, 150);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Properties.ReadOnly = true;
            this.txtProductCode.Size = new System.Drawing.Size(150, 20);
            this.txtProductCode.TabIndex = 10;
            // 
            // lblLicenseType
            // 
            this.lblLicenseType.Location = new System.Drawing.Point(320, 152);
            this.lblLicenseType.Name = "lblLicenseType";
            this.lblLicenseType.Size = new System.Drawing.Size(69, 13);
            this.lblLicenseType.TabIndex = 11;
            this.lblLicenseType.Text = "License Type :";
            // 
            // txtLicenseType
            // 
            this.txtLicenseType.Location = new System.Drawing.Point(410, 150);
            this.txtLicenseType.Name = "txtLicenseType";
            this.txtLicenseType.Properties.ReadOnly = true;
            this.txtLicenseType.Size = new System.Drawing.Size(120, 20);
            this.txtLicenseType.TabIndex = 12;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(570, 152);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 13);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Status :";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(660, 150);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Properties.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(150, 20);
            this.txtStatus.TabIndex = 14;
            // 
            // lblGeneratedBy
            // 
            this.lblGeneratedBy.Location = new System.Drawing.Point(570, 188);
            this.lblGeneratedBy.Name = "lblGeneratedBy";
            this.lblGeneratedBy.Size = new System.Drawing.Size(73, 13);
            this.lblGeneratedBy.TabIndex = 15;
            this.lblGeneratedBy.Text = "Generated By :";
            // 
            // txtGeneratedBy
            // 
            this.txtGeneratedBy.Location = new System.Drawing.Point(660, 186);
            this.txtGeneratedBy.Name = "txtGeneratedBy";
            this.txtGeneratedBy.Properties.ReadOnly = true;
            this.txtGeneratedBy.Size = new System.Drawing.Size(150, 20);
            this.txtGeneratedBy.TabIndex = 16;
            // 
            // lblCurrency
            // 
            this.lblCurrency.Location = new System.Drawing.Point(34, 188);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(51, 13);
            this.lblCurrency.TabIndex = 17;
            this.lblCurrency.Text = "Currency :";
            // 
            // txtCurrency
            // 
            this.txtCurrency.Location = new System.Drawing.Point(150, 186);
            this.txtCurrency.Name = "txtCurrency";
            this.txtCurrency.Properties.ReadOnly = true;
            this.txtCurrency.Size = new System.Drawing.Size(120, 20);
            this.txtCurrency.TabIndex = 18;
            // 
            // lblChecksum
            // 
            this.lblChecksum.Location = new System.Drawing.Point(34, 224);
            this.lblChecksum.Name = "lblChecksum";
            this.lblChecksum.Size = new System.Drawing.Size(108, 13);
            this.lblChecksum.TabIndex = 19;
            this.lblChecksum.Text = "Checksum (SHA-256) :";
            // 
            // txtChecksum
            // 
            this.txtChecksum.Location = new System.Drawing.Point(150, 222);
            this.txtChecksum.Name = "txtChecksum";
            this.txtChecksum.Properties.ReadOnly = true;
            this.txtChecksum.Size = new System.Drawing.Size(240, 20);
            this.txtChecksum.TabIndex = 20;
            // 
            // btnCopyChecksum
            // 
            this.btnCopyChecksum.Location = new System.Drawing.Point(396, 220);
            this.btnCopyChecksum.Name = "btnCopyChecksum";
            this.btnCopyChecksum.Size = new System.Drawing.Size(60, 24);
            this.btnCopyChecksum.TabIndex = 21;
            this.btnCopyChecksum.Text = "Copy";
            // 
            // btnVerifyChecksum
            // 
            this.btnVerifyChecksum.Location = new System.Drawing.Point(462, 220);
            this.btnVerifyChecksum.Name = "btnVerifyChecksum";
            this.btnVerifyChecksum.Size = new System.Drawing.Size(60, 24);
            this.btnVerifyChecksum.TabIndex = 22;
            this.btnVerifyChecksum.Text = "Verify";
            // 
            // lblChecksumStatus
            // 
            this.lblChecksumStatus.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblChecksumStatus.Appearance.Options.UseFont = true;
            this.lblChecksumStatus.Location = new System.Drawing.Point(528, 226);
            this.lblChecksumStatus.Name = "lblChecksumStatus";
            this.lblChecksumStatus.Size = new System.Drawing.Size(0, 15);
            this.lblChecksumStatus.TabIndex = 23;
            // 
            // lblModules
            // 
            this.lblModules.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblModules.Appearance.Options.UseFont = true;
            this.lblModules.Location = new System.Drawing.Point(34, 260);
            this.lblModules.Name = "lblModules";
            this.lblModules.Size = new System.Drawing.Size(47, 17);
            this.lblModules.TabIndex = 24;
            this.lblModules.Text = "Module";
            // 
            // grdModules
            // 
            this.grdModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdModules.Location = new System.Drawing.Point(34, 285);
            this.grdModules.MainView = this.viewModules;
            this.grdModules.Name = "grdModules";
            this.grdModules.Size = new System.Drawing.Size(1100, 200);
            this.grdModules.TabIndex = 25;
            this.grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewModules});
            // 
            // viewModules
            // 
            this.viewModules.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colModuleName,
            this.colModuleDescription});
            this.viewModules.GridControl = this.grdModules;
            this.viewModules.Name = "viewModules";
            this.viewModules.OptionsBehavior.Editable = false;
            this.viewModules.OptionsView.ShowGroupPanel = false;
            this.viewModules.OptionsView.ShowIndicator = false;
            // 
            // colModuleName
            // 
            this.colModuleName.Caption = "Name";
            this.colModuleName.FieldName = "Name";
            this.colModuleName.Name = "colModuleName";
            this.colModuleName.Visible = true;
            this.colModuleName.VisibleIndex = 0;
            this.colModuleName.Width = 200;
            // 
            // colModuleDescription
            // 
            this.colModuleDescription.Caption = "Description";
            this.colModuleDescription.FieldName = "Description";
            this.colModuleDescription.Name = "colModuleDescription";
            this.colModuleDescription.Visible = true;
            this.colModuleDescription.VisibleIndex = 1;
            this.colModuleDescription.Width = 500;
            // 
            // lblRemark
            // 
            this.lblRemark.Location = new System.Drawing.Point(34, 505);
            this.lblRemark.Name = "lblRemark";
            this.lblRemark.Size = new System.Drawing.Size(43, 13);
            this.lblRemark.TabIndex = 26;
            this.lblRemark.Text = "Remark :";
            // 
            // memRemark
            // 
            this.memRemark.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memRemark.Location = new System.Drawing.Point(34, 525);
            this.memRemark.Name = "memRemark";
            this.memRemark.Properties.ReadOnly = true;
            this.memRemark.Size = new System.Drawing.Size(1100, 57);
            this.memRemark.TabIndex = 27;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnBack.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBack.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnBack.Appearance.Options.UseBackColor = true;
            this.btnBack.Appearance.Options.UseFont = true;
            this.btnBack.Appearance.Options.UseForeColor = true;
            this.btnBack.Location = new System.Drawing.Point(1034, 602);
            this.btnBack.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 36);
            this.btnBack.TabIndex = 28;
            this.btnBack.Text = "Back";
            // 
            // LicenseRecordDetailsPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.memRemark);
            this.Controls.Add(this.lblRemark);
            this.Controls.Add(this.grdModules);
            this.Controls.Add(this.lblModules);
            this.Controls.Add(this.lblChecksumStatus);
            this.Controls.Add(this.btnVerifyChecksum);
            this.Controls.Add(this.btnCopyChecksum);
            this.Controls.Add(this.txtChecksum);
            this.Controls.Add(this.lblChecksum);
            this.Controls.Add(this.txtCurrency);
            this.Controls.Add(this.lblCurrency);
            this.Controls.Add(this.txtGeneratedBy);
            this.Controls.Add(this.lblGeneratedBy);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtLicenseType);
            this.Controls.Add(this.lblLicenseType);
            this.Controls.Add(this.txtProductCode);
            this.Controls.Add(this.lblProductCode);
            this.Controls.Add(this.dtExpiryDate);
            this.Controls.Add(this.lblExpiryDate);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.dtIssueDate);
            this.Controls.Add(this.lblIssueDate);
            this.Controls.Add(this.txtCompanyName);
            this.Controls.Add(this.lblCompanyName);
            this.Controls.Add(this.headerPanel);
            this.Name = "LicenseRecordDetailsPage";
            this.Size = new System.Drawing.Size(1168, 652);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeneratedBy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrency.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChecksum.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}