using System.Collections.Generic;
using System.Linq;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class UnionGenerator
{
    internal static void GenerateSerializationSourceForUnion(AvroGenerationContext context)
    {
        var unionSchema = context.Schema as UnionSchema;

        if (context.SerializableTypeMetadata is UnionSerializableTypeMetadata unionSerializableTypeMetadata)
        {
            GenerateExplicitUnionSerialization(context, unionSchema!, unionSerializableTypeMetadata);
            return;
        }

        if (context.SerializableTypeMetadata is NullableSerializableTypeMetadata
            or RecordSerializableTypeMetadata { IsNullable: true }
            or DictionarySerializableTypeMetadata
            or IterableSerializableTypeMetadata)
        {
            GenerateImplicitUnionSerialization(context, unionSchema!);
            return;
        }

        throw new AvroGeneratorException($"Union schema {unionSchema!.Name} is not compatible with {context.SerializableTypeMetadata.FullNameDisplay}");
    }

    private static void GenerateExplicitUnionSerialization(
        AvroGenerationContext context,
        UnionSchema unionSchema,
        UnionSerializableTypeMetadata unionSerializableTypeMetadata)
    {
        context.SerializationCode.WriteLine($"switch ({context.SourceAccessor}.Index)");

        using (context.SerializationCode.WriteBlock())
        {
            for (var unionSchemaIndex = 0; unionSchemaIndex < unionSchema.Schemas.Count; unionSchemaIndex++)
            {
                var schema = unionSchema.Schemas[unionSchemaIndex];
                var unionTypeSerializableTypeMetadata = unionSerializableTypeMetadata.UnionTypes.ElementAt(unionSchemaIndex);

                context.SerializationCode.WriteLine($"case {unionSchemaIndex + 1}:");
                using (context.SerializationCode.WriteBlock())
                {
                    context.SerializationCode.WriteLine(context.WriteCall("IntSchema", unionSchemaIndex.ToString()));
                    context.SerializationCode.WriteLine($"var branchValue = {context.SourceAccessor}.Value{unionSchemaIndex + 1};");

                    schema.Generate(context with
                    {
                        Schema = schema,
                        SerializableTypeMetadata = unionTypeSerializableTypeMetadata,
                        SourceAccessor = "branchValue"
                    });

                    context.SerializationCode.WriteLine("break;");
                }
            }

            context.SerializationCode.WriteLine("default:");
            using (context.SerializationCode.WriteBlock())
            {
                context.SerializationCode.WriteLine($"throw new AvroSerializationException(\"Invalid union branch index {{{context.SourceAccessor}.Index}} for {context.SerializableTypeMetadata.FullNameDisplay}\");");
            }
        }
    }

    private static void GenerateImplicitUnionSerialization(AvroGenerationContext context, UnionSchema unionSchema)
    {
        var effectiveTypeMetadata = context.SerializableTypeMetadata is NullableSerializableTypeMetadata nullableSerializableTypeMetadata
            ? nullableSerializableTypeMetadata.InnerNullableTypeSymbol
            : context.SerializableTypeMetadata;

        for (var unionSchemaIndex = 0; unionSchemaIndex < unionSchema.Schemas.Count; unionSchemaIndex++)
        {
            var schema = unionSchema.Schemas[unionSchemaIndex];

            var useRecordPatternMatch = schema is RecordSchema && effectiveTypeMetadata is RecordSerializableTypeMetadata;

            var canSerializedCheck = useRecordPatternMatch
                ? $"{context.SourceAccessor} is {effectiveTypeMetadata.FullNameDisplay} actualRecord"
                : GetCanSerializeCheck(schema, context.SourceAccessor, context.CustomLogicalTypesMetadata, effectiveTypeMetadata.FullNameDisplay);

            context.SerializationCode.WriteLine(unionSchemaIndex == 0
                ? $"if ({canSerializedCheck})"
                : $"else if ({canSerializedCheck})");

            using (context.SerializationCode.WriteBlock())
            {
                context.SerializationCode.WriteLine(context.WriteCall("IntSchema", unionSchemaIndex.ToString()));

                schema.Generate(context with
                {
                    Schema = schema,
                    SerializableTypeMetadata = effectiveTypeMetadata,
                    SourceAccessor = useRecordPatternMatch ? "actualRecord" : context.SourceAccessor
                });
            }
        }
    }

    private static string GetCanSerializeCheck(Schema schema, string sourceAccesor, IEnumerable<CustomLogicalTypeMetadata> customLogicalTypes, string typeFullName = null)
    {
        return schema switch
        {
            PrimitiveSchema primitiveSchema => primitiveSchema.Name switch
            {
                "boolean" => $"BooleanSchema.CanSerialize({sourceAccesor})",
                "int" => $"IntSchema.CanSerialize({sourceAccesor})",
                "long" => $"LongSchema.CanSerialize({sourceAccesor})",
                "string" => $"StringSchema.CanSerialize({sourceAccesor})",
                "bytes" => $"BytesSchema.CanSerialize({sourceAccesor})",
                "double" => $"DoubleSchema.CanSerialize({sourceAccesor})",
                "float" => $"FloatSchema.CanSerialize({sourceAccesor})",
                "null" => $"NullSchema.CanSerialize({sourceAccesor})",
                _ => throw new AvroGeneratorException(
                    $"Required type was not satisfied to serialize {primitiveSchema.Name}")
            },
            LogicalSchema logicalSchema => logicalSchema.LogicalTypeName switch
            {
                "date" => $"DateSchema.CanSerialize({sourceAccesor})",
                "uuid" => $"UuidSchema.CanSerialize({sourceAccesor})",
                "time-millis" => $"TimeMillisSchema.CanSerialize({sourceAccesor})",
                "timestamp-millis" => $"TimestampMillisSchema.CanSerialize({sourceAccesor})",
                "local-timestamp-millis" => $"TimestampMillisSchema.CanSerialize({sourceAccesor})",
                "time-micros" => $"TimeMillisSchema.CanSerialize({sourceAccesor})",
                "timestamp-micros" => $"TimestampMillisSchema.CanSerialize({sourceAccesor})",
                "local-timestamp-micros" => $"TimestampMillisSchema.CanSerialize({sourceAccesor})",
                _ when customLogicalTypes.Any(l => l.Name.Equals(logicalSchema.LogicalTypeName))
                    => GetCustomLogicalTypeCanSerializeCheck(logicalSchema, customLogicalTypes.First(l => l.Name.Equals(logicalSchema.LogicalTypeName)), sourceAccesor),
                _ => GetCanSerializeCheck(logicalSchema.BaseSchema, sourceAccesor, customLogicalTypes)
            },
            RecordSchema => $"RecordSchema.CanSerialize<{typeFullName}>({sourceAccesor})",
            ArraySchema => $"ArraySchema.CanSerialize({sourceAccesor})",
            FixedSchema => $"FixedSchema.CanSerialize({sourceAccesor})",
            MapSchema => $"MapSchema.CanSerialize({sourceAccesor})",
            UnionSchema => throw new AvroGeneratorException("Unions cannot hold directly unions"),
            _ => null
        };
    }

    private static string GetCustomLogicalTypeCanSerializeCheck(LogicalSchema logicalSchema, CustomLogicalTypeMetadata logicalTypeMetadata, string sourceAccessor)
    {
        var logicalTypesProperties = logicalTypeMetadata.OrderedSchemaPropertiesCanSerialize.Select(logicalSchema.GetProperty).Where(v => v is not null);

        if (!logicalTypesProperties.Count().Equals(logicalTypeMetadata.OrderedSchemaPropertiesCanSerialize.Count()))
            throw new AvroGeneratorException("Logical type properties could not be mapped");

        return $"{logicalTypeMetadata.LogicalTypeFullyQualifiedName}.CanSerialize({sourceAccessor}{(logicalTypesProperties.Any() ? "," + string.Join(",", logicalTypesProperties) : "")})";
    }
}
