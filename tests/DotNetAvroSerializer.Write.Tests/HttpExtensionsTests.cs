using DotNetAvroSerializer.Extensions.Http;
using DotNetAvroSerializer;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class HttpExtensionsTests
{
    [Fact]
    public async Task ToHttpContent_WithSyncSerializer_WritesSerializedPayload()
    {
        var serializer = new SyncSerializer();
        using var content = serializer.ToHttpContent("foo");
        var bytes = await content.ReadAsByteArrayAsync();

        bytes.Should().Equal(new byte[] { 0x41, 0x42, 0x43 });
        serializer.SerializeToStreamCallCount.Should().Be(1);
        content.Headers.ContentType!.MediaType.Should().Be("application/avro");
    }

    [Fact]
    public async Task ToHttpContent_WithAsyncSerializer_WritesSerializedPayload()
    {
        var serializer = new AsyncSerializer();
        using var content = serializer.ToHttpContent("foo", "avro/binary");
        var bytes = await content.ReadAsByteArrayAsync();

        bytes.Should().Equal(new byte[] { 0x31, 0x32, 0x33 });
        serializer.SerializeToStreamAsyncCallCount.Should().Be(1);
        content.Headers.ContentType!.MediaType.Should().Be("avro/binary");
    }

    [Fact]
    public async Task ToHttpContent_WithCanceledToken_ThrowsOperationCanceledException()
    {
        var serializer = new AsyncSerializer();
        using var content = serializer.ToHttpContent("foo");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var action = async () => await content.ReadAsByteArrayAsync(cts.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    private sealed class SyncSerializer : IAvroSerializer<string>
    {
        public int SerializeToStreamCallCount { get; private set; }

        public byte[] Serialize(string source) => throw new NotSupportedException();

        public void SerializeToStream(Stream outputStream, string source)
        {
            SerializeToStreamCallCount++;
            outputStream.WriteByte(0x41);
            outputStream.WriteByte(0x42);
            outputStream.WriteByte(0x43);
        }
    }

    private sealed class AsyncSerializer : IAsyncAvroSerializer<string>
    {
        public int SerializeToStreamAsyncCallCount { get; private set; }

        public Task<byte[]> SerializeAsync(string source, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public async Task SerializeToStreamAsync(Stream outputStream, string source, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SerializeToStreamAsyncCallCount++;
            await outputStream.WriteAsync(new byte[] { 0x31, 0x32, 0x33 }, cancellationToken);
        }
    }
}
#pragma warning restore CA2007
