using Avro;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class MapGenerator
    {
        public static void GenerateSerializationSourceFoMap(MapSchema schema, StringBuilder code, GeneratorExecutionContext context, INamedTypeSymbol dictionarySymbol, string sourceAccesor)
        {
            if (!(dictionarySymbol.AllInterfaces.Any(i => i.Name.Contains("Dictionary")) || dictionarySymbol.Name.Contains("Dictionary")))
                throw new Exception("Type for map schema is not satisfied");

            if (!dictionarySymbol.TypeArguments.First().Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Map keys have to be strings");

            code.AppendLine($@"LongSchema.Write(outputStream, {sourceAccesor}.Count());");
            code.AppendLine($@"foreach(var item in {sourceAccesor})");
            code.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ValueSchema, code, context, dictionarySymbol.TypeArguments.ElementAt(1), "item.Value");

            code.AppendLine("}");
            code.AppendLine("LongSchema.Write(outputStream, 0L);");
        }
    }
}
