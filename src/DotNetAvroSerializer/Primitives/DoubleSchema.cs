using System;
using System.Buffers.Binary;
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
            throw new AvroSerializationException("Cannot serialize null value to double");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        outputStream.Write(buffer);
    }

    public static Task WriteAsync(Stream outputStream, double? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to double");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, double value, CancellationToken cancellationToken = default)
    {
        return WriteCoreAsync(outputStream, value, cancellationToken).AsTask();
    }

    private static ValueTask WriteCoreAsync(Stream outputStream, double value, CancellationToken cancellationToken)
    {
        var buffer = new byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        return outputStream.WriteAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
    }
}
