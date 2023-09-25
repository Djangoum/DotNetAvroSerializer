using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class LogicalTypeSerializableTypeMetadata : SerializableTypeMetadata
{
    public LogicalTypeSerializableTypeMetadata(ISymbol logicalTypeSymbol)
        : base(logicalTypeSymbol)
    {
        TypeName = logicalTypeSymbol.Name;
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.LogicalType;
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
