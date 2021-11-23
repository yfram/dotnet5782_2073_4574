using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IBL.Exceptions
{
    public class ObjectDoesntExistException : Exception
    {
        public ObjectDoesntExistException()
        {
        }

        public ObjectDoesntExistException(string message) : base(message)
        {
        }

        public ObjectDoesntExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectDoesntExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
