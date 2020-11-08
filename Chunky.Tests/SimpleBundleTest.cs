using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chunky.Tests
{
    [TestClass]
    public class SimpleBundleTest
    {
        [TestMethod]
        public void TestRead()
        {
            using Stream stream = File.OpenRead(@"test-data\mw\GLOBALB.BUN");
            var bundle = Bundle.FromStream(stream);
            Assert.IsTrue(bundle.Any());
        }
    }
}