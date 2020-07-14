using System.Collections.Generic;

namespace Chunky.Modules.TexturePack.Data
{
    /// <summary>
    ///     Represents an animated texture record.
    /// </summary>
    public class AnimatedTexture
    {
        /// <summary>
        ///     Gets or sets the name of the animated texture.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the name hash of the animated texture.
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        ///     Gets or sets the number of frames per second in the animation.
        /// </summary>
        public byte FramesPerSecond { get; set; }

        /// <summary>
        ///     Gets or sets the list of textures used in the animation.
        /// </summary>
        public List<Texture> Frames { get; set; }
    }
}