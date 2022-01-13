// File BlException.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Runtime.Serialization;

namespace BlApi.Exceptions
{
    public class BlException : Exception
    {
        public int ObjectId;
        public Type TypeOfWrongObject;

        public BlException(string message, int id, Type typeOfWrongObject) : base(message)
        {
            ObjectId = id;
            TypeOfWrongObject = typeOfWrongObject;
        }

        public BlException()
        {
        }

        public BlException(string message) : base(message)
        {
        }

        public BlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
