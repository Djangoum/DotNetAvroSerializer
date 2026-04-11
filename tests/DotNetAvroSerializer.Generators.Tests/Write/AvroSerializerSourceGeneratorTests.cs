using System.Globalization;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Write;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace DotNetAvroSerializer.Generators.Tests.Write;

public class AvroSerializerSourceGeneratorTests
{
    [Fact]
    public void Initialize_MustGenerateSyncSerializationApisForAvroSerializer()
    {
        const string source = """
using System.IO;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
        public virtual void SerializeToStream(Stream outputStream, T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"int\"}")]
public partial class IntSerializer : AvroSerializer<int>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("public override byte[] Serialize");
        generatedSource.Should().Contain("public override void SerializeToStream");
        generatedSource.Should().Contain("IntSchema.Write(outputStream, source);");
        generatedSource.Should().NotContain("public override async Task<byte[]> SerializeAsync");
        generatedSource.Should().NotContain("public override async Task SerializeToStreamAsync");
        generatedSource.Should().NotContain("using System.Linq;");
    }

    [Fact]
    public void Initialize_MustGenerateAsyncSerializationApisForAsyncAvroSerializer()
    {
        const string source = """
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AsyncAvroSerializer<T>
    {
        public int AsyncArrayItemCountBlockSize { get; set; } = 1024;
        public virtual Task<byte[]> SerializeAsync(T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
        public virtual Task SerializeToStreamAsync(Stream outputStream, T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"int\"}")]
public partial class IntSerializer : AsyncAvroSerializer<int>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("public override async Task<byte[]> SerializeAsync");
        generatedSource.Should().Contain("public override async Task SerializeToStreamAsync");
        generatedSource.Should().Contain("await IntSchema.WriteAsync(outputStream, source, cancellationToken);");
        generatedSource.Should().NotContain("public override byte[] Serialize");
        generatedSource.Should().NotContain("public override void SerializeToStream");
    }

    [Fact]
    public void Initialize_MustGenerateCollectionCountWithoutLinqExtensionsForSyncSerializer()
    {
        const string source = """
using System.IO;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
        public virtual void SerializeToStream(Stream outputStream, T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"array\",\"items\":\"int\"}")]
public partial class IntArraySerializer : AvroSerializer<int[]>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("var sourceCount = GetCollectionCount(source);");
        generatedSource.Should().NotContain(".Count()");
        generatedSource.Should().NotContain("using System.Linq;");
    }

    [Fact]
    public void Initialize_MustGenerateStreamedArrayBlocksForAsyncSerializer()
    {
        const string source = """
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AsyncAvroSerializer<T>
    {
        public int AsyncArrayItemCountBlockSize { get; set; } = 1024;
        public virtual Task<byte[]> SerializeAsync(T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
        public virtual Task SerializeToStreamAsync(Stream outputStream, T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"array\",\"items\":\"int\"}")]
public partial class IntArraySerializer : AsyncAvroSerializer<int[]>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("AsyncArrayItemCountBlockSize");
        generatedSource.Should().Contain("new List<global::System.Int32>");
        generatedSource.Should().Contain("if (sourceBatch.Count == sourceBlockSize)");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, sourceBatch.Count, cancellationToken);");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, 0L, cancellationToken);");
        generatedSource.Should().NotContain("GetCollectionCount(source)");
    }

    [Fact]
    public void Initialize_MustGenerateAwaitForeachForIAsyncEnumerable()
    {
        const string source = """
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AsyncAvroSerializer<T>
    {
        public int AsyncArrayItemCountBlockSize { get; set; } = 1024;
        public virtual Task<byte[]> SerializeAsync(T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
        public virtual Task SerializeToStreamAsync(Stream outputStream, T source, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"array\",\"items\":\"int\"}")]
public partial class IntArraySerializer : AsyncAvroSerializer<IAsyncEnumerable<int>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("await foreach(var itemsource in source.WithCancellation(cancellationToken))");
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForInvalidSchema()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"record\",\"name\":\"BrokenRecord\"}")]
public partial class BrokenSerializer : AvroSerializer<int>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("Record schema BrokenRecord has no fields", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticWhenSerializerDoesNotInheritFromAvroSerializer()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

public abstract class OtherSerializer<T>
{
}

[AvroSchema("{\"type\":\"int\"}")]
public partial class InvalidSerializer : OtherSerializer<int>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.SerializerMustInheritFromAvroSerializerDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("InvalidSerializer", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForIAsyncEnumerableWithSyncSerializer()
    {
        const string source = """
using System.Collections.Generic;
using System.IO;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
        public virtual void SerializeToStream(Stream outputStream, T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"array\",\"items\":\"int\"}")]
public partial class InvalidSerializer : AvroSerializer<IAsyncEnumerable<int>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("Use AsyncAvroSerializer<T>", StringComparison.Ordinal)
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("materialize it to a sync collection", StringComparison.Ordinal));
    }


    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)!
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));

        return CSharpCompilation.Create(
            assemblyName: "DotNetAvroSerializer.Generators.Tests.SourceGeneration",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source) },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
