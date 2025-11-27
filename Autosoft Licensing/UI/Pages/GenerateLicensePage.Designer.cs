using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms; // Added: resolves DockStyle / AnchorStyles references
using DevExpress.Utils;

namespace Autosoft_Licensing.UI.Pages
{
    partial class GenerateLicensePage
    {
        private IContainer components = null;

        // Header & nav
        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;
        private SimpleButton btnLogout;
        private PanelControl navPanel;
        private PanelControl btnNav_GenerateLicense;
        private PanelControl btnNav_LicenseRecords;
        private PanelControl btnNav_ManageProduct;
        private PanelControl btnNav_ManageUser;
        private PictureEdit picNav_Generate;
        private LabelControl lblNav_Generate;
        private PictureEdit picNav_Records;
        private LabelControl lblNav_Records;
        private PictureEdit picNav_Product;
        private LabelControl lblNav_Product;
        private PictureEdit picNav_User;
        private LabelControl lblNav_User;
        private PanelControl pnlNavLogout;        // replaced previous SimpleButton with a Panel containing text button + picture
        private SimpleButton btnNavLogoutText;
        private PictureEdit picNav_Logout;

        // Upload
        private SimpleButton btnUploadArl;

        // Info group
        private GroupControl grpInfo;
        private LabelControl lblCompanyName;
        private TextEdit txtCompanyName;
        private LabelControl lblProductId;
        private TextEdit txtProductId;
        private LabelControl lblProductName;
        private TextEdit txtProductName;

        // Types group
        private GroupControl grpTypes;
        private RadioGroup rgLicenseType;
        private LabelControl lblMonths;
        private SpinEdit numSubscriptionMonths;

        // Modules group
        private GroupControl grpModules;
        private GridControl grdModules;
        private GridView grdModulesView;
        private DevExpress.XtraGrid.Columns.GridColumn colEnabled;
        private RepositoryItemCheckEdit repositoryCheckEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colModuleName;

        // Dates & remark & key
        private LabelControl lblIssueDate;
        private DateEdit dtIssueDate;
        private LabelControl lblExpireDate;
        private DateEdit dtExpireDate;
        private LabelControl lblRemark;
        private MemoEdit memRemark;

        private LabelControl lblLicenseKey;
        private TextEdit txtLicenseKey;

