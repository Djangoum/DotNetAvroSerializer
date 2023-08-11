using AvroSerializer.Primitives;
using System;
using System.IO;

namespace AvroSerializer.LogicalTypes
{
    public static class DateSchema
    {
        static DateOnly UnixEpochDate = new DateOnly(1970, 1, 1);

        public static void Write(Stream outputStream, DateOnly dateOnly)
        {
            IntSchema.Write(outputStream, dateOnly.DayNumber - UnixEpochDate.DayNumber);
        }

        public static void Write(Stream outputStream, DateTime date)
        {
            Write(outputStream, DateOnly.FromDateTime(date));
        }
    }
}
