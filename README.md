# Dotnet Avro Serializer

Dotnet AvroSerializer is a source generator based avro serialization library for .NET. This Library is **Under construction**. This is not yet published and is also an experimentation exercice for the author (me :D) to learn about avro and source generators.

The library currently only supports serialization (not deserialization)

## Examples 

Dotnet Avro Serializer produces serializers from an avro schema and a C# type as serializable data.

Whenever avro serializer finds a **public partial class** inheriting **AvroSerializer<>** and have an **AvroSchema** attribute with a valid and corresponding avro schema it will generate serialization code. 

Currently Dotnet Avro Serializer creates a couple of methods: 

- **Serialize** : Takes an instance of the generic parameter provided to **AvroSerializer<>** and returns an array of bytes containing avro binary serialized data.
- **SerializeToStream** : Takes a **Stream** and an instance of the generic parameter provided to **AvroSerializer<>**. Writes binary serialized data to the stream. 

Simplest example of avro serializer would be serializing a primitive type like an int
```csharp
[AvroSchema(@"{ ""type"": ""int"" }")]
public partial class IntSerializer : AvroSerializer<int>
{

}
```
Generated code looks like this
```csharp
using AvroSerializer.Primitives;
using AvroSerializer.LogicalTypes;
using AvroSerializer.Exceptions;
using AvroSerializer.ComplexTypes;
using System.Linq;

namespace AvroSerializer.Write.Tests
{
    public partial class IntSerializer
    {
        public byte[] Serialize(int source)
        {
            var outputStream = new MemoryStream();
            SerializeToStream(outputStream, source);
            return outputStream.ToArray();
        }

        public void SerializeToStream(Stream outputStream, int source)
        {
            IntSchema.Write(outputStream, source);
        }
    }
}
```

A much complex example would be to serialize a class with multiple complex types nested like the following. Here you can find, the schema, the serializer and the C# types to serialize. 

```csharp
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
public partial class RecordWithComplexTypesSerializer : AvroSerializer<RecordWithComplexTypes>
{

}

public class RecordWithComplexTypes
{
    public required InnerRecord InnerRecord { get; set; }
    public required InnerRecord[] InnerRecords { get; set; }
    public required double[] Doubles { get; set; }
    public float? NullableFloat { get; set; }
    public required IDictionary<string, InnerRecord> MapField { get; set; }
}

public class InnerRecord
{
    public required string Field1 { get; set; }
    public int Field2 { get; set; }
}
```
Generated code looks like this
```csharp
public partial class RecordWithComplexTypesSerializer
{
    public byte[] Serialize(global::ConsoleApp9.Serializers.RecordWithComplexTypes source)
    {
        var outputStream = new MemoryStream();
        SerializeToStream(outputStream, source);
        return outputStream.ToArray();
    }

    public void SerializeToStream(Stream outputStream, global::ConsoleApp9.Serializers.RecordWithComplexTypes source)
    {
        StringSchema.Write(outputStream, source.InnerRecord.Field1);
        IntSchema.Write(outputStream, source.InnerRecord.Field2);
        if (source.InnerRecords.Count() > 0)
            LongSchema.Write(outputStream, (long)source.InnerRecords.Count());
        foreach (var item in source.InnerRecords)
        {
            StringSchema.Write(outputStream, item.Field1);
            IntSchema.Write(outputStream, item.Field2);
        }

        LongSchema.Write(outputStream, 0L);
        if (source.Doubles.Count() > 0)
            LongSchema.Write(outputStream, (long)source.Doubles.Count());
        foreach (var item in source.Doubles)
        {
            DoubleSchema.Write(outputStream, item);
        }

        LongSchema.Write(outputStream, 0L);
        if (NullSchema.CanSerialize(source.NullableFloat))
        {
            IntSchema.Write(outputStream, 0);
            NullSchema.Write(outputStream, source.NullableFloat);
        }
        else if (FloatSchema.CanSerialize(source.NullableFloat))
        {
            IntSchema.Write(outputStream, 1);
            FloatSchema.Write(outputStream, source.NullableFloat);
        }

        if (source.MapField.Count() > 0)
            LongSchema.Write(outputStream, source.MapField.Count());
        foreach (var item in source.MapField)
        {
            StringSchema.Write(outputStream, item.Key);
            StringSchema.Write(outputStream, item.Value.Field1);
            IntSchema.Write(outputStream, item.Value.Field2);
        }

        LongSchema.Write(outputStream, 0L);
    }
}
```

Key feature of Dotnet Avro Serializer is mainly performance improvement over other libraries out there. As it's using source generators does not have any runtime reflection and directly serializes. 

## Rules and limitations :

There are some rules that Dotnet Avro Serializer source generators expect in order to generate serialization code. 

- Serialization code will be generated only once per serializer class (multiple **AvroSchema** attributes will be ignored and only the first one will be taken) 
- Avro arrays must be C# arrays (not working with IEnumerables's for example)
- Maps must implement IDictionary
- Enums must be C# enums
- Fixed must be byte[] 
- Record fields must match class **properties** names
- Nullable types are not suitable to serialize primitives or any type unless they are marked as **unions** between "null" and the C# nullable type we want to serialize in the avro schema.


I'll add a list of missing features to complete a first usable (not production ready) version of the write part. 

- [x] Decimal Logical Type
- [x] Unions
- [x] Maps
- [x] Fixed
- [x] Enum
- [ ] Time (micro) Logical Type
- [ ] Timestamp (micro) Logical Type
- [ ] Local timestamp (micro) Logical Type
- [ ] Custom logical types (?)

Known issues : 

- [x] Do not support for more than one usage of the same enum per schema

Everything is pending for deserialization :) 