using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class PrimitiveSerializableTypeMetadata : SerializableTypeMetadata
    {
        public PrimitiveSerializableTypeMetadata(ITypeSymbol primitiveTypeSymbol)
            : base(primitiveTypeSymbol)
        {
            TypeName = primitiveTypeSymbol.Name;
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Primitive;
        internal string TypeName { get; init; }

        internal static bool IsAllowedPrimitiveType (ITypeSymbol typeSymbol)
        {
            return typeSymbol.Name.Equals("string", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("int", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("long", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("bool", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("Boolean", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("double", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("float", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("single", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("null", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("Int32", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("Int64", System.StringComparison.InvariantCultureIgnoreCase)
                || typeSymbol.Name.Equals("byte", System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
