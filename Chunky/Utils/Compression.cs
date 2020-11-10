using System;
using System.Runtime.InteropServices;

namespace Chunky.Utils
{
    /// <summary>
    ///     Exposes functions to interface with NativeLibrary.dll
    /// </summary>
    public static class Compression
    {
        /// <summary>
        ///     Decompresses data from a compressed buffer.
        /// </summary>
        /// <param name="compressedData">The compressed (source) data.</param>
        /// <param name="decompressedData">The destination buffer.</param>
        /// <returns>The number of bytes written to the destination.</returns>
        public static int Decompress(byte[] compressedData, byte[] decompressedData)
        {
            return _internal_decompress(compressedData, compressedData.Length, decompressedData,
                decompressedData.Length);
        }

        /// <summary>
        ///     Compresses data into a buffer.
        /// </summary>
        /// <param name="uncompressedData">The uncompressed (source) data.</param>
        /// <param name="compressedData">The destination buffer.</param>
        /// <returns>The number of bytes written to the destination.</returns>
        public static int Compress(byte[] uncompressedData, ref byte[] compressedData)
        {
            var size = _internal_compress(uncompressedData, uncompressedData.Length, compressedData);

            if (compressedData.Length < size) throw new Exception();

            Array.Resize(ref compressedData, size);

            return size;
        }

        // unsigned char* in, int in_size, unsigned char* out, int out_size
        [DllImport("NativeLibrary", EntryPoint = "LZDecompress", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _internal_decompress([In] byte[] inData, int inSize, [Out] byte[] outData,
            int outSize);

        // unsigned char* in, int in_size, unsigned char* out
        [DllImport("NativeLibrary", EntryPoint = "JLZCompress", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _internal_compress([In] byte[] inData, int inSize, [Out] byte[] outData);
    }
}