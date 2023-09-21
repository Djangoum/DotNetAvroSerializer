using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class LogicalTypeGenerator
    {
        internal static void GenerateSerializationSourceForLogicalType(LogicalSchema logicalSchema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata serializableTypeMetadata, IEnumerable<CustomLogicalTypeMetadata> customLogicalTypes, string sourceAccesor)
        {
            if (serializableTypeMetadata is null)
                throw new AvroGeneratorException($"Logical type is not satisfied {serializableTypeMetadata}");

            string serializerCallCode = default;

            if (serializableTypeMetadata is LogicalTypeSerializableTypeMetadata logicalTypeName)
            {
                serializerCallCode = logicalSchema.LogicalType.Name switch
                {
                    "date" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                               || logicalTypeName.TypeName.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {sourceAccesor});",
                    "uuid" when logicalTypeName.TypeName.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {sourceAccesor});",
                    "time-millis" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {sourceAccesor});",
                    "time-micros" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {sourceAccesor});",
                    "timestamp-millis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                    "timestamp-micros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {sourceAccesor});",
                    "local-timestamp-milis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                    "local-timestamp-mcros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMiicrosSchema.Write(outputStream, {sourceAccesor});",

                    _ => null
                };
            }
            else if (customLogicalTypes.Any(a => a.Name.Equals(logicalSchema.LogicalTypeName)))
            {
                var customLogicalType = customLogicalTypes.First(c => c.Name.Equals(logicalSchema.LogicalTypeName));

                var logicalTypesValues = customLogicalType.OrderedSchemaProperties.Select(p => logicalSchema.GetProperty(p));

                SerializationGenerator.GenerateSerializatonSourceForSchema(
                    logicalSchema.BaseSchema,
                    serializationCode, 
                    privateFieldsCode, 
                    serializableTypeMetadata, 
                    customLogicalTypes, 
                    $"{customLogicalType.LogicalTypeFullyQualifiedName}.ConvertToBaseSchemaType({sourceAccesor},{string.Join(",", logicalTypesValues)})");
            }
            else
            {
                throw new AvroGeneratorException($"Logical type is not satisfied {serializableTypeMetadata}");
            }

            if (serializerCallCode is not null)
            {   // known logical type
                serializationCode.AppendLine(serializerCallCode);
            }
            else
            {
                throw new AvroGeneratorException($"Logical type is not satisfied 2 {serializableTypeMetadata}");
                // custom logical type serialize base schema
            }
        }
    }
}
