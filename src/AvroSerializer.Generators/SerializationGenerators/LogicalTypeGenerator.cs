﻿using Avro;
using AvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class LogicalTypeGenerator
    {
        public static void GenerateSerializationSourceForLogicalType(LogicalSchema logicalSchema, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            var serializerCallCode = logicalSchema.LogicalType.Name switch
            {
                "date" when originTypeSymbol.Name.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeSymbol.Name.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {sourceAccesor});",
                "uuid" when originTypeSymbol.Name.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {sourceAccesor});",
                "time-millis" when originTypeSymbol.Name.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {sourceAccesor});",
                "time-micros" when originTypeSymbol.Name.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {sourceAccesor});",
                "timestamp-millis" when originTypeSymbol.Name.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                "timestamp-micros" when originTypeSymbol.Name.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {sourceAccesor});",
                "local-timestamp-milis" when originTypeSymbol.Name.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                "local-timestamp-mcros" when originTypeSymbol.Name.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMiicrosSchema.Write(outputStream, {sourceAccesor});",

                _ => null
            };

            if (serializerCallCode is not null)
            {   // known logical type
                serializationCode.AppendLine(serializerCallCode);
            }
            else
            {
                // unknown logical type serialize base schema
                SerializationGenerator.GenerateSerializatonSourceForSchema(logicalSchema.BaseSchema, serializationCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);
            }
        }
    }
}
