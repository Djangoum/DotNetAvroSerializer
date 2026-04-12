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
        generatedSource.Should().Contain("/// Serializes <paramref name=\"source\"/> to Avro binary format and returns the resulting payload.");
        generatedSource.Should().Contain("/// <param name=\"source\">The value to serialize.</param>");
        generatedSource.Should().Contain("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"outputStream\"/> is <see langword=\"null\"/>.</exception>");
        generatedSource.Should().Contain("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema.</exception>");
        generatedSource.Should().Contain("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"source\"/> is <see langword=\"null\"/>.</exception>");
        generatedSource.Should().Contain("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"DotNetAvroSerializer\", \"1.0.0.0\")]");
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
        generatedSource.Should().Contain("/// <param name=\"cancellationToken\">A token that can be used to cancel asynchronous serialization.</param>");
        generatedSource.Should().Contain("/// For array and map schemas, serialization is streamed in Avro blocks.");
        generatedSource.Should().Contain("/// A final zero-sized block is written to mark the end of the collection.");
        generatedSource.Should().Contain("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"outputStream\"/> is <see langword=\"null\"/>.</exception>");
        generatedSource.Should().Contain("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema, or when async collection block size is invalid.</exception>");
        generatedSource.Should().Contain("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"DotNetAvroSerializer\", \"1.0.0.0\")]");
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
        generatedSource.Should().Contain("new List<int>");
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
    public void Initialize_MustGenerateStreamedMapBlocksForAsyncSerializer()
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

[AvroSchema("{\"type\":\"map\",\"values\":\"int\"}")]
public partial class IntMapSerializer : AsyncAvroSerializer<Dictionary<string, int>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("AsyncArrayItemCountBlockSize");
        generatedSource.Should().Contain("new List<global::System.Collections.Generic.KeyValuePair<string,");
        generatedSource.Should().Contain("if (sourceBatch.Count == sourceBlockSize)");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, sourceBatch.Count, cancellationToken);");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, 0L, cancellationToken);");
        generatedSource.Should().NotContain("GetCollectionCount(source)");
    }

    [Fact]
    public void Initialize_MustGenerateStreamedArrayBlocksForAsyncSerializer_WithComplexType()
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

public class ItemRecord
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[AvroSchema("{\"type\":\"array\",\"items\":{\"type\":\"record\",\"name\":\"ItemRecord\",\"fields\":[{\"name\":\"id\",\"type\":\"int\"},{\"name\":\"name\",\"type\":\"string\"}]}}")]
public partial class ItemArraySerializer : AsyncAvroSerializer<List<ItemRecord>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("new List<global::Sample.ItemRecord>");
        generatedSource.Should().Contain("if (sourceBatch.Count == sourceBlockSize)");
        generatedSource.Should().Contain("await IntSchema.WriteAsync(outputStream, itemsourceInBlock.Id, cancellationToken);");
        generatedSource.Should().Contain("await StringSchema.WriteAsync(outputStream, itemsourceInBlock.Name, cancellationToken);");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, 0L, cancellationToken);");
        generatedSource.Should().NotContain("GetCollectionCount(source)");
    }

    [Fact]
    public void Initialize_MustGenerateStreamedMapBlocksForAsyncSerializer_WithComplexType()
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

public class ItemRecord
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[AvroSchema("{\"type\":\"map\",\"values\":{\"type\":\"record\",\"name\":\"ItemRecord\",\"fields\":[{\"name\":\"id\",\"type\":\"int\"},{\"name\":\"name\",\"type\":\"string\"}]}}")]
public partial class ItemMapSerializer : AsyncAvroSerializer<Dictionary<string, ItemRecord>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var generatedSource = driver.GetRunResult().Results.Single().GeneratedSources.Single().SourceText.ToString();

        generatedSource.Should().Contain("new List<global::System.Collections.Generic.KeyValuePair<string, global::Sample.ItemRecord>>");
        generatedSource.Should().Contain("if (sourceBatch.Count == sourceBlockSize)");
        generatedSource.Should().Contain("await IntSchema.WriteAsync(outputStream, itemsourceInBlock.Value.Id, cancellationToken);");
        generatedSource.Should().Contain("await StringSchema.WriteAsync(outputStream, itemsourceInBlock.Value.Name, cancellationToken);");
        generatedSource.Should().Contain("await LongSchema.WriteAsync(outputStream, 0L, cancellationToken);");
        generatedSource.Should().NotContain("GetCollectionCount(source)");
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

    [Fact]
    public void Initialize_MustWarnWhenMultipleAvroSchemaAttributesArePresent()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }
}

