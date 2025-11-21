/*
PAGE: LicenseRecordsDetailsPage.cs
ROLE: Dealer Admin
PURPOSE:
  Show detailed information for a single license record selected from LicenseRecordsPage. Allow admin to toggle Edit mode to adjust expiry or module flags,
  then save changes to the dealer database. Provide navigation back to LicenseRecordsPage.

KEY UI ELEMENTS:
  - Labels/TextEdits (read-only initially) for: CompanyName, ProductName, ProductCode, IssueDate, ExpiryDate, Status, GeneratedBy, GeneratedOn
  - GridControl: Modules with Description and enabled flags
  - TextArea: Remarks
  - Buttons: Edit, Save, Cancel, Back
  - Visuals: If license is expired, show a banner "License expired." (use exact string) and for demo show "Demo license expired."

BACKEND SERVICE CALLS:
  - On load: ServiceRegistry.Database.GetLicenseById(id)
  - On Save: update license metadata using available database methods such as ServiceRegistry.Database.UpdateLicenseStatus(licenseId, status) and ServiceRegistry.Database.SetLicenseModules(licenseId, moduleCodes). If your DB layer exposes a full update method, call that (e.g., UpdateLicense or equivalent).

VALIDATION & RULES:
  - ExpiryDate must be >= IssueDate
  - If license is Demo and attempted expiry extension violates Demo rule in this business instance, allow only planned manual override (follow instructor rule: admin can adjust for Paid; ensure UI warns if adjusting Demo)
  - Show the exact messages for expiry/tamper condition if data indicates

ACCESS CONTROL:
  - Only Admin role can Save changes; otherwise fields read-only.

UX NOTES:
  - Save should be transactional; show success toast or dialog on success.
  - Consider auditing text for admin to record reason for manual change (even if not stored, prompt admin to include remark).

ACCEPTANCE CRITERIA:
  - Page loads correct license details from DB.
  - Admin edits and saves legal changes; DB updated.
  - Proper messages displayed for expired/demo cases.

COPILOT PROMPTS:
  - "// Implement LoadLicenseDetails(id) => ServiceRegistry.Database.GetLicenseById(id)"
  - "// Implement SaveLicense_Click => Validate expiry and persist changes using ServiceRegistry.Database.SetLicenseModules(licenseId, moduleCodes) and ServiceRegistry.Database.UpdateLicenseStatus(licenseId, status) or other appropriate Database methods available for updating license metadata."
*/
