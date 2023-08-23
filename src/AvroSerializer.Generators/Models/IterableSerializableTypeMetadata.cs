using Microsoft.CodeAnalysis;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class IterableSerializableTypeMetadata : SerializableTypeMetadata
    {
        public IterableSerializableTypeMetadata(SerializableTypeMetadata itemsTypeMetadata, ITypeSymbol iterableSymbol)
            : base(iterableSymbol)
        {
            ItemsTypeMetadata = itemsTypeMetadata;
        }

        internal SerializableTypeMetadata ItemsTypeMetadata { get; set; }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Enumerable;

        internal static bool IsValidArrayType(ITypeSymbol symbol)
            => symbol is IArrayTypeSymbol
                || symbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.AllInterfaces.Any(i => i.Name.Contains("IEnumerable"));

        internal static ITypeSymbol GetIterableItemsTypeSymbol(ITypeSymbol iterableTypeSymbol)
            => iterableTypeSymbol switch
            {
                IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.ElementType,
                INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.AllInterfaces.Any(i => i.Name.Contains("IEnumerable")) => namedTypeSymbol.TypeArguments.First(),

                _ => null
            };
    }
}
