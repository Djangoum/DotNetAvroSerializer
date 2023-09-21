using DotNetAvroSerializer;

namespace ConsoleApp1.Serializers;

[AvroSchema(@"{
		  ""logicalType"": ""regex-string"",
		  ""type"": ""string"",
		  ""regex"": "".+""
		}", new[] { typeof(RegexStringLogicalType) })]
public partial class IntegerLongSerializer : AvroSerializer<string>
{

}
        
[LogicalTypeName("regex-string")]
public static class RegexStringLogicalType
{
    public static bool CanSerialize(object? value) => value is string;

    public static object ConvertToBaseSchemaType(string logicalTypeValue, [LogicalTypePropertyName("regex-string")]string regex)
    {
        return logicalTypeValue;
    }
}