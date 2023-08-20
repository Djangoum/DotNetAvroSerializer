using Avro;
using AvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    public static class EnumGenerator
    {
        public static void GenerateSerializationSourceForEnum(EnumSchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            var symbol = originTypeSymbol as ITypeSymbol;

            if (symbol.TypeKind != TypeKind.Enum)
                throw new AvroGeneratorException($"Enum type was not satisfied to serialize {schema.Name}");

            privateFieldsCode.AppendLine(schema.Name, $"private readonly string[] {schema.Name}Values = new string[] {{ \"{string.Join(@""",""", schema.Symbols)}\" }};");

            serializationCode.AppendLine($@"var indexOfEnumValue = Array.IndexOf({schema.Name}Values, {sourceAccesor}.ToString());
if (indexOfEnumValue < 0) throw new AvroSerializationException($""Enum value provided {{{sourceAccesor}}} not found in symbols for enum {schema.Name}"");
IntSchema.Write(outputStream, indexOfEnumValue);");
        }
    }
}
