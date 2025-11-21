// Designer-generated layout for LoginPage.
// Layout and visual properties placed here to match the provided wireframe:
// - thin top bluish-gray strip, pale yellow banner with "Autosoft Licensing"
// - centered white panel with light bluish border containing labels, fields and login button
// - control names required by logic: lblLoginTitle, txtUsername, txtPassword, btnLogin, lblError
// NOTE: dynamic centering is performed at runtime in LoginPage_Load; designer uses a reasonable
// centered location so the designer can render the control correctly.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing.UI.Pages
{
    partial class LoginPage
    {
        private IContainer components = null;

        private PanelControl topStrip;
        private PanelControl topBanner;
        private LabelControl lblProductTitle;

        private PanelControl panelCenter;
        private LabelControl lblLoginTitle;
        private LabelControl lblUser;
        private TextEdit txtUsername;
        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private SimpleButton btnLogin;
        private LabelControl lblError;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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
            this.topStrip = new DevExpress.XtraEditors.PanelControl();
            this.topBanner = new DevExpress.XtraEditors.PanelControl();
            this.lblProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.panelCenter = new DevExpress.XtraEditors.PanelControl();
            this.lblLoginTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblUser = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.lblError = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.topStrip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topBanner)).BeginInit();
            this.topBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelCenter)).BeginInit();
            this.panelCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // topStrip
            // 
            this.topStrip.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(173)))), ((int)(((byte)(186)))));
            this.topStrip.Appearance.Options.UseBackColor = true;
            this.topStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.topStrip.Location = new System.Drawing.Point(0, 0);
            this.topStrip.LookAndFeel.UseDefaultLookAndFeel = false;
            this.topStrip.Name = "topStrip";
            this.topStrip.Size = new System.Drawing.Size(1083, 12);
            this.topStrip.TabIndex = 2;
            // 
            // topBanner
            // 
            this.topBanner.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(211)))));
            this.topBanner.Appearance.Options.UseBackColor = true;
            this.topBanner.Controls.Add(this.lblProductTitle);
            this.topBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBanner.Location = new System.Drawing.Point(0, 12);
            this.topBanner.LookAndFeel.SkinMaskColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(211)))));
            this.topBanner.LookAndFeel.UseDefaultLookAndFeel = false;
            this.topBanner.Name = "topBanner";
            this.topBanner.Padding = new System.Windows.Forms.Padding(14, 10, 8, 10);
            this.topBanner.Size = new System.Drawing.Size(1083, 52);
            this.topBanner.TabIndex = 1;
            this.topBanner.Paint += new System.Windows.Forms.PaintEventHandler(this.TopBanner_Paint);
            // 
            // lblProductTitle
            // 
            this.lblProductTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblProductTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblProductTitle.Appearance.Options.UseFont = true;
            this.lblProductTitle.Appearance.Options.UseForeColor = true;
            this.lblProductTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProductTitle.Location = new System.Drawing.Point(12, 14);
            this.lblProductTitle.Name = "lblProductTitle";
            this.lblProductTitle.Size = new System.Drawing.Size(250, 24);
            this.lblProductTitle.TabIndex = 0;
            this.lblProductTitle.Text = "Autosoft Licensing";
            // 
            // panelCenter
            // 
            this.panelCenter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelCenter.Appearance.BackColor = System.Drawing.Color.White;
            this.panelCenter.Appearance.Options.UseBackColor = true;
            this.panelCenter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelCenter.Controls.Add(this.lblLoginTitle);
            this.panelCenter.Controls.Add(this.lblUser);
            this.panelCenter.Controls.Add(this.txtUsername);
            this.panelCenter.Controls.Add(this.lblPassword);
            this.panelCenter.Controls.Add(this.txtPassword);
            this.panelCenter.Controls.Add(this.btnLogin);
            this.panelCenter.Controls.Add(this.lblError);
            this.panelCenter.Location = new System.Drawing.Point(261, 107);
            this.panelCenter.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Padding = new System.Windows.Forms.Padding(32);
            this.panelCenter.Size = new System.Drawing.Size(560, 420);
            this.panelCenter.TabIndex = 0;
            // 
            // lblLoginTitle
            // 
            this.lblLoginTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblLoginTitle.Appearance.Options.UseFont = true;
            this.lblLoginTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblLoginTitle.Location = new System.Drawing.Point(24, 8);
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Size = new System.Drawing.Size(512, 0);
            this.lblLoginTitle.TabIndex = 0;
            // 
            // lblUser
            // 
            this.lblUser.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblUser.Appearance.Options.UseFont = true;
            this.lblUser.Appearance.Options.UseTextOptions = true;
            this.lblUser.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblUser.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblUser.Location = new System.Drawing.Point(96, 88);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(120, 24);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "Login :";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(240, 86);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUsername.Properties.Appearance.Options.UseFont = true;
            this.txtUsername.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtUsername.Size = new System.Drawing.Size(180, 22);
            this.txtUsername.TabIndex = 0;
            // 
            // lblPassword
            // 
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPassword.Appearance.Options.UseFont = true;
            this.lblPassword.Appearance.Options.UseTextOptions = true;
            this.lblPassword.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPassword.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPassword.Location = new System.Drawing.Point(96, 148);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(120, 24);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password :";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(240, 146);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(180, 22);
            this.txtPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnLogin.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(206)))), ((int)(((byte)(215)))));
            this.btnLogin.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLogin.Appearance.Options.UseBackColor = true;
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Location = new System.Drawing.Point(237, 240);
            this.btnLogin.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(86, 34);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            // 
            // lblError
            // 
            this.lblError.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblError.Appearance.Options.UseForeColor = true;
            this.lblError.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblError.Location = new System.Drawing.Point(24, 300);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(512, 36);
            this.lblError.TabIndex = 3;
            this.lblError.Visible = false;
            // 
            // LoginPage
            // 
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.topBanner);
            this.Controls.Add(this.topStrip);
            this.Name = "LoginPage";
            this.Size = new System.Drawing.Size(1083, 634);
            this.Load += new System.EventHandler(this.LoginPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.topStrip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topBanner)).EndInit();
            this.topBanner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelCenter)).EndInit();
            this.panelCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
