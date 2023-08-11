using System;
using System.IO;

namespace AvroSerializer.Primitives
{
    public class DoubleSchema
    {
        public static void Write(Stream outputStream, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            BytesSchema.Write(outputStream, bytes);
        }
    }
}
