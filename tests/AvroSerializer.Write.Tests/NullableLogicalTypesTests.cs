// using FluentAssertions;
// using System.Globalization;
//
// namespace DotNetAvroSerializer.Write.Tests
// {
//     public class NullableLogicalTypesTests
//     {
//
//         [Theory]
//         [InlineData(null, "00")]
//         [InlineData("a826d88f-45af-4b8d-8fb5-57106261dde6", "024861383236643838662D343561662D346238642D386662352D353731303632363164646536")]
//         public void SerializeUuid(string? uuidString, string hexString)
//         {
//             var parsed = Guid.TryParse(uuidString, out var uuid);
//
//             NullableUuidSerializer serializer = new NullableUuidSerializer();
//
//             var result = serializer.Serialize(parsed ? uuid : null);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData(null, "00")]
//         [InlineData("2023-01-02", "02BEAE02")]
//         public void SerializeDate(string? dateString, string hexString)
//         {
//             var parsed = DateOnly.TryParse(dateString, out var date);
//
//             var serializer = new NullableDateSerializer();
//
//             var result = serializer.Serialize(parsed ? date : null);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [InlineData(null, "00")]
//         [InlineData("20:01:02", "02E0D0DC44")]
//         public void SerializeTime(string? timeOnlyTuple, string hexString)
//         {
//             var parsed = TimeOnly.TryParse(timeOnlyTuple, out var timeOnly);
//
//             var serializer = new NullableTimeMillisSerializer();
//
//             var result = serializer.Serialize(parsed ? timeOnly : null);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         [Theory]
//         [MemberData(nameof(SerializeTimestampData))]
//         public void SerializeTiemstamp(DateTime? dateString, string hexString)
//         {
//             var serializer = new NullableTimestampMillisSerializer();
//
//             var result = serializer.Serialize(dateString is not null ? dateString.Value : null);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo(hexString);
//         }
//
//         public static IEnumerable<object[]> SerializeTimestampData =>
//             new List<object[]>
//             {
//                 new object[] { null!, "00" },
//                 new object[] { new DateTime(2023, 01, 02, 15, 36, 55, DateTimeKind.Utc), "02B0A787B2AE61" }
//             };
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", { ""type"": ""int"", ""logicalType"": ""date"" } ] }")]
//     public partial class NullableDateSerializer : AvroSerializer<DateOnly?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", { ""type"": ""long"", ""logicalType"": ""timestamp-millis"" } ] }")]
//     public partial class NullableTimestampMillisSerializer : AvroSerializer<DateTime?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", { ""type"" :""int"", ""logicalType"": ""time-millis"" } ] }")]
//     public partial class NullableTimeMillisSerializer : AvroSerializer<TimeOnly?>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": [ ""null"", { ""type"" : ""string"", ""logicalType"": ""uuid"" } ] }")]
//     public partial class NullableUuidSerializer : AvroSerializer<Guid?>
//     {
//
//     }
// }