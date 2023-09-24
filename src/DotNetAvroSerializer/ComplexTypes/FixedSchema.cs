namespace DotNetAvroSerializer.ComplexTypes;

public static class FixedSchema
{
    public static bool CanSerialize(object? value) => value is byte[];
}
