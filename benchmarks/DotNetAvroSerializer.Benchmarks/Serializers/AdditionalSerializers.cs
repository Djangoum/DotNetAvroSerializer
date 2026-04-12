using System.Text.RegularExpressions;
using DotNetAvroSerializer.Benchmarks.Models;

namespace DotNetAvroSerializer.Benchmarks.Serializers;

[AvroSchema(@"{ ""type"": [""int"", ""long""] }")]
public partial class PrimitiveUnionSerializer : AvroSerializer<Union<int, long>>
{
}

[AvroSchema(@"{ ""type"": [""null"", ""string""] }")]
public partial class NullableStringUnionSerializer : AvroSerializer<string?>
{
}

[AvroSchema(@"{
  ""type"": ""record"",
  ""name"": ""LogicalTypeRecord"",
  ""fields"": [
    { ""name"": ""DateField"", ""type"": { ""type"": ""int"", ""logicalType"": ""date"" } },
    { ""name"": ""TimeField"", ""type"": { ""type"": ""int"", ""logicalType"": ""time-millis"" } },
    { ""name"": ""TimestampField"", ""type"": { ""type"": ""long"", ""logicalType"": ""timestamp-millis"" } },
    { ""name"": ""UuidField"", ""type"": { ""type"": ""string"", ""logicalType"": ""uuid"" } }
  ]
}")]
public partial class LogicalTypeRecordSerializer : AvroSerializer<LogicalTypeRecord>
{
}

[AvroSchema(@"{
  ""type"": [ 
    ""null"", 
    { ""logicalType"": ""regex-string"", ""type"": ""string"", ""regex"": "".+""} 
  ]
}", new[] { typeof(RegexStringLogicalType) })]
public partial class NullableRegexStringSerializer : AvroSerializer<string?>
{
}

[LogicalTypeName("regex-string")]
public static class RegexStringLogicalType
{
    public static bool CanSerialize(object? value, string regex) => value is string;

    public static string ConvertToBaseSchemaType(string logicalTypeValue, [LogicalTypePropertyName("regex")] string regexPattern)
    {
        var regex = new Regex(regexPattern, RegexOptions.Compiled);

        if (!regex.Matches(logicalTypeValue).Any())
        {
            throw new DotNetAvroSerializer.Exceptions.AvroSerializationException("Regex validation failed");
        }

        return logicalTypeValue;
    }
}
