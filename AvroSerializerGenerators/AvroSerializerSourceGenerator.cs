using Avro;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
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
                var originTypeName = ((GenericNameSyntax)serializer.BaseList.Types.First().Type).TypeArgumentList.Arguments.First();

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

                var serializeCode = GenerateSerializationSource(schema, context, originTypeName);

                context.AddSource($"{serializer.Identifier}.g.cs",
                    SourceText.From(
$@"using AvroSerializer.Abstractions.Types;

namespace {GetNamespace(serializer)}
{{
    public partial class {serializer.Identifier}
    {{
        public Stream Serialize({originTypeName} obj)
        {{
            var outputStream = new MemoryStream();
            {serializeCode}
            return outputStream;
        }}
    }}
}}", Encoding.UTF8));
            }
        }

        private string GenerateSerializationSource(Schema schema, GeneratorExecutionContext context, TypeSyntax originTypeName)
        {
            StringBuilder sb = new StringBuilder();

            if (schema is PrimitiveSchema primitiveSchema)
            {
                switch (primitiveSchema.Name)
                {
                    case "boolean":
                        if (!originTypeName.ToString().Equals("bool"))
                            throw new Exception();

                        sb.Append(
@$"
            BooleanSchema.Write(outputStream, obj);
");
                        break;
                    case "int":
                        if (!originTypeName.ToString().Equals("int"))
                            throw new Exception();

                        sb.Append(
@$"
            IntSchema.Write(outputStream, obj);
");
                        break;
                    case "long":
                        if (!originTypeName.ToString().Equals("long"))
                            throw new Exception();

                        sb.Append(
@$"
            LongSchema.Write(outputStream, obj);
");
                        break;
                    case "string":
                        if (!originTypeName.ToString().Equals("string"))
                            throw new Exception();

                        sb.Append(
@$"
            StringSchema.Write(outputStream, obj);
");
                        break;
                    case "bytes":
                        if (!originTypeName.ToString().Equals("byte[]"))
                            throw new Exception();

                        sb.Append(
@$"
            BytesSchema.Write(outputStream, obj);
");
                        break;
                }
            }

            return sb.ToString();
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

        // determine the namespace the class/enum/struct is declared in, if any
        static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }
    }


}
