using System;
using System.Runtime.Serialization;

namespace DotNetBay.Data.FileStorage
{
    [Serializable]
    public class FileStorageException : Exception
    {
        public FileStorageException()
        {
        }

        public FileStorageException(string message)
            : base(message)
        {
        }

        public FileStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FileStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
