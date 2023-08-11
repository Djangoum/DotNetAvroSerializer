﻿using AvroSerializer.Primitives;
using System;
using System.IO;

namespace AvroSerializer.LogicalTypes
{
    public static class TimeMillisSchema
    {
        internal static readonly TimeOnly MaxTime = new TimeOnly(23, 59, 59);
        internal static TimeOnly UnixEpochTime = new TimeOnly(0, 0, 0);

        public static void Write(Stream outputStream, TimeOnly time)
        {
            if (time > MaxTime)
                throw new ArgumentOutOfRangeException(nameof(time), "A 'time-millis' value can only have the range '00:00:00' to '23:59:59'.");

            IntSchema.Write(outputStream, (int)(time - UnixEpochTime).TotalMilliseconds);
        }
    }
}