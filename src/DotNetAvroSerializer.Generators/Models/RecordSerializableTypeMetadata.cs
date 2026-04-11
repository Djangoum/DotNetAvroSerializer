using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record RecordSerializableTypeMetadata : SerializableTypeMetadata
{
    public RecordSerializableTypeMetadata(ITypeSymbol typeSymbol, Compilation compilation)
        : base(typeSymbol)
    {
        Fields = GetBaseTypesAndThis(typeSymbol)
            .SelectMany(t => t
                .GetMembers()
                .Where(s => s.Kind is SymbolKind.Property && s.DeclaredAccessibility == Accessibility.Public)
                .Cast<IPropertySymbol>()
                .Select(p => new FieldSerializableTypeMetadata(From(p.Type, compilation), p, p.Name)))
            .ToImmutableArray()
            .AsEquatableArray();
    }

    private static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    internal EquatableArray<FieldSerializableTypeMetadata> Fields { get; }
}
