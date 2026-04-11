using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record NullableSerializableTypeMetadata : SerializableTypeMetadata
{
    public NullableSerializableTypeMetadata(SerializableTypeMetadata nullableSerializableType, ITypeSymbol nullableSymbol)
        : base(nullableSymbol)
    {
        InnerNullableTypeSymbol = nullableSerializableType;
    }

    internal SerializableTypeMetadata InnerNullableTypeSymbol { get; }

    internal static bool IsNullableType(ITypeSymbol typeSymbol)
        => typeSymbol.NullableAnnotation is NullableAnnotation.Annotated
        || typeSymbol is INamedTypeSymbol
        {
            SpecialType: SpecialType.System_Nullable_T
        }
        || typeSymbol is INamedTypeSymbol
        {
            ConstructedFrom.SpecialType: SpecialType.System_Nullable_T
        };

    internal static ITypeSymbol GetInnerNullableTypeSymbol(ITypeSymbol typeSymbol)
        => typeSymbol switch
        {
            INamedTypeSymbol { ConstructedFrom.SpecialType: SpecialType.System_Nullable_T } namedTypeSymbol
            when namedTypeSymbol.TypeArguments.Any() => namedTypeSymbol.TypeArguments.First(),
            INamedTypeSymbol { SpecialType: SpecialType.System_Nullable_T } namedTypeSymbol
            when namedTypeSymbol.TypeArguments.Any() => namedTypeSymbol.TypeArguments.First(),
            INamedTypeSymbol => typeSymbol.WithNullableAnnotation(NullableAnnotation.None),
            IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.WithNullableAnnotation(NullableAnnotation.None),
            _ => null
        };
}
