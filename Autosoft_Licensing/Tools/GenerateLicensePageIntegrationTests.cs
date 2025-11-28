using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autosoft_Licensing.UI.Pages;
using Autosoft_Licensing.Tests.Helpers;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class GenerateLicensePageIntegrationTests
    {
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
    }
}