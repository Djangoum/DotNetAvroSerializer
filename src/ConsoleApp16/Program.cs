
using FluentAssertions;
using SolTechnology.Avro;

Console.WriteLine(Convert.ToHexString(AvroConvert.SerializeHeadless(new TimeOnly(20,01,02), typeof(TimeOnly))));
