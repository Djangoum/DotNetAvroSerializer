using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Schemas;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal record SerializerMetadata(
    string SerializerClassName,
    string SerializerNamespace,
    Schema AvroSchema,
    SerializableTypeMetadata SerializableTypeMetadata,
    EquatableArray<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata,
    SmallLocation SerializerLocation)
{
    public Location GetSerializerLocation()
        => Location.Create(SerializerLocation.FilePath, SerializerLocation.TextSpan, SerializerLocation.LineSpan);
}
