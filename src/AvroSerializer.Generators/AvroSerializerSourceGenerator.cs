using Avro;
using AvroSerializer.Generators.Helpers;
using AvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AvroSerializer.Generators
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
                if (!Debugger.IsAttached)Debugger.Launch();
                var schema = Schema.Parse(schemaString);

                var nameTypeSymbol = context.Compilation.GetSemanticModel(serializer.SyntaxTree).GetSymbolInfo(originType);

                var serializeCode = SerializationCodeGeneratorLoop(schema, context, nameTypeSymbol.Symbol);

                context.AddSource($"{serializer.Identifier}.g.cs",
                    SourceText.From(
$@"using AvroSerializer.Primitives;
using AvroSerializer.LogicalTypes;
using AvroSerializer.Exceptions;
using AvroSerializer.ComplexTypes;
using System.Linq;

namespace {Namespaces.GetNamespace(serializer)}
{{
    public partial class {serializer.Identifier}
    {{
        public byte[] Serialize({originType} source)
        {{
            var outputStream = new MemoryStream();
{serializeCode}
            return outputStream.ToArray();
        }}
    }}
}}", Encoding.UTF8));
            }
        }

        private string SerializationCodeGeneratorLoop(Schema schema, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor = "source")
        {
            var code = new StringBuilder();

            SerializationGenerator.GenerateSerializatonSourceForSchema(schema, code, context, originTypeSymbol, sourceAccesor);

            return code.ToString();
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
