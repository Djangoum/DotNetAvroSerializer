using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class DictionarySerializableTypeMetadata : SerializableTypeMetadata
{
    public DictionarySerializableTypeMetadata(SerializableTypeMetadata valuesTypeMetadata, ISymbol dictionaryTypeSymbol)
        : base(dictionaryTypeSymbol)
    {
        ValuesMetadata = valuesTypeMetadata;

        if (dictionaryTypeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            KeysTypeName = namedTypeSymbol.TypeArguments.ElementAt(0).Name;
        }
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.Map;
    internal SerializableTypeMetadata ValuesMetadata { get; }
    internal string KeysTypeName { get; }

    internal static bool IsValidMapType(ITypeSymbol symbol)
        => symbol is INamedTypeSymbol dictionaryTypeSymbol
        && (dictionaryTypeSymbol.Name == "IDictionary"
            || dictionaryTypeSymbol.AllInterfaces.Any(i => i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.IDictionary<global::System.String")));

    internal static ITypeSymbol GetValuesTypeSymbol(ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol dictionaryTypeSymbol)
            return dictionaryTypeSymbol.TypeArguments.ElementAt(1);

        return null;
    }

    public override bool Equals(SerializableTypeMetadata other)
        => base.Equals(other)
            && other is DictionarySerializableTypeMetadata dictionarySerializableType
            && KeysTypeName.Equals(dictionarySerializableType.KeysTypeName, StringComparison.InvariantCultureIgnoreCase)
            && ValuesMetadata.Equals(dictionarySerializableType.ValuesMetadata);
}
