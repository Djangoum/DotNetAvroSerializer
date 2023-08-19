using Avro;
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public class FixedGenerator
    {
        public static void GenerateSerializationSourceForFixed(FixedSchema schema, StringBuilder code, IArrayTypeSymbol byteArrayTypeSymbol, string sourceAccesor)
        {
            if (!byteArrayTypeSymbol.ElementType.Name.Equals("byte", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"Required type was not satisfied to serialize {schema.Name}");

            code.AppendLine(@$"if ({sourceAccesor}.Length != {schema.Size}) throw new AvroSerializationException(""Byte array {sourceAccesor} has to be of a fixed length of {schema.Size} but found {{{sourceAccesor}.Length}}"");");
            code.AppendLine($@"BytesSchema.Write(outputStream, {sourceAccesor});");
        }
    }
}
