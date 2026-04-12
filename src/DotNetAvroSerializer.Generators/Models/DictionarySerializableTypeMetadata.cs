using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record DictionarySerializableTypeMetadata : SerializableTypeMetadata
{
    public DictionarySerializableTypeMetadata(SerializableTypeMetadata valuesTypeMetadata, ITypeSymbol dictionaryTypeSymbol)
        : base(dictionaryTypeSymbol)
    {
        ValuesMetadata = valuesTypeMetadata;
        KeysTypeName = GetDictionaryTypeSymbol(dictionaryTypeSymbol)?.TypeArguments.ElementAt(0).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    internal SerializableTypeMetadata ValuesMetadata { get; }
    internal string KeysTypeName { get; }

    internal static bool IsValidMapType(ITypeSymbol symbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName("System.Collections.Generic.IDictionary`2");

        return GetDictionaryTypeSymbol(symbol, dictionaryType) is not null;
    }

    internal static ITypeSymbol GetValuesTypeSymbol(ITypeSymbol symbol)
    {
        var dictionaryTypeSymbol = GetDictionaryTypeSymbol(symbol);

        if (dictionaryTypeSymbol is not null)
            return dictionaryTypeSymbol.TypeArguments.ElementAt(1);

        return null;
    }

    private static INamedTypeSymbol GetDictionaryTypeSymbol(ITypeSymbol symbol, INamedTypeSymbol dictionaryType = null)
    {
        if (symbol is not INamedTypeSymbol namedTypeSymbol)
            return null;

        if (dictionaryType is not null && namedTypeSymbol.OriginalDefinition.Equals(dictionaryType, SymbolEqualityComparer.Default))
            return namedTypeSymbol;

        return namedTypeSymbol.AllInterfaces.FirstOrDefault(i =>
            i.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Collections.Generic.IDictionary<TKey, TValue>");
    }
}
