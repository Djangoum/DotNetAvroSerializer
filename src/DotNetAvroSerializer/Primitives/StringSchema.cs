using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer.Primitives;

public class StringSchema
{
    public static bool CanSerialize(object? value) => value is string;

    public static void Write(Stream outputStream, string value)
    {
        var stringBytes = Encoding.UTF8.GetBytes(value);
        LongSchema.Write(outputStream, stringBytes.Length);
        outputStream.Write(stringBytes);
    }

    public static Task WriteAsync(Stream outputStream, string value, CancellationToken cancellationToken = default)
        => WriteCoreAsync(outputStream, value, cancellationToken).AsTask();

    private static async ValueTask WriteCoreAsync(Stream outputStream, string value, CancellationToken cancellationToken)
    {
        var stringBytes = Encoding.UTF8.GetBytes(value);
        await LongSchema.WriteAsync(outputStream, stringBytes.Length, cancellationToken).ConfigureAwait(false);
        await outputStream.WriteAsync(stringBytes.AsMemory(0, stringBytes.Length), cancellationToken).ConfigureAwait(false);
    }
}
