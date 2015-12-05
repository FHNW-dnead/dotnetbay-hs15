using System;
using System.Runtime.Serialization;

namespace DotNetBay.Core
{
    [Serializable]
    public class AuctionStateException : Exception
    {
        public AuctionStateException()
        {
        }

        public AuctionStateException(string message)
            : base(message)
        {
        }

        public AuctionStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AuctionStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
