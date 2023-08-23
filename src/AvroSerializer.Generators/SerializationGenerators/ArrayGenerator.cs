using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class ArrayGenerator
    {
        internal static void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata serializableType, string sourceAccesor)
        {
            if (serializableType is null || serializableType is not IterableSerializableTypeMetadata iterableSerializableTypeMetadata)
                throw new AvroGeneratorException($"Array type for {schema.Name} is not satisfied {serializableType.GetType().Name} provided, arrays must be arrays or anything that implements IEnumerable");

            serializationCode.AppendLine(@$"if ({sourceAccesor}.Count() > 0) LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            serializationCode.AppendLine($@"foreach(var item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)} in {sourceAccesor})");
            serializationCode.AppendLine("{");

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema.ItemSchema, serializationCode, privateFieldsCode, iterableSerializableTypeMetadata.ItemsTypeMetadata, $"item{VariableNamesHelpers.RemoveSpecialCharacters(sourceAccesor)}");

            serializationCode.AppendLine("}");
            serializationCode.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }
    }
}
