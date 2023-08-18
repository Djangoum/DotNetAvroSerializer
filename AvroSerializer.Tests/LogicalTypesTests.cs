using FluentAssertions;

namespace AvroSerializer.Write.Tests
{
    public class LogicalTypesTests
    {
        [Fact]
        public void SerializeUuid()
        {
            var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");

            UuidSerializer serializer = new UuidSerializer();

            var result = serializer.Serialize(uuid);

            Convert.ToHexString(result).Should().BeEquivalentTo("4861383236643838662D343561662D346238642D386662352D353731303632363164646536");
        }

        [Fact]
        public void SerializeDate()
        {
            var date = new DateOnly(2023, 01, 02);

            var serializer = new DateSerializer();

            var result = serializer.Serialize(date);

            Convert.ToHexString(result).Should().BeEquivalentTo("BEAE02");
        }

        [Fact]
        public void SerializeTime()
        {
            var date = new TimeOnly(20, 01, 02);

            var serializer = new TimeMillisSerializer();

            var result = serializer.Serialize(date);

            Convert.ToHexString(result).Should().BeEquivalentTo("E0D0DC44");
        }

        [Fact]
        public void SerializeTiemstamp()
        {
            var date = new DateTime(2023, 01, 02, 15, 36, 55);

            var serializer = new TimestampMillisSerializer();

            var result = serializer.Serialize(date);

            Convert.ToHexString(result).Should().BeEquivalentTo("B0A787B2AE61");
        }
    }

    [AvroSchema(@"{ ""type"": ""enum"", ""name"": ""Foo"", ""symbols"": [""Value1"", ""Value""] }")]
    public partial class EnumSerializer : AvroSerializer<EnumerationValues>
    {

    }

    public enum EnumerationValues
    {
        Value1,
        Value
    }

    [AvroSchema(@"{ ""type"": ""int"", ""logicalType"": ""date"" }")]
    public partial class DateSerializer : AvroSerializer<DateOnly>
    {

    }

    [AvroSchema(@"{ ""type"": ""long"", ""logicalType"": ""timestamp-millis"" }")]
    public partial class TimestampMillisSerializer : AvroSerializer<DateTime>
    {

    }

    [AvroSchema(@"{ ""type"": ""int"", ""logicalType"": ""time-millis"" }")]
    public partial class TimeMillisSerializer : AvroSerializer<TimeOnly>
    {

    }

    [AvroSchema(@"{ ""type"": ""string"", ""logicalType"": ""uuid"" }")]
    public partial class UuidSerializer : AvroSerializer<Guid>
    {

    }
}
