using System;

namespace DotNetAvroSerializer;

[AttributeUsage(AttributeTargets.Class)]
public class AvroSchemaAttribute : Attribute
{
    public AvroSchemaAttribute(string schemaFilePath, Type[]? allowedCustomLogicalTypes = null)
    {
        SchemaFilePath = schemaFilePath;
        AllowedCustomLogicalTypes = allowedCustomLogicalTypes;
    }

    public string SchemaFilePath { get; }
    public Type[]? AllowedCustomLogicalTypes { get; }
}
