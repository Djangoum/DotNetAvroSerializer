using Avro;
using Microsoft.CodeAnalysis;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class ArrayGenerator
    {
        public static void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder code, GeneratorExecutionContext context, IArrayTypeSymbol originTypeSymbol, string sourceAccesor)
        {
            code.AppendLine(@$"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            code.AppendLine($@"foreach(var item in {sourceAccesor})");
            code.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ItemSchema, code, context, originTypeSymbol.ElementType, "item");

            code.AppendLine("}");
            code.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }
    }
}
