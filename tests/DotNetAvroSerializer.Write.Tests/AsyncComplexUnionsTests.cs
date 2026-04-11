using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncComplexUnionsTests
{

    [Theory]
    [MemberData(nameof(RecordsUnionTestData))]
    public async Task SerializeRecordsUnionAsync(UnionSideOne? sideOne, UnionSideTwo? sideTwo, string expected)
    {
        Union<UnionSideOne, UnionSideTwo> union = sideOne is not null ? sideOne : sideTwo!;

        var result = await new AsyncRecordsUnionSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(RecordsUnionTestData))]
    public async Task SerializeRecordsUnionToStreamAsync(UnionSideOne? sideOne, UnionSideTwo? sideTwo, string expected)
    {
        Union<UnionSideOne, UnionSideTwo> union = sideOne is not null ? sideOne : sideTwo!;
        using var stream = new MemoryStream();

        await new AsyncRecordsUnionSerializer().SerializeToStreamAsync(stream, union);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> RecordsUnionTestData =>
        new List<object[]>
        {
            new object[] { new UnionSideOne { Id = 1, Name = "name" }, null!, "0002086E616D65" },
            new object[] { null!, new UnionSideTwo { Identifier = 1, SecondName = "name" }, "0202086E616D65" }
        };
}
#pragma warning restore CA2007

[AvroSchema(@"{
         ""type"": [
             {
                 ""type"": ""record"",
                 ""name"" : ""unionSideOne"",
                 ""fields"": [
                     {
                         ""name"": ""id"",
                         ""type"": ""int""
                     },
                     {
                         ""name"": ""name"",
                         ""type"": ""string""
                     }
                 ]
             },
             {
                 ""type"": ""record"",
                 ""name"": ""unionSideTwo"",
                 ""fields"": [
                     {
                         ""name"": ""identifier"",
                         ""type"": ""int""
                     },
                     {
                         ""name"": ""SecondName"",
                         ""type"": ""string""
                     }
                 ]
             }
         ]
     }")]
public partial class AsyncRecordsUnionSerializer : AsyncAvroSerializer<Union<UnionSideOne, UnionSideTwo>> { }
