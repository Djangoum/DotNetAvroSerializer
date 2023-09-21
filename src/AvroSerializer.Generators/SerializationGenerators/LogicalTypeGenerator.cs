using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class LogicalTypeGenerator
    {
        internal static void GenerateSerializationSourceForLogicalType(LogicalSchema logicalSchema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SerializableTypeMetadata serializableTypeMetadata, IEnumerable<CustomLogicalTypeMetadata> customLogicalTypes, string sourceAccesor)
        {
            if (serializableTypeMetadata is null || serializableTypeMetadata is not LogicalTypeSerializableTypeMetadata logicalTypeSerializableTypeMetadata)
                throw new AvroGeneratorException($"Logical type is not satisfied {serializableTypeMetadata}");
            
            var serializerCallCode = logicalSchema.LogicalType.Name switch
            {
                "date" when logicalTypeSerializableTypeMetadata.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                           || logicalTypeSerializableTypeMetadata.TypeName.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {sourceAccesor});",
                "uuid" when logicalTypeSerializableTypeMetadata.TypeName.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {sourceAccesor});",
                "time-millis" when logicalTypeSerializableTypeMetadata.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {sourceAccesor});",
                "time-micros" when logicalTypeSerializableTypeMetadata.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {sourceAccesor});",
                "timestamp-millis" when logicalTypeSerializableTypeMetadata.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                "timestamp-micros" when logicalTypeSerializableTypeMetadata.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {sourceAccesor});",
                "local-timestamp-milis" when logicalTypeSerializableTypeMetadata.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                "local-timestamp-mcros" when logicalTypeSerializableTypeMetadata.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMiicrosSchema.Write(outputStream, {sourceAccesor});",

                _ => null
            };

            if (serializerCallCode is not null)
            {   // known logical type
                serializationCode.AppendLine(serializerCallCode);
            }
            else
            {
                // custom logical type serialize base schema
                SerializationGenerator.GenerateSerializatonSourceForSchema(logicalSchema.BaseSchema, serializationCode, privateFieldsCode, serializableTypeMetadata, customLogicalTypes, sourceAccesor);
            }
        }
    }
}
