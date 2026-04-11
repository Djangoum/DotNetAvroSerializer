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

        if (context.Mode is SerializationMode.Sync)
        {
            var countVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}Count";

            context.SerializationCode.WriteLine($"var {countVar} = GetCollectionCount({context.SourceAccessor});");
            context.SerializationCode.WriteLine($"if ({countVar} > 0) {context.WriteCall("LongSchema", countVar)}");
            context.SerializationCode.WriteLine($"foreach(var {itemVar} in {context.SourceAccessor})");

            using (context.SerializationCode.WriteBlock())
            {
                context.SerializationCode.WriteLine(context.WriteCall("StringSchema", $"{itemVar}.Key"));

                schema!.ValueSchema.Generate(context with
                {
                    Schema = schema!.ValueSchema,
                    SerializableTypeMetadata = dictionaryTypeMetadata.ValuesMetadata,
                    SourceAccessor = $"{itemVar}.Value"
                });
            }

            context.SerializationCode.WriteLine(context.WriteCall("LongSchema", "0L"));
            return;
        }

        var blockSizeVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}BlockSize";
        var batchVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}Batch";
        var valueType = dictionaryTypeMetadata.ValuesMetadata.FullNameDisplay;

        context.SerializationCode.WriteLine($"var {blockSizeVar} = AsyncArrayItemCountBlockSize;");
        context.SerializationCode.WriteLine($"if ({blockSizeVar} <= 0) throw new AvroSerializationException(\"AsyncArrayItemCountBlockSize must be greater than zero.\");");
        context.SerializationCode.WriteLine($"var {batchVar} = new List<global::System.Collections.Generic.KeyValuePair<string, {valueType}>>({blockSizeVar});");
        context.SerializationCode.WriteLine($"foreach(var {itemVar} in {context.SourceAccessor})");

        using (context.SerializationCode.WriteBlock())
        {
            context.SerializationCode.WriteLine($"{batchVar}.Add({itemVar});");
            context.SerializationCode.WriteLine($"if ({batchVar}.Count == {blockSizeVar})");
            using (context.SerializationCode.WriteBlock())
            {
                context.SerializationCode.WriteLine(context.WriteCall("LongSchema", $"{batchVar}.Count"));
                context.SerializationCode.WriteLine($"foreach(var {itemVar}InBlock in {batchVar})");
                using (context.SerializationCode.WriteBlock())
                {
                    context.SerializationCode.WriteLine(context.WriteCall("StringSchema", $"{itemVar}InBlock.Key"));

                    schema!.ValueSchema.Generate(context with
                    {
                        Schema = schema!.ValueSchema,
                        SerializableTypeMetadata = dictionaryTypeMetadata.ValuesMetadata,
                        SourceAccessor = $"{itemVar}InBlock.Value"
                    });
                }

                context.SerializationCode.WriteLine($"{batchVar}.Clear();");
            }
        }

        context.SerializationCode.WriteLine($"if ({batchVar}.Count > 0)");
        using (context.SerializationCode.WriteBlock())
        {
            context.SerializationCode.WriteLine(context.WriteCall("LongSchema", $"{batchVar}.Count"));
            context.SerializationCode.WriteLine($"foreach(var {batchVar}Item in {batchVar})");
            using (context.SerializationCode.WriteBlock())
            {
                context.SerializationCode.WriteLine(context.WriteCall("StringSchema", $"{batchVar}Item.Key"));

                schema!.ValueSchema.Generate(context with
                {
                    Schema = schema!.ValueSchema,
                    SerializableTypeMetadata = dictionaryTypeMetadata.ValuesMetadata,
                    SourceAccessor = $"{batchVar}Item.Value"
                });
            }
        }

        context.SerializationCode.WriteLine(context.WriteCall("LongSchema", "0L"));
    }
}
