using AvroSerializer.Exceptions;
using System;
using System.IO;

namespace DotNetAvroSerializer.Primitives
{
    public static class FloatSchema
    {
        public static bool CanSerialize(object? value) => value is float;

        public static void Write(Stream outputStream, float? value)
        {
            if (value is null)
                throw new AvroSerializationException("Cannot serialize null value to int");

            Write(outputStream, value.Value);
        }

        public static void Write(Stream outputStream, float value)
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
