﻿using System;
using System.IO;
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
}
