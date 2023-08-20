using System.IO;
using System.Text;

namespace AvroSerializer.Primitives
{
    public class StringSchema
    {
        public static bool CanSerialize(object? value) => value is string;

        public static void Write(Stream outputStream, string value)
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            LongSchema.Write(outputStream, stringBytes.Length);
            outputStream.Write(stringBytes, 0, stringBytes.Length);
        }
    }
}
