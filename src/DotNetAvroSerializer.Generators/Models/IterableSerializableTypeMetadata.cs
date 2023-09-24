﻿using Microsoft.CodeAnalysis;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class IterableSerializableTypeMetadata : SerializableTypeMetadata
    {
        public IterableSerializableTypeMetadata(SerializableTypeMetadata itemsTypeMetadata, ISymbol iterableSymbol)
            : base(iterableSymbol)
        {
            ItemsTypeMetadata = itemsTypeMetadata;
        }

        internal SerializableTypeMetadata ItemsTypeMetadata { get; }

        protected override SerializableTypeKind Kind => SerializableTypeKind.Enumerable;

        internal static bool IsValidArrayType(ITypeSymbol symbol)
            => symbol is IArrayTypeSymbol
                || symbol is INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol 
                && namedTypeSymbol.AllInterfaces.Any(i => i.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T);

        internal static ITypeSymbol GetIterableItemsTypeSymbol(ITypeSymbol iterableTypeSymbol)
            => iterableTypeSymbol switch
            {
                IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.ElementType,
                INamedTypeSymbol { SpecialType: not SpecialType.System_String } namedTypeSymbol when namedTypeSymbol.AllInterfaces.Any(i => i.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T) => namedTypeSymbol.TypeArguments.First(),

                _ => null
            };

        public override bool Equals(SerializableTypeMetadata other)
        {
            return base.Equals(other)
                && other is IterableSerializableTypeMetadata iterableSerializableType
                && ItemsTypeMetadata.Equals(iterableSerializableType.ItemsTypeMetadata);
        }
    }
}
