using Avro;
using Avro.Generic;
using Avro.IO;
using DotNetAvroSerializer.Benchmarks.Models;

namespace DotNetAvroSerializer.Benchmarks.Serializers;

public sealed class ApacheAvroStringSerializer
{
    private static readonly Schema ParsedSchema = Avro.Schema.Parse(@"{ ""type"": ""string"" }");
    private static readonly GenericDatumWriter<string> Writer = new(ParsedSchema);

    public void SerializeToStream(Stream outputStream, string value)
    {
        var encoder = new BinaryEncoder(outputStream);
        Writer.Write(value, encoder);
    }
}

public sealed class ApacheAvroPrimitiveUnionSerializer
{
    private static readonly Schema ParsedSchema = Avro.Schema.Parse(@"{ ""type"": [""int"", ""long""] }");
    private static readonly GenericDatumWriter<object> Writer = new(ParsedSchema);

    public void SerializeToStream(Stream outputStream, object value)
    {
        var encoder = new BinaryEncoder(outputStream);
        Writer.Write(value, encoder);
    }
}

public sealed class ApacheAvroNullableStringUnionSerializer
{
    private static readonly Schema ParsedSchema = Avro.Schema.Parse(@"{ ""type"": [""null"", ""string""] }");
    private static readonly GenericDatumWriter<object?> Writer = new(ParsedSchema);

    public void SerializeToStream(Stream outputStream, object? value)
    {
        var encoder = new BinaryEncoder(outputStream);
        Writer.Write(value, encoder);
    }
}

public sealed class ApacheAvroComplexRecordSerializer
{
    private static readonly RecordSchema ParsedSchema = (RecordSchema)Avro.Schema.Parse(@"{
        ""type"": ""record"",
        ""name"" : ""recordWithComplexTypes"",
        ""fields"" :[
            {
                ""name"": ""InnerRecord"",
                ""type"": {
                    ""name"": ""InnerRecord"",
                    ""type"": ""record"",
                    ""fields"": [
                        { ""name"": ""Field1"", ""type"": ""string"" },
                        { ""name"": ""Field2"", ""type"": ""int"" }
                    ]
                } 
            },
            {
                ""name"": ""InnerRecords"",
                ""type"": { ""type"": ""array"", ""items"": ""InnerRecord"" }
            },
            {
                ""name"": ""Doubles"",
                ""type"": { ""type"": ""array"", ""items"": ""double"" }
            },
            {
                ""name"": ""NullableFloat"",
                ""type"": [ ""null"", ""float"" ]
            },
            {
                ""name"": ""MapField"",
                ""type"": { ""type"": ""map"", ""values"": ""InnerRecord"" }
            }
        ]
    }");

    private static readonly GenericDatumWriter<GenericRecord> Writer = new(ParsedSchema);

    public void SerializeToStream(Stream outputStream, RecordWithComplexTypes value)
    {
        var record = new GenericRecord(ParsedSchema);
        record.Add("InnerRecord", ToInnerRecord(value.InnerRecord));
        record.Add("InnerRecords", value.InnerRecords.Select(ToInnerRecord).ToArray());
        record.Add("Doubles", value.Doubles.ToArray());
        record.Add("NullableFloat", value.NullableFloat.HasValue ? value.NullableFloat.Value : null);
        record.Add("MapField", value.MapField.ToDictionary(k => k.Key, v => (object)ToInnerRecord(v.Value)));

        var encoder = new BinaryEncoder(outputStream);
        Writer.Write(record, encoder);
    }

    private static GenericRecord ToInnerRecord(InnerRecord source)
    {
        var innerRecordSchema = (RecordSchema)ParsedSchema.GetField("InnerRecord").Schema;
        var record = new GenericRecord(innerRecordSchema);
        record.Add("Field1", source.Field1);
        record.Add("Field2", source.Field2);
        return record;
    }
}
