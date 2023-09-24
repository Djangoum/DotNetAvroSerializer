using System;
using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class PrimitiveTypesGenerator
{
    internal static void GenerateSerializationSourceForPrimitive(AvroGenerationContext context)
    {
        var schema = context.Schema as PrimitiveSchema;

        if (context.SerializableTypeMetadata is null)
            throw new AvroGeneratorException($"Primitive type was not satisfied {context.SerializableTypeMetadata}");

        var serializerCallCode = schema!.Name switch
        {
            "boolean" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                           && (primitiveTypeMetadata.TypeName.Equals("bool", StringComparison.InvariantCultureIgnoreCase)
                               || primitiveTypeMetadata.TypeName.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase)) => $"BooleanSchema.Write(outputStream, {context.SourceAccessor});",
            "int" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                       && (primitiveTypeMetadata.TypeName.Equals("int", StringComparison.InvariantCultureIgnoreCase)
                           || primitiveTypeMetadata.TypeName.Equals("Int32", StringComparison.InvariantCultureIgnoreCase)) => $"IntSchema.Write(outputStream, {context.SourceAccessor});",
            "long" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                        && (primitiveTypeMetadata.TypeName.Equals("long", StringComparison.InvariantCultureIgnoreCase)
                            || primitiveTypeMetadata.TypeName.Equals("Int64", StringComparison.InvariantCultureIgnoreCase)) => $"LongSchema.Write(outputStream, {context.SourceAccessor});",
            "string" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                          && (primitiveTypeMetadata.TypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase)) => $"StringSchema.Write(outputStream, {context.SourceAccessor});",
            "bytes" when context.SerializableTypeMetadata is IterableSerializableTypeMetadata { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata primitiveTypeMetadata }
                         && primitiveTypeMetadata.TypeName.Equals("byte", StringComparison.InvariantCultureIgnoreCase) => $"BytesSchema.Write(outputStream, {context.SourceAccessor});",
            "double" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                          && primitiveTypeMetadata.TypeName.Equals("double", StringComparison.InvariantCultureIgnoreCase) => $"DoubleSchema.Write(outputStream, {context.SourceAccessor});",
            "float" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                         && primitiveTypeMetadata.TypeName.Equals("single", StringComparison.InvariantCultureIgnoreCase) => $"FloatSchema.Write(outputStream, {context.SourceAccessor});",
            "null" => $"NullSchema.Write(outputStream, {context.SourceAccessor});",
            _ => throw new AvroGeneratorException($"Required type was not satisfied to serialize {schema!.Name} {context.SerializableTypeMetadata} found")
        };

        context.SerializationCode.AppendLine(serializerCallCode);
    }
}
