/*
PAGE: ManageProductPage.cs
ROLE: Dealer Admin (Product Manager)
PURPOSE:
  CRUD management of products and their modules so GenerateLicensePage can reference product metadata (modules list, descriptions, release notes).

KEY UI ELEMENTS:
  - GridControl: list of products (ProductID, ProductName, CreatedBy, DateCreated, LastModified)
  - Buttons: Create, View, Edit, Delete, Search
  - Product form (for Create/Edit): ProductID, ProductName, CreatedBy (auto), DateCreated, LastModified, Modules grid (Name, Description), Description, Release Notes
  - Buttons in product form: Save, Cancel

BACKEND SERVICE CALLS:
  - ServiceRegistry.Database.GetProducts(), GetProductById(id)/GetProductByProductId(productId), InsertProduct(product), UpdateProduct(product), DeleteProduct(id)

VALIDATION & RULES:
  - ProductID uniqueness enforced
  - Module names non-empty
  - Only Admin role or product manager role can Create/Edit/Delete
  - When deleting a product, warn that existing licenses may reference this ProductID (deletion allowed but recommended to avoid)

ACCESS CONTROL:
  - ManageProduct page only visible to users with ManageProduct permission

UX NOTES:
  - Use inline validation for required fields
  - Modules grid supports add/remove rows easily (simple UI)

ACCEPTANCE CRITERIA:
  - Products persist to DB and appear in GenerateLicensePage module dropdown on refresh.
  - Module changes reflect in Generate license modules list.

COPILOT PROMPTS:
  - "// Implement LoadProducts to call ServiceRegistry.Database.GetProducts() and bind grid."
  - "// Implement SaveProduct to call ServiceRegistry.Database.InsertProduct(product) for new or ServiceRegistry.Database.UpdateProduct(product) for updates after validation."
*/
