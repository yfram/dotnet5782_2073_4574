using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IBL.Exceptions
{
    public class ObjectAllreadyExistsException : Exception
    {
        public ObjectAllreadyExistsException()
        {
        }

        public ObjectAllreadyExistsException(string message) : base(message)
        {
        }

        public ObjectAllreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectAllreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
