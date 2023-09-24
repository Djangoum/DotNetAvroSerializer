using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using System;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class MapGenerator
    {
        internal static void GenerateSerializationSourceForMap(AvroGenerationContext context)
        {
            var schema = context.Schema as MapSchema;
            
            if (context.SerializableTypeMetadata is not DictionarySerializableTypeMetadata dictionaryTypeMetadata)
                throw new AvroGeneratorException($"Type for map schema was not satisfied. Maps must implement IDictionary but {context.SerializableTypeMetadata} found");

            if (!dictionaryTypeMetadata.KeysTypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase))
                throw new AvroGeneratorException($"Map keys have to be strings but {dictionaryTypeMetadata.KeysTypeName}");

            context.SerializationCode.AppendLine($"if ({context.SourceAccessor}.Count() > 0) LongSchema.Write(outputStream, {context.SourceAccessor}.Count());");
            context.SerializationCode.AppendLine($"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)} in {context.SourceAccessor})");
            context.SerializationCode.AppendLine("{");

            context.SerializationCode.AppendLine($"StringSchema.Write(outputStream, item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}.Key);");
            
            schema!.ValueSchema.Generate(context with
            {
                Schema = schema!.ValueSchema,
                SerializableTypeMetadata = dictionaryTypeMetadata.ValuesMetadata,
                SourceAccessor = $"item{VariableNamesHelpers.RemoveSpecialCharacters(context.SourceAccessor)}.Value"
            });

            context.SerializationCode.AppendLine("}");
            context.SerializationCode.AppendLine("LongSchema.Write(outputStream, 0L);");
        }
    }
}
