using System.Collections.Immutable;
using System.Linq;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record FieldSerializableTypeMetadata : SerializableTypeMetadata
{
    public FieldSerializableTypeMetadata(SerializableTypeMetadata innerSerializableType, IPropertySymbol propertySymbol, string propertyName)
        : base(propertySymbol.Type)
    {
        InnerSerializableType = innerSerializableType;
        Name = propertyName;
        Names = GetFieldNames(propertySymbol);
    }

    internal string Name { get; }
    internal EquatableArray<string> Names { get; }
    internal SerializableTypeMetadata InnerSerializableType { get; }

    private static EquatableArray<string> GetFieldNames(ISymbol fieldSymbol)
    {
        return fieldSymbol
            .GetAttributes()
            .Where(a => a.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::DotNetAvroSerializer.AvroFieldAttribute")
            .Where(a => a.ConstructorArguments.Any())
            .Select(a => a.ConstructorArguments[0].Value.ToString())
            .ToImmutableArray()
            .AsEquatableArray();
    }
}
