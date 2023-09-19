// using FluentAssertions;
//
// namespace DotNetAvroSerializer.Write.Tests
// {
//     public class NullablePrimitivesTests
//     {
//         [Theory]
//         [InlineData(326, "028C05")]
//         [InlineData(1235234, "02C4E49601")]
//         [InlineData(null, "00")]
//         public void SerializaNullableInt(int? input, string hexStringResult)
//         {
//             var result = new NullableIntegerSerializer().Serialize(input);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexStringResult);
//         }
//
//         [Theory]
//         [InlineData(3263453453458L, "02A4F2DECFFABD01")]
//         [InlineData(long.MaxValue, "02FEFFFFFFFFFFFFFFFF01")]
//         [InlineData(null, "00")]
//         public void SerializeNullableLong(long? originLong, string hexString)
//         {
//             var serializer = new NullableLongSerializer();
//
//             var result = serializer.Serialize(originLong);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData("foo", "0206666F6F")]
//         [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur consectetur finibus tempus. Ut eros odio, auctor eu turpis quis, finibus sodales ipsum. Morbi at sollicitudin leo, ac tincidunt massa. Vivamus.", "02A4034C6F72656D20697073756D20646F6C6F722073697420616D65742C20636F6E73656374657475722061646970697363696E6720656C69742E2043757261626974757220636F6E73656374657475722066696E696275732074656D7075732E2055742065726F73206F64696F2C20617563746F722065752074757270697320717569732C2066696E6962757320736F64616C657320697073756D2E204D6F72626920617420736F6C6C696369747564696E206C656F2C2061632074696E636964756E74206D617373612E20566976616D75732E")]
//         [InlineData(null, "00")]
//         public void SerializeNullableString(string? originString, string hexString)
//         {
//             var serializer = new NullableStringSerializer();
//
//             var result = serializer.Serialize(originString);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//
//         [Theory]
//         [InlineData(true, "0201")]
//         [InlineData(false, "0200")]
//         [InlineData(null, "00")]
//         public void SerializeNullableBool(bool? originBoolean, string hexString)
//         {
//             var serializer = new NullableBooleanSerializer();
//
//             var result = serializer.Serialize(originBoolean);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData(1.2d, "02333333333333F33F")]
//         [InlineData(124.34d, "02F6285C8FC2155F40")]
//         [InlineData(null, "00")]
//         public void SerializeNullableDouble(double? originDouble, string hexString)
//         {
//             var serializer = new NullableDoubleSerializer();
//
//             var result = serializer.Serialize(originDouble);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData(1.2F, "029A99993F")]
//         [InlineData(124.34F, "0214AEF842")]
//         [InlineData(null, "00")]
//         public void SerializeNullableFloat(float? originFloat, string hexString)
//         {
//             var serializer = new NullableFloatSerializer();
//
//             var result = serializer.Serialize(originFloat);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData(new byte[] { 123, 253, 100, 10 }, "02087BFD640A")]
//         [InlineData(null, "00")]
//         public void SerializeNullableBytes(byte[]? originBytes, string hexString)
//         {
//             var serializer = new NullableBytesSerializer();
//
//             var result = serializer.Serialize(originBytes);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//     }
//
//     [AvroSchema(@"{ ""type"": [""null"", ""int""] }")]
//     public partial class NullableIntegerSerializer : AvroSerializer<int?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [""null"", ""long""] }")]
//     public partial class NullableLongSerializer : AvroSerializer<long?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", ""string""] }")]
//     public partial class NullableStringSerializer : AvroSerializer<string?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", ""boolean"" ] }")]
//     public partial class NullableBooleanSerializer : AvroSerializer<bool?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", ""double"" ] }")]
//     public partial class NullableDoubleSerializer : AvroSerializer<double?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", ""bytes"" ] }")]
//     public partial class NullableBytesSerializer : AvroSerializer<byte[]?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", ""float"" ] }")]
//     public partial class NullableFloatSerializer : AvroSerializer<float?>
//     {
//
//     }
// }
