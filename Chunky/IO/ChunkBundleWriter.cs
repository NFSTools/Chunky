using System;
using System.Collections.Generic;
using System.IO;

namespace Chunky.IO
{
    public class ChunkBundleWriter : IDisposable
    {
        private readonly Bundle _bundle;
        private readonly Stack<Chunk> _chunkStack;
        private readonly Stream _stream;
        private readonly BinaryWriter _writer;

        public ChunkBundleWriter(Bundle bundle, Stream stream)
        {
            _bundle = bundle ?? throw new ArgumentNullException(nameof(bundle));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite) throw new ArgumentException("Stream is not writable", nameof(stream));

            _writer = new BinaryWriter(_stream);
            _chunkStack = new Stack<Chunk>();
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _writer?.Dispose();
        }

        public void WriteResources()
        {
            foreach (var resource in _bundle)
            {
                var writer = resource.CreateWriter();
                var alignment = writer.GetAlignment();

                if (alignment != 0) AlignmentChunk(alignment);

                BeginChunk(writer.GetChunkId());
                writer.Write(this, _writer);
                EndChunk();
            }
        }

        public Chunk BeginChunk(uint type)
        {
            var chunk = new Chunk {Id = type, Offset = _stream.Position};
            _chunkStack.Push(chunk);
            _writer.Write(type);
            _writer.Write(0);
            return chunk;
        }

        public void EndChunk()
        {
            var chunk = _chunkStack.Pop();
            chunk.Size = (int) (_stream.Position - chunk.DataOffset);
            _stream.Position = chunk.Offset + 4;
            _writer.Write(chunk.Size);
            _stream.Position = chunk.EndOffset;
        }

        public void AlignmentChunk(int boundary)
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