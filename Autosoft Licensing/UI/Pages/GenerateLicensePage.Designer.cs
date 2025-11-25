

//No rush — think carefully and match the wireframe visually and behaviorally.
//Reference screenshot (visual target): C: \Users\User\Pictures\Screenshots\Screenshot 2025-11-24 170655.png
//Goal: Adjust the GenerateLicensePage UI so it matches the wireframe exactly (layout, spacing, sizes, fonts, padding, and alignment). Use DevExpress controls where possible. You are fixing only the Designer/layout and minor view plumbing — do not change any backend logic.
//Rules (must follow):
//1.Use DevExpress LayoutControl(DevExpress.XtraLayout.LayoutControl) as the root layout container.
//•	Replace any ad-hoc absolute positions with rows/columns in LayoutControl.
//•	Use LayoutControlGroups and empty SpaceItems to precisely control spacing.
//2.	Page sections and exact arrangement (left-to-right):
//•	Top navigation row (full width): • Use DevExpress BarManager or simple horizontal PanelControl with four SimpleButtons:
//•	btnNav_GenerateLicense(active, orange underline)
//•	btnNav_LicenseRecords
//•	btnNav_ManageProduct
//•	btnNav_ManageUser • Style: font Segoe UI 10, padding 12; active tab: color #F07B22 underline 3px. • Icons: if no assets provided, use DevExpress built-in SVG icons (SvgImage property). If you must reference files, use placeholder path "Assets/generate.png" etc.
//•	Below nav, left-to-right three panels with exact widths: • Info GroupControl(width ~40%): -Title "Info" - Fields vertically: Company Name(TextEdit full-width), Product ID(TextEdit smaller width under), Product Name(TextEdit full-width). -Labels aligned right of field captions as in wireframe. - Inner padding: 12px. • Types GroupControl(width ~20%): -Title "Types" - RadioGroup with 3 options stacked (Demo / Subscription / Permanent). - SpinEdit labeled "Months:" aligned below RadioGroup; hide / disable when Demo selected. • Modules GroupControl (width ~35%): -Title "Modules" - Use DevExpress CheckedListBoxControl or GridControl with RepositoryItemCheckEdit. - Must show vertical scroll bar when items exceed visible area. - Ensure group height equals Info/Types group height.
//•	Under those groups, the date fields aligned left: • Issue Date(DateEdit) • Expire Date(DateEdit) • Put both on same column as Info group, left aligned with Info group inner padding.
//•	Remarks: • A full-width MemoEdit under dates aligned with Info column (same left & right margins as Info).
//•	Bottom row: • Left: readonly TextEdit txtLicenseKey(stretch full width from left margin to before buttons) • Right: 3 buttons horizontally aligned: btnGenerateLicenseKey, btnPreview, btnDownload. Buttons should be right-aligned and vertically centered with the txtLicenseKey control. • Button sizes: 140px × 32px, spacing 12px.
//3.	Typography and colors:
//•	Use Segoe UI for all labels and controls.
//•	Field labels: 10pt, ForeColor #333333.
//•	Big title / nav heading: 14pt bold.
//•	GroupControl header background: light #F5F6F7 with border #D1D1D1.
//•	Buttons: use DevExpress style, primary grey for action, primary blue for active if required.
//4.	Margins & spacing (exact):
//•	Outer page padding: 20px from form edge.
//•	Gap between top nav and groups: 18px.
//•	Horizontal gap between Info/Types/Modules groups: 24px.
//•	Vertical gap between groups and Issue/Expire/Remarks: 18px.
//•	Bottom gap before license key row: 20px.
//5.Controls properties & names(must use these names so backend binding stays intact):
//•	txtCompanyName(TextEdit, ReadOnly = true)
//•	txtProductId(TextEdit, ReadOnly = true)
//•	txtProductName(TextEdit, ReadOnly = true)
//•	rgLicenseType(RadioGroup)
//•	spnSubscriptionMonths(SpinEdit)
//•	chkModules(CheckedListBoxControl) or grdModules(GridControl + viewModules)
//•	dtIssueDate(DateEdit)
//•	dtExpireDate(DateEdit)
//•	memRemark(MemoEdit)
//•	txtLicenseKey(TextEdit, ReadOnly = true)
//•	btnGenerateLicenseKey(SimpleButton)
//•	btnPreview(SimpleButton)
//•	btnDownload(SimpleButton)
//•	btnNav_GenerateLicense, btnNav_LicenseRecords, btnNav_ManageProduct, btnNav_ManageUser (SimpleButton / SvgImage)
//6.	Visual details to correct from current state:
//•	The Modules group in your screenshot is too large horizontally; reduce to ~35% width and align top with Info group header.
//•	Types group needs the radio options centered vertically with the Months SpinEdit below (not left-flushed).
//•	The Issue/Expire date fields must be aligned under Info group and not indented; set their Left constraint equal to Info group's left padding (use LayoutControlItem with ControlAlignment).
//•	License Key textbox must stretch to left margin (same left as Info group) and end right before the buttons; ensure it is vertically aligned with the buttons.
//•	The "Generate License Key" button must be visually primary (slightly darker border) and placed immediately to the right of txtLicenseKey; Preview then Download follow.
//•	Add 3px orange underline under "Generate License" nav button to show active state.
//7.	Accessibility & behavior:
//•	Ensure TabOrder is logical: top nav → upload → Info fields → Types → Modules → dates → remark → license key → buttons.
//•	Set Anchor/Dock: groups dock Top with fixed heights; modules fill vertical space.
//•	Make controls DPI-aware (use LayoutControl to adapt).
//8.	Use the reference screenshot (/mnt/data/3063181f-a3b5-478d-a081-1f766e7df742.png) as the authoritative visual. Adjust paddings, sizes and fonts until the page visually matches it. Output must be a Designer.cs update (and any small helper code if required for layout).
//9.	If icons are missing, use DevExpress built-in SVG icons using ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromResources(...) or load placeholders from Assets/*.png.
//Implementation note:
//•	Keep all executable logic unchanged. This change is purely layout and style. Add // LAYOUT: adjusted to wireframe comments where you alter the Designer.
//After completing the Designer changes, return only the updated Designer.cs code snippet (or the patch) and a short list of the exact property changes you made (sizes, paddings, names). Then stop and wait for review.




