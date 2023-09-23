using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (serializableTypeMetadata is LogicalTypeSerializableTypeMetadata logicalTypeName)
            {
                var serializerCallCode = logicalSchema.LogicalType.Name switch
                {
                    "date" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                               || logicalTypeName.TypeName.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {sourceAccesor});",
                    "uuid" when logicalTypeName.TypeName.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {sourceAccesor});",
                    "time-millis" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {sourceAccesor});",
                    "time-micros" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {sourceAccesor});",
                    "timestamp-millis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {sourceAccesor});",
                    "timestamp-micros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {sourceAccesor});",
                    "local-timestamp-millis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {sourceAccesor});",
                    "local-timestamp-micros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {sourceAccesor});",

                    _ => null
                };

                if (serializerCallCode is not null)
                {   // known logical type
                    serializationCode.AppendLine(serializerCallCode);
                }
                else
                {
                    throw new AvroGeneratorException($"Logical type is not satisfied 2 {serializableTypeMetadata}");
                }
            }
            else if (customLogicalTypes.Any(a => a.Name.Equals(logicalSchema.LogicalTypeName)))
            {
                var customLogicalType = customLogicalTypes.First(c => c.Name.Equals(logicalSchema.LogicalTypeName));

                var logicalTypesValues = customLogicalType.OrderedSchemaPropertiesConvertToBaseType.Select(logicalSchema.GetProperty).Where(v => v is not null);

                if (!logicalTypesValues.Count().Equals(customLogicalType.OrderedSchemaPropertiesConvertToBaseType.Count()))
                    throw new AvroGeneratorException("Logical type properties could not be mapped");

                if (logicalSchema.BaseSchema is not PrimitiveSchema)
                    throw new AvroGeneratorException("Custom logical types with complex base types are not supported");
                
                SerializationGenerator.GenerateSerializatonSourceForSchema(
                    logicalSchema.BaseSchema,
                    serializationCode, 
                    privateFieldsCode, 
                    serializableTypeMetadata, 
                    customLogicalTypes, 
                    $"{customLogicalType.LogicalTypeFullyQualifiedName}.ConvertToBaseSchemaType({sourceAccesor}{(logicalTypesValues.Any() ? "," + string.Join(",", logicalTypesValues) : "")})");
            }
            else
            {
                throw new AvroGeneratorException($"Logical type is not satisfied {serializableTypeMetadata}");
            }
        }
    }
}
