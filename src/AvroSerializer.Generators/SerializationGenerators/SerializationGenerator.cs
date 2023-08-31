using Avro;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class SerializationGenerator
    {
        internal static void GenerateSerializatonSourceForSchema(Schema schema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata serializableTypeMetadata, string sourceAccesor)
        {
            switch (schema)
            {
                case RecordSchema recordSchema:
                    foreach (var field in recordSchema.Fields)
                    {
                        RecordGenerator.GenerateSerializationSourceForRecordField(field, serializationCode, privateFieldsCode, serializableTypeMetadata as RecordSerializableTypeMetadata, sourceAccesor);
                    }
                    break;

                case ArraySchema arraySchema:
                    ArrayGenerator.GenerateSerializationSourceForArray(arraySchema, serializationCode, privateFieldsCode, serializableTypeMetadata as IterableSerializableTypeMetadata, sourceAccesor);
                    break;

                case EnumSchema enumSchema:
                    EnumGenerator.GenerateSerializationSourceForEnum(enumSchema, serializationCode, privateFieldsCode, serializableTypeMetadata, sourceAccesor);
                    break;

                case FixedSchema fixedSchema:
                    FixedGenerator.GenerateSerializationSourceForFixed(fixedSchema, serializationCode, serializableTypeMetadata, sourceAccesor);
                    break;

                case UnionSchema unionSchema:
                    UnionGenerator.GenerateSerializationSourceForUnion(unionSchema, serializationCode, privateFieldsCode, serializableTypeMetadata, sourceAccesor);
                    break;
                     
                case MapSchema mapSchema:
                    MapGenerator.GenerateSerializationSourceFoMap(mapSchema, serializationCode, privateFieldsCode, serializableTypeMetadata, sourceAccesor);
                    break;

                case LogicalSchema logicalSchema:
                    LogicalTypeGenerator.GenerateSerializationSourceForLogicalType(logicalSchema, serializationCode, privateFieldsCode, serializableTypeMetadata, sourceAccesor);
                    break;

                case PrimitiveSchema primitiveSchema:
                    PrimitiveTypesGenerator.GenerateSerializationSourceForPrimitive(primitiveSchema, serializationCode, serializableTypeMetadata, sourceAccesor);
                    break;
            }
        }
    }
}
