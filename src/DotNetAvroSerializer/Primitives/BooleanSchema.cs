using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

    public static async Task WriteAsync(Stream outputStream, bool? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        await WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, bool value, CancellationToken cancellationToken = default)
    {
        var byteBuffer = new[] { (byte)(value ? 1 : 0) };
        return outputStream.WriteAsync(byteBuffer, 0, byteBuffer.Length, cancellationToken);
    }
}
