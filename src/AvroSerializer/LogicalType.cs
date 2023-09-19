using System;

namespace DotNetAvroSerializer;

public abstract class LogicalType<TLogicalType>
{
    public abstract bool CanSerialize(object? value);
    public virtual bool Validate(TLogicalType logicalTypeValue) => true;
    public abstract object ConvertToBaseType(TLogicalType logicalTypeValue);
}

public class LogicalTypeAttribute : Attribute
{
    public string Name { get; }

    public LogicalTypeAttribute(string name)
    {
        Name = name;
    }
}