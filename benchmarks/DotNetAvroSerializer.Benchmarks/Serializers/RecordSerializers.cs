using DotNetAvroSerializer.Benchmarks.Models;

namespace DotNetAvroSerializer.Benchmarks.Serializers
{
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
    public partial class ClassWithPrimitivesSerializer : AvroSerializer<ClassWithPrimitives>
    {

    }

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
}
