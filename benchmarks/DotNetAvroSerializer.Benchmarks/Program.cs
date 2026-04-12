using System.IO.Pipelines;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using DotNetAvroSerializer;
using DotNetAvroSerializer.Benchmarks.BufferWriters;
using DotNetAvroSerializer.Benchmarks.Models;
using DotNetAvroSerializer.Benchmarks.Serializers;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

public enum BenchmarkPayloadSize
{
    Tiny,
    Medium,
    Large
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
public class DotNetAvroSerializersBenchmarks
{
    private readonly StringSerializer _stringSerializer = new();
    private readonly ClassWithPrimitivesSerializer _primitivesSerializer = new();
    private readonly RecordWithComplexTypesSerializer _complexSerializer = new();
    private readonly PrimitiveUnionSerializer _primitiveUnionSerializer = new();
    private readonly NullableStringUnionSerializer _nullableStringUnionSerializer = new();
    private readonly LogicalTypeRecordSerializer _logicalTypeRecordSerializer = new();
    private readonly NullableRegexStringSerializer _customLogicalTypeSerializer = new();
    private readonly PooledByteBufferWriter _pooledWriter = new();

    private string _stringValue = string.Empty;
    private ClassWithPrimitives _primitivesValue = null!;
    private RecordWithComplexTypes _complexValue = null!;
    private Union<int, long> _primitiveUnionValue;
    private string? _nullableStringUnionValue;
    private LogicalTypeRecord _logicalTypeValue = null!;
    private string? _customLogicalTypeValue;

    [Params(BenchmarkPayloadSize.Tiny, BenchmarkPayloadSize.Medium, BenchmarkPayloadSize.Large)]
    public BenchmarkPayloadSize Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var data = BenchmarkDataFactory.Create(Size);
        _stringValue = data.StringValue;
        _primitivesValue = data.PrimitivesValue;
        _complexValue = data.ComplexValue;
        _primitiveUnionValue = data.PrimitiveUnionValue;
        _nullableStringUnionValue = data.NullableStringUnionValue;
        _logicalTypeValue = data.LogicalTypeValue;
        _customLogicalTypeValue = data.CustomLogicalTypeValue;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _pooledWriter.Dispose();
    }

    [Benchmark]
    public byte[] DotNet_Serialize_String() => _stringSerializer.Serialize(_stringValue);

    [Benchmark]
    public void DotNet_SerializeToStream_String()
    {
        using var output = new MemoryStream();
        _stringSerializer.SerializeToStream(output, _stringValue);
    }

    [Benchmark]
    public void DotNet_SerializeToPooledBuffer_String()
    {
        _pooledWriter.Reset();
        using var output = new BufferWriterStream(_pooledWriter);
        _stringSerializer.SerializeToStream(output, _stringValue);
    }

    [Benchmark]
    public void DotNet_SerializeToPipeWriter_String()
    {
        var pipe = new Pipe();
        using var output = pipe.Writer.AsStream();
        _stringSerializer.SerializeToStream(output, _stringValue);
        output.Flush();
        pipe.Reader.TryRead(out var result);
        pipe.Reader.AdvanceTo(result.Buffer.End);
    }

    [Benchmark]
    public byte[] DotNet_Serialize_PrimitivesRecord() => _primitivesSerializer.Serialize(_primitivesValue);

    [Benchmark]
    public byte[] DotNet_Serialize_ComplexRecord() => _complexSerializer.Serialize(_complexValue);

    [Benchmark]
    public void DotNet_SerializeToStream_ComplexRecord()
    {
        using var output = new MemoryStream();
        _complexSerializer.SerializeToStream(output, _complexValue);
    }

    [Benchmark]
    public void DotNet_SerializeToPooledBuffer_ComplexRecord()
    {
        _pooledWriter.Reset();
        using var output = new BufferWriterStream(_pooledWriter);
        _complexSerializer.SerializeToStream(output, _complexValue);
    }

    [Benchmark]
    public void DotNet_SerializeToPipeWriter_ComplexRecord()
    {
        var pipe = new Pipe();
        using var output = pipe.Writer.AsStream();
        _complexSerializer.SerializeToStream(output, _complexValue);
        output.Flush();
        pipe.Reader.TryRead(out var result);
        pipe.Reader.AdvanceTo(result.Buffer.End);
    }

    [Benchmark]
    public byte[] DotNet_Serialize_PrimitiveUnion() => _primitiveUnionSerializer.Serialize(_primitiveUnionValue);

    [Benchmark]
    public byte[] DotNet_Serialize_NullableUnion() => _nullableStringUnionSerializer.Serialize(_nullableStringUnionValue);

    [Benchmark]
    public byte[] DotNet_Serialize_LogicalTypes() => _logicalTypeRecordSerializer.Serialize(_logicalTypeValue);

