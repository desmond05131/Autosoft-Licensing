using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Autosoft_Licensing.UI.Pages
{
    partial class LicenseRecordsPage
    {
        private IContainer components = null;

        // ... [Member variables omitted for brevity, identical to previous] ...
        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;
        private PanelControl navPanel;
        private PanelControl btnNav_GenerateLicense;
        private PictureEdit picNav_Generate;
        private LabelControl lblNav_Generate;
        private PanelControl btnNav_LicenseRecords;
        private PictureEdit picNav_Records;
        private LabelControl lblNav_Records;
        private PanelControl underlineRecords;
        private PanelControl btnNav_ManageProduct;
        private PictureEdit picNav_Product;
        private LabelControl lblNav_Product;
        private PanelControl btnNav_ManageUser;
        private PictureEdit picNav_User;
        private LabelControl lblNav_User;
        private PanelControl btnNav_GeneralSetting;
        private PictureEdit picNav_Setting;
        private LabelControl lblNav_Setting;
        private PanelControl btnNav_Logout;
        private PictureEdit picNav_Logout;
        private LabelControl lblNav_Logout;
        private GroupControl grpFiltering;
        private LabelControl lblCompanyName;
        private ComboBoxEdit cmbCompanyName;
        private LabelControl lblProductCode;
        private ComboBoxEdit cmbProductCode;
        private LabelControl lblLicenseType;
        private ComboBoxEdit cmbLicenseType;
        private LabelControl lblIssueDate;
        private ComboBoxEdit cmbIssueDateMode;
        private DateEdit dtIssueDateSingle;
        private LabelControl lblIssueDateFrom;
        private DateEdit dtIssueDateFrom;
        private LabelControl lblIssueDateTo;
        private DateEdit dtIssueDateTo;
        private LabelControl lblExpiryDate;
        private ComboBoxEdit cmbExpiryDateMode;
        private DateEdit dtExpiryDateSingle;
        private LabelControl lblExpiryDateFrom;
        private DateEdit dtExpiryDateFrom;
        private LabelControl lblExpiryDateTo;
        private DateEdit dtExpiryDateTo;
        private CheckEdit chkShowExpired;
        private LabelControl lblCountdownDays;
        private SpinEdit numCountdownDays;
        private SimpleButton btnRefresh;
        private GridControl grdLicenses;
        private GridView viewLicenses;
        private DevExpress.XtraGrid.Columns.GridColumn colCompanyName;
        private DevExpress.XtraGrid.Columns.GridColumn colProductCode;
        private DevExpress.XtraGrid.Columns.GridColumn colProductName;
        private DevExpress.XtraGrid.Columns.GridColumn colLicenseType;
        private DevExpress.XtraGrid.Columns.GridColumn colIssueDate;
        private DevExpress.XtraGrid.Columns.GridColumn colExpiryDate;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colCountdown;
        private SimpleButton btnCreate;
        private SimpleButton btnView;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;

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
            this.navPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_GenerateLicense = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Generate = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Generate = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_LicenseRecords = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Records = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Records = new DevExpress.XtraEditors.LabelControl();
            this.underlineRecords = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_ManageProduct = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Product = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Product = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_ManageUser = new DevExpress.XtraEditors.PanelControl();
            this.picNav_User = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_User = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_GeneralSetting = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Setting = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Setting = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_Logout = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Logout = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Logout = new DevExpress.XtraEditors.LabelControl();
            this.grpFiltering = new DevExpress.XtraEditors.GroupControl();
            this.lblCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.cmbCompanyName = new DevExpress.XtraEditors.ComboBoxEdit();
            this.chkShowExpired = new DevExpress.XtraEditors.CheckEdit();
            this.lblProductCode = new DevExpress.XtraEditors.LabelControl();
            this.cmbProductCode = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblCountdownDays = new DevExpress.XtraEditors.LabelControl();
            this.numCountdownDays = new DevExpress.XtraEditors.SpinEdit();
            this.lblLicenseType = new DevExpress.XtraEditors.LabelControl();
            this.cmbLicenseType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblIssueDate = new DevExpress.XtraEditors.LabelControl();
            this.cmbIssueDateMode = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dtIssueDateSingle = new DevExpress.XtraEditors.DateEdit();
            this.lblIssueDateFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtIssueDateFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblIssueDateTo = new DevExpress.XtraEditors.LabelControl();
            this.dtIssueDateTo = new DevExpress.XtraEditors.DateEdit();
            this.lblExpiryDate = new DevExpress.XtraEditors.LabelControl();
            this.cmbExpiryDateMode = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dtExpiryDateSingle = new DevExpress.XtraEditors.DateEdit();
            this.lblExpiryDateFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtExpiryDateFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblExpiryDateTo = new DevExpress.XtraEditors.LabelControl();
            this.dtExpiryDateTo = new DevExpress.XtraEditors.DateEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.grdLicenses = new DevExpress.XtraGrid.GridControl();
            this.viewLicenses = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCompanyName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colProductCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLicenseType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIssueDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colExpiryDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCountdown = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.btnView = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navPanel)).BeginInit();
            this.navPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GenerateLicense)).BeginInit();
            this.btnNav_GenerateLicense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Generate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_LicenseRecords)).BeginInit();
            this.btnNav_LicenseRecords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Records.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlineRecords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).BeginInit();
            this.btnNav_ManageProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).BeginInit();
            this.btnNav_ManageUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GeneralSetting)).BeginInit();
            this.btnNav_GeneralSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Setting.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_Logout)).BeginInit();
            this.btnNav_Logout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFiltering)).BeginInit();
            this.grpFiltering.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowExpired.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProductCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountdownDays.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLicenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIssueDateMode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateSingle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateSingle.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExpiryDateMode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateSingle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateSingle.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLicenses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLicenses)).BeginInit();
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
            this.headerPanel.Size = new System.Drawing.Size(999, 60);
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
            this.lblHeaderTitle.Text = "Autosoft Licensing";
            // 
            // navPanel
            // 
            this.navPanel.Appearance.BackColor = System.Drawing.Color.White;
            this.navPanel.Appearance.Options.UseBackColor = true;
            this.navPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.navPanel.Controls.Add(this.btnNav_GenerateLicense);
            this.navPanel.Controls.Add(this.btnNav_LicenseRecords);
            this.navPanel.Controls.Add(this.btnNav_ManageProduct);
            this.navPanel.Controls.Add(this.btnNav_ManageUser);
            this.navPanel.Controls.Add(this.btnNav_GeneralSetting);
            this.navPanel.Controls.Add(this.btnNav_Logout);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navPanel.Location = new System.Drawing.Point(0, 60);
            this.navPanel.Name = "navPanel";
            this.navPanel.Size = new System.Drawing.Size(999, 52);
            this.navPanel.TabIndex = 1;
            // 
            // btnNav_GenerateLicense
            // 
            this.btnNav_GenerateLicense.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_GenerateLicense.Appearance.BorderColor = System.Drawing.Color.Transparent;
            this.btnNav_GenerateLicense.Appearance.Options.UseBackColor = true;
            this.btnNav_GenerateLicense.Appearance.Options.UseBorderColor = true;
            this.btnNav_GenerateLicense.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_GenerateLicense.Controls.Add(this.picNav_Generate);
            this.btnNav_GenerateLicense.Controls.Add(this.lblNav_Generate);
            this.btnNav_GenerateLicense.Location = new System.Drawing.Point(12, 6);
            this.btnNav_GenerateLicense.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNav_GenerateLicense.Name = "btnNav_GenerateLicense";
            this.btnNav_GenerateLicense.Size = new System.Drawing.Size(180, 44);
            this.btnNav_GenerateLicense.TabIndex = 0;
            // 
            // picNav_Generate
            // 
            this.picNav_Generate.EditValue = global::Autosoft_Licensing.Properties.Resources.Generate;
            this.picNav_Generate.Location = new System.Drawing.Point(78, 4);
            this.picNav_Generate.Name = "picNav_Generate";
            this.picNav_Generate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Generate.Properties.ReadOnly = true;
            this.picNav_Generate.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Generate.Size = new System.Drawing.Size(24, 24);
            this.picNav_Generate.TabIndex = 0;
            // 
            // lblNav_Generate
            // 
            this.lblNav_Generate.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_Generate.Appearance.Options.UseFont = true;
            this.lblNav_Generate.Appearance.Options.UseTextOptions = true;
            this.lblNav_Generate.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNav_Generate.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_Generate.Location = new System.Drawing.Point(0, 28);
            this.lblNav_Generate.Name = "lblNav_Generate";
            this.lblNav_Generate.Size = new System.Drawing.Size(180, 16);
            this.lblNav_Generate.TabIndex = 1;
            this.lblNav_Generate.Text = "Generate License";
            // 
            // btnNav_LicenseRecords
            // 
            this.btnNav_LicenseRecords.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_LicenseRecords.Appearance.Options.UseBackColor = true;
            this.btnNav_LicenseRecords.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_LicenseRecords.Controls.Add(this.picNav_Records);
            this.btnNav_LicenseRecords.Controls.Add(this.lblNav_Records);
            this.btnNav_LicenseRecords.Controls.Add(this.underlineRecords);
            this.btnNav_LicenseRecords.Location = new System.Drawing.Point(206, 6);
            this.btnNav_LicenseRecords.Name = "btnNav_LicenseRecords";
            this.btnNav_LicenseRecords.Size = new System.Drawing.Size(150, 44);
            this.btnNav_LicenseRecords.TabIndex = 1;
            // 
            // picNav_Records
            // 
            this.picNav_Records.EditValue = global::Autosoft_Licensing.Properties.Resources.Records;
            this.picNav_Records.Location = new System.Drawing.Point(63, 4);
            this.picNav_Records.Name = "picNav_Records";
            this.picNav_Records.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Records.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Records.Size = new System.Drawing.Size(24, 24);
            this.picNav_Records.TabIndex = 0;
            // 
            // lblNav_Records
            // 
            this.lblNav_Records.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_Records.Appearance.Options.UseFont = true;
            this.lblNav_Records.Appearance.Options.UseTextOptions = true;
            this.lblNav_Records.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNav_Records.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_Records.Location = new System.Drawing.Point(0, 28);
            this.lblNav_Records.Name = "lblNav_Records";
            this.lblNav_Records.Size = new System.Drawing.Size(150, 16);
            this.lblNav_Records.TabIndex = 1;
            this.lblNav_Records.Text = "License Records";
            // 
            // underlineRecords
            // 
            this.underlineRecords.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(153)))), ((int)(((byte)(51)))));
            this.underlineRecords.Appearance.Options.UseBackColor = true;
            this.underlineRecords.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.underlineRecords.Location = new System.Drawing.Point(20, 34);
            this.underlineRecords.Name = "underlineRecords";
            this.underlineRecords.Size = new System.Drawing.Size(110, 4);
            this.underlineRecords.TabIndex = 2;
            // 
            // btnNav_ManageProduct
            // 
            this.btnNav_ManageProduct.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_ManageProduct.Appearance.Options.UseBackColor = true;
            this.btnNav_ManageProduct.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_ManageProduct.Controls.Add(this.picNav_Product);
            this.btnNav_ManageProduct.Controls.Add(this.lblNav_Product);
            this.btnNav_ManageProduct.Location = new System.Drawing.Point(366, 6);
            this.btnNav_ManageProduct.Name = "btnNav_ManageProduct";
            this.btnNav_ManageProduct.Size = new System.Drawing.Size(150, 44);
            this.btnNav_ManageProduct.TabIndex = 2;
            // 
            // picNav_Product
            // 
            this.picNav_Product.EditValue = global::Autosoft_Licensing.Properties.Resources.Product;
            this.picNav_Product.Location = new System.Drawing.Point(63, 4);
            this.picNav_Product.Name = "picNav_Product";
            this.picNav_Product.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Product.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Product.Size = new System.Drawing.Size(24, 24);
            this.picNav_Product.TabIndex = 0;
            // 
            // lblNav_Product
            // 
            this.lblNav_Product.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_Product.Appearance.Options.UseFont = true;
            this.lblNav_Product.Appearance.Options.UseTextOptions = true;
            this.lblNav_Product.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNav_Product.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_Product.Location = new System.Drawing.Point(0, 28);
            this.lblNav_Product.Name = "lblNav_Product";
            this.lblNav_Product.Size = new System.Drawing.Size(150, 16);
            this.lblNav_Product.TabIndex = 1;
            this.lblNav_Product.Text = "Manage Product";
            // 
            // btnNav_ManageUser
            // 
            this.btnNav_ManageUser.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_ManageUser.Appearance.Options.UseBackColor = true;
            this.btnNav_ManageUser.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_ManageUser.Controls.Add(this.picNav_User);
            this.btnNav_ManageUser.Controls.Add(this.lblNav_User);
            this.btnNav_ManageUser.Location = new System.Drawing.Point(526, 6);
            this.btnNav_ManageUser.Name = "btnNav_ManageUser";
            this.btnNav_ManageUser.Size = new System.Drawing.Size(150, 44);
            this.btnNav_ManageUser.TabIndex = 3;
            // 
            // picNav_User
            // 
            this.picNav_User.EditValue = global::Autosoft_Licensing.Properties.Resources.User;
            this.picNav_User.Location = new System.Drawing.Point(63, 4);
            this.picNav_User.Name = "picNav_User";
            this.picNav_User.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_User.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_User.Size = new System.Drawing.Size(24, 24);
            this.picNav_User.TabIndex = 0;
            // 
            // lblNav_User
            // 
            this.lblNav_User.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_User.Appearance.Options.UseFont = true;
            this.lblNav_User.Appearance.Options.UseTextOptions = true;
            this.lblNav_User.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNav_User.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_User.Location = new System.Drawing.Point(0, 28);
            this.lblNav_User.Name = "lblNav_User";
            this.lblNav_User.Size = new System.Drawing.Size(150, 16);
            this.lblNav_User.TabIndex = 1;
            this.lblNav_User.Text = "Manage User";
            // 
            // btnNav_GeneralSetting
            // 
            this.btnNav_GeneralSetting.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_GeneralSetting.Appearance.BorderColor = System.Drawing.Color.Transparent;
            this.btnNav_GeneralSetting.Appearance.Options.UseBackColor = true;
            this.btnNav_GeneralSetting.Appearance.Options.UseBorderColor = true;
            this.btnNav_GeneralSetting.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_GeneralSetting.Controls.Add(this.picNav_Setting);
            this.btnNav_GeneralSetting.Controls.Add(this.lblNav_Setting);
            this.btnNav_GeneralSetting.Location = new System.Drawing.Point(686, 6);
            this.btnNav_GeneralSetting.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNav_GeneralSetting.Name = "btnNav_GeneralSetting";
            this.btnNav_GeneralSetting.Size = new System.Drawing.Size(160, 44);
            this.btnNav_GeneralSetting.TabIndex = 4;
            // 
            // picNav_Setting
            // 
            this.picNav_Setting.EditValue = global::Autosoft_Licensing.Properties.Resources.Setting;
            this.picNav_Setting.Location = new System.Drawing.Point(68, 4);
            this.picNav_Setting.Name = "picNav_Setting";
            this.picNav_Setting.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Setting.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Setting.Size = new System.Drawing.Size(24, 24);
            this.picNav_Setting.TabIndex = 0;
            // 
            // lblNav_Setting
            // 
            this.lblNav_Setting.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_Setting.Appearance.Options.UseFont = true;
            this.lblNav_Setting.Appearance.Options.UseTextOptions = true;
            this.lblNav_Setting.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNav_Setting.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_Setting.Location = new System.Drawing.Point(0, 28);
            this.lblNav_Setting.Name = "lblNav_Setting";
            this.lblNav_Setting.Size = new System.Drawing.Size(160, 16);
            this.lblNav_Setting.TabIndex = 1;
            this.lblNav_Setting.Text = "General Setting";
            // 
            // btnNav_Logout
            // 
            this.btnNav_Logout.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_Logout.Appearance.Options.UseBackColor = true;
            this.btnNav_Logout.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_Logout.Controls.Add(this.picNav_Logout);
            this.btnNav_Logout.Controls.Add(this.lblNav_Logout);
            this.btnNav_Logout.Location = new System.Drawing.Point(892, 6);
            this.btnNav_Logout.Name = "btnNav_Logout";
            this.btnNav_Logout.Size = new System.Drawing.Size(120, 44);
            this.btnNav_Logout.TabIndex = 5;
            // 
            // picNav_Logout
            // 
            this.picNav_Logout.EditValue = global::Autosoft_Licensing.Properties.Resources.Exit;
            this.picNav_Logout.Location = new System.Drawing.Point(86, 8);
            this.picNav_Logout.Name = "picNav_Logout";
            this.picNav_Logout.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Logout.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Logout.Size = new System.Drawing.Size(24, 24);
            this.picNav_Logout.TabIndex = 1;
            // 
            // lblNav_Logout
            // 
            this.lblNav_Logout.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNav_Logout.Appearance.Options.UseFont = true;
            this.lblNav_Logout.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNav_Logout.Location = new System.Drawing.Point(8, 14);
            this.lblNav_Logout.Name = "lblNav_Logout";
            this.lblNav_Logout.Size = new System.Drawing.Size(80, 16);
            this.lblNav_Logout.TabIndex = 0;
            this.lblNav_Logout.Text = "Logout";
            // 
            // grpFiltering
            // 
            this.grpFiltering.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFiltering.Appearance.BackColor = System.Drawing.Color.White;
            this.grpFiltering.Appearance.Options.UseBackColor = true;
            this.grpFiltering.Controls.Add(this.lblCompanyName);
            this.grpFiltering.Controls.Add(this.cmbCompanyName);
            this.grpFiltering.Controls.Add(this.chkShowExpired);
            this.grpFiltering.Controls.Add(this.lblProductCode);
            this.grpFiltering.Controls.Add(this.cmbProductCode);
            this.grpFiltering.Controls.Add(this.lblCountdownDays);
            this.grpFiltering.Controls.Add(this.numCountdownDays);
            this.grpFiltering.Controls.Add(this.lblLicenseType);
            this.grpFiltering.Controls.Add(this.cmbLicenseType);
            this.grpFiltering.Controls.Add(this.lblIssueDate);
            this.grpFiltering.Controls.Add(this.cmbIssueDateMode);
            this.grpFiltering.Controls.Add(this.dtIssueDateSingle);
            this.grpFiltering.Controls.Add(this.lblIssueDateFrom);
            this.grpFiltering.Controls.Add(this.dtIssueDateFrom);
            this.grpFiltering.Controls.Add(this.lblIssueDateTo);
            this.grpFiltering.Controls.Add(this.dtIssueDateTo);
            this.grpFiltering.Controls.Add(this.lblExpiryDate);
            this.grpFiltering.Controls.Add(this.cmbExpiryDateMode);
            this.grpFiltering.Controls.Add(this.dtExpiryDateSingle);
            this.grpFiltering.Controls.Add(this.lblExpiryDateFrom);
            this.grpFiltering.Controls.Add(this.dtExpiryDateFrom);
            this.grpFiltering.Controls.Add(this.lblExpiryDateTo);
            this.grpFiltering.Controls.Add(this.dtExpiryDateTo);
            this.grpFiltering.Location = new System.Drawing.Point(12, 124);
            this.grpFiltering.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.grpFiltering.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grpFiltering.Name = "grpFiltering";
            this.grpFiltering.Size = new System.Drawing.Size(955, 165);
            this.grpFiltering.TabIndex = 2;
            this.grpFiltering.Text = "Filtering";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Location = new System.Drawing.Point(12, 36);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(82, 13);
            this.lblCompanyName.TabIndex = 0;
            this.lblCompanyName.Text = "Company Name :";
            // 
            // cmbCompanyName
            // 
            this.cmbCompanyName.Location = new System.Drawing.Point(120, 34);
            this.cmbCompanyName.Name = "cmbCompanyName";
            this.cmbCompanyName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbCompanyName.Size = new System.Drawing.Size(200, 22);
            this.cmbCompanyName.TabIndex = 1;
            // 
            // chkShowExpired
            // 
            this.chkShowExpired.Location = new System.Drawing.Point(380, 32);
            this.chkShowExpired.Name = "chkShowExpired";
            this.chkShowExpired.Properties.Caption = "Show expired license";
            this.chkShowExpired.Size = new System.Drawing.Size(160, 17);
            this.chkShowExpired.TabIndex = 2;
            // 
            // lblProductCode
            // 
            this.lblProductCode.Location = new System.Drawing.Point(12, 70);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(72, 13);
            this.lblProductCode.TabIndex = 3;
            this.lblProductCode.Text = "Product Code :";
            // 
            // cmbProductCode
            // 
            this.cmbProductCode.Location = new System.Drawing.Point(120, 68);
            this.cmbProductCode.Name = "cmbProductCode";
            this.cmbProductCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbProductCode.Size = new System.Drawing.Size(200, 22);
            this.cmbProductCode.TabIndex = 4;
            // 
            // lblCountdownDays
            // 
            this.lblCountdownDays.Location = new System.Drawing.Point(380, 70);
            this.lblCountdownDays.Name = "lblCountdownDays";
            this.lblCountdownDays.Size = new System.Drawing.Size(96, 13);
            this.lblCountdownDays.TabIndex = 5;
            this.lblCountdownDays.Text = "Countdown (days) :";
            // 
            // numCountdownDays
            // 
            this.numCountdownDays.EditValue = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numCountdownDays.Location = new System.Drawing.Point(500, 68);
            this.numCountdownDays.Name = "numCountdownDays";
            this.numCountdownDays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numCountdownDays.Properties.IsFloatValue = false;
            this.numCountdownDays.Properties.MaskSettings.Set("mask", "N00");
            this.numCountdownDays.Size = new System.Drawing.Size(80, 22);
            this.numCountdownDays.TabIndex = 6;
            // 
            // lblLicenseType
            // 
            this.lblLicenseType.Location = new System.Drawing.Point(12, 104);
            this.lblLicenseType.Name = "lblLicenseType";
            this.lblLicenseType.Size = new System.Drawing.Size(69, 13);
            this.lblLicenseType.TabIndex = 7;
            this.lblLicenseType.Text = "License Type :";
            // 
            // cmbLicenseType
            // 
            this.cmbLicenseType.Location = new System.Drawing.Point(120, 102);
            this.cmbLicenseType.Name = "cmbLicenseType";
            this.cmbLicenseType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbLicenseType.Size = new System.Drawing.Size(200, 22);
            this.cmbLicenseType.TabIndex = 8;
            // 
            // lblIssueDate
            // 
            this.lblIssueDate.Location = new System.Drawing.Point(340, 104);
            this.lblIssueDate.Name = "lblIssueDate";
            this.lblIssueDate.Size = new System.Drawing.Size(59, 13);
            this.lblIssueDate.TabIndex = 9;
            this.lblIssueDate.Text = "Issue Date :";
            // 
            // cmbIssueDateMode
            // 
            this.cmbIssueDateMode.Location = new System.Drawing.Point(420, 102);
            this.cmbIssueDateMode.Name = "cmbIssueDateMode";
            this.cmbIssueDateMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbIssueDateMode.Size = new System.Drawing.Size(100, 22);
            this.cmbIssueDateMode.TabIndex = 10;
            // 
            // dtIssueDateSingle
            // 
            this.dtIssueDateSingle.EditValue = new System.DateTime(2025, 12, 3, 9, 26, 31, 366);
            this.dtIssueDateSingle.Location = new System.Drawing.Point(530, 102);
            this.dtIssueDateSingle.Name = "dtIssueDateSingle";
            this.dtIssueDateSingle.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDateSingle.Size = new System.Drawing.Size(120, 22);
            this.dtIssueDateSingle.TabIndex = 11;
            // 
            // lblIssueDateFrom
            // 
            this.lblIssueDateFrom.Location = new System.Drawing.Point(530, 104);
            this.lblIssueDateFrom.Name = "lblIssueDateFrom";
            this.lblIssueDateFrom.Size = new System.Drawing.Size(31, 13);
            this.lblIssueDateFrom.TabIndex = 12;
            this.lblIssueDateFrom.Text = "From :";
            this.lblIssueDateFrom.Visible = false;
            // 
            // dtIssueDateFrom
            // 
            this.dtIssueDateFrom.EditValue = new System.DateTime(2025, 11, 3, 9, 26, 31, 382);
            this.dtIssueDateFrom.Location = new System.Drawing.Point(570, 102);
            this.dtIssueDateFrom.Name = "dtIssueDateFrom";
            this.dtIssueDateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDateFrom.Size = new System.Drawing.Size(100, 22);
            this.dtIssueDateFrom.TabIndex = 13;
            this.dtIssueDateFrom.Visible = false;
            // 
            // lblIssueDateTo
            // 
            this.lblIssueDateTo.Location = new System.Drawing.Point(680, 104);
            this.lblIssueDateTo.Name = "lblIssueDateTo";
            this.lblIssueDateTo.Size = new System.Drawing.Size(19, 13);
            this.lblIssueDateTo.TabIndex = 14;
            this.lblIssueDateTo.Text = "To :";
            this.lblIssueDateTo.Visible = false;
            // 
            // dtIssueDateTo
            // 
            this.dtIssueDateTo.EditValue = new System.DateTime(2025, 12, 3, 9, 26, 31, 397);
            this.dtIssueDateTo.Location = new System.Drawing.Point(710, 102);
            this.dtIssueDateTo.Name = "dtIssueDateTo";
            this.dtIssueDateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDateTo.Size = new System.Drawing.Size(100, 22);
            this.dtIssueDateTo.TabIndex = 15;
            this.dtIssueDateTo.Visible = false;
            // 
            // lblExpiryDate
            // 
            this.lblExpiryDate.Location = new System.Drawing.Point(12, 138);
            this.lblExpiryDate.Name = "lblExpiryDate";
            this.lblExpiryDate.Size = new System.Drawing.Size(63, 13);
            this.lblExpiryDate.TabIndex = 16;
            this.lblExpiryDate.Text = "Expiry Date :";
            // 
            // cmbExpiryDateMode
            // 
            this.cmbExpiryDateMode.Location = new System.Drawing.Point(120, 136);
            this.cmbExpiryDateMode.Name = "cmbExpiryDateMode";
            this.cmbExpiryDateMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbExpiryDateMode.Size = new System.Drawing.Size(100, 22);
            this.cmbExpiryDateMode.TabIndex = 17;
            // 
            // dtExpiryDateSingle
            // 
            this.dtExpiryDateSingle.EditValue = new System.DateTime(2025, 12, 3, 9, 26, 31, 429);
            this.dtExpiryDateSingle.Location = new System.Drawing.Point(230, 136);
            this.dtExpiryDateSingle.Name = "dtExpiryDateSingle";
            this.dtExpiryDateSingle.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiryDateSingle.Size = new System.Drawing.Size(120, 22);
            this.dtExpiryDateSingle.TabIndex = 18;
            // 
            // lblExpiryDateFrom
            // 
            this.lblExpiryDateFrom.Location = new System.Drawing.Point(230, 138);
            this.lblExpiryDateFrom.Name = "lblExpiryDateFrom";
            this.lblExpiryDateFrom.Size = new System.Drawing.Size(31, 13);
            this.lblExpiryDateFrom.TabIndex = 19;
            this.lblExpiryDateFrom.Text = "From :";
            this.lblExpiryDateFrom.Visible = false;
            // 
            // dtExpiryDateFrom
            // 
            this.dtExpiryDateFrom.EditValue = new System.DateTime(2025, 11, 3, 9, 26, 31, 429);
            this.dtExpiryDateFrom.Location = new System.Drawing.Point(270, 136);
            this.dtExpiryDateFrom.Name = "dtExpiryDateFrom";
            this.dtExpiryDateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiryDateFrom.Size = new System.Drawing.Size(100, 22);
            this.dtExpiryDateFrom.TabIndex = 20;
            this.dtExpiryDateFrom.Visible = false;
            // 
            // lblExpiryDateTo
            // 
            this.lblExpiryDateTo.Location = new System.Drawing.Point(380, 138);
            this.lblExpiryDateTo.Name = "lblExpiryDateTo";
            this.lblExpiryDateTo.Size = new System.Drawing.Size(19, 13);
            this.lblExpiryDateTo.TabIndex = 21;
            this.lblExpiryDateTo.Text = "To :";
            this.lblExpiryDateTo.Visible = false;
            // 
            // dtExpiryDateTo
            // 
            this.dtExpiryDateTo.EditValue = new System.DateTime(2025, 12, 3, 9, 26, 31, 444);
            this.dtExpiryDateTo.Location = new System.Drawing.Point(410, 136);
            this.dtExpiryDateTo.Name = "dtExpiryDateTo";
            this.dtExpiryDateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiryDateTo.Size = new System.Drawing.Size(100, 22);
            this.dtExpiryDateTo.TabIndex = 22;
            this.dtExpiryDateTo.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Location = new System.Drawing.Point(12, 299);
            this.btnRefresh.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 36);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            // 
            // grdLicenses
            // 
            this.grdLicenses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdLicenses.Location = new System.Drawing.Point(12, 345);
            this.grdLicenses.MainView = this.viewLicenses;
            this.grdLicenses.Name = "grdLicenses";
            this.grdLicenses.Size = new System.Drawing.Size(955, 355);
            this.grdLicenses.TabIndex = 4;
            this.grdLicenses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewLicenses});
            // 
            // viewLicenses
            // 
            this.viewLicenses.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCompanyName,
            this.colProductCode,
            this.colProductName,
            this.colLicenseType,
            this.colIssueDate,
            this.colExpiryDate,
            this.colStatus,
            this.colCountdown});
            this.viewLicenses.GridControl = this.grdLicenses;
            this.viewLicenses.Name = "viewLicenses";
            this.viewLicenses.OptionsBehavior.Editable = false;
            this.viewLicenses.OptionsView.ShowFooter = true;
            this.viewLicenses.OptionsView.ShowGroupPanel = false;
            this.viewLicenses.OptionsView.ShowIndicator = false;
            // 
            // colCompanyName
            // 
            this.colCompanyName.Caption = "Company Name";
            this.colCompanyName.FieldName = "CompanyName";
            this.colCompanyName.Name = "colCompanyName";
            this.colCompanyName.Visible = true;
            this.colCompanyName.VisibleIndex = 0;
            this.colCompanyName.Width = 180;
            // 
            // colProductCode
            // 
            this.colProductCode.Caption = "Product Code";
            this.colProductCode.FieldName = "ProductCode";
            this.colProductCode.Name = "colProductCode";
            this.colProductCode.Visible = true;
            this.colProductCode.VisibleIndex = 1;
            this.colProductCode.Width = 100;
            // 
            // colProductName
            // 
            this.colProductName.Caption = "Product Name";
            this.colProductName.FieldName = "ProductName";
            this.colProductName.Name = "colProductName";
            this.colProductName.Visible = true;
            this.colProductName.VisibleIndex = 2;
            this.colProductName.Width = 200;
            // 
            // colLicenseType
            // 
            this.colLicenseType.Caption = "License Type";
            this.colLicenseType.FieldName = "LicenseType";
            this.colLicenseType.Name = "colLicenseType";
            this.colLicenseType.Visible = true;
            this.colLicenseType.VisibleIndex = 3;
            this.colLicenseType.Width = 120;
            // 
            // colIssueDate
            // 
            this.colIssueDate.Caption = "Issue Date";
            this.colIssueDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.colIssueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colIssueDate.FieldName = "IssueDate";
            this.colIssueDate.Name = "colIssueDate";
            this.colIssueDate.Visible = true;
            this.colIssueDate.VisibleIndex = 4;
            this.colIssueDate.Width = 110;
            // 
            // colExpiryDate
            // 
            this.colExpiryDate.Caption = "Expiry Date";
            this.colExpiryDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.colExpiryDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colExpiryDate.FieldName = "ExpiryDate";
            this.colExpiryDate.Name = "colExpiryDate";
            this.colExpiryDate.Visible = true;
            this.colExpiryDate.VisibleIndex = 5;
            this.colExpiryDate.Width = 110;
            // 
            // colStatus
            // 
            this.colStatus.Caption = "Status";
            this.colStatus.FieldName = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 6;
            this.colStatus.Width = 100;
            // 
            // colCountdown
            // 
            this.colCountdown.Caption = "Countdown";
            this.colCountdown.FieldName = "Countdown";
            this.colCountdown.Name = "colCountdown";
            this.colCountdown.Visible = true;
            this.colCountdown.VisibleIndex = 7;
            this.colCountdown.Width = 100;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCreate.Appearance.Options.UseFont = true;
            this.btnCreate.Location = new System.Drawing.Point(404, 710);
            this.btnCreate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(120, 36);
            this.btnCreate.TabIndex = 5;
            this.btnCreate.Text = "Create";
            // 
            // btnView
            // 
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnView.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnView.Appearance.Options.UseFont = true;
            this.btnView.Location = new System.Drawing.Point(538, 710);
            this.btnView.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(120, 36);
            this.btnView.TabIndex = 6;
            this.btnView.Text = "View";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(672, 710);
            this.btnEdit.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 36);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Text = "Edit";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(806, 710);
            this.btnDelete.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 36);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            // 
            // LicenseRecordsPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.grdLicenses);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.grpFiltering);
            this.Controls.Add(this.navPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "LicenseRecordsPage";
            // INCREASED HEIGHT to 760 to ensure buttons (at Y=710) are visible
            this.Size = new System.Drawing.Size(1000, 760);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navPanel)).EndInit();
            this.navPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GenerateLicense)).EndInit();
            this.btnNav_GenerateLicense.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Generate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_LicenseRecords)).EndInit();
            this.btnNav_LicenseRecords.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Records.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlineRecords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).EndInit();
            this.btnNav_ManageProduct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).EndInit();
            this.btnNav_ManageUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GeneralSetting)).EndInit();
            this.btnNav_GeneralSetting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Setting.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_Logout)).EndInit();
            this.btnNav_Logout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFiltering)).EndInit();
            this.grpFiltering.ResumeLayout(false);
            this.grpFiltering.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowExpired.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProductCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountdownDays.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLicenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIssueDateMode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateSingle.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateSingle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExpiryDateMode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateSingle.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateSingle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiryDateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLicenses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLicenses)).EndInit();
            this.ResumeLayout(false);

        }

    }
}