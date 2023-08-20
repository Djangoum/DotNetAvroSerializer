using DotNetAvroSerializer.Benchmarks.Serializers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DotNetAvroSerializer.Benchmarks.Models;

BenchmarkRunner.Run<AvroSerializersBenchmarks>();

[MemoryDiagnoser]
public class AvroSerializersBenchmarks
{
    const string sut = "weaofij34wf89340hjvse9iefv8umnfso89fcvyso89rfl3yvncl8s478fncvl9487ynslvm4oglnsv48gvhns59l78ghsvel47i8tghvwa39l8rawu3ñofghes5l9tsh9ln7ag39ybn3a9o7ryh4oln8vbsekhe9lrgbnseio85goa9w348urwi734yri7zgv8ose4h9o4uhiasl3hr9alw8en9";

    [Benchmark]
    public byte[] AvroSerializerStringSerialization()
    {
        return new StringSerializer().Serialize(sut);
    }

    [Benchmark]
    public byte[] AvroSerializerRecordWithPrimitives()
    {
        return new ClassWithPrimitivesSerializer().Serialize(new ClassWithPrimitives
        {
            BoolField = true,
            BytesField = new byte[] { 1, 2, 3, 4 },
            DoubleField = 12.5d,
            FloatField = 15.6f,
            IntegerField = 42,
            LongField = 1234346567567563454,
            StringField = "test text"
        });
    }


    [Benchmark]
    public byte[] AvroSerializerRecordWithComplexTypes()
    {
        return new RecordWithComplexTypesSerializer().Serialize(new RecordWithComplexTypes
        {
            InnerRecord = new InnerRecord
            {
                Field1 = "teststring",
                Field2 = 124
            },
            Doubles = new List<double> { 1.2d, 3.4d, 12.6d },
            InnerRecords = new[]
                {
                    new InnerRecord
                    {
                        Field1 = "teststring",
                        Field2 = 124
                    },
                    new InnerRecord
                    {
                        Field1 = "teststring",
                        Field2 = 124
                    }
                },
            NullableFloat = 12.6f,
            MapField = new Dictionary<string, InnerRecord>
                {
                    {
                        "key1",
                        new InnerRecord
                        {
                            Field1 = "teststring",
                            Field2 = 124
                        }
                    },
                    {
                        "key2",
                        new InnerRecord
                        {
                            Field1 = "teststring",
                            Field2 = 124
                        }
                    }
                }
        });
    }
}