// Designer partial for GenerateLicensePage
// NOTE: kept intentionally minimal and designer-friendly.
// Moved property-intensive initialization and event wiring into the code-behind constructor
// to avoid designer parse-time null-reference issues.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Autosoft_Licensing.UI.Pages
{
    partial class GenerateLicensePage
    {
        private IContainer components = null;

        // Top navigation-ish buttons (visual parity with wireframe)
        private SimpleButton btnNav_GenerateLicense;
        private SimpleButton btnNav_LicenseRecords;
        private SimpleButton btnNav_ManageProduct;
        private SimpleButton btnNav_ManageUser;

        private SimpleButton btnUploadArl;

        // Info group
        private GroupControl grpInfo;
        private TextEdit txtCompanyName;
        private TextEdit txtProductId;
        private TextEdit txtProductName;
        private LabelControl lblCompanyName;
        private LabelControl lblProductId;
        private LabelControl lblProductName;

        // Types group
        private GroupControl grpTypes;
        private RadioGroup rgLicenseType;
        private SpinEdit numSubscriptionMonths;
        private LabelControl lblMonths;

        // Modules group (replaced GridControl with DevExpress CheckedListBoxControl)
        private GroupControl grpModules;
        private CheckedListBoxControl chkModules;

        // Dates & remark & key
        private LabelControl lblIssueDate;
        private DateEdit dtIssueDate;
        private LabelControl lblExpireDate;
        private DateEdit dtExpireDate;
        private LabelControl lblRemark;
        private MemoEdit memRemark;

        private LabelControl lblLicenseKey;
        private TextEdit txtLicenseKey;

        // Action buttons
        private SimpleButton btnGenerateKey;
        private SimpleButton btnPreview;
        private SimpleButton btnDownload;

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

        /// <summary>
        /// Minimal InitializeComponent to make the designer stable.
        /// Heavy initialization (Properties.*, event wiring) is done in the code-behind constructor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNav_GenerateLicense = new DevExpress.XtraEditors.SimpleButton();
            this.btnNav_LicenseRecords = new DevExpress.XtraEditors.SimpleButton();
            this.btnNav_ManageProduct = new DevExpress.XtraEditors.SimpleButton();
            this.btnNav_ManageUser = new DevExpress.XtraEditors.SimpleButton();
            this.btnUploadArl = new DevExpress.XtraEditors.SimpleButton();
            this.grpInfo = new DevExpress.XtraEditors.GroupControl();
            this.lblCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.txtCompanyName = new DevExpress.XtraEditors.TextEdit();
            this.lblProductId = new DevExpress.XtraEditors.LabelControl();
            this.txtProductId = new DevExpress.XtraEditors.TextEdit();
            this.lblProductName = new DevExpress.XtraEditors.LabelControl();
            this.txtProductName = new DevExpress.XtraEditors.TextEdit();
            this.grpTypes = new DevExpress.XtraEditors.GroupControl();
            this.rgLicenseType = new DevExpress.XtraEditors.RadioGroup();
            this.lblMonths = new DevExpress.XtraEditors.LabelControl();
            this.numSubscriptionMonths = new DevExpress.XtraEditors.SpinEdit();
            this.grpModules = new DevExpress.XtraEditors.GroupControl();
            this.chkModules = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.lblIssueDate = new DevExpress.XtraEditors.LabelControl();
            this.dtIssueDate = new DevExpress.XtraEditors.DateEdit();
            this.lblExpireDate = new DevExpress.XtraEditors.LabelControl();
            this.dtExpireDate = new DevExpress.XtraEditors.DateEdit();
            this.lblRemark = new DevExpress.XtraEditors.LabelControl();
            this.memRemark = new DevExpress.XtraEditors.MemoEdit();
            this.lblLicenseKey = new DevExpress.XtraEditors.LabelControl();
            this.txtLicenseKey = new DevExpress.XtraEditors.TextEdit();
            this.btnGenerateKey = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownload = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpInfo)).BeginInit();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTypes)).BeginInit();
            this.grpTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgLicenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubscriptionMonths.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpModules)).BeginInit();
            this.grpModules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkModules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNav_GenerateLicense
            // 
            this.btnNav_GenerateLicense.Location = new System.Drawing.Point(12, 12);
            this.btnNav_GenerateLicense.Name = "btnNav_GenerateLicense";
            this.btnNav_GenerateLicense.Size = new System.Drawing.Size(140, 28);
            this.btnNav_GenerateLicense.TabIndex = 0;
            this.btnNav_GenerateLicense.Text = "Generate License";
            // 
            // btnNav_LicenseRecords
            // 
            this.btnNav_LicenseRecords.Location = new System.Drawing.Point(160, 12);
            this.btnNav_LicenseRecords.Name = "btnNav_LicenseRecords";
            this.btnNav_LicenseRecords.Size = new System.Drawing.Size(120, 28);
            this.btnNav_LicenseRecords.TabIndex = 1;
            this.btnNav_LicenseRecords.Text = "License Records";
            // 
            // btnNav_ManageProduct
            // 
            this.btnNav_ManageProduct.Location = new System.Drawing.Point(288, 12);
            this.btnNav_ManageProduct.Name = "btnNav_ManageProduct";
            this.btnNav_ManageProduct.Size = new System.Drawing.Size(120, 28);
            this.btnNav_ManageProduct.TabIndex = 2;
            this.btnNav_ManageProduct.Text = "Manage Product";
            // 
            // btnNav_ManageUser
            // 
            this.btnNav_ManageUser.Location = new System.Drawing.Point(416, 12);
            this.btnNav_ManageUser.Name = "btnNav_ManageUser";
            this.btnNav_ManageUser.Size = new System.Drawing.Size(120, 28);
            this.btnNav_ManageUser.TabIndex = 3;
            this.btnNav_ManageUser.Text = "Manage User";
            // 
            // btnUploadArl
            // 
            this.btnUploadArl.Location = new System.Drawing.Point(12, 56);
            this.btnUploadArl.Name = "btnUploadArl";
            this.btnUploadArl.Size = new System.Drawing.Size(160, 34);
            this.btnUploadArl.TabIndex = 4;
            this.btnUploadArl.Text = "Upload License File";
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.lblCompanyName);
            this.grpInfo.Controls.Add(this.txtCompanyName);
            this.grpInfo.Controls.Add(this.lblProductId);
            this.grpInfo.Controls.Add(this.txtProductId);
            this.grpInfo.Controls.Add(this.lblProductName);
            this.grpInfo.Controls.Add(this.txtProductName);
            this.grpInfo.Location = new System.Drawing.Point(12, 110);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(420, 160);
            this.grpInfo.TabIndex = 5;
            this.grpInfo.Text = "Info";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCompanyName.Location = new System.Drawing.Point(12, 28);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(110, 22);
            this.lblCompanyName.TabIndex = 0;
            this.lblCompanyName.Text = "Company Name :";
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(128, 26);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Properties.ReadOnly = true;
            this.txtCompanyName.Size = new System.Drawing.Size(280, 20);
            this.txtCompanyName.TabIndex = 1;
            // 
            // lblProductId
            // 
            this.lblProductId.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProductId.Location = new System.Drawing.Point(12, 60);
            this.lblProductId.Name = "lblProductId";
            this.lblProductId.Size = new System.Drawing.Size(110, 22);
            this.lblProductId.TabIndex = 2;
            this.lblProductId.Text = "Product ID :";
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(128, 58);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.Properties.ReadOnly = true;
            this.txtProductId.Size = new System.Drawing.Size(150, 20);
            this.txtProductId.TabIndex = 3;
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProductName.Location = new System.Drawing.Point(12, 92);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(110, 22);
            this.lblProductName.TabIndex = 4;
            this.lblProductName.Text = "Product Name :";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(128, 90);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Properties.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(280, 20);
            this.txtProductName.TabIndex = 5;
            // 
            // grpTypes
            // 
            this.grpTypes.Controls.Add(this.rgLicenseType);
            this.grpTypes.Controls.Add(this.lblMonths);
            this.grpTypes.Controls.Add(this.numSubscriptionMonths);
            this.grpTypes.Location = new System.Drawing.Point(444, 110);
            this.grpTypes.Name = "grpTypes";
            this.grpTypes.Size = new System.Drawing.Size(220, 160);
            this.grpTypes.TabIndex = 6;
            this.grpTypes.Text = "Types";
            // 
            // rgLicenseType
            // 
            this.rgLicenseType.Location = new System.Drawing.Point(12, 24);
            this.rgLicenseType.Name = "rgLicenseType";
            this.rgLicenseType.Size = new System.Drawing.Size(190, 80);
            this.rgLicenseType.TabIndex = 0;
            // 
            // lblMonths
            // 
            this.lblMonths.Location = new System.Drawing.Point(12, 110);
            this.lblMonths.Name = "lblMonths";
            this.lblMonths.Size = new System.Drawing.Size(42, 13);
            this.lblMonths.TabIndex = 1;
            this.lblMonths.Text = "Months :";
            // 
            // numSubscriptionMonths
            // 
            this.numSubscriptionMonths.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numSubscriptionMonths.Location = new System.Drawing.Point(70, 108);
            this.numSubscriptionMonths.Name = "numSubscriptionMonths";
            this.numSubscriptionMonths.Size = new System.Drawing.Size(80, 20);
            this.numSubscriptionMonths.TabIndex = 2;
            // 
            // grpModules
            // 
            this.grpModules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpModules.Controls.Add(this.chkModules);
            this.grpModules.Location = new System.Drawing.Point(701, 110);
            this.grpModules.Name = "grpModules";
            this.grpModules.Size = new System.Drawing.Size(320, 260);
            this.grpModules.TabIndex = 7;
            this.grpModules.Text = "Modules";
            // 
            // chkModules
            // 
            this.chkModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkModules.Location = new System.Drawing.Point(2, 23);
            this.chkModules.Name = "chkModules";
            this.chkModules.Size = new System.Drawing.Size(316, 235);
            this.chkModules.TabIndex = 0;
            // 
            // lblIssueDate
            // 
            this.lblIssueDate.Location = new System.Drawing.Point(12, 288);
            this.lblIssueDate.Name = "lblIssueDate";
            this.lblIssueDate.Size = new System.Drawing.Size(59, 13);
            this.lblIssueDate.TabIndex = 8;
            this.lblIssueDate.Text = "Issue Date :";
            // 
            // dtIssueDate
            // 
            this.dtIssueDate.EditValue = new System.DateTime(2025, 11, 24, 0, 0, 0, 0);
            this.dtIssueDate.Location = new System.Drawing.Point(100, 286);
            this.dtIssueDate.Name = "dtIssueDate";
            this.dtIssueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIssueDate.Size = new System.Drawing.Size(150, 20);
            this.dtIssueDate.TabIndex = 9;
            // 
            // lblExpireDate
            // 
            this.lblExpireDate.Location = new System.Drawing.Point(12, 320);
            this.lblExpireDate.Name = "lblExpireDate";
            this.lblExpireDate.Size = new System.Drawing.Size(63, 13);
            this.lblExpireDate.TabIndex = 10;
            this.lblExpireDate.Text = "Expire Date :";
            // 
            // dtExpireDate
            // 
            this.dtExpireDate.EditValue = new System.DateTime(2025, 11, 24, 0, 0, 0, 0);
            this.dtExpireDate.Location = new System.Drawing.Point(100, 318);
            this.dtExpireDate.Name = "dtExpireDate";
            this.dtExpireDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpireDate.Size = new System.Drawing.Size(150, 20);
            this.dtExpireDate.TabIndex = 11;
            // 
            // lblRemark
            // 
            this.lblRemark.Location = new System.Drawing.Point(12, 352);
            this.lblRemark.Name = "lblRemark";
            this.lblRemark.Size = new System.Drawing.Size(43, 13);
            this.lblRemark.TabIndex = 12;
            this.lblRemark.Text = "Remark :";
            // 
            // memRemark
            // 
            this.memRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memRemark.Location = new System.Drawing.Point(100, 350);
            this.memRemark.Name = "memRemark";
            this.memRemark.Size = new System.Drawing.Size(921, 80);
            this.memRemark.TabIndex = 13;
            // 
            // lblLicenseKey
            // 
            this.lblLicenseKey.Location = new System.Drawing.Point(12, 444);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(63, 13);
            this.lblLicenseKey.TabIndex = 14;
            this.lblLicenseKey.Text = "License Key :";
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseKey.Location = new System.Drawing.Point(100, 442);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Properties.ReadOnly = true;
            this.txtLicenseKey.Size = new System.Drawing.Size(741, 20);
            this.txtLicenseKey.TabIndex = 15;
            // 
            // btnGenerateKey
            // 
            this.btnGenerateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateKey.Location = new System.Drawing.Point(851, 438);
            this.btnGenerateKey.Name = "btnGenerateKey";
            this.btnGenerateKey.Size = new System.Drawing.Size(160, 30);
            this.btnGenerateKey.TabIndex = 16;
            this.btnGenerateKey.Text = "Generate License Key";
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(1023, 438);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(80, 30);
            this.btnPreview.TabIndex = 17;
            this.btnPreview.Text = "Preview";
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Location = new System.Drawing.Point(1115, 438);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(160, 30);
            this.btnDownload.TabIndex = 18;
            this.btnDownload.Text = "Download License";
            // 
            // GenerateLicensePage
            // 
            this.Controls.Add(this.btnNav_GenerateLicense);
            this.Controls.Add(this.btnNav_LicenseRecords);
            this.Controls.Add(this.btnNav_ManageProduct);
            this.Controls.Add(this.btnNav_ManageUser);
            this.Controls.Add(this.btnUploadArl);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.grpTypes);
            this.Controls.Add(this.grpModules);
            this.Controls.Add(this.lblIssueDate);
            this.Controls.Add(this.dtIssueDate);
            this.Controls.Add(this.lblExpireDate);
            this.Controls.Add(this.dtExpireDate);
            this.Controls.Add(this.lblRemark);
            this.Controls.Add(this.memRemark);
            this.Controls.Add(this.lblLicenseKey);
            this.Controls.Add(this.txtLicenseKey);
            this.Controls.Add(this.btnGenerateKey);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnDownload);
            this.Name = "GenerateLicensePage";
            this.Size = new System.Drawing.Size(1189, 580);
            ((System.ComponentModel.ISupportInitialize)(this.grpInfo)).EndInit();
            this.grpInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTypes)).EndInit();
            this.grpTypes.ResumeLayout(false);
            this.grpTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgLicenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubscriptionMonths.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpModules)).EndInit();
            this.grpModules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkModules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIssueDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpireDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memRemark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenseKey.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
