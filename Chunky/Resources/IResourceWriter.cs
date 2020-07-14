using System.IO;
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
        /// <param name="bundleWriter">The chunk bundle writer</param>
        /// <param name="binaryWriter">The binary stream writer</param>
        void Write(ChunkBundleWriter bundleWriter, BinaryWriter binaryWriter);

        /// <summary>
        ///     Gets the required chunk alignment of the resource type.
        /// </summary>
        /// <returns>The required alignment</returns>
        int GetAlignment()
        {
            return 0;
        }
    }
}