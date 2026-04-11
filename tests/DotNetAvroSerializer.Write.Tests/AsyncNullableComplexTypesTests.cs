using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncNullableComplexTypesTests
{
    // ── SerializeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task SerializeNullableRecordValueAsync()
    {
        var result = await new NullableRecordSerializer().SerializeAsync(new UnionSideOne { Id = 1, Name = "name" });

        Convert.ToHexString(result).Should().BeEquivalentTo("0002086E616D65");
    }

    [Fact]
    public async Task SerializeNullableRecordNullAsync()
    {
        var result = await new NullableRecordSerializer().SerializeAsync(null);

        Convert.ToHexString(result).Should().BeEquivalentTo("02");
    }

    [Theory]
    [MemberData(nameof(SerializeEnumerableData))]
    public async Task SerializeNullableArrayIEnumerableAsync(IEnumerable<int>? input, string expected)
    {
        var result = await new NullableStringArrayAnnotatedEnumerable().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(SerializeNullableMapData))]
    public async Task SerializeNullableMapWithNullableRecordAsync(Dictionary<string, UnionSideOne?> input, string expected)
    {
        var result = await new NullableMapOfNullableRecords().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    // ── SerializeToStreamAsync ────────────────────────────────────────────────

    [Fact]
    public async Task SerializeNullableRecordValueToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new NullableRecordSerializer().SerializeToStreamAsync(stream, new UnionSideOne { Id = 1, Name = "name" });

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("0002086E616D65");
    }

    [Fact]
    public async Task SerializeNullableRecordNullToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new NullableRecordSerializer().SerializeToStreamAsync(stream, null);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("02");
    }

    [Fact]
    public async Task SerializeNullableArrayToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new NullableStringArrayAnnotatedEnumerable().SerializeToStreamAsync(stream, new int[] { 1, 2, 3, 4 });

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("00080204060800");
    }

    // ── MemberData ────────────────────────────────────────────────────────────

    public static IEnumerable<object[]> SerializeEnumerableData =>
        new List<object[]>
        {
            new object[] { new int[] { 1, 2, 3, 4 }, "00080204060800" },
            new object[] { null!, "02" }
        };

    public static IEnumerable<object[]> SerializeNullableMapData =>
        new List<object[]>
        {
            new object[]
            {
                new Dictionary<string, UnionSideOne?> { { "item1", null } },
                "00020A6974656D310000"
            },
            new object[]
            {
                new Dictionary<string, UnionSideOne?> { { "item1", new UnionSideOne { Id = 1, Name = "name" } } },
                "00020A6974656D310202086E616D6500"
            },
            new object[] { null!, "02" }
        };
}
#pragma warning restore CA2007
