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
public partial class NullableRecordSerializer : AvroSerializer<UnionSideOne?>
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
public partial class NullableStringArrayAnnotatedEnumerable : AsyncAvroSerializer<IEnumerable<int>?>
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
public partial class NullableMapOfNullableRecords : AsyncAvroSerializer<Dictionary<string, UnionSideOne?>>
{

}
