using System.IO;
using System.Linq;
using Chunky.IO;
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
            var chunkBundleLoader = new ChunkBundleLoader();
            var bundle = chunkBundleLoader.LoadBundle(stream);
            Assert.IsTrue(bundle.Any());
        }
    }
}