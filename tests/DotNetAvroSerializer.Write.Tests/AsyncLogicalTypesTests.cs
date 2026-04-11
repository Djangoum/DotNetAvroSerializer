using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncLogicalTypesTests
{
    // ── SerializeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task SerializeUuidAsync()
    {
        var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");

        var result = await new UuidSerializer().SerializeAsync(uuid);

        Convert.ToHexString(result).Should().BeEquivalentTo("4861383236643838662D343561662D346238642D386662352D353731303632363164646536");
    }

    [Fact]
    public async Task SerializeDateAsync()
    {
        var date = new DateOnly(2023, 01, 02);

        var result = await new DateSerializer().SerializeAsync(date);

        Convert.ToHexString(result).Should().BeEquivalentTo("BEAE02");
    }

    [Fact]
    public async Task SerializeTimeMillisAsync()
    {
        var time = new TimeOnly(20, 01, 02);

        var result = await new TimeMillisSerializer().SerializeAsync(time);

        Convert.ToHexString(result).Should().BeEquivalentTo("E0D0DC44");
    }

    [Fact]
    public async Task SerializeTimestampMillisAsync()
    {
        var dateTime = new DateTime(2023, 01, 02, 15, 36, 55);

        var result = await new TimestampMillisSerializer().SerializeAsync(dateTime);

        Convert.ToHexString(result).Should().BeEquivalentTo("B0A787B2AE61");
    }

    // ── SerializeToStreamAsync ────────────────────────────────────────────────

    [Fact]
    public async Task SerializeUuidToStreamAsync()
    {
        var uuid = new Guid("a826d88f-45af-4b8d-8fb5-57106261dde6");
        using var stream = new MemoryStream();

        await new UuidSerializer().SerializeToStreamAsync(stream, uuid);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("4861383236643838662D343561662D346238642D386662352D353731303632363164646536");
    }

    [Fact]
    public async Task SerializeDateToStreamAsync()
    {
        var date = new DateOnly(2023, 01, 02);
        using var stream = new MemoryStream();

        await new DateSerializer().SerializeToStreamAsync(stream, date);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("BEAE02");
    }

    [Fact]
    public async Task SerializeTimeMillisToStreamAsync()
    {
        var time = new TimeOnly(20, 01, 02);
        using var stream = new MemoryStream();

        await new TimeMillisSerializer().SerializeToStreamAsync(stream, time);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("E0D0DC44");
    }

    [Fact]
    public async Task SerializeTimestampMillisToStreamAsync()
    {
        var dateTime = new DateTime(2023, 01, 02, 15, 36, 55);
        using var stream = new MemoryStream();

        await new TimestampMillisSerializer().SerializeToStreamAsync(stream, dateTime);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("B0A787B2AE61");
    }
}
#pragma warning restore CA2007
