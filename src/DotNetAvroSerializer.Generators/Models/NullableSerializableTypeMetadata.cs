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
            => typeSymbol.NullableAnnotation is NullableAnnotation.Annotated || (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == "Nullable");

        internal static ITypeSymbol GetInnerNullableTypeSymbol(ITypeSymbol typeSymbol)
        {
             if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
             {
                if (namedTypeSymbol.TypeArguments.Any())
                {
                    return namedTypeSymbol.TypeArguments.First();
                }
                else
                {
                    return typeSymbol.WithNullableAnnotation(NullableAnnotation.None);
                }
            }
            else if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return arrayTypeSymbol.WithNullableAnnotation(NullableAnnotation.None);
            }

            return null;
        }

        public override bool Equals(SerializableTypeMetadata other)
        {
            return base.Equals(other)
                && other is NullableSerializableTypeMetadata nullableSerializableType
                && nullableSerializableType.InnerNullableTypeSymbol.Equals(nullableSerializableType.InnerNullableTypeSymbol);
        }
    }
}
