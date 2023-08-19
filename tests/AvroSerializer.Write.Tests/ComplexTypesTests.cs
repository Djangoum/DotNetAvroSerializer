using FluentAssertions;

namespace AvroSerializer.Write.Tests
{
    public class ComplexTypesTests
    {
        [Fact]
        public void SerializeArrayOfInts()
        {
            var array = new int[] { 1, 2, 3, 4 };

            IntArraySerializer serializer = new IntArraySerializer();

            var result = serializer.Serialize(array);

            Convert.ToHexString(result).Should().BeEquivalentTo("080204060800");
        }

        [Fact]
        public void SerializeEnum()
        {
            EnumSerializer serializer = new EnumSerializer();

            var result = serializer.Serialize(TestEnum.Value3);

            Convert.ToHexString(result).Should().BeEquivalentTo("04");
        }

        [Fact]
        public void SerializeMapOfInts()
        {
            MapSerializer serializer = new MapSerializer();

            var result = serializer.Serialize(new Dictionary<string, int>()
            {
                { "item1", 1 },
                { "item2", 2 },
                { "item3", 3 }
            });

            Convert.ToHexString(result).Should().BeEquivalentTo("060A6974656D31020A6974656D32040A6974656D330600");
        }
    }

    [AvroSchema(@"{ ""type"" : ""map"", ""values"": ""int"" }")]
    public partial class MapSerializer : AvroSerializer<IDictionary<string, int>>
    {

    }

    [AvroSchema(@"{ ""type"" : ""array"", ""items"": ""int"" }")]
    public partial class IntArraySerializer : AvroSerializer<int[]>
    {

    }

    [AvroSchema(@"{ ""type"": ""enum"", ""name"": ""foo"", ""symbols"": [ ""Value1"", ""Value2"", ""Value3"" ]}")]
    public partial class EnumSerializer : AvroSerializer<TestEnum>
    {

    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}
