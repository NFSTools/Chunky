using System;
using System.Runtime.Serialization;

namespace Chunky.Resources
{
    [Serializable]
    public class ResourceReadException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ResourceReadException()
        {
        }

        public ResourceReadException(string message) : base(message)
        {
        }

        public ResourceReadException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ResourceReadException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}