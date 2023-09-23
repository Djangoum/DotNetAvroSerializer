using System.Collections.Generic;
using System.Linq;
using Avro.Util;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Models;

public class CustomLogicalTypeMetadata
{
    public CustomLogicalTypeMetadata(string name, string logicalTypeFullyQualifiedName, IEnumerable<IParameterSymbol> orderedSchemaProperties)
    {
        Name = name;
        LogicalTypeFullyQualifiedName = logicalTypeFullyQualifiedName;
        OrderedSchemaProperties = orderedSchemaProperties.Select(p =>
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

    public string Name { get; set; }
    public string LogicalTypeFullyQualifiedName { get; set; }
    public IEnumerable<string> OrderedSchemaProperties { get; set; }
}