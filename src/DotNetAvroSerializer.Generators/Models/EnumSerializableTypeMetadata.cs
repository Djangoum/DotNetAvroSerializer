using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record EnumSerializableTypeMetadata : SerializableTypeMetadata
{
    public EnumSerializableTypeMetadata(ITypeSymbol enumSymbol)
        : base(enumSymbol)
    {
    }

    internal static bool IsEnumType(ITypeSymbol enumSymbol)
        => enumSymbol.TypeKind is TypeKind.Enum;
}
