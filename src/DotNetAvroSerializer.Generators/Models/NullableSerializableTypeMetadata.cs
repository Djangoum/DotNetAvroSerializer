using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class NullableSerializableTypeMetadata : SerializableTypeMetadata
{
    public NullableSerializableTypeMetadata(SerializableTypeMetadata nullableSerializableType, ITypeSymbol nullableSymbol)
        : base(nullableSymbol)
    {
        InnerNullableTypeSymbol = nullableSerializableType;
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.Nullable;
    internal SerializableTypeMetadata InnerNullableTypeSymbol { get; }

    internal static bool IsNullableType(ITypeSymbol typeSymbol)
        => typeSymbol.NullableAnnotation is NullableAnnotation.Annotated || typeSymbol is INamedTypeSymbol
        {
            SpecialType: SpecialType.System_Nullable_T
        };

    internal static ITypeSymbol GetInnerNullableTypeSymbol(ITypeSymbol typeSymbol)
        => typeSymbol switch
        {
            INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.TypeArguments.Any() => namedTypeSymbol
                .TypeArguments.First(),
            INamedTypeSymbol => typeSymbol.WithNullableAnnotation(NullableAnnotation.None),
            IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.WithNullableAnnotation(NullableAnnotation.None),
            _ => null
        };

    public override bool Equals(SerializableTypeMetadata other)
        => base.Equals(other)
            && other is NullableSerializableTypeMetadata nullableSerializableType
            && nullableSerializableType.InnerNullableTypeSymbol.Equals(nullableSerializableType.InnerNullableTypeSymbol);
}
