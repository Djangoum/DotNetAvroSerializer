using System;

namespace AvroSerializer
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
