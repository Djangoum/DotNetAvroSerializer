using System;
using System.Linq;
using System.Runtime.CompilerServices;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;
using DotNetAvroSerializer.Generators.SerializationGenerators;

namespace DotNetAvroSerializer.Generators.Extensions;

internal static class SchemaExtensions
{
    private static void Generate(this PrimitiveSchema schema, AvroGenerationContext ctx) => PrimitiveTypesGenerator.GenerateSerializationSourceForPrimitive(ctx);

    private static void Generate(this ArraySchema schema, AvroGenerationContext ctx) => ArrayGenerator.GenerateSerializationSourceForArray(ctx);

    private static void Generate(this EnumSchema schema, AvroGenerationContext ctx) => EnumGenerator.GenerateSerializationSourceForEnum(ctx);

    private static void Generate(this FixedSchema schema, AvroGenerationContext ctx) => FixedGenerator.GenerateSerializationSourceForFixed(ctx);

    private static void Generate(this UnionSchema schema, AvroGenerationContext ctx) => UnionGenerator.GenerateSerializationSourceForUnion(ctx);

    private static void Generate(this MapSchema schema, AvroGenerationContext ctx) => MapGenerator.GenerateSerializationSourceForMap(ctx);

    private static void Generate(this LogicalSchema schema, AvroGenerationContext ctx) => LogicalTypeGenerator.GenerateSerializationSourceForLogicalType(ctx);

    private static void Generate(this RecordSchema schema, AvroGenerationContext ctx)
    {
        var recordTypeMetadata = ctx.SerializableTypeMetadata as RecordSerializableTypeMetadata;

        if (recordTypeMetadata is not null)
        {
            var duplicateAlias = recordTypeMetadata.Fields
                .SelectMany(f => f.Names.Select(alias => (Alias: alias, PropertyName: f.Name)))
                .GroupBy(a => a.Alias, StringComparer.InvariantCultureIgnoreCase)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateAlias is not null)
            {
                var properties = string.Join(", ", duplicateAlias
                    .Select(d => d.PropertyName)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .OrderBy(name => name, StringComparer.InvariantCultureIgnoreCase));
                throw new AvroGeneratorException(
                    DiagnosticsDescriptors.DuplicateAvroFieldAliasDescriptor,
                    $"AvroField alias '{duplicateAlias.Key}' is used by multiple properties in {recordTypeMetadata}: {properties}.");
            }
        }

        foreach (var field in schema.Fields)
        {
            field.Generate(ctx with { Schema = schema });
        }
    }

    private static void Generate(this Field field, AvroGenerationContext ctx)
    {
        var recordTypeMetadata = ctx.SerializableTypeMetadata as RecordSerializableTypeMetadata;
        var matchingProperties = recordTypeMetadata!.Fields.Where(f => IsFieldMatch(f, field.Name)).ToArray();

        if (matchingProperties.Length > 1)
        {
            var properties = string.Join(", ", matchingProperties
                .Select(m => m.Name)
                .OrderBy(name => name, StringComparer.InvariantCultureIgnoreCase));
            throw new AvroGeneratorException(
                DiagnosticsDescriptors.AmbiguousFieldBindingDescriptor,
                $"Avro field '{field.Name}' in {recordTypeMetadata} matches multiple properties: {properties}.");
        }

        var property = matchingProperties.FirstOrDefault();

        if (property is null)
            throw new AvroGeneratorException($"Property {field.Name} not found in {recordTypeMetadata}");

        field.Schema.Generate(ctx with { Schema = field.Schema, SerializableTypeMetadata = property.InnerSerializableType, SourceAccessor = $"{ctx.SourceAccessor}.{property.Name}" });
    }

    private static bool IsFieldMatch(FieldSerializableTypeMetadata fieldMetadata, string avroFieldName)
        => fieldMetadata.Name.Equals(avroFieldName, StringComparison.InvariantCultureIgnoreCase)
            || fieldMetadata.Names.Any(a => a.Equals(avroFieldName, StringComparison.InvariantCultureIgnoreCase));

    internal static void Generate(this Schema schema, AvroGenerationContext context)
    {
        Action<AvroGenerationContext> generator = schema switch
        {
            RecordSchema recordSchema => recordSchema.Generate,
            ArraySchema recordSchema => recordSchema.Generate,
            EnumSchema recordSchema => recordSchema.Generate,
            FixedSchema recordSchema => recordSchema.Generate,
            UnionSchema recordSchema => recordSchema.Generate,
            MapSchema recordSchema => recordSchema.Generate,
            LogicalSchema recordSchema => recordSchema.Generate,
            PrimitiveSchema recordSchema => recordSchema.Generate,

            _ => throw new UnreachableException()
        };

        generator(context);
    }
}
