using ConsoleApp16.Serializers;
using FluentAssertions;
using SolTechnology.Avro;

var serialization1 = new EnumSerializer().Serialize(TestEnum.Value3);
var serialization2 = AvroConvert.SerializeHeadless(TestEnum.Value3, typeof(TestEnum));

Console.WriteLine(Convert.ToHexString(serialization1));
Console.WriteLine(Convert.ToHexString(serialization2));

serialization1.Should().BeEquivalentTo(serialization2);