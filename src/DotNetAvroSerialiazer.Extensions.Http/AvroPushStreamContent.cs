using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerialiazer.Extensions.Http;

public sealed class AvroPushStreamContent : HttpContent
{
    private readonly Func<Stream, CancellationToken, Task> _serializeToStreamAsync;

    public AvroPushStreamContent(Func<Stream, CancellationToken, Task> serializeToStreamAsync, string mediaType)
    {
        ArgumentNullException.ThrowIfNull(serializeToStreamAsync);
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType);

        _serializeToStreamAsync = serializeToStreamAsync;
        Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
        _serializeToStreamAsync(stream, CancellationToken.None);

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken) =>
        _serializeToStreamAsync(stream, cancellationToken);

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }
}
