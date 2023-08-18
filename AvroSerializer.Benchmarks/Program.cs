using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ConsoleApp16.Serializers;
using PromotionEngine.Avro;

BenchmarkRunner.Run<AvroSerializersBenchmarks>();

[MemoryDiagnoser]
public class AvroSerializersBenchmarks
{
    const string sut = "weaofij34wf89340hjvse9iefv8umnfso89fcvyso89rfl3yvncl8s478fncvl9487ynslvm4oglnsv48gvhns59l78ghsvel47i8tghvwa39l8rawu3ñofghes5l9tsh9ln7ag39ybn3a9o7ryh4oln8vbsekhe9lrgbnseio85goa9w348urwi734yri7zgv8ose4h9o4uhiasl3hr9alw8en9";
    private UserPromotionAssignationDataEventApiV2 sut2 = new UserPromotionAssignationDataEventApiV2
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

    //[Benchmark]
    //public byte[] AvroSerializerStringSerialization()
    //{
    //    return new StringSerializer2().Serialize(sut);
    //}

    //[Benchmark]
    //public byte[] AvroConvertStringSerialization()
    //{
    //    return AvroConvert.SerializeHeadless(sut, @"{ ""type"": ""string"" }");
    //}

    [Benchmark(Baseline = true)]
    public byte[] AvroSerializerLoyaltyUserPromotionAssignationSerialization()
    {
        return new UserPromotionAssigantionDataeventApiV2Serializer().Serialize(sut2);
    }

    [Benchmark]
    public byte[] AvroConvertLoyaltyUserPromotionAssignationSerialization()
    {
        return AvroConvert.SerializeHeadless(sut2, @"{
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
            ""type"": ""string"",
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
    }");
    }
}