using AvroSerializer.Abstractions.Exceptions;
using System.IO;

namespace AvroSerializer.Abstractions.Types
{
    public static class IntSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            if (value is int parsedInt)
            {
                ulong n = (ulong)((parsedInt << 1) ^ (parsedInt >> 63));
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
