using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer.Extensions.Http;

public static class AvroSerializerHttpExtensions
{
    public const string DefaultMediaType = "application/avro";

    public static HttpContent ToHttpContent<TSerializable>(
        this IAvroSerializer<TSerializable> serializer,
        TSerializable source,
        string mediaType = DefaultMediaType)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType);

        return new AvroPushStreamContent(
            (stream, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                serializer.SerializeToStream(stream, source);
                return Task.CompletedTask;
            },
            mediaType);
    }

    public static HttpContent ToHttpContent<TSerializable>(
        this IAsyncAvroSerializer<TSerializable> serializer,
        TSerializable source,
        string mediaType = DefaultMediaType)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType);

        return new AvroPushStreamContent(
            (stream, cancellationToken) => serializer.SerializeToStreamAsync(stream, source, cancellationToken),
            mediaType);
    }
}
