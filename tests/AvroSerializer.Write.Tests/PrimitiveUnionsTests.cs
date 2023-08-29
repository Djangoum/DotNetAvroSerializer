//namespace DotNetAvroSerializer.Write.Tests
//{
//    public class PrimitiveUnionsTests
//    {

//    }

//    [AvroSchema(@"{ ""type"": [""int"", ""long""] }")]
//    public partial class IntegerLongSerializer : AvroSerializer<Union<int, long>>
//    {

//    }

//    [AvroSchema(@"{ ""type"": [""long"", ""string""] }")]
//    public partial class LongStringSerializer : AvroSerializer<Union<long, string>>
//    {

//    }

//    [AvroSchema(@"{ ""type"": [ ""boolean"", ""double""] }")]
//    public partial class BoolDoubleSerializer : AvroSerializer<Union<bool, double>>
//    {

//    }

//    [AvroSchema(@"{ ""type"": [ ""bytes"", ""int"" ] }")]
//    public partial class BytesIntSerializer : AvroSerializer<Union<byte[], int>>
//    {

//    }

//    [AvroSchema(@"{ ""type"": [ ""string"", ""float"" ] }")]
//    public partial class StringFloatSerializer : AvroSerializer<Union<string, float>>
//    {

//    }

//    [AvroSchema(@"{ ""type"": [ ""bytes"", ""string"" ] }")]
//    public partial class BytesStringSerializer : AvroSerializer<Union<byte[], string>>
//    {

//    }
//}
