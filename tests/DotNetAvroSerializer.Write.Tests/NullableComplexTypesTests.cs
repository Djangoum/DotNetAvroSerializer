using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;
public class NullableComplexTypesTests
{
    [Fact]
    public void SerializeNullableRecordValue()
    {
        var serializer = new NullableRecordSerializer();

        var result = serializer.Serialize(new UnionSideOne
        {
            Name = "name",
            Id = 1
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("0002086E616D65");
    }

    [Fact]
    public void SerializeNullableRecordNull()
    {
        var serializer = new NullableRecordSerializer();

        var result = serializer.Serialize(null);

        Convert.ToHexString(result).Should().BeEquivalentTo("02");
    }

    [Theory]
    [MemberData(nameof(SerializeEnumerable))]
    public void SerializeNullableArrayIEnumerable(IEnumerable<int>? enumerable, string hexString)
    {
        var serializer = new NullableStringArrayAnnotatedEnumerable();

        var result = serializer.Serialize(enumerable);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [MemberData(nameof(SerializeNullableMap))]
    public void SerializeNullableMapWithNullableRecord(Dictionary<string, UnionSideOne?> map, string hexString)
    {
        var serializer = new NullableMapOfNullableRecords();

        var result = serializer.Serialize(map);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    public static IEnumerable<object[]> SerializeEnumerable =>
        new List<object[]>
        {
            new object[] { new int[] { 1, 2, 3, 4 }, "00080204060800" },
            new object[] { null!, "02" }
        };

    public static IEnumerable<object[]> SerializeNullableMap =>
        new List<object[]>
        {
            new object[] { new Dictionary<string, UnionSideOne?>
            {
                { "item1", null }
            }, "00020A6974656D310000" },
            new object[] { new Dictionary<string, UnionSideOne?>
            {
                {
                    "item1", new UnionSideOne
                    {
                        Name = "name",
                        Id = 1
                    }
                }
            }, "00020A6974656D310202086E616D6500" },
            new object[] { null!, "02" }
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
                ""type"": ""null""
            }
         ]
     }")]
public partial class NullableRecordSerializer : AvroSerializer<UnionSideOne>
{

}

[AvroSchema(@"{
         ""type"": [
             {
                 ""type"": ""array"",
                 ""items"" : ""int""
             },
             {
                ""type"": ""null""
            }
         ]
     }")]
public partial class NullableStringArrayAnnotatedEnumerable : AvroSerializer<IEnumerable<int>?>
{

}

[AvroSchema(@"{
    ""type"": [
        {
            ""type"": ""map"",
            ""values"" : [
                {
                    ""type"" : ""null""
                },
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
                }
            ]
        },
        {
            ""type"": ""null""
        }
    ]
}")]
public partial class NullableMapOfNullableRecords : AvroSerializer<Dictionary<string, UnionSideOne?>?>
{

}
