namespace AvroSerializer.Write.Tests.Models
{
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
}
