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
    public static class ArrayGenerator
    {
        public static void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SourceProductionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            ITypeSymbol arrayContentTypeSymbol;

            if (originTypeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                arrayContentTypeSymbol = arrayTypeSymbol.ElementType;
            }
            else if (originTypeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.AllInterfaces.Any(i => i.Name.Contains("IEnumerable")))
            {
                arrayContentTypeSymbol = namedTypeSymbol.TypeArguments.First();
            }
            else
                throw new AvroGeneratorException($"Array type for {schema.Name} is not satisfied {originTypeSymbol.Name} provided, arrays must be arrays or anything that implements IEnumerable");

            serializationCode.AppendLine(@$"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)} in {sourceAccesor})");
            serializationCode.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ItemSchema, serializationCode, privateFieldsCode, context, arrayContentTypeSymbol, $"item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }
    }
}
