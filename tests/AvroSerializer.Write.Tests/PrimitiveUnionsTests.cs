using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests
{
    public class PrimitiveUnionsTests
    {
        [Theory]
        [InlineData(326, null, "008C05")]
        [InlineData(null, 3263453453458L, "02A4F2DECFFABD01")]
        public void SerializeIntLongUnion(int? intValue, long? longValue, string hexString)
        {
            Union<int, long> union;

            if (intValue is null)
            {
                union = longValue!.Value;
            }
            else
            {
                union = intValue!.Value;
            }

            var serializer = new IntegerLongSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(3263453453458L, null, "00A4F2DECFFABD01")]
        [InlineData(null, "foo", "0206666F6F")]
        public void SerializeLongString(long? longValue, string? stringValue, string? hexString)
        {
            Union<long, string> union;

            if (longValue is null)
            {
                union = stringValue!;
            }
            else
            {
                union = longValue!.Value;
            }

            var serializer = new LongStringSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(true, null, "0001")]
        [InlineData(null, 124.34d, "02F6285C8FC2155F40")]
        public void SerializeBooleanDouble(bool? booleanValue, double? doubleValue, string hexString)
        {
            Union<bool, double> union;

            if (booleanValue is null)
            {
                union = doubleValue!.Value;
            }
            else
            {
                union = booleanValue!.Value;
            }

            var serializer = new BoolDoubleSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(new byte[] { 123, 253, 100, 10 }, null, "00087BFD640A")]
        [InlineData(null, 326, "028C05")]
        public void SerializeBytesInt(byte[]? bytesValue, int? intValue, string hexString)
        {
            Union<byte[], int> union;

            if (bytesValue is null)
            {
                union = intValue!.Value;
            }
            else
            {
                union = bytesValue!;
            }

            var serializer = new BytesIntSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData("foo", null, "0006666F6F")]
        [InlineData(null, 124.34F, "0214AEF842")]
        public void SerializeStringFloat(string? stringValue, float? floatValue, string hexString)
        {
            Union<string, float> union;

            if (stringValue is null)
            {
                union = floatValue!.Value;
            }
            else
            {
                union = stringValue!;
            }

            var serializer = new StringFloatSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(new byte[] { 123, 253, 100, 10 }, null, "00087BFD640A")]
        [InlineData(null, "foo", "0206666F6F")]
        public void SerializeBytesString(byte[]? bytesValue, string? stringValue, string hexString)
        {
            Union<byte[], string> union;

            if (bytesValue is null)
            {
                union = stringValue!;
            }
            else
            {
                union = bytesValue!;
            }

            var serializer = new BytesStringSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(new byte[] { 123, 253, 100, 10 }, null, null, null, "00087BFD640A")]
        [InlineData(null, "foo", null, null, "0206666F6F")]
        [InlineData(null, null, 326, null, "048C05")]
        [InlineData(null, null, null, 3263453453458L, "06A4F2DECFFABD01")]
        public void SerializeBytesStringIntLong(byte[]? bytes, string? stringValue, int? integer, long? longValue, string hexString)
        {
            Union<byte[], string, int, long> union;

            if (bytes is not null)
            {
                union = bytes!;
            }
            else if (stringValue is not null)
            {
                union = stringValue!;
            }
            else if (integer is not null)
            {
                union = integer!.Value;
            }
            else
            {
                union = longValue!.Value;
            }

            var serializer = new BytesStringIntLongSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }

        [Theory]
        [InlineData(326, null, "008C05")]
        [InlineData(null, 3263453453458L, "02A4F2DECFFABD01")]
        [InlineData(null, null, "04")]
        public void SerializeIntLongNull(int? integer, long? longValue, string hexString)
        {
            Union<int, long, Null> union;

            if (integer is not null)
            {
                union = integer!.Value;
            }
            else if (longValue is not null)
            {
                union = longValue!.Value;
            }
            else
            {
                union = Null.Instance;
            }

            var serializer = new IntLongNullSerializer();

            var result = serializer.Serialize(union);

            Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
        }
    }

    [AvroSchema(@"{ ""type"": [""int"", ""long""] }")]
    public partial class IntegerLongSerializer : AvroSerializer<Union<int, long>>
    {

    }

    [AvroSchema(@"{ ""type"": [""long"", ""string""] }")]
    public partial class LongStringSerializer : AvroSerializer<Union<long, string>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""boolean"", ""double""] }")]
    public partial class BoolDoubleSerializer : AvroSerializer<Union<bool, double>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""bytes"", ""int"" ] }")]
    public partial class BytesIntSerializer : AvroSerializer<Union<byte[], int>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""string"", ""float"" ] }")]
    public partial class StringFloatSerializer : AvroSerializer<Union<string, float>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""bytes"", ""string"" ] }")]
    public partial class BytesStringSerializer : AvroSerializer<Union<byte[], string>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""bytes"", ""string"", ""int"", ""long"" ] }")]
    public partial class BytesStringIntLongSerializer : AvroSerializer<Union<byte[], string, int, long>>
    {

    }

    [AvroSchema(@"{ ""type"": [ ""int"", ""long"", ""null"" ] }")]
    public partial class IntLongNullSerializer : AvroSerializer<Union<int, long, Null>>
    {

    }
}
