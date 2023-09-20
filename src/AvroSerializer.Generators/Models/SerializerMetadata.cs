using System.Collections.Generic;
using Avro;

namespace DotNetAvroSerializer.Generators.Models
{
    internal record SerializerMetadata(string SerializerClassName, string SerializerNamespace, Schema AvroSchema, SerializableTypeMetadata SerializableTypeMetadata, IEnumerable<CustomLogicalTypeMetadata> LogicalTypesFullyQualifiedNames);
}