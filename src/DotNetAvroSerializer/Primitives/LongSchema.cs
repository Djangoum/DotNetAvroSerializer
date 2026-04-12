using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

public class LongSchema
{
    public static bool CanSerialize(object? value) => value is long;

    public static void Write(Stream outputStream, long? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, long value)
    {
        Span<byte> buffer = stackalloc byte[10];
        var length = Encode(value, buffer);
        outputStream.Write(buffer[..length]);
    }

    public static Task WriteAsync(Stream outputStream, long? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, long value, CancellationToken cancellationToken = default)
    {
        return WriteCoreAsync(outputStream, value, cancellationToken).AsTask();
    }

    private static ValueTask WriteCoreAsync(Stream outputStream, long value, CancellationToken cancellationToken)
    {
        var buffer = new byte[10];
        var length = Encode(value, buffer);
        return outputStream.WriteAsync(buffer.AsMemory(0, length), cancellationToken);
    }

    private static int Encode(long value, Span<byte> destination)
    {
        ulong n = (ulong)((value << 1) ^ (value >> 63));
        var length = 0;

        while ((n & ~0x7FUL) != 0)
        {
            destination[length++] = (byte)((n & 0x7F) | 0x80);
            n >>= 7;
        }

        destination[length++] = (byte)n;
        return length;
    }
}
