using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class LogicalTypeSerializableTypeMetadata : SerializableTypeMetadata
    {
        public LogicalTypeSerializableTypeMetadata(ITypeSymbol logicalTypeSymbol)
            : base(logicalTypeSymbol)
        {
            TypeName = logicalTypeSymbol.Name;
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.LogicalType;
        internal string TypeName { get; set; }

        internal static bool IsValidLogicalType(ITypeSymbol typeSymbol)
        {
            return typeSymbol.Name
                is "DateTime"
                or "DateOnly"
                or "TimeOnly"
                or "Guid";
        }
    }
}
