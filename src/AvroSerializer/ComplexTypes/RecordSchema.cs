namespace DotNetAvroSerializer.ComplexTypes
{
    public static class RecordSchema
    {
        public static bool CanSerialize(object? value) => value is not null && value.GetType().IsClass && value is not string;
    }
}
