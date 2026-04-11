using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer;

public abstract class AvroSerializer<TSerializable> : IAvroSerializer<TSerializable>
{
    public virtual byte[] Serialize(TSerializable source) { throw new NotImplementedException(); }
    public virtual void SerializeToStream(Stream outputStream, TSerializable source) { throw new NotImplementedException(); }
    public virtual Task<byte[]> SerializeAsync(TSerializable source, CancellationToken cancellationToken = default) { throw new NotImplementedException(); }
    public virtual Task SerializeToStreamAsync(Stream outputStream, TSerializable source, CancellationToken cancellationToken = default) { throw new NotImplementedException(); }
}
