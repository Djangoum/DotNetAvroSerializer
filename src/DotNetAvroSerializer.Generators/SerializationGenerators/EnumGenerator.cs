using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;

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

        context.SerializationCode.WriteLine($"var indexOfEnumValue = Array.IndexOf({schema.Name}Values, {context.SourceAccessor}.ToString());");
        context.SerializationCode.WriteLine($"if (indexOfEnumValue < 0) throw new AvroSerializationException($\"Enum value provided {{{context.SourceAccessor}}} not found in symbols for enum {schema.Name}\");");
        context.SerializationCode.WriteLine(context.WriteCall("IntSchema", "indexOfEnumValue"));
    }
}
