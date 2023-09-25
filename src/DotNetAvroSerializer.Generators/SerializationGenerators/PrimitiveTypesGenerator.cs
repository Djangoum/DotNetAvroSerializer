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
            "boolean" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Boolean } => $"BooleanSchema.Write(outputStream, {context.SourceAccessor});",
            "int" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int32 } => $"IntSchema.Write(outputStream, {context.SourceAccessor});",
            "long" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int64 } => $"LongSchema.Write(outputStream, {context.SourceAccessor});",
            "string" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_String } => $"StringSchema.Write(outputStream, {context.SourceAccessor});",
            "bytes" when context.SerializableTypeMetadata is IterableSerializableTypeMetadata { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Byte } } => $"BytesSchema.Write(outputStream, {context.SourceAccessor});",
            "double" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Double } => $"DoubleSchema.Write(outputStream, {context.SourceAccessor});",
            "float" when context.SerializableTypeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Single } => $"FloatSchema.Write(outputStream, {context.SourceAccessor});",
            "null" => $"NullSchema.Write(outputStream, {context.SourceAccessor});",
            _ => throw new AvroGeneratorException($"Required type was not satisfied to serialize {schema!.Name}, {context.SerializableTypeMetadata} found")
        };

        context.SerializationCode.AppendLine(serializerCallCode);
    }
}
