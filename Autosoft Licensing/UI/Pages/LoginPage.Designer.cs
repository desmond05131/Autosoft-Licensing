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
            this.components = new Container();
            this.panelCenter = new PanelControl();
            this.lblLoginTitle = new LabelControl();
            this.lblUser = new LabelControl();
            this.txtUsername = new TextEdit();
            this.lblPassword = new LabelControl();
            this.txtPassword = new TextEdit();
            this.btnLogin = new SimpleButton();
            this.lblError = new LabelControl();

            // 
            // LoginPage (UserControl)
            // 
            this.Name = "LoginPage";
            this.Dock = DockStyle.Fill;

            // 
            // panelCenter
            // 
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelCenter.Size = new Size(520, 320);
            this.panelCenter.Location = new Point((this.Width - this.panelCenter.Width) / 2, 60);
            this.panelCenter.Anchor = AnchorStyles.None;
            this.panelCenter.Padding = new Padding(24);
            this.panelCenter.TabIndex = 0;

            // 
            // lblLoginTitle
            // 
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Text = "Login";
            this.lblLoginTitle.Appearance.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            this.lblLoginTitle.Appearance.Options.UseFont = true;
            this.lblLoginTitle.AutoSizeMode = LabelAutoSizeMode.Vertical;
            this.lblLoginTitle.Location = new Point(24, 12);
            this.lblLoginTitle.Size = new Size(472, 28);

            // 
            // lblUser
            // 
            this.lblUser.Name = "lblUser";
            this.lblUser.Text = "Login :";
            this.lblUser.Appearance.Font = new Font("Segoe UI", 10F);
            this.lblUser.Location = new Point(80, 70);
            this.lblUser.AutoSizeMode = LabelAutoSizeMode.Vertical;

            // 
            // txtUsername
            // 
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Location = new Point(200, 66);
            this.txtUsername.Size = new Size(180, 22);
            this.txtUsername.Anchor = AnchorStyles.Top;
            this.txtUsername.TabIndex = 0;

            // 
            // lblPassword
            // 
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Text = "Password :";
            this.lblPassword.Appearance.Font = new Font("Segoe UI", 10F);
            this.lblPassword.Location = new Point(80, 120);
            this.lblPassword.AutoSizeMode = LabelAutoSizeMode.Vertical;

            // 
            // txtPassword
            // 
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Location = new Point(200, 116);
            this.txtPassword.Size = new Size(180, 22);
            this.txtPassword.Anchor = AnchorStyles.Top;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.TabIndex = 1;

            // 
            // btnLogin
            // 
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Text = "Login";
            this.btnLogin.Size = new Size(90, 30);
            this.btnLogin.Location = new Point((this.panelCenter.Width - this.btnLogin.Width) / 2, 200);
            this.btnLogin.Anchor = AnchorStyles.Bottom;
            this.btnLogin.TabIndex = 2;

            // 
            // lblError
            // 
            this.lblError.Name = "lblError";
            this.lblError.Text = "";
            this.lblError.Appearance.ForeColor = Color.DarkRed;
            this.lblError.AutoSizeMode = LabelAutoSizeMode.Vertical;
            this.lblError.Location = new Point(24, 250);
            this.lblError.Size = new Size(472, 40);
            this.lblError.Visible = false;

            // Add controls to panel
            this.panelCenter.Controls.Add(this.lblLoginTitle);
            this.panelCenter.Controls.Add(this.lblUser);
            this.panelCenter.Controls.Add(this.txtUsername);
            this.panelCenter.Controls.Add(this.lblPassword);
            this.panelCenter.Controls.Add(this.txtPassword);
            this.panelCenter.Controls.Add(this.btnLogin);
            this.panelCenter.Controls.Add(this.lblError);

            // Add panel to the page
            this.Controls.Add(this.panelCenter);

            // After adding controls, perform a simple layout centering
            this.Load += LoginPage_Load;
        }

        private void LoginPage_Load(object sender, EventArgs e)
        {
            // Center the panel within the parent control at runtime
            if (this.panelCenter != null)
            {
                var parentWidth = this.ClientSize.Width;
                var parentHeight = this.ClientSize.Height;
                this.panelCenter.Left = (parentWidth - this.panelCenter.Width) / 2;
                this.panelCenter.Top = Math.Max(40, (parentHeight - this.panelCenter.Height) / 2);
            }
        }
    }
}
