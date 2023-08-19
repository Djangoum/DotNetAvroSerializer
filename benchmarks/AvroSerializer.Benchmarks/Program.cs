using AvroSerializer.Benchmarks.Serializers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
}