using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace DotNetAvroSerializer.Generators.Models
{
    internal class DictionarySerializableTypeMetadata : SerializableTypeMetadata
    {
        public DictionarySerializableTypeMetadata(SerializableTypeMetadata valuesTypeMetadata, ITypeSymbol dictionaryTypeSymbol)
            : base(dictionaryTypeSymbol)
        {
            ValuesMetadata = valuesTypeMetadata;

            if (dictionaryTypeSymbol is not null  && dictionaryTypeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                KeysTypeName = namedTypeSymbol.TypeArguments.ElementAt(0).Name;
            }
        }

        internal override SerializableTypeKind Kind => SerializableTypeKind.Map;
        internal SerializableTypeMetadata ValuesMetadata { get; set; }
        internal string KeysTypeName { get; set; }

        internal static bool IsValidMapType(ITypeSymbol symbol)
            => symbol is INamedTypeSymbol dictionaryTypeSymbol 
            && (dictionaryTypeSymbol.Name == "IDictionary"
                || dictionaryTypeSymbol.AllInterfaces.Any(i => i.Name.Contains("Dictionary")));

        internal static ITypeSymbol GetValuesTypeSymbol(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol dictionaryTypeSymbol)
                return dictionaryTypeSymbol.TypeArguments.ElementAt(1);

            return null;
        } 
    }
}
