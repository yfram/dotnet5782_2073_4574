using IBL.BO;
using System;
using System.Runtime.Serialization;

namespace IBL
{
    internal class DoesNotExistsException : Exception
    {
        private int id;

        public DoesNotExistsException()
        {
        }

        public DoesNotExistsException(string message) : base(message)
        {
        }

        public DoesNotExistsException(string message, int _id) : this(message)
        {
            id = _id;
        }

        public DoesNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}