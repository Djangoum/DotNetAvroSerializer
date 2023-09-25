using System.IO;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

public static class BooleanSchema
{
    public static bool CanSerialize(object? value) => value is bool;

    public static void Write(Stream outputStream, bool? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, bool value)
    {
        outputStream.WriteByte((byte)(value ? 1 : 0));
    }
}
