using Avro;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators
{
    [Generator]
    partial class AvroSerializerSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var collector = (AvroSerializersCollector)context.SyntaxReceiver;

            foreach (var serializer in collector.AvroSerializers)
            {
                var originType = ((GenericNameSyntax)serializer.BaseList.Types.First().Type).TypeArgumentList.Arguments.First();

                var attribute = serializer
                    .AttributeLists
                    .SelectMany(x => x.Attributes)
                    .Where(attr => attr.Name.ToString() == "AvroSchema")
                    .Single();

                var attributeSchemaText = attribute.ArgumentList.Arguments.First().Expression;

                var schemaString = context.Compilation
                    .GetSemanticModel(serializer.SyntaxTree)
                    .GetConstantValue(attributeSchemaText)
                    .ToString();

                var schema = Schema.Parse(schemaString);

                var nameTypeSymbol = context.Compilation.GetSemanticModel(serializer.SyntaxTree).GetSymbolInfo(originType);

                var (serializationCode, privateFieldsCode) = SerializationCodeGeneratorLoop(schema, context, nameTypeSymbol.Symbol);

                context.AddSource($"{serializer.Identifier}.g.cs",
                    CSharpSyntaxTree.ParseText(SourceText.From(
$@"using DotNetAvroSerializer.Primitives;
using DotNetAvroSerializer.LogicalTypes;
using DotNetAvroSerializer.Exceptions;
using DotNetAvroSerializer.ComplexTypes;
using System.Linq;

namespace {Namespaces.GetNamespace(serializer)}
{{
    public partial class {serializer.Identifier}
    {{
        {privateFieldsCode}

        public byte[] Serialize({context.Compilation.GetSemanticModel(originType.SyntaxTree).GetTypeInfo(originType).Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source)
        {{
            var outputStream = new MemoryStream();
            
            SerializeToStream(outputStream, source);

            return outputStream.ToArray();
        }}

        public void SerializeToStream(Stream outputStream, {context.Compilation.GetSemanticModel(originType.SyntaxTree).GetTypeInfo(originType).Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source)
        {{
{serializationCode}
        }}
    }}
}}", Encoding.UTF8)).GetRoot().NormalizeWhitespace().SyntaxTree.GetText());
            }
        }

        private (string serializationCode, string privateFieldsCode) SerializationCodeGeneratorLoop(Schema schema, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor = "source")
        {
            var serializatonCode = new StringBuilder();
            var privateFieldsCode = new PrivateFieldsCode();

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializatonCode, privateFieldsCode, context, originTypeSymbol, sourceAccesor);

            return (serializatonCode.ToString(), privateFieldsCode.ToString());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AvroSerializersCollector());
        }

        public class AvroSerializersCollector : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> AvroSerializers { get; } = new List<ClassDeclarationSyntax>();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax c
                    && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false))
                {
                    var type2 = c.BaseList?.Types.First().Type;
                    AvroSerializers.Add(c);
                }
            }
        }
    }
}
