using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IBL.Exceptions
{
    public class BlException : Exception
    {
        internal int ObjectId;
        internal Type TypeOfWrongObject;

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
