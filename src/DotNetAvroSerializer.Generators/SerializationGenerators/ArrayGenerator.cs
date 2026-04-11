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
        var iterableItemsType = iterableSerializableTypeMetadata.ItemsTypeMetadata.FullNameDisplay;

        if (context.Mode is SerializationMode.Sync)
        {
            if (iterableSerializableTypeMetadata.IsAsyncEnumerable)
            {
                throw new AvroGeneratorException(
                    $"Type {iterableSerializableTypeMetadata.FullNameDisplay} cannot be serialized by sync serializers because it contains IAsyncEnumerable data. Use AsyncAvroSerializer<T> instead or materialize it to a sync collection that implements IEnumerable.");
            }

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
            return;
        }

        var blockSizeVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}BlockSize";
        var batchVar = $"{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}Batch";

        context.SerializationCode.WriteLine($"var {blockSizeVar} = AsyncArrayItemCountBlockSize;");
        context.SerializationCode.WriteLine($"if ({blockSizeVar} <= 0) throw new AvroSerializationException(\"AsyncArrayItemCountBlockSize must be greater than zero.\");");
        context.SerializationCode.WriteLine($"var {batchVar} = new List<{iterableItemsType}>({blockSizeVar});");

        var iteratorExpression = iterableSerializableTypeMetadata.IsAsyncEnumerable
            ? $"{context.SourceAccessor}.WithCancellation(cancellationToken)"
            : context.SourceAccessor;
        var iteratorKeyword = iterableSerializableTypeMetadata.IsAsyncEnumerable
            ? "await foreach"
            : "foreach";

        context.SerializationCode.WriteLine($"{iteratorKeyword}(var {itemVar} in {iteratorExpression})");
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
                    schema!.ItemSchema.Generate(context with
                    {
                        Schema = schema!.ItemSchema,
                        SourceAccessor = $"{itemVar}InBlock",
                        SerializableTypeMetadata = iterableSerializableTypeMetadata.ItemsTypeMetadata
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
                schema.ItemSchema.Generate(context with
                {
                    Schema = schema.ItemSchema,
                    SourceAccessor = $"{batchVar}Item",
                    SerializableTypeMetadata = iterableSerializableTypeMetadata.ItemsTypeMetadata
                });
            }
        }

        context.SerializationCode.WriteLine(context.WriteCall("LongSchema", "0L"));
    }
}
