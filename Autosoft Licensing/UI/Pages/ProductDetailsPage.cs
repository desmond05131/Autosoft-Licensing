/*
PAGE: ProductDetailsPage.cs
ROLE: Dealer Admin
PURPOSE:
  View or edit a single product and its module listing. This page is opened from ManageProductPage or as part of editing before generating a license.

KEY UI ELEMENTS:
  - TextEdits for ProductID (read-only if editing), ProductName, CreatedBy, DateCreated, LastModified
  - Grid: Modules list with Name and Description
  - TextAreas: Description and Release Notes
  - Buttons: Save, Cancel, Back

BACKEND SERVICE CALLS:
  - On load: ServiceRegistry.Database.GetProductByProductId(productId) or ServiceRegistry.Database.GetProductById(id)
  - On save: ServiceRegistry.Database.InsertProduct(product) for new product or ServiceRegistry.Database.UpdateProduct(product) for updates

VALIDATION & RULES:
  - ProductID uniqueness (if creating)
  - At least one module recommended (not mandatory)
  - Only Admin/product-manager roles may save

UX NOTES:
  - Use consistent layout with ManageProductPage
  - Indicate if ProductID is referenced by existing licenses (optional alert)

ACCEPTANCE CRITERIA:
  - Save updates persist and reflect in ManageProductPage and GenerateLicensePage

COPILOT PROMPTS:
  - "// Implement LoadProductDetails(productId) -> ServiceRegistry.Database.GetProductByProductId(productId) (or GetProductById(id) if you have numeric id)"
  - "// Implement Save -> validate module list and call ServiceRegistry.Database.InsertProduct(product) for new or ServiceRegistry.Database.UpdateProduct(product) for existing products"
*/
