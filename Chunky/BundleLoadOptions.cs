using System;
using System.Collections.Generic;
using Chunky.Resources;

namespace Chunky
{
    /// <summary>
    ///     A container for bundle loading options, such as chunk ID registrations.
    /// </summary>
    public class BundleLoadOptions
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BundleLoadOptions" /> class.
        /// </summary>
        public BundleLoadOptions()
        {
            ChunkMappings = new Dictionary<uint, Type>();
        }

        /// <summary>
        ///     A dictionary holding mappings in the form [chunk ID, type of chunk handler]
        /// </summary>
        public Dictionary<uint, Type> ChunkMappings { get; }

        /// <summary>
        ///     Adds a new chunk mapping.
        /// </summary>
        /// <param name="chunkId">The chunk ID to map.</param>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <typeparam name="TReader">The resource reader type.</typeparam>
        public void AddChunkMapping<TResource, TReader>(uint chunkId) where TResource : IResource
            where TReader : IResourceReader<TResource>
        {
            ChunkMappings[chunkId] = typeof(TReader);
        }
    }
}