using AvroSerializer.Abstractions.Exceptions;
using System.IO;
using System.Text;

namespace AvroSerializer.Abstractions.Types
{
    public class StringSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            if (value is string parsedString)
            {
                var stringBytes = Encoding.UTF8.GetBytes(parsedString);
                outputStream.WriteByte((byte) parsedString.Length);
                outputStream.Write(stringBytes, 0, stringBytes.Length);
            }

            throw new AvroSerializationException();
        }
    }
}
