/*
PAGE: LicenseRecordsPage.cs
ROLE: Dealer Admin / Support
PURPOSE:
  Master view and search/filter UI for previously generated license records. Supports filtering, color-coded status display, and actions: View, Edit, Delete, Create.

KEY UI ELEMENTS:
  - Filters section: CompanyName (dropdown), ProductCode (textbox/dropdown), LicenseType (dropdown), IssueDate and ExpiryDate (DateEdit or range), checkbox "Show expired license", Countdown(days) selector
  - Buttons: Refresh, Create, View, Edit, Delete
  - GridControl: columns: CompanyName, ProductCode, ProductName, LicenseType, IssueDate, ExpiryDate, Status, Countdown
  - Row styling: Expired rows red, Soon-to-expire yellow, Active neutral
  - Pagination or virtual grid for large datasets (if applicable)

BACKEND SERVICE CALLS:
  - On Refresh: ServiceRegistry.Database.GetLicenses(filter)
  - On Create: navigate to GenerateLicensePage (optionally prefill)
  - On View: open LicenseRecordsDetailsPage with selected record id
  - On Edit: navigate to GenerateLicensePage prefilled (Admin only)
  - On Delete: mark license as deleted or update status via ServiceRegistry.Database.UpdateLicenseStatus(licenseId, status) (confirmation dialog required)

VALIDATION & RULES:
  - Filters validated; date ranges must be valid
  - Delete protected: require confirmation dialog and Admin rights
  - Deleting does not alter any customer-side .ASL files (just local DB)

ACCESS CONTROL:
  - Edit/Delete/Create buttons enabled only for users with rights from IUserService

UX NOTES:
  - Grid should be sortable and support double-click to open details
  - Provide count summary and export CSV option (optional)
  - Show inline counts for filtered rows

ACCEPTANCE CRITERIA:
  - Filters work, grid refreshes with correct data and coloring
  - Actions call the correct services; View/Edit opens expected pages

COPILOT PROMPTS:
  - "// Implement RefreshButton_Click to call ServiceRegistry.Database.GetLicenses(filter) and bind grid."
  - "// Implement DeleteButton_Click to confirm and call ServiceRegistry.Database.UpdateLicenseStatus(licenseId, \"Deleted\") (or ServiceRegistry.Database.DeleteLicense(licenseId) if your DB impl exposes it) for Admins only."
*/
