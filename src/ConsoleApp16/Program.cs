using ConsoleApp16.Serializers;
using FluentAssertions;
using SolTechnology.Avro;

Console.WriteLine("Hello, World!");

var objectToSerialize = new UserPromotionAssignationDataEventApiV2
{
   EventId = Guid.NewGuid(),
   EventTimestamp = DateTime.UtcNow,
   ApiTimestamp = DateTime.UtcNow,
   UserPromotionId = Guid.NewGuid(),
   ClientId = "770032892348945",
   PromotionId = Guid.NewGuid(),
   InternalPromotionId = "DEAS0000234923",
   StartDisplayDateTime  = DateTime.UtcNow,
   StartValidityDateTime = DateTime.UtcNow,
   EndValidityDateTime = DateTime.UtcNow,
   External = new UserPromotionDataEventApiExternalData
   {
       AppName = "Personalized"
   }
};

var serializer = new UserPromotionAssignationDataEventApiV2Serializer();

var serialized = serializer.Serialize(objectToSerialize);

var schema = @"{
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
      ""name"": ""External"",
      ""type"": {
        ""type"": ""record"",
        ""name"": ""External"",
        ""namespace"": ""eu.scrm.dp.schemas.UserPromotionLoyaltyAssignation"",
        ""fields"": [
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

var resultSerialized = AvroConvert.SerializeHeadless(objectToSerialize, schema);

Console.WriteLine($"MyLibrary: {Convert.ToHexString(serialized)} - AvroConvert: {Convert.ToHexString(resultSerialized)}");

Convert.ToHexString(serialized).Should().BeEquivalentTo(Convert.ToHexString(resultSerialized));

public class UserPromotionAssignationDataEventApiV2
{
    public Guid EventId { get; set; }
    public DateTime EventTimestamp { get; set; }
    public DateTime ApiTimestamp { get; set; }
    public Guid UserPromotionId { get; set; }
    public string ClientId { get; set; }
    public Guid PromotionId { get; set; }
    public string InternalPromotionId { get; set; }
    public string AssignationMethod => "Assignable"; // TODO: adapt behaviour when Standard and Segmented user promotions are implemented
    public DateTime StartDisplayDateTime { get; set; }
    public DateTime StartValidityDateTime { get; set; }
    public DateTime EndValidityDateTime { get; set; }
    public double? PersonalDiscount { get; set; }
    public UserPromotionDataEventApiExternalData External { get; set; }
}

public class EventApiBase
{
    public Guid EventId { get; set; }
    public DateTime EventTimestamp { get; set; }
    public DateTime ApiTimestamp { get; set; }
}


public class MyClass
{
    public int Number { get; set; }
    public string Name { get; set; }
} 

public class UserPromotionDataEventApiExternalData
{
    public string CampaignId { get; set; }
    public string AppName { get; set; }
}
