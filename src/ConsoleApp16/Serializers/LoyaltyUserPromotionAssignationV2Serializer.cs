//using AvroSerializer;

//namespace ConsoleApp16.Serializers
//{
//    [AvroSchema(@"{
//  ""type"": ""record"",
//  ""name"": ""UserPromotionLoyaltyAssignation"",
//  ""namespace"": ""eu.scrm.dp.schemas"",
//  ""fields"": [
//    {
//      ""name"": ""EventId"",
//      ""type"": {
//        ""logicalType"": ""uuid"",
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""EventTimestamp"",
//      ""type"": {
//        ""type"": ""long"",
//        ""logicalType"": ""timestamp-millis""
//      }
//    },
//    {
//      ""name"": ""ApiTimestamp"",
//      ""type"": {
//        ""type"": ""long"",
//        ""logicalType"": ""timestamp-millis""
//      }
//    },
//    {
//      ""name"": ""UserPromotionId"",
//      ""type"": {
//        ""logicalType"": ""uuid"",
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""ClientId"",
//      ""type"": {
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""PromotionId"",
//      ""type"": {
//        ""logicalType"": ""uuid"",
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""InternalPromotionId"",
//      ""type"": {
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""AssignationMethod"",
//      ""type"": {
//        ""type"": ""string""
//      }
//    },
//    {
//      ""name"": ""StartDisplayDateTime"",
//      ""type"": {
//        ""type"": ""long"",
//        ""logicalType"": ""timestamp-millis""
//      }
//    },
//    {
//      ""name"": ""StartValidityDateTime"",
//      ""type"": {
//        ""type"": ""long"",
//        ""logicalType"": ""timestamp-millis""
//      }
//    },
//    {
//      ""name"": ""EndValidityDateTime"",
//      ""type"": {
//        ""type"": ""long"",
//        ""logicalType"": ""timestamp-millis""
//      }
//    },
//    {
//      ""name"": ""External"",
//      ""type"": {
//        ""type"": ""record"",
//        ""name"": ""External"",
//        ""namespace"": ""eu.scrm.dp.schemas.UserPromotionLoyaltyAssignation"",
//        ""fields"": [
//          {
//            ""name"": ""AppName"",
//            ""type"": {
//              ""type"": ""string""
//            }
//          }
//        ]
//      }
//    }
//  ]
//}")]
//    public partial class UserPromotionAssignationDataEventApiV2Serializer : AvroSerializer<UserPromotionAssignationDataEventApiV2>
//    {

//    }

//    //[AvroSchema("{\"type\": \"boolean\"}")]
//    //public partial class BooleanSerializer : AvroSerializer<bool>
//    //{
//    //}

//    //[AvroSchema("{\"type\": \"string\"}")]
//    //public partial class StringSerializer : AvroSerializer<string>
//    //{
//    //}

//    //[AvroSchema("{\"type\": \"int\"}")]
//    //public partial class IntSerializer : AvroSerializer<int>
//    //{
//    //}

//    //[AvroSchema("{\"type\": \"long\"}")]
//    //public partial class LongSerializer : AvroSerializer<long>
//    //{
//    //}

//    //[AvroSchema("{\"type\": \"bytes\"}")]
//    //public partial class BytesSerializer : AvroSerializer<byte[]>
//    //{
//    //}
//}
