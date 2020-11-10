using System;
using System.Runtime.Serialization;

namespace Chunky.Resources
{
    /// <summary>
    ///     Thrown when an error occurs in resource processing.
    /// </summary>
    [Serializable]
    public class ResourceReadException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <inheritdoc />
        public ResourceReadException()
        {
        }

        /// <inheritdoc />
        public ResourceReadException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ResourceReadException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected ResourceReadException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}