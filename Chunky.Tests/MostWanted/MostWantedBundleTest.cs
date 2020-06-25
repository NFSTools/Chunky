using System.IO;
using Chunky.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chunky.Tests.MostWanted
{
    [TestClass]
    public class MostWantedBundleTest
    {
        private static Bundle _bundle;

        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            using Stream stream = File.OpenRead(@"test-data\mw\GLOBALB.BUN");
            var chunkBundleLoader = new ChunkBundleLoader();
            chunkBundleLoader.RegisterResource<MostWantedShaderResource, MostWantedShaderResourceReader>(0x135200);
            _bundle = chunkBundleLoader.LoadBundle(stream);
        }

        [TestMethod]
        public void TestShaderResource()
        {
            var shaderResource = _bundle.FindResourceByName<MostWantedShaderResource>("ALUMINUM");
            Assert.IsNotNull(shaderResource);
            Assert.AreEqual(shaderResource.Name, "ALUMINUM");
        }
    }
}