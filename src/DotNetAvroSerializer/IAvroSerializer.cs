using System.IO;

namespace DotNetAvroSerializer;

public interface IAvroSerializer<TSerializable>
{
    byte[] Serialize(TSerializable source);
    void SerializeToStream(Stream outputStream, TSerializable source);
}
