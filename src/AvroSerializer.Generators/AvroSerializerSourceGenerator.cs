using Avro;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators
{
    [Generator]
    partial class AvroSerializerSourceGenerator : IIncrementalGenerator
    {
        private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax c && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<(ClassDeclarationSyntax serializer, Schema avroSchema, ISymbol serializableSymbol)> classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "DotNetAvroSerializer.AvroSchemaAttribute",
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static item => item is not null)
                .Select(static (item, _) => item.Value);

            context.RegisterSourceOutput(classDeclarations, (context, serializerData) =>
            {
                var (serializationCode, privateFieldsCode) = SerializationCodeGeneratorLoop(serializerData.avroSchema, context, serializerData.serializableSymbol);

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
            });
        }

        private static (ClassDeclarationSyntax serializerClass, Schema schema, ISymbol serializableInput)? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx)
        {
            var serializerSyntax = ctx.TargetNode as ClassDeclarationSyntax;

            var attribute = serializerSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Where(attr => attr.Name.ToString() == "AvroSchema")
                .FirstOrDefault();

            if (attribute is null) return null;

            var attributeSchemaText = attribute.ArgumentList.Arguments.First()?.Expression;

            if (attributeSchemaText is null) return null;

            var schemaString = ctx
                .SemanticModel
                .GetConstantValue(attributeSchemaText)
                .ToString();

            Schema schema;

            try
            {
                schema = Schema.Parse(schemaString);

            }
            catch (Exception)
            {
                return null;
            }

            var serializableType = GetSerializableTypeSymbol(serializerSyntax, ctx);

            return (serializerSyntax, schema, serializableType);
        }

        private static ISymbol GetSerializableTypeSymbol(ClassDeclarationSyntax serializerSyntax, GeneratorAttributeSyntaxContext ctx) => ctx.SemanticModel.GetSymbolInfo(((GenericNameSyntax)serializerSyntax.BaseList.Types.First().Type).TypeArgumentList.Arguments.First()).Symbol;

        private (string serializationCode, string privateFieldsCode) SerializationCodeGeneratorLoop(Schema schema, SourceProductionContext context, ISymbol originTypeSymbol, string sourceAccesor = "source")
        {
            var serializatonCode = new StringBuilder();
            var privateFieldsCode = new PrivateFieldsCode();

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializatonCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);

            return (serializatonCode.ToString(), privateFieldsCode.ToString());
        }
    }
}
