using System;
using System.Linq;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Polyfills;
using DotNetAvroSerializer.Generators.Schemas;

namespace DotNetAvroSerializer.Generators.SerializationGenerators;

internal static class LogicalTypeGenerator
{
    internal static void GenerateSerializationSourceForLogicalType(AvroGenerationContext context)
    {
        var schema = context.Schema as LogicalSchema;

        if (context.SerializableTypeMetadata is null)
            throw new AvroGeneratorException($"Logical type is not satisfied {context.SerializableTypeMetadata}");

        if (context.SerializableTypeMetadata is LogicalTypeSerializableTypeMetadata logicalTypeName)
        {
            var serializerCallCode = schema.LogicalTypeName switch
            {
                "date" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase)
                           || logicalTypeName.TypeName.Equals(nameof(DateOnly), StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {context.SourceAccessor});",
                "uuid" when logicalTypeName.TypeName.Equals(nameof(Guid), StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {context.SourceAccessor});",
                "time-millis" when logicalTypeName.TypeName.Equals(nameof(TimeOnly), StringComparison.InvariantCultureIgnoreCase) => $"TimeMillisSchema.Write(outputStream, {context.SourceAccessor});",
                "time-micros" when logicalTypeName.TypeName.Equals(nameof(TimeOnly), StringComparison.InvariantCultureIgnoreCase) => $"TimeMicrosSchema.Write(outputStream, {context.SourceAccessor});",
                "timestamp-millis" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {context.SourceAccessor});",
                "timestamp-micros" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {context.SourceAccessor});",
                "local-timestamp-millis" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"TimestampMillisSchema.Write(outputStream, {context.SourceAccessor});",
                "local-timestamp-micros" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"TimestampMicrosSchema.Write(outputStream, {context.SourceAccessor});",

                _ => null
            };

            var serializerCallCodeAsync = schema.LogicalTypeName switch
            {
                "date" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase)
                           || logicalTypeName.TypeName.Equals(nameof(DateOnly), StringComparison.InvariantCultureIgnoreCase) => $"await DateSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "uuid" when logicalTypeName.TypeName.Equals(nameof(Guid), StringComparison.InvariantCultureIgnoreCase) => $"await UuidSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "time-millis" when logicalTypeName.TypeName.Equals(nameof(TimeOnly), StringComparison.InvariantCultureIgnoreCase) => $"await TimeMillisSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "time-micros" when logicalTypeName.TypeName.Equals(nameof(TimeOnly), StringComparison.InvariantCultureIgnoreCase) => $"await TimeMicrosSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "timestamp-millis" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"await TimestampMillisSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "timestamp-micros" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"await TimestampMicrosSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "local-timestamp-millis" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"await TimestampMillisSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                "local-timestamp-micros" when logicalTypeName.TypeName.Equals(nameof(DateTime), StringComparison.InvariantCultureIgnoreCase) => $"await TimestampMicrosSchema.WriteAsync(outputStream, {context.SourceAccessor}, cancellationToken);",
                _ => null
            };

            if (serializerCallCode is not null)
            {
                context.SerializationCode.AppendLine(serializerCallCode);
                context.AsyncSerializationCode.AppendLine(serializerCallCodeAsync);
            }
            else
            {
                throw new AvroGeneratorException(
                    $"Logical type is not satisfied instead {context.SerializableTypeMetadata.FullNameDisplay} found");
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
