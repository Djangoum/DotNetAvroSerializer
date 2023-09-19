using System;
using Avro;
using Avro.Util;

namespace DotNetAvroSerializer.Generators.Helpers;

public class CustomLogicalType : LogicalType
{
    public CustomLogicalType(string name) : base(name)
    {
    }

    public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema)
    {
        throw new NotImplementedException();
    }

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema)
    {
        throw new NotImplementedException();
    }

    public override Type GetCSharpType(bool nullible)
    {
        throw new NotImplementedException();
    }

    public override bool IsInstanceOfLogicalType(object logicalValue)
    {
        throw new NotImplementedException();
    }
}