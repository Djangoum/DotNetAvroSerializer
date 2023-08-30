using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests
{
    public class ComplexUnionsTests
    {
        [Theory]
        [MemberData(nameof(RecordsUnionTestData))]
        public void SerializeRecordsUnion(UnionSideOne? unionSideOne, UnionSideTwo? unionSideTwo, string hexString)
        {
            Union<UnionSideOne, UnionSideTwo> union;

            if (unionSideOne is not null)
            {
                union = unionSideOne;
            }
            else
            {
                union = unionSideTwo!;
            }

            var serializer = new RecordsUnionSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        public static IEnumerable<object[]> RecordsUnionTestData =>
            new List<object[]>
            {
                new object[] { 
                    new UnionSideOne
                    {
                        Name = "name",
                        Id = 1,
                    },
                    null!,
                    "0002086E616D65"
                },
                new object[] {
                    null!,
                    new UnionSideTwo
                    {
                        SecondName = "name",
                        Identifier = 1
                    },
                    "0202086E616D65"
                }
            };
    }

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
    public partial class RecordsUnionSerializer : AvroSerializer<Union<UnionSideOne, UnionSideTwo>>
    {

    }
}
