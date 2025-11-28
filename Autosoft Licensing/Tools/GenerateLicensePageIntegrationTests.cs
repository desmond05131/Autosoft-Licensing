using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DevExpress.XtraEditors;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Tests.Helpers;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class GenerateLicensePageIntegrationTests
    {
        // Plan (pseudocode):
        // 1. Add a small UI-hosted test that uses the existing UiTestHost helper.
        // 2. Inside using(UiTestHost) create the page on the UI thread via host.Invoke.
        // 3. Show the control using host.ShowControl so the page creates its Win32 handle on the UI thread.
        // 4. Query page.IsHandleCreated via host.Invoke and Assert.IsTrue to ensure no Win32Exception occurred.
        // 5. Keep existing RunWithFormHost helper and previous integration tests unchanged.

        [TestMethod]
        public void GenerateLicensePage_HandleCreated_WhenHostedOnUiThread()
        {
            using (var host = new UiTestHost("GeneratePageTestHost"))
            {
                GenerateLicensePage page = null;

                // Create and show the page on the UI thread to avoid Win32 handle creation on the test thread.
                host.Invoke(() =>
                {
                    page = new GenerateLicensePage();
                    host.ShowControl(page);
                });

                // Verify handle was created on the UI thread and no Win32Exception was thrown.
                bool handleCreated = host.Invoke(() => page.IsHandleCreated);
                Assert.IsTrue(handleCreated, "Expected page handle to be created when hosted on the UI thread.");
            }
        }

        // Run an action that manipulates the GenerateLicensePage inside a real UI thread/form.
        // The action runs on the UI thread via Control.Invoke.
        private void RunWithFormHost(Action<GenerateLicensePage, Form> uiAction, int timeoutMs = 8000)
        {
            var ready = new ManualResetEventSlim();
            var finished = new ManualResetEventSlim();
            GenerateLicensePage page = null;
            Form host = null;

            Thread uiThread = new Thread(() =>
            {
                Application.EnableVisualStyles();

                host = new Form { Width = 1200, Height = 800, StartPosition = FormStartPosition.Manual };
                page = new GenerateLicensePage { Dock = DockStyle.Fill };
                host.Controls.Add(page);

                // FORCE handle creation on the UI thread BEFORE signalling readiness.
                // This prevents the test thread from causing the handle to be created on the wrong thread
                // when it later calls host.Invoke(...) — the root cause of intermittent Win32Exception.
                try
                {
                    var _ = host.Handle;  // ensure host handle made on UI thread
                    var __ = page.Handle; // ensure child control handle also created on UI thread
                }
                catch
                {
                    // ignore - we'll still signal ready so tests can run and surface real failures
                }

                // signal that UI created (and handle exists)
                ready.Set();

                try
                {
                    Application.Run(host);
                }
                finally
                {
                    finished.Set();
                }
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.IsBackground = true;
            uiThread.Start();

            if (!ready.Wait(timeoutMs))
                throw new TimeoutException("UI host did not initialize in time.");

            try
            {
                // Execute the supplied action on the UI thread
                host.Invoke(new Action(() => uiAction(page, host)));
            }
            finally
            {
                // Close the form and wait for thread to finish
                if (host != null && !host.IsDisposed)
                {
                    try
                    {
                        host.Invoke(new Action(() =>
                        {
                            if (!host.IsDisposed) host.Close();
                        }));
                    }
                    catch { /* ignore */ }
                }

                // Wait for ui thread to exit
                if (!finished.Wait(timeoutMs))
                {
                    // Best-effort abort (rare). Do nothing further.
                }
            }
        }

        [TestMethod]
        public void UploadArl_PopulatesFields_BindsModules_InHost()
        {
            // Arrange - prepare mocks
            var mockArlReader = new Mock<IArlReaderService>();
            var mockAsl = new Mock<IAslGeneratorService>();
            var mockProduct = new Mock<IProductService>();
            var mockDb = new Mock<ILicenseDatabaseService>();
            var mockUser = new Mock<IUserService>();

            var arl = new ArlRequest
            {
                CompanyName = "Acme Corp",
                ProductID = "PROD-001",
                ProductName = "Acme Product",
                DealerCode = "DLR01",
                RequestedPeriodMonths = 3,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "MOD_A", "MOD_B" }
            };

            mockArlReader.Setup(x => x.ParseArl(It.IsAny<string>())).Returns(arl);

            var modules = new[]
            {
                new ModuleDto { ModuleCode = "MOD_A", ModuleName = "Module A" },
                new ModuleDto { ModuleCode = "MOD_B", ModuleName = "Module B" },
            };

            mockProduct.Setup(x => x.GetModulesByProductId(arl.ProductID)).Returns(modules);
            mockProduct.Setup(x => x.GetProductName(arl.ProductID)).Returns(arl.ProductName);

            // Act & Assert inside UI host
            RunWithFormHost((page, host) =>
            {
                // Inject mocks
                page.Initialize(mockArlReader.Object, mockAsl.Object, mockProduct.Object, mockDb.Object, mockUser.Object);

                // Simulate post-parse flow (the real handler shows OpenFileDialog so we set state directly)
                // Set private _currentRequest
                var fiCurrentRequest = typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic);
                fiCurrentRequest.SetValue(page, arl);

                // Set UI controls like the upload handler would
                var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var txtProductName = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

                txtCompany.Text = arl.CompanyName;
                txtProductId.Text = arl.ProductID;
                txtProductName.Text = mockProduct.Object.GetProductName(arl.ProductID);

                // Call private BindModules to bind grid
                var bindMethod = typeof(GenerateLicensePage).GetMethod("BindModules", BindingFlags.Instance | BindingFlags.NonPublic);
                bindMethod.Invoke(page, new object[] { modules, arl.ModuleCodes });

                // Set dates and enable generation as the real handler does
                var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                dtIssue.DateTime = DateTime.UtcNow.Date;
                dtExpire.DateTime = dtIssue.DateTime.AddMonths(Math.Max(1, arl.RequestedPeriodMonths));

                var btnGenerate = (SimpleButton)typeof(GenerateLicensePage).GetField("btnGenerateKey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var btnPreview = (SimpleButton)typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var btnDownload = (SimpleButton)typeof(GenerateLicensePage).GetField("btnDownload", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

                btnGenerate.Enabled = true;
                btnPreview.Enabled = false;
                btnDownload.Enabled = false;

                // Now assert UI fields reflect ARL
                Assert.AreEqual("Acme Corp", txtCompany.Text);
                Assert.AreEqual("PROD-001", txtProductId.Text);
                Assert.AreEqual("Acme Product", txtProductName.Text);

                // Grid rows
                var grdModules = (DevExpress.XtraGrid.GridControl)typeof(GenerateLicensePage).GetField("grdModules", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var view = grdModules.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                Assert.IsNotNull(view);
                Assert.AreEqual(2, view.DataRowCount);

                // Validate module names and enabled flags
                var names = new System.Collections.Generic.List<string>();
                var enabledFlags = new System.Collections.Generic.List<bool>();
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    var row = view.GetRow(i);
                    var propName = row.GetType().GetProperty("ModuleName");
                    var propEnabled = row.GetType().GetProperty("Enabled");
                    names.Add(propName.GetValue(row)?.ToString());
                    enabledFlags.Add((bool)(propEnabled.GetValue(row) ?? false));
                }

                CollectionAssert.AreEquivalent(new[] { "Module A", "Module B" }, names);
                CollectionAssert.AreEqual(new[] { true, true }, enabledFlags);
            });
        }

        [TestMethod]
        public void GenerateKey_WhenNoDuplicate_GeneratesKey_AndEnablesPreviewDownload_InHost()
        {
            // Arrange - mocks
            var mockArlReader = new Mock<IArlReaderService>();
            var mockAsl = new Mock<IAslGeneratorService>();
            var mockProduct = new Mock<IProductService>();
            var mockDb = new Mock<ILicenseDatabaseService>();
            var mockUser = new Mock<IUserService>();

            // Ensure duplicate check returns false
            mockDb.Setup(d => d.ExistsDuplicateLicense(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(false);

            var arl = new ArlRequest
            {
                CompanyName = "GoodCo",
                ProductID = "GOOD-01",
                ProductName = "Good Product",
                DealerCode = "DLR-GOOD",
                RequestedPeriodMonths = 12,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "M1", "M2" }
            };

            // Act & Assert inside UI host
            RunWithFormHost((page, host) =>
            {
                page.Initialize(mockArlReader.Object, mockAsl.Object, mockProduct.Object, mockDb.Object, mockUser.Object);

                // Prime page state like upload would
                typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(page, arl);

                var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                txtCompany.Text = arl.CompanyName;
                txtProductId.Text = arl.ProductID;

                var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                dtIssue.DateTime = DateTime.UtcNow.Date;
                dtExpire.DateTime = dtIssue.DateTime.AddMonths(12);

                var txtLicenseKey = (TextEdit)typeof(GenerateLicensePage).GetField("txtLicenseKey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                txtLicenseKey.Text = string.Empty;

                // Call btnGenerateKey_Click handler reflectively on UI thread
                var method = typeof(GenerateLicensePage).GetMethod("btnGenerateKey_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(page, new object[] { null, EventArgs.Empty });

                // Assert: license key generated and preview/download enabled
                Assert.IsFalse(string.IsNullOrWhiteSpace(txtLicenseKey.Text));
                Assert.AreEqual(32, txtLicenseKey.Text.Length, "Expected 32-char key produced by skeleton generator.");

                var btnPreview = (SimpleButton)typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var btnDownload = (SimpleButton)typeof(GenerateLicensePage).GetField("btnDownload", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);

                Assert.IsTrue(btnPreview.Enabled, "Preview should be enabled after key generation.");
                Assert.IsTrue(btnDownload.Enabled, "Download should be enabled after key generation.");

                // The skeleton's GenerateKey implementation does not call the ASL generator; ensure it wasn't called yet.
                mockAsl.Verify(x => x.CreateAndSaveAsl(It.IsAny<LicenseData>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Never);
            });
        }

        [TestMethod]
        public void PreviewButton_DoesNotThrow_ShowInfoSkeleton_InHost()
        {
            // Arrange - minimal mocks
            var mockArlReader = new Mock<IArlReaderService>();
            var mockAsl = new Mock<IAslGeneratorService>();
            var mockProduct = new Mock<IProductService>();
            var mockDb = new Mock<ILicenseDatabaseService>();
            var mockUser = new Mock<IUserService>();

            var arl = new ArlRequest
            {
                CompanyName = "PreviewCo",
                ProductID = "PRE-01",
                RequestedPeriodMonths = 1,
                RequestDateUtc = DateTime.UtcNow,
                ModuleCodes = new[] { "M1" }
            };

            RunWithFormHost((page, host) =>
            {
                page.Initialize(mockArlReader.Object, mockAsl.Object, mockProduct.Object, mockDb.Object, mockUser.Object);

                // Prime payload (as Generate step would)
                typeof(GenerateLicensePage).GetField("_currentRequest", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(page, arl);

                // Ensure required text fields are populated (btnGenerateKey checks these)
                var txtCompany = (TextEdit)typeof(GenerateLicensePage).GetField("txtCompanyName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var txtProductId = (TextEdit)typeof(GenerateLicensePage).GetField("txtProductId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                txtCompany.Text = arl.CompanyName;
                txtProductId.Text = arl.ProductID;

                var dtIssue = (DateEdit)typeof(GenerateLicensePage).GetField("dtIssueDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                var dtExpire = (DateEdit)typeof(GenerateLicensePage).GetField("dtExpireDate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                dtIssue.DateTime = DateTime.UtcNow.Date;
                dtExpire.DateTime = dtIssue.DateTime.AddMonths(1);

                // Create a payload as GenerateKey would
                var methodGen = typeof(GenerateLicensePage).GetMethod("btnGenerateKey_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                methodGen.Invoke(page, new object[] { null, EventArgs.Empty });

                // Now call Preview click handler - in skeleton this shows info message; ensure no exception
                var methodPreview = typeof(GenerateLicensePage).GetMethod("btnPreview_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                methodPreview.Invoke(page, new object[] { null, EventArgs.Empty });

                // No exception means pass. Optionally assert that btnPreview was enabled.
                var btnPreview = (SimpleButton)typeof(GenerateLicensePage).GetField("btnPreview", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
                Assert.IsTrue(btnPreview.Enabled);
            });
        }
    }
}