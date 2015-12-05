using System;
using System.Runtime.Serialization;

namespace DotNetBay.Core
{
    [Serializable]
    public class MissingAuctionException : Exception
    {
        public MissingAuctionException()
        {
        }

        public MissingAuctionException(string message)
            : base(message)
        {
        }

        public MissingAuctionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MissingAuctionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
