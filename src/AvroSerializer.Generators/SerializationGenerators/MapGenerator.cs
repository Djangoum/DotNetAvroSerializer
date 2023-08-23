using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class MapGenerator
    {
        internal static void GenerateSerializationSourceFoMap(MapSchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata dictionarySymbol, string sourceAccesor)
        {
            if (dictionarySymbol is null || dictionarySymbol is not DictionarySerializableTypeMetadata dictonaryTypeMetadata)
                throw new AvroGeneratorException($"Type for map schema was not satisfied. Maps must implement IDictionary but {dictionarySymbol} found");

            if (!dictonaryTypeMetadata.KeysTypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase))
                throw new AvroGeneratorException($"Map keys have to be strings but {dictonaryTypeMetadata.KeysTypeName}");

            serializationCode.AppendLine($@"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, {sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)} in {sourceAccesor})");
            serializationCode.AppendLine("{");

            serializationCode.AppendLine($@"StringSchema.Write(outputStream, item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}.Key);");
            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ValueSchema, serializationCode, privateFieldsCode, dictonaryTypeMetadata.ValuesMetadata, $"item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}.Value");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine("LongSchema.Write(outputStream, 0L);");
        }
    }
}
