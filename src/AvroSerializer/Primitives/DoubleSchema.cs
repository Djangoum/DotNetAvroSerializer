using DotNetAvroSerializer.Exceptions;
using System;
using System.IO;

namespace DotNetAvroSerializer.Primitives
{
    public class DoubleSchema
    {
        public static bool CanSerialize(object? value) => value is double;

        public static void Write(Stream outputStream, double? value)
        {
            if (value is null)
                throw new AvroSerializationException("Cannot serialize null value to int");

            Write(outputStream, value.Value);
        }

        public static void Write(Stream outputStream, double value)
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
