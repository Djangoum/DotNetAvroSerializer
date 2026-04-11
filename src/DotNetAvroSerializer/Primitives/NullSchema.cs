using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer.Primitives;

public static class NullSchema
{
    public static bool CanSerialize(object? value) => value is null || value is Null;

    public static void Write(Stream outputStream, object value)
    {
        // Zero bytes written
    }

    public static Task WriteAsync(Stream outputStream, object value, CancellationToken cancellationToken = default)
    {
        // Zero bytes written
        return Task.CompletedTask;
    }
}
