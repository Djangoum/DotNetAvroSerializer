using DotNetAvroSerializer.Write.Tests.Models;
using FluentAssertions;

namespace DotNetAvroSerializer.Write.Tests;

#pragma warning disable CA2007
public class AsyncComplexTypesTests
{

    [Fact]
    public async Task SerializeArrayOfIntsAsync()
    {
        var result = await new IntArraySerializer().SerializeAsync(new int[] { 1, 2, 3, 4 });

        Convert.ToHexString(result).Should().BeEquivalentTo("080204060800");
    }

    [Fact]
    public async Task SerializeArrayOfIntsAsync_WithStreamingBlocks()
    {
        var serializer = new IntArraySerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(new int[] { 1, 2, 3, 4 });

        Convert.ToHexString(result).Should().BeEquivalentTo("04020404060800");
    }

    [Fact]
    public async Task SerializeArrayOfIntsAsIAsyncEnumerableAsync_WithStreamingBlocks()
    {
        var serializer = new IntAsyncEnumerableSerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(GetIntsAsyncEnumerable());

        Convert.ToHexString(result).Should().BeEquivalentTo("04020404060800");
    }

    [Fact]
    public async Task SerializeEnumAsync()
    {
        var result = await new AsyncEnumSerializer().SerializeAsync(TestEnum.Value3);

        Convert.ToHexString(result).Should().BeEquivalentTo("04");
    }

