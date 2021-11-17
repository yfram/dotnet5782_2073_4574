using IBL.BO;
using System;
using System.Runtime.Serialization;

namespace IBL
{
    internal class AlreadyExistsException : Exception
    {
        private int id;

        public AlreadyExistsException()
        {
        }

        public AlreadyExistsException(string message) : base(message)
        {
        }

        public AlreadyExistsException(string message, int _id) : this(message)
        {
            id = _id;
        }

        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}