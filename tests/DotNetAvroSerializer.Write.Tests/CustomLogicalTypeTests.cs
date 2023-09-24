using System.Text.RegularExpressions;
using DotNetAvroSerializer.Exceptions;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

public class CustomLogicalTypeTests
{
    [Theory]
    [InlineData("", true)]
    [InlineData("foo", false)]
    public void SerializeNullRegexStringThrowCheckCustomLogicalType(string stringValue, bool shouldThrow)
    {
        var serializer = new NullRegexStringSerializer();

        var serializationFunc = () => serializer.Serialize(stringValue);

        if (shouldThrow)
            serializationFunc.Should().ThrowExactly<AvroSerializationException>();
        else
            serializationFunc.Should().NotThrow();
    }

    [Theory]
    [InlineData("foo", "0206666F6F")]
    [InlineData(null, "00")]
    public void SerializeNullRegexString(string? stringValue, string hexString)
    {
        var serializer = new NullRegexStringSerializer();

        var result = serializer.Serialize(stringValue);

        Convert.ToHexString(result).Should().Be(hexString);
    }
}

[AvroSchema(@"{
      ""type"": [
        ""null"",
        {
          ""logicalType"": ""regex-string"",
          ""type"": ""string"",
          ""regex"": "".+""
        }
      ]
    }", new[] { typeof(RegexStringLogicalType) })]
public partial class NullRegexStringSerializer : AvroSerializer<string?>
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
            throw new AvroSerializationException("Regex validation failed");
        }

        return logicalTypeValue;
    }
}