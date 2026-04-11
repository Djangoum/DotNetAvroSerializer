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
