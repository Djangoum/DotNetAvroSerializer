using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncLogicalTypesTests
{

    [Fact]
    public async Task SerializeUuidAsync()
    {
        var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");

        var result = await new AsyncUuidSerializer().SerializeAsync(uuid);

        Convert.ToHexString(result).Should().BeEquivalentTo("4861383236643838662D343561662D346238642D386662352D353731303632363164646536");
    }

    [Fact]
    public async Task SerializeDateAsync()
    {
        var date = new DateOnly(2023, 01, 02);

        var result = await new AsyncDateSerializer().SerializeAsync(date);

        Convert.ToHexString(result).Should().BeEquivalentTo("BEAE02");
    }

    [Fact]
    public async Task SerializeTimeMillisAsync()
    {
        var time = new TimeOnly(20, 01, 02);

        var result = await new AsyncTimeMillisSerializer().SerializeAsync(time);

        Convert.ToHexString(result).Should().BeEquivalentTo("E0D0DC44");
    }

    [Fact]
    public async Task SerializeTimestampMillisAsync()
    {
        var dateTime = new DateTime(2023, 01, 02, 15, 36, 55);

        var result = await new AsyncTimestampMillisSerializer().SerializeAsync(dateTime);

        Convert.ToHexString(result).Should().BeEquivalentTo("B0A787B2AE61");
    }

    [Fact]
    public async Task SerializeUuidToStreamAsync()
    {
        var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");
        using var stream = new MemoryStream();

        await new AsyncUuidSerializer().SerializeToStreamAsync(stream, uuid);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("4861383236643838662D343561662D346238642D386662352D353731303632363164646536");
    }

    [Fact]
    public async Task SerializeDateToStreamAsync()
    {
        var date = new DateOnly(2023, 01, 02);
        using var stream = new MemoryStream();

        await new AsyncDateSerializer().SerializeToStreamAsync(stream, date);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("BEAE02");
    }

    [Fact]
    public async Task SerializeTimeMillisToStreamAsync()
    {
        var time = new TimeOnly(20, 01, 02);
        using var stream = new MemoryStream();

        await new AsyncTimeMillisSerializer().SerializeToStreamAsync(stream, time);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("E0D0DC44");
    }

    [Fact]
    public async Task SerializeTimestampMillisToStreamAsync()
    {
        var dateTime = new DateTime(2023, 01, 02, 15, 36, 55);
        using var stream = new MemoryStream();

        await new AsyncTimestampMillisSerializer().SerializeToStreamAsync(stream, dateTime);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("B0A787B2AE61");
    }
}
#pragma warning restore CA2007

[AvroSchema(@"{ ""type"": ""int"", ""logicalType"": ""date"" }")]
public partial class AsyncDateSerializer : AsyncAvroSerializer<DateOnly> { }

[AvroSchema(@"{ ""type"": ""long"", ""logicalType"": ""timestamp-millis"" }")]
public partial class AsyncTimestampMillisSerializer : AsyncAvroSerializer<DateTime> { }

[AvroSchema(@"{ ""type"": ""int"", ""logicalType"": ""time-millis"" }")]
public partial class AsyncTimeMillisSerializer : AsyncAvroSerializer<TimeOnly> { }

[AvroSchema(@"{ ""type"": ""string"", ""logicalType"": ""uuid"" }")]
public partial class AsyncUuidSerializer : AsyncAvroSerializer<Guid> { }
