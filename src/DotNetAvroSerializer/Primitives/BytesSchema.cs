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
        outputStream.Write(value, 0, value.Length);
    }

    public static async Task WriteAsync(Stream outputStream, byte[] value, CancellationToken cancellationToken = default)
    {
        await LongSchema.WriteAsync(outputStream, value.Length, cancellationToken);
        await outputStream.WriteAsync(value, 0, value.Length, cancellationToken);
    }
}
