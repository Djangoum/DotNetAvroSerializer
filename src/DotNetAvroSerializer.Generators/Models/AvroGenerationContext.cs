using System.Text;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.Models;

internal readonly record struct AvroGenerationContext(
    Schema Schema,
    StringBuilder SerializationCode,
    PrivateFieldsCode PrivateFieldsCode,
    EquatableArray<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata,
    SerializableTypeMetadata SerializableTypeMetadata,
    string SourceAccessor,
    SerializationMode Mode)
{
    /// <summary>
    /// Returns a schema write statement for the current mode:
    /// sync  → <c>SchemaType.Write(outputStream, value);</c>
    /// async → <c>await SchemaType.WriteAsync(outputStream, value, cancellationToken);</c>
    /// </summary>
    internal string WriteCall(string schemaTypeName, string value)
        => Mode == SerializationMode.Async
            ? $"await {schemaTypeName}.WriteAsync(outputStream, {value}, cancellationToken);"
            : $"{schemaTypeName}.Write(outputStream, {value});";

    internal static AvroGenerationContext From(SerializerMetadata serializerMetadata, Schema schema,
        SerializationMode mode, string sourceAccessor = "source")
    {
        var serializationCode = new StringBuilder();
        var privateFieldsCode = new PrivateFieldsCode();

        return new AvroGenerationContext(schema, serializationCode, privateFieldsCode,
            serializerMetadata.CustomLogicalTypesMetadata, serializerMetadata.SerializableTypeMetadata, sourceAccessor, mode);
    }
}
