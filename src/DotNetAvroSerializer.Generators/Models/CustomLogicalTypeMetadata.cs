using System.Collections.Generic;
using System.Linq;
using Avro.Util;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

public class CustomLogicalTypeMetadata
{
    public CustomLogicalTypeMetadata(string name, string logicalTypeFullyQualifiedName, IEnumerable<IParameterSymbol> orderedConvertToBaseTypeParameters, IEnumerable<IParameterSymbol> canSerializeOrderedParameters)
    {
        Name = name;
        LogicalTypeFullyQualifiedName = logicalTypeFullyQualifiedName;
        OrderedSchemaPropertiesConvertToBaseType = orderedConvertToBaseTypeParameters.Select(p =>
        {
            var overridenName = p
                .GetAttributes()
                .FirstOrDefault(a => a
                    .AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Equals("global::DotNetAvroSerializer.LogicalTypePropertyNameAttribute"))?
                .ConstructorArguments.First()
                .Value;

            return overridenName is not null ? overridenName.ToString() : p.Name;
        });

        OrderedSchemaPropertiesCanSerialize = canSerializeOrderedParameters.Select(p =>
        {
            var overridenName = p
                .GetAttributes()
                .FirstOrDefault(a => a
                    .AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Equals("global::DotNetAvroSerializer.LogicalTypePropertyNameAttribute"))?
                .ConstructorArguments.First()
                .Value;

            return overridenName is not null ? overridenName.ToString() : p.Name;
        });

        LogicalTypeFactory.Instance.Register(new CustomLogicalType(name));
    }

    public string Name { get; }
    public string LogicalTypeFullyQualifiedName { get; }
    public IEnumerable<string> OrderedSchemaPropertiesConvertToBaseType { get; }
    public IEnumerable<string> OrderedSchemaPropertiesCanSerialize { get; }
}