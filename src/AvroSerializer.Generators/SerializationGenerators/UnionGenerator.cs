using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class UnionGenerator
    {
        internal static void GenerateSerializationSourceForUnion(UnionSchema unionSchema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata originTypeSymbol, IEnumerable<CustomLogicalTypeMetadata> customLogicalTypes, string sourceAccesor)
        {
            foreach (var schema in unionSchema.Schemas)
            {
                var unionSchemaIndex = unionSchema.Schemas.IndexOf(schema);

                if (originTypeSymbol is UnionSerializableTypeMetadata unionSerializableTypeMetadata)
                {
                    var unionTypeSerializableTypeMetadata = unionSerializableTypeMetadata.UnionTypes.ElementAt(unionSchemaIndex);

                    var canSerializedCheck = GetCanSerializeCheck(schema, $"{sourceAccesor}.GetUnionValue()", unionTypeSerializableTypeMetadata.FullNameDisplay);

                    if (unionSchemaIndex == 0)
                    {
                        serializationCode.AppendLine($@"if ({canSerializedCheck}) {{");
                    }
                    else
                    {
                        serializationCode.AppendLine($@"else if ({canSerializedCheck}) {{");
                    }

                    serializationCode.AppendLine($"IntSchema.Write(outputStream, {unionSchemaIndex});");

                    SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializationCode, privateFieldsCode, unionTypeSerializableTypeMetadata, customLogicalTypes, $"(({unionTypeSerializableTypeMetadata.FullNameDisplay}){sourceAccesor})");
                }
                else if (originTypeSymbol is NullableSerializableTypeMetadata nullableSeralizableTypeMetadata)
                {
                    var canSerializedCheck = GetCanSerializeCheck(schema, sourceAccesor);

                    if (unionSchemaIndex == 0)
                    {
                        serializationCode.AppendLine($@"if ({canSerializedCheck}) {{");
                    }
                    else
                    {
                        serializationCode.AppendLine($@"else if ({canSerializedCheck}) {{");
                    }

                    serializationCode.AppendLine($"IntSchema.Write(outputStream, {unionSchemaIndex});");

                    SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializationCode, privateFieldsCode, nullableSeralizableTypeMetadata.InnerNullableTypeSymbol, customLogicalTypes, sourceAccesor);
                }

                serializationCode.AppendLine("}");
            }
        }

        private static string GetCanSerializeCheck(Schema schema, string sourceAccesor, string typeFullName = null)
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
                RecordSchema => $"RecordSchema.CanSerialize<{typeFullName}>({sourceAccesor})",
                ArraySchema => $"ArraySchema.CanSerialize({sourceAccesor})",
                FixedSchema => $"FixedSchema.CanSerialize({sourceAccesor})",
                MapSchema => $"MapSchema.CanSerialize({sourceAccesor})",
                UnionSchema => throw new AvroGeneratorException("Unions cannot hold directly unions"),
                _ => null
            };
        }
    }
}
