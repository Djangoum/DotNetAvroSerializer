using Microsoft.CodeAnalysis;
using System;

namespace DotNetAvroSerializer.Generators.Models
{
    internal abstract class SerializableTypeMetadata : IEquatable<SerializableTypeMetadata>
    {
        public SerializableTypeMetadata(ITypeSymbol symbol)
        {
            FullNameDisplay = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            StringRepresentation = symbol.ToString();
        }

        internal abstract SerializableTypeKind Kind { get; }
        internal bool IsValid { get; set; }
        internal string FullNameDisplay { get; set; }
        protected virtual string StringRepresentation { get; set; }

        internal static SerializableTypeMetadata From(ITypeSymbol symbol)
        {
            if (symbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Struct or TypeKind.Array or TypeKind.Enum)
            {
                if (LogicalTypeSerializableTypeMetadata.IsValidLogicalType(symbol))
                {
                    return new LogicalTypeSerializableTypeMetadata(symbol);
                }
                else if (NullableSerializableTypeMetadata.IsNullableType(symbol))
                {
                    return new NullableSerializableTypeMetadata(From(NullableSerializableTypeMetadata.GetInnerNullableTypeSymbol(symbol)), symbol);
                }
                else if (UnionSerializableTypeMetadata.IsUnionType(symbol))
                {
                    return new UnionSerializableTypeMetadata(symbol, UnionSerializableTypeMetadata.GetInnerUnionTypeSymbols(symbol));
                }
                else if (DictionarySerializableTypeMetadata.IsValidMapType(symbol))
                {
                    return new DictionarySerializableTypeMetadata(From(DictionarySerializableTypeMetadata.GetValuesTypeSymbol(symbol)), symbol);
                }
                else if (IterableSerializableTypeMetadata.IsValidArrayType(symbol))
                {
                    return new IterableSerializableTypeMetadata(From(IterableSerializableTypeMetadata.GetIterableItemsTypeSymbol(symbol)), symbol);
                }
                else if (EnumSerializableTypeMetadata.IsEnumType(symbol))
                {
                    return new EnumSerializableTypeMetadata(symbol);
                }
                else if (PrimitiveSerializableTypeMetadata.IsAllowedPrimitiveType(symbol))
                {
                    return new PrimitiveSerializableTypeMetadata(symbol);
                }
                else
                {
                    return new RecordSerializableTypeMetadata(symbol);
                }
            }

            return null;
        }

        public override string ToString()
        {
            return StringRepresentation;
        }

        public virtual bool Equals(SerializableTypeMetadata other)
        {
            return Kind == other.Kind && FullNameDisplay == other.FullNameDisplay;
        }
    }
}
