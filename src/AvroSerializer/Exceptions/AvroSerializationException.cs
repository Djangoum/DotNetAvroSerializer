using System;

namespace AvroSerializer.Exceptions
{
    public class AvroSerializationException : Exception
    {
        public AvroSerializationException(string message) : base(message)
        {

        }
    }
}
