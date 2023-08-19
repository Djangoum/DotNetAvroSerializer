using ConsoleApp16.Serializers;
using FluentAssertions;
using SolTechnology.Avro;

//var serialization1 = new IntArraySerializer().Serialize();
var serialization2 = AvroConvert.SerializeHeadless(new int[] { 1, 2, 3, 4 }, typeof(int[]));

//Console.WriteLine(Convert.ToHexString(serialization1));
Console.WriteLine(Convert.ToHexString(serialization2));

//serialization1.Should().BeEquivalentTo(serialization2);