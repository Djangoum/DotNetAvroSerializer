namespace DotNetAvroSerializer.Write.Tests;

public class RealTypesTests
{

    [AvroSchema(@"{
      ""type"": [
        ""null"",
        {
          ""logicalType"": ""regex-string"",
          ""type"": ""string"",
          ""regex"": "".+""
        }
      ]
    }", new []{ typeof(RegexStringLogicalType) })]
    public partial class NullRegexStringSerializer : AvroSerializer<Union<Null, string>>
    {
        
    }
    
    [LogicalTypeName("regex-string")]
    public static class RegexStringLogicalType
    {
        public static bool CanSerialize(object? value, string regex2esgrers) => value is string;

        public static string ConvertToBaseSchemaType(string logicalTypeValue, string regex)
        {
          return logicalTypeValue;
        }
    }
}