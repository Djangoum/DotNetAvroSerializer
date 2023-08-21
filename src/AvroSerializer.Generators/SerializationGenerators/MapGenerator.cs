using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    public static class MapGenerator
    {
        public static void GenerateSerializationSourceFoMap(MapSchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SourceProductionContext context, INamedTypeSymbol dictionarySymbol, string sourceAccesor)
        {
            if (!(dictionarySymbol.AllInterfaces.Any(i => i.Name.Contains("Dictionary")) || dictionarySymbol.Name.Contains("Dictionary")))
                throw new AvroGeneratorException("Type for map schema is not satisfied");

            if (!dictionarySymbol.TypeArguments.First().Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
                throw new AvroGeneratorException("Map keys have to be strings");

            serializationCode.AppendLine($@"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, {sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)} in {sourceAccesor})");
            serializationCode.AppendLine("{");

            serializationCode.AppendLine($@"StringSchema.Write(outputStream, item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}.Key);");
            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ValueSchema, serializationCode, privateFieldsCode, context, dictionarySymbol.TypeArguments.ElementAt(1), $"item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}.Value");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine("LongSchema.Write(outputStream, 0L);");
        }
    }
}
