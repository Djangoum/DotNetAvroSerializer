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

public abstract class BenchmarkDataContext
{
    [Params(BenchmarkPayloadSize.Tiny, BenchmarkPayloadSize.Medium, BenchmarkPayloadSize.Large)]
    public BenchmarkPayloadSize Size { get; set; }

    protected BenchmarkData Data { get; private set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        Data = BenchmarkDataFactory.Create(Size);
        OnSetup(Data);
    }

    protected virtual void OnSetup(BenchmarkData data)
    {
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class StringSerializationBenchmarks : BenchmarkDataContext
{
    private readonly StringSerializer _dotNetSerializer = new();
    private readonly ApacheAvroStringSerializer _apacheSerializer = new();

    [Benchmark(Baseline = true)]
    public void ApacheAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _apacheSerializer.SerializeToStream(output, Data.StringValue);
    }

    [Benchmark]
    public void DotNetAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _dotNetSerializer.SerializeToStream(output, Data.StringValue);
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class PrimitiveUnionSerializationBenchmarks : BenchmarkDataContext
{
    private readonly PrimitiveUnionSerializer _dotNetSerializer = new();
    private readonly ApacheAvroPrimitiveUnionSerializer _apacheSerializer = new();

    [Benchmark(Baseline = true)]
    public void ApacheAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _apacheSerializer.SerializeToStream(output, Data.PrimitiveUnionValue.GetUnionValue()!);
    }

    [Benchmark]
    public void DotNetAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _dotNetSerializer.SerializeToStream(output, Data.PrimitiveUnionValue);
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class NullableUnionSerializationBenchmarks : BenchmarkDataContext
{
    private readonly NullableStringUnionSerializer _dotNetSerializer = new();
    private readonly ApacheAvroNullableStringUnionSerializer _apacheSerializer = new();

    [Benchmark(Baseline = true)]
    public void ApacheAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _apacheSerializer.SerializeToStream(output, Data.NullableStringUnionValue);
    }

    [Benchmark]
    public void DotNetAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _dotNetSerializer.SerializeToStream(output, Data.NullableStringUnionValue);
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ComplexRecordSerializationBenchmarks : BenchmarkDataContext
{
    private readonly RecordWithComplexTypesSerializer _dotNetSerializer = new();
    private readonly ApacheAvroComplexRecordSerializer _apacheSerializer = new();

    [Benchmark(Baseline = true)]
    public void ApacheAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _apacheSerializer.SerializeToStream(output, Data.ComplexValue);
    }

    [Benchmark]
    public void DotNetAvro_SerializeToStream()
    {
        using var output = new MemoryStream();
        _dotNetSerializer.SerializeToStream(output, Data.ComplexValue);
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class DotNetAvroOnlyBenchmarks : BenchmarkDataContext
{
    private readonly StringSerializer _stringSerializer = new();
    private readonly ClassWithPrimitivesSerializer _primitivesSerializer = new();
    private readonly RecordWithComplexTypesSerializer _complexSerializer = new();
    private readonly LogicalTypeRecordSerializer _logicalTypeRecordSerializer = new();
    private readonly NullableRegexStringSerializer _customLogicalTypeSerializer = new();
    private readonly PooledByteBufferWriter _pooledWriter = new();

    [GlobalCleanup]
    public void Cleanup()
    {
        _pooledWriter.Dispose();
    }

    [Benchmark]
    public byte[] DotNetAvro_Serialize_String() => _stringSerializer.Serialize(Data.StringValue);

    [Benchmark]
    public void DotNetAvro_SerializeToPooledBuffer_String()
    {
        _pooledWriter.Reset();
        using var output = new BufferWriterStream(_pooledWriter);
        _stringSerializer.SerializeToStream(output, Data.StringValue);
    }

    [Benchmark]
    public byte[] DotNetAvro_Serialize_PrimitivesRecord() => _primitivesSerializer.Serialize(Data.PrimitivesValue);

    [Benchmark]
    public byte[] DotNetAvro_Serialize_ComplexRecord() => _complexSerializer.Serialize(Data.ComplexValue);

    [Benchmark]
    public void DotNetAvro_SerializeToPooledBuffer_ComplexRecord()
    {
        _pooledWriter.Reset();
        using var output = new BufferWriterStream(_pooledWriter);
        _complexSerializer.SerializeToStream(output, Data.ComplexValue);
    }

    [Benchmark]
    public byte[] DotNetAvro_Serialize_LogicalTypes() => _logicalTypeRecordSerializer.Serialize(Data.LogicalTypeValue);

    [Benchmark]
    public byte[] DotNetAvro_Serialize_CustomLogicalType() => _customLogicalTypeSerializer.Serialize(Data.CustomLogicalTypeValue);
}

public sealed class BenchmarkData
{
    public required string StringValue { get; init; }
    public required ClassWithPrimitives PrimitivesValue { get; init; }
    public required RecordWithComplexTypes ComplexValue { get; init; }
    public required Union<int, long> PrimitiveUnionValue { get; init; }
    public string? NullableStringUnionValue { get; init; }
    public required LogicalTypeRecord LogicalTypeValue { get; init; }
    public required string CustomLogicalTypeValue { get; init; }
}

public static class BenchmarkDataFactory
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
