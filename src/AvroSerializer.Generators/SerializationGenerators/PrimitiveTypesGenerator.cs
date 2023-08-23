using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class PrimitiveTypesGenerator
    {
        internal static void GenerateSerializationSourceForPrimitive(PrimitiveSchema primitiveSchema, StringBuilder code, SerializableTypeMetadata serializableTypeMetadata, string sourceAccesor = "source")
        {
            if (serializableTypeMetadata is null)
                throw new AvroGeneratorException($"Primitive type was not satisfied {serializableTypeMetadata}");

            var serializerCallCode = primitiveSchema.Name switch
            {
                "boolean" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata 
                            && (primitiveTypeMetadata.TypeName.Equals("bool", StringComparison.InvariantCultureIgnoreCase)
                           || primitiveTypeMetadata.TypeName.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase)) => $"BooleanSchema.Write(outputStream, {sourceAccesor});",
                "int" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                            && (primitiveTypeMetadata.TypeName.Equals("int", StringComparison.InvariantCultureIgnoreCase)
                           || primitiveTypeMetadata.TypeName.Equals("Int32", StringComparison.InvariantCultureIgnoreCase)) => $"IntSchema.Write(outputStream, {sourceAccesor});",
                "long" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                            && (primitiveTypeMetadata.TypeName.Equals("long", StringComparison.InvariantCultureIgnoreCase)
                            || primitiveTypeMetadata.TypeName.Equals("Int64", StringComparison.InvariantCultureIgnoreCase)) => $"LongSchema.Write(outputStream, {sourceAccesor});",
                "string" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                            && (primitiveTypeMetadata.TypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase)) => $"StringSchema.Write(outputStream, {sourceAccesor});",
                "bytes" when serializableTypeMetadata is IterableSerializableTypeMetadata iterableTypeMetadata
                            && iterableTypeMetadata.ItemsTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata 
                            && primitiveTypeMetadata.TypeName.Equals("byte", StringComparison.InvariantCultureIgnoreCase) => $"BytesSchema.Write(outputStream, {sourceAccesor});",
                "double" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                            && primitiveTypeMetadata.TypeName.Equals("double", StringComparison.InvariantCultureIgnoreCase) => $"DoubleSchema.Write(outputStream, {sourceAccesor});",
                "float" when serializableTypeMetadata is PrimitiveSerializableTypeMetadata primitiveTypeMetadata
                            && primitiveTypeMetadata.TypeName.Equals("single", StringComparison.InvariantCultureIgnoreCase) => $"FloatSchema.Write(outputStream, {sourceAccesor});",
                "null" => $"NullSchema.Write(outputStream, {sourceAccesor});",
                _ => throw new AvroGeneratorException($"Required type was not satisfied to serialize {primitiveSchema.Name} {serializableTypeMetadata.ToString()} found")
            };

            code.AppendLine(serializerCallCode);
        }
    }
}
