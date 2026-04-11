using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncPrimitiveUnionsTests
{
    // ── SerializeAsync ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(326, null, "008C05")]
    [InlineData(null, 3263453453458L, "02A4F2DECFFABD01")]
    public async Task SerializeIntLongUnionAsync(int? intValue, long? longValue, string expected)
    {
        Union<int, long> union;
        if (intValue is null) union = longValue!.Value;
        else union = intValue.Value;

        var result = await new IntegerLongSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(3263453453458L, null, "00A4F2DECFFABD01")]
    [InlineData(null, "foo", "0206666F6F")]
    public async Task SerializeLongStringUnionAsync(long? longValue, string? stringValue, string expected)
    {
        Union<long, string> union;
        if (longValue is null) union = stringValue!;
        else union = longValue.Value;

        var result = await new LongStringSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, null, "0001")]
    [InlineData(null, 124.34d, "02F6285C8FC2155F40")]
    public async Task SerializeBooleanDoubleUnionAsync(bool? boolValue, double? doubleValue, string expected)
    {
        Union<bool, double> union;
        if (boolValue is null) union = doubleValue!.Value;
        else union = boolValue.Value;

        var result = await new BoolDoubleSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, null, "00087BFD640A")]
    [InlineData(null, 326, "028C05")]
    public async Task SerializeBytesIntUnionAsync(byte[]? bytesValue, int? intValue, string expected)
    {
        Union<byte[], int> union;
        if (bytesValue is null) union = intValue!.Value;
        else union = bytesValue;

        var result = await new BytesIntSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("foo", null, "0006666F6F")]
    [InlineData(null, 124.34F, "0214AEF842")]
    public async Task SerializeStringFloatUnionAsync(string? stringValue, float? floatValue, string expected)
    {
        Union<string, float> union;
        if (stringValue is null) union = floatValue!.Value;
        else union = stringValue;

        var result = await new StringFloatSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, null, "00087BFD640A")]
    [InlineData(null, "foo", "0206666F6F")]
    public async Task SerializeBytesStringUnionAsync(byte[]? bytesValue, string? stringValue, string expected)
    {
        Union<byte[], string> union;
        if (bytesValue is null) union = stringValue!;
        else union = bytesValue;

        var result = await new BytesStringSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(new byte[] { 123, 253, 100, 10 }, null, null, null, "00087BFD640A")]
    [InlineData(null, "foo", null, null, "0206666F6F")]
    [InlineData(null, null, 326, null, "048C05")]
    [InlineData(null, null, null, 3263453453458L, "06A4F2DECFFABD01")]
    public async Task SerializeBytesStringIntLongUnionAsync(byte[]? bytes, string? stringValue, int? integer, long? longValue, string expected)
    {
        Union<byte[], string, int, long> union;

        if (bytes is not null) union = bytes;
        else if (stringValue is not null) union = stringValue;
        else if (integer.HasValue) union = integer.Value;
        else union = longValue!.Value;

        var result = await new BytesStringIntLongSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(326, null, "008C05")]
    [InlineData(null, 3263453453458L, "02A4F2DECFFABD01")]
    [InlineData(null, null, "04")]
    public async Task SerializeIntLongNullUnionAsync(int? integer, long? longValue, string expected)
    {
        Union<int, long, Null> union;

        if (integer.HasValue) union = integer.Value;
        else if (longValue.HasValue) union = longValue.Value;
        else union = Null.Instance;

        var result = await new IntLongNullSerializer().SerializeAsync(union);

        Convert.ToHexString(result).Should().BeEquivalentTo(expected);
    }

    // ── SerializeToStreamAsync ────────────────────────────────────────────────

    [Theory]
    [InlineData(326, null, "008C05")]
    [InlineData(null, 3263453453458L, "02A4F2DECFFABD01")]
    public async Task SerializeIntLongUnionToStreamAsync(int? intValue, long? longValue, string expected)
    {
        Union<int, long> union;
        if (intValue is null) union = longValue!.Value;
        else union = intValue.Value;
        using var stream = new MemoryStream();

        await new IntegerLongSerializer().SerializeToStreamAsync(stream, union);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(true, null, "0001")]
    [InlineData(null, 124.34d, "02F6285C8FC2155F40")]
    public async Task SerializeBooleanDoubleUnionToStreamAsync(bool? boolValue, double? doubleValue, string expected)
    {
        Union<bool, double> union;
        if (boolValue is null) union = doubleValue!.Value;
        else union = boolValue.Value;
        using var stream = new MemoryStream();

        await new BoolDoubleSerializer().SerializeToStreamAsync(stream, union);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo(expected);
    }
}
#pragma warning restore CA2007
