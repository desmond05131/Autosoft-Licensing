using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Autosoft_Licensing.UI.Pages
{
    partial class ProductDetailsPage
    {
        private IContainer components;
        private Panel pnlHeader;
        private Label lblHeaderTitle;

        private LabelControl lblProductId;
        private TextEdit txtProductId;

        private LabelControl lblCreatedBy;
        private TextEdit txtCreatedBy;

        private LabelControl lblLastModified;
        private TextEdit txtLastModified;

        private LabelControl lblProductName;
        private TextEdit txtProductName;

        private LabelControl lblDateCreated;
        private TextEdit txtDateCreated;

        private LabelControl lblModulesTitle;
        private SimpleButton btnAdd;
        private SimpleButton btnMinus;

        private GridControl grdModules;
        private GridView viewModules;
        private GridColumn colModuleName;
        private GridColumn colDescription;

        private LabelControl lblDescription;
        private MemoEdit memDescription;

        private LabelControl lblReleaseNotes;
        private MemoEdit memReleaseNotes;

        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblProductId = new DevExpress.XtraEditors.LabelControl();
            this.txtProductId = new DevExpress.XtraEditors.TextEdit();
            this.lblCreatedBy = new DevExpress.XtraEditors.LabelControl();
            this.txtCreatedBy = new DevExpress.XtraEditors.TextEdit();
            this.lblLastModified = new DevExpress.XtraEditors.LabelControl();
            this.txtLastModified = new DevExpress.XtraEditors.TextEdit();
            this.lblProductName = new DevExpress.XtraEditors.LabelControl();
            this.txtProductName = new DevExpress.XtraEditors.TextEdit();
            this.lblDateCreated = new DevExpress.XtraEditors.LabelControl();
            this.txtDateCreated = new DevExpress.XtraEditors.TextEdit();
            this.lblModulesTitle = new DevExpress.XtraEditors.LabelControl();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnMinus = new DevExpress.XtraEditors.SimpleButton();
            this.grdModules = new DevExpress.XtraGrid.GridControl();
            this.viewModules = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colModuleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.memDescription = new DevExpress.XtraEditors.MemoEdit();
            this.lblReleaseNotes = new DevExpress.XtraEditors.LabelControl();
            this.memReleaseNotes = new DevExpress.XtraEditors.MemoEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedBy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastModified.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateCreated.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReleaseNotes.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(211)))));
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1089, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblHeaderTitle.Location = new System.Drawing.Point(20, 15);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(167, 25);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "Autosoft Licensing";
            // 
            // lblProductId
            // 
            this.lblProductId.Location = new System.Drawing.Point(20, 80);
            this.lblProductId.Name = "lblProductId";
            this.lblProductId.Size = new System.Drawing.Size(58, 13);
            this.lblProductId.TabIndex = 1;
            this.lblProductId.Text = "Product ID :";
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(120, 76);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.Size = new System.Drawing.Size(200, 20);
            this.txtProductId.TabIndex = 2;
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Location = new System.Drawing.Point(340, 80);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(61, 13);
            this.lblCreatedBy.TabIndex = 3;
            this.lblCreatedBy.Text = "Created By :";
            // 
            // txtCreatedBy
            // 
            this.txtCreatedBy.Location = new System.Drawing.Point(430, 76);
            this.txtCreatedBy.Name = "txtCreatedBy";
            this.txtCreatedBy.Properties.ReadOnly = true;
            this.txtCreatedBy.Size = new System.Drawing.Size(200, 20);
            this.txtCreatedBy.TabIndex = 4;
            // 
            // lblLastModified
            // 
            this.lblLastModified.Location = new System.Drawing.Point(650, 80);
            this.lblLastModified.Name = "lblLastModified";
            this.lblLastModified.Size = new System.Drawing.Size(96, 13);
            this.lblLastModified.TabIndex = 5;
            this.lblLastModified.Text = "Last Modified Date :";
            // 
            // txtLastModified
            // 
            this.txtLastModified.Location = new System.Drawing.Point(780, 76);
            this.txtLastModified.Name = "txtLastModified";
            this.txtLastModified.Properties.ReadOnly = true;
            this.txtLastModified.Size = new System.Drawing.Size(200, 20);
            this.txtLastModified.TabIndex = 6;
            // 
            // lblProductName
            // 
            this.lblProductName.Location = new System.Drawing.Point(20, 120);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(74, 13);
            this.lblProductName.TabIndex = 7;
            this.lblProductName.Text = "Product Name :";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(120, 116);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(300, 20);
            this.txtProductName.TabIndex = 8;
            // 
            // lblDateCreated
            // 
            this.lblDateCreated.Location = new System.Drawing.Point(450, 120);
            this.lblDateCreated.Name = "lblDateCreated";
            this.lblDateCreated.Size = new System.Drawing.Size(72, 13);
            this.lblDateCreated.TabIndex = 9;
            this.lblDateCreated.Text = "Date Created :";
            // 
            // txtDateCreated
            // 
            this.txtDateCreated.Location = new System.Drawing.Point(540, 116);
            this.txtDateCreated.Name = "txtDateCreated";
            this.txtDateCreated.Properties.ReadOnly = true;
            this.txtDateCreated.Size = new System.Drawing.Size(200, 20);
            this.txtDateCreated.TabIndex = 10;
            // 
            // lblModulesTitle
            // 
            this.lblModulesTitle.Location = new System.Drawing.Point(20, 160);
            this.lblModulesTitle.Name = "lblModulesTitle";
            this.lblModulesTitle.Size = new System.Drawing.Size(46, 13);
            this.lblModulesTitle.TabIndex = 11;
            this.lblModulesTitle.Text = "Modules :";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(120, 156);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(80, 28);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "Add";
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(210, 156);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(80, 28);
            this.btnMinus.TabIndex = 13;
            this.btnMinus.Text = "Minus";
            // 
            // grdModules
            // 
            this.grdModules.Location = new System.Drawing.Point(20, 190);
            this.grdModules.MainView = this.viewModules;
            this.grdModules.Name = "grdModules";
            this.grdModules.Size = new System.Drawing.Size(960, 150);
            this.grdModules.TabIndex = 14;
            this.grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewModules});
            // 
            // viewModules
            // 
            this.viewModules.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colModuleName,
            this.colDescription});
            this.viewModules.GridControl = this.grdModules;
            this.viewModules.Name = "viewModules";
            // 
            // colModuleName
            // 
            this.colModuleName.Caption = "Module";
            this.colModuleName.FieldName = "ModuleName";
            this.colModuleName.Name = "colModuleName";
            this.colModuleName.Visible = true;
            this.colModuleName.VisibleIndex = 0;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(20, 355);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(53, 13);
            this.lblDescription.TabIndex = 15;
            this.lblDescription.Text = "Description";
            // 
            // memDescription
            // 
            this.memDescription.Location = new System.Drawing.Point(20, 375);
            this.memDescription.Name = "memDescription";
            this.memDescription.Size = new System.Drawing.Size(460, 130);
            this.memDescription.TabIndex = 16;
            // 
            // lblReleaseNotes
            // 
            this.lblReleaseNotes.Location = new System.Drawing.Point(500, 355);
            this.lblReleaseNotes.Name = "lblReleaseNotes";
            this.lblReleaseNotes.Size = new System.Drawing.Size(69, 13);
            this.lblReleaseNotes.TabIndex = 17;
            this.lblReleaseNotes.Text = "Release Notes";
            // 
            // memReleaseNotes
            // 
            this.memReleaseNotes.Location = new System.Drawing.Point(500, 375);
            this.memReleaseNotes.Name = "memReleaseNotes";
            this.memReleaseNotes.Size = new System.Drawing.Size(480, 130);
            this.memReleaseNotes.TabIndex = 18;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(820, 520);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(905, 520);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            // 
            // ProductDetailsPage
            // 
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblProductId);
            this.Controls.Add(this.txtProductId);
            this.Controls.Add(this.lblCreatedBy);
            this.Controls.Add(this.txtCreatedBy);
            this.Controls.Add(this.lblLastModified);
            this.Controls.Add(this.txtLastModified);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(this.lblDateCreated);
            this.Controls.Add(this.txtDateCreated);
            this.Controls.Add(this.lblModulesTitle);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.grdModules);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.memDescription);
            this.Controls.Add(this.lblReleaseNotes);
            this.Controls.Add(this.memReleaseNotes);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "ProductDetailsPage";
            this.Size = new System.Drawing.Size(1089, 452);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedBy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastModified.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateCreated.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReleaseNotes.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
