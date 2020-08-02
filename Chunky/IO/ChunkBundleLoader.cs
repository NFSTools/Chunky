using System;
using System.Collections.Generic;
using System.IO;
using Chunky.Resources;
using Chunky.Utils;

namespace Chunky.IO
{
    /// <summary>
    ///     Basic implementation of <see cref="IChunkBundleLoader" />.
    /// </summary>
    public class ChunkBundleLoader : IChunkBundleLoader
    {
        private readonly Dictionary<uint, ObjectActivator<IResourceReader>> _resourceReaderDictionary =
            new Dictionary<uint, ObjectActivator<IResourceReader>>();

        private Chunk _lastChunk;

        /// <inheritdoc />
        public Bundle LoadBundle(Stream stream)
        {
            var resources = new List<IResource>();

            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    var chunk = NextChunk(reader);
                    if (chunk == null || chunk.Id == 0) continue;
                    var resource = LoadResource(reader, chunk);

                    resources.Add(resource);
                }
            }

            return new Bundle(resources);
        }

        /// <inheritdoc />
        public void RegisterResource<TResource, TResourceReader>(uint chunkId) where TResource : IResource
            where TResourceReader : IResourceReader<TResource>
        {
            _resourceReaderDictionary[chunkId] =
                ReflectionHelpers.GetActivator<IResourceReader>(
                    typeof(TResourceReader).GetConstructor(Type.EmptyTypes));
        }

        private IResource LoadResource(BinaryReader binaryReader, Chunk chunk)
        {
            var reader = _resourceReaderDictionary.TryGetValue(chunk.Id, out var activator)
                ? activator()
                : new GenericResourceReader();

            void LoadChunk(Chunk chunkToLoad)
            {
                binaryReader.BaseStream.Position = chunkToLoad.DataOffset;

                // Only process nested chunks if we're working with an actual resource
                if (chunkToLoad.IsContainer && !(reader is GenericResourceReader))
                    chunkToLoad.Children.ForEach(LoadChunk);
                else
                    reader.ProcessChunk(chunkToLoad, binaryReader);

                if (binaryReader.BaseStream.Position != chunkToLoad.EndOffset)
                    throw new ChunkReaderException(
                        $"Expected to be at 0x{chunkToLoad.EndOffset:X}, but we are at 0x{binaryReader.BaseStream.Position:X}");
            }

            LoadChunk(chunk);

            return reader.GetResource();
        }

        private Chunk NextChunk(BinaryReader reader, bool recursing = false)
        {
            var id = reader.ReadUInt32();

            // Ugly but necessary hardcoded check.
            // If we encounter data with the CompressInPlace marker, we should parse the CIPHeader structure and just skip over the data.
            // The resource handler will have to deal with compressed data on its own. This is usually pretty simple, because offsets to compressed blocks
            // are usually found in an *actual* chunk.
            if (id == 0x55441122)
            {
                // read USize
                reader.ReadUInt32();

                // read CSize
                var compressedSize = reader.ReadUInt32();

                if (compressedSize < 12) throw new ChunkReaderException("Invalid size in compressed data block");

                if (reader.BaseStream.Position + (compressedSize - 12) > reader.BaseStream.Length)
                    throw new ChunkReaderException("Overflowing compressed data block");

                // skip to next block
                reader.BaseStream.Position += compressedSize - 12;

                return null;
            }

            var size = reader.ReadInt32();

            if (reader.BaseStream.Position + size > reader.BaseStream.Length)
                throw new ChunkReaderException($"Overflowing chunk detected at {reader.BaseStream.Position - 8}.");

            var chunk = new Chunk {Id = id, Size = size, Offset = reader.BaseStream.Position - 8};

            if (_lastChunk != null)
                _lastChunk.NextChunk = chunk;

            if (chunk.IsContainer)
                // Read children
                while (reader.BaseStream.Position < chunk.EndOffset)
                {
                    var nextChunk = NextChunk(reader, true);
                    if (nextChunk == null || nextChunk.Id == 0) continue;
                    chunk.Children.Add(nextChunk);
                }

            if (!recursing)
            {
                chunk.PreviousChunk = _lastChunk;
                _lastChunk = chunk;
            }

            reader.BaseStream.Position = chunk.EndOffset;
            return chunk;
        }
    }
}