using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class RecordSerializableTypeMetadata : SerializableTypeMetadata
{

    public RecordSerializableTypeMetadata(ITypeSymbol typeSymbol, Compilation compilation)
        : base(typeSymbol)
    {
        Fields = GetBaseTypesAndThis(typeSymbol).SelectMany(typeSymbol => typeSymbol
            .GetMembers()
            .Where(s => s.Kind is SymbolKind.Property && s.DeclaredAccessibility == Accessibility.Public)
            .Cast<IPropertySymbol>()
            .Select(t => new FieldSerializableTypeMetadata(From(t.Type, compilation), t, t.Name)));
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

    protected override SerializableTypeKind Kind => SerializableTypeKind.Record;

    internal IEnumerable<FieldSerializableTypeMetadata> Fields { get; }

    public override bool Equals(SerializableTypeMetadata other)
        => base.Equals(other)
            && other is RecordSerializableTypeMetadata recordSerializableType
            && Fields.SequenceEqual(recordSerializableType.Fields);
}
