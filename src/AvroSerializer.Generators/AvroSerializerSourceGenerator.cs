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

                var serializeCode = SerializationCodeGeneratorLoop(schema, context, originType.ToString());

                context.AddSource($"{serializer.Identifier}.g.cs",
                    SourceText.From(
$@"using AvroSerializer.Primitives;
using AvroSerializer.LogicalTypes;
using System.Linq;

namespace {GetNamespace(serializer)}
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

        private string SerializationCodeGeneratorLoop(Schema schema, GeneratorExecutionContext context, string originTypeName, string sourceAccesor = "source")
        {
            var code = new StringBuilder();

            GenerateSerializatonSourceForSchema(schema, code, context, originTypeName, sourceAccesor);

            return code.ToString();
        }

        private void GenerateSerializatonSourceForSchema(Schema schema, StringBuilder code, GeneratorExecutionContext context, string originTypeName, string sourceAccesor)
        {
            switch (schema)
            {
                case RecordSchema recordSchema:
                    foreach (var field in recordSchema.Fields)
                    {
                        GenerateSerializationSourceForRecordField(field, code, context, originTypeName, sourceAccesor);
                    }
                    break;

                case LogicalSchema logicalSchema:
                    GenerateSerializationSourceForLogicalType(logicalSchema, code, originTypeName, sourceAccesor);
                    break;

                case ArraySchema arraySchema:
                    GenerateSerializationSourceForArray(arraySchema, code, context, originTypeName, sourceAccesor);
                    break;

                case EnumSchema enumSchema:
                    // Handle EnumSchema case
                    break;

                case FixedSchema fixedSchema:
                    // Handle FixedSchema case
                    break;

                case UnionSchema unionSchema:
                    // Handle Union case
                    break;

                case PrimitiveSchema primitiveSchema:
                    GenerateSerializationSourceForPrimitive(primitiveSchema, code, originTypeName, sourceAccesor);
                    break;
            }
        }

        private void GenerateSerializationSourceForLogicalType(LogicalSchema logicalSchema, StringBuilder code, string originTypeName, string sourceAccesor)
        {
            var serializerCallCode = logicalSchema.LogicalType.Name switch
            {
                "date" when originTypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeName.Equals("DateOnly", StringComparison.InvariantCultureIgnoreCase) => $"DateSchema.Write(outputStream, {sourceAccesor});",
                "uuid" when originTypeName.Equals("Guid", StringComparison.InvariantCultureIgnoreCase) => $"UuidSchema.Write(outputStream, {sourceAccesor});",
                "time-millis" when originTypeName.Equals("TimeOnly", StringComparison.InvariantCultureIgnoreCase) => $"TimeMillis.Write(outputStream, {sourceAccesor});",
                "timestamp-millis" when originTypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",
                "local-timestamp-milis" when originTypeName.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase) => $"TimestampMilisSchema.Write(outputStream, {sourceAccesor});",

                _ => throw new Exception()
            };

            code.AppendLine(serializerCallCode);
        }

        private void GenerateSerializationSourceForArray(ArraySchema schema, StringBuilder code, GeneratorExecutionContext context, string originTypeName, string sourceAccesor)
        {
            code.AppendLine(@$"LongSchema.Write(outputStream, (long){sourceAccesor}.Count());");
            code.AppendLine($@"foreach(var item in {sourceAccesor})");
            code.AppendLine("{");

            GenerateSerializatonSourceForSchema(schema.ItemSchema, code, context, originTypeName, "item");

            code.AppendLine("}");
            code.AppendLine(@$"LongSchema.Write(outputStream, 0L);");
        }

        private void GenerateSerializationSourceForRecordField(Field field, StringBuilder code, GeneratorExecutionContext context, string originTypeName, string sourceAccesor) 
        {
            var classSymbol = context.Compilation.GetTypeByMetadataName(originTypeName);

            var property = classSymbol.GetMembers().FirstOrDefault(s => s.Kind is SymbolKind.Property && s.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)) as IPropertySymbol;

            string typeName;

            if (property.Type.AllInterfaces.Any(i => i.Name.Contains("IEnumerable")) && !property.Type.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
            {
                if (property.Type is INamedTypeSymbol namedTypeSymbol)
                {
                    typeName = namedTypeSymbol.TypeArguments.First().Name;
                }
                else if(property.Type is IArrayTypeSymbol arrayTypeSymbol)
                {
                    typeName = arrayTypeSymbol.ElementType.Name;
                }
                else
                {
                    typeName = property.Type.Name;
                }
            }
            else
            {
                typeName = property.Type.Name;
            }

            GenerateSerializatonSourceForSchema(field.Schema, code, context, typeName, $"{sourceAccesor}.{property.Name}");
        }

        private void GenerateSerializationSourceForPrimitive(PrimitiveSchema primitiveSchema, StringBuilder code, string originTypeName, string sourceAccesor = "source")
        {
            var serializerCallCode = primitiveSchema.Name switch
            {
                "boolean" when originTypeName.Equals("bool", StringComparison.InvariantCultureIgnoreCase) => $"BooleanSchema.Write(outputStream, {sourceAccesor});",
                "int" when originTypeName.Equals("int", StringComparison.InvariantCultureIgnoreCase)
                           || originTypeName.Equals("Int32", StringComparison.InvariantCultureIgnoreCase) => $"IntSchema.Write(outputStream, {sourceAccesor});",
                "long" when originTypeName.Equals("long", StringComparison.InvariantCultureIgnoreCase)
                            || originTypeName.Equals("Int64", StringComparison.InvariantCultureIgnoreCase) => $"LongSchema.Write(outputStream, {sourceAccesor});",
                "string" when originTypeName.Equals("string", StringComparison.InvariantCultureIgnoreCase) => $"StringSchema.Write(outputStream, {sourceAccesor});",
                "bytes" when originTypeName.Equals("byte[]", StringComparison.InvariantCultureIgnoreCase) => $"BytesSchema.Write(outputStream, {sourceAccesor});",
                "double" when originTypeName.Equals("double", StringComparison.InvariantCultureIgnoreCase) => $"DoubleSchema.Write(outputStream, {sourceAccesor});",
                "float" when originTypeName.Equals("float", StringComparison.InvariantCultureIgnoreCase) => $"FloatSchema.Write(outputStream, {sourceAccesor});",

                _ => throw new Exception()
            };

            code.AppendLine(serializerCallCode);
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
