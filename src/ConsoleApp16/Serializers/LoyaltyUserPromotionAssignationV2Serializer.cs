﻿using AvroSerializer;

namespace ConsoleApp16.Serializers
{

    //[AvroSchema(@"{
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
    //        ""type"": ""string"",
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
    //      ""name"": ""PersonalDiscount"",
    //      ""type"": [
    //        ""null"",
    //        ""double""
    //      ],
    //      ""default"": null
    //    },
    //    {
    //      ""name"": ""External"",
    //      ""type"": {
    //        ""type"": ""record"",
    //        ""name"": ""External"",
    //        ""namespace"": ""eu.scrm.dp.schemas.UserPromotionLoyaltyAssignation"",
    //        ""fields"": [
    //          {
    //            ""name"": ""CampaignId"",
    //            ""type"": [
    //              ""null"",
    //              ""string""
    //            ],
    //            ""default"": null
    //          },
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
    //public partial class UserPromotionAssigantionDataeventApiV2Serializer : AvroSerializer<UserPromotionAssignationDataEventApiV2>
    //{

    //}

    //[AvroSchema(@"{ ""type"": ""string"" }")]
    //public partial class StringSerializer2 : AvroSerializer<string>
    //{

    //}

    [AvroSchema(@"{ ""type"": ""array"", ""items"": ""int"" }")]
    public partial class IntArraySerializer : AvroSerializer<string>
    {

    }
}
