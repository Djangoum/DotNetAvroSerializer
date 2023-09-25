namespace DotNetAvroSerializer.ComplexTypes;

public static class RecordSchema
{
    public static bool CanSerialize<TRecord>(object? value) where TRecord : class => value is not null && value is TRecord;
}
