using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class UnionGenerator
    {
        internal static void GenerateSerializationSourceForUnion(UnionSchema unionSchema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata originTypeSymbol, string sourceAccesor)
        {
            foreach (var schema in unionSchema.Schemas)
            {
                var canSerializedCheck = GetCanSerializeCheck(schema, sourceAccesor);

                if (unionSchema.Schemas.IndexOf(schema) == 0)
                {
                    serializationCode.AppendLine($@"if ({canSerializedCheck}) {{");
                }
                else
                {
                    serializationCode.AppendLine($@"else if ({canSerializedCheck}) {{");
                }
                serializationCode.AppendLine($"IntSchema.Write(outputStream, {unionSchema.Schemas.IndexOf(schema)});");

                var newSymbol = originTypeSymbol switch
                {
                    NullableSerializableTypeMetadata nullableSerializableType => nullableSerializableType.InnerNullableTypeSymbol,
                    _ => originTypeSymbol
                };

                SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializationCode, privateFieldsCode, newSymbol, sourceAccesor);

                serializationCode.AppendLine("}");
            }
        }

        private static string GetCanSerializeCheck(Schema schema, string sourceAccesor)
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
                    "timestamp-millis" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",
                    "local-timestamp-milis" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",
                    "time-micros" => $"TimeMillisSchema.CanSerialize({sourceAccesor})",
                    "timestamp-micros" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",
                    "local-timestamp-micros" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",

                    _ => GetCanSerializeCheck(logicalSchema.BaseSchema, sourceAccesor)
                },
                RecordSchema => $"RecordSchema.CanSerialize({sourceAccesor})",
                ArraySchema => $"ArraySchema.CanSerialize({sourceAccesor})",
                FixedSchema => $"FixedSchema.CanSerialize({sourceAccesor})",
                MapSchema => $"MapSchema.CanSerialize({sourceAccesor})",
                UnionSchema => throw new AvroGeneratorException("Unions cannot hold directly unions"),
                _ => null
            };
        }
    }
}
