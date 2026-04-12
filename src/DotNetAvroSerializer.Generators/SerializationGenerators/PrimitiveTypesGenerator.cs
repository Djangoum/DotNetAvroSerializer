using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class PrimitiveTypesGenerator
{
    internal static void GenerateSerializationSourceForPrimitive(AvroGenerationContext context)
    {
        var schema = context.Schema as PrimitiveSchema;

        if (context.SerializableTypeMetadata is null)
            throw new AvroGeneratorException($"Primitive type was not satisfied {context.SerializableTypeMetadata}");

        if (schema!.Name is not "null" && context.SerializableTypeMetadata is NullableSerializableTypeMetadata)
            throw new AvroGeneratorException(
                DiagnosticsDescriptors.UnsupportedNullablePatternDescriptor,
                $"Nullable type {context.SerializableTypeMetadata.FullNameDisplay} must be represented by an Avro union that includes 'null'.");

        var serializerCallCode = schema!.Name switch
        {
            "boolean" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Boolean } => context.WriteCall("BooleanSchema", context.SourceAccessor),
            "int" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int32 } => context.WriteCall("IntSchema", context.SourceAccessor),
            "long" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int64 } => context.WriteCall("LongSchema", context.SourceAccessor),
            "string" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_String } => context.WriteCall("StringSchema", context.SourceAccessor),
            "bytes" when context.SerializableTypeMetadata is IterableSerializableTypeMetadata { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Byte } } => context.WriteCall("BytesSchema", context.SourceAccessor),
            "double" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Double } => context.WriteCall("DoubleSchema", context.SourceAccessor),
            "float" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Single } => context.WriteCall("FloatSchema", context.SourceAccessor),
            "null" => context.WriteCall("NullSchema", context.SourceAccessor),
            _ => throw new AvroGeneratorException($"Required type was not satisfied to serialize {schema!.Name}, {context.SerializableTypeMetadata} found")
        };

        context.SerializationCode.WriteLine(serializerCallCode);
    }
}
