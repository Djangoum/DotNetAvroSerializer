using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class FieldSerializableTypeMetadata : SerializableTypeMetadata
    {
        public FieldSerializableTypeMetadata(SerializableTypeMetadata innerSerializableType, IPropertySymbol propertySymbol, string propertyName)
            : base(propertySymbol.Type)
        {
            InnerSerializableType = innerSerializableType;
            Name = propertyName;

            GetFieldNames(propertySymbol);
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Field;

        internal string Name { get; set; }
        internal ICollection<string> Names { get; set; } = new List<string>();
        internal SerializableTypeMetadata InnerSerializableType { get; set; }

        private void GetFieldNames(IPropertySymbol fieldSymbol)
        {
            var avroFieldAttributes = fieldSymbol.GetAttributes().Where(a => a.AttributeClass.Name == "AvroFieldAttribute");

            foreach (var avroAttribute in avroFieldAttributes)
            {
                if (avroAttribute.ConstructorArguments.Any())
                {
                    var fieldName = avroAttribute.ConstructorArguments[0].Value.ToString();

                    Names.Add(fieldName);
                }
            }

            Names = avroFieldAttributes.Select(a => a.ConstructorArguments[0].Value.ToString()).ToArray();
        }

        public override bool Equals(SerializableTypeMetadata other)
        {
            return base.Equals(other)
                && other is FieldSerializableTypeMetadata fieldSerializableType
                && InnerSerializableType.Equals(fieldSerializableType);
        }
    }
}
