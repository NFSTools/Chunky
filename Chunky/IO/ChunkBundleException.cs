using System;
using System.Runtime.Serialization;

namespace Chunky.IO
{
    [Serializable]
    public class ChunkBundleException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ChunkBundleException()
        {
        }

        public ChunkBundleException(string message) : base(message)
        {
        }

        public ChunkBundleException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChunkBundleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}