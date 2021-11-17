using IBL.BO;
using System;
using System.Runtime.Serialization;

namespace IBL
{
    internal class NoFreeStationsAvailable : Exception
    {
        private int id;
        private Location location;

        public NoFreeStationsAvailable()
        {
        }

        public NoFreeStationsAvailable(string message) : base(message)
        {
        }

        public NoFreeStationsAvailable(int id, Location location)
        {
            this.id = id;
            this.location = location;
        }

        public NoFreeStationsAvailable(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoFreeStationsAvailable(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}