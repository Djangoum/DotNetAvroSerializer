namespace DotNetAvroSerializer.Benchmarks.Models;

public sealed class LogicalTypeRecord
{
    public DateOnly DateField { get; set; }
    public TimeOnly TimeField { get; set; }
    public DateTime TimestampField { get; set; }
    public Guid UuidField { get; set; }
}
