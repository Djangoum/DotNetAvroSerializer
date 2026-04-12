using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record DictionarySerializableTypeMetadata : SerializableTypeMetadata
{
    private const string DictionaryMetadataName = "global::System.Collections.Generic.IDictionary<TKey, TValue>";

    public DictionarySerializableTypeMetadata(SerializableTypeMetadata valuesTypeMetadata, ITypeSymbol dictionaryTypeSymbol)
        : base(dictionaryTypeSymbol)
    {
        ValuesMetadata = valuesTypeMetadata;
        var keyType = GetDictionaryTypeSymbol(dictionaryTypeSymbol)?.TypeArguments.ElementAt(0);
        KeysTypeName = keyType?.ToString();
        KeysSpecialType = keyType?.SpecialType ?? SpecialType.None;
    }

    internal SerializableTypeMetadata ValuesMetadata { get; }
    internal string KeysTypeName { get; }
    internal SpecialType KeysSpecialType { get; }

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

        if (namedTypeSymbol.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == DictionaryMetadataName)
            return namedTypeSymbol;

        if (dictionaryType is not null && namedTypeSymbol.OriginalDefinition.Equals(dictionaryType, SymbolEqualityComparer.Default))
            return namedTypeSymbol;

        return namedTypeSymbol.AllInterfaces.FirstOrDefault(i =>
            i.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == DictionaryMetadataName);
    }
}
