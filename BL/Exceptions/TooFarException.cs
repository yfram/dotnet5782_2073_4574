using System;
using System.Runtime.Serialization;

namespace IBL
{
    [Serializable]
    internal class TooFarException : Exception
    {
        public TooFarException()
        {
        }

        public TooFarException(string message) : base(message)
        {
        }

        public TooFarException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TooFarException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}