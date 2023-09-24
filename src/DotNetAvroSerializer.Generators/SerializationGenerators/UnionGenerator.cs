using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class UnionGenerator
    {
        internal static void GenerateSerializationSourceForUnion(AvroGenerationContext context)
        {
            var unionSchema = context.Schema as UnionSchema;
            
            foreach (var schema in unionSchema!.Schemas)
            {
                var unionSchemaIndex = unionSchema.Schemas.IndexOf(schema);

                if (context.SerializableTypeMetadata is UnionSerializableTypeMetadata unionSerializableTypeMetadata)
                {
                    var unionTypeSerializableTypeMetadata = unionSerializableTypeMetadata.UnionTypes.ElementAt(unionSchemaIndex);

                    var canSerializedCheck = GetCanSerializeCheck(schema, $"{context.SourceAccessor}.GetUnionValue()", context.CustomLogicalTypesMetadata, unionTypeSerializableTypeMetadata.FullNameDisplay);

                    if (unionSchemaIndex == 0)
                    {
                        context.SerializationCode.AppendLine($"if ({canSerializedCheck}) {{");
                    }
                    else
                    {
                        context.SerializationCode.AppendLine($"else if ({canSerializedCheck}) {{");
                    }

                    context.SerializationCode.AppendLine($"IntSchema.Write(outputStream, {unionSchemaIndex});");

                    schema.Generate(context with
                    {
                        Schema = schema,
                        SerializableTypeMetadata = unionTypeSerializableTypeMetadata,
                        SourceAccessor = $"(({unionTypeSerializableTypeMetadata.FullNameDisplay}){context.SourceAccessor})"
                    });
                }
                else if (context.SerializableTypeMetadata is NullableSerializableTypeMetadata nullableSerializableTypeMetadata)
                {
                    var canSerializedCheck = GetCanSerializeCheck(schema, context.SourceAccessor, context.CustomLogicalTypesMetadata);

                    if (unionSchemaIndex == 0)
                    {
                        context.SerializationCode.AppendLine($"if ({canSerializedCheck}) {{");
                    }
                    else
                    {
                        context.SerializationCode.AppendLine($"else if ({canSerializedCheck}) {{");
                    }

                    context.SerializationCode.AppendLine($"IntSchema.Write(outputStream, {unionSchemaIndex});");
                    
                    schema.Generate(context with
                    {
                        Schema = schema,
                        SerializableTypeMetadata = nullableSerializableTypeMetadata.InnerNullableTypeSymbol
                    });
                }

                context.SerializationCode.AppendLine("}");
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
                    _ => throw new Exception($"Required type was not satisfied to serialize {primitiveSchema.Name}")
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
}
