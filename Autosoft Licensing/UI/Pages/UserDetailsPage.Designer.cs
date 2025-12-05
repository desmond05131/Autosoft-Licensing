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
        private IContainer components = null;

        private PanelControl headerPanel;
        private LabelControl lblHeaderTitle;
        private PanelControl contentPanel;

        private LabelControl lblUsername;
        private TextEdit txtUsername;

        private LabelControl lblDisplayName;
        private TextEdit txtDisplayName;

        private LabelControl lblPassword;
        private TextEdit txtPassword;

        private CheckEdit chkIsActive;

        private LabelControl lblAccessRight;
        private GridControl grdPermissions;
        private GridView viewPermissions;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colChecked;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryCheckPermission;

        private SimpleButton btnSave;

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
            this.contentPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.lblDisplayName = new DevExpress.XtraEditors.LabelControl();
            this.txtDisplayName = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();
            this.lblAccessRight = new DevExpress.XtraEditors.LabelControl();
            this.grdPermissions = new DevExpress.XtraGrid.GridControl();
            this.viewPermissions = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChecked = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryCheckPermission = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentPanel)).BeginInit();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).BeginInit();
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
            this.headerPanel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.headerPanel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1109, 60);
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
            // contentPanel
            // 
            this.contentPanel.Appearance.BackColor = System.Drawing.Color.White;
            this.contentPanel.Appearance.Options.UseBackColor = true;
            this.contentPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.contentPanel.Controls.Add(this.lblUsername);
            this.contentPanel.Controls.Add(this.txtUsername);
            this.contentPanel.Controls.Add(this.lblDisplayName);
            this.contentPanel.Controls.Add(this.txtDisplayName);
            this.contentPanel.Controls.Add(this.lblPassword);
            this.contentPanel.Controls.Add(this.txtPassword);
            this.contentPanel.Controls.Add(this.chkIsActive);
            this.contentPanel.Controls.Add(this.lblAccessRight);
            this.contentPanel.Controls.Add(this.grdPermissions);
            this.contentPanel.Controls.Add(this.btnSave);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 60);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1109, 418);
            this.contentPanel.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.Appearance.Options.UseFont = true;
            this.lblUsername.Location = new System.Drawing.Point(30, 30);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(66, 17);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username :";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(130, 26);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Properties.Appearance.Options.UseFont = true;
            this.txtUsername.Size = new System.Drawing.Size(300, 24);
            this.txtUsername.TabIndex = 1;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDisplayName.Appearance.Options.UseFont = true;
            this.lblDisplayName.Location = new System.Drawing.Point(30, 70);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(88, 17);
            this.lblDisplayName.TabIndex = 2;
            this.lblDisplayName.Text = "Display Name :";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(130, 66);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDisplayName.Properties.Appearance.Options.UseFont = true;
            this.txtDisplayName.Size = new System.Drawing.Size(300, 24);
            this.txtDisplayName.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPassword.Appearance.Options.UseFont = true;
            this.lblPassword.Location = new System.Drawing.Point(30, 110);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(63, 17);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password :";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(130, 106);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(300, 24);
            this.txtPassword.TabIndex = 5;
            // 
            // chkIsActive
            // 
            this.chkIsActive.EditValue = true;
            this.chkIsActive.Location = new System.Drawing.Point(450, 28);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkIsActive.Properties.Appearance.Options.UseFont = true;
            this.chkIsActive.Properties.Caption = "Active";
            this.chkIsActive.Size = new System.Drawing.Size(75, 20);
            this.chkIsActive.TabIndex = 6;
            this.chkIsActive.Visible = false;
            // 
            // lblAccessRight
            // 
            this.lblAccessRight.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccessRight.Appearance.Options.UseFont = true;
            this.lblAccessRight.Location = new System.Drawing.Point(30, 160);
            this.lblAccessRight.Name = "lblAccessRight";
            this.lblAccessRight.Size = new System.Drawing.Size(77, 17);
            this.lblAccessRight.TabIndex = 7;
            this.lblAccessRight.Text = "Access Right";
            // 
            // grdPermissions
            // 
            this.grdPermissions.Location = new System.Drawing.Point(30, 185);
            this.grdPermissions.MainView = this.viewPermissions;
            this.grdPermissions.Name = "grdPermissions";
            this.grdPermissions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryCheckPermission});
            this.grdPermissions.Size = new System.Drawing.Size(520, 240);
            this.grdPermissions.TabIndex = 8;
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
            this.viewPermissions.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewPermissions.OptionsView.ColumnAutoWidth = false;
            this.viewPermissions.OptionsView.ShowGroupPanel = false;
            this.viewPermissions.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.viewPermissions_ShowingEditor);
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowEdit = false;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 0;
            this.colDescription.Width = 360;
            // 
            // colChecked
            // 
            this.colChecked.Caption = "Check?";
            this.colChecked.ColumnEdit = this.repositoryCheckPermission;
            this.colChecked.FieldName = "IsChecked";
            this.colChecked.Name = "colChecked";
            this.colChecked.Visible = true;
            this.colChecked.VisibleIndex = 1;
            this.colChecked.Width = 120;
            // 
            // repositoryCheckPermission
            // 
            this.repositoryCheckPermission.AutoHeight = false;
            this.repositoryCheckPermission.Name = "repositoryCheckPermission";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(75)))), ((int)(((byte)(255)))));
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Location = new System.Drawing.Point(1009, 398);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // UserDetailsPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "UserDetailsPage";
            this.Size = new System.Drawing.Size(1109, 478);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contentPanel)).EndInit();
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPermissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCheckPermission)).EndInit();
            this.ResumeLayout(false);

        }
    }
}