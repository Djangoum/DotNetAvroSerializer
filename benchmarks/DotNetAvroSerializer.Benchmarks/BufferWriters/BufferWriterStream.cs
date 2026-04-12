using System.Buffers;

namespace DotNetAvroSerializer.Benchmarks.BufferWriters;

public sealed class BufferWriterStream : Stream
{
    private readonly IBufferWriter<byte> _bufferWriter;

    public BufferWriterStream(IBufferWriter<byte> bufferWriter)
    {
        _bufferWriter = bufferWriter;
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(buffer.AsSpan(offset, count));
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        var destination = _bufferWriter.GetSpan(buffer.Length);
        buffer.CopyTo(destination);
        _bufferWriter.Advance(buffer.Length);
    }

    public override void WriteByte(byte value)
    {
        var destination = _bufferWriter.GetSpan(1);
        destination[0] = value;
        _bufferWriter.Advance(1);
    }
}
