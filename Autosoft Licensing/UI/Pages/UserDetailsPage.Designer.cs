using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autosoft_Licensing.UI.Pages
{
    partial class UserDetailsPage
    {
        private System.ComponentModel.IContainer components = null;

        private DevExpress.XtraEditors.PanelControl pnlHeader;
        private DevExpress.XtraEditors.LabelControl lblHeader;

        private DevExpress.XtraEditors.LabelControl lblUsername;
        private DevExpress.XtraEditors.TextEdit txtUsername;

        private DevExpress.XtraEditors.LabelControl lblPassword;
        private DevExpress.XtraEditors.TextEdit txtPassword;

        private DevExpress.XtraEditors.CheckEdit chkIsActive;

        private DevExpress.XtraEditors.GroupControl groupAccess;
        private DevExpress.XtraEditors.CheckEdit chkGenerate;
        private DevExpress.XtraEditors.CheckEdit chkRecords;
        private DevExpress.XtraEditors.CheckEdit chkProduct;
        private DevExpress.XtraEditors.CheckEdit chkUsers;

        // Optional fields to match existing model convenience
        private DevExpress.XtraEditors.LabelControl lblDisplayName;
        private DevExpress.XtraEditors.TextEdit txtDisplayName;
        private DevExpress.XtraEditors.LabelControl lblRole;
        private DevExpress.XtraEditors.TextEdit txtRole;
        private DevExpress.XtraEditors.LabelControl lblEmail;
        private DevExpress.XtraEditors.TextEdit txtEmail;

        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblHeader = new DevExpress.XtraEditors.LabelControl();
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();
            this.groupAccess = new DevExpress.XtraEditors.GroupControl();
            this.chkGenerate = new DevExpress.XtraEditors.CheckEdit();
            this.chkRecords = new DevExpress.XtraEditors.CheckEdit();
            this.chkProduct = new DevExpress.XtraEditors.CheckEdit();
            this.chkUsers = new DevExpress.XtraEditors.CheckEdit();
            this.lblDisplayName = new DevExpress.XtraEditors.LabelControl();
            this.txtDisplayName = new DevExpress.XtraEditors.TextEdit();
            this.lblRole = new DevExpress.XtraEditors.LabelControl();
            this.txtRole = new DevExpress.XtraEditors.TextEdit();
            this.lblEmail = new DevExpress.XtraEditors.LabelControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlHeader)).BeginInit();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAccess)).BeginInit();
            this.groupAccess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkGenerate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecords.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProduct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUsers.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRole.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1089, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Appearance.Options.UseFont = true;
            this.lblHeader.Location = new System.Drawing.Point(12, 14);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(91, 21);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "User Details";
            // 
            // lblUsername
            // 
            this.lblUsername.Location = new System.Drawing.Point(20, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(48, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(120, 66);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(250, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(20, 100);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(46, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(120, 96);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(250, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // chkIsActive
            // 
            this.chkIsActive.Location = new System.Drawing.Point(120, 126);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Properties.Caption = "Is Active?";
            this.chkIsActive.Size = new System.Drawing.Size(100, 20);
            this.chkIsActive.TabIndex = 5;
            // 
            // groupAccess
            // 
            this.groupAccess.Controls.Add(this.chkGenerate);
            this.groupAccess.Controls.Add(this.chkRecords);
            this.groupAccess.Controls.Add(this.chkProduct);
            this.groupAccess.Controls.Add(this.chkUsers);
            this.groupAccess.Location = new System.Drawing.Point(20, 170);
            this.groupAccess.Name = "groupAccess";
            this.groupAccess.Size = new System.Drawing.Size(730, 140);
            this.groupAccess.TabIndex = 12;
            this.groupAccess.Text = "Access Right";
            // 
            // chkGenerate
            // 
            this.chkGenerate.Location = new System.Drawing.Point(20, 40);
            this.chkGenerate.Name = "chkGenerate";
            this.chkGenerate.Properties.Caption = "Generate License";
            this.chkGenerate.Size = new System.Drawing.Size(180, 20);
            this.chkGenerate.TabIndex = 0;
            // 
            // chkRecords
            // 
            this.chkRecords.Location = new System.Drawing.Point(20, 70);
            this.chkRecords.Name = "chkRecords";
            this.chkRecords.Properties.Caption = "License record";
            this.chkRecords.Size = new System.Drawing.Size(180, 20);
            this.chkRecords.TabIndex = 1;
            // 
            // chkProduct
            // 
            this.chkProduct.Location = new System.Drawing.Point(260, 40);
            this.chkProduct.Name = "chkProduct";
            this.chkProduct.Properties.Caption = "Manage Product";
            this.chkProduct.Size = new System.Drawing.Size(180, 20);
            this.chkProduct.TabIndex = 2;
            // 
            // chkUsers
            // 
            this.chkUsers.Location = new System.Drawing.Point(260, 70);
            this.chkUsers.Name = "chkUsers";
            this.chkUsers.Properties.Caption = "Manage User";
            this.chkUsers.Size = new System.Drawing.Size(180, 20);
            this.chkUsers.TabIndex = 3;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Location = new System.Drawing.Point(400, 70);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(64, 13);
            this.lblDisplayName.TabIndex = 6;
            this.lblDisplayName.Text = "Display Name";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(500, 66);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(250, 20);
            this.txtDisplayName.TabIndex = 7;
            // 
            // lblRole
            // 
            this.lblRole.Location = new System.Drawing.Point(400, 100);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(21, 13);
            this.lblRole.TabIndex = 8;
            this.lblRole.Text = "Role";
            // 
            // txtRole
            // 
            this.txtRole.Location = new System.Drawing.Point(500, 96);
            this.txtRole.Name = "txtRole";
            this.txtRole.Size = new System.Drawing.Size(250, 20);
            this.txtRole.TabIndex = 9;
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(400, 130);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(24, 13);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(500, 126);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(250, 20);
            this.txtEmail.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(560, 330);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(660, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            // 
            // UserDetailsPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkIsActive);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.txtRole);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.groupAccess);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "UserDetailsPage";
            this.Size = new System.Drawing.Size(1089, 495);
            ((System.ComponentModel.ISupportInitialize)(this.pnlHeader)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAccess)).EndInit();
            this.groupAccess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkGenerate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRecords.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkProduct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUsers.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRole.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
