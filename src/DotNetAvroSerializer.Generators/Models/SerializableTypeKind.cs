namespace DotNetAvroSerializer.Generators.Models
{
    public enum SerializableTypeKind
    {
        Record, 
        Primitive,
        Map,
        Enumerable,
        LogicalType,
        Field,
        Enum,
        Nullable,
        Union
    }
}
