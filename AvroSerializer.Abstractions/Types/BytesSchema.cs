using AvroSerializer.Abstractions.Exceptions;
using System.IO;

namespace AvroSerializer.Abstractions.Types
{
    public class BytesSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            if (value is byte[] parsedByteArray)
            {
                outputStream.WriteByte((byte)parsedByteArray.Length);
                outputStream.Write(parsedByteArray, 0, parsedByteArray.Length);
            }

            throw new AvroSerializationException();
        }
    }
}
