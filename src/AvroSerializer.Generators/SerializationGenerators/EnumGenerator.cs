using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class EnumGenerator
    {
        internal static void GenerateSerializationSourceForEnum(EnumSchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata originTypeSymbol, string sourceAccesor)
        {
            if (originTypeSymbol is null || originTypeSymbol is not EnumSerializableTypeMetadata)
                throw new AvroGeneratorException($"Enum type was not satisfied to serialize {schema.Name}");

            privateFieldsCode.AppendLine(schema.Name, $"private readonly string[] {schema.Name}Values = new string[] {{ \"{string.Join(@""",""", schema.Symbols)}\" }};");

            serializationCode.AppendLine($@"var indexOfEnumValue = Array.IndexOf({schema.Name}Values, {sourceAccesor}.ToString());
if (indexOfEnumValue < 0) throw new AvroSerializationException($""Enum value provided {{{sourceAccesor}}} not found in symbols for enum {schema.Name}"");
IntSchema.Write(outputStream, indexOfEnumValue);");
        }
    }
}
