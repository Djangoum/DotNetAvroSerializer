namespace DotNetAvroSerializer.Write.Tests.Models
{
    public class ClassWithPrimitives
    {
        public int IntegerField { get; set; }
        public long LongField { get; set; }
        public required string StringField { get; set; }
        public float FloatField { get; set; }
        public double DoubleField { get; set; }
        public bool BoolField { get; set; }
        public required byte[] BytesField { get; set; }
    }
}
