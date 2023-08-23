using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class FieldSerializableTypeMetadata : SerializableTypeMetadata
    {
        public FieldSerializableTypeMetadata(SerializableTypeMetadata innerSerializableType, ITypeSymbol fieldSymbol, string propertyName)
            : base(fieldSymbol)
        {
            InnerSerializableType = innerSerializableType;
            Name = propertyName;
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Field;

        internal string Name { get; set; }
        internal SerializableTypeMetadata InnerSerializableType { get; set; }

        public override bool Equals(SerializableTypeMetadata other)
        {
            return base.Equals(other)
                && other is FieldSerializableTypeMetadata fieldSerializableType
                && InnerSerializableType.Equals(fieldSerializableType);
        }
    }
}
