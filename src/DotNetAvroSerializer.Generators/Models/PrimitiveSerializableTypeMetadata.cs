using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class PrimitiveSerializableTypeMetadata : SerializableTypeMetadata
{
    public PrimitiveSerializableTypeMetadata(ITypeSymbol primitiveTypeSymbol)
        : base(primitiveTypeSymbol)
    {
        TypeName = primitiveTypeSymbol.Name;
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.Primitive;

    internal string TypeName { get; }

    internal static bool IsAllowedPrimitiveType(ITypeSymbol typeSymbol)
    {
        return typeSymbol.SpecialType is SpecialType.System_String
               || typeSymbol.SpecialType is SpecialType.System_Int32
               || typeSymbol.SpecialType is SpecialType.System_Int64
               || typeSymbol.SpecialType is SpecialType.System_Boolean
               || typeSymbol.SpecialType is SpecialType.System_Double
               || typeSymbol.SpecialType is SpecialType.System_Single
               || typeSymbol.SpecialType is SpecialType.System_Byte;
    }
}
