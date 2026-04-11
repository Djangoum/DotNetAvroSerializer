using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer;

public interface IAvroSerializer<TSerializable>
{
    byte[] Serialize(TSerializable source);
    void SerializeToStream(Stream outputStream, TSerializable source);
    Task<byte[]> SerializeAsync(TSerializable source, CancellationToken cancellationToken = default);
    Task SerializeToStreamAsync(Stream outputStream, TSerializable source, CancellationToken cancellationToken = default);
}
