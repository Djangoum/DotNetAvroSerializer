namespace DotNetAvroSerializer.Write.Tests.Models
{
    public class UserPromotionViewRecordsDataEventApiV2
    {
        public IEnumerable<UserPromotionViewDataEventApiV2> Records { get; set; } = Enumerable.Empty<UserPromotionViewDataEventApiV2>();
    }

    public class UserPromotionViewDataEventApiV2
    {
        public Guid EventId { get; set; }
        public DateTime EventTimestamp { get; set; }
        public DateTime ApiTimestamp { get; set; }
        public Guid? UserPromotionId { get; set; }
        public required string ClientId { get; set; }
        public Guid PromotionId { get; set; }
        public required string Channel { get; set; }
        public bool IsActivable { get; set; }
        public required string ActionLocation { get; set; }
    }

    [AvroSchema(@"{
  ""type"": ""record"",
  ""name"": ""UserPromotionLoyaltyView"",
  ""namespace"": ""eu.scrm.dp.schemas"",
  ""fields"": [
    {
      ""name"": ""Records"",
      ""type"": {
        ""type"": ""array"",
        ""items"": {
          ""type"": ""record"",
          ""name"": ""UserPromotionLoyaltyViewEvent"",
          ""namespace"": ""eu.scrm.dp.schemas.UserPromotionLoyaltyView"",
          ""fields"": [
              {
                ""name"": ""EventId"",
                ""type"": {
                  ""logicalType"": ""uuid"",
                  ""type"": ""string""
                }
              },
              {
                ""name"": ""EventTimestamp"",
                ""type"": {
                  ""type"": ""long"",
                  ""logicalType"": ""timestamp-millis""
                }
              },
              {
                ""name"": ""ApiTimestamp"",
                ""type"": {
                  ""type"": ""long"",
                  ""logicalType"": ""timestamp-millis""
                }
              },
              {
                ""name"": ""UserPromotionId"",
                ""type"": [
                  ""null"",
                  {
                    ""type"": ""string"",
                    ""logicalType"": ""uuid""
                  }
                ],
                ""default"": null
              },
              {
                ""name"": ""ClientId"",
                ""type"": {
                  ""type"": ""string""
                }
              },
              {
                ""name"": ""PromotionId"",
                ""type"": {
                  ""logicalType"": ""uuid"",
                  ""type"": ""string""
                }
              },
              {
                ""name"": ""Channel"",
                ""type"": {
                  ""type"": ""string""
                }
              },
              {
                ""name"": ""IsActivable"",
                ""type"": ""boolean""
              },
              {
                ""name"": ""ActionLocation"",
                ""type"": {
                  ""type"": ""string""
                }
              }
          ]
        }
      }
    }
  ]
}")]
    public partial class UserPromotionLoyaltyViewSerializer : AvroSerializer<UserPromotionViewRecordsDataEventApiV2>
    {

    }


}
