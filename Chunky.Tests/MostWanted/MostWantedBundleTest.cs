using System.Collections.Generic;
using System.IO;
using Chunky.IO;
using Chunky.Resources;
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

        [TestMethod]
        public void TestCustomWrite()
        {
            using Stream stream = File.Open(@"simple-test.bun", FileMode.Create, FileAccess.Write);
            var bundle = new Bundle(new List<IResource> {new MostWantedShaderResource {Name = "TESTING1234"}});
            using var chunkBundleWriter = new ChunkBundleWriter(bundle, stream);
            chunkBundleWriter.WriteResources();
        }

        [TestMethod]
        public void TestReWrite()
        {
            GenericAlignmentHelper.SetAlignment(0xB3300000, 0x80);

            using Stream stream = File.Open(@"rewrite-test.bun", FileMode.Create, FileAccess.Write);
            using var chunkBundleWriter = new ChunkBundleWriter(_bundle, stream);
            chunkBundleWriter.WriteResources();
        }
    }
}