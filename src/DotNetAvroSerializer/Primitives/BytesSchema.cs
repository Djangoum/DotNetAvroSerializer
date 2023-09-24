using System.IO;

namespace DotNetAvroSerializer.Primitives;

public class BytesSchema
{
    public static bool CanSerialize(object? value) => value is byte[];

    public static void Write(Stream outputStream, byte[] value)
    {
        LongSchema.Write(outputStream, value.Length);
        outputStream.Write(value, 0, value.Length);
    }
}
