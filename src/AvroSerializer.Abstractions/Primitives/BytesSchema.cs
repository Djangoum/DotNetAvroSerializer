using AvroSerializer.Exceptions;
using System.IO;

namespace AvroSerializer.Primitives
{
    public class BytesSchema
    {
        public static void Write(Stream outputStream, byte[] value)
        {
            LongSchema.Write(outputStream, (long)value.Length);
            outputStream.Write(value, 0, value.Length);
        }
    }
}
