using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal class FixedGenerator
    {
        internal static void GenerateSerializationSourceForFixed(FixedSchema schema, StringBuilder code, SerializableTypeMetadata byteArraySerializableType, string sourceAccesor)
        {
            if (byteArraySerializableType is null 
                || byteArraySerializableType is not IterableSerializableTypeMetadata iterableSerializableTypeMetadata
                || iterableSerializableTypeMetadata.ItemsTypeMetadata is not PrimitiveSerializableTypeMetadata primitiveTypeMetadata 
                || !primitiveTypeMetadata.TypeName.Equals("byte", StringComparison.InvariantCultureIgnoreCase))
                throw new AvroGeneratorException($"Required type was not satisfied to serialize {schema.Name}");

            code.AppendLine(@$"if ({sourceAccesor}.Length != {schema.Size}) throw new AvroSerializationException(""Byte array {sourceAccesor} has to be of a fixed length of {schema.Size} but found {{{sourceAccesor}.Length}}"");");
            code.AppendLine($@"BytesSchema.Write(outputStream, {sourceAccesor});");
        }
    }
}
