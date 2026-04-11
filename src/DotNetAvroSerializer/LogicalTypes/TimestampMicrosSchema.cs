using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;

namespace DotNetAvroSerializer.LogicalTypes;

public static class TimestampMicrosSchema
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
        LongSchema.Write(outputStream, (date - UnixEpochDateTime).Ticks / 10);
    }

    public static async Task WriteAsync(Stream outputStream, DateTime? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        await WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, DateTime date, CancellationToken cancellationToken = default)
    {
        return LongSchema.WriteAsync(outputStream, (date - UnixEpochDateTime).Ticks / 10, cancellationToken);
    }
}
