using System.Collections;

namespace AvroSerializer.ComplexTypes
{
    public static class MapSchema
    {
        public static bool CanSerialize(object? value) => value is IDictionary;
    }
}
