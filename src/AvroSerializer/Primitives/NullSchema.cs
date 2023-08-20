using System.IO;

namespace DotNetAvroSerializer.Primitives
{
    public static class NullSchema
    {
        public static bool CanSerialize(object? value) => value is null;

        public static void Write(Stream outputStream, object value)
        {
            // Zero bytes written
        }
    }
}
