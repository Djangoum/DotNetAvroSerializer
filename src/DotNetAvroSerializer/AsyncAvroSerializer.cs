using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer;

public abstract class AsyncAvroSerializer<TSerializable> : IAsyncAvroSerializer<TSerializable>
{
    public int AsyncArrayItemCountBlockSize { get; set; } = 1024;

    public virtual Task<byte[]> SerializeAsync(TSerializable source, CancellationToken cancellationToken = default) { throw new NotImplementedException(); }
    public virtual Task SerializeToStreamAsync(Stream outputStream, TSerializable source, CancellationToken cancellationToken = default) { throw new NotImplementedException(); }
}
