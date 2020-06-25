using System.IO;
using Chunky.Resources;

namespace Chunky
{
    /// <summary>
    ///     Exposes an API for reading resources from chunks.
    /// </summary>
    public interface IResourceReader
    {
        /// <summary>
        ///     Gets the final resource that was generated.
        /// </summary>
        /// <returns>The generated resource.</returns>
        IResource GetResource();

        /// <summary>
        ///     Processes the given chunk to load resource data.
        /// </summary>
        /// <param name="chunk">The chunk to process.</param>
        /// <param name="reader">The <see cref="BinaryReader" /> to read data from</param>
        void ProcessChunk(Chunk chunk, BinaryReader reader);
    }

    /// <summary>
    ///     Exposes an API for reading resources from chunks.
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource" /> implementation</typeparam>
    public interface IResourceReader<out TResource> : IResourceReader where TResource : IResource
    {
        /// <summary>
        ///     Gets the final resource that was generated.
        /// </summary>
        /// <returns>The generated resource.</returns>
        new TResource GetResource();
    }
}