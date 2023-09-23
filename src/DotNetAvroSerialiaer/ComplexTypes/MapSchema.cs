using System.Collections;

namespace DotNetAvroSerializer.ComplexTypes
{
    public static class MapSchema
    {
        public static bool CanSerialize(object? value) => value is IDictionary;
    }
}
