using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chunky.Tests.World
{
    [TestClass]
    public class WorldBundleTest
    {
        [TestMethod]
        public void TestBundleWithCompressInPlace()
        {
            using Stream stream = File.OpenRead(@"test-data\world\GEOMETRY.BIN");
            var bundle = Bundle.FromStream(stream);
            Assert.AreEqual(bundle.Count(), 1);
        }
    }
}