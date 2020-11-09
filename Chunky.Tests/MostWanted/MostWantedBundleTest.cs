using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var loadOptions = new BundleLoadOptions();
            loadOptions.AddChunkMapping<MostWantedShaderResource, MostWantedShaderResourceReader>(0x135200);

            _bundle = Bundle.FromStream(stream, loadOptions);
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
            bundle.WriteToStream(stream);
        }

        [TestMethod]
        public void TestReWrite()
        {
            GenericAlignmentHelper.SetAlignment(0xB3300000, 0x80);

            using Stream stream = File.Open(@"rewrite-test.bun", FileMode.Create, FileAccess.Write);
            _bundle.WriteToStream(stream);
        }

        [TestMethod]
        public void TestDuplicateResourceBugFix()
        {
            using Stream stream = File.OpenRead(@"test-data\mw\STREAML2RA_154.BUN");
            var bundle = Bundle.FromStream(stream, new BundleLoadOptions());
            Assert.AreEqual(1, bundle.FindResourcesByType<GenericResource>().Count(r => r.ChunkId == 0xB3300000));
        }
    }
}