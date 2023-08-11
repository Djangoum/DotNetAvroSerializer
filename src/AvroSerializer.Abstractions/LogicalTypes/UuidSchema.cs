using AvroSerializer.Primitives;
using System;
using System.IO;

namespace AvroSerializer.LogicalTypes
{
    public static class UuidSchema
    {
        public static void Write(Stream outputStream, Guid guid)
        {
            StringSchema.Write(outputStream, guid.ToString());
        }
    }
}
