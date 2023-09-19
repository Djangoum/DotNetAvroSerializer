// using DotNetAvroSerializer;
// using DotNetAvroSerializer.Write.Tests.Models;
// using FluentAssertions;
//
// namespace DotNetAvroSerializer.Write.Tests
// {
//     public class ComplexTypesTests
//     {
//         [Fact]
//         public void SerializeArrayOfInts()
//         {
//             var array = new int[] { 1, 2, 3, 4 };
//
//             IntArraySerializer serializer = new IntArraySerializer();
//
//             var result = serializer.Serialize(array);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("080204060800");
//         }
//
//         [Fact]
//         public void SerializeEnum()
//         {
//             EnumSerializer serializer = new EnumSerializer();
//
//             var result = serializer.Serialize(TestEnum.Value3);
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("04");
//         }
//
//         [Fact]
//         public void SerializeMapOfInts()
//         {
//             MapSerializer serializer = new MapSerializer();
//
//             var result = serializer.Serialize(new Dictionary<string, int>()
//             {
//                 { "item1", 1 },
//                 { "item2", 2 },
//                 { "item3", 3 }
//             });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("060A6974656D31020A6974656D32040A6974656D330600");
//         }
//
//         [Fact]
//         public void SerializeArrayOfRecords()
//         {
//             ArrayOfRecordsSerializer serializer = new ArrayOfRecordsSerializer();
//
//             var result = serializer.Serialize(new List<InnerRecord>
//             {
//                 new InnerRecord
//                 {
//                     Field2 = 2,
//                     Field1 = "holiwis"
//                 },
//                 new InnerRecord
//                 {
//                     Field2 = 2,
//                     Field1 = "holiwis"
//                 }
//             });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("040E686F6C69776973040E686F6C697769730400");
//         }
//
//         [Fact]
//         public void SeralizeRecordWithPrimitiveTypes()
//         {
//             ClassWithPrimitivesSerializer serializer = new ClassWithPrimitivesSerializer();
//
//             var result = serializer.Serialize(new ClassWithPrimitives
//             {
//                 BoolField = true,
//                 BytesField = new byte[] { 1, 2, 3, 4 },
//                 DoubleField = 12.5d,
//                 FloatField = 15.6f,
//                 IntegerField = 42,
//                 LongField = 1234346567567563454,
//                 StringField = "test text"
//             });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("54FCEAB299BAEAA3A122127465737420746578749A9979410000000000002940010801020304");
//         }
//
//         [Fact]
//         public void SerializeFixed()
//         {
//             FixedSerializer serializer = new FixedSerializer();
//
//             var result = serializer.Serialize(new byte[] { 1, 2, 3, 4 });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("0801020304");
//         }
//
//         [Fact]
//         public void SerializeRecordWithComplexTypes()
//         {
//             RecordWithComplexTypesSerializer serializer = new RecordWithComplexTypesSerializer();
//
//             var result = serializer.Serialize(new RecordWithComplexTypes
//             {
//                 InnerRecord = new InnerRecord
//                 {
//                     Field1 = "teststring",
//                     Field2 = 124
//                 },
//                 Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
//                 InnerRecords = new[]
//                 {
//                     new InnerRecord
//                     {
//                         Field1 = "teststring",
//                         Field2 = 124
//                     },
//                     new InnerRecord
//                     {
//                         Field1 = "teststring",
//                         Field2 = 124
//                     }
//                 },
//                 NullableFloat = 12.6f,
//                 MapField = new Dictionary<string, InnerRecord>
//                 {
//                     {
//                         "key1",
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         }
//                     },
//                     {
//                         "key2",
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         }
//                     }
//                 }
//             });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("1474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F80100");
//         }
//
//         [Fact]
//         public void SerializeArrayOfRecordsWithComplextTypes()
//         {
//             ArrayOfRecordWithComplexTypesSerializer serializer = new ArrayOfRecordWithComplexTypesSerializer();
//
//             var result = serializer.Serialize(
//                 new List<RecordWithComplexTypes>
//                 {
//                 new RecordWithComplexTypes
//                 {
//                     InnerRecord = new InnerRecord
//                     {
//                         Field1 = "teststring",
//                         Field2 = 124
//                     },
//                     Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
//                     InnerRecords = new[]
//                     {
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         },
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         }
//                     },
//                     NullableFloat = 12.6f,
//                     MapField = new Dictionary<string, InnerRecord>
//                     {
//                         {
//                             "key1",
//                             new InnerRecord
//                             {
//                                 Field1 = "teststring",
//                                 Field2 = 124
//                             }
//                         },
//                         {
//                             "key2",
//                             new InnerRecord
//                             {
//                                 Field1 = "teststring",
//                                 Field2 = 124
//                             }
//                         }
//                     }
//                 },
//                 new RecordWithComplexTypes
//                 {
//                     InnerRecord = new InnerRecord
//                     {
//                         Field1 = "teststring",
//                         Field2 = 124
//                     },
//                     Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
//                     InnerRecords = new[]
//                     {
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         },
//                         new InnerRecord
//                         {
//                             Field1 = "teststring",
//                             Field2 = 124
//                         }
//                     },
//                     NullableFloat = 12.6f,
//                     MapField = new Dictionary<string, InnerRecord>
//                     {
//                         {
//                             "key1",
//                             new InnerRecord
//                             {
//                                 Field1 = "teststring",
//                                 Field2 = 124
//                             }
//                         },
//                         {
//                             "key2",
//                             new InnerRecord
//                             {
//                                 Field1 = "teststring",
//                                 Field2 = 124
//                             }
//                         }
//                     }
//                 }
//                 });
//
//             Convert.ToHexString(result).Should().BeEquivalentTo("041474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F801001474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F8010000");
//         }
//     }
//
//     [AvroSchema(@"{
//         ""type"": ""record"",
//         ""name"" : ""recordWithOverridenNames"",
//         ""fields"": [
//             {
//                 ""name"": ""matching_name"",
//                 ""type"": ""string""
//             }
//         ]
//     }")]
//     public partial class RecordWitOverridenFieldNamesSerializer : AvroSerializer<RecordWithOverridenNames>
//     {
//
//     }
//
//     [AvroSchema(@"{
//         ""type"": ""array"",
//         ""items"": {
//             ""type"": ""record"",
//             ""name"" : ""recordWithComplexTypes"",
//             ""fields"" :[
//                 {
//                     ""name"": ""InnerRecord"",
//                     ""type"": {
//                         ""name"": ""InnerRecord"",
//                         ""type"": ""record"",
//                         ""fields"": [
//                             {
//                                 ""name"": ""Field1"",
//                                 ""type"": ""string""
//                             },
//                             {
//                                 ""name"": ""Field2"",
//                                 ""type"": ""int""
//                             }
//                         ]
//                     } 
//                 },
//                 {
//                     ""name"": ""InnerRecords"",
//                     ""type"": {
//                         ""type"": ""array"",
//                         ""items"": ""InnerRecord""
//                     }
//                 },
//                 {
//                     ""name"": ""Doubles"",
//                     ""type"": {
//                         ""type"": ""array"",
//                         ""items"": ""double""
//                     }
//                 },
//                 {
//                     ""name"": ""NullableFloat"",
//                     ""type"": [ ""null"", ""float"" ]
//                 },
//                 {
//                     ""name"": ""MapField"",
//                     ""type"": {
//                         ""type"": ""map"",
//                         ""names"": ""dictionary"",
//                         ""values"": ""InnerRecord""
//                     }
//                 }
//             ]
//         }
//     }")]
//     public partial class ArrayOfRecordWithComplexTypesSerializer : AvroSerializer<IEnumerable<RecordWithComplexTypes>>
//     {
//
//     }
//     [AvroSchema(@"{
//         ""type"": ""array"",
//         ""items"": {
//             ""name"": ""InnerRecord"",
//             ""type"": ""record"",
//             ""fields"": [
//                 {
//                     ""name"": ""Field1"",
//                     ""type"": ""string""
//                 },
//                 {
//                     ""name"": ""Field2"",
//                     ""type"": ""int""
//                 }
//             ]
//         } 
//     }")]
//     public partial class ArrayOfRecordsSerializer : AvroSerializer<IEnumerable<InnerRecord>>
//     {
//
//     }
//
//     [AvroSchema(@"{
//         ""type"": ""record"",
//         ""name"" : ""recordWithComplexTypes"",
//         ""fields"" :[
//             {
//                 ""name"": ""InnerRecord"",
//                 ""type"": {
//                     ""name"": ""InnerRecord"",
//                     ""type"": ""record"",
//                     ""fields"": [
//                         {
//                             ""name"": ""Field1"",
//                             ""type"": ""string""
//                         },
//                         {
//                             ""name"": ""Field2"",
//                             ""type"": ""int""
//                         }
//                     ]
//                 } 
//             },
//             {
//                 ""name"": ""InnerRecords"",
//                 ""type"": {
//                     ""type"": ""array"",
//                     ""items"": ""InnerRecord""
//                 }
//             },
//             {
//                 ""name"": ""Doubles"",
//                 ""type"": {
//                     ""type"": ""array"",
//                     ""items"": ""double""
//                 }
//             },
//             {
//                 ""name"": ""NullableFloat"",
//                 ""type"": [ ""null"", ""float"" ]
//             },
//             {
//                 ""name"": ""MapField"",
//                 ""type"": {
//                     ""type"": ""map"",
//                     ""names"": ""dictionary"",
//                     ""values"": ""InnerRecord""
//                 }
//             }
//         ]
//     }")]
//     public partial class RecordWithComplexTypesSerializer : AvroSerializer<RecordWithComplexTypes>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": ""fixed"", ""size"" : 4, ""name"": ""sixteenLenght"" }")]
//     public partial class FixedSerializer : AvroSerializer<byte[]>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"" : ""map"", ""values"": ""int"" }")]
//     public partial class MapSerializer : AvroSerializer<IDictionary<string, int>>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"" : ""array"", ""items"": ""int"" }")]
//     public partial class IntArraySerializer : AvroSerializer<int[]>
//     {
//
//     }
//
//     [AvroSchema(@"{ ""type"": ""enum"", ""name"": ""foo"", ""symbols"": [ ""Value1"", ""Value2"", ""Value3"" ]}")]
//     public partial class EnumSerializer : AvroSerializer<TestEnum>
//     {
//
//     }
//
//     [AvroSchema(@"
//     {
//         ""type"": ""record"",
//         ""name"" : ""classWithPrimitivesSerializer"",
//         ""fields"" :[
//             { 
//                 ""name"": ""IntegerField"",
//                 ""type"": ""int""
//             },
//             {
//                 ""name"": ""LongField"",
//                 ""type"": ""long""
//             },
//             {
//                 ""name"": ""StringField"",
//                 ""type"": ""string""
//             },
//             {
//                 ""name"": ""FloatField"",
//                 ""type"": ""float""
//             },
//             {
//                 ""name"": ""DoubleField"",
//                 ""type"": ""double""
//             },
//             {
//                 ""name"": ""BoolField"",
//                 ""type"": ""boolean""
//             },
//             {
//                 ""name"": ""BytesField"",
//                 ""type"": ""bytes""
//             }
//         ]
//     }")]
//     public partial class ClassWithPrimitivesSerializer : AvroSerializer<ClassWithPrimitives>
//     {
//
//     }
//
//     public enum TestEnum
//     {
//         Value1,
//         Value2,
//         Value3
//     }
// }
