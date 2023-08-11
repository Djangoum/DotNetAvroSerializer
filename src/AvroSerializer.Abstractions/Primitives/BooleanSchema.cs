using AvroSerializer.Exceptions;
using System.IO;

namespace AvroSerializer.Primitives
{
    public static class BooleanSchema
    {
        public static void Write(Stream outputStream, bool value)
        {
            outputStream.WriteByte((byte) (value ? 1 : 0));
        }
    }
}
