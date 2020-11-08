using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunky.IO;
using Chunky.Resources;
using Chunky.Utils;
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

        /// <summary>
        ///     Reads the given stream and returns a new <see cref="Bundle" /> object.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> to read data from.</param>
        /// <param name="options">The <see cref="BundleLoadOptions" /> to use when loading the bundle.</param>
        /// <returns>The new <see cref="Bundle" /> object.</returns>
        public static Bundle FromStream(Stream stream, BundleLoadOptions options = null)
        {
            options ??= new BundleLoadOptions();

            // Create activators from options
            var activatorMap = options.ChunkMappings.ToDictionary(
                p => p.Key,
                p => ReflectionHelpers.GetActivator<IResourceReader>(p.Value.GetConstructor(Type.EmptyTypes)));

            var chunkReader = new ChunkReader(stream);
            var resources = new List<IResource>();

            void ProcessChunks(long endOffset, bool recursing = false, IResourceReader resourceReader = null)
            {
                while (stream.Position < endOffset)
                {
                    var chunk = chunkReader.NextChunk();

                    // Only process non-padding chunks
                    if (chunk.Id != 0)
                    {
                        // Make sure we're not already within a container chunk.
                        // Children of containers call back to the container handler.
                        if (!recursing)
                        {
                            resourceReader = activatorMap.TryGetValue(chunk.Id, out var activator)
                                ? activator()
                                : new GenericResourceReader();

                            if (chunk.IsContainer)
                                ProcessChunks(chunk.EndOffset, true, resourceReader);
                            else
                                resourceReader.ProcessChunk(chunk, chunkReader.BinaryReader);
                        }
                        else
                        {
                            if (resourceReader == null) throw new ArgumentNullException(nameof(resourceReader));

                            resourceReader.ProcessChunk(chunk, chunkReader.BinaryReader);
                        }

                        resources.Add(resourceReader.GetResource());
                    }

                    // Jump to the end of the chunk
                    stream.Position = chunk.EndOffset;
                }
            }

            ProcessChunks(stream.Length);

            return new Bundle(resources);
        }
    }
}