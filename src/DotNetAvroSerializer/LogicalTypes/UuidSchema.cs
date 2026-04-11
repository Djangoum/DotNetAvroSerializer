using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.Primitives;

namespace DotNetAvroSerializer.LogicalTypes;

public static class UuidSchema
{
    public static bool CanSerialize(object? value) => value is Guid;

    public static void Write(Stream outputStream, Guid? value)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        Write(outputStream, value.Value);
    }

    public static void Write(Stream outputStream, Guid guid)
    {
        StringSchema.Write(outputStream, guid.ToString());
    }

    public static Task WriteAsync(Stream outputStream, Guid? value, CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new AvroSerializationException("Cannot serialize null value to int");

        return WriteAsync(outputStream, value.Value, cancellationToken);
    }

    public static Task WriteAsync(Stream outputStream, Guid guid, CancellationToken cancellationToken = default)
    {
        return StringSchema.WriteAsync(outputStream, guid.ToString(), cancellationToken);
    }
}
