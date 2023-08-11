using AvroSerializer.Abstractions.Exceptions;
using System.IO;

namespace AvroSerializer.Abstractions.Types
{
    public class LongSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            if (value is long parsedLong)
            {
                ulong n = (ulong)((parsedLong << 1) ^ (parsedLong >> 63));
                while ((n & ~0x7FUL) != 0)
                {
                    outputStream.WriteByte((byte)((n & 0x7f) | 0x80));
                    n >>= 7;
                }
                outputStream.WriteByte((byte)n);
            }

            throw new AvroSerializationException();
        }
    }
}
