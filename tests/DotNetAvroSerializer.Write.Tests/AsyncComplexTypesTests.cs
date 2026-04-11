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
    public async Task SerializeEnumAsync()
    {
        var result = await new EnumSerializer().SerializeAsync(TestEnum.Value3);

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
    public async Task SerializeRecordWithPrimitiveTypesAsync()
    {
        var result = await new ClassWithPrimitivesSerializer().SerializeAsync(new ClassWithPrimitives
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
        var result = await new FixedSerializer().SerializeAsync(new byte[] { 1, 2, 3, 4 });

        Convert.ToHexString(result).Should().BeEquivalentTo("0801020304");
    }

    [Fact]
    public async Task SerializeRecordWithComplexTypesAsync()
    {
        var result = await new RecordWithComplexTypesSerializer().SerializeAsync(new RecordWithComplexTypes
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

        await new EnumSerializer().SerializeToStreamAsync(stream, TestEnum.Value3);

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

        await new ClassWithPrimitivesSerializer().SerializeToStreamAsync(stream, new ClassWithPrimitives
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
}
#pragma warning restore CA2007
