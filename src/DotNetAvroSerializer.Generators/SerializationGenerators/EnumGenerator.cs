using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class EnumGenerator
{
    internal static void GenerateSerializationSourceForEnum(AvroGenerationContext context)
    {
        var schema = context.Schema as EnumSchema;

        if (context.SerializableTypeMetadata is not EnumSerializableTypeMetadata)
            throw new AvroGeneratorException(
                $"Enum type was not satisfied to serialize {schema!.Name} instead {context.SerializableTypeMetadata.FullNameDisplay} was found");

        context.PrivateFieldsCode.AppendLine(schema!.Name, $"private readonly string[] {schema.Name}Values = new string[] {{ \"{string.Join(@""",""", schema.Symbols)}\" }};");

        context.SerializationCode.AppendLine($@"var indexOfEnumValue = Array.IndexOf({schema.Name}Values, {context.SourceAccessor}.ToString());
if (indexOfEnumValue < 0) throw new AvroSerializationException($""Enum value provided {{{context.SourceAccessor}}} not found in symbols for enum {schema.Name}"");
IntSchema.Write(outputStream, indexOfEnumValue);");
    }
}
