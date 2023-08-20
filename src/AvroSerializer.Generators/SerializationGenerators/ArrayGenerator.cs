using Avro;
using AvroSerializer.Generators.Exceptions;
using AvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class ArrayGenerator
    {
        public static void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, GeneratorExecutionContext context, IArrayTypeSymbol originTypeSymbol, string sourceAccesor)
        {
            if (originTypeSymbol is null)
                throw new AvroGeneratorException($"Array type for {schema.Name} is not satisfied {originTypeSymbol.Name} provided");

            serializationCode.AppendLine(@$"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item in {sourceAccesor})");
            serializationCode.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ItemSchema, serializationCode, privateFieldsCode, context, originTypeSymbol.ElementType, "item");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }
    }
}
