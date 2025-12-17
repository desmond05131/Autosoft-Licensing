using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms; // DockStyle/AnchorStyles
using DevExpress.Utils;

namespace Autosoft_Licensing.UI.Pages
{
    partial class ManageUserPage
    {
        private IContainer components = null;

        // Header & nav
        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;
        private PanelControl navPanel;
        private PanelControl btnNav_GenerateLicense;
        private PictureEdit picNav_Generate;
        private LabelControl lblNav_Generate;
        private PanelControl underlineUsers; // active underline (under Manage User)
        private PanelControl btnNav_LicenseRecords;
        private PictureEdit picNav_Records;
        private LabelControl lblNav_Records;
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

        // Action bar
        private PanelControl actionPanel;
        private SimpleButton btnCreate;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;

        // Grid
        private GridControl grdUsers;
        private GridView viewUsers;
        private DevExpress.XtraGrid.Columns.GridColumn colUsername;
        private DevExpress.XtraGrid.Columns.GridColumn colIsActive;
        private RepositoryItemCheckEdit repositoryCheckActive;

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
            this.btnNav_ManageProduct = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Product = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Product = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_ManageUser = new DevExpress.XtraEditors.PanelControl();
            this.picNav_User = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_User = new DevExpress.XtraEditors.LabelControl();
            this.underlineUsers = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_GeneralSetting = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Setting = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Setting = new DevExpress.XtraEditors.LabelControl();
            this.btnNav_Logout = new DevExpress.XtraEditors.PanelControl();
            this.picNav_Logout = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_Logout = new DevExpress.XtraEditors.LabelControl();
            this.actionPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.grdUsers = new DevExpress.XtraGrid.GridControl();
            this.viewUsers = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colUsername = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryCheckActive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
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
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).BeginInit();
            this.btnNav_ManageProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).BeginInit();
            this.btnNav_ManageUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlineUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GeneralSetting)).BeginInit();
            this.btnNav_GeneralSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Setting.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_Logout)).BeginInit();
            this.btnNav_Logout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionPanel)).BeginInit();
            this.actionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckActive)).BeginInit();
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
            this.btnNav_ManageUser.Controls.Add(this.underlineUsers);
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
            // underlineUsers
            // 
            this.underlineUsers.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(153)))), ((int)(((byte)(51)))));
            this.underlineUsers.Appearance.Options.UseBackColor = true;
            this.underlineUsers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.underlineUsers.Location = new System.Drawing.Point(20, 34);
            this.underlineUsers.Name = "underlineUsers";
            this.underlineUsers.Size = new System.Drawing.Size(140, 4);
            this.underlineUsers.TabIndex = 2;
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
            // actionPanel
            // 
            this.actionPanel.Appearance.BackColor = System.Drawing.Color.White;
            this.actionPanel.Appearance.Options.UseBackColor = true;
            this.actionPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.actionPanel.Controls.Add(this.btnCreate);
            this.actionPanel.Controls.Add(this.btnEdit);
            this.actionPanel.Controls.Add(this.btnDelete);
            this.actionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.actionPanel.Location = new System.Drawing.Point(0, 112);
            this.actionPanel.Name = "actionPanel";
            this.actionPanel.Size = new System.Drawing.Size(999, 44);
            this.actionPanel.TabIndex = 2;
            // 
            // btnCreate
            // 
            this.btnCreate.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnCreate.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCreate.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Appearance.Options.UseBackColor = true;
            this.btnCreate.Appearance.Options.UseFont = true;
            this.btnCreate.Appearance.Options.UseForeColor = true;
            this.btnCreate.Location = new System.Drawing.Point(12, 8);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(80, 30);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Create";
            // 
            // btnEdit
            // 
            this.btnEdit.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEdit.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Appearance.Options.UseBackColor = true;
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Appearance.Options.UseForeColor = true;
            this.btnEdit.Location = new System.Drawing.Point(100, 8);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(80, 30);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            // 
            // btnDelete
            // 
            this.btnDelete.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Location = new System.Drawing.Point(188, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 30);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            // 
            // grdUsers
            // 
            this.grdUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdUsers.Location = new System.Drawing.Point(12, 168);
            this.grdUsers.MainView = this.viewUsers;
            this.grdUsers.Name = "grdUsers";
            this.grdUsers.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryCheckActive});
            this.grdUsers.Size = new System.Drawing.Size(975, 485);
            this.grdUsers.TabIndex = 3;
            this.grdUsers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewUsers});
            // 
            // viewUsers
            // 
            this.viewUsers.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.viewUsers.Appearance.Row.Options.UseFont = true;
            this.viewUsers.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUsername,
            this.colIsActive});
            this.viewUsers.GridControl = this.grdUsers;
            this.viewUsers.Name = "viewUsers";
            this.viewUsers.OptionsBehavior.Editable = false;
            this.viewUsers.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewUsers.OptionsView.ColumnAutoWidth = false;
            this.viewUsers.OptionsView.ShowGroupPanel = false;
            this.viewUsers.OptionsView.ShowIndicator = false;
            // 
            // colUsername
            // 
            this.colUsername.Caption = "Username";
            this.colUsername.FieldName = "Username";
            this.colUsername.Name = "colUsername";
            this.colUsername.Visible = true;
            this.colUsername.VisibleIndex = 0;
            this.colUsername.Width = 300;
            // 
            // colIsActive
            // 
            this.colIsActive.AppearanceCell.Options.UseTextOptions = true;
            this.colIsActive.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colIsActive.AppearanceHeader.Options.UseTextOptions = true;
            this.colIsActive.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colIsActive.Caption = "Is active ?";
            this.colIsActive.ColumnEdit = this.repositoryCheckActive;
            this.colIsActive.FieldName = "IsActive";
            this.colIsActive.Name = "colIsActive";
            this.colIsActive.Visible = true;
            this.colIsActive.VisibleIndex = 1;
            this.colIsActive.Width = 100;
            // 
            // repositoryCheckActive
            // 
            this.repositoryCheckActive.AutoHeight = false;
            this.repositoryCheckActive.Name = "repositoryCheckActive";
            // 
            // ManageUserPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.grdUsers);
            this.Controls.Add(this.actionPanel);
            this.Controls.Add(this.navPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "ManageUserPage";
            this.Size = new System.Drawing.Size(999, 665);
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
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageProduct)).EndInit();
            this.btnNav_ManageProduct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Product.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).EndInit();
            this.btnNav_ManageUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlineUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_GeneralSetting)).EndInit();
            this.btnNav_GeneralSetting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Setting.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_Logout)).EndInit();
            this.btnNav_Logout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionPanel)).EndInit();
            this.actionPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckActive)).EndInit();
            this.ResumeLayout(false);

        }
    }
}