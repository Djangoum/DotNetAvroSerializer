using Avro;
using DotNetAvroSerializer.Generators.Helpers;
using System.Collections.Generic;
using System.Text;

namespace DotNetAvroSerializer.Generators.Models;

internal readonly record struct AvroGenerationContext(
    Schema Schema,
    StringBuilder SerializationCode,
    PrivateFieldsCode PrivateFieldsCode,
    IEnumerable<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata,
    SerializableTypeMetadata SerializableTypeMetadata,
    string SourceAccessor)
{
    internal static AvroGenerationContext From(SerializerMetadata serializerMetadata, Schema schema,
        string sourceAccessor = "source")
    {
        var serializationCode = new StringBuilder();
        var privateFieldsCode = new PrivateFieldsCode();
        
        return new AvroGenerationContext(schema, serializationCode, privateFieldsCode,
            serializerMetadata.CustomLogicalTypesMetadata, serializerMetadata.SerializableTypeMetadata, sourceAccessor);
    }
}