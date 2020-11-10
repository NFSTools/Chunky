using System;
using System.Runtime.Serialization;

namespace Chunky.IO
{
    /// <summary>
    ///     Thrown when an error occurs in chunk processing.
    /// </summary>
    [Serializable]
    public class ChunkStreamException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <inheritdoc />
        public ChunkStreamException()
        {
        }

        /// <inheritdoc />
        public ChunkStreamException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ChunkStreamException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected ChunkStreamException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}