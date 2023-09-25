using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class ArrayGenerator
{
    internal static void GenerateSerializationSourceForArray(AvroGenerationContext context)
    {
        var schema = context.Schema as ArraySchema;

        if (context.SerializableTypeMetadata is not IterableSerializableTypeMetadata iterableSerializableTypeMetadata)
            throw new AvroGeneratorException($"Array type for {schema!.Name} is not satisfied {context.SerializableTypeMetadata?.GetType().Name} provided, arrays must be arrays or anything that implements IEnumerable");

        context.SerializationCode.AppendLine($"if ({context.SourceAccessor}.Count() > 0) LongSchema.Write(outputStream, (long){context.SourceAccessor}.Count());");
        context.SerializationCode.AppendLine($"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)} in {context.SourceAccessor})");
        context.SerializationCode.AppendLine("{");

        schema!.ItemSchema.Generate(context
            with
        {
            Schema = schema!.ItemSchema,
            SourceAccessor = $"item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}",
            SerializableTypeMetadata = iterableSerializableTypeMetadata.ItemsTypeMetadata
        });

        context.SerializationCode.AppendLine("}");
        context.SerializationCode.AppendLine("LongSchema.Write(outputStream, 0L);");
    }
}
