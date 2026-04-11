using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class ArrayGenerator
{
    internal static void GenerateSerializationSourceForArray(AvroGenerationContext context)
    {
        var schema = context.Schema as ArraySchema;

        if (context.SerializableTypeMetadata is not IterableSerializableTypeMetadata iterableSerializableTypeMetadata)
            throw new AvroGeneratorException(
                $"Array type for {schema!.Name} is not satisfied {context.SerializableTypeMetadata?.FullNameDisplay} provided, arrays must be arrays or anything that implements IEnumerable");

        var itemVar = $"item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}";
        var countVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}Count";

        context.SerializationCode.WriteLine($"var {countVar} = GetCollectionCount({context.SourceAccessor});");
        context.SerializationCode.WriteLine($"if ({countVar} > 0) {context.WriteCall("LongSchema", countVar)}");
        context.SerializationCode.WriteLine($"foreach(var {itemVar} in {context.SourceAccessor})");

        using (context.SerializationCode.WriteBlock())
        {
            schema!.ItemSchema.Generate(context with
            {
                Schema = schema!.ItemSchema,
                SourceAccessor = itemVar,
                SerializableTypeMetadata = iterableSerializableTypeMetadata.ItemsTypeMetadata
            });
        }

        context.SerializationCode.WriteLine(context.WriteCall("LongSchema", "0L"));
    }
}
