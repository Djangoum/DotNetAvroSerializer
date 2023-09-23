using System;

namespace DotNetAvroSerializer.Exceptions
{
    public class AvroSerializationException : Exception
    {
        public AvroSerializationException(string message) : base(message)
        {

        }
    }
}
