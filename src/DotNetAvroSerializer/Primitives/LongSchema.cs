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
        ulong n = (ulong)(value << 1 ^ value >> 63);
        while ((n & ~0x7FUL) != 0)
        {
            outputStream.WriteByte((byte)(n & 0x7f | 0x80));
            n >>= 7;
        }
        outputStream.WriteByte((byte)n);
    }

    public static async Task WriteAsync(Stream outputStream, long? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        await WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, long value, CancellationToken cancellationToken = default)
    {
        ulong n = (ulong)(value << 1 ^ value >> 63);
        var buffer = new byte[10];
        var length = 0;

        while ((n & ~0x7FUL) != 0)
        {
            buffer[length++] = (byte)(n & 0x7f | 0x80);
            n >>= 7;
        }

        buffer[length++] = (byte)n;
        return outputStream.WriteAsync(buffer, 0, length, cancellationToken);
    }
}
