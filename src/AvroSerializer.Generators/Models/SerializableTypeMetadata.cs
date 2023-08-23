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
                return symbol switch
                {
                    _ when PrimitiveSerializableTypeMetadata.IsAllowedPrimitiveType(symbol) => new PrimitiveSerializableTypeMetadata(symbol),
                    _ when LogicalTypeSerializableTypeMetadata.IsValidLogicalType(symbol) => new LogicalTypeSerializableTypeMetadata(symbol),
                    _ when NullableSerializableTypeMetadata.IsNullableType(symbol) => new NullableSerializableTypeMetadata(From(NullableSerializableTypeMetadata.GetInnerNullableTypeSymbol(symbol)), symbol),
                    _ when DictionarySerializableTypeMetadata.IsValidMapType(symbol) => new DictionarySerializableTypeMetadata(From(DictionarySerializableTypeMetadata.GetValuesTypeSymbol(symbol)), symbol),
                    _ when IterableSerializableTypeMetadata.IsValidArrayType(symbol) => new IterableSerializableTypeMetadata(From(IterableSerializableTypeMetadata.GetIterableItemsTypeSymbol(symbol)), symbol),
                    _ when EnumSerializableTypeMetadata.IsEnumType(symbol) => new EnumSerializableTypeMetadata(symbol),

                    _ => new RecordSerializableTypeMetadata(symbol),
                };
            }

            return null;
        }

        public override string ToString()
        {
            return StringRepresentation;
        }

        public bool Equals(SerializableTypeMetadata other)
        {
            return Kind == other.Kind && FullNameDisplay == other.FullNameDisplay;
        }
    }
}
