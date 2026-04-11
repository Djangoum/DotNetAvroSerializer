using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

internal sealed record CustomLogicalTypeMetadata
{
    internal CustomLogicalTypeMetadata(string name, string logicalTypeFullyQualifiedName, IEnumerable<IParameterSymbol> orderedConvertToBaseTypeParameters, IEnumerable<IParameterSymbol> canSerializeOrderedParameters)
    {
        Name = name;
        LogicalTypeFullyQualifiedName = logicalTypeFullyQualifiedName;
        OrderedSchemaPropertiesConvertToBaseType = orderedConvertToBaseTypeParameters.Select(GetParameterName).ToImmutableArray().AsEquatableArray();
        OrderedSchemaPropertiesCanSerialize = canSerializeOrderedParameters.Select(GetParameterName).ToImmutableArray().AsEquatableArray();
    }

    private static string GetParameterName(IParameterSymbol p)
    {
        var overridenName = p
            .GetAttributes()
            .FirstOrDefault(a => a
                .AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Equals("global::DotNetAvroSerializer.LogicalTypePropertyNameAttribute"))?
            .ConstructorArguments.First()
            .Value;

        return overridenName is not null ? overridenName.ToString() : p.Name;
    }

    internal string Name { get; }
    internal string LogicalTypeFullyQualifiedName { get; }
    internal EquatableArray<string> OrderedSchemaPropertiesConvertToBaseType { get; }
    internal EquatableArray<string> OrderedSchemaPropertiesCanSerialize { get; }
}
