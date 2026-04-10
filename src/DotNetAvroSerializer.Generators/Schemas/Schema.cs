using System.Collections.Generic;

namespace DotNetAvroSerializer.Generators.Schemas;

internal abstract class Schema
{
    private readonly Dictionary<string, string> properties = new();

    protected Schema(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public string GetProperty(string propertyName) =>
        properties.TryGetValue(propertyName, out var value) ? value : null;

    protected void SetProperties(Dictionary<string, string> schemaProperties)
    {
        foreach (var (key, value) in schemaProperties)
        {
            properties[key] = value;
        }
    }
}

internal sealed class PrimitiveSchema : Schema
{
    public PrimitiveSchema(string name) : base(name)
    {
    }
}

internal sealed class ArraySchema : Schema
{
    public ArraySchema(Schema itemSchema) : base("array")
    {
        ItemSchema = itemSchema;
    }

    public Schema ItemSchema { get; }
}

internal sealed class EnumSchema : Schema
{
    public EnumSchema(string name, IReadOnlyList<string> symbols) : base(name)
    {
        Symbols = symbols;
    }

    public IReadOnlyList<string> Symbols { get; }
}

internal sealed class FixedSchema : Schema
{
    public FixedSchema(string name, int size) : base(name)
    {
        Size = size;
    }

    public int Size { get; }
}

internal sealed class MapSchema : Schema
{
    public MapSchema(Schema valueSchema) : base("map")
    {
        ValueSchema = valueSchema;
    }

    public Schema ValueSchema { get; }
}

internal sealed class Field
{
    public Field(string name, Schema schema)
    {
        Name = name;
        Schema = schema;
    }

    public string Name { get; }
    public Schema Schema { get; }
}

internal sealed class RecordSchema : Schema
{
    public RecordSchema(string name, IReadOnlyList<Field> fields) : base(name)
    {
        Fields = fields;
    }

    public IReadOnlyList<Field> Fields { get; }
}

internal sealed class UnionSchema : Schema
{
    public UnionSchema(IReadOnlyList<Schema> schemas) : base("union")
    {
        Schemas = schemas;
    }

    public IReadOnlyList<Schema> Schemas { get; }
}

internal sealed class LogicalSchema : Schema
{
    public LogicalSchema(string logicalTypeName, Schema baseSchema, Dictionary<string, string> properties) : base(baseSchema.Name)
    {
        LogicalTypeName = logicalTypeName;
        BaseSchema = baseSchema;
        SetProperties(properties);
    }

    public string LogicalTypeName { get; }
    public Schema BaseSchema { get; }
}
