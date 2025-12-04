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
            this.components = new System.ComponentModel.Container();
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

            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedBy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastModified.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateCreated.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReleaseNotes.Properties)).BeginInit();

            // Panel Header
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 60;
            this.pnlHeader.BackColor = Color.FromArgb(253, 243, 211);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);

            this.lblHeaderTitle.Text = "Autosoft Licensing";
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblHeaderTitle.Location = new Point(20, 15);

            // Row 1: Product ID, Created By, Last Modified
            this.lblProductId.Location = new Point(20, 80);
            this.lblProductId.Text = "Product ID :";
            this.txtProductId.Location = new Point(120, 76);
            this.txtProductId.Size = new Size(200, 24);

            this.lblCreatedBy.Location = new Point(340, 80);
            this.lblCreatedBy.Text = "Created By :";
            this.txtCreatedBy.Location = new Point(430, 76);
            this.txtCreatedBy.Size = new Size(200, 24);
            this.txtCreatedBy.Properties.ReadOnly = true;

            this.lblLastModified.Location = new Point(650, 80);
            this.lblLastModified.Text = "Last Modified Date :";
            this.txtLastModified.Location = new Point(780, 76);
            this.txtLastModified.Size = new Size(200, 24);
            this.txtLastModified.Properties.ReadOnly = true;

            // Row 2: Product Name, Date Created
            this.lblProductName.Location = new Point(20, 120);
            this.lblProductName.Text = "Product Name :";
            this.txtProductName.Location = new Point(120, 116);
            this.txtProductName.Size = new Size(300, 24);

            this.lblDateCreated.Location = new Point(450, 120);
            this.lblDateCreated.Text = "Date Created :";
            this.txtDateCreated.Location = new Point(540, 116);
            this.txtDateCreated.Size = new Size(200, 24);
            this.txtDateCreated.Properties.ReadOnly = true;

            // Modules section title and buttons
            this.lblModulesTitle.Location = new Point(20, 160);
            this.lblModulesTitle.Text = "Modules :";

            this.btnAdd.Location = new Point(120, 156);
            this.btnAdd.Size = new Size(80, 28);
            this.btnAdd.Text = "Add";

            this.btnMinus.Location = new Point(210, 156);
            this.btnMinus.Size = new Size(80, 28);
            this.btnMinus.Text = "Minus";

            // GridControl for Modules
            this.grdModules.Location = new Point(20, 190);
            this.grdModules.Size = new Size(960, 150);
            this.grdModules.MainView = this.viewModules;
            this.grdModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.viewModules });

            this.viewModules.GridControl = this.grdModules;
            this.viewModules.OptionsBehavior.Editable = true;

            // Columns: Module, Description
            this.colModuleName.Caption = "Module";
            this.colModuleName.FieldName = "ModuleName";
            this.colModuleName.Visible = true;
            this.colModuleName.VisibleIndex = 0;
            this.viewModules.Columns.Add(this.colModuleName);

            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            this.viewModules.Columns.Add(this.colDescription);

            // Footer section: Description (left), Release Notes (right)
            this.lblDescription.Location = new Point(20, 355);
            this.lblDescription.Text = "Description";
            this.memDescription.Location = new Point(20, 375);
            this.memDescription.Size = new Size(460, 130);

            this.lblReleaseNotes.Location = new Point(500, 355);
            this.lblReleaseNotes.Text = "Release Notes";
            this.memReleaseNotes.Location = new Point(500, 375);
            this.memReleaseNotes.Size = new Size(480, 130);

            // Bottom action bar: Save, Cancel
            this.btnSave.Location = new Point(820, 520);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Text = "Save";

            this.btnCancel.Location = new Point(905, 520);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Text = "Cancel";

            // Add controls to the UserControl
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

            // Root size
            this.Size = new Size(1000, 570);

            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedBy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastModified.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateCreated.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReleaseNotes.Properties)).EndInit();
        }
    }
}
