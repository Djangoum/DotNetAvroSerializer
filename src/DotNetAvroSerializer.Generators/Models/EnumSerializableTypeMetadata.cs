using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class EnumSerializableTypeMetadata : SerializableTypeMetadata
    {
        public EnumSerializableTypeMetadata(ISymbol enumSymbol)
            : base(enumSymbol)
        {
            
        }

        protected override SerializableTypeKind Kind => SerializableTypeKind.Enum;

        internal static bool IsEnumType(ITypeSymbol enumSymbol)
            => enumSymbol.TypeKind is TypeKind.Enum;
    }
}
