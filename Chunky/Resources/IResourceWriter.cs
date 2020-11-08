using Chunky.IO;

namespace Chunky.Resources
{
    /// <summary>
    ///     Exposes an interface for a resource writer.
    /// </summary>
    public interface IResourceWriter
    {
        /// <summary>
        ///     Gets the chunk ID of the resource type.
        /// </summary>
        /// <returns>The chunk ID</returns>
        uint GetChunkId();

        /// <summary>
        ///     Writes the resource data to a binary stream.
        /// </summary>
        /// <param name="chunkWriter">The chunk bundle writer</param>
        void Write(ChunkWriter chunkWriter);

        /// <summary>
        ///     Align the bundle output stream as needed
        /// </summary>
        /// <param name="chunkWriter">The chunk bundle writer</param>
        void Align(ChunkWriter chunkWriter)
        {
            // No-op by default
        }
    }
}