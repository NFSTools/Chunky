using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Chunky.Utils
{
    /// <summary>
    ///     Exposes helper functions for working with binary streams.
    /// </summary>
    public static class BinaryHelpers
    {
        /// <summary>
        ///     Reads a structure from a binary stream.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <returns>An instance of <typeparamref name="T" /> read from the stream.</returns>
        public static T ReadStruct<T>(BinaryReader reader) where T : struct
        {
            return ReadStruct<T>(reader.BaseStream);
        }

        /// <summary>
        ///     Reads a structure from a binary stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <returns>An instance of <typeparamref name="T" /> read from the stream.</returns>
        public static T ReadStruct<T>(Stream stream) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var buffer = ReadBytesRequired(stream, size);
            return BufferToStructure<T>(buffer);
        }

        /// <summary>
        ///     Converts a byte array to a structure instance.
        /// </summary>
        /// <param name="buffer">The byte array to unmarshal.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <returns>A new structure of type <typeparamref name="T" />.</returns>
        public static T BufferToStructure<T>(byte[] buffer) where T : struct
        {
            Debug.Assert(buffer.Length == Marshal.SizeOf<T>(), "buffer.Length == Marshal.SizeOf<T>()");
            using var handle = new DisposableGcHandle(buffer, GCHandleType.Pinned);

            return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        }

        /// <summary>
        ///     Reads an array of bytes from a binary stream,
        ///     ensuring the data is completely read.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>An array of bytes read from the stream.</returns>
        /// <exception cref="InvalidDataException">if the data cannot be read completely</exception>
        public static byte[] ReadBytesRequired(BinaryReader reader, int length)
        {
            return ReadBytesRequired(reader.BaseStream, length);
        }

        /// <summary>
        ///     Reads an array of bytes from a binary stream,
        ///     ensuring the data is completely read.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>An array of bytes read from the stream.</returns>
        /// <exception cref="InvalidDataException">if the data cannot be read completely</exception>
        public static byte[] ReadBytesRequired(Stream stream, int length)
        {
            var buffer = new byte[length];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidDataException($"Could not read {buffer.Length} bytes from stream");

            return buffer;
        }

        /// <summary>
        ///     Marshals a structure to a byte array.
        /// </summary>
        /// <param name="data">The structure to marshal.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <returns>An array of bytes representing the structure.</returns>
        public static byte[] MarshalStruct<T>(T data) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var buffer = new byte[size];

            using var gcHandle = new DisposableGcHandle(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(data, gcHandle.AddrOfPinnedObject(), false);

            return buffer;
        }

        /// <summary>
        ///     Writes a structure to a binary stream.
        /// </summary>
        /// <param name="writer">The binary stream to write the structure to.</param>
        /// <param name="data">The structure to write.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        public static void WriteStruct<T>(BinaryWriter writer, T data) where T : struct
        {
            WriteStruct(writer.BaseStream, data);
        }


        /// <summary>
        ///     Writes a structure to a binary stream.
        /// </summary>
        /// <param name="stream">The binary stream to write the structure to.</param>
        /// <param name="data">The structure to write.</param>
        /// <typeparam name="T">The structure type.</typeparam>
        public static void WriteStruct<T>(Stream stream, T data) where T : struct
        {
            stream.Write(MarshalStruct(data));
        }
    }
}