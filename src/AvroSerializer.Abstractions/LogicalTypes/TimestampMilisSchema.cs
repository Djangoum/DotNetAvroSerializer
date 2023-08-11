using AvroSerializer.Primitives;
using System;
using System.IO;

namespace AvroSerializer.LogicalTypes
{
    public static class TimestampMilisSchema
    {
        static DateTime UnixEpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static void Write(Stream outputStream, DateTime date)
        {
            LongSchema.Write(outputStream, (long)(date - UnixEpochDateTime).TotalMilliseconds);
        }
    }
}
