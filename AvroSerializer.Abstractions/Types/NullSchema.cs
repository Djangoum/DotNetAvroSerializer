using System.IO;

namespace AvroSerializer.Abstractions.Types
{
    public static class NullSchema
    {
        public static void Write(Stream outputStream, object value)
        {
            // Zero bytes written
        }
    }
}
