using System.IO;
using System.Text;

namespace AvroSerializer.Primitives
{
    public class StringSchema
    {
        public static void Write(Stream outputStream, string value)
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            LongSchema.Write(outputStream, (long)stringBytes.Length);
            outputStream.Write(stringBytes, 0, stringBytes.Length);
        }
    }
}
