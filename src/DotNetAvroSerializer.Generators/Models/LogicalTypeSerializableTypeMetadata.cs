using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record LogicalTypeSerializableTypeMetadata : SerializableTypeMetadata
{
    public LogicalTypeSerializableTypeMetadata(ITypeSymbol logicalTypeSymbol)
        : base(logicalTypeSymbol)
    {
        TypeName = logicalTypeSymbol.Name;
    }

    internal string TypeName { get; }

    internal static bool IsValidLogicalType(ITypeSymbol typeSymbol)
    {
        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            is "global::System.DateTime"
            or "global::System.DateOnly"
            or "global::System.TimeOnly"
            or "global::System.Guid";
    }
}
