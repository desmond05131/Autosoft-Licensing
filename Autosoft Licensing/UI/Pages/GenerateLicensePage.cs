/*
PAGE: GenerateLicensePage.cs
ROLE: Dealer Admin (Dealer EXE)
PURPOSE:
  Primary license generation screen. Load a customer's .ARL file, display parsed fields, allow admin to adjust allowed fields
  (expiry, modules, license type where allowed), generate the license key/payload, preview the license payload, and export a .ASL file.

KEY UI ELEMENTS:
  - Button: Upload Request File (.ARL)
  - Read-only labels/textboxes: CompanyName, ProductID, DealerCode, CurrencyCode, RequestDateUtc
  - Dropdown / Radio: LicenseType (Demo / Subscription / Permanent)
  - Numeric / dropdown: RequestedPeriodMonths (auto 1 for Demo; for Subscription allow 3/6/12/24 etc.)
  - DateEdit: IssueDate (default Today), ExpireDate (auto-calc but editable for Admin where rules allow)
  - Checklist/Grid: Modules (list from Products metadata) with checkboxes
  - TextArea: Remarks
  - TextBox (readonly): LicenseKey (auto-generated)
  - Buttons: Generate License Key, Preview (opens PreviewLicense control), Download License (.ASL)
  - Validation area: inline error messages
  - Use DevExpress controls: SimpleButton, DateEdit, GridControl, MemoEdit, TextEdit, RadioGroup

BACKEND SERVICE CALLS (use your implemented services):
  - On Upload: read file and parse into LicenseRequest. Use ServiceRegistry.File.ReadBytes/ReadFileBase64 and JsonHelper.Deserialize<LicenseRequest>(json) or a dedicated parser if implemented.
  - On Generate License Key: use ServiceRegistry.Encryption.BuildJsonWithChecksum(licenseData) to get canonical JSON; use ServiceRegistry.License.GenerateAsl(licenseData, key, iv) to produce encrypted ASL (base64). For checksum/canonical JSON use Encryption service.
  - On Download: use ServiceRegistry.License.GenerateAsl(...) (to produce Base64) and ServiceRegistry.File.WriteFileBase64(path, base64); then call ServiceRegistry.Database.InsertLicense(metadata) to persist metadata if desired.
  - Use product metadata from ServiceRegistry.Database.GetProductByProductId(productId) / ServiceRegistry.Database.GetProducts() to populate Modules checklist

VALIDATION & BUSINESS RULES:
  - Must show the exact business error strings:
      - Missing ARL or required fields -> "Invalid license request file."
      - If user attempts illegal combination (e.g., Demo with >1 month) -> block and show inline message mentioning rule.
  - Demo license MUST be fixed to 1 month from IssueDate (UI should auto-set and disable period control for Demo)
  - Subscription default suggestions: 12 months; admin can choose other allowed values (3/6/12/24)
  - ExpireDate must be >= IssueDate

ACCESS CONTROL:
  - Only users with GenerateLicense right see/enable Generate/Download buttons (check current user permissions via ServiceRegistry.User or the provided user context)

UX NOTES:
  - After Upload, auto-fill fields and show preview button enabled.
  - Generating the license key should not auto-write to disk—only prepare payload; require explicit Download to save .ASL.
  - Use progress indicators for encryption writes (if file large/slow).
  - Provide a "Reset" or "Clear" action.

ACCEPTANCE CRITERIA:
  - Upload of valid .ARL populates UI fields.
  - Generate License Key returns a formatted LicenseKey and payload preview.
  - Download writes a Base64 .ASL file that can be decrypted by client DLL (roundtrip validated).
  - UI enforces Demo rule and shows the specified error strings when violations occur.

COPILOT PROMPTS (for inline use):
  - "// Implement UploadArlButton_Click: read the selected file (ServiceRegistry.File.ReadBytes or ReadFileBase64) and parse to a LicenseRequest using JsonHelper.Deserialize<LicenseRequest>(json) or your ARL parser. On parse failure show 'Invalid license request file.'"
  - "// Implement GenerateButton_Click: build canonical JSON with ServiceRegistry.Encryption.BuildJsonWithChecksum(licenseData) and/or call ServiceRegistry.License.GenerateAsl(licenseData, CryptoConstants.AesKey, CryptoConstants.AesIV) to prepare the Base64 ASL; display generated LicenseKey and enable Download button."
  - "// Implement DownloadButton_Click: open SaveFileDialog, call ServiceRegistry.License.GenerateAsl(licenseData, CryptoConstants.AesKey, CryptoConstants.AesIV) to obtain base64 ASL, call ServiceRegistry.File.WriteFileBase64(path, base64) to persist the file, then call ServiceRegistry.Database.InsertLicense(metadata) to record the generated license."
*/
