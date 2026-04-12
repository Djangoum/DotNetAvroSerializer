using System.Collections.Generic;
using System.Linq;
using DotNetAvroSerializer.Generators.Diagnostics;
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
            var unionTypes = unionSerializableTypeMetadata.UnionTypes.AsImmutableArray();

            if (unionTypes.Length != unionSchema!.Schemas.Count)
            {
                throw new AvroGeneratorException(
                    DiagnosticsDescriptors.UnionSchemaOrderMismatchDescriptor,
                    $"Union type {context.SerializableTypeMetadata.FullNameDisplay} has {unionTypes.Length} members but Avro union has {unionSchema.Schemas.Count} schemas.");
            }

            for (var i = 0; i < unionSchema.Schemas.Count; i++)
            {
                if (IsSchemaCompatibleWithType(unionSchema.Schemas[i], unionTypes[i]))
                    continue;

                throw new AvroGeneratorException(
                    DiagnosticsDescriptors.UnionSchemaOrderMismatchDescriptor,
                    $"Union member order mismatch at index {i}: schema '{unionSchema.Schemas[i].Name}' does not match type '{unionTypes[i].FullNameDisplay}'.");
            }
        }
        else if (IsNullableSerializableType(context.SerializableTypeMetadata) && unionSchema!.Schemas.All(s => s is not PrimitiveSchema { Name: "null" }))
        {
            throw new AvroGeneratorException(
                DiagnosticsDescriptors.UnsupportedNullablePatternDescriptor,
                $"Nullable type {context.SerializableTypeMetadata.FullNameDisplay} requires an Avro union containing 'null'.");
        }

        for (var unionSchemaIndex = 0; unionSchemaIndex < unionSchema!.Schemas.Count; unionSchemaIndex++)
        {
            var schema = unionSchema.Schemas[unionSchemaIndex];

            if (context.SerializableTypeMetadata is UnionSerializableTypeMetadata currentUnionMetadata)
            {
                var unionTypeSerializableTypeMetadata = currentUnionMetadata.UnionTypes.ElementAt(unionSchemaIndex);
                if (unionSchemaIndex == 0)
                {
                    context.SerializationCode.WriteLine($"switch ({context.SourceAccessor}.Index)");
                    context.SerializationCode.WriteLine("{");
                }

                context.SerializationCode.WriteLine($"case {unionSchemaIndex + 1}:");
                context.SerializationCode.WriteLine("{");
                context.SerializationCode.WriteLine(context.WriteCall("IntSchema", unionSchemaIndex.ToString()));

                var unionValueAccessor = $"unionValue{unionSchemaIndex + 1}";
                context.SerializationCode.WriteLine(
                    $"var {unionValueAccessor} = ({unionTypeSerializableTypeMetadata.FullNameDisplay}){context.SourceAccessor}.Value{unionSchemaIndex + 1}!;");

                schema.Generate(context with
                {
                    Schema = schema,
                    SerializableTypeMetadata = unionTypeSerializableTypeMetadata,
                    SourceAccessor = unionValueAccessor
                });

                context.SerializationCode.WriteLine("break;");
                context.SerializationCode.WriteLine("}");

                if (unionSchemaIndex == unionSchema.Schemas.Count - 1)
                {
                    context.SerializationCode.WriteLine("default:");
                    context.SerializationCode.WriteLine("{");
                    context.SerializationCode.WriteLine($"throw new AvroSerializationException(\"Union index {{ {context.SourceAccessor}.Index }} is not valid for {context.SerializableTypeMetadata.FullNameDisplay}.\");");
                    context.SerializationCode.WriteLine("}");
                    context.SerializationCode.WriteLine("}");
                }
            }
            else if (context.SerializableTypeMetadata is NullableSerializableTypeMetadata or RecordSerializableTypeMetadata { IsNullable: true } or DictionarySerializableTypeMetadata or IterableSerializableTypeMetadata)
            {
                var nullableSerializableTypeMetadata = context.SerializableTypeMetadata as NullableSerializableTypeMetadata;
                var effectiveSerializableTypeMetadata = nullableSerializableTypeMetadata?.InnerNullableTypeSymbol ?? context.SerializableTypeMetadata;
                var isRecordSchema = schema is RecordSchema;
                var actualRecordVariableName = $"actualRecord{unionSchemaIndex}";

                var canSerializedCheck = isRecordSchema
                    ? $"{context.SourceAccessor} is {effectiveSerializableTypeMetadata.FullNameDisplay} {actualRecordVariableName}"
                    : GetCanSerializeCheck(schema, context.SourceAccessor, context.CustomLogicalTypesMetadata, context.SerializableTypeMetadata.FullNameDisplay);

                context.SerializationCode.WriteLine(unionSchemaIndex == 0
                    ? $"if ({canSerializedCheck})"
                    : $"else if ({canSerializedCheck})");

                using (context.SerializationCode.WriteBlock())
                {
                    context.SerializationCode.WriteLine(context.WriteCall("IntSchema", unionSchemaIndex.ToString()));

                    schema.Generate(context with
                    {
                        Schema = schema,
                        SerializableTypeMetadata = effectiveSerializableTypeMetadata,
                        SourceAccessor = isRecordSchema ? actualRecordVariableName : context.SourceAccessor
                    });
                }
            }
            else
            {
                throw new AvroGeneratorException(
                    $"Union index {unionSchemaIndex} for schema {schema.Name} instead {context.SerializableTypeMetadata.FullNameDisplay}");
            }
        }
    }

    private static bool IsNullableSerializableType(SerializableTypeMetadata serializableTypeMetadata)
        => serializableTypeMetadata is NullableSerializableTypeMetadata
            or RecordSerializableTypeMetadata { IsNullable: true }
            or DictionarySerializableTypeMetadata
            or IterableSerializableTypeMetadata;

    private static bool IsSchemaCompatibleWithType(Schema schema, SerializableTypeMetadata typeMetadata)
        => schema switch
        {
            PrimitiveSchema { Name: "boolean" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Boolean },
            PrimitiveSchema { Name: "int" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int32 },
            PrimitiveSchema { Name: "long" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Int64 },
            PrimitiveSchema { Name: "string" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_String },
            PrimitiveSchema { Name: "bytes" } => typeMetadata is IterableSerializableTypeMetadata { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Byte } },
            PrimitiveSchema { Name: "double" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Double },
            PrimitiveSchema { Name: "float" } => typeMetadata is PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Single },
            PrimitiveSchema { Name: "null" } => IsNullableSerializableType(typeMetadata) || typeMetadata.IsNullable || IsDotNetAvroNullType(typeMetadata),
            LogicalSchema logicalSchema => typeMetadata is LogicalTypeSerializableTypeMetadata || IsSchemaCompatibleWithType(logicalSchema.BaseSchema, typeMetadata),
            RecordSchema => typeMetadata is RecordSerializableTypeMetadata,
            ArraySchema => typeMetadata is IterableSerializableTypeMetadata,
            FixedSchema => typeMetadata is IterableSerializableTypeMetadata { ItemsTypeMetadata: PrimitiveSerializableTypeMetadata { SpecialType: Microsoft.CodeAnalysis.SpecialType.System_Byte } },
            MapSchema => typeMetadata is DictionarySerializableTypeMetadata,
            UnionSchema => false,
            _ => false
        };

    private static bool IsDotNetAvroNullType(SerializableTypeMetadata typeMetadata)
        => typeMetadata.FullNameDisplay == "global::DotNetAvroSerializer.Null";

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
