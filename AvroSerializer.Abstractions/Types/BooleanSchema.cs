using AvroSerializer.Abstractions.Exceptions;
using System.IO;

namespace AvroSerializer.Abstractions.Types
{
    public static class BooleanSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            if (value is bool parsedBool)
            {
                outputStream.WriteByte((byte) (parsedBool ? 1 : 0));
            }

            throw new AvroSerializationException();
        }
    }
}