    [Benchmark]
    public byte[] DotNet_Serialize_CustomLogicalType() => _customLogicalTypeSerializer.Serialize(_customLogicalTypeValue);
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
public class ApacheAvroSerializersBenchmarks
{
    private readonly ApacheAvroStringSerializer _stringSerializer = new();
    private readonly ApacheAvroPrimitiveUnionSerializer _primitiveUnionSerializer = new();
    private readonly ApacheAvroNullableStringUnionSerializer _nullableUnionSerializer = new();
    private readonly ApacheAvroComplexRecordSerializer _complexRecordSerializer = new();

    private string _stringValue = string.Empty;
    private object _primitiveUnionValue = 0;
    private object? _nullableUnionValue;
    private RecordWithComplexTypes _complexValue = null!;

    [Params(BenchmarkPayloadSize.Tiny, BenchmarkPayloadSize.Medium, BenchmarkPayloadSize.Large)]
    public BenchmarkPayloadSize Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var data = BenchmarkDataFactory.Create(Size);
        _stringValue = data.StringValue;
        _primitiveUnionValue = data.PrimitiveUnionValue.GetUnionValue()!;
        _nullableUnionValue = data.NullableStringUnionValue;
        _complexValue = data.ComplexValue;
    }

    [Benchmark]
    public void ApacheAvro_SerializeToStream_String()
    {
        using var output = new MemoryStream();
        _stringSerializer.SerializeToStream(output, _stringValue);
    }

    [Benchmark]
    public void ApacheAvro_SerializeToStream_PrimitiveUnion()
    {
        using var output = new MemoryStream();
        _primitiveUnionSerializer.SerializeToStream(output, _primitiveUnionValue);
    }

    [Benchmark]
    public void ApacheAvro_SerializeToStream_NullableUnion()
    {
        using var output = new MemoryStream();
        _nullableUnionSerializer.SerializeToStream(output, _nullableUnionValue);
    }

    [Benchmark]
    public void ApacheAvro_SerializeToStream_ComplexRecord()
    {
        using var output = new MemoryStream();
        _complexRecordSerializer.SerializeToStream(output, _complexValue);
    }
}

file sealed class BenchmarkData
{
    public required string StringValue { get; init; }
    public required ClassWithPrimitives PrimitivesValue { get; init; }
    public required RecordWithComplexTypes ComplexValue { get; init; }
    public required Union<int, long> PrimitiveUnionValue { get; init; }
    public string? NullableStringUnionValue { get; init; }
    public required LogicalTypeRecord LogicalTypeValue { get; init; }
    public required string CustomLogicalTypeValue { get; init; }
}

file static class BenchmarkDataFactory
{
    public static BenchmarkData Create(BenchmarkPayloadSize size)
    {
        var payloadLength = size switch
        {
            BenchmarkPayloadSize.Tiny => 32,
            BenchmarkPayloadSize.Medium => 2_048,
            _ => 65_536
        };

        var itemsCount = size switch
        {
            BenchmarkPayloadSize.Tiny => 4,
            BenchmarkPayloadSize.Medium => 64,
            _ => 1_024
        };

        var stringPayload = new string('a', payloadLength);
        var bytePayload = Enumerable.Repeat((byte)8, payloadLength).ToArray();

        var innerRecords = Enumerable.Range(1, itemsCount)
            .Select(i => new InnerRecord
            {
                Field1 = $"inner-{i}-{stringPayload[..Math.Min(stringPayload.Length, 16)]}",
                Field2 = i
            })
            .ToArray();

        var doubles = Enumerable.Range(0, itemsCount).Select(i => (double)i + 0.123d).ToList();
        var map = innerRecords.ToDictionary(
            keySelector: r => $"{r.Field1}-{r.Field2}",
            elementSelector: r => new InnerRecord { Field1 = r.Field1, Field2 = r.Field2 });

        return new BenchmarkData
        {
            StringValue = stringPayload,
            PrimitivesValue = new ClassWithPrimitives
            {
                BoolField = true,
                BytesField = bytePayload,
                DoubleField = 1234.5678d,
                FloatField = 123.45f,
                IntegerField = payloadLength,
                LongField = payloadLength * 1_000_000L,
                StringField = stringPayload
            },
            ComplexValue = new RecordWithComplexTypes
            {
                InnerRecord = new InnerRecord
                {
                    Field1 = stringPayload[..Math.Min(stringPayload.Length, 32)],
                    Field2 = payloadLength
                },
                InnerRecords = innerRecords,
                Doubles = doubles,
                NullableFloat = 12.6f,
                MapField = map
            },
            PrimitiveUnionValue = size is BenchmarkPayloadSize.Tiny ? (Union<int, long>)42 : (Union<int, long>)(payloadLength * 1_000_000L),
            NullableStringUnionValue = size is BenchmarkPayloadSize.Tiny ? null : stringPayload,
            LogicalTypeValue = new LogicalTypeRecord
            {
                DateField = new DateOnly(2026, 04, 12),
                TimeField = new TimeOnly(15, 43, 8),
                TimestampField = new DateTime(2026, 04, 12, 15, 43, 8, DateTimeKind.Utc),
                UuidField = Guid.Parse("c7926932-fd30-43af-a376-c32ba4a43a06")
            },
            CustomLogicalTypeValue = stringPayload
        };
    }
}
