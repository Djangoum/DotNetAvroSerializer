using Avro;
using AvroSerializer.Generators.Exceptions;
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class PrimitiveTypesGenerator
    {
        public static void GenerateSerializationSourceForPrimitive(PrimitiveSchema primitiveSchema, StringBuilder code, ISymbol originTypeSymbol, string sourceAccesor = "source")
        {
            var serializerCallCode = primitiveSchema.Name switch
            {
                "boolean" when originTypeSymbol.Name.Equals("bool", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeSymbol.Name.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeSymbol.Name.Equals("bool?", StringComparison.InvariantCultureIgnoreCase) => $"BooleanSchema.Write(outputStream, {sourceAccesor});",
                "int" when originTypeSymbol.Name.Equals("int", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeSymbol.Name.Equals("Int32", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeSymbol.Name.Equals("int?", StringComparison.InvariantCultureIgnoreCase) => $"IntSchema.Write(outputStream, {sourceAccesor});",
                "long" when originTypeSymbol.Name.Equals("long", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeSymbol.Name.Equals("Int64", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeSymbol.Name.Equals("long?", StringComparison.InvariantCultureIgnoreCase) => $"LongSchema.Write(outputStream, {sourceAccesor});",
                "string" when originTypeSymbol.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeSymbol.Name.Equals("string?", StringComparison.InvariantCultureIgnoreCase) => $"StringSchema.Write(outputStream, {sourceAccesor});",
                "bytes" when originTypeSymbol is IArrayTypeSymbol arrayTypeSymbol
                            && arrayTypeSymbol.ElementType.Name.Equals("byte", StringComparison.InvariantCultureIgnoreCase) => $"BytesSchema.Write(outputStream, {sourceAccesor});",
                "double" when originTypeSymbol.Name.Equals("double", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeSymbol.Name.Equals("double?", StringComparison.InvariantCultureIgnoreCase) => $"DoubleSchema.Write(outputStream, {sourceAccesor});",
                "float" when originTypeSymbol.Name.Equals("single", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeSymbol.Name.Equals("single?", StringComparison.InvariantCultureIgnoreCase) => $"FloatSchema.Write(outputStream, {sourceAccesor});",
                "null" => $"NullSchema.Write(outputStream, {sourceAccesor});",
                _ => throw new AvroGeneratorException($"Required type was not satisfied to serialize {primitiveSchema.Name}")
            };

            code.AppendLine(serializerCallCode);
        }
    }
}
