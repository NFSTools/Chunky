using System.IO;
using Chunky.Resources;

namespace Chunky.IO
{
    /// <summary>
    ///     Exposes an interface for reading chunk bundles.
    /// </summary>
    public interface IChunkBundleLoader
    {
        /// <summary>
        ///     Loads chunks from the given stream and returns a new <see cref="Bundle" /> instance
        ///     with the chunk data.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> object to read chunks from.</param>
        /// <returns>A new instance of <see cref="Bundle" /> with the chunk data.</returns>
        Bundle LoadBundle(Stream stream);

        /// <summary>
        ///     Registers a resource handler for the given chunk ID.
        /// </summary>
        /// <param name="chunkId">The chunk ID to register a handler for.</param>
        /// <typeparam name="TResource">The <see cref="IResource" /> type.</typeparam>
        /// <typeparam name="TResourceReader">The <see cref="IResourceReader{TResource}" /> type.</typeparam>
        void RegisterResource<TResource, TResourceReader>(uint chunkId) where TResource : IResource
            where TResourceReader : IResourceReader<TResource>;
    }
}