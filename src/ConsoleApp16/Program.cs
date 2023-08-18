using AvroSerializer;
using ConsoleApp16.Serializers;
using FluentAssertions;
using PromotionEngine.Avro;

const string schema = @"{
  ""type"": ""record"",
  ""name"": ""UserPromotionLoyaltyAssignation"",
  ""namespace"": ""eu.scrm.dp.schemas"",
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
      ""type"": {
        ""logicalType"": ""uuid"",
        ""type"": ""string""
      }
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
      ""name"": ""InternalPromotionId"",
      ""type"": {
        ""type"": ""string""
      }
    },
    {
      ""name"": ""AssignationMethod"",
      ""type"": {
        ""type"": ""string""
      }
    },
    {
      ""name"": ""StartDisplayDateTime"",
      ""type"": {
        ""type"": ""long"",
        ""logicalType"": ""timestamp-millis""
      }
    },
    {
      ""name"": ""StartValidityDateTime"",
      ""type"": {
        ""type"": ""long"",
        ""logicalType"": ""timestamp-millis""
      }
    },
    {
      ""name"": ""EndValidityDateTime"",
      ""type"": {
        ""type"": ""long"",
        ""logicalType"": ""timestamp-millis""
      }
    },
    {
      ""name"": ""PersonalDiscount"",
      ""type"": [
        ""null"",
        ""double""
      ],
      ""default"": null
    },
    {
      ""name"": ""External"",
      ""type"": {
        ""type"": ""record"",
        ""name"": ""External"",
        ""namespace"": ""eu.scrm.dp.schemas.UserPromotionLoyaltyAssignation"",
        ""fields"": [
          {
            ""name"": ""CampaignId"",
            ""type"": [
              ""null"",
              ""string""
            ],
            ""default"": null
          },
          {
            ""name"": ""AppName"",
            ""type"": {
              ""type"": ""string""
            }
          }
        ]
      }
    }
  ]
}";

var sut = new UserPromotionAssignationDataEventApiV2
{
    EventId = Guid.NewGuid(),
    EventTimestamp = DateTime.Now,
    ApiTimestamp = DateTime.Now,
    UserPromotionId = Guid.NewGuid(),
    ClientId = "2344534534534512",
    PromotionId = Guid.NewGuid(),
    InternalPromotionId = "DEAS1294923423",
    StartDisplayDateTime = DateTime.Now,
    StartValidityDateTime = DateTime.Now,
    EndValidityDateTime = DateTime.Now,
    PersonalDiscount = 12.5d,
    External = new UserPromotionDataEventApiExternalData
    {
        AppName = "PROMOTIONENGINE",
        CampaignId = "CAM"
    }
};

var serialization1 = new UserPromotionAssigantionDataeventApiV2Serializer().Serialize(sut);
var serialization2 = AvroConvert.SerializeHeadless(new int[] { 1, 2, 3, 4 }, typeof(int[]));

Console.WriteLine(Convert.ToHexString(serialization1));
Console.WriteLine(Convert.ToHexString(serialization2));

serialization1.Should().BeEquivalentTo(serialization2);
public class UserPromotionAssignationDataEventApiV2
{
    public Guid EventId { get; set; }
    public DateTime EventTimestamp { get; set; }
    public DateTime ApiTimestamp { get; set; }
    public Guid UserPromotionId { get; set; }
    public required string ClientId { get; set; }
    public Guid PromotionId { get; set; }
    public required string InternalPromotionId { get; set; }
    public string AssignationMethod => "Assignable"; // TODO: adapt behaviour when Standard and Segmented user promotions are implemented
    public DateTime StartDisplayDateTime { get; set; }
    public DateTime StartValidityDateTime { get; set; }
    public DateTime EndValidityDateTime { get; set; }
    public double? PersonalDiscount { get; set; }
    public required UserPromotionDataEventApiExternalData External { get; set; }
}

public class UserPromotionDataEventApiExternalData
{
    public required string CampaignId { get; set; }
    public required string AppName { get; set; }
}
