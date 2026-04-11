using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;

namespace DotNetAvroSerializer.LogicalTypes;

public static class TimeMillisSchema
{
    private static readonly TimeOnly MaxTime = new TimeOnly(23, 59, 59);
    private static readonly TimeOnly UnixEpochTime = new TimeOnly(0, 0, 0);

    public static bool CanSerialize(object? value) => value is TimeOnly;

    public static void Write(Stream outputStream, TimeOnly? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to time-millis");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, TimeOnly time)
    {
        if (time > MaxTime)
            throw new ArgumentOutOfRangeException(nameof(time), "A 'time-millis' value can only have the range '00:00:00' to '23:59:59'.");

        IntSchema.Write(outputStream, (int)(time - UnixEpochTime).TotalMilliseconds);
    }

    public static Task WriteAsync(Stream outputStream, TimeOnly? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to time-millis");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, TimeOnly time, CancellationToken cancellationToken = default)
    {
        if (time > MaxTime)
            throw new ArgumentOutOfRangeException(nameof(time), "A 'time-millis' value can only have the range '00:00:00' to '23:59:59'.");

        return IntSchema.WriteAsync(outputStream, (int)(time - UnixEpochTime).TotalMilliseconds, cancellationToken);
    }
}
