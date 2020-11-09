using System.IO;
using System.Runtime.InteropServices;
using Chunky.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chunky.Tests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SimpleStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string StringValue;

        public int IntValue;

        public float FloatValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ByteArray;
    }

    [TestClass]
    public class BinaryHelpersTest
    {
        private static readonly SimpleStructure BenchmarkStructure = new SimpleStructure
        {
            StringValue = "Hello World!",
            IntValue = 1337,
            FloatValue = 1337f,
            ByteArray = new byte[] {0x13, 0x37, 0x73, 0x31, 0x12, 0x34, 0x56, 0x78}
        };

        [TestMethod]
        public void TestBufferUnmarshal()
        {
            byte[] inData =
            {
                // StringValue
                0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64, 0x21, 0x00, 0x00, 0x00, 0x00,
                // IntValue
                0x39, 0x05, 0x00, 0x00,
                // FloatValue
                0x00, 0x20, 0xa7, 0x44,
                // ByteArray
                0x13, 0x37, 0x73, 0x31, 0x12, 0x34, 0x56, 0x78
            };

            var unmarshalledStructure = BinaryHelpers.BufferToStructure<SimpleStructure>(inData);

            Assert.AreEqual(BenchmarkStructure.StringValue, unmarshalledStructure.StringValue);
            Assert.AreEqual(BenchmarkStructure.IntValue, unmarshalledStructure.IntValue);
            Assert.AreEqual(BenchmarkStructure.FloatValue, unmarshalledStructure.FloatValue);
            CollectionAssert.AreEqual(BenchmarkStructure.ByteArray, unmarshalledStructure.ByteArray);
        }

        [TestMethod]
        public void TestBufferRoundTrip()
        {
            var marshalled = BinaryHelpers.MarshalStruct(BenchmarkStructure);
            var unmarshalledStructure = BinaryHelpers.BufferToStructure<SimpleStructure>(marshalled);

            Assert.AreEqual(BenchmarkStructure.StringValue, unmarshalledStructure.StringValue);
            Assert.AreEqual(BenchmarkStructure.IntValue, unmarshalledStructure.IntValue);
            Assert.AreEqual(BenchmarkStructure.FloatValue, unmarshalledStructure.FloatValue);
            CollectionAssert.AreEqual(BenchmarkStructure.ByteArray, unmarshalledStructure.ByteArray);
        }

        [TestMethod]
        public void TestStreamRoundTrip()
        {
            using var ms = new MemoryStream();
            BinaryHelpers.WriteStruct(ms, BenchmarkStructure);
            ms.Position = 0;

            Assert.AreEqual(ms.Length, Marshal.SizeOf<SimpleStructure>());

            var unmarshalledStructure = BinaryHelpers.ReadStruct<SimpleStructure>(ms);

            Assert.AreEqual(BenchmarkStructure.StringValue, unmarshalledStructure.StringValue);
            Assert.AreEqual(BenchmarkStructure.IntValue, unmarshalledStructure.IntValue);
            Assert.AreEqual(BenchmarkStructure.FloatValue, unmarshalledStructure.FloatValue);
            CollectionAssert.AreEqual(BenchmarkStructure.ByteArray, unmarshalledStructure.ByteArray);
        }
    }
}