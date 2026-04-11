using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncPrimitivesTests
{
    // ── SerializeAsync ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("foo", "06666F6F")]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur consectetur finibus tempus. Ut eros odio, auctor eu turpis quis, finibus sodales ipsum. Morbi at sollicitudin leo, ac tincidunt massa. Vivamus.", "A4034C6F72656D20697073756D20646F6C6F722073697420616D65742C20636F6E73656374657475722061646970697363696E6720656C69742E2043757261626974757220636F6E73656374657475722066696E696275732074656D7075732E2055742065726F73206F64696F2C20617563746F722065752074757270697320717569732C2066696E6962757320736F64616C657320697073756D2E204D6F72626920617420736F6C6C696369747564696E206C656F2C2061632074696E636964756E74206D617373612E20566976616D75732E")]
    public async Task SerializeStringAsync(string input, string expected)
    {
        var result = await new StringSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(326, "8C05")]
    [InlineData(1235234, "C4E49601")]
    public async Task SerializeIntAsync(int input, string expected)
    {
        var result = await new IntSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(3263453453458L, "A4F2DECFFABD01")]
    [InlineData(long.MaxValue, "FEFFFFFFFFFFFFFFFF01")]
    public async Task SerializeLongAsync(long input, string expected)
    {
        var result = await new LongSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, "01")]
    [InlineData(false, "00")]
    public async Task SerializeBoolAsync(bool input, string expected)
    {
        var result = await new BooleanSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2d, "333333333333F33F")]
    [InlineData(124.34d, "F6285C8FC2155F40")]
    public async Task SerializeDoubleAsync(double input, string expected)
    {
        var result = await new DoubleSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2F, "9A99993F")]
    [InlineData(124.34F, "14AEF842")]
    public async Task SerializeFloatAsync(float input, string expected)
    {
        var result = await new FloatSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, "087BFD640A")]
    public async Task SerializeBytesAsync(byte[] input, string expected)
    {
        var result = await new BytesSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    // ── SerializeToStreamAsync ────────────────────────────────────────────────

    [Theory]
    [InlineData("foo", "06666F6F")]
    [InlineData("bar", "06626172")]
    public async Task SerializeStringToStreamAsync(string input, string expected)
    {
        using var stream = new MemoryStream();

        await new StringSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(326, "8C05")]
    [InlineData(1235234, "C4E49601")]
    public async Task SerializeIntToStreamAsync(int input, string expected)
    {
        using var stream = new MemoryStream();

        await new IntSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(3263453453458L, "A4F2DECFFABD01")]
    [InlineData(long.MaxValue, "FEFFFFFFFFFFFFFFFF01")]
    public async Task SerializeLongToStreamAsync(long input, string expected)
    {
        using var stream = new MemoryStream();

        await new LongSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, "01")]
    [InlineData(false, "00")]
    public async Task SerializeBoolToStreamAsync(bool input, string expected)
    {
        using var stream = new MemoryStream();

        await new BooleanSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2d, "333333333333F33F")]
    [InlineData(124.34d, "F6285C8FC2155F40")]
    public async Task SerializeDoubleToStreamAsync(double input, string expected)
    {
        using var stream = new MemoryStream();

        await new DoubleSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2F, "9A99993F")]
    [InlineData(124.34F, "14AEF842")]
    public async Task SerializeFloatToStreamAsync(float input, string expected)
    {
        using var stream = new MemoryStream();

        await new FloatSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, "087BFD640A")]
    public async Task SerializeBytesToStreamAsync(byte[] input, string expected)
    {
        using var stream = new MemoryStream();

        await new BytesSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }
}
#pragma warning restore CA2007
