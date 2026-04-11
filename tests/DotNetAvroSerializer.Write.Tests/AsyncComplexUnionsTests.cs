using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncComplexUnionsTests
{
    // ── SerializeAsync ────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(RecordsUnionTestData))]
    public async Task SerializeRecordsUnionAsync(UnionSideOne? sideOne, UnionSideTwo? sideTwo, string expected)
    {
        Union<UnionSideOne, UnionSideTwo> union = sideOne is not null ? sideOne : sideTwo!;

        var result = await new RecordsUnionSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    // ── SerializeToStreamAsync ────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(RecordsUnionTestData))]
    public async Task SerializeRecordsUnionToStreamAsync(UnionSideOne? sideOne, UnionSideTwo? sideTwo, string expected)
    {
        Union<UnionSideOne, UnionSideTwo> union = sideOne is not null ? sideOne : sideTwo!;
        using var stream = new MemoryStream();

        await new RecordsUnionSerializer().SerializeToStreamAsync(stream, union);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    // ── MemberData ────────────────────────────────────────────────────────────

    public static IEnumerable<object[]> RecordsUnionTestData =>
        new List<object[]>
        {
            new object[] { new UnionSideOne { Id = 1, Name = "name" }, null!, "0002086E616D65" },
            new object[] { null!, new UnionSideTwo { Identifier = 1, SecondName = "name" }, "0202086E616D65" }
        };
}
#pragma warning restore CA2007
