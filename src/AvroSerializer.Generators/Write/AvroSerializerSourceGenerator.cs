using Avro;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DotNetAvroSerializer.Generators.Write
{
    [Generator]
    public partial class AvroSerializerSourceGenerator : IIncrementalGenerator
    {
        private static bool IsSyntaxGenerationCandidate(SyntaxNode node) => node is ClassDeclarationSyntax c && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<(SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors)> classDeclarationsWithErrors = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxGenerationCandidate(s),
                    transform: static (ctx, ct) => GetTargetDataForGeneration(ctx, ct));

            context.RegisterSourceOutput(classDeclarationsWithErrors.SelectMany(static (values, _) => values.errors), (ctx, error) =>
            {
                ctx.ReportDiagnostic(error);
            });

            IncrementalValuesProvider<(SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors)> classDeclarations = classDeclarationsWithErrors
                .Where(static (item) => item.errors.IsEmpty);

            context.RegisterSourceOutput(classDeclarations, (context, serializerData) =>
            {
                var (serializationCode, privateFieldsCode, diagnostic) = SerializationCodeGeneratorLoop(serializerData.serializerMetadata, serializerData.serializerMetadata.AvroSchema);

                if (diagnostic is not null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    context.AddSource($"{serializerData.serializerMetadata.SerializerClassName}.write.g.cs", 
                        GetGeneratedSerializationSource(
                            serializerData.serializerMetadata.SerializerNamespace, 
                            serializerData.serializerMetadata.SerializerClassName,
                            serializerData.serializerMetadata.SerializableTypeMetadata.FullNameDisplay,
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
                .Where(attr => attr.Name.ToString() == "AvroSchema")
                .FirstOrDefault();

            if (attribute is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingAvroSchemaDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            var attributeSchemaText = attribute.ArgumentList.Arguments.First()?.Expression;

            var schemaString = ctx
                .SemanticModel
                .GetConstantValue(attributeSchemaText)
                .ToString();

            ct.ThrowIfCancellationRequested();

            if (schemaString is null || attributeSchemaText is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingSchemaInAvroSchemaAttributeDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            Schema schema = default;

            try
            {
                schema = Schema.Parse(schemaString);
            }
            catch (Exception)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
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

            return (new SerializerMetadata(serializerSyntax.Identifier.ToString(), Namespaces.GetNamespace(serializerSyntax), schema, serializableTypeMetadata), diagnostics);
        }

        private static ITypeSymbol GetSerializableTypeSymbol(ClassDeclarationSyntax serializerSyntax, GeneratorSyntaxContext ctx)
        {
            var serializerGenericArgument = ((GenericNameSyntax)serializerSyntax.BaseList.Types.First().Type).TypeArgumentList.Arguments.First();

            var symbol = ctx.SemanticModel.GetSymbolInfo(serializerGenericArgument).Symbol as ITypeSymbol;

            if (serializerGenericArgument.Kind() == SyntaxKind.NullableType)
            {
                symbol = symbol.WithNullableAnnotation(NullableAnnotation.Annotated);
            }

            return symbol;
        }

        private (string serializationCode, string privateFieldsCode, Diagnostic diagnostic) SerializationCodeGeneratorLoop(SerializerMetadata serializerMetadata, Schema schema, string sourceAccesor = "source")
        {
            try
            {
                var serializatonCode = new StringBuilder();
                var privateFieldsCode = new PrivateFieldsCode();

                SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializatonCode, privateFieldsCode, serializerMetadata.SerializableTypeMetadata, sourceAccesor);

                return (serializatonCode.ToString(), privateFieldsCode.ToString(), null);
            }
            catch (AvroGeneratorException ex)
            {
                return (string.Empty, string.Empty, Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor, null, ex.Message));
            }
        }
    }
}
