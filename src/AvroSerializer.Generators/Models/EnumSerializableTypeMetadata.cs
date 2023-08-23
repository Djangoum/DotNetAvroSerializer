using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class EnumSerializableTypeMetadata : SerializableTypeMetadata
    {
        public EnumSerializableTypeMetadata(ITypeSymbol enumSymbol)
            : base(enumSymbol)
        {
            
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Enum;

        internal static bool IsEnumType(ITypeSymbol enumSymbol)
            => enumSymbol.TypeKind is TypeKind.Enum;
    }
}
