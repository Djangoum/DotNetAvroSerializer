using System;

namespace AvroSerializer.Abstractions
{
    public class AvroSchemaAttribute : Attribute
    {
        public AvroSchemaAttribute(string schemaFilePath)
        {
            SchemaFilePath = schemaFilePath;
        }

        public string SchemaFilePath { get; }
    }
}
