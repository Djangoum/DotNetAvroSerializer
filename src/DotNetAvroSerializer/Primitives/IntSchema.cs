using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;

namespace DotNetAvroSerializer.Primitives;

public static class IntSchema
{
    public static bool CanSerialize(object? value) => value is int;

    public static void Write(Stream outputStream, int? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, int value)
    {
        ulong n = (ulong)(value << 1 ^ value >> 63);
        while ((n & ~0x7FUL) != 0)
        {
            outputStream.WriteByte((byte)(n & 0x7f | 0x80));
            n >>= 7;
        }
        outputStream.WriteByte((byte)n);
    }

    public static Task WriteAsync(Stream outputStream, int? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, int value, CancellationToken cancellationToken = default)
    {
        ulong n = (ulong)(value << 1 ^ value >> 63);
        var buffer = new byte[5];
        var length = 0;

        while ((n & ~0x7FUL) != 0)
        {
            buffer[length++] = (byte)(n & 0x7f | 0x80);
            n >>= 7;
        }

        buffer[length++] = (byte)n;
        return outputStream.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, length), cancellationToken).AsTask();
    }
}
