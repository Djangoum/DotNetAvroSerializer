using System;
using System.IO;

namespace AvroSerializer.Primitives
{
    public static class FloatSchema
    {
        public static void Write(Stream outputStream,  float value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            outputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
