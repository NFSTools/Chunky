using System;
using System.IO;

namespace Chunky.IO
{
    /// <summary>
    ///     Exposes an API for reading chunks from a stream.
    /// </summary>
    public class ChunkReader : IDisposable
    {
        private readonly Stream _stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChunkReader" /> class.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <exception cref="ArgumentException">if <paramref name="stream" /> is not readable.</exception>
        public ChunkReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Stream is not readable", nameof(stream));
            BinaryReader = new BinaryReader(stream);
        }

        /// <summary>
        ///     Gets the <see cref="System.IO.BinaryReader" /> object
        ///     used by the chunk reader.
        /// </summary>
        public BinaryReader BinaryReader { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream?.Dispose();
            BinaryReader?.Dispose();
        }

        /// <summary>
        ///     Reads a chunk header from the stream and returns a new <see cref="Chunk" /> object.
        /// </summary>
        /// <returns>A new <see cref="Chunk" /> object with info about the chunk that was read.</returns>
        /// <exception cref="ChunkStreamException">if an invalid chunk is read, or if the stream is at EOF</exception>
        /// <remarks>
        ///     This method only reads a header. Processing the data is left to the user.
        ///     If the chunk type is [22 11 44 55], the following data is interpreted
        ///     as a header for a compressed buffer. The chunk is then skipped, and
        ///     another chunk is read.
        /// </remarks>
        public Chunk NextChunk()
        {
            return NextChunkInternal();
        }

        private Chunk NextChunkInternal()
        {
            if (BinaryReader.BaseStream.Position >= BinaryReader.BaseStream.Length)
                throw new ChunkStreamException("Can't read chunks beyond the end of the stream!");

            var id = BinaryReader.ReadUInt32();

            // Ugly but necessary hardcoded check.
            // If we encounter data with the CompressInPlace marker, we should parse the CIPHeader structure and just skip over the data.
            // The resource handler will have to deal with compressed data on its own. This is usually pretty simple, because offsets to compressed blocks
            // are usually found in an *actual* chunk.
            if (id == 0x55441122)
            {
                // read USize
                BinaryReader.ReadUInt32();

                // read CSize
                var compressedSize = BinaryReader.ReadUInt32();

                if (compressedSize < 12) throw new ChunkStreamException("Invalid size in compressed data block");

                if (BinaryReader.BaseStream.Position + (compressedSize - 12) > BinaryReader.BaseStream.Length)
                    throw new ChunkStreamException("Overflowing compressed data block");

                // skip to next block and try again
                BinaryReader.BaseStream.Position += compressedSize - 12;
                return NextChunk();
            }

            var size = BinaryReader.ReadInt32();

            if (BinaryReader.BaseStream.Position + size > BinaryReader.BaseStream.Length)
                throw new ChunkStreamException(
                    $"Overflowing chunk detected at {BinaryReader.BaseStream.Position - 8}.");

            return new Chunk {Id = id, Size = size, Offset = BinaryReader.BaseStream.Position - 8};
        }
    }
}