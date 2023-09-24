using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Models;
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

    public static void Generate(this LogicalSchema schema, AvroGenerationContext ctx) => LogicalTypeGenerator.GenerateSerializationSourceForLogicalType(ctx);

    private static void Generate(this RecordSchema schema, AvroGenerationContext ctx)
    {
        foreach (var field in schema.Fields)
        {
            field.Generate(ctx with { Schema = schema });
        }
    }

    private static void Generate(this Field field, AvroGenerationContext ctx)
    {
        var recordTypeMetadata = ctx.SerializableTypeMetadata as RecordSerializableTypeMetadata;
        var property = recordTypeMetadata!.Fields.FirstOrDefault(f => 
            f.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)
            || f.Names.Contains(field.Name)
        );

        if (property is null)
            throw new AvroGeneratorException($"Property {field.Name} not found in {recordTypeMetadata}");

        field.Schema.Generate(ctx with { Schema = field.Schema, SerializableTypeMetadata = property.InnerSerializableType , SourceAccessor = $"{ctx.SourceAccessor}.{property.Name}"  });
    }
    
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