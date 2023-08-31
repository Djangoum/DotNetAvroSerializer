namespace DotNetAvroSerializer.Write.Tests.Models
{
    public class UnionSideOne
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class UnionSideTwo
    {
        public int Identifier { get; set; }
        public required string SecondName { get; set; }
    }
}
