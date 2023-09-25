namespace DotNetAvroSerializer.ComplexTypes;

public static class ArraySchema
{
    public static bool CanSerialize(object? value) => value is not null && value.GetType().IsArray;
}
