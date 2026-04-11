using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record IterableSerializableTypeMetadata : SerializableTypeMetadata
{
    public IterableSerializableTypeMetadata(SerializableTypeMetadata itemsTypeMetadata, ITypeSymbol iterableSymbol)
        : base(iterableSymbol)
    {
        ItemsTypeMetadata = itemsTypeMetadata;
    }

    internal SerializableTypeMetadata ItemsTypeMetadata { get; }

    internal static bool IsValidArrayType(ITypeSymbol symbol)
        => symbol is IArrayTypeSymbol
            || symbol is INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol
            && namedTypeSymbol.AllInterfaces.Any(i => i.SpecialType is SpecialType.System_Collections_IEnumerable);

    internal static ITypeSymbol GetIterableItemsTypeSymbol(ITypeSymbol iterableTypeSymbol)
        => iterableTypeSymbol switch
        {
            IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.ElementType,
            INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol when namedTypeSymbol.AllInterfaces.Any(i => i.SpecialType is SpecialType.System_Collections_IEnumerable) => namedTypeSymbol.TypeArguments.First(),

            _ => null
        };
}
