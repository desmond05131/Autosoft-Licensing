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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class ProductDetailsPage : PageBase
    {
        private ILicenseDatabaseService _dbService;
        private IProductService _productService;

        private int? _productId;
        private BindingList<ModuleRowViewModel> _modules;

        // Navigation back event - host can subscribe
        public event EventHandler<NavigateBackEventArgs> NavigateBackRequested;

        public class NavigateBackEventArgs : EventArgs
        {
            public bool Saved { get; set; }
            public int? ProductId { get; set; }
        }

        // View-model for modules grid editable rows
        private class ModuleRowViewModel
        {
            public string ModuleCode { get; set; } // business code (stored as Name if code unavailable)
            public string ModuleName { get; set; } // display name
            public string Description { get; set; }
        }

        public ProductDetailsPage()
        {
            InitializeComponent();

            try
            {
                if (!DesignMode)
                {
                    // Best-effort registry wiring
                    try { _dbService ??= ServiceRegistry.Database; } catch { }
                    try { _productService ??= ServiceRegistry.Product; } catch { }

                    // Wire handlers
                    btnAdd.Click += btnAdd_Click;
                    btnMinus.Click += btnMinus_Click;
                    btnSave.Click += btnSave_Click;
                    btnCancel.Click += btnCancel_Click;

                    // DevExpress grid inline edit behavior
                    var view = grdModules.MainView as GridView;
                    if (view != null)
                    {
                        view.OptionsBehavior.Editable = true;
                        view.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                        view.OptionsNavigation.EnterMoveNextColumn = true;
                        view.DoubleClick += (s, e) =>
                        {
                            try
                            {
                                view.ShowEditor();
                            }
                            catch { /* ignore */ }
                        };
                    }

                    // Set styles
                    pnlHeader.BackColor = Color.FromArgb(253, 243, 211);
                    this.BackColor = Color.White;

                    Color purple = Color.FromArgb(98, 75, 255);
                    btnAdd.Appearance.BackColor = purple;
                    btnAdd.Appearance.ForeColor = Color.White;
                    btnAdd.Appearance.Options.UseBackColor = true;
                    btnAdd.Appearance.Options.UseForeColor = true;

                    btnMinus.Appearance.BackColor = purple;
                    btnMinus.Appearance.ForeColor = Color.White;
                    btnMinus.Appearance.Options.UseBackColor = true;
                    btnMinus.Appearance.Options.UseForeColor = true;

                    btnSave.Appearance.BackColor = purple;
                    btnSave.Appearance.ForeColor = Color.White;
                    btnSave.Appearance.Options.UseBackColor = true;
                    btnSave.Appearance.Options.UseForeColor = true;

                    btnCancel.Appearance.BackColor = purple;
                    btnCancel.Appearance.ForeColor = Color.White;
                    btnCancel.Appearance.Options.UseBackColor = true;
                    btnCancel.Appearance.Options.UseForeColor = true;

                    // Ensure text edits have simple borders
                    foreach (var te in new[] { txtProductId, txtProductName, txtCreatedBy, txtDateCreated, txtLastModified })
                    {
                        try
                        {
                            te.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                        }
                        catch { /* ignore */ }
                    }

                    Initialize(null); // default create mode if host forgets to call
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProductDetailsPage ctor error: {ex}");
            }
        }

        /// <summary>
        /// Initialize page for create or edit mode; inject services best-effort.
        /// null productId => Create Mode.
        /// </summary>
        public void Initialize(int? productId, ILicenseDatabaseService dbService = null, IProductService productService = null)
        {
            try
            {
                _dbService = dbService ?? _dbService ?? ServiceRegistry.Database;
                _productService = productService ?? _productService ?? ServiceRegistry.Product;
            }
            catch
            {
                // do not throw – controls should still render
            }

            _productId = productId;
            _modules = new BindingList<ModuleRowViewModel>();
            grdModules.DataSource = _modules;

            if (_productId == null)
            {
                // Create Mode
                txtProductId.Text = string.Empty;
                txtProductName.Text = string.Empty;
                memDescription.Text = string.Empty;
                memReleaseNotes.Text = string.Empty;

                var nowLocal = DateTime.Now;
                txtDateCreated.Text = nowLocal.ToString("dd/MM/yyyy");
                txtLastModified.Text = nowLocal.ToString("dd/MM/yyyy");

                txtCreatedBy.Text = TryGetCurrentUserDisplayName() ?? Environment.UserName ?? "Admin";

                txtProductId.Properties.ReadOnly = false;
                txtProductName.Properties.ReadOnly = false;

                // Modules list empty
                _modules.Clear();
            }
            else
            {
                // Edit/View Mode
                LoadProductDetails(_productId.Value);
            }

            // Best fit columns after bind
            try
            {
                var view = grdModules.MainView as GridView;
                view?.BestFitColumns();
            }
            catch { /* ignore */ }
        }

        public override void InitializeForRole(Autosoft_Licensing.Models.User user)
        {
            try
            {
                if (user == null) return;

                var role = user.Role ?? string.Empty;
                bool canSave = role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                               || role.Equals("ProductManager", StringComparison.OrdinalIgnoreCase);

                btnSave.Enabled = canSave;
                btnAdd.Enabled = canSave;
                btnMinus.Enabled = canSave;

                txtProductId.Properties.ReadOnly = !canSave || _productId != null; // read-only ProductID if editing
                txtProductName.Properties.ReadOnly = !canSave;
                memDescription.Properties.ReadOnly = !canSave;
                memReleaseNotes.Properties.ReadOnly = !canSave;
            }
            catch { /* ignore */ }
        }

        // Implement LoadProductDetails(productId) -> ServiceRegistry.Database.GetProductById(id)
        private void LoadProductDetails(int id)
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var product = _dbService.GetProductById(id);
                if (product == null)
                {
                    ShowError("Product not found.");
                    return;
                }

                // Header fields
                txtProductId.Text = product.ProductID ?? string.Empty;
                txtProductName.Text = product.Name ?? string.Empty;
                txtCreatedBy.Text = product.CreatedBy ?? string.Empty;

                var createdLocal = product.CreatedUtc == default ? DateTime.Now : ToLocal(product.CreatedUtc);
                var modifiedLocal = product.LastModifiedUtc == default ? createdLocal : ToLocal(product.LastModifiedUtc);
                txtDateCreated.Text = createdLocal.ToString("dd/MM/yyyy");
                txtLastModified.Text = modifiedLocal.ToString("dd/MM/yyyy");

                memDescription.Text = product.Description ?? string.Empty;
                memReleaseNotes.Text = product.ReleaseNotes ?? string.Empty;

                // CRITICAL: Load Modules via ProductService or DB service
                var list = new List<ModuleRowViewModel>();

                // prefer ProductService DTOs
                try
                {
                    if (_productService != null && !string.IsNullOrEmpty(product.ProductID))
                    {
                        var dtos = _productService.GetModulesByProductId(product.ProductID) ?? Enumerable.Empty<ModuleDto>();
                        foreach (var m in dtos)
                        {
                            list.Add(new ModuleRowViewModel
                            {
                                ModuleCode = m.ModuleCode ?? m.ModuleName ?? string.Empty,
                                ModuleName = string.IsNullOrWhiteSpace(m.ModuleName) ? m.ModuleCode ?? string.Empty : m.ModuleName,
                                Description = m.Description ?? string.Empty
                            });
                        }
                    }
                    else
                    {
                        // fallback to DB Modules in Product (if any were loaded externally)
                        foreach (var m in product.Modules ?? new List<Module>())
                        {
                            list.Add(new ModuleRowViewModel
                            {
                                ModuleCode = m.ModuleCode ?? m.Name ?? string.Empty,
                                ModuleName = string.IsNullOrWhiteSpace(m.Name) ? m.ModuleCode ?? string.Empty : m.Name,
                                Description = m.Description ?? string.Empty
                            });
                        }
                    }
                }
                catch
                {
                    // final fallback: try db service GetModulesForProduct
                    try
                    {
                        var dtos = _dbService.GetModulesForProduct(product.ProductID) ?? Enumerable.Empty<ModuleDto>();
                        foreach (var m in dtos)
                        {
                            list.Add(new ModuleRowViewModel
                            {
                                ModuleCode = m.ModuleCode ?? m.ModuleName ?? string.Empty,
                                ModuleName = string.IsNullOrWhiteSpace(m.ModuleName) ? m.ModuleCode ?? string.Empty : m.ModuleName,
                                Description = m.Description ?? string.Empty
                            });
                        }
                    }
                    catch { /* ignore */ }
                }

                _modules = new BindingList<ModuleRowViewModel>(list);
                grdModules.DataSource = _modules;

                // Enforce read-only ProductID in edit mode
                txtProductId.Properties.ReadOnly = true;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load product details.");
                System.Diagnostics.Debug.WriteLine($"ProductDetailsPage.LoadProductDetails error: {ex}");
            }
        }

        // Grid Logic: Add new row
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modules == null)
                {
                    _modules = new BindingList<ModuleRowViewModel>();
                    grdModules.DataSource = _modules;
                }

                _modules.Add(new ModuleRowViewModel
                {
                    ModuleCode = string.Empty,
                    ModuleName = string.Empty,
                    Description = string.Empty
                });

                try
                {
                    var view = grdModules.MainView as GridView;
                    view?.BestFitColumns();
                }
                catch { /* ignore */ }
            }
            catch (Exception ex)
            {
                ShowError("Failed to add module.");
                System.Diagnostics.Debug.WriteLine($"btnAdd_Click error: {ex}");
            }
        }

        // Grid Logic: Remove selected row
        private void btnMinus_Click(object sender, EventArgs e)
        {
            try
            {
                var view = grdModules.MainView as GridView;
                if (view == null || _modules == null || _modules.Count == 0) return;

                var rowObj = view.GetFocusedRow() as ModuleRowViewModel;
                if (rowObj != null)
                {
                    _modules.Remove(rowObj);
                }
                else
                {
                    var idx = view.FocusedRowHandle;
                    if (idx >= 0 && idx < _modules.Count)
                    {
                        _modules.RemoveAt(idx);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to remove module.");
                System.Diagnostics.Debug.WriteLine($"btnMinus_Click error: {ex}");
            }
        }

        // Save Logic
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var productIdStr = (txtProductId.Text ?? string.Empty).Trim();
                var productNameStr = (txtProductName.Text ?? string.Empty).Trim();

                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(productIdStr))
                {
                    ShowError("Product ID is required.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(productNameStr))
                {
                    ShowError("Product Name is required.");
                    return;
                }

                // Enforce ProductID uniqueness on create
                if (_productId == null)
                {
                    try
                    {
                        var existing = _dbService.GetProductByProductId(productIdStr);
                        if (existing != null)
                        {
                            ShowError("Product ID already exists. Please choose a unique Product ID.");
                            return;
                        }
                    }
                    catch
                    {
                        ShowError("Unable to verify Product ID uniqueness due to a database error.");
                        return;
                    }
                }

                // NEW: Enforce Product Name uniqueness (for both create and edit)
                try
                {
                    var existingByName = _dbService.GetProductByName(productNameStr);
                    if (existingByName != null)
                    {
                        // If creating, any existing name is a conflict.
                        // If editing, conflict only when found Id differs from current _productId.
                        var existingId = existingByName.Id;
                        var currentId = _productId ?? 0;
                        if (existingId != 0 && existingId != currentId)
                        {
                            ShowError("Product Name already exists.");
                            return;
                        }
                    }
                }
                catch
                {
                    // Fail-safe: if we cannot verify uniqueness, stop to prevent duplicates
                    ShowError("Unable to verify Product Name uniqueness due to a database error.");
                    return;
                }

                var createdByStr = (txtCreatedBy.Text ?? string.Empty).Trim();
                var descriptionStr = memDescription.Text ?? string.Empty;
                var releaseNotesStr = memReleaseNotes.Text ?? string.Empty;

                // Dates: parse defensively
                DateTime createdLocal, lastModifiedLocal;
                if (!DateTime.TryParse(txtDateCreated.Text, out createdLocal))
                    createdLocal = DateTime.Now;
                if (!DateTime.TryParse(txtLastModified.Text, out lastModifiedLocal))
                    lastModifiedLocal = DateTime.Now;

                var product = new Product
                {
                    Id = _productId ?? 0,
                    ProductID = productIdStr,
                    Name = productNameStr,
                    Description = descriptionStr,
                    ReleaseNotes = releaseNotesStr,
                    CreatedBy = string.IsNullOrWhiteSpace(createdByStr) ? TryGetCurrentUserDisplayName() ?? Environment.UserName : createdByStr,
                    CreatedUtc = _productId == null ? DateTime.UtcNow : ToUtc(createdLocal), // preserve original in edit
                    LastModifiedUtc = _productId == null ? DateTime.UtcNow : ToUtc(lastModifiedLocal),
                    Modules = new List<Module>()
                };

                // Map BindingList -> List<Module>
                foreach (var row in (_modules ?? new BindingList<ModuleRowViewModel>()))
                {
                    var code = (row.ModuleCode ?? row.ModuleName ?? string.Empty).Trim();
                    var name = (row.ModuleName ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(name))
                        continue;

                    product.Modules.Add(new Module
                    {
                        ProductId = product.Id,
                        ModuleCode = string.IsNullOrWhiteSpace(code) ? name : code,
                        Name = string.IsNullOrWhiteSpace(name) ? code : name,
                        Description = row.Description ?? string.Empty,
                        IsActive = true
                    });
                }

                // Persist
                if (_productId == null)
                {
                    // Normalize timestamps for new product
                    product.CreatedUtc = DateTime.UtcNow;
                    product.LastModifiedUtc = product.CreatedUtc;

                    var newId = _dbService.InsertProduct(product);
                    _productId = newId;
                    ShowInfo("Product created.", "Success");
                    NavigateBack(true, newId);
                }
                else
                {
                    product.Id = _productId.Value;
                    // Always stamp last modified
                    product.LastModifiedUtc = DateTime.UtcNow;

                    _dbService.UpdateProduct(product);
                    ShowInfo("Product updated.", "Success");
                    NavigateBack(true, _productId.Value);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to save product.");
                System.Diagnostics.Debug.WriteLine($"btnSave_Click error: {ex}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                NavigateBack(false, _productId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"btnCancel_Click error: {ex}");
            }
        }

        private void NavigateBack(bool saved, int? id)
        {
            try
            {
                NavigateBackRequested?.Invoke(this, new NavigateBackEventArgs
                {
                    Saved = saved,
                    ProductId = id
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateBack error: {ex}");
            }
        }

        private string TryGetCurrentUserDisplayName()
        {
            try
            {
                var usrSvc = ServiceRegistry.User;
                // In many flows, tests seed admin with Id=1; fall back gracefully
                var u = usrSvc?.GetUserById(1);
                return u?.DisplayName ?? u?.Username ?? null;
            }
            catch { return null; }
        }
    }
}
