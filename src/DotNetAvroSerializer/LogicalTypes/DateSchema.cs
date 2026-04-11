using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;

namespace DotNetAvroSerializer.LogicalTypes;

public static class DateSchema
{
    static readonly DateOnly UnixEpochDate = new DateOnly(1970, 1, 1);

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

    public static async Task WriteAsync(Stream outputStream, DateOnly? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        await WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static async Task WriteAsync(Stream outputStream, DateTime? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        await WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, DateOnly dateOnly, CancellationToken cancellationToken = default)
    {
        return IntSchema.WriteAsync(outputStream, dateOnly.DayNumber - UnixEpochDate.DayNumber, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, DateTime date, CancellationToken cancellationToken = default)
    {
        return WriteAsync(outputStream, DateOnly.FromDateTime(date), cancellationToken);
    }
}
