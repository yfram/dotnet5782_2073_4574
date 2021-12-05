using System;
using System.Runtime.Serialization;

namespace IBL.Exceptions
{
    public class DroneStateException : Exception
    {
        public DroneStateException()
        {
        }

        public DroneStateException(string message) : base(message)
        {
        }

        public DroneStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
