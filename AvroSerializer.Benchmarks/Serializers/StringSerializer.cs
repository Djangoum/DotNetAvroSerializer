namespace AvroSerializer.Benchmarks.Serializers
{
    [AvroSchema($@"{{ ""type"": ""string"" }}")]
    public partial class StringSerializer : AvroSerializer<string>
    {
    }
}
