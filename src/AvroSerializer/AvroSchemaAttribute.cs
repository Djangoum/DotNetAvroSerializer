using System;

namespace DotNetAvroSerializer
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
