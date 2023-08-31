using DotNetAvroSerializer;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

public class PrimitivesTests
{
    [Theory]
    [InlineData("foo", "06666F6F")]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur consectetur finibus tempus. Ut eros odio, auctor eu turpis quis, finibus sodales ipsum. Morbi at sollicitudin leo, ac tincidunt massa. Vivamus.", "A4034C6F72656D20697073756D20646F6C6F722073697420616D65742C20636F6E73656374657475722061646970697363696E6720656C69742E2043757261626974757220636F6E73656374657475722066696E696275732074656D7075732E2055742065726F73206F64696F2C20617563746F722065752074757270697320717569732C2066696E6962757320736F64616C657320697073756D2E204D6F72626920617420736F6C6C696369747564696E206C656F2C2061632074696E636964756E74206D617373612E20566976616D75732E")]
    public void SerializeString(string originString, string hexString)
    {
        var serializer = new StringSerializer();

        var result = serializer.Serialize(originString);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(326, "8C05")]
    [InlineData(1235234, "C4E49601")]
    public void SerializeInt(int originInt, string hexString)
    {
        var serializer = new IntSerializer();

        var result = serializer.Serialize(originInt);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(3263453453458L, "A4F2DECFFABD01")]
    [InlineData(long.MaxValue, "FEFFFFFFFFFFFFFFFF01")]
    public void SerializeLong(long originLong, string hexString)
    {
        var serializer = new LongSerializer();

        var result = serializer.Serialize(originLong);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(true, "01")]
    [InlineData(false, "00")]
    public void SerializeBool(bool originBoolean, string hexString)
    {
        var serializer = new BooleanSerializer();

        var result = serializer.Serialize(originBoolean);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(1.2d, "333333333333F33F")]
    [InlineData(124.34d, "F6285C8FC2155F40")]
    public void SerializeDouble(double originDouble, string hexString)
    {
        var serializer = new DoubleSerializer();

        var result = serializer.Serialize(originDouble);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(1.2F, "9A99993F")]
    [InlineData(124.34F, "14AEF842")]
    public void SerializeFloat(float originFloat, string hexString)
    {
        var serializer = new FloatSerializer();

        var result = serializer.Serialize(originFloat);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, "087BFD640A")]
    public void SerializeBytes(byte[] originBytes, string hexString)
    {
        var serializer = new BytesSerializer();

        var result = serializer.Serialize(originBytes);

        Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
    }
}

[AvroSchema(@"{ ""type"": ""string"" }")]
public partial class StringSerializer : AvroSerializer<string>
{

}

[AvroSchema(@"{ ""type"": ""boolean"" }")]
public partial class BooleanSerializer : AvroSerializer<bool>
{

}

[AvroSchema(@"{ ""type"": ""double"" }")]
public partial class DoubleSerializer : AvroSerializer<double>
{

}

[AvroSchema(@"{ ""type"": ""bytes"" }")]
public partial class BytesSerializer : AvroSerializer<byte[]>
{

}

[AvroSchema(@"{ ""type"": ""int"" }")]
public partial class IntSerializer : AvroSerializer<int>
{

}

[AvroSchema(@"{ ""type"": ""long"" }")]
public partial class LongSerializer : AvroSerializer<long>
{

}

[AvroSchema(@"{ ""type"": ""float"" }")]
public partial class FloatSerializer : AvroSerializer<float>
{

}
