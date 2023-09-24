using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class UnionSerializableTypeMetadata : SerializableTypeMetadata
{
    protected override SerializableTypeKind Kind => SerializableTypeKind.Union;

    public UnionSerializableTypeMetadata(ISymbol typeSymbol, IEnumerable<SerializableTypeMetadata> unionTypes)
        : base(typeSymbol)
    {
        UnionTypes = unionTypes;
    }

    internal static bool IsUnionType(ITypeSymbol typeSymbol)
        => typeSymbol is INamedTypeSymbol { Name: "Union" };

    internal IEnumerable<SerializableTypeMetadata> UnionTypes { get; }

    internal static IEnumerable<SerializableTypeMetadata> GetInnerUnionTypeSymbols(ITypeSymbol typeSymbol)
    {
        var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

        return namedTypeSymbol!.TypeArguments.Select(From).ToList();
    }

    public override bool Equals(SerializableTypeMetadata other)
    {
        return base.Equals(other)
            && other is UnionSerializableTypeMetadata unionSerializableTypeMetadata
            && unionSerializableTypeMetadata.UnionTypes.SequenceEqual(UnionTypes);
    }
}
