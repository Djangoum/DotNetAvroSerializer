using System;

namespace DotNetAvroSerializer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AvroFieldAttribute : Attribute
    {
        public AvroFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
 