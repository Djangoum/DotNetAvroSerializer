﻿using Microsoft.CodeAnalysis;
using System;

namespace DotNetAvroSerializer.Generators.Models
{
    internal abstract class SerializableTypeMetadata : IEquatable<SerializableTypeMetadata>
    {
        protected SerializableTypeMetadata(ISymbol symbol)
        {
            FullNameDisplay = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            StringRepresentation = symbol.ToString();
        }

        protected abstract SerializableTypeKind Kind { get; }
        internal bool IsValid { get; set; }
        internal string FullNameDisplay { get; }
        private string StringRepresentation { get; }

        internal static SerializableTypeMetadata From(ITypeSymbol symbol)
        {
            if (symbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Struct or TypeKind.Array or TypeKind.Enum)
            {
                if (LogicalTypeSerializableTypeMetadata.IsValidLogicalType(symbol))
                {
                    return new LogicalTypeSerializableTypeMetadata(symbol);
                }

                if (NullableSerializableTypeMetadata.IsNullableType(symbol))
                {
                    return new NullableSerializableTypeMetadata(From(NullableSerializableTypeMetadata.GetInnerNullableTypeSymbol(symbol)), symbol);
                }

                if (UnionSerializableTypeMetadata.IsUnionType(symbol))
                {
                    return new UnionSerializableTypeMetadata(symbol, UnionSerializableTypeMetadata.GetInnerUnionTypeSymbols(symbol));
                }

                if (DictionarySerializableTypeMetadata.IsValidMapType(symbol))
                {
                    return new DictionarySerializableTypeMetadata(From(DictionarySerializableTypeMetadata.GetValuesTypeSymbol(symbol)), symbol);
                }

                if (IterableSerializableTypeMetadata.IsValidArrayType(symbol))
                {
                    return new IterableSerializableTypeMetadata(From(IterableSerializableTypeMetadata.GetIterableItemsTypeSymbol(symbol)), symbol);
                }

                if (EnumSerializableTypeMetadata.IsEnumType(symbol))
                {
                    return new EnumSerializableTypeMetadata(symbol);
                }

                if (PrimitiveSerializableTypeMetadata.IsAllowedPrimitiveType(symbol))
                {
                    return new PrimitiveSerializableTypeMetadata(symbol);
                }

                return new RecordSerializableTypeMetadata(symbol);
            }

            return null;
        }

        public override string ToString()
        {
            return StringRepresentation;
        }

        public virtual bool Equals(SerializableTypeMetadata other)
        {
            return other is not null && Kind == other.Kind && FullNameDisplay == other.FullNameDisplay;
        }
    }
}