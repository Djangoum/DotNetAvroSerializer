using System.Buffers;

namespace DotNetAvroSerializer.Benchmarks.BufferWriters;

public sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
{
    private byte[] _buffer;
    private int _writtenCount;

    public PooledByteBufferWriter(int initialCapacity = 256)
    {
        _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
    }

    public ReadOnlyMemory<byte> WrittenMemory => _buffer.AsMemory(0, _writtenCount);

    public void Advance(int count)
    {
        _writtenCount += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        EnsureCapacity(sizeHint);
        return _buffer.AsMemory(_writtenCount);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        EnsureCapacity(sizeHint);
        return _buffer.AsSpan(_writtenCount);
    }

    public void Reset()
    {
        _writtenCount = 0;
    }

    public void Dispose()
    {
        var buffer = _buffer;
        _buffer = Array.Empty<byte>();
        _writtenCount = 0;

        if (buffer.Length > 0)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private void EnsureCapacity(int sizeHint)
    {
        sizeHint = sizeHint <= 0 ? 1 : sizeHint;
        var required = _writtenCount + sizeHint;

        if (required <= _buffer.Length)
        {
            return;
        }

        var newSize = Math.Max(required, _buffer.Length * 2);
        var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
        _buffer.AsSpan(0, _writtenCount).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }
}
