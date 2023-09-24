namespace DotNetAvroSerializer.Write.Tests.Models;

public class RecordWithOverridenNames
{
    [AvroField("matching_name")]
    public required string NonMatchingName { get; set; }
}
