using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class RecordSerializableTypeMetadata : SerializableTypeMetadata
    {
        public RecordSerializableTypeMetadata(ITypeSymbol typeSymbol)
            : base(typeSymbol)
        {
            Fields = typeSymbol
                    .GetMembers()
                    .Where(s => s.Kind is SymbolKind.Property)
                    .Cast<IPropertySymbol>()
                    .Select(t => new FieldSerializableTypeMetadata(From(t.Type), t, t.Name));
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Record;
        internal string Name { get; set; }

        internal IEnumerable<FieldSerializableTypeMetadata> Fields { get; }

        public override bool Equals(SerializableTypeMetadata other)
        {
            return base.Equals(other)
                && other is RecordSerializableTypeMetadata recordSerializableType 
                && Fields.SequenceEqual(recordSerializableType.Fields);
        }
    }
}
