using Avro;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    public static class SerializationGenerator
    {
        public static void GenerateSerializatonSourceForSchema(Schema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SourceProductionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            switch (schema)
            {
                case RecordSchema recordSchema:
                    foreach (var field in recordSchema.Fields)
                    {
                        RecordGenerator.GenerateSerializationSourceForRecordField(field, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
                    }
                    break;

                case LogicalSchema logicalSchema:
                    LogicalTypeGenerator.GenerateSerializationSourceForLogicalType(logicalSchema, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
                    break;

                case ArraySchema arraySchema:
                    ArrayGenerator.GenerateSerializationSourceForArray(arraySchema, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
                    break;

                case EnumSchema enumSchema:
                    EnumGenerator.GenerateSerializationSourceForEnum(enumSchema, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
                    break;

                case FixedSchema fixedSchema:
                    FixedGenerator.GenerateSerializationSourceForFixed(fixedSchema, serializationCode, originTypeSymbol as IArrayTypeSymbol, sourceAccesor);
                    break;

                case UnionSchema unionSchema:
                    UnionGenerator.GenerateSerializationSourceForUnion(unionSchema, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
                    break;

                case MapSchema mapSchema:
                    MapGenerator.GenerateSerializationSourceFoMap(mapSchema, serializationCode, privateFieldsCode, context, originTypeSymbol as INamedTypeSymbol, sourceAccesor);
                    break;

                case PrimitiveSchema primitiveSchema:
                    PrimitiveTypesGenerator.GenerateSerializationSourceForPrimitive(primitiveSchema, serializationCode, originTypeSymbol, sourceAccesor);
                    break;
            }
        }
    }
}