namespace Sample;

[AvroSchema("{\"type\":\"int\"}")]
[AvroSchema("{\"type\":\"long\"}")]
public partial class IntSerializer : AvroSerializer<int>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.MultipleAvroSchemaAttributesDescriptor.Id
            && d.Severity == DiagnosticSeverity.Warning
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("only the first one will be used", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForDuplicateAvroFieldAlias()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public sealed class AvroFieldAttribute : System.Attribute
    {
        public AvroFieldAttribute(string fieldName)
        {
        }
    }
}

namespace Sample;

public class Item
{
    [AvroField("id")]
    public int FirstId { get; set; }

    [AvroField("id")]
    public int SecondId { get; set; }
}

[AvroSchema("{\"type\":\"record\",\"name\":\"Item\",\"fields\":[{\"name\":\"id\",\"type\":\"int\"}]}")]
public partial class ItemSerializer : AvroSerializer<Item>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.DuplicateAvroFieldAliasDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("AvroField alias 'id'", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForAmbiguousFieldBinding()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AvroSchemaAttribute : System.Attribute
    {
        public AvroSchemaAttribute(string schema, System.Type[] allowedCustomLogicalTypes = null)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public sealed class AvroFieldAttribute : System.Attribute
    {
        public AvroFieldAttribute(string fieldName)
        {
        }
    }
}

namespace Sample;

public class Item
{
    public int Id { get; set; }

    [AvroField("id")]
    public int LegacyId { get; set; }
}

[AvroSchema("{\"type\":\"record\",\"name\":\"Item\",\"fields\":[{\"name\":\"id\",\"type\":\"int\"}]}")]
public partial class ItemSerializer : AvroSerializer<Item>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.AmbiguousFieldBindingDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("matches multiple properties", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForUnionOrderMismatch()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
    }

    public sealed class Union<T1, T2>
    {
        public byte Index => throw new System.NotImplementedException();
        public T1 Value1 => throw new System.NotImplementedException();
        public T2 Value2 => throw new System.NotImplementedException();
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

[AvroSchema("[\"int\",\"string\"]")]
public partial class UnionSerializer : AvroSerializer<Union<string, int>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.UnionSchemaOrderMismatchDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("Union member order mismatch", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForUnsupportedNullablePattern()
    {
        const string source = """
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
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

[AvroSchema("[\"int\",\"long\"]")]
public partial class NullableSerializer : AvroSerializer<int?>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.UnsupportedNullablePatternDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("requires an Avro union containing 'null'", StringComparison.Ordinal));
    }

    [Fact]
    public void Initialize_MustEmitDiagnosticForUnsupportedMapKeyType()
    {
        const string source = """
using System.Collections.Generic;
using DotNetAvroSerializer;

namespace DotNetAvroSerializer
{
    public abstract class AvroSerializer<T>
    {
        public virtual byte[] Serialize(T source) => throw new System.NotImplementedException();
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

[AvroSchema("{\"type\":\"map\",\"values\":\"int\"}")]
public partial class InvalidMapSerializer : AvroSerializer<Dictionary<int, int>>
{
}
""";

        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AvroSerializerSourceGenerator().AsSourceGenerator());

        driver = driver.RunGenerators(compilation);

        var diagnostics = driver.GetRunResult().Results.Single().Diagnostics;

        diagnostics.Should().ContainSingle(d =>
            d.Id == DiagnosticsDescriptors.UnsupportedMapKeyTypeDescriptor.Id
            && d.GetMessage(CultureInfo.InvariantCulture).Contains("require dictionary keys of type string", StringComparison.Ordinal));
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
