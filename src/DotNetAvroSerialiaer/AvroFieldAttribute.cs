using System;

namespace DotNetAvroSerializer
{
    public class AvroFieldAttribute : Attribute
    {
        public AvroFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
 