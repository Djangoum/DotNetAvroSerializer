using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal class FieldSerializableTypeMetadata : SerializableTypeMetadata
{
    public FieldSerializableTypeMetadata(SerializableTypeMetadata innerSerializableType, IPropertySymbol propertySymbol, string propertyName)
        : base(propertySymbol.Type)
    {
        InnerSerializableType = innerSerializableType;
        Name = propertyName;

        GetFieldNames(propertySymbol);
    }

    protected override SerializableTypeKind Kind => SerializableTypeKind.Field;

    internal string Name { get; set; }
    internal ICollection<string> Names { get; private set; } = new List<string>();
    internal SerializableTypeMetadata InnerSerializableType { get; set; }

    private void GetFieldNames(ISymbol fieldSymbol)
    {
        var avroFieldAttributes = fieldSymbol
            .GetAttributes()
            .Where(a => a.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::DotNetAvroSerializer.AvroFieldAttribute");

        foreach (var avroAttribute in avroFieldAttributes)
        {
            if (avroAttribute.ConstructorArguments.Any())
            {
                var fieldName = avroAttribute.ConstructorArguments[0].Value.ToString();

                Names.Add(fieldName);
            }
        }

        Names = avroFieldAttributes.Select(a => a.ConstructorArguments[0].Value.ToString()).ToArray();
    }

    public override bool Equals(SerializableTypeMetadata other)
        => base.Equals(other)
            && other is FieldSerializableTypeMetadata fieldSerializableType
            && InnerSerializableType.Equals(fieldSerializableType);
}
