using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer.Primitives;

public class BytesSchema
{
    public static bool CanSerialize(object? value) => value is byte[];

    public static void Write(Stream outputStream, byte[] value)
    {
        LongSchema.Write(outputStream, value.Length);
        outputStream.Write(value);
    }

    public static Task WriteAsync(Stream outputStream, byte[] value, CancellationToken cancellationToken = default)
        => WriteCoreAsync(outputStream, value, cancellationToken).AsTask();

    private static async ValueTask WriteCoreAsync(Stream outputStream, byte[] value, CancellationToken cancellationToken)
    {
        await LongSchema.WriteAsync(outputStream, value.Length, cancellationToken).ConfigureAwait(false);
        await outputStream.WriteAsync(value.AsMemory(0, value.Length), cancellationToken).ConfigureAwait(false);
    }
}
