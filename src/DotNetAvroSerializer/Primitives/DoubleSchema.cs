using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

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

    public static Task WriteAsync(Stream outputStream, double? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, double value, CancellationToken cancellationToken = default)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return outputStream.WriteAsync(new ReadOnlyMemory<byte>(bytes, 0, bytes.Length), cancellationToken).AsTask();
    }
}
