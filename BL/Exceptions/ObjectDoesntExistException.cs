// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Runtime.Serialization;

namespace BlApi.Exceptions
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
