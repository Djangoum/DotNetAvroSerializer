using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
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
                var canSerializedCheck = schema switch
                {
                    LogicalSchema logicalSchema => logicalSchema.Name switch
                    {
                        "date" => $"DateSchema.CanSerialize({sourceAccesor})",
                        "uuid" => $"UuidSchema.CanSerialize({sourceAccesor})",
                        "time-millis" => $"TimeMillisSchema.CanSerialize({sourceAccesor})",
                        "timestamp-millis" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",
                        "local-timestamp-milis" => $"TimestampMilisSchema.CanSerialize({sourceAccesor})",

                        _ => throw new Exception($"Unknown logicalType {logicalSchema.LogicalTypeName}")
                    },
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
                    RecordSchema recordSchema => $"RecordSchema.CanSerialize({sourceAccesor})",
                    UnionSchema => throw new AvroGeneratorException("Unions cannot hold directly unions"),
                    _ => null
                };

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
    }
}
