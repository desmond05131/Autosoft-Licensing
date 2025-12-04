using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;

namespace Autosoft_Licensing.UI.Pages
{
    partial class ManageProductPage
    {
        private IContainer components = null;

        // Header & nav (same pattern as other pages)
        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;

        private PanelControl navPanel;
        private PanelControl btnNav_GenerateLicense;
        private PictureEdit picNav_Generate;
        private LabelControl lblNav_Generate;

        private PanelControl btnNav_LicenseRecords;
        private PictureEdit picNav_Records;
        private LabelControl lblNav_Records;

        private PanelControl btnNav_ManageProduct;
        private PictureEdit picNav_Product;
        private LabelControl lblNav_Product;
        private PanelControl underlineProduct;

        private PanelControl btnNav_ManageUser;
        private PictureEdit picNav_User;
        private LabelControl lblNav_User;

        private PanelControl pnlNavLogout;
        private SimpleButton btnNavLogoutText;
        private PictureEdit picNav_Logout;

        // Action bar
        private SimpleButton btnCreate;
        private SimpleButton btnView;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;

        // Search (use ButtonEdit to get a magnifier icon button)
        private ButtonEdit txtSearch;

        // Grid
        private GridControl grdProducts;
        private GridView viewProducts;
        private GridColumn colProductId;
        private GridColumn colProductName;
        private GridColumn colCreatedBy;
        private GridColumn colDateCreated;
        private GridColumn colLastModified;

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
            this.underlineProduct = new DevExpress.XtraEditors.PanelControl();
            this.btnNav_ManageUser = new DevExpress.XtraEditors.PanelControl();
            this.picNav_User = new DevExpress.XtraEditors.PictureEdit();
            this.lblNav_User = new DevExpress.XtraEditors.LabelControl();
            this.pnlNavLogout = new DevExpress.XtraEditors.PanelControl();
            this.btnNavLogoutText = new DevExpress.XtraEditors.SimpleButton();
            this.picNav_Logout = new DevExpress.XtraEditors.PictureEdit();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.btnView = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearch = new DevExpress.XtraEditors.ButtonEdit();
            this.grdProducts = new DevExpress.XtraGrid.GridControl();
            this.viewProducts = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDateCreated = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLastModified = new DevExpress.XtraGrid.Columns.GridColumn();
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
            ((System.ComponentModel.ISupportInitialize)(this.underlineProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).BeginInit();
            this.btnNav_ManageUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlNavLogout)).BeginInit();
            this.pnlNavLogout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewProducts)).BeginInit();
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
            this.headerPanel.Size = new System.Drawing.Size(1089, 60);
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
            this.navPanel.Controls.Add(this.pnlNavLogout);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navPanel.Location = new System.Drawing.Point(0, 60);
            this.navPanel.Name = "navPanel";
            this.navPanel.Size = new System.Drawing.Size(1089, 52);
            this.navPanel.TabIndex = 1;
            // 
            // btnNav_GenerateLicense
            // 
            this.btnNav_GenerateLicense.Appearance.BackColor = System.Drawing.Color.White;
            this.btnNav_GenerateLicense.Appearance.Options.UseBackColor = true;
            this.btnNav_GenerateLicense.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnNav_GenerateLicense.Controls.Add(this.picNav_Generate);
            this.btnNav_GenerateLicense.Controls.Add(this.lblNav_Generate);
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
            this.btnNav_ManageProduct.Controls.Add(this.underlineProduct);
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
            // underlineProduct
            // 
            this.underlineProduct.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(153)))), ((int)(((byte)(51)))));
            this.underlineProduct.Appearance.Options.UseBackColor = true;
            this.underlineProduct.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.underlineProduct.Location = new System.Drawing.Point(10, 40);
            this.underlineProduct.Name = "underlineProduct";
            this.underlineProduct.Size = new System.Drawing.Size(130, 4);
            this.underlineProduct.TabIndex = 2;
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
            // pnlNavLogout
            // 
            this.pnlNavLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNavLogout.Appearance.BackColor = System.Drawing.Color.White;
            this.pnlNavLogout.Appearance.Options.UseBackColor = true;
            this.pnlNavLogout.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlNavLogout.Controls.Add(this.btnNavLogoutText);
            this.pnlNavLogout.Controls.Add(this.picNav_Logout);
            this.pnlNavLogout.Location = new System.Drawing.Point(959, 11);
            this.pnlNavLogout.Name = "pnlNavLogout";
            this.pnlNavLogout.Size = new System.Drawing.Size(110, 30);
            this.pnlNavLogout.TabIndex = 4;
            // 
            // btnNavLogoutText
            // 
            this.btnNavLogoutText.AllowFocus = false;
            this.btnNavLogoutText.Appearance.BackColor = System.Drawing.Color.White;
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
            // btnCreate
            // 
            this.btnCreate.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnCreate.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Appearance.Options.UseBackColor = true;
            this.btnCreate.Appearance.Options.UseForeColor = true;
            this.btnCreate.Location = new System.Drawing.Point(12, 124);
            this.btnCreate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(120, 36);
            this.btnCreate.TabIndex = 5;
            this.btnCreate.Text = "Create";
            // 
            // btnView
            // 
            this.btnView.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnView.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnView.Appearance.Options.UseBackColor = true;
            this.btnView.Appearance.Options.UseForeColor = true;
            this.btnView.Location = new System.Drawing.Point(146, 124);
            this.btnView.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(120, 36);
            this.btnView.TabIndex = 6;
            this.btnView.Text = "View";
            // 
            // btnEdit
            // 
            this.btnEdit.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnEdit.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Appearance.Options.UseBackColor = true;
            this.btnEdit.Appearance.Options.UseForeColor = true;
            this.btnEdit.Location = new System.Drawing.Point(280, 124);
            this.btnEdit.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 36);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Text = "Edit";
            // 
            // btnDelete
            // 
            this.btnDelete.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnDelete.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Location = new System.Drawing.Point(414, 124);
            this.btnDelete.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 36);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(789, 127);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)});
            this.txtSearch.Properties.NullValuePrompt = "Search products...";
            this.txtSearch.Size = new System.Drawing.Size(288, 20);
            this.txtSearch.TabIndex = 9;
            // 
            // grdProducts
            // 
            this.grdProducts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdProducts.Location = new System.Drawing.Point(12, 176);
            this.grdProducts.MainView = this.viewProducts;
            this.grdProducts.Name = "grdProducts";
            this.grdProducts.Size = new System.Drawing.Size(1065, 441);
            this.grdProducts.TabIndex = 10;
            this.grdProducts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewProducts});
            // 
            // viewProducts
            // 
            this.viewProducts.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colProductId,
            this.colProductName,
            this.colCreatedBy,
            this.colDateCreated,
            this.colLastModified});
            this.viewProducts.GridControl = this.grdProducts;
            this.viewProducts.Name = "viewProducts";
            this.viewProducts.OptionsBehavior.Editable = false;
            this.viewProducts.OptionsView.ShowGroupPanel = false;
            this.viewProducts.OptionsView.ShowIndicator = false;
            // 
            // colProductId
            // 
            this.colProductId.Caption = "Product ID";
            this.colProductId.FieldName = "ProductID";
            this.colProductId.Name = "colProductId";
            this.colProductId.Visible = true;
            this.colProductId.VisibleIndex = 0;
            this.colProductId.Width = 160;
            // 
            // colProductName
            // 
            this.colProductName.Caption = "Product Name";
            this.colProductName.FieldName = "ProductName";
            this.colProductName.Name = "colProductName";
            this.colProductName.Visible = true;
            this.colProductName.VisibleIndex = 1;
            this.colProductName.Width = 260;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.Caption = "Created By";
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.Visible = true;
            this.colCreatedBy.VisibleIndex = 2;
            this.colCreatedBy.Width = 140;
            // 
            // colDateCreated
            // 
            this.colDateCreated.Caption = "Date Created";
            this.colDateCreated.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.colDateCreated.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDateCreated.FieldName = "DateCreated";
            this.colDateCreated.Name = "colDateCreated";
            this.colDateCreated.Visible = true;
            this.colDateCreated.VisibleIndex = 3;
            this.colDateCreated.Width = 140;
            // 
            // colLastModified
            // 
            this.colLastModified.Caption = "Last Modified";
            this.colLastModified.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.colLastModified.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colLastModified.FieldName = "LastModified";
            this.colLastModified.Name = "colLastModified";
            this.colLastModified.Visible = true;
            this.colLastModified.VisibleIndex = 4;
            this.colLastModified.Width = 140;
            // 
            // ManageProductPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.grdProducts);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.navPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "ManageProductPage";
            this.Size = new System.Drawing.Size(1089, 629);
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
            ((System.ComponentModel.ISupportInitialize)(this.underlineProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNav_ManageUser)).EndInit();
            this.btnNav_ManageUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_User.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlNavLogout)).EndInit();
            this.pnlNavLogout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNav_Logout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewProducts)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
