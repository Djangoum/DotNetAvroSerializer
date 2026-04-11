using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncNullableLogicalTypesTests
{
    [Theory]
    [InlineData(null, "00")]
    [InlineData("a826d88f-45af-4b8d-8fb5-57106261dde6", "024861383236643838662D343561662D346238642D386662352D353731303632363164646536")]
    public async Task SerializeNullableUuidAsync(string? uuidString, string expected)
    {
        var parsed = Guid.TryParse(uuidString, out var uuid);

        var result = await new NullableUuidSerializer().SerializeAsync(parsed ? uuid : null);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(null, "00")]
    [InlineData("2023-01-02", "02BEAE02")]
    public async Task SerializeNullableDateAsync(string? dateString, string expected)
    {
        var parsed = DateOnly.TryParse(dateString, out var date);

        var result = await new NullableDateSerializer().SerializeAsync(parsed ? date : null);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(null, "00")]
    [InlineData("20:01:02", "02E0D0DC44")]
    public async Task SerializeNullableTimeMillisAsync(string? timeString, string expected)
    {
        var parsed = TimeOnly.TryParse(timeString, out var time);

        var result = await new NullableTimeMillisSerializer().SerializeAsync(parsed ? time : null);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(TimestampData))]
    public async Task SerializeNullableTimestampMillisAsync(DateTime? input, string expected)
    {
        var result = await new NullableTimestampMillisSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task SerializeNullableUuidToStreamAsync()
    {
        var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");
        using var stream = new MemoryStream();

        await new NullableUuidSerializer().SerializeToStreamAsync(stream, uuid);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("024861383236643838662D343561662D346238642D386662352D353731303632363164646536");
    }

    [Fact]
    public async Task SerializeNullableUuidNullToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new NullableUuidSerializer().SerializeToStreamAsync(stream, null);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("00");
    }

    [Fact]
    public async Task SerializeNullableDateToStreamAsync()
    {
        var date = new DateOnly(2023, 01, 02);
        using var stream = new MemoryStream();

        await new NullableDateSerializer().SerializeToStreamAsync(stream, date);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("02BEAE02");
    }

    [Fact]
    public async Task SerializeNullableTimeMillisToStreamAsync()
    {
        var time = new TimeOnly(20, 01, 02);
        using var stream = new MemoryStream();

        await new NullableTimeMillisSerializer().SerializeToStreamAsync(stream, time);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("02E0D0DC44");
    }

    public static IEnumerable<object[]> TimestampData =>
        new List<object[]>
        {
            new object[] { null!, "00" },
            new object[] { new DateTime(2023, 01, 02, 15, 36, 55, DateTimeKind.Utc), "02B0A787B2AE61" }
        };
}
#pragma warning restore CA2007
