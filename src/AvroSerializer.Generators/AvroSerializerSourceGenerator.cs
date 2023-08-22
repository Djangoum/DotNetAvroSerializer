using Avro;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators
{
    [Generator]
    partial class AvroSerializerSourceGenerator : IIncrementalGenerator
    {
        private static bool IsSyntaxGenerationCandidate(SyntaxNode node) => node is ClassDeclarationSyntax c && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<(ClassDeclarationSyntax serializer, Schema avroSchema, ISymbol serializableSymbol, ImmutableArray<Diagnostic> errors)> classDeclarationsWithErrors = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxGenerationCandidate(s),
                    transform: static (ctx, _) => GetTargetDataForGeneration(ctx));

            context.RegisterSourceOutput(classDeclarationsWithErrors.SelectMany(static (values, _) => values.errors), (ctx, error) =>
            {
                ctx.ReportDiagnostic(error);
            });

            IncrementalValuesProvider<(ClassDeclarationSyntax serializer, Schema avroSchema, ISymbol serializableSymbol, ImmutableArray<Diagnostic> errors)> classDeclarations = classDeclarationsWithErrors
                .Where(static (item) => item.errors.IsEmpty);

            context.RegisterSourceOutput(classDeclarations, (context, serializerData) =>
            {
                var (serializationCode, privateFieldsCode, diagnostic) = SerializationCodeGeneratorLoop(serializerData.serializer, serializerData.avroSchema, context, serializerData.serializableSymbol);

                if (diagnostic is not null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    context.AddSource($"{serializerData.serializer.Identifier}.g.cs",
                    CSharpSyntaxTree.ParseText(SourceText.From(
$@"using DotNetAvroSerializer.Primitives;
using DotNetAvroSerializer.LogicalTypes;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.ComplexTypes;
using System.Linq;
using System.IO;

namespace {Namespaces.GetNamespace(serializerData.serializer)}
{{
    public partial class {serializerData.serializer.Identifier}
    {{
        {privateFieldsCode}

        public byte[] Serialize({serializerData.serializableSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source)
        {{
            var outputStream = new MemoryStream();
            
            SerializeToStream(outputStream, source);

            return outputStream.ToArray();
        }}

        public void SerializeToStream(Stream outputStream, {serializerData.serializableSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source)
        {{
{serializationCode}
        }}
    }}
}}", Encoding.UTF8)).GetRoot().NormalizeWhitespace().SyntaxTree.GetText());
                }
            });
        }

        private static (ClassDeclarationSyntax serializerClass, Schema schema, ISymbol serializableInput, ImmutableArray<Diagnostic>) GetTargetDataForGeneration(GeneratorSyntaxContext ctx)
        {
            var diagnostics = ImmutableArray<Diagnostic>.Empty;
            var serializerSyntax = ctx.Node as ClassDeclarationSyntax;

            if (!(serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                && serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))))
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializersMustBePartialClassesDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (serializerSyntax, null, null, diagnostics);
            }

            var attribute = serializerSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Where(attr => attr.Name.ToString() == "AvroSchema")
                .FirstOrDefault();

            if (attribute is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingAvroSchemaDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (serializerSyntax, null, null, diagnostics);
            }

            var attributeSchemaText = attribute.ArgumentList.Arguments.First()?.Expression;

            var schemaString = ctx
                .SemanticModel
                .GetConstantValue(attributeSchemaText)
                .ToString();

            if (schemaString is null || attributeSchemaText is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingSchemaInAvroSchemaAttributeDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (serializerSyntax, null, null, diagnostics);
            }

            Schema schema = default;

            try
            {
                schema = Schema.Parse(schemaString);
            }
            catch (Exception)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (serializerSyntax, null, null, diagnostics);
            }

            var serializableType = GetSerializableTypeSymbol(serializerSyntax, ctx);

            return (serializerSyntax, schema, serializableType, diagnostics);
        }

        private static ISymbol GetSerializableTypeSymbol(ClassDeclarationSyntax serializerSyntax, GeneratorSyntaxContext ctx) => ctx.SemanticModel.GetSymbolInfo(((GenericNameSyntax)serializerSyntax.BaseList.Types.First().Type).TypeArgumentList.Arguments.First()).Symbol;

        private (string serializationCode, string privateFieldsCode, Diagnostic diagnostic) SerializationCodeGeneratorLoop(ClassDeclarationSyntax serializerSyntax, Schema schema, SourceProductionContext context, ISymbol serializableTypeSymbol, string sourceAccesor = "source")
        {
            try
            {
                var serializatonCode = new StringBuilder();
                var privateFieldsCode = new PrivateFieldsCode();

                SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializatonCode, privateFieldsCode, context, serializableTypeSymbol, sourceAccesor);

                return (serializatonCode.ToString(), privateFieldsCode.ToString(), null);
            }
            catch (AvroGeneratorException ex)
            {
                return (string.Empty, string.Empty, Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor, serializerSyntax.GetLocation(), ex.Message));
            }

        }
    }
}
