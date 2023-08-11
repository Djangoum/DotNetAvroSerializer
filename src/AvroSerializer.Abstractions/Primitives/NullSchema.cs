using System.IO;

namespace AvroSerializer.Primitives
{
    public static class NullSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            // Zero bytes written
        }
    }
}
