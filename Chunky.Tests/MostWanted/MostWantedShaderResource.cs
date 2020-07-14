using System;
using System.IO;
using Chunky.Resources;

namespace Chunky.Tests.MostWanted
{
    public class MostWantedShaderResource : INamedResource
    {
        public string GetResourceTypeName()
        {
            return "Shader Information";
        }

        public IResourceWriter CreateWriter()
        {
            throw new NotImplementedException();
        }

        public string Name { get; set; }
    }

    public class MostWantedShaderResourceReader : IResourceReader<MostWantedShaderResource>
    {
        private readonly MostWantedShaderResource _resource;

        public MostWantedShaderResourceReader()
        {
            _resource = new MostWantedShaderResource();
        }

        IResource IResourceReader.GetResource()
        {
            return GetResource();
        }

        public MostWantedShaderResource GetResource()
        {
            return _resource;
        }

        public void ProcessChunk(Chunk chunk, BinaryReader reader)
        {
            if (reader.ReadUInt32() != 1) throw new InvalidDataException("Corrupted shader resource");

            if (reader.ReadUInt32() != 3) throw new InvalidDataException("Corrupted shader resource");

            if (reader.ReadUInt32() != 3) throw new InvalidDataException("Corrupted shader resource");

            reader.ReadUInt32(); // hash
            reader.ReadUInt32();
            _resource.Name = new string(reader.ReadChars(0x1C)).TrimEnd('\0');
            reader.ReadBytes(0x78);
        }
    }
}