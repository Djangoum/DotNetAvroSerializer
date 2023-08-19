using Avro;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class SerializationGenerator
    {
        public static void GenerateSerializatonSourceForSchema(Schema schema, StringBuilder code, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            switch (schema)
            {
                case RecordSchema recordSchema:
                    foreach (var field in recordSchema.Fields)
                    {
                        RecordGenerator.GenerateSerializationSourceForRecordField(field, code, context, originTypeSymbol, sourceAccesor);
                    }
                    break;

                case LogicalSchema logicalSchema:
                    LogicalTypeGenerator.GenerateSerializationSourceForLogicalType(logicalSchema, code, context, originTypeSymbol, sourceAccesor);
                    break;

                case ArraySchema arraySchema:
                    ArrayGenerator.GenerateSerializationSourceForArray(arraySchema, code, context, originTypeSymbol as IArrayTypeSymbol, sourceAccesor);
                    break;

                case EnumSchema enumSchema:
                    EnumGenerator.GenerateSerializationSourceForEnum(enumSchema, code, context, originTypeSymbol, sourceAccesor);
                    break;

                case FixedSchema fixedSchema:
                    FixedGenerator.GenerateSerializationSourceForFixed(fixedSchema, code, originTypeSymbol, sourceAccesor);
                    break;

                case UnionSchema unionSchema:
                    UnionGenerator.GenerateSerializationSourceForUnion(unionSchema, code, context, originTypeSymbol, sourceAccesor);
                    break;

                case MapSchema mapSchema:
                    MapGenerator.GenerateSerializationSourceFoMap(mapSchema, code, context, originTypeSymbol as INamedTypeSymbol, sourceAccesor);
                    break;

                case PrimitiveSchema primitiveSchema:
                    PrimitiveTypesGenerator.GenerateSerializationSourceForPrimitive(primitiveSchema, code, originTypeSymbol, sourceAccesor);
                    break;
            }
        }
    }
}
