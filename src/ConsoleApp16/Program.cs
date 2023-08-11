
using FluentAssertions;
using SolTechnology.Avro;

Console.WriteLine(Convert.ToHexString(AvroConvert.SerializeHeadless(1.2d, typeof(double))));

Console.WriteLine(Convert.ToHexString(AvroConvert.SerializeHeadless(124.341d, typeof(double))));