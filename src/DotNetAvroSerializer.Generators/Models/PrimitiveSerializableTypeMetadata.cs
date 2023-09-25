using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class PrimitiveSerializableTypeMetadata : SerializableTypeMetadata
{
    public PrimitiveSerializableTypeMetadata(ITypeSymbol primitiveTypeSymbol)
        : base(primitiveTypeSymbol)
    {
        TypeName = primitiveTypeSymbol.Name;
        SpecialType = primitiveTypeSymbol.SpecialType;
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.Primitive;

    internal string TypeName { get; }
    internal SpecialType SpecialType { get; }

    internal static bool IsAllowedPrimitiveType(ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType is SpecialType.System_String
            || typeSymbol.SpecialType is SpecialType.System_Int32
            || typeSymbol.SpecialType is SpecialType.System_Int64
            || typeSymbol.SpecialType is SpecialType.System_Boolean
            || typeSymbol.SpecialType is SpecialType.System_Double
            || typeSymbol.SpecialType is SpecialType.System_Single
            || typeSymbol.SpecialType is SpecialType.System_Byte;
}
