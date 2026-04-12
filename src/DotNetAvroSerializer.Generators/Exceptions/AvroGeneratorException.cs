using System;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Exceptions;

public class AvroGeneratorException : Exception
{
    public DiagnosticDescriptor Descriptor { get; }

    public AvroGeneratorException(string message) : base(message)
    {
    }

    public AvroGeneratorException(DiagnosticDescriptor descriptor, string message) : base(message)
    {
        Descriptor = descriptor;
    }
}
