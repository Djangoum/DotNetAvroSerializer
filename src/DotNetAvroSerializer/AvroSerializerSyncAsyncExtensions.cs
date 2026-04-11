using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer;

public static class AvroSerializerSyncAsyncExtensions
{
    public static Task<byte[]> SerializeAsync<TSerializable>(
        this IAvroSerializer<TSerializable> serializer,
        TSerializable source,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(serializer);

        return Task.FromResult(serializer.Serialize(source));
    }

    public static Task SerializeToStreamAsync<TSerializable>(
        this IAvroSerializer<TSerializable> serializer,
        Stream outputStream,
        TSerializable source,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(serializer);

        serializer.SerializeToStream(outputStream, source);
        return Task.CompletedTask;
    }
}
