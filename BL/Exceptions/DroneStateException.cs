using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
