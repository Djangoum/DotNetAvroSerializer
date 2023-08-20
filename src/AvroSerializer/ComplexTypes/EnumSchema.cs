namespace AvroSerializer.ComplexTypes
{
    public static class EnumSchema
    {
        public static bool CanSerialize(object? value) => value is not null && value.GetType().IsEnum;
    }
}
