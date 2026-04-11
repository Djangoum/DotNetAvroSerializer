using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.Models;

internal readonly record struct AvroGenerationContext(
    Schema Schema,
    IndentedTextWriter SerializationCode,
    PrivateFieldsCode PrivateFieldsCode,
    EquatableArray<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata,
    SerializableTypeMetadata SerializableTypeMetadata,
    string SourceAccessor,
    SerializationMode Mode)
{
    internal string WriteCall(string schemaTypeName, string value)
        => Mode == SerializationMode.Async
            ? $"await {schemaTypeName}.WriteAsync(outputStream, {value}, cancellationToken);"
            : $"{schemaTypeName}.Write(outputStream, {value});";

    internal static AvroGenerationContext From(SerializerMetadata serializerMetadata, Schema schema,
        SerializationMode mode, string sourceAccessor = "source")
    {
        var serializationCode = new IndentedTextWriter();
        var privateFieldsCode = new PrivateFieldsCode();

        return new AvroGenerationContext(schema, serializationCode, privateFieldsCode,
            serializerMetadata.CustomLogicalTypesMetadata, serializerMetadata.SerializableTypeMetadata, sourceAccessor, mode);
    }
}
