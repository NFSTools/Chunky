using System;
using System.Collections.Generic;
using System.IO;
using Chunky.Utils;

namespace Chunky.IO
{
    /// <summary>
    ///     Exposes an API for writing chunks to a stream.
    /// </summary>
    public class ChunkWriter : IDisposable
    {
        private readonly Stack<Chunk> _chunkStack;
        private readonly Stream _stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChunkWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentException">if <paramref name="stream" /> is not writable.</exception>
        public ChunkWriter(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite) throw new ChunkStreamException("Stream is not writable");

            BinaryWriter = new BinaryWriter(_stream);
            _chunkStack = new Stack<Chunk>();
        }

        /// <summary>
        ///     Gets the <see cref="System.IO.BinaryWriter" /> object
        ///     used by the chunk writer.
        /// </summary>
        public BinaryWriter BinaryWriter { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream?.Dispose();
            BinaryWriter?.Dispose();

            if (_chunkStack.Count != 0) throw new ChunkStreamException("Chunk stack must be empty.");
        }

        /// <summary>
        ///     Initializes a new chunk and writes a header to the stream.
        /// </summary>
        /// <param name="type">The chunk type.</param>
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
        ///     Finalizes the current chunk and updates its header.
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
        ///     Generates an alignment (null-filled) chunk to align the stream to the given byte boundary.
        /// </summary>
        /// <param name="boundary">The desired alignment.</param>
        public void AlignmentChunk(uint boundary)
        {
            if (_stream.Position % boundary != 0)
            {
                BeginChunk(0);
                BinaryHelpers.AlignStream(_stream, boundary);
                EndChunk();
            }
        }
    }
}