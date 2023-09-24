using Avro;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DotNetAvroSerializer.Generators.Extensions;
using System.Diagnostics;

namespace DotNetAvroSerializer.Generators.Write
{
    [Generator]
    public partial class AvroSerializerSourceGenerator : IIncrementalGenerator
    {
        private static bool IsSyntaxGenerationCandidate(SyntaxNode node) => node is ClassDeclarationSyntax c && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<(SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors)> serializersMetadataWithErrors = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxGenerationCandidate(s),
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
                var (serializationCode, privateFieldsCode, diagnostic) = SerializationCodeGeneratorLoop(serializerData, serializerData.AvroSchema);

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
                            serializationCode,
                            privateFieldsCode));
                }
            });
        }

        private static (SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors) GetTargetDataForGeneration(GeneratorSyntaxContext ctx, CancellationToken ct)
        {
            var diagnostics = ImmutableArray<Diagnostic>.Empty;
            var serializerSyntax = ctx.Node as ClassDeclarationSyntax;

            ct.ThrowIfCancellationRequested();

            if (!(serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                && serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))))
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializersMustBePartialClassesDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            var attribute = serializerSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(attr => attr.Name.ToString() == "AvroSchema");

            if (attribute is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingAvroSchemaDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            var attributeSchemaText = attribute.ArgumentList?.Arguments.First()?.Expression;
            var customLogicalTypes = attribute.ArgumentList?.Arguments.Count > 1
                ? attribute.ArgumentList.Arguments.ElementAt(1).Expression
                : null;

            var (customLogicalTypeNames, logicalTypesDiagnostics) = CustomLogicalTypesMetadataProcessor.GetCustomLogicalTypesMetadata(customLogicalTypes, serializerSyntax, ctx);

            if (logicalTypesDiagnostics.Any())
            {
                diagnostics = diagnostics.AddRange(logicalTypesDiagnostics);
                return (null, diagnostics);
            }

            var schemaString = ctx
                .SemanticModel
                .GetConstantValue(attributeSchemaText);

            ct.ThrowIfCancellationRequested();

            if (!schemaString.HasValue)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingSchemaInAvroSchemaAttributeDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            Schema schema = default;

            try
            {
                schema = Schema.Parse(schemaString.ToString());
            }
            catch (Exception ex)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString(), ex.Message));
                return (null, diagnostics);
            }

            ct.ThrowIfCancellationRequested();

            var serializableType = GetSerializableTypeSymbol(serializerSyntax, ctx);

            var serializableTypeMetadata = SerializableTypeMetadata.From(serializableType);

            if (serializableTypeMetadata is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeIsNotAllowedDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
            }

            ct.ThrowIfCancellationRequested();

            var serializerMetadata = new SerializerMetadata(serializerSyntax.Identifier.ToString(),
                Namespaces.GetNamespace(serializerSyntax), schema, serializableTypeMetadata, customLogicalTypeNames);
            
            serializerMetadata.ExtractLocation(serializerSyntax.GetLocation());
            
            return (serializerMetadata, diagnostics);
        }
        
        private static ITypeSymbol GetSerializableTypeSymbol(ClassDeclarationSyntax serializerSyntax, GeneratorSyntaxContext ctx)
        {
            var serializerGenericArgument = ((GenericNameSyntax)serializerSyntax.BaseList.Types.First().Type).TypeArgumentList.Arguments.First();

            var symbol = ctx.SemanticModel.GetSymbolInfo(serializerGenericArgument).Symbol as ITypeSymbol;

            if (serializerGenericArgument.Kind() == SyntaxKind.NullableType)
            {
                symbol = symbol!.WithNullableAnnotation(NullableAnnotation.Annotated);
            }

            return symbol;
        }

        private static (string serializationCode, string privateFieldsCode, Diagnostic diagnostic) SerializationCodeGeneratorLoop(SerializerMetadata serializerMetadata, Schema schema, string sourceAccessor = "source")
        {
            try
            {
                var generationContext = AvroGenerationContext.From(serializerMetadata, schema);

                schema.Generate(generationContext);

                return (generationContext.SerializationCode.ToString(), generationContext.PrivateFieldsCode.ToString(), null);
            }
            catch (AvroGeneratorException ex)
            {
                return (string.Empty, string.Empty, Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor, serializerMetadata.GetSerializerLocation(), ex.Message));
            }
        }
    }
}
