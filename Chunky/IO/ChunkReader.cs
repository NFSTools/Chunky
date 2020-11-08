using System;
using System.IO;

namespace Chunky.IO
{
    public class ChunkReader : IDisposable
    {
        private readonly Stream _stream;

        public ChunkReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Stream is not readable", nameof(stream));
            BinaryReader = new BinaryReader(stream);
        }

        public BinaryReader BinaryReader { get; }

        public void Dispose()
        {
            _stream?.Dispose();
            BinaryReader?.Dispose();
        }

        public Chunk NextChunk()
        {
            return NextChunkInternal();
        }

        private Chunk NextChunkInternal()
        {
            if (BinaryReader.BaseStream.Position >= BinaryReader.BaseStream.Length)
                throw new ChunkBundleException("Can't read chunks beyond the end of the stream!");

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

                if (compressedSize < 12) throw new ChunkBundleException("Invalid size in compressed data block");

                if (BinaryReader.BaseStream.Position + (compressedSize - 12) > BinaryReader.BaseStream.Length)
                    throw new ChunkBundleException("Overflowing compressed data block");

                // skip to next block and try again
                BinaryReader.BaseStream.Position += compressedSize - 12;
                return NextChunk();
            }

            var size = BinaryReader.ReadInt32();

            if (BinaryReader.BaseStream.Position + size > BinaryReader.BaseStream.Length)
                throw new ChunkBundleException(
                    $"Overflowing chunk detected at {BinaryReader.BaseStream.Position - 8}.");

            return new Chunk {Id = id, Size = size, Offset = BinaryReader.BaseStream.Position - 8};
        }
    }
}