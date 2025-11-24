using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autosoft_Licensing.Services.Impl;

namespace Autosoft_Licensing.Tools
{
    [TestClass]
    public class LicenseKeyGeneratorTests
    {
        [TestMethod]
        public void GenerateKey_FormatLooksCorrect_NoDb()
        {
            var gen = new LicenseKeyGenerator(database: null, maxAttempts: 3);

            var key = gen.GenerateKey("Acme Co", "SAMPLE-PRODUCT", seed: 42);

            Assert.IsFalse(string.IsNullOrWhiteSpace(key));
            // Expect at least one dash and multiple segments
            var parts = key.Split('-');
            Assert.IsTrue(parts.Length >= 6, "Expected prefix + 5 groups of hex (>=6 segments)");
            Assert.IsTrue(parts[0].Length >= 1 && parts[0].Length <= 4, "Prefix length should be 1..4");
            // Each subsequent segment should be 4 hex chars (we check length and hex chars)
            for (int i = 1; i < parts.Length; i++)
            {
                Assert.AreEqual(4, parts[i].Length, $"Segment {i} length must be 4");
                // all hex uppercase A-F0-9
                foreach (var ch in parts[i])
                {
                    Assert.IsTrue(Uri.IsHexDigit(ch), $"Character '{ch}' in segment {i} is not hex");
                }
            }
        }

        [TestMethod]
        public void GenerateKey_DifferentSeeds_ProduceDifferentKeys()
        {
            var gen = new LicenseKeyGenerator(null, 3);
            var k1 = gen.GenerateKey("Co", "PROD", seed: 1);
            var k2 = gen.GenerateKey("Co", "PROD", seed: 2);
            Assert.AreNotEqual(k1, k2);
        }
    }
}
