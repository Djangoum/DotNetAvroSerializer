using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record IterableSerializableTypeMetadata : SerializableTypeMetadata
{
    public IterableSerializableTypeMetadata(SerializableTypeMetadata itemsTypeMetadata, ITypeSymbol iterableSymbol, bool isAsyncEnumerable)
        : base(iterableSymbol)
    {
        ItemsTypeMetadata = itemsTypeMetadata;
        IsAsyncEnumerable = isAsyncEnumerable;
    }

    internal SerializableTypeMetadata ItemsTypeMetadata { get; }
    internal bool IsAsyncEnumerable { get; }

    internal static bool IsValidArrayType(ITypeSymbol symbol)
        => symbol is IArrayTypeSymbol
            || symbol is INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol
            && (
                namedTypeSymbol.SpecialType is SpecialType.System_Collections_IEnumerable
                || namedTypeSymbol.AllInterfaces.Any(i => i.SpecialType is SpecialType.System_Collections_IEnumerable)
                || GetGenericIEnumerableType(namedTypeSymbol) is not null
                || GetGenericIAsyncEnumerableType(namedTypeSymbol) is not null);

    internal static bool IsAsyncEnumerableType(ITypeSymbol symbol)
        => symbol is INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol
            && GetGenericIAsyncEnumerableType(namedTypeSymbol) is not null;

    internal static ITypeSymbol GetIterableItemsTypeSymbol(ITypeSymbol iterableTypeSymbol)
        => iterableTypeSymbol switch
        {
            IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.ElementType,
            INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol when GetGenericIEnumerableType(namedTypeSymbol) is { } enumerableTypeSymbol => enumerableTypeSymbol.TypeArguments.First(),
            INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol when GetGenericIAsyncEnumerableType(namedTypeSymbol) is { } asyncEnumerableTypeSymbol => asyncEnumerableTypeSymbol.TypeArguments.First(),

            _ => null
        };

    private static INamedTypeSymbol GetGenericIEnumerableType(INamedTypeSymbol symbol)
        => GetSelfOrAnyInterface(symbol, static i => i.OriginalDefinition.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T);

    private static INamedTypeSymbol GetGenericIAsyncEnumerableType(INamedTypeSymbol symbol)
        => GetSelfOrAnyInterface(symbol, static i =>
            i.OriginalDefinition is { MetadataName: "IAsyncEnumerable`1", ContainingNamespace: { } ns }
            && ns.ToDisplayString() == "System.Collections.Generic");

    private static INamedTypeSymbol GetSelfOrAnyInterface(INamedTypeSymbol symbol, Func<INamedTypeSymbol, bool> predicate)
    {
        if (symbol.IsGenericType && predicate(symbol))
        {
            return symbol;
        }

        return symbol.AllInterfaces.FirstOrDefault(i => i.IsGenericType && predicate(i));
    }
}
