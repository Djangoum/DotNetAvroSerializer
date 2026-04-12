using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

public static class BooleanSchema
{
    private const byte TrueByte = 1;
    private const byte FalseByte = 0;

    public static bool CanSerialize(object? value) => value is bool;

    public static void Write(Stream outputStream, bool? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to boolean");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, bool value)
    {
        Span<byte> buffer = stackalloc byte[1];
        buffer[0] = value ? TrueByte : FalseByte;
        outputStream.Write(buffer);
    }

    public static Task WriteAsync(Stream outputStream, bool? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to boolean");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, bool value, CancellationToken cancellationToken = default)
    {
        return WriteCoreAsync(outputStream, value, cancellationToken).AsTask();
    }

    private static ValueTask WriteCoreAsync(Stream outputStream, bool value, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[1];
        buffer[0] = value ? TrueByte : FalseByte;
        return outputStream.WriteAsync(buffer.AsMemory(0, 1), cancellationToken);
    }
}
