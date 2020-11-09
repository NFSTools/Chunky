using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

            return new Bundle(ProcessChunks(chunkReader, stream, stream.Length, activatorMap).ToList());
        }

        private static IEnumerable<IResource> ProcessChunks(ChunkReader chunkReader, Stream stream, long readLength,
            Dictionary<uint, ObjectActivator<IResourceReader>> activators,
            IResourceReader resourceReader = null)
        {
            var endPosition = stream.Position + readLength;

            while (stream.Position < endPosition)
            {
                var chunk = chunkReader.NextChunk();

                // Make sure we're dealing with a data chunk (ID 0 = padding)
                if (chunk.Id != 0)
                {
                    // resourceReader will be null if we're at the top level
                    if (resourceReader == null)
                    {
                        // Create resource reader and recurse
                        var activatorExists = activators.TryGetValue(chunk.Id, out var activator);
                        var newReader = activatorExists
                            ? activator()
                            : new GenericResourceReader();

                        if (activatorExists && chunk.IsContainer)
                            ProcessChunks(chunkReader, stream, chunk.Size, activators, newReader);
                        else
                            newReader.ProcessChunk(chunk, chunkReader.BinaryReader);

                        var resource = newReader.GetResource();
                        Debug.Assert(resource != null, "resource != null");
                        yield return resource;
                    }
                    else
                    {
                        resourceReader.ProcessChunk(chunk, chunkReader.BinaryReader);
                    }
                }

                stream.Position = chunk.EndOffset;
            }
        }
    }
}