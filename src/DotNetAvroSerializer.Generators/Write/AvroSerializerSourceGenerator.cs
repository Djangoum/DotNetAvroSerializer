using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Extensions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.Schemas;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace DotNetAvroSerializer.Generators.Write;

[Generator]
public partial class AvroSerializerSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors)> serializersMetadataWithErrors = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "DotNetAvroSerializer.AvroSchemaAttribute",
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, ct) => GetTargetDataForGeneration(ctx, ct));

        IncrementalValuesProvider<Diagnostic> errors =
            serializersMetadataWithErrors.SelectMany(static (values, _) => values.errors);

        IncrementalValuesProvider<SerializerMetadata> validSerializers = serializersMetadataWithErrors
            .Where(static (item) => item.errors.IsEmpty)
            .Select(static (s, ct) => s.serializerMetadata);

        context.RegisterSourceOutput(errors, (ctx, error) =>
        {
            ctx.ReportDiagnostic(error);
        });

        context.RegisterSourceOutput(validSerializers, (ctx, serializerData) =>
        {
            var (serializationCode, asyncSerializationCode, privateFieldsCode, diagnostic) = SerializationCodeGeneratorLoop(serializerData, serializerData.AvroSchema);

            if (diagnostic is not null)
            {
                ctx.ReportDiagnostic(diagnostic);
            }
            else
            {
                ctx.AddSource($"{serializerData.SerializerClassName}.write.g.cs",
                    GetGeneratedSerializationSource(
                        serializerData.SerializerNamespace,
                        serializerData.SerializerClassName,
                        serializerData.SerializableTypeMetadata.FullNameDisplay,
                        serializerData.SerializerApiKind,
                        serializationCode,
                        asyncSerializationCode,
                        privateFieldsCode));
            }
        });
    }

    private static (SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors) GetTargetDataForGeneration(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var diagnostics = ImmutableArray<Diagnostic>.Empty;
        var serializerSyntax = (ClassDeclarationSyntax)ctx.TargetNode;

        ct.ThrowIfCancellationRequested();

        if (!(serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
            && serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))))
        {
            diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializersMustBePartialClassesDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
            return (null, diagnostics);
        }

        var avroSchemaAttribute = ctx.Attributes[0];

        var schemaStringConstant = avroSchemaAttribute.ConstructorArguments.Length > 0
            ? avroSchemaAttribute.ConstructorArguments[0].Value?.ToString()
            : null;

        if (schemaStringConstant is null)
        {
            diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingSchemaInAvroSchemaAttributeDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
            return (null, diagnostics);
        }

        var (customLogicalTypeNames, logicalTypesDiagnostics) = CustomLogicalTypesMetadataProcessor.GetCustomLogicalTypesMetadata(avroSchemaAttribute);

        if (logicalTypesDiagnostics.Any())
        {
            diagnostics = diagnostics.AddRange(logicalTypesDiagnostics);
            return (null, diagnostics);
        }

        ct.ThrowIfCancellationRequested();

        if (!AvroSchemaParser.TryParse(schemaStringConstant, out var schema, out var schemaValidationError))
        {
            diagnostics = diagnostics.Add(Diagnostic.Create(
                DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor,
                serializerSyntax.GetLocation(),
                serializerSyntax.Identifier.ToString(),
                schemaValidationError));
            return (null, diagnostics);
        }

        ct.ThrowIfCancellationRequested();

        var serializerSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
        var serializerTypeData = GetSerializerTypeData(serializerSymbol, ctx.SemanticModel.Compilation);

        if (serializerTypeData is null)
        {
            diagnostics = diagnostics.Add(Diagnostic.Create(
                DiagnosticsDescriptors.SerializerMustInheritFromAvroSerializerDescriptor,
                serializerSyntax.GetLocation(),
                serializerSyntax.Identifier.ToString()));
            return (null, diagnostics);
        }

        var (serializableType, serializerApiKind) = serializerTypeData.Value;
        var serializableTypeMetadata = SerializableTypeMetadata.From(serializableType, ctx.SemanticModel.Compilation);

        if (serializableTypeMetadata is null)
        {
            diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeIsNotAllowedDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
        }

        ct.ThrowIfCancellationRequested();

        var rawLocation = serializerSyntax.GetLocation();
        var smallLocation = new SmallLocation(rawLocation.SourceTree?.FilePath, rawLocation.SourceSpan, rawLocation.GetLineSpan().Span);

        var serializerMetadata = new SerializerMetadata(
            serializerSyntax.Identifier.ToString(),
            Namespaces.GetNamespace(serializerSyntax),
            schema,
            serializableTypeMetadata,
            serializerApiKind,
            customLogicalTypeNames,
            smallLocation);

        return (serializerMetadata, diagnostics);
    }

    private static (ITypeSymbol serializableType, SerializerApiKind serializerApiKind)? GetSerializerTypeData(INamedTypeSymbol serializerSymbol, Compilation compilation)
    {
        var avroSerializerType = compilation.GetTypeByMetadataName("DotNetAvroSerializer.AvroSerializer`1");
        var asyncAvroSerializerType = compilation.GetTypeByMetadataName("DotNetAvroSerializer.AsyncAvroSerializer`1");

        if (serializerSymbol.BaseType is not { IsGenericType: true, TypeArguments.Length: 1 } baseType)
        {
            return null;
        }

        if (SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, avroSerializerType))
        {
            return (baseType.TypeArguments[0], SerializerApiKind.Sync);
        }

        if (SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, asyncAvroSerializerType))
        {
            return (baseType.TypeArguments[0], SerializerApiKind.Async);
        }

        return null;
    }

    private static (string serializationCode, string asyncSerializationCode, string privateFieldsCode, Diagnostic diagnostic) SerializationCodeGeneratorLoop(SerializerMetadata serializerMetadata, Schema schema)
    {
        try
        {
            var syncSerializationCode = string.Empty;
            var asyncSerializationCode = string.Empty;
            var privateFieldsCode = string.Empty;

            if (serializerMetadata.SerializerApiKind is SerializerApiKind.Sync)
            {
                var syncContext = AvroGenerationContext.From(serializerMetadata, schema, SerializationMode.Sync);
                schema.Generate(syncContext);

                syncSerializationCode = syncContext.SerializationCode.ToString();
                privateFieldsCode = syncContext.PrivateFieldsCode.ToString();
            }
            else
            {
                var asyncContext = AvroGenerationContext.From(serializerMetadata, schema, SerializationMode.Async);
                schema.Generate(asyncContext);

                asyncSerializationCode = asyncContext.SerializationCode.ToString();
                privateFieldsCode = asyncContext.PrivateFieldsCode.ToString();
            }

            return (syncSerializationCode, asyncSerializationCode, privateFieldsCode, null);
        }
        catch (AvroGeneratorException ex)
        {
            return (string.Empty, string.Empty, string.Empty, Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor, serializerMetadata.GetSerializerLocation(), ex.Message));
        }
    }
}
