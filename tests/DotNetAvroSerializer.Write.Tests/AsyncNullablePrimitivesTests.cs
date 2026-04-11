using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncNullablePrimitivesTests
{

    [Theory]
    [InlineData(326, "028C05")]
    [InlineData(1235234, "02C4E49601")]
    [InlineData(null, "00")]
    public async Task SerializeNullableIntAsync(int? input, string expected)
    {
        var result = await new AsyncNullableIntegerSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(3263453453458L, "02A4F2DECFFABD01")]
    [InlineData(long.MaxValue, "02FEFFFFFFFFFFFFFFFF01")]
    [InlineData(null, "00")]
    public async Task SerializeNullableLongAsync(long? input, string expected)
    {
        var result = await new AsyncNullableLongSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("foo", "0206666F6F")]
    [InlineData(null, "00")]
    public async Task SerializeNullableStringAsync(string? input, string expected)
    {
        var result = await new AsyncNullableStringSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, "0201")]
    [InlineData(false, "0200")]
    [InlineData(null, "00")]
    public async Task SerializeNullableBoolAsync(bool? input, string expected)
    {
        var result = await new AsyncNullableBooleanSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2d, "02333333333333F33F")]
    [InlineData(124.34d, "02F6285C8FC2155F40")]
    [InlineData(null, "00")]
    public async Task SerializeNullableDoubleAsync(double? input, string expected)
    {
        var result = await new AsyncNullableDoubleSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1.2F, "029A99993F")]
    [InlineData(124.34F, "0214AEF842")]
    [InlineData(null, "00")]
    public async Task SerializeNullableFloatAsync(float? input, string expected)
    {
        var result = await new AsyncNullableFloatSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, "02087BFD640A")]
    [InlineData(null, "00")]
    public async Task SerializeNullableBytesAsync(byte[]? input, string expected)
    {
        var result = await new AsyncNullableBytesSerializer().SerializeAsync(input);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(326, "028C05")]
    [InlineData(null, "00")]
    public async Task SerializeNullableIntToStreamAsync(int? input, string expected)
    {
        using var stream = new MemoryStream();

        await new AsyncNullableIntegerSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("foo", "0206666F6F")]
    [InlineData(null, "00")]
    public async Task SerializeNullableStringToStreamAsync(string? input, string expected)
    {
        using var stream = new MemoryStream();

        await new AsyncNullableStringSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, "0201")]
    [InlineData(null, "00")]
    public async Task SerializeNullableBoolToStreamAsync(bool? input, string expected)
    {
        using var stream = new MemoryStream();

        await new AsyncNullableBooleanSerializer().SerializeToStreamAsync(stream, input);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }
}
#pragma warning restore CA2007

[AvroSchema(@"{ ""type"": [""null"", ""int""] }")]
public partial class AsyncNullableIntegerSerializer : AsyncAvroSerializer<int?> { }

[AvroSchema(@"{ ""type"": [""null"", ""long""] }")]
public partial class AsyncNullableLongSerializer : AsyncAvroSerializer<long?> { }

[AvroSchema(@"{ ""type"": [ ""null"", ""string""] }")]
public partial class AsyncNullableStringSerializer : AsyncAvroSerializer<string?> { }

[AvroSchema(@"{ ""type"": [ ""null"", ""boolean"" ] }")]
public partial class AsyncNullableBooleanSerializer : AsyncAvroSerializer<bool?> { }

[AvroSchema(@"{ ""type"": [ ""null"", ""double"" ] }")]
public partial class AsyncNullableDoubleSerializer : AsyncAvroSerializer<double?> { }

[AvroSchema(@"{ ""type"": [ ""null"", ""bytes"" ] }")]
public partial class AsyncNullableBytesSerializer : AsyncAvroSerializer<byte[]?> { }

[AvroSchema(@"{ ""type"": [ ""null"", ""float"" ] }")]
public partial class AsyncNullableFloatSerializer : AsyncAvroSerializer<float?> { }
