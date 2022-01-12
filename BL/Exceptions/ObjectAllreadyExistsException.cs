// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Runtime.Serialization;

namespace BlApi.Exceptions
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
