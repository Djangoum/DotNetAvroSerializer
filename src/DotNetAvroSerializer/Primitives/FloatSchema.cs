using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

public static class FloatSchema
{
    public static bool CanSerialize(object? value) => value is float;

    public static void Write(Stream outputStream, float? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to float");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        outputStream.Write(buffer);
    }

    public static Task WriteAsync(Stream outputStream, float? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to float");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, float value, CancellationToken cancellationToken = default)
    {
        return WriteCoreAsync(outputStream, value, cancellationToken).AsTask();
    }

    private static ValueTask WriteCoreAsync(Stream outputStream, float value, CancellationToken cancellationToken)
    {
        var buffer = new byte[sizeof(float)];
        BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        return outputStream.WriteAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
    }
}
