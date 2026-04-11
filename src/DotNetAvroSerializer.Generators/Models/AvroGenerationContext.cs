using System.Collections.Generic;
using System.Text;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.Models;

internal readonly record struct AvroGenerationContext(
    Schema Schema,
    StringBuilder SerializationCode,
    StringBuilder AsyncSerializationCode,
    PrivateFieldsCode PrivateFieldsCode,
    IEnumerable<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata,
    SerializableTypeMetadata SerializableTypeMetadata,
    string SourceAccessor)
{
    internal static AvroGenerationContext From(SerializerMetadata serializerMetadata, Schema schema,
        string sourceAccessor = "source")
    {
        var serializationCode = new StringBuilder();
        var asyncSerializationCode = new StringBuilder();
        var privateFieldsCode = new PrivateFieldsCode();

        return new AvroGenerationContext(schema, serializationCode, asyncSerializationCode, privateFieldsCode,
            serializerMetadata.CustomLogicalTypesMetadata, serializerMetadata.SerializableTypeMetadata, sourceAccessor);
    }
}
