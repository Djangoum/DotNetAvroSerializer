using Microsoft.CodeAnalysis;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class NullableSerializableTypeMetadata : SerializableTypeMetadata
    {
        public NullableSerializableTypeMetadata(SerializableTypeMetadata nullableSerializableType, ITypeSymbol nullableSymbol)
            : base(nullableSymbol)
        {
            InnerNullableTypeSymbol = nullableSerializableType;
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Nullable;
        internal SerializableTypeMetadata InnerNullableTypeSymbol { get; }

        internal static bool IsNullableType(ITypeSymbol typeSymbol)
            => typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == "Nullable";

        internal static ITypeSymbol GetInnerNullableTypeSymbol(ITypeSymbol typeSymbol)
        {
            var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

            return namedTypeSymbol.TypeArguments.First();
        }
    }
}
