using System;
using System.Runtime.Serialization;

namespace Chunky.IO
{
    [Serializable]
    public class ChunkReaderException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ChunkReaderException()
        {
        }

        public ChunkReaderException(string message) : base(message)
        {
        }

        public ChunkReaderException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChunkReaderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}