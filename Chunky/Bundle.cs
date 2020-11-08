using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunky.IO;
using Chunky.Resources;
using JetBrains.Annotations;

namespace Chunky
{
    /// <summary>
    ///     Represents a resource bundle.
    /// </summary>
    public class Bundle : IEnumerable<IResource>
    {
        private readonly List<IResource> _resources;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bundle" /> class.
        /// </summary>
        /// <param name="resources">A list of <see cref="IResource" /> objects.</param>
        public Bundle([NotNull] List<IResource> resources)
        {
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public IEnumerator<IResource> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Finds the resource with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="TResource"></typeparam>
        /// <returns></returns>
        public TResource FindResourceByName<TResource>(string name) where TResource : INamedResource
        {
            return _resources.OfType<TResource>().First(r => string.Equals(r.Name, name));
        }

        /// <summary>
        ///     Finds all resources of the given type.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <returns></returns>
        public IEnumerable<TResource> FindResourcesByType<TResource>() where TResource : IResource
        {
            return _resources.OfType<TResource>();
        }

        /// <summary>
        ///     Writes the bundle resources to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write resources to.</param>
        public void WriteToStream(Stream stream)
        {
            using var chunkWriter = new ChunkWriter(stream);

            foreach (var resource in _resources)
            {
                var resourceWriter = resource.CreateWriter();
                resourceWriter.Align(chunkWriter);
                chunkWriter.BeginChunk(resourceWriter.GetChunkId());
                resourceWriter.Write(chunkWriter);
                chunkWriter.EndChunk();
            }
        }
    }
}