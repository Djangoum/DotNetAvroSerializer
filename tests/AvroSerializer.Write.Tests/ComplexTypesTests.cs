using AvroSerializer.Write.Tests.Models;
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

        [Fact]
        public void SeralizeRecordWithPrimitiveTypes()
        {
            ClassWithPrimitivesSerializer serializer = new ClassWithPrimitivesSerializer();

            var result = serializer.Serialize(new ClassWithPrimitives
            {
                BoolField = true,
                BytesField = new byte[] { 1, 2, 3, 4 },
                DoubleField = 12.5d,
                FloatField = 15.6f,
                IntegerField = 42,
                LongField = 1234346567567563454,
                StringField = "test text"
            });

            Convert.ToHexString(result).Should().BeEquivalentTo("54FCEAB299BAEAA3A122127465737420746578749A9979410000000000002940010801020304");
        }

        [Fact]
        public void SerializeFixed()
        {
            FixedSerializer serializer = new FixedSerializer();

            var result = serializer.Serialize(new byte[] { 1, 2, 3, 4 });

            Convert.ToHexString(result).Should().BeEquivalentTo("0801020304");
        }
    }

    [AvroSchema(@"{ ""type"": ""fixed"", ""size"" : 4, ""name"": ""sixteenLenght"" }")]
    public partial class FixedSerializer : AvroSerializer<byte[]>
    {

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

    [AvroSchema(@"
    {
        ""type"": ""record"",
        ""name"" : ""classWithPrimitivesSerializer"",
        ""fields"" :[
            { 
                ""name"": ""IntegerField"",
                ""type"": ""int""
            },
            {
                ""name"": ""LongField"",
                ""type"": ""long""
            },
            {
                ""name"": ""StringField"",
                ""type"": ""string""
            },
            {
                ""name"": ""FloatField"",
                ""type"": ""float""
            },
            {
                ""name"": ""DoubleField"",
                ""type"": ""double""
            },
            {
                ""name"": ""BoolField"",
                ""type"": ""boolean""
            },
            {
                ""name"": ""BytesField"",
                ""type"": ""bytes""
            }
        ]
    }")]
    public partial class ClassWithPrimitivesSerializer : AvroSerializer<ClassWithPrimitives>
    {

    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}
