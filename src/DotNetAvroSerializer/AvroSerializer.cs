using System;
using System.IO;

namespace DotNetAvroSerializer;

public abstract class AvroSerializer<TSerializable> : IAvroSerializer<TSerializable>
{
    public virtual byte[] Serialize(TSerializable source) { throw new NotImplementedException(); }
    public virtual void SerializeToStream(Stream outputStream, TSerializable source) { throw new NotImplementedException(); }
}
