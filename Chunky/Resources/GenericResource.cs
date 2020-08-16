using System.Collections.Generic;
using System.IO;
using Chunky.IO;

namespace Chunky.Resources
{
    public static class GenericAlignmentHelper
    {
        private static readonly Dictionary<uint, int> AlignmentMap = new Dictionary<uint, int>();

        public static void SetAlignment(uint chunkId, int alignment)
        {
            AlignmentMap[chunkId] = alignment;
        }

        public static int GetAlignment(uint chunkId)
        {
            return AlignmentMap.TryGetValue(chunkId, out var alignment) ? alignment : 0;
        }
    }

    /// <summary>
    ///     Writer implementation for <see cref="GenericResource" />.
    /// </summary>
    public class GenericResourceWriter : IResourceWriter
    {
        private readonly GenericResource _resource;

        public GenericResourceWriter(GenericResource resource)
        {
            _resource = resource;
        }

        public uint GetChunkId()
        {
            return _resource.ChunkId;
        }

        public int GetAlignment()
        {
            return GenericAlignmentHelper.GetAlignment(_resource.ChunkId);
        }

        public void Write(ChunkBundleWriter bundleWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_resource.Data);
        }
    }

    /// <summary>
    ///     Represents an unknown resource. Data is stored as a byte array.
    /// </summary>
    public class GenericResource : IResource
    {
        public uint ChunkId { get; set; }
        public byte[] Data { get; set; }

        public string GetResourceTypeName()
        {
            return "UNKNOWN";
        }

        public IResourceWriter CreateWriter()
        {
            return new GenericResourceWriter(this);
        }
    }

    /// <summary>
    ///     Implementation of <see cref="IResourceReader{TResource}" /> that returns a new <see cref="GenericResource" />
    ///     instance.
    /// </summary>
    public class GenericResourceReader : IResourceReader<GenericResource>
    {
        private GenericResource _resource;

        IResource IResourceReader.GetResource()
        {
            return GetResource();
        }

        public GenericResource GetResource()
        {
            return _resource;
        }

        public void ProcessChunk(Chunk chunk, BinaryReader reader)
        {
            _resource = new GenericResource
            {
                ChunkId = chunk.Id,
                Data = reader.ReadBytes(chunk.Size)
            };

            if (_resource.Data.Length != chunk.Size)
                throw new ResourceReadException(
                    $"Expected to read {chunk.Size} bytes but only got {_resource.Data.Length}");
        }
    }
}