using System;
using System.IO;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;

namespace DotNetAvroSerializer.LogicalTypes;

public static class TimestampMillisSchema
{
    static readonly DateTime UnixEpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static bool CanSerialize(object? value) => value is DateTime;

    public static void Write(Stream outputStream, DateTime? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, DateTime date)
    {
        LongSchema.Write(outputStream, (long)(date - UnixEpochDateTime).TotalMilliseconds);
    }
}
