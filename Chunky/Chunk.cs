namespace Chunky
{
    /// <summary>
    ///     Represents a chunk. A chunk is a unit of data within a bundle.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        ///     Gets or sets the chunk ID.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        ///     Gets or sets the chunk size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     Gets or sets the chunk offset.
        /// </summary>
        /// <remarks>This is the offset of the chunk header, not data.</remarks>
        public long Offset { get; set; }

        /// <summary>
        ///     Gets the offset of the chunk data.
        /// </summary>
        public long DataOffset => Offset + 8;

        /// <summary>
        ///     Gets the offset of the end of the chunk.
        /// </summary>
        public long EndOffset => Offset + 8 + Size;

        /// <summary>
        ///     Determines whether the chunk is a container chunk.
        ///     Container chunks contain child chunks.
        /// </summary>
        public bool IsContainer => (Id & 0x80000000) == 0x80000000;
    }
}