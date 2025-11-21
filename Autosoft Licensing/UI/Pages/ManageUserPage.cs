/*
PAGE: ManageUserPage.cs
ROLE: Dealer Admin (super-admin)
PURPOSE:
  Manage Dealer EXE user accounts and their access rights to the Dealer app (Generate License, License Records, Manage Product, Manage User).

KEY UI ELEMENTS:
  - GridControl: list of users (Username, DisplayName, IsActive, Roles)
  - Buttons: Create, View, Edit, Delete
  - User form: Username, DisplayName, Password, ConfirmPassword, Role checkboxes (GenerateLicense, LicenseRecord, ManageProduct, ManageUser), IsActive checkbox
  - Notes area: "Admin account cannot be deleted" hint

BACKEND SERVICE CALLS:
  - ServiceRegistry.Database.GetUsers(), InsertUser(user), UpdateUser(user), DeleteUser(id)

VALIDATION & RULES:
  - Username unique
  - Cannot delete 'Admin' account (enforce in UI and service guard)
  - Password rules per internal policy (min length; for prototype may be simple)
  - At least one Admin must remain

ACCESS CONTROL:
  - Only Admin role may access ManageUser page
  - Users with ManageUser permission can edit certain flags; only highest admin can assign ManageUser rights

UX NOTES:
  - Show a purple note "Admin cannot delete User Admin" per wireframe
  - Confirm delete with modal; if attempting to delete Admin account show warning and block

ACCEPTANCE CRITERIA:
  - Create/Edit/Delete operate as expected with permission enforcement
  - Admin account protected from deletion

COPILOT PROMPTS:
  - "// Implement LoadUsers to call ServiceRegistry.Database.GetUsers() and bind grid"
  - "// Implement DeleteUser to check against default Admin and call ServiceRegistry.Database.DeleteUser(userId)"
*/
