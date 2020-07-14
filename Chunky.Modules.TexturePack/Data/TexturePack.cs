using System;
using Chunky.Resources;

namespace Chunky.Modules.TexturePack.Data
{
    /// <summary>
    ///     Represents a texture pack loaded from a chunk bundle.
    /// </summary>
    public class TexturePack : INamedResource
    {
        /// <summary>
        ///     Gets or sets the file name of the texture pack.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the type of the texture pack.
        /// </summary>
        public TexturePackType Type { get; set; }

        public string GetResourceTypeName()
        {
            return "TexturePack";
        }

        public IResourceWriter CreateWriter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets or sets the name of the texture pack.
        /// </summary>
        public string Name { get; set; }
    }
}