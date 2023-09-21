using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;
using System;
using System.IO;

namespace DotNetAvroSerializer.LogicalTypes
{
    public static class DateSchema
    {
        static DateOnly UnixEpochDate = new DateOnly(1970, 1, 1);

        public static bool CanSerialize(object? value) => value is DateOnly;

        public static void Write(Stream outputStream, DateOnly? value)
        {
            if (value is null)
                throw new AvroSerializationException("Cannot serialize null value to int");

            Write(outputStream, value.Value);
        }

        public static void Write(Stream outputStream, DateTime? value)
        {
            if (value is null)
                throw new AvroSerializationException("Cannot serialize null value to int");

            Write(outputStream, value.Value);
        }

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
