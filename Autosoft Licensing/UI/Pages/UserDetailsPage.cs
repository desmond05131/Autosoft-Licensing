/*
PAGE: UserDetailsPage.cs
ROLE: Dealer Admin
PURPOSE:
  Create or edit a single user account and assign access rights for Dealer app capabilities.

KEY UI ELEMENTS:
  - TextBox: Username (readonly for edit), DisplayName
  - Password and Confirm Password fields (for create or change)
  - Checkbox list: Access rights (Generate License, License record, Manage Product, Manage User)
  - Buttons: Save, Cancel

BACKEND SERVICE CALLS:
  - On load (edit): ServiceRegistry.Database.GetUserById(id) or ServiceRegistry.Database.GetUserByUsername(username)
  - On Save: ServiceRegistry.Database.InsertUser(user) or ServiceRegistry.Database.UpdateUser(user)

VALIDATION & RULES:
  - Require non-empty username and displayname
  - Password required for new users; for updates optional unless changing password
  - Only Admin can assign ManageUser right
  - Prevent deleting or demoting the default Admin (this logic enforced in ManageUserPage Delete; here just prevent editing default admin to remove all admin roles)

ACCESS CONTROL:
  - Only users with ManageUser permission can use/save this form

UX NOTES:
  - If password fields left blank on edit, keep existing password
  - Show message on success (UI transient toast)

ACCEPTANCE CRITERIA:
  - New users created in DB and appear on ManageUserPage
  - Edits persist and role changes reflected in app permissions

COPILOT PROMPTS:
  - "// Implement SaveUser -> validate fields, optionally hash password, and call ServiceRegistry.Database.InsertUser(user) for new users or ServiceRegistry.Database.UpdateUser(user) for updates."
*/
