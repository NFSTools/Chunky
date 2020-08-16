using System.IO;
using Chunky.IO;
using Chunky.Resources;

namespace Chunky.Tests.MostWanted
{
    public class MostWantedShaderWriter : IResourceWriter
    {
        private readonly MostWantedShaderResource _resource;

        public MostWantedShaderWriter(MostWantedShaderResource resource)
        {
            _resource = resource;
        }

        public uint GetChunkId()
        {
            return 0x135200;
        }

        public void Write(ChunkBundleWriter bundleWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(1);
            binaryWriter.Write(3);
            binaryWriter.Write(3);
            binaryWriter.Write(0x13371337);
            binaryWriter.Write(0);

            var nameBytes = new char[0x1C];
            nameBytes[0] = 'H';
            nameBytes[1] = 'E';
            nameBytes[2] = 'L';
            nameBytes[3] = 'L';
            nameBytes[4] = 'O';
            binaryWriter.Write(nameBytes);
            for (var i = 0; i < 0x78; i++)
                binaryWriter.Write((byte) i);
        }
    }

    public class MostWantedShaderResource : INamedResource
    {
        public string GetResourceTypeName()
        {
            return "Shader Information";
        }

        public IResourceWriter CreateWriter()
        {
            return new MostWantedShaderWriter(this);
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