        // Action buttons
        private SimpleButton btnGenerateKey;
        private SimpleButton btnPreview;
        private SimpleButton btnDownload;

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
            this.btnLogout = new DevExpress.XtraEditors.SimpleButton();
            this.navPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_GenerateLicense = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Generate = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Generate = new DevExpress.XtraEditors.LabelControl();
            this.underline = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_LicenseRecords = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Records = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Records = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_ManageProduct = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Product = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Product = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_ManageUser = new DevExpress.XtraEditors.PanelControl();
            this.picNav_User = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_User = new DevExpress.XtraEditors.LabelControl();
            this.pnlNavLogout = new DevExpress.XtraEditors.PanelControl();
            this.btnNavLogoutText = new DevExpress.XtraEditors.SimpleButton();
            this.picNav_Logout = new DevExpress.XtraEditors.PictureEdit();
            this.btnUploadArl = new DevExpress.XtraEditors.SimpleButton();
            this.grpInfo = new DevExpress.XtraEditors.GroupControl();
            this.lblCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.txtCompanyName = new DevExpress.XtraEditors.TextEdit();
            this.lblProductId = new DevExpress.XtraEditors.LabelControl();
            this.txtProductId = new DevExpress.XtraEditors.TextEdit();
            this.lblProductName = new DevExpress.XtraEditors.LabelControl();
            this.txtProductName = new DevExpress.XtraEditors.TextEdit();
            this.grpTypes = new DevExpress.XtraEditors.GroupControl();
            this.rgLicenseType = new DevExpress.XtraEditors.RadioGroup();
            this.lblMonths = new DevExpress.XtraEditors.LabelControl();
            this.numSubscriptionMonths = new DevExpress.XtraEditors.SpinEdit();
            this.grpModules = new DevExpress.XtraEditors.GroupControl();
            this.grdModules = new DevExpress.XtraGrid.GridControl();
            this.grdModulesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colEnabled = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colModuleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblIssueDate = new DevExpress.XtraEditors.LabelControl();
            this.dtIssueDate = new DevExpress.XtraEditors.DateEdit();
            this.lblExpireDate = new DevExpress.XtraEditors.LabelControl();
            this.dtExpireDate = new DevExpress.XtraEditors.DateEdit();
            this.lblRemark = new DevExpress.XtraEditors.LabelControl();
            this.memRemark = new DevExpress.XtraEditors.MemoEdit();
            this.lblLicenseKey = new DevExpress.XtraEditors.LabelControl();
            this.txtLicenseKey = new DevExpress.XtraEditors.TextEdit();
            this.btnGenerateKey = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownload = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navPanel)).BeginInit();
            this.navPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GenerateLicense)).BeginInit();
            this.btnNav_GenerateLicense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Generate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.underline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_LicenseRecords)).BeginInit();
            this.btnNav_LicenseRecords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Records.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).BeginInit();
            this.btnNav_ManageProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).BeginInit();
            this.btnNav_ManageUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlNavLogout)).BeginInit();
            this.pnlNavLogout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInfo)).BeginInit();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTypes)).BeginInit();
            this.grpTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgLicenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubscriptionMonths.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpModules)).BeginInit();
            this.grpModules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModulesView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(211)))));
            this.headerPanel.Appearance.Options.UseBackColor = true;
            this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Controls.Add(this.btnLogout);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.headerPanel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(960, 60);
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
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnLogout.Appearance.BorderColor = System.Drawing.Color.Gray;
            this.btnLogout.Appearance.Options.UseBackColor = true;
            this.btnLogout.Appearance.Options.UseBorderColor = true;
            this.btnLogout.ImageOptions.Image = global::Autosoft_Licensing.Properties.Resources.Exit;
            this.btnLogout.Location = new System.Drawing.Point(1755, 13);
            this.btnLogout.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(110, 34);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "Logout";
            // 
            // navPanel
            // 
            this.navPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.navPanel.Controls.Add(this.btnNav_GenerateLicense);
            this.navPanel.Controls.Add(this.btnNav_LicenseRecords);
            this.navPanel.Controls.Add(this.btnNav_ManageProduct);
            this.navPanel.Controls.Add(this.btnNav_ManageUser);
            this.navPanel.Controls.Add(this.pnlNavLogout);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navPanel.Location = new System.Drawing.Point(0, 60);
            this.navPanel.Name = "navPanel";
            this.navPanel.Size = new System.Drawing.Size(960, 52);
            this.navPanel.TabIndex = 1;
            // 
            // btnNav_GenerateLicense
            // 
            this.btnNav_GenerateLicense.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_GenerateLicense.Controls.Add(this.picNav_Generate);
            this.btnNav_GenerateLicense.Controls.Add(this.lblNav_Generate);
            this.btnNav_GenerateLicense.Controls.Add(this.underline);
            this.btnNav_GenerateLicense.Location = new System.Drawing.Point(12, 6);
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
            // underline
            // 
            this.underline.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(153)))), ((int)(((byte)(51)))));
            this.underline.Appearance.Options.UseBackColor = true;
            this.underline.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.underline.Location = new System.Drawing.Point(20, 34);
            this.underline.Name = "underline";
            this.underline.Size = new System.Drawing.Size(140, 4);
            this.underline.TabIndex = 2;
            // 
            // btnNav_LicenseRecords
            // 
            this.btnNav_LicenseRecords.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_LicenseRecords.Controls.Add(this.picNav_Records);
            this.btnNav_LicenseRecords.Controls.Add(this.lblNav_Records);
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
            // btnNav_ManageProduct
            // 
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
            // pnlNavLogout
            // 
            this.pnlNavLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNavLogout.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlNavLogout.Controls.Add(this.btnNavLogoutText);
            this.pnlNavLogout.Controls.Add(this.picNav_Logout);
            this.pnlNavLogout.Location = new System.Drawing.Point(848, 11);
            this.pnlNavLogout.Name = "pnlNavLogout";
            this.pnlNavLogout.Size = new System.Drawing.Size(110, 30);
            this.pnlNavLogout.TabIndex = 4;
            // 
            // btnNavLogoutText
            // 
            this.btnNavLogoutText.AllowFocus = false;
            this.btnNavLogoutText.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnNavLogoutText.Appearance.Options.UseBackColor = true;
            this.btnNavLogoutText.Location = new System.Drawing.Point(0, 0);
            this.btnNavLogoutText.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNavLogoutText.Name = "btnNavLogoutText";
            this.btnNavLogoutText.Size = new System.Drawing.Size(78, 30);
            this.btnNavLogoutText.TabIndex = 0;
            this.btnNavLogoutText.Text = "Logout";
            // 
            // picNav_Logout
            // 
            this.picNav_Logout.EditValue = global::Autosoft_Licensing.Properties.Resources.Exit;
            this.picNav_Logout.Location = new System.Drawing.Point(80, 0);
            this.picNav_Logout.Name = "picNav_Logout";
            this.picNav_Logout.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picNav_Logout.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picNav_Logout.Size = new System.Drawing.Size(30, 30);
            this.picNav_Logout.TabIndex = 1;
            // 
            // btnUploadArl
            // 
            this.btnUploadArl.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnUploadArl.Appearance.Options.UseBackColor = true;
            this.btnUploadArl.Location = new System.Drawing.Point(12, 124);
            this.btnUploadArl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnUploadArl.Name = "btnUploadArl";
            this.btnUploadArl.Size = new System.Drawing.Size(160, 36);
            this.btnUploadArl.TabIndex = 2;
            this.btnUploadArl.Text = "Upload License File";
            // 
            // grpInfo
            // 
            this.grpInfo.AppearanceCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.grpInfo.AppearanceCaption.Options.UseBackColor = true;
            this.grpInfo.Controls.Add(this.lblCompanyName);
            this.grpInfo.Controls.Add(this.txtCompanyName);
            this.grpInfo.Controls.Add(this.lblProductId);
            this.grpInfo.Controls.Add(this.txtProductId);
            this.grpInfo.Controls.Add(this.lblProductName);
            this.grpInfo.Controls.Add(this.txtProductName);
            this.grpInfo.Location = new System.Drawing.Point(12, 120);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(420, 160);
            this.grpInfo.TabIndex = 3;
            this.grpInfo.Text = "Info";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Location = new System.Drawing.Point(12, 36);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(82, 13);
            this.lblCompanyName.TabIndex = 0;
            this.lblCompanyName.Text = "Company Name :";
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(140, 34);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Properties.ReadOnly = true;
            this.txtCompanyName.Size = new System.Drawing.Size(260, 20);
            this.txtCompanyName.TabIndex = 1;
            // 
            // lblProductId
            // 
            this.lblProductId.Location = new System.Drawing.Point(12, 68);
            this.lblProductId.Name = "lblProductId";
            this.lblProductId.Size = new System.Drawing.Size(58, 13);
            this.lblProductId.TabIndex = 2;
            this.lblProductId.Text = "Product ID :";
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(140, 66);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.Properties.ReadOnly = true;
            this.txtProductId.Size = new System.Drawing.Size(120, 20);
            this.txtProductId.TabIndex = 3;
            // 
            // lblProductName
            // 
            this.lblProductName.Location = new System.Drawing.Point(12, 100);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(74, 13);
            this.lblProductName.TabIndex = 4;
            this.lblProductName.Text = "Product Name :";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(140, 98);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Properties.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(260, 20);
            this.txtProductName.TabIndex = 5;
            // 
            // grpTypes
            // 
            this.grpTypes.AppearanceCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.grpTypes.AppearanceCaption.Options.UseBackColor = true;
            this.grpTypes.Controls.Add(this.rgLicenseType);
            this.grpTypes.Controls.Add(this.lblMonths);
            this.grpTypes.Controls.Add(this.numSubscriptionMonths);
            this.grpTypes.Location = new System.Drawing.Point(444, 120);
            this.grpTypes.Name = "grpTypes";
            this.grpTypes.Size = new System.Drawing.Size(220, 160);
            this.grpTypes.TabIndex = 4;
            this.grpTypes.Text = "Types";
            // 
            // rgLicenseType
            // 
            this.rgLicenseType.Location = new System.Drawing.Point(10, 28);
            this.rgLicenseType.Name = "rgLicenseType";
            this.rgLicenseType.Size = new System.Drawing.Size(200, 90);
            this.rgLicenseType.TabIndex = 0;
            // 
            // lblMonths
            // 
            this.lblMonths.Location = new System.Drawing.Point(12, 128);
            this.lblMonths.Name = "lblMonths";
            this.lblMonths.Size = new System.Drawing.Size(42, 13);
            this.lblMonths.TabIndex = 1;
            this.lblMonths.Text = "Months :";
            // 
            // numSubscriptionMonths
            // 
            this.numSubscriptionMonths.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSubscriptionMonths.Location = new System.Drawing.Point(74, 126);
            this.numSubscriptionMonths.Name = "numSubscriptionMonths";
            this.numSubscriptionMonths.Properties.IsFloatValue = false;
            this.numSubscriptionMonths.Properties.MaskSettings.Set("mask", "N00");
            this.numSubscriptionMonths.Properties.MaxValue = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.numSubscriptionMonths.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSubscriptionMonths.Size = new System.Drawing.Size(80, 20);
            this.numSubscriptionMonths.TabIndex = 2;
            // 
            // grpModules
            // 
            this.grpModules.AppearanceCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.grpModules.AppearanceCaption.Options.UseBackColor = true;
            this.grpModules.Controls.Add(this.grdModules);
            this.grpModules.Location = new System.Drawing.Point(680, 120);
            this.grpModules.Name = "grpModules";
            this.grpModules.Size = new System.Drawing.Size(320, 260);
            this.grpModules.TabIndex = 5;
            this.grpModules.Text = "Modules";
            // 
            // grdModules
            // 
            this.grdModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdModules.Location = new System.Drawing.Point(8, 28);
            this.grdModules.MainView = this.grdModulesView;
            this.grdModules.Name = "grdModules";
            this.grdModules.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryCheckEdit});
            this.grdModules.Size = new System.Drawing.Size(304, 220);
            this.grdModules.TabIndex = 0;
            this.grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdModulesView});
            // 
            // grdModulesView
            // 
            this.grdModulesView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grdModulesView.Appearance.Row.Options.UseFont = true;
            this.grdModulesView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colEnabled,
            this.colModuleName});
            this.grdModulesView.GridControl = this.grdModules;
            this.grdModulesView.Name = "grdModulesView";
            this.grdModulesView.OptionsView.ColumnAutoWidth = false;
            this.grdModulesView.OptionsView.ShowIndicator = false;
            // 
            // colEnabled
            // 
            this.colEnabled.ColumnEdit = this.repositoryCheckEdit;
            this.colEnabled.FieldName = "Enabled";
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.Visible = true;
            this.colEnabled.VisibleIndex = 0;
            this.colEnabled.Width = 48;
            // 
            // repositoryCheckEdit
            // 
            this.repositoryCheckEdit.AutoHeight = false;
            this.repositoryCheckEdit.Name = "repositoryCheckEdit";
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
            // lblIssueDate
            // 
            this.lblIssueDate.Location = new System.Drawing.Point(12, 400);
            this.lblIssueDate.Name = "lblIssueDate";
            this.lblIssueDate.Size = new System.Drawing.Size(59, 13);
            this.lblIssueDate.TabIndex = 6;
            this.lblIssueDate.Text = "Issue Date :";
            // 
            // dtIssueDate
            // 
            this.dtIssueDate.EditValue = new System.DateTime(2025, 11, 26, 0, 0, 0, 0);
            this.dtIssueDate.Location = new System.Drawing.Point(100, 396);
            this.dtIssueDate.Name = "dtIssueDate";
            this.dtIssueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDate.Size = new System.Drawing.Size(150, 20);
            this.dtIssueDate.TabIndex = 7;
            // 
            // lblExpireDate
            // 
            this.lblExpireDate.Location = new System.Drawing.Point(280, 400);
            this.lblExpireDate.Name = "lblExpireDate";
            this.lblExpireDate.Size = new System.Drawing.Size(63, 13);
            this.lblExpireDate.TabIndex = 8;
            this.lblExpireDate.Text = "Expire Date :";
            // 
            // dtExpireDate
            // 
            this.dtExpireDate.EditValue = new System.DateTime(2025, 11, 26, 0, 0, 0, 0);
            this.dtExpireDate.Location = new System.Drawing.Point(360, 396);
            this.dtExpireDate.Name = "dtExpireDate";
            this.dtExpireDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpireDate.Size = new System.Drawing.Size(150, 20);
            this.dtExpireDate.TabIndex = 9;
            // 
            // lblRemark
            // 
            this.lblRemark.Location = new System.Drawing.Point(12, 436);
            this.lblRemark.Name = "lblRemark";
            this.lblRemark.Size = new System.Drawing.Size(43, 13);
            this.lblRemark.TabIndex = 10;
            this.lblRemark.Text = "Remark :";
            // 
            // memRemark
            // 
            this.memRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memRemark.Location = new System.Drawing.Point(100, 432);
            this.memRemark.Name = "memRemark";
            this.memRemark.Size = new System.Drawing.Size(1012, 80);
            this.memRemark.TabIndex = 11;
            // 
            // lblLicenseKey
            // 
            this.lblLicenseKey.Location = new System.Drawing.Point(12, 528);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(63, 13);
            this.lblLicenseKey.TabIndex = 12;
            this.lblLicenseKey.Text = "License Key :";
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseKey.Location = new System.Drawing.Point(100, 514);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Properties.ReadOnly = true;
            this.txtLicenseKey.Size = new System.Drawing.Size(672, 20);
            this.txtLicenseKey.TabIndex = 13;
            // 
            // btnGenerateKey
            // 
            this.btnGenerateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateKey.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnGenerateKey.Appearance.Options.UseBackColor = true;
            this.btnGenerateKey.Location = new System.Drawing.Point(792, 520);
            this.btnGenerateKey.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnGenerateKey.Name = "btnGenerateKey";
            this.btnGenerateKey.Size = new System.Drawing.Size(160, 30);
            this.btnGenerateKey.TabIndex = 14;
            this.btnGenerateKey.Text = "Generate License Key";
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnPreview.Appearance.Options.UseBackColor = true;
            this.btnPreview.Location = new System.Drawing.Point(964, 520);
            this.btnPreview.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(80, 30);
            this.btnPreview.TabIndex = 15;
            this.btnPreview.Text = "Preview";
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.btnDownload.Appearance.Options.UseBackColor = true;
            this.btnDownload.Location = new System.Drawing.Point(1060, 520);
            this.btnDownload.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(160, 30);
            this.btnDownload.TabIndex = 16;
            this.btnDownload.Text = "Download License";
            // 
            // GenerateLicensePage
            // 
            this.Controls.Add(this.navPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.btnUploadArl);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.grpTypes);
            this.Controls.Add(this.grpModules);
            this.Controls.Add(this.lblIssueDate);
            this.Controls.Add(this.dtIssueDate);
            this.Controls.Add(this.lblExpireDate);
            this.Controls.Add(this.dtExpireDate);
            this.Controls.Add(this.lblRemark);
            this.Controls.Add(this.memRemark);
            this.Controls.Add(this.lblLicenseKey);
            this.Controls.Add(this.txtLicenseKey);
            this.Controls.Add(this.btnGenerateKey);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnDownload);
            this.Name = "GenerateLicensePage";
            this.Size = new System.Drawing.Size(960, 617);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navPanel)).EndInit();
            this.navPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GenerateLicense)).EndInit();
            this.btnNav_GenerateLicense.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Generate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.underline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_LicenseRecords)).EndInit();
            this.btnNav_LicenseRecords.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Records.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).EndInit();
            this.btnNav_ManageProduct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).EndInit();
            this.btnNav_ManageUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlNavLogout)).EndInit();
            this.pnlNavLogout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpInfo)).EndInit();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTypes)).EndInit();
            this.grpTypes.ResumeLayout(false);
            this.grpTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgLicenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubscriptionMonths.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpModules)).EndInit();
            this.grpModules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModulesView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private PanelControl underline;
    }
}
