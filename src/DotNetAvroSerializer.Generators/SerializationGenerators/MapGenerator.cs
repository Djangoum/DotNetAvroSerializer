using System;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class MapGenerator
{
    internal static void GenerateSerializationSourceForMap(AvroGenerationContext context)
    {
        var schema = context.Schema as MapSchema;

        if (context.SerializableTypeMetadata is not DictionarySerializableTypeMetadata dictionaryTypeMetadata)
            throw new AvroGeneratorException($"Type for map schema was not satisfied. Maps must implement IDictionary but {context.SerializableTypeMetadata} found");

        if (!dictionaryTypeMetadata.KeysTypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase))
            throw new AvroGeneratorException($"Map keys have to be strings but {dictionaryTypeMetadata.KeysTypeName}");

        var itemVar = $"item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}";

        context.SerializationCode.AppendLine($"if ({context.SourceAccessor}.Count() > 0) {context.WriteCall("LongSchema", $"{context.SourceAccessor}.Count()")}");
        context.SerializationCode.AppendLine($"foreach(var {itemVar} in {context.SourceAccessor})");
        context.SerializationCode.AppendLine("{");
        context.SerializationCode.AppendLine(context.WriteCall("StringSchema", $"{itemVar}.Key"));

        schema!.ValueSchema.Generate(context with
        {
            Schema = schema!.ValueSchema,
            SerializableTypeMetadata = dictionaryTypeMetadata.ValuesMetadata,
            SourceAccessor = $"{itemVar}.Value"
        });

        context.SerializationCode.AppendLine("}");
        context.SerializationCode.AppendLine(context.WriteCall("LongSchema", "0L"));
    }
}
