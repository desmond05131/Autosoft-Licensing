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

            // Initialize Row 1
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();

            // Initialize Row 2
            this.lblDisplayName = new DevExpress.XtraEditors.LabelControl();
            this.txtDisplayName = new DevExpress.XtraEditors.TextEdit();

            // Initialize Row 3 (Role)
            this.lblRole = new DevExpress.XtraEditors.LabelControl();
            this.cmbRole = new DevExpress.XtraEditors.ComboBoxEdit();

            // Initialize Row 4 (Email)
            this.lblEmail = new DevExpress.XtraEditors.LabelControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();

            // Initialize Row 5 (Password)
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();

            // Initialize Row 6 (Created)
            this.lblCreated = new DevExpress.XtraEditors.LabelControl();
            this.txtCreatedUtc = new DevExpress.XtraEditors.TextEdit();

            // Grid & Buttons
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

            // headerPanel
            this.headerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(253, 243, 211);
            this.headerPanel.Appearance.Options.UseBackColor = true;
            this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Size = new System.Drawing.Size(1000, 60);

            // lblHeaderTitle
            this.lblHeaderTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.Location = new System.Drawing.Point(18, 18);
            this.lblHeaderTitle.Text = "Autosoft Licensing";

            // contentPanel
            this.contentPanel.Appearance.BackColor = System.Drawing.Color.White;
            this.contentPanel.Appearance.Options.UseBackColor = true;
            this.contentPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add ALL controls here
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

            // Row 1: Username (Label Y=33), Is Active aligned with textbox at Y=30
            this.lblUsername.Location = new System.Drawing.Point(30, 33);
            this.lblUsername.Text = "Username :";
            this.txtUsername.Location = new System.Drawing.Point(130, 30);
            this.txtUsername.Size = new System.Drawing.Size(300, 24);

            this.chkIsActive.Location = new System.Drawing.Point(450, 30);
            this.chkIsActive.Properties.Caption = "Is Active?";
            this.chkIsActive.Visible = true;

            // Row 2: Display Name (Label Y=73)
            this.lblDisplayName.Location = new System.Drawing.Point(30, 73);
            this.lblDisplayName.Text = "Display Name :";
            this.txtDisplayName.Location = new System.Drawing.Point(130, 70);
            this.txtDisplayName.Size = new System.Drawing.Size(300, 24);

            // Row 3: Password (Moved Up) (Label Y=113)
            this.lblPassword.Location = new System.Drawing.Point(30, 113);
            this.lblPassword.Text = "Password :";
            this.txtPassword.Location = new System.Drawing.Point(130, 110);
            this.txtPassword.Size = new System.Drawing.Size(300, 24);
            this.txtPassword.Properties.UseSystemPasswordChar = true;

            // Row 4: Role (Moved Down) (Label Y=153)
            this.lblRole.Location = new System.Drawing.Point(30, 153);
            this.lblRole.Text = "Role :";
            this.cmbRole.Location = new System.Drawing.Point(130, 150);
            this.cmbRole.Size = new System.Drawing.Size(300, 24);
            this.cmbRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            // Row 5: Email (Moved Down) (Label Y=193)
            this.lblEmail.Location = new System.Drawing.Point(30, 193);
            this.lblEmail.Text = "Email :";
            this.txtEmail.Location = new System.Drawing.Point(130, 190);
            this.txtEmail.Size = new System.Drawing.Size(300, 24);

            // Row 6: Created (Label Y=233)
            this.lblCreated.Location = new System.Drawing.Point(30, 233);
            this.lblCreated.Text = "Created (UTC) :";
            this.txtCreatedUtc.Location = new System.Drawing.Point(130, 230);
            this.txtCreatedUtc.Size = new System.Drawing.Size(300, 24);
            this.txtCreatedUtc.Properties.ReadOnly = true;
            this.txtCreatedUtc.Enabled = false;

            // Grid: Access Rights
            this.lblAccessRight.Location = new System.Drawing.Point(30, 270);
            this.lblAccessRight.Text = "Access Right";
            this.grdPermissions.Location = new System.Drawing.Point(30, 295);
            this.grdPermissions.Size = new System.Drawing.Size(520, 160);
            this.grdPermissions.MainView = this.viewPermissions;
            this.grdPermissions.RepositoryItems.Add(this.repositoryCheckPermission);

            // View Config
            this.viewPermissions.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { this.colDescription, this.colChecked });
            this.viewPermissions.OptionsView.ShowGroupPanel = false;
            this.colDescription.FieldName = "Description";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 0;
            this.colDescription.Width = 350;
            this.colChecked.FieldName = "IsChecked";
            this.colChecked.Caption = "Check?";
            this.colChecked.ColumnEdit = this.repositoryCheckPermission;
            this.colChecked.Visible = true;
            this.colChecked.VisibleIndex = 1;
            this.colChecked.Width = 100;

            // Buttons
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(900, 500);
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));

            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(990, 500);
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(98, 75, 255);
            this.btnSave.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;

            // Finalizing
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
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