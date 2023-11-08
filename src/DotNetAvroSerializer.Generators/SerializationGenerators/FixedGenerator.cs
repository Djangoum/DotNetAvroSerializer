using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class FixedGenerator
{
    internal static void GenerateSerializationSourceForFixed(AvroGenerationContext context)
    {
        var schema = context.Schema as FixedSchema;

        if (context.SerializableTypeMetadata is not IterableSerializableTypeMetadata
            { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Byte } })
            throw new AvroGeneratorException(
                $"byte[] type was not satisfied to serialize {schema!.Name} instead {context.SerializableTypeMetadata.FullNameDisplay} was found.");

        context.SerializationCode.AppendLine(@$"if ({context.SourceAccessor}.Length != {schema!.Size}) throw new AvroSerializationException(""Byte array {context.SourceAccessor} has to be of a fixed length of {schema.Size} but found {{{context.SourceAccessor}.Length}}"");");
        context.SerializationCode.AppendLine($@"BytesSchema.Write(outputStream, {context.SourceAccessor});");
    }
}
