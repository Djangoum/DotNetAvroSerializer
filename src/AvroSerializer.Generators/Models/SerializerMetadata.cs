using Avro;

namespace DotNetAvroSerializer.Generators.Models
{
    internal record SerializerMetadata(string SerializerClassName, string SerializerNamespace, Schema AvroSchema, SerializableTypeMetadata SerializableTypeMetadata);
}