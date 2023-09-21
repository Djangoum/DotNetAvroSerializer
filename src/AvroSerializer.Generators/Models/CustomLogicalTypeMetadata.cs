using System.Collections.Generic;

namespace DotNetAvroSerializer.Generators.Models;

public class CustomLogicalTypeMetadata
{
    public string Name { get; set; }
    public string LogicalTypeFullyQualifiedName { get; set; }
    public IEnumerable<string> OrderedSchemaProperties { get; set; }
}