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

        /// <inheritdoc />
        public Bundle LoadBundle(Stream stream)
        {
            var resources = new List<IResource>();

            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    var chunk = NextChunk(reader);
                    if (chunk.Id == 0) continue;
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

        private Chunk NextChunk(BinaryReader reader)
        {
            var id = reader.ReadUInt32();
            var size = reader.ReadInt32();

            if (id != 0x55441122 /* Don't check chunk size for compressed data */ &&
                reader.BaseStream.Position + size > reader.BaseStream.Length)
                throw new ChunkReaderException($"Overflowing chunk detected at {reader.BaseStream.Position - 8}.");

            var chunk = new Chunk {Id = id, Size = size, Offset = reader.BaseStream.Position - 8};

            if (chunk.IsContainer)
                // Read children
                while (reader.BaseStream.Position < chunk.EndOffset)
                {
                    var nextChunk = NextChunk(reader);
                    if (nextChunk.Id == 0) continue;
                    chunk.Children.Add(nextChunk);
                }

            reader.BaseStream.Position = chunk.EndOffset;
            return chunk;
        }
    }
}