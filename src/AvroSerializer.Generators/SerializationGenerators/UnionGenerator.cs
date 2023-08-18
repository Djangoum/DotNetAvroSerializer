using Avro;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class UnionGenerator
    {
        public static void GenerateSerializationSourceForUnion(UnionSchema unionSchema, StringBuilder code, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
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
                    UnionSchema => throw new Exception("Unions cannot hold directly unions"),
                    _ => null
                };

                if (unionSchema.Schemas.IndexOf(schema) == 0)
                {
                    code.AppendLine($@"if ({canSerializedCheck}) {{");
                }
                else
                {
                    code.AppendLine($@"else if ({canSerializedCheck}) {{");
                }
                code.AppendLine($"IntSchema.Write(outputStream, {unionSchema.Schemas.IndexOf(schema)});");

                var newSymbol = originTypeSymbol.Name switch
                {
                    "Nullable" when !schema.Name.Equals("null", StringComparison.InvariantCultureIgnoreCase) && originTypeSymbol is INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.TypeArguments.First(),
                    _ => originTypeSymbol
                };

                SerializationGenerator.GenerateSerializatonSourceForSchema(schema, code, context, newSymbol, sourceAccesor);

                code.AppendLine("}");
            }
        }
    }
}
