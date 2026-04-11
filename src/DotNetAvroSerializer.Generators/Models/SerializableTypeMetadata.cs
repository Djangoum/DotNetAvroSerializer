using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal abstract record SerializableTypeMetadata
{
    private readonly string _stringRepresentation;

    protected SerializableTypeMetadata(ITypeSymbol typeSymbol)
    {
        FullNameDisplay = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        _stringRepresentation = typeSymbol.ToString();

        IsNullable = (typeSymbol.IsReferenceType && typeSymbol.NullableAnnotation is NullableAnnotation.None)
            || typeSymbol.NullableAnnotation is NullableAnnotation.Annotated;
    }

    internal bool IsValid { get; init; }
    internal string FullNameDisplay { get; }
    public bool IsNullable { get; }

    internal static SerializableTypeMetadata From(ITypeSymbol symbol, Compilation compilation)
    {
        if (symbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Struct or TypeKind.Array or TypeKind.Enum)
        {
            if (LogicalTypeSerializableTypeMetadata.IsValidLogicalType(symbol))
            {
                return new LogicalTypeSerializableTypeMetadata(symbol);
            }

            if (NullableSerializableTypeMetadata.IsNullableType(symbol))
            {
                return new NullableSerializableTypeMetadata(From(NullableSerializableTypeMetadata.GetInnerNullableTypeSymbol(symbol), compilation), symbol);
            }

            if (UnionSerializableTypeMetadata.IsUnionType(symbol))
            {
                return new UnionSerializableTypeMetadata(symbol, UnionSerializableTypeMetadata.GetInnerUnionTypeSymbols(symbol, compilation));
            }

            if (DictionarySerializableTypeMetadata.IsValidMapType(symbol, compilation))
            {
                return new DictionarySerializableTypeMetadata(From(DictionarySerializableTypeMetadata.GetValuesTypeSymbol(symbol), compilation), symbol);
            }

            if (IterableSerializableTypeMetadata.IsValidArrayType(symbol))
            {
                return new IterableSerializableTypeMetadata(From(IterableSerializableTypeMetadata.GetIterableItemsTypeSymbol(symbol), compilation), symbol);
            }

            if (EnumSerializableTypeMetadata.IsEnumType(symbol))
            {
                return new EnumSerializableTypeMetadata(symbol);
            }

            if (PrimitiveSerializableTypeMetadata.IsAllowedPrimitiveType(symbol))
            {
                return new PrimitiveSerializableTypeMetadata(symbol);
            }

            return new RecordSerializableTypeMetadata(symbol, compilation);
        }

        return null;
    }

    public override string ToString() => _stringRepresentation;
}
