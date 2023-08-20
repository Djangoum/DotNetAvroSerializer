using Avro;
using AvroSerializer.Generators.Exceptions;
using AvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class ArrayGenerator
    {
        public static void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
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
                throw new AvroGeneratorException($"Array type for {schema.Name} is not satisfied {originTypeSymbol.Name} provided");

            serializationCode.AppendLine(@$"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item in {sourceAccesor})");
            serializationCode.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ItemSchema, serializationCode, privateFieldsCode, context, arrayContentTypeSymbol, "item");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }
    }
}
