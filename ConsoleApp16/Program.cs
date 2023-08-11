using ConsoleApp16.Serializers;

Console.WriteLine("Hello, World!");

var serializer = new BooleanSerializer();
serializer.Serialize(true);

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

public class UserPromotionDataEventApiExternalData
{
    public string CampaignId { get; set; }
    public string AppName { get; set; }
}
