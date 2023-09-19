using System;

namespace DotNetAvroSerializer
{
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
}