    [Fact]
    public async Task SerializeMapOfIntsWithIDictionaryAsync()
    {
        var result = await new IDictionaryMapSerializer().SerializeAsync(new Dictionary<string, int>
        {
            { "item1", 1 },
            { "item2", 2 },
            { "item3", 3 }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("060A6974656D31020A6974656D32040A6974656D330600");
    }

    [Fact]
    public async Task SerializeMapOfIntsWithDictionaryAsync()
    {
        var result = await new DictionaryMapSerializer().SerializeAsync(new Dictionary<string, int>
        {
            { "item1", 1 },
            { "item2", 2 },
            { "item3", 3 }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("060A6974656D31020A6974656D32040A6974656D330600");
    }

    [Fact]
    public async Task SerializeMapOfIntsWithDictionaryAsync_WithStreamingBlocks()
    {
        var serializer = new DictionaryMapSerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(new Dictionary<string, int>
        {
            { "item1", 1 },
            { "item2", 2 },
            { "item3", 3 }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("040A6974656D31020A6974656D3204020A6974656D330600");
    }

    [Fact]
    public async Task SerializeArrayOfRecordsAsync()
    {
        var result = await new ArrayOfRecordsSerializer().SerializeAsync(new List<InnerRecord>
        {
            new InnerRecord { Field1 = "holiwis", Field2 = 2 },
            new InnerRecord { Field1 = "holiwis", Field2 = 2 }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("040E686F6C69776973040E686F6C697769730400");
    }

    [Fact]
    public async Task SerializeArrayOfRecordsAsync_WithStreamingBlocks()
    {
        var serializer = new ArrayOfRecordsSerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(new List<InnerRecord>
        {
            new InnerRecord { Field1 = "holiwis", Field2 = 2 },
            new InnerRecord { Field1 = "holiwis", Field2 = 2 },
            new InnerRecord { Field1 = "holiwis", Field2 = 2 }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("040E686F6C69776973040E686F6C6977697304020E686F6C697769730400");
    }

    [Fact]
    public async Task SerializeArrayOfRecordsAsIAsyncEnumerableAsync_WithStreamingBlocks()
    {
        var serializer = new AsyncEnumerableOfRecordsSerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(GetInnerRecordsAsyncEnumerable());

        Convert.ToHexString(result).Should().BeEquivalentTo("040E686F6C69776973040E686F6C6977697304020E686F6C697769730400");
    }

    [Fact]
    public async Task SerializeMapOfRecordsAsync_WithStreamingBlocks()
    {
        var serializer = new AsyncMapOfRecordsSerializer
        {
            AsyncArrayItemCountBlockSize = 2
        };

        var result = await serializer.SerializeAsync(new Dictionary<string, InnerRecord>
        {
            { "item1", new InnerRecord { Field1 = "holiwis", Field2 = 2 } },
            { "item2", new InnerRecord { Field1 = "holiwis", Field2 = 2 } },
            { "item3", new InnerRecord { Field1 = "holiwis", Field2 = 2 } }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("040A6974656D310E686F6C69776973040A6974656D320E686F6C6977697304020A6974656D330E686F6C697769730400");
    }

    [Fact]
    public async Task SerializeRecordWithPrimitiveTypesAsync()
    {
        var result = await new AsyncClassWithPrimitivesSerializer().SerializeAsync(new ClassWithPrimitives
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
    public async Task SerializeFixedAsync()
    {
        var result = await new AsyncFixedSerializer().SerializeAsync(new byte[] { 1, 2, 3, 4 });

        Convert.ToHexString(result).Should().BeEquivalentTo("0801020304");
    }

    [Fact]
    public async Task SerializeRecordWithComplexTypesAsync()
    {
        var result = await new AsyncRecordWithComplexTypesSerializer().SerializeAsync(new RecordWithComplexTypes
        {
            InnerRecord = new InnerRecord { Field1 = "teststring", Field2 = 124 },
            Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
            InnerRecords = new[]
            {
                new InnerRecord { Field1 = "teststring", Field2 = 124 },
                new InnerRecord { Field1 = "teststring", Field2 = 124 }
            },
            NullableFloat = 12.6f,
            MapField = new Dictionary<string, InnerRecord>
            {
                { "key1", new InnerRecord { Field1 = "teststring", Field2 = 124 } },
                { "key2", new InnerRecord { Field1 = "teststring", Field2 = 124 } }
            }
        });

        Convert.ToHexString(result).Should().BeEquivalentTo("1474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F80100");
    }

    [Fact]
    public async Task SerializeArrayOfIntsToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new IntArraySerializer().SerializeToStreamAsync(stream, new int[] { 1, 2, 3, 4 });

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("080204060800");
    }

    [Fact]
    public async Task SerializeEnumToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new AsyncEnumSerializer().SerializeToStreamAsync(stream, TestEnum.Value3);

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("04");
    }

    [Fact]
    public async Task SerializeMapOfIntsToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new IDictionaryMapSerializer().SerializeToStreamAsync(stream, new Dictionary<string, int>
        {
            { "item1", 1 },
            { "item2", 2 },
            { "item3", 3 }
        });

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("060A6974656D31020A6974656D32040A6974656D330600");
    }

    [Fact]
    public async Task SerializeRecordWithPrimitiveTypesToStreamAsync()
    {
        using var stream = new MemoryStream();

        await new AsyncClassWithPrimitivesSerializer().SerializeToStreamAsync(stream, new ClassWithPrimitives
        {
            BoolField = true,
            BytesField = new byte[] { 1, 2, 3, 4 },
            DoubleField = 12.5d,
            FloatField = 15.6f,
            IntegerField = 42,
            LongField = 1234346567567563454,
            StringField = "test text"
        });

        Convert.ToHexString(stream.ToArray()).Should().BeEquivalentTo("54FCEAB299BAEAA3A122127465737420746578749A9979410000000000002940010801020304");
    }

    [Fact]
    public async Task SerializeArrayOfRecordsWithComplexTypesAsync()
    {
        var result = await new ArrayOfRecordWithComplexTypesSerializer().SerializeAsync(
            new List<RecordWithComplexTypes>
            {
                new RecordWithComplexTypes
                {
                    InnerRecord = new InnerRecord { Field1 = "teststring", Field2 = 124 },
                    Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
                    InnerRecords = new[]
                    {
                        new InnerRecord { Field1 = "teststring", Field2 = 124 },
                        new InnerRecord { Field1 = "teststring", Field2 = 124 }
                    },
                    NullableFloat = 12.6f,
                    MapField = new Dictionary<string, InnerRecord>
                    {
                        { "key1", new InnerRecord { Field1 = "teststring", Field2 = 124 } },
                        { "key2", new InnerRecord { Field1 = "teststring", Field2 = 124 } }
                    }
                },
                new RecordWithComplexTypes
                {
                    InnerRecord = new InnerRecord { Field1 = "teststring", Field2 = 124 },
                    Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
                    InnerRecords = new[]
                    {
                        new InnerRecord { Field1 = "teststring", Field2 = 124 },
                        new InnerRecord { Field1 = "teststring", Field2 = 124 }
                    },
                    NullableFloat = 12.6f,
                    MapField = new Dictionary<string, InnerRecord>
                    {
                        { "key1", new InnerRecord { Field1 = "teststring", Field2 = 124 } },
                        { "key2", new InnerRecord { Field1 = "teststring", Field2 = 124 } }
                    }
                }
            });

        Convert.ToHexString(result).Should().BeEquivalentTo("041474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F801001474657374737472696E67F801041474657374737472696E67F8011474657374737472696E67F8010006333333333333F33F3333333333330B40333333333333294000029A99494104086B6579311474657374737472696E67F801086B6579321474657374737472696E67F8010000");
    }

    private static async IAsyncEnumerable<int> GetIntsAsyncEnumerable()
    {
        yield return 1;
        await Task.Yield();
        yield return 2;
        await Task.Yield();
        yield return 3;
        await Task.Yield();
        yield return 4;
    }

    private static async IAsyncEnumerable<InnerRecord> GetInnerRecordsAsyncEnumerable()
    {
        yield return new InnerRecord { Field1 = "holiwis", Field2 = 2 };
        await Task.Yield();
        yield return new InnerRecord { Field1 = "holiwis", Field2 = 2 };
        await Task.Yield();
        yield return new InnerRecord { Field1 = "holiwis", Field2 = 2 };
    }
}
#pragma warning restore CA2007

[AvroSchema(@"{ ""type"": ""enum"", ""name"": ""foo"", ""symbols"": [ ""Value1"", ""Value2"", ""Value3"" ]}")]
public partial class AsyncEnumSerializer : AsyncAvroSerializer<TestEnum> { }

[AvroSchema(@"{ ""type"": ""fixed"", ""size"" : 4, ""name"": ""sixteenLength"" }")]
public partial class AsyncFixedSerializer : AsyncAvroSerializer<byte[]> { }

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
public partial class AsyncClassWithPrimitivesSerializer : AsyncAvroSerializer<ClassWithPrimitives> { }

[AvroSchema(@"{
          ""type"": ""record"",
          ""name"" : ""recordWithComplexTypes"",
          ""fields"" :[
             {
                 ""name"": ""InnerRecord"",
                 ""type"": {
                     ""name"": ""InnerRecord"",
                     ""type"": ""record"",
                     ""fields"": [
                         {
                             ""name"": ""Field1"",
                             ""type"": ""string""
                         },
                         {
                             ""name"": ""Field2"",
                             ""type"": ""int""
                         }
                     ]
                 } 
             },
             {
                 ""name"": ""InnerRecords"",
                 ""type"": {
                     ""type"": ""array"",
                     ""items"": ""InnerRecord""
                 }
             },
             {
                 ""name"": ""Doubles"",
                 ""type"": {
                     ""type"": ""array"",
                     ""items"": ""double""
                 }
             },
             {
                 ""name"": ""NullableFloat"",
                 ""type"": [ ""null"", ""float"" ]
             },
             {
                 ""name"": ""MapField"",
                 ""type"": {
                     ""type"": ""map"",
                     ""names"": ""dictionary"",
                     ""values"": ""InnerRecord""
                 }
             }
          ]
      }")]
public partial class AsyncRecordWithComplexTypesSerializer : AsyncAvroSerializer<RecordWithComplexTypes> { }

[AvroSchema(@"{
         ""type"" : ""map"",
         ""values"": {
             ""name"": ""InnerRecord"",
             ""type"": ""record"",
             ""fields"": [
                 {
                     ""name"": ""Field1"",
                     ""type"": ""string""
                 },
                 {
                     ""name"": ""Field2"",
                     ""type"": ""int""
                 }
             ]
         }
     }")]
public partial class AsyncMapOfRecordsSerializer : AsyncAvroSerializer<Dictionary<string, InnerRecord>> { }

[AvroSchema(@"{ ""type"": ""array"", ""items"": ""int"" }")]
public partial class IntAsyncEnumerableSerializer : AsyncAvroSerializer<IAsyncEnumerable<int>> { }

[AvroSchema(@"{
         ""type"": ""array"",
         ""items"": {
             ""name"": ""InnerRecord"",
             ""type"": ""record"",
             ""fields"": [
                 {
                     ""name"": ""Field1"",
                     ""type"": ""string""
                 },
                 {
                     ""name"": ""Field2"",
                     ""type"": ""int""
                 }
             ]
         }
     }")]
public partial class AsyncEnumerableOfRecordsSerializer : AsyncAvroSerializer<IAsyncEnumerable<InnerRecord>> { }
