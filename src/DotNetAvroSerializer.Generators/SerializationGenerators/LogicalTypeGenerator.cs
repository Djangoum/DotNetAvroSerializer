using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Models;
using System;
using System.Linq;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class LogicalTypeGenerator
    {
        internal static void GenerateSerializationSourceForLogicalType(AvroGenerationContext context)
        {
            var schema = context.Schema as LogicalSchema;
            
            if (context.SerializableTypeMetadata is null)
                throw new AvroGeneratorException($"Logical type is not satisfied {context.SerializableTypeMetadata}");

            if (context.SerializableTypeMetadata is LogicalTypeSerializableTypeMetadata logicalTypeName)
            {
                var serializerCallCode = schema.LogicalType.Name switch
                {
                    "date" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                               || logicalTypeName.TypeName.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {context.SourceAccessor});",
                    "uuid" when logicalTypeName.TypeName.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {context.SourceAccessor});",
                    "time-millis" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {context.SourceAccessor});",
                    "time-micros" when logicalTypeName.TypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {context.SourceAccessor});",
                    "timestamp-millis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {context.SourceAccessor});",
                    "timestamp-micros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {context.SourceAccessor});",
                    "local-timestamp-millis" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {context.SourceAccessor});",
                    "local-timestamp-micros" when logicalTypeName.TypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {context.SourceAccessor});",

                    _ => null
                };

                if (serializerCallCode is not null)
                {   // known logical type
                    context.SerializationCode.AppendLine(serializerCallCode);
                }
                else
                {
                    throw new AvroGeneratorException($"Logical type is not satisfied {context.SerializableTypeMetadata}");
                }
            }
            else if (context.CustomLogicalTypesMetadata.Any(a => a.Name.Equals(schema!.LogicalTypeName)))
            {
                var customLogicalType = context.CustomLogicalTypesMetadata.First(c => c.Name.Equals(schema!.LogicalTypeName));

                var logicalTypesValues = customLogicalType.OrderedSchemaPropertiesConvertToBaseType.Select(schema!.GetProperty).Where(v => v is not null);

                if (!logicalTypesValues.Count().Equals(customLogicalType.OrderedSchemaPropertiesConvertToBaseType.Count()))
                    throw new AvroGeneratorException("Logical type properties could not be mapped");

                if (schema!.BaseSchema is not PrimitiveSchema)
                    throw new AvroGeneratorException("Custom logical types with complex base types are not supported");

                schema.BaseSchema.Generate(context
                    with
                    {
                        Schema = schema.BaseSchema,
                        SourceAccessor =
                        $"{customLogicalType.LogicalTypeFullyQualifiedName}.ConvertToBaseSchemaType({context.SourceAccessor}{(logicalTypesValues.Any() ? "," + string.Join(",", logicalTypesValues) : "")})"
                    });
            }
            else
            {
                throw new AvroGeneratorException($"Logical type is not satisfied {context.SerializableTypeMetadata}");
            }
        }
    }
}
