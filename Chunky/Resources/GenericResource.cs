using System.IO;

namespace Chunky.Resources
{
    /// <summary>
    ///     Represents an unknown resource. Data is stored as a byte array.
    /// </summary>
    public class GenericResource : IResource
    {
        public uint ChunkId { get; set; }
        public byte[] Data { get; set; }

        public string GetResourceTypeName()
        {
            return "UNKNOWN";
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

        public GenericResource GetResource()
        {
            return _resource;
        }

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