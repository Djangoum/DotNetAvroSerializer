using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetAvroSerializer;

public interface IAsyncAvroSerializer<TSerializable>
{
    Task<byte[]> SerializeAsync(TSerializable source, CancellationToken cancellationToken = default);
    Task SerializeToStreamAsync(Stream outputStream, TSerializable source, CancellationToken cancellationToken = default);
}
