using System;
using System.Collections.Generic;
using System.IO;

namespace Chunky.IO
{
    /// <summary>
    ///     Exposes API for writing chunks to a stream
    /// </summary>
    public class ChunkWriter : IDisposable
    {
        private readonly Stack<Chunk> _chunkStack;
        private readonly Stream _stream;

        public ChunkWriter(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite) throw new ArgumentException("Stream is not writable", nameof(stream));

            BinaryWriter = new BinaryWriter(_stream);
            _chunkStack = new Stack<Chunk>();
        }

        public BinaryWriter BinaryWriter { get; }

        public void Dispose()
        {
            _stream?.Dispose();
            BinaryWriter?.Dispose();

            if (_chunkStack.Count != 0) throw new ChunkBundleException("Chunk stack must be empty.");
        }

        /// <summary>
        ///     Push a new chunk onto the chunk stack
        /// </summary>
        /// <param name="type">The chunk type ID</param>
        /// <returns>A new <see cref="Chunk" /> object.</returns>
        public Chunk BeginChunk(uint type)
        {
            var chunk = new Chunk {Id = type, Offset = _stream.Position};
            _chunkStack.Push(chunk);
            BinaryWriter.Write(type);
            BinaryWriter.Write(0);
            return chunk;
        }

        /// <summary>
        ///     Pop a chunk from the chunk stack and finalize it.
        /// </summary>
        public void EndChunk()
        {
            var chunk = _chunkStack.Pop();
            chunk.Size = (int) (_stream.Position - chunk.DataOffset);
            _stream.Position = chunk.Offset + 4;
            BinaryWriter.Write(chunk.Size);
            _stream.Position = chunk.EndOffset;
        }

        /// <summary>
        ///     Generate an alignment (null-filled) chunk to align the stream to the given byte boundary.
        /// </summary>
        /// <param name="boundary">The desired alignment</param>
        public void AlignmentChunk(uint boundary)
        {
            if (_stream.Position % boundary != 0)
            {
                BeginChunk(0);
                if (_stream.Position % boundary != 0)
                {
                    var offset = boundary - _stream.Position % boundary;
                    _stream.Seek(offset, SeekOrigin.Current);
                }

                EndChunk();
            }
        }
    }
}