using System;
 
namespace DotNetAvroSerializer;

[AttributeUsage(AttributeTargets.Class)]  
public class LogicalTypeNameAttribute : Attribute
{
    public string Name { get; }

    public LogicalTypeNameAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Parameter)]  
public class LogicalTypePropertyNameAttribute : Attribute
{
    public string Name { get; }

    public LogicalTypePropertyNameAttribute(string name)
    {
        Name = name;
    }
}