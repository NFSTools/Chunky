using System.Collections.Generic;
using System.IO;
using Chunky.IO;

namespace Chunky.Resources
{
    /// <summary>
    ///     Allows the developer to specify alignments for chunk types
    ///     that are handled by the <see cref="GenericResourceReader" />.
    /// </summary>
    public static class GenericAlignmentHelper
    {
        private static readonly Dictionary<uint, uint> AlignmentMap = new Dictionary<uint, uint>();

        /// <summary>
        ///     Retrieves a chunk ID's mapped alignment.
        /// </summary>
        /// <param name="chunkId">The chunk ID.</param>
        /// <returns>The chunk alignment.</returns>
        /// <remarks>If no mapping is found, returns 0.</remarks>
        public static uint GetAlignment(uint chunkId)
        {
            return AlignmentMap.TryGetValue(chunkId, out var alignment) ? alignment : 0;
        }

        /// <summary>
        ///     Adds a chunk alignment mapping.
        /// </summary>
        /// <param name="chunkId">The chunk ID.</param>
        /// <param name="alignment">The chunk alignment.</param>
        public static void SetAlignment(uint chunkId, uint alignment)
        {
            AlignmentMap[chunkId] = alignment;
        }
    }

    /// <summary>
    ///     Writer implementation for <see cref="GenericResource" />.
    /// </summary>
    public class GenericResourceWriter : IResourceWriter
    {
        private readonly GenericResource _resource;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericResourceWriter" /> class.
        /// </summary>
        /// <param name="resource"></param>
        public GenericResourceWriter(GenericResource resource)
        {
            _resource = resource;
        }

        /// <inheritdoc />
        public uint GetChunkId()
        {
            return _resource.ChunkId;
        }

        /// <inheritdoc />
        public void Write(ChunkWriter chunkWriter)
        {
            chunkWriter.BinaryWriter.Write(_resource.Data);
        }

        /// <inheritdoc />
        public void Align(ChunkWriter chunkWriter)
        {
            var alignment = GenericAlignmentHelper.GetAlignment(_resource.ChunkId);

            if (alignment > 0) chunkWriter.AlignmentChunk(alignment);
        }
    }

    /// <summary>
    ///     Represents an unknown resource. Data is stored as a byte array.
    /// </summary>
    public class GenericResource : IResource
    {
        /// <summary>
        ///     Gets or sets the ID of the chunk represented by the resource.
        /// </summary>
        public uint ChunkId { get; set; }

        /// <summary>
        ///     Gets or sets the data of the chunk represented by the resource.
        /// </summary>
        public byte[] Data { get; set; }

        /// <inheritdoc />
        public string GetResourceTypeName()
        {
            return "UNKNOWN";
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public GenericResource GetResource()
        {
            return _resource;
        }

        /// <inheritdoc />
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