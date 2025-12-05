using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Autosoft_Licensing.UI.Pages
{
    partial class UserDetailsPage
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private DevExpress.XtraEditors.PanelControl headerPanel;
        private DevExpress.XtraEditors.LabelControl lblHeaderTitle;
        private DevExpress.XtraEditors.PanelControl contentPanel;

        // Row 1
        private DevExpress.XtraEditors.LabelControl lblUsername;
        private DevExpress.XtraEditors.TextEdit txtUsername;
        private DevExpress.XtraEditors.CheckEdit chkIsActive;

        // Row 2
        private DevExpress.XtraEditors.LabelControl lblDisplayName;
        private DevExpress.XtraEditors.TextEdit txtDisplayName;

        // Row 3
        private DevExpress.XtraEditors.LabelControl lblRole;
        private DevExpress.XtraEditors.ComboBoxEdit cmbRole;

        // Row 4
        private DevExpress.XtraEditors.LabelControl lblEmail;
        private DevExpress.XtraEditors.TextEdit txtEmail;

        // Row 5
        private DevExpress.XtraEditors.LabelControl lblPassword;
        private DevExpress.XtraEditors.TextEdit txtPassword;

        // Row 6
        private DevExpress.XtraEditors.LabelControl lblCreated;
        private DevExpress.XtraEditors.TextEdit txtCreatedUtc;

        // Grid
        private DevExpress.XtraEditors.LabelControl lblAccessRight;
        private DevExpress.XtraGrid.GridControl grdPermissions;
        private DevExpress.XtraGrid.Views.Grid.GridView viewPermissions;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colChecked;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryCheckPermission;

        // Buttons
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblHeaderTitle = new DevExpress.XtraEditors.LabelControl();
            this.contentPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();
            this.lblDisplayName = new DevExpress.XtraEditors.LabelControl();
            this.txtDisplayName = new DevExpress.XtraEditors.TextEdit();
            this.lblRole = new DevExpress.XtraEditors.LabelControl();
            this.cmbRole = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblEmail = new DevExpress.XtraEditors.LabelControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblCreated = new DevExpress.XtraEditors.LabelControl();
            this.txtCreatedUtc = new DevExpress.XtraEditors.TextEdit();
            this.lblAccessRight = new DevExpress.XtraEditors.LabelControl();
            this.grdPermissions = new DevExpress.XtraGrid.GridControl();
            this.viewPermissions = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChecked = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryCheckPermission = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentPanel)).BeginInit();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedUtc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermissions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPermissions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckPermission)).BeginInit();
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
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1089, 60);
            this.headerPanel.TabIndex = 1;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.Appearance.Options.UseFont = true;
            this.lblHeaderTitle.Location = new System.Drawing.Point(18, 18);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(166, 25);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "Autosoft Licensing";
            // 
            // contentPanel
            // 
            this.contentPanel.Appearance.BackColor = System.Drawing.Color.White;
            this.contentPanel.Appearance.Options.UseBackColor = true;
            this.contentPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.contentPanel.Controls.Add(this.lblUsername);
            this.contentPanel.Controls.Add(this.txtUsername);
            this.contentPanel.Controls.Add(this.chkIsActive);
            this.contentPanel.Controls.Add(this.lblDisplayName);
            this.contentPanel.Controls.Add(this.txtDisplayName);
            this.contentPanel.Controls.Add(this.lblRole);
            this.contentPanel.Controls.Add(this.cmbRole);
            this.contentPanel.Controls.Add(this.lblEmail);
            this.contentPanel.Controls.Add(this.txtEmail);
            this.contentPanel.Controls.Add(this.lblPassword);
            this.contentPanel.Controls.Add(this.txtPassword);
            this.contentPanel.Controls.Add(this.lblCreated);
            this.contentPanel.Controls.Add(this.txtCreatedUtc);
            this.contentPanel.Controls.Add(this.lblAccessRight);
            this.contentPanel.Controls.Add(this.grdPermissions);
            this.contentPanel.Controls.Add(this.btnSave);
            this.contentPanel.Controls.Add(this.btnCancel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 60);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1089, 520);
            this.contentPanel.TabIndex = 0;
            // 
            // lblUsername
            // 
            this.lblUsername.Location = new System.Drawing.Point(30, 33);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username :";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(130, 30);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(300, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // chkIsActive
            // 
            this.chkIsActive.Location = new System.Drawing.Point(450, 30);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Properties.Caption = "Is Active?";
            this.chkIsActive.Size = new System.Drawing.Size(75, 20);
            this.chkIsActive.TabIndex = 2;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Location = new System.Drawing.Point(30, 73);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(71, 13);
            this.lblDisplayName.TabIndex = 3;
            this.lblDisplayName.Text = "Display Name :";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(130, 70);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(300, 20);
            this.txtDisplayName.TabIndex = 4;
            // 
            // lblRole
            // 
            this.lblRole.Location = new System.Drawing.Point(30, 153);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(28, 13);
            this.lblRole.TabIndex = 5;
            this.lblRole.Text = "Role :";
            // 
            // cmbRole
            // 
            this.cmbRole.Location = new System.Drawing.Point(130, 150);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbRole.Size = new System.Drawing.Size(300, 20);
            this.cmbRole.TabIndex = 6;
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(30, 193);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(31, 13);
            this.lblEmail.TabIndex = 7;
            this.lblEmail.Text = "Email :";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(130, 190);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(300, 20);
            this.txtEmail.TabIndex = 8;
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(30, 113);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 9;
            this.lblPassword.Text = "Password :";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(130, 110);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(300, 20);
            this.txtPassword.TabIndex = 10;
            // 
            // lblCreated
            // 
            this.lblCreated.Location = new System.Drawing.Point(30, 233);
            this.lblCreated.Name = "lblCreated";
            this.lblCreated.Size = new System.Drawing.Size(77, 13);
            this.lblCreated.TabIndex = 11;
            this.lblCreated.Text = "Created (UTC) :";
            // 
            // txtCreatedUtc
            // 
            this.txtCreatedUtc.Enabled = false;
            this.txtCreatedUtc.Location = new System.Drawing.Point(130, 230);
            this.txtCreatedUtc.Name = "txtCreatedUtc";
            this.txtCreatedUtc.Properties.ReadOnly = true;
            this.txtCreatedUtc.Size = new System.Drawing.Size(300, 20);
            this.txtCreatedUtc.TabIndex = 12;
            // 
            // lblAccessRight
            // 
            this.lblAccessRight.Location = new System.Drawing.Point(30, 270);
            this.lblAccessRight.Name = "lblAccessRight";
            this.lblAccessRight.Size = new System.Drawing.Size(61, 13);
            this.lblAccessRight.TabIndex = 13;
            this.lblAccessRight.Text = "Access Right";
            // 
            // grdPermissions
            // 
            this.grdPermissions.Location = new System.Drawing.Point(30, 295);
            this.grdPermissions.MainView = this.viewPermissions;
            this.grdPermissions.Name = "grdPermissions";
            this.grdPermissions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryCheckPermission});
            this.grdPermissions.Size = new System.Drawing.Size(520, 160);
            this.grdPermissions.TabIndex = 14;
            this.grdPermissions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewPermissions});
            // 
            // viewPermissions
            // 
            this.viewPermissions.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDescription,
            this.colChecked});
            this.viewPermissions.GridControl = this.grdPermissions;
            this.viewPermissions.Name = "viewPermissions";
            this.viewPermissions.OptionsView.ShowGroupPanel = false;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 0;
            this.colDescription.Width = 350;
            // 
            // colChecked
            // 
            this.colChecked.Caption = "Check?";
            this.colChecked.ColumnEdit = this.repositoryCheckPermission;
            this.colChecked.FieldName = "IsChecked";
            this.colChecked.Name = "colChecked";
            this.colChecked.Visible = true;
            this.colChecked.VisibleIndex = 1;
            this.colChecked.Width = 100;
            // 
            // repositoryCheckPermission
            // 
            this.repositoryCheckPermission.Name = "repositoryCheckPermission";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnSave.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Location = new System.Drawing.Point(910, 500);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(820, 500);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            // 
            // UserDetailsPage
            // 
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "UserDetailsPage";
            this.Size = new System.Drawing.Size(1000, 550); // Base size so anchors position correctly
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentPanel)).EndInit();
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreatedUtc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPermissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckPermission)).EndInit();
            this.ResumeLayout(false);

        }
    }
}