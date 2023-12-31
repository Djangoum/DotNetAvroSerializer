[![publish](https://github.com/Djangoum/DotNetAvroSerializer/actions/workflows/publish.yml/badge.svg?branch=main)](https://github.com/Djangoum/DotNetAvroSerializer/actions/workflows/publish.yml)
# Dotnet Avro Serializer

Dotnet AvroSerializer

Dotnet AvroSerializer is a cutting-edge Avro serialization library for .NET, driven by the power of source generators. Please note that this library is currently under active development and is not yet published.

> [!WARNING]  
> As of now, the library exclusively offers serialization capabilities and does not support deserialization. We are actively working on expanding its functionality to cover deserialization as well.

## How Dotnet Avro Serializer Works

Install both serializer and generator package.

```ps
dotnet add package DotnetAvroSerializer
dotnet add package DotnetAvroSerializer.Generators
```

Dotnet Avro Serializer is a powerful tool that automates the creation of serializers from Avro schemas and C# types. It streamlines the process of converting your C# objects into Avro binary serialized data effortlessly.

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
public partial class IntSerializer
{
    public override byte[] Serialize(int source)
    {
        var outputStream = new MemoryStream();
        SerializeToStream(outputStream, source);
        return outputStream.ToArray();
    }

    public override void SerializeToStream(Stream outputStream, int source)
    {
        IntSchema.Write(outputStream, source);
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
    public override byte[] Serialize(global::Serializers.RecordWithComplexTypes source)
    {
        var outputStream = new MemoryStream();
        SerializeToStream(outputStream, source);
        return outputStream.ToArray();
    }

    public override void SerializeToStream(Stream outputStream, global::Serializers.RecordWithComplexTypes source)
    {
        StringSchema.Write(outputStream, source.InnerRecord.Field1);
        IntSchema.Write(outputStream, source.InnerRecord.Field2);
        if (source.InnerRecords.Count() > 0)
            LongSchema.Write(outputStream, (long)source.InnerRecords.Count());
        foreach (var itemsourceInnerRecords in source.InnerRecords)
        {
            StringSchema.Write(outputStream, itemsourceInnerRecords.Field1);
            IntSchema.Write(outputStream, itemsourceInnerRecords.Field2);
        }

        LongSchema.Write(outputStream, 0L);
        if (source.Doubles.Count() > 0)
            LongSchema.Write(outputStream, (long)source.Doubles.Count());
        foreach (var itemsourceDoubles in source.Doubles)
        {
            DoubleSchema.Write(outputStream, itemsourceDoubles);
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
        foreach (var itemsourceMapField in source.MapField)
        {
            StringSchema.Write(outputStream, itemsourceMapField.Key);
            StringSchema.Write(outputStream, itemsourceMapField.Value.Field1);
            IntSchema.Write(outputStream, itemsourceMapField.Value.Field2);
        }

        LongSchema.Write(outputStream, 0L);
    }
}
```

## Overriding field names in records

In some cases, field records may not precisely correspond to class properties. DotNetAvroSerializer consistently employs a case-insensitive approach when searching for field names. However, there are instances where Avro schemas may incorporate names that deviate from typical C# syntax. The following example illustrates how you can customize and override a field name to address such scenarios.

```csharp
[AvroSchema(@"{
    ""name"": ""InnerRecord"",
    ""type"": ""record"",
    ""fields"": [
        {
            ""name"": ""field_name_1"",
            ""type"": ""string""
        },
        {
            ""name"": ""field_name_2"",
            ""type"": ""int""
        }
    ]
}")]
public partial class RecordWithComplexTypesSerializer : AvroSerializer<InnerRecord>
{

}

public class InnerRecord
{
    [AvroField("field_name_1")]
    public required string Field1 { get; set; }
    [AvroField("field_name_2")]
    public int Field2 { get; set; }
}
```

One of the standout features of the Dotnet Avro Serializer is its significant performance enhancement compared to other available libraries. This improvement is primarily attributed to its utilization of source generators, eliminating the need for any runtime reflection. Instead, it efficiently carries out serialization directly to an output stream or byte array.

## Unions

In the DotNetAvroSerializer library, unions are typically represented using the **Union** struct. Let's explore this concept with an example: 

```csharp
[AvroSchema(@"{ ""type"": [""int"", ""long""] }")]
public partial class IntegerLongSerializer : AvroSerializer<Union<int, long>>
{

}
```

The **Union<>** struct is designed to simplify the handling of unions by providing implicit and explicit operators for its generic parameter types. This makes it easy to assign values to the union, as demonstrated in the following code: 

```csharp
Union<int, long, Null> union = 2L; // assigning long value to union.
```

Once assigned, a union becomes immutable and cannot be modified. To assign a new value, you need to create a new instance of the union.

For unions consisting of an integer and a long, you must use the **Union<int, long>** struct. It's important to note that, as of now, DotNetAvroSerializer only supports unions with up to four elements.

The example provided is the simplest form of a union. However, unions can become more complex, such as unions of integers, longs, and null values. To represent null values within unions, DotNetAvroSerializer utilizes the **Null** struct, like so:

```csharp
[AvroSchema(@"{ ""type"": [ ""int"", ""long"", ""null"" ] }")]
public partial class IntLongNullSerializer : AvroSerializer<Union<int, long, Null>>
{

}
```

In special cases where unions involve only any type and null, you can use nullable types to represent these unions, eliminating the need for the **Union<>** struct. For example:

```csharp
[AvroSchema(@"{ ""type"": [ ""null"", ""boolean""] }")]
public partial class NullableStringSerializer : AvroSerializer<bool?>
{

}
```

This approach simplifies the code when dealing with unions containing only any type and null values.

## Custom logical types

Most .NET Avro libraries do not support custom logical types. Whenever these libraries detect unrecognized logical types, they throw an exception. However, this conflicts with what Avro specifies in its [docs](https://avro.apache.org/docs/1.11.1/specification/#logical-types). According to Avro's specification, these logical types should be serialized using the base types instead. In line with this specification, DotNetAvroSerializer allows the use of custom logical types and permits developers to specify their behavior during serialization.

How to define these logical types ? 

```csharp 
[AvroSchema(@"{
  ""type"": [
    ""null"",
    {
      ""logicalType"": ""regex-string"",
      ""type"": ""string"",
      ""regex"": "".+""
    }
  ]
}", new []{ typeof(RegexStringLogicalType) })]
public partial class NullRegexStringSerializer : AvroSerializer<string?>
{
}

[LogicalTypeName("regex-string")]
public static class RegexStringLogicalType
{
    public static bool CanSerialize(object? value, [LogicalTypePropertyName("regex")]string regexPattern) => value is string;

    public static string ConvertToBaseSchemaType(string logicalTypeValue, [LogicalTypePropertyName("regex")]string regexPattern)
    {
        var regex = new Regex(regexPattern, RegexOptions.Compiled);

        if (!regex.Matches(logicalTypeValue).Any())
        {
            throw new AvroSerializationException("Regex validation failed");
        }
          
        return logicalTypeValue;
    }
}
```

**AvroSchema** attribute allows a second parameters where to specify a **Type[]** with types that are logical types. 

### Rules for custom logical types 

- Must be static classes
- Must have **LogicalTypeName** attribute defined 
- Must have **CanSerialize** method accepting **object?** and returning **bool** can have extra parameters that match schema properties
- Must have **ConverToBaseSchemaType** accepting logical type and returning base schema type C# representation
- Return type of **ConvertToBaseSchemaType** method must match schema base type C# representation
- Extra parameters for **CanSerialize** and **ConvertToBaseSchemaType** methods are meant to receive logical type schema properties.
- Extra parameters are mapped using parameter name or using the **LogicalTypePropertyName** attribute
- All schema properties must be mapped or an error is thrown

## Rules and limitations :

There are some rules that Dotnet Avro Serializer source generators expect in order to generate serialization code. 

- Serialization code will be generated only once per serializer class (multiple **AvroSchema** attributes will be ignored and only the first one will be taken) 
- Avro arrays must be C# arrays or implement IEnumerable and have a generic argument. List\<T>, IEnumerable\<T>, ICollection\<T> -> all of these would work.
- Maps must implement IDictionary
- Enums must be C# enums
- Fixed must be byte[] 
- Record fields must match class **properties** names (case insensitive) or decorate properties with **AvroField** attribute
- Class names and namespaces can differ from schema specification
- Nullable types are not suitable to serialize primitives or any type unless they are marked as **unions** between "null" and the C# nullable type we want to serialize in the avro schema.

## Benchmarks 

Find some benchmarks results here. 

```
|                               Method |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|------------------------------------- |---------:|--------:|--------:|-------:|----------:|
|    AvroSerializerStringSerialization | 144.5 ns | 1.75 ns | 1.37 ns | 0.1004 |     840 B |
|   AvroSerializerRecordWithPrimitives | 158.7 ns | 2.91 ns | 2.72 ns | 0.0753 |     632 B |
| AvroSerializerRecordWithComplexTypes | 723.1 ns | 7.88 ns | 6.58 ns | 0.1869 |    1568 B |
```

## Important note 
As of now, the library exclusively offers serialization capabilities and does not support deserialization. We are actively working on expanding its functionality to cover deserialization as well.

## Performance Improvements

- [ ] Reduce boxing and casts
- [ ] Allow logicalTypes with complex base types
- [ ] Decimal logical